﻿using EnsoulSharp.Common;

namespace HyperCarryNautilus
{
    internal class CommonUtilities
    {
        public static HitChance GetHitChance(string name)
        {
            var hitChance = ConfigMenu.config.Item(name).GetValue<StringList>();

            switch (hitChance.Items[hitChance.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }
    }
}