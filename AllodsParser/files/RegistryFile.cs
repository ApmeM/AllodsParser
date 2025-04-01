using System.Text.Json;

public class RegistryFile
{
    public enum RegistryNodeType
    {
        String = 0,
        Directory = 1,
        Int = 2,
        Float = 4,
        Array = 6
    }

    public class RegistryNode
    {
        public string Name { get; set; } = "";

        public RegistryNodeType Type { get; set; }

        public object Value { get; set; } = null;
    }

    private readonly string filename;
    internal Dictionary<string, object> Root;

    private static RegistryNode ReadValue(MemoryStream ms, BinaryReader br, uint data_origin)
    {
        br.BaseStream.Position += 4; // uint e_unk1 = msb.ReadUInt32();
        uint e_offset = br.ReadUInt32(); // 0x1C
        uint e_count = br.ReadUInt32(); // 0x18
        uint e_type = br.ReadUInt32();// 0x14
        string e_name = Core.UnpackByteString(866, br.ReadBytes(16));

        RegistryNode subnode = new RegistryNode();

        subnode.Name = e_name;
        subnode.Type = (RegistryNodeType)e_type;

        switch (subnode.Type)
        {
            case RegistryNodeType.String:
                ms.Seek(data_origin + e_offset, SeekOrigin.Begin);
                subnode.Value = Core.UnpackByteString(866, br.ReadBytes((int)e_count));
                break;
            case RegistryNodeType.Directory:
                var first = e_offset;
                var last = e_offset + e_count;
                var children = new Dictionary<string, object>();
                for (uint i = first; i < last; i++)
                {
                    ms.Seek(0x18 + 0x20 * i, SeekOrigin.Begin);
                    var newValue = ReadValue(ms, br, data_origin);
                    children[newValue.Name] = newValue.Value;
                }
                subnode.Value = children;
                break;
            case RegistryNodeType.Int:
                subnode.Value = (int)e_offset;
                break;
            case RegistryNodeType.Float:
                // well, we gotta rewind and read it again
                // C-style union trickery won't work
                ms.Seek(-0x1C, SeekOrigin.Current);
                subnode.Value = br.ReadDouble();
                break;
            case RegistryNodeType.Array:
                if ((e_count % 4) != 0)
                {
                    return null;
                }
                uint e_acount = e_count / 4;
                var value = new int[e_acount];
                ms.Seek(data_origin + e_offset, SeekOrigin.Begin);
                for (uint j = 0; j < e_acount; j++)
                {
                    value[j] = br.ReadInt32();
                }
                subnode.Value = value;

                break;
        }
        return subnode;
    }

    public RegistryFile(string filename, byte[] data)
    {
        this.filename = filename;

        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader br = new BinaryReader(ms);
        if (br.ReadUInt32() != 0x31415926)
        {
            Console.Error.WriteLine("Couldn't load \"{0}\" (not a registry file)", filename);
            return;
        }

        uint root_offset = br.ReadUInt32();
        uint root_size = br.ReadUInt32();
        br.BaseStream.Position += 4; // uint reg_flags = br.ReadUInt32();
        uint reg_eatsize = br.ReadUInt32();
        br.BaseStream.Position += 4; // uint reg_junk = msb.ReadUInt32();

        var first = root_offset;
        var last = root_offset + root_size;
        this.Root = new Dictionary<string, object>();
        for (uint i = first; i < last; i++)
        {
            ms.Seek(0x18 + 0x20 * i, SeekOrigin.Begin);
            var newValue = ReadValue(ms, br, 0x1C + 0x20 * reg_eatsize);
            this.Root[newValue.Name] = newValue.Value;
        }
    }

    public void Save(string path)
    {
        Directory.CreateDirectory(path);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(this.Root, options);
        File.WriteAllText(Path.Join(path, filename), json);
    }
}
