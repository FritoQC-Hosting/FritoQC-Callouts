using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response;
using System.Reflection;

namespace FelixsCallouts
{
    public class Main : Plugin
    {

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Felix's Callouts" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "By FruityQC Loaded");
            Game.LogTrivial("Go on duty to fully load Felix's Callouts");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
        }

        public override void Finally()
        {
            Game.LogTrivial("Felix's Callouts have been Unloaded");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("FelixsCallouts by FruityQC#0968 | Version 1.1.0 | Has been loaded");

            }
        }

        private static void RegisterCallouts()
        {

            Functions.RegisterCallout(typeof(Callouts.HighSpeedChase));
            Functions.RegisterCallout(typeof(Callouts.IntoxicatedIndividual));
            Functions.RegisterCallout(typeof(Callouts.GunShotsReported));
            Functions.RegisterCallout(typeof(Callouts.PanicButton));

        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;
        }

        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == Plugin.ToLower())
                {
                    if (minversion == null || an.Version.CompareTo(minversion) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
