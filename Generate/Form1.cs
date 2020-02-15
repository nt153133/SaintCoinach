using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Generate.External;
using Generate.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestMaster.LuaParser;
using SaintCoinach.Text;
using SaintCoinach.Xiv;
using TitanBots.Remote;
using Formatting = System.Xml.Formatting;

namespace Generate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string NpcName = textBox1.Text.Trim();
            var results = Program.MappyHelper.GetNpcsByName(NpcName);

            var spell = (AozAction) comboBox1.SelectedItem;

            //richTextBox2.Text += $"{spell.Action.Name} - {spell.Action.Key}\n";
            richTextBox1.Text = "";

            richTextBox2.Text = "";

            richTextBox2.Text += $"Count: {results.Count()}\n";
            List<HotSpot> hotspots = new List<HotSpot>();

            var first = Program.MappyHelper.GetNpcByName(NpcName);

            richTextBox2.Text += $"Filtered Count: {results.Count(i => i.MapTerritoryID == first.MapTerritoryID && i.Location.Distance2D(first.Location) > 75)}";
            foreach (var result in results.Where(i => i.MapTerritoryID == first.MapTerritoryID && i.Location.Distance2D(first.Location) > 75))
            {
                richTextBox1.Text += $"{result.Name} {result.MapTerritoryID} {result.GetCords()}\n";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").PlaceName.Name} ";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").IsAetheryte} ";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").Key}\n";
                hotspots.Add(new HotSpot(result.Name.Replace(' ', '_'), result.Location, 50));
            }

            TargetMob target = new TargetMob();
            target.Id = SaintCHelper.GetNpcIdByName(first.Name);

            GrindArea grindArea = new GrindArea();

            grindArea.Name = first.Name.Replace(' ', '_');
            grindArea.Hotspots = hotspots;
            grindArea.MinLevel = 60;
            grindArea.MaxLevel = 80;
            grindArea.TargetMobs = new List<TargetMob> {target};

            Profile profile = new Profile();

            profile.Name = spell.Action.Name;
            profile.Order = " ";
            profile.GrindAreas = new List<GrindArea> {grindArea};
            profile.KillRadius = 50;

            int spellId = spell.Action.Key;

            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            string filename = @"G:\test.xml";
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, profile, ns);
            writer.Close();

            var doc = new XmlDocument();
            doc.Load(filename);
            var root = doc.GetElementsByTagName("Profile")[0];
            root.Normalize();
            var node1 = doc.GetElementsByTagName("Order")[0];
            // doc.

            //XmlNode myNode = doc.CreateNode(XmlNodeType.Element, "If");
            XmlNode myNode = doc.CreateElement("If");
            var attr1 = doc.CreateAttribute("condition");
            attr1.Value = "Core.Me.CurrentJob == ClassJobType.BlueMage";
            myNode.Attributes.Append(attr1);
            //myNode.Normalize();
            var newNode = node1.AppendChild(myNode);

            XmlNode myNode2 = doc.CreateElement("If");
            var attr2 = doc.CreateAttribute("condition");
            attr2.Value = $"not ActionManager.HasSpell({spellId})";
            myNode2.Attributes.Append(attr2);
            myNode2.Normalize();
            var newNode1 = newNode.AppendChild(myNode2);

            XmlNode myNode3 = doc.CreateElement("If");
            var attr3 = doc.CreateAttribute("condition");
            attr3.Value = $"not IsOnMap({first.MapTerritoryID})";
            myNode3.Attributes.Append(attr3);
            myNode3.Normalize();
            var newNode2 = newNode1.AppendChild(myNode3);

            XmlNode myNode4 = doc.CreateElement("TeleportTo");
            var attr4 = doc.CreateAttribute("name");
            attr4.Value = $"{first.TerritoryType.As<Aetheryte>("Aetheryte").PlaceName.Name}";
            var attr5 = doc.CreateAttribute("aetheryteId");
            attr5.Value = $"{first.TerritoryType.As<Aetheryte>("Aetheryte").Key}";
            var attr6 = doc.CreateAttribute("force");
            attr6.Value = $"true";
            myNode4.Attributes.Append(attr4);
            myNode4.Attributes.Append(attr5);
            myNode4.Attributes.Append(attr6);
            myNode4.Normalize();
            var newNode3 = newNode2.AppendChild(myNode4);


            XmlNode myNode5 = doc.CreateElement("Grind");
            var attr7 = doc.CreateAttribute("grindRef");
            attr7.Value = $"{first.Name.Replace(' ', '_')}";
            var attr8 = doc.CreateAttribute("postCombatDelay");
            attr8.Value = $"1";
            var attr9 = doc.CreateAttribute("while");
            attr9.Value = $"not ActionManager.HasSpell({spellId})";
            myNode5.Attributes.Append(attr7);
            myNode5.Attributes.Append(attr8);
            myNode5.Attributes.Append(attr9);
            myNode5.Normalize();
            var newNode4 = newNode1.AppendChild(myNode5);

            doc.Save($@"g:\{spell.Action.Name}.xml");
            // myNode.AppendChild();


