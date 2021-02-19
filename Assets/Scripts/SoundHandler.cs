using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioClip Music_IntroMusic;
    public AudioClip Music_IntroMusicOnce;
    public AudioClip SFX_Clicked;
    public AudioClip SFX_Poped;
    public AudioClip SFX_Poped2;

    public AudioSource SFXSource;
    public AudioSource SFXSource2;
    public AudioSource MusicSource;

    public static SoundHandler Instance;

    public void Start() {
        Instance = this;
    }

    public enum SFX {
        CLICKED, POPED, POPED2
    }

    public enum MUSIC {
        INTRO_ONCE,
        INTRO,
    }

    public void Play_SFX(SFX type) {
        switch(type) {
            case SFX.CLICKED: 
                SFXSource.PlayOneShot(SFX_Clicked); break;
            case SFX.POPED:
                SFXSource2.PlayOneShot(SFX_Poped);
                break;
            case SFX.POPED2:
                SFXSource2.PlayOneShot(SFX_Poped2);
                break;
        }
    }

    public void Play_SFX(int typeInt) {
        try {
            SFX type = (SFX)typeInt;
            Play_SFX(type);
        } catch(System.Exception e) { Debug.Log(e.ToString()); }
    }

    public void Play_Music(MUSIC type) {
        switch(type) {
            case MUSIC.INTRO_ONCE: MusicSource.PlayOneShot(Music_IntroMusicOnce); break;
            case MUSIC.INTRO: MusicSource.clip = Music_IntroMusic; break;
        }

        if(type != MUSIC.INTRO_ONCE) {
            MusicSource.Play();
            MusicSource.loop = true;
        } else {
            MusicSource.loop = false;
        }
    }

    public void Play_Music(int typeInt) {
        try {
            MUSIC type = (MUSIC)typeInt;
            Play_Music(type);
        } catch (System.Exception e) { Debug.Log(e.ToString()); }
    }
}
