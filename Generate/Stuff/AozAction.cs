using SaintCoinach.Ex.Relational;
using SaintCoinach.Xiv;

namespace Generate.Stuff
{
    public class AozAction2: XivRow
    {
        
        public Action Action => As<Action>("Action");
        
        public byte Byte => (byte) GetRaw(1);

        public AozAction2(IXivSheet sheet, IRelationalRow sourceRow) : base(sheet, sourceRow)
        {
            
        }
    }
}