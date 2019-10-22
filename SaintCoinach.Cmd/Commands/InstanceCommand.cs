using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using SaintCoinach.Xiv;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

#pragma warning disable CS1998

namespace SaintCoinach.Cmd.Commands {
    public class InstanceCommand : ActionCommandBase {
        private ARealmReversed _Realm;

        public InstanceCommand(ARealmReversed realm)
            : base("instance", "Save Instance - ContentFinderCondition Id.") {
            _Realm = realm;
        }

        public override async Task<bool> InvokeAsync(string paramList) 
        {
            string CsvFileFormat = "instanceIds.txt";

    


            
            var ContentFinderCond = _Realm.GameData.GetSheet<ContentFinderCondition>();
            var InstanceContent = _Realm.GameData.GetSheet<InstanceContent>();
            //var InstanceContent = _Realm.GameData.GetSheet("InstanceContent");
            Dictionary<int,int> mapping = new Dictionary<int, int>();

            foreach (var instance in InstanceContent)
            {
                if (instance.ContentFinderCondition == 0) continue;
                mapping.Add(instance.Key, instance.ContentFinderCondition);
                
                

            }

            OutputInformation($"Count - Here");
            var keyList = mapping.Keys.ToArray();
            OutputInformation($"Count - {keyList.Length}");
            
            StringBuilder sb = new StringBuilder();
            //OutputInformation($" {keyList[0]},");  //{mapping[keyList[keyList.Count -1]]}}},");
            sb.Append("Dictionary<uint, uint> ContendFinderIds = new Dictionary<uint, uint> \n{\n");
          
            for (int j = 0; j < (keyList.Length -1); j++)
            {
                //OutputInformation($"{{{keyList[j]}, }},");
                OutputInformation($"{keyList[j]},  {mapping[keyList[j]]},");
                sb.Append($"\t{{{keyList[j]},  {mapping[keyList[j]]}}},\n");
            }
        
            OutputInformation($"{keyList[keyList.Length -1]},  {mapping[keyList[keyList.Length -1]]},");
            
            sb.Append($"\t{{{keyList[keyList.Length -1]},  {mapping[keyList[keyList.Length -1]]}}}\n");
            sb.Append("};\n");
            
            //OutputInformation($"{{{instance.Key},  {instance.ContentFinderCondition}}},");
            
            OutputInformation($"Count - {mapping.Count}");
            
            try
            {
                using (StreamWriter outputFile = new StreamWriter(CsvFileFormat, false))
                {
                    outputFile.Write(sb);
                }
            } catch (Exception e) {
                OutputError(e.Message);
            }

            return true;
        }
    }
}
