using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Schema.Abstractions;

/// <summary>
/// Generic representation of a schema of specific resource type.
/// The schema contains the set of property definitions
/// that constitute the resource type.
/// </summary>
public interface ISchema
{
    /// <summary>
    /// Get all defined properties in this schema.
    /// </summary>
    ISet<IPropertyDefinition> Properties { get; }

    public bool TryGetPropertyByName(string name, out IPropertyDefinition? propertyDefinition);
}
