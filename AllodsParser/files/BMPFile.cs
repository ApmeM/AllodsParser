using AllodsParser;
using SixLabors.ImageSharp;

public class BMPFile : IFile
{
    private readonly string filename;
    private readonly byte[] bytes;

    public BMPFile(string filename, byte[] bytes)
    {
        this.filename = filename;
        this.bytes = bytes;
    }

    public void Save(string path)
    {
        Directory.CreateDirectory(path);

        var texture = Image.Load(bytes);
        texture.Save(Path.Join(path, filename));
        // uint colormask = 0;
        // bool has_colormask = false;
        // if (has_colormask)
        // {
        //     //colormask &= 0xF0F0F0;
        //     for (int i = 0; i < colors.Length; i++)
        //     {
        //         uint pixel = ((uint)colors[i].r << 16) | ((uint)colors[i].g << 8) | colors[i].b | ((uint)colors[i].a << 24);
        //         if (/*(pixel & 0xF0F0F0) == colormask*/ (pixel & 0xFFFFFF) == colormask)
        //             colors[i].a = 0;
        //     }
        //     texture.SetPixels32(colors);
        // }

        // texture.Apply(false);
    }
}
