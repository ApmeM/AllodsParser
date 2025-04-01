public class AlmData
    {
        public uint Width;
        public uint Height;
        public float SolarAngle;
        public uint TimeOfDay;
        public uint Darkness;
        public uint Contrast;
        public uint UseTiles;

        public uint CountPlayers;
        public uint CountStructures;
        public uint CountUnits;
        public uint CountTriggers;
        public uint CountSacks;
        public uint CountGroups;
        public uint CountInns;
        public uint CountShops;
        public uint CountPointers;
        public uint CountMusic;
        public string Name;
        public uint RecPlayers;
        public uint Level;
        public uint Junk1;
        public uint Junk2;
        public string Author;

        public void LoadFromStream(BinaryReader br)
        {
            Width = br.ReadUInt32();
            Height = br.ReadUInt32();
            SolarAngle = br.ReadSingle();
            TimeOfDay = br.ReadUInt32();
            Darkness = br.ReadUInt32();
            Contrast = br.ReadUInt32();
            UseTiles = br.ReadUInt32();
            CountPlayers = br.ReadUInt32();
            CountStructures = br.ReadUInt32();
            CountUnits = br.ReadUInt32();
            CountTriggers = br.ReadUInt32();
            CountSacks = br.ReadUInt32();
            CountGroups = br.ReadUInt32();
            CountInns = br.ReadUInt32();
            CountShops = br.ReadUInt32();
            CountPointers = br.ReadUInt32();
            CountMusic = br.ReadUInt32();
            Name = Core.UnpackByteString(1251, br.ReadBytes(0x40));
            RecPlayers = br.ReadUInt32();
            Level = br.ReadUInt32();
            Junk1 = br.ReadUInt32();
            Junk2 = br.ReadUInt32();
            //Console.WriteLine("data junk1 = {0}, junk2 = {1}, light = {2}", Junk1, Junk2, Darkness);
            Author = Core.UnpackByteString(1251, br.ReadBytes(0x200));
        }
    }
