using System;
using System.Collections.Generic;

namespace FelixsCallouts.Utility
{
    internal static class FelixsUtils
    {
        private static readonly List<string> VehicleModels = new List<string>
        {
            "ADDER",
            "BUFFALO",
            "SULTAN",
            "F620"
            // Add more vehicle models as needed
        };

        private static readonly Random Random = new Random(); // Initialize Random instance

        // Method to return a random vehicle model
        public static string GetRandomVehicleModel()
        {
            if (VehicleModels.Count == 0)
            {
                throw new InvalidOperationException("No vehicle models available.");
            }

            // Return a random vehicle model
            return VehicleModels[Random.Next(VehicleModels.Count)];
        }
    }
}
