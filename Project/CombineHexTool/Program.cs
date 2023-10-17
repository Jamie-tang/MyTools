using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace CombineHexTool
{
    class Program
    {
        public static string IniPath;
        public static string HexFilePath1;
        public static string HexFilePath2;
        public static string CombineHexFilePath;
        public static string CombineBinFilePath;
        public static UInt32 u32iniStartAddr = 0, u32HexFileStartAddr = 0;
        public static DataLineMessage stDataLineMessage;

        static void Main(string[] args)
        {
            HexFile hexFile = new HexFile();
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
                hexFile.getHexFileData(HexFilePath1, 1);
                HexFilePath2 = Environment.CurrentDirectory + "\\" + iniFile.szSourceFile2Name;
                hexFile.getHexFileData(HexFilePath2, 2);

                CombineBinFilePath = Environment.CurrentDirectory + "\\" + iniFile.szBinFileFileName;
                hexFile.ParseHexToBinFile(HexFile.LineList1, HexFile.LineList2, CombineBinFilePath);

                hexFile.ResizeDataLength(HexFile.HexFile1, 1);  //Resize数据长度并添加checksum到Line尾，  且计算bin文件的Crc16，并添加到指定位置
                hexFile.ResizeDataLength(HexFile.HexFile2, 2);  //Resize数据长度并添加checksum到Line尾，  且计算bin文件的Crc16，并添加到指定位置

                CombineHexFilePath = Environment.CurrentDirectory + "\\" + iniFile.szCombineFileName;
                hexFile.MergeHexFile(HexFile.LineList1, HexFile.LineList2, CombineHexFilePath); // 生成合并Hex文件
            }
            Console.ReadKey();
        }
    }
}
