using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace UATaxBot
{
    class Visualizer
    {
        private static bool colorFlag = true;
        public static void DrawStartText(User me)
        {
            Console.Title = me.Username;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 118));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("                                              << UATaxBot version 1.0 >>".ToUpper());
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 118));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"                                  Start listening for @{me.Username} at {DateTime.Now}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 118));
            Console.ResetColor();
        }
        public static void DrawLogText(string name, string text)
        {



            Console.ForegroundColor = (colorFlag) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            Console.WriteLine("{0, -60}{1}", $"{name} {text}", DateTime.Now);
            Console.ResetColor();
            colorFlag = !colorFlag;
        }
    }
}
