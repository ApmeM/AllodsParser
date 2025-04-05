using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class ObjectsRegConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<RegFile>()
                .Where(a => a.relativeFilePath == "graphics/objects/objects.reg")
                .ToList();

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
                    a =>
                    {
                        var path = ((string)a.Value).Split("\\");
                        if (path.Length == 3)
                        {
                            return path[0] + "_" + path[1];
                        }
                        else
                        {
                            return path[0];
                        }

                    });

            yield return new ObjectsRegFile
            {
                Objects = toConvert.Root
                    .Where(a => a.Key != "Global")
                    .Where(a => a.Key != "Files")
                    .Select(a =>
                    {
                        var value = (Dictionary<string, object>)a.Value;
                        return new ObjectsRegFile.ObjectsFileContent
                        {
                            Description = GetString(value, "DescText"),
                            Parent = GetInt(value, "Parent"),
                            Id = GetInt(value, "ID"),
                            File = fileList[GetInt(value, "File")],
                            Index = GetInt(value, "Index"),
                            Phases = GetInt(value, "Phases"),
                            Width = GetInt(value, "Width"),
                            Height = GetInt(value, "Height"),
                            CenterX = GetInt(value, "CenterX"),
                            CenterY = GetInt(value, "CenterY"),
                            AnimationTime = GetIntArray(value, "AnimationTime"),
                            AnimationFrame = GetIntArray(value, "AnimationFrame"),
                            DeadObject = GetInt(value, "DeadObject"),
                            InMapEditor = GetInt(value, "InMapEditor") == 1,
                            IconId = GetInt(value, "IconId"),
                        };
                    })
                    .ToList(),
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