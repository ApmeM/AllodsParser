using System.Text.Json;

namespace AllodsParser
{
    public class StructureRegFile : BaseFile
    {
        public class StructuresFileContent
        {
            public string Description { get; set; }
            public int Id { get; set; }
            public string File { get; set; }
            public int TileWidth { get; set; }
            public int TileHeight { get; set; }
            public int FullHeight { get; set; }
            public int SelectionX1 { get; set; }
            public int SelectionX2 { get; set; }
            public int SelectionY1 { get; set; }
            public int SelectionY2 { get; set; }
            public int ShadowY { get; set; }
            public int Phases { get; set; }
            public string AnimMask { get; set; }
            public int[] AnimFrame { get; set; }
            public int[] AnimTime { get; set; }
            public string Picture { get; set; }
            public int IconID { get; set; }
        }

        public List<StructuresFileContent> Structures;

        protected override void SaveInternal(string outputFileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this.Structures, options);
            File.WriteAllText(outputFileName, json);
        }
    }
}