namespace AllodsParser
{
    public abstract class BaseFileLoader
    {
        public virtual BaseFile Load(string relativeFilePath, byte[] bytes)
        {
            BaseFile result;

            if (bytes.Length == 0)
            {
                Console.Error.WriteLine($"File {relativeFilePath} is empty.");
                result = new EmptyFile();
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                using var br = new BinaryReader(ms);

                result = LoadInternal(relativeFilePath, ms, br);
            }

            result.relativeFileDirectory = Path.GetDirectoryName(relativeFilePath);
            result.relativeFileExtension = Path.GetExtension(relativeFilePath);
            result.relativeFileName = Path.GetFileNameWithoutExtension(relativeFilePath);

            return result;
        }

        protected abstract BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br);
    }
}