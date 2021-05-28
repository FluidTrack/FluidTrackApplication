using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SoundHandler : MonoBehaviour
{
    public AudioClip Music_IntroMusic;
    public AudioClip Music_IntroMusicOnce;
    public AudioClip Music_Chiptune_01;
    public AudioClip Music_Achieve;
    public AudioClip Music_Prize;
    public AudioClip SFX_Clicked;
    public AudioClip SFX_Clicked2;
    public AudioClip SFX_Clicked3;
    public AudioClip SFX_Clicked4;
    public AudioClip SFX_Poped;
    public AudioClip SFX_Poped2;
    public AudioClip SFX_Congratulation1;
    public AudioClip SFX_Congratulation2;
    public AudioClip SFX_Tada1;
    public AudioClip SFX_Tada2;
    public AudioClip SFX_Tada3;
    public AudioClip SFX_Tada4;
    public AudioClip SFX_Tada5;
    public AudioClip SFX_Yeah;
    public AudioClip SFX_Scan;
    public AudioClip SFX_Connect;
    public AudioClip SFX_Disconnect;
    public AudioClip SFX_Coin;
    public AudioClip SFX_Back;
    public AudioClip SFX_SmallPing;
    public AudioClip SFX_Error;
    public AudioClip SFX_SmallestView;
    public AudioClip SFX_Water;
    public AudioClip SFX_MongMongJump;
    public AudioClip SFX_MongMongHello;
    public AudioClip SFX_MongMongBeep;
    public AudioClip SFX_Data;
    public AudioClip SFX_Data2;

    public AudioSource SFXSource;
    public AudioSource SFXSource2;
    public AudioSource MusicSource;
    public AudioSource MusicSource2;
    public AudioSource MongMongSource;
    public AudioSource MongMongSource2;

    public static SoundHandler Instance;
    public bool Music_Enable = false;
    public bool SFX_Enable = false;
    public bool isMusicPlaying = false;

    public void Awake() {
        Instance = this;
    }

    public enum SFX {
        CLICKED, POPED, POPED2, CONGRATULATION1, CONGRATULATION2,
        TADA1, TADA2, TADA3, YEAH, SCAN,
        CONNECT, DISCONNECT, COIN, BACK, SMALL_PING,
        ERROR, CLICKED2, CLICKED3, CLICKED4, SMALLEST_VEIW,WATER,
        TADA4, TADA5,MONG_JUMP, MONG_HELLO, MONG_BEEP,DATA,DATA2
    }

    public enum MUSIC {
        INTRO_ONCE,
        INTRO,
        CHIPTUNE_01,
        ACHIEVE,
        PRIZE_01
    }

    public void Play_SFX(SFX type) {
        if (!SFX_Enable) return;

        switch(type) {
            case SFX.CLICKED: 
                SFXSource.PlayOneShot(SFX_Clicked); break;
            case SFX.CLICKED2:
                SFXSource.PlayOneShot(SFX_Clicked2); break;
            case SFX.CLICKED3:
                SFXSource.PlayOneShot(SFX_Clicked3); break;
            case SFX.CLICKED4:
                SFXSource.PlayOneShot(SFX_Clicked4); break;
            case SFX.POPED:
                SFXSource2.PlayOneShot(SFX_Poped);
                break;
            case SFX.POPED2:
                SFXSource2.PlayOneShot(SFX_Poped2);
                break;
            case SFX.CONGRATULATION1:
                SFXSource2.PlayOneShot(SFX_Congratulation1);
                break;
            case SFX.CONGRATULATION2:
                SFXSource2.PlayOneShot(SFX_Congratulation2);
                break;
            case SFX.TADA1:
                SFXSource2.PlayOneShot(SFX_Tada1);
                break;
            case SFX.TADA2:
                SFXSource2.PlayOneShot(SFX_Tada2);
                break;
            case SFX.TADA3:
                SFXSource2.PlayOneShot(SFX_Tada3);
                break;
            case SFX.TADA4:
                SFXSource2.PlayOneShot(SFX_Tada4);
                break;
            case SFX.TADA5:
                SFXSource2.PlayOneShot(SFX_Tada5);
                break;
            case SFX.YEAH:
                SFXSource2.PlayOneShot(SFX_Yeah);
                break;
            case SFX.SCAN:
                SFXSource2.PlayOneShot(SFX_Scan);
                break;
            case SFX.CONNECT:
                SFXSource2.PlayOneShot(SFX_Connect);
                break;
            case SFX.DISCONNECT:
                SFXSource2.PlayOneShot(SFX_Disconnect);
                break;
            case SFX.COIN:
                SFXSource2.PlayOneShot(SFX_Coin);
                break;
            case SFX.BACK:
                SFXSource2.PlayOneShot(SFX_Back);
                break;
            case SFX.SMALL_PING:
                SFXSource.PlayOneShot(SFX_SmallPing);
                break;
            case SFX.ERROR:
                SFXSource.PlayOneShot(SFX_Error);
                break;
            case SFX.SMALLEST_VEIW:
                SFXSource.PlayOneShot(SFX_SmallestView);
                break;
            case SFX.WATER:
                SFXSource.PlayOneShot(SFX_Water);
                break;
            case SFX.MONG_JUMP:
                SFXSource2.PlayOneShot(SFX_MongMongJump);
            break;
            case SFX.MONG_HELLO:
                SFXSource2.PlayOneShot(SFX_MongMongHello);
            break;
            case SFX.MONG_BEEP:
                SFXSource2.PlayOneShot(SFX_MongMongBeep);
            break;
            case SFX.DATA:
                SFXSource2.PlayOneShot(SFX_Data);
            break;
            case SFX.DATA2:
                SFXSource2.PlayOneShot(SFX_Data2);
            break;
        }
    }

    public void ChangeMusicVolume(float volume) {
        MusicSource.volume = volume;
        MusicSource2.volume = volume;
    }

    public void ChangeSFXVolume(float volume) {
        SFXSource.volume = volume;
        SFXSource2.volume = volume;
    }

    public void Play_SFX(int typeInt) {
        try {
            SFX type = (SFX)typeInt;
            Play_SFX(type);
        } catch(System.Exception e) { Debug.Log(e.ToString()); }
    }

    public void Play_Music(MUSIC type) {
        if (!Music_Enable) return;

        switch (type) {
            case MUSIC.INTRO_ONCE: MusicSource.PlayOneShot(Music_IntroMusicOnce); break;
            case MUSIC.INTRO: MusicSource.clip = Music_IntroMusic; break;
            case MUSIC.CHIPTUNE_01: MusicSource.clip = Music_Chiptune_01; break;
            case MUSIC.ACHIEVE: MusicSource2.PlayOneShot(Music_Achieve); break;
            case MUSIC.PRIZE_01: MusicSource2.PlayOneShot(Music_Achieve); break;
        }

        if(type != MUSIC.INTRO_ONCE && type != MUSIC.ACHIEVE && type != MUSIC.PRIZE_01) {
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

    public void Update() {
        isMusicPlaying = MusicSource.isPlaying;
    }

    public void Start() {
        try {
            FileStream fs =
                new FileStream(DataHandler.dataPath + "/settingData.data",
                               FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            float MusicVolume = float.Parse(sr.ReadLine());
            float SFXVolume = float.Parse(sr.ReadLine());
            bool MusicEnable = (int.Parse(sr.ReadLine()) == 1);
            bool SFXEnable = (int.Parse(sr.ReadLine()) == 1);
            ChangeMusicVolume(MusicVolume);
            ChangeSFXVolume(SFXVolume);
            MusicSource.enabled = MusicEnable;
            MusicSource2.enabled = MusicEnable;
            SFXSource.enabled = SFXEnable;
            SFXSource2.enabled = SFXEnable;
            sr.Close(); fs.Close();
        } catch (System.Exception e) {
            FileStream fs = new FileStream(DataHandler.dataPath + "/settingData.data",
                                           FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string MusicVolume = MusicSource.volume.ToString() + "\n";
            string SFXVolume = SFXSource.volume.ToString() + "\n";
            string MusicEnable = ( MusicSource.enabled ) ? "1\n" : "0\n" ;
            string SFXEnable = ( SFXSource.enabled ) ? "1" : "0";
            sw.WriteLine(MusicVolume + SFXVolume + MusicEnable + SFXEnable);
            sw.Close(); fs.Close();
        }
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic() {
        SoundHandler.Instance.Play_Music(SoundHandler.MUSIC.CHIPTUNE_01);
    }

    public void StopMongMong() {
        MongMongSource.Stop();
        MongMongSource2.Stop();
    }
}
