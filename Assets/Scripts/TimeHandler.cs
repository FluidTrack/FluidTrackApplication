using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeHandler : MonoBehaviour {
    public class DateTimeStamp {
        static public int[] NormalYearDaysList = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static public int[] LeafYearDaysList = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static public string[] DateList = { "월요일","화요일","수요일","목요일",
                                            "금요일","토요일","일요일"};
        public static bool isLeafYear(int year) {
            if (( year % 4 ) == 0) {
                if (( year % 100 ) == 0) {
                    if (( year % 400 ) == 0) {
                        return true;
                    } else return false;
                } else return true;
            } else return false;
        }

        public int Years;
        public int Months;
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Date;

        public DateTimeStamp(int yyyy, int mm, int dd,
                        int hh, int MM, int ss) {
            this.Years = yyyy; this.Months = mm; this.Days = dd;
            this.Hours = hh; this.Minutes = MM; this.Seconds = ss;
        }

        public DateTimeStamp(DateTimeStamp stamp) {
            this.Years = stamp.Years; this.Months = stamp.Months;
            this.Days = stamp.Days; this.Hours = stamp.Hours;
            this.Minutes = stamp.Minutes; this.Seconds = stamp.Seconds;
            this.Date = stamp.Date;
        }

        public DateTimeStamp(DateTime datetime) {
            this.Years = datetime.Year; this.Months = datetime.Month;
            this.Days = datetime.Day; this.Hours = datetime.Hour;
            this.Minutes = datetime.Minute; this.Seconds = datetime.Second;
            switch (datetime.DayOfWeek) {
                case DayOfWeek.Monday: this.Date = 0; break;
                case DayOfWeek.Tuesday: this.Date = 1; break;
                case DayOfWeek.Wednesday: this.Date = 2; break;
                case DayOfWeek.Thursday: this.Date = 3; break;
                case DayOfWeek.Friday: this.Date = 4; break;
                case DayOfWeek.Saturday: this.Date = 5; break;
                case DayOfWeek.Sunday: this.Date = 6; break;
            }
        }

        public DateTimeStamp(String datetimeString) {
            try {
                string[] str = datetimeString.Split(' ');
                string[] date_string = str[0].Split('-');
                string[] time_string = str[1].Split(':');
                this.Years = int.Parse(date_string[0]);
                this.Months = int.Parse(date_string[1]);
                this.Days = int.Parse(date_string[2]);
                this.Hours = int.Parse(time_string[0]);
                this.Minutes = int.Parse(time_string[1]);
                this.Seconds = int.Parse(time_string[2]);
            } catch (System.Exception e) {
                Debug.LogWarning(e.ToString());
                Debug.Log(datetimeString);
                Debug.Log("DateTimeStamp Create exception");
                DateTime datetime = DateTime.Now;
                this.Years = datetime.Year; this.Months = datetime.Month;
                this.Days = datetime.Day; this.Hours = datetime.Hour;
                this.Minutes = datetime.Minute; this.Seconds = datetime.Second;
            }
        }

        override public string ToString() {
            return Years + "-" + Months + "-" + Days + " " +
                    Hours + ":" + Minutes + ":" + Seconds;
        }

        public string ToDateString() {
            return Years + "-" + Months + "-" + Days;
        }

        public static int CmpDateTimeStamp(DateTimeStamp a, DateTimeStamp b) {
            if (a.Years > b.Years) return 1;
            else if (a.Years < b.Years) return -1;
            else {
                if (a.Months > b.Months) return 1;
                else if (a.Months < b.Months) return -1;
                else {
                    if (a.Days > b.Days) return 1;
                    else if (a.Days < b.Days) return -1;
                    else return 0;
                }
            }
        }

        public static int CmpDateTimeStampDetail(DateTimeStamp a, DateTimeStamp b) {
            int result = CmpDateTimeStamp(a, b);
            if (result != 0) return result;
            if (a.Hours > b.Hours) return 1;
            else if (a.Hours < b.Hours) return -1;
            else {
                if (a.Minutes > b.Minutes) return 1;
                else if (a.Minutes < b.Minutes) return -1;
                else {
                    if (a.Seconds > b.Seconds) return 1;
                    else if (a.Seconds < b.Seconds) return -1;
                    else return 0;
                }
            }
        }

        public static int CmpDateTimeStamp(string a, string b) {
            DateTimeStamp _a = new DateTimeStamp(a);
            DateTimeStamp _b = new DateTimeStamp(b);
            return CmpDateTimeStamp(_a, _b);
        }

        public static int CmpDateTimeStampDetail(string a, string b) {
            DateTimeStamp _a = new DateTimeStamp(a);
            DateTimeStamp _b = new DateTimeStamp(b);
            return CmpDateTimeStampDetail(_a, _b);
        }

        public static DateTimeStamp operator +(DateTimeStamp stamp, int offset) {
            DateTimeStamp result = new DateTimeStamp(stamp);
            int dateOffset = offset % 7;
            result.Date += dateOffset;
            result.Date = ( result.Date >= 7 ) ? result.Date % 7 : result.Date;

            for (int i = 0; i < offset; i++) {
                int[] daysList = ( isLeafYear(result.Years) ) ?
                                    LeafYearDaysList : NormalYearDaysList;
                result.Days++;
                if (result.Days > daysList[result.Months - 1]) {
                    result.Months++;
                    result.Days = 1;
                    if (result.Months > 12) {
                        result.Months = 1;
                        result.Years++;
                    }
                }
            }
            return result;
        }

        public static DateTimeStamp operator -(DateTimeStamp stamp, int offset) {
            DateTimeStamp result = new DateTimeStamp(stamp);
            int dateOffset = offset % 7;
            result.Date -= dateOffset;
            result.Date = ( result.Date < 0 ) ? 7 + result.Date : result.Date;

            for (int i = 0; i < offset; i++) {
                int[] daysList = ( isLeafYear(result.Years) ) ?
                                    LeafYearDaysList : NormalYearDaysList;
                result.Days--;
                if (result.Days <= 0) {
                    result.Months--;
                    if (result.Months <= 0) {
                        result.Months = 12;
                        result.Years--;
                        daysList = ( isLeafYear(result.Years) ) ?
                                    LeafYearDaysList : NormalYearDaysList;
                    }
                    result.Days = daysList[result.Months - 1];
                }
            }
            return result;
        }
    }

    public static DateTimeStamp CreateNewStamp(string str) {
        return new DateTimeStamp(str);
    }

    public static DateTimeStamp CurrentTime;
    public static DateTimeStamp CreationTime;
    public static DateTimeStamp HomeCanvasTime;
    public static DateTimeStamp LogCanvasTime;
    public static DateTimeStamp FlowerCanvasTime;
    public static DateTimeStamp TableCanvasTime;
    public static DateTimeStamp CalendarCanvasTime;

    static internal string GetCurrentTime() {
        CurrentTime = new DateTimeStamp(DateTime.Now);
        HomeCanvasTime = CurrentTime;
        LogCanvasTime = CurrentTime;
        FlowerCanvasTime = CurrentTime;
        TableCanvasTime = CurrentTime;
        CalendarCanvasTime = CurrentTime;
        return CurrentTime.ToString();
    }
}
