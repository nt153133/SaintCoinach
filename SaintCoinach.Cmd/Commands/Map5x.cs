﻿using System;
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
 using SaintCoinach.Graphics.Lgb;
 using SaintCoinach.Xiv;

namespace SaintCoinach.Cmd.Commands {
    public class map5x : ActionCommandBase {
        private ARealmReversed _Realm;

        public map5x(ARealmReversed realm)
            : base("DumpDeep", "Export all data (default), or only specific data files, seperated by spaces; including all languages.") {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList) {
            const string CsvFileFormat = "exd/map5x.csv";

            IEnumerable<string> filesToExport;

            
		var target = new FileInfo(Path.Combine(_Realm.GameVersion, CsvFileFormat));
			if (!target.Directory.Exists)
				target.Directory.Create();
            var engine = new FileHelperEngine<MappyNPC>();
            var result = engine.ReadFile(@"H:\xivapi_mappy_2019-10-13-10-11-12.csv");
	        
			var DeepDungeons = _Realm.GameData.GetSheet("DeepDungeon");
			var Pomandander = _Realm.GameData.GetSheet("DeepDungeonItem");
           // var ContentFinderCond = _Realm.GameData.GetSheet("ContentFinderCondition");
            var ContentFinderCond = _Realm.GameData.GetSheet <ContentFinderCondition>();
            var InstanceContent = _Realm.GameData.GetSheet("InstanceContent");
            //var TerritoryType = _Realm.GameData.GetSheet("TerritoryType");
            var TerritoryType = _Realm.GameData.GetSheet <TerritoryType>();
            var Quests = _Realm.GameData.GetSheet <Quest>();

            foreach (var dd in DeepDungeons.Where(i => !string.IsNullOrWhiteSpace(i[22].ToString()) ))
            {
                
                OutputInformation($"Deep Dungeon: {dd[22]}");
                //OutputInformation($"Name: {Pomandander[dd.AsInt32("Inventory1")][9]}");

                //int test = dd.AsInt32("ContentFinderCondition");
                
                ContentFinderCondition CF = new ContentFinderCondition(ContentFinderCond,ContentFinderCond[dd.AsInt32("ContentFinderCondition")] );
                var ter = TerritoryType.First(i => i.Key == CF.As<TerritoryType>("TerritoryType").Key);
                
                OutputInformation($"Name: {CF.Name}");
                var lobby = TerritoryType.First(i =>
                    (i.PlaceName == (ter.PlaceName)) && i.AsInt32("TerritoryIntendedUse") == 12);
                OutputInformation($"Lobby: {lobby.Key} ");
                var questid = CF.As<Quest>("UnlockQuest");
                OutputInformation($"Quest: {questid.Key}");
                //OutputInformation($"[NPC] Name: {questid.TargetENpc}");
                //OutputInformation($"[NPC] NpcId: {questid.TargetENpc.Key}");
                
                MappyNPC npc = result.First(i => i.ENpcResidentID == questid.TargetENpc.Key);
                OutputInformation($"[NPC] Name: {npc.Name.Replace('"',' ').Trim()}");
                OutputInformation($"[NPC] Aetheryte: {lobby.As<XivRow>("Aetheryte").Key}");
                OutputInformation($"[NPC] ZoneId: {npc.MapTerritoryID}");
                OutputInformation($"[NPC] Location: ({npc.CoordinateX}, {npc.CoordinateZ}, {npc.CoordinateY})");
                
                //OutputInformation($"Location ZonePlaceName: {ter.ZonePlaceName} Key: {ter.Key}");
                OutputInformation("");
            }
            //21

            foreach (var ct in ContentFinderCond.Where(i => i.ContentType.Name == "Deep Dungeons"))
            {
                var ter = TerritoryType.First(i => i.Key == ct.As<TerritoryType>("TerritoryType").Key);
                        //ContentFinderCondition CF = new ContentFinderCondition(ContentFinderCond,ct );
                //OutputInformation($"Name: {ct.Name} MapId: {ter.Key}");
            }

            //MapCol();

            //SaveAsCsv(data, Language.English, "d:\\out.csv");
            
            
           // OutputInformation($"Count: {result.Length}");
           // MappyNPC npc = result.First(i => i.ENpcResidentID == 1025846);
           // OutputInformation($"Mappy: {npc.Name} {npc.CoordinateX} {npc.CoordinateY} {npc.CoordinateZ}");

            return true;
        }

        public void MapCol()
        {
            const string CsvFileFormat = "collision/{0}";
            var c = 0;
            var allMaps = _Realm.GameData.GetSheet<Xiv.TerritoryType>().Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg).Select(i => i.First());
            var territoryTypes = allMaps.ToList();
            OutputInformation("{0} Maps found", _Realm.GameData.GetSheet<Xiv.TerritoryType>().Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg).Count());
            //foreach (var map in territoryTypes)
            //{
           var map = territoryTypes.First();
                var territory = new Graphics.Territory(map);
                OutputInformation("{0} files", territory.LgbFiles.Length);
                //continue;
               // OutputInformation();
               // if (territory.Collider == null) continue;

				
                //var target = new FileInfo(Path.Combine(_Realm.GameVersion, string.Format(CsvFileFormat, map.Key)));

                

                foreach (var vec in territory.LgbFiles[0].Groups)
                {
                    foreach (var e in vec.Entries)
                    {
                        if (e == null)
                            continue;
                        if (e.GetType() == typeof(LgbGimmickEntry))
                        {
                            if ((e as LgbGimmickEntry).Gimmick.File != null)
                            {
                                //OutputInformation($"{(e as LgbModelEntry).CollisionFile.File.Path}");
                                if ((e as LgbGimmickEntry).Gimmick.File.Path.Contains("wall"))
                                {
                                    var gim = (e as LgbGimmickEntry).Gimmick;

                                    OutputInformation($"{gim}");
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
                        }
                            
                    }
                    
                }

                // territory.Collider.Vectors

                //System.IO.File.WriteAllBytes(target.FullName, territory.Collider.Vectors);
                //c++;
           // }
            OutputInformation("{0} collisions saved", c);
        }
        public static void SaveAsCsv(Ex.Relational.IRelationalSheet sheet, Language language, string path) {
            using (var s = new StreamWriter(path, false, Encoding.UTF8)) {
                var indexLine = new StringBuilder("key");
                var nameLine = new StringBuilder("#");
                var typeLine = new StringBuilder("int32");

                var colIndices = new List<int>();
                foreach (var col in sheet.Header.Columns) {
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
