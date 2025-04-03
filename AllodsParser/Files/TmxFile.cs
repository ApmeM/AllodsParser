using SimpleTiled;

namespace AllodsParser
{
    public class TmxFile : BaseFile
    {
        internal TmxMap Map;

        protected override void SaveInternal(string outputFileName)
        {
            using (var f = File.OpenWrite(outputFileName))
            {
                TiledHelper.Write(Map, f);
            }
        }
    }
}