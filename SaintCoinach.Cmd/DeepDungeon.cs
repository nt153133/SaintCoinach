using System.Collections.Generic;

namespace SaintCoinach.Cmd
{
    public class DeepDungeon
    {
        public int Index;
        public string Name;
        public string NameWithoutArticle;
        public int ContentFinderId;
        public Dictionary<int, int> PomanderMapping;

        public int LobbyId;
        public int UnlockQuest;
        public EntranceNpc Npc;

        public List<FloorSetting> Floors = new List<FloorSetting>();
        //internal int StartAt;
        //internal int StartAtContentFinderId;

        public DeepDungeon(int index, string name, int contentFinderId)
        {
            Index = index;
            Name = name;
            ContentFinderId = contentFinderId;
        }

        public DeepDungeon(int index, string name, string nameWithoutArticle, int contentFinderId, Dictionary<int, int> pomanderMapping, int lobbyId, int unlockQuest, EntranceNpc npc)
        {
            Index = index;
            Name = name;
            NameWithoutArticle = nameWithoutArticle;
            ContentFinderId = contentFinderId;
            PomanderMapping = pomanderMapping;
            LobbyId = lobbyId;
            UnlockQuest = unlockQuest;
            Npc = npc;
        }

        public override string ToString()
        {
            var output =
                $"{NameWithoutArticle} ({Index})\n" +
                $"Lobby: {LobbyId}\n" +
                $"UnlockQuest: {UnlockQuest}\n" +
                $"{Npc}";
                    
            return output;
        }

    }

    public class FloorSetting
    {
        public string Name;
        public int InstanceId;
        public int ContentFinderId;
        public int MapId;
        public int QuestId;

        public int Start;

        public int End;
        //public string QuestName;

        public FloorSetting(string name, int instanceId, int mapId, int questId)
        {
            Name = name;
            InstanceId = instanceId;
            MapId = mapId;
            QuestId = questId;
        }

        public FloorSetting(string name, int instanceId, int contentFinderId, int mapId, int questId, int start, int end)
        {
            Name = name;
            InstanceId = instanceId;
            ContentFinderId = contentFinderId;
            MapId = mapId;
            QuestId = questId;
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(FloorSetting))
            {
                return ((FloorSetting) obj).InstanceId == this.InstanceId;
            }
            return base.Equals(obj);
        }

        //Name: {ct.Name} MapId: {ter.Key} Quest: {questid.Key} ContentFinderId: {CF.Key} InstanceID: {ic.Key}
    }
}