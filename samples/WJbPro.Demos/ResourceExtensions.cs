using System.Reflection;

namespace WJbPro.Demos;

public static class ResourceExtensions
{
    public static string ReadEmbeddedResource(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new FileNotFoundException(
                $"Embedded resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    public static async Task<string> ReadEmbeddedResourceAsync(this Assembly assembly, string resourceName)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new FileNotFoundException(
                $"Embedded resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    public static string[] GetEmbeddedResources(this Assembly assembly)
        => assembly.GetManifestResourceNames();
}