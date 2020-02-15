﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tharga.Toolkit.Console.Command.Base;

namespace SaintCoinach.Cmd.Commands {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3 {
        public Vector3(float x, float y, float z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Vector3(string commaseperated) {
            string[] array = commaseperated.Split(new char[]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries);
            this.X = float.Parse(array[0], CultureInfo.InvariantCulture);
            this.Y = float.Parse(array[1], CultureInfo.InvariantCulture);
            this.Z = float.Parse(array[2], CultureInfo.InvariantCulture);
        }

        public float X;
        public float Y;
        public float Z;
    }

    public class LeveCommand : ActionCommandBase {

        private ARealmReversed _Realm;

        public LeveCommand(ARealmReversed realm)
            : base("leve", "Builds Leve Database") {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            if (string.IsNullOrEmpty(paramList) || !File.Exists(paramList))
            {
                OutputInformation($"Please specify db location {paramList}");
                return false;
            }
            if(File.Exists($"{paramList}.backup"))
                File.Delete($"{paramList}.backup");
            File.Copy(paramList, $"{paramList}.backup");
            var existing = JsonConvert.DeserializeObject<LeveDatabase>(File.ReadAllText(paramList));

            var items = _Realm.GameData.GetSheet<SaintCoinach.Xiv.CraftLeve>();
            var levenpcs = _Realm.GameData.GetSheet < SaintCoinach.Xiv.Level>();
            foreach (var ir in items)
            {
                var t = ir.Leve;
                if (t.LeveAssignmentType.Key > 4 && t.LeveAssignmentType.Key < 13 && existing.Leves.All(i => i.LeveId != t.Key))
                {
                    var item = ir.Items.First().Item;
                    var next = new Leve
                    {
                        Classes = t.LeveAssignmentType.Name,
                        ItemId = item.Key,
                        ItemName = item.Name,
                        LeveId = t.Key,
                        //Level = t.CharacterLevel, //t.StartLevel.Map.TerritoryType.Key,
                        Name = t.Name,
                        NumItems = ir.Items.Sum(i => i.Count),
                        //PickUpNpc = (t.ZonePlaceName.Key == 2404) ? 1018997 : 0,
                        TurnInNpc = t.LevemeteLevel.Object.Key 
                    };
                    if(next.PickUpNpc == 0)
                        continue;
                    
                    existing.Leves.Add(next);
                    OutputInformation($"Added Leve # {next.LeveId} - {next.Name} - {next.Classes}");
                    if (existing.Npcs.All(i => i.NpcId != next.TurnInNpc))
                    {
                        var x = levenpcs.First(i => i.Object.Key  == next.TurnInNpc);

                        var nnpc = new LeveNpc
                        {
                            LocationName = x.Map.PlaceName.Name,
                            MapId = x.Map.Key,
                            NpcId = next.TurnInNpc,
                            Pos = new Vector3(x.X, x.Y, x.Z)
                        };
                        OutputInformation($"Added Turn In NPC # {nnpc.NpcId} - {nnpc.LocationName} - {nnpc.Pos}");
                        existing.Npcs.Add(nnpc);
                    }

                    if (next.PickUpNpc != 0 && existing.Npcs.All(i => i.NpcId != next.PickUpNpc)) {
                        var x = levenpcs.First(i => i.Object.Key == next.PickUpNpc);

                        var nnpc = new LeveNpc {
                            LocationName = x.Map.PlaceName.Name,
                            MapId = x.Map.Key,
                            NpcId = next.PickUpNpc,
                            Pos = new Vector3(x.X, x.Y, x.Z)
                        };
                        OutputInformation($"Added Turn In NPC # {nnpc.NpcId} - {nnpc.LocationName} - {nnpc.Pos}");
                        existing.Npcs.Add(nnpc);
                    }
                }
            }

            OutputInformation($"Validating NPC data...");
            var copy = new List<LeveNpc>();
            
            foreach(var nnpc in existing.Npcs)
            {
                OutputInformation($"Checking {nnpc.NpcId}");
                var x = levenpcs.FirstOrDefault(i => i.Object.Key == nnpc.NpcId);
                if (x == null)
                    continue;
                OutputInformation($"Found {levenpcs.Count(i => i.Object.Key == nnpc.NpcId)}");

                if (nnpc.Pos.X != x.X || nnpc.Pos.Y != x.Y || nnpc.Pos.Z != x.Z || nnpc.MapId != x.Map.Key)
                {
                    
                    OutputInformation($"Updating {nnpc.NpcId}");
                    OutputInformation($"Updating {x.X} {x.Y} {x.Z}");
                    OutputInformation($"Updating {x.Map.Key}");
                    OutputInformation($"Updating {x.Map.PlaceName.Name}");
                    copy.Add(new LeveNpc {
                        LocationName = x.Map.PlaceName.Name,
                        MapId = x.Map.Key,
                        NpcId = x.Object.Key,
                        Pos = new Vector3(x.X, x.Y, x.Z)
                    });
                    OutputInformation($"Done Updating {nnpc.NpcId}");
                }
            }
            OutputInformation($"Before Remove");
            existing.Npcs.RemoveAll(x => copy.Any(i => i.NpcId == x.NpcId));
            OutputInformation($"After Remove");
            existing.Npcs.AddRange(copy);
            OutputInformation($"After Add Range");
            File.WriteAllText(paramList, JsonConvert.SerializeObject(existing, Formatting.Indented));

            return true;
        }

        public class LeveDatabase {
            public List<Leve> Leves;
            public List<LeveNpc> Npcs;
        }

        public class Leve {
            public int LeveId { get; set; }
            public string Name { get; set; }
            public string Classes { get; set; }
            public int Level { get; set; }
            public int PickUpNpc { get; set; }
            public int TurnInNpc { get; set; }
            public int NumItems { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }

        }

        public class LeveNpc {
            public int NpcId { get; set; }
            public Vector3 Pos { get; set; }
            public int MapId { get; set; }
            public string LocationName { get; set; }
        }
    }
}
