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

/*
    protected override Image<Rgba32>? ConvertInternal(List<Image<Rgba32>> toConvert)
    {
        var newWidth = toConvert.Max(a => a.Width);
        var newHeight = toConvert.Max(a => a.Height);

        var result = new Image<Rgba32>(newWidth * toConvert.Count, newHeight);

        for (int i = 0; i < toConvert.Count; i++)
        {
            result.Mutate(a => a.DrawImage(toConvert[i], new Point(newWidth * i, 0), 1));
        }

        return result;
    }
*/