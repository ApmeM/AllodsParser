namespace AllodsParser
{
    public class BinaryFileLoader : BaseFileLoader
    {
        protected override BaseFile LoadInternal(string relativeFilePath, MemoryStream ms, BinaryReader br)
        {
            return new BinaryFile
            {
                Data = br.ReadBytes((int)ms.Length)
            };
        }
    }
}