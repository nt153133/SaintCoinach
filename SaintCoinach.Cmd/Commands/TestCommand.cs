using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaintCoinach.Xiv;
using Tharga.Toolkit.Console.Command.Base;

namespace SaintCoinach.Cmd.Commands
{
    public class TestCommand : ActionCommandBase
    {
        private ARealmReversed _Realm;

        public TestCommand(SaintCoinach.ARealmReversed realm) : base("test", "Just testing")
        {
            _Realm = realm;
        }



        public override Task<bool> InvokeAsync(string paramList)
        {
            var recipes = _Realm.GameData.GetSheet<Recipe>();
            var craftaction = _Realm.GameData.GetSheet<CraftAction>();
            var generalActions = _Realm.GameData.GetSheet<Action>();
            var classes = _Realm.GameData.GetSheet<ClassJob>();

            var actions = craftaction.Where(row => row.ClassJobCategory.Key != 0 && row.ClassJob != null);
            List<ClassAction> actionList = new List<ClassAction>();
            foreach (var job in classes)
            {
                //OutputInformation($"{craft.Name} {craft.Key} {craft.ClassJob.Name.ToString()}");
                //var temp = new ClassAction(craft.Key, craft.Name.ToString(), craft.ClassJob.Name.ToString().FirstCharToUpper());
                OutputInformation($"{job.Name.ToString().FirstCharToUpper()} = {job.Key},");
                //actionList.Add(temp);
            }

            return Task.FromResult(true);
        }
    }
}