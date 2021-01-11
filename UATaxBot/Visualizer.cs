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
            Console.WriteLine("                                              << UATaxBot version 0.3 >>".ToUpper());
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
        public static void DrawLogText(string text)
        {
            Console.ForegroundColor = (colorFlag) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.ResetColor();
            colorFlag = !colorFlag;
        }
    }
}
