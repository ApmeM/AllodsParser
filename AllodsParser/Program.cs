using System.Text;

namespace AllodsParser
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var resources = "/home/vas/Downloads/Allods2_en/data/";
            var result = "../Result";

            if (args.Length > 0)
            {
                resources = args[0];
            }
            if (args.Length > 1)
            {
                result = args[1];
            }

            if (Directory.Exists(result))
            {
                Directory.Delete(result, true);
            }
            Directory.CreateDirectory(result);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            foreach (var file in Directory.GetFiles(resources, "*.bmp", SearchOption.AllDirectories)) new BMPFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.wav", SearchOption.AllDirectories)) new BinaryFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.txt", SearchOption.AllDirectories)) new BinaryFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.reg", SearchOption.AllDirectories)) new RegistryFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.16", SearchOption.AllDirectories)) new Image16File(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.16a", SearchOption.AllDirectories)) new Image16aFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.256", SearchOption.AllDirectories)) new Image256File(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
            foreach (var file in Directory.GetFiles(resources, "*.alm", SearchOption.AllDirectories)) new AlmFile(Path.GetFileName(file), File.ReadAllBytes(file)).Save(Path.GetDirectoryName(file).Replace(resources, result + "/"));
        }
    }
}
