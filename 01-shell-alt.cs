///usr/bin/env dotnet "$0" "$@" ; exit $?
Console.WriteLine("No lemon, No melon.");

var filePath = AppContext.GetData("EntryPointFilePath") as string;
var directoryPath = AppContext.GetData("EntryPointFileDirectoryPath") as string;
Console.WriteLine($"File: {filePath}");
Console.WriteLine($"Directory: {directoryPath}");
