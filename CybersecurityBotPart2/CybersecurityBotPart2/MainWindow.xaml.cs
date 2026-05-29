using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CybersecurityBotGUI.Models;
using CybersecurityBotGUI.Services;

namespace CybersecurityBotGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml — Part 2 GUI chatbot.
    /// Implements: GUI design, keyword recognition, random responses,
    /// conversation flow, memory/recall, sentiment detection, error handling,
    /// and code optimisation using OOP + collections.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ── Services ─────────────────────────────────────────────────────────────
        private ChatbotService? _chatbot;
        private SentimentDetector _sentimentDetector = new();
        private AudioPlayer _audioPlayer = new();

        // ── State ────────────────────────────────────────────────────────────────
        private int _messageCount = 0;
        private bool _isTyping = false;
        private string _userName = string.Empty;

        // ── Colours (matching the XAML theme) ───────────────────────────────────
        private static readonly SolidColorBrush BrushBotBubble = new(Color.FromRgb(0x1C, 0x23, 0x33));
        private static readonly SolidColorBrush BrushUserBubble = new(Color.FromRgb(0x1A, 0x3A, 0x2A));
        private static readonly SolidColorBrush BrushSysBubble = new(Color.FromRgb(0x1A, 0x18, 0x10));
        private static readonly SolidColorBrush BrushBotText = new(Color.FromRgb(0xE6, 0xED, 0xF3));
        private static readonly SolidColorBrush BrushUserText = new(Color.FromRgb(0x00, 0xFF, 0x88));
        private static readonly SolidColorBrush BrushSysText = new(Color.FromRgb(0xFF, 0xC1, 0x07));
        private static readonly SolidColorBrush BrushTimestamp = new(Color.FromRgb(0x8B, 0x94, 0x9E));
        private static readonly SolidColorBrush BrushBotBorder = new(Color.FromRgb(0x00, 0xD4, 0xFF));
        private static readonly SolidColorBrush BrushUserBorder = new(Color.FromRgb(0x00, 0xFF, 0x88));
        private static readonly SolidColorBrush BrushSysBorder = new(Color.FromRgb(0xFF, 0xC1, 0x07));

        // ── ASCII art (carried over from Part 1) ─────────────────────────────────
        private const string AsciiArt =
            "  ██████╗██╗   ██╗██████╗ ███████╗██████╗ ███████╗███████╗ ██████╗    " +
            "  ██╔══██╗██║ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝██╔════╝   " +
            "  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝███████╗█████╗  ██║        " +
            "  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗╚════██║██╔══╝  ██║        " +
            "  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║███████║███████╗╚██████╗   " +
            "   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝ ╚═════╝  " +
            "              ═══════[ CYBERSECURITY AWARENESS BOT v2.0 ]═══════       ";

        // ── Constructor ──────────────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
            AsciiArtBlock.Text = AsciiArt;
            // Play greeting audio as soon as window is created
            _audioPlayer.PlayGreeting();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  WINDOW EVENTS
        // ════════════════════════════════════════════════════════════════════════

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameInputBox.Focus();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  NAME DIALOG
        // ════════════════════════════════════════════════════════════════════════

        private void NameInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnStartChat_Click(sender, e);
        }

        private void BtnStartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInputBox.Text.Trim();

            // Validate name (2-30 chars, not empty)
            if (string.IsNullOrEmpty(name) || name.Length < 2)
            {
                NameErrorLabel.Text = "⚠ Please enter a valid name (at least 2 characters).";
                NameErrorLabel.Visibility = Visibility.Visible;
                NameInputBox.Focus();
                return;
            }
            if (name.Length > 30)
            {
                NameErrorLabel.Text = "⚠ Name must be 30 characters or fewer.";
                NameErrorLabel.Visibility = Visibility.Visible;
                NameInputBox.Focus();
                return;
            }

            // Initialise chatbot with the user's name
            _userName = name;
            _chatbot = new ChatbotService(_userName);

            // Hide the dialog
            NameDialog.Visibility = Visibility.Collapsed;

            // Update memory sidebar with name
            RefreshMemorySidebar();

            // Show personalised welcome messages
            ShowWelcomeMessages();

            InputBox.Focus();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  WELCOME SEQUENCE
        // ════════════════════════════════════════════════════════════════════════

        private async void ShowWelcomeMessages()
        {
            AddSystemMessage($"🔒 Session started — Welcome, {_userName}!");
            await Task.Delay(600);

            AddBotMessage(
                $"Hi {_userName}! 👋 I'm your Cybersecurity Awareness Bot.\n\n" +
                "I can help you with:\n" +
                "  🔑 Password safety\n" +
                "  🎣 Phishing recognition\n" +
                "  🔒 Privacy best practices\n" +
                "  🦠 Malware awareness\n" +
                "  ⚠️ Scam detection\n" +
                "  🌐 Safe browsing\n\n" +
                "Type 'help' to see all commands, or just ask me anything about cybersecurity!");

            SetStatus($"Chat active with {_userName}");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  INPUT HANDLING
        // ════════════════════════════════════════════════════════════════════════

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !_isTyping)
            {
                e.Handled = true;
                _ = SendMessageAsync();
            }
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Live sentiment update in sidebar as user types
            if (!string.IsNullOrWhiteSpace(InputBox.Text))
            {
                Sentiment s = _sentimentDetector.Detect(InputBox.Text);
                UpdateSentimentIndicator(s);
            }
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isTyping) await SendMessageAsync();
        }

        private async Task SendMessageAsync()
        {
            if (_chatbot == null) return;

            string input = InputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            InputBox.Clear();
            InputBox.IsEnabled = false;
            SendBtn.IsEnabled = false;
            _isTyping = true;

            // Show user bubble
            AddUserMessage(input);

            // Update sentiment from the sent message
            Sentiment sentiment = _sentimentDetector.Detect(input);
            UpdateSentimentIndicator(sentiment);

            // Brief delay to simulate typing
            SetStatus("Bot is thinking…");
            await Task.Delay(400 + input.Length * 8);

            // Get bot response
            string? response = _chatbot.ProcessMessage(input);

            if (response == null)
            {
                // User said goodbye
                AddBotMessage($"Goodbye, {_userName}! 👋 Stay safe online. Remember: Think before you click! 🔒");
                AddSystemMessage("Session ended. Close the window to exit.");
                SetStatus("Session ended");
            }
            else if (response == "__QUIZ__")
            {
                // Show inline quiz
                AddBotMessage("Great! Let's test your cybersecurity knowledge. Starting the quiz now…");
                await Task.Delay(500);
                ShowQuizPanel();
            }
            else
            {
                AddBotMessage(response);
                RefreshMemorySidebar();
                SetStatus($"Ready — {_messageCount} messages exchanged");
            }

            InputBox.IsEnabled = true;
            SendBtn.IsEnabled = true;
            _isTyping = false;
            InputBox.Focus();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  TOOLBAR BUTTONS
        // ════════════════════════════════════════════════════════════════════════

        private async void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (_chatbot == null) return;
            InputBox.Text = "help";
            await SendMessageAsync();
        }

        private async void BtnMemory_Click(object sender, RoutedEventArgs e)
        {
            if (_chatbot == null) return;
            InputBox.Text = "show memory";
            await SendMessageAsync();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            MessagePanel.Children.Clear();
            _messageCount = 0;
            MsgCountLabel.Text = string.Empty;
            AddSystemMessage("Chat cleared. Ready for a new conversation!");
        }

        private async void TopicBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_chatbot == null || sender is not Button btn) return;
            string topic = btn.Tag?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(topic)) return;
            InputBox.Text = topic;
            await SendMessageAsync();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  MESSAGE BUBBLE BUILDERS
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Adds a user message bubble (right-aligned, green).</summary>
        private void AddUserMessage(string text)
        {
            _messageCount++;
            var container = BuildBubble(
                sender: $"[{_userName}]",
                text: text,
                bg: BrushUserBubble,
                border: BrushUserBorder,
                textColor: BrushUserText,
                rightAlign: true);

            MessagePanel.Children.Add(container);
            ScrollToBottom();
            MsgCountLabel.Text = $"{_messageCount} msgs";
        }

        /// <summary>Adds a bot message bubble (left-aligned, blue/white).</summary>
        private void AddBotMessage(string text)
        {
            _messageCount++;
            var container = BuildBubble(
                sender: "[CyberBot]",
                text: text,
                bg: BrushBotBubble,
                border: BrushBotBorder,
                textColor: BrushBotText,
                rightAlign: false);

            MessagePanel.Children.Add(container);
            ScrollToBottom();
            MsgCountLabel.Text = $"{_messageCount} msgs";
        }

        /// <summary>Adds a system/notification message (centred, yellow).</summary>
        private void AddSystemMessage(string text)
        {
            var container = BuildBubble(
                sender: "[SYSTEM]",
                text: text,
                bg: BrushSysBubble,
                border: BrushSysBorder,
                textColor: BrushSysText,
                rightAlign: false,
                isSystem: true);

            MessagePanel.Children.Add(container);
            ScrollToBottom();
        }

        /// <summary>
        /// Constructs a chat bubble Border containing sender label,
        /// message text, and timestamp — reusable for all message types.
        /// </summary>
        private Grid BuildBubble(
            string sender,
            string text,
            SolidColorBrush bg,
            SolidColorBrush border,
            SolidColorBrush textColor,
            bool rightAlign,
            bool isSystem = false)
        {
            // Outer margin wrapper
            var outerGrid = new Grid { Margin = new Thickness(0, 4, 0, 4) };

            // Bubble border
            var bubble = new Border
            {
                Background = bg,
                BorderBrush = border,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 8, 12, 8),
                MaxWidth = 620,
                HorizontalAlignment = isSystem
                    ? HorizontalAlignment.Center
                    : rightAlign
                        ? HorizontalAlignment.Right
                        : HorizontalAlignment.Left
            };

            // Inner stack: sender + text + time
            var inner = new StackPanel();

            // Sender label row
            var senderRow = new Grid();
            senderRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            senderRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var senderLabel = new TextBlock
            {
                Text = sender,
                Foreground = border,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                FontWeight = FontWeights.Bold
            };
            Grid.SetColumn(senderLabel, 0);

            var timeLabel = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                Foreground = BrushTimestamp,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Grid.SetColumn(timeLabel, 1);

            senderRow.Children.Add(senderLabel);
            senderRow.Children.Add(timeLabel);
            inner.Children.Add(senderRow);

            // Message text
            var msgText = new TextBlock
            {
                Text = text,
                Foreground = textColor,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0),
                LineHeight = 20
            };
            inner.Children.Add(msgText);

            bubble.Child = inner;
            outerGrid.Children.Add(bubble);

            // Fade-in animation
            bubble.Opacity = 0;
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
            bubble.BeginAnimation(OpacityProperty, fade);

            return outerGrid;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  QUIZ (inline — no separate window)
        // ════════════════════════════════════════════════════════════════════════

        // Quiz data: question, choices (A/B/C/D or True/False), correct answer index, explanation
        private readonly List<(string Q, string[] Choices, int Answer, string Explanation)> _quizQuestions = new()
        {
            ("What should you do if you receive an email asking for your password?",
             new[]{"A) Reply with your password","B) Delete the email","C) Report it as phishing","D) Ignore it"},
             2, "Correct! Reporting phishing helps protect everyone. Legitimate services NEVER ask for your password via email."),

            ("How long should a strong password be?",
             new[]{"A) 4 characters","B) 6 characters","C) 8 characters","D) 12+ characters"},
             3, "Correct! Passwords should be at least 12 characters. Longer is always better for security."),

            ("True or False: Using the same password for all accounts is safe if it's a strong password.",
             new[]{"A) True","B) False"},
             1, "False! If one site is breached, attackers try your credentials everywhere. Always use unique passwords!"),

            ("What does HTTPS in a URL indicate?",
             new[]{"A) The site is fast","B) The site is encrypted and more secure","C) The site is free","D) The site is government-owned"},
             1, "Correct! HTTPS means the connection is encrypted. Never enter personal info on HTTP-only sites."),

            ("Which of these is an example of social engineering?",
             new[]{"A) A software update notification","B) A call from 'Microsoft' saying your PC is hacked","C) A login page asking for your password","D) An antivirus scan"},
             1, "Correct! Unsolicited calls claiming your PC is infected are classic social engineering / vishing attacks."),

            ("True or False: Public Wi-Fi is safe to use for online banking.",
             new[]{"A) True","B) False"},
             1, "False! Public Wi-Fi can be intercepted. Always use a VPN or mobile data for sensitive transactions."),

            ("What is two-factor authentication (2FA)?",
             new[]{"A) Logging in twice","B) Using two different browsers","C) A second verification step beyond your password","D) Having two email accounts"},
             2, "Correct! 2FA adds a second layer — even if your password is stolen, the attacker still can't log in."),

            ("What is 'phishing'?",
             new[]{"A) A secure login method","B) Tricking users into revealing sensitive info via fake messages","C) A type of encryption","D) A firewall technique"},
             1, "Correct! Phishing uses deceptive emails, messages, or websites to steal credentials and personal data."),

            ("How often should you update your software and operating system?",
             new[]{"A) Never — updates break things","B) Once a year","C) As soon as updates are available","D) Only when prompted by a friend"},
             2, "Correct! Updates patch security vulnerabilities. Delaying updates leaves known holes open to attackers."),

            ("True or False: A padlock icon in the browser means a website is legitimate and trustworthy.",
             new[]{"A) True","B) False"},
             1, "False! HTTPS only means the connection is encrypted — phishing sites can also use HTTPS. Always check the domain name carefully."),

            ("Which of these is the safest way to store passwords?",
             new[]{"A) Write them in a notebook","B) Use the same password everywhere","C) Use a reputable password manager","D) Save them in a text file on your desktop"},
             2, "Correct! Password managers generate and store strong, unique passwords securely."),
        };

        private int _quizIndex = 0;
        private int _quizScore = 0;
        private bool _quizActive = false;
        private bool _awaitingAnswer = false;

        private void ShowQuizPanel()
        {
            if (_quizActive) return; // prevent re-entry
            _quizIndex = 0;
            _quizScore = 0;
            _quizActive = true;
            _awaitingAnswer = false;

            AddSystemMessage($"🎮 QUIZ STARTED — {_quizQuestions.Count} questions");
            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            if (_quizIndex >= _quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            var (q, choices, _, _) = _quizQuestions[_quizIndex];

            // Build question bubble manually with answer buttons
            var outerGrid = new Grid { Margin = new Thickness(0, 6, 0, 6) };
            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x0D, 0x1B, 0x2A)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xD4, 0xFF)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(14, 10, 14, 10),
                MaxWidth = 640,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var panel = new StackPanel();

            // Question header
            panel.Children.Add(new TextBlock
            {
                Text = $"[CyberBot] Q{_quizIndex + 1}/{_quizQuestions.Count}",
                Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xD4, 0xFF)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 6)
            });

            // Question text
            panel.Children.Add(new TextBlock
            {
                Text = q,
                Foreground = new SolidColorBrush(Color.FromRgb(0xE6, 0xED, 0xF3)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Answer buttons
            var answerPanel = new WrapPanel { Orientation = Orientation.Vertical };
            for (int i = 0; i < choices.Length; i++)
            {
                int capturedIndex = i;
                var btn = new Button
                {
                    Content = choices[i],
                    Margin = new Thickness(0, 3, 0, 3),
                    Padding = new Thickness(12, 6, 12, 6),
                    Background = new SolidColorBrush(Color.FromRgb(0x16, 0x1B, 0x22)),
                    Foreground = new SolidColorBrush(Color.FromRgb(0xE6, 0xED, 0xF3)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0x30, 0x36, 0x3D)),
                    BorderThickness = new Thickness(1),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    Cursor = Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Template = CreateRoundedButtonTemplate()
                };
                btn.Click += (s, e) => HandleQuizAnswer(capturedIndex, answerPanel, choices.Length);
                answerPanel.Children.Add(btn);
            }
            panel.Children.Add(answerPanel);

            bubble.Child = inner_panel(panel);
            outerGrid.Children.Add(bubble);
            MessagePanel.Children.Add(outerGrid);

            _awaitingAnswer = true;
            ScrollToBottom();
        }

        // Workaround: WPF Border only accepts one child, wrap in a ScrollViewer if needed
        private static UIElement inner_panel(StackPanel p) => p;

        private void HandleQuizAnswer(int selectedIndex, WrapPanel answerPanel, int totalChoices)
        {
            if (!_awaitingAnswer) return;
            _awaitingAnswer = false;

            var (_, choices, correctIndex, explanation) = _quizQuestions[_quizIndex];
            bool correct = selectedIndex == correctIndex;

            if (correct) _quizScore++;

            // Colour all buttons: green = correct, red = wrong selection
            for (int i = 0; i < answerPanel.Children.Count; i++)
            {
                if (answerPanel.Children[i] is Button b)
                {
                    b.IsEnabled = false;
                    if (i == correctIndex)
                        b.Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x3A, 0x2A));
                    else if (i == selectedIndex && !correct)
                        b.Background = new SolidColorBrush(Color.FromRgb(0x3A, 0x1A, 0x1A));
                }
            }

            // Feedback message
            string feedback = correct
                ? $"✅ Correct! {explanation}"
                : $"❌ Not quite. {explanation}";

            AddBotMessage(feedback);
            _quizIndex++;

            // Short delay then next question
            Task.Delay(800).ContinueWith(_ =>
            {
                Dispatcher.Invoke(ShowNextQuestion);
            });
        }

        private void EndQuiz()
        {
            _quizActive = false;
            int total = _quizQuestions.Count;
            double pct = (_quizScore / (double)total) * 100;

            string grade = pct switch
            {
                >= 90 => "🏆 Outstanding! You're a cybersecurity pro!",
                >= 70 => "🌟 Great job! You know your stuff.",
                >= 50 => "👍 Good effort! Keep learning to stay safe.",
                _ => "📚 Keep studying — cybersecurity knowledge could save you one day!"
            };

            AddBotMessage(
                $"🎮 QUIZ COMPLETE!\n\n" +
                $"Your score: {_quizScore}/{total} ({pct:0}%)\n\n" +
                $"{grade}\n\n" +
                "Type 'quiz' to try again, or ask me anything about cybersecurity!");

            AddSystemMessage("Quiz ended");
        }

        /// <summary>Creates a simple rounded-corner ControlTemplate for quiz buttons.</summary>
        private static ControlTemplate CreateRoundedButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetBinding(Border.BackgroundProperty,
                new System.Windows.Data.Binding("Background")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            borderFactory.SetBinding(Border.BorderBrushProperty,
                new System.Windows.Data.Binding("BorderBrush")
                { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.MarginProperty, new Thickness(12, 6, 12, 6));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            borderFactory.AppendChild(contentFactory);
            template.VisualTree = borderFactory;
            return template;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SIDEBAR / STATUS HELPERS
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Refreshes the memory sidebar labels from the chatbot's UserMemory.</summary>
        private void RefreshMemorySidebar()
        {
            if (_chatbot == null) return;
            var mem = _chatbot.Memory;

            MemoryNameLabel.Text = $"👤 Name: {mem.Name}";
            MemoryTopicLabel.Text = string.IsNullOrEmpty(mem.FavouriteTopic)
                ? "⭐ Interest: (none yet)"
                : $"⭐ Interest: {mem.FavouriteTopic}";
            MemoryLastLabel.Text = string.IsNullOrEmpty(mem.LastTopic)
                ? "💬 Last topic: (none)"
                : $"💬 Last topic: {mem.LastTopic}";
        }

        /// <summary>Updates the sentiment badge in the sidebar.</summary>
        private void UpdateSentimentIndicator(Sentiment sentiment)
        {
            var (emoji, label, color) = sentiment switch
            {
                Sentiment.Worried => ("😰", "Worried", Color.FromRgb(0xFF, 0x47, 0x57)),
                Sentiment.Frustrated => ("😤", "Frustrated", Color.FromRgb(0xFF, 0x91, 0x00)),
                Sentiment.Curious => ("🤔", "Curious", Color.FromRgb(0x00, 0xD4, 0xFF)),
                Sentiment.Positive => ("😊", "Positive", Color.FromRgb(0x00, 0xFF, 0x88)),
                _ => ("😐", "Neutral", Color.FromRgb(0x8B, 0x94, 0x9E))
            };

            SentimentLabel.Text = $"{emoji} {label}";
            SentimentLabel.Foreground = new SolidColorBrush(color);
            SentimentBorder.BorderBrush = new SolidColorBrush(color);
        }

        private void SetStatus(string message)
        {
            StatusLabel.Text = message;
        }

        private void ScrollToBottom()
        {
            ChatScroll.ScrollToEnd();
        }

        // ── Placeholder label helpers ────────────────────────────────────────────
        private void InputBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            PlaceholderLabel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void InputBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputBox.Text))
                PlaceholderLabel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}