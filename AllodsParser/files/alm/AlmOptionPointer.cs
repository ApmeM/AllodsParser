public class AlmOptionPointer
    {
        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            InstanceOn = br.ReadUInt32();
            InstanceID = br.ReadUInt32();
        }

        public uint InstanceID;
        public uint InstanceOn;
        public uint ID;
    }
