using SixLabors.ImageSharp;

namespace AllodsParser
{
    public class StructuresRegConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFile = files
                .OfType<RegFile>()
                .First(a => a.relativeFilePath == "graphics/structures/structures.reg");

            var newFile = new StructureRegFile
            {
                Structures = oldFile.Root
                    .Where(a => a.Key != "Global")
                    .Select(a =>
                    {
                        var value = (Dictionary<string, object>)a.Value;
                        return new StructureRegFile.StructuresFileContent
                        {
                            Description = (string)value["DescText"],
                            Id = (int)value["ID"],
                            File = (string)value["File"],
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
                relativeFileDirectory = oldFile.relativeFileDirectory,
                relativeFileName = oldFile.relativeFileName
            };

            files.Remove(oldFile);
            files.Add(newFile);
            return files;
        }
    }
}