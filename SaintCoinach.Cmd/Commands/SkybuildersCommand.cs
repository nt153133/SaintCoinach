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
    public class SkybuildersCommand: ActionCommandBase {
        private ARealmReversed _Realm;

        public SkybuildersCommand(ARealmReversed realm)
            : base("sky", "orderbot skybuilders") {
            _Realm = realm;
        }

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

/*
            var tmp = classes[33];
            
            foreach(var cl in tmp.ClassJobs)
                OutputInformation($@"""{cl.Name.ToString().FirstCharToUpper()}"",");


            using (StreamWriter outputFile = new StreamWriter("CraftingActions.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(actionList, Formatting.Indented));
            }*/

            foreach (var rec in recipes.Where(row => row.ResultItem.Name.ToString().Contains("Skybuilders") && row.RecipeLevel.RecipeLevelTable.Key == 430))
            {
                OutputInformation($"\t\t<!-- {rec.ResultItem.Name} -->");
                OutputInformation($"\t\t<If Condition=\"Core.Player.CurrentJob == Enums.ClassJobType.{rec.ClassJob.Name.ToString().FirstCharToUpper()}\">");
                
                string test = "\t\t<While Condition=\"";
                foreach (var ingredient in rec.Ingredients.Where(i => i.Item.Key > 20)) 
                    test = test + $@"ItemCount({ingredient.Item.Key}) &gt; {ingredient.Count} and ";
                test += $@"Core.Player.CurrentJob == Enums.ClassJobType.{rec.ClassJob.Name.ToString().FirstCharToUpper()}"">";
                OutputInformation(test);
                
                OutputInformation($"\t\t\t<Synthesize RecipeId=\"{rec.Key}\"/>");
                
                OutputInformation($"\t\t\t<While Condition=\"CraftingManager.IsCrafting\">");
                OutputInformation(getMacro(actionList,rec.ClassJob.Name.ToString().FirstCharToUpper()));
                OutputInformation("\t\t\t</While>");
                OutputInformation("\t\t</While>\n\t\t</If>\n");
            }

            foreach (var g in DawnGrowMember)
            {
             //   OutputInformation($"{g[0]}, {int.Parse(g[2].ToString().Split('/')[3].Split('.')[0])},  {int.Parse(g[3].ToString().Split('/')[3].Split('.')[0])}");
            }
            
            OutputInformation($"Quest:");

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