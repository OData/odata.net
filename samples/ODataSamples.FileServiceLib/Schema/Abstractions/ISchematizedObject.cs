using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.Schema.Abstractions;

/// <summary>
/// Represents an resource instance/object
/// that is based on the specified schema.
/// </summary>
/// <typeparam name="TSchema"></typeparam>
public interface ISchematizedObject<TSchema>
    where TSchema : ISchema
{
    /// <summary>
    /// The schema containing the definition of properties
    /// that constitute the resource type.
    /// </summary>
    TSchema Schema { get; }

    /// <summary>
    /// The data associated with this object.
    /// </summary>
    IDictionary<IPropertyDefinition, object?> Data { get; }
}
