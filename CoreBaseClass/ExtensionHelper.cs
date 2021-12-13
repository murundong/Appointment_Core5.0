using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreBaseClass
{
    public static class ExtensionHelper
    {
        public static bool IsAsync(this MethodInfo method)
        {
            return (
               method.ReturnType == typeof(Task) ||
               (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
           );
        }
        public static DateTime ToDateTime(this long stamp)
        {
            return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).AddSeconds(stamp);

            //return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(stamp);
        }
        public static bool TheSameDayAs(this DateTime dt, DateTime cp)
        {
            return dt.Year == cp.Year && dt.Month == cp.Month && dt.Day == cp.Day;
        }

       public static DateTime GetDateTimeWithoutTime(this DateTime dt)
        {
            return Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
        }
        public static DateTime GetFirstWeekDay(this DateTime dt)
        {
            int weeknow = (int)dt.DayOfWeek;
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            return Convert.ToDateTime(dt.AddDays(daydiff).ToString("yyyy-MM-dd"));
        }

        public static DateTime NowDate(this DateTime dt)
        {
            return Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
        }

        public static DateTime GetLastDayOfWeek(this DateTime dt)
        {
            int weeknow = Convert.ToInt32(dt.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            return Convert.ToDateTime(dt.AddDays(daydiff).ToString("yyyy-MM-dd"));
        }

        public static DateTime GetPrevFirstWeekDay(this DateTime dt)
        {
            return Convert.ToDateTime(DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 7 + 1).ToString("yyyy-MM-dd"));
        }

        public static DateTime GetPrevLastWeekDay(this DateTime dt)
        {
            return Convert.ToDateTime(DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek) - 7 + 1).ToString("yyyy-MM-dd"));
        }

        public static long GetTimeTicks(this DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        public static string GetWeekOfTime(this DateTime dt)
        {
            string[] Day = new string[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = Day[Convert.ToInt32(dt.DayOfWeek.ToString("d"))].ToString();
            return week;
        }

        public static string GetDateTimeWithoutDate(this DateTime dt)
        {
            return dt.ToString("HH:mm");
        }

        public static string GetInitial(this string str)
        {
            if (str.CompareTo("吖") < 0)
            {
                string s = str.Substring(0, 1).ToUpper();
                if (char.IsNumber(s, 0))
                {
                    return "#";
                }
                else
                {
                    return s;
                }

            }
            else if (str.CompareTo("八") < 0)
            {
                return "A";
            }
            else if (str.CompareTo("嚓") < 0)
            {
                return "B";
            }
            else if (str.CompareTo("咑") < 0)
            {
                return "C";
            }
            else if (str.CompareTo("妸") < 0)
            {
                return "D";
            }
            else if (str.CompareTo("发") < 0)
            {
                return "E";
            }
            else if (str.CompareTo("旮") < 0)
            {
                return "F";
            }
            else if (str.CompareTo("铪") < 0)
            {
                return "G";
            }
            else if (str.CompareTo("讥") < 0)
            {
                return "H";
            }
            else if (str.CompareTo("咔") < 0)
            {
                return "J";
            }
            else if (str.CompareTo("垃") < 0)
            {
                return "K";
            }
            else if (str.CompareTo("嘸") < 0)
            {
                return "L";
            }
            else if (str.CompareTo("拏") < 0)
            {
                return "M";
            }
            else if (str.CompareTo("噢") < 0)
            {
                return "N";
            }
            else if (str.CompareTo("妑") < 0)
            {
                return "O";
            }
            else if (str.CompareTo("七") < 0)
            {
                return "P";
            }
            else if (str.CompareTo("亽") < 0)
            {
                return "Q";
            }
            else if (str.CompareTo("仨") < 0)
            {
                return "R";
            }
            else if (str.CompareTo("他") < 0)
            {
                return "S";
            }
            else if (str.CompareTo("哇") < 0)
            {
                return "T";
            }
            else if (str.CompareTo("夕") < 0)
            {
                return "W";
            }
            else if (str.CompareTo("丫") < 0)
            {
                return "X";
            }
            else if (str.CompareTo("帀") < 0)
            {
                return "Y";
            }
            else if (str.CompareTo("咗") < 0)
            {
                return "Z";
            }
            else
            {
                return "#";
            }

        }
    }
}
