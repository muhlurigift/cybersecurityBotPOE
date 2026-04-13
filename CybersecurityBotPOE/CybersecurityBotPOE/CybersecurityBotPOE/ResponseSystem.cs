using System;
using System.Collections.Generic;

namespace CybersecurityBotPOE
{
    public class ResponseSystem
    {
        private readonly Dictionary<string, string> _responses;

        public ResponseSystem()
        {
            _responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Basic questions
                { "how are you", "I'm functioning perfectly! Thanks for asking. Ready to help you stay safe online." },
                { "what's your purpose", "My purpose is to educate and assist you with cybersecurity best practices, helping you stay safe from online threats like phishing, weak passwords, and unsafe browsing habits." },
                { "what can i ask you about", "You can ask me about:\n- Password safety tips\n- How to identify phishing emails\n- Safe browsing practices\n- General cybersecurity advice\n- How to create strong passwords\n- What to do if you suspect a scam" },
                
                // Password safety
                { "password", "Strong passwords should be at least 12 characters long and include uppercase, lowercase, numbers, and symbols. Never reuse passwords across different sites!" },
                { "password safety", "Use a password manager, enable two-factor authentication whenever possible, and avoid using personal information in your passwords." },
                
                // Phishing
                { "phishing", "Phishing is when attackers try to trick you into revealing sensitive information via fake emails, messages, or websites. Always verify the sender and never click suspicious links!" },
                { "phishing email", "Signs of a phishing email: urgent language, spelling errors, suspicious links, requests for personal info, and mismatched email addresses." },
                
                // Safe browsing
                { "browsing", "Safe browsing tips: Use HTTPS websites, keep your browser updated, avoid clicking pop-ups, and use ad-blockers for extra protection." },
                { "safe browsing", "Always check for the padlock icon in the address bar, don't download files from untrusted sources, and clear your cache regularly." },
                
                // General tips
                { "tips", "Top cybersecurity tips:\n1. Use unique passwords for every account\n2. Enable two-factor authentication\n3. Keep software updated\n4. Think before you click\n5. Use a VPN on public Wi-Fi" },
                { "security", "Cybersecurity is about protecting your digital life. Always stay vigilant and question anything that seems suspicious!" },
                
                // Help command
                { "help", "Available commands:\n- 'how are you' - Check on me\n- 'what's your purpose' - Learn why I exist\n- 'what can i ask you about' - See topics\n- 'password' - Password safety tips\n- 'phishing' - Phishing information\n- 'browsing' - Safe browsing tips\n- 'tips' - General cybersecurity tips\n- 'exit' - End the conversation" }
            };
        }

        public string GetResponse(string userInput)
        {
            // Check for exit command
            if (userInput == "exit" || userInput == "quit" || userInput == "goodbye")
            {
                return "exit";
            }

            // Check for exact matches in dictionary
            foreach (var key in _responses.Keys)
            {
                if (userInput.Contains(key.ToLower()))
                {
                    return _responses[key];
                }
            }

            // Default response for unrecognized input
            return "I didn't quite understand that. Could you rephrase? Try asking about 'password', 'phishing', 'browsing', or type 'help' for all options.";
        }
    }
}