using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Generate.External;
using Generate.Stuff;
using Newtonsoft.Json;
using SaintCoinach;
using SaintCoinach.IO;
using SaintCoinach.Xiv;

namespace Generate
{
    public static class SaintCHelper
    {
        public static ARealmReversed realm;
        
        public static List<GarlandDBNode> GarlandBellGathering { get; set; }
        
        static SaintCHelper()
        {
            var dataPath = @"G:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn";

            realm = new ARealmReversed(dataPath, @"SaintCoinach.History.zip", SaintCoinach.Ex.Language.English, @"app_data.sqlite");
            realm.Packs.GetPack(new PackIdentifier("exd", PackIdentifier.DefaultExpansion, 0)).KeepInMemory = true;

            Console.WriteLine("Game version: {0}", realm.GameVersion);
            Console.WriteLine("Definition version: {0}", realm.DefinitionVersion);

            //LoadGarlandData();
        }

        public static TerritoryType GetTerritoryById(int id)
        {
            var TerritoryType = realm.GameData.GetSheet<TerritoryType>();

            return TerritoryType.FirstOrDefault(i => i.Key == id);
        }

        public static IEnumerable<AozAction> GetBlueMageSpells()
        {
            var actions = realm.GameData.GetSheet<AozAction>("AozAction").Where(i => i.Action.Key > 0);

            return actions;
        }

        public static int GetNpcIdByName(string name)
        {
            var NameSheet = realm.GameData.GetSheet<BNpcName>();

            int id = 0;

            var npc = NameSheet.FirstOrDefault(i => i.Singular.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (npc != null)
                id = npc.Key;

            return id;

        }

        public static Dictionary<int, KeyValuePair<int, int>> GetRetainerTaskLevels()
        {
            var RangeSheet = realm.GameData.GetSheet("RetainerTaskLvRange");
            var dict = new Dictionary<int, KeyValuePair<int, int>>();
            int i = 0;
            foreach (var row in RangeSheet.Skip(1))
            {
                dict.Add(i, new KeyValuePair<int, int>(row.AsInt16("Min"),row.AsInt16("Max")));
                i++;
            }

            return dict;
        }

        public static List<RetainerTaskData> GetRetainerData()
        {
            var retainerTaskData = new List<RetainerTaskData>();
            var retainerTaskSheet = realm.GameData.GetSheet<RetainerTask>();
            var retainerTaskNorm = realm.GameData.GetSheet<RetainerTaskNormal>();
            var retainerTaskRand = realm.GameData.GetSheet<RetainerTaskRandom>();

            foreach (var task in retainerTaskSheet.Where(i=> i.Task.Key > 0))
            {
                if (task.IsRandom)
                {
                    retainerTaskData.Add(new RetainerTaskData(task, retainerTaskRand.First(i => i.Key == task.Task.Key)));
                }
                else
                {
                    retainerTaskData.Add(new RetainerTaskData(task, retainerTaskNorm.First(i => i.Key == task.Task.Key)));
                    //retainerTaskData.Add(new RetainerTaskData(task, retainerTaskNorm.First(i => i.Key == task.AsInt16("Task"))));
                }
            }
            
            return retainerTaskData;
        }
        
        private static void LoadGarlandData()
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

    }
    
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "":   throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default:   return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}