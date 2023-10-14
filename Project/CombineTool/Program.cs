using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace CombineTool
{
    class Program
    {

        public class _HexConfig
        {
            public Int32 AppSize = 0x4000;
            public Int32 AppLoction = 0x1800;
            public Int32 AppCrcLoction = 0x1800;
            public Int32 BootSize = 0x1800;
            public Int32 BootLocation = 0;
            public Int32 BootCrcLocation = 0x1800;
            public Int32 FlashSize = 0x5800;
            public Int32 ConfigAdd = 0x030;
            public Int32 ConfigSize = 0x28;
            public string ChipType = "PIC18";
            public string ChipLoction = "SEC";
            public string AppExportName = "";
            public string BootExportName = "";
            public string CombineFileName = "";
            public string BootImportName = "";
            public string AppImportName = "";
            public string BinFileName = "";

        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        public static _HexConfig HexConfig = new _HexConfig();
        static string IniReadKey(string iniFile, string Section, string key)//read from ini file
        {

            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, key, "", temp, 1024, iniFile);
            return temp.ToString();
        }
        static void IniWriteKey(string iniFile, string Section, string key, string val)//read from ini file
        {
            WritePrivateProfileString(Section, key, val, iniFile);
        }

        static UInt16[] CRC16table =
            {
            0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
            0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
            0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
            0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
            0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
            0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
            0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
            0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
            0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
            0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
            0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
            0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
            0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
            0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
            0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
            0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
            0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
            0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
            0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
            0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
            0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
            0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
            0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
            0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
            0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
            0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
            0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
            0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
            0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
            0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
            0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
            0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
            };


       static  void ReadParamererFromConfigFile(string filename)
        {
            string buff = "";
            string path = System.Environment.CurrentDirectory + "\\";
         //   IniFile Ini = new IniFile();
            buff = IniReadKey(filename, "Import", "SECTOR_MAIN_LOCATION");
            HexConfig.AppLoction = Convert.ToInt32(buff, 16);
            buff = IniReadKey(filename, "Import", "SECTOR_MAIN_SIZE");
            HexConfig.AppSize = Convert.ToInt32(buff, 16);
            buff = IniReadKey(filename, "Import", "SECTOR_MAIN_CRC_LOCATION");
            HexConfig.AppCrcLoction = Convert.ToInt32(buff, 16);
            buff =IniReadKey(filename, "Import", "MAIN_FILE_NAME");
            HexConfig.AppImportName = path + buff;

            buff = IniReadKey(filename, "Import", "BOOT_FILE_NAME");
            HexConfig.BootImportName = path + buff;
            buff = IniReadKey(filename, "Import", "SECTOR_BOOT_LOCATION");
            HexConfig.BootLocation = Convert.ToInt32(buff, 16);
            buff = IniReadKey(filename, "Import", "SECTOR_BOOT_SIZE");
            HexConfig.BootSize = Convert.ToInt32(buff, 16);
            //   HexConfig.ConfigAdd=
            buff = IniReadKey(filename, "Import", "FLASH_SIZE");
            HexConfig.FlashSize = Convert.ToInt32(buff, 16);
            buff =IniReadKey(filename, "Import", "SECTOR_BOOT_CRC_LOCATION");
            HexConfig.BootCrcLocation = Convert.ToInt32(buff, 16);



            buff = IniReadKey(filename, "Export", "EXPORT_MAIN_FILE_NAME");
            HexConfig.AppExportName = path + buff;
            buff = IniReadKey(filename, "Export", "EXPORT_MAIN_SECTOR_BINARY");
            HexConfig.BinFileName = path + buff;
            buff = IniReadKey(filename, "Export", "EXPORT_BOOT_FILE_NAME");
            HexConfig.BootExportName = path + buff;
            buff = IniReadKey(filename, "Export", "EXPORT_COMBINE_FILE_NAME");
            HexConfig.CombineFileName = path + buff;


           buff = IniReadKey(filename, "General", "CONFIG_SIZE");
           HexConfig.ConfigSize = Convert.ToInt32(buff, 16);
           buff = IniReadKey(filename, "General", "CHIP_TYPE");
           HexConfig.ChipType = buff;
           buff = IniReadKey(filename, "General", "CHIP_LOCATION");
           if (buff != null)
           {
               HexConfig.ChipLoction = buff;
           }
           if (HexConfig.ChipType == "PIC18F")
           {
               HexConfig.ConfigAdd = 0x030;
           }
           else if (HexConfig.ChipType == "dsPIC33FJ")
           {
               HexConfig.ConfigAdd = 0x01F0;
           }
           else
           {
               HexConfig.ChipType = "dsPIC33FJ";
               HexConfig.ConfigAdd = 0x01F0;
           }


        }
       static byte[] GetConfigBitsFromBoot(string BootFileName)
       {
           byte[] data = new byte[100];

           StreamReader sr = new StreamReader(BootFileName);
           Int64 add = 0;
           Int64 BootSectorLocation = HexConfig.BootLocation;
           Int64 FlashSize = HexConfig.FlashSize;
           Int64 BootAppSize = HexConfig.BootSize;
           UInt16[] DefaultConfig ={
                                    0x000F,       //0
                                    0x0000,       //2
                                    0x0007,       //4
                                    0x0087,       //6
                                    0x00E7,       //8
                                    0x00df,       //a
                                    0x000F,       //c
                                    0x00C3,       //e
                                    0x00FF,       //10
                                    0x00FF};      //12

           byte[] arry = new byte[FlashSize];
           byte[] Config = new byte[100];

           Int64 i = 0;

           for (i = 0; i < arry.Length; i++)
           {
               if ((i + 1) % 4 == 0)
               {
                   arry[i] = 0x00;
               }
               else
               {
                   arry[i] = 0xFF;
               }

           }

           if (HexConfig.ChipType == "dsPIC33FJ")
           {
               for (i = 0; i < DefaultConfig.Length; i++)
               {
                   Config[i * 4 + 0] = Convert.ToByte(DefaultConfig[i] & 0xFF);
                   Config[i * 4 + 1] = Convert.ToByte((DefaultConfig[i] & 0xFF00) >> 8);
                   Config[i * 4 + 2] = 0x00;
                   Config[i * 4 + 3] = 0x00;

               }
           }

           string szLine = "";
           string szHex = "";
           string szConfig = "";
           int data_n = 0;
           UInt16 CRC16 = 0;
           Int32 hexAddLow = 0x00;
           Int32 hexAddHigh = 0x00;
           byte dataType = 0x00;
           bool DataIsConfigFlag = false;
           Int32 ConfigAdd = HexConfig.ConfigAdd;
           int m = 0;
           Int64 j = 0;
           byte[] databuff = new byte[100];
           while (true)
           {
               szLine = sr.ReadLine(); //读取一行数据  
               if (szLine == null) //读完所有行  
               {
                   break;
               }
               else if (szLine == ":00000001FF")
               {
                 //  Console.WriteLine("got a end ");
                   break;
               }
               else
               {

                   data_n = Convert.ToInt16(szLine.Substring(1, 2), 16);
                   //:0200000401F009
                   dataType = Convert.ToByte(Convert.ToInt16(szLine.Substring(7, 2), 16) & 0xFF);

                   if (dataType == 0x04)
                   {
                       hexAddHigh = Convert.ToInt16(szLine.Substring(9, 4), 16);
                       if (hexAddHigh == ConfigAdd) // if it is a config address 
                       {
                           DataIsConfigFlag = true;
                       }
                       else
                       {
                           DataIsConfigFlag = false;
                       }
                   }
                   else if (dataType == 0x00)
                   {
                       if (DataIsConfigFlag) //  if it is a Config 
                       {
                           szConfig = szLine.Substring(9, szLine.Length - 11);
                           hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                           for (m = 0, j = 0; m < szConfig.Length; m += 2) //两字符合并成一个16进制字节  
                           {
                               databuff[j] = (byte)Int16.Parse(szConfig.Substring(m, 2), NumberStyles.HexNumber);
                               Config[hexAddLow + j] = databuff[j];
                               j++;
                           }
                       }
                       else
                       {
                           szHex = szLine.Substring(9, szLine.Length - 11); //读取有效字符：后0和1  
                           hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                           for (m = 0, j = 0; m < szHex.Length; m += 2) //两字符合并成一个16进制字节  
                           {
                               databuff[j] = (byte)Int16.Parse(szHex.Substring(m, 2), NumberStyles.HexNumber);
                               arry[hexAddLow + j] = databuff[j];
                               j++;
                           }
                       }
                   }
               }
           }

           return Config;
       }

       static void StreamToFile(Stream stream, string fileName)
       {
           // 把 Stream 转换成 byte[]
           byte[] bytes = new byte[stream.Length];
           stream.Read(bytes, 0, bytes.Length);
           // 设置当前流的位置为流的开始
           stream.Seek(0, SeekOrigin.Begin);
           // 把 byte[] 写入文件
           FileStream fs = new FileStream(fileName, FileMode.Create);
           BinaryWriter bw = new BinaryWriter(fs);
           bw.Write(bytes);
           bw.Flush();
           bw.Close();
           fs.Close();
       }
       static void WriteListToTextFile(List<string> list, string txtFile)
       {

           //创建一个文件流，用以写入或者创建一个StreamWriter

           FileStream fs = new FileStream(txtFile,
           FileMode.OpenOrCreate, FileAccess.Write);

           StreamWriter sw = new StreamWriter(fs);

           sw.Flush();

           //
           sw.BaseStream.Seek(0, SeekOrigin.Begin);

           for (int i = 0; i < list.Count; i++)
           {
               sw.WriteLine(list[i]);
           }

           sw.Flush();

           sw.Close();

           fs.Close();

       }
   
       /**
       * @brief calculate CRC16 value with previous value
       * @author Kaka.zhuo
       * @param[in] NONE no output value
       * @param[out] NONE no output value
       * @return NONE no output value
       * @note 
       */
   static    UInt16 UpdateCRC(byte value, UInt16 crc)
       {
           UInt16 tmp;

           byte index;
           tmp = crc;
           index = Convert.ToByte((value ^ Convert.ToByte(tmp & 0xFF))); //XOR the input byte with low byte of CRC register to get index of table
           tmp = Convert.ToUInt16((tmp >> 8)); //Shift CRC regsiter to right 8 bits
           tmp ^= CRC16table[index];//XOR the CRC register with the content of table
           return tmp;
       }

   static byte[] GetMainAppData(string AppFileName)
   {
       byte[] data = new byte[100];

       StreamReader sr = new StreamReader(AppFileName);
       Int64 add = 0;
       Int64 MainSectorLocation = HexConfig.AppLoction;
       Int64 MainSectorCrcLocation = HexConfig.AppCrcLoction;
       Int64 FlashSize = HexConfig.FlashSize;
       Int64 MainAppSize = HexConfig.AppSize;
       UInt16[] DefaultConfig ={
                                    0x000F,       //0
                                    0x0000,       //2
                                    0x0007,       //4
                                    0x0087,       //6
                                    0x00E7,       //8
                                    0x00df,       //a
                                    0x000F,       //c
                                    0x00C3,       //e
                                    0x00FF,       //10
                                    0x00FF};      //12

       byte[] arry = new byte[FlashSize];
       byte[] Config = new byte[100];

       Int64 i = 0;
       string szLine = "";
       string szHex = "";
       string szConfig = "";
       int data_n = 0;
       UInt16 CRC16 = 0;
       Int32 hexAddLow = 0x00;
       Int32 hexAddHigh = 0x00;
       byte dataType = 0x00;
       bool DataIsConfigFlag = false;
       Int32 ConfigAdd = HexConfig.ConfigAdd;
       int m = 0;
       Int64 j = 0;
       byte[] databuff = new byte[100];

       for (i = 0; i < arry.Length; i++)
       {
           if ((i + 1) % 4 == 0)
           {
               if (HexConfig.ChipType == "dsPIC33FJ")
               {
                   arry[i] = 0x00;
               }
               else if(HexConfig.ChipType == "PIC18F")
               {
                   arry[i] = 0xFF;
               }
           }
           else
           {
               arry[i] = 0xFF;
           }

       }

       for (i = 0; i < DefaultConfig.Length; i++)
       {
           Config[i * 4 + 0] = Convert.ToByte(DefaultConfig[i] & 0xFF);
           Config[i * 4 + 1] = Convert.ToByte((DefaultConfig[i] & 0xFF00) >> 8);
           Config[i * 4 + 2] = 0x00;
           Config[i * 4 + 3] = 0x00;

       }

  
       while (true)
       {
           szLine = sr.ReadLine(); //读取一行数据  
           if (szLine == null) //读完所有行  
           {
               break;
           }
           else if (szLine == ":00000001FF")
           {
            //   Console.WriteLine("got a end ");
               break;
              
           }
           else
           {
               data_n = Convert.ToInt16(szLine.Substring(1, 2), 16);
               //:0200000401F009
               dataType = Convert.ToByte(Convert.ToInt16(szLine.Substring(7, 2), 16) & 0xFF);

               if (dataType == 0x04)
               {
                   hexAddHigh = Convert.ToInt16(szLine.Substring(9, 4), 16);
                   if (hexAddHigh == ConfigAdd) // if it is a config address 
                   {
                       DataIsConfigFlag = true;
                   }
                   else
                   {
                       DataIsConfigFlag = false;
                   }
               }
               else if (dataType == 0x00)
               {
                   if (DataIsConfigFlag) //  if it is a Config 
                   {
                       szConfig = szLine.Substring(9, szLine.Length - 11);
                       hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                       for (m = 0, j = 0; m < szConfig.Length; m += 2) //两字符合并成一个16进制字节  
                       {
                           databuff[j] = (byte)Int16.Parse(szConfig.Substring(m, 2), NumberStyles.HexNumber);
                           Config[hexAddLow + j] = databuff[j];
                           j++;
                       }
                   }
                   else
                   {
                       szHex = szLine.Substring(9, szLine.Length - 11); //读取有效字符：后0和1  
                       hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                       for (m = 0, j = 0; m < szHex.Length; m += 2) //两字符合并成一个16进制字节  
                       {
                           databuff[j] = (byte)Int16.Parse(szHex.Substring(m, 2), NumberStyles.HexNumber);
                           arry[hexAddLow + j + hexAddHigh*0x10000] = databuff[j];
                           j++;
                       }

                   }
               }

           }

       }
       for (i = 0; i < MainAppSize - 64; i++)
       {
           CRC16 = UpdateCRC(arry[i + MainSectorLocation + 64], CRC16);
       }
       arry[MainSectorCrcLocation] = Convert.ToByte(CRC16 & 0xFF);
       arry[MainSectorCrcLocation + 1] = Convert.ToByte((CRC16 & 0xFF00) >> 8);
     //  Console.WriteLine(" ");
    //   Console.WriteLine("-------------------------------------------------------------");
       Console.WriteLine("CRC has put to App Sector ");
       string str = string.Format("{0:X4}", CRC16);

       Console.WriteLine("CRC is :" + str);
       Console.WriteLine(" ");
       return arry;
   }


   static byte[] GetBootData(string BootFileName)
   {
       byte[] data = new byte[100];

       StreamReader sr = new StreamReader(BootFileName);
       Int64 add = 0;
       Int64 BootSectorLocation = HexConfig.BootLocation; //0
       Int64 FlashSize = HexConfig.FlashSize;//10000
       Int64 BootAppSize = HexConfig.BootSize; //2800
       Int64 BootCrcLocation = HexConfig.BootCrcLocation; //1800
       UInt16[] DefaultConfig =
       {
            0X000F,       //0
            0X0000,       //2
            0X0007,       //4
            0X0087,       //6
            0X00E7,       //8
            0X00DF,       //A
            0X000F,       //C
            0X00C3,       //E
            0X00FF,       //10
            0X00FF        //12
        };

       byte[] arry = new byte[FlashSize];
       byte[] Config = new byte[100];

       Int64 i = 0;
       string szLine = "";
       string szHex = "";
       string szConfig = "";
       int data_n = 0;
       UInt16 CRC16 = 0;
       Int32 hexAddLow = 0x00;
       Int32 hexAddHigh = 0x00;
       byte dataType = 0x00;
       bool DataIsConfigFlag = false;
       Int32 ConfigAdd = HexConfig.ConfigAdd;
       int m = 0;
       Int64 j = 0;
       byte[] databuff = new byte[100];

       for (i = 0; i < arry.Length; i++)
       {
           if ((i + 1) % 4 == 0)
           {
               if (HexConfig.ChipType == "dsPIC33FJ")
               {
                   arry[i] = 0x00;
               }
               else if (HexConfig.ChipType == "PIC18F")
               {
                   arry[i] = 0xFF;
               }
           }
           else
           {
               arry[i] = 0xFF;
           }

       }

       for (i = 0; i < DefaultConfig.Length; i++)
       {
           Config[i * 4 + 0] = Convert.ToByte(DefaultConfig[i] & 0xFF);
           Config[i * 4 + 1] = Convert.ToByte((DefaultConfig[i] & 0xFF00) >> 8);
           Config[i * 4 + 2] = 0x00;
           Config[i * 4 + 3] = 0x00;

       }


       while (true)
       {
           szLine = sr.ReadLine(); //读取一行数据  
           if (szLine == null) //读完所有行  
           {
               break;
           }
           else if (szLine == ":00000001FF")
           {
            //   Console.WriteLine("got a end ");
               break;

           }
           else
           {
               data_n = Convert.ToInt16(szLine.Substring(1, 2), 16); //
               //:0200000401F009
               dataType = Convert.ToByte(Convert.ToInt16(szLine.Substring(7, 2), 16) & 0xFF); //数据类型

               if (dataType == 0x04)
               {
                   hexAddHigh = Convert.ToInt16(szLine.Substring(9, 4), 16);
                   if (hexAddHigh == ConfigAdd) // if it is a config address 
                   {
                       DataIsConfigFlag = true;
                   }
                   else
                   {
                       DataIsConfigFlag = false;
                   }
               }
               else if (dataType == 0x00)
               {
                   if (DataIsConfigFlag) //  if it is a Config 
                   {
                       szConfig = szLine.Substring(9, szLine.Length - 11);
                       hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                       for (m = 0, j = 0; m < szConfig.Length; m += 2) //两字符合并成一个16进制字节  
                       {
                           databuff[j] = (byte)Int16.Parse(szConfig.Substring(m, 2), NumberStyles.HexNumber);
                           Config[hexAddLow + j] = databuff[j];
                           j++;
                       }
                   }
                   else
                   {
                       szHex = szLine.Substring(9, szLine.Length - 11); //读取有效字符：后0和1  
                       hexAddLow = Convert.ToInt32(szLine.Substring(3, 4), 16);
                       for (m = 0, j = 0; m < szHex.Length; m += 2) //两字符合并成一个16进制字节  
                       {
                           databuff[j] = (byte)Int16.Parse(szHex.Substring(m, 2), NumberStyles.HexNumber);
                           arry[hexAddLow + j] = databuff[j];
                           j++;
                       }

                   }
               }

           }

       }

       // for boot sector ,this crc is not used for program 
       for (i = 0; i < BootAppSize; i++)
       {
           CRC16 = UpdateCRC(arry[i], CRC16);
       }
       arry[BootCrcLocation] = Convert.ToByte(CRC16 & 0xFF);
       arry[BootCrcLocation + 1] = Convert.ToByte((CRC16 & 0xFF00) >> 8);

  //     Console.WriteLine(" ");
    //   Console.WriteLine("-------------------------------------------------------------");
       Console.WriteLine("CRC has put to Boot Sector ");
       string str = string.Format("{0:X4}", CRC16);

       Console.WriteLine("CRC is :" + str);
       Console.WriteLine(" ");
       return arry;

   }
   static void GenerateBinFileByApp(byte[] app, string FileName)
   {
       Stream stm = new MemoryStream(app);
       StreamToFile(stm, FileName);
       Console.WriteLine("Succeed build File : ");
       Console.WriteLine(FileName.Substring(FileName.LastIndexOf('\\') + 1, FileName.Length - 1 - (FileName.LastIndexOf('\\')))); 
       Console.WriteLine("");
   }
   static byte GetCheckSum(byte[] data, int len)
   {
       byte checksum = 0;
       int i = 0;
       UInt64 sum = 0;


       for (i = 0; i < len; i++)
       {
           sum += data[i];
       }
       sum = 0x100 - (sum & 0xFF);

       checksum = Convert.ToByte(sum & 0xFF);
       return checksum;

   }

   static void FormatHexByBin(byte[] bin, byte[] Config, byte dataLen, string FileName)
   {

       Int64 i = 0;
       int j = 0;
       // string
       string Header = ":";
       byte DataNum = 0x10;
       byte DataType = 0x00;
       string str = "";
       string databuf = "";
       byte Checksum = 0;
       byte[] data = new byte[100];
       Int32 HexAddLow = 0;
       Int32 HexAddHigh = 0;
       List<string> sList = new List<string>();
       // generate Header and program data at first 
       for (i = 0; i < bin.Length; i = i + dataLen)
       {
           databuf = "";

           if (i == 0 || HexAddLow >= 0x10000)
           {
               if (HexAddLow >= 0x10000)
               {
                   HexAddLow = 0;
                   HexAddHigh++;
               }
               DataType = 0x04;
               DataNum = 0x02;
               data[0] = DataNum;
               data[1] = 0x00;   //Const zero 
               data[2] = 0x00;   //Const zero 
               data[3] = DataType;
               data[4] = Convert.ToByte((HexAddHigh & 0xFF00) >> 8);
               data[5] = Convert.ToByte((HexAddHigh & 0xFF));
               Checksum = GetCheckSum(data, DataNum + 4);

               for (j = 0; j < DataNum + 4; j++)
               {
                   databuf += string.Format("{0:X2}", data[j]);
               }
               str = Header + databuf + string.Format("{0:X2}", Checksum);
               sList.Add(str);
               databuf = "";

           }

           for (j = 0; j < dataLen && ((i + j) < bin.Length); j++)
           {
               DataNum = dataLen;
               DataType = 0;
               data[0] = DataNum;
               data[1] = Convert.ToByte((HexAddLow & 0xFF00) >> 8);
               data[2] = Convert.ToByte((HexAddLow & 0xFF));
               data[3] = DataType;
               data[j + 4] = bin[i + j];
               databuf += string.Format("{0:X2}", data[j + 4]);
           }
           Checksum = GetCheckSum(data, DataNum + 4);

           str = Header + string.Format("{0:X2}", dataLen) + string.Format("{0:X4}", HexAddLow) + string.Format("{0:X2}", DataType) + databuf + string.Format("{0:X2}", Checksum);
           sList.Add(str);
           HexAddLow += dataLen;
       }
       // generate the configration bits 

       HexAddHigh = HexConfig.ConfigAdd;
       HexAddLow = 0;
       int sizeConfig = HexConfig.ConfigSize;
       for (i = 0; i < sizeConfig; i = i + dataLen)
       {
           databuf = "";

           if (i == 0 || HexAddLow >= 0x10000)
           {
               if (HexAddLow >= 0x10000)
               {
                   HexAddLow = 0;
                   HexAddHigh++;
               }
               DataType = 0x04;
               DataNum = 0x02;
               data[0] = DataNum;
               data[1] = 0x00;   //Const zero 
               data[2] = 0x00;   //Const zero 
               data[3] = DataType;
               data[4] = Convert.ToByte((HexAddHigh & 0xFF00) >> 8);
               data[5] = Convert.ToByte((HexAddHigh & 0xFF));
               Checksum = GetCheckSum(data, DataNum + 4);

               for (j = 0; j < DataNum + 4; j++)
               {
                   databuf += string.Format("{0:X2}", data[j]);
               }
               str = Header + databuf + string.Format("{0:X2}", Checksum);
               sList.Add(str);
               databuf = "";
           }

           for (j = 0; j < dataLen && (i + j) < sizeConfig; j++)
           {
               DataNum = dataLen;
               DataType = 0;
               data[0] = DataNum;
               data[1] = Convert.ToByte((HexAddLow & 0xFF00) >> 8);
               data[2] = Convert.ToByte((HexAddLow & 0xFF));
               data[3] = DataType;
               data[j + 4] = Config[i + j];
               databuf += string.Format("{0:X2}", data[j + 4]);
           }
           data[0] = Convert.ToByte(j & 0xFF);
           Checksum = GetCheckSum(data, j + 4);

           str = Header + string.Format("{0:X2}", j) + string.Format("{0:X4}", HexAddLow) + string.Format("{0:X2}", DataType) + databuf + string.Format("{0:X2}", Checksum);
           sList.Add(str);
           HexAddLow += dataLen;
       }
       str = ":00000001FF";
       sList.Add(str);

       WriteListToTextFile(sList, FileName);

       Console.WriteLine("Succeed build File : " );//+ );
       Console.WriteLine(FileName.Substring(FileName.LastIndexOf('\\') + 1, FileName.Length - 1 - (FileName.LastIndexOf('\\')))); 
       Console.WriteLine("");
   }


        /*
          This tool is design for Combine H3C hex file to a combine file .
         
         */
        static void Main(string[] args)
        {
            string file = System.Environment.CurrentDirectory + "\\FileConverter.ini";

         //   Version ApplicationVersion = new Version(Assembly.ProductVersion);

          
            string ss = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           
            Console.WriteLine("Program Build Version : " + ss);
      
            if (!File.Exists(file))
            {

                Console.WriteLine("Connot Find Right Config File !!! ");
                Console.ReadLine();
                return;
            }
            ReadParamererFromConfigFile(file); //读取INI配置

            if (!File.Exists(HexConfig.BootImportName)) // 查找hex文件
            {
                Console.WriteLine("Connot Find Boot Import File !!! ");
                Console.ReadLine();
                return;
            }
            byte[] boot = GetBootData(HexConfig.BootImportName); //GetMainAppData(HexConfig.BootImportName);//
            if (!File.Exists(HexConfig.AppImportName))
            {

                Console.WriteLine("Connot Find App Import File !!! ");
                Console.ReadLine();
                return;
            }
            byte[] App = GetMainAppData(HexConfig.AppImportName);
            byte[] Bin = new byte[HexConfig.AppSize];
            byte[] Config = GetConfigBitsFromBoot(HexConfig.BootImportName);
            Int64 i = 0;
            Int64 j = 0;
            byte[] Combine = new byte[HexConfig.FlashSize];
            // sec bin header 
            byte[] SecBinHeader = new byte[64];
            int m = 0;

            /*
              Transmit the boot code to combine arry 
             */
            for (i = HexConfig.BootLocation, j = 0; i < HexConfig.BootSize; i++)
            {
                Combine[i] = boot[i];
            }
            /*
             Transmit the App code to combine arry 
            */
            for (i = HexConfig.AppLoction; i < HexConfig.FlashSize; i++)
            {
                Combine[i] = App[i];
            }

            for (i = 0; i < HexConfig.AppSize; i++)
            {
                Bin[i] = App[i + HexConfig.AppLoction];
            }

          /*
            For secondary DSP ,in order to get a fast reading speed and check the header info
           * Compress the 1st 64 byte to 32 byte ,remove all the 0 ,just fetch the usefull datas
           
           */
            if (HexConfig.ChipLoction == "SEC" && HexConfig.ChipType == "dsPIC33FJ")
            {
                m = 0;
                SecBinHeader[m++] = App[ HexConfig.AppCrcLoction];
                SecBinHeader[m++] = App[HexConfig.AppCrcLoction+1];
                for (i = 4; i < 64; i = i + 4)
                {
                    SecBinHeader[m++] = Bin[i];
                    SecBinHeader[m++] = Bin[i + 1];
                }
                for (i = 0; i < 64; i++)
                {
                    Bin[i] = SecBinHeader[i];
                }
            }

            FormatHexByBin(boot, Config, 16, HexConfig.BootExportName);
            FormatHexByBin(App, Config, 16, HexConfig.AppExportName);
            FormatHexByBin(Combine, Config, 16, HexConfig.CombineFileName);
            GenerateBinFileByApp(Bin, HexConfig.BinFileName);
          
            Console.WriteLine("Process Finished!");
            Console.WriteLine("Copyright @ ASPOWER Firmware Team 2018.");
            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }
    }
}
