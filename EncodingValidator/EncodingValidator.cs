namespace TextTools;
using System.Text;

public static class EncodingValidator
{
    public static bool CheckEncoding(string path, string encodingStr)
    {
        Encoding encoding = encodingStr.ToUpperInvariant() switch
        {
            "UNICODE" or "UTF16" or "UTF16-LE" => new UnicodeEncoding(false, false, true),
            "UTF8" => new UTF8Encoding(false, true),
            "UTF32" => new UTF32Encoding(false, false, true),
            "BIGENDIANUNICODE" or "UTF16-BE" => new UnicodeEncoding(true, false, true),
            _ => throw new InvalidOperationException($"Unknown encoding '{encodingStr}' or that encoding does not support validation.")
        };

        try
        {
            using var sr = new StreamReader(path, encoding, true);
            while (sr.ReadLine() is string line)
            {
            }

            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }       
}
