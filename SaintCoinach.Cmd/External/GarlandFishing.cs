﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumptruck.External.GarlandDB
{
    class GarlandFishing
    {
        public Dictionary<string, GarlandBait> bait { get; set; }

        public List<GarlandFish> fish { get; set; }
    }

    public class GarlandFish
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("patch")]
        public long Patch { get; set; }

        [JsonProperty("bait")]
        public string[] Bait { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("icon")]
        public long Icon { get; set; }

        [JsonProperty("func")]
        public string Func { get; set; }

        [JsonProperty("rarity")]
        public long Rarity { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("lvl")]
        public long Lvl { get; set; }

        [JsonProperty("coords")]
        public double[] Coords { get; set; }

        [JsonProperty("radius")]
        public long Radius { get; set; }

        [JsonProperty("zone")]
        public string Zone { get; set; }
    }

    public class GarlandBait
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("icon")]
        public long Icon { get; set; }
    }
}
