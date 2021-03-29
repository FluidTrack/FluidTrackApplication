using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour, IPointerClickHandler {
    public LogCanvasHandler LogHandler;
    public bool isUp = true;
    public int index = 0;
    public enum LOG_TYPE { WATER,DRINK,PEE,POO,};

    public void Start() {
        index = int.Parse(this.name.Split('_')[1]) - 1;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!DataHandler.User_isDataLoaded)
            return;

        int hours = ( 5 + index >= 24 ) ? index - 24 + 5 : 5 + index;

        if (isUp) {
            if (LogHandler.WaterButtonClicked) {
                DataHandler.User_isWaterDataCreated = false;
                DataHandler.WaterLog newLog = new DataHandler.WaterLog();
                newLog.id = DataHandler.User_id;
                newLog.timestamp = ( new TimeHandler.DateTimeStamp(
                                     TimeHandler.LogCanvasTime.Years,
                                     TimeHandler.LogCanvasTime.Months,
                                     TimeHandler.LogCanvasTime.Days
                                     , hours, 59, 59) ).ToString();
                newLog.type = 0;
                StartCoroutine(DataHandler.CreateWaterlogs(newLog));
                StartCoroutine(CheckCreateFunction(LOG_TYPE.WATER));
            } else if (LogHandler.DrinkButtonClicked) {
                DataHandler.User_isWaterDataCreated = false;
                DataHandler.WaterLog newLog = new DataHandler.WaterLog();
                newLog.id = DataHandler.User_id;
                newLog.timestamp = ( new TimeHandler.DateTimeStamp(
                                     TimeHandler.LogCanvasTime.Years,
                                     TimeHandler.LogCanvasTime.Months,
                                     TimeHandler.LogCanvasTime.Days
                                     , hours, 59, 59) ).ToString();
                newLog.type = 1;
                StartCoroutine(DataHandler.CreateWaterlogs(newLog));
                StartCoroutine(CheckCreateFunction(LOG_TYPE.DRINK));
            } else if (LogHandler.PeeButtonClicked) {
                DataHandler.User_isPeeDataLoaded = false;
                DataHandler.PeeLog newLog = new DataHandler.PeeLog();
                newLog.id = DataHandler.User_id;
                newLog.timestamp = ( new TimeHandler.DateTimeStamp(
                                     TimeHandler.LogCanvasTime.Years,
                                     TimeHandler.LogCanvasTime.Months,
                                     TimeHandler.LogCanvasTime.Days
                                     , hours, 59, 59) ).ToString();
                StartCoroutine(DataHandler.CreatePeelogs(newLog));
                StartCoroutine(CheckCreateFunction(LOG_TYPE.PEE));
            }
        } else {
            if (LogHandler.PooButtonClicked) {
                DataHandler.User_isPooDataLoaded = false;
                DataHandler.PoopLog newLog = new DataHandler.PoopLog();
                newLog.id = DataHandler.User_id;
                newLog.timestamp = ( new TimeHandler.DateTimeStamp(
                                     TimeHandler.LogCanvasTime.Years,
                                     TimeHandler.LogCanvasTime.Months,
                                     TimeHandler.LogCanvasTime.Days
                                     , hours, 59, 59) ).ToString();
                newLog.type = 0;
                StartCoroutine(DataHandler.CreatePooplogs(newLog));
                StartCoroutine(CheckCreateFunction(LOG_TYPE.POO));
            }
        }
    }

    IEnumerator CheckCreateFunction( LOG_TYPE type ) {
        bool flag = false;
        while(true) {
            switch(type) {
                case LOG_TYPE.WATER: flag = DataHandler.User_isWaterDataCreated; break;
                case LOG_TYPE.DRINK: flag = DataHandler.User_isDrinkDataCreated; break;
                case LOG_TYPE.PEE: flag = DataHandler.User_isPeeDataCreated; break;
                case LOG_TYPE.POO: flag = DataHandler.User_isPooDataCreated; break;
            }
            if(flag) {
                LogHandler.scroll.OnDisable();
                LogHandler.scroll2.OnDisable();
                StartCoroutine(LogHandler.scroll.FetchData());
                StartCoroutine(LogHandler.scroll2.FetchData());
                if (LogHandler.WaterButtonClicked)
                    LogHandler.OnWaterButtonClick();
                else if (LogHandler.DrinkButtonClicked)
                    LogHandler.OnDrinkButtonClick();
                else if (LogHandler.PeeButtonClicked)
                    LogHandler.OnPeeButtonClick();
                else if (LogHandler.PooButtonClicked)
                    LogHandler.OnPooButtonClick();
                break;
            }
            yield return 0;
        }
        switch (type) {
            case LOG_TYPE.WATER: DataHandler.User_isWaterDataCreated = false; break;
            case LOG_TYPE.DRINK: DataHandler.User_isDrinkDataCreated = false; break;
            case LOG_TYPE.PEE: DataHandler.User_isPeeDataCreated = false; break;
            case LOG_TYPE.POO: DataHandler.User_isPooDataCreated = false; break;
        }
    }
}
