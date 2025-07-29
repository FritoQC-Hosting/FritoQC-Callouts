//To Create your own INI file, pull a pro gamer move and simply copy/paste a random INI file and rename it to what you want to call it.

using Rage;
using System.Windows.Forms;

namespace FritosCallouts
{
    internal static class Config
    {
        public static readonly InitializationFile INIFile = new InitializationFile(@"Plugins\LSPDFR\FritoQCsCallouts.ini");

        //All Callouts
        public static readonly bool GunShotsReported = INIFile.ReadBoolean("Callouts", "Gunshots Reported", true);
        public static readonly bool HitAndRun = INIFile.ReadBoolean("Callouts", "Hit and Run", true);
        public static readonly bool IntoxicatedIndividual = INIFile.ReadBoolean("Callouts", "Intoxicated Individual", true);
        public static readonly bool PanicButton = INIFile.ReadBoolean("Callouts", "Panic button", true);
        public static readonly bool FightInProgress = INIFile.ReadBoolean("Callouts", "Fight In Progress", true);

        //All keys

        // public static readonly Keys MainInteractionKey = INIFile.ReadEnum<Keys>("Keys", "Main Key", Keys.Y);
    }
}
