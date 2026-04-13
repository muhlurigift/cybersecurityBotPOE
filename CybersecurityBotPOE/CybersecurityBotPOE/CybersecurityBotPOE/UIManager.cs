using System;
using System.Threading;
using System.Threading.Tasks;

namespace CybersecurityBotPOE
{
    public class UIManager
    {
        public void DisplayAsciiArt()
        {
            string asciiArt = @"
    +================================================================================+
    |                                                                                |
    |      ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗ ██████╗██╗   ██╗        |
    |     ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝╚██╗ ██╔╝        |
    |     ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗██║      ╚████╔╝         |
    |     ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██║       ╚██╔╝          |
    |     ╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║╚██████╗   ██║           |
    |      ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝ ╚═════╝   ╚═╝           |
    |                                                                                |
    |                     █████╗ ██╗    ██╗ █████╗ ██████╗ ███████╗                 |
    |                    ██╔══██╗██║    ██║██╔══██╗██╔══██╗██╔════╝                 |
    |                    ███████║██║ █╗ ██║███████║██████╔╝███████╗                 |
    |                    ██╔══██║██║███╗██║██╔══██║██╔══██╗╚════██║                 |
    |                    ██║  ██║╚███╔███╔╝██║  ██║██║  ██║███████║                 |
    |                    ╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝                 |
    |                                                                                |
    |                 [ C Y B E R S E C U R I T Y   A W A R E N E S S ]             |
    |                                                                                |
    +================================================================================+
            ";

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(asciiArt);
            Console.ResetColor();
        }

        public void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("   CYBERSECURITY AWARENESS BOT - Your Online Safety Companion");
            Console.WriteLine(new string('=', 80));
            Console.ResetColor();
        }

        public void DisplayPersonalizedWelcome(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nWelcome, {userName}!");
            Console.ResetColor();

            Console.WriteLine("\nI'm here to help you stay safe online!");
            Console.WriteLine("You can ask me about:");
            Console.WriteLine("  [1] Password Safety");
            Console.WriteLine("  [2] Phishing Attacks");
            Console.WriteLine("  [3] Safe Browsing");
            Console.WriteLine("  [4] General Cybersecurity Tips");
            Console.WriteLine("\nType 'help' to see all available commands, or 'exit' to quit.");
        }

        public void DisplayDivider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n" + new string('-', 80));
            Console.ResetColor();
        }

        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] {message}");
            Console.ResetColor();
        }

        public void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCCESS] {message}");
            Console.ResetColor();
        }

        public void DisplayInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[INFO] {message}");
            Console.ResetColor();
        }

        public void DisplayGoodbye(string userName)
        {
            DisplayDivider();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nGoodbye, {userName}! Stay safe online!");
            Console.WriteLine("   Remember: Think before you click!");
            Console.ResetColor();
        }

        public async Task TypewriterEffect(string text, int delayMs = 30)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n[Bot] > ");

            foreach (char c in text)
            {
                Console.Write(c);
                await Task.Delay(delayMs);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public void DisplayColoredText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}