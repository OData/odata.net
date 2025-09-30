using ODataSamples.FileServiceLib.Schema.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Schema.Common;

public interface IBaseItemSchema : ISchema
{
    IPropertyDefinition Id { get; }
    IPropertyDefinition IsProtected { get; }
    IPropertyDefinition FileName { get; }
    IPropertyDefinition Description { get; }
    IPropertyDefinition ExternalId { get; }
    IPropertyDefinition Tags { get; }
    IPropertyDefinition FileContent { get; }
    IPropertyDefinition AllExtensionsNames { get; }
    IPropertyDefinition AllExtensions { get; }
    IPropertyDefinition ItemProperties { get; }
    IPropertyDefinition CreatedAt { get; }
    IPropertyDefinition EntityACL { get; }
    IPropertyDefinition BinaryData { get; }
    IPropertyDefinition ByteCollection { get; }
    IPropertyDefinition ActivityStats { get; }
}
