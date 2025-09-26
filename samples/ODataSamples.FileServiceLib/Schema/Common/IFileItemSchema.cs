using ODataSamples.FileServiceLib.Schema.Abstractions;

namespace ODataSamples.FileServiceLib.Schema.Common;

public interface IFileItemSchema : ISchema
{
    IPropertyDefinition FileExtension { get; }
    IPropertyDefinition FileSize { get; }
    IPropertyDefinition Version { get; }
}
