#!/usr/bin/env dotnet
Console.WriteLine("No lemon, no melon.");

var filePath = AppContext.GetData("EntryPointFilePath") as string;
var directoryPath = AppContext.GetData("EntryPointFileDirectoryPath") as string;
Console.WriteLine($"File: {filePath}");
Console.WriteLine($"Directory: {directoryPath}");
