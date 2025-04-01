public class AlmSack
    {
        public void LoadFromStream(BinaryReader br)
        {
            NumberOfItems = br.ReadUInt32();
            UnitID = br.ReadUInt32();
            X = br.ReadUInt32();
            Y = br.ReadUInt32();
            Gold = br.ReadUInt32();
            Items = new AlmSackItem[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                Items[i] = new AlmSackItem();
                Items[i].LoadFromStream(br);
            }
        }

        public uint Gold;
        public uint Y;
        public uint X;
        public uint UnitID;
        public uint NumberOfItems;
        public AlmSackItem[] Items;
    }
