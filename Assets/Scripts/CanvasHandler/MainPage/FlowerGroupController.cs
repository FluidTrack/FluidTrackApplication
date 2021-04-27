using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerGroupController : MonoBehaviour
{
    public Image[] Flowers;
    public Text[] FlowerTexts;
    public BigCloudController BCC;

    private List<int> flowerCounts;
    private List<int> flowerRealCounts;
    private int num = 0;
    private int maxCount = 0;


    public void OnDisable() {
        for(int i = 0; i < Flowers.Length; i++) {
            Flowers[i].sprite = FlowerSpritesGroup.FlowerSprite[i].Sprites[0];
            FlowerTexts[i].text = "0";
        }
    }

    public void Awake() {
        for (int i = 0; i < Flowers.Length; i++)
            Flowers[i].sprite = FlowerSpritesGroup.FlowerSprite[i].Sprites[0];
    }

    public void ChangeSprite(int range) {
        if (GardenSpotHandler.weeklyData == null) return;

        int pass = BCC.pass;
        for(int i = 0; i <= pass && i < Flowers.Length; i++) 
            Flowers[i].gameObject.SetActive(true);
        for (int i = pass+1; i < Flowers.Length; i++)
            Flowers[i].gameObject.SetActive(false);

        num = range;
        flowerRealCounts = new List<int>();
        for (int i = 0; i < num; i++) {
            maxCount = ( maxCount < GardenSpotHandler.weeklyData[i] ) ? GardenSpotHandler.weeklyData[i] : maxCount;
            flowerRealCounts.Add(0);
        }
        flowerCounts = GardenSpotHandler.weeklyData;
        StartCoroutine(IncreasementFlowers());
    }

    public IEnumerator IncreasementFlowers() {
        for(int i = 0; i <= maxCount+1; i++) {
            yield return new WaitForSeconds(0.05f);
            for(int j = 0; j < num; j ++) {
                if (flowerRealCounts[j] <= flowerCounts[j]+1) {
                    Flowers[j].sprite = FlowerSpritesGroup.FlowerSprite[j].Sprites[flowerRealCounts[j]];
                    FlowerTexts[j].text = ((flowerRealCounts[j]-1) <= 0) ? "0" : ( ( flowerRealCounts[j] - 1 ).ToString());
                    flowerRealCounts[j]++;
                }
            }
        }
    }
}
