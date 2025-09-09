using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Attributes;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ODataValueWriterAttribute(Type writerType) : Attribute
{
    public Type WriterType { get; } = writerType;
}
