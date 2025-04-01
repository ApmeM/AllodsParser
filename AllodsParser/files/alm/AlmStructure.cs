public class AlmStructure
    {
        public float X;
        public float Y;
        public int TypeID;
        public short Health;
        public int Player;
        public int ID;
        public bool IsBridge;
        public int Width;
        public int Height;

        public void LoadFromStream(BinaryReader br)
        {
            int xRaw = br.ReadInt32();
            int yRaw = br.ReadInt32();
            X = ((xRaw & 0x0000FF00) >> 8) + (float)(xRaw & 0x00FF) / 256; // 00 10 00 80 = 16.5
            Y = ((yRaw & 0x0000FF00) >> 8) + (float)(yRaw & 0x00FF) / 256;
            TypeID = br.ReadInt32();
            Health = br.ReadInt16();
            Player = br.ReadInt32();
            ID = br.ReadInt16();
            if ((TypeID & 0x01000000) != 0)
            {
                Width = br.ReadInt32();
                Height = br.ReadInt32();
                IsBridge = true;
            }
            TypeID &= 0xFFFF;
        }
    }
