using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDrinkHandler : MonoBehaviour
{
    public static SelectDrinkHandler Instance;
    public bool isDelete = true;
    public Button LeftButton;
    public Button RightButton;
    public Button OkayButton;
    public RectTransform Pannel;
    public List<GameObject> spwan;
    public Text titleText;
    public GameObject Prefabs;
    public GameObject ModifyObject;

    public List<int> auto;
    public List<int> noneauto;
    private List<DataHandler.DrinkLog> drinkLogs;
    private int page = 0;
    private bool isClicked = false;
    private List<int> realClickedIconIndex = new List<int>();

    public void OnEnable() {
        Instance = this;
        spwan = new List<GameObject>();
        drinkLogs = new List<DataHandler.DrinkLog>();

        for(int i = 0; i < noneauto.Count; i++)
            foreach(DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs)
                if(log.log_id == noneauto[i]) {
                    drinkLogs.Add(log);
                    break;
                }
     
        for (int i = 0; i < auto.Count; i++)
            foreach (DataHandler.DrinkLog log in DataHandler.Drink_logs.DrinkLogs)
                if (log.log_id == auto[i]) {
                    drinkLogs.Add(log);
                    break;
                }
        page = 0;
        LeftButton.interactable = false;
        RightButton.interactable =  (drinkLogs.Count > 4) ;

        DrawIcons();
    }

    public void DrawIcons() {
        if(spwan != null) {
            int size = spwan.Count;
            for (int i = 0; i < size; i++) {
                GameObject temp = spwan[i];
                spwan[i] = null;
                Destroy(temp);
            }
            spwan.Clear();
        }

        Pannel.sizeDelta = new Vector2(( drinkLogs.Count >= 4 ) ? 1200 : drinkLogs.Count * 300f, 400f);
        for (int i = page; i < page + 4 && i < drinkLogs.Count; i++ ) {
            GameObject newIcon = Instantiate(Prefabs, Pannel.transform);
            spwan.Add(newIcon);
            newIcon.GetComponent<RectTransform>().anchoredPosition
                = new Vector2((i-page) * 300f, 0f);
            DrinkSelectIcon icon = newIcon.GetComponent<DrinkSelectIcon>();
            icon.Image.sprite = icon.sprites[drinkLogs[i].type];
            if (isClicked) {
                if(realClickedIconIndex.Contains(i)) icon.Image.color = new Color(1, 1, 1, 1);
                else icon.Image.color = new Color(1, 1, 1, 0.3f);
            } else icon.Image.color = new Color(1, 1, 1, 1);

            icon.OtherText.SetActive(drinkLogs[i].type == 0);
            string str = drinkLogs[i].volume + "ml\n";
            TimeHandler.DateTimeStamp tempStamp = new TimeHandler.DateTimeStamp(drinkLogs[i].timestamp);
            int showTime = tempStamp.Hours;
            string str2 = "새벽";
            if (showTime >= 6 && showTime <= 11) str2 = "아침";
            else if (showTime >= 12 && showTime <= 14) str2 = "점심";
            else if (showTime >= 15 && showTime <= 17) str2 = "낮";
            else if (showTime >= 18 && showTime <= 20) str2 = "저녁";
            else if (showTime >= 21) str2 = "밤";
            if (showTime >= 12)
                showTime -= 12;
            showTime = ( showTime == 0 ) ? 12 : showTime; str += str2 + " " + showTime + "시";
            if (drinkLogs[i].auto == 1)
                str += tempStamp.Minutes + "분";
            str += "\n"+ icon.strings[drinkLogs[i].type];
            icon.Description.text = str;
            icon.index = ( i - page );
        }

    }

    public void RightButtonClicked() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        page++;
        LeftButton.interactable = true;
        if(page+4 >= drinkLogs.Count)
            RightButton.interactable = false;
        DrawIcons();
    }

    public void LeftButtonClicked() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        page--;
        RightButton.interactable = true;
        if (page <= 0)
            LeftButton.interactable = false;
        DrawIcons();
    }

    public void IconClick(int index) {
        if (realClickedIconIndex.Contains(index))
        {
            realClickedIconIndex.Remove(page + index);
        }
        else
        {
            realClickedIconIndex.Add(page + index);
        }

        if (realClickedIconIndex.Count > 0)
        {
            OkayButton.interactable = true;
        }
        else
        {
            OkayButton.interactable = false;

        }
        isClicked = true;
        DrawIcons();
    }

    public void OkayButtonClick() {
        bool contains = false;
        if(isDelete) {
            
            int index = 0;
            DataHandler.DrinkLog[] array1 = new DataHandler.DrinkLog[DataHandler.Drink_logs.DrinkLogs.Length - realClickedIconIndex.Count];
            for (int i = 0; i < DataHandler.Drink_logs.DrinkLogs.Length; i++) {
                foreach (int realIndex in realClickedIconIndex)
                {
                    if (DataHandler.Drink_logs.DrinkLogs[i].log_id == drinkLogs[realIndex].log_id)
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains)
                {
                    contains = false;
                    continue;
                }


                array1[index++] = DataHandler.Drink_logs.DrinkLogs[i];
            }

            foreach (int realIndex in realClickedIconIndex)
            {
                StartCoroutine(DataHandler.DeleteDrinkLogs(drinkLogs[realIndex].log_id));
            }
            DataHandler.Drink_logs.DrinkLogs = array1;

            StartCoroutine(WaitDelete());
            OkayButton.interactable = false;
        } else {
            foreach (int realIndex in realClickedIconIndex)
            {
                ModifyDrinkLogHandler.Target = drinkLogs[realIndex];
            }
            ModifyObject.SetActive(true);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator WaitDelete() {
        yield return 0;
        LogCanvasHandler.Instance.Fetching();
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        this.gameObject.SetActive(false);
    }

    public void CancelButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        this.gameObject.SetActive(false);
    }

    public void OnDisable() {
        int size = spwan.Count;
        for(int i = 0; i < size; i ++) {
            GameObject temp = spwan[i];
            spwan[i] = null;
            Destroy(temp);
        }
        spwan.Clear();
        OkayButton.interactable = false;
        isClicked = false;
    }
}
