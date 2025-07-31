using System;
using System.Collections.Generic;
using System.Linq;
using Rage;

namespace FritosCallouts.Utility
{
    internal static class FritosUtils
    {
        // --- Random vehicle list ---
        private static readonly List<string> VehicleModels = new List<string>
        {
            "ADDER", "BUFFALO", "SULTAN", "F620", "ELEGY", "COMET", "INFERNUS",
            "SENTINEL", "ZENTORNO", "WARRENER", "BLISTA", "DILETTANTE", "MANANA", "PRIMO"
        };

        private static readonly Random Random = new Random();

        public static string GetRandomVehicleModel()
        {
            if (VehicleModels.Count == 0)
                throw new InvalidOperationException("No vehicle models available.");

            return VehicleModels[Random.Next(VehicleModels.Count)];
        }

        public static void Debug(string message)
        {
            if (Main.Debug_Mode)
            {
                Game.LogTrivial($"[FritoQCCallouts] : {message}");
            }
        }

        // --- Location list by category ---
        private static readonly Dictionary<string, List<Vector3>> LocationCategories = new Dictionary<string, List<Vector3>>
        {
            ["store"] = new List<Vector3>
            {
                new Vector3(24.13f, -1345.15f, 29.5f),     // 24/7 Supermarket
                new Vector3(-47.52f, -1756.88f, 29.42f),   // LTD Gasoline
                new Vector3(1135.8f, -982.28f, 46.42f),    // Rob's Liquor
                new Vector3(1165.15f, -324.5f, 69.21f),    // 24/7 Mirror Park
                new Vector3(2557.98f, 382.28f, 108.62f),   // 24/7 Sandy Shores
            },
            ["market"] = new List<Vector3>
            {
                new Vector3(373.87f, 325.94f, 103.57f),    // Vinewood Market
                new Vector3(-1487.72f, -379.27f, 40.16f),  // Vespucci Market
                new Vector3(-1222.77f, -908.03f, 12.33f),  // Del Perro Market
            },
            ["clothing"] = new List<Vector3>
            {
                new Vector3(85.15f, -1391.14f, 29.29f),      // Suburban 85.15646, -1391.14, 29.29269 -94.90337 2.934158E-06 -0.0001901247
                //new Vector3(-710.15f, -153.26f, 37.42f),   // Ponsonbys
                //new Vector3(425.6f, -806.25f, 29.5f),      // Binco
                //new Vector3(-1193.42f, -767.29f, 17.32f),  // Suburban West
                //new Vector3(5.26f, 6510.89f, 31.88f),      // Paleto Bay Clothing
            }
        };


        public static Vector3 GetRandomLocation(string category = null)
        {
            List<Vector3> available;

            if (!string.IsNullOrEmpty(category) && LocationCategories.TryGetValue(category.ToLower(), out available))
            {
                return available[Random.Next(available.Count)];
            }

            // If no category or invalid category, return from all
            var allLocations = LocationCategories.SelectMany(kv => kv.Value).ToList();
            if (allLocations.Count == 0)
                throw new InvalidOperationException("No locations available.");

            return allLocations[Random.Next(allLocations.Count)];
        }
    }
}
