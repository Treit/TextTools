using System.IO;
using System.Text;

if (args.Length < 3)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <file> <inputEncoding> <outputEncoding> [--includeBOM] [--noBackup]");
    return 1;
}

try
{
    var inputFile = args[0];

    if (!File.Exists(inputFile))
    {
        Console.WriteLine($"Could not find file '{inputFile}'.");
        return 2;
    }

    var inputEncodingStr = args[1];
    var outputEncodingStr = args[2];
    var includeBOM = false;
    foreach (var arg in args)
    {
        if (arg.ToUpperInvariant() is "-INCLUDEBOM" or "--INCLUDEBOM" or "/INCLUDEBOM")
        {
            includeBOM = true;
            break;
        }
    }

    var backup = true;
    foreach (var arg in args)
    {
        if (arg.ToUpperInvariant() is "-NOBACKUP" or "--NOBACKUP" or "/NOBACKUP")
        {
            backup = false;
            break;
        }
    }

#pragma warning disable SYSLIB0001 // Type or member is obsolete
    var inputEncoding = inputEncodingStr.ToUpperInvariant() switch
    {
        "ASCII" or "ANSI" => Encoding.ASCII,
        "UNICODE" or "UTF16" or "UTF16-LE" => new UnicodeEncoding(false, includeBOM, false),
        "UTF8" => new UTF8Encoding(includeBOM, false),
        "UTF7" => Encoding.UTF7,
        "UTF32" => new UTF32Encoding(false, includeBOM, false),
        "LATIN1" or "ISO8859-1" => Encoding.Latin1,
        "BIGENDIANUNICODE" or "UTF16-BE" => new UnicodeEncoding(true, includeBOM, false),
        _ => throw new InvalidOperationException($"Unknown encoding '{inputEncodingStr}'")
    };

    var outputEncoding = outputEncodingStr.ToUpperInvariant() switch
    {
        "ASCII" or "ANSI" => Encoding.ASCII,
        "UNICODE" or "UTF16" or "UTF16-LE" => new UnicodeEncoding(false, includeBOM, true),
        "UTF8" => new UTF8Encoding(includeBOM, true),
        "UTF7" => Encoding.UTF7,
        "UTF32" => new UTF32Encoding(false, includeBOM, true),
        "LATIN1" or "ISO8859-1" => Encoding.Latin1,
        "BIGENDIANUNICODE" or "UTF16-BE" => new UnicodeEncoding(true, includeBOM, true),
        _ => throw new InvalidOperationException($"Unknown encoding '{inputEncodingStr}'")
    };


#pragma warning restore SYSLIB0001 // Type or member is obsolete

    var tmpFile = Path.GetTempFileName();
    using (var sr = new StreamReader(inputFile, inputEncoding, true))
    using (var sw = new StreamWriter(tmpFile, false, outputEncoding))
    {
        while (sr.ReadLine() is string line)
        {
            sw.WriteLine(line);
        }
    }

    if (backup)
    {
        File.Copy(inputFile, $"{inputFile}.bak", true);
    }

    File.Copy(tmpFile, inputFile, true);
    File.Delete(tmpFile);

    return 0;
}
catch (Exception e)
{
    Console.WriteLine(e);
    return 99;
}