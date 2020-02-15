using SaintCoinach.Ex.Relational;
using SaintCoinach.Imaging;

namespace SaintCoinach.Xiv
{
    public class MapMarker : XivSubRow
    {

        public int X => AsInt16("X");
        public int Y => AsInt16("Y");

        public ImageFile Icon { get { return AsImage("Icon"); } }

        public PlaceName Place => As<PlaceName>("PlaceName{Subtext}");


        public MapMarker(IXivSheet sheet, IRelationalRow sourceRow) : base(sheet, sourceRow)
        {
        }
    }
}