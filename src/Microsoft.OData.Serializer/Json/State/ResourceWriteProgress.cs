namespace Microsoft.OData.Serializer.Json.State;

/// <summary>
/// Represents the progress of writing a resource or collection.
/// </summary>
internal enum ResourceWriteProgress : byte
{
    // The order of enum members matters, the values are sorted.
    /// <summary>
    /// No part of the resource has been written yet.
    /// </summary>
    None = 0,
    /// <summary>
    /// The start token has been written.
    /// </summary>
    StartToken,
    /// <summary>
    /// The pre-value metadata have been written. This includes context url and annotations.
    /// </summary>
    PreValueMetadata,
    /// <summary>
    /// The value has been written. In the case of a resource, this means the declared/selected properties have been written.
    /// </summary>
    Value,
    /// <summary>
    /// The dynamic properties have been written. These are properties not declared in the model but present on the resource.
    /// </summary>
    DynamicProperties,
    /// <summary>
    /// The post-value metadata have been written. This includes annotations.
    /// </summary>
    PostValueMetadata,
    /// <summary>
    /// The end token has been written.
    /// </summary>
    EndToken,
}
