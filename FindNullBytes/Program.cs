if (args.Length < 1)
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

    var filesToCheck = File.Exists(inputPath) ? 
        [inputPath] :
        Directory.EnumerateFiles(inputPath, glob, new EnumerationOptions { RecurseSubdirectories = true });

    foreach (var file in filesToCheck)
    {
        Console.WriteLine(file);
        if (HasNullBytes(file))
        {
            Console.WriteLine($"File '{file}' contains null bytes.");
        }
    }

    return 0;

}
catch (Exception e)
{
    Console.WriteLine(e);
    return 99;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <file> <encoding> [--glob <pattern>]");
}

bool HasNullBytes(string fileName)
{
    using var file = File.OpenRead(fileName);
    var read = 0;
    var buffer = new byte[1024];

    while ((read = file.Read(buffer)) > 0)
    {
        for (int i = 0; i < read; i++)
        {
            if (buffer[i] == 0)
            {
                return true;
            }
        }
    }

    return false;
}