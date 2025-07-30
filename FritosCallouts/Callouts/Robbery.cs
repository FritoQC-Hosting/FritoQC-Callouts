using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using System.Threading;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using FritosCallouts.Utility;

namespace FritosCallouts.Callouts
{
    [CalloutInterface("Robbry in progress", CalloutProbability.Low, "Suspect's potentially armed", "Code 3", "LSPD")]

    public class Robbery : Callout
    {

        private Vector3 Location;
        private Ped Suspect1;
        private Ped Suspect2;
        private Blip Blip1;
        private Blip Blip2;

        public override bool OnBeforeCalloutDisplayed()
        {
            Location = FritosUtils.GetRandomLocation();
            ShowCalloutAreaBlipBeforeAccepting(Location, 30f);
            AddMinimumDistanceCheck(50f, Location);
            CalloutMessage = "Robbery in progress";
            CalloutPosition = Location;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_ROBBERY_01 CRIME_ROBBERY_04 IN_OR_ON_POSITION", Location);
            CalloutInterfaceAPI.Functions.SendMessage(this, "Proceed with caution");

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() //Init
        {
            Suspect1 = new Ped(Location.Around(5f));
            Suspect1.IsPersistent = true;
            Suspect1.BlockPermanentEvents = true;
            if (new Random().Next(0, 2) == 0) // Randomly assign a weapon
            {
                Suspect1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 100, true);
            }

            Suspect2 = new Ped(Location.Around(5f));
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            Blip WP = new Blip(Location);
            WP.EnableRoute(System.Drawing.Color.Blue);
            WP.Scale = 0.8f;
            WP.Name = "Robbery in progress";
            if (new Random().Next(0, 2) == 0) // Randomly assign a weapon
            {
                Suspect2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 100, true);
            }


            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();


        }

        public override void End()
        {
            base.End();

            //Game.DisplayNotification("Code 4");
            //LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("");

            if (CopPed.Exists()) { CopPed.Dismiss(); }
            if (CopBlip.Exists()) { CopBlip.Delete(); }
            if (CopCar.Exists()) { CopCar.Dismiss(); }

            if (SuspectPed.Exists()) { SuspectPed.Dismiss(); }
            if (SuspectBlip.Exists()) { SuspectBlip.Delete(); }

            Game.LogTrivial("FC | Panic Button Ended");

        }

    }
}
