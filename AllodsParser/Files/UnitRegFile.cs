using System.Text.Json;

namespace AllodsParser
{
    public class UnitRegFile : BaseFile
    {
        public class UnitFileContent
        {
            public string Description { get; set; }
            public int Id { get; set; }
            public string File { get; set; }
            public int SelectionX1 { get; set; }
            public int SelectionX2 { get; set; }
            public int SelectionY1 { get; set; }
            public int SelectionY2 { get; set; }
            public int Parent { get; internal set; }
            public int Index { get; internal set; }
            public int MovePhases { get; internal set; }
            public int MoveBeginPhases { get; internal set; }
            public int AttackPhases { get; internal set; }
            public int DyingPhases { get; internal set; }
            public int Width { get; internal set; }
            public int Height { get; internal set; }
            public int CenterX { get; internal set; }
            public int CenterY { get; internal set; }
            public int[] AttackAnimTime { get; internal set; }
            public int[] AttackAnimFrame { get; internal set; }
            public int[] MoveAnimTime { get; internal set; }
            public int[] MoveAnimFrame { get; internal set; }
            public int Dying { get; internal set; }
            public int BonePhases { get; internal set; }
            public int Palette { get; internal set; }
            public int[] Sound { get; internal set; }
            public int Projectile { get; internal set; }
            public int AttackDelay { get; internal set; }
            public int ShootDelay { get; internal set; }
            public int[] ShootOffset { get; internal set; }
            public string InfoPicture { get; internal set; }
            public bool InMapEditor { get; internal set; }
        }

        public List<UnitFileContent> Units;

        protected override void SaveInternal(string outputFileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this.Units, options);
            File.WriteAllText(outputFileName, json);
        }
    }
}