﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaintCoinach.Cmd.DeepDungeonExporter;
using SaintCoinach.Ex;
using SaintCoinach.Ex.Variant2;
//using SaintCoinach.Cmd.DeepDungeonExporter;
using SaintCoinach.Graphics.TerritoryParts;

using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;
using SaintCoinach.Graphics;
using SaintCoinach.Xiv;

namespace SaintCoinach.Cmd.Commands {


    public class DeepCommand : ActionCommandBase {

        private ARealmReversed _Realm;

        public DeepCommand(ARealmReversed realm)
            : base("deep", "Export deep dungeon nav mesh.") {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList) {

            var c = 1;
            var allMaps = _Realm.GameData.GetSheet<SaintCoinach.Xiv.TerritoryType>().Where(i => i.PlaceName.ToString().Contains("Heaven-on-High")).GroupBy(i => i.Bg).Select(i => i.First());
//var allMaps = _Realm.GameData.GetSheet<SaintCoinach.Xiv.TerritoryType>().Where(i => i.PlaceName.ToString().Contains("The Palace of the Dead")).GroupBy(i => i.Bg).Select(i => i.First());
            const string FilePath = @".\maps\";

            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            try
            {
                foreach (var map in allMaps)
                {
                    var territory = new Graphics.Territory(map);
                    OutputInformation(territory.Name);

                    //var _LgbPartsContainer = new ComponentContainer();
                    if(territory.Collider != null)
                        OutputInformation("PCB FILE FOUND");
                    

                    var fp = Path.Combine(FilePath, $"{territory.Name}.");
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);
                    await DumpMap(fp + "obj", territory);
                    

                    
                    c++;
                }
            }
            catch (Exception ex)
            {
                OutputError(ex.ToString());
            }

            OutputInformation("{0} maps saved", c);

            return true;
        }

        private async Task DumpMap(string fp, Territory territory)
        {
            using (var sw = File.CreateText(fp))
            {
                //sw.WriteLine("mtllib nav_test.mtl");

                //sw.WriteLine($"o {territory.Name}.obj");

                var cnt = 1;

                //if (territory.Terrain != null)
                //{
                //    foreach (var part in territory.Terrain.Parts)
                //    {
                //        var fc = new FileContentModel(part, false);
                //        cnt = fc.MakeObj(sw, cnt);
                //    }
                //}
                
                foreach (var lgb in territory.LgbFiles)
                {
                    
                    foreach (var group in lgb.Groups)
                    {
                        Console.WriteLine(group.Name);
                        foreach (var part in group.Entries)
                        {
                            var asMdl = part as SaintCoinach.Graphics.Lgb.LgbModelEntry;
                            var asGim = part as SaintCoinach.Graphics.Lgb.LgbGimmickEntry;

                            if (asMdl?.Collider != null)
                            {
                                
                                sw.WriteLine("c " + asMdl.Header.Id);
                                var x = new FileContentModel(asMdl.Collider, false);
                                cnt = x.MakeObj(sw, cnt);

                            }

                            if (asGim?.Gimmick != null)
                            {
                                //if (gimicks.Contains(asGim.Header.Id))
                                //    asGim.Toggle();
                                sw.WriteLine("w " + asGim.Header.Id);
                                var x = new FileContentSgbModel(asGim.Gimmick, true)
                                {

                                    Transformation =
                                        Matrix.Scaling(asGim.Header.Scale.ToDx())
                                        * Matrix.RotationX(asGim.Header.Rotation.X)
                                        * Matrix.RotationY(asGim.Header.Rotation.Y)
                                        * Matrix.RotationZ(asGim.Header.Rotation.Z)
                                        * Matrix.Translation(asGim.Header.Translation.ToDx()),
                                };
                                cnt = x.MakeObj(sw, cnt);
                                //Console.WriteLine(cnt);
                            }
                        }
                    }
                }
             /*   
                foreach (var lgb in territory.LgbFiles) {
                    foreach (var group in lgb.Groups) {
                        foreach (var part in group.Entries) {
                            var asMdl = part as SaintCoinach.Graphics.Lgb.LgbModelEntry;
                            var asGim = part as SaintCoinach.Graphics.Lgb.LgbGimmickEntry;

                            //we only care about gimmicks.
                            if (asGim == null)
                                continue;

                            //Console.WriteLine(asGim.Header.Id);


                          //  if (asGim.Gimmick != null) {
                                sw.WriteLine("w " + asGim.Header.Id);
                            if (asGim.Header.Id == 7350862) {
                                //find the wall
                                
                                var x = new FileContentSgbModel(asGim.Gimmick, true) {
                                    Transformation =
                                        Matrix.Scaling(asGim.Header.Scale.ToDx())
                                        * Matrix.RotationX(asGim.Header.Rotation.X)
                                        * Matrix.RotationY(asGim.Header.Rotation.Y)
                                        * Matrix.RotationZ(asGim.Header.Rotation.Z)
                                        * Matrix.Translation(asGim.Header.Translation.ToDx()),
                                };
                                
                               cnt = x.MakeObj(sw, cnt);
                                //Console.WriteLine(
                                Console.WriteLine();
                                Console.WriteLine(asGim.Header.Translation.X);
                                Console.WriteLine(asGim.Header.Translation.Y);
                                Console.WriteLine(asGim.Header.Translation.Z);

                                Console.WriteLine(asGim.Header.Rotation.X);
                                Console.WriteLine(asGim.Header.Rotation.Z);
                            }
                        }
                    }
                }*/
            }
        
        }

        static string ToPathSafeString(string input, char invalidReplacement = '_') {
            var sb = new StringBuilder(input);
            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
                sb.Replace(c, invalidReplacement);
            return sb.ToString();
        }
    }
}
