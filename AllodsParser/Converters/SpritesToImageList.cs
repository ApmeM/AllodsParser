namespace AllodsParser
{
    public class SpritesToImageList : BaseFileConverter
    {
        public override List<BaseFile> Convert(List<BaseFile> files)
        {
            var oldFiles = files
                .OfType<ImageSpritesFile>()
                .ToList();

            var newFiles = oldFiles.SelectMany(a => ConvertFile(a, files)).ToList();

            oldFiles.ForEach(f => files.Remove(f));
            newFiles.ForEach(f => files.Add(f));

            return files;
        }

        private IEnumerable<ImageFile> ConvertFile(ImageSpritesFile toConvert, List<BaseFile> files)
        {
            for (int i = 0; i < toConvert.Sprites.Count; i++)
            {
                var f = toConvert.Sprites[i];
                yield return new ImageFile{
                    Image = f,
                    relativeFileExtension = ".png",
                    relativeFileDirectory = toConvert.relativeFileDirectory,
                    relativeFileName = toConvert.relativeFileName + "." + i
                };
            }
        }
    }
}