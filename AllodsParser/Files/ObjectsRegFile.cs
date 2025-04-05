using System.Text.Json;

namespace AllodsParser
{
    public class ObjectsRegFile : BaseFile
    {
        public class ObjectsFileContent
        {
            public string Description { get; set; }
            public int Id { get; set; }
            public string File { get; set; }
            public int Parent { get; internal set; }
            public int Index { get; internal set; }
            public int Width { get; internal set; }
            public int Height { get; internal set; }
            public int CenterX { get; internal set; }
            public int CenterY { get; internal set; }
            public bool InMapEditor { get; internal set; }
            public int IconId { get; internal set; }
            public int DeadObject { get; internal set; }
            public int Phases { get; internal set; }
            public int[] AnimationTime { get; internal set; }
            public int[] AnimationFrame { get; internal set; }
        }

        public List<ObjectsFileContent> Objects;

        protected override void SaveInternal(string outputFileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this.Objects, options);
            File.WriteAllText(outputFileName, json);
        }
    }
}