using System.Text.Json;

namespace AllodsParser
{
    internal class RegFile : BaseFile
    {
        public Dictionary<string, object> Root;

        protected override void SaveInternal(string outputFileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this.Root, options);
            File.WriteAllText(outputFileName, json);
        }
    }
}