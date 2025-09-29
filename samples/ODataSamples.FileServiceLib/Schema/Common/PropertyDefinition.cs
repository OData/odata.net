using ODataSamples.FileServiceLib.Schema.Abstractions;

namespace ODataSamples.FileServiceLib.Schema.Common;

internal class PropertyDefinition(string name, Type propertyType) : IPropertyDefinition
{
    public string Name => name;

    public Type PropertyType => propertyType;

    public bool Equals(IPropertyDefinition? other)
    {
        if (other is null) return false;
        return other.Name == Name && other.PropertyType == PropertyType;
    }
}
