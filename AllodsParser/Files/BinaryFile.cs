namespace AllodsParser
{
    public class BinaryFile : BaseFile
    {
        public byte[] Data;

        protected override void SaveInternal(string outputFileName)
        {
            File.WriteAllBytes(outputFileName, Data);
        }
    }
}