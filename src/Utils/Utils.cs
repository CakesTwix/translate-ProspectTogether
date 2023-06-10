﻿using Newtonsoft.Json;
using ProspectorInfo.Models;
using System.Collections.Generic;
using Vintagestory.API.Util;

namespace ProspectorInfo.Utils
{
    internal class OreNames : Dictionary<string, string>
    {
        public OreNames()
        {
            IDictionary<string, string> oreValues = Vintagestory.API.Config.Lang.GetAllEntries();
            // game:ore-lapis is a leftover and unused so it can be removed. See https://discord.com/channels/302152934249070593/351624415039193098/1009372460568805427
            oreValues.RemoveAll((key, val) => !key.Contains(":ore-") || key.CountChars('-') != 1 || key.Contains("_") || key == "game:ore-lapis");
            foreach (var elem in oreValues)
                if (!TryGetValue(elem.Value, out string _)) // Ores with the same translation will be saved under the same tag
                    Add(elem.Value, elem.Key);
        }
    }

    public enum MapMode
    {
        Default,
        Heatmap
    }

}
