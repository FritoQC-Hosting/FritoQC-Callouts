using Rage;

namespace FritosCallouts
{
    internal static class Config
    {
        public static readonly InitializationFile INIFile = new InitializationFile(@"Plugins\LSPDFR\FritoQCCallouts.ini");
        public static bool CreatedDefault = false;

        static Config()
        {
            if (!INIFile.Exists())
            {
                CreatedDefault = true;

                INIFile.Write("Comments", "Info", "All callouts can be true or false. Default is true.");

                INIFile.Write("Callouts", "Gunshots Reported", "true");
                INIFile.Write("Callouts", "Hit and Run", "true");
                INIFile.Write("Callouts", "Intoxicated Individual", "true");
                INIFile.Write("Callouts", "Panic Button", "true");
                INIFile.Write("Callouts", "Fight In Progress", "true");
            }
        }

        public static readonly bool GunShotsReported = INIFile.ReadBoolean("Callouts", "Gunshots Reported", true);
        public static readonly bool HitAndRun = INIFile.ReadBoolean("Callouts", "Hit and Run", true);
        public static readonly bool IntoxicatedIndividual = INIFile.ReadBoolean("Callouts", "Intoxicated Individual", true);
        public static readonly bool PanicButton = INIFile.ReadBoolean("Callouts", "Panic Button", true);
        public static readonly bool FightInProgress = INIFile.ReadBoolean("Callouts", "Fight In Progress", true);
    }
}
