using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class SpriteToPerPaletteImageConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            if (Program.SpriteMergeVariant != Program.SpriteMergerFlags.PerPaletteSprite)
            {
                return;
            }

            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<ImageFile> ConvertFile(SpritesWithPalettesFile toConvert, List<BaseFile> files)
        {
            if (toConvert.Sprites.Count == 0)
            {
                Console.Error.WriteLine($"Sprite {toConvert.relativeFilePath} does not have sprites converted.");
                yield break;
            }

            var newWidth = toConvert.Sprites.Max(a => a.Width);
            var newHeight = toConvert.Sprites.Max(a => a.Height);

            var sprite = new Image<Rgba32>(toConvert.Sprites.Count * newWidth, newHeight);

            for (int i = 0; i < toConvert.Sprites.Count; i++)
            {
                sprite.Mutate(a => a.DrawImage(toConvert.Sprites[i], new Point(newWidth * i, 0), 1));
            }

            if (toConvert.Palettes.Count == 0)
            {
                yield return new ImageFile
                {
                    Image = sprite,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = toConvert.relativeFileDirectory,
                    relativeFileName = toConvert.relativeFileName + ".0"
                };
                yield break;
            }

            for (var j = 0; j < toConvert.Palettes.Count; j++)
            {
                var newImage = new Image<Rgba32>(sprite.Width, sprite.Height);
                for (var x = 0; x < sprite.Width; x++)
                    for (var y = 0; y < sprite.Height; y++)
                    {
                        var color = toConvert.Palettes[j][sprite[x, y].R, 0];
                        color.A = sprite[x, y].A;
                        newImage[x, y] = color;
                    }

                yield return new ImageFile
                {
                    Image = newImage,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = toConvert.relativeFileDirectory,
                    relativeFileName = toConvert.relativeFileName + "." + j
                };
            }
        }
    }
}