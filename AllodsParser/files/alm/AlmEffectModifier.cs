public class AlmEffectModifier
    {
        public void LoadFromStream(BinaryReader br)
        {
            TypeOfMod = br.ReadUInt16();
            Value = br.ReadUInt32();
        }

        public uint Value;
        public ushort TypeOfMod;
    }
