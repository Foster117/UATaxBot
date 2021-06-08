using System;
using Telegram.Bot.Types;

namespace UATaxBot
{
    class Visualizer
    {

        public static void PrintStartText(User me)
        {
            Console.Title = me.Username;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 118));
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("                                              << UATaxBot version 1.1 >>".ToUpper());
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

        public static void DrawErrorMessage(int errorId, string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error code - {errorId} {errorMessage}");
            Console.ResetColor();
        }
    }
}
