namespace AllodsParser
{
    public abstract class BaseFileConverter
    {
        public virtual List<BaseFile> Convert(List<BaseFile> files)
        {
            return files;
        }
    }
}