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
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Linq;

namespace FritosCallouts.Callouts
{
    [CalloutInterface("Air one needing ground units", CalloutProbability.Medium, "Air one requesting ground units.", "Code 2", "LSPD")]

    public class HeliPursuit : Callout
    {

        private Vector3 Location;
        private Ped Suspect;
        private Vehicle Vehicle;
        private Vehicle Heli;
        private LHandle Pursuit;
        private bool onScene = false;
        private Blip WP;
        private Ped Cop;

        public override bool OnBeforeCalloutDisplayed()
        {
            Location = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));
            ShowCalloutAreaBlipBeforeAccepting(Location, 50f);
            AddMinimumDistanceCheck(50f, Location);
            CalloutMessage = "Air one requesting ground units.";
            CalloutPosition = Location;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 ASSISTANCE_REQUIRED_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION", Location);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() //Init
        {
            Suspect = new Ped(Location.Around(2f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;    
            Vehicle = new Vehicle(FritosUtils.GetRandomVehicleModel(), Location.Around(2f));
            Suspect.WarpIntoVehicle(Vehicle, -1);
            FritosUtils.Debug("Suspect spawned in vehicle");

            Cop = new Ped(Location.Around(2f));
            Heli = new Vehicle("POLMAV", new Vector3(Location.X, Location.Y, Location.Z + 50f));
            Heli.IsEngineOn = true;
            Cop.WarpIntoVehicle(Heli, -1);
            Cop.IsPersistent = true;
            Cop.BlockPermanentEvents = true;
            Heli.IsPersistent = true;
            Heli.IsEngineOn = true;
            Heli.IsPositionFrozen = true; // Freeze the heli in place
            FritosUtils.Debug("Cop spawned in heli");


            if (new Random().Next(0, 2) == 0) // Randomly assign a weapon
            {
                Suspect.Inventory.GiveNewWeapon(WeaponHash.CombatPistol, 100, true);
                FritosUtils.Debug("Suspect given weapon");

            }


            //WP = new Blip(Location);
            //WP.Color = System.Drawing.Color.Blue;
            //WP = Suspect.AttachBlip();
            //WP.EnableRoute(System.Drawing.Color.Blue);
            //WP.Scale = 0.8f;
            //WP.Name = "Air one";
            //FritosUtils.Debug("WP Created and Attached");

            CalloutInterfaceAPI.Functions.SendMessage(this, $"Vehicle is a {CalloutInterfaceAPI.Functions.GetColorName(Vehicle.PrimaryColor)} on {CalloutInterfaceAPI.Functions.GetColorName(Vehicle.SecondaryColor)} {Vehicle.Model.Name}");          
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("UNITS_RESPOND_CODE_02_02");


            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(Location) <= 300f && !onScene)
            {
                //WP.IsRouteEnabled = false;
                //WP.Delete();

                Heli.IsPositionFrozen = false; // Unfreeze the heli
                Pursuit = LSPD_First_Response.Mod.API.Functions.CreatePursuit(); //Making prusuit
                LSPD_First_Response.Mod.API.Functions.AddPedToPursuit(Pursuit, Suspect);
                LSPD_First_Response.Mod.API.Functions.AddCopToPursuit(Pursuit, Cop);
                LSPD_First_Response.Mod.API.Functions.SetPursuitIsActiveForPlayer(Pursuit, true);

                LSPD_First_Response.Mod.API.Functions.SetPedResistanceChance(Suspect, 1f);
                onScene = true;
            }

            if (onScene && !Suspect.IsAlive)
            {
                FritosUtils.Debug("Heli Pursuit Suspect dead");
                End();
            }

        }

        public override void End()
        {
            base.End();

            if (Suspect.Exists()) { Suspect.Dismiss(); }
            if (Cop.Exists()) { Cop.Dismiss(); }
            if (Vehicle.Exists()) { Vehicle.Dismiss(); }
            if (Heli.Exists()) { Heli.Dismiss(); }
            if (WP.Exists()) { WP.Delete(); }

            if (LSPD_First_Response.Mod.API.Functions.IsPursuitStillRunning(Pursuit)) { LSPD_First_Response.Mod.API.Functions.ForceEndPursuit(Pursuit); }

            FritosUtils.Debug("Robbery Ended");
        }

    }
}
