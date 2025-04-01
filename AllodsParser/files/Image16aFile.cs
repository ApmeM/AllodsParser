using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class Image16aFile
{
    // load palette
    // this is also used in .256/.16a to retrieve own palette
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

    private readonly List<Image<Rgba32>> frames = new List<Image<Rgba32>>();

    private readonly string filename;

    public Image16aFile(string filename, byte[] data)
    {
        this.filename = filename;

        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader br = new BinaryReader(ms);

        ms.Position = ms.Length - 4;
        int count = br.ReadInt32() & 0x7FFFFFFF;

        ms.Position = 0;

        // read palette
        var palette = LoadPaletteFromStream(br);

        int oldCount = count;
        for (int i = 0; i < count; i++)
        {
            uint w = br.ReadUInt32();
            uint h = br.ReadUInt32();
            uint ds = br.ReadUInt32();
            long cpos = ms.Position;

            if (w > 512 || h > 512 || ds > 1000000)
            {
                Console.WriteLine("Invalid sprite \"{0}\": NULL frame #{1}", filename, i);
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
    }


    public void Save(string path)
    {
        if (frames.Count == 0)
        {
            return;
        }
        Directory.CreateDirectory(path);

        var newWidth = frames.Max(a => a.Width);
        var newHeight = frames.Max(a => a.Height);

        var result = new Image<Rgba32>(newWidth * frames.Count, newHeight);

        for (int i = 0; i < frames.Count; i++)
        {
            result.Mutate(a => a.DrawImage(frames[i], new Point(newWidth * i, 0), 1));
            // this.frames[i].Save(Path.Join(path, filename.Replace(".256", $".frame{i}.png")), new PngEncoder());
        }

        result.Save(Path.Join(path, filename.Replace(".16a", $".frame.all.png")), new PngEncoder());
    }
}