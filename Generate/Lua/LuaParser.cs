//﻿using log4net;
using NLua;
using NLua.Exceptions;
using SaintCoinach;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
//using ProtoBuf;
//using Dumptruck.XivDb;
using TitanBots.Remote;

namespace QuestMaster.LuaParser
{


    public class LuaParser
    {
        // private static readonly ILog log = LogManager.GetLogger(typeof(LuaParser));

        public ARealmReversed realm { get; set; }
        public SaintCoinach.Xiv.Quest scQuest { get; set; }

        //public QuestData BuiltQuest { get; set; }

        private Lua state;

        private string Dict;

        public LuaParser(SaintCoinach.Xiv.Quest scQuest, ARealmReversed realm)
        {
            //BuiltQuest = built;
            this.scQuest = scQuest;
            this.realm = realm;
            SetupState();
        }

        /// <summary>
        /// will setup the lua state
        /// </summary>
        void SetupState()
        {
            state = new Lua();
            //allow us to get a lua stack
            state.UseTraceback = true;
            state.RegisterFunction("print", this, this.GetType().GetMethod("Print"));

            //var x = state.DoString("print('hello world')");

            Dict = scQuest.Id.ToString().Split('_')[0];

            //state[Dict] = new Dictionary<string, object>();
            state.DoString(Dict + " = {}");

            for (int i = 0; i < 50; i++)
            {
                var key = scQuest.AsString($"Script{{Instruction}}[{i}]").ToString();
                var value = scQuest.AsInt32($"Script{{Arg}}[{i}]");
                if (string.IsNullOrEmpty(key))
                    continue;
                _scriptVars[key] = value;

                state.DoString($"{Dict}.{key} = {value}");
            }

            for (int i = 0; i < 30; i++)
            {
                state.DoString($"{Dict}.SEQ_{i} = {i}");
            }

            state.DoString($"{Dict}.SEQ_OFFER = 0");
            //SEQ_FINISH
            state.DoString($"{Dict}.SEQ_FINISH = 255");
            state.DoString($"{Dict}.NUM_OF_ITEMS_FILTER_NQ = {(byte) ItemFilter.NQ}");
            state.DoString($"{Dict}.NUM_OF_ITEMS_FILTER_HQ = {(byte) ItemFilter.HQ}");
            state.DoString($"{Dict}.NUM_OF_ITEMS_FILTER_NQ_OR_HQ = {(byte) ItemFilter.NQ_OR_HQ}");
            state.DoString($"{Dict}.NUM_OF_ITEMS_FILTER_NQ_AND_HQ = {(byte) ItemFilter.NQ_AND_HQ}");
            state.DoString($@"
function {Dict}:GetQuestId()
    return {scQuest.Key}
end");

            cleanFile();

        }

        public int TestId(int seq, int todo)
        {
            var p = new LuaGameObject(seq);
            state["player"] = p;
            var func = state.DoString($"return {Dict}:GetTodoArgs(player, {todo})");
            return Convert.ToInt32(func[0]);
        }
    

    /// <summary>
        /// GetTodoArgs()[Sequence][TODO]{Param1, Qty}
        /// </summary>
        /// <returns></returns>
        public Tuple<ITODO, int> GetTodoArgs(int seq, int todo)
        {
            try
            {
                var p = new LuaGameObject(seq);
                state["player"] = p;

                var func = state.DoString($"return {Dict}:GetTodoArgs(player, {todo})");
                //state[$"{Dict}:GetTodoArgs"] as LuaFunction;
                if (func == null)
                    return new Tuple<ITODO, int>(null, 0);

                ITODO ret = null;
                if (func[0] != null && func[0] is ITODO)
                {
                    ret = func[0] as ITODO;
                }

                return new Tuple<ITODO, int>(ret, Convert.ToInt32(func[1]));
            }catch(Exception ex)
            {
                throw ex;
            }
        }


        //public bool TestUseItem()
        //{
        //    var p = new LuaGameObject(9);
        //    state["player"] = p;
        //    var go = new LuaGameObject(_scriptVars["EVENTRANGE0"]);
        //    state["Target"] = go;
        //    var func = state.DoString($"return {Dict}:IsEventItemUsable(player, Target, {_scriptVars["ITEM1"]})");

        //    return func != null && (bool)func[0];
        //}


        public List<UseItem> GetItemUseable(int seq)
        {
            if (!ItemUseable) return null;
            var p = new LuaGameObject(seq);
            state["player"] = p;

            var items = _scriptVars.Where(i => i.Key.StartsWith("ITEM")).ToList();
            var ret = new List<UseItem>();

            foreach (var k in _scriptVars)
            {
                if (k.Key.StartsWith("EVENTRANGE") ||
                    k.Key.StartsWith("EOBJECT") ||
                    k.Key.StartsWith("ENEMY")
                    )
                {
                    var go = new LuaGameObject(k.Value);
                    state["gameObject"] = go;
                    foreach (var item in items)
                    {
                        var func = state.DoString($"return {Dict}:IsEventItemUsable(player, gameObject, {item.Value})");
                        if(func == null)
                        {
                            //log.Warn("Failed to do IsEventItemUsable");
                        }
                        if(func[0] is bool && ((bool)func[0]))
                        {
                            var toadd = new UseItem
                            {
                                ObjectId = k.Value,
                                ItemId = item.Value,
                            };
                            if(go.LastField != null)
                            {
                                toadd.GameObjectState = (byte)go.LastField.Item1;
                            }
                            if(p.LastField != null)
                            {
                                toadd.Field = (byte)p.LastField.Item1;
                                toadd.FieldPos = (byte)p.LastField.Item2;

                                p.LastField = null;
                            }
                            ret.Add(toadd);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// get the actions we have to perform on the target
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="actor"></param>
        /// <returns></returns>
        public List<ActionTarget> IsActionTarget(int seq)
        {
            if (!ActionTarget) return null;

            var p = new LuaGameObject(seq);
            state["player"] = p;

            var ret = new List<ActionTarget>();

            var actors = _scriptVars.Where(i => i.Key.StartsWith("ACTOR")).ToList();
            foreach (var actor in actors)
            {
                var a = new LuaGameObject(actor.Value);
                state["target"] = a;

                var actions = _scriptVars.Where(i => i.Key.StartsWith("ACTION")).ToList();
                foreach(var action in actions)
                {
                    var func = state.DoString($"return {Dict}:IsActionTarget(player, target, {action.Value})");
                    if(func[0] is bool && ((bool)func[0]))
                    {
                        ret.Add(new ActionTarget
                        {
                            SpellId = (uint)action.Value,
                            actor = (uint)actor.Value
                        });
                    }
                }
            }

            return ret;

        }


        public List<HandOverItem> GetNpcTradeItemInfo(int sequence, uint actor)
        {
            List<HandOverItem> ret = new List<HandOverItem>();
            try
            {
                state.DoString("tabledata = {}"); //wipe the table
              
                var func = state.DoString($"return {Dict}:{tradeFunction}(1, {sequence}, {actor})");
                var data = state.DoString($"return tabledata");
                if (data is null || data[0] is null)
                    return null;

                
                foreach (LuaTable record in data)
                {
                    foreach (LuaTable row in record.Values)
                    {
                        ret.Add(new HandOverItem
                        {
                            ItemId = Convert.ToUInt32(row[1]),
                            Count = Convert.ToUInt32(row[2]),
                            HQ_Only = Convert.ToBoolean(row[3]),
                            UNK_3 = row[4],
                            UNK_4 = row[5],
                            UNK_5 = row[6],
                            UNK_6 = row[7],
                            UNK_7 = row[8],
                            UNK_8 = row[9],
                            UNK_9 = row[10],
                            UNK_10 = row[11],
                            UNK_11 = row[12],
                            UNK_12 = row[13],
                            UNK_13 = row[14],
                            EventItem = false
                        });
                    }
                }
            }
            catch (LuaScriptException ex)
            {
                if (!ex.Message.Contains($"attempt to call method '{tradeFunction}' (a nil value)"))
                {
                    throw;
                }
            }
            try
            {
                state.DoString("tabledata = {}"); //wipe the table

                var p = new LuaGameObject(sequence);

                state["player"] = p;

                var funcb = state.DoString($"return {Dict}:GetEventItems(player)"); //ItemId, QtyInInventory, HQ?

                if(funcb != null)
                {
                    ret.Add(new HandOverItem
                    {
                        ItemId = Convert.ToUInt32(funcb[0]),
                        Count = 1,
                        HQ_Only = Convert.ToBoolean(funcb[1]),
                        EventItem = true
                    });
                }
            }
            catch (LuaScriptException ex)
            {
                if (!ex.Message.Contains("attempt to call method 'GetEventItems' (a nil value)"))
                {
                    throw;
                }
            }

            return ret;
        }

        public Dictionary<string, int> _scriptVars = new Dictionary<string, int>();
        private bool ItemUseable = false;
        private bool ActionTarget = false;

       

        private string tradeFunction = "getNpcTradeItemInfo";

        private void cleanFile()
        {
            var fileName = $@"G:\titanbotsmono\Toolbox\Dumptruck\bin\Release\2019.12.19.0000.0000\game_script\quest\{scQuest.Key.ToString()}.luab";
            //var fileName = $@"generated\{scQuest.Id.ToString()}.lua";
            string text = File.ReadAllText(fileName);
            // text = text.Replace("--- quest data section start\r\n", "");
            //// 

            // text = text.Replace("({})", "tabledata");

            // //force case
            if (text.Contains("GetNpcTradeItemInfo"))
                tradeFunction = "GetNpcTradeItemInfo";

            ItemUseable = CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, "IsEventItemUsable", CompareOptions.IgnoreCase) >= 0;
            ActionTarget = CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, "IsActionTarget", CompareOptions.IgnoreCase) >= 0;
            try
            {
                state.DoFile(fileName);
              //  state.DoString(text);
            }
            catch (LuaException ex)
            {
                //log.Warn($"[LUA] Failed to pares full quest data for {scQuest.Key} - {scQuest.Name}");
//                state.DoString(text.Split(new string[] { "()\r\n;" }, StringSplitOptions.None)[1]);
            }
        }

        public void Print(object format)
        {
            //log.Info($"[LUA] {format}");
        }
    }
}
