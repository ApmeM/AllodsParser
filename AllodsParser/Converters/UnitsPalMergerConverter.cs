namespace AllodsParser
{
    public class UnitsPalMergerConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<PalFile>()
                .Where(a => a.relativeFileDirectory.Contains("/units/"))
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<SpritesWithPalettesFile> ConvertFile(PalFile toConvert, List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<SpritesWithPalettesFile>()
                .Where(a => a.relativeFileDirectory == toConvert.relativeFileDirectory)
                .ToList();

            if (oldFiles.Count == 0)
            {
                Console.WriteLine($"Cant find Sprites for palette {toConvert.relativeFilePath}");
                yield break;
            }

            SpritesWithPalettesFile? oldFile = null;

            if (oldFiles.Count == 2)
            {
                if (oldFiles[0].relativeFileName == oldFiles[1].relativeFileName + "b")
                {
                    oldFile = oldFiles[1];
                }
                if (oldFiles[0].relativeFileName + "b" == oldFiles[1].relativeFileName)
                {
                    oldFile = oldFiles[0];
                }
            }

            if (oldFile == null)
            {
                Console.WriteLine($"Too many sprites for palette {toConvert.relativeFilePath}");
                yield break;
            }

            oldFile.Palettes.Clear();
            oldFile.Palettes.AddRange(toConvert.Palettes);

            yield break;
        }
    }
}