using CalloutInterfaceAPI;
using FritosCallouts.Utility;
using LSPD_First_Response.Engine;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Security.Policy;
using CIA = CalloutInterfaceAPI.Functions;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;

namespace FritosCallouts.Callouts
{
    [CalloutInterface("[FQ] JewelleryRobbery", CalloutProbability.Low, "Jewellery Robbery in progress", "Code 3", "")]
    internal class JewelleryRobbery : Callout
    {
        private Vector3 SceneLocation = new Vector3(-632.1814f, -236.8454f, 38.04819f);

        private Ped[] _suspects;
        private Vehicle[] _bikes;
        private LHandle Pursuit;
        private bool PursuitCreated = false;
        private Blip Sceneblip;

        public override bool OnBeforeCalloutDisplayed()
        {
            ShowCalloutAreaBlipBeforeAccepting(SceneLocation, 10f);
            AddMinimumDistanceCheck(10f, SceneLocation);
            CalloutMessage = "Jewellery Robbery in progress.";
            CalloutPosition = SceneLocation;

            LSPDFR.PlayScannerAudioUsingPosition(
                "ATTENTION_ALL_UNITS_02 WE_HAVE CRIME_ROBBERY_01 IN_OR_ON_POSITION SUSPECTS_ARE_01 ON_01 VEHICLE_CATEGORY_BICYCLE_01 UNITS_RESPOND_CODE_03_02",
                CalloutPosition
            );

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            _suspects = new Ped[3];
            _suspects[0] = new Ped("player_zero", new Vector3(-632.1973f, -236.8982f, 38.04816f), 140.5453f);
            _suspects[1] = new Ped(new Vector3(-632.1973f, -236.8982f, 38.04816f), 140.5453f);
            _suspects[2] = new Ped(new Vector3(-632.1973f, -236.8982f, 38.04816f), 140.5453f);
            
            _bikes = new Vehicle[3];
            _bikes[0] = new Vehicle("bati2", new Vector3(-636.9227f, -236.1809f, 37.36633f), 54.26329f);
            _bikes[1] = new Vehicle("bati", new Vector3(-638.1968f, -241.4963f, 37.56949f), 42.1154f);
            _bikes[2] = new Vehicle("bati", new Vector3(-634.3521f, -240.5563f, 37.54824f), 62.36589f);

            Sceneblip = new Blip(SceneLocation, 30f);
            Sceneblip.Color = Color.Yellow;
            Sceneblip.EnableRoute(Color.Yellow);

            foreach (Ped suspect in _suspects)
            {
                suspect.IsPersistent = true;
                suspect.BlockPermanentEvents = true;
                suspect.Inventory.GiveNewWeapon(WeaponHash.CarbineRifle, 500, true);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            try
            {
                if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SceneLocation) <= 150f)
                {
                    Pursuit = LSPDFR.CreatePursuit();
                    PursuitCreated = true;

                    for (int i = 0; i < _suspects.Length; i++)
                    {
                        if (_suspects[i].Exists() && _bikes[i].Exists())
                        {
                            _suspects[i].GiveHelmet(false, HelmetTypes.RegularMotorcycleHelmet, 0);
                            _suspects[i].WarpIntoVehicle(_bikes[i], -1);
                            LSPDFR.AddPedToPursuit(Pursuit, _suspects[i]);
                        }
                    }
                    LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                    LSPDFR.RequestBackup(SceneLocation, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.AirUnit);
                    CIA.SendMessage(this, "Air Unit Dispatched");
                    LSPDFR.PlayScannerAudio("HELI_APPROACHING_DISPATCH_02");
                    Sceneblip.DisableRoute();
                    Sceneblip.Delete();
                }

                    if (PursuitCreated && !LSPDFR.IsPursuitStillRunning(Pursuit)) { End(); }
            } 
            catch (Exception ex)
            {
                Game.LogTrivial($"[FritoQC Callouts] Error in JewelleryRobbery.Process(): {ex.Message}");
            }

        }

        public override void End()
        {
            base.End();
            try
            {
                Game.DisplayNotification("Code 4");
                foreach (Ped suspect in _suspects)
                {
                    if (suspect.Exists()) suspect.Dismiss();
                }

                foreach (Vehicle bike in _bikes)
                {
                    if (bike.Exists()) bike.Dismiss();
                }

                Game.LogTrivial("[FritoQC Callouts] Jewellery Robbery ended.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[FritoQC Callouts] Error ending JewelleryRobbery callout: {ex.Message}");
            }
        }

    }
}