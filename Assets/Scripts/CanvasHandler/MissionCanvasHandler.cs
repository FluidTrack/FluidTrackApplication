using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCanvasHandler : MonoBehaviour
{
    static public MissionCanvasHandler Instance;
    public GameObject HoleParents;
    public GameObject LinkParents;
    public Text TodayText;
    public Text LinkText;
    public Text TotalText;
    public RectTransform ProgressBar;
    public RectTransform Arrow;
    public RectTransform TodayMark;
    public Sprite Fail;
    public Sprite Success;
    public int Goal = 10;

    private List<Image> hole;
    public List<GameObject> link;
    private Dictionary<string,int> dateList;
    private int linkCount = 0;
    private int dateCount = 0;
    private int TotalDateCount = 56;
    private bool isOverflowed = false;


    public void Start() {
        Instance = this;
        hole = new List<Image>();
        link = new List<GameObject>();
        dateList = new Dictionary<string, int>();

        int length = HoleParents.transform.childCount;
        for (int i = 0; i < length; i++)
            hole.Add(HoleParents.transform.GetChild(i).GetComponent<Image>());
        length = LinkParents.transform.childCount;
        for (int i = 0; i < length; i++)
            link.Add(LinkParents.transform.GetChild(i).gameObject);
    }

    public void OnEnable() {
        //StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
        StartCoroutine(FetchCheck_User());
    }

    public void OnDisable() {
        for (int i = 0; i < TotalDateCount; i++)
            hole[i].sprite = Fail;
        for (int i = 0; i < link.Count; i++)
            link[i].SetActive(false);
        TodayMark.GetComponent<Animator>().SetTrigger("Reset");
        TodayMark.gameObject.SetActive(false);
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
        int length = (int)hole.Count;
        isOverflowed = true;
        dateCount = 0;
        dateList.Clear();
        for (int i = 0; i < length; i++,stamp += 1) {
            string tempStr = stamp.ToDateString();
            dateList.Add(tempStr,i);
            if(isOverflowed) dateCount++;
            if (tempStr == currentTimeStamp) {
                TodayMark.anchoredPosition = hole[i].GetComponent<RectTransform>().anchoredPosition;
                isOverflowed = false;
            }
        }

        ProgressBar.sizeDelta = new Vector2(((float)(dateCount)/TotalDateCount) * 235f,30f);
        Arrow.anchoredPosition = new Vector2(10f + ( (float)( dateCount ) / TotalDateCount ) * 235f, -40f);

        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        StartCoroutine(FetchCheck_Garden());
    }


    IEnumerator FetchCheck_Garden() {
        yield return 0;
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
        }
        TodayText.text = dateCount.ToString();
    }
}
