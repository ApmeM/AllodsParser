using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class RegToUnitsConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<RegFile>()
                .Where(a => a.relativeFilePath == "graphics/units/units.reg")
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

            yield return new RegUnitsFile
            {
                Units = toConvert.Root
                    .Where(a => a.Key != "Global")
                    .Where(a => a.Key != "Files")
                    .Select(a =>
                    {
                        var value = (Dictionary<string, object>)a.Value;

                        var file = fileList[GetInt(value, "File")];
                        var sprite = files
                            .OfType<SpritesWithPalettesFile>()
                            .Where(a => Path.Join(a.relativeFileDirectory, a.relativeFileName).EndsWith(file))
                            .First()
                            .Sprites[0];

                        return new RegUnitsFile.UnitFileContent
                        {
                            Description = GetString(value, "DescText"),
                            Parent = GetInt(value, "Parent"),
                            Id = GetInt(value, "ID"),
                            File = file,
                            Index = GetInt(value, "Index"),
                            MovePhases = GetInt(value, "MovePhases"),
                            MoveBeginPhases = GetInt(value, "MoveBeginPhases"),
                            AttackPhases = GetInt(value, "AttackPhases"),
                            DyingPhases = GetInt(value, "DyingPhases"),
                            Width = sprite.Width, //GetInt(value, "Width") == -1 ? sprite.Width : GetInt(value, "Width"),
                            Height = sprite.Height, //GetInt(value, "Height") == -1 ? sprite.Height : GetInt(value, "Height"),
                            CenterX = GetInt(value, "CenterX"),
                            CenterY = GetInt(value, "CenterY"),
                            SelectionX1 = GetInt(value, "SelectionX1"),
                            SelectionX2 = GetInt(value, "SelectionX2"),
                            SelectionY1 = GetInt(value, "SelectionY1"),
                            SelectionY2 = GetInt(value, "SelectionY2"),
                            AttackAnimTime = GetIntArray(value, "AttackAnimTime"),
                            AttackAnimFrame = GetIntArray(value, "AttackAnimFrame"),
                            MoveAnimTime = GetIntArray(value, "MoveAnimTime"),
                            MoveAnimFrame = GetIntArray(value, "MoveAnimFrame"),
                            Dying = GetInt(value, "Dying"),
                            BonePhases = GetInt(value, "BonePhases"),
                            Palette = GetInt(value, "Palette"),
                            Sound = GetIntArray(value, "Sound"),
                            Projectile = GetInt(value, "Projectile"),
                            AttackDelay = GetInt(value, "AttackDelay"),
                            ShootDelay = GetInt(value, "ShootDelay"),
                            ShootOffset = GetIntArray(value, "ShootOffset"),
                            InfoPicture = GetString(value, "InfoPicture"),
                            InMapEditor = GetInt(value, "InMapEditor") == 1,
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