public class AlmPlayer
    {
        public int Color;
        public uint Flags;//1=AI,2=quest kill
        public int Money;
        public string Name;
        public ushort[] Diplomacy;

        public void LoadFromStream(BinaryReader br)
        {
            Color = br.ReadInt32();
            Flags = br.ReadUInt32();
            Money = br.ReadInt32();
            Name = Core.UnpackByteString(1251, br.ReadBytes(0x20));
            Diplomacy = new ushort[16];
            for (int i = 0; i < 16; i++)
                Diplomacy[i] = br.ReadUInt16();
        }
    }
