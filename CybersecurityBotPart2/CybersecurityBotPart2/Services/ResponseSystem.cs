using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotGUI.Services
{
    /// <summary>
    /// Core response engine for the chatbot.
    /// Handles keyword recognition, random multi-responses, follow-up conversation flow,
    /// memory recall, and default fallback — all Part 2 requirements.
    /// </summary>
    public class ResponseSystem
    {
        private readonly Random _rng = new Random();

        // ── Keyword → single deterministic response ──────────────────────────────
        // Used for topics where one definitive answer is best.
        private readonly Dictionary<string, string> _keywordResponses =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // --- General bot queries ---
            ["how are you"] =
                "I'm running at 100% efficiency and ready to help! How can I assist you with cybersecurity today?",

            ["what is your purpose"] =
                "My purpose is to educate South African citizens about cybersecurity threats and how to stay safe online. " +
                "I can help you understand phishing, password safety, privacy, malware, scams, and safe browsing!",

            ["what can i ask"] =
                "You can ask me about:\n• Password safety\n• Phishing attacks\n• Privacy settings\n• Malware & viruses\n• Scam detection\n• Safe browsing\n• Two-factor authentication (2FA)\n• VPNs\n• Social engineering\n\nType 'help' for quick commands.",

            ["help"] =
                "📋 Available commands:\n" +
                "  • 'password'      – Password safety tips\n" +
                "  • 'phishing'      – Phishing awareness\n" +
                "  • 'privacy'       – Privacy best practices\n" +
                "  • 'malware'       – Malware & virus info\n" +
                "  • 'scam'          – Scam detection tips\n" +
                "  • 'browsing'      – Safe browsing\n" +
                "  • '2fa'           – Two-factor authentication\n" +
                "  • 'vpn'           – VPN usage\n" +
                "  • 'social engineering' – Social engineering threats\n" +
                "  • 'quiz'          – Start a quick quiz\n" +
                "  • 'show memory'   – See what I remember about you\n" +
                "  • 'clear'         – Clear the chat\n" +
                "  • 'goodbye'       – End the session",

            // --- 2FA ---
            ["2fa"] =
                "🔐 Two-Factor Authentication (2FA) adds a second layer of security to your accounts. " +
                "Even if your password is stolen, the attacker still cannot log in without the second factor. " +
                "Enable 2FA on email, banking, and social media accounts immediately!",

            ["two-factor"] =
                "🔐 Two-factor authentication (2FA) requires something you know (password) AND something you have " +
                "(a phone or hardware key). Use apps like Google Authenticator or Microsoft Authenticator for the best protection.",

            // --- VPN ---
            ["vpn"] =
                "🌐 A VPN (Virtual Private Network) encrypts your internet traffic, hiding it from ISPs, hackers, " +
                "and snoopers on public Wi-Fi. Always use a reputable VPN when connecting at coffee shops, airports, or hotels.",

            // --- Safe browsing ---
            ["browsing"] =
                "🌍 Safe Browsing Tips:\n" +
                "• Look for HTTPS (padlock icon) before entering personal info\n" +
                "• Keep your browser updated — patches fix security holes\n" +
                "• Use a browser extension like uBlock Origin to block malicious ads\n" +
                "• Avoid downloading software from unofficial sources\n" +
                "• Clear cookies and cache regularly to reduce tracking",

            // --- Social engineering ---
            ["social engineering"] =
                "🎭 Social engineering is when attackers manipulate people psychologically to reveal confidential info. " +
                "Common tactics include pretexting (inventing a scenario), baiting (fake prizes), and vishing (phone scams). " +
                "Always verify the identity of anyone requesting sensitive information!",

            // --- Privacy ---
            ["privacy"] =
                "🔒 Privacy Best Practices:\n" +
                "• Review app permissions regularly — revoke anything unnecessary\n" +
                "• Use privacy-focused browsers like Firefox or Brave\n" +
                "• Limit what personal info you share on social media\n" +
                "• Enable full-disk encryption on your devices\n" +
                "• Opt out of data-sharing wherever possible",
        };

        // ── Keyword → multiple random responses ──────────────────────────────────
        // For topics like phishing and scams the bot rotates through tips.
        private readonly Dictionary<string, List<string>> _randomResponses =
            new(StringComparer.OrdinalIgnoreCase)
        {
            ["phishing"] = new List<string>
            {
                "🎣 Phishing Tip: Legitimate organisations will NEVER ask for your password via email. " +
                "If you receive such a request, report it immediately!",

                "🎣 Phishing Tip: Always hover over links before clicking — the actual URL often reveals the deception. " +
                "Look for misspellings like 'paypa1.com' instead of 'paypal.com'.",

                "🎣 Phishing Tip: Urgency is a red flag! Messages like 'Your account will be closed in 24 hours!' " +
                "are designed to panic you into acting without thinking.",

                "🎣 Phishing Tip: Check the sender's email address carefully. " +
                "'support@paypal-secure.com' is NOT PayPal — the real domain would be '@paypal.com'.",

                "🎣 Phishing Tip: When in doubt, go directly to the website by typing the URL yourself " +
                "instead of clicking any link in an email or SMS."
            },

            ["password"] = new List<string>
            {
                "🔑 Password Tip: Use a passphrase — a string of four or more random words like " +
                "'correct-horse-battery-staple'. It's long, easy to remember, and very hard to crack!",

                "🔑 Password Tip: Never reuse passwords. If one site is breached, attackers try the same " +
                "credentials everywhere. A password manager like Bitwarden stores unique passwords for every account.",

                "🔑 Password Tip: Avoid obvious substitutions like '@' for 'a' or '3' for 'e'. " +
                "Modern crackers know every trick. Aim for true randomness with 12+ characters.",

                "🔑 Password Tip: Change your passwords immediately if you see a data-breach notification. " +
                "Check haveibeenpwned.com to see if your email has been exposed.",

                "🔑 Password Tip: Don't use personal information in passwords — names, birthdays, " +
                "and phone numbers can all be found on social media."
            },

            ["scam"] = new List<string>
            {
                "⚠️ Scam Alert: 'You've won a prize!' messages are almost always scams. " +
                "Legitimate competitions don't ask for payment to claim winnings.",

                "⚠️ Scam Alert: Romance scammers build emotional relationships online and then request money for " +
                "'emergencies'. Never send money to someone you haven't met in person.",

                "⚠️ Scam Alert: Tech support scammers call pretending to be Microsoft or your bank. " +
                "Real companies will NEVER call you unsolicited to fix your computer.",

                "⚠️ Scam Alert: In South Africa, SIM-swap fraud is common. Criminals port your number to a " +
                "new SIM to intercept OTPs. Contact your network immediately if your phone loses signal unexpectedly.",

                "⚠️ Scam Alert: Invoice fraud targets businesses — attackers intercept legitimate invoices and " +
                "change the bank account details. Always verify payment details via a known phone number."
            },

            ["malware"] = new List<string>
            {
                "🦠 Malware Tip: Keep your operating system and antivirus software updated. " +
                "Most successful malware exploits vulnerabilities that were already patched!",

                "🦠 Malware Tip: Never open email attachments from unknown senders — even '.docx' and '.pdf' " +
                "files can carry malicious macros.",

                "🦠 Malware Tip: Ransomware encrypts your files and demands payment. Your best defence is regular " +
                "offline backups — the 3-2-1 rule: 3 copies, 2 different media, 1 offsite.",

                "🦠 Malware Tip: Pirated software is a common malware delivery vehicle. The 'free' version often " +
                "includes a hidden keylogger or trojan."
            }
        };

        // ── Follow-up triggers ────────────────────────────────────────────────────
        private readonly List<string> _followUpTriggers = new()
        {
            "tell me more", "more", "explain more", "another tip", "give me another",
            "go on", "continue", "what else", "more info", "another one"
        };

        // ── Exit triggers ─────────────────────────────────────────────────────────
        private readonly List<string> _exitTriggers = new()
        {
            "exit", "quit", "goodbye", "bye", "see you", "farewell", "close"
        };

        // ── Topic interest triggers (for memory recall) ───────────────────────────
        private readonly Dictionary<string, string> _topicInterestTriggers =
            new(StringComparer.OrdinalIgnoreCase)
        {
            ["i'm interested in privacy"]   = "privacy",
            ["interested in phishing"]      = "phishing",
            ["interested in passwords"]     = "password",
            ["interested in malware"]       = "malware",
            ["interested in scams"]         = "scam",
            ["i care about privacy"]        = "privacy",
            ["i care about passwords"]      = "password",
            ["i want to learn about"]       = "" // handled separately
        };

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Processes raw user input and returns an appropriate bot response.
        /// Also updates the lastTopic tracker so follow-ups work correctly.
        /// </summary>
        public string GetResponse(string input, string? lastTopic, out string? newTopic)
        {
            newTopic = lastTopic; // default: keep previous topic
            string lower = input.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(lower))
                return "I didn't catch that — could you rephrase?";

            // 1. Exit check
            if (_exitTriggers.Any(t => lower.Contains(t)))
                return "__EXIT__";

            // 2. Follow-up / "tell me more"
            if (_followUpTriggers.Any(t => lower.Contains(t)))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return GetRandomForTopic(lastTopic) ??
                           GetDeterministicForTopic(lastTopic) ??
                           "I've shared everything I know about that topic. Try asking something else!";
                return "What topic would you like to explore further? You can ask about phishing, passwords, scams, malware, privacy, and more!";
            }

            // 3. Memory-interest trigger ("I'm interested in X")
            string? rememberedTopic = ExtractTopicInterest(lower);
            if (rememberedTopic != null)
            {
                newTopic = rememberedTopic;
                return $"Great! I'll remember that you're interested in {rememberedTopic}. It's a crucial part of staying safe online. " +
                       $"Here's a tip to start:\n\n{GetFirstTipForTopic(rememberedTopic)}";
            }

            // 4. Random-response keywords (phishing, scam, malware, password when used as tip request)
            foreach (var key in _randomResponses.Keys)
            {
                if (lower.Contains(key))
                {
                    newTopic = key;
                    return GetRandomForTopic(key)!;
                }
            }

            // 5. Deterministic keyword responses
            foreach (var key in _keywordResponses.Keys)
            {
                if (lower.Contains(key))
                {
                    newTopic = key;
                    return _keywordResponses[key];
                }
            }

            // 6. Default fallback
            return "I'm not sure I understand. Could you try rephrasing? " +
                   "You can ask about topics like 'phishing', 'password', 'scam', 'malware', 'privacy', or type 'help'.";
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private string? GetRandomForTopic(string topic)
        {
            if (_randomResponses.TryGetValue(topic, out var list) && list.Count > 0)
                return list[_rng.Next(list.Count)];
            return null;
        }

        private string? GetDeterministicForTopic(string topic)
        {
            if (_keywordResponses.TryGetValue(topic, out var response))
                return response;
            return null;
        }

        private string GetFirstTipForTopic(string topic)
        {
            if (_randomResponses.TryGetValue(topic, out var list) && list.Count > 0)
                return list[0];
            if (_keywordResponses.TryGetValue(topic, out var single))
                return single;
            return $"I have a lot to share about {topic}. Just ask and I'll keep the tips coming!";
        }

        private string? ExtractTopicInterest(string lower)
        {
            // Detect "I'm interested in X" or "I want to learn about X"
            string[] interestPhrases = { "i'm interested in", "i am interested in", "interested in", "i care about", "i want to learn about" };
            string[] topics = { "phishing", "password", "passwords", "scam", "scams", "malware", "privacy", "vpn", "browsing", "2fa" };

            foreach (var phrase in interestPhrases)
            {
                if (lower.Contains(phrase))
                {
                    foreach (var topic in topics)
                    {
                        if (lower.Contains(topic))
                            return topic.TrimEnd('s'); // normalise "passwords" → "password", "scams" → "scam"
                    }
                }
            }
            return null;
        }

        /// <summary>Returns true if the input is a quiz request.</summary>
        public bool IsQuizRequest(string input)
        {
            string lower = input.ToLowerInvariant();
            return lower.Contains("quiz") || lower.Contains("test me") || lower.Contains("test my knowledge");
        }
    }
}
