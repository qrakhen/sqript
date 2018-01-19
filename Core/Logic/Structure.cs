using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Struqture
    {
        public const string
            MEMBER = ":";
        public const int
            OPEN = 0x0,
            CLOSE = 0x1,
            DELIMITER = 0x2;

        public static readonly string[] Context = new string[3] { "{", "}", "," };
        public static readonly string[] Funqtion = new string[3] { "~(", ")", " " };
        public static readonly string[] Call = new string[3] { "(", ")", "," };
        public static readonly string[] Collection = new string[3] { "[", "]", "," };
        public static readonly string[] TypeWrapper = new string[3] { "<", ">", "," };
    }
}
