using ODataSamples.FileServiceLib.Models;
using ODataSamples.FileServiceLib.Schema.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Schema.Common;

internal class FileItemSchema : IFileItemSchema
{
    private readonly HashSet<IPropertyDefinition> _allProperties;
    private readonly Dictionary<string, IPropertyDefinition> _propertiesByName;

    public FileItemSchema()
    {
        _allProperties = [.. this.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(IPropertyDefinition))
            .Select(p => p.GetValue(this))
            .OfType<IPropertyDefinition>()];

        _propertiesByName = _allProperties.ToDictionary(p => p.Name);
    }

    public IPropertyDefinition FileExtension { get; } = new PropertyDefinition("FileExtension", typeof(string));

    public IPropertyDefinition FileSize { get; } = new PropertyDefinition("FileSize", typeof(long));

    public IPropertyDefinition Version { get; } = new PropertyDefinition("Version", typeof(int));

    public IPropertyDefinition Id { get; } = new PropertyDefinition("Id", typeof(string));

    public IPropertyDefinition IsProtected { get; } = new PropertyDefinition("IsProtected", typeof(bool));

    public IPropertyDefinition FileName { get; } = new PropertyDefinition("FileName", typeof(string));

    public IPropertyDefinition Description { get; } = new PropertyDefinition("Description", typeof(string));

    public IPropertyDefinition ExternalId { get; } = new PropertyDefinition("ExternalId", typeof(Guid));

    public IPropertyDefinition Tags { get; } = new PropertyDefinition("Tags", typeof(ICollection<string>));

    public IPropertyDefinition FileContent { get; } = new PropertyDefinition("FileContent", typeof(FileContent));

    public IPropertyDefinition AllExtensions { get; } = new PropertyDefinition("AllExtensions", typeof(IOpenPropertyValue));

    public IPropertyDefinition ItemProperties { get; } = new PropertyDefinition("ItemProperties", typeof(IOpenPropertyValue));

    public IPropertyDefinition CreatedAt { get; } = new PropertyDefinition("CreatedAt", typeof(DateTimeOffset));

    public IPropertyDefinition EntityACL { get; } = new PropertyDefinition("EntityACL", typeof(AccessControlList));

    public IPropertyDefinition BinaryData { get; } = new PropertyDefinition("BinaryData", typeof(byte[]));

    public IPropertyDefinition ByteCollection { get; } = new PropertyDefinition("ByteCollection", typeof(byte[]));

    public IPropertyDefinition ActivityStats { get;  } = new PropertyDefinition("ActivityStats", typeof(IEnumerable<ActivityStat>));

    public ISet<IPropertyDefinition> Properties => _allProperties;

    public bool TryGetPropertyByName(string name, out IPropertyDefinition? propertyDefinition)
    {
        propertyDefinition = null;
        return _propertiesByName.TryGetValue(name, out propertyDefinition);
    }
}
