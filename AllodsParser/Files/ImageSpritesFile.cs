using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class ImageSpritesFile : BaseFile
{
    public List<Image<Rgba32>> Sprites;

    protected override void SaveInternal(string outputFileName)
    {
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