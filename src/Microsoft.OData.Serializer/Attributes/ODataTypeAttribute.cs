using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ODataTypeAttribute(string fullTypeName) : Attribute
{
    public string FullTypeName { get; } = fullTypeName;
}