/*            if (result != null)
            {
                richTextBox1.Text = $"{result.Name} {result.MapTerritoryID} {result.GetCords()}\n";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").PlaceName.Name}\n";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").IsAetheryte}\n";
                richTextBox1.Text += $"{result.TerritoryType.As<Aetheryte>("Aetheryte").Key}\n";
            }*/

            //richTextBox1.Text += result;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = "";
            foreach (var spell in SaintCHelper.GetBlueMageSpells())
            {
                richTextBox3.Text += $"{spell.Action.Name} - {spell.Action.Key}\n";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var spell = (AozAction) comboBox1.SelectedItem;

            richTextBox2.Text += $"{spell.Action.Name} - {spell.Action.Key}\n";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = SaintCHelper.GetBlueMageSpells().ToList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var levels = SaintCHelper.GetRetainerTaskLevels();

/*
            foreach (var taskRange in levels)
            {
                richTextBox2.Text += $"{taskRange.Key}  ({taskRange.Value.Key} - {taskRange.Value.Value})\n";
            }
*/

            foreach (var taskRange in SaintCHelper.GetRetainerData())
            {
                richTextBox2.Text += $"{taskRange.ToString()}\n";
            }

            //richTextBox2.Text += $"{SaintCHelper.realm.GameData.IsLibraAvailable}";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<RetainerTaskData> levels = SaintCHelper.GetRetainerData();

            using (StreamWriter outputFile = new StreamWriter(@"H:\Ventures.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(levels, (Newtonsoft.Json.Formatting) Formatting.Indented));
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
/*            var cabinet = SaintCHelper.realm.GameData.GetSheet("Cabinet");
            var cabinetList = cabinet.Where(i => i.As<Item>("Item").Key != 0).Select(row => row.As<Item>("Item").Key);
            //int[] test = new[] {1, 3, 3};
            foreach (var item in cabinetList)
            {
                
                richTextBox2.Text += $"{item},\n";
            }*/
            var materia = SaintCHelper.realm.GameData.GetSheet<Materia>();
            //  var list = materia.Where(i => i.Items.Count() > 2 && i.Items.Count() <= 10);
            Dictionary<int, List<MateriaItem>> list = new Dictionary<int, List<MateriaItem>>();
            foreach (var mat in materia.Where(i => i.Items.Count() > 2 && i.Items.Count() <= 10))
            {
                richTextBox2.Text += $"{mat.Key}\n";
                list.Add(mat.Key, new List<MateriaItem>());
                foreach (var item in mat.Items)
                {
                    richTextBox2.Text += $"\t{item.Item} {item.Tier} {item.Value}\n";
                    list[mat.Key].Add(new MateriaItem(item.Item.Key, item.Tier, item.Value, item.BaseParam));
                }
            }

            richTextBox2.Text += $"   {list[23][7].Key}\n"; 
            using (StreamWriter outputFile = new StreamWriter(@"H:\Materia.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(list));
            }
/*            var leves = SaintCHelper.realm.GameData.GetSheet<Leve>();
            var craftleves = SaintCHelper.realm.GameData.GetSheet<CraftLeve>();

            foreach (var item in leves.Where(i=> i.ClassJobCategory.Key == 19 && i.ClassJobLevel >= 70))
            {
                //var items = craftleves[item.DataId]  .Items.First().Item.Name
                //craftleves.First(i=> i.Key == item.DataId).Items.First().Item.Name.ToString()
                
                richTextBox2.Text += $"{item.Key},{item.Name},{item.ClassJobLevel},{item.DataId.Items.First().Item.Name},{item.DataId.Items.First().Item.Key},{item.DataId.Items.First().Count}, {item.DataId.Repeats}\n";
            }*/
        }

        public class MateriaItem
        {
            public int Key;
            public int Tier;
            public int Value;
            public string Stat;

            public MateriaItem(int key, int tier, int value, string stat)
            {
                this.Key = key;
                this.Tier = tier;
                this.Value = value;
                this.Stat = stat;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var input = richTextBox4.Text;

            Regex ItemRegex = new Regex(@"(.*)\s+x\s+(\d+).*", RegexOptions.Compiled);
            List<LisbethOrder> outList = new List<LisbethOrder>();

            var lines = richTextBox4.Text.Split('\n');
            int id = 0;
            foreach (var line in lines)
            {
                if (ItemRegex.IsMatch(line))
                {
                    string test = $@"<CraftAction Name=""{ItemRegex.Match(line).Groups[1]}"" ActionId=""XXXX""/>";
                    string item = ItemRegex.Match(line).Groups[1].Value.Trim().Replace('-', ' ');
                    int count = int.Parse(ItemRegex.Match(line).Groups[2].Value.Trim());
                    var realItem = SaintCHelper.realm.GameData.Items.First(s => s.Name.ToString().Equals(item));
                    var recipe = SaintCHelper.realm.GameData.GetSheet<Recipe>().First(i => i.ResultItem == realItem);

                    LisbethOrder result = new LisbethOrder(id, 1, realItem.Key, count, recipe.ClassJob.Name.ToString().FirstCharToUpper());
                    outList.Add(result);
                    id++;
                    richTextBox2.Text += $"{result}\n"; // $"{item} - {count} {realItem.Key} {recipe.ClassJob.Name.ToString().FirstCharToUpper()}\n";
                }
                else
                {
                    richTextBox2.Text += $"{line}\n";
                }
            }

            richTextBox1.Text = JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var recipes = SaintCHelper.realm.GameData.GetSheet<CompanyCraftSequence>().Skip(1).Where(i => i.CompanyCraftDraftCategory.Key == 2);
            Dictionary<string, Dictionary<Item, int>> itemDictionaryType = new Dictionary<string, Dictionary<Item, int>>();

            foreach (var recipe in recipes)
            {
                var parts = recipe.CompanyCraftParts.Where(i => i.Key > 0);
                string typeS = recipe.ResultItem.Name.ToString().Split('-')[0];
                if (!itemDictionaryType.ContainsKey(typeS))
                    itemDictionaryType.Add(typeS, new Dictionary<Item, int>());

                richTextBox2.Text += $"Part: {recipe.ResultItem.Name} {recipe.CompanyCraftType} {recipe.CompanyCraftDraft}\n";

                //        foreach (var testItem in recipe.CompanyCraftDraft.RequiredItems)
                //        {
                //          richTextBox1.Text += $"{type} {recipe.CompanyCraftDraft != null}\n";
                //        }
                //        
                List<LisbethOrder> outList = new List<LisbethOrder>();
                Dictionary<Item, int> itemDictionary = new Dictionary<Item, int>();
                foreach (var part in parts)
                {
                    var phases = part.CompanyCraftProcesses.Select(i => i.Requests);
                    int phaseNum = 1;
                    //richTextBox2.Text += $"Sub part: {part.CompanyCraftType.Name}\n";
                    foreach (var phase in phases)
                    {
                        //richTextBox2.Text += $"\tPhase: {phaseNum}\n";
                        foreach (var item in phase)
                        {
                            string itemS = item.SupplyItem.Item.Name;
                            int count = item.TotalQuantity;
                            if (itemDictionary.ContainsKey(item.SupplyItem.Item))
                            {
                                itemDictionary[item.SupplyItem.Item] += count;
                            }
                            else
                            {
                                itemDictionary.Add(item.SupplyItem.Item, count);
                            }

                            if (itemDictionaryType[typeS].ContainsKey(item.SupplyItem.Item))
                            {
                                itemDictionaryType[typeS][item.SupplyItem.Item] += count;
                            }
                            else
                            {
                                itemDictionaryType[typeS].Add(item.SupplyItem.Item, count);
                            }
                        }

                        phaseNum++;
                    }
                }

                int id = 0;
                foreach (var item in itemDictionary)
                {
                    var recipeItem = SaintCHelper.realm.GameData.GetSheet<Recipe>().FirstOrDefault(i => i.ResultItem == item.Key);
                    if (recipeItem == null)
                        continue;
                    LisbethOrder result = new LisbethOrder(id, 1, item.Key.Key, item.Value, recipeItem.ClassJob.Name.ToString().FirstCharToUpper());
                    outList.Add(result);
                    richTextBox2.Text += $"\t\t{result}\n";
                    id++;
                }


                var location = Path.Combine("H:\\FCWorkshop", typeS);
                if (!Directory.Exists(location))
                    Directory.CreateDirectory(location);


                using (StreamWriter outputFile = new StreamWriter(Path.Combine(location, $"{recipe.ResultItem.Name}.json"), false))
                {
                    outputFile.Write(JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None));
                }
            }

            foreach (var dictType in itemDictionaryType)
            {
                richTextBox1.Text += $"{dictType.Key}\n";
                List<LisbethOrder> outList = new List<LisbethOrder>();
                int id = 0;
                foreach (var item in dictType.Value)
                {
                    richTextBox1.Text += $"\t{item.Key} x {item.Value}\n";
                    var recipeItem = SaintCHelper.realm.GameData.GetSheet<Recipe>().FirstOrDefault(i => i.ResultItem == item.Key);
                    if (recipeItem == null)
                        continue;
                    LisbethOrder result = new LisbethOrder(id, 1, item.Key.Key, item.Value, recipeItem.ClassJob.Name.ToString().FirstCharToUpper());
                    outList.Add(result);
                    //richTextBox2.Text += $"\t\t{result}\n";
                    id++;
                }

                using (StreamWriter outputFile = new StreamWriter(Path.Combine("H:\\FCWorkshop", $"{dictType.Key}.json"), false))
                {
                    outputFile.Write(JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None));
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var input = richTextBox4.Text;

            //Regex ItemRegex = new Regex(@"(.*)\s+x\s+(\d+).*", RegexOptions.Compiled);
            Regex ItemRegex = new Regex(@"(\d+)x\s+(.+)", RegexOptions.Compiled);
            List<LisbethOrder> outList = new List<LisbethOrder>();
            string listName = textBox2.Text.Trim();

            var lines = richTextBox4.Text.Split('\n');
            int id = 0;
            foreach (var line in lines)
            {
                if (ItemRegex.IsMatch(line))
                {
                    string item = ItemRegex.Match(line).Groups[2].Value.Trim(); //.Replace('-', ' ');
                    int count = int.Parse(ItemRegex.Match(line).Groups[1].Value.Trim());
                    var realItem = SaintCHelper.realm.GameData.Items.First(s => s.Name.ToString().Equals(item));
                    var recipe = SaintCHelper.realm.GameData.GetSheet<Recipe>().First(i => i.ResultItem == realItem);

                    LisbethOrder result = new LisbethOrder(id, 1, realItem.Key, count, recipe.ClassJob.Name.ToString().FirstCharToUpper());
                    outList.Add(result);
                    id++;
                    richTextBox2.Text += $"{item} {result}\n"; // $"{item} - {count} {realItem.Key} {recipe.ClassJob.Name.ToString().FirstCharToUpper()}\n";
                }
                else
                {
                    richTextBox2.Text += $"{line}\n";
                }
            }

            richTextBox1.Text = listName + "\n";
            richTextBox1.Text += JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("H:\\FCWorkshop", $"{listName}.json"), false))
            {
                outputFile.Write(JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None));
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var list = SaintCHelper.realm.GameData.GetSheet<Quest>().Where(i => i.JournalGenre.JournalCategory.Key == 54).Select(i => i.JournalGenre).Distinct();
            


            // var items = SaintCHelper.realm.GameData.Items;

            var items = SaintCHelper.realm.GameData.Items;
            var recipes = SaintCHelper.realm.GameData.GetSheet<Recipe>();
            
            
            
            foreach (var classN in list)
            {
                richTextBox2.Text += $" {classN.Name}\n";
                List<LisbethOrder> outList = new List<LisbethOrder>();
                int group = 0;
                
                var quests = SaintCHelper.realm.GameData.GetSheet<Quest>().Where(i => i.JournalGenre.Key == classN.Key);
                foreach (var quest in quests.OrderBy(q => q.AsInt16("ClassJobLevel[0]")))
                {
                    if (!quest.Instructions.Any(i => i.Key.ToString().Contains("RITEM")))
                        continue;
                    QuestData questData;
                    using (StreamReader sr = new StreamReader($@"G:\titanbotsmono\Toolbox\Dumptruck\bin\Release\2019.11.06.0000.0000\quests\{quest.Key}.json"))
                    {
                        // Read the stream to a string, and write the string to the console.
                        String line = sr.ReadToEnd();
                        questData = JsonConvert.DeserializeObject<QuestData>(line);
                    }

                    richTextBox1.Text += quest.Name + $"({quest.Key}) Level: {quest.AsInt16("ClassJobLevel[0]")}\n";

                    richTextBox3.Text += $"{questData.Name}\n";
                    foreach (var step1 in questData.StepProgression)
                    {
                        richTextBox3.Text += $"{step1.Step} {step1.Todo.HandOver.Count}\n";
                        foreach (var todo in step1.Todo.HandOver)
                        {
                            richTextBox3.Text += $"\t{todo.ItemId} {todo.Count} {todo.HQ_Only}\n";
                        }
                    }

                    int step = 0;
                    foreach (var arg in quest.Instructions.Where(i => i.Key.ToString().Contains("RITEM")))
                    {
                        //richTextBox1.Text += "\t" +  arg.Value + "\n";

                        if (SaintCHelper.realm.GameData.GetSheet<Recipe>().All(i => i.ResultItem.Key != arg.Value))
                            continue;

                        var recipe = SaintCHelper.realm.GameData.GetSheet<Recipe>().First(i => i.ResultItem.Key == arg.Value);


                        //    id++;

                        int count = 1;
                        if (questData.StepProgression.First(i => i.Step == 255).Todo.Count > 0)
                            count = questData.StepProgression.First(i => i.Step == 255).Todo.Count;


                        LisbethOrder result = new LisbethOrder(step, quest.AsInt16("ClassJobLevel[0]"), arg.Value, count, recipe.ClassJob.Name.ToString().FirstCharToUpper());

                        outList.Add(result);

                        richTextBox1.Text += "\t" + recipe.ResultItem.Name + $"  {count}\n";
                        step++;
                    }

                    group++;
                }

               // var lua = new LuaParser(quests.First(i => i.Key == 65671), SaintCHelper.realm);

                //  var test1 = lua.GetTodoArgs(5, 0); 1000691
             //   var test = lua._scriptVars;
             
/*                using (StreamWriter outputFile = new StreamWriter(Path.Combine("H:\\", $"{classN.Name}.json"), false))
                {
                    outputFile.Write(JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.Indented));
                }*/

            }
            //var test1 = lua.GetTodoArgs(1, 0);

            //      var test2 = lua.GetNpcTradeItemInfo(1, 1000691);//lua.TestId(1, 0);
/*            var q = quests.First(i => i.Key == 65671);
            var qnKey = (q["Id"] as XivString);
            var folder = qnKey.ToString().Split('_')[1].Substring(0, 3);
            var text = SaintCHelper.realm.GameData.GetSheet($"quest/{folder}/{qnKey}");
            var textdict = new Dictionary<string, string>();

            foreach (var x in text)
            {
                textdict[(x[0] as XivString).ToString()] = (x[1] as XivString).ToString();
            }


            var todo = BuildTodo(q, textdict, lua, qnKey.ToString().ToUpper());*/

            // richTextBox3.Text += "\t" + questData. + "\n";
            //     foreach (var t in questData.StepProgression)
            //      {
            //         richTextBox3.Text += t.Todo.Description + "\t" + t.Todo.Count + "\n";
            //     }
        }

        Dictionary<byte, TodoStep> BuildTodo(Quest q, Dictionary<string, string> textdict, LuaParser lua, string QuestId)
        {
            var ret = new Dictionary<byte, TodoStep>();
            byte last = 0;
            var t = 0;
            var todoindex = new byte[25] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            for (int c = 0; c < 64; c++)
            {
                var seq = (byte) q.GetRaw($"ActorSpawnSeq[{c}]"); //TODO{Sequence}
                if (seq == 0 && c > 0 && last > 0)
                    break;

                var task_index = (byte) q.GetRaw($"ActorDespawnSeq[{c}]"); //TODO{INDEX}  //task index
                if (task_index == 255)
                {
                    continue;
                }
                //i = last;
                else
                {
                    if (task_index != last)
                    {
                        last = task_index;
                        t = 0;
                    }
                    else
                        t++;
                }

                if (!ret.ContainsKey(task_index))
                {
                    ret[task_index] = new TodoStep();
                    ret[task_index] = new TodoStep();
                    ret[task_index].Task = todoindex[seq == 255 ? 24 : seq]++; // task_index;
                    ret[task_index].Sequence = seq;

                    var args = lua.GetTodoArgs(seq, task_index);
                    ret[task_index].Count = args.Item2;

                    if (task_index == 255)
                        ret[task_index].Description = $"Unknown";
                    else
                        ret[task_index].Description = textdict[$"TEXT_{QuestId}_TODO_{task_index:D2}"];
                }

                if (ret[task_index].Task > task_index)
                {
                    ret[task_index].Task = task_index;
                }


                var bnpc = (uint) q.GetRaw($"TODO{{BNpcName}}[{c}]");
                if (bnpc != 0)
                    ret[task_index].BNpcNames.Add(bnpc);

                if (c < 32)
                    ret[task_index].QuestUInt8A.Add((byte) q.GetRaw($"QuestUInt8A[{c}]"));
                else
                    ret[task_index].QuestUInt8B.Add((byte) q.GetRaw($"QuestUInt8B[{c - 32}]"));

                ret[task_index].UNK_B.Add((byte) q.GetRaw($"TODO{{UNK}}[{c}]")); //405
                ret[task_index].UNK_D.Add((byte) q.GetRaw($"TODO{{UNK_B}}[{c}]")); //533
                ret[task_index].UNK_E.Add((ushort) q.GetRaw($"TODO{{UNK_C}}[{c}]")); //597

                ret[task_index].UNK_BOOL_A.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_A}}[{c}]")); //661
                ret[task_index].UNK_BOOL_B.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_B}}[{c}]")); //726
                ret[task_index].UNK_BOOL_C.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_C}}[{c}]")); //789

                ret[task_index].SetBitF.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_D}}[{c}]")); //853  -- SetBitFlag ?

                ret[task_index].UNK_BOOL_E.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_E}}[{c}]")); //917
                ret[task_index].UNK_BOOL_F.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_F}}[{c}]")); //981
                ret[task_index].UNK_BOOL_G.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_G}}[{c}]")); //1045
                ret[task_index].UNK_BOOL_H.Add((bool) q.GetRaw($"TODO{{UNK_BOOL_H}}[{c}]")); //1109

                var obj = (uint) q.GetRaw($"ActorSpawn[{c}]");
                if (obj != 0)
                {
                    ret[task_index].RelatedObjects.Add(obj);
                    var handOver = lua.GetNpcTradeItemInfo(ret[task_index].Sequence, obj);
                    if (handOver != null)
                        ret[task_index].HandOver.AddRange(handOver);
                }
            }

            return ret;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var items = SaintCHelper.realm.GameData.Items;
            var recipes = SaintCHelper.realm.GameData.GetSheet<Recipe>();
            var Gatherpoints = SaintCHelper.realm.GameData.GetSheet<GatheringPoint>();
            var GatherBase = SaintCHelper.realm.GameData.GetSheet<GatheringPointBase>();
            var GatherItem = SaintCHelper.realm.GameData.GetSheet<GatheringItem>();

           // HashSet<ItemBase> itemList = new HashSet<ItemBase>();
            HashSet<int> itemList = new HashSet<int>();

            foreach (GarlandDBNode bellItem in SaintCHelper.GarlandBellGathering)
            {
                var itemsbell = bellItem.Items.ToList().Select(x => (int)x.Id);
                foreach (var bellItemId in itemsbell)
                {
                    itemList.Add(bellItemId);
                }
            }
            //stacksize == 999
            //point base . gatheringlevel < 70
