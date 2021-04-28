using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSFX : MonoBehaviour
{
    private SoundHandler sound;
    public void Start() {
        sound = SoundHandler.Instance;
    }

    public void PlayJumpSound() {
        sound.Play_SFX(SoundHandler.SFX.MONG_JUMP);
    }

    public void PlayActSound() {
        sound.Play_SFX(SoundHandler.SFX.MONG_HELLO);
    }

    public void PlayBeepSound() {
        sound.Play_SFX(SoundHandler.SFX.MONG_BEEP);
    }
}
