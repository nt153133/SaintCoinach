﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumptruck.External.GarlandDB
{
    public partial class GarlandDBNode
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("func")]
        public string Func { get; set; }

        [JsonProperty("items")]
        public GarlandDBItem[] Items { get; set; }

        [JsonProperty("stars")]
        public long Stars { get; set; }

        [JsonProperty("time")]
        public uint[] Time { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("zone")]
        public string Zone { get; set; }

        [JsonProperty("coords")]
        public long[] Coords { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uptime")]
        public uint Uptime { get; set; }

        [JsonProperty("lvl")]
        public long Lvl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("patch")]
        public long Patch { get; set; }
    }

    public partial class GarlandDBItem
    {
        [JsonProperty("item")]
        public string ItemItem { get; set; }

        [JsonProperty("icon")]
        public long Icon { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("slot")]
        public string Slot { get; set; }
    }
}
