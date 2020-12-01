using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    internal static string ServerAddress = "http://fluidtrack.site/";
    internal static string dataPath;
    internal static int User_id;
    internal static string User_name;
    internal static string User_moa_band_name;
    internal static int User_item_0;
    internal static int User_item_1;
    internal static int User_item_2;
    internal static int User_item_3;
    internal static int User_item_4;
    internal static string User_morning_call_time;
    internal static string User_breakfast_time;
    internal static string User_lunch_time;
    internal static string User_dinner_time;
    internal static string User_school_time;
    internal static string User_home_time;
    internal static string User_water_skip;
    internal static string User_drink_skip;
    internal static string User_pee_skip;
    internal static string User_poop_skip;
    internal static string User_font_family;
    internal static int User_font_size;


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
