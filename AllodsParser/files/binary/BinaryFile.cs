namespace AllodsParser
{
    public class BinaryFile : BaseFile<byte[], byte[]>
    {
        protected override byte[]? LoadInternal(MemoryStream ms, BinaryReader br)
        {
            return br.ReadBytes((int)ms.Length);
        }

        protected override byte[]? ConvertInternal(byte[] toConvert)
        {
            return toConvert;
        }
        
        protected override void SaveInternal(string outputFileName, byte[]? toSave)
        {
            File.WriteAllBytes(outputFileName, toSave);
        }
    }
}