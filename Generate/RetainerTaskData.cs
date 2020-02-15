using Newtonsoft.Json;
using SaintCoinach.Xiv;

namespace Generate
{
    public class RetainerTaskData
    {
        public int Id { get; set; }
        public byte ClassJobCategory { get; set; }
        public bool IsRandom { get; set; }
        public int RetainerLevel { get; set; }
        public int VentureCost { get; set; }
        public int MaxTime { get; set; }
        public int Experience { get; set; }
        public int RequiredItemLevel { get; set; }
        public int RequiredGathering { get; set; }
        public string NameRaw { get; set; }
        public int ItemId { get; set; }

        public RetainerTaskData(RetainerTask task, RetainerTaskNormal norm)
        {
            Id = task.Key;
            ClassJobCategory = task.ClassJobCategory;
            IsRandom = task.IsRandom;
            RetainerLevel = task.RetainerLevel;
            VentureCost = task.VentureCost;
            MaxTime = task.MaxTime;
            Experience = task.Experience;
            RequiredItemLevel = task.RequiredItemLevel;
            RequiredGathering = task.RequiredGathering;
            NameRaw = norm.Item.Name.ToString();
            ItemId = norm.Item.Key;
        }
        
        public RetainerTaskData(RetainerTask task, RetainerTaskRandom rand)
        {
            Id = task.Key;
            ClassJobCategory = task.ClassJobCategory;
            IsRandom = task.IsRandom;
            RetainerLevel = task.RetainerLevel;
            VentureCost = task.VentureCost;
            MaxTime = task.MaxTime;
            Experience = task.Experience;
            RequiredItemLevel = task.RequiredItemLevel;
            RequiredGathering = task.RequiredGathering;
            NameRaw = rand.Name;
            ItemId = 0;
        }
        
        public RetainerTaskData(RetainerTask task, RetainerTaskBase rand)
        {
            Id = task.Key;
            ClassJobCategory = task.ClassJobCategory;
            IsRandom = task.IsRandom;
            RetainerLevel = task.RetainerLevel;
            VentureCost = task.VentureCost;
            MaxTime = task.MaxTime;
            Experience = task.Experience;
            RequiredItemLevel = task.RequiredItemLevel;
            RequiredGathering = task.RequiredGathering;
            if (IsRandom)
            {
                NameRaw = ((RetainerTaskRandom) rand).Name;
                ItemId = 0;
            }
            else
            {
                NameRaw = ((RetainerTaskNormal) rand).Item.Name.ToString();
                ItemId = ((RetainerTaskNormal) rand).Item.Key;
            }
        }

        [JsonConstructor]
        public RetainerTaskData(int id, byte classJobCategory, bool isRandom, int retainerLevel, int ventureCost, int maxTime, int experience, int requiredItemLevel, int requiredGathering, string nameRaw, int itemId)
        {
            Id = id;
            ClassJobCategory = classJobCategory;
            IsRandom = isRandom;
            RetainerLevel = retainerLevel;
            VentureCost = ventureCost;
            MaxTime = maxTime;
            Experience = experience;
            RequiredItemLevel = requiredItemLevel;
            RequiredGathering = requiredGathering;
            NameRaw = nameRaw;
            ItemId = itemId;
        }

        public override string ToString()
        {
            return $"{Id} - {IsRandom} {NameRaw}";
        }
    }
}