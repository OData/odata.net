//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.JsonLight;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
        /// Constructs a <see cref="JsonLightInstanceAnnotationWriter"/> that can write a collection of <see cref="ODataInstanceAnnotation"/>.
        /// </summary>
        /// <param name="valueSerializer">The <see cref="IODataJsonLightValueSerializer"/> to use for writing values of instance annotations.
        /// The <see cref="IJsonWriter"/> that is also used internally will be acquired from the this instance.</param>
        /// <param name="typeNameOracle">The oracle to use to determine the type name to write for entries and values.</param>
        internal JsonLightInstanceAnnotationWriter(IODataJsonLightValueSerializer valueSerializer, JsonLightTypeNameOracle typeNameOracle)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(valueSerializer != null, "valueSerializer should not be null");
            Debug.Assert(valueSerializer.Version >= ODataVersion.V3, "This is a Json Light only class, version should never be below V3.");
            this.valueSerializer = valueSerializer;
            this.typeNameOracle = typeNameOracle;
        }

        /// <summary>
        /// JsonWriter instance to use for writing term names.
        /// </summary>
        private IJsonWriter JsonWriter 
        { 
            get { return this.valueSerializer.JsonWriter; } 
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        /// <param name="tracker">The tracker to track if instance annotations are written.</param>
        internal void WriteInstanceAnnotations(IEnumerable<ODataInstanceAnnotation> instanceAnnotations, InstanceAnnotationWriteTracker tracker)
        {
            DebugUtils.CheckNoExternalCallers();
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
                    this.WriteInstanceAnnotation(annotation);
                    tracker.MarkAnnotationWritten(annotation.Name);
                }
            }
        }

        /// <summary>
        /// Writes all the instance annotations specified in <paramref name="instanceAnnotations"/>.
        /// </summary>
        /// <param name="instanceAnnotations">Collection of instance annotations to write.</param>
        internal void WriteInstanceAnnotations(IEnumerable<ODataInstanceAnnotation> instanceAnnotations)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations should not be null if we called this");
            this.WriteInstanceAnnotations(instanceAnnotations, new InstanceAnnotationWriteTracker());
        }

        /// <summary>
        /// Writes an instance annotation.
        /// </summary>
        /// <param name="instanceAnnotation">The instance annotation to write.</param>
        internal void WriteInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation)
        {
            DebugUtils.CheckNoExternalCallers();

            string name = instanceAnnotation.Name;
            ODataValue value = instanceAnnotation.Value;
            Debug.Assert(!string.IsNullOrEmpty(name), "name should not be null or empty");
            Debug.Assert(!ODataAnnotationNames.IsODataAnnotationName(name), "A reserved name cannot be used as instance annotation key");
            Debug.Assert(value != null, "value should not be null because we use ODataNullValue for null instead");
            Debug.Assert(!(value is ODataStreamReferenceValue), "!(value is ODataStreamReferenceValue) -- ODataInstanceAnnotation and InstanceAnnotationCollection will throw if the value is a stream value.");
            Debug.Assert(this.valueSerializer.Model != null, "this.valueSerializer.Model != null");

            if (this.valueSerializer.Settings.ShouldSkipAnnotation(name))
            {
                return;
            }

            IEdmTypeReference expectedType = MetadataUtils.LookupTypeOfValueTerm(name, this.valueSerializer.Model);

            if (value is ODataNullValue)
            {
                if (expectedType != null && !expectedType.IsNullable)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueSerializer_NullValueNotAllowedForInstanceAnnotation(instanceAnnotation.Name, expectedType.ODataFullName()));
                }

                this.JsonWriter.WriteName(name);
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
                this.JsonWriter.WriteName(name);
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
                    ODataJsonLightWriterUtils.WriteODataTypePropertyAnnotation(this.JsonWriter, name, collectionTypeNameToWrite);                    
                }

                this.JsonWriter.WriteName(name);
                this.valueSerializer.WriteCollectionValue(collectionValue, expectedType, false /*isTopLevelProperty*/, false /*isInUri*/, treatLikeOpenProperty);
                return;
            }

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            Debug.Assert(primitiveValue != null, "Did we add a new subclass of ODataValue?");

            string primitiveTypeNameToWrite = this.typeNameOracle.GetValueTypeNameForWriting(primitiveValue, expectedType, typeFromValue, treatLikeOpenProperty);
            if (primitiveTypeNameToWrite != null)
            {
                ODataJsonLightWriterUtils.WriteODataTypePropertyAnnotation(this.JsonWriter, name, primitiveTypeNameToWrite);
            }

            this.JsonWriter.WriteName(name);
            this.valueSerializer.WritePrimitiveValue(primitiveValue.Value, expectedType);
        }
    }
}
