using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace FelixsCallouts.Callouts
{
    [CalloutInfo("PanicButton", CalloutProbability.High)]

    class GunShotsReported : Callout
    {

        private Ped Suspect;
        private Blip SuspectBlip;
        private Vector3 Spawnpoint;
        private float heading;

        public override bool OnBeforeCalloutDisplayed()
        {

            //Spawnpoint = World.GetRandomPositionOnStreet();
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            heading = 66.64632f;
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Panic Button Triggered";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 CRIME_OFFICER_IN_NEED_OF_ASSISTANCE_04 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", Spawnpoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped("s_f_y_cop_01", Spawnpoint, heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Kill = true;

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            

        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }
            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }

            Game.LogTrivial("FelixsCallouts | Panic Button Ended");

        }

    }
}
