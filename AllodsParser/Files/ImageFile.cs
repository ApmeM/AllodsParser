using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

public class ImageFile : BaseFile
{
    public Image<Rgba32> Image;

    protected override void SaveInternal(string outputFileName)
    {
        Image.Save(outputFileName, new PngEncoder());
    }
}
