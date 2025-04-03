namespace AllodsParser
{
    public abstract class BaseFile
    {
        public string relativeFilePath => Path.Combine(relativeFileDirectory, relativeFileName+relativeFileExtension);
        public string relativeFileDirectory { get; set; }
        public string relativeFileName { get; set; }
        public string relativeFileExtension { get; set; }

        public void Save(string outputFileBase)
        {
            var outputFileName = Path.Combine(outputFileBase, relativeFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));
            this.SaveInternal(outputFileName);
        }

        protected abstract void SaveInternal(string outputFileName);
    }
}