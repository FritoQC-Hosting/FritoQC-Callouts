using Rage;
using System;
using System.Linq;
using System.Reflection;

namespace FritosCallouts
{
    public static class STP
    {
        private static bool SafeCheck(string action)
        {
            if (!Main.STP)
            {
                Game.LogTrivial($"[FritoQC Callouts] StopThePed not installed, skipping '{action}'.");
                return false;
            }

            if (Main.Debug_Mode)
                Game.LogTrivial($"[FritoQC Callouts] STP - {action}");

            return true;
        }
        private static Type GetSTPFunctions()
        {
            var stpAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "StopThePed");
            if (stpAssembly == null) return null;

            return stpAssembly.GetType("StopThePed.API.Functions");
        }

        public static void SetPedDrunk(Ped ped, bool state)
        {
            if (!SafeCheck("SetPedDrunk")) return;

            try
            {
                var functionsType = GetSTPFunctions();
                if (functionsType == null) return;

                var method = functionsType.GetMethod("setPedAlcoholOverLimit", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, new object[] { ped, state });
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Failed to call SetPedDrunk via StopThePed.");
            }
        }

        public static void SetPedUnderDrugInfluence(Ped ped, bool state)
        {
            if (!SafeCheck("SetPedUnderDrugInfluence")) return;

            try
            {
                var functionsType = GetSTPFunctions();
                if (functionsType == null) return;

                var method = functionsType.GetMethod("setPedUnderDrugsInfluence", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, new object[] { ped, state });
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Failed to call SetPedUnderDrugInfluence via StopThePed.");
            }
        }

        public static void SetVehicleInsurance(Vehicle veh, object stpVehicleStatus)
        {
            if (!SafeCheck("SetVehicleInsurance")) return;

            try
            {
                var functionsType = GetSTPFunctions();
                if (functionsType == null) return;

                var method = functionsType.GetMethod("setVehicleInsuranceStatus", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, new object[] { veh, stpVehicleStatus });
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Failed to call SetVehicleInsurance via StopThePed.");
            }
        }

        public static void SetVehicleRegistration(Vehicle veh, object stpVehicleStatus)
        {
            if (!SafeCheck("SetVehicleRegistration")) return;

            try
            {
                var functionsType = GetSTPFunctions();
                if (functionsType == null) return;

                var method = functionsType.GetMethod("setVehicleRegistrationStatus", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, new object[] { veh, stpVehicleStatus });
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Failed to call SetVehicleRegistration via StopThePed.");
            }
        }

        public static void InjectPedItems(Ped ped)
        {
            if (!SafeCheck("InjectPedItems")) return;

            try
            {
                var functionsType = GetSTPFunctions();
                if (functionsType == null) return;

                var method = functionsType.GetMethod("injectPedSearchItems", BindingFlags.Public | BindingFlags.Static);
                method?.Invoke(null, new object[] { ped });
            }
            catch
            {
                Game.LogTrivial("[FritoQC Callouts] Failed to call InjectPedItems via StopThePed.");
            }
        }
    }
}
