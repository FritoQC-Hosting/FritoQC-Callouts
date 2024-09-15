using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using CalloutInterfaceAPI;

namespace FelixsCallouts.Callouts
{
    //[CalloutInfo("GunShotsReported", CalloutProbability.High)]
    [CalloutInterface("CALLOUTNAME", CalloutProbability.Low, "DESCIPTION", "Code 3", "LSPD")]


    class TEMPLATE : Callout
    {

        private Ped Suspect;
        private Blip SuspectBlip;
        private Vector3 Spawnpoint;

        public override bool OnBeforeCalloutDisplayed()
        {

            //Spawnpoint = World.GetRandomPositionOnStreet();
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "MDT MESSAGE BEFORE ACCEPTING";
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 WE_HAVE CRIME_GUNFIRE_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
        }

        public override void End()
        {
            base.End();

        }

    }
}
