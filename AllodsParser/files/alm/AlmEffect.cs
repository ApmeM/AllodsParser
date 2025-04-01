public class AlmEffect
    {
        public bool IsItem;

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            IsItem = X == 0 && Y == 0;
            MagicOrFlag = br.ReadUInt16();
            if (!IsItem)
            {
                StructureID = br.ReadUInt32();
            }
            else
            {
                MagicMinDmg = br.ReadUInt16();
                AvgDmg = br.ReadUInt16();
            }
            TypeID = br.ReadUInt16();
            MagicPower = br.ReadUInt16();
            CountOfModifier = br.ReadUInt32();
            EffectModifiers = new AlmEffectModifier[CountOfModifier];
            for (int i = 0; i < CountOfModifier; i++)
            {
                EffectModifiers[i] = new AlmEffectModifier();
                EffectModifiers[i].LoadFromStream(br);
            }
        }

        public uint CountOfModifier;
        public ushort MagicPower;
        public ushort TypeID;
        public ushort AvgDmg;
        public ushort MagicMinDmg;
        public uint StructureID;
        public uint MagicOrFlag;
        public uint Y;
        public uint X;
        public uint ID;
        public AlmEffectModifier[] EffectModifiers;
    }
