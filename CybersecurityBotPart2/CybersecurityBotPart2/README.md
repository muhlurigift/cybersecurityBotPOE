# Cybersecurity Awareness Bot — Part 2 (PROG6221)

A **WPF (Windows Presentation Foundation)** GUI chatbot that educates South African citizens about cybersecurity.  
Builds on Part 1 and adds keyword recognition, random responses, conversation flow, memory/recall, and sentiment detection.

---

## ✅ Part 2 Features Implemented

| Feature | Status |
|---|---|
| WPF GUI (dark cybersecurity theme) | ✅ |
| ASCII art carried over from Part 1 | ✅ |
| Voice greeting (WAV) on launch | ✅ |
| Keyword recognition (10+ topics) | ✅ |
| Random multi-responses (phishing, password, scam, malware) | ✅ |
| Conversation flow / follow-up handling | ✅ |
| Memory & recall (favourite topic, last topic) | ✅ |
| Sentiment detection (Worried / Curious / Frustrated / Positive) | ✅ |
| Inline cybersecurity quiz (11 questions) | ✅ |
| Input validation & graceful error handling | ✅ |
| OOP: classes, dictionaries, lists, delegates pattern | ✅ |
| GitHub Actions CI (Windows runner, build passes ✅) | ✅ |

---

## 🚀 How to Run

### Requirements
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Visual Studio 2022 **or** `dotnet` CLI

### Clone & Run
```bash
git clone <your-repo-url>
cd CybersecurityBotPart2
dotnet run
```

### Build Release
```bash
dotnet build --configuration Release
```

The compiled `.exe` will be in `bin/Release/net8.0-windows/`.

---

## 📁 Project Structure

```
CybersecurityBotPart2/
├── .github/
│   └── workflows/
│       └── dotnet-wpf.yml       ← GitHub Actions CI
├── Models/
│   ├── ChatMessage.cs           ← Message data model
│   └── UserMemory.cs            ← Memory/recall model
├── Services/
│   ├── AudioPlayer.cs           ← WAV greeting player
│   ├── ChatbotService.cs        ← Main orchestrator
│   ├── ResponseSystem.cs        ← Keyword + random responses
│   └── SentimentDetector.cs     ← Sentiment analysis
├── App.xaml / App.xaml.cs       ← WPF application entry
├── MainWindow.xaml              ← UI layout (XAML)
├── MainWindow.xaml.cs           ← UI code-behind + quiz
├── greeting.wav                 ← Voice greeting audio
├── CybersecurityBotGUI.csproj   ← Project file
└── README.md
```

---

## 🎮 How to Use

1. Enter your **name** in the welcome dialog.
2. Type messages or click the **quick topic buttons** in the sidebar.
3. Try commands like:
   - `phishing tip` — get a random phishing tip
   - `password` — password safety advice
   - `I'm interested in privacy` — bot remembers your interest
   - `tell me more` — follow-up on last topic
   - `show memory` — see what the bot remembers about you
   - `quiz` — start the cybersecurity quiz
   - `help` — list all commands
   - `goodbye` — end the session

---

## 🔁 GitHub Actions CI

CI runs on every push. The workflow:
1. Checks out code
2. Sets up .NET 8
3. Restores packages
4. Builds in Release mode
5. Uploads the binary as an artefact

> 📸 See CI screenshot in `/docs/ci-screenshot.png`

---

## 📚 References

Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review.  
*The African Journal of Information and Communication*, 28(28).  
doi: https://doi.org/10.23962/10539/32213
