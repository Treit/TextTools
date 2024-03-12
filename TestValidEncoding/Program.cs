using TextTools;

if (args.Length < 2)
{
    PrintUsage();
    return 1;
}

var inputPath = args[0];
var glob = "*.*";

for (int i = 0; i < args.Length; i++)
{
    if (args[i].ToLowerInvariant() is "--glob" or "/glob"  or "-glob")
    {
        if (i + 1 >= args.Length)
        {
            PrintUsage();
            return 1;
        }

        glob = args[i + 1];
    }
}

try
{
    if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
    {
        Console.WriteLine($"Could not find file or folder '{inputPath}'.");
        return 2;
    }

    var encodingStr = args[1];

    var filesToCheck = File.Exists(inputPath) ? 
        [inputPath] :
        Directory.EnumerateFiles(inputPath, glob, new EnumerationOptions { RecurseSubdirectories = true });

    foreach (var file in filesToCheck)
    {
        var valid = EncodingValidator.CheckEncoding(file, encodingStr);
        var validStr = valid ? "valid" : "invalid";

        Console.WriteLine($"{validStr},{new FileInfo(file).FullName}");
    }

    return 0;

}
catch
{
    Console.WriteLine(e);
    return 99;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <file> <encoding> [--glob <pattern>]");
}