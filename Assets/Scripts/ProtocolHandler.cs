using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class ProtocolHandler : MonoBehaviour
{
    public BluetoothManager BT;
    public TotalManager Main;
    public Text DebugText;

    public void ParsingBytes(byte[] bytes) {
        int length = bytes.Length;
        byte cmd = bytes[0];

        //DebugText.text += "1 ▶";
        //for(int i = 0; i < length; i++) {
        //    DebugText.text += bytes[i].ToString();
        //    if (i != length - 1) DebugText.text += "-";
        //}
        //DebugText.text += "\n";

        switch (cmd) {
            //==============================================================================
            // 타임존 변경 Confirm
            //==============================================================================
            case 0x03:
            AlertHandler.GetInstance().Pop_Msg("모아밴드의 시간이 성공적으로 변경됨.");
            break;

            //==============================================================================
            // 배터리 확인
            //==============================================================================
            case 0x07:
                if (bytes[3] == 0)
                     AlertHandler.GetInstance().Pop_BatInfo((int)bytes[2]);
                else AlertHandler.GetInstance().Pop_ChargeBat((int)bytes[2]);
            break;

            //==============================================================================
            // 배터리 부족 Alert
            //==============================================================================
            case 0x12:
                if (bytes[3] == 0)
                    AlertHandler.GetInstance().Pop_LowBat((int)bytes[2]);
                break;

            //==============================================================================
            // Button Input
            //==============================================================================
            case 0x24:
                //string stamp = GetCurrentTimeStamp();
                //switch(bytes[6]) {
                //    case 3: Main.AddRealtimeLog(MainPanelHandler.LOG_TYPE.POO,stamp); break;
                //    case 1: Main.AddRealtimeLog(MainPanelHandler.LOG_TYPE.WATER,stamp); break;
                //    case 2: Main.AddRealtimeLog(MainPanelHandler.LOG_TYPE.PEE,stamp); break;
                //}
                break;

            //==============================================================================
            // History
            //==============================================================================
            case 0x27:
                //if(bytes[1] == 0) {
                //    MainPanelHandler.GetInstance().BlindControl(false);
                //} else {
                //    try {
                //        int Length = ( bytes[1] ) / 5;
                //        for (int i = 0; i < length; i++) {
                //            string historyStamp = MakeTimeStamp(bytes[2 + 5 * i], bytes[3 + 5 * i], bytes[4 + 5 * i], bytes[5 + 5 * i]);
                //            switch (bytes[6 + 5 * i]) {
                //                case 3: Main.AddHistoryLog(MainPanelHandler.LOG_TYPE.POO, historyStamp); break;
                //                case 1: Main.AddHistoryLog(MainPanelHandler.LOG_TYPE.WATER, historyStamp); break;
                //                case 2: Main.AddHistoryLog(MainPanelHandler.LOG_TYPE.PEE, historyStamp); break;
                //            }
                //        }
                //    } catch (System.Exception e) {
                //        e.ToString();
                //    }
                //}
                break;
            default: break;
        }
    }

    static public byte[] SetTimerToCurrent() {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[10];
        bytes[0] = 0x03;
        bytes[1] = 8;
        bytes[2] = (byte)now.Year;
        bytes[3] = (byte)(now.Month);
        bytes[4] = (byte)now.Day;
        bytes[5] = (byte)now.Hour;
        bytes[6] = (byte)now.Minute;
        bytes[7] = (byte)now.Second;
        bytes[8] = 0;
        bytes[9] = 0;
        return bytes;
    }

    static public byte[] SetTimer(int year, int month, int day, int hour, int min, int sec) {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[10];
        bytes[0] = 0x03;
        bytes[1] = 8;
        bytes[2] = (byte)year;
        bytes[3] = (byte)month;
        bytes[4] = (byte)day;
        bytes[5] = (byte)hour;
        bytes[6] = (byte)min;
        bytes[7] = (byte)sec;
        bytes[8] = 0;
        bytes[9] = 0;
        return bytes;
    }

    static public byte[] QueryBattery() {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[2];
        bytes[0] = 0x07;
        bytes[1] = 0;
        return bytes;
    }

    public string GetCurrentTimeStamp() {
        DateTime now = DateTime.Now;
        string result = now.Year + "-";
        result += (now.Month) + "-";
        result += now.Day + " ";
        result += now.Hour + ":";
        result += now.Minute + ":";
        result += now.Second + "";
        return result;
    }

    public string MakeTimeStamp(byte day, byte hour, byte min, byte sec) {
        int day_int = ( day / 16 ) * 10 + day % 16;
        int hour_int = ( hour / 16 ) * 10 + hour % 16;
        int min_int = ( min / 16 ) * 10 + min % 16;
        int sec_int = ( sec / 16 ) * 10 + sec % 16;
        DateTime now = DateTime.Now;
        return MakeTimeStamp(now.Year, ( now.Month ),
            day_int, hour_int, min_int, sec_int);
    }
    
    public string MakeTimeStamp(int day, int hour, int min, int sec) {
        DateTime now = DateTime.Now;
        return MakeTimeStamp(now.Year, ( now.Month ), day, hour, min, sec);
    }

    public string MakeTimeStamp(int year,int month,int day, int hour, int min, int sec) {
        return year + "-" + month + "-" + day + " " + hour + ":" + min + ":" + sec;
    }
}
