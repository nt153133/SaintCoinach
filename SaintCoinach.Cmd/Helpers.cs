namespace SaintCoinach.Cmd
{
    public class Helpers
    {
        
    }
    
    class LevelBuilder
    {
        public static Level Level(SaintCoinach.Xiv.Level lvl)
        {
            if (lvl == null)
                return null;
            var ret = new Level();
            ret.Id = (uint)lvl.GetRaw("Object"); //Object.Key;
            ret.ObjectId = lvl.Key;
            ret.Type = lvl.Type;
            var x = lvl.GetRaw(9);
            ret.ZoneId = (ushort)x;
            ret.X = lvl.X;
            ret.Y = lvl.Y;
            ret.Z = lvl.Z;
            ret.Radius = lvl.Radius;
            return ret;
        }
    }
}