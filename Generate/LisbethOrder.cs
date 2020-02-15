
namespace Generate
{
    public class LisbethOrder
    {
        public int Id;
        public int Group;
        public int Item;
        public int Amount;
        public bool Enabled = true;
        public string Type;

        public LisbethOrder(int id, int @group, int item, int amount, string type)
        {
            Id = id;
            Group = @group;
            Item = item;
            Amount = amount;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Id}, {Group}, {Item}, {Amount}, {Enabled}, {Type}";
        }
        
        public enum SourceType
        {
            None,
            Purchase,
            Gather,
            Grind,
            Fisher,
            Carpenter,
            Blacksmith,
            Armorer,
            Goldsmith,
            Leatherworker,
            Weaver,
            Alchemist,
            Culinarian,
            Desynthesize,
            Spiritbind,
            ConvertMateria,
            RetainerRefresh,
            Exchange,
            Retainer,
            GatherMasterpiece,
            CraftMasterpiece,
            Leve
        }
    }
}