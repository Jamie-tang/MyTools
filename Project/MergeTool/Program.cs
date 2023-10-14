using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

//hex格式解析：<0x3a>[数据长度1Byte][数据地址2Byte][数据类型1Byte][数据nByte][校验1Byte]<0x0d><0x0a>
/*
'00' Data Record 数据
'01' End of File Record 文件结束标志
'02' Extended Segment Address Record 延伸段地址
'03' Start Segment Address Record   起始延伸地址
'04' Extended Linear Address Record 扩展线性地址 也就是基地址
'05' Start Linear Address Record       程序起始地址也就是程序入口地址(main)
0800 这个就是基地址(0x0800<<16)
 */
namespace MergeTool
{
    class Program
    {
        public static string IniPath;
        public static string HexFilePath1;
        public static string HexFilePath2;
        public static string CombineHexFilePath;
        public static UInt32 iniStartAddr = 0, HexFileStartAddr = 0;
        public static DataLineMessage stDataLineMessage;

        static void Main(string[] args)
        {
            HexFile hexFile1 = new HexFile();
            IniFile iniFile = new IniFile();
            stDataLineMessage = new DataLineMessage();
            IniPath = Environment.CurrentDirectory + "\\Config.ini";
            if (!File.Exists(IniPath))
            {
                Console.WriteLine("没有找到名为Config.ini的配置文件");
                Console.ReadLine();
                return;
            }
            else
            {
                iniFile.ReadIniFlie(IniPath);
                HexFilePath1 = Environment.CurrentDirectory + "\\" + iniFile.szSourceFile1Name;
                hexFile1.getHexFileData(HexFilePath1, 1);
                HexFilePath2 = Environment.CurrentDirectory + "\\" + iniFile.szSourceFile2Name;
                hexFile1.getHexFileData(HexFilePath2, 2);
                CombineHexFilePath = Environment.CurrentDirectory + "\\" + iniFile.szCombineFileName;
                hexFile1.MergeHexFile(HexFile.HexFile1, HexFile.HexFile2, CombineHexFilePath);
            }
        }
    }
}
