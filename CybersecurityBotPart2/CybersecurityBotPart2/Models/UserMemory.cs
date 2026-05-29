using System.Collections.Generic;

namespace CybersecurityBotGUI.Models
{
    /// <summary>
    /// Stores details the chatbot has learned about the user during the conversation.
    /// This powers the Memory and Recall feature required by Part 2.
    /// </summary>
    public class UserMemory
    {
        /// <summary>The user's name, collected at startup.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The cybersecurity topic the user expressed most interest in.</summary>
        public string? FavouriteTopic { get; set; }

        /// <summary>The last topic that was discussed – used for follow-up handling.</summary>
        public string? LastTopic { get; set; }

        /// <summary>
        /// Generic key-value store for any extra facts the bot remembers
        /// (e.g., "concern" → "phishing").
        /// </summary>
        public Dictionary<string, string> Facts { get; set; } = new Dictionary<string, string>();

        /// <summary>Returns true if any extra facts have been stored.</summary>
        public bool HasFacts => Facts.Count > 0;
    }
}
