using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Attributes;


[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ODataValueWriterAttribute(Type writerType) : Attribute
{
    public Type WriterType { get; } = writerType;
}