/*
            foreach (var point in Gatherpoints.Where(i=> i.AsInt16("Type") == 1 && i.Base.GatheringLevel < 70))
            {
                foreach (var item in point.Base.Items.Where(j => j.Item.StackSize == 999))
                {
                    richTextBox2.Text += $"{itemList.Add(item.Item)}";
                }
            }

            foreach (var outItem in itemList)
            {
                richTextBox3.Text += $"{outItem.Name} - {outItem.Key}\n";
            }
*/
            foreach (var outItem in itemList)
            {
                richTextBox3.Text += $"{outItem} - {items[outItem].Name}\n";
            }
            
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("H:\\", $"TimedNodeItems.json"), false))
            {
                outputFile.Write(JsonConvert.SerializeObject(itemList.ToList(), (Newtonsoft.Json.Formatting) Formatting.None));
            }
            //richTextBox3.Text += $"{item.Item.Name} - {item.Key}\n";
        }

        private void button12_Click(object sender, EventArgs e)
        {
                var Json = "[{'Item': 1895,'Group': 0,'Amount': 1,'Collectable': false,'QuickSynth': false,'SuborderQuickSynth': false,'Hq': false,'Food': 0,'Primary': true,'Type': 'Carpenter','Enabled': true,'Manual': 0,'Medicine': 0}]";
                
                JObject settings;
                var settingsFilePath = Path.Combine(@"G:\CleanRB",@"Settings\Kayla D'orden\lisbethV4.json");
                using (StreamReader reader = File.OpenText(settingsFilePath))
                {
                    settings = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                    settings["Orders"] = JToken.Parse(Json);
                }

                using (StreamWriter outputFile = new StreamWriter(settingsFilePath, false))
                {
                    outputFile.Write(JsonConvert.SerializeObject(settings, (Newtonsoft.Json.Formatting) Formatting.None));
                }
            
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var recipes = SaintCHelper.realm.GameData.GetSheet<Recipe>();

            var skybuilders = recipes.Where(row => row.ResultItem.Name.ToString().Contains("Skybuilders"));

            foreach (var recipe in skybuilders)
            {
                richTextBox3.Text += $"{recipe.ResultItem.Name} - {recipe.ResultItem.Key}\n";
                
                richTextBox2.Text += $"{recipe.ResultItem.Key},";
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            var DoH = Enumerable.Range(12, 16);
            var DoL = Enumerable.Range(28, 6);
            var Armor = Enumerable.Range(34, 10);
            var Jewelry = Enumerable.Range(40, 4);
            List<LisbethOrder> outList = new List<LisbethOrder>();
            var recipes = SaintCHelper.realm.GameData.GetSheet<Recipe>();

            var skybuilders = recipes.Where(row => row.ResultItem.AsInt16("Level{Equip}") >= 43 && row.ResultItem.AsInt16("Level{Equip}") <= 43);
            var skybuilders2 = recipes.Where(row => row.ResultItem.AsInt16("Level{Equip}") >= 43 && row.ResultItem.AsInt16("Level{Equip}") <= 43);
            int id = 0;

            var items = skybuilders.Where(i =>
                (Armor.Contains(i.ResultItem.ItemUICategory.Key) || DoH.Contains(i.ResultItem.ItemUICategory.Key)) && !i.ResultItem.Name.ToString().Contains("Vintage") &&
                (i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 70 || i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 71 || i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 11));
            
            var items2 = skybuilders2.Where(i =>
                (Jewelry.Contains(i.ResultItem.ItemUICategory.Key)) &&
                (i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 70 || i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 71 || i.ResultItem.As<BaseParam>("BaseParam[0]").Key == 11));

            
            foreach (var recipe in items.Concat(items2).Distinct())
            {
                richTextBox3.Text += $"{recipe.ResultItem.Name} - {recipe.ResultItem.Key}\n";
                //recipe.ResultItem.
                richTextBox2.Text += $"{recipe.ResultItem.Key},";
                var item = new LisbethOrder(id, 1, recipe.ResultItem.Key,1, recipe.ClassJob.Name.ToString().FirstCharToUpper());
                outList.Add(item);
                id++;
            } 
            
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("H:\\", $"DohLevel38-422.json"), false))
            {
                outputFile.Write(JsonConvert.SerializeObject(outList, (Newtonsoft.Json.Formatting) Formatting.None));
            }
        }
    }
}