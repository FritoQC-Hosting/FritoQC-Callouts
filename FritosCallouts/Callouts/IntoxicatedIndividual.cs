using System;
using System.Collections.Generic;
using System.Text;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using Rage.Native;

namespace FritosCallouts.Callouts
{
    //[CalloutInfo("IntoxicatedIndividual", CalloutProbability.High)]
    [CalloutInterface("[FQ] Intoxicated Individual", CalloutProbability.High, "Drunk person seen in public.", "Code 2", "LSPD")]

    class IntoxicatedIndividual : Callout
    {
        private Ped Suspect;
        private Blip SuspectBlip;
        private Vector3 Spawnpoint;
        private float heading;
        private int counter;
        private string maleFemale;


        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            heading = 66.64632f;
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = ("Intoxicated Person seen in public");
            CalloutPosition = Spawnpoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", Spawnpoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped("ig_money", Spawnpoint, heading);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Blue;
            SuspectBlip.IsRouteEnabled = true;


            Suspect.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_drinking@beer@male@enter"), "enter", 5f, AnimationFlags.StayInEndFrame).WaitForStatus(TaskStatus.NoTask, 3500); //Starting task

            Suspect.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_drinking@beer@male@base"), "base", 5f, AnimationFlags.Loop);


            if (Main.Debug_Mode) Game.LogTrivial("Suspect spawned");

            if (Main.STP) STP.SetPedDrunk(Suspect, true); else if (Main.Debug_Mode) Game.LogTrivial("Native - Set Drunk"); NativeFunction.Natives.SET_PED_IS_DRUNK(Suspect, true);

            if (Suspect.IsMale)
                maleFemale = "sir";
            else
                maleFemale = "ma'am";

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(Suspect) <= 10f)
            {
                Game.DisplayHelp("Press ~y~Y ~w~to talk to the suspect", false);

                if(Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                {
                    counter++;

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("Officer: Excuse me " + maleFemale + ",  do you need any assistance ?");
                    }
                    if (counter == 2)
                    {
                        Game.DisplaySubtitle("Suspect: *hic* Whats the officer mister the problem... *hic*");
                    }
                    if (counter == 3)
                    {
                        Game.DisplaySubtitle("Officer: How many drink did you had recently ?");
                    }
                    if (counter == 4)
                    {
                        Game.DisplaySubtitle("Suspect: Let me count *hic* 1, 5, 7 ?");
                    }
                    if (counter == 5)
                    {
                        Game.DisplaySubtitle("Officer: It's not legal to be intoxicated in public, I'm going to have to write you a citation.");
                        Suspect.Tasks.PlayAnimation(new AnimationDictionary("amb@world_human_drinking@beer@male@exit"), "exit", 5f, AnimationFlags.None); //Clearing task
                    }
                    if (counter == 6)
                    {
                        Game.DisplaySubtitle("Suspect: Fuck you !");
                        Suspect.Tasks.Wander();
                    }
                    if (counter >= 6)
                    {
                        Game.DisplaySubtitle("Conversation Over");
                    }
                    Game.LogTrivial("End of Conversation");
                }
                
            }

            if (Suspect.IsCuffed || Suspect.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect.Exists())
            {
                End();
            }

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

            Game.LogTrivial("[FritoQC Callouts] Intoxicated Person Ended");

        }
    }
}
