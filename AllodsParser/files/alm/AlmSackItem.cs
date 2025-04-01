public class AlmSackItem
    {
        public void LoadFromStream(BinaryReader br)
        {
            ItemID = br.ReadUInt32();
            Wielded = br.ReadUInt16();
            EffectNumber = br.ReadUInt32();
        }

        public uint EffectNumber;
        public ushort Wielded;
        public uint ItemID;
    }
