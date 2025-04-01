using System.Text;

namespace AllodsParser
{
    internal static class Program
    {
        public static Dictionary<string, Func<IFile>> FileFactory = new Dictionary<string, Func<IFile>>{
            {".bmp", ()=>new BinaryFile()},
            {".png", ()=>new BinaryFile()},
            {".wav", ()=>new BinaryFile()},
            {".txt", ()=>new BinaryFile()},
            {".reg", ()=>new RegFile()},
            {".16",  ()=>new Image16File()},
            {".16a", ()=>new Image16aFile()},
            {".256", ()=>new Image256File()},
            {".alm", ()=>new AlmFile()},
        };

        public static void Main(string[] args)
        {
            var resources = "/home/vas/Downloads/Allods2_en/data/";
            var result = "../Result";

            if (args.Length > 0)
            {
                resources = args[0];
            }
            if (args.Length > 1)
            {
                result = args[1];
            }

            if (Directory.Exists(result))
            {
                Directory.Delete(result, true);
            }
            Directory.CreateDirectory(result);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var knownUnknowns = new HashSet<string>();

            foreach (var filePath in Directory.GetFiles(resources, "*.*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(filePath);
                if (!FileFactory.ContainsKey(ext))
                {
                    if (!knownUnknowns.Contains(ext))
                    {
                        Console.WriteLine($"Unknown file extension: {ext}");
                        knownUnknowns.Add(ext);
                    }
                    continue;
                }

                var relativePath = Path.GetRelativePath(resources, filePath);

                var file = FileFactory[ext]();
                file.Init(resources, relativePath);
                file.Save(result);
            }

            // foreach (var file in Directory.GetFiles(resources, "*.alm", SearchOption.AllDirectories)) new AlmFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
        }
    }
}
