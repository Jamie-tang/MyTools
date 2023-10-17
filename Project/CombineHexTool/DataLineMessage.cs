using System;
using System.Collections.Generic;
using System.Text;

namespace CombineHexTool
{
    public class DataLineMessage
    {
        public byte length;
        public UInt32 LowAddr;
        public byte type;
        public UInt32 HighAddr;//数据域
        public byte checksum;
        public UInt32 HexStartAddr;
    }
}
