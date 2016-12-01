using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace CodeCollect
{


    /// <summary>
    /// TXT文件操作类，自动判断编码格式读取
    /// mxm 2016-11-28
    /// </summary>
    public static class TxtOpt2
    {
        public static Encoding enType = null;

        /// <summary>
        /// 获取文件编码格式
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Encoding GetGeneralType(string filePath)
        {
            if (enType == null)
                enType = GetType(filePath);
            return enType;
        }

        /// <summary>
        /// 如果路径为空，默认日志写到C:\\Error.txt，每写一条都换一行
        /// </summary>
        /// <param name="path">文件物理全路径</param>
        /// <param name="str">写入的内容</param>
        /// <returns></returns>
        public static bool WriteTxt(string path, string str)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                string fileName = path;
                if (!File.Exists(fileName))
                {
                    //新建文件并写入
                    StreamWriter sw = File.CreateText(fileName);
                    sw.Write(str);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    //C#追加文件 
                    StreamWriter sw = File.AppendText(fileName);
                    sw.Write(str);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取网站根目录或EXE目录+\
        /// </summary>
        /// <returns></returns>
        public static string GetExeOrWebRootPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 读取文件全部内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadTxt(string path)
        {
            string resVal = "";
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                StreamReader f2 = new StreamReader(path, GetGeneralType(path));
                resVal = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return resVal;
        }

        /// <summary>
        /// 获取文件大小(字节)，文件不存在返回-1
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static long GetFileLength(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return -1;
            }
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}