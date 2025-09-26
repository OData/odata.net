using ODataSamples.FileServiceLib.Schema.Abstractions;

namespace ODataSamples.FileServiceLib.Schema.Common;

public interface IFileItemSchema : IBaseItemSchema
{
    IPropertyDefinition FileExtension { get; }
    IPropertyDefinition FileSize { get; }
    IPropertyDefinition Version { get; }
}
