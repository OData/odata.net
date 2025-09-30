using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Schema.Abstractions;

/// <summary>
/// Represents a property of a resource type.
/// </summary>
public interface IPropertyDefinition : IEquatable<IPropertyDefinition>
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the CLR type of the property data.
    /// </summary>
    Type PropertyType { get; }
}
