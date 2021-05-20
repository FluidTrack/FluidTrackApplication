using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageSpriteHandler : MonoBehaviour
{
    public Sprite[] Sprites;
    public Sprite[] Sprites_Up;
    public GameObject Particle;

    public Image Image;
    public RectTransform Rect;
    
    private float[] offset_x = { 0f,-5f,-7f,-5f,-5f,-5f,-8f,-6f };
    private float[] offset_y = { 0f,0f,0f,0f,0f,0f,0f,0f };
    private float[] size_x = { 69f,82f,99f,76f,73f,80f,88f,75f, };
    private float[] size_y = { 65f,71f,62f,77f,75f,82f,91f,70f };

    public void SetStep(int step) {
        Image.sprite = Sprites[step];
        Particle.SetActive(false);
        Rect.anchoredPosition = new Vector2(offset_x[step], offset_y[step]);
        Rect.sizeDelta = new Vector2(size_x[step], size_y[step]);
    }

    public void SetStep_Up(int step) {
        Image.sprite = Sprites_Up[step];
        Particle.SetActive(true);
        Rect.anchoredPosition = new Vector2(offset_x[step], offset_y[step]);
        Rect.sizeDelta = new Vector2(size_x[step], size_y[step]);
    }

    public void ChangeSprite(int step) {
        Rect.anchoredPosition = new Vector2(offset_x[step], offset_y[step]);
        Rect.sizeDelta = new Vector2(size_x[step], size_y[step]);
        Image.sprite = Sprites[step];
    }

    public void ChangeSprite_Up(int step) {
        Rect.anchoredPosition = new Vector2(offset_x[step], offset_y[step]);
        Rect.sizeDelta = new Vector2(size_x[step], size_y[step]);
        Image.sprite = Sprites_Up[step];
    }

    public void Clear() {   
        Image.sprite = Sprites[0];
        Particle.SetActive(false);
    }
}
