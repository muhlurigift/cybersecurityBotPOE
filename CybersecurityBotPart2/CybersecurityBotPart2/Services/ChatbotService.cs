using System;
using CybersecurityBotGUI.Models;
using CybersecurityBotGUI.Services;

namespace CybersecurityBotGUI.Services
{
    /// <summary>
    /// Orchestrates the chatbot's core logic:
    /// combines ResponseSystem, SentimentDetector, and UserMemory
    /// to produce a complete, contextual reply for any user input.
    /// </summary>
    public class ChatbotService
    {
        private readonly ResponseSystem    _responseSystem;
        private readonly SentimentDetector _sentimentDetector;
        private readonly UserMemory        _memory;

        // Tracks the topic from the previous turn for follow-up handling
        private string? _lastTopic;

        public ChatbotService(string userName)
        {
            _responseSystem    = new ResponseSystem();
            _sentimentDetector = new SentimentDetector();
            _memory            = new UserMemory { Name = userName };
        }

        // ── Public properties ────────────────────────────────────────────────────

        /// <summary>Exposes memory so the GUI can display recall info.</summary>
        public UserMemory Memory => _memory;

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Processes a single user message and returns the bot's reply.
        /// Returns null if the user wants to exit (so the GUI can handle it).
        /// </summary>
        public string? ProcessMessage(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't catch that — could you rephrase?";

            // --- Special commands ---
            string lower = input.Trim().ToLower();

            if (lower == "show memory" || lower == "what do you remember")
                return BuildMemoryRecap();

            if (lower == "clear memory")
            {
                _memory.FavouriteTopic = null;
                _memory.Facts.Clear();
                return "I've cleared everything I remembered. Fresh start! 🧹";
            }

            // --- Quiz handled by GUI directly ---
            if (_responseSystem.IsQuizRequest(input))
                return "__QUIZ__";

            // --- Sentiment detection ---
            Sentiment sentiment = _sentimentDetector.Detect(input);
            string empathy      = _sentimentDetector.GetEmpathyPrefix(sentiment);

            // --- Core response ---
            string rawResponse = _responseSystem.GetResponse(input, _lastTopic, out string? newTopic);

            // Exit signal
            if (rawResponse == "__EXIT__")
                return null;

            // Update last topic for next turn
            _lastTopic = newTopic;

            // Update memory if a topic interest was detected
            UpdateMemoryFromResponse(input, newTopic);

            // --- Build final reply ---
            string finalResponse = string.IsNullOrEmpty(empathy)
                ? rawResponse
                : empathy + rawResponse;

            // Append memory-recall personalisation if relevant
            string memoryNote = BuildMemoryNote(newTopic);
            if (!string.IsNullOrEmpty(memoryNote))
                finalResponse += "\n\n" + memoryNote;

            return finalResponse;
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void UpdateMemoryFromResponse(string input, string? topic)
        {
            string lower = input.ToLowerInvariant();

            // Remember favourite topic
            if (topic != null && _memory.FavouriteTopic == null)
            {
                if (lower.Contains("interested in") || lower.Contains("care about") || lower.Contains("learn about"))
                {
                    _memory.FavouriteTopic = topic;
                    _memory.Facts["interest"] = topic;
                }
            }

            _memory.LastTopic = topic;
        }

        private string BuildMemoryNote(string? currentTopic)
        {
            // If the user previously expressed a favourite topic and the current topic matches, personalise
            if (!string.IsNullOrEmpty(_memory.FavouriteTopic) && _memory.FavouriteTopic == currentTopic)
                return $"💡 As someone interested in {_memory.FavouriteTopic}, you might also want to review your account security settings.";

            return string.Empty;
        }

        private string BuildMemoryRecap()
        {
            string recap = $"🧠 Here's what I remember about you, {_memory.Name}:\n";
            recap += $"• Name: {_memory.Name}\n";

            if (!string.IsNullOrEmpty(_memory.FavouriteTopic))
                recap += $"• Favourite topic: {_memory.FavouriteTopic}\n";

            if (!string.IsNullOrEmpty(_memory.LastTopic))
                recap += $"• Last discussed topic: {_memory.LastTopic}\n";

            if (_memory.Facts.ContainsKey("interest"))
                recap += $"• Expressed interest in: {_memory.Facts["interest"]}\n";

            if (recap.Split('\n').Length <= 3)
                recap += "• (Not much yet — keep chatting so I can learn more about you!)";

            return recap;
        }
    }
}
