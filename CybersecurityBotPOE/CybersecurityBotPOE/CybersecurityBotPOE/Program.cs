using System;
using System.Threading.Tasks;

namespace CybersecurityBotPOE
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";
            Console.SetWindowSize(100, 40);

            var chatbot = new Chatbot();
            await chatbot.StartAsync();
        }
    }
}