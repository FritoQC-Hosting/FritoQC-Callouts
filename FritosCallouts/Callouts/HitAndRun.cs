using CalloutInterfaceAPI;
using FritosCallouts.Utility;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Drawing;
using CIA = CalloutInterfaceAPI.Functions;
// Namespace aliases to prevent ambiguity
using LSPDFR = LSPD_First_Response.Mod.API.Functions;

namespace FritosCallouts.Callouts
{
    [CalloutInterface(
        "Hit and Run",
        CalloutProbability.High,
        "Hit and run vehicle spotted in the area.",
        "Code 3",
        "LSPD"
    )]
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

            LSPDFR.PlayScannerAudioUsingPosition(
                "WE_HAVE CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION",
                Spawnpoint
            );

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            try
            {
                SuspectVehicle = new Vehicle(FritosUtils.GetRandomVehicleModel(), Spawnpoint)
                {
                    IsPersistent = true
                };

                STP.SetVehicleInsurance(SuspectVehicle, "None");

                Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f))
                {
                    IsPersistent = true,
                    BlockPermanentEvents = true
                };
                Suspect.WarpIntoVehicle(SuspectVehicle, -1);

                STP.SetPedDrunk(Suspect, true);

                SuspectBlip = Suspect.AttachBlip();
                SuspectBlip.Color = Color.Red;
                SuspectBlip.IsRouteEnabled = false;

                PursuitCreated = false;

                CIA.SendMessage(
                    this,
                    $"Vehicle is a {CIA.GetColorName(SuspectVehicle.PrimaryColor)} on {CIA.GetColorName(SuspectVehicle.SecondaryColor)} {SuspectVehicle.Model.Name}"
                );
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[FritoQC] Error initializing HitAndRun callout: {ex}");
                End();
                return false;
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            try
            {
                if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 200f)
                {
                    Pursuit = LSPDFR.CreatePursuit();
                    LSPDFR.AddPedToPursuit(Pursuit, Suspect);
                    LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                    PursuitCreated = true;
                }

                if (PursuitCreated && !LSPDFR.IsPursuitStillRunning(Pursuit))
                    End();
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[FritoQC] Error in HitAndRun.Process(): {ex.Message}");
            }
        }

        public override void End()
        {
            base.End();

            try
            {
                if (Suspect.Exists()) SuspectBlip?.Delete();
                if (SuspectVehicle.Exists()) SuspectVehicle.Dismiss();

                Game.LogTrivial("[FritoQC Callouts] Hit and Run ended.");
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[FritoQC] Error ending HitAndRun callout: {ex.Message}");
            }
        }
    }
}
