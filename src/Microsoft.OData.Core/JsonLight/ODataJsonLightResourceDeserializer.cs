//---------------------------------------------------------------------
// <copyright file="ODataJsonLightResourceDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight deserializer for entries and resource sets.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
    internal sealed class ODataJsonLightResourceDeserializer : ODataJsonLightPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightInputContext">The JsonLight input context to read from.</param>
        internal ODataJsonLightResourceDeserializer(ODataJsonLightInputContext jsonLightInputContext)
            : base(jsonLightInputContext)
        {
        }

        /// <summary>
        /// Reads the start of the JSON array for the content of the resource set.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:     The start of the resource set property array; this method will fail if the node is anything else.
        /// Post-Condition: JsonNodeType.StartObject:    The first item in the resource set
        ///                 JsonNodeType.PrimitiveValue: A null resource, or a primitive value within an untyped collection
        ///                 JsonNodeType.StartArray:     A nested collection within an untyped collection
        ///                 JsonNodeType.EndArray:       The end of the resource set
        /// </remarks>
        internal void ReadResourceSetContentStart()
        {
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartArray();
            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the end of the array containing the resource set content.
        /// </summary>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndArray
        /// Post-Condition: JsonNodeType.Property   if the resource set is part of an expanded nested resource info and there are more properties in the object
        ///                 JsonNodeType.EndObject  if the resource set is a top-level resource set or the expanded nested resource info is the last property of the payload
        ///                 JsonNodeType.EndOfInput  if the resource set is in a Uri operation parameter
        ///                 JsonNodeType.StartArray      if the resource set is a member of an untyped collection followed by a collection
        ///                 JsonNodeType.PrimitiveValue  if the resource set is a member of an untyped collection followed by a primitive value
        ///                 JsonNodeType.StartObject     if the resource set is a member of an untyped collection followed by a resource
        ///                 JsonNodeType.EndArray        if the resource set is the last member of an untyped collection
        /// </remarks>
        internal void ReadResourceSetContentEnd()
        {
            this.AssertJsonCondition(JsonNodeType.EndArray);
            this.JsonReader.AssertNotBuffering();

            this.JsonReader.ReadEndArray();

            this.AssertJsonCondition(JsonNodeType.EndOfInput, JsonNodeType.EndObject, JsonNodeType.Property, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.EndArray);
        }

        /// <summary>
        /// Reads the resource type name annotation (odata.type)
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method fills the ODataResource.TypeName property if the type name is found in the payload.
        /// </remarks>
        internal void ReadResourceTypeName(IODataJsonLightReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.type property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType, propertyName) == 0
                    || this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataTypePropertyName, propertyName))
                {
                    Debug.Assert(resourceState.Resource.TypeName == null, "type name should not have already been set");

                    // Read over the property to move to its value.
                    this.JsonReader.Read();

                    // Read the annotation value.
                    resourceState.Resource.TypeName = this.ReadODataTypeAnnotationValue();
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the OData 4.01 deleted resource annotation (odata.removed)
        /// </summary>
        /// <returns>Returns True if the resource is a deleted resource, otherwise returns false </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method Creates an ODataDeltaDeletedEntry and fills in the Id and Reason properties, if specified in the payload.
        /// </remarks>
        internal ODataDeletedResource IsDeletedResource()
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataDeletedResource deletedResource = null;

            // If the current node is the deleted property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataRemoved, propertyName) == 0
                    || this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataRemovedPropertyName, propertyName))
                {
                    DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;
                    Uri id = null;

                    // Read over the property to move to its value.
                    this.JsonReader.Read();

                    // Read the removed object and extract the reason, if present
                    this.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.PrimitiveValue /*null*/);
                    if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
                    {
                        while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.Read())
                        {
                            // If the current node is the reason property - read it.
                            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                            string.CompareOrdinal(JsonLightConstants.ODataReasonPropertyName, this.JsonReader.GetPropertyName()) == 0)
                            {
                                // Read over the property to move to its value.
                                this.JsonReader.Read();

                                // Read the reason value.
                                if (string.CompareOrdinal(JsonLightConstants.ODataReasonDeletedValue, this.JsonReader.ReadStringValue()) == 0)
                                {
                                    reason = DeltaDeletedEntryReason.Deleted;
                                }
                            }
                        }
                    }
                    else if (this.JsonReader.Value != null)
                    {
                        throw new ODataException(Strings.ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject(this.JsonReader.Value));
                    }

                    // read over end object or null value
                    this.JsonReader.Read();

                    // A deleted object must have at least either the odata id annotation or the key values
                    if (this.JsonReader.NodeType != JsonNodeType.Property)
                    {
                        throw new ODataException(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
                    }

                    // If the next property is the id property - read it.
                    propertyName = this.JsonReader.GetPropertyName();
                    if (string.CompareOrdinal(JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataId, propertyName) == 0
                        || this.CompareSimplifiedODataAnnotation(JsonLightConstants.SimplifiedODataIdPropertyName, propertyName))
                    {
                        // Read over the property to move to its value.
                        this.JsonReader.Read();

                        // Read the id value.
                        id = UriUtils.StringToUri(this.JsonReader.ReadStringValue());
                    }

                    deletedResource = ReaderUtils.CreateDeletedResource(id, reason);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return deletedResource;
        }

        /// <summary>
        /// Reads an OData 4.0 delete entry
        /// </summary>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// <returns>The <see cref="ODataDeletedResource"/> read.</returns>
        /// <remarks>
        /// This method Creates an ODataDeltaDeletedEntry and fills in the Id and Reason properties, if specified in the payload.
        /// </remarks>
        internal ODataDeletedResource ReadDeletedEntry()
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            Uri id = null;
            DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;

            // If the current node is the id property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataIdPropertyName, this.JsonReader.GetPropertyName()) == 0)
            {
                // Read over the property to move to its value.
                this.JsonReader.Read();

                // Read the Id value.
                id = this.JsonReader.ReadUriValue();
                Debug.Assert(id != null, "value for Id must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the reason property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataReasonPropertyName, this.JsonReader.GetPropertyName()) == 0)
            {
                // Read over the property to move to its value.
                this.JsonReader.Read();

                // Read the reason value.
                if (string.CompareOrdinal(JsonLightConstants.ODataReasonDeletedValue, this.JsonReader.ReadStringValue()) == 0)
                {
                    reason = DeltaDeletedEntryReason.Deleted;
                }
            }

            // Ignore unknown primitive properties in a 4.0 deleted entry
            while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.Read())
            {
                if (this.JsonReader.NodeType == JsonNodeType.StartObject || this.JsonReader.NodeType == JsonNodeType.StartArray)
                {
                    throw new ODataException(Strings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }
            }

            return ReaderUtils.CreateDeletedResource(id, reason);
        }

        /// <summary>
        /// Reads the delta (deleted) link source.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Source property if the id is found in the payload.
        /// </remarks>
        internal void ReadDeltaLinkSource(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the source property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataSourcePropertyName, this.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(link.Source == null, "source should not have already been set");

                // Read over the property to move to its value.
                this.JsonReader.Read();

                // Read the source value.
                link.Source = this.JsonReader.ReadUriValue();
                Debug.Assert(link.Source != null, "value for source must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta (deleted) link relationship.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Relationship property if the id is found in the payload.
        /// </remarks>
        internal void ReadDeltaLinkRelationship(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the relationship property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataRelationshipPropertyName, this.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(link.Relationship == null, "relationship should not have already been set");

                // Read over the property to move to its value.
                this.JsonReader.Read();

                // Read the relationship value.
                link.Relationship = this.JsonReader.ReadStringValue();
                Debug.Assert(link.Relationship != null, "value for relationship must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the delta (deleted) link target.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Target property if the id is found in the payload.
        /// </remarks>
        internal void ReadDeltaLinkTarget(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the target property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.CompareOrdinal(JsonLightConstants.ODataTargetPropertyName, this.JsonReader.GetPropertyName()) == 0)
            {
                Debug.Assert(link.Target == null, "target should not have already been set");

                // Read over the property to move to its value.
                this.JsonReader.Read();

                // Read the source value.
                link.Target = this.JsonReader.ReadUriValue();
                Debug.Assert(link.Target != null, "value for target must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the content of a resource until a nested resource info is detected.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        ///                 JsonNodeType.Property               If we've read a deferred link (this is the property after the deferred link)
        ///                 JsonNodeType.StartObject            Expanded resource
        ///                 JsonNodeType.StartArray             Expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null
        /// </remarks>
        internal ODataJsonLightReaderNestedResourceInfo ReadResourceContent(IODataJsonLightReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.ResourceType != null && this.Model.IsUserModel(), "A non-null resource type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            Debug.Assert(resourceState.ResourceType != null, "In JSON we must always have an structured type when reading resource.");

            // Figure out whether we have more properties for this resource
            // read all the properties until we hit a link
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    resourceState.PropertyAndAnnotationCollector,
                    this.ReadEntryPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                object value = ReadODataOrCustomInstanceAnnotationValue(resourceState, propertyParsingResult, propertyName);
                                this.ApplyEntryInstanceAnnotation(resourceState, propertyName, value);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithoutValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.NestedDeltaResourceSet:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, /*isDeltaResourceSet*/ true);
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, /*isDeltaResourceSet*/ false);
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                this.ReadMetadataReferencePropertyValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.EndOfObject:
                                break;
                        }
                    });

                if (readerNestedResourceInfo != null)
                {
                    // we found a nested resource info
                    // stop parsing the resource content and return to the caller
                    break;
                }

                Debug.Assert(
                    this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "After reading a property the reader should be positioned on another property or have hit the end of the object.");
            }

            this.JsonReader.AssertNotBuffering();

            // The reader can be either on
            //  - StartObject - if it's an expanded resource
            //  - StartArray - if it's an expanded resource set
            //  - Property - if it's a deferred link
            //  - PrimitiveValue (null) - if it's an expanded null resource
            //  - EndObject - end of the resource
            Debug.Assert(
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.Property ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue && this.JsonReader.Value == null ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Property or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="resourceState">The IODataJsonLightReaderResourceState.</param>
        /// <param name="propertyParsingResult">The PropertyParsingResult.</param>
        /// <param name="annotationName">The annotation name</param>
        /// <returns>The annotation value.</returns>
        internal object ReadODataOrCustomInstanceAnnotationValue(IODataJsonLightReaderResourceState resourceState, PropertyParsingResult propertyParsingResult, string annotationName)
        {
            object value = this.ReadEntryInstanceAnnotation(annotationName, resourceState.AnyPropertyFound, /*typeAnnotationFound*/ true, resourceState.PropertyAndAnnotationCollector);
            if (propertyParsingResult == PropertyParsingResult.ODataInstanceAnnotation)
            {
                resourceState.PropertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, value);
            }
            else
            {
                resourceState.PropertyAndAnnotationCollector.AddCustomScopeAnnotation(annotationName, value);
            }

            return value;
        }

        /// <summary>
        /// Validates resource metadata.
        /// </summary>
        /// <param name="resourceState">The resource state to use.</param>
        internal void ValidateMediaEntity(IODataJsonLightReaderResourceState resourceState)
        {
            ODataResourceBase resource = resourceState.Resource;
            if (resource != null)
            {
                IEdmEntityType entityType = resourceState.ResourceType as IEdmEntityType;
                if (entityType != null)
                {
                    // If the entity in the model has a default stream and if no MR related metadata exists in the resource payload, create an empty MediaResource.
                    // Note that for responses the metadata builder will compute the default stream.  For requests we really don't need to add the default stream since the service knows its metadata.
                    // We leave this here for now so we don't introduce a breaking change.
                    if (!this.ReadingResponse && entityType.HasStream && resource.MediaResource == null)
                    {
                        ODataStreamReferenceValue mediaResource = resource.MediaResource;
                        ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                        this.SetEntryMediaResource(resourceState, mediaResource);
                    }

                    this.ReaderValidator.ValidateMediaResource(resource, entityType);
                }
            }
        }

        /// <summary>
        /// Reads the resource set instance annotations for a top-level resource set.
        /// </summary>
        /// <param name="resourceSet">The <see cref="ODataResourceSet"/> to read the instance annotations for.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the top-level scope.</param>
        /// <param name="forResourceSetStart">true when parsing the instance annotations before the resource set property;
        /// false when parsing the instance annotations after the resource set property.</param>
        /// <param name="readAllResourceSetProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        internal void ReadTopLevelResourceSetAnnotations(ODataResourceSetBase resourceSet, PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool forResourceSetStart, bool readAllResourceSetProperties)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            this.JsonReader.AssertNotBuffering();

            bool buffering = false;
            try
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    bool foundValueProperty = false;

                    if (!forResourceSetStart && readAllResourceSetProperties)
                    {
                        // If this is not called for reading ResourceSetStart and we already scanned ahead and processed all resource set properties, we already checked for duplicate property names.
                        // Use an empty duplicate property name checker since this.ParseProperty() read through the same property annotation of instance annotations again.
                        propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(false);
                    }

                    this.ProcessProperty(
                        propertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValue,
                        (propertyParseResult, propertyName) =>
                        {
                            switch (propertyParseResult)
                            {
                                case PropertyParsingResult.ODataInstanceAnnotation:
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    ReadODataOrCustomInstanceAnnotationValue(resourceSet, propertyAndAnnotationCollector,
                                        forResourceSetStart, readAllResourceSetProperties, propertyParseResult, propertyName);
                                    break;

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.CompareOrdinal(JsonLightConstants.ODataValuePropertyName, propertyName) == 0)
                                    {
                                        // We found the resource set property and are done parsing property annotations;
                                        // When we are in the mode where we scan ahead and read all resource set properties
                                        // (for the reordering scenario), we have to start buffering and continue
                                        // reading. Otherwise we found the resourceSet's data property and are done.
                                        if (readAllResourceSetProperties)
                                        {
                                            this.JsonReader.StartBuffering();
                                            buffering = true;

                                            this.JsonReader.SkipValue();
                                        }
                                        else
                                        {
                                            foundValueProperty = true;
                                        }
                                    }
                                    else
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet(propertyName, JsonLightConstants.ODataValuePropertyName));
                                    }

                                    break;
                                case PropertyParsingResult.PropertyWithoutValue:
                                    // If we find a property without a value it means that we did not find the resource set property (yet)
                                    // but an invalid property annotation
                                    throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet(propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    if (!(resourceSet is ODataResourceSet))
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(propertyName));
                                    }

                                    this.ReadMetadataReferencePropertyValue((ODataResourceSet)resourceSet, propertyName);
                                    break;

                                default:
                                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightResourceDeserializer_ReadTopLevelResourceSetAnnotations));
                            }
                        });

                    if (foundValueProperty)
                    {
                        return;
                    }
                }
            }
            finally
            {
                if (buffering)
                {
                    Debug.Assert(readAllResourceSetProperties, "Expect the reader to be in buffering mode only when scanning to the end.");
                    this.JsonReader.StopBuffering();
                }
            }

            if (forResourceSetStart && !readAllResourceSetProperties)
            {
                // We did not find any properties or only instance annotations.
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound(JsonLightConstants.ODataValuePropertyName));
            }
        }

        /// <summary>
        /// Reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="resourceSet">The ODataResourceSetBase.</param>
        /// <param name="propertyAndAnnotationCollector">The PropertyAndAnnotationCollector.</param>
        /// <param name="forResourceSetStart">true when parsing the instance annotations before the resource set property;
        /// false when parsing the instance annotations after the resource set property.</param>
        /// <param name="readAllResourceSetProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        /// <param name="propertyParseResult">The PropertyParsingResult.</param>
        /// <param name="annotationName">The annotation name.</param>
        internal void ReadODataOrCustomInstanceAnnotationValue(ODataResourceSetBase resourceSet,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool forResourceSetStart,
            bool readAllResourceSetProperties, PropertyParsingResult propertyParseResult, string annotationName)
        {
            if (propertyParseResult == PropertyParsingResult.ODataInstanceAnnotation)
            {
                // #### annotation 1 ####
                // built-in "odata." annotation value is added to propertyAndAnnotationCollector then later to resourceSet.InstanceAnnotations.
                propertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, this.JsonReader.Value);
            }

            // When we are reading the start of a resource set (in scan-ahead mode or not) or when
            // we read the end of a resource set and not in scan-ahead mode, read the value;
            // otherwise skip it.
            if (forResourceSetStart || !readAllResourceSetProperties)
            {
                // #### annotation 2 ####
                // custom annotation value will be directly added to resourceSet.InstanceAnnotations.
                this.ReadAndApplyResourceSetInstanceAnnotationValue(annotationName, resourceSet, propertyAndAnnotationCollector);
            }
            else
            {
                this.JsonReader.SkipValue();
            }
        }

        /// <summary>
        /// Reads a value of property annotation on the resource level.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>The value of the property annotation.</returns>
        /// <remarks>
        /// This method should read the property annotation value and return a representation of the value which will be later
        /// consumed by the resource reading code.
        ///
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the property annotation
        /// </remarks>
        internal object ReadEntryPropertyAnnotationValue(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            string typeName;
            if (this.TryReadODataTypeAnnotationValue(propertyAnnotationName, out typeName))
            {
                return typeName;
            }

            switch (propertyAnnotationName)
            {
                case ODataAnnotationNames.ODataNavigationLinkUrl:  // odata.navigationLinkUrl
                case ODataAnnotationNames.ODataAssociationLinkUrl: // odata.associationLinkUrl
                case ODataAnnotationNames.ODataNextLink:           // odata.nextLink
                case ODataAnnotationNames.ODataMediaEditLink:      // odata.mediaEditLink
                case ODataAnnotationNames.ODataMediaReadLink:      // odata.mediaReadLink
                case ODataAnnotationNames.ODataContext:            // odata.context
                    return this.ReadAndValidateAnnotationStringValueAsUri(propertyAnnotationName);

                case ODataAnnotationNames.ODataCount:              // odata.count
                    return this.ReadAndValidateAnnotationAsLongForIeee754Compatible(propertyAnnotationName);

                case ODataAnnotationNames.ODataMediaETag:          // odata.mediaEtag
                case ODataAnnotationNames.ODataMediaContentType:   // odata.mediaContentType
                    return this.ReadAndValidateAnnotationStringValue(propertyAnnotationName);

                // odata.bind
                case ODataAnnotationNames.ODataBind:
                    // The value of the odata.bind annotation can be either an array of strings or a string (collection or singleton nested resource info).
                    // Note that we don't validate that the cardinality of the navigation property matches the payload here, since we don't want to lookup the property twice.
                    // We will validate that later when we consume the value of the property annotation.
                    if (this.JsonReader.NodeType != JsonNodeType.StartArray)
                    {
                        return new ODataEntityReferenceLink
                        {
                            Url = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataBind)
                        };
                    }

                    LinkedList<ODataEntityReferenceLink> entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();

                    // Read over the start array
                    this.JsonReader.Read();
                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        entityReferenceLinks.AddLast(
                            new ODataEntityReferenceLink
                            {
                                Url = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataBind)
                            });
                    }

                    // Read over the end array
                    this.JsonReader.Read();
                    if (entityReferenceLinks.Count == 0)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_EmptyBindArray(ODataAnnotationNames.ODataBind));
                    }

                    return entityReferenceLinks;

                case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                default:
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(propertyAnnotationName));
            }
        }

        /// <summary>
        /// Reads instance annotation in the resource object.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="anyPropertyFound">true if a non-annotation property has already been encountered.</param>
        /// <param name="typeAnnotationFound">true if the 'odata.type' annotation has already been encountered, or should have been by now.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the resource being read.</param>
        /// <returns>The value of the annotation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        internal object ReadEntryInstanceAnnotation(string annotationName, bool anyPropertyFound, bool typeAnnotationFound, PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    if (!typeAnnotationFound)
                    {
                        return this.ReadODataTypeAnnotationValue();
                    }

                    // We already read the odata.type if it was the first property in ReadResourceStart, so any other occurrence means
                    // that it was not the first property.
                    throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst);

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty(annotationName));
                    }

                    return this.ReadAnnotationStringValueAsUri(annotationName);

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty(annotationName));
                    }

                    return this.ReadAndValidateAnnotationStringValue(annotationName);

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                case ODataAnnotationNames.ODataMediaEditLink:   // 'odata.mediaEditLink'
                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    return this.ReadAndValidateAnnotationStringValueAsUri(annotationName);

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    return this.ReadAndValidateAnnotationStringValue(annotationName);

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    return this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, annotationName);
            }
        }

        /// <summary>
        /// Reads instance annotation in the resource object.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="annotationValue">The value of the annotation.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "The casts aren't actually being done multiple times, since they occur in different cases of the switch statement.")]
        internal void ApplyEntryInstanceAnnotation(IODataJsonLightReaderResourceState resourceState, string annotationName, object annotationValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            ODataResourceBase resource = resourceState.Resource;
            ODataStreamReferenceValue mediaResource = resource.MediaResource;
            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    resource.TypeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)annotationValue));
                    break;

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (annotationValue == null)
                    {
                        resource.IsTransient = true;
                    }
                    else
                    {
                        resource.Id = (Uri)annotationValue;
                    }

                    break;

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    resource.ETag = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                    resource.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                    resource.ReadLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaEditLink:   // 'odata.mediaEditLink'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ReadLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ContentType = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    ODataJsonLightReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ETag = (string)annotationValue;
                    break;

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    resource.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, annotationValue.ToODataValue()));
                    break;
            }

            if (mediaResource != null && resource.MediaResource == null)
            {
                this.SetEntryMediaResource(resourceState, mediaResource);
            }
        }

        /// <summary>
        /// Reads the value of a resource set annotation (count or next link).
        /// </summary>
        /// <param name="annotationName">The name of the annotation found.</param>
        /// <param name="resourceSet">The resource set to read the annotation for; if non-null, the annotation value will be assigned to the resource set.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker instance.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource set object
        ///                 JsonNodeType.Property               The next annotation after the current annotation
        /// </remarks>
        internal void ReadAndApplyResourceSetInstanceAnnotationValue(string annotationName, ODataResourceSetBase resourceSet, PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataCount:
                    resourceSet.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                    break;

                case ODataAnnotationNames.ODataNextLink:
                    resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                    break;

                case ODataAnnotationNames.ODataDeltaLink:
                    resourceSet.DeltaLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataDeltaLink);
                    break;
                case ODataAnnotationNames.ODataType:

                    // TODO: skip the odata.type;
                    this.ReadAndValidateAnnotationStringValue(ODataAnnotationNames.ODataType);
                    break;
                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    object instanceAnnotationValue = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, annotationName);
                    resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, instanceAnnotationValue.ToODataValue()));
                    break;
            }
        }

        /// <summary>
        /// Reads resource property which doesn't have value, just annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              The end of the resource object.
        ///                 JsonNodeType.Property               The property after the one we're to read.
        /// Post-Condition: JsonNodeType.EndObject              This method doesn't move the reader.
        ///                 JsonNodeType.Property
        /// </remarks>
        internal ODataJsonLightReaderNestedResourceInfo ReadPropertyWithoutValue(IODataJsonLightReaderResourceState resourceState, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
            if (edmProperty != null && !edmProperty.Type.IsUntyped())
            {
                // Declared property - read it.
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (navigationProperty != null)
                {
                    if (this.ReadingResponse)
                    {
                        // Deferred link
                        readerNestedResourceInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, navigationProperty);
                    }
                    else
                    {
                        // Entity reference link or links
                        readerNestedResourceInfo = navigationProperty.Type.IsCollection()
                            ? ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, /*isExpanded*/ false)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, /*isExpanded*/ false);

                        if (!readerNestedResourceInfo.HasEntityReferenceLink)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(propertyName, ODataAnnotationNames.ODataBind));
                        }
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                }
                else
                {
                    IEdmTypeReference propertyTypeReference = edmProperty.Type;
                    if (propertyTypeReference.IsStream())
                    {
                        Debug.Assert(propertyName == edmProperty.Name, "propertyName == edmProperty.Name");
                        ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                        AddResourceProperty(resourceState, edmProperty.Name, streamPropertyValue);
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType(propertyName, propertyTypeReference.FullName()));
                    }
                }
            }
            else
            {
                // Undeclared property - we need to run detection algorithm here.
                readerNestedResourceInfo = this.ReadUndeclaredProperty(resourceState, propertyName, /*propertyWithValue*/ false);
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Reads any next link annotation immediately after the end of a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set being read.</param>
        /// <param name="expandedNestedResourceInfo">The information about the expanded link. This must be non-null if we're reading an expanded resource set, and must be null if we're reading a top-level resource set.</param>
        /// <param name="propertyAndAnnotationCollector">The top-level duplicate property names checker, if we're reading a top-level resource set.</param>
        internal void ReadNextLinkAnnotationAtResourceSetEnd(
            ODataResourceSetBase resourceSet,
            ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // Check for annotations on the resource set that occur after the resource set itself. (Note: the only allowed one is odata.nextLink, and we fail for anything else.)
            // We do this slightly differently depending on whether the resource set was an expanded navigation or a top-level resource set.
            if (expandedNestedResourceInfo != null)
            {
                this.ReadExpandedResourceSetAnnotationsAtResourceSetEnd(resourceSet, expandedNestedResourceInfo);
            }
            else
            {
                Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

                // Check for resource set instance annotations that appear after the resource set.
                bool isReordering = this.JsonReader is ReorderingJsonReader;
                this.ReadTopLevelResourceSetAnnotations(resourceSet, propertyAndAnnotationCollector, /*forResourceSetStart*/false, /*readAllResourceSetProperties*/isReordering);
            }
        }

        /// <summary>
        /// Reads the information of a deferred link.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationPropertyName">The name of the navigation property for which to read the deferred link.</param>
        /// <param name="navigationProperty">The navigation property for which to read the deferred link. This can be null.</param>
        /// <returns>Returns the nested resource info for the deferred nested resource info read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        private static ODataJsonLightReaderNestedResourceInfo ReadDeferredNestedResourceInfo(IODataJsonLightReaderResourceState resourceState, string navigationPropertyName, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(navigationProperty == null || navigationPropertyName == navigationProperty.Name, "navigationProperty == null || navigationPropertyName == navigationProperty.Name");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationPropertyName,
                IsCollection = navigationProperty == null ? null : (bool?)navigationProperty.Type.IsCollection()
            };

            foreach (var propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataNavigationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataAssociationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                        break;

                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation(nestedResourceInfo.Name, propertyAnnotation.Key));
                }
            }

            return ODataJsonLightReaderNestedResourceInfo.CreateDeferredLinkInfo(nestedResourceInfo, navigationProperty);
        }

        /// <summary>
        /// We fail here if we encounter any other property annotation for the expanded navigation (since these should come before the property itself).
        /// </summary>
        /// <param name="resourceSet">The resource set that was just read.</param>
        /// <param name="expandedNestedResourceInfo">The information for the current expanded nested resource info being read.</param>
        private void ReadExpandedResourceSetAnnotationsAtResourceSetEnd(ODataResourceSetBase resourceSet, ODataJsonLightReaderNestedResourceInfo expandedNestedResourceInfo)
        {
            Debug.Assert(expandedNestedResourceInfo != null, "expandedNestedResourceInfo != null");
            Debug.Assert(expandedNestedResourceInfo.NestedResourceInfo.IsCollection == true, "Only collection navigation properties can have resourceSet content.");

            // Look at the next property in the owning resource, if it's a property annotation for the expanded nested resource info property, read it.
            string propertyName, annotationName;
            while (this.JsonReader.NodeType == JsonNodeType.Property &&
                   TryParsePropertyAnnotation(this.JsonReader.GetPropertyName(), out propertyName, out annotationName) &&
                   string.CompareOrdinal(propertyName, expandedNestedResourceInfo.NestedResourceInfo.Name) == 0)
            {
                if (!this.ReadingResponse)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation(propertyName, annotationName));
                }

                // Read over the property name.
                this.JsonReader.Read();

                switch (this.CompleteSimplifiedODataAnnotation(annotationName))
                {
                    case ODataAnnotationNames.ODataNextLink:
                        if (resourceSet.NextPageLink != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation(ODataAnnotationNames.ODataNextLink, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        if (resourceSet.Count != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation(ODataAnnotationNames.ODataCount, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet(annotationName, expandedNestedResourceInfo.NestedResourceInfo.Name));
                }
            }
        }

        /// <summary>
        /// Sets specified media resource on a resource and hooks up metadata builder.
        /// </summary>
        /// <param name="resourceState">The resource state to use.</param>
        /// <param name="mediaResource">The media resource to set.</param>
        private void SetEntryMediaResource(IODataJsonLightReaderResourceState resourceState, ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(mediaResource != null, "mediaResource != null");
            ODataResourceBase resource = resourceState.Resource;
            Debug.Assert(resource != null, "resource != null");

            ODataResourceMetadataBuilder builder =
                this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                    this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment);
            mediaResource.SetMetadataBuilder(builder, /*propertyName*/ null);
            resource.MediaResource = mediaResource;
        }

        /// <summary>
        /// Reads resource property (which is neither instance nor property annotation) which has a value.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <param name="isDeltaResourceSet">The property being read represents a nested delta resource set.</param>
        /// <returns>A reader nested resource info representing the nested resource info detected while reading the resource contents; null if no nested resource info was detected.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the property
        ///                 JsonNodeType.StartObject            Expanded resource
        ///                 JsonNodeType.StartArray             Expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null resource
        /// </remarks>
        private ODataJsonLightReaderNestedResourceInfo ReadPropertyWithValue(IODataJsonLightReaderResourceState resourceState, string propertyName, bool isDeltaResourceSet)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo = null;
            IEdmStructuredType resouceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resouceType);

            if (edmProperty != null && !edmProperty.Type.IsUntyped())
            {
                IEdmStructuralProperty structuredProperty = edmProperty as IEdmStructuralProperty;
                IEdmStructuredType structuredPropertyTypeOrItemType = structuredProperty == null ? null : structuredProperty.Type.ToStructuredType();
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (structuredPropertyTypeOrItemType != null)
                {
                    // Complex property or collection of complex property.
                    bool isCollection = structuredProperty.Type.IsCollection();
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName);

                    if (isCollection)
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceSetNestedResourceInfo(resourceState, structuredProperty, structuredPropertyTypeOrItemType, structuredProperty.Name);
                    }
                    else
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceNestedResourceInfo(resourceState, structuredProperty, structuredPropertyTypeOrItemType, structuredProperty.Name);
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                }
                else if (navigationProperty != null)
                {
                    // Expanded link
                    bool isCollection = navigationProperty.Type.IsCollection();
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName);
                    if (isCollection)
                    {
                        readerNestedResourceInfo = this.ReadingResponse
                            ? ReadExpandedResourceSetNestedResourceInfo(resourceState, navigationProperty, navigationProperty.Type.ToStructuredType(), propertyName, /*isDeltaResourceSet*/ isDeltaResourceSet)
                            : ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, /*isExpanded*/ true);
                    }
                    else
                    {
                        readerNestedResourceInfo = this.ReadingResponse
                            ? ReadExpandedResourceNestedResourceInfo(resourceState, navigationProperty, propertyName, navigationProperty.Type.ToStructuredType(), this.MessageReaderSettings)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, /*isExpanded*/ true);
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                }
                else
                {
                    IEdmTypeReference propertyTypeReference = edmProperty.Type;
                    if (propertyTypeReference.IsStream())
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_StreamPropertyWithValue(propertyName));
                    }

                    // NOTE: we currently do not check whether the property should be skipped
                    //       here because this can only happen for navigation properties and open properties.
                    this.ReadEntryDataProperty(resourceState, edmProperty, ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName));
                }
            }
            else
            {
                // Undeclared property - we need to run detection alogorithm here.
                readerNestedResourceInfo = this.ReadUndeclaredProperty(resourceState, propertyName, /*propertyWithValue*/ true);

                // Note that if nested resource info is returned it's already validated, so we just report it here.
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Read a resource-level data property and check its version compliance.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="edmProperty">The EDM property of the property being read, or null if the property is an open property.</param>
        /// <param name="propertyTypeName">The type name specified for the property in property annotation, or null if no such type name is available.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private void ReadEntryDataProperty(IODataJsonLightReaderResourceState resourceState, IEdmProperty edmProperty, string propertyTypeName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");
            this.JsonReader.AssertNotBuffering();

            // EdmLib bridge marks all key properties as non-nullable, but Astoria allows them to be nullable.
            // If the property has an annotation to ignore null values, we need to omit the property in requests.
            ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse
                ? ODataNullValueBehaviorKind.Default
                : this.Model.NullValueReadBehaviorKind(edmProperty);
            object propertyValue = this.ReadNonEntityValue(
                propertyTypeName,
                edmProperty.Type,
                /*propertyAndAnnotationCollector*/ null,
                /*collectionValidator*/ null,
                nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                /*isTopLevelPropertyValue*/ false,
                /*insideComplexValue*/ false,
                edmProperty.Name);

            if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.IgnoreValue || propertyValue != null)
            {
                AddResourceProperty(resourceState, edmProperty.Name, propertyValue);
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
        }

        /// <summary>
        /// Read an open property.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        /// <returns>The NestedResourceInfo or null.</returns>
        private ODataJsonLightReaderNestedResourceInfo InnerReadUndeclaredProperty(IODataJsonLightReaderResourceState resourceState, IEdmStructuredType owningStructuredType, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue(propertyName));
            }

            object propertyValue = null;
            bool insideComplexValue = false;
            string outerPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);
            string payloadTypeName = TryReadOrPeekPayloadType(resourceState.PropertyAndAnnotationCollector, propertyName, insideComplexValue);
            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ReaderValidationUtils.ResolvePayloadTypeName(
                this.Model,
                null, // expectedTypeReference
                payloadTypeName,
                EdmTypeKind.Complex,
                this.MessageReaderSettings.ClientCustomTypeResolver,
                out payloadTypeKind);
            IEdmTypeReference payloadTypeReference = null;
            if (!string.IsNullOrEmpty(payloadTypeName) && payloadType != null)
            {
                // only try resolving for known type (the below will throw on unknown type name) :
                ODataTypeAnnotation typeAnnotation;
                EdmTypeKind targetTypeKind;
                payloadTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*expectStructuredType*/ null,
                    /*defaultPrimitivePayloadType*/ null,
                    null, // expectedTypeReference
                    payloadTypeName,
                    this.Model,
                    this.GetNonEntityValueKind,
                    out targetTypeKind,
                    out typeAnnotation);
            }

            payloadTypeReference = ResolveUntypedType(
                this.JsonReader.NodeType,
                this.JsonReader.Value,
                payloadTypeName,
                payloadTypeReference,
                this.MessageReaderSettings.PrimitiveTypeResolver,
                this.MessageReaderSettings.ReadUntypedAsString,
                !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

            IEdmStructuredType payloadTypeOrItemType = payloadTypeReference.ToStructuredType();
            if (payloadTypeOrItemType != null)
            {
                // Complex property or collection of complex property.
                bool isCollection = payloadTypeReference.IsCollection();
                ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName);
                ODataJsonLightReaderNestedResourceInfo readerNestedResourceInfo;
                if (isCollection)
                {
                    readerNestedResourceInfo = ReadNonExpandedResourceSetNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }
                else
                {
                    readerNestedResourceInfo = ReadNonExpandedResourceNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }

                return readerNestedResourceInfo;
            }

            if (!(payloadTypeReference is IEdmUntypedTypeReference))
            {
                this.JsonReader.AssertNotBuffering();
                propertyValue = this.ReadNonEntityValue(
                    outerPayloadTypeName,
                    payloadTypeReference,
                    /*propertyAndAnnotationCollector*/ null,
                    /*collectionValidator*/ null,
                    /*validateNullValue*/ true,
                    /*isTopLevelPropertyValue*/ false,
                    /*insideComplexValue*/ false,
                    propertyName,
                    /*isDynamicProperty*/true);
            }
            else
            {
                propertyValue = this.JsonReader.ReadAsUntypedOrNullValue();
            }

            ValidationUtils.ValidateOpenPropertyValue(propertyName, propertyValue);
            AddResourceProperty(resourceState, propertyName, propertyValue);
            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                        this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                        "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");

            return null;
        }

        /// <summary>
        /// Read an undeclared property. That is a property which is not declared by the model, but the owning type is not an open type.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue:  propertyWithValue is true and the reader is positioned on the first node of the property value.
        ///                 JsonNodeType.StartObject:
        ///                 JsonNodeType.StartArray:
        ///                 JsonNodeType.Property:        propertyWithValue is false and the reader is positioned on the node after the property.
        ///                 JsonNodeType.EndObject:
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        /// <returns>A nested resource info instance if the property read is a nested resource info which should be reported to the caller.
        /// Otherwise null if the property was either ignored or read and added to the list of properties on the resource.</returns>
        private ODataJsonLightReaderNestedResourceInfo ReadUndeclaredProperty(IODataJsonLightReaderResourceState resourceState, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
#if DEBUG
            if (propertyWithValue)
            {
                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);
            }
            else
            {
                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }
#endif
            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName);
            object propertyAnnotationValue;

            // If the property has 'odata.navigationLink' or 'odata.associationLink' annotation, read it as a navigation property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataNavigationLinkUrl, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataAssociationLinkUrl, out propertyAnnotationValue))
            {
                // Read it as a deferred link - we never read the expanded content.
                ODataJsonLightReaderNestedResourceInfo navigationLinkInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, /*navigationProperty*/ null);
                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(navigationLinkInfo.NestedResourceInfo);

                // If the property is expanded, ignore the content if we're asked to do so.
                if (propertyWithValue)
                {
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, null, propertyName);

                    // Since we marked the nested resource info as deferred the reader will not try to read its content
                    // instead it will behave as if it was a real deferred link (without a property value).
                    // So skip the value here to move to the next property in the payload, which will look exactly the same
                    // as if the nested resource info was deferred.
                    this.JsonReader.SkipValue();
                }

                return navigationLinkInfo;
            }

            // If the property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' annotation, read it as a stream property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaEditLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaReadLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaContentType, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaETag, out propertyAnnotationValue))
            {
                // Stream properties can't have a value
                if (propertyWithValue)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_StreamPropertyWithValue(propertyName));
                }

                ODataStreamReferenceValue streamPropertyValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                AddResourceProperty(resourceState, propertyName, streamPropertyValue);
                return null;
            }

            if (resourceState.ResourceType.IsOpen)
            {
                // Open property - read it as such.
                ODataJsonLightReaderNestedResourceInfo nestedResourceInfo =
                    this.InnerReadUndeclaredProperty(resourceState, resourceState.ResourceType, propertyName, propertyWithValue);
                return nestedResourceInfo;
            }

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType(propertyName));
            }

            // Validate that the property doesn't have unrecognized annotations
            // We ignore the type name since we might not have the full model and thus might not be able to resolve it correctly.
            ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);

            if (!this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
            {
                bool isTopLevelPropertyValue = false;
                ODataJsonLightReaderNestedResourceInfo nestedResourceInfo =
                    this.InnerReadUndeclaredProperty(resourceState, propertyName, isTopLevelPropertyValue);
                return nestedResourceInfo;
            }
            else
            {
                Debug.Assert(
                    this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType,
                    "this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType");
            }

            return null;
        }

        /// <summary>
        /// Reads a stream property value from the property annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="streamPropertyName">The name of the stream property to read the value for.</param>
        /// <returns>The newly created stream reference value.</returns>
        private ODataStreamReferenceValue ReadStreamPropertyValue(IODataJsonLightReaderResourceState resourceState, string streamPropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(streamPropertyName), "!string.IsNullOrEmpty(streamPropertyName)");

            // Fail on stream properties in requests as they cannot appear there.
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_StreamPropertyInRequest);
            }

            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();

            foreach (var propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(streamPropertyName))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataMediaEditLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaEditLink annotation should have been parsed as a non-null Uri.");
                        streamReferenceValue.EditLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaReadLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaReadLink annotation should have been parsed as a non-null Uri.");
                        streamReferenceValue.ReadLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaETag:
                        Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaEtag annotation should have been parsed as a non-null string.");
                        streamReferenceValue.ETag = (string)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaContentType:
                        Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaContentType annotation should have been parsed as a non-null string.");
                        streamReferenceValue.ContentType = (string)propertyAnnotation.Value;
                        break;

                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation(streamPropertyName, propertyAnnotation.Key));
                }
            }

            ODataResourceMetadataBuilder builder =
                this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                    this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment);

            // Note that we set the metadata builder even when streamProperty is null, which is the case when the stream property is undeclared.
            // For undeclared stream properties, we will apply conventional metadata evaluation just as declared stream properties.
            streamReferenceValue.SetMetadataBuilder(builder, streamPropertyName);

            return streamReferenceValue;
        }

        /// <summary>
        /// Reads one operation for the resource being read.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:   first node of the operation value.
        /// Post-Condition: JsonNodeType.Property:      the property after the current operation being read when there is one target for the operation.
        ///                 JsonNodeType.StartObject:   the first node of the next operation value when there are multiple targets for the operation.
        ///                 JsonNodeType.EndArray:      the end-array of the operation values when there are multiple target for the operation.
        /// </remarks>
        private void ReadSingleOperationValue(IODataJsonOperationsDeserializerContext readerContext, IODataJsonLightReaderResourceState resourceState, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(readerContext != null, "readerContext != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonLightReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(metadataReferencePropertyName, readerContext.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            readerContext.JsonReader.ReadStartObject();

            var operation = this.CreateODataOperationAndAddToEntry(readerContext, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
                {
                    readerContext.JsonReader.ReadPropertyName();
                    readerContext.JsonReader.SkipValue();
                }

                readerContext.JsonReader.ReadEndObject();
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(readerContext.JsonReader.ReadPropertyName());
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = readerContext.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // skip over all unknown properties and read the next property or
                        // the end of the metadata for the current propertyName
                        readerContext.JsonReader.SkipValue();
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_OperationMissingTargetProperty(metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            readerContext.JsonReader.ReadEndObject();

            // Sets the metadata builder to evaluate by convention any operation property that's not on the wire.
            // Note we must only set this after the operation is read from the wire since we lose the ability to tell
            // what was on the wire and what is being dynamically computed.
            this.SetMetadataBuilder(resourceState, operation);
        }

        /// <summary>
        /// Reads one operation for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        private void ReadSingleOperationValue(ODataResourceSet resourceSet, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonLightUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonLightReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(metadataReferencePropertyName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            this.JsonReader.ReadStartObject();

            var operation = this.CreateODataOperationAndAddToResourceSet(resourceSet, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    this.JsonReader.ReadPropertyName();
                    this.JsonReader.SkipValue();
                }

                this.JsonReader.ReadEndObject();
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(this.JsonReader.ReadPropertyName());
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = this.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // skip over all unknown properties and read the next property or
                        // the end of the metadata for the current propertyName
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_OperationMissingTargetProperty(metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            this.JsonReader.ReadEndObject();
        }

        /// <summary>
        /// Sets the metadata builder for the operation.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="operation">The operation to set the metadata builder on.</param>
        private void SetMetadataBuilder(IODataJsonLightReaderResourceState resourceState, ODataOperation operation)
        {
            ODataResourceMetadataBuilder builder =
                this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                    this.JsonLightInputContext.ODataSimplifiedOptions.EnableReadingKeyAsSegment);
            operation.SetMetadataBuilder(builder, this.ContextUriParseResult.MetadataDocumentUri);
        }

        /// <summary>
        /// Creates a new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.</returns>
        private ODataOperation CreateODataOperationAndAddToEntry(IODataJsonOperationsDeserializerContext readerContext, string metadataReferencePropertyName)
        {
            string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonLightInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            var operation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

            if (isAction)
            {
                readerContext.AddActionToResource((ODataAction)operation);
            }
            else
            {
                readerContext.AddFunctionToResource((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Creates a new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.
        /// </summary>
        /// <param name="resourceSet">The resource set to add the action or function .</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A new instance of ODataAction or ODataFunction for the <paramref name="metadataReferencePropertyName"/>.</returns>
        private ODataOperation CreateODataOperationAndAddToResourceSet(ODataResourceSet resourceSet, string metadataReferencePropertyName)
        {
            string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonLightInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            var operation = ODataJsonLightUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

            if (isAction)
            {
                resourceSet.AddAction((ODataAction)operation);
            }
            else
            {
                resourceSet.AddFunction((ODataFunction)operation);
            }

            return operation;
        }

        /// <summary>
        /// Read the metadata reference property value for the resource being read.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:      first node of the metadata reference property's value. Currently
        ///                                             actions and functions are the only supported metadata reference property,
        ///                                             we will throw if this is not a start object or start array node.
        /// Post-Condition: JsonNodeType.Property:      the property after the annotation value
        ///                 JsonNodeType.EndObject:     the end-object of the resource
        /// </remarks>
        private void ReadMetadataReferencePropertyValue(IODataJsonLightReaderResourceState resourceState, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.Resource != null, "resourceState.Resource != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) > -1, "metadataReferencePropertyName.IndexOf(JsonLightConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            IODataJsonOperationsDeserializerContext readerContext = new OperationsDeserializerContext(resourceState.Resource, this);

            bool insideArray = false;
            if (readerContext.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                readerContext.JsonReader.ReadStartArray();
                insideArray = true;
            }

            do
            {
                this.ReadSingleOperationValue(readerContext, resourceState, metadataReferencePropertyName, insideArray);
            }
            while (insideArray && readerContext.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                readerContext.JsonReader.ReadEndArray();
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Read the metadata reference property value for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        private void ReadMetadataReferencePropertyValue(ODataResourceSet resourceSet, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) > -1, "metadataReferencePropertyName.IndexOf(JsonLightConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            bool insideArray = false;
            if (this.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                this.JsonReader.ReadStartArray();
                insideArray = true;
            }

            do
            {
                this.ReadSingleOperationValue(resourceSet, metadataReferencePropertyName, insideArray);
            }
            while (insideArray && this.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                this.JsonReader.ReadEndArray();
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Validates that we can read metadata reference property.
        /// </summary>
        private void ValidateCanReadMetadataReferenceProperty()
        {
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest);
            }
        }

        /// <summary>
        /// Operations deserializer context to pass to JSON operations reader.
        /// </summary>
        private sealed class OperationsDeserializerContext : IODataJsonOperationsDeserializerContext
        {
            /// <summary>
            /// The resource to add operations to.
            /// </summary>
            private ODataResourceBase resource;

            /// <summary>
            /// The deserializer to use.
            /// </summary>
            private ODataJsonLightResourceDeserializer jsonLightResourceDeserializer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="resource">The resource to add operations to.</param>
            /// <param name="jsonLightResourceDeserializer">The deserializer to use.</param>
            public OperationsDeserializerContext(ODataResourceBase resource, ODataJsonLightResourceDeserializer jsonLightResourceDeserializer)
            {
                Debug.Assert(resource != null, "resource != null");
                Debug.Assert(jsonLightResourceDeserializer != null, "jsonLightResourceDeserializer != null");

                this.resource = resource;
                this.jsonLightResourceDeserializer = jsonLightResourceDeserializer;
            }

            /// <summary>
            /// The JSON reader to read the operations value from.
            /// </summary>
            public IJsonReader JsonReader
            {
                get
                {
                    return this.jsonLightResourceDeserializer.JsonReader;
                }
            }

            /// <summary>
            /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
            /// </summary>
            /// <param name="uriFromPayload">The URI string from the payload to process.</param>
            /// <returns>An absolute URI to report.</returns>
            public Uri ProcessUriFromPayload(string uriFromPayload)
            {
                return this.jsonLightResourceDeserializer.ProcessUriFromPayload(uriFromPayload);
            }

            /// <summary>
            /// Adds the specified action to the current resource.
            /// </summary>
            /// <param name="action">The action whcih is fully populated with the data from the payload.</param>
            public void AddActionToResource(ODataAction action)
            {
                Debug.Assert(action != null, "action != null");
                this.resource.AddAction(action);
            }

            /// <summary>
            /// Adds the specified function to the current resource.
            /// </summary>
            /// <param name="function">The function whcih is fully populated with the data from the payload.</param>
            public void AddFunctionToResource(ODataFunction function)
            {
                Debug.Assert(function != null, "function != null");
                this.resource.AddFunction(function);
            }
        }
    }
}
