namespace AllodsParser
{
    public class PalMergerConverter : BaseFileConverter
    {
        public override void Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<PalFile>()
                .GroupBy(a => a.relativeFileDirectory)
                .ToList();

            Console.WriteLine($"{this.GetType()} converts {oldFiles.Count} files");

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(oldFile =>
            {
                foreach (var f in oldFile)
                {
                    files.Remove(f);
                }
            });

            newFiles.ForEach(f => files.Add(f));
        }

        private IEnumerable<PalFile> ConvertFile(IGrouping<string, PalFile> toConvert, List<BaseFile> files)
        {
            yield return new PalFile
            {
                Palettes = toConvert.OrderBy(a => a.relativeFileName).SelectMany(a => a.Palettes).ToList(),
                relativeFileExtension = ".pal",
                relativeFileDirectory = toConvert.First().relativeFileDirectory,
                relativeFileName = "palette"
            };
        }
    }
}