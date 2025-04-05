using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class UnitsImageConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<ImageSpritesFile>()
                .Where(a => a.relativeFileDirectory.StartsWith("graphics/units/"))
                .ToList();

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            var regFile = files
                .OfType<UnitRegFile>()
                .Single();
            foreach (var unit in regFile.Units)
            {
                var path = unit.File.Split("\\");
                unit.File = path[0] + "\\" + path[1];
            }

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(ImageSpritesFile toConvert, List<BaseFile> files)
        {
            var regFile = files
                .OfType<UnitRegFile>()
                .Single();

            var unitDescriptions = regFile.Units.Where(a =>
            {
                var splitFile = a.File.Split("\\");

                return toConvert.relativeFileName == splitFile[2] &&
                    toConvert.relativeFileDirectory.EndsWith(Path.Combine(splitFile[0], splitFile[1]));
            }).ToList();

            if (unitDescriptions.Count == 0)
            {
                var unitDescription = new UnitRegFile.UnitFileContent
                {
                    Id = -1,
                    File = toConvert.relativeFileDirectory.Remove(0, "graphics/units/".Length).Replace("/", "\\") + "\\" + toConvert.relativeFileName,
                };
                regFile.Units.Add(unitDescription);
                unitDescriptions.Add(unitDescription);
            }

            var Width = toConvert.Sprites.Max(a => a.Width);
            var CenterX = Width / 2;
            var Height = toConvert.Sprites.Max(a => a.Height);
            var CenterY = Height / 2;

            var framesCount = toConvert.Sprites.Count;
            var result = new Image<Rgba32>(Width * framesCount, Height);

            for (int i = 0; i < framesCount; i++)
            {
                var f = toConvert.Sprites[i];

                result.Mutate(a => a.DrawImage(toConvert.Sprites[i], new Point(Width * i, 0), 1));
            }

            var path = unitDescriptions[0].File.Split("\\");

            foreach (var unit in unitDescriptions)
            {
                unit.Width = Width;
                unit.CenterX = CenterX;
                unit.Height = Height;
                unit.CenterY = CenterY;
            }

            yield return new ImageFile
            {
                Image = result,
                relativeFileExtension = ".png",
                relativeFileDirectory = Path.Combine(regFile.relativeFileDirectory, path[0].ToLower()),
                relativeFileName = path[1].ToLower() + (toConvert.relativeFileName.EndsWith("b") ? "b" : "")
            };
        }
    }
}