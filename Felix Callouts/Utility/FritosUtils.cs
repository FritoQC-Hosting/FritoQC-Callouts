using System;
using System.Collections.Generic;

namespace FritosCallouts.Utility
{
    internal static class FritosUtils
    {
        //RANDOM VEHICLE
        private static readonly List<string> VehicleModels = new List<string>
        {
            "ADDER",       // Sports car
            "BUFFALO",     // Muscle car
            "SULTAN",      // Sports sedan
            "F620",        // Sports car
            "ELEGY",       // Sports car
            "COMET",       // Sports car
            "INFERNUS",    // Super car
            "SENTINEL",    // Sports car
            "ZENTORNO",    // Super car
            "WARRENER",    // Compact car
            "BLISTA",      // Compact car
            "DILETTANTE",  // Compact car
            "MANANA",      // Classic car
            "PRIMO",       // Sedan
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
















// FritoLays