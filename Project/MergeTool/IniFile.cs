using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MergeTool
{
    public class IniFile
    {
        public string szSourceFile1Name;
        public string szSourceFile1StartAddr;
        public string szSourceFile1EndAddr;
        public string szSourceFile2Name;
        public string szSourceFile2StartAddr;
        public string szSourceFile2EndAddr;
        public string szCombineFileName;
        public string szCombineFileStartAddr;
        public string szCombineFileEndAddr;
        public string szCombineFileCheckSumAddr;
        public string szCombineFileLineCharNum;
        public string szBinFileStartAddr;
        public string szBinFileEndAddr;
        public string szBinFileCheckSumAddr;
        public string szBinFileLineCharNum;
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
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


        public void ReadIniFlie(string iniFile)
        {
            string section = "Config";
            szSourceFile1Name = IniReadKey(iniFile, section, "SourceFile1Name");
            szSourceFile1StartAddr = IniReadKey(iniFile, section, "SourceFile1StartAddr");
            szSourceFile1EndAddr = IniReadKey(iniFile, section, "SourceFile1EndAddr");
            szSourceFile2Name = IniReadKey(iniFile, section, "SourceFile2Name");
            szSourceFile2StartAddr = IniReadKey(iniFile, section, "SourceFile2StartAddr");
            szSourceFile2EndAddr = IniReadKey(iniFile, section, "SourceFile2EndAddr");
            szCombineFileName = IniReadKey(iniFile, section, "CombineFileName");
            szCombineFileStartAddr = IniReadKey(iniFile, section, "CombineFileStartAddr");
            szCombineFileEndAddr = IniReadKey(iniFile, section, "CombineFileEndAddr");
            szCombineFileCheckSumAddr = IniReadKey(iniFile, section, "CombineFileCheckSumAddr");
            szCombineFileLineCharNum = IniReadKey(iniFile, section, "CombineFileLineCharNum");
            szBinFileStartAddr = IniReadKey(iniFile, section, "BinFileStartAddr");
            szBinFileEndAddr = IniReadKey(iniFile, section, "BinFileEndAddr");
            szBinFileCheckSumAddr = IniReadKey(iniFile, section, "BinFileCheckSumAddr");
            szBinFileLineCharNum = IniReadKey(iniFile, section, "BinFileLineCharNum");
        }

        public void WriteIniFile(string iniFile)
        {
            string section = "Config";
            IniWriteKey(iniFile, section, "SourceFile1Name", szSourceFile1Name);
            IniWriteKey(iniFile, section, "SourceFile1StartAddr", szSourceFile1StartAddr);
            IniWriteKey(iniFile, section, "SourceFile1EndAddr", szSourceFile1EndAddr);
            IniWriteKey(iniFile, section, "SourceFile2Name", szSourceFile2Name);
            IniWriteKey(iniFile, section, "SourceFile2StartAddr", szSourceFile2StartAddr);
            IniWriteKey(iniFile, section, "SourceFile2EndAddr", szSourceFile2EndAddr);
            IniWriteKey(iniFile, section, "CombineFileName", szCombineFileName);
            IniWriteKey(iniFile, section, "CombineFileStartAddr", szCombineFileStartAddr);
            IniWriteKey(iniFile, section, "CombineFileEndAddr", szCombineFileEndAddr);
            IniWriteKey(iniFile, section, "CombineFileCheckSumAddr", szCombineFileCheckSumAddr);
            IniWriteKey(iniFile, section, "CombineFileLineCharNum", szCombineFileLineCharNum);
            IniWriteKey(iniFile, section, "BinFileStartAddr", szBinFileStartAddr);
            IniWriteKey(iniFile, section, "BinFileEndAddr", szBinFileEndAddr);
            IniWriteKey(iniFile, section, "BinFileCheckSumAddr", szBinFileCheckSumAddr);
            IniWriteKey(iniFile, section, "BinFileLineCharNum", szBinFileLineCharNum);
        }
        
        public string IniReadKey(string iniFile, string Section, string key)//read from ini file
        {

            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, key, "", temp, 1024, iniFile);
            return temp.ToString();
        }
        public void IniWriteKey(string iniFile, string Section, string key, string val)//read from ini file
        {
            WritePrivateProfileString(Section, key, val, iniFile);
        }
    }
}
