public class AlmInnInfo
    {
        public enum QuestType
        {
            Delivery = 0x02,
            RaiseDead = 0x04,
            KillAllHumans = 0x10,
            KillAllMonsters = 0x20,
            KillAllUndeadNecro = 0x40
        }

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();
            Flag = (QuestType)br.ReadUInt32();
            DeliveryID = br.ReadUInt32();
        }

        public uint DeliveryID;
        public QuestType Flag;
        public uint ID;
    }
