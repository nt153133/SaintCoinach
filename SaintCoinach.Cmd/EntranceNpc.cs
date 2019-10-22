namespace SaintCoinach.Cmd
{
    public class EntranceNpc
    {
        public int NpcId { get; }
        public string Name { get; }
        public float[] Location;
        public int MapId { get; }
        public int AetheryteId { get; set; }

        public EntranceNpc(float[] location = null, int npcId = default, string name = null, int mapId = default, int aetheryteId = default)
        {
            Location = location;
            NpcId = npcId;
            Name = name;
            MapId = mapId;
            AetheryteId = aetheryteId;
        }
        
        public EntranceNpc(MappyNPC npc, int aetheryteId)
        {
            Location = new []{npc.CoordinateX, npc.CoordinateZ, npc.CoordinateY};
            NpcId = npc.ENpcResidentID;
            Name = npc.Name.Replace('"',' ').Trim();
            MapId = npc.MapTerritoryID;
            AetheryteId = aetheryteId;
        }

        public override string ToString()
        {
            return $"NPC:\n\tNpcId: {NpcId}\n\tName: {Name}\n\tZoneId: {MapId}\n\tAetheryteId: {AetheryteId}\n\tLocation: ({Location[0]}, {Location[1]}, {Location[2]})";
        }
    }
}