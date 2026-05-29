using System;

namespace CybersecurityBotGUI.Models
{
    /// <summary>
    /// Represents a single chat message in the conversation.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>Who sent the message – either the user or the bot.</summary>
        public MessageSender Sender { get; set; }

        /// <summary>The text content of the message.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>The time the message was sent.</summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>Display-friendly timestamp string.</summary>
        public string TimeDisplay => Timestamp.ToString("HH:mm");
    }

    /// <summary>
    /// Identifies the originator of a chat message.
    /// </summary>
    public enum MessageSender
    {
        User,
        Bot,
        System
    }
}
