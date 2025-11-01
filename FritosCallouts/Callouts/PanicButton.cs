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
    //[CalloutInfo("PanicButton", CalloutProbability.Low)]
    [CalloutInterface("[FQ] Panic Button", CalloutProbability.Low, "Officer in distress", "Code 99", "")]
    
    public class PanicButton : Callout
    {

        private Ped CopPed;
        private Vehicle CopCar;
        private Blip CopBlip;
        private Vector3 Spawnpoint;
        private float Heading;

        private Ped SuspectPed;
        private Blip SuspectBlip;
        private Vector3 SuspectSpawn;
        private bool SuspectSpawned = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Panic Button Triggered";
            CalloutPosition = Spawnpoint;
            Heading = 66.64632f;

            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_04 IN_OR_ON_POSITION", Spawnpoint);
            CalloutInterfaceAPI.Functions.SendMessage(this, "Officer in distress\nCode 99");


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() //Init
        {
            CopCar = new Vehicle("POLICE", Spawnpoint);
            CopCar.IsPersistent = true;

            CopPed = new Ped("s_f_y_cop_01", CopCar.GetOffsetPositionFront(5f), Heading);
            CopPed.IsPersistent = true;
            CopPed.BlockPermanentEvents = true;
            CopPed.WarpIntoVehicle(CopCar, -1);
            CopPed.Tasks.CruiseWithVehicle(CopCar, 10f, VehicleDrivingFlags.Normal);

            CopBlip = CopPed.AttachBlip();
            CopBlip.Color = System.Drawing.Color.Blue;
            CopBlip.IsRouteEnabled = true;

            //Game.DisplayHelp("This Callout will not end it self unless you revive the ped or call the coroner", false);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
             
            if (Game.LocalPlayer.Character.DistanceTo(CopPed) <= 300f) //Kill the cop in the car
            {
                if (CopPed.IsAlive)
                {
                    CopCar.Explode();
                    
                    CopPed.Kill();
                }
            }

            if (Game.LocalPlayer.Character.DistanceTo(CopPed) <= 100f) // Spawn Suspect
            {   
                if (SuspectSpawned == true) { return; }

                SuspectSpawn = World.GetNextPositionOnStreet(CopPed.Position.Around(150f));
                SuspectPed = new Ped(SuspectSpawn);
                SuspectPed.IsPersistent = true;
                SuspectPed.BlockPermanentEvents = true;
                SuspectPed.Tasks.ReactAndFlee(Game.LocalPlayer.Character);
                SuspectPed.Inventory.GiveNewWeapon("WEAPON_MINISMG", 100, true);
                STP.InjectPedItems(SuspectPed);
                CalloutInterfaceAPI.Functions.SendMessage(this, "Suspect reported seen in the area, proceed with caution.");
                LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 SUSPECT_LAST_SEEN_01 ON_FOOT_02 CLOSE_TO_01", SuspectPed.Position);
                SuspectBlip = SuspectPed.AttachBlip();
                SuspectPed.Tasks.FightAgainst(Game.LocalPlayer.Character);
                if (SuspectPed.IsAlive) { SuspectSpawned = true; }
            }

            if (SuspectSpawned && SuspectPed.IsDead)
            {
                End();
            }
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

            Game.LogTrivial("[FritoQC Callouts] Panic Button Ended");

        }

    }
}
