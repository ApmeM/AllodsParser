using System.Text;


public class Core
{
    public static string UnpackByteString(int encoding, byte[] bytes)
    {
        var codePages = CodePagesEncodingProvider.Instance.GetEncoding(encoding);
        string str_in = codePages.GetString(bytes);
        string str_out = str_in;
        int i = str_out.IndexOf('\0');
        if (i >= 0) str_out = str_out.Substring(0, i);
        return str_out;
    }
}
