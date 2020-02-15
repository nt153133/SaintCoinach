using System.Collections.Generic;
using SaintCoinach.Ex.Relational;

namespace SaintCoinach.Xiv
{
    public class Aetheryte : XivRow, ILocatable
    {
        private IEnumerable<ILocation> _Locations;
        
        public PlaceName PlaceName => As<PlaceName>("PlaceName");

        public PlaceName AethernetName => As<PlaceName>("AethernetName");
        
        public TerritoryType Territory => As<TerritoryType>("Territory");
        
        public Map Map => As<Map>("Map");

        public bool IsAetheryte => AsBoolean("IsAetheryte");

        public Aetheryte(IXivSheet sheet, IRelationalRow sourceRow) : base(sheet, sourceRow)
        {
        }

        public IEnumerable<ILocation> Locations => new []{As<Level>("Level[0]"),As<Level>("Level[1]"),As<Level>("Level[2]"),As<Level>("Level[3]")};
    }
}