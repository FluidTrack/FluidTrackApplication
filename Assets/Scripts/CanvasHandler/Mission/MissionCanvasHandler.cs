using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCanvasHandler : MonoBehaviour
{
    static public MissionCanvasHandler Instance;
    public GameObject HoleParents;
    public GameObject LinkParents;
    public GameObject Line_4_Weeks;
    public GameObject Line_6_Weeks;
    public GameObject Line_8_Weeks;
    public Animator PannelAnimator;
    public Text DebugText;
    public Text DebugTextLabel;
    public Text TodayText;
    public Text LinkText;
    public Text TotalText;
    public RectTransform TodayMark;
    public RectTransform TodayMarkText;
    public Sprite Fail;
    public Sprite Success;
    public Sprite Future;
    public int Goal = 10;
    public int TotalDateCount = 28;
    public Color ActiveColor;
    public Color InactiveColor;

    private List<Image> hole;
    public List<GameObject> link;
    public List<Image> Line;
    private Dictionary<string,int> dateList;
    private int linkCount = 0;
    private int dateCount = 0;
    private bool isOverflowed = false;
    private int currentIndex = 0;

    public void Awake() {
        Instance = this;
        hole = new List<Image>();
        link = new List<GameObject>();
        dateList = new Dictionary<string, int>();

        int length = HoleParents.transform.childCount;
        for (int i = 0; i < length; i++) {
            hole.Add(HoleParents.transform.GetChild(i).GetComponent<Image>());
            HoleParents.transform.GetChild(i).gameObject.SetActive(false);
        }
        length = LinkParents.transform.childCount;
        for (int i = 0; i < length; i++)
            link.Add(LinkParents.transform.GetChild(i).gameObject);

        Line = new List<Image>();
        Image[] images = Line_4_Weeks.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
            Line.Add(images[i]);
        images = Line_6_Weeks.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
            Line.Add(images[i]);
        images = Line_8_Weeks.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
            Line.Add(images[i]);

        Line_4_Weeks.SetActive(false);
        Line_6_Weeks.SetActive(false);
        Line_8_Weeks.SetActive(false);
    }

    public void Start() {
        DebugText.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugTextLabel.gameObject.SetActive(TotalManager.instance.isDebugMode);
        PannelAnimator.keepAnimatorControllerStateOnDisable = true;
    }

    public void OnEnable() {
        StartCoroutine(FetchCheck_User());
    }

    public void OnDisable() {
        for (int i = 0; i <= currentIndex; i++) {
            hole[i].sprite = Fail;
            Line[i].color =  ActiveColor;
        }
        for (int i = currentIndex+1; i < TotalDateCount; i++) {
            hole[ i ].sprite = Future;
            if(i < TotalDateCount -1)
                Line[i-1].color  = InactiveColor;
        }

        for (int i = 0; i < link.Count; i++) {
            link[i].SetActive(false);
        }

        TodayMark.GetComponent<Animator>().SetTrigger("Reset");
        PannelAnimator.GetComponent<Animator>().SetInteger("Weeks",DataHandler.User_periode);
        TodayMark.gameObject.SetActive(false);
        TodayMarkText.gameObject.SetActive(false);
        TodayText.text = "0";
        LinkText.text = "0 일";
        TotalText.text = "0 일";
    }

    IEnumerator FetchCheck_User() {
        yield return new WaitForSeconds(0.3f);
        while (!DataHandler.User_isDataLoaded) {
            yield return 0;
        }
        TimeHandler.GetCurrentTime();
        string currentTimeStamp = TimeHandler.TableCanvasTime.ToDateString();
        TimeHandler.DateTimeStamp stamp = new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
        int length = TotalDateCount;
        isOverflowed = true;
        dateCount = 0;
        dateList.Clear();

        TotalDateCount = 7 * DataHandler.User_periode;
        PannelAnimator.SetInteger("Weeks", DataHandler.User_periode);
        Line_4_Weeks.SetActive(true);
        Line_6_Weeks.SetActive(DataHandler.User_periode >= 6);
        Line_8_Weeks.SetActive(DataHandler.User_periode >= 8);
        DebugText.text = DataHandler.User_periode + " Weeks data";

        for (int i = 0; i < hole.Count; i++) {
            if (i < TotalDateCount) hole[i].gameObject.SetActive(true);
            else hole[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < length; i++,stamp += 1) {
            if(!isOverflowed) {
                hole[i].sprite = Future;
                if(i < length -1)
                    Line[i].color = InactiveColor;
            } else {
                hole[i].sprite = Fail;
                Line[i].color = ActiveColor;
                string tempStr = stamp.ToDateString();
                dateList.Add(tempStr,i);
                if(isOverflowed) dateCount++;
                if (tempStr == currentTimeStamp) {
                    TodayMark.anchoredPosition = hole[i].GetComponent<RectTransform>().anchoredPosition;
                    TodayMarkText.anchoredPosition = hole[i].GetComponent<RectTransform>().anchoredPosition;
                    isOverflowed = false;
                    currentIndex = i;
                    Line[i].color = InactiveColor;
                }
            }
        }

        if(isOverflowed) {
            TimeHandler.DateTimeStamp tempDate =
                new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
            dateCount = 0;
            while (true) {
                dateCount++;
                tempDate = tempDate + 1;
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp
                    (tempDate, TimeHandler.TableCanvasTime) == 0) {
                    break;
                }
                if (dateCount >= 100) break;
            }

        }
        DataHandler.User_isGardenDataLoaded = false;
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        StartCoroutine(FetchCheck_Garden());
    }


    IEnumerator FetchCheck_Garden() {
        yield return new WaitForSeconds(0.3f);
        while (!DataHandler.User_isGardenDataLoaded) {
            yield return 0;
        }
        DataHandler.User_isGardenDataLoaded = false;

        List<int> flowers = new List<int>();
        for(int i = 0; i < TotalDateCount; i++)
            flowers.Add(0);

        foreach(DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
            try {
                int index = dateList[log.timestamp.Split(' ')[0]];
                flowers[index] = log.flower;
            } catch(System.Exception e) { e.ToString(); }
        }

        linkCount = 0;
        TotalText.text = linkCount + " 일";
        int max = -1, count = 0;
        for (int i = 0; i < TotalDateCount; i++) {
            //Debug.Log(i + " : " + flowers[i]);
            if (flowers[i] < Goal) { 
                max = ( max < count ) ? count : max;
                count = 0;
                continue;
            }
            count++;
            if(count > max)
                LinkText.text = count + " 일";
            linkCount++;
            TotalText.text = linkCount + " 일";
            hole[i].sprite = Success;
            hole[i].GetComponent<Animator>().SetTrigger("Pop");
            if (i > 0 && flowers[i-1] >= Goal) link[i - 1].SetActive(true);
            SoundHandler.Instance.Play_SFX(1);
            yield return new WaitForSeconds(0.13f);
        }
        yield return new WaitForSeconds(0.2f);
        LinkText.text = max + " 일";
        for (int i = 0; i < dateCount; i++) {
            TodayText.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }

        if (!isOverflowed) {
            SoundHandler.Instance.Play_SFX(1);
            TodayMark.gameObject.SetActive(true);
            TodayMarkText.gameObject.SetActive(true);
        }
        TodayText.text = dateCount.ToString();
    }
}
