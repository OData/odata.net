using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

/// <summary>
/// When applied to a class, specifies the condition under which properties of the class should be ignored during OData serialization.
/// </summary>
/// <param name="condition"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ODataIgnorePropertiesAttribute(ODataIgnoreCondition condition) : Attribute
{
    public ODataIgnoreCondition Condition { get; } = condition;
}
