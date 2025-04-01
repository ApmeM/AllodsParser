public class AlmShop
    {
        [Flags]
        public enum AlmShopItemMaterial
        {
            Iron        = 0x00000001,
            Bronze      = 0x00000002,
            Steel       = 0x00000004,
            Silver      = 0x00000008,
            Gold        = 0x00000010,
            Mithrill    = 0x00000020,
            Adamantium  = 0x00000040,
            Meteoric    = 0x00000080,
            Wood        = 0x00000100,
            MagicWood   = 0x00000200,
            Leather     = 0x00000400,
            HardLeather = 0x00000800,
            DragonLeather = 0x00001000,
            Crystal     = 0x00002000,
            None	    = 0x00004000
        }

        [Flags]
        public enum AlmShopItemType
        {
            Weapon = 0x00400000,
            Shield = 0x00800000,
            Armor = 0x01000000,
            ArmorMage = 0x02000000,
            Other = 0x04000000,
            Wands = 0x08000000,
        }

        [Flags]
        public enum AlmShopItemExtra
        {
            Common = 0x10000000,
            Magic = 0x20000000
        }

        [Flags]
        public enum AlmShopItemClass
        {
            Common      = 0x00008000,
            Uncommon    = 0x00010000,
            Rare        = 0x00020000,
            VeryRare    = 0x00040000,
            Elven       = 0x00080000,
            Bad         = 0x00100000,
            Good        = 0x00200000,
        }

        public class AlmShopShelf
        {
            public AlmShopItemMaterial ItemMaterials;
            public AlmShopItemType ItemTypes;
            public AlmShopItemExtra ItemExtras;
            public AlmShopItemClass ItemClasses;
            public uint PriceMin;
            public uint PriceMax;
            public uint MaxItems;
            public uint MaxSameItems;
        }

        private static uint GetFlagsMask(Type t)
        {
            uint mask = 0;
            IEnumerable<Enum> values = Enum.GetValues(t).Cast<Enum>();
            foreach (Enum v in values)
                mask |= (uint)((int)Enum.ToObject(t, v));
            return mask;
        }

        public uint ID;
        public AlmShopShelf[] Shelves = new AlmShopShelf[4];

        public void LoadFromStream(BinaryReader br)
        {
            ID = br.ReadUInt32();

            for (int i = 0; i < 4; i++)
                Shelves[i] = new AlmShopShelf();

            for (int i = 0; i < 4; i++)
            {
                uint flags = br.ReadUInt32();
                Shelves[i].ItemMaterials = (AlmShopItemMaterial)(flags & GetFlagsMask(typeof(AlmShopItemMaterial)));
                Shelves[i].ItemTypes = (AlmShopItemType)(flags & GetFlagsMask(typeof(AlmShopItemType)));
                Shelves[i].ItemExtras = (AlmShopItemExtra)(flags & GetFlagsMask(typeof(AlmShopItemExtra)));
                Shelves[i].ItemClasses = (AlmShopItemClass)(flags & GetFlagsMask(typeof(AlmShopItemClass)));
            }

            for (int i = 0; i < 4; i++)
                Shelves[i].PriceMin = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].PriceMax = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].MaxItems = br.ReadUInt32();
            for (int i = 0; i < 4; i++)
                Shelves[i].MaxSameItems = br.ReadUInt32();
        }
    }
