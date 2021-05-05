using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LogSpriteHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public enum LOG { WATER, POO, PEE, DRINK};
    private LogCanvasHandler handler;
    public bool isTop = false;
    public Text Number;

    public class LogScript {
        public LOG LogType = LOG.WATER;
        internal TimeHandler.DateTimeStamp TimeStamp;
        internal int Log_id;
        internal int Type = 0;

        public LogScript() {
            this.TimeStamp = null;
            this.Log_id = 0;
            this.Type = 0;
            this.LogType = LOG.WATER;
        }

        public LogScript(string timeStamp, int log_id, int type, LOG logType) {
            this.TimeStamp = TimeHandler.CreateNewStamp(timeStamp);
            this.Log_id = log_id;
            this.Type = type;
            this.LogType = logType;
        }

        public LogScript(TimeHandler.DateTimeStamp timeStamp, int log_id, int type, LOG logType) {
            this.TimeStamp = timeStamp;
            this.Log_id = log_id;
            this.Type = type;
            this.LogType = logType;
        }
    }

    public class Comparer : IComparer<LogScript> {
        public int Compare(LogScript x, LogScript y) {
            return TimeHandler.DateTimeStamp.
                CmpDateTimeStampDetail(x.TimeStamp, y.TimeStamp);
        }
    }
    
    public LogScript log;


    private void OnEnable() {
        handler =
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].
            GetComponent<LogCanvasHandler>();
    }

    public void SetData(LogScript target) {
        this.log = new LogScript(target.TimeStamp, target.Log_id,
                                 target.Type, target.LogType);
    }

    private bool isPress = false;
    private int count = 0;
    public void Update() {
        if (isPress) {
            count++;
            if(count >= 180) {
                PressLog();
                isPress = false;
                count = 0;
            }
        }
    }

    //public void OnPointerClick(PointerEventData eventData) {
    //    SoundHandler.Instance.Play_SFX(SoundHandler.SFX.COIN);
    //    if (!handler.WaterButtonClicked &&
    //       !handler.DrinkButtonClicked &&
    //       !handler.PooButtonClicked &&
    //       !handler.PeeButtonClicked) {
    //        Debug.Log(log.Log_id);
    //        Debug.Log(log.TimeStamp);
    //        Debug.Log(log.LogType);
    //        Debug.Log(log.Type);
    //        handler.DebugText.text = "[ " + log.Log_id + "] : ";
    //        handler.DebugText.text += "|" + log.TimeStamp + "| ";
    //        handler.DebugText.text += log.LogType;
    //        if (log.LogType == LOG.DRINK)
    //            handler.DebugText.text += " / volume : " + log.Type;
    //        else if (log.LogType == LOG.POO)
    //            handler.DebugText.text += " / type : " + log.Type;
    //    }
    //    Debug.Log(log.TimeStamp.Hours - LogCanvasHandler.Instance.currentFirstHour);
    //    int index = log.TimeStamp.Hours - LogCanvasHandler.Instance.currentFirstHour;

    //}
    public void OnPointerDown(PointerEventData eventData) {
        isPress = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (isPress) {
            if (count >= 120)
                PressLog();
            isPress = false;
            count = 0;
        }
    }

    public void PressLog() {
        Debug.Log(log.TimeStamp.Hours - LogCanvasHandler.Instance.currentFirstHour);
        int index = log.TimeStamp.Hours - LogCanvasHandler.Instance.currentFirstHour;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED2);
        if (log.LogType == LOG.POO || log.LogType == LOG.PEE)
             LogBlocker.Instance.BlockOnDetailDown(index, false, ( log.LogType == LOG.POO ));
        else LogBlocker.Instance.BlockOnDetailUp(index, false, ( log.LogType == LOG.DRINK ));
        LogCanvasHandler.Instance.PressLogIndex = index;
        switch(log.LogType) {
            case LOG.WATER: LogCanvasHandler.Instance.PressLogType = LogCanvasHandler.LOG_TYPE.WATER; break;
            case LOG.DRINK: LogCanvasHandler.Instance.PressLogType = LogCanvasHandler.LOG_TYPE.DRINK; break;
            case LOG.PEE: LogCanvasHandler.Instance.PressLogType = LogCanvasHandler.LOG_TYPE.PEE; break;
            case LOG.POO: LogCanvasHandler.Instance.PressLogType = LogCanvasHandler.LOG_TYPE.POOP; break;
        }
    }
}
