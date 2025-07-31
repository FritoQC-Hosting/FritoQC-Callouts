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
        private LHandle Pursuit;
        private bool onScene = false;
        private Blip WP;

        public override bool OnBeforeCalloutDisplayed()
        {
            Location = FritosUtils.GetRandomLocation("clothing");
            ShowCalloutAreaBlipBeforeAccepting(Location, 30f);
            AddMinimumDistanceCheck(50f, Location);
            CalloutMessage = "Robbery in progress";
            CalloutPosition = Location;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_ROBBERY_01 CRIME_ROBBERY_04 IN_OR_ON_POSITION", Location);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() //Init
        {
            Vector3 pos1 = Location.Around(5f);
            Suspect1 = new Ped(pos1);
            Suspect1.Position = new Vector3(pos1.X, pos1.Y, (World.GetGroundZ(pos1, false, false) ?? pos1.Z) + 0.5f);
            Suspect1.IsPersistent = true;
            Suspect1.BlockPermanentEvents = true;

            Vector3 pos2 = Location.Around(5f);
            Suspect2 = new Ped(pos2);
            Suspect2.Position = new Vector3(pos2.X, pos2.Y, (World.GetGroundZ(pos2, false, false) ?? pos2.Z) + 0.5f);
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;


            if (new Random().Next(0, 2) == 0) // Randomly assign a weapon
            {
                Suspect1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 100, true);
            }

            if (new Random().Next(0, 2) == 0) // Randomly assign a weapon
            {   
                Suspect2.Inventory.GiveNewWeapon("WEAPON_PISTOL", 100, true);
            }

            WP = new Blip(Location);
            WP.Position = Location;
            WP.Color = System.Drawing.Color.Red;
            WP.EnableRoute(System.Drawing.Color.Red);
            WP.Scale = 0.8f;
            WP.Name = "Robbery in progress";

            CalloutInterfaceAPI.Functions.SendMessage(this, "Proceed with caution\nSuspects may be armed");
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("UNITS_RESPOND_CODE_03_02");


            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(Location) <= 100f && !onScene)
            {
                WP.Delete();
                //Blip1 = Suspect1.AttachBlip();
                //Blip1.Color = Color.Red;
                //Blip1.Flash(1000, 10000);

                //Blip2 = Suspect2.AttachBlip();
                //Blip2.Color = Color.Red;
                //Blip2.Flash(1000, 10000);

                Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit(); //Making prusuit
                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect1);
                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect2);
                LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                
                Suspect1.Tasks.Flee(Game.LocalPlayer.Character, 200f, -1);
                Suspect2.Tasks.Flee(Game.LocalPlayer.Character, 200f, -1);
                
                onScene = true;
            }

            if (onScene && Suspect2.Inventory.HasLoadedWeapon && Game.LocalPlayer.Character.DistanceTo(Suspect1) <= 10f)
            {
                if (Suspect1.IsAlive)
                {
                    Suspect1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }

            if (onScene && Suspect2.Inventory.HasLoadedWeapon && Game.LocalPlayer.Character.DistanceTo(Suspect2) <= 15f)
            {
                if (Suspect2.IsAlive)
                {
                    Suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                }
            }


            if (onScene && !Suspect1.IsAlive && !Suspect2.IsAlive)
            {
                FritosUtils.Debug("Robbery suspects dead");
                End();
            }

        }

        public override void End()
        {
            base.End();

            if (Suspect1.Exists()) { Suspect1.Dismiss(); }
            if (Suspect2.Exists()) { Suspect2.Dismiss(); }
            if (Blip1.Exists()) { Blip1.Delete(); }
            if (Blip2.Exists()) { Blip2.Delete(); }
            if(WP.Exists()) { WP.Delete(); }

            if (LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit)) { LSPD_First_Response.Mod.API.Functions.ForceEndPursuit(Pursuit); }

            FritosUtils.Debug("Robbery Ended");
        }

    }
}
