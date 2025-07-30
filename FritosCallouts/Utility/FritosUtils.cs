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

        // --- Location list by category ---
        private static readonly Dictionary<string, List<Vector3>> LocationCategories = new Dictionary<string, List<Vector3>>
        {
            ["store"] = new List<Vector3>
            {
                new Vector3(24.13f, -1345.15f, 29.5f),   // 24/7 Supermarket
                new Vector3(-47.52f, -1756.88f, 29.42f), // LTD Gasoline
            },
            ["market"] = new List<Vector3>
            {
                new Vector3(373.87f, 325.94f, 103.57f),  // Vinewood Market
            },
            ["clothing"] = new List<Vector3>
            {
                new Vector3(72.3f, -1399.1f, 29.38f),    // Suburban
                new Vector3(-710.15f, -153.26f, 37.42f), // Ponsonbys
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
