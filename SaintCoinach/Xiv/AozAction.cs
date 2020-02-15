using SaintCoinach.Ex.Relational;

namespace SaintCoinach.Xiv
{
    public class AozAction: XivRow
    {
        
        public Action Action => As<Action>("Action");

        public string Name => Action.Name;
        
        public byte Byte => (byte) GetRaw(1);

        public AozAction(IXivSheet sheet, IRelationalRow sourceRow) : base(sheet, sourceRow)
        {
            
        }
    }
}