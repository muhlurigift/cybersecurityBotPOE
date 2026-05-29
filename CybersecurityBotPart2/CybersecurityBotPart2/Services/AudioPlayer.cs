using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace CybersecurityBotGUI.Services
{
    /// <summary>
    /// Plays the voice greeting WAV file when the application starts.
    /// Falls back gracefully when audio is unavailable.
    /// </summary>
    public class AudioPlayer
    {
        private readonly string _wavPath;

        public AudioPlayer()
        {
            // Look for greeting.wav next to the executable (same as Part 1)
            _wavPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
        }

        /// <summary>
        /// Attempts to play the greeting.wav file asynchronously so the UI doesn't block.
        /// </summary>
        public void PlayGreeting()
        {
            // Only SoundPlayer works on Windows WPF without extra packages
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            if (!File.Exists(_wavPath))
                return;

            try
            {
                // SoundPlayer is non-blocking when using PlayAsync
                var player = new SoundPlayer(_wavPath);
                player.LoadAsync();       // load into memory asynchronously
                player.Play();            // plays asynchronously – UI stays responsive
            }
            catch (Exception ex)
            {
                // Swallow audio errors – they must never crash the application
                System.Diagnostics.Debug.WriteLine($"[AudioPlayer] Could not play greeting: {ex.Message}");
            }
        }
    }
}
