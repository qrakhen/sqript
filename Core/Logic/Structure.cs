using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Struqture
    {
        public const int
            OPEN = 0x0,
            CLOSE = 0x1,
            DELIMITER = 0x2;

        public static readonly string[] QONT = new string[3] { "{", "}", ":" };
        public static readonly string[] FUNQ = new string[3] { "~(", ")", " " };
        public static readonly string[] QALL = new string[3] { "(", ")", "," };
        public static readonly string[] QOLL = new string[3] { "[", "]", "," };
        public static readonly string[] TYPE = new string[3] { "<", ">", "," };
    }
}
