using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class HTTPGETFinder
    {
        private static int state;

        private static bool Iswp(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z') || (symbol >= '0' && symbol <= '9') || (symbol == '.'))
                return true;
            else
                return false;
        }
        private static bool Isw(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z') || (symbol >= '0' && symbol <= '9'))
                return true;
            else
                return false;
        }
        public static bool CheckString(string input)
        {
            state = 0;
            foreach(char symbol in input)
            {
                switch (state)
                {
                    case 0: { if (symbol == 'h') state = 1; else return false; break; }
                    case 1: { if (symbol == 't') state = 2; else return false; break; }
                    case 2: { if (symbol == 't') state = 3; else return false; break; }
                    case 3: { if (symbol == 'p') state = 4; else return false; break; }
                    case 4: { if (symbol == 's') state = 6; else if (symbol == ':') state = 5; else return false; break; }
                    case 5: { if (symbol == '/') state = 7; else return false; break; }
                    case 6: { if (symbol == ':') state = 5; else return false; break; }
                    case 7: { if (symbol == '/') state = 8; else return false; break; }
                    case 8: { if (Iswp(symbol)) state = 9; else return false; break; }
                    case 9: { if (Iswp(symbol)) state = 9; else if (symbol == '/') state = 10; else return false; break; }
                    case 10: { if (Iswp(symbol)) state = 11; else if (symbol == '?') state = 12; else return false; break; }
                    case 11: { if (Iswp(symbol)) state = 11; else if (symbol == '/') state = 10; else return false; break; }
                    case 12: { if (Isw(symbol)) state = 13; else return false; break; }
                    case 13: { if (Isw(symbol)) state = 13; else if (symbol == '=') state = 14; else return false; break; }
                    case 14: { if (Isw(symbol)) state = 15; else return false; break; }
                    case 15: { if (Isw(symbol)) state = 15; else return false; break; }
                    default:
                        return false;
                }
            }
            if (state == 15)
                return true;
            else
                return false;
        }
    }
}
