using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace MergeTool
{
    public class HexFile
    {
        public static string IniPath;
        public static string HexFilePath1;
        public static string CombineHexFilePath;
        public static List<string> LineList1 = new List<string> { };
        public static List<string> LineList2 = new List<string> { };
        public static List<string> HexFile1 = new List<string> { };
        public static List<string> HexFile2 = new List<string> { };
        public static string HexFile1FirstLine = "";
        public static string HexFile2FirstLine = "";
        public static UInt32 iniStartAddr = 0, HexFileStartAddr = 0;
        public static DataLineMessage stDataLineMessage;
        public static IniFile iniFile;
        public static UInt32 HexFile1StartAddr;
        public static UInt32 HexFile2StartAddr;
        public static List<byte> DataBuff = new List<byte> { };
        public HexFile()
        {
            iniFile = new IniFile();
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
                CombineHexFilePath = Environment.CurrentDirectory + "\\" + iniFile.szCombineFileName;
            }
        }
        /// <summary>
        /// 读取Hex1文件
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public void getHexFileData(string FileName, int index)
        {
            string szline = "";
            int m = 0;
            if (!File.Exists(FileName))
            {
                Console.WriteLine("hex文件不存在");
                return;
            }
            else
            {
                StreamReader HexFileReader = new StreamReader(FileName);
                do
                {
                    szline = HexFileReader.ReadLine();
                    if (szline != null)
                    {
                        if(index ==1) //区别两个Hex文件
                            LineList1.Add(szline);
                        else
                            LineList2.Add(szline);
                        int datatype = Parse_HexLineData(szline); //解析行，并返回数据类型
                        switch (datatype)
                        {
                            case 0x00:
                                if (index == 1)
                                    FillLostData(LineList1, m); //填补空缺数据
                                else
                                    FillLostData(LineList2, m); //填补空缺数据
                                break;
                            case 0x01:
                                break;
                            case 0x02:
                                break;
                            case 0x03:
                                break;
                            case 0x04:
                                if (m == 0)
                                {
                                    if (!ChkStartAddrIsValid(szline, index))
                                    {
                                        return;
                                    }
                                }
                                break;
                            case 0x05:
                                if (index == 1)
                                    FillLostData(LineList1, m); //填补空缺数据
                                else
                                    FillLostData(LineList2, m); //填补空缺数据
                                break;
                            default:
                                Console.WriteLine("数据类型不是00， 01， 02， 03， 04， 05， 请检查Hex文件数据是否正确, 解析失败");
                                break;//解析失败
                        }
                        m++;
                        //Console.WriteLine("此时文件Line为： {0}", m);
                    }
                } while (szline != null);
                for (int i = 0; i < DataBuff.Count; i++)
                {
                    Console.WriteLine("DataBuff总数： {0}", i + 1);
                }
                //if (index == 1)
                //    HexFile1 = LineListNew;
                //else
                //    HexFile2 = LineListNew;
            }
        }
        public byte Parse_HexLineData(string szLine)
        {
            //冒号 本行数据长度(1byte) 本行数据的起始地址(2byte) 数据类型(1byte) 数据(N byte) 校验码(1byte)
            int Len = szLine.Length - 1;
            byte[] BytesData = new byte[Len / 2];  //声明一个长度为hexstring长度一半的字节组
            try
            {
                if (Len % 2 == 0)
                {
                    if (szLine.Substring(0, 1) == ":")    //判断首字符是":"
                    {

                        for (int i = 0; i < BytesData.Length; i++)
                        {
                            BytesData[i] = Convert.ToByte(szLine.Substring(i * 2 + 1, 2), 16);  //将hexstring的两个字符转换成16进制的字节组            
                        }
                        if (BytesData[0] != (BytesData.Length - 5))//长度域与实际长度不符
                        {
                            Console.WriteLine("数据长度域与实际长度不符");
                        }
                    }
                }
            }
            catch
            {

            }
            return BytesData[3];
        }

        /// <summary>
        /// 填充缺失的数据
        /// </summary>
        /// <param name="LineList">String类型集合</param>
        /// <returns></returns>
        public void FillLostData(List<string> LineList, int index)
        {
            int PreDataLen = 0, CurrentDataLen = 0;
            int CurrentDataAddr, PreDataAddr;
            byte FillData = 0xFF;
            string szData = "";

            CurrentDataAddr = int.Parse(LineList[index].Substring(3, 4), NumberStyles.HexNumber);
            PreDataAddr = int.Parse(LineList[index - 1].Substring(3, 4), NumberStyles.HexNumber);

            PreDataLen = int.Parse(LineList[index - 1].Substring(1, 2), NumberStyles.HexNumber);
            CurrentDataLen = int.Parse(LineList[index].Substring(1, 2), NumberStyles.HexNumber);
            szData = LineList[index].Substring(9, LineList[index].Length - 11);

            byte[] dataBuffer = new byte[CurrentDataLen];
            int datatype = Parse_HexLineData(LineList[index]); //解析行，并返回数据类型


            if (datatype == 0x00)
            {
                if (index > 1 && (CurrentDataAddr - PreDataAddr) > CurrentDataLen)
                {
                    var FillNum = CurrentDataAddr - PreDataAddr - PreDataLen; //两组line的Offset的差值
                    if(FillNum <= 0)
                    {
                        Console.WriteLine("第{0}行, {1}, 此行偏移量与上一行的偏移量的差，小于或等于0, 请检查数据是否正确", index, LineList[index]);
                    }
                    for (int i = 0; i < FillNum; i++)
                    {
                        DataBuff.Add(FillData);
                    }

                    for (int i = 0; i < CurrentDataLen; i++)  //填充完FF之后，解析当前Lines数据
                    {
                        dataBuffer[i] = byte.Parse(szData.Substring(i * 2, 2), NumberStyles.HexNumber);
                        DataBuff.Add(dataBuffer[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < CurrentDataLen; i++)
                    {
                        dataBuffer[i] = byte.Parse(szData.Substring(i * 2, 2), NumberStyles.HexNumber);
                        DataBuff.Add(dataBuffer[i]);
                    }
                }
            }
            else if (datatype == 0x05) //只解析首地址数据
            {
                int MaxAddr = 0xFFFF;
                var FillNum = Math.Abs((MaxAddr - PreDataAddr - PreDataLen));
                for (int i = 0; i < FillNum; i++)
                {
                    DataBuff.Add(FillData);
                }
                DataBuff.Add(FillData);
            }
            else
            {
                //for (int i = 0; i < CurrentDataLen; i++)
                //{
                //    dataBuffer[i] = byte.Parse(szData.Substring(i * 2, 2), NumberStyles.HexNumber);
                //    DataBuff.Add(dataBuffer[i]);
                //}
                ;
            }
        }
        /// <summary>
        /// 检查文件开始地址是否正确
        /// </summary>
        /// <param name="FirstLine">Hex文件首行</param>
        public  bool ChkStartAddrIsValid(string FirstLine, int index)
        {
            stDataLineMessage.HighAddr = UInt32.Parse(FirstLine.Substring(9, 4), NumberStyles.HexNumber);
            stDataLineMessage.LowAddr = UInt32.Parse(FirstLine.Substring(3, 4), NumberStyles.HexNumber);
            stDataLineMessage.HexStartAddr = UInt32.Parse(FirstLine.Substring(9, 4) + FirstLine.Substring(3, 4), NumberStyles.HexNumber);
            if (index == 1)
            {
                iniStartAddr = UInt32.Parse(iniFile.szSourceFile1StartAddr, NumberStyles.HexNumber);
                HexFile1StartAddr = stDataLineMessage.HexStartAddr;
            }
            else
            {
                iniStartAddr = UInt32.Parse(iniFile.szSourceFile2StartAddr, NumberStyles.HexNumber);
                HexFile2StartAddr = stDataLineMessage.HexStartAddr;
            }
            if (stDataLineMessage.HexStartAddr < iniStartAddr)
            {
                Console.WriteLine("Hex文件中的开始地址小于ini文件设置的开始地址, ini配置中的地址:0x{0}, Hex文件中读取到的起始地址:0x{1}", String.Format("{0:X8}", iniStartAddr), String.Format("{0:X8}", stDataLineMessage.HexStartAddr));
                return false;
            }
            return true;
        }

        public  void MergeHexFile(List<string> List1,List<string>List2, string SaveMergeHexPath)
        {
            List<string> MergeHexFile = new List<string> { };
            int HexFile1StartAddr = 0, HexFile2StartAddr = 0;
            HexFile1StartAddr = int.Parse(List1[0].Substring(9, 4), NumberStyles.HexNumber) << 16;
            HexFile2StartAddr = int.Parse(List2[0].Substring(9, 4), NumberStyles.HexNumber);
            if(HexFile1StartAddr < HexFile2StartAddr)
            {
                MergeHexFile.Add(List1[0]);
                for (int i=0; i< HexFile1.Count; i++)
                {
                    MergeHexFile.Add(HexFile1[i]);
                }
                MergeHexFile.Add(List2[0]);
                for (int i = 0; i < HexFile1.Count; i++)
                {
                    MergeHexFile.Add(HexFile2[i]);
                }
            }

            StreamWriter swCombineHex = new StreamWriter(SaveMergeHexPath);
            for(int i = 0; i < MergeHexFile.Count; i++)
            {
                swCombineHex.WriteLine(MergeHexFile[i]);
            }
            swCombineHex.Close();
            Console.WriteLine();
        }

        public void ResizeDataLength(List<string> LineList)
        {
            string DataStr = "", LineStr = "";
            int DataLen, i, j, DataType, len1, len2, counter = 1 ;
            int CombineLineCharNum = int.Parse(iniFile.szCombineFileLineCharNum);
            byte[] dataBuffer = new byte[CombineLineCharNum]; //根据ini配置的长度来规定长度
            int Offset = 0x0000;
            string lineStr = ":", str = "", s;
            
            for (i = 0; i < DataBuff.Count / CombineLineCharNum; i++)
            {
                lineStr = ":" +  string.Format("{0:X2}", CombineLineCharNum) + String.Format("{0:X4}", Offset) + "00";
                for (j = 0; j < CombineLineCharNum; j++)
                {
                    s = string.Format("{0:X2}", DataBuff[i * CombineLineCharNum + j]);
                    lineStr += s;
                }
                Offset += CombineLineCharNum;

                caclChkSum(lineStr,out str);
                lineStr = str;
                LineList.Add(lineStr);
            }
        }

        public void ParseHexToBinFile()
        {

        }

        public static void caclChkSum(in string inLine, out string outStr)
        {
            byte[] Arr;
            byte checkSum = 0;
            UInt64 Sum = 0;
            string str = inLine.Substring(1, inLine.Length - 1);
            int len = str.Length;
            Arr = new byte[len / 2];
            for (int i = 0; i < len / 2; i++)
            {
                Arr[i] = byte.Parse(inLine.Substring(i * 2 + 1, 2), NumberStyles.HexNumber);
                Sum += Arr[i];
            }
            Sum = 0x100 - (Sum & 0xFF);
            checkSum = Convert.ToByte(Sum & 0xFF);
            outStr = inLine + String.Format("{0:X2}", checkSum);
        }
    }
}
