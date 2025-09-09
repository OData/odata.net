using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Attributes;

/// <summary>
/// The attribute is applied on a class to specify a custom writer for a specific property
/// that is not necessarily defined on the class.
/// </summary>
/// <param name="propertyName"></param>
/// <param name="writerType">A custom type that extends one of the ODataPropertyValueWriter variants.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ODataPropertyValueWriterAttribute(string propertyName, Type writerType) : Attribute
{
    public string PropertyName { get; } = propertyName;
    public Type WriterType { get; } = writerType;
}
