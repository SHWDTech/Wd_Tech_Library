using System;
using System.Text;

namespace SHWDTech.ByteConvert
{
    public class HexStringConverter
    {
        /// <summary>
        /// 将输入的Byte数组转换为十六进制显示的字符串
        /// </summary>
        /// <param name="data">需要转换的Byte数组</param>
        /// <param name="addPad">是否要添加空字符</param>
        /// <returns>data的字符串表示形式</returns>
        public static string ByteArrayToHexString(byte[] data, bool addPad = true)
        {
            var sb = new StringBuilder(data.Length * 3);
            foreach (var b in data)
            {
                var Char = Convert.ToString(b, 16).PadLeft(2, '0');
                if (addPad)
                {
                    Char = Char.PadRight(3, ' ');
                }
                sb.Append(Char);
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 将输入的Byte数组转换为UTF8字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToUtf8String(byte[] data) => Encoding.UTF8.GetString(data);

        /// <summary>
        /// 将输入的Byte数组转换为字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isHexMode"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] data, bool isHexMode)
            => isHexMode ? ByteArrayToHexString(data) : ByteArrayToUtf8String(data);

        /// <summary>
        /// 将输入的字符串转换为Byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GbkStringToByteArray(string str) => Encoding.GetEncoding("GBK").GetBytes(str);

        /// <summary>
        /// 将输入的HEX字符串转换为Byte数组
        /// </summary>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string str)
        {
            str = str.Replace(" ", "");
            if (str.Length % 2 != 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            var buffer = new byte[str.Length / 2];

            try
            {
                for (var i = 0; i < str.Length; i += 2)
                {
                    buffer[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (Exception)
            {
                return null;
            }


            return buffer;
        }

        /// <summary>
        /// 字符串转换为字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isHexMode"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str, bool isHexMode)
            => isHexMode ? HexStringToByteArray(str) : GbkStringToByteArray(str);
    }
}
