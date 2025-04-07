using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class PalFileLoader : BaseFileLoader
{
    private static Image<Rgba32> LoadPaletteFromStream(BinaryReader br)
    {
        var texture = new Image<Rgba32>(256, 1);

        for (int i = 0; i < 256; i++)
        {
            byte b = br.ReadByte();
            byte g = br.ReadByte();
            byte r = br.ReadByte();
            br.BaseStream.Position += 1; // byte a = br.ReadByte();
            texture[i, 0] = new Rgba32(r, g, b, 255);
        }
        return texture;
    }

    protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
    {
        var palettes = new List<Image<Rgba32>>();

        if (
            (relativeFilePath.ToLower().Contains("heroes") ||
            relativeFilePath.ToLower().Contains("heroes_l") ||
            relativeFilePath.ToLower().Contains("humans")) &&
            relativeFilePath.ToLower().EndsWith("human.pal")
            )
        {
            for (int j = 0; j < 16; j++)
            {
                uint offset = (uint)(j * 256 * 4);
                ms.Seek(offset, SeekOrigin.Begin);
                palettes.Add(LoadPaletteFromStream(br));
            }
        }
        else
        {
            ms.Seek(0x36, SeekOrigin.Begin);
            palettes.Add(LoadPaletteFromStream(br));
        }

        return new PalFile
        {
            Palettes = palettes
        };
    }
}
