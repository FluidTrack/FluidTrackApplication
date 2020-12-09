using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class DataHandler : MonoBehaviour
{
    internal static string ServerAddress = "http://fluidtrack.site/";
    internal static bool User_isDataLoaded = false;
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
        dataPath = Application.persistentDataPath;
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

    static public T JsonParsing <T>(string jsondata) {
        try {
            return JsonUtility.FromJson<T>(jsondata);
        } catch (System.Exception e) {
            Debug.LogError("Parsing Error\n" +  e.ToString());
            QuitApplication();
            return default(T);
        }
    }

    static public IEnumerator Create_users () {
        yield return 0;
        UnityWebRequest request = new UnityWebRequest();
        string url = "create_users";
        url += "?name=" + User_name;
        url += "&moa_band_name=" + User_moa_band_name;
        url += "&item_0=0&item_1=0&item_2=0&item_3=0&item_4=0";
        url += "&morning_call_time=" + User_morning_call_time + ":00";
        url += "&breakfast_time=" + User_breakfast_time + ":00";
        url += "&lunch_time=" + User_lunch_time + ":00";
        url += "&dinner_time=" + User_dinner_time + ":00";
        url += "&school_time=" + User_school_time + ":00";
        url += "&home_time=" + User_home_time + ":00";
        url += "&water_skip=" + User_water_skip + ":00";
        url += "&drink_skip=" + User_drink_skip + ":00";
        url += "&pee_skip=" + User_pee_skip + ":00";
        url += "&poop_skip=" + User_poop_skip + ":00";
        url += "&font_family=" + User_font_family;
        url += "&font_size=" + User_font_size;

        using (request = UnityWebRequest.Get(DataHandler.ServerAddress + url)) {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
                QuitApplication();
            else {
                Debug.Log(request.downloadHandler.text);
                try {
                    User_id = int.Parse(request.downloadHandler.text);
                    FileStream fs =
                        new FileStream(dataPath + "/userData", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(request.downloadHandler.text);
                    User_isDataLoaded = true;
                    sw.Close();
                    fs.Close();
                } catch (System.Exception e ) {
                    Debug.LogError(e.ToString());
                }
            }
            
        }
    }

    static public IEnumerator read_users(int target_id) {
        yield return 0;

        UnityWebRequest request = new UnityWebRequest();
        string url = "read_users";
        url += "?id=" + target_id;

        using (request = UnityWebRequest.Get(DataHandler.ServerAddress + url)) {
            yield return request.SendWebRequest();
            if (request.isNetworkError)
                QuitApplication();
            else {
                string jsonString = request.downloadHandler.text;
                UserLogsJson data = JsonParsing<UserLogsJson>(jsonString);
                Debug.Log(data.UserLogs[0].name);
                User_name = data.UserLogs[0].name;
                User_moa_band_name = data.UserLogs[0].moa_band_name;
                User_item_0 = data.UserLogs[0].item_0;
                User_item_1 = data.UserLogs[0].item_1;
                User_item_2 = data.UserLogs[0].item_2;
                User_item_3 = data.UserLogs[0].item_3;
                User_item_4 = data.UserLogs[0].item_4;
                User_morning_call_time = data.UserLogs[0].morning_call_time;
                User_breakfast_time = data.UserLogs[0].breakfast_time;
                User_lunch_time = data.UserLogs[0].lunch_time;
                User_dinner_time = data.UserLogs[0].dinner_time;
                User_school_time = data.UserLogs[0].school_time;
                User_home_time = data.UserLogs[0].home_time;
                User_water_skip = data.UserLogs[0].water_skip;
                User_drink_skip = data.UserLogs[0].drink_skip;
                User_pee_skip = data.UserLogs[0].pee_skip;
                User_poop_skip = data.UserLogs[0].poop_skip;
                User_font_family = data.UserLogs[0].font_family;
                User_font_size = data.UserLogs[0].font_size;
                User_isDataLoaded = true;
            }
        }
    }

    static public void QuitApplication() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif
    }
}
