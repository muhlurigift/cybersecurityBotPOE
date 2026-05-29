using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotGUI.Services
{
    /// <summary>
    /// Detects simple sentiments in user input so the chatbot can respond empathetically.
    /// Covers: Worried, Curious, Frustrated, Positive, and Neutral (default).
    /// </summary>
    public class SentimentDetector
    {
        // ── Sentiment keyword maps ───────────────────────────────────────────────

        private static readonly List<string> WorriedKeywords = new()
        {
            "worried", "scared", "afraid", "nervous", "anxious", "fear", "panic",
            "terrified", "concerned", "unsafe", "vulnerable", "exposed"
        };

        private static readonly List<string> CuriousKeywords = new()
        {
            "curious", "wondering", "interested", "want to know", "tell me",
            "how does", "why does", "what is", "explain", "learn", "understand"
        };

        private static readonly List<string> FrustratedKeywords = new()
        {
            "frustrated", "annoyed", "confused", "lost", "don't understand",
            "doesn't make sense", "complicated", "difficult", "hard", "upset", "angry"
        };

        private static readonly List<string> PositiveKeywords = new()
        {
            "great", "awesome", "love", "happy", "excited", "good", "perfect",
            "excellent", "thanks", "thank you", "helpful", "nice", "cool"
        };

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Analyses the input text and returns the dominant sentiment.
        /// </summary>
        public Sentiment Detect(string input)
        {
            string lower = input.ToLowerInvariant();

            if (WorriedKeywords.Any(k => lower.Contains(k)))    return Sentiment.Worried;
            if (FrustratedKeywords.Any(k => lower.Contains(k))) return Sentiment.Frustrated;
            if (CuriousKeywords.Any(k => lower.Contains(k)))    return Sentiment.Curious;
            if (PositiveKeywords.Any(k => lower.Contains(k)))   return Sentiment.Positive;

            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns an empathetic prefix sentence for the detected sentiment.
        /// The chatbot prepends this to its normal response.
        /// </summary>
        public string GetEmpathyPrefix(Sentiment sentiment) => sentiment switch
        {
            Sentiment.Worried =>
                "It's completely understandable to feel that way — cyber threats can be genuinely unsettling. " +
                "Let me reassure you with some practical guidance. ",

            Sentiment.Frustrated =>
                "I can see this might feel confusing — cybersecurity has a lot of moving parts. " +
                "Let me break it down more simply for you. ",

            Sentiment.Curious =>
                "Great question! I love helping people learn about staying safe online. ",

            Sentiment.Positive =>
                "Glad to hear that! Your enthusiasm for cybersecurity awareness is fantastic. ",

            _ => string.Empty
        };
    }

    /// <summary>
    /// Enumeration of detectable user sentiments.
    /// </summary>
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Positive
    }
}
