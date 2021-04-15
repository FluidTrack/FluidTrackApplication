using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingWindowHandler : MonoBehaviour
{
    public static SettingWindowHandler Instance;
    public Scrollbar musicScroll;
    public Scrollbar sfxScroll;
    public CheckBoxButton musicEnable;
    public CheckBoxButton sfxEnable;
    public Text MoabandStatusText;

    public void Awake() {
        Instance = this;
    }

    private void OnEnable() {
        TouchAndMouseManager.Instance.isTouchEnable = false;
        musicScroll.value = SoundHandler.Instance.MusicSource.volume;
        sfxScroll.value = SoundHandler.Instance.SFXSource.volume;
        musicEnable.ChangeStatus(SoundHandler.Instance.MusicSource.enabled);
        sfxEnable.ChangeStatus(SoundHandler.Instance.SFXSource.enabled);
        MoabandStatusText.text = MoabandStatusHandler.Instance.statusText.text;
    }

    public void ChangeMusicVolume() {
        SoundHandler.Instance.MusicSource.volume = musicScroll.value;
        SoundHandler.Instance.MusicSource2.volume = musicScroll.value;
    }

    public void ChangeSFXVolume() {
        SoundHandler.Instance.ChangeSFXVolume(sfxScroll.value);
    }

    public void ToggleMusicEnable() {
        SoundHandler.Instance.MusicSource.enabled = !SoundHandler.Instance.MusicSource.enabled;
        SoundHandler.Instance.MusicSource2.enabled = !SoundHandler.Instance.MusicSource2.enabled;
        if (SoundHandler.Instance.MusicSource.enabled) {
            SoundHandler.Instance.PlayBackgroundMusic();
        }
    }

    public void ToggleSFXEnable() {
        SoundHandler.Instance.SFXSource.enabled = !SoundHandler.Instance.SFXSource.enabled;
        SoundHandler.Instance.SFXSource2.enabled = !SoundHandler.Instance.SFXSource2.enabled;
    }

    private void OnDisable() {
        TouchAndMouseManager.Instance.isTouchEnable = true;
        try {
            FileStream fs = new FileStream(DataHandler.dataPath + "/settingData.data",
                                               FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string MusicVolume = SoundHandler.Instance.MusicSource.volume.ToString() + "\n";
            string SFXVolume = SoundHandler.Instance.SFXSource.volume.ToString() + "\n";
            string MusicEnable = ( SoundHandler.Instance.MusicSource.enabled ) ? "1\n" : "0\n";
            string SFXEnable = ( SoundHandler.Instance.SFXSource.enabled ) ? "1" : "0";
            sw.WriteLine(MusicVolume + SFXVolume + MusicEnable + SFXEnable);
            sw.Close(); fs.Close();
        } catch(System.Exception e) { e.ToString(); }
    }
}
