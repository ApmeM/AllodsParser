using System.Text;

namespace AllodsParser
{
    public class RegFileLoader : BaseFileLoader
    {
        public enum RegistryNodeType
        {
            String = 0,
            Directory = 1,
            Int = 2,
            Float = 4,
            Array = 6
        }


        public static string UnpackByteString(int encoding, byte[] bytes)
        {
            var codePages = CodePagesEncodingProvider.Instance.GetEncoding(encoding);
            string str_in = codePages.GetString(bytes);
            string str_out = str_in;
            int i = str_out.IndexOf('\0');
            if (i >= 0) str_out = str_out.Substring(0, i);
            return str_out;
        }

        private static (string, object)? ReadValue(MemoryStream ms, BinaryReader br, uint data_origin)
        {
            br.BaseStream.Position += 4; // uint e_unk1 = msb.ReadUInt32();
            uint e_offset = br.ReadUInt32(); // 0x1C
            uint e_count = br.ReadUInt32(); // 0x18
            uint e_type = br.ReadUInt32();// 0x14
            string e_name = UnpackByteString(866, br.ReadBytes(16));

            var name = e_name;

            switch ((RegistryNodeType)e_type)
            {
                case RegistryNodeType.String:
                    ms.Seek(data_origin + e_offset, SeekOrigin.Begin);
                    return (name, UnpackByteString(866, br.ReadBytes((int)e_count)));
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

        protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
        {
            if (br.ReadUInt32() != 0x31415926)
            {
                Console.Error.WriteLine($"Couldn't load {relativeFilePath}: (not a registry file)");
                return new EmptyFile();
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
                    return new EmptyFile();
                }
                root[newValue.Value.Item1] = newValue.Value.Item2;
            }

            return new RegFile
            {
                Root = root
            };
        }
    }
}