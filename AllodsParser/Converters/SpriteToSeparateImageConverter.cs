using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AllodsParser
{
    public class SpriteToSeparateImageConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            for (int i = 0; i < toConvert.Sprites.Count; i++)
            {
                if (toConvert.Palettes.Count == 0)
                {
                    yield return new ImageFile
                    {
                        Image = toConvert.Sprites[i],
                        relativeFileExtension = ".png",
                        relativeFileDirectory = toConvert.relativeFileDirectory,
                        relativeFileName = toConvert.relativeFileName + "." + i
                    };

                }
                for (var j = 0; j < toConvert.Palettes.Count; j++)
                {
                    var f = toConvert.Sprites[i];
                    var p = toConvert.Palettes[j];

                    var newImage = new Image<Rgba32>(f.Width, f.Height);
                    for (var x = 0; x < f.Width; x++)
                        for (var y = 0; y < f.Height; y++)
                        {
                            var color = p[f[x, y].R, 0];
                            color.A = f[x, y].A;
                            newImage[x, y] = color;
                        }

                    yield return new ImageFile
                    {
                        Image = newImage,
                        relativeFileExtension = ".png",
                        relativeFileDirectory = toConvert.relativeFileDirectory,
                        relativeFileName = toConvert.relativeFileName + "." + i + "." + j
                    };
                }
            }
        }
    }
}