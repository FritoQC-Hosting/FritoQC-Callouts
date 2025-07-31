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
                new Vector3(29.07f, -1351.26f, 29.33f),    // 24/7 Supermarket
                new Vector3(-54.33f, -1758.2f, 29.09f),    // LTD Gasoline
                new Vector3(1142.64f, -980.99f, 46.19f),   // Rob's Liquor
                new Vector3(1159.95f, -328.84f, 69.05f),   // 24/7 Mirror Park
                new Vector3(2562.28f, 385.35f, 108.62f),   // 24/7 Sandy Shores
            },
            ["market"] = new List<Vector3>
            {
                new Vector3(376.00f, 321.36f, 103.43f),    // Vinewood Market
                new Vector3(-1492.75f, -385.20f, 39.87f),  // Vespucci Market
                new Vector3(-1227.84f, -900.36f, 12.28f),  // Del Perro Market
            },
            ["clothing"] = new List<Vector3>
            {
                new Vector3(85.15f, -1391.14f, 29.29f),      // Suburban
                new Vector3(-718.53f, -157.93f, 36.99f),     // Ponsonbys
                new Vector3(414.62f, -807.78f, 29.33f),      // Binco
                new Vector3(-1206.07f, -781.57f, 17.10f),    // Suburban West
                new Vector3(-2.52f, 6518.33f, 31.48f),       // Paleto Bay Clothing
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
