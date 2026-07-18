static class TypeHelper
{
    public static string GetDefaultKey(Type type)
    {
        var key = string.IsNullOrEmpty(type.Namespace)
            ? type.Name
            : $"{type.Namespace}.{type.Name}";

        if (key.EndsWith("Action"))
            key = key[..^6];

        return key.ToLowerInvariant();
    }

    public static string GetName(Type type)
        => $"{type.FullName}, {type.Assembly.GetName().Name}";

    public static Type? Resolve(string typeName)
    {
        var fullName = typeName.Split(',')[0].Trim();

        return Type.GetType(typeName)
            ?? AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetType(fullName, throwOnError: false))
                .FirstOrDefault(x => x != null);
    }
}
