using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageSpotHandler : MonoBehaviour
{
    public GameObject FlowerBody;
    public GameObject FlowerHead;
    public GameObject GrassObject;
    public GameObject FenceObject;
    public Transform FlowerParents;
    public FlowerPageGroundHandler ground;

    public int flowerCount = 0;
    public int Step = 0;
    public List<GameObject> FlowerParts;
    public Color[] StepColor = new Color[8];
    public Color[] StepFenceColor = new Color[8];

    private float[] Body_Offset_X_1 = { 0f };
    private float[] Body_Offset_Y_1 = { 0f };
    private float[] Body_Offset_X_2 = { -50f, 50f };
    private float[] Body_Offset_Y_2 = { 0f, 0f };
    private float[] Body_Offset_X_3 = { -90f, 0f, 90f };
    private float[] Body_Offset_Y_3 = { 0f, 0f, 0f };

    private float[] Head_Offset_X_1 = { 0f };
    private float[] Head_Offset_Y_1 = { 105.3f };
    private float[] Head_Offset_X_2 = { -53f, 47f };
    private float[] Head_Offset_Y_2 = { 105.3f, 105.3f };
    private float[] Head_Offset_X_3 = { -95f, -4f, 86f, -48f, 48, -138f, 138f, -95f, -4f, 86f, };
    private float[] Head_Offset_Y_3 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f, 247f, 247f };

    public void Awake() {
        FlowerParts = new List<GameObject>();
    }

    public void OnEnable() {
        if (FlowerParts == null)
            FlowerParts = new List<GameObject>();
    }

    public void OnDisable() {
        try {
            foreach (GameObject go in FlowerParts)
                Destroy(go);
            FlowerParts.Clear();
        } catch (System.Exception e) { e.ToString(); }
    }

    public void InitSpot(DataHandler.GardenLog logData) {
        ground.ChangeStep(Step);
        int new_flowerCount = ( logData != null ) ? logData.flower : 0;
        flowerCount = new_flowerCount;
        try {
            foreach (GameObject go in FlowerParts)
                Destroy(go);
            FlowerParts.Clear();
        } catch (System.Exception e) { e.ToString(); }

        if (flowerCount != 0) {
            if (flowerCount == 1) {
                for (int i = 0; i < 1; i++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_1[i], Body_Offset_Y_1[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_1[i], Head_Offset_Y_1[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            } else if (flowerCount == 2) {
                for (int i = 0; i < 2; i++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_2[i], Body_Offset_Y_2[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_2[i], Head_Offset_Y_2[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            } else if (flowerCount >= 3) {
                for (int i = 0; i < 3; i++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_3[i], Body_Offset_Y_3[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_3[i], Head_Offset_Y_3[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_3[i], Head_Offset_Y_3[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            }
        }

        StartCoroutine(SpawnGrass(logData));
    }

    IEnumerator SpawnGrass(DataHandler.GardenLog logData) {
        yield return new WaitForSeconds(0.7f);
        if (logData.item_0 <= 0 && logData.log_pee > 0) {
            FlowerPageHandler.Instance.SpawnEffect();
            logData.item_0 = 1;
            StartCoroutine(DataHandler.UpdateGardenLogs(logData));
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA5);
            yield return new WaitForSeconds(0.2f);
            GrassObject.SetActive(true);
            yield return new WaitForSeconds(1f);
        } else {
            GrassObject.SetActive(logData.item_0 > 0 || logData.log_pee > 0);
        }

        CheckFence(logData);
    }

    public void CheckFence(DataHandler.GardenLog logData) {
        if (logData.item_1 <= 0 && logData.log_poop > 0) {
            FlowerPageHandler.Instance.SpawnEffect();
            StartCoroutine(SpawnFence());
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA4);
            logData.item_1 = 1;
            StartCoroutine(DataHandler.UpdateGardenLogs(logData));
        } else {
            FenceObject.SetActive(logData.item_1 > 0 || logData.log_poop > 0);
            FenceObject.GetComponent<FlowerPageFenceHandler>().ChangeColor(Step);
        }
    }

    IEnumerator SpawnFence() {
        yield return new WaitForSeconds(0.2f);
        FenceObject.SetActive(true);
        FenceObject.GetComponent<FlowerPageFenceHandler>().ChangeColor(Step);
        yield return new WaitForSeconds(1f);
    }
}
