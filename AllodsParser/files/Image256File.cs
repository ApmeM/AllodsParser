﻿using System.Data.Common;
using System.IO.Pipes;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class Image256File
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

    public Image256File(string filename, byte[] data)
    {
        if (data.Length == 0)
        {
            Console.Error.WriteLine($"Empty file {filename}");
            return;
        }

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
                ushort ipx = br.ReadByte();
                ipx |= (ushort)(ipx << 8);
                ipx &= 0xC03F;
                ids--;

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
                    for (int j = 0; j < ipx; j++)
                    {
                        byte idx = br.ReadByte();
                        //uint px = (ss << 16) | (ss << 8) | (ss) | 0xFF000000;

                        texture[ix, iy] = palette[(int)idx, 0];

                        SpriteAddIXIY(ref ix, ref iy, w, 1);
                    }

                    ids -= ipx;
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

        var result = new Image<Rgba32>(newWidth, newHeight * frames.Count);

        for (int i = 0; i < frames.Count; i++)
        {
            result.Mutate(a => a.DrawImage(frames[i], new Point(0, newHeight * i), 1));
            this.frames[i].Save(Path.Join(path, filename.Replace(".256", $".frame{i}.png")), new PngEncoder());
        }

        result.Save(Path.Join(path, filename.Replace(".256", $".frame.all.png")), new PngEncoder());
    }
}
