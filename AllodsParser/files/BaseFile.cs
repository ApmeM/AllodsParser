namespace AllodsParser
{
    public abstract class BaseFile<TInner, TOuter> : IFile
    {
        private bool IsLoadedInner = false;
        private bool IsLoadedOuter = false;

        private TInner? innerData;
        private TOuter? outerData;
        protected string inputFileBase;
        protected string relativeFilePath;

        public void Init(string inputFileBase, string relativeFilePath)
        {
            this.inputFileBase = inputFileBase;
            this.relativeFilePath = relativeFilePath;
        }

        public TInner? LoadRaw()
        {
            if (IsLoadedInner)
            {
                return this.innerData;
            }

            this.IsLoadedInner = true;
            this.innerData = default;

            var fileName = Path.Combine(inputFileBase, relativeFilePath);

            var bytes = File.ReadAllBytes(fileName);
            if (bytes.Length == 0)
            {
                Console.Error.WriteLine($"File {fileName} is empty.");
                return this.innerData;
            }

            using var ms = new MemoryStream(bytes);
            using var br = new BinaryReader(ms);

            this.innerData = LoadInternal(ms, br);

            return this.innerData;
        }

        public TOuter? LoadAndConvert()
        {
            var raw = LoadRaw();
            if (IsLoadedOuter)
            {
                return this.outerData;
            }

            this.IsLoadedOuter = true;
            this.outerData = default;

            if (raw == null)
            {
                return this.outerData;
            }

            this.outerData = ConvertInternal(raw);

            return this.outerData;
        }

        public void Save(string outputFileBase)
        {

            var toSave = LoadAndConvert();
            if (toSave == null)
            {
                Console.Error.WriteLine($"Nothing to save for {relativeFilePath}.");
                return;
            }


            var outputFileName = Path.Combine(outputFileBase, relativeFilePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));
            this.SaveInternal(outputFileName, toSave);
        }

        protected abstract TInner? LoadInternal(MemoryStream ms, BinaryReader br);
        protected abstract TOuter? ConvertInternal(TInner toConvert);
        protected abstract void SaveInternal(string outputFileName, TOuter toSave);
    }
}