using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;
using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Ex.Relational;
using SaintCoinach.Graphics;
using SaintCoinach.Graphics.Lgb;
using SaintCoinach.Graphics.Pcb;
using SaintCoinach.Graphics.Sgb;
using SaintCoinach.Xiv;
using Newtonsoft.Json;
using Action = System.Action;

namespace SaintCoinach.Cmd.Commands
{
    public class map5x : ActionCommandBase
    {
        private ARealmReversed _Realm;

        public map5x(ARealmReversed realm)
            : base("DumpDeep",
                "Export all data (default), or only specific data files, seperated by spaces; including all languages.")
        {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            string CsvFileFormat = "exd/map5x.csv";

            IEnumerable<string> filesToExport;


            //var target = new FileInfo(Path.Combine(_Realm.GameVersion, CsvFileFormat));
            //	if (!target.Directory.Exists)
            //		target.Directory.Create();
            var engine = new FileHelperEngine<MappyNPC>();
            var result = engine.ReadFile(@"H:\xivapi_mappy_2019-10-13-10-11-12.csv");

            var DeepDungeons = _Realm.GameData.GetSheet("DeepDungeon");
            var Pomandander = _Realm.GameData.GetSheet("DeepDungeonItem");
            // var ContentFinderCond = _Realm.GameData.GetSheet("ContentFinderCondition");
            var ContentFinderCond = _Realm.GameData.GetSheet<ContentFinderCondition>();
            var InstanceContent = _Realm.GameData.GetSheet("InstanceContent");
            //var TerritoryType = _Realm.GameData.GetSheet("TerritoryType");
            var TerritoryType = _Realm.GameData.GetSheet<TerritoryType>();
            var Quests = _Realm.GameData.GetSheet<Quest>();
            var PlaceName = _Realm.GameData.GetSheet<PlaceName>();
            var Aetheryte = _Realm.GameData.GetSheet("Aetheryte");
            var test = _Realm.Packs.GetFile(@"bg/ffxiv/fst_f1/cnt/f1c1/collision/f1c1_a1_ge01a.pcb");

            var poms = Pomanders();
            var DeepDungeonList = new List<DeepDungeon>();

            foreach (var dd in DeepDungeons.Where(i => i.Key > 0))
            {
                var CurrentDeepDungeon =
                    new DeepDungeon(dd.Key, dd["Name"].ToString(), dd.AsInt32("ContentFinderCondition"));
                var pomanderMap = new Dictionary<int, int>();
               // OutputInformation($"Deep Dungeon: {dd["Name"]}");

                for (int i = 1; i < 17; i++)
                    pomanderMap.Add(dd.AsInt32($"Inventory{i}"), i - 1);
                //OutputInformation($"{dd.AsInt32($"Inventory{i}")} , {i-1}");

                CurrentDeepDungeon.PomanderMapping = pomanderMap;

                //ContentFinderCondition Gives us the info of different conditions for the DD floors.
                ContentFinderCondition CF = new ContentFinderCondition(ContentFinderCond,
                    ContentFinderCond[dd.AsInt32("ContentFinderCondition")]);
                CurrentDeepDungeon.ContentFinderId = CF.Key;

                TerritoryType ter = TerritoryType[CF.As<TerritoryType>("TerritoryType").Key];
                //TerritoryType.First(i => i.Key == CF.As<TerritoryType>("TerritoryType").Key);


               // OutputInformation($"Name: {CF.Name} ({ter.PlaceName.NameWithoutArticle})");
                CurrentDeepDungeon.NameWithoutArticle = ter.PlaceName.NameWithoutArticle;

                //Get Lobby mapid
                var lobby = TerritoryType.First(i =>
                    i.PlaceName == ter.PlaceName && i.AsInt32("TerritoryIntendedUse") == 12);
              //  OutputInformation($"Lobby: {lobby.Key} ");
                CurrentDeepDungeon.LobbyId = lobby.Key;

                var questid = CF.As<Quest>("UnlockQuest");
               // OutputInformation($"Quest: {questid.Name} ({questid.Key})");
                CurrentDeepDungeon.UnlockQuest = questid.Key;

                MappyNPC npc = result.First(i => i.ENpcResidentID == questid.TargetENpc.Key);
                EntranceNpc enpc = new EntranceNpc(npc, lobby.As<XivRow>("Aetheryte").Key);
                CurrentDeepDungeon.Npc = enpc;


              //  OutputInformation($"{enpc}");

              //  OutputInformation($"hmm: {Aetheryte[enpc.AetheryteId].As<PlaceName>("PlaceName").Name}");


                //OutputInformation($"hmm: {Aetheryte[enpc.AetheryteId].As<PlaceName>("PlaceName").Name}");
                //OutputInformation($"Location ZonePlaceName: {ter.ZonePlaceName} Key: {ter.Key}");
             //   OutputInformation("");
                DeepDungeonList.Add(CurrentDeepDungeon);
            }

            foreach (var DeepIndex in DeepDungeonList)
            {
                OutputInformation($"{DeepIndex}");
                bool foundSecond = false;
                foreach (var ct in ContentFinderCond.Where(i =>
                    i.ContentType.Name == "Deep Dungeons" && i.Name.ToString().Contains(DeepIndex.NameWithoutArticle)))
                {
                    var ter = TerritoryType.First(i => i.Key == ct.As<TerritoryType>("TerritoryType").Key);
                    ContentFinderCondition CF = new ContentFinderCondition(ContentFinderCond, ct); //Content
                    InstanceContent ic = new InstanceContent(InstanceContent, CF.Content);
                    var questid = CF.As<Quest>("UnlockQuest");


                   // OutputInformation($"Name: {ct.Name} MapId: {ter.Key} Quest: {questid.Key} ContentFinderId: {CF.Key} InstanceID: {ic.Key}");
                    var name = ct.Name.ToString();

                    var name1 = name.Substring(name.IndexOf('(') + 1, name.IndexOf(')') - name.IndexOf('(') - 1);
                    var name2 = name1.Split(' ')[1].Split('-');
                    int start;
                    int.TryParse(name2[0], out start);
                    int end;
                    int.TryParse(name2[1], out end);
                    
                    DeepIndex.Floors.Add(new FloorSetting(ct.Name, ic.Key, CF.Key, ter.Key,questid.Key, start, end));
                    //OutputInformation($"{start} to floor {end}");
/*
                    if (!foundSecond && ter.Key > DeepIndex.LobbyId)
                    {
                        DeepIndex.StartAt = start;
                        DeepIndex.StartAtContentFinderId = CF.Key;
                        foundSecond = true;
                    }
                    */
                }

                foreach (var floor in DeepIndex.Floors)
                {
                   // OutputInformation($"{floor}");
                }

                //OutputInformation($"You can start at floor {DeepIndex.StartAt} ({(ContentFinderCond[DeepIndex.StartAtContentFinderId].Name)})");
                
            }
            //21
            //JsonConvert.SerializeObject(DeepDungeonList)
            using (StreamWriter outputFile = new StreamWriter("DeepDungeons.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(DeepDungeonList, Formatting.Indented));
            }



            // var pcb = new PcbFile(test);
            // foreach (var p in pcb.Data)
            //  {
            //     OutputInformation($"{p.ToString()}");
            // }
            //  OutputInformation($"{pcb.ToString()}");


            //CsvFileFormat = "collision/{0}";
            //var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(CsvFileFormat,"f1c1_a1_ge01a.pcb")));
            //if (!target.Directory.Exists)
            //    target.Directory.Create();
            //                        
            //System.IO.File.WriteAllBytes(target.FullName, Makeobj);

            //MapCol();

            //SaveAsCsv(data, Language.English, "d:\\out.csv");


            // OutputInformation($"Count: {result.Length}");
            // MappyNPC npc = result.First(i => i.ENpcResidentID == 1025846);
            // OutputInformation($"Mappy: {npc.Name} {npc.CoordinateX} {npc.CoordinateY} {npc.CoordinateZ}");

            return true;
        }

        public Dictionary<int, string> Pomanders()
        {
            var Pomander = _Realm.GameData.GetSheet("DeepDungeonItem");
            var result = new Dictionary<int, string>();

            foreach (var pom in Pomander.Where(i => i.Key > 0))
            {
                //OutputInformation($"{pom.Key} : {pom.AsString("Name")} {PomanderToEnum(pom.AsString("Name"))}"); //({pom.As<Xiv.Action>(11)})");
                result.Add(pom.Key, PomanderToEnum(pom.AsString("Name")));
                //OutputInformation($"({(pom[11] as Xiv.Action).Key})");
            }

            return result;
        }

        public string PomanderToEnum(string name)
        {
            //var nameParts = name.Split($" of ", StringSplitOptions.RemoveEmptyEntries);
            var first = name.IndexOf(' ');
            var last = name.LastIndexOf(' ') + 1;


            return $"{name.Substring(0, first)}.{name.Substring(last)}";
        }

        public void MapCol()
        {
            const string CsvFileFormat = "collision/{0}";
            var c = 0;
            var allMaps = _Realm.GameData.GetSheet<TerritoryType>()
                .Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg)
                .Select(i => i.First());
            var territoryTypes = allMaps.ToList();
            OutputInformation("{0} Maps found",
                _Realm.GameData.GetSheet<TerritoryType>()
                    .Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg).Count());
            //foreach (var map in territoryTypes)
            //{
            var map = territoryTypes.First();
            var territory = new Territory(map);
            OutputInformation("{0} files", territory.LgbFiles.Length);
            //continue;
            // OutputInformation();
            // if (territory.Collider == null) continue;


            //var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(CsvFileFormat, map.Key)));


            foreach (var vec in territory.LgbFiles[0].Groups)
            foreach (var e in vec.Entries)
            {
                if (e == null)
                    continue;
                if (e.GetType() == typeof(LgbGimmickEntry))
                    if ((e as LgbGimmickEntry).Gimmick.File != null)
                        //OutputInformation($"{(e as LgbModelEntry).CollisionFile.File.Path}");
                        if ((e as LgbGimmickEntry).Gimmick.File.Path.Contains("wall"))
                        {
                            var gim = (e as LgbGimmickEntry).Gimmick;
                            //gim.Header.
                            //OutputInformation($"{gim.File}");
                            //foreach (var gd in gim.Data)
                            {
                            }

                            //OutputInformation($"{(e as LgbGimmickEntry).Gimmick.File}");
                            //var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(CsvFileFormat, (e as LgbGimmickEntry).Gimmick.File.Path.Split('/')[5])));
                            //if (!target.Directory.Exists)
                            //    target.Directory.Create();

                            //System.IO.File.WriteAllBytes(target.FullName, (e as LgbGimmickEntry).Gimmick.File.GetData());
                            c++;
                        }
            }

            // territory.Collider.Vectors

            //System.IO.File.WriteAllBytes(target.FullName, territory.Collider.Vectors);
            //c++;
            // }
            OutputInformation("{0} collisions saved", c);
        }

        public static void SaveAsCsv(IRelationalSheet sheet, Language language, string path)
        {
            using (var s = new StreamWriter(path, false, Encoding.UTF8))
            {
                var indexLine = new StringBuilder("key");
                var nameLine = new StringBuilder("#");
                var typeLine = new StringBuilder("int32");

                var colIndices = new List<int>();
                foreach (var col in sheet.Header.Columns)
                {
                    indexLine.AppendFormat(",{0}", col.Index);
                    nameLine.AppendFormat(",{0}", col.Name);
                    typeLine.AppendFormat(",{0}", col.ValueType);

                    colIndices.Add(col.Index);
                }

                s.WriteLine(indexLine);
                s.WriteLine(nameLine);
                s.WriteLine(typeLine);

                ExdHelper.WriteRows(s, sheet, language, colIndices, false);
            }
        }
    }
}