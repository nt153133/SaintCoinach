using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FileHelpers;
using Newtonsoft.Json;
using SaintCoinach.Cmd.External;
using SaintCoinach.Xiv;
using Tharga.Toolkit.Console.Command.Base;
using Action = SaintCoinach.Xiv.Action;

namespace SaintCoinach.Cmd.Commands
{
    public class TestCommand : ActionCommandBase
    {
        private ARealmReversed _Realm;

        public TestCommand(SaintCoinach.ARealmReversed realm) : base("test", "Just testing")
        {
            _Realm = realm;
        }

        static FileHelperEngine<MappyNPC>  engine = new FileHelperEngine<MappyNPC>();
        static MappyNPC[] mappyDataResult; //= engine.ReadFile(@"H:\xivapi_mappy_2019-11-16-12-53-32.csv");

        public override Task<bool> InvokeAsync(string paramList)
        {
            var recipes = _Realm.GameData.GetSheet<Recipe>();
            var craftaction = _Realm.GameData.GetSheet<CraftAction>();
            var generalActions = _Realm.GameData.GetSheet<Action>();
            var classes = _Realm.GameData.GetSheet<ClassJob>();
            var maps1 =_Realm.GameData.GetSheet<TerritoryType>();

            //var gather = _Realm.GameData.GetSheet<GatheringPoint>();
            mappyDataResult = engine.ReadFile(@"H:\xivapi_mappy_2019-11-16-12-53-32.csv");
            


            var maps = _Realm.GameData.GetSheet2<MapMarker>().Cast<MapMarker>();
            var Gather = _Realm.GameData.GetSheet<GatheringPoint>();

            var spot = _Realm.GameData.GetSheet<FishingSpot>()[179];
            var map = maps1.Where(i=> i.Key == 398).First().Map;
            float startWorldX = 504.1477f;
            float startWorldY = 34.56757f;
            var PlaceX = (float)ToMapCoordinate3d(startWorldX, map.OffsetX, map.SizeFactor);
            var PlaceY = (float)ToMapCoordinate3d(startWorldY, map.OffsetY, map.SizeFactor) ;
            OutputInformation($"OffsetX: {map.OffsetX} OffsetY: {map.OffsetY} SizeFactor {map.SizeFactor}");
            OutputInformation($"WorldToMap: {map.PlaceName}: ({PlaceX}, {PlaceY})");
            OutputInformation($"Backwards: {test(PlaceX,map.OffsetX, map.SizeFactor)} {test(PlaceY, map.OffsetY, map.SizeFactor)}");
            
            OutputInformation($"Map: {spot.PlaceName}: ({spot.MapX}, {spot.MapY}) {spot.Radius}");
            float f = HalfHelper.Unpack((ushort) spot.X);
            float f1 = HalfHelper.Unpack((ushort) spot.Z);
            OutputInformation($"World: {spot.PlaceName}: ({f}, {f1}) {spot.Radius}");
            
            
/*            foreach (var c in Gather)
            {
                ParseGather(c, maps);
            }
            
            foreach (var item in GatherData)
            {
                //OutputInformation($"{craft.Name} {craft.Key} {craft.ClassJob.Name.ToString()}");
                //var temp = new ClassAction(craft.Key, craft.Name.ToString(), craft.ClassJob.Name.ToString().FirstCharToUpper());
                OutputInformation($"{item.Key} {item.Value.NodeId} {item.Value.Position.Id} {item.Value.Position.ToVec3<Vector3>()}");
                //actionList.Add(temp);
            }*/
            
            return Task.FromResult(true);
        }
        
        private List<GarlandDBNode> GarlandBellGathering { get; set; }

        private GarlandFishing GarlandFishing { get; set; }
        private Dictionary<uint, GatheringData> GatherData = new Dictionary<uint, GatheringData>();

        private void LoadGarlandData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.garlandtools.org/bell/nodes.js");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            string Html;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Html = reader.ReadToEnd();
                    }

            //replace the javascript so we can json decode it.
            Html = Html.Replace("gt.bell.nodes =", "").Replace("];", "]");

            GarlandBellGathering = JsonConvert.DeserializeObject<List<GarlandDBNode>>(Html);
            
        }
        
         private void ParseGather(GatheringPoint g, IEnumerable<MapMarker> maps)
        {
            //Beast Tribe and Leve Quests get Territory type 1 most of the time....
            if (g.Base.Key == 0 || g.TerritoryType.Key == 1 || g.Base.GatheringLevel == 0 || !g.Base.Items.Any() || g.Base.Items.All(i => i.Item.Key >= 2000000 && i.Item.Key < 3000000))
                return;
            var ng = new GatheringData();

            ng.NodeId = Convert.ToUInt32(g.Key);

            if(g.GatheringPointBonus.Length >= 1)
                ng.Bonus = Convert.ToUInt32(g.GatheringPointBonus[0].Key);


            ng.Level = Convert.ToUInt32(g.TerritoryType.Key);

            ng.Position = LevelBuilder.Level(_Realm.GameData.GetSheet<SaintCoinach.Xiv.Level>().FirstOrDefault(i => i.Object == g));



            /// <summary>
            /// 0 -> Mining
            /// 1 -> Quarrying
            /// 2 -> Logging
            /// 3 -> Harrvesting
            /// 4 -> SpearFishing
            /// </summary>

            var type = Convert.ToByte(g.Base.Type.Key); //
            if (type < 2)
                ng.Job = 16; //miner
            else if (type < 4)
                ng.Job = 17; //bot
            else
                ng.Job = 18; //fisher

            if (g.GatheringSubCategory == null)
            {
                ng.RequiredBook = 0;
            }
            else
            {
                ng.RequiredBook = Convert.ToUInt32(g.GatheringSubCategory.Item.Key);
            }



            ng.Territory = Convert.ToUInt32(g.TerritoryType.Key);

            ng.TerritoryName = g.TerritoryType.Name.ToString();


            if (g.PlaceName.Key != 0)
            {

                ng.Place = Convert.ToUInt32(g.PlaceName.Key);

                ng.PlaceName = g.PlaceName.Name.ToString();

                var map = g.TerritoryType.Map;
                var X = maps.Where(i => i.Place.Key == g.PlaceName.Key);
                //hard coded fix for hell's lid
                if(g.PlaceName.Key == 2762)
                {
                    X = X.Where(i => i.Key == 17);
                }
                if (X.Count() == 0)
                {
                    var Y = _Realm.GameData.GetSheet<SpearfishingNotebook>().Where(t => t.PlaceName.Key == g.PlaceName.Key);
                    if (Y.Count() == 0 || Y.Count() > 1)
                        Debugger.Break();

                    ng.PlaceX = (float)map.ToMapCoordinate3d(Y.First().X, map.OffsetX);
                    ng.PlaceY = (float)map.ToMapCoordinate3d(Y.First().Y, map.OffsetY);

                }
                else if (X.Count() > 1)
                    Debugger.Break();
                else
                {

                    ng.PlaceX = (float)map.ToMapCoordinate3d(X.First().X, map.OffsetX);
                    ng.PlaceY = (float)map.ToMapCoordinate3d(X.First().Y, map.OffsetY);
                }
            }
            ng.Timed = g.Base.IsLimited;
            var bg = g.Base.Key;
            
            MappyNPC npc = mappyDataResult.First(i => i.ENpcResidentID == ng.NodeId);
            //OutputInformation($"{g.Base.Key} count - {mappyDataResult.Count(i => i.ENpcResidentID == ng.NodeId)} ");
            if (ng.Timed)
            {
                var garland = GarlandBellGathering.First(i => i.Id == bg);
                ng.Timer = new List<uint>( garland.Time );
                ng.Duration = garland.Uptime;
                
            }
            if (ng.Position != null)
                OutputInformation($"{ng.NodeId} {g.Base.Key}  {ng.Position.ZoneId} {ng.NodeId} {ng.Position.ToVec3<Vector3>()}");
            else
            {
                OutputInformation($"{ng.NodeId} {g.Base.Items.FirstOrDefault().Item.Name} ({npc.CoordinateX}, {npc.CoordinateZ}, {npc.CoordinateY})");
            }
            GatherData.Add(ng.NodeId, ng);
        }
         
/*         public static Vector3 get3dPosFrom2d( float x, float y )
         {
             Vector3 ret;
             float scale2 = ( float ) mapScale / 100.0f;
             ret.X = ( x * scale2 ) + ( ( float ) img.height * 2.f ); //( x / scale2 ) - mapOffsetX;
             ret.Y = ( y * scale2 ) + ( ( float ) img.height * 2.f ); //( y / scale2 ) - mapOffsetY;

             return ret;
         }*/

        private static float test(float value, int offset, int sizefactor )
        {
            float c = sizefactor / 100;
            return (1024*(2*c*(value - 1) - 41) / (41*c)) - offset;
        }
        
        public double ToMapCoordinate3d(double value, int offset, int SizeFactor)
        {
            var c = SizeFactor / 100.0;
            var offsetValue = (value + offset) * c;
            return ((41.0 / c) * ((offsetValue + 1024.0) / 2048.0)) + 1;
        }
    }
}