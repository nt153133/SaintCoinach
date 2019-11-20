using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaintCoinach.Xiv;
using Tharga.Toolkit.Console.Command.Base;
using Action = SaintCoinach.Xiv.Action;

namespace SaintCoinach.Cmd.Commands
{
    public class DawnCommand : ActionCommandBase
    {
        private ARealmReversed _Realm;

        public DawnCommand(ARealmReversed realm)
            : base("dawn", "Trust stuff")
        {
            _Realm = realm;
        }

        Dictionary<int, int> itemIndex = new Dictionary<int, int>
        {
            {20,4},
            {40,3},
            {150,2 },
            {290,1},
            {430,0}
        };

        Dictionary<string, int> jobIndex = new Dictionary<string, int> 
        {
            {"carpenter", 0},
            {"blacksmith", 1},
            {"armorer", 2},
            {"goldsmith", 3},
            {"leatherworker", 4},
            {"weaver", 5},
            {"alchemist", 6},
            {"culinarian", 7}
        };

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var DawnQuestMember = _Realm.GameData.GetSheet("DawnQuestMember");
            var DawnContent = _Realm.GameData.GetSheet("DawnContent");
            var DawnGrowMember = _Realm.GameData.GetSheet("DawnGrowMember");
            
            var recipes = _Realm.GameData.GetSheet<Recipe>();
            var craftaction = _Realm.GameData.GetSheet<CraftAction>();
            var generalActions = _Realm.GameData.GetSheet<Action>();
            var classes = _Realm.GameData.GetSheet<ClassJobCategory>();

            var actions = craftaction.Where(row => row.ClassJobCategory.Key != 0 && row.ClassJob != null);
            List<ClassAction> actionList = new List<ClassAction>();
            foreach (var craft in actions)
            {
                //OutputInformation($"{craft.Name} {craft.Key} {craft.ClassJob.Name.ToString()}");
                var temp = new ClassAction(craft.Key, craft.Name.ToString(), craft.ClassJob.Name.ToString().FirstCharToUpper());
                 //OutputInformation($"craft: {temp}");
                 actionList.Add(temp);
            }

            var genActions = generalActions.Where(row => row.ActionCategory.Key == 7 &&  row.ClassJob !=null && row.ClassJob.Key > 0 && row.ClassJob != null && row.Name != null);
            //OutputInformation($"{genActions.Count()}");
            foreach (var craft in genActions)
            {
                //OutputInformation($"{craft.Key}");
               // OutputInformation($"{craft.Name}"); //{craft.ClassJob.Name.ToString()}");
                
                var temp = new ClassAction(craft.Key, craft.Name.ToString(), craft.ClassJob.Name.ToString().FirstCharToUpper());
                //OutputInformation($"gen: {temp}");
                 actionList.Add(temp);
            }


            var tmp = classes[33];
            int j = 0;
            foreach (var cl in tmp.ClassJobs)
            {
                string outt = $@"{{""{cl.Name.ToString().FirstCharToUpper()}"", {j}}},";
                //OutputInformation(outt);
              //  Console.WriteLine(outt);
                j++;
            }
            
            foreach (var rec in recipes.Where(row => row.ResultItem.Name.ToString().Contains("Skybuilders")).OrderBy(r=> r.ClassJob.Key))
            {
                string check = $"if (InventoryManager.FilledSlots.Any(i => i.RawItemId == {rec.ResultItem.Key}))\n\tawait test.HandInItem({rec.ResultItem.Key}, {itemIndex[rec.RecipeLevelTable.Key]}, {jobIndex[rec.ClassJob.Name]});\n";
                OutputInformation($"// {rec.ResultItem.Name} ({rec.ClassJob.Name}) ");
                OutputInformation(check);
                
            }

/*
            using (StreamWriter outputFile = new StreamWriter("CraftingActions.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(actionList, Formatting.Indented));
            }*/

            foreach (var rec in recipes.Where(row => row.ResultItem.Name.ToString().Contains("Skybuilders")))
            {
                
                OutputInformation($"{rec.ResultItem.Key},{rec.ResultItem.Name},{rec.ClassJob.Name.ToString().FirstCharToUpper()},{rec.RecipeLevel.RecipeLevelTable.Key}");

            }

            foreach (var g in DawnGrowMember)
            {
             //   OutputInformation($"{g[0]}, {int.Parse(g[2].ToString().Split('/')[3].Split('.')[0])},  {int.Parse(g[3].ToString().Split('/')[3].Split('.')[0])}");
            }
            
            //OutputInformation($"Quest:");

            foreach (var g in DawnQuestMember.Where(i => i[0] != null))
            {
              //  OutputInformation($"\"{g[0]}\", {int.Parse(g[2].ToString().Split('/')[3].Split('.')[0])}, {int.Parse(g[3].ToString().Split('/')[3].Split('.')[0])}, {g.GetRaw(4)}");
            }

            return true;
        }

        private static string getMacro(List<ClassAction> actions, string job)
        {
            Regex ItemRegex = new Regex(@"/ac\s+""(.*)"".*", RegexOptions.Compiled);

            string output = "";
            
            //var lines = richTextBox1.Text.Split('\n');
            string macro = 
                @"/ac ""Reflect"" <wait.3>
/ac ""Manipulation"" <wait.2>
/ac ""Ingenuity"" <wait.2>
/ac ""Innovation"" <wait.2>
/ac ""Delicate Synthesis"" <wait.3>
/ac ""Delicate Synthesis"" <wait.3>
/ac ""Delicate Synthesis"" <wait.3>
/ac ""Delicate Synthesis"" <wait.3>
/ac ""Ingenuity"" <wait.2>
/ac ""Innovation"" <wait.2>
/ac ""Prudent Touch"" <wait.3>
/ac ""Prudent Touch"" <wait.3>
/ac ""Prudent Touch"" <wait.3>
/ac ""Basic Touch"" <wait.3>
/ac ""Ingenuity"" <wait.2>
/ac ""Innovation"" <wait.2>
/ac ""Basic Touch"" <wait.3>
/ac ""Great Strides"" <wait.2>
/ac ""Byregot's Blessing"" <wait.3>
/ac ""Basic Synthesis"" <wait.3>
";
            foreach (var line in macro.Split('\n'))
            {
                if (ItemRegex.IsMatch(line))
                {
                    string test = $"\t\t\t\t<CraftAction Name=\"{ItemRegex.Match(line).Groups[1]}\" ActionId=\"{actions.First(act => act.Class == job && act.Name.Equals(ItemRegex.Match(line).Groups[1].Value)).ActionId}\"/>";
                    output += test + "\n"; //$"{ItemRegex.Match(line).Groups[1]}\n"; //  regex.Match(line).Groups[2] + "\n";
                }

            }

            return output.Trim();
        }
    }
    

}