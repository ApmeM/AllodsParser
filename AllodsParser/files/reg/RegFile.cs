using System.Text.Json;

namespace AllodsParser
{
    public class RegFile : BaseFile<Dictionary<string, object>, Dictionary<string, object>>
    {
        private static (string, object)? ReadValue(MemoryStream ms, BinaryReader br, uint data_origin)
        {
            br.BaseStream.Position += 4; // uint e_unk1 = msb.ReadUInt32();
            uint e_offset = br.ReadUInt32(); // 0x1C
            uint e_count = br.ReadUInt32(); // 0x18
            uint e_type = br.ReadUInt32();// 0x14
            string e_name = Core.UnpackByteString(866, br.ReadBytes(16));

            var name = e_name;

            switch ((RegistryNodeType)e_type)
            {
                case RegistryNodeType.String:
                    ms.Seek(data_origin + e_offset, SeekOrigin.Begin);
                    return (name, Core.UnpackByteString(866, br.ReadBytes((int)e_count)));
                case RegistryNodeType.Directory:
                    var first = e_offset;
                    var last = e_offset + e_count;
                    var children = new Dictionary<string, object>();
                    for (uint i = first; i < last; i++)
                    {
                        ms.Seek(0x18 + 0x20 * i, SeekOrigin.Begin);
                        var newValue = ReadValue(ms, br, data_origin);
                        if (newValue == null)
                        {
                            return null;
                        }

                        children[newValue.Value.Item1] = newValue.Value.Item2;
                    }
                    return (name, children);
                case RegistryNodeType.Int:
                    return (name, (int)e_offset);
                case RegistryNodeType.Float:
                    // well, we gotta rewind and read it again
                    // C-style union trickery won't work
                    ms.Seek(-0x1C, SeekOrigin.Current);
                    return (name, br.ReadDouble());
                case RegistryNodeType.Array:
                    if (e_count % 4 != 0)
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
                    return (name, value);
                default:
                    return null;
            }
        }

        protected override Dictionary<string, object>? LoadInternal(MemoryStream ms, BinaryReader br)
        {
            if (br.ReadUInt32() != 0x31415926)
            {
                Console.Error.WriteLine($"Couldn't load {relativeFilePath}: (not a registry file)");
                return default;
            }

            uint root_offset = br.ReadUInt32();
            uint root_size = br.ReadUInt32();
            br.BaseStream.Position += 4; // uint reg_flags = br.ReadUInt32();
            uint reg_eatsize = br.ReadUInt32();
            br.BaseStream.Position += 4; // uint reg_junk = msb.ReadUInt32();

            var first = root_offset;
            var last = root_offset + root_size;
            var root = new Dictionary<string, object>();
            for (uint i = first; i < last; i++)
            {
                ms.Seek(0x18 + 0x20 * i, SeekOrigin.Begin);
                var newValue = ReadValue(ms, br, 0x1C + 0x20 * reg_eatsize);
                if (newValue == null)
                {
                    Console.Error.WriteLine($"Invalid content of registry file {relativeFilePath}");
                    return default;
                }
                root[newValue.Value.Item1] = newValue.Value.Item2;
            }
            return root;
        }

        protected override Dictionary<string, object> ConvertInternal(Dictionary<string, object> toConvert)
        {
            return toConvert;
        }

        protected override void SaveInternal(string outputFileName, Dictionary<string, object> toSave)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(toSave, options);
            File.WriteAllText(outputFileName, json);
        }
    }
}