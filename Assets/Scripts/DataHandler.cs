using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    internal static string ServerAddress = "http://fluidtrack.site/";
    internal static string dataPath;

    public void Start() {
        dataPath = Application.dataPath;
    }

    [System.Serializable]
    public class UserLog {
        public int id;
        public string name;
        public string moa_band_name;
        public int item_0;
        public int item_1;
        public int item_2;
        public int item_3;
        public int item_4;
        public string morning_call_time;
        public string breakfast_time;
        public string lunch_time;
        public string dinner_time;
        public string school_time;
        public string home_time;
        public string water_skip;
        public string drink_skip;
        public string pee_skip;
        public string poop_skip;
        public string font_family;
        public int font_size;
    }

    [System.Serializable]
    public class UserLogsJson {
        public UserLog[] UserLogs;
    }

    [System.Serializable]
    public class GardenLog {
        public int log_id;
        public int id;
        public string timestamp;
        public int flower;
        public int item_0;
        public int item_1;
        public int item_2;
        public int item_3;
        public int item_4;
    }

    [System.Serializable]
    public class GardenLogsJson {
        public GardenLog[] GardenLogs;
    }

    [System.Serializable]
    public class WaterLog {
        public int log_id;
        public int id;
        public string timestamp;
        public int type;
    }

    [System.Serializable]
    public class WaterLogsJson {
        public WaterLog[] WaterLogs;
    }

    [System.Serializable]
    public class PoopLog {
        public int log_id;
        public int id;
        public string timestamp;
        public int type;
    }

    [System.Serializable]
    public class PoopLogsJson {
        public PoopLog[] PoopLogs;
    }

    [System.Serializable]
    public class PeeLog {
        public int log_id;
        public int id;
        public string timestamp;
    }

    [System.Serializable]
    public class PeeLogsJson {
        public PeeLog[] PeeLogs;
    }
}
