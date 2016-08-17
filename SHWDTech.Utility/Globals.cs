using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SHWDTech.Utility.Enum;

namespace SHWDTech.Utility
{
    public class Globals
    {
        private static string _applicationPath = string.Empty;

        /// <summary>
        /// 返回应用程序启动路径
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_applicationPath))
                {
                    _applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                    _applicationPath = _applicationPath.Replace("/", Path.DirectorySeparatorChar.ToString());
                    _applicationPath = _applicationPath.TrimEnd(Path.DirectorySeparatorChar);
                }

                return _applicationPath;
            }
        }

        /// <summary>
        /// 判断指定的Object是否是null、 全部是空格，或着不存在的值（DBNull）其中的一项
        /// </summary>
        /// <param name="obj">待检测的Object值</param>
        /// <returns>返回布尔类型，若参数是null、全部是空格，活着不存在的值（DBNull）中的一项，则返回True，否则返回False</returns>
        public static bool IsNullOrEmpty(object obj) =>
            obj == null || (obj is string && string.IsNullOrWhiteSpace(obj.ToString())) || obj is DBNull || (obj is Guid && (Guid)obj == Guid.Empty);

        /// <summary>
        /// 产生一个随机数字符串
        /// </summary>
        /// <param name="length">指定的字符串长度</param>
        /// <returns></returns>
        public static string Random(int length)
        {
            var rd = new Random(Guid.NewGuid().GetHashCode());

            if (length <= 0) return rd.Next(2).ToString();

            var sb = new StringBuilder();
            for (var i = 0; i < 100; i++)
            {
                sb.Append(rd.Next(SafeInt("9".PadLeft(length, '9'))));

                if (sb.Length >= length)
                {
                    sb.Remove(length, sb.Length - length);
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 无论原值是什么类型，返回一个32位有符号整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int SafeInt(object obj)
            => SafeInt(obj, 0);

        /// <summary>
        /// 返回一个32位有符号整型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int SafeInt(object obj, int defaultValue)
        {
            if (!IsNullOrEmpty(obj))
            {
                try
                {
                    return Convert.ToInt32(obj);
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取MD5加密字符串
        /// </summary>
        /// <param name="str">被加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string GetMd5(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str))).ToLower().Replace("-", "");
        }

        /// <summary>
        /// 生成22个字符的标识码
        /// </summary>
        /// <returns>标识码</returns>
        public static string NewIdentityCode()
        {
            Thread.Sleep(1);

            var identityNum = GetDateBytes(DateTime.Now);

            return identityNum.ToString("X2");
        }

        /// <summary>
        /// 根据传入的时间，计算一个ulong类型的值，存储时间信息（主要用来生成标识码）
        /// </summary>
        /// <param name="dt">传入的时间</param>
        /// <returns>一个Long</returns>
        public static ulong GetDateBytes(DateTime dt)
        {
            ulong identityNum = 0;

            //年取值0-9999， 12位 = 4096
            identityNum |= (ulong)dt.Year << DateTimeToUlongMark.YearMark;

            //月取值1-12， 4位 = 16
            identityNum |= (ulong)dt.Month << DateTimeToUlongMark.MonthMark;

            //日取值0-31，5位 = 32
            identityNum |= (ulong)dt.Day << DateTimeToUlongMark.DayMark;

            //时取值0-23，5位 = 32
            identityNum |= (ulong)dt.Hour << DateTimeToUlongMark.HourMark;

            //分取值0-59，6位 = 64
            identityNum |= (ulong)dt.Minute << DateTimeToUlongMark.MinuteMark;

            //秒取值0-59，6位 = 64
            identityNum |= (ulong)dt.Second << DateTimeToUlongMark.SecondMark;

            //毫秒取值0-999，10位 = 1024
            identityNum |= (ulong)dt.Millisecond << DateTimeToUlongMark.MillisecondMark;

            return identityNum;
        }

        /// <summary>
        /// 根据传入的无符号长整型，计算一个时间值
        /// </summary>
        /// <param name="dtLong"></param>
        /// <returns></returns>
        public static DateTime GetDateFormLong(ulong dtLong)
        {
            var year = Convert.ToInt32((dtLong & UlongToDateTimeFlag.YearFlag) >> DateTimeToUlongMark.YearMark);

            var month = Convert.ToInt32((dtLong & UlongToDateTimeFlag.MonthFlag) >> DateTimeToUlongMark.MonthMark);

            var day = Convert.ToInt32((dtLong & UlongToDateTimeFlag.DayFlag) >> DateTimeToUlongMark.DayMark);

            var hour = Convert.ToInt32((dtLong & UlongToDateTimeFlag.HourFlag) >> DateTimeToUlongMark.HourMark);

            var minute = Convert.ToInt32((dtLong & UlongToDateTimeFlag.MinuteFlag) >> DateTimeToUlongMark.MinuteMark);

            var second = Convert.ToInt32((dtLong & UlongToDateTimeFlag.SecondFlag) >> DateTimeToUlongMark.SecondMark);

            var millisecond = Convert.ToInt32((dtLong & UlongToDateTimeFlag.MillisecondFlag) >> DateTimeToUlongMark.MillisecondMark);

            var dt = new DateTime(year, month, day, hour, minute, second, millisecond);

            return dt;
        }

        /// <summary>
        /// 获得有顺序的GUID，用Guid前10位加时间参数生成，时间加在最前面6个字节
        /// 需要注意的是，在SQL SERVER数据库中，使用GUID字段类型保存的话，SQL SERVER对GUID类型字段排序算法是以最后6字节为主要依据，
        /// 这与Oracle不同，为了保证排序规则与Oracle一致，在SQL SERVER中要使用Binary(16)数据类型来保存。
        /// </summary>
        /// <returns>返回一个有顺序的GUID</returns>
        public static Guid NewCombId()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;
            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Date.Ticks - baseDate.Ticks);
            var msecs = new TimeSpan(now.Ticks - (now.Date.Ticks));

            // Convert to a byte array 
            // SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            for (var i = 15; i >= 6; i--)
            {
                guidArray[i] = guidArray[i - 6];
            }

            Array.Copy(daysArray, daysArray.Length - 2, guidArray, 0, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, 2, 4);

            return new Guid(guidArray);
        }

        /// <summary>
        /// 获取本地IP地址的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpAddressString()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            var ipv4 = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();

            if (ipv4.Count == 0) return string.Empty;

            foreach (var ipAddress in ipv4)
            {
                var ipStr = ipAddress.ToString().Split('.');
                if (ipStr[0] == "192") return ipAddress.ToString();
            }

            return ipv4[0].ToString();
        }

        /// <summary>
        /// 获取所有IP地址的列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIpV4AddressStringList()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.Where(obj => obj.AddressFamily == AddressFamily.InterNetwork)
                .Select(ipAddress => ipAddress.ToString()).ToList();
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            var ipv4 = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();

            if (ipv4.Count == 0) return null;

            foreach (var ipAddress in ipv4)
            {
                var ipStr = ipAddress.ToString().Split('.');
                if (ipStr[0] == "192") return ipAddress;
            }

            return ipv4[0];
        }

        public static bool IsWindows10()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            if (reg == null) return false;
            var productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        /// <summary>
        /// 获取可空类型浮点数值字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetNullableDouble(double? value)
        {
            if (value == null) return 0.0;

            return value.Value;
        }

        /// <summary>
        /// 获取可空类型时间字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetNullableDateTimeString(DateTime? value, string format)
            => value?.ToString(format) ?? "N/A";

        /// <summary>
        /// 获取DisplayName属性
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default))
                {
                    return displayName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    // Check for DisplayName attribute and set the column header accordingly
                    var attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    foreach (var t in attributes)
                    {
                        var displayName = t as DisplayNameAttribute;
                        if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default))
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }

            return null;
        }
    }
}
