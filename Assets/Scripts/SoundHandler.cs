using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioClip Music_IntroMusic;
    public AudioClip Music_IntroMusicOnce;
    public AudioClip SFX_Clicked;

    public AudioSource SFXSource;
    public AudioSource MusicSource;

    public static SoundHandler Instance;

    public void Start() {
        Instance = this;
    }

    public enum SFX {
        CLICKED,
    }

    public enum MUSIC {
        INTRO_ONCE,
        INTRO,
    }

    public void Play_SFX(SFX type) {
        switch(type) {
            case SFX.CLICKED: SFXSource.PlayOneShot(SFX_Clicked); break;
        }
    }

    public void Play_SFX(int typeInt) {
        try {
            SFX type = (SFX)typeInt;
            switch (type) {
                case SFX.CLICKED: SFXSource.PlayOneShot(SFX_Clicked); break;
            }
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
            switch (type) {
                case MUSIC.INTRO_ONCE: MusicSource.PlayOneShot(Music_IntroMusicOnce); break;
                case MUSIC.INTRO: MusicSource.clip = Music_IntroMusic; break;
            }

            if (type != MUSIC.INTRO_ONCE) {
                MusicSource.Play();
                MusicSource.loop = true;
            } else {
                MusicSource.loop = false;
            }
        } catch (System.Exception e) { Debug.Log(e.ToString()); }
    }
}
