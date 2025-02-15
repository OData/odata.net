﻿//---------------------------------------------------------------------
// <copyright file="JsonInstanceAnnotationWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    #endregion

    /// <summary>
    /// Class responsible for writing a collection of <see cref="ODataInstanceAnnotation"/>.
    /// </summary>
    internal sealed class JsonInstanceAnnotationWriter
    {
        /// <summary>
        /// Value serializer, responsible for serializing the annotation values.
        /// </summary>
        private readonly ODataJsonValueSerializer valueSerializer;

        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private readonly JsonTypeNameOracle typeNameOracle;

        /// <summary>
        /// JsonWriter instance to use for writing term names.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// OData annotation writer.
        /// </summary>
        private readonly JsonODataAnnotationWriter odataAnnotationWriter;

        /// <summary>
        /// The writer validator used during writing.
        /// </summary>
        private readonly IWriterValidator writerValidator;

        /// <summary>
        /// Constructs a <see cref="JsonInstanceAnnotationWriter"/> that can write a collection of <see cref="ODataInstanceAnnotation"/>.
        /// </summary>
        /// <param name="valueSerializer">The <see cref="ODataJsonValueSerializer"/> to use for writing values of instance annotations.
        /// The <see cref="IJsonWriter"/> that is also used internally will be acquired from the this instance.</param>
        /// <param name="typeNameOracle">The oracle to use to determine the type name to write for entries and values.</param>
        internal JsonInstanceAnnotationWriter(ODataJsonValueSerializer valueSerializer, JsonTypeNameOracle typeNameOracle)
        {
            Debug.Assert(valueSerializer != null, "valueSerializer should not be null");
            this.valueSerializer = valueSerializer;
            this.typeNameOracle = typeNameOracle;
            this.jsonWriter = this.valueSerializer.JsonWriter;
            this.odataAnnotationWriter = this.valueSerializer.ODataAnnotationWriter;
            this.writerValidator = this.valueSerializer.MessageWriterSettings.Validator;
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        internal void WriteInstanceAnnotations(
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            InstanceAnnotationWriteTracker tracker,
            bool ignoreFilter = false,
            string propertyName = null)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            Debug.Assert(tracker != null, "tracker should not be null if we called this");

            // this method runs in a hot path hence the optimizations

            if (instanceAnnotations.Count == 0)
            {
                return;
            }

            HashSet<string> instanceAnnotationNames = new HashSet<string>(StringComparer.Ordinal);

            // this method is called with a List most of the time
            // foreach against a List does not allocate the enumerator to the heap,
            // but foreach against an IEnumerable does
            if (instanceAnnotations is List<ODataInstanceAnnotation> instanceAnnotationsList)
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotationsList)
                {
                    this.WriteAndTrackInstanceAnnotation(annotation, tracker, instanceAnnotationNames, ignoreFilter, propertyName);
                }
            }
            else
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotations)
                {
                    this.WriteAndTrackInstanceAnnotation(annotation, tracker, instanceAnnotationNames, ignoreFilter, propertyName);
                }
            }
        }


        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        /// <param name="isUndeclaredProperty">If writing an undeclared property.</param>
        internal void WriteInstanceAnnotations(
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            string propertyName = null,
            bool isUndeclaredProperty = false)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            if (isUndeclaredProperty)
            {
                // write undeclared property's all annotations
                WriteInstanceAnnotationsForUndeclaredProperty(instanceAnnotations, propertyName);
            }
            else
            {
                this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker(), false, propertyName);
            }
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/> of error.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        internal void WriteInstanceAnnotationsForError(ICollection<ODataInstanceAnnotation> instanceAnnotations)
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
        internal void WriteInstanceAnnotation(
            ODataInstanceAnnotation instanceAnnotation,
            bool ignoreFilter = false,
            string propertyName = null)
        {
            string name = instanceAnnotation.Name;
            ODataValue value = instanceAnnotation.Value;
            Debug.Assert(!string.IsNullOrEmpty(name), "name should not be null or empty");
            Debug.Assert(value != null, "value should not be null because we use ODataNullValue for null instead");
            Debug.Assert(!(value is ODataStreamReferenceValue), "!(value is ODataStreamReferenceValue) -- ODataInstanceAnnotation and InstanceAnnotationCollection will throw if the value is a stream value.");
            Debug.Assert(this.valueSerializer.Model != null, "this.valueSerializer.Model != null");

            if (!ignoreFilter && this.valueSerializer.MessageWriterSettings.ShouldSkipAnnotation(name))
            {
                return;
            }

            IEdmTypeReference expectedType = MetadataUtils.LookupTypeOfTerm(name, this.valueSerializer.Model);

            if (value is ODataNullValue)
            {
                if (expectedType != null && !expectedType.IsNullable)
                {
                    throw new ODataException(
                        Error.Format(SRResources.JsonInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation,
                            instanceAnnotation.Name, expectedType.FullName()));
                }

                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteNullValue();
                return;
            }

            // If we didn't find an expected type from looking up the term in the model, treat this value the same way we would for open property values.
            // That is, write the type name (unless its a primitive value with a JSON-native type).  If we did find an expected type, treat the annotation value like a
            // declared property with an expected type. This will still write out the type if the value type is more derived than the declared type, for example.
            bool treatLikeOpenProperty = expectedType == null;

            ODataResourceValue resourceValue = value as ODataResourceValue;
            if (resourceValue != null)
            {
                IDuplicatePropertyNameChecker duplicatePropertyNameChecker = this.valueSerializer.GetDuplicatePropertyNameChecker();
                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteResourceValue(resourceValue,
                    expectedType,
                    treatLikeOpenProperty,
                    duplicatePropertyNameChecker);
                this.valueSerializer.ReturnDuplicatePropertyNameChecker(duplicatePropertyNameChecker);
                return;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                IEdmTypeReference typeFromCollectionValue = (IEdmCollectionTypeReference)TypeNameOracle.ResolveAndValidateTypeForCollectionValue(
                    this.valueSerializer.Model, expectedType, collectionValue, treatLikeOpenProperty, this.writerValidator);
                string collectionTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(collectionValue, expectedType, typeFromCollectionValue, treatLikeOpenProperty);
                if (collectionTypeNameToWrite != null)
                {
                    this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, collectionTypeNameToWrite);
                }

                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteCollectionValue(collectionValue, expectedType, typeFromCollectionValue, false /*isTopLevelProperty*/, false /*isInUri*/, treatLikeOpenProperty);
                return;
            }

            ODataUntypedValue untypedValue = value as ODataUntypedValue;
            if (untypedValue != null)
            {
                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteUntypedValue(untypedValue);
                return;
            }

            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                this.WriteInstanceAnnotationName(propertyName, name);
                this.valueSerializer.WriteEnumValue(enumValue, expectedType);
                return;
            }

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            Debug.Assert(primitiveValue != null, "Did we add a new subclass of ODataValue?");

            IEdmTypeReference typeFromPrimitiveValue = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);

            string primitiveTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(primitiveValue, expectedType, typeFromPrimitiveValue, treatLikeOpenProperty);
            if (primitiveTypeNameToWrite != null)
            {
                this.odataAnnotationWriter.WriteODataTypePropertyAnnotation(name, primitiveTypeNameToWrite);
            }

            this.WriteInstanceAnnotationName(propertyName, name);
            this.valueSerializer.WritePrimitiveValue(primitiveValue.Value, typeFromPrimitiveValue, expectedType);
        }

        /// <summary>
        /// Asynchronously writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal Task WriteInstanceAnnotationsAsync(
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            InstanceAnnotationWriteTracker tracker,
            bool ignoreFilter = false,
            string propertyName = null)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            Debug.Assert(tracker != null, "tracker should not be null if we called this");

            // this method runs in a hot path hence the optimizations

            if (instanceAnnotations.Count == 0)
            {
                return TaskUtils.CompletedTask;
            }

            HashSet<string> instanceAnnotationNames = new HashSet<string>(StringComparer.Ordinal);

            // this method is called with a List most of the time
            // foreach against a List does not allocate the enumerator to the heap,
            // but foreach against an IEnumerable does due to boxing
            if (instanceAnnotations is List<ODataInstanceAnnotation> instanceAnnotationsList)
            {
                return WriteInstanceAnnotationsListAsync(instanceAnnotationsList, tracker, instanceAnnotationNames, ignoreFilter, propertyName);

                async Task WriteInstanceAnnotationsListAsync(
                    List<ODataInstanceAnnotation> innerInstanceAnnotationsList,
                    InstanceAnnotationWriteTracker innerInstanceAnnotationWriteTracker,
                    HashSet<string> innerInstanceAnnotationNames,
                    bool innerIgnoreFilter,
                    string innerPropertyName)
                {
                    foreach (ODataInstanceAnnotation annotation in innerInstanceAnnotationsList)
                    {
                        await this.WriteAndTrackInstanceAnnotationAsync(
                            annotation,
                            innerInstanceAnnotationWriteTracker,
                            innerInstanceAnnotationNames,
                            innerIgnoreFilter,
                            innerPropertyName).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                return WriteInstanceAnnotationsInnerAsync(instanceAnnotations, tracker, instanceAnnotationNames, ignoreFilter, propertyName);

                async Task WriteInstanceAnnotationsInnerAsync(
                    ICollection<ODataInstanceAnnotation> innerInstanceAnnotations,
                    InstanceAnnotationWriteTracker innerInstanceAnnotationWriteTracker,
                    HashSet<string> innerInstanceAnnotationNames,
                    bool innerIgnoreFilter,
                    string innerPropertyName)
                {
                    foreach (ODataInstanceAnnotation annotation in innerInstanceAnnotations)
                    {
                        await this.WriteAndTrackInstanceAnnotationAsync(
                            annotation,
                            innerInstanceAnnotationWriteTracker,
                            innerInstanceAnnotationNames,
                            innerIgnoreFilter,
                            innerPropertyName).ConfigureAwait(false);
                    }
                }
            }
        }


        /// <summary>
        /// Asynchronously writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to</param>
        /// <param name="isUndeclaredProperty">If writing an undeclared property.</param>
        internal Task WriteInstanceAnnotationsAsync(
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            string propertyName = null,
            bool isUndeclaredProperty = false)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            if (isUndeclaredProperty)
            {
                // write undeclared property's all annotations
                return WriteInstanceAnnotationsForUndeclaredPropertyAsync(instanceAnnotations, propertyName);
            }
            else
            {
                return this.WriteInstanceAnnotationsAsync(instanceAnnotations, new InstanceAnnotationWriteTracker(), false, propertyName);
            }
        }

        /// <summary>
        /// Asynchronously writes all the instance annotations specified in <paramref name="instanceAnnotations"/> of error.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        internal Task WriteInstanceAnnotationsForErrorAsync(ICollection<ODataInstanceAnnotation> instanceAnnotations)
        {
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            return this.WriteInstanceAnnotationsAsync(instanceAnnotations, new InstanceAnnotationWriteTracker(), true);
        }

        /// <summary>
        /// Asynchronously writes an instance annotation.
        /// </summary>
        /// <param name="instanceAnnotation">The instance annotation to write.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies tos</param>
        internal async Task WriteInstanceAnnotationAsync(
            ODataInstanceAnnotation instanceAnnotation,
            bool ignoreFilter = false,
            string propertyName = null)
        {
            string annotationName = instanceAnnotation.Name;
            ODataValue annotationValue = instanceAnnotation.Value;
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "annotationName should not be null or empty");
            Debug.Assert(annotationValue != null, "annotationValue should not be null because we use ODataNullValue for null instead");
            Debug.Assert(!(annotationValue is ODataStreamReferenceValue), "!(annotationValue is ODataStreamReferenceValue) -- ODataInstanceAnnotation and InstanceAnnotationCollection will throw if the value is a stream value.");
            Debug.Assert(this.valueSerializer.Model != null, "this.valueSerializer.Model != null");

            if (!ignoreFilter && this.valueSerializer.MessageWriterSettings.ShouldSkipAnnotation(annotationName))
            {
                return;
            }

            IEdmTypeReference expectedType = MetadataUtils.LookupTypeOfTerm(annotationName, this.valueSerializer.Model);

            if (annotationValue is ODataNullValue)
            {
                if (expectedType != null && !expectedType.IsNullable)
                {
                    throw new ODataException(
                        Error.Format(SRResources.JsonInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation,
                            annotationName, expectedType.FullName()));
                }

                await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                        .ConfigureAwait(false);
                await this.valueSerializer.WriteNullValueAsync()
                        .ConfigureAwait(false);
                return;
            }

            // If we didn't find an expected type from looking up the term in the model, treat this value the same way we would for open property values.
            // That is, write the type name (unless its a primitive value with a JSON-native type).  If we did find an expected type, treat the annotation value like a
            // declared property with an expected type. This will still write out the type if the value type is more derived than the declared type, for example.
            bool treatLikeOpenProperty = expectedType == null;

            ODataResourceValue resourceValue = annotationValue as ODataResourceValue;
            if (resourceValue != null)
            {
                IDuplicatePropertyNameChecker duplicatePropertyNameChecker = this.valueSerializer.GetDuplicatePropertyNameChecker();
                await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                    .ConfigureAwait(false);
                await this.valueSerializer.WriteResourceValueAsync(resourceValue,
                    expectedType,
                    treatLikeOpenProperty,
                    duplicatePropertyNameChecker).ConfigureAwait(false);
                this.valueSerializer.ReturnDuplicatePropertyNameChecker(duplicatePropertyNameChecker);

                return;
            }

            ODataCollectionValue collectionValue = annotationValue as ODataCollectionValue;
            if (collectionValue != null)
            {
                IEdmTypeReference typeFromCollectionValue = (IEdmCollectionTypeReference)TypeNameOracle.ResolveAndValidateTypeForCollectionValue(
                    this.valueSerializer.Model, expectedType, collectionValue, treatLikeOpenProperty, this.writerValidator);
                string collectionTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(collectionValue, expectedType, typeFromCollectionValue, treatLikeOpenProperty);
                if (collectionTypeNameToWrite != null)
                {
                    await this.odataAnnotationWriter.WriteODataTypePropertyAnnotationAsync(annotationName, collectionTypeNameToWrite)
                        .ConfigureAwait(false);
                }

                await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                    .ConfigureAwait(false);
                await this.valueSerializer.WriteCollectionValueAsync(
                    collectionValue,
                    expectedType,
                    typeFromCollectionValue,
                    false /*isTopLevelProperty*/,
                    false /*isInUri*/,
                    treatLikeOpenProperty).ConfigureAwait(false);
                return;
            }

            ODataUntypedValue untypedValue = annotationValue as ODataUntypedValue;
            if (untypedValue != null)
            {
                await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                    .ConfigureAwait(false);
                await this.valueSerializer.WriteUntypedValueAsync(untypedValue)
                    .ConfigureAwait(false);
                return;
            }

            ODataEnumValue enumValue = annotationValue as ODataEnumValue;
            if (enumValue != null)
            {
                await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                    .ConfigureAwait(false);
                await this.valueSerializer.WriteEnumValueAsync(enumValue, expectedType)
                    .ConfigureAwait(false);
                return;
            }

            ODataPrimitiveValue primitiveValue = annotationValue as ODataPrimitiveValue;
            Debug.Assert(primitiveValue != null, "Did we add a new subclass of ODataValue?");

            IEdmTypeReference typeFromPrimitiveValue = TypeNameOracle.ResolveAndValidateTypeForPrimitiveValue(primitiveValue);

            string primitiveTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(primitiveValue, expectedType, typeFromPrimitiveValue, treatLikeOpenProperty);
            if (primitiveTypeNameToWrite != null)
            {
                await this.odataAnnotationWriter.WriteODataTypePropertyAnnotationAsync(annotationName, primitiveTypeNameToWrite)
                    .ConfigureAwait(false);
            }

            await this.WriteInstanceAnnotationNameAsync(propertyName, annotationName)
                .ConfigureAwait(false);
            await this.valueSerializer.WritePrimitiveValueAsync(primitiveValue.Value, typeFromPrimitiveValue, expectedType)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Write instance annotation if not already written.
        /// </summary>
        /// <param name="annotation">The instance annotation to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        /// <param name="instanceAnnotationNames">Set used to detect a duplicate annotation.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to.</param>
        private void WriteAndTrackInstanceAnnotation(
            ODataInstanceAnnotation annotation,
            InstanceAnnotationWriteTracker tracker,
            HashSet<string> instanceAnnotationNames,
            bool ignoreFilter = false,
            string propertyName = null)
        {

            if (!instanceAnnotationNames.Add(annotation.Name))
            {
                throw new ODataException(Error.Format(SRResources.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection, annotation.Name));
            }

            if (!tracker.IsAnnotationWritten(annotation.Name)
                        && (!ODataAnnotationNames.IsODataAnnotationName(annotation.Name) || ODataAnnotationNames.IsUnknownODataAnnotationName(annotation.Name)))
            {
                this.WriteInstanceAnnotation(annotation, ignoreFilter, propertyName);
                tracker.MarkAnnotationWritten(annotation.Name);
            }
        }

        /// <summary>
        /// Write all the instance annotations specified in <paramref name="instanceAnnotations"/> for the
        /// undeclared property called <paramref name="propertyName"/>
        /// </summary>
        /// <param name="instanceAnnotations">The collection of instance annotations</param>
        /// <param name="propertyName">The name of the property the instance annotations apply to</param>
        private void WriteInstanceAnnotationsForUndeclaredProperty(ICollection<ODataInstanceAnnotation> instanceAnnotations, string propertyName)
        {
            // write all the annotations of the undeclared property
            // optimize the foreach when instanceAnnotations is a List
            if (instanceAnnotations is List<ODataInstanceAnnotation> instanceAnnotationsList)
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotationsList)
                {
                    this.WriteInstanceAnnotation(annotation, true, propertyName);
                }
            }
            else
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotations)
                {
                    this.WriteInstanceAnnotation(annotation, true, propertyName);
                }
            }
        }

        /// <summary>
        /// Asynchronously write instance annotation if not already written.
        /// </summary>
        /// <param name="annotation">The instance annotation to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        /// <param name="instanceAnnotationNames">Set used to detect a duplicate annotation.</param>
        /// <param name="ignoreFilter">Whether to ignore the filter in settings.</param>
        /// <param name="propertyName">The name of the property this instance annotation applies to.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteAndTrackInstanceAnnotationAsync(
            ODataInstanceAnnotation annotation,
            InstanceAnnotationWriteTracker tracker,
            HashSet<string> instanceAnnotationNames,
            bool ignoreFilter = false,
            string propertyName = null)
        {
            if (!instanceAnnotationNames.Add(annotation.Name))
            {
                throw new ODataException(Error.Format(SRResources.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection, annotation.Name));
            }

            if (!tracker.IsAnnotationWritten(annotation.Name)
                && (!ODataAnnotationNames.IsODataAnnotationName(annotation.Name) || ODataAnnotationNames.IsUnknownODataAnnotationName(annotation.Name)))
            {
                await this.WriteInstanceAnnotationAsync(annotation, ignoreFilter, propertyName)
                    .ConfigureAwait(false);
                tracker.MarkAnnotationWritten(annotation.Name);
            }
        }

        /// <summary>
        /// Asynchronously all the instance annotations specified in <paramref name="instanceAnnotations"/> for the
        /// undecalared property called <paramref name="propertyName"/>
        /// </summary>
        /// <param name="instanceAnnotations">The collection of instance annotations</param>
        /// <param name="propertyName">The name of the property the instance annotations apply to</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteInstanceAnnotationsForUndeclaredPropertyAsync(ICollection<ODataInstanceAnnotation> instanceAnnotations, string propertyName)
        {
            // write undeclared property's all annotations
            // optimize the foreach when instanceAnnotations is a List to avoid enumerator allocations on the heap
            if (instanceAnnotations is List<ODataInstanceAnnotation> instanceAnnotationsList)
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotationsList)
                {
                    await this.WriteInstanceAnnotationAsync(annotation, true, propertyName)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                foreach (ODataInstanceAnnotation annotation in instanceAnnotations)
                {
                    await this.WriteInstanceAnnotationAsync(annotation, true, propertyName)
                        .ConfigureAwait(false);
                }
            }
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

        /// <summary>
        /// Asynchronously writes the name of the instance annotation.
        /// </summary>
        /// <param name="propertyName">The name of the property this instance annotation applied to.</param>
        /// <param name="annotationName">The name of the instance annotation.</param>
        private Task WriteInstanceAnnotationNameAsync(string propertyName, string annotationName)
        {
            if (propertyName != null)
            {
                return this.jsonWriter.WritePropertyAnnotationNameAsync(propertyName, annotationName);
            }
            else
            {
                return this.jsonWriter.WriteInstanceAnnotationNameAsync(annotationName);
            }
        }
    }
}
