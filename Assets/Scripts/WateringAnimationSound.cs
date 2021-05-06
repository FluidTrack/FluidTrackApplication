using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringAnimationSound : MonoBehaviour
{
    public void PlaySFX() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.WATER);
    }
}
