using SaintCoinach.Ex.Relational;

namespace SaintCoinach.Xiv
{
    public class SpearfishingNotebook : XivRow
    {

        public int X => AsInt16("X");
        public int Y => AsInt16("Y");
        public uint Radius => As<ushort>("Radius");
        public PlaceName PlaceName => As<PlaceName>("PlaceName");
        public GatheringPointBase GatheringPointBase => As<GatheringPointBase>("GatheringPointBase");

        public SpearfishingNotebook(IXivSheet sheet, IRelationalRow sourceRow) : base(sheet, sourceRow)
        {
        }
    }
}