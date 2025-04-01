public class AlmGroup
    {
        [Flags]
        public enum AlmGroupFlags
        {
            AiInstantEnabled = 1, RandomPositions = 2,
            QuestKill = 4, QuestIntercept = 8 
        }

        public void LoadFromStream(BinaryReader br)
        {
            GroupID = br.ReadUInt32();
            RepopTime = br.ReadUInt32();
            GroupFlag = (AlmGroupFlags)br.ReadUInt32();
            InstanceID = br.ReadUInt32();
        }

        public uint InstanceID;
        public AlmGroupFlags GroupFlag;
        public uint RepopTime;
        public uint GroupID;
    }
