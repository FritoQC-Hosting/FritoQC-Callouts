using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using CalloutInterfaceAPI;
using FritosCallouts.Utility;

namespace FritosCallouts.Callouts
{
    //[CalloutInfo("HighSpeedChase", CalloutProbability.High)]
    [CalloutInterface("Hit and Run", CalloutProbability.High, "Hit and run vehicle spotted in the area.", "Code 3", "LSPD")]

    public class HitAndRun : Callout
    {

        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(800f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Hit and Run vehicle spotted in the area.";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle(FritosUtils.GetRandomVehicleModel(), Spawnpoint);
            SuspectVehicle.IsPersistent = true;
            STP.SetVehicleInsurance(SuspectVehicle, StopThePed.API.STPVehicleStatus.None);

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            STP.SetPedDrunk(Suspect, true);

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = false;

            PursuitCreated = false;

            CalloutInterfaceAPI.Functions.SendMessage(this, $"Vehicle is a {CalloutInterfaceAPI.Functions.GetColorName(SuspectVehicle.PrimaryColor)} on {CalloutInterfaceAPI.Functions.GetColorName(SuspectVehicle.SecondaryColor)} {SuspectVehicle.Model.Name}");

            //TODO- Dommager le vehicle un peu

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 200f)
            {
                Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit();
                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;
            }

            if (PursuitCreated && !LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit))
            {
                End();
            }

        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists())
            {
                //Suspect.Dismiss();
            }
            if (Suspect.Exists())
            {
                SuspectBlip.Delete();
            }
            if (SuspectVehicle.Exists())
            {
                SuspectVehicle.Dismiss();
            }

            Game.LogTrivial("FritoQC Callouts | High Speed Chase Ended");

        }

    }
}
