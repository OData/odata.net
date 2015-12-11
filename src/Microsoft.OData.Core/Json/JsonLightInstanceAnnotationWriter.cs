﻿//---------------------------------------------------------------------
// <copyright file="JsonLightInstanceAnnotationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion

    /// <summary>
    /// Class responsible for writing a collection of <see cref="ODataInstanceAnnotation"/>.
    /// </summary>
    internal sealed class JsonLightInstanceAnnotationWriter
    {
        /// <summary>
        /// Value serializer, responsible for serializing the annotation values.
        /// </summary>
        private readonly IODataJsonLightValueSerializer valueSerializer;

        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private readonly JsonLightTypeNameOracle typeNameOracle;

        /// <summary>
        /// JsonWriter instance to use for writing term names.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonLightODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// Constructs a <see cref="JsonLightInstanceAnnotationWriter"/> that can write a collection of <see cref="ODataInstanceAnnotation"/>.
        /// </summary>
        /// <param name="valueSerializer">The <see cref="IODataJsonLightValueSerializer"/> to use for writing values of instance annotations.
        /// The <see cref="IJsonWriter"/> that is also used internally will be acquired from the this instance.</param>
        /// <param name="typeNameOracle">The oracle to use to determine the type name to write for entries and values.</param>
        internal JsonLightInstanceAnnotationWriter(IODataJsonLightValueSerializer valueSerializer, JsonLightTypeNameOracle typeNameOracle)
        {
            Debug.Assert(valueSerializer != null, "valueSerializer should not be null");
            this.valueSerializer = valueSerializer;
            this.typeNameOracle = typeNameOracle;
            this.jsonWriter = this.valueSerializer.JsonWriter;
            this.odataAnnotationWriter = new JsonLightODataAnnotationWriter(this.jsonWriter, valueSerializer.Settings.ODataSimplified);
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        internal void WriteInstanceAnnotations(IEnumerable<ODataInstanceAnnotation> instanceAnnotations, InstanceAnnotationWriteTracker tracker, bool ignoreFilter = false, string propertyName = null)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            Debug.Assert(tracker != null, "tracker should not be null if we called this");

            HashSet<string> instanceAnnotationNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var annotation in instanceAnnotations)
            {
                if (!instanceAnnotationNames.Add(annotation.Name))
                {
                    throw new ODataException(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection(annotation.Name));
                }

                if (!tracker.IsAnnotationWritten(annotation.Name))
                {
                    this.WriteInstanceAnnotation(annotation, ignoreFilter, propertyName);
                    tracker.MarkAnnotationWritten(annotation.Name);
                }
            }
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        internal void WriteInstanceAnnotations(IEnumerable<ODataInstanceAnnotation> instanceAnnotations, string propertyName = null)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker(), false, propertyName);
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/> of error.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        internal void WriteInstanceAnnotationsForError(IEnumerable<ODataInstanceAnnotation> instanceAnnotations)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker(), true);
        }

        /// <summary>
        /// Writes an instance annotation.
        /// </summary>
        /// <param name="instanceAnnotation">The instance annotation to write.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        internal void WriteInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation, bool ignoreFilter = false, string propertyName = null)
        {
            string name = instanceAnnotation.Name;
            ODataValue value = instanceAnnotation.Value;
            Debug.Assert(!string.IsNullOrEmpty(name), "name should not be null or empty");
            Debug.Assert(!ODataAnnotationNames.IsODataAnnotationName(name), "A reserved name cannot be used as instance annotation key");
            Debug.Assert(value != null, "value should not be null because we use ODataNullValue for null instead");
            Debug.Assert(!(value is ODataStreamReferenceValue), "!(value is ODataStreamReferenceValue) -- ODataInstanceAnnotation and InstanceAnnotationCollection will throw if the value is a stream value.");
            Debug.Assert(this.valueSerializer.Model != null, "this.valueSerializer.Model != null");

            if (!ignoreFilter && this.valueSerializer.Settings.ShouldSkipAnnotation(name))
            {
                return;
            }

            IEdmTypeReference expectedType = MetadataUtils.LookupTypeOfValueTerm(name, this.valueSerializer.Model);

            if (value is ODataNullValue)
            {
                if (expectedType != null && !expectedType.IsNullable)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(instanceAnnotation.Name, expectedType.FullName()));
                }

                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteNullValue();
                return;
            }

            // If we didn't find an expected type from looking up the term in the model, treat this value the same way we would for open property values. 
            // That is, write the type name (unless its a primitive value with a JSON-native type).  If we did find an expected type, treat the annotation value like a 
            // declared property with an expected type. This will still write out the type if the value type is more derived than the declared type, for example.
            bool treatLikeOpenProperty = expectedType == null;

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteComplexValue(complexValue, expectedType, false /*isTopLevel*/, treatLikeOpenProperty, this.valueSerializer.CreateDuplicatePropertyNamesChecker());
                return;
            }

            IEdmTypeReference typeFromValue = TypeNameOracle.ResolveAndValidateTypeNameForValue(this.valueSerializer.Model, expectedType, value, treatLikeOpenProperty);
            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                string collectionTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(collectionValue, expectedType, typeFromValue, treatLikeOpenProperty);
                if (collectionTypeNameToWrite != null)
                {
                    this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, collectionTypeNameToWrite);
                }

                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteCollectionValue(collectionValue, expectedType, false /*isTopLevelProperty*/, false /*isInUri*/, treatLikeOpenProperty);
                return;
            }

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            Debug.Assert(primitiveValue != null, "Did we add a new subclass of ODataValue?");

            string primitiveTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(primitiveValue, expectedType, typeFromValue, treatLikeOpenProperty);
            if (primitiveTypeNameToWrite != null)
            {
                this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, primitiveTypeNameToWrite);
            }

            this.WriteInstanceAnnotationName(propertyName, name);
            this.valueSerializer.WritePrimitiveValue(primitiveValue.Value, expectedType);
        }

        /// <summary>
        /// Write the name of the instance annotation
        /// </summary>
        /// <param name="propertyName">The name of the property this instance annotation applied to</param>
        /// <param name="annotationName">The name of the instance annotation</param>
        private void WriteInstanceAnnotationName(string propertyName, string annotationName)
        {
            if (propertyName != null)
            {
                this.jsonWriter.WritePropertyAnnotationName(propertyName, annotationName);
            }
            else
            {
                this.jsonWriter.WriteInstanceAnnotationName(annotationName);
            }
        }
    }
}