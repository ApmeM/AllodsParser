﻿using System.Text;

namespace AllodsParser
{
    internal static class Program
    {
        public enum SpriteMergerFlags
        {
            SingleSprite,
            PerPaletteSprite,
            EachSprite
        }

        public static SpriteMergerFlags SpriteMergeVariant = SpriteMergerFlags.SingleSprite;

        public static Dictionary<string, Func<BaseFileLoader>> FileFactory = new Dictionary<string, Func<BaseFileLoader>>{
            {".ttf", ()=>new BinaryFileLoader()},
            {".bmp", ()=>new BinaryFileLoader()},
            {".png", ()=>new BinaryFileLoader()},
            {".wav", ()=>new BinaryFileLoader()},
            {".txt", ()=>new BinaryFileLoader()},
            {".reg", ()=>new RegFileLoader()},
            {".16",  ()=>new Image16FileLoader()},
            {".16a", ()=>new Image16aFileLoader()},
            {".256", ()=>new Image256FileLoader()},
            {".alm", ()=>new AlmFileLoader()},
            {".pal", ()=>new PalFileLoader()},
        };

        public static List<BaseFileConverter> FileConverters = new List<BaseFileConverter>{
            new RegToStructuresConverter(),
            new RegToUnitsConverter(),
            new RegToObjectsConverter(),
            new PalMergerConverter(),
            new UnitsPalMergerConverter(),
            new StructuresReconstructionConverter(),
            new SpriteToSeparateImageConverter(),
            new SpriteToPerPaletteImageConverter(),
            new SpriteToSingleImageConverter(),
            new AlmToTmxConverter(),
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

            var files = new List<BaseFile>();

            Console.WriteLine("Loading...");

            foreach (var filePath in Directory.GetFiles(resources, "*.*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(filePath);
                if (!FileFactory.ContainsKey(ext))
                {
                    if (!knownUnknowns.Contains(ext))
                    {
                        Console.Error.WriteLine($"Unknown file extension: {ext}");
                        knownUnknowns.Add(ext);
                    }
                    continue;
                }

                var relativePath = Path.GetRelativePath(resources, filePath);

                var bytes = File.ReadAllBytes(filePath);

                files.Add(FileFactory[ext]().Load(relativePath, bytes));
            }

            Console.WriteLine("Converting...");
            FileConverters.ForEach(a => a.Convert(files));

            Console.WriteLine("Saving.");
            foreach (var file in files)
            {
                file.Save(result);
            }
        }
    }
}
