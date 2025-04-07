using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class PalFile : BaseFile
{
    public List<Image<Rgba32>> Palettes;
}
