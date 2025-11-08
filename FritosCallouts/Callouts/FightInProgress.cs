using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using CalloutInterfaceAPI;
using FritosCallouts.Utility;

namespace FritosCallouts.Callouts
{
    //[CalloutInfo("GunShotsReported", CalloutProbability.High)]
    [CalloutInterface("[FQ] Fight In Progress", CalloutProbability.Medium, "2 Individuals fighting", "Code 3", "")]


    class FightInProgress : Callout
    {

        private Ped Suspect1;
        private Ped Suspect2;
        private Blip SuspectBlip1;
        private Blip SuspectBlip2;
        private Vector3 Spawnpoint;
        private bool Notified;

        public override bool OnBeforeCalloutDisplayed()
        {
            Game.LogTrivialDebug("FIGHT IN PROGRESS BEFORE CALLOUT DISPLAYED");
            //Spawnpoint = World.GetRandomPositionOnStreet();
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(800f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 15f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "2 Individuals reported fighting in public";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect1 = new Ped(Spawnpoint);
            Suspect1.IsPersistent = true;
            Suspect1.BlockPermanentEvents = true;

            Suspect2 = new Ped(Spawnpoint);
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            STP.SetPedDrunk(Suspect2, true);

            SuspectBlip1 = Suspect1.AttachBlip();
            SuspectBlip1.Color = System.Drawing.Color.Red;
            SuspectBlip1.IsRouteEnabled = false;

            SuspectBlip2 = Suspect2.AttachBlip();
            SuspectBlip2.Color = System.Drawing.Color.DarkRed;
            SuspectBlip2.IsRouteEnabled = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(Suspect1) <= 200f && !Notified)
            {
                CalloutInterfaceAPI.Functions.SendMessage(this, "Let dispatch know when to end callout!");
                Notified = true;
            }

            if (Game.LocalPlayer.Character.DistanceTo(Suspect1) <= 50f)
            {
                Suspect1.Tasks.FightAgainst(Suspect2);
                Suspect2.Tasks.FightAgainst(Suspect1);
            }
        }

        public override void End()
        {
            base.End();

            if (Suspect1.Exists() && Suspect2.Exists())
            {
                Suspect1.Dismiss();
                Suspect2.Dismiss();

                Suspect1.Delete();
                Suspect2.Delete();
            }
        }

    }
}
