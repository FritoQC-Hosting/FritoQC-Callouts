// FritoLays

using System;
using System.Net;
using System.Threading;
using LSPD_First_Response.Mod.API;
using Rage;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;

namespace FelixsCallouts
{
    public class Main : Plugin
    {
        public static Version NewVersion = new Version();
        public static Version curVersion = new Version("2.1");
        public static bool STP; //if STP is installed by the user
        public static bool CalloutInterface; //if Callout Interface is installed by the user
        public static bool UpToDate; //if the Plugin is updated.
        public static bool Beta = false;
        public static bool Debug_Mode = true;

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("FELIXSCALLOUTS: Felix's Callouts " + curVersion + " by Fruity has been loaded.");
        }
        public override void Finally()
        {
            Game.LogTrivial("FELIXSCALLOUTS: FelixsCallouts has been cleaned up.");
        }
        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                int num = (int)Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "Felix's Callouts", "~y~v." + curVersion + " ~b~by Fruity", " ~g~Loaded Successfully. ~b~Enjoy!");
                GameFiber.StartNew(delegate
                {
                    Game.LogTrivial("FELIXSCALLOUTS: Player Went on Duty. Checking for Updates.");
                    try
                    {
                        Thread FetchVersionThread = new Thread(() =>
                        {
                            using (WebClient client = new WebClient())
                            {
                                try
                                {
                                    string s = client.DownloadString("http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=36706&textOnly=1");

                                    NewVersion = new Version(s);
                                }
                                catch (Exception) { Game.LogTrivial("FELIXSCALLOUTS: LSPDFR API down. Aborting checks."); }
                            }
                        });

                        FetchVersionThread.Start();
                        try
                        {
                            while (FetchVersionThread.ThreadState != System.Threading.ThreadState.Stopped)
                            {
                                GameFiber.Yield();
                            }
                            // compare the versions  
                            if (curVersion.CompareTo(NewVersion) < 0)
                            {
                                Game.LogTrivial("FELIXSCALLOUTS: Finished Checking Felix's Callouts for Updates.");
                                Game.LogTrivial("FELIXSCALLOUTS: Update Available for Felis's Callouts. Installed Version " + curVersion + " ,New Version " + NewVersion);
                                Game.DisplayNotification("~g~Update Available~w~ for ~b~FelixsCallouts! Download at ~y~lcpdfr.com.");
                                Game.DisplayNotification("It is ~y~Strongly Recommended~w~ to~g~ Update~b~ FelixsCallouts. ~w~Playing on an Old Version ~r~May Cause Issues!");
                                Game.LogTrivial("====================FELIXSCALLOUTS WARNING====================");
                                Game.LogTrivial("Outdated FelixsCallouts Version. Please Update if You Experience Issues!!");
                                Game.LogTrivial("====================FELIXSCALLOUTS WARNING====================");
                                UpToDate = false;
                            }
                            else if (curVersion.CompareTo(NewVersion) > 0)
                            {
                                Game.LogTrivial("FELIXSCALLOUTS: DETECTED BETA RELEASE. DO NOT REDISTRIBUTE. PLEASE REPORT ALL ISSUES.");
                                Game.DisplayNotification("FELIXSCALLOUTS: ~r~DETECTED BETA RELEASE. ~w~DO NOT REDISTRIBUTE. PLEASE REPORT ALL ISSUES.");
                                UpToDate = true;
                                Beta = true;
                            }
                            else
                            {
                                Game.LogTrivial("FELIXSCALLOUTS: Finished Checking Felix's Callouts for Updates.");
                                Game.DisplayNotification("You are on the ~g~Latest Version~w~ of ~b~FelixsCallouts.");
                                Game.LogTrivial("FELIXSCALLOUTS: Felix's Callouts Callouts is Up to Date.");
                                UpToDate = true;
                            }
                        }
                        catch (Exception)
                        {
                            Game.LogTrivial("FELIXSCALLOUTS: Error while Processing Thread to Check for Updates.");
                        }
                    }
                    catch (Exception)
                    {
                        Game.LogTrivial("FELIXSCALLOUTS: Error while checking Felis's Callouts for updates.");
                    }
                });
                RegisterCallouts();
            }
        }
        private static void RegisterCallouts()
        {
            Game.LogTrivial("==========FELIXSCALLOUTS INFORMATION==========");
            Game.LogTrivial("FelixsCallouts by Fruity");
            Game.LogTrivial("Version " + curVersion + "");
            Game.LogTrivial("Please Join My Discord Server to Report Bugs/Improvements: https://discord.gg/qkmpBJmjJd. Enjoy!");

            if (Config.INIFile.Exists()) Game.LogTrivial("FelixsCallouts Config is Installed by User.");
            else Game.LogTrivial("FelixsCallouts Config is NOT Installed by User.");

            if (Functions.GetAllUserPlugins().ToList().Any(a => a != null && a.FullName.Contains("StopThePed")) == true)
            {
                Game.LogTrivial("StopThePed is Installed by User.");
                STP = true;
            }
            else
            {
                Game.LogTrivial("StopThePed is NOT Installed by User.");
                STP = false;
            }

            if (Functions.GetAllUserPlugins().ToList().Any(a => a != null && a.FullName.Contains("CalloutInterface")) == true)
            {
                Game.LogTrivial("CalloutInterface is Installed by User.");
                CalloutInterface = true;
            }
            else
            {
                Game.LogTrivial("CalloutInterface is NOT Installed by User.");
                CalloutInterface = false;
            }

            //CALLOUTS
            Game.LogTrivial("Started Registering Callouts.");

            if (Config.GunShotsReported || !Config.INIFile.Exists()) Functions.RegisterCallout(typeof(Callouts.GunShotsReported));
            if (Config.HitAndRun || !Config.INIFile.Exists()) Functions.RegisterCallout(typeof(Callouts.HitAndRun));
            if (Config.IntoxicatedIndividual || !Config.INIFile.Exists()) Functions.RegisterCallout(typeof(Callouts.IntoxicatedIndividual));
            if (Config.PanicButton || !Config.INIFile.Exists()) Functions.RegisterCallout(typeof(Callouts.PanicButton));
            if (Config.FightInProgress || !Config.INIFile.Exists()) Functions.RegisterCallout(typeof(Callouts.FightInProgress));

            Game.LogTrivial("Finished Registering Callouts.");

            ////BETA CALLOUTS
            if (Beta)
            {
                Game.LogTrivial("Started Registering Beta Callouts.");


                Game.LogTrivial("Finished Registering Beta Callouts.");
            }
        }
    }
}