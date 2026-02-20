using Rage;
using System.IO;
using System.Linq;

namespace FritoQCCallouts
{
    internal static class Config
    {
        private const string FilePath = @"Plugins\LSPDFR\FritoQCCallouts.ini";
        public static InitializationFile INIFile = new InitializationFile(FilePath);
        public static bool CreatedDefault = false;

        public static bool DebugMode;
        public static bool FightInProgress;
        public static bool GunShotsReported;
        public static bool HitAndRun;
        public static bool IntoxicatedIndividual;
        public static bool PanicButton;

        static Config()
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(FilePath))
            {
                INIFile.Create();
                INIFile = new InitializationFile(FilePath);

                // --- General ---
                INIFile.Write("General", "Debug Mode", "false");

                // --- Callouts ---
                INIFile.Write("Callouts", "Fight In Progress", "true");
                INIFile.Write("Callouts", "Gunshots Reported", "true");
                INIFile.Write("Callouts", "Hit and Run", "true");
                INIFile.Write("Callouts", "Intoxicated Individual", "true");
                INIFile.Write("Callouts", "Panic Button", "true");

                Game.LogTrivial("[FritoQC Callouts] Created configuration file with defaults.");
                CreatedDefault = true;
            }
            else
            {
                // Repair missing keys silently
                WriteIfMissing("General", "Debug Mode", "false");
                WriteIfMissing("Callouts", "Fight In Progress", "true");
                WriteIfMissing("Callouts", "Gunshots Reported", "true");
                WriteIfMissing("Callouts", "Hit and Run", "true");
                WriteIfMissing("Callouts", "Intoxicated Individual", "true");
                WriteIfMissing("Callouts", "Panic Button", "true");
            }

            // --- Read values after setup ---
            DebugMode = SafeReadBool("General", "Debug Mode", false);
            FightInProgress = SafeReadBool("Callouts", "Fight In Progress", true);
            GunShotsReported = SafeReadBool("Callouts", "Gunshots Reported", true);
            HitAndRun = SafeReadBool("Callouts", "Hit and Run", true);
            IntoxicatedIndividual = SafeReadBool("Callouts", "Intoxicated Individual", true);
            PanicButton = SafeReadBool("Callouts", "Panic Button", true);
        }

        private static void WriteIfMissing(string section, string key, string value)
        {
            var current = INIFile.ReadString(section, key, null);
            if (string.IsNullOrWhiteSpace(current))
                INIFile.Write(section, key, value);
        }

        private static bool SafeReadBool(string section, string key, bool defaultValue)
        {
            string raw = INIFile.ReadString(section, key, null);
            if (string.IsNullOrEmpty(raw))
                raw = defaultValue.ToString();

            raw = new string(raw.Where(c => !char.IsControl(c)).ToArray()).Trim().Trim('"');

            if (bool.TryParse(raw, out bool result))
                return result;

            Game.LogTrivial($"[FritoQC Callouts] Warning: Invalid boolean value for '{key}' in section [{section}], using default {defaultValue}");
            INIFile.Write(section, key, defaultValue.ToString().ToLowerInvariant());
            return defaultValue;
        }
    }
}
