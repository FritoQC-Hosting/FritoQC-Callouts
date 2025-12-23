using CalloutInterfaceAPI;
using FritosCallouts.Callouts;
using LSPD_First_Response.Engine;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Text;

namespace FritosCallouts.Callouts
{
    [CalloutInterface("[FQ] JewelleryRobbery", CalloutProbability.Low, "Jewellery Robbery in progress", "Code 3", "")]
    internal class JewelleryRobbery : Callout
    {
        private Vector3 SceneLocation = new Vector3(-632.1814f, -236.8454f, 38.04819f);

        private Ped SuspectOne;
        private Ped SuspectTwo;
        private Ped SuspectThree;

        private Vehicle BikeOne;
        private Vehicle BikeTwo;
        private Vehicle BikeThree;

        private LHandle Pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            ShowCalloutAreaBlipBeforeAccepting(SceneLocation, 30f);
            AddMinimumDistanceCheck(30f, SceneLocation);
            CalloutMessage = "Jewellery Robbery in progress.";
            CalloutPosition = SceneLocation;
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