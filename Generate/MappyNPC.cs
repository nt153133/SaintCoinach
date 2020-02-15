
 using Clio.Utilities;
 using FileHelpers;

 using SaintCoinach.Xiv;

 namespace Generate
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
        [FieldQuoted]
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


        public string GetCords() => $"{CoordinateX}, {CoordinateZ}, {CoordinateY}";

        public TerritoryType TerritoryType => SaintCHelper.GetTerritoryById(MapTerritoryID);
        
        public Vector3 Location => new Vector3(CoordinateX, CoordinateZ, CoordinateY);


    }
}