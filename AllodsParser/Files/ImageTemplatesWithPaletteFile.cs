using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class SpritesWithPalettesFile : BaseFile
{
    public List<Image<Rgba32>> Palettes;
    public List<Image<Rgba32>> Sprites;
}