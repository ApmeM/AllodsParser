using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class ObjectsImageConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<ImageSpritesFile>()
                .Where(a => a.relativeFileDirectory.StartsWith("graphics/objects/"))
                .ToList();

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(ImageSpritesFile toConvert, List<BaseFile> files)
        {
            var regFile = files
                .OfType<ObjectsRegFile>()
                .Single();

            var newFileName = Path.Combine(toConvert.relativeFileDirectory).Remove(0, regFile.relativeFileDirectory.Length + 1).Replace("/", "_");

            var objDescriptions = regFile.Objects.Where(a => a.File == newFileName || a.File + "b" == newFileName).ToList();

            if (objDescriptions.Count == 0)
            {
                var objDescription = new ObjectsRegFile.ObjectsFileContent
                {
                    Id = -1,
                    File = newFileName,
                };
                regFile.Objects.Add(objDescription);
                objDescriptions.Add(objDescription);
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

            var path = objDescriptions[0].File.Split("\\");

            foreach (var obj in objDescriptions)
            {
                obj.Width = Width;
                obj.CenterX = CenterX;
                obj.Height = Height;
                obj.CenterY = CenterY;
            }

            yield return new ImageFile
            {
                Image = result,
                relativeFileExtension = ".png",
                relativeFileDirectory = regFile.relativeFileDirectory,
                relativeFileName = newFileName + (toConvert.relativeFileName.EndsWith("b") ? "b" : "")
            };
        }
    }
}