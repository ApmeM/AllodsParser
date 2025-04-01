namespace AllodsParser
{
    public interface IFile
    {
        public void Init(string inputFileBase, string relativeFilePath);
        public void Save(string outputFileBase);
    }
}