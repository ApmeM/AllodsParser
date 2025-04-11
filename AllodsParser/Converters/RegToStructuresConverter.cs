using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class RegToStructuresConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<RegFile>()
                .Where(a => a.relativeFilePath == "graphics/structures/structures.reg")
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        public IEnumerable<BaseFile> ConvertFile(RegFile toConvert, List<BaseFile> files)
        {
            yield return new RegStructureFile
            {
                Structures = toConvert.Root
                    .Where(a => a.Key != "Global")
                    .Select(a =>
                    {
                        var value = (Dictionary<string, object>)a.Value;
                        return new RegStructureFile.StructuresFileContent
                        {
                            Description = (string)value["DescText"],
                            Id = (int)value["ID"],
                            File = ((string)value["File"]).Replace("\\", "/"),
                            TileWidth = (int)value["TileWidth"],
                            TileHeight = (int)value["TileHeight"],
                            FullHeight = (int)value["FullHeight"],
                            SelectionX1 = (int)value["SelectionX1"],
                            SelectionX2 = (int)value["SelectionX2"],
                            SelectionY1 = (int)value["SelectionY1"],
                            SelectionY2 = (int)value["SelectionY2"],
                            ShadowY = (int)value["ShadowY"],
                            Phases = (int)value["Phases"],
                            AnimMask = (string)value["AnimMask"],
                            AnimFrame = value["AnimFrame"]?.GetType() != typeof(int[]) ? new int[0] : (int[])value["AnimFrame"],
                            AnimTime = value["AnimTime"]?.GetType() != typeof(int[]) ? new int[0] : (int[])value["AnimTime"],
                            Picture = (string)value["Picture"],
                            IconID = !value.ContainsKey("IconID") ? -1 : (int)value["IconID"]
                        };
                    })
                    .ToList(),
                relativeFileExtension = ".json",
                relativeFileDirectory = toConvert.relativeFileDirectory,
                relativeFileName = toConvert.relativeFileName
            };
        }
    }
}