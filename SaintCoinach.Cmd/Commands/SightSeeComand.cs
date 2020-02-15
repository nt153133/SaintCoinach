
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using SaintCoinach.Xiv;
    using Tharga.Toolkit.Console.Command.Base;

    namespace SaintCoinach.Cmd.Commands
    {
        public class SightSeeComand : ActionCommandBase
        {
            private ARealmReversed _Realm;

            public SightSeeComand(SaintCoinach.ARealmReversed realm) : base("Sight", "Sight see generation")
            {
                _Realm = realm;
            }

            private List<SightSeePoint> points = new List<SightSeePoint>();

            public override Task<bool> InvokeAsync(string paramList)
            {
                var adventures = _Realm.GameData.GetSheet<Adventure>();
                var emote = _Realm.GameData.GetSheet("TextCommand");
                StringBuilder output = new StringBuilder();


                foreach (var adventure in adventures.Where(i=> i.MaxTime ==0 && i.MinTime ==0))
                //foreach (var adventure in adventures)
                {
                    int emoteKey = (int) adventure.Emote.GetRaw("TextCommand");
                    int aeKey = (int) adventure.Level.Map.TerritoryType.GetRaw("Aetheryte");
                    
                    //OutputInformation($"{adventure.Name} {emote[emoteKey].AsString("Command")} {adventure.Level.X} {adventure.Level.Y} {adventure.Level.Z}");
                    var point = new SightSeePoint(adventure.Name, emote[emoteKey].AsString("Command"), new Vector3(adventure.Level.X, adventure.Level.Y, adventure.Level.Z), adventure.Level.Map.TerritoryType.Key, aeKey, adventure.Key);
                    points.Add(point);
                    OutputInformation(point.ToString() + " AE: " + aeKey);
                }

                output.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                output.AppendLine("<Profile>");
                output.AppendLine("\t<Name>[OrderBot] Sightseeing Log (Non-Timed)</Name>");
                output.AppendLine("\t<BehaviorDirectory>..\\Quest Behaviors</BehaviorDirectory>");
                output.AppendLine("\t<Order>");

                foreach (var point in points)
                {
                    output.Append(point.GenerateGoTo());
                }
                output.AppendLine("\t</Order>");
                output.AppendLine("\t<CodeChunks>");
                
                foreach (var cmds in points.GroupBy(i => i.EmoteCommand1).Select(grp => grp.First()))
                {
                    output.Append(Generate_CodeChunkEmote(cmds.EmoteCommand1));
                }
                output.AppendLine("\t</CodeChunks>");

                string[] dismount = new[]
                {
                    "if (Core.Player.IsMounted)",
                    "{",
                    "\tff14bot.Managers.ActionManager.Dismount();",
                    "\tawait Coroutine.Sleep(3000);",
                    "}"
                };
                output.AppendLine(Generate_CodeChunk("dismount", dismount));
                output.AppendLine("</Profile>");
                
                using (StreamWriter outputFile = new StreamWriter("SightSeeLog.xml", false))
                {
                    //outputFile.Write(JsonConvert.SerializeObject(DeepDungeonList, Formatting.Indented));
                    outputFile.Write(output.ToString());
                }
                return Task.FromResult(true);
            }
            
            
            public string Generate_CodeChunkEmote(string emote)
            {
                string[] commands = new[] {"await Coroutine.Sleep(1500);",$"ff14bot.Managers.ChatManager.SendChat(\"{emote}\");", "await Coroutine.Sleep(1000);"};

                return Generate_CodeChunk(emote.Split('/')[1], commands);

            }

            public string Generate_CodeChunk(string Name, string[] commands)
            {
                StringBuilder sb = new StringBuilder(512);

                sb.AppendLine($"\t\t<CodeChunk Name=\"{Name}\"><![CDATA[ ");

                foreach (var cmd in commands)
                {
                    sb.AppendLine($"\t\t{cmd}");
                }
                sb.AppendLine($"\t\t]]></CodeChunk>");

                return sb.ToString();
            }
        }

        class SightSeePoint
        {
            private string Name;
            private string EmoteCommand;
            private Vector3 Location;
            private int MapId;
            private int Aetheryte;
            private int AdventureKey;
            public SightSeePoint(string name, string emoteCommand, Vector3 location)
            {
                Name = name;
                EmoteCommand = emoteCommand;
                Location = location;
            }

            public SightSeePoint(string name, string emoteCommand, Vector3 location, int mapId)
            {
                Name = name;
                EmoteCommand = emoteCommand;
                Location = location;
                MapId = mapId;
            }

            public SightSeePoint(string name, string emoteCommand, Vector3 location, int mapId, int aetheryte)
            {
                Name = name;
                EmoteCommand = emoteCommand;
                Location = location;
                MapId = mapId;
                Aetheryte = aetheryte;
            }

            public SightSeePoint(string name, string emoteCommand, Vector3 location, int mapId, int aetheryte, int adventureKey)
            {
                Name = name;
                EmoteCommand = emoteCommand;
                Location = location;
                MapId = mapId;
                Aetheryte = aetheryte;
                AdventureKey = adventureKey;
            }

            public string Name1 => Name;

            public string EmoteCommand1 => EmoteCommand;

            public Vector3 Location1 => Location;

            public int MapId1 => MapId;

            public int Aetheryte1 => Aetheryte;

            public override string ToString()
            {
                return $"{Name} {EmoteCommand} ZoneId=\"{MapId}\" XYZ=\"{Location.AsString()}\"";
            }

            public string GenerateGoTo()
            {
                StringBuilder sb = new StringBuilder(512);

                sb.AppendLine($"\t\t<!-- {Name}({AdventureKey}) {EmoteCommand} {Location.AsString()}  -->");


                sb.AppendLine($"\t\t<TeleportTo AetheryteId=\"{Aetheryte}\"/>");
                sb.AppendLine($"\t\t<FlyTo ZoneId=\"{MapId}\" XYZ=\"{Location.AsString()}\" Name=\"{Name}\" ArrivalTolerance=\"0.5\" Land=\"true\" AllowedVariance=\"0.0\"/>");
                sb.AppendLine($"\t\t<RunCode Name=\"dismount\"/>");
                sb.AppendLine($"\t\t<RunCode Name=\"{EmoteCommand.Split('/')[1]}\"/>\n");

                //sb.AppendLine($"\t\t]]></CodeChunk>");

                return sb.ToString();
            }

        }
        
        public static class Vector3Extentions
        {
            public static string AsString(this Vector3 input)
            {
                return $"{input.X}, {input.Y}, {input.Z}";
            }
        }

    }