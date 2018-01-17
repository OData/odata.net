//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON Lite format that supports look-ahead and re-ordering of payloads.
    /// </summary>
    /// <remarks>TODO: not sure if this class could be implemented as a decorator as well.</remarks>
    internal sealed class ReorderingJsonReader : BufferingJsonReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerReader">The inner JSON reader.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        internal ReorderingJsonReader(IJsonReader innerReader, int maxInnerErrorDepth)
            : base(innerReader, JsonLightConstants.ODataErrorPropertyName, maxInnerErrorDepth)
        {
            Debug.Assert(innerReader != null, "innerReader != null");
        }

        /// <summary>
        /// Called whenever we find a new object value in the payload.
        /// Buffers and re-orders an object value for later consumption by the JsonLight reader.
        /// </summary>
        /// <remarks>
        /// This method is called when the reader is in the buffering mode and can read ahead (buffering) as much as it needs to
        /// once it returns the reader will be returned to the position before the method was called.
        /// The reader is always positioned on a start object when this method is called.
        /// </remarks>
        protected override void ProcessObjectValue()
        {
            Debug.Assert(this.currentBufferedNode.NodeType == JsonNodeType.StartObject, "this.currentBufferedNode.NodeType == JsonNodeType.StartObject");
            this.AssertBuffering();

            Stack<BufferedObject> bufferedObjectStack = new Stack<BufferedObject>();
            while (true)
            {
                switch (this.currentBufferedNode.NodeType)
                {
                    case JsonNodeType.StartObject:
                        {
                            // New object record - add the node to our stack
                            BufferedObject bufferedObject = new BufferedObject { ObjectStart = this.currentBufferedNode };
                            bufferedObjectStack.Push(bufferedObject);

                            // See if it's an in-stream error
                            base.ProcessObjectValue();
                            this.currentBufferedNode = bufferedObject.ObjectStart;

                            this.ReadInternal();
                        }

                        break;
                    case JsonNodeType.EndObject:
                        {
                            // End of object record
                            // Pop the node from our stack
                            BufferedObject bufferedObject = bufferedObjectStack.Pop();

                            // If there is a previous property record, mark its last value node.
                            if (bufferedObject.CurrentProperty != null)
                            {
                                bufferedObject.CurrentProperty.EndOfPropertyValueNode = this.currentBufferedNode.Previous;
                            }

                            // Now perform the re-ordering on the buffered nodes
                            bufferedObject.Reorder();

                            if (bufferedObjectStack.Count == 0)
                            {
                                // No more objects to process - we're done.
                                return;
                            }

                            this.ReadInternal();
                        }

                        break;
                    case JsonNodeType.Property:
                        {
                            BufferedObject bufferedObject = bufferedObjectStack.Peek();

                            // If there is a current property, mark its last value node.
                            if (bufferedObject.CurrentProperty != null)
                            {
                                bufferedObject.CurrentProperty.EndOfPropertyValueNode = this.currentBufferedNode.Previous;
                            }

                            BufferedProperty bufferedProperty = new BufferedProperty();
                            bufferedProperty.PropertyNameNode = this.currentBufferedNode;

                            string propertyName;
                            string annotationName;
                            this.ReadPropertyName(out propertyName, out annotationName);

                            bufferedProperty.PropertyAnnotationName = annotationName;
                            bufferedObject.AddBufferedProperty(propertyName, annotationName, bufferedProperty);

                            if (annotationName != null)
                            {
                                // Instance-level property annotation - no reordering in its value
                                // or instance-level annotation - no reordering in its value either
                                // So skip its value while buffering.
                                this.BufferValue();
                            }
                        }

                        break;

                    default:
                        // Read over (buffer) everything else
                        this.ReadInternal();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a property name from the JSON reader and determines if it's a regular property, an instance annotation or a property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the regular property which the reader is positioned on or which a property annotation belongs to.</param>
        /// <param name="annotationName">The name of the instance or property annotation, or null if the reader is on a regular property.</param>
        private void ReadPropertyName(out string propertyName, out string annotationName)
        {
            string jsonPropertyName = this.GetPropertyName();
            Debug.Assert(!string.IsNullOrEmpty(jsonPropertyName), "The JSON reader guarantees that property names are not null or empty.");

            this.ReadInternal();

            if (jsonPropertyName.StartsWith("@", StringComparison.Ordinal))
            {
                // Instance-level annotation for the instance itself; not property name.
                propertyName = null;
                annotationName = jsonPropertyName.Substring(1);
                if (annotationName.IndexOf('.') == -1)
                {
                    annotationName = JsonLightConstants.ODataAnnotationNamespacePrefix + annotationName;
                }
            }
            else
            {
                int separatorIndex = jsonPropertyName.IndexOf(JsonLightConstants.ODataPropertyAnnotationSeparatorChar);
                if (separatorIndex > 0)
                {
                    // This is a property annotation; compute the property and annotation names
                    propertyName = jsonPropertyName.Substring(0, separatorIndex);
                    annotationName = jsonPropertyName.Substring(separatorIndex + 1);
                }
                else
                {
                    // This is either a regular data property or an instance-level annotation
                    int dotIndex = jsonPropertyName.IndexOf('.');
                    if (dotIndex < 0)
                    {
                        // Regular property
                        propertyName = jsonPropertyName;
                        annotationName = null;
                    }
                    else
                    {
                        if (ODataJsonLightUtils.IsMetadataReferenceProperty(jsonPropertyName))
                        {
                            // Metadata reference property
                            propertyName = null;
                            annotationName = jsonPropertyName;
                        }
                        else
                        {
                            // unexpected instance annotation name
                            throw new ODataException(Microsoft.OData.Strings.JsonReaderExtensions_UnexpectedInstanceAnnotationName(jsonPropertyName));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads over a value buffering it.
        /// </summary>
        private void BufferValue()
        {
            this.AssertBuffering();

            // Skip the value bufferring it in the process.
            int depth = 0;
            do
            {
                switch (this.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        break;
                    case JsonNodeType.StartArray:
                    case JsonNodeType.StartObject:
                        depth++;
                        break;
                    case JsonNodeType.EndArray:
                    case JsonNodeType.EndObject:
                        Debug.Assert(depth > 0, "Seen too many scope ends.");
                        depth--;
                        break;
                    default:
                        break;
                }

                this.ReadInternal();
            }
            while (depth > 0);
        }

        /// <summary>
        /// A data structure to represent the buffered object with information about its properties,
        /// their order and annotations.
        /// </summary>
        private sealed class BufferedObject
        {
            /// <summary>The cache for properties.</summary>
            /// <remarks>The key is the property or instance annotation name,
            /// the value are the buffered properties grouped by property name (incl. annotation properties).</remarks>
            private readonly Dictionary<string, object> propertyCache;

            /// <summary>The set of data property names.</summary>
            /// <remarks>Data properties are the properties that are neither an instance annotation property nor a property annotation.</remarks>
            private readonly HashSet<string> dataProperties;

            /// <summary>A list of property names with their annotation name.</summary>
            /// <remarks>This is needed to properly maintain the relative order of annotation properties if no data
            /// property for the annotation property exists in the object.</remarks>
            private readonly List<KeyValuePair<string, string>> propertyNamesWithAnnotations;

            /// <summary>
            /// Constructor.
            /// </summary>
            internal BufferedObject()
            {
                this.propertyNamesWithAnnotations = new List<KeyValuePair<string, string>>();
                this.dataProperties = new HashSet<string>(StringComparer.Ordinal);
                this.propertyCache = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            /// <summary>
            /// The node in the linked list of buffered nodes where this object starts.
            /// </summary>
            internal BufferedNode ObjectStart { get; set; }

            /// <summary>
            /// The current buffered property being processed.
            /// </summary>
            internal BufferedProperty CurrentProperty { get; private set; }

            /// <summary>
            /// Adds a new buffered property to the list of buffered properties for this object.
            /// </summary>
            /// <param name="propertyName">The name of the data property (null for instance annotations).</param>
            /// <param name="annotationName">The name of the annotation (null for data properties).</param>
            /// <param name="bufferedProperty">The buffered property to add.</param>
            internal void AddBufferedProperty(string propertyName, string annotationName, BufferedProperty bufferedProperty)
            {
                this.CurrentProperty = bufferedProperty;
                string lookupName = propertyName ?? annotationName;

                // We have to record the relative positions of all properties so we can later on propertly sort them.
                // Note that we have to also capture the positions of the property annotations as long as we have not
                // seen a data property for that annotation since the data property might be missing.
                if (propertyName == null)
                {
                    this.propertyNamesWithAnnotations.Add(new KeyValuePair<string, string>(annotationName, null));
                }
                else if (!this.dataProperties.Contains(propertyName))
                {
                    if (annotationName == null)
                    {
                        this.dataProperties.Add(propertyName);
                    }

                    this.propertyNamesWithAnnotations.Add(new KeyValuePair<string, string>(propertyName, annotationName));
                }

                object storedValue;
                if (this.propertyCache.TryGetValue(lookupName, out storedValue))
                {
                    Debug.Assert(storedValue != null, "storedValue != null");
                    List<BufferedProperty> storedProperties;

                    BufferedProperty storedProperty = storedValue as BufferedProperty;
                    if (storedProperty != null)
                    {
                        storedProperties = new List<BufferedProperty>(4);
                        storedProperties.Add(storedProperty);
                        this.propertyCache[lookupName] = storedProperties;
                    }
                    else
                    {
                        storedProperties = (List<BufferedProperty>)storedValue;
                    }

                    storedProperties.Add(bufferedProperty);
                }
                else
                {
                    this.propertyCache.Add(lookupName, bufferedProperty);
                }
            }

            /// <summary>
            /// Reorders the buffered properties to conform to the required payload order.
            /// </summary>
            /// <remarks>
            /// The required order is: odata.context comes first, odata.removed comes next (for deleted resources),
            /// then comes odata.type, then all odata.* property annotations
            /// and finally, we preserve the relative order of custom annotations and data properties.</remarks>
            internal void Reorder()
            {
                BufferedNode currentNode = this.ObjectStart;

                // Sort the property names preserving the relative order of the properties except for the OData instance annotations.
                IEnumerable<string> sortedPropertyNames = this.SortPropertyNames();
                foreach (string propertyName in sortedPropertyNames)
                {
                    Debug.Assert(this.propertyCache.ContainsKey(propertyName), "Property must be in the cache for its name to be in the list of property names.");
                    object storedValue = this.propertyCache[propertyName];
                    BufferedProperty storedProperty = storedValue as BufferedProperty;
                    if (storedProperty != null)
                    {
                        storedProperty.InsertAfter(currentNode);
                        currentNode = storedProperty.EndOfPropertyValueNode;
                    }
                    else
                    {
                        IEnumerable<BufferedProperty> sortedProperties = SortBufferedProperties((IList<BufferedProperty>)storedValue);
                        foreach (BufferedProperty sortedProperty in sortedProperties)
                        {
                            sortedProperty.InsertAfter(currentNode);
                            currentNode = sortedProperty.EndOfPropertyValueNode;
                        }
                    }
                }
            }

            /// <summary>
            /// Sort the data properties and property annotations stored for a particular property name.
            /// </summary>
            /// <param name="bufferedProperties">The list of buffered properties to sort.</param>
            /// <returns>The sorted enumerable of buffered properties.</returns>
            /// <remarks>The sort order is for all odata.* property annotations to come before the data property
            /// but otherwise preserve the relative order of custom property annotations with regard to the position of the data property.</remarks>
            private static IEnumerable<BufferedProperty> SortBufferedProperties(IList<BufferedProperty> bufferedProperties)
            {
                Debug.Assert(bufferedProperties != null && bufferedProperties.Count > 1, "bufferedProperties != null && bufferedProperties.Count > 1");

                // NOTE: we re-order the property annotations so that ...
                //       1) all odata.* property annotations come before the data property.
                //       2) we preserve the relative order of custom property annotations with regard to the data property.
                List<BufferedProperty> delayedProperties = null;
                for (int i = 0; i < bufferedProperties.Count; ++i)
                {
                    BufferedProperty bufferedProperty = bufferedProperties[i];

                    string annotationName = bufferedProperty.PropertyAnnotationName;
                    if (annotationName == null || !IsODataInstanceAnnotation(annotationName))
                    {
                        // This is either the data property or a custom annotation; we have to delay reporting it until we reported all OData.* annotations
                        if (delayedProperties == null)
                        {
                            delayedProperties = new List<BufferedProperty>();
                        }

                        delayedProperties.Add(bufferedProperty);
                    }
                    else
                    {
                        yield return bufferedProperty;
                    }
                }

                // If we delayed reporting of any property annotations, report them now preserving their relative order.
                if (delayedProperties != null)
                {
                    for (int i = 0; i < delayedProperties.Count; ++i)
                    {
                        yield return delayedProperties[i];
                    }
                }
            }

            /// <summary>
            /// Checks whether an annotation name is an odata.* annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.* annotation; otherwise false.</returns>
            private static bool IsODataInstanceAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal);
            }

            /// <summary>
            /// Checks whether an annotation name is a odata.context annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.context annotation; otherwise false.</returns>
            private static bool IsODataContextAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return string.CompareOrdinal(ODataAnnotationNames.ODataContext, annotationName) == 0;
            }

            /// <summary>
            /// Checks whether an annotation name is a odata.removed annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.removed annotation; otherwise false.</returns>
            private static bool IsODataRemovedAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return string.CompareOrdinal(ODataAnnotationNames.ODataRemoved, annotationName) == 0;
            }

            /// <summary>
            /// Checks whether an annotation name is a odata.type annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.type annotation; otherwise false.</returns>
            private static bool IsODataTypeAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return string.CompareOrdinal(ODataAnnotationNames.ODataType, annotationName) == 0;
            }

            /// <summary>
            /// Checks whether an annotation name is a odata.id annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.id annotation; otherwise false.</returns>
            private static bool IsODataIdAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return string.CompareOrdinal(ODataAnnotationNames.ODataId, annotationName) == 0;
            }

            /// <summary>
            /// Checks whether an annotation name is a odata.etag annotation.
            /// </summary>
            /// <param name="annotationName">The annotation name to check.</param>
            /// <returns>true if the annotation name represents an odata.etag annotation; otherwise false.</returns>
            private static bool IsODataETagAnnotation(string annotationName)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                return string.CompareOrdinal(ODataAnnotationNames.ODataETag, annotationName) == 0;
            }

            /// <summary>
            /// Sorts the property names for an object.
            /// </summary>
            /// <returns>The sorted enumerable of property names.</returns>
            /// <remarks>The sort order is to put odata.context first, then odata.type, odata.id, and odata.etag, followed by all other odata.* instance annotations.
            /// For the rest, we preserve the relative order of custom annotations with regard to the data property.
            /// Note that we choose the position of the first property annotation in cases where no data property for a set of
            /// property annotations exists.</remarks>
            private IEnumerable<string> SortPropertyNames()
            {
                string contextAnnotationName = null;
                string removedAnnotationName = null;
                string typeAnnotationName = null;
                string idAnnotationName = null;
                string etagAnnotationName = null;
                List<String> odataAnnotationNames = null;
                List<String> otherNames = null;
                foreach (KeyValuePair<string, string> propertyNameWithAnnotation in this.propertyNamesWithAnnotations)
                {
                    string propertyName = propertyNameWithAnnotation.Key;

                    // First ignore a property annotation if we found a data property for it (since we will use the
                    // position of the data property). To keep the property annotations is important for cases
                    // where no data property exists for a set of property annotations.
                    if (propertyNameWithAnnotation.Value != null && this.dataProperties.Contains(propertyName))
                    {
                        continue;
                    }
                    else
                    {
                        // Add the property name to the 'dataProperties' to make sure we process the annotations for
                        // a property that is not in the payload at the position of the first such annotation.
                        this.dataProperties.Add(propertyName);
                    }

                    // Then find the special properties 'odata.context', 'odata.type', 'odata.id', and 'odata.etag' before separating
                    // the rest into odata.* annotations and regular properties (and custom annotations).
                    if (IsODataContextAnnotation(propertyName))
                    {
                        contextAnnotationName = propertyName;
                    }
                    else if (IsODataRemovedAnnotation(propertyName))
                    {
                        removedAnnotationName = propertyName;
                    }
                    else if (IsODataTypeAnnotation(propertyName))
                    {
                        typeAnnotationName = propertyName;
                    }
                    else if (IsODataIdAnnotation(propertyName))
                    {
                        idAnnotationName = propertyName;
                    }
                    else if (IsODataETagAnnotation(propertyName))
                    {
                        etagAnnotationName = propertyName;
                    }
                    else if (IsODataInstanceAnnotation(propertyName))
                    {
                        if (odataAnnotationNames == null)
                        {
                            odataAnnotationNames = new List<string>();
                        }

                        odataAnnotationNames.Add(propertyName);
                    }
                    else
                    {
                        if (otherNames == null)
                        {
                            otherNames = new List<string>();
                        }

                        otherNames.Add(propertyName);
                    }
                }

                if (contextAnnotationName != null)
                {
                    yield return contextAnnotationName;
                }

                if (removedAnnotationName != null)
                {
                    yield return removedAnnotationName;
                }

                if (typeAnnotationName != null)
                {
                    yield return typeAnnotationName;
                }

                if (idAnnotationName != null)
                {
                    yield return idAnnotationName;
                }

                if (etagAnnotationName != null)
                {
                    yield return etagAnnotationName;
                }

                if (odataAnnotationNames != null)
                {
                    foreach (string propertyName in odataAnnotationNames)
                    {
                        yield return propertyName;
                    }
                }

                if (otherNames != null)
                {
                    foreach (string propertyName in otherNames)
                    {
                        yield return propertyName;
                    }
                }
            }
        }

        /// <summary>
        /// A data structure to represent a buffered property.
        /// </summary>
        private sealed class BufferedProperty
        {
            /// <summary>
            /// The annotation name for this buffered property (either instance annotation or property annotation).
            /// </summary>
            internal string PropertyAnnotationName { get; set; }

            /// <summary>
            /// The node in the linked list of buffered nodes that represents the property name of the buffered property.
            /// </summary>
            internal BufferedNode PropertyNameNode { get; set; }

            /// <summary>
            /// The node in the linked list of buffered nodes that represents the end of the property value of the buffered property.
            /// </summary>
            internal BufferedNode EndOfPropertyValueNode { get; set; }

            /// <summary>
            /// Reorders the buffered property to be positioned after the <paramref name="node"/> node.
            /// </summary>
            /// <param name="node">The node after which to insert this buffered property.</param>
            internal void InsertAfter(BufferedNode node)
            {
                Debug.Assert(node != null, "node != null");

                // First take out the nodes representing the property from the linked list
                BufferedNode predecessor = this.PropertyNameNode.Previous;
                Debug.Assert(predecessor != null, "There must always be a predecessor for a property node.");
                BufferedNode successor = this.EndOfPropertyValueNode.Next;
                Debug.Assert(successor != null, "There should always be at least the end-object node after the property value.");
                predecessor.Next = successor;
                successor.Previous = predecessor;

                // Then insert the nodes representing the property after the 'insertAfter' node
                successor = node.Next;
                node.Next = this.PropertyNameNode;
                this.PropertyNameNode.Previous = node;
                this.EndOfPropertyValueNode.Next = successor;
                if (successor != null)
                {
                    successor.Previous = this.EndOfPropertyValueNode;
                }
            }
        }
    }
}
