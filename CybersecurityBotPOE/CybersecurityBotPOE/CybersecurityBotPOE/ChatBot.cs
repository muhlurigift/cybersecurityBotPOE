using System;
using System.Threading.Tasks;

namespace CybersecurityBotPOE
{
    public class Chatbot
    {
        private readonly UIManager _uiManager;
        private readonly ResponseSystem _responseSystem;
        private readonly AudioPlayer _audioPlayer;
        private string _userName;

        public Chatbot()
        {
            _uiManager = new UIManager();
            _responseSystem = new ResponseSystem();
            _audioPlayer = new AudioPlayer();
        }

        public async Task StartAsync()
        {
            // Play voice greeting
            _audioPlayer.PlayGreeting();

            // Display ASCII art and welcome screen
            _uiManager.DisplayAsciiArt();
            _uiManager.DisplayHeader();

            // Get user name with validation
            _userName = GetValidatedName();

            // Personalized welcome
            _uiManager.DisplayPersonalizedWelcome(_userName);

            // Main interaction loop
            await RunChatLoop();
        }

        private string GetValidatedName()
        {
            while (true)
            {
                _uiManager.DisplayColoredText("\n[?] What's your name? ", ConsoleColor.Cyan);
                string name = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(name) && name.Length >= 2 && name.Length <= 30)
                {
                    return name;
                }
                else if (string.IsNullOrEmpty(name))
                {
                    _uiManager.DisplayError("I didn't catch that. Please enter a valid name (2-30 characters).");
                }
                else if (name.Length < 2)
                {
                    _uiManager.DisplayError("Name must be at least 2 characters long.");
                }
                else
                {
                    _uiManager.DisplayError("Name is too long. Please use 30 characters or less.");
                }
            }
        }

        private async Task RunChatLoop()
        {
            bool running = true;

            while (running)
            {
                _uiManager.DisplayDivider();
                _uiManager.DisplayColoredText($"\n[{_userName}] > ", ConsoleColor.Green);
                string userInput = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(userInput))
                {
                    _uiManager.DisplayError("I didn't quite understand that. Could you rephrase?");
                    continue;
                }

                string response = _responseSystem.GetResponse(userInput);

                if (response.ToLower() == "exit")
                {
                    running = false;
                    _uiManager.DisplayGoodbye(_userName);
                }
                else
                {
                    await _uiManager.TypewriterEffect(response, 30);
                }
            }
        }
    }
}