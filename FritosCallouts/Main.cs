using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace FritoQCCallouts
{
    public class Main : Plugin
    {
        public static Version NewVersion = new Version();
        public static Version curVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public static bool STP;
        public static bool CalloutInterface;
        public static bool UpToDate;
        public static bool Beta = false;

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        }

        public override void Finally()
        {
            Game.LogTrivial("[FritoQC Callouts] Plugin cleaned up.");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (!OnDuty) return;

            CheckForUpdates();
            RegisterCallouts();
        }

        private static void CheckForUpdates()
        {
            Game.LogTrivial("[FritoQC Callouts] Player went on duty. Checking for updates...");

            GameFiber.StartNew(() =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        string s = client.DownloadString(
                            "http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=36706&textOnly=1"
                        );

                        if (Version.TryParse(s.Trim(), out var v))
                            NewVersion = v;
                    }

                    // If no version data, skip checks
                    if (NewVersion.Major == 0 && NewVersion.Minor == 0 && NewVersion.Build == 0)
                    {
                        Game.LogTrivial("[FritoQC Callouts] Invalid or missing version info, skipping Beta check.");
                        return;
                    }

                    Version curTrimmed = new Version(curVersion.Major, curVersion.Minor, curVersion.Build);
                    Version newTrimmed = new Version(NewVersion.Major, NewVersion.Minor, NewVersion.Build);

                    if (curTrimmed.CompareTo(newTrimmed) < 0)
                    {
                        // 🔴 Outdated
                        Game.LogTrivial($"[FritoQC Callouts] Update available. Installed {curTrimmed}, New {newTrimmed}.");
                        Game.DisplayNotification($"~r~FritoQC Callouts~w~ is ~y~Outdated~w~!\nInstalled: ~b~{curTrimmed}~w~\nLatest: ~g~{newTrimmed}");
                        UpToDate = false;
                    }
                    else if (curTrimmed.CompareTo(newTrimmed) > 0)
                    {
                        // 🧪 Beta
                        Game.LogTrivial($"[FritoQC Callouts] Detected BETA release. Installed {curTrimmed}, Latest {newTrimmed}.");
                        Game.DisplayNotification($"~o~FritoQC Callouts~w~ ~r~BETA~w~ detected!\nVersion: ~b~{curTrimmed}");
                        UpToDate = true;
                        Beta = true;
                    }
                    else
                    {
                        // ✅ Up to date (silent)
                        Game.LogTrivial("[FritoQC Callouts] Plugin is up to date.");
                        UpToDate = true;
                    }
                }
                catch (Exception ex)
                {
                    Game.LogTrivial($"[FritoQC Callouts] Error while checking for updates: {ex.Message}");
                }
            });
        }

        private static void RegisterCallouts()
        {
            if (Config.CreatedDefault)
                Game.LogTrivial("[FritoQC Callouts] Default configuration file created at " + Config.INIFile.FileName);

            try
            {
                var _ = Config.GunShotsReported; // force config load
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Error reading config file. Using defaults.");
            }

            STP = Functions.GetAllUserPlugins().ToList()
                .Any(a => a?.FullName.Contains("StopThePed") == true);
            Game.LogTrivial("[FritoQC Callouts] StopThePed " + (STP ? "detected." : "not installed."));

            CalloutInterface = Functions.GetAllUserPlugins().ToList()
                .Any(a => a?.FullName.Contains("CalloutInterface") == true);
            Game.LogTrivial("[FritoQC Callouts] Callout Interface " + (CalloutInterface ? "detected." : "not installed."));

            if (Config.GunShotsReported) Functions.RegisterCallout(typeof(Callouts.GunShotsReported));
            if (Config.HitAndRun) Functions.RegisterCallout(typeof(Callouts.HitAndRun));
            if (Config.IntoxicatedIndividual) Functions.RegisterCallout(typeof(Callouts.IntoxicatedIndividual));
            if (Config.PanicButton) Functions.RegisterCallout(typeof(Callouts.PanicButton));
            if (Config.FightInProgress) Functions.RegisterCallout(typeof(Callouts.FightInProgress));
            if (Config.JewelleryRobbery) Functions.RegisterCallout(typeof(Callouts.JewelleryRobbery));

            Game.LogTrivial("[FritoQC Callouts] Finished registering callouts.");

            if (Beta)
            {
                Game.LogTrivial("[FritoQC Callouts] Registering Beta callouts...");
                Game.LogTrivial("[FritoQC Callouts] Finished registering Beta callouts.");
            }
        }
    }
}
