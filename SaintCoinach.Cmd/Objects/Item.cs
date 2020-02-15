namespace SaintCoinach.Cmd
{
    public class Item
    {
        public Item()
        {
        }

        internal Item(bool hq = false)
        {
            //IsHighQuality = hq;
        }

        public uint StackSize { get; set; }

        public uint GilBase { get; set; }

        public uint RepairItemId { get; set; }

        public ushort Icon { get; set; }

        public ushort PhysicalDamage { get; set; }

        public ushort MagicDamage { get; set; }

        public ushort Delay { get; set; }

        public ushort BlockRate { get; set; }

        public ushort BlockStrength { get; set; }

        public ushort Defense { get; set; }

        public ushort MagicDefense { get; set; }

        public ushort Stat1 { get; set; }

        public ushort Stat2 { get; set; }

        public ushort Stat3 { get; set; }

        public ushort Stat4 { get; set; }

        public ushort Stat5 { get; set; }

        public ushort Stat6 { get; set; }

        public ushort ItemLevel { get; set; }

        public byte RequiredLevel { get; set; }

       // public ItemRole ItemRole { get; set; }

        //public ItemUiCategory EquipmentCatagory { get; set; }

        public byte Rarity { get; set; }

        public byte Convertible { get; set; }

        public byte MateriaSlots { get; set; }

        public byte EquipSlotCategory { get; set; }

        public ItemAttribute StatType1 { get; set; }

        public ItemAttribute StatType2 { get; set; }

        public ItemAttribute StatType3 { get; set; }

        public ItemAttribute StatType4 { get; set; }

        public ItemAttribute StatType5 { get; set; }

        public ItemAttribute StatType6 { get; set; }

        //public BonusType SetBonusType { get; set; }

        public ItemAttribute SetBonusStatType1 { get; set; }

        public ItemAttribute SetBonusStatType2 { get; set; }

        public ItemAttribute SetBonusStatType3 { get; set; }

        public ItemAttribute SetBonusStatType4 { get; set; }

        public ItemAttribute SetBonusStatType5 { get; set; }

        public byte RepairClass { get; set; }

        public byte Affinity { get; set; }

        //public GrandCompany UniformId { get; set; }

        public byte GilModifier { get; set; }

        public short SetBonusValue1 { get; set; }

        public short SetBonusValue2 { get; set; }

        public short SetBonusValue3 { get; set; }

        public short SetBonusValue4 { get; set; }

        public short SetBonusValue5 { get; set; }

        public byte AetherialReductionIndex { get; set; }

        public ushort DesynthesisIndex { get; set; }
        
        
    }
}