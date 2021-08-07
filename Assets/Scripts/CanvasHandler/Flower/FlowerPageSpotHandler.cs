using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageSpotHandler : MonoBehaviour
{
    public Animator ScaleAnim;
    public Animator NumberAnim;

    public int flowerCount = 0;
    public int Step = 0;
    public List<FlowerPageSpriteHandler> FlowerParts;
    public FlowerPageGroundHandler ground;

    private DataHandler.GardenLog currentLog;
    private bool isSparkling = false;

    public void OnDisable() {
        foreach (FlowerPageSpriteHandler handler in FlowerParts)
            handler.Clear();
        ScaleAnim.SetTrigger("Reset");
        isSparkling = false;
    }

    public void InitSpot(DataHandler.GardenLog logData) {
        currentLog = logData;
        ground.ChangeStep(Step);

        foreach (FlowerPageSpriteHandler handler in FlowerParts)
            handler.ChangeSprite(Step);

        flowerCount = ( logData != null ) ? logData.flower : 0;
        flowerCount = ( flowerCount > 10 ) ? 10 : flowerCount;
        if (logData.item_0 > 0) {
            for(int i = 0; i < flowerCount; i++)
                FlowerParts[i].SetStep_Up(Step);
            isSparkling = true;
        } else {
            for (int i = 0; i < flowerCount; i++)
                FlowerParts[i].SetStep(Step);
            isSparkling = false;
        }
        ScaleAnim.SetBool("isGrowing", logData.item_1 > 0);

        NumberAnim.SetTrigger("Set" + flowerCount);
        NumberAnim.SetInteger("flowers", flowerCount);
    }

    public void Watering() {
        NumberAnim.speed = 1f;
        flowerCount++;
        if(isSparkling) {
            foreach (FlowerPageSpriteHandler handler in FlowerParts)
                handler.ChangeSprite_Up(Step);
            FlowerParts[flowerCount - 1].Particle.SetActive(true);
        }
        NumberAnim.SetInteger("flowers", flowerCount);
    }

    public void Watering2(float speed) {
        NumberAnim.speed = speed;
        flowerCount++;
        if (isSparkling) {
            foreach (FlowerPageSpriteHandler handler in FlowerParts)
                handler.ChangeSprite_Up(Step);
            FlowerParts[flowerCount - 1].Particle.SetActive(true);
        }
        NumberAnim.SetInteger("flowers", flowerCount);
    }

    public void DragPee() {
        for (int i = 0; i < flowerCount; i++)
            FlowerParts[i].SetStep_Up(Step);
        foreach (FlowerPageSpriteHandler handler in FlowerParts)
            handler.ChangeSprite_Up(Step);
    }

    public void DragPoo() {
        ScaleAnim.SetTrigger("Growing");
    }
}
