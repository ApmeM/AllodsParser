public class AlmMusic
    {
        public void LoadFromStream(BinaryReader br)
        {
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            Radius = br.ReadUInt32();
            for (int i = 0; i < TypeIDs.Length; i++)
                TypeIDs[i] = br.ReadUInt32();
        }

        public uint[] TypeIDs = new uint[4];
        public uint Radius;
        public uint Y;
        public uint X;
    }
