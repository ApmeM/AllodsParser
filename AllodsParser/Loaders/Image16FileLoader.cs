using AllodsParser;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class Image16FileLoader : BaseFileLoader
{
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
        for (int i = 0; i < count; i++)
        {
            uint w = br.ReadUInt32();
            uint h = br.ReadUInt32();
            uint ds = br.ReadUInt32();
            long cpos = ms.Position;

            if (w > 512 || h > 512 || ds > 1000000)
            {
                Console.Error.WriteLine($"Invalid sprite {relativeFilePath}: Empty frame #{i}");
                i--;
                count--;
                continue;
            }

            var texture = new Image<Rgba32>((int)w, (int)h);
            frames.Add(texture);

            int ix = 0;
            int iy = 0;
            int ids = (int)ds;
            while (ids > 0)
            {
                ushort ipx = br.ReadByte();
                ipx |= (ushort)(ipx << 8);
                ipx &= 0xC03F;
                ids -= 1;

                if ((ipx & 0xC000) > 0)
                {
                    if ((ipx & 0xC000) == 0x4000)
                    {
                        ipx &= 0x3F;
                        SpriteAddIXIY(ref ix, ref iy, w, ipx * w);
                    }
                    else
                    {
                        ipx &= 0x3F;
                        SpriteAddIXIY(ref ix, ref iy, w, ipx);
                    }
                }
                else
                {
                    ipx &= 0x3F;

                    byte[] bytes = new byte[ipx];
                    for (int j = 0; j < ipx; j++)
                        bytes[j] = br.ReadByte();

                    for (int j = 0; j < ipx; j++)
                    {
                        uint alpha1 = (bytes[j] & 0x0Fu) | ((bytes[j] & 0x0Fu) << 4);
                        texture[ix, iy] = new Rgba32((float)alpha1 / 255, 0, 0, 1);
                        SpriteAddIXIY(ref ix, ref iy, w, 1);

                        if (j != ipx - 1 || (bytes[bytes.Length - 1] & 0xF0) > 0)
                        {
                            uint alpha2 = (bytes[j] & 0xF0u) | ((bytes[j] & 0xF0u) >> 4);
                            texture[ix, iy] = new Rgba32((float)alpha2 / 255, 0, 0, 1);
                            SpriteAddIXIY(ref ix, ref iy, w, 1);
                        }
                    }

                    ids -= ipx;
                }
            }

            ms.Position = cpos + ds;
        }

        return new SpritesWithPalettesFile
        {
            Sprites = frames,
            Palettes = new List<Image<Rgba32>>()
        };
    }
}