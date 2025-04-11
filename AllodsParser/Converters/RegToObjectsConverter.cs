using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class RegToObjectsConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<RegFile>()
                .Where(a => a.relativeFilePath == "graphics/objects/objects.reg")
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");
            
            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        public IEnumerable<BaseFile> ConvertFile(RegFile toConvert, List<BaseFile> files)
        {
            var fileList = ((Dictionary<string, object>)toConvert.Root["Files"])
                .ToDictionary(
                    a => int.Parse(a.Key.Remove(0, "File".Length)),
                    a => ((string)a.Value).Replace("\\", "/"));

            yield return new RegObjectsFile
            {
                Objects = toConvert.Root
                    .Where(a => a.Key != "Global")
                    .Where(a => a.Key != "Files")
                    .Select(a =>
                    {
                        var value = (Dictionary<string, object>)a.Value;

                        var file = fileList[GetInt(value, "File")];
                        var spriteFile = files
                            .OfType<SpritesWithPalettesFile>()
                            .Where(a => Path.Join(a.relativeFileDirectory, a.relativeFileName).EndsWith(file.Replace("fire", "dead")))
                            .FirstOrDefault();

                        if (spriteFile == null)
                        {
                            Console.WriteLine($"Sprite files not found for object {file}");
                            return null;
                        }

                        return new RegObjectsFile.ObjectsFileContent
                        {
                            Description = GetString(value, "DescText"),
                            Parent = GetInt(value, "Parent"),
                            Id = GetInt(value, "ID"),
                            File = file,
                            Index = GetInt(value, "Index"),
                            Phases = GetInt(value, "Phases"),
                            Width = spriteFile.Sprites[0].Width, //GetInt(value, "Width") == -1 ? spriteFile.Sprites[0].Width : GetInt(value, "Width"),
                            Height = spriteFile.Sprites[0].Height, //GetInt(value, "Height") == -1 ? spriteFile.Sprites[0].Height : GetInt(value, "Height"),
                            CenterX = spriteFile.Sprites[0].Width / 2, //GetInt(value, "CenterX"),
                            CenterY = spriteFile.Sprites[0].Height / 2, //GetInt(value, "CenterY"),
                            AnimationTime = GetIntArray(value, "AnimationTime"),
                            AnimationFrame = GetIntArray(value, "AnimationFrame"),
                            DeadObject = GetInt(value, "DeadObject"),
                            InMapEditor = GetInt(value, "InMapEditor") == 1,
                            IconId = GetInt(value, "IconId"),
                        };
                    })
                    .Where(a => a != null)
                    .ToList()!,
                relativeFileExtension = ".json",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }

        public string GetString(Dictionary<string, object> value, string key)
        {
            if (!value.ContainsKey(key))
            {
                return string.Empty;
            }
            return (string)value[key];
        }
        public int GetInt(Dictionary<string, object> value, string key)
        {
            if (!value.ContainsKey(key))
            {
                return -1;
            }
            return (int)value[key];
        }
        public int[] GetIntArray(Dictionary<string, object> value, string key)
        {
            if (!value.ContainsKey(key))
            {
                return new int[0];
            }

            if (value[key].GetType() != typeof(int[]))
            {
                return new int[0];
            }
            return (int[])value[key];
        }
    }
}