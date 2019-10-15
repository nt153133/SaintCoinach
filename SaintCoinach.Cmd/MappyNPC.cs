using FileHelpers;

namespace SaintCoinach.Cmd
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class MappyNPC
    {
        public string Hash;
        public string ContentIndex;
        public int ENpcResidentID;
        public int BNpcNameID;
        public int BNpcBaseID;
        public string Name;
        public string Type;
        public string MapID;
        public int MapIndex;
        public int MapTerritoryID;
        public int PlaceNameID;
        public float CoordinateX;
        public float CoordinateY;
        public float CoordinateZ;

        public float PosX;

        public float PosY;

        public float PixelX;

        public float PixelY;
       // [FieldValueDiscarded]
        public string Managed;

        [FieldOptional] public string other;
    }
}