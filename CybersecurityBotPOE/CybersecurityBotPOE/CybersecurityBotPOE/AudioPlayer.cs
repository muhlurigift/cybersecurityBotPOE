using System;
using System.IO;
using System.Media;

namespace CybersecurityBotPOE  // Make sure this matches your project namespace
{
    public class AudioPlayer
    {
        public void PlayGreeting()
        {
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

            if (File.Exists(audioPath))
            {
                try
                {
                    using (var soundPlayer = new SoundPlayer(audioPath))
                    {
                        soundPlayer.PlaySync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Audio Error] Could not play greeting: {ex.Message}");
                    PlayFallbackGreeting();
                }
            }
            else
            {
                PlayFallbackGreeting();
            }
        }

        private void PlayFallbackGreeting()
        {
            // Play a simple notification sound as alternative to WAV file
            Console.Beep(800, 200);
            Console.Beep(1000, 200);
            Console.Beep(1200, 400);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[Voice Greeting] Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.");
            Console.ResetColor();
        }
    }
}