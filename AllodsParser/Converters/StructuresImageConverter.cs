using System.Runtime.Versioning;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class StructuresImageConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<StructureRegFile>()
                .ToList();

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            // oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(StructureRegFile toConvert, List<BaseFile> files)
        {
            var filesCreated = new HashSet<string>();
            for (int i = 0; i < toConvert.Structures.Count; i++)
            {
                var f = toConvert.Structures[i];

                if (filesCreated.Contains(f.File.ToLower()))
                {
                    continue;
                }

                filesCreated.Add(f.File.ToLower());

                var path = f.File.Split("\\");

                var sprites = files
                    .OfType<ImageSpritesFile>()
                    .Where(a => a.relativeFileDirectory.EndsWith("/" + path[0], StringComparison.InvariantCultureIgnoreCase))
                    .Where(a =>
                        a.relativeFileName.Equals(path[1], StringComparison.InvariantCultureIgnoreCase) ||
                        a.relativeFileName.Equals(path[1] + "b", StringComparison.InvariantCultureIgnoreCase)
                        )
                    .ToList();

                if (sprites.Count != 2)
                {
                    Console.Error.WriteLine($"Cant find both sprites for structure {f.File}");
                    continue;
                }

                foreach (var sprite in sprites)
                {

                    var newWidth = sprite.Sprites.Max(a => a.Width);
                    var newHeight = sprite.Sprites.Max(a => a.Height);

                    var framesCount = Math.Max(1, f.AnimFrame.Length);
                    var frameWidth = newWidth * f.TileWidth;
                    var frameHeight = newHeight * f.TileHeight;

                    var result = new Image<Rgba32>(frameWidth * framesCount, frameHeight);

                    for (var frameNumber = 0; frameNumber < framesCount; frameNumber++)
                    {
                        for (int j = 0; j < f.TileWidth * f.TileHeight; j++)
                        {
                            result.Mutate(a => a.DrawImage(sprite.Sprites[j], new Point(newWidth * (j % f.TileWidth) + frameWidth * frameNumber, newHeight * (j / f.TileWidth)), 1));
                        }
                    }

                    files.Remove(sprite);

                    yield return new ImageFile
                    {
                        Image = result,
                        relativeFileExtension = ".png",
                        relativeFileDirectory = sprite.relativeFileDirectory,
                        relativeFileName = sprite.relativeFileName
                    };
                }
            }
        }
    }
}