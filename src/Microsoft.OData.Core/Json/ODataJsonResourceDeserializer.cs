﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Vocabularies.V1;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Core;

    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for entries and resource sets.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
    internal sealed class ODataJsonResourceDeserializer : ODataJsonPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonResourceDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
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
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadResourceSetContentStart, this.JsonReader.NodeType));
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
        internal void ReadResourceTypeName(IODataJsonReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.type property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.Equals(ODataJsonConstants.PrefixedODataTypePropertyName, propertyName, StringComparison.Ordinal)
                    || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataTypePropertyName, propertyName))
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
        /// <returns>The <see cref="ODataDeletedResource"/> that was read.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method Creates an ODataDeltaDeletedEntry and fills in the Id and Reason properties, if specified in the payload.
        /// </remarks>
        internal ODataDeletedResource ReadDeletedResource()
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataDeletedResource deletedResource = null;

            // If the current node is the deleted property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.GetPropertyName();
                if (string.Equals(ODataJsonConstants.PrefixedODataRemovedPropertyName, propertyName, StringComparison.Ordinal)
                    || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataRemovedPropertyName, propertyName))
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
                                string.Equals(ODataJsonConstants.ODataReasonPropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
                            {
                                // Read over the property to move to its value.
                                this.JsonReader.Read();

                                // Read the reason value.
                                if (string.Equals(ODataJsonConstants.ODataReasonDeletedValue, this.JsonReader.ReadStringValue(), StringComparison.Ordinal))
                                {
                                    reason = DeltaDeletedEntryReason.Deleted;
                                }
                            }
                        }
                    }
                    else
                    {
                        object value = this.JsonReader.GetValue();
                        if (value != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_DeltaRemovedAnnotationMustBeObject, value));
                        }
                    }

                    // read over end object or null value
                    this.JsonReader.Read();

                    // A deleted object must have at least either the odata id annotation or the key values
                    if (this.JsonReader.NodeType != JsonNodeType.Property)
                    {
                        throw new ODataException(SRResources.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
                    }

                    // If the next property is the id property - read it.
                    propertyName = this.JsonReader.GetPropertyName();
                    if (string.Equals(ODataJsonConstants.PrefixedODataIdPropertyName, propertyName, StringComparison.Ordinal)
                        || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataIdPropertyName, propertyName))
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

            while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.NodeType != JsonNodeType.EndOfInput)
            {
                if (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    // If the current node is the id property - read it.
                    if (string.Equals(ODataJsonConstants.ODataIdPropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
                    {
                        // Read over the property to move to its value.
                        this.JsonReader.Read();

                        // Read the Id value.
                        id = this.JsonReader.ReadUriValue();
                        Debug.Assert(id != null, "value for Id must be provided");

                        continue;
                    }
                    // If the current node is the reason property - read it.
                    else if (string.Equals(ODataJsonConstants.ODataReasonPropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
                    {
                        // Read over the property to move to its value.
                        this.JsonReader.Read();

                        // Read the reason value.
                        if (string.Equals(ODataJsonConstants.ODataReasonDeletedValue, this.JsonReader.ReadStringValue(), StringComparison.Ordinal))
                        {
                            reason = DeltaDeletedEntryReason.Deleted;
                        }

                        continue;
                    }
                }

                this.JsonReader.Read();

                if (this.JsonReader.NodeType == JsonNodeType.StartObject || this.JsonReader.NodeType == JsonNodeType.StartArray)
                {
                    throw new ODataException(SRResources.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }

                // Ignore unknown primitive properties in a 4.0 deleted entry
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
                string.Equals(ODataJsonConstants.ODataSourcePropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
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
                string.Equals(ODataJsonConstants.ODataRelationshipPropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
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
                string.Equals(ODataJsonConstants.ODataTargetPropertyName, this.JsonReader.GetPropertyName(), StringComparison.Ordinal))
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
        internal ODataJsonReaderNestedInfo ReadResourceContent(IODataJsonReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.ResourceType != null && this.Model.IsUserModel(), "A non-null resource type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataJsonReaderNestedInfo readerNestedResourceInfo = null;
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
                                this.ReadOverPropertyName();
                                object value = ReadODataOrCustomInstanceAnnotationValue(resourceState, propertyParsingResult, propertyName);
                                this.ApplyEntryInstanceAnnotation(resourceState, propertyName, value);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithoutValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.NestedDeltaResourceSet:
                                // Will read over property name in ReadPropertyWithValue
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, /*isDeltaResourceSet*/ true);
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                // Will read over property name in ReadPropertyWithValue
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = this.ReadPropertyWithValue(resourceState, propertyName, /*isDeltaResourceSet*/ false);
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                this.ReadOverPropertyName();
                                this.ReadMetadataReferencePropertyValue(resourceState, propertyName);
                                break;

                            case PropertyParsingResult.EndOfObject:
                                this.ReadOverPropertyName();
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
            //  - PrimitiveValue- if it's a stream or an expanded null resource
            //  - EndObject - end of the resource
            Debug.Assert(
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.Property ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                readerNestedResourceInfo is ODataJsonReaderNestedInfo ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Property or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="resourceState">The IODataJsonReaderResourceState.</param>
        /// <param name="propertyParsingResult">The PropertyParsingResult.</param>
        /// <param name="annotationName">The annotation name</param>
        /// <returns>The annotation value.</returns>
        internal object ReadODataOrCustomInstanceAnnotationValue(IODataJsonReaderResourceState resourceState, PropertyParsingResult propertyParsingResult, string annotationName)
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
        internal void ValidateMediaEntity(IODataJsonReaderResourceState resourceState)
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
                        ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
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
                            this.ReadOverPropertyName();
                            switch (propertyParseResult)
                            {
                                case PropertyParsingResult.ODataInstanceAnnotation:
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    ReadODataOrCustomInstanceAnnotationValue(resourceSet, propertyAndAnnotationCollector,
                                        forResourceSetStart, readAllResourceSetProperties, propertyParseResult, propertyName);
                                    break;

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
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
                                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_InvalidPropertyInTopLevelResourceSet, propertyName, ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    break;
                                case PropertyParsingResult.PropertyWithoutValue:
                                    // If we find a property without a value it means that we did not find the resource set property (yet)
                                    // but an invalid property annotation
                                    throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet, propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    if (!(resourceSet is ODataResourceSet))
                                    {
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                                    }

                                    this.ReadMetadataReferencePropertyValue((ODataResourceSet)resourceSet, propertyName);
                                    break;

                                default:
                                    throw new ODataException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.ODataJsonResourceDeserializer_ReadTopLevelResourceSetAnnotations));
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
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_ExpectedResourceSetPropertyNotFound, ODataJsonConstants.ODataValuePropertyName));
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
                propertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, this.JsonReader.GetValue());
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
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
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
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_EmptyBindArray, ODataAnnotationNames.ODataBind));
                    }

                    return entityReferenceLinks;

                case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                default:
                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyAnnotationName));
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
                    throw new ODataException(SRResources.ODataJsonResourceDeserializer_ResourceTypeAnnotationNotFirst);

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty, annotationName));
                    }

                    return this.ReadAnnotationStringValueAsUri(annotationName);

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty, annotationName));
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

                case ODataAnnotationNames.ODataRemoved: // 'odata.removed'
                    {
                        throw new ODataException(SRResources.ODataJsonResourceDeserializer_UnexpectedDeletedEntryInResponsePayload);
                    }

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
        internal void ApplyEntryInstanceAnnotation(IODataJsonReaderResourceState resourceState, string annotationName, object annotationValue)
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
                    ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.EditLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ReadLink = (Uri)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                    ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
                    mediaResource.ContentType = (string)annotationValue;
                    break;

                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    ODataJsonReaderUtils.EnsureInstance(ref mediaResource);
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
        internal ODataJsonReaderNestedInfo ReadPropertyWithoutValue(IODataJsonReaderResourceState resourceState, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataJsonReaderNestedInfo readerNestedInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
            if (edmProperty == null || edmProperty.Type.IsUntyped())
            {
                // Undeclared property - we need to run detection algorithm here.
                readerNestedInfo = this.ReadUndeclaredProperty(resourceState, propertyName, propertyWithValue: false);

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
                return readerNestedInfo;
            }

            // Declared property - read it.
            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo;
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
                        ? ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: false)
                        : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: false);

                    if (!readerNestedResourceInfo.HasEntityReferenceLink)
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink, propertyName, ODataAnnotationNames.ODataBind));
                    }
                }

                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                readerNestedInfo = readerNestedResourceInfo;
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
                    readerNestedInfo = ReadNestedPropertyInfoWithoutValue(resourceState, edmProperty.Name, edmProperty);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return readerNestedInfo;
        }

        /// <summary>
        /// Reads any next link annotation immediately after the end of a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set being read.</param>
        /// <param name="expandedNestedResourceInfo">The information about the expanded link. This must be non-null if we're reading an expanded resource set, and must be null if we're reading a top-level resource set.</param>
        /// <param name="propertyAndAnnotationCollector">The top-level duplicate property names checker, if we're reading a top-level resource set.</param>
        internal void ReadNextLinkAnnotationAtResourceSetEnd(
            ODataResourceSetBase resourceSet,
            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo,
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
        private static ODataJsonReaderNestedResourceInfo ReadDeferredNestedResourceInfo(IODataJsonReaderResourceState resourceState, string navigationPropertyName, IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(navigationPropertyName), "!string.IsNullOrEmpty(navigationPropertyName)");
            Debug.Assert(navigationProperty == null || navigationPropertyName == navigationProperty.Name, "navigationProperty == null || navigationPropertyName == navigationProperty.Name");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = navigationPropertyName,
                IsCollection = navigationProperty == null ? null : (bool?)navigationProperty.Type.IsCollection()
            };

            foreach (KeyValuePair<string, object> propertyAnnotation
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

                    case ODataAnnotationNames.ODataType:
                        Debug.Assert(propertyAnnotation.Value is String && propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a non-null string.");
                        nestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation((string)propertyAnnotation.Value);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                        nestedResourceInfo.Count = (long?)propertyAnnotation.Value;
                        break;

                    // TODO: do we support odata.context uri here? why?
                    case ODataAnnotationNames.ODataContext:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.ContextUrl = (Uri)propertyAnnotation.Value;
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation, nestedResourceInfo.Name, propertyAnnotation.Key));
                }
            }

            return ODataJsonReaderNestedResourceInfo.CreateDeferredLinkInfo(nestedResourceInfo, navigationProperty);
        }

        /// <summary>
        /// We fail here if we encounter any other property annotation for the expanded navigation (since these should come before the property itself).
        /// </summary>
        /// <param name="resourceSet">The resource set that was just read.</param>
        /// <param name="expandedNestedResourceInfo">The information for the current expanded nested resource info being read.</param>
        private void ReadExpandedResourceSetAnnotationsAtResourceSetEnd(ODataResourceSetBase resourceSet, ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo)
        {
            Debug.Assert(expandedNestedResourceInfo != null, "expandedNestedResourceInfo != null");
            Debug.Assert(expandedNestedResourceInfo.NestedResourceInfo.IsCollection == true, "Only collection navigation properties can have resourceSet content.");

            // Look at the next property in the owning resource, if it's a property annotation for the expanded nested resource info property, read it.
            string propertyName, annotationName;
            while (this.JsonReader.NodeType == JsonNodeType.Property &&
                   TryParsePropertyAnnotation(this.JsonReader.GetPropertyName(), out propertyName, out annotationName) &&
                   string.Equals(propertyName, expandedNestedResourceInfo.NestedResourceInfo.Name, StringComparison.Ordinal))
            {
                if (!this.ReadingResponse)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedPropertyAnnotation, propertyName, annotationName));
                }

                // Read over the property name.
                this.JsonReader.Read();

                switch (this.CompleteSimplifiedODataAnnotation(annotationName))
                {
                    case ODataAnnotationNames.ODataNextLink:
                        if (resourceSet.NextPageLink != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation, ODataAnnotationNames.ODataNextLink, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.NextPageLink = this.ReadAndValidateAnnotationStringValueAsUri(ODataAnnotationNames.ODataNextLink);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        if (resourceSet.Count != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation, ODataAnnotationNames.ODataCount, expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.Count = this.ReadAndValidateAnnotationAsLongForIeee754Compatible(ODataAnnotationNames.ODataCount);
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet, annotationName, expandedNestedResourceInfo.NestedResourceInfo.Name));
                }
            }
        }

        /// <summary>
        /// Sets specified media resource on a resource and hooks up metadata builder.
        /// </summary>
        /// <param name="resourceState">The resource state to use.</param>
        /// <param name="mediaResource">The media resource to set.</param>
        private void SetEntryMediaResource(IODataJsonReaderResourceState resourceState, ODataStreamReferenceValue mediaResource)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(mediaResource != null, "mediaResource != null");
            ODataResourceBase resource = resourceState.Resource;
            Debug.Assert(resource != null, "resource != null");

            ODataResourceMetadataBuilder builder =
                this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                    this.JsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                    /*isDelta*/ false);
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
        private ODataJsonReaderNestedInfo ReadPropertyWithValue(IODataJsonReaderResourceState resourceState, string propertyName, bool isDeltaResourceSet)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property, JsonNodeType.StartObject, JsonNodeType.StartArray);

            ODataJsonReaderNestedInfo readerNestedInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
            bool isCollection = edmProperty == null ? false : edmProperty.Type.IsCollection();
            IEdmStructuralProperty structuralProperty = edmProperty as IEdmStructuralProperty;

            if (structuralProperty != null)
            {
                ODataJsonReaderNestedInfo nestedInfo = TryReadAsStream(resourceState, structuralProperty, structuralProperty.Type, structuralProperty.Name);
                if (nestedInfo != null)
                {
                    return nestedInfo;
                }
            }

            if (edmProperty != null && !edmProperty.Type.IsUntyped())
            {
                this.ReadOverPropertyName();
                IEdmStructuredType structuredPropertyTypeOrItemType = structuralProperty == null ? null : structuralProperty.Type.ToStructuredType();
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (structuredPropertyTypeOrItemType != null)
                {
                    ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                    // Complex property or collection of complex property.
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName, edmProperty.Type);

                    if (isCollection)
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceSetNestedResourceInfo(resourceState, structuralProperty, structuredPropertyTypeOrItemType, structuralProperty.Name);
                    }
                    else
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceNestedResourceInfo(resourceState, structuralProperty, structuredPropertyTypeOrItemType, structuralProperty.Name);
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                    readerNestedInfo = readerNestedResourceInfo;
                }
                else if (navigationProperty != null)
                {
                    ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                    // Expanded link
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName, edmProperty.Type);
                    if (isCollection)
                    {
                        readerNestedResourceInfo = this.ReadingResponse || isDeltaResourceSet
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
                    readerNestedInfo = readerNestedResourceInfo;
                }
                else
                {
                    IEnumerable<string> derivedTypeConstraints = this.JsonInputContext.Model.GetDerivedTypeConstraints(edmProperty);
                    if (derivedTypeConstraints != null)
                    {
                        resourceState.PropertyAndAnnotationCollector.SetDerivedTypeValidator(propertyName, new DerivedTypeValidator(edmProperty.Type.Definition, derivedTypeConstraints, "property", propertyName));
                    }

                    // NOTE: we currently do not check whether the property should be skipped
                    //       here because this can only happen for navigation properties and open properties.
                    this.ReadEntryDataProperty(resourceState, edmProperty, ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName));
                }
            }
            else
            {
                // Undeclared property - we need to run detection algorithm here.
                readerNestedInfo = this.ReadUndeclaredProperty(resourceState, propertyName, /*propertyWithValue*/ true);

                // Note that if nested resource info is returned it's already validated, so we just report it here.
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue);
            return readerNestedInfo;
        }

        /// <summary>
        /// Checks to see if the current property should be read as a stream and, if so reads it
        /// </summary>
        /// <param name="resourceState">current ResourceState</param>
        /// <param name="property">The property being serialized (null for a dynamic property)</param>
        /// <param name="propertyType">The type of the property being serialized</param>
        /// <param name="propertyName">The name of the property being serialized</param>
        /// <returns>The ODataJsonReaderNestedInfo for a nested stream property, or null if the property shouldn't be streamed</returns>
        private ODataJsonReaderNestedInfo TryReadAsStream(IODataJsonReaderResourceState resourceState, IEdmStructuralProperty property, IEdmTypeReference propertyType, string propertyName)
        {
            Debug.Assert(propertyName != null, "Property name must not be null");

            bool isCollection = false;
            IEdmPrimitiveType primitiveType = null;
            if (propertyType != null)
            {
                primitiveType = propertyType.Definition.AsElementType() as IEdmPrimitiveType;
                isCollection = propertyType.IsCollection();
            }
            else
            {
                isCollection = this.JsonReader.NodeType != JsonNodeType.PrimitiveValue;
            }

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStream = this.MessageReaderSettings.ReadAsStreamFunc;

            // is the property a stream or a stream collection,
            // untyped collection,
            // or a binary or binary collection the client wants to read as a stream...
            if (
                (primitiveType != null &&
                    (primitiveType.IsStream() ||
                        (readAsStream != null
                             && (property == null || !property.IsKey())  // don't stream key properties
                             && (primitiveType.IsBinary() || primitiveType.IsString() || isCollection))
                         && readAsStream(primitiveType, isCollection, propertyName, property))) ||
                (propertyType != null &&
                    isCollection &&
                    propertyType.Definition.AsElementType().IsUntyped()) ||
                (propertyType == null
                    && (isCollection || this.JsonReader.CanStream())
                    && readAsStream != null
                    && readAsStream(null, isCollection, propertyName, property)))
            {
                if (isCollection)
                {
                    this.ReadOverPropertyName();
                    IEdmType elementType = propertyType == null ? EdmCoreModel.Instance.GetUntypedType() : propertyType.Definition.AsElementType();

                    // Collection of streams, or binary/string values to read as streams
                    return ReadStreamCollectionNestedResourceInfo(resourceState, property, propertyName, elementType);
                }
                else
                {
                    ODataPropertyInfo propertyInfo;
                    if (primitiveType != null && primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream)
                    {
                        ODataStreamPropertyInfo streamPropertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);

                        // If it has an instance annotation saying that the content type is JSON, don't read propertyName
                        // otherwise, if we are on start object, BufferingJsonReader will read ahead to try and determine
                        // if we are reading an instream error, which destroys our ability to stream json stream values.
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            bool isJson = false;
                            if (streamPropertyInfo.ContentType != null)
                            {
                                if (streamPropertyInfo.ContentType.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal))
                                {
                                    isJson = true;
                                }
                            }
                            else if (property != null)
                            {
                                IEdmVocabularyAnnotation mediaTypeAnnotation = property.VocabularyAnnotations(this.Model).FirstOrDefault(a => a.Term == CoreVocabularyModel.MediaTypeTerm);
                                if (mediaTypeAnnotation != null)
                                {
                                    // If the property does not have a mediaType annotation specifying application/json, then read over the property name
                                    IEdmStringConstantExpression stringExpression = mediaTypeAnnotation.Value as IEdmStringConstantExpression;
                                    if (stringExpression != null && stringExpression.Value.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal))
                                    {
                                        isJson = true;
                                    }
                                }
                            }

                            if (!isJson)
                            {
                                // Not reading JSON stream, so read over property name
                                this.ReadOverPropertyName();
                            }
                        }

                        // Add the stream reference property
                        ODataStreamReferenceValue streamReferenceValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                        AddResourceProperty(resourceState, propertyName, streamReferenceValue);

                        propertyInfo = streamPropertyInfo;
                    }
                    else
                    {
                        this.ReadOverPropertyName();

                        propertyInfo = new ODataPropertyInfo
                        {
                            PrimitiveTypeKind = primitiveType == null ? EdmPrimitiveTypeKind.None : primitiveType.PrimitiveKind,
                            Name = propertyName,
                        };
                    }

                    // return without reading over the property node; we will create a stream over the value
                    this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property);
                    return new ODataJsonReaderNestedPropertyInfo(propertyInfo, property);
                }
            }

            return null;
        }

        /// <summary>
        /// Reads over the current property name if positioned on a property
        /// </summary>
        private void ReadOverPropertyName()
        {
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.JsonReader.Read();
            }
        }

        /// <summary>
        /// Read a resource-level data property without value.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The property name, it must not be null.</param>
        /// <param name="property">The property itself, it could be null if it's dynamic property.</param>
        /// <returns>The NestedResourceInfo or null.</returns>
        private static ODataJsonReaderNestedPropertyInfo ReadNestedPropertyInfoWithoutValue(IODataJsonReaderResourceState resourceState,
            string propertyName, IEdmProperty property)
        {
            Debug.Assert(resourceState != null, "resourceState must not be null");
            Debug.Assert(propertyName != null, "Property name must not be null");

            IEdmTypeReference propertyType = property?.Type;
            IEdmPrimitiveType primitiveType = propertyType == null ? null : propertyType.Definition.AsElementType() as IEdmPrimitiveType;
            ODataPropertyInfo propertyInfo = new ODataPropertyInfo
            {
                PrimitiveTypeKind = primitiveType == null ? EdmPrimitiveTypeKind.None : primitiveType.PrimitiveKind,
                Name = propertyName,
            };

            if (primitiveType != null)
            {
                AddResourceProperty(resourceState, propertyName, propertyInfo);
            }
            else
            {
                // Complex and complex collection property
                AttachODataAnnotations(resourceState.PropertyAndAnnotationCollector, propertyName, propertyInfo);
                AttachCustomAnnotations(resourceState.PropertyAndAnnotationCollector, propertyName, propertyInfo);

                resourceState.PropertyAndAnnotationCollector.CheckForDuplicatePropertyNames(propertyInfo);
            }

            return new ODataJsonReaderNestedPropertyInfo(propertyInfo, property, false);
        }

        /// <summary>
        /// Returns whether or not a StreamPropertyInfo value specifies a content-type of application/json.
        /// </summary>
        /// <param name="streamPropertyInfo">The StreamPropertyInfo that may specify application/json.</param>
        /// <returns>True, if the StreamPropertyInfo specifies application/json, otherwise false.</returns>
        private static bool IsJsonStream(ODataStreamPropertyInfo streamPropertyInfo)
        {
            return streamPropertyInfo.ContentType != null && streamPropertyInfo.ContentType.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal);
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
        private void ReadEntryDataProperty(IODataJsonReaderResourceState resourceState, IEdmProperty edmProperty, string propertyTypeName)
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
                /*insideResourceValue*/ false,
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
        private ODataJsonReaderNestedInfo InnerReadUndeclaredProperty(IODataJsonReaderResourceState resourceState, IEdmStructuredType owningStructuredType, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            if (!propertyWithValue)
            {
                return ReadNestedPropertyInfoWithoutValue(resourceState, propertyName, null);
            }

            object propertyValue = null;
            bool insideResourceValue = false;
            string outerPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);
            string payloadTypeName = TryReadOrPeekPayloadType(resourceState.PropertyAndAnnotationCollector, propertyName, insideResourceValue);
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

            ODataJsonReaderNestedInfo nestedResourceInfo = TryReadAsStream(resourceState, null, payloadTypeReference, propertyName);
            if (nestedResourceInfo != null)
            {
                return nestedResourceInfo;
            }

            payloadTypeReference = ResolveUntypedType(
                this.JsonReader.NodeType,
                this.JsonReader.GetValue(),
                payloadTypeName,
                payloadTypeReference,
                this.MessageReaderSettings.PrimitiveTypeResolver,
                this.MessageReaderSettings.ReadUntypedAsString,
                !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

            bool isCollection = payloadTypeReference.IsCollection();
            IEdmStructuredType payloadTypeOrItemType = payloadTypeReference.ToStructuredType();
            if (payloadTypeOrItemType != null)
            {
                // Complex property or collection of complex property.
                ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName, payloadTypeReference);
                if (isCollection)
                {
                    return ReadNonExpandedResourceSetNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }
                else
                {
                    return ReadNonExpandedResourceNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }
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
                    /*insideResourceValue*/ false,
                    propertyName,
                    /*isDynamicProperty*/true);
            }
            else
            {
                if (base.MessageReaderSettings.EnableUntypedCollections && this.JsonReader.NodeType == JsonNodeType.StartArray)
                {
                    propertyValue = new ODataCollectionValue()
                    {
                        TypeAnnotation = new ODataTypeAnnotation(EdmUntypedStructuredType.Instance.FullName, EdmUntypedStructuredType.Instance),
                        TypeName = EdmUntypedStructuredType.Instance.FullName,
                        Items = this.JsonReader.ReadUntypedCollectionValues().ToList(),
                    };
                }
                else
                {
                    propertyValue = this.JsonReader.ReadAsUntypedOrNullValue();
                }
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
        private ODataJsonReaderNestedInfo ReadUndeclaredProperty(IODataJsonReaderResourceState resourceState, string propertyName, bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
#if DEBUG
            if (propertyWithValue)
            {
                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property, JsonNodeType.StartObject, JsonNodeType.StartArray);
            }
            else
            {
                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }
#endif
            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            IDictionary<string, object> odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName);
            object propertyAnnotationValue;

            // If the property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' annotation, read it as a stream property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaEditLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaReadLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaContentType, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaETag, out propertyAnnotationValue))
            {
                // Add the stream reference property
                ODataStreamReferenceValue streamReferenceValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                AddResourceProperty(resourceState, propertyName, streamReferenceValue);

                if (propertyWithValue)
                {
                    ODataStreamPropertyInfo propertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);
                    if (!IsJsonStream(propertyInfo))
                    {
                        // Not a JSON Stream, so skip over property name in JSON reader
                        this.JsonReader.Read();
                    }

                    return new ODataJsonReaderNestedPropertyInfo(propertyInfo, null);
                }

                return null;
            }

            // It's not a JSON stream, so skip the property name.
            // If the property does not have a value we will have already skipped the name
            if (propertyWithValue)
            {
                this.JsonReader.Read();
            }

            // If the property has 'odata.navigationLink' or 'odata.associationLink' annotation, read it as a navigation property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataNavigationLinkUrl, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataAssociationLinkUrl, out propertyAnnotationValue))
            {
                // Read it as a deferred link - we never read the expanded content.
                ODataJsonReaderNestedResourceInfo navigationLinkInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, /*navigationProperty*/ null);
                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(navigationLinkInfo.NestedResourceInfo);

                // If the property is expanded, ignore the content if we're asked to do so.
                if (propertyWithValue)
                {
                    ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, null, propertyName, resourceState.ResourceType.ToTypeReference());

                    // Since we marked the nested resource info as deferred the reader will not try to read its content
                    // instead it will behave as if it was a real deferred link (without a property value).
                    // So skip the value here to move to the next property in the payload, which will look exactly the same
                    // as if the nested resource info was deferred.
                    this.JsonReader.SkipValue();
                }

                return navigationLinkInfo;
            }

            if (resourceState.ResourceType.IsOpen)
            {
                // Open property - read it as such.
                ODataJsonReaderNestedInfo nestedResourceInfo =
                    this.InnerReadUndeclaredProperty(resourceState, resourceState.ResourceType, propertyName, propertyWithValue);
                return nestedResourceInfo;
            }

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_PropertyWithoutValueWithUnknownType, propertyName));
            }

            // Validate that the property doesn't have unrecognized annotations
            // We ignore the type name since we might not have the full model and thus might not be able to resolve it correctly.
            ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);

            if (!this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
            {
                bool isTopLevelPropertyValue = false;
                ODataJsonReaderNestedResourceInfo nestedResourceInfo =
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
        private ODataStreamReferenceValue ReadStreamPropertyValue(IODataJsonReaderResourceState resourceState, string streamPropertyName)
        {
            ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue();
            ReadStreamInfo(streamReferenceValue, resourceState, streamPropertyName);
            ODataResourceMetadataBuilder builder =
            this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                this.JsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                /*isDelta*/ false);

            // Note that we set the metadata builder even when streamProperty is null, which is the case when the stream property is undeclared.
            // For undeclared stream properties, we will apply conventional metadata evaluation just as declared stream properties.
            streamReferenceValue.SetMetadataBuilder(builder, streamPropertyName);

            return streamReferenceValue;
        }

        /// <summary>
        /// Reads a stream property info from the property annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="streamPropertyName">The name of the stream property to read the value for.</param>
        /// <returns>The newly created stream reference value.</returns>
        private ODataStreamPropertyInfo ReadStreamPropertyInfo(IODataJsonReaderResourceState resourceState, string streamPropertyName)
        {
            ODataStreamPropertyInfo streamInfo = new ODataStreamPropertyInfo
            {
                Name = streamPropertyName,
            };

            ReadStreamInfo(streamInfo, resourceState, streamPropertyName);
            ODataResourceMetadataBuilder builder =
            this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                this.JsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                /*isDelta*/ false);

            // Note that we set the metadata builder even when streamProperty is null, which is the case when the stream property is undeclared.
            // For undeclared stream properties, we will apply conventional metadata evaluation just as declared stream properties.
            streamInfo.SetMetadataBuilder(builder, streamPropertyName);

            return streamInfo;
        }

        /// <summary>
        /// Populates StreamInfo from the property annotations.
        /// </summary>
        /// <param name="streamInfo">The stream info to populate.</param>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="streamPropertyName">The name of the stream property to read the value for.</param>
        private void ReadStreamInfo(IODataStreamReferenceInfo streamInfo, IODataJsonReaderResourceState resourceState, string streamPropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(streamPropertyName), "!string.IsNullOrEmpty(streamPropertyName)");

            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(streamPropertyName))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataMediaEditLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaEditLink annotation should have been parsed as a non-null Uri.");
                        streamInfo.EditLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaReadLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.mediaReadLink annotation should have been parsed as a non-null Uri.");
                        streamInfo.ReadLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaETag:
                        Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaEtag annotation should have been parsed as a non-null string.");
                        streamInfo.ETag = (string)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataMediaContentType:
                        Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.mediaContentType annotation should have been parsed as a non-null string.");
                        streamInfo.ContentType = (string)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataType:
                        Debug.Assert(((string)propertyAnnotation.Value).Contains("Stream", StringComparison.Ordinal), "Non-stream type annotation on stream property");
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedStreamPropertyAnnotation, streamPropertyName, propertyAnnotation.Key));
                }
            }

            // Streams in requests cannot contain links or etags
            if (!this.ReadingResponse)
            {
                if (streamInfo.ETag != null || streamInfo.EditLink != null || streamInfo.ReadLink != null)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_StreamPropertyInRequest, streamPropertyName));
                }
            }
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
        private void ReadSingleOperationValue(IODataJsonOperationsDeserializerContext readerContext, IODataJsonReaderResourceState resourceState, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(readerContext != null, "readerContext != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue, metadataReferencePropertyName, readerContext.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            readerContext.JsonReader.ReadStartObject();

            ODataOperation operation = this.CreateODataOperationAndAddToEntry(readerContext, metadataReferencePropertyName);

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
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation, operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation, operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = readerContext.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
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
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_OperationMissingTargetProperty, metadataReferencePropertyName));
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
            Debug.Assert(ODataJsonUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue, metadataReferencePropertyName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            this.JsonReader.ReadStartObject();

            ODataOperation operation = this.CreateODataOperationAndAddToResourceSet(resourceSet, metadataReferencePropertyName);

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
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation, operationPropertyName, metadataReferencePropertyName));
                        }

                        string titleString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTitleName);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation, operationPropertyName, metadataReferencePropertyName));
                        }

                        string targetString = this.JsonReader.ReadStringValue(JsonConstants.ODataOperationTargetName);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
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
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_OperationMissingTargetProperty, metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            this.JsonReader.ReadEndObject();
        }

        /// <summary>
        /// Sets the metadata builder for the operation.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="operation">The operation to set the metadata builder on.</param>
        private void SetMetadataBuilder(IODataJsonReaderResourceState resourceState, ODataOperation operation)
        {
            ODataResourceMetadataBuilder builder =
                this.MetadataContext.GetResourceMetadataBuilderForReader(resourceState,
                    this.JsonInputContext.MessageReaderSettings.EnableReadingKeyAsSegment,
                    /*isDelta*/ false);
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
            string fullyQualifiedOperationName = ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            ODataOperation operation = ODataJsonUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

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
            string fullyQualifiedOperationName = ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);
            IEdmOperation firstActionOrFunction = this.JsonInputContext.Model.ResolveOperations(fullyQualifiedOperationName).FirstOrDefault();

            bool isAction;

            if (firstActionOrFunction == null)
            {
                // Ignore the unknown function/action.
                return null;
            }

            ODataOperation operation = ODataJsonUtils.CreateODataOperation(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName, firstActionOrFunction, out isAction);

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
        private void ReadMetadataReferencePropertyValue(IODataJsonReaderResourceState resourceState, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.Resource != null, "resourceState.Resource != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator, StringComparison.Ordinal) > -1, "metadataReferencePropertyName.IndexOf(ODataJsonConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

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
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator, StringComparison.Ordinal) > -1, "metadataReferencePropertyName.IndexOf(ODataJsonConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

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
                throw new ODataException(SRResources.ODataJsonResourceDeserializer_MetadataReferencePropertyInRequest);
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
            private ODataJsonResourceDeserializer jsonResourceDeserializer;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="resource">The resource to add operations to.</param>
            /// <param name="jsonResourceDeserializer">The deserializer to use.</param>
            public OperationsDeserializerContext(ODataResourceBase resource, ODataJsonResourceDeserializer jsonResourceDeserializer)
            {
                Debug.Assert(resource != null, "resource != null");
                Debug.Assert(jsonResourceDeserializer != null, "jsonResourceDeserializer != null");

                this.resource = resource;
                this.jsonResourceDeserializer = jsonResourceDeserializer;
            }

            /// <summary>
            /// The JSON reader to read the operations value from.
            /// </summary>
            public IJsonReader JsonReader
            {
                get
                {
                    return this.jsonResourceDeserializer.JsonReader;
                }
            }

            /// <summary>
            /// Given a URI from the payload, this method will try to make it absolute, or fail otherwise.
            /// </summary>
            /// <param name="uriFromPayload">The URI string from the payload to process.</param>
            /// <returns>An absolute URI to report.</returns>
            public Uri ProcessUriFromPayload(string uriFromPayload)
            {
                return this.jsonResourceDeserializer.ProcessUriFromPayload(uriFromPayload);
            }

            /// <summary>
            /// Adds the specified action to the current resource.
            /// </summary>
            /// <param name="action">The action which is fully populated with the data from the payload.</param>
            public void AddActionToResource(ODataAction action)
            {
                Debug.Assert(action != null, "action != null");
                this.resource.AddAction(action);
            }

            /// <summary>
            /// Adds the specified function to the current resource.
            /// </summary>
            /// <param name="function">The function which is fully populated with the data from the payload.</param>
            public void AddFunctionToResource(ODataFunction function)
            {
                Debug.Assert(function != null, "function != null");
                this.resource.AddFunction(function);
            }
        }

        /// <summary>
        /// Asynchronously reads the start of the JSON array for the content of the resource set.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartArray:     The start of the resource set property array; this method will fail if the node is anything else.
        /// Post-Condition: JsonNodeType.StartObject:    The first item in the resource set
        ///                 JsonNodeType.PrimitiveValue: A null resource, or a primitive value within an untyped collection
        ///                 JsonNodeType.StartArray:     A nested collection within an untyped collection
        ///                 JsonNodeType.EndArray:       The end of the resource set
        /// </remarks>
        internal async Task ReadResourceSetContentStartAsync()
        {
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType != JsonNodeType.StartArray)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadResourceSetContentStart, this.JsonReader.NodeType));
            }

            await this.JsonReader.ReadStartArrayAsync()
                .ConfigureAwait(false);
            this.JsonReader.AssertNotBuffering();
        }

        /// <summary>
        /// Asynchronously reads the end of the array containing the resource set content.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
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
        internal async Task ReadResourceSetContentEndAsync()
        {
            this.AssertJsonCondition(JsonNodeType.EndArray);
            this.JsonReader.AssertNotBuffering();

            await this.JsonReader.ReadEndArrayAsync()
                .ConfigureAwait(false);

            this.AssertJsonCondition(
                JsonNodeType.EndOfInput,
                JsonNodeType.EndObject,
                JsonNodeType.Property,
                JsonNodeType.StartArray,
                JsonNodeType.PrimitiveValue,
                JsonNodeType.StartObject,
                JsonNodeType.EndArray);
        }

        /// <summary>
        /// Asynchronously reads the resource type name annotation (odata.type)
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method fills the ODataResource.TypeName property if the type name is found in the payload.
        /// </remarks>
        internal async Task ReadResourceTypeNameAsync(IODataJsonReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the odata.type property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = await this.JsonReader.GetPropertyNameAsync()
                    .ConfigureAwait(false);

                if (string.Equals(ODataJsonConstants.PrefixedODataTypePropertyName, propertyName, StringComparison.Ordinal)
                    || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataTypePropertyName, propertyName))
                {
                    Debug.Assert(resourceState.Resource.TypeName == null, "type name should not have already been set");

                    // Read over the property to move to its value.
                    await this.JsonReader.ReadAsync()
                        .ConfigureAwait(false);

                    // Read the annotation value.
                    resourceState.Resource.TypeName = await this.ReadODataTypeAnnotationValueAsync()
                        .ConfigureAwait(false);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously reads the OData 4.01 deleted resource annotation (odata.removed)
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataDeletedResource"/> that was read.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the resource object.
        ///                 JsonNodeType.EndObject         End of the resource object.
        /// Post-Condition: JsonNodeType.Property          The property after the odata.type (if there was any), or the property on which the method was called.
        ///                 JsonNodeType.EndObject         End of the resource object.
        ///
        /// This method Creates an ODataDeltaDeletedEntry and fills in the Id and Reason properties, if specified in the payload.
        /// </remarks>
        internal async Task<ODataDeletedResource> ReadDeletedResourceAsync()
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataDeletedResource deletedResource = null;

            // If the current node is the deleted property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = await this.JsonReader.GetPropertyNameAsync()
                    .ConfigureAwait(false);
                if (string.Equals(ODataJsonConstants.PrefixedODataRemovedPropertyName, propertyName, StringComparison.Ordinal)
                    || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataRemovedPropertyName, propertyName))
                {
                    DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;
                    Uri id = null;

                    // Read over the property to move to its value.
                    await this.JsonReader.ReadAsync()
                        .ConfigureAwait(false);

                    // Read the removed object and extract the reason, if present
                    this.AssertJsonCondition(JsonNodeType.StartObject, JsonNodeType.PrimitiveValue /*null*/);
                    object removedValue;
                    if (this.JsonReader.NodeType != JsonNodeType.PrimitiveValue)
                    {
                        while (this.JsonReader.NodeType != JsonNodeType.EndObject && await this.JsonReader.ReadAsync().ConfigureAwait(false))
                        {
                            // If the current node is the reason property - read it.
                            if (this.JsonReader.NodeType == JsonNodeType.Property
                                && string.Equals(ODataJsonConstants.ODataReasonPropertyName, await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), StringComparison.Ordinal))
                            {
                                // Read over the property to move to its value.
                                await this.JsonReader.ReadAsync()
                                    .ConfigureAwait(false);

                                // Read the reason value.
                                if (string.Equals(ODataJsonConstants.ODataReasonDeletedValue, await this.JsonReader.ReadStringValueAsync().ConfigureAwait(false), StringComparison.Ordinal))
                                {
                                    reason = DeltaDeletedEntryReason.Deleted;
                                }
                            }
                        }
                    }
                    else if ((removedValue = await this.JsonReader.GetValueAsync().ConfigureAwait(false)) != null)
                    {
                        throw new ODataException(
                            Error.Format(SRResources.ODataJsonResourceDeserializer_DeltaRemovedAnnotationMustBeObject, removedValue));
                    }

                    // Read over end object or null value
                    await this.JsonReader.ReadAsync()
                        .ConfigureAwait(false);

                    // A deleted object must have at least either the odata id annotation or the key values
                    if (this.JsonReader.NodeType != JsonNodeType.Property)
                    {
                        throw new ODataException(SRResources.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
                    }

                    // If the next property is the id property - read it.
                    propertyName = await this.JsonReader.GetPropertyNameAsync()
                        .ConfigureAwait(false);
                    if (string.Equals(ODataJsonConstants.PrefixedODataIdPropertyName, propertyName, StringComparison.Ordinal)
                        || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataIdPropertyName, propertyName))
                    {
                        // Read over the property to move to its value.
                        await this.JsonReader.ReadAsync()
                            .ConfigureAwait(false);

                        // Read the id value.
                        id = UriUtils.StringToUri(await this.JsonReader.ReadStringValueAsync().ConfigureAwait(false));
                    }

                    deletedResource = ReaderUtils.CreateDeletedResource(id, reason);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return deletedResource;
        }

        /// <summary>
        /// Asynchronously reads an OData 4.0 delete entry
        /// </summary>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataDeletedResource"/> read.
        /// </returns>
        /// <remarks>
        /// This method Creates an ODataDeltaDeletedEntry and fills in the Id and Reason properties, if specified in the payload.
        /// </remarks>
        internal async Task<ODataDeletedResource> ReadDeletedEntryAsync()
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            Uri id = null;
            DeltaDeletedEntryReason reason = DeltaDeletedEntryReason.Changed;

            while (this.JsonReader.NodeType != JsonNodeType.EndObject && this.JsonReader.NodeType != JsonNodeType.EndOfInput)
            {
                if (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = await this.JsonReader.GetPropertyNameAsync()
                        .ConfigureAwait(false);

                    // If the current node is the id property - read it.
                    if (string.Equals(ODataJsonConstants.ODataIdPropertyName, propertyName, StringComparison.Ordinal))
                    {
                        // Read over the property to move to its value.
                        await this.JsonReader.ReadAsync()
                            .ConfigureAwait(false);

                        // Read the Id value.
                        id = await this.JsonReader.ReadUriValueAsync()
                            .ConfigureAwait(false);
                        Debug.Assert(id != null, "value for Id must be provided");

                        continue;
                    }
                    // If the current node is the reason property - read it.
                    else if (string.Equals(ODataJsonConstants.ODataReasonPropertyName, propertyName, StringComparison.Ordinal))
                    {
                        // Read over the property to move to its value.
                        await this.JsonReader.ReadAsync()
                            .ConfigureAwait(false);

                        // Read the reason value.
                        if (string.Equals(ODataJsonConstants.ODataReasonDeletedValue, await this.JsonReader.ReadStringValueAsync().ConfigureAwait(false), StringComparison.Ordinal))
                        {
                            reason = DeltaDeletedEntryReason.Deleted;
                        }

                        continue;
                    }
                }

                await this.JsonReader.ReadAsync().ConfigureAwait(false);

                if (this.JsonReader.NodeType == JsonNodeType.StartObject || this.JsonReader.NodeType == JsonNodeType.StartArray)
                {
                    throw new ODataException(SRResources.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry);
                }

                // Ignore unknown primitive properties in a 4.0 deleted entry
            }

            return ReaderUtils.CreateDeletedResource(id, reason);
        }

        /// <summary>
        /// Asynchronously reads the delta (deleted) link source.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Source property if the id is found in the payload.
        /// </remarks>
        internal async Task ReadDeltaLinkSourceAsync(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the source property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.Equals(ODataJsonConstants.ODataSourcePropertyName, await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), StringComparison.Ordinal))
            {
                Debug.Assert(link.Source == null, "source should not have already been set");

                // Read over the property to move to its value.
                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);

                // Read the source value.
                link.Source = await this.JsonReader.ReadUriValueAsync()
                    .ConfigureAwait(false);
                Debug.Assert(link.Source != null, "value for source must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously reads the delta (deleted) link relationship.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Relationship property if the id is found in the payload.
        /// </remarks>
        internal async Task ReadDeltaLinkRelationshipAsync(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the relationship property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.Equals(ODataJsonConstants.ODataRelationshipPropertyName, await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), StringComparison.Ordinal))
            {
                Debug.Assert(link.Relationship == null, "relationship should not have already been set");

                // Read over the property to move to its value.
                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);

                // Read the relationship value.
                link.Relationship = await this.JsonReader.ReadStringValueAsync()
                    .ConfigureAwait(false);
                Debug.Assert(link.Relationship != null, "value for relationship must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously reads the delta (deleted) link target.
        /// </summary>
        /// <param name="link">The delta (deleted) link being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property          The first property after the odata.context in the link object.
        ///                 JsonNodeType.EndObject         End of the link object.
        /// Post-Condition: JsonNodeType.Property          The properties.
        ///                 JsonNodeType.EndObject         End of the link object.
        ///
        /// This method fills the ODataDelta(Deleted)Link.Target property if the id is found in the payload.
        /// </remarks>
        internal async Task ReadDeltaLinkTargetAsync(ODataDeltaLinkBase link)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If the current node is the target property - read it.
            if (this.JsonReader.NodeType == JsonNodeType.Property &&
                string.Equals(ODataJsonConstants.ODataTargetPropertyName, await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), StringComparison.Ordinal))
            {
                Debug.Assert(link.Target == null, "target should not have already been set");

                // Read over the property to move to its value.
                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);

                // Read the source value.
                link.Target = await this.JsonReader.ReadUriValueAsync()
                    .ConfigureAwait(false);
                Debug.Assert(link.Target != null, "value for target must be provided");
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously reads the content of a resource until a nested resource info is detected.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a reader nested resource info
        /// representing the nested resource info detected while reading the resource contents,
        /// or null if no nested resource info was detected.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property               The property to read
        ///                 JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        /// Post-Condition: JsonNodeType.EndObject              If no (more) properties exist in the resource's content
        ///                 JsonNodeType.Property               If we've read a deferred link (this is the property after the deferred link)
        ///                 JsonNodeType.StartObject            Expanded resource
        ///                 JsonNodeType.StartArray             Expanded resource set
        ///                 JsonNodeType.PrimitiveValue (null)  Expanded null
        /// </remarks>
        internal async Task<ODataJsonReaderNestedInfo> ReadResourceContentAsync(IODataJsonReaderResourceState resourceState)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.ResourceType != null && this.Model.IsUserModel(), "A non-null resource type and non-null model are required.");
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Pre-Condition: JsonNodeType.Property or JsonNodeType.EndObject");
            this.JsonReader.AssertNotBuffering();

            ODataJsonReaderNestedInfo readerNestedResourceInfo = null;
            Debug.Assert(resourceState.ResourceType != null, "In JSON we must always have a structured type when reading resource.");

            // Figure out whether we have more properties for this resource
            // read all the properties until we hit a link
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                await this.ProcessPropertyAsync(
                    resourceState.PropertyAndAnnotationCollector,
                    this.ReadEntryPropertyAnnotationValueAsync,
                    async (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                            case PropertyParsingResult.CustomInstanceAnnotation:
                                await this.ReadOverPropertyNameAsync()
                                    .ConfigureAwait(false);
                                object value = await this.ReadODataOrCustomInstanceAnnotationValueAsync(resourceState, propertyParsingResult, propertyName)
                                    .ConfigureAwait(false);
                                this.ApplyEntryInstanceAnnotation(resourceState, propertyName, value);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = await this.ReadPropertyWithoutValueAsync(resourceState, propertyName)
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.NestedDeltaResourceSet:
                                // Will read over property name in ReadPropertyWithValue
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = await this.ReadPropertyWithValueAsync(resourceState, propertyName, isDeltaResourceSet: true)
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.PropertyWithValue:
                                // Will read over property name in ReadPropertyWithValue
                                resourceState.AnyPropertyFound = true;
                                readerNestedResourceInfo = await this.ReadPropertyWithValueAsync(resourceState, propertyName, isDeltaResourceSet: false)
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                await this.ReadOverPropertyNameAsync()
                                    .ConfigureAwait(false);
                                await this.ReadMetadataReferencePropertyValueAsync(resourceState, propertyName)
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.EndOfObject:
                                await this.ReadOverPropertyNameAsync()
                                    .ConfigureAwait(false);
                                break;
                        }
                    }).ConfigureAwait(false);

                if (readerNestedResourceInfo != null)
                {
                    // We found a nested resource info
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
            //  - PrimitiveValue- if it's a stream or an expanded null resource
            //  - EndObject - end of the resource
            Debug.Assert(
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartObject ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.StartArray ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.Property ||
                readerNestedResourceInfo != null && this.JsonReader.NodeType == JsonNodeType.PrimitiveValue ||
                readerNestedResourceInfo is ODataJsonReaderNestedInfo ||
                this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeType.Property or JsonNodeType.EndObject or JsonNodeType.Primitive (with null value)");

            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Asynchronously reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="resourceState">The IODataJsonReaderResourceState.</param>
        /// <param name="propertyParsingResult">The PropertyParsingResult.</param>
        /// <param name="annotationName">The annotation name</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the annotation value.
        /// </returns>
        internal async Task<object> ReadODataOrCustomInstanceAnnotationValueAsync(
            IODataJsonReaderResourceState resourceState,
            PropertyParsingResult propertyParsingResult,
            string annotationName)
        {
            object value = await this.ReadEntryInstanceAnnotationAsync(
                annotationName,
                resourceState.AnyPropertyFound,
                typeAnnotationFound: true,
                propertyAndAnnotationCollector: resourceState.PropertyAndAnnotationCollector).ConfigureAwait(false);
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
        /// Asynchronously reads the resource set instance annotations for a top-level resource set.
        /// </summary>
        /// <param name="resourceSet">The <see cref="ODataResourceSet"/> to read the instance annotations for.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the top-level scope.</param>
        /// <param name="forResourceSetStart">true when parsing the instance annotations before the resource set property;
        /// false when parsing the instance annotations after the resource set property.</param>
        /// <param name="readAllResourceSetProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal async Task ReadTopLevelResourceSetAnnotationsAsync(
            ODataResourceSetBase resourceSet,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool forResourceSetStart,
            bool readAllResourceSetProperties)
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
                        // Use an empty duplicate property name checker since this.ParsePropertyAsync() read through the same property annotation of instance annotations again.
                        propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(false);
                    }

                    await this.ProcessPropertyAsync(
                        propertyAndAnnotationCollector,
                        this.ReadTypePropertyAnnotationValueAsync,
                        async (propertyParseResult, propertyName) =>
                        {
                            await this.ReadOverPropertyNameAsync()
                                .ConfigureAwait(false);
                            switch (propertyParseResult)
                            {
                                case PropertyParsingResult.ODataInstanceAnnotation:
                                case PropertyParsingResult.CustomInstanceAnnotation:
                                    await this.ReadODataOrCustomInstanceAnnotationValueAsync(
                                        resourceSet,
                                        propertyAndAnnotationCollector,
                                        forResourceSetStart,
                                        readAllResourceSetProperties,
                                        propertyParseResult,
                                        propertyName).ConfigureAwait(false);
                                    break;

                                case PropertyParsingResult.PropertyWithValue:
                                    if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                    {
                                        // We found the resource set property and are done parsing property annotations;
                                        // When we are in the mode where we scan ahead and read all resource set properties
                                        // (for the reordering scenario), we have to start buffering and continue
                                        // reading. Otherwise we found the resourceSet's data property and are done.
                                        if (readAllResourceSetProperties)
                                        {
                                            await this.JsonReader.StartBufferingAsync()
                                                .ConfigureAwait(false);
                                            buffering = true;

                                            await this.JsonReader.SkipValueAsync()
                                                .ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            foundValueProperty = true;
                                        }
                                    }
                                    else
                                    {
                                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_InvalidPropertyInTopLevelResourceSet, propertyName, ODataJsonConstants.ODataValuePropertyName));
                                    }

                                    break;
                                case PropertyParsingResult.PropertyWithoutValue:
                                    // If we find a property without a value it means that we did not find the resource set property (yet)
                                    // but an invalid property annotation
                                    throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet, propertyName));

                                case PropertyParsingResult.EndOfObject:
                                    break;

                                case PropertyParsingResult.MetadataReferenceProperty:
                                    if (!(resourceSet is ODataResourceSet))
                                    {
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                                    }

                                    await this.ReadMetadataReferencePropertyValueAsync((ODataResourceSet)resourceSet, propertyName)
                                        .ConfigureAwait(false);
                                    break;

                                default:
                                    throw new ODataException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.ODataJsonResourceDeserializer_ReadTopLevelResourceSetAnnotations));
                            }
                        }).ConfigureAwait(false);

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
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_ExpectedResourceSetPropertyNotFound, ODataJsonConstants.ODataValuePropertyName));
            }
        }

        /// <summary>
        /// Asynchronously reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="resourceSet">The ODataResourceSetBase.</param>
        /// <param name="propertyAndAnnotationCollector">The PropertyAndAnnotationCollector.</param>
        /// <param name="forResourceSetStart">true when parsing the instance annotations before the resource set property;
        /// false when parsing the instance annotations after the resource set property.</param>
        /// <param name="readAllResourceSetProperties">true if we should scan ahead for the annotations and ignore the actual data properties (used with
        /// the reordering reader); otherwise false.</param>
        /// <param name="propertyParseResult">The PropertyParsingResult.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal async Task ReadODataOrCustomInstanceAnnotationValueAsync(
            ODataResourceSetBase resourceSet,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool forResourceSetStart,
            bool readAllResourceSetProperties,
            PropertyParsingResult propertyParseResult,
            string annotationName)
        {
            if (propertyParseResult == PropertyParsingResult.ODataInstanceAnnotation)
            {
                // #### annotation 1 ####
                // built-in "odata." annotation value is added to propertyAndAnnotationCollector then later to resourceSet.InstanceAnnotations.
                propertyAndAnnotationCollector.AddODataScopeAnnotation(annotationName, await this.JsonReader.GetValueAsync().ConfigureAwait(false));
            }

            // When we are reading the start of a resource set (in scan-ahead mode or not) or when
            // we read the end of a resource set and not in scan-ahead mode, read the value;
            // otherwise skip it.
            if (forResourceSetStart || !readAllResourceSetProperties)
            {
                // #### annotation 2 ####
                // custom annotation value will be directly added to resourceSet.InstanceAnnotations.
                await this.ReadAndApplyResourceSetInstanceAnnotationValueAsync(annotationName, resourceSet, propertyAndAnnotationCollector)
                    .ConfigureAwait(false);
            }
            else
            {
                await this.JsonReader.SkipValueAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads a value of property annotation on the resource level.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the property annotation.
        /// </returns>
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
        internal async Task<object> ReadEntryPropertyAnnotationValueAsync(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            Tuple<bool, string> readODataTypeAnnotationResult = await this.TryReadODataTypeAnnotationValueAsync(propertyAnnotationName)
                .ConfigureAwait(false);
            if (readODataTypeAnnotationResult.Item1)
            {
                return readODataTypeAnnotationResult.Item2; // TypeName
            }

            switch (propertyAnnotationName)
            {
                case ODataAnnotationNames.ODataNavigationLinkUrl:  // odata.navigationLinkUrl
                case ODataAnnotationNames.ODataAssociationLinkUrl: // odata.associationLinkUrl
                case ODataAnnotationNames.ODataNextLink:           // odata.nextLink
                case ODataAnnotationNames.ODataMediaEditLink:      // odata.mediaEditLink
                case ODataAnnotationNames.ODataMediaReadLink:      // odata.mediaReadLink
                case ODataAnnotationNames.ODataContext:            // odata.context
                    return await this.ReadAndValidateAnnotationStringValueAsUriAsync(propertyAnnotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataCount:              // odata.count
                    return await this.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(propertyAnnotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataMediaETag:          // odata.mediaEtag
                case ODataAnnotationNames.ODataMediaContentType:   // odata.mediaContentType
                    return await this.ReadAndValidateAnnotationStringValueAsync(propertyAnnotationName)
                        .ConfigureAwait(false);

                // odata.bind
                case ODataAnnotationNames.ODataBind:
                    // The value of the odata.bind annotation can be either an array of strings or a string (collection or singleton nested resource info).
                    // Note that we don't validate that the cardinality of the navigation property matches the payload here, since we don't want to lookup the property twice.
                    // We will validate that later when we consume the value of the property annotation.
                    if (this.JsonReader.NodeType != JsonNodeType.StartArray)
                    {
                        return new ODataEntityReferenceLink
                        {
                            Url = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataBind).ConfigureAwait(false)
                        };
                    }

                    LinkedList<ODataEntityReferenceLink> entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();

                    // Read over the start array
                    await this.JsonReader.ReadAsync()
                        .ConfigureAwait(false);
                    while (this.JsonReader.NodeType != JsonNodeType.EndArray)
                    {
                        entityReferenceLinks.AddLast(
                            new ODataEntityReferenceLink
                            {
                                Url = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataBind).ConfigureAwait(false)
                            });
                    }

                    // Read over the end array
                    await this.JsonReader.ReadAsync()
                        .ConfigureAwait(false);
                    if (entityReferenceLinks.Count == 0)
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_EmptyBindArray, ODataAnnotationNames.ODataBind));
                    }

                    return entityReferenceLinks;

                case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                default:
                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyAnnotationName));
            }
        }

        /// <summary>
        /// Asynchronously reads instance annotation in the resource object.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation found.</param>
        /// <param name="anyPropertyFound">true if a non-annotation property has already been encountered.</param>
        /// <param name="typeAnnotationFound">true if the 'odata.type' annotation has already been encountered, or should have been by now.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker for the resource being read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the annotation.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the instance annotation property
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource object
        ///                 JsonNodeType.Property               The next property after the instance annotation
        /// </remarks>
        internal async Task<object> ReadEntryInstanceAnnotationAsync(
            string annotationName,
            bool anyPropertyFound,
            bool typeAnnotationFound,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataType:   // 'odata.type'
                    if (!typeAnnotationFound)
                    {
                        return await this.ReadODataTypeAnnotationValueAsync()
                            .ConfigureAwait(false);
                    }

                    // We already read the odata.type if it was the first property in ReadResourceStart, so any other occurrence means
                    // that it was not the first property.
                    throw new ODataException(SRResources.ODataJsonResourceDeserializer_ResourceTypeAnnotationNotFirst);

                case ODataAnnotationNames.ODataId:   // 'odata.id'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(
                            Error.Format(SRResources.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty, annotationName));
                    }

                    return await this.ReadAnnotationStringValueAsUriAsync(annotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataETag:   // 'odata.etag'
                    if (anyPropertyFound)
                    {
                        throw new ODataException(
                            Error.Format(SRResources.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty, annotationName));
                    }

                    return await this.ReadAndValidateAnnotationStringValueAsync(annotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataEditLink:    // 'odata.editLink'
                case ODataAnnotationNames.ODataReadLink:    // 'odata.readLink'
                case ODataAnnotationNames.ODataMediaEditLink:   // 'odata.mediaEditLink'
                case ODataAnnotationNames.ODataMediaReadLink:   // 'odata.mediaReadLink'
                    return await this.ReadAndValidateAnnotationStringValueAsUriAsync(annotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataMediaContentType:  // 'odata.mediaContentType'
                case ODataAnnotationNames.ODataMediaETag:  // 'odata.mediaEtag'
                    return await this.ReadAndValidateAnnotationStringValueAsync(annotationName)
                        .ConfigureAwait(false);

                case ODataAnnotationNames.ODataRemoved: // 'odata.removed'
                    {
                        throw new ODataException(SRResources.ODataJsonResourceDeserializer_UnexpectedDeletedEntryInResponsePayload);
                    }

                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    return await this.ReadCustomInstanceAnnotationValueAsync(propertyAndAnnotationCollector, annotationName)
                        .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads the value of a resource set annotation (count or next link).
        /// </summary>
        /// <param name="annotationName">The name of the annotation found.</param>
        /// <param name="resourceSet">The resource set to read the annotation for; if non-null, the annotation value will be assigned to the resource set.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker instance.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue         The value of the annotation
        /// Post-Condition: JsonNodeType.EndObject              The end of the resource set object
        ///                 JsonNodeType.Property               The next annotation after the current annotation
        /// </remarks>
        internal async Task ReadAndApplyResourceSetInstanceAnnotationValueAsync(
            string annotationName,
            ODataResourceSetBase resourceSet,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            switch (annotationName)
            {
                case ODataAnnotationNames.ODataCount:
                    resourceSet.Count = await this.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(ODataAnnotationNames.ODataCount)
                        .ConfigureAwait(false);
                    break;

                case ODataAnnotationNames.ODataNextLink:
                    resourceSet.NextPageLink = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataNextLink)
                        .ConfigureAwait(false);
                    break;

                case ODataAnnotationNames.ODataDeltaLink:
                    resourceSet.DeltaLink = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataDeltaLink)
                        .ConfigureAwait(false);
                    break;
                case ODataAnnotationNames.ODataType:
                    await this.ReadAndValidateAnnotationStringValueAsync(ODataAnnotationNames.ODataType)
                        .ConfigureAwait(false);
                    break;
                default:
                    ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                    Debug.Assert(
                        !this.MessageReaderSettings.ShouldSkipAnnotation(annotationName),
                        "!this.MessageReaderSettings.ShouldReadAndValidateAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                    object instanceAnnotationValue = await this.ReadCustomInstanceAnnotationValueAsync(propertyAndAnnotationCollector, annotationName)
                        .ConfigureAwait(false);
                    resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotationName, instanceAnnotationValue.ToODataValue()));
                    break;
            }
        }

        /// <summary>
        /// Asynchronously reads resource property which doesn't have value, just annotations.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a reader nested resource info representing 
        /// the nested resource info detected while reading the resource contents,
        /// or null if no nested resource info was detected.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.EndObject              The end of the resource object.
        ///                 JsonNodeType.Property               The property after the one we're to read.
        /// Post-Condition: JsonNodeType.EndObject              This method doesn't move the reader.
        ///                 JsonNodeType.Property
        /// </remarks>
        internal async Task<ODataJsonReaderNestedInfo> ReadPropertyWithoutValueAsync(IODataJsonReaderResourceState resourceState, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            ODataJsonReaderNestedInfo readerNestedInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
            if (edmProperty == null || edmProperty.Type.IsUntyped())
            {
                // Undeclared property - we need to run detection algorithm here.
                readerNestedInfo = await this.ReadUndeclaredPropertyAsync(resourceState, propertyName, propertyWithValue: false)
                    .ConfigureAwait(false);

                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
                return readerNestedInfo;
            }

            // Declared property - read it.
            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo;
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
                        ? ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: false)
                        : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: false);

                    if (!readerNestedResourceInfo.HasEntityReferenceLink)
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink, propertyName, ODataAnnotationNames.ODataBind));
                    }
                }

                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                readerNestedInfo = readerNestedResourceInfo;
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
                    readerNestedInfo = ReadNestedPropertyInfoWithoutValue(resourceState, edmProperty.Name, edmProperty);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return readerNestedInfo;
        }

        /// <summary>
        /// Asynchronously reads any next link annotation immediately after the end of a resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set being read.</param>
        /// <param name="expandedNestedResourceInfo">The information about the expanded link. This must be non-null if we're reading an expanded resource set, and must be null if we're reading a top-level resource set.</param>
        /// <param name="propertyAndAnnotationCollector">The top-level duplicate property names checker, if we're reading a top-level resource set.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal Task ReadNextLinkAnnotationAtResourceSetEndAsync(
            ODataResourceSetBase resourceSet,
            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            // Check for annotations on the resource set that occur after the resource set itself. (Note: the only allowed one is odata.nextLink, and we fail for anything else.)
            // We do this slightly differently depending on whether the resource set was an expanded navigation or a top-level resource set.
            if (expandedNestedResourceInfo != null)
            {
                return this.ReadExpandedResourceSetAnnotationsAtResourceSetEndAsync(resourceSet, expandedNestedResourceInfo);
            }
            else
            {
                Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

                // Check for resource set instance annotations that appear after the resource set.
                bool isReordering = this.JsonReader is ReorderingJsonReader;
                return this.ReadTopLevelResourceSetAnnotationsAsync(
                    resourceSet,
                    propertyAndAnnotationCollector,
                    forResourceSetStart: false,
                    readAllResourceSetProperties: isReordering);
            }
        }

        /// <summary>
        /// We fail here if we encounter any other property annotation for the expanded navigation (since these should come before the property itself).
        /// </summary>
        /// <param name="resourceSet">The resource set that was just read.</param>
        /// <param name="expandedNestedResourceInfo">The information for the current expanded nested resource info being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadExpandedResourceSetAnnotationsAtResourceSetEndAsync(
            ODataResourceSetBase resourceSet,
            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo)
        {
            Debug.Assert(expandedNestedResourceInfo != null, "expandedNestedResourceInfo != null");
            Debug.Assert(expandedNestedResourceInfo.NestedResourceInfo.IsCollection == true, "Only collection navigation properties can have resourceSet content.");

            // Look at the next property in the owning resource, if it's a property annotation for the expanded nested resource info property, read it.
            string propertyName, annotationName;
            while (this.JsonReader.NodeType == JsonNodeType.Property &&
                TryParsePropertyAnnotation(await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), out propertyName, out annotationName) &&
                string.Equals(propertyName, expandedNestedResourceInfo.NestedResourceInfo.Name, StringComparison.Ordinal))
            {
                if (!this.ReadingResponse)
                {
                    throw new ODataException(
                        Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedPropertyAnnotation, propertyName, annotationName));
                }

                // Read over the property name.
                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);

                switch (this.CompleteSimplifiedODataAnnotation(annotationName))
                {
                    case ODataAnnotationNames.ODataNextLink:
                        if (resourceSet.NextPageLink != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation,
                                    ODataAnnotationNames.ODataNextLink,
                                    expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.NextPageLink = await this.ReadAndValidateAnnotationStringValueAsUriAsync(ODataAnnotationNames.ODataNextLink)
                            .ConfigureAwait(false);
                        break;

                    case ODataAnnotationNames.ODataCount:
                        if (resourceSet.Count != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation,
                                    ODataAnnotationNames.ODataCount,
                                    expandedNestedResourceInfo.NestedResourceInfo.Name));
                        }

                        // Read the property value.
                        resourceSet.Count = await this.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(ODataAnnotationNames.ODataCount)
                            .ConfigureAwait(false);
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                    default:
                        throw new ODataException(
                            Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet,
                                annotationName,
                                expandedNestedResourceInfo.NestedResourceInfo.Name));
                }
            }
        }

        /// <summary>
        /// Asynchronously reads resource property (which is neither instance nor property annotation) which has a value.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the property read.</param>
        /// <param name="isDeltaResourceSet">The property being read represents a nested delta resource set.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a reader nested resource info representing
        /// the nested resource info detected while reading the resource contents,
        /// or null if no nested resource info was detected.
        /// </returns>
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
        private async Task<ODataJsonReaderNestedInfo> ReadPropertyWithValueAsync(
            IODataJsonReaderResourceState resourceState,
            string propertyName,
            bool isDeltaResourceSet)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property, JsonNodeType.StartObject, JsonNodeType.StartArray);

            ODataJsonReaderNestedInfo readerNestedInfo = null;
            IEdmStructuredType resourceType = resourceState.ResourceType;
            IEdmProperty edmProperty = this.ReaderValidator.ValidatePropertyDefined(propertyName, resourceType);
            bool isCollection = edmProperty == null ? false : edmProperty.Type.IsCollection();
            IEdmStructuralProperty structuralProperty = edmProperty as IEdmStructuralProperty;

            if (structuralProperty != null)
            {
                ODataJsonReaderNestedInfo nestedInfo = await this.TryReadAsStreamAsync(
                    resourceState,
                    structuralProperty,
                    structuralProperty.Type,
                    structuralProperty.Name).ConfigureAwait(false);
                if (nestedInfo != null)
                {
                    return nestedInfo;
                }
            }

            if (edmProperty != null && !edmProperty.Type.IsUntyped())
            {
                await this.ReadOverPropertyNameAsync()
                    .ConfigureAwait(false);
                IEdmStructuredType structuredPropertyTypeOrItemType = structuralProperty == null ? null : structuralProperty.Type.ToStructuredType();
                IEdmNavigationProperty navigationProperty = edmProperty as IEdmNavigationProperty;
                if (structuredPropertyTypeOrItemType != null)
                {
                    ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                    // Complex property or collection of complex property.
                    await ValidateExpandedNestedResourceInfoPropertyValueAsync(
                        this.JsonReader,
                        isCollection,
                        propertyName,
                        edmProperty.Type).ConfigureAwait(false);

                    if (isCollection)
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceSetNestedResourceInfo(resourceState, structuralProperty, structuredPropertyTypeOrItemType, structuralProperty.Name);
                    }
                    else
                    {
                        readerNestedResourceInfo = ReadNonExpandedResourceNestedResourceInfo(resourceState, structuralProperty, structuredPropertyTypeOrItemType, structuralProperty.Name);
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                    readerNestedInfo = readerNestedResourceInfo;
                }
                else if (navigationProperty != null)
                {
                    ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                    // Expanded link
                    await ValidateExpandedNestedResourceInfoPropertyValueAsync(
                        this.JsonReader,
                        isCollection,
                        propertyName,
                        edmProperty.Type).ConfigureAwait(false);
                    if (isCollection)
                    {
                        readerNestedResourceInfo = this.ReadingResponse || isDeltaResourceSet
                            ? ReadExpandedResourceSetNestedResourceInfo(resourceState, navigationProperty, navigationProperty.Type.ToStructuredType(), propertyName, isDeltaResourceSet: isDeltaResourceSet)
                            : ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: true);
                    }
                    else
                    {
                        readerNestedResourceInfo = this.ReadingResponse
                            ? ReadExpandedResourceNestedResourceInfo(resourceState, navigationProperty, propertyName, navigationProperty.Type.ToStructuredType(), this.MessageReaderSettings)
                            : ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(resourceState, navigationProperty, propertyName, isExpanded: true);
                    }

                    resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);
                    readerNestedInfo = readerNestedResourceInfo;
                }
                else
                {
                    IEnumerable<string> derivedTypeConstraints = this.JsonInputContext.Model.GetDerivedTypeConstraints(edmProperty);
                    if (derivedTypeConstraints != null)
                    {
                        resourceState.PropertyAndAnnotationCollector.SetDerivedTypeValidator(propertyName, new DerivedTypeValidator(edmProperty.Type.Definition, derivedTypeConstraints, "property", propertyName));
                    }

                    // NOTE: We currently do not check whether the property should be skipped
                    //       here because this can only happen for navigation properties and open properties.
                    await this.ReadEntryDataPropertyAsync(
                        resourceState,
                        edmProperty,
                        ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName)).ConfigureAwait(false);
                }
            }
            else
            {
                // Undeclared property - we need to run detection algorithm here.
                readerNestedInfo = await this.ReadUndeclaredPropertyAsync(resourceState, propertyName, propertyWithValue: true)
                    .ConfigureAwait(false);

                // Note that if nested resource info is returned it's already validated, so we just report it here.
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject, JsonNodeType.StartObject, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue);
            return readerNestedInfo;
        }

        /// <summary>
        /// Asynchronously checks to see if the current property should be read as a stream and, if so reads it
        /// </summary>
        /// <param name="resourceState">current ResourceState</param>
        /// <param name="property">The property being serialized (null for a dynamic property)</param>
        /// <param name="propertyType">The type of the property being serialized</param>
        /// <param name="propertyName">The name of the property being serialized</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataJsonReaderNestedInfo"/> for a nested stream property,
        /// or null if the property shouldn't be streamed.
        /// </returns>
        private async Task<ODataJsonReaderNestedInfo> TryReadAsStreamAsync(
            IODataJsonReaderResourceState resourceState,
            IEdmStructuralProperty property,
            IEdmTypeReference propertyType,
            string propertyName)
        {
            Debug.Assert(propertyName != null, "Property name must not be null");

            bool isCollection = false;
            IEdmPrimitiveType primitiveType = null;
            if (propertyType != null)
            {
                primitiveType = propertyType.Definition.AsElementType() as IEdmPrimitiveType;
                isCollection = propertyType.IsCollection();
            }
            else
            {
                isCollection = this.JsonReader.NodeType != JsonNodeType.PrimitiveValue;
            }

            Func<IEdmPrimitiveType, bool, string, IEdmProperty, bool> readAsStream = this.MessageReaderSettings.ReadAsStreamFunc;

            // Is the property a stream or a stream collection,
            // untyped collection,
            // or a binary or binary collection the client wants to read as a stream...
            if (
                (primitiveType != null &&
                    (primitiveType.IsStream() ||
                        (readAsStream != null
                             && (property == null || !property.IsKey())  // don't stream key properties
                             && (primitiveType.IsBinary() || primitiveType.IsString() || isCollection))
                         && readAsStream(primitiveType, isCollection, propertyName, property))) ||
                (propertyType != null &&
                    isCollection &&
                    propertyType.Definition.AsElementType().IsUntyped()) ||
                (propertyType == null
                    && (isCollection || await this.JsonReader.CanStreamAsync().ConfigureAwait(false))
                    && readAsStream != null
                    && readAsStream(null, isCollection, propertyName, property)))
            {
                if (isCollection)
                {
                    await this.ReadOverPropertyNameAsync()
                        .ConfigureAwait(false);
                    IEdmType elementType = propertyType == null ? EdmCoreModel.Instance.GetUntypedType() : propertyType.Definition.AsElementType();

                    // Collection of streams, or binary/string values to read as streams
                    return ReadStreamCollectionNestedResourceInfo(resourceState, property, propertyName, elementType);
                }
                else
                {
                    ODataPropertyInfo propertyInfo;
                    if (primitiveType != null && primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream)
                    {
                        ODataStreamPropertyInfo streamPropertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);

                        // If it has an instance annotation saying that the content type is JSON, don't read propertyName
                        // otherwise, if we are on start object, BufferingJsonReader will read ahead to try and determine
                        // if we are reading an instream error, which destroys our ability to stream json stream values.
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            bool isJson = false;
                            if (streamPropertyInfo.ContentType?.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal) == true)
                            {
                                isJson = true;
                            }
                            else if (property != null)
                            {
                                IEdmVocabularyAnnotation mediaTypeAnnotation = property.VocabularyAnnotations(this.Model).FirstOrDefault(a => a.Term == CoreVocabularyModel.MediaTypeTerm);
                                if (mediaTypeAnnotation != null)
                                {
                                    // If the property does not have a mediaType annotation specifying application/json, then read over the property name
                                    IEdmStringConstantExpression stringExpression = mediaTypeAnnotation.Value as IEdmStringConstantExpression;
                                    if (stringExpression != null && stringExpression.Value.Contains(MimeConstants.MimeApplicationJson, StringComparison.Ordinal))
                                    {
                                        isJson = true;
                                    }
                                }
                            }

                            if (!isJson)
                            {
                                // Not reading JSON stream, so read over property name
                                await this.ReadOverPropertyNameAsync()
                                    .ConfigureAwait(false);
                            }
                        }

                        // Add the stream reference property
                        ODataStreamReferenceValue streamReferenceValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                        AddResourceProperty(resourceState, propertyName, streamReferenceValue);

                        propertyInfo = streamPropertyInfo;
                    }
                    else
                    {
                        await this.ReadOverPropertyNameAsync()
                            .ConfigureAwait(false);

                        propertyInfo = new ODataPropertyInfo
                        {
                            PrimitiveTypeKind = primitiveType == null ? EdmPrimitiveTypeKind.None : primitiveType.PrimitiveKind,
                            Name = propertyName,
                        };
                    }

                    // return without reading over the property node; we will create a stream over the value
                    this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property);
                    return new ODataJsonReaderNestedPropertyInfo(propertyInfo, property);
                }
            }

            return null;
        }

        /// <summary>
        /// Asynchronously reads over the current property name if positioned on a property
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private Task ReadOverPropertyNameAsync()
        {
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                return this.JsonReader.ReadAsync();
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously read a resource-level data property and check its version compliance.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="edmProperty">The EDM property of the property being read, or null if the property is an open property.</param>
        /// <param name="propertyTypeName">The type name specified for the property in property annotation, or null if no such type name is available.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private async Task ReadEntryDataPropertyAsync(IODataJsonReaderResourceState resourceState, IEdmProperty edmProperty, string propertyTypeName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");
            this.JsonReader.AssertNotBuffering();

            // EdmLib bridge marks all key properties as non-nullable, but Astoria allows them to be nullable.
            // If the property has an annotation to ignore null values, we need to omit the property in requests.
            ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse
                ? ODataNullValueBehaviorKind.Default
                : this.Model.NullValueReadBehaviorKind(edmProperty);
            object propertyValue = await this.ReadNonEntityValueAsync(
                propertyTypeName,
                edmProperty.Type,
                propertyAndAnnotationCollector: null,
                collectionValidator: null,
                validateNullValue: nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                isTopLevelPropertyValue: false,
                insideResourceValue: false,
                propertyName: edmProperty.Name).ConfigureAwait(false);

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
        /// Asynchronously read an open property.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataJsonReaderNestedInfo"/> or null.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  The reader is positioned on the first node of the property value
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private async Task<ODataJsonReaderNestedInfo> InnerReadUndeclaredPropertyAsync(
            IODataJsonReaderResourceState resourceState,
            IEdmStructuredType owningStructuredType,
            string propertyName,
            bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            this.JsonReader.AssertNotBuffering();

            if (!propertyWithValue)
            {
                return ReadNestedPropertyInfoWithoutValue(resourceState, propertyName, null);
            }

            object propertyValue = null;
            bool insideResourceValue = false;
            string outerPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);
            string payloadTypeName = await this.TryReadOrPeekPayloadTypeAsync(resourceState.PropertyAndAnnotationCollector, propertyName, insideResourceValue)
                .ConfigureAwait(false);
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
                    expectStructuredType: null,
                    defaultPrimitivePayloadType: null,
                    expectedTypeReference: null,
                    payloadTypeName: payloadTypeName,
                    model: this.Model,
                    typeKindFromPayloadFunc: this.GetNonEntityValueKind,
                    targetTypeKind: out targetTypeKind,
                    typeAnnotation: out typeAnnotation);
            }

            ODataJsonReaderNestedInfo nestedResourceInfo = await this.TryReadAsStreamAsync(resourceState, null, payloadTypeReference, propertyName)
                .ConfigureAwait(false);
            if (nestedResourceInfo != null)
            {
                return nestedResourceInfo;
            }

            payloadTypeReference = ResolveUntypedType(
                this.JsonReader.NodeType,
                await this.JsonReader.GetValueAsync().ConfigureAwait(false),
                payloadTypeName,
                payloadTypeReference,
                this.MessageReaderSettings.PrimitiveTypeResolver,
                this.MessageReaderSettings.ReadUntypedAsString,
                !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

            bool isCollection = payloadTypeReference.IsCollection();
            IEdmStructuredType payloadTypeOrItemType = payloadTypeReference.ToStructuredType();
            if (payloadTypeOrItemType != null)
            {
                // Complex property or collection of complex property.
                await ValidateExpandedNestedResourceInfoPropertyValueAsync(
                    this.JsonReader,
                    isCollection,
                    propertyName,
                    payloadTypeReference).ConfigureAwait(false);
                if (isCollection)
                {
                    return ReadNonExpandedResourceSetNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }
                else
                {
                    return ReadNonExpandedResourceNestedResourceInfo(resourceState, null, payloadTypeOrItemType, propertyName);
                }
            }

            if (!(payloadTypeReference is IEdmUntypedTypeReference))
            {
                this.JsonReader.AssertNotBuffering();
                propertyValue = await this.ReadNonEntityValueAsync(
                    outerPayloadTypeName,
                    payloadTypeReference,
                    propertyAndAnnotationCollector: null,
                    collectionValidator: null,
                    validateNullValue: true,
                    isTopLevelPropertyValue: false,
                    insideResourceValue: false,
                    propertyName: propertyName,
                    isDynamicProperty: true).ConfigureAwait(false);
            }
            else
            {
                propertyValue = await this.JsonReader.ReadAsUntypedOrNullValueAsync()
                    .ConfigureAwait(false);
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
        /// Asynchronously read an undeclared property. That is a property which is not declared by the model, but the owning type is not an open type.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="propertyName">The name of the open property to read.</param>
        /// <param name="propertyWithValue">true if the property has a value, false if it doesn't.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a nested resource info instance
        /// if the property read is a nested resource info which should be reported to the caller;
        /// otherwise null if the property was either ignored or read and
        /// added to the list of properties on the resource.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue:  propertyWithValue is true and the reader is positioned on the first node of the property value.
        ///                 JsonNodeType.StartObject:
        ///                 JsonNodeType.StartArray:
        ///                 JsonNodeType.Property:        propertyWithValue is false and the reader is positioned on the node after the property.
        ///                 JsonNodeType.EndObject:
        /// Post-Condition: JsonNodeType.Property:    the next property of the resource
        ///                 JsonNodeType.EndObject:   the end-object node of the resource
        /// </remarks>
        private async Task<ODataJsonReaderNestedInfo> ReadUndeclaredPropertyAsync(
            IODataJsonReaderResourceState resourceState,
            string propertyName,
            bool propertyWithValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
#if DEBUG
            if (propertyWithValue)
            {
                this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.Property, JsonNodeType.StartObject, JsonNodeType.StartArray);
            }
            else
            {
                this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            }
#endif
            // Undeclared property
            // Detect whether it's a link property or value property.
            // Link properties are stream properties and deferred links.
            IDictionary<string, object> odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName);
            object propertyAnnotationValue;

            // If the property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' annotation, read it as a stream property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaEditLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaReadLink, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaContentType, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataMediaETag, out propertyAnnotationValue))
            {
                // Add the stream reference property
                ODataStreamReferenceValue streamReferenceValue = this.ReadStreamPropertyValue(resourceState, propertyName);
                AddResourceProperty(resourceState, propertyName, streamReferenceValue);

                if (propertyWithValue)
                {
                    ODataStreamPropertyInfo propertyInfo = this.ReadStreamPropertyInfo(resourceState, propertyName);
                    if (!IsJsonStream(propertyInfo))
                    {
                        // Not a JSON Stream, so skip over property name in JSON reader
                        await this.JsonReader.ReadAsync()
                            .ConfigureAwait(false);
                    }

                    return new ODataJsonReaderNestedPropertyInfo(propertyInfo, null);
                }

                return null;
            }

            // It's not a JSON stream, so skip the property name.
            // If the property does not have a value we will have already skipped the name
            if (propertyWithValue)
            {
                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);
            }

            // If the property has 'odata.navigationLink' or 'odata.associationLink' annotation, read it as a navigation property
            if (odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataNavigationLinkUrl, out propertyAnnotationValue) ||
                odataPropertyAnnotations.TryGetValue(ODataAnnotationNames.ODataAssociationLinkUrl, out propertyAnnotationValue))
            {
                // Read it as a deferred link - we never read the expanded content.
                ODataJsonReaderNestedResourceInfo navigationLinkInfo = ReadDeferredNestedResourceInfo(resourceState, propertyName, navigationProperty: null);
                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(navigationLinkInfo.NestedResourceInfo);

                // If the property is expanded, ignore the content if we're asked to do so.
                if (propertyWithValue)
                {
                    await ValidateExpandedNestedResourceInfoPropertyValueAsync(
                        this.JsonReader,
                        null,
                        propertyName,
                        resourceState.ResourceType.ToTypeReference()).ConfigureAwait(false);

                    // Since we marked the nested resource info as deferred the reader will not try to read its content
                    // instead it will behave as if it was a real deferred link (without a property value).
                    // So skip the value here to move to the next property in the payload, which will look exactly the same
                    // as if the nested resource info was deferred.
                    await this.JsonReader.SkipValueAsync()
                        .ConfigureAwait(false);
                }

                return navigationLinkInfo;
            }

            if (resourceState.ResourceType.IsOpen)
            {
                // Open property - read it as such.
                ODataJsonReaderNestedInfo nestedResourceInfo = await this.InnerReadUndeclaredPropertyAsync(
                    resourceState,
                    resourceState.ResourceType,
                    propertyName,
                    propertyWithValue).ConfigureAwait(false);
                return nestedResourceInfo;
            }

            // Property without a value can't be ignored if we don't know what it is.
            if (!propertyWithValue)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_PropertyWithoutValueWithUnknownType, propertyName));
            }

            // Validate that the property doesn't have unrecognized annotations
            // We ignore the type name since we might not have the full model and thus might not be able to resolve it correctly.
            ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, propertyName);

            if (!this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
            {
                bool isTopLevelPropertyValue = false;
                ODataJsonReaderNestedResourceInfo nestedResourceInfo = await this.InnerReadUndeclaredPropertyAsync(
                    resourceState,
                    propertyName,
                    isTopLevelPropertyValue).ConfigureAwait(false);
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
        /// Asynchronously reads one operation for the resource being read.
        /// </summary>
        /// <param name="readerContext">The Json operation deserializer context.</param>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:   first node of the operation value.
        /// Post-Condition: JsonNodeType.Property:      the property after the current operation being read when there is one target for the operation.
        ///                 JsonNodeType.StartObject:   the first node of the next operation value when there are multiple targets for the operation.
        ///                 JsonNodeType.EndArray:      the end-array of the operation values when there are multiple target for the operation.
        /// </remarks>
        private async Task ReadSingleOperationValueAsync(
            IODataJsonOperationsDeserializerContext readerContext,
            IODataJsonReaderResourceState resourceState,
            string metadataReferencePropertyName,
            bool insideArray)
        {
            Debug.Assert(readerContext != null, "readerContext != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (readerContext.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(
                    Error.Format(SRResources.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue,
                        metadataReferencePropertyName,
                        readerContext.JsonReader.NodeType));
            }

            // Read over the start-object node of the metadata object for the operations
            await readerContext.JsonReader.ReadStartObjectAsync()
                .ConfigureAwait(false);

            ODataOperation operation = this.CreateODataOperationAndAddToEntry(readerContext, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
                {
                    await readerContext.JsonReader.ReadPropertyNameAsync()
                        .ConfigureAwait(false);
                    await readerContext.JsonReader.SkipValueAsync()
                        .ConfigureAwait(false);
                }

                await readerContext.JsonReader.ReadEndObjectAsync()
                    .ConfigureAwait(false);
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (readerContext.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(
                    await readerContext.JsonReader.ReadPropertyNameAsync().ConfigureAwait(false));
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation,
                                    operationPropertyName,
                                    metadataReferencePropertyName));
                        }

                        string titleString = await readerContext.JsonReader.ReadStringValueAsync(JsonConstants.ODataOperationTitleName)
                            .ConfigureAwait(false);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation,
                                    operationPropertyName,
                                    metadataReferencePropertyName));
                        }

                        string targetString = await readerContext.JsonReader.ReadStringValueAsync(JsonConstants.ODataOperationTargetName)
                            .ConfigureAwait(false);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = readerContext.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // Skip over all unknown properties and read the next property or
                        // the end of the metadata for the current propertyName
                        await readerContext.JsonReader.SkipValueAsync()
                            .ConfigureAwait(false);
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_OperationMissingTargetProperty, metadataReferencePropertyName));
            }

            // read the end-object node of the target / title pair
            await readerContext.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            // Sets the metadata builder to evaluate by convention any operation property that's not on the wire.
            // Note we must only set this after the operation is read from the wire since we lose the ability to tell
            // what was on the wire and what is being dynamically computed.
            this.SetMetadataBuilder(resourceState, operation);
        }

        /// <summary>
        /// Asynchronously reads one operation for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <param name="insideArray">true if the operation value is inside an array, i.e. multiple targets for the operation; false otherwise.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadSingleOperationValueAsync(ODataResourceSet resourceSet, string metadataReferencePropertyName, bool insideArray)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(ODataJsonUtils.IsMetadataReferenceProperty(metadataReferencePropertyName), "ODataJsonReaderUtils.IsMetadataReferenceProperty(metadataReferencePropertyName)");

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                throw new ODataException(
                    Error.Format(SRResources.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue, metadataReferencePropertyName, this.JsonReader.NodeType));
            }

            // read over the start-object node of the metadata object for the operations
            await this.JsonReader.ReadStartObjectAsync()
                .ConfigureAwait(false);

            ODataOperation operation = this.CreateODataOperationAndAddToResourceSet(resourceSet, metadataReferencePropertyName);

            // Ignore the unrecognized operation.
            if (operation == null)
            {
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    await this.JsonReader.ReadPropertyNameAsync()
                        .ConfigureAwait(false);
                    await this.JsonReader.SkipValueAsync()
                        .ConfigureAwait(false);
                }

                await this.JsonReader.ReadEndObjectAsync()
                    .ConfigureAwait(false);
                return;
            }

            Debug.Assert(operation.Metadata != null, "operation.Metadata != null");

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string operationPropertyName = ODataAnnotationNames.RemoveAnnotationPrefix(
                    await this.JsonReader.ReadPropertyNameAsync().ConfigureAwait(false));
                switch (operationPropertyName)
                {
                    case JsonConstants.ODataOperationTitleName:
                        if (operation.Title != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation,
                                    operationPropertyName,
                                    metadataReferencePropertyName));
                        }

                        string titleString = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataOperationTitleName)
                            .ConfigureAwait(false);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(titleString, operationPropertyName, metadataReferencePropertyName);
                        operation.Title = titleString;
                        break;

                    case JsonConstants.ODataOperationTargetName:
                        if (operation.Target != null)
                        {
                            throw new ODataException(
                                Error.Format(SRResources.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation,
                                    operationPropertyName,
                                    metadataReferencePropertyName));
                        }

                        string targetString = await this.JsonReader.ReadStringValueAsync(JsonConstants.ODataOperationTargetName)
                            .ConfigureAwait(false);
                        ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(targetString, operationPropertyName, metadataReferencePropertyName);
                        operation.Target = this.ProcessUriFromPayload(targetString);
                        break;

                    default:
                        // skip over all unknown properties and read the next property or
                        // the end of the metadata for the current propertyName
                        await this.JsonReader.SkipValueAsync()
                            .ConfigureAwait(false);
                        break;
                }
            }

            if (operation.Target == null && insideArray)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_OperationMissingTargetProperty, metadataReferencePropertyName));
            }

            // Read the end-object node of the target / title pair
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously read the metadata reference property value for the resource being read.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:      first node of the metadata reference property's value. Currently
        ///                                             actions and functions are the only supported metadata reference property,
        ///                                             we will throw if this is not a start object or start array node.
        /// Post-Condition: JsonNodeType.Property:      the property after the annotation value
        ///                 JsonNodeType.EndObject:     the end-object of the resource
        /// </remarks>
        private async Task ReadMetadataReferencePropertyValueAsync(IODataJsonReaderResourceState resourceState, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(resourceState.Resource != null, "resourceState.Resource != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator, StringComparison.Ordinal) > -1, "metadataReferencePropertyName.IndexOf(ODataJsonConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            IODataJsonOperationsDeserializerContext readerContext = new OperationsDeserializerContext(resourceState.Resource, this);

            bool insideArray = false;
            if (readerContext.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                await readerContext.JsonReader.ReadStartArrayAsync()
                    .ConfigureAwait(false);
                insideArray = true;
            }

            do
            {
                await this.ReadSingleOperationValueAsync(readerContext, resourceState, metadataReferencePropertyName, insideArray)
                    .ConfigureAwait(false);
            }
            while (insideArray && readerContext.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                await readerContext.JsonReader.ReadEndArrayAsync()
                    .ConfigureAwait(false);
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously read the metadata reference property value for the resource set being read.
        /// </summary>
        /// <param name="resourceSet">The resource set to read.</param>
        /// <param name="metadataReferencePropertyName">The name of the metadata reference property being read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private async Task ReadMetadataReferencePropertyValueAsync(ODataResourceSet resourceSet, string metadataReferencePropertyName)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(metadataReferencePropertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator, StringComparison.Ordinal) > -1, "metadataReferencePropertyName.IndexOf(ODataJsonConstants.ContextUriFragmentIndicator) > -1");
            this.JsonReader.AssertNotBuffering();

            this.ValidateCanReadMetadataReferenceProperty();

            // Validate that the property name is a valid absolute URI or a valid URI fragment.
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(this.ContextUriParseResult.MetadataDocumentUri, metadataReferencePropertyName);

            bool insideArray = false;
            if (this.JsonReader.NodeType == JsonNodeType.StartArray)
            {
                await this.JsonReader.ReadStartArrayAsync()
                    .ConfigureAwait(false);
                insideArray = true;
            }

            do
            {
                await this.ReadSingleOperationValueAsync(resourceSet, metadataReferencePropertyName, insideArray)
                    .ConfigureAwait(false);
            }
            while (insideArray && this.JsonReader.NodeType != JsonNodeType.EndArray);

            if (insideArray)
            {
                await this.JsonReader.ReadEndArrayAsync()
                    .ConfigureAwait(false);
            }

            this.JsonReader.AssertNotBuffering();
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
        }
    }
}
