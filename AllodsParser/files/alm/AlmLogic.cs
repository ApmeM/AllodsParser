using System.Numerics;

public class AlmLogic
    {
        public AlmLogicItem[] LogicItems;
        public AlmCheckItem[] CheckItems;
        public AlmTrigger[] TriggerItems;
        public enum InstanceType
        {
            Num = 1, Group = 2,
            Player = 3, Unit = 4,
            X = 5, Y = 6,
            Item = 8, Building = 9
        }

        public void LoadFromStream(BinaryReader br)
        {
            NumberOfItems = br.ReadUInt32();
            LogicItems = new AlmLogicItem[NumberOfItems];
            for (int i = 0; i < NumberOfItems; i++)
            {
                LogicItems[i] = new AlmLogicItem();
                LogicItems[i].LoadFromStream(br);
            }
            NumberOfChecks = br.ReadUInt32();
            CheckItems = new AlmCheckItem[NumberOfChecks];
            for (int i = 0; i < NumberOfChecks; i++)
            {
                CheckItems[i] = new AlmCheckItem();
                CheckItems[i].LoadFromStream(br);
            }
            NumberOfTriggers = br.ReadUInt32();
            TriggerItems = new AlmTrigger[NumberOfTriggers];
            for (int i = 0; i < NumberOfTriggers; i++)
            {
                TriggerItems[i] = new AlmTrigger();
                TriggerItems[i].LoadFromStream(br);
            }
        }

        public class AlmLogicItem
        {
            public uint ExecOnceFlag;
            public uint TypeID;
            public uint TypeIndex;
            public string Name;
            public LogicVal[] Values;
            public struct LogicVal
            {
                public string Name;
                public uint Value;
                public InstanceType InstType;
            }

            public void LoadFromStream(BinaryReader br)
            {
                Name = Core.UnpackByteString(1251, br.ReadBytes(0x40));
                TypeID = br.ReadUInt32();
                TypeIndex = br.ReadUInt32();
                ExecOnceFlag = br.ReadUInt32();

                Values = new LogicVal[10];
                for (int i = 0; i < 10; i++)
                {
                    Values[i].Value = br.ReadUInt32();
                }
                for (int i = 0; i < 10; i++)
                {
                    Values[i].InstType = (InstanceType)br.ReadUInt32();
                }
                for (int i = 0; i < 10; i++)
                {
                    Values[i].Name = Core.UnpackByteString(1251, br.ReadBytes(0x40));
                }
            }
        }

        public class AlmCheckItem : AlmLogicItem{}

        public class AlmTrigger
        {
            public enum TriggerOperator
            {
                Equal = 0, NotEqual = 1,
                MoreThan = 2, LowerThan = 3,
                MoreOrEq = 4, LowerOrEq = 5
            }

            public void LoadFromStream(BinaryReader br)
            {
                Name = Core.UnpackByteString(1251, br.ReadBytes(0x80));

                for (int i = 0; i < 6; i++)
                    Checks[i] = br.ReadUInt32();

                for (int i = 0; i < 4; i++)
                    Instances[i] = br.ReadUInt32();

                for (int i = 0; i < 3; i++)
                    Operators[i] = (TriggerOperator)br.ReadUInt32();

                RunOnce = br.ReadUInt32();
            }

            public string Name;

            public uint RunOnce;
            public uint[] Checks = new uint[6];
            public uint[] Instances = new uint[4];
            public TriggerOperator[] Operators = new TriggerOperator[3]; // between every 2 Checks
        }

        public uint NumberOfTriggers;
        public uint NumberOfChecks;
        public uint NumberOfItems;
        

        public Vector2 GetDropLocation()
        {
            if (LogicItems != null && LogicItems.Length > 0)
            {
                var dropLoc = LogicItems.Where(x => x.Name.ToLower() == "drop location")
                                        .OrderBy(x => Guid.NewGuid())
                                        .FirstOrDefault();
                if (dropLoc != null)
                {
                    return new Vector2(dropLoc.Values.Where(x => x.InstType == InstanceType.X).Select(x => x.Value).FirstOrDefault(),
                                       dropLoc.Values.Where(x => x.InstType == InstanceType.Y).Select(x => x.Value).FirstOrDefault());
                }
            }
            return new Vector2(6,6);
        }
    }
