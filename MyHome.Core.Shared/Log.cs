using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHome.Shared
{
    public class Log
    {
        public static bool ConsoleOut { get; set; }

        public static void L(string str, params object[] param)
        {
            if (ConsoleOut)
            {
                Console.WriteLine(String.Format("[{0}] {1}", DateTime.Now, String.Format(str, param)));
            }
            else
            {
                Debug.WriteLine(String.Format("[{0}] {1}", DateTime.Now, String.Format(str, param)));
            }
        }
    }
}
