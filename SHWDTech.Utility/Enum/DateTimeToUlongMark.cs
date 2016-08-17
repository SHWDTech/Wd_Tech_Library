namespace SHWDTech.Utility.Enum
{
    /// <summary>
    /// 时间和无符号长整形互相转换的标志位
    /// </summary>
    public static class DateTimeToUlongMark
    {
        /// <summary>
        /// 年
        /// </summary>
        public const byte YearMark = 36;

        /// <summary>
        /// 月
        /// </summary>
        public const byte MonthMark = 32;

        /// <summary>
        /// 日
        /// </summary>
        public const byte DayMark = 27;

        /// <summary>
        /// 时
        /// </summary>
        public const byte HourMark = 22;

        /// <summary>
        /// 分
        /// </summary>
        public const byte MinuteMark = 16;

        /// <summary>
        /// 秒
        /// </summary>
        public const byte SecondMark = 10;

        /// <summary>
        /// 毫秒
        /// </summary>
        public const byte MillisecondMark = 0;
    }
}
