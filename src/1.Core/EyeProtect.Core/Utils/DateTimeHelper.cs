using System;

namespace EyeProtect.Core.Utils
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取这个月第一天
        /// </summary>
        /// <param name="time">这个月的任意时间</param>
        /// <returns></returns>
        public static DateTime ThisMonthFirstDay(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1);
        }

        /// <summary>
        /// 获取这个月第一天
        /// </summary>
        /// <param name="time">这个月的任意时间</param>
        /// <returns></returns>
        public static DateTime? ThisMonthFirstDay(this DateTime? time)
        {
            return time.HasValue ? new DateTime(time.Value.Year, time.Value.Month, 1) : new DateTime?();
        }

        /// <summary>
        /// 获取下个月第一天
        /// </summary>
        /// <param name="time">这个月的任意时间</param>
        /// <returns></returns>
        public static DateTime NextMonthFirstDay(this DateTime time)
        {
            return (new DateTime(time.Year, time.Month, 1)).AddMonths(1);
        }

        /// <summary>
        /// 转化为日期字符串
        /// </summary>
        public static string ToDateString(this DateTime? dateTime,string format= "yyyy-MM-dd")
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            return dateTime.Value.ToDateString(format);
        }

        /// <summary>
        /// 转化为日期字符串
        /// </summary>
        public static string ToDateString(this DateTime dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// 转化为日期字符串
        /// </summary>
        public static string ToDateTimeString(this DateTime? dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (dateTime == null)
            {
                return string.Empty;
            }
            return dateTime.Value.ToDateTimeString(format);
        }

        /// <summary>
        /// 转化为日期字符串
        /// </summary>
        public static string ToDateTimeString(this DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }
    }
}
