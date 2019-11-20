namespace SaintCoinach.Cmd
{
    public class ClassAction
    {
        public int ActionId { get; }
        public string Name { get; }
        public string Class { get; }

        public ClassAction(int actionId, string name, string @class)
        {
            ActionId = actionId;
            Name = name;
            Class = @class;
        }

        public override string ToString()
        {
            return $"{Name} {ActionId} {Class}";
        }
    }
}