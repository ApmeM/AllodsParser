using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class Image16aFileLoader : BaseFileLoader
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

    private static void SpriteAddIXIY(ref int ix, ref int iy, uint w, uint add)
    {
        int x = ix;
        int y = iy;
        for (int i = 0; i < add; i++)
        {
            x++;
            if (x >= w)
            {
                y++;
                x = x - (int)w;
            }
        }

        ix = x;
        iy = y;
    }

    protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
    {
        var frames = new List<Image<Rgba32>>();
        ms.Position = ms.Length - 4;
        int count = br.ReadInt32() & 0x7FFFFFFF;

        ms.Position = 0;

        // read palette
        var palette = LoadPaletteFromStream(br);

        for (int i = 0; i < count; i++)
        {
            uint w = br.ReadUInt32();
            uint h = br.ReadUInt32();
            uint ds = br.ReadUInt32();
            long cpos = ms.Position;

            if (w > 512 || h > 512 || ds > 1000000)
            {
                Console.WriteLine($"Invalid sprite {relativeFilePath}: Empty frame #{i}");
                i--;
                count--;
                continue;
            }

            Image<Rgba32> texture = new Image<Rgba32>((int)w, (int)h);

            int ix = 0;
            int iy = 0;
            int ids = (int)ds;
            while (ids > 0)
            {
                ushort ipx = br.ReadUInt16();
                ipx &= 0xC0FF;
                ids -= 2;

                if ((ipx & 0xC000) > 0)
                {
                    if ((ipx & 0xC000) == 0x4000)
                    {
                        ipx &= 0xFF;
                        SpriteAddIXIY(ref ix, ref iy, w, ipx * w);
                    }
                    else
                    {
                        ipx &= 0xFF;
                        SpriteAddIXIY(ref ix, ref iy, w, ipx);
                    }
                }
                else
                {
                    ipx &= 0xFF;
                    for (int j = 0; j < ipx; j++)
                    {
                        uint ss = br.ReadUInt16();
                        uint alpha = (((ss & 0xFF00) >> 9) & 0x0F) + (((ss & 0xFF00) >> 5) & 0xF0);
                        uint idx = ((ss & 0xFF00) >> 1) + ((ss & 0x00FF) >> 1);
                        idx &= 0xFF;
                        alpha &= 0xFF;
                        var color = palette[(int)idx, 0];
                        color.A = (byte)alpha;
                        texture[ix, iy] = color;
                        SpriteAddIXIY(ref ix, ref iy, w, 1);
                    }

                    ids -= ipx * 2;
                }
            }

            frames.Add(texture);
            ms.Position = cpos + ds;
        }

        return new ImageSpritesFile
        {
            Sprites = frames
        };
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