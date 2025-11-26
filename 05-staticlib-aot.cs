#:property OutputType=Library
#:property PublishAot=True
#:property NativeLib=Static
#:property RuntimeIdentifier=osx-arm64

// dotnet publish ./05-staticlib-aot.cs

using System.Runtime.InteropServices;

internal static class NativeEntryPoints
{
    [UnmanagedCallersOnly(EntryPoint = "mymethod")]
    public static void MyMethod(nint ptrText)
    {
        if (ptrText == default)
            return;

        string? text = Marshal.PtrToStringUni(ptrText);
        Console.WriteLine($"{DateTime.Now} {text}");
    }
}
