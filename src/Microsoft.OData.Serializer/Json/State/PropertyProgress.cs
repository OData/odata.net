namespace Microsoft.OData.Serializer.Json.State;

internal enum PropertyProgress : byte
{
    /// <summary>
    /// Not yet started writing the property.
    /// </summary>
    None = 0,
    /// <summary>
    /// Written the pre-value metadata (annotations) for the property.
    /// </summary>
    PreValueMetadata,
    /// <summary>
    /// Written the name of the property.
    /// </summary>
    Name,
    /// <summary>
    /// Completed writing the property value.
    /// </summary>
    Value,
    /// <summary>
    /// Completed writing the post-value metadata (annotations) for the property.
    /// </summary>
    PostValueMetadata
}
