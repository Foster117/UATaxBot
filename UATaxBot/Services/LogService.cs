using System;
using System.Collections.Generic;
using System.Text;

namespace UATaxBot.Services
{
    static class LogService
    {
        private static bool ColorFlag { get; set; } = true;
        public static void PrintLogText(string name, string text)
        {
            Console.ForegroundColor = (ColorFlag) ? ConsoleColor.Gray : ConsoleColor.DarkGray;
            Console.WriteLine("{0, -60}{1}", $"{name} {text}", DateTime.Now);
            Console.ResetColor();
            ColorFlag = !ColorFlag;
        }
    }
}
