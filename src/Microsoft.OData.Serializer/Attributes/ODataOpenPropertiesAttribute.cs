using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer;

/// <summary>
/// When applied to a property, indicates that the property holds a collection
/// of open/dynamic properties for an open type. The property should be
/// a type that implements IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt;.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ODataOpenPropertiesAttribute : Attribute
{
}
