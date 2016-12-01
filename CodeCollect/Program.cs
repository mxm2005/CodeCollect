using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCollect
{
    class Program
    {
        static void Main(string[] args)
        {
        	
		//demo 测试文件编码格式判断
            string ff = "E:\\CheckResultPath\\DetailFile\\20161130\\18833-80887119085622.xml";
            //FileInfo inf = new FileInfo(ff);
            Console.WriteLine(JudgeFileEncodingType.GetType(ff).ToString());

            Console.Read();
        }
    }
}
