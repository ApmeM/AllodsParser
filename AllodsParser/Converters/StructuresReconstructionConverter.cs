using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AllodsParser
{
    public class StructuresReconstructionConverter : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<RegStructureFile>()
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            // oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(RegStructureFile toConvert, List<BaseFile> files)
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

                var path = f.File.Split("/");

                var sprites = files
                    .OfType<SpritesWithPalettesFile>()
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

                    var frameWidth = newWidth * f.TileWidth;
                    var frameHeight = newHeight * f.FullHeight;

                    var template = new Image<Rgba32>(frameWidth, frameHeight);
                    for (var j = 0; j < f.TileWidth * f.FullHeight; j++)
                    {
                        template.Mutate(a => a.DrawImage(sprite.Sprites[0], new Point(newWidth * (j % f.TileWidth), newHeight * (j / f.TileWidth)), 1));
                        sprite.Sprites.RemoveAt(0);
                    }

                    var newSprites = new List<Image<Rgba32>>
                    {
                        template.Clone()
                    };

                    for (var frame = 0; frame < f.AnimFrame.Length - 1; frame++)
                    {
                        var newSprite = template.Clone();
                        newSprites.Add(newSprite);

                        for (int pos = 0; pos < f.AnimMask.Length; pos++)
                        {
                            if (f.AnimMask[pos] == '-')
                            {
                                continue;
                            }

                            newSprite.Mutate(a => a.DrawImage(sprite.Sprites[0], new Point(newWidth * (pos % f.TileWidth), newHeight * (pos / f.TileWidth)), 1));
                            sprite.Sprites.RemoveAt(0);
                        }
                    }

                    if (sprite.Sprites.Count == f.TileWidth * f.FullHeight)
                    {
                        var newSprite = new Image<Rgba32>(frameWidth, frameHeight);
                        newSprites.Add(newSprite);
                        for (var j = 0; j < f.TileWidth * f.FullHeight; j++)
                        {
                            newSprite.Mutate(a => a.DrawImage(sprite.Sprites[0], new Point(newWidth * (j % f.TileWidth), newHeight * (j / f.TileWidth)), 1));
                            sprite.Sprites.RemoveAt(0);
                        }
                    }
                    else if(sprite.Sprites.Count != 0)
                    {
                        Console.WriteLine($"Unknown number of tiles left for {sprite.relativeFilePath}.");
                    }

                    sprite.Sprites.AddRange(newSprites);
                }
            }
            yield break;
        }
    }
}