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

namespace Microsoft.Data.OData.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Json;

    /// <summary>
    /// Responsible for reading annotation groups (both declarations and references) in JSON Light.
    /// </summary>
    internal sealed class JsonLightAnnotationGroupDeserializer : ODataJsonLightDeserializer
    {
        /// <summary>
        /// Mapping of all the annotation groups encountered so far, keyed by name.
        /// </summary>
        private readonly Dictionary<string, ODataJsonLightAnnotationGroup> annotationGroups;

        /// <summary>
        /// Initializes a new <see cref="JsonLightAnnotationGroupDeserializer"/>.
        /// </summary>
        /// <param name="inputContext">The JSON light input context.</param>
        internal JsonLightAnnotationGroupDeserializer(ODataJsonLightInputContext inputContext) 
            : base(inputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            this.annotationGroups = new Dictionary<string, ODataJsonLightAnnotationGroup>(EqualityComparer<string>.Default);
        }

        /// <summary>
        /// Reads the current property node as an annotaion group, if the property name matches. Otherwise, it does not move the reader.
        /// </summary>
        /// <param name="readPropertyAnnotationValue">Fired whenever an OData property annotation is seen. Takes the name of the property annotation and should read and return the annotation's value.</param>
        /// <param name="readInstanceAnnotationValue">Fired whenever an OData instance annotation is seen. Takes the name of the instance annotation and should read and return the annotation's value.</param>
        /// <returns>The annotation group which was read, or null if we did not encounter an annotation group.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property:     The property to consider as an annotion group.
        /// Post-Condition: JsonNodeType.Property:     If the property is not an annotation group, the reader will not move.
        ///                 Any:          The node after the annotation group property, if one was read.
        /// </remarks>
        internal ODataJsonLightAnnotationGroup ReadAnnotationGroup(Func<string, object> readPropertyAnnotationValue, Func<string, DuplicatePropertyNamesChecker, object> readInstanceAnnotationValue)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertJsonCondition(JsonNodeType.Property);

            string propertyName = this.JsonReader.GetPropertyName();

            if (string.CompareOrdinal(propertyName, ODataAnnotationNames.ODataAnnotationGroup) == 0)
            {
                // Advance the reader past the property name.
                this.JsonReader.ReadPropertyName();
                return this.ReadAnnotationGroupDeclaration(readPropertyAnnotationValue, readInstanceAnnotationValue);
            }

            if (string.CompareOrdinal(propertyName, ODataAnnotationNames.ODataAnnotationGroupReference) == 0)
            {
                // Advance the reader past the property name.
                this.JsonReader.ReadPropertyName();
                return this.ReadAnnotationGroupReference();
            }

            return null;
        }

        /// <summary>
        /// Adds the given annotation group to the set of groups which can be retrieved by annotation group references.
        /// </summary>
        /// <param name="annotationGroup">The annotation group to add.</param>
        internal void AddAnnotationGroup(ODataJsonLightAnnotationGroup annotationGroup)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotationGroup != null, "annotationGroup != null");

            if (this.annotationGroups.ContainsKey(annotationGroup.Name))
            {
                throw new ODataException(Strings.JsonLightAnnotationGroupDeserializer_MultipleAnnotationGroupsWithSameName(annotationGroup.Name));
            }

            this.annotationGroups.Add(annotationGroup.Name, annotationGroup);
        }

        /// <summary>
        /// Checks to see that the name of the given annotation group has not yet been set. Throws otherwise.
        /// </summary>
        /// <param name="annotationGroup">The annotation group to check.</param>
        private static void VerifyAnnotationGroupNameNotYetFound(ODataJsonLightAnnotationGroup annotationGroup)
        {
            Debug.Assert(annotationGroup != null, "annotaionGroup != null");

            if (!string.IsNullOrEmpty(annotationGroup.Name))
            {
                throw new ODataException(Strings.JsonLightAnnotationGroupDeserializer_EncounteredMultipleNameProperties);
            }
        }

        /// <summary>
        /// Returns whether the given property name indicates this property contains the name of the annotation group.
        /// </summary>
        /// <param name="propertyName">The property name to check.</param>
        /// <returns>true if the property name is annotation group name property, false otherwise.</returns>
        private static bool IsAnnotationGroupName(string propertyName)
        {
            return string.CompareOrdinal(propertyName, JsonLightConstants.ODataAnnotationGroupNamePropertyName) == 0;
        }

        /// <summary>
        /// Verifies that the name of the given annotation group was set, and throws otherwise.
        /// </summary>
        /// <param name="annotationGroup">The annnotation group to check.</param>
        private static void VerifyAnnotationGroupNameFound(ODataJsonLightAnnotationGroup annotationGroup)
        {
            Debug.Assert(annotationGroup != null, "annotationGroup != null");

            if (string.IsNullOrEmpty(annotationGroup.Name))
            {
                throw new ODataException(Strings.JsonLightAnnotationGroupDeserializer_AnnotationGroupDeclarationWithoutName);
            }
        }

        /// <summary>
        /// Verifies that the given property name is a valid annotation name, and throws if not.
        /// </summary>
        /// <param name="propertyName">The property name to check.</param>
        /// <param name="annotationGroup">The annotation group which this property would be added to.</param>
        private static void VerifyDataPropertyIsAnnotationName(string propertyName, ODataJsonLightAnnotationGroup annotationGroup)
        {
            Debug.Assert(annotationGroup != null, "annotationGroup != null");

            if (!IsAnnotationGroupName(propertyName))
            {
                throw CreateExceptionForInvalidAnnotationInsideAnnotationGroup(propertyName, annotationGroup);
            }
        }

        /// <summary>
        /// Creates an ODataException to throw when a non-annotation property is found inside an annotation group.
        /// </summary>
        /// <param name="propertyName">The name of the property found inside an annotation group.</param>
        /// <param name="annotationGroup">The annotation group it was found in.</param>
        /// <returns>An ODataException with an appropriate message, including the annotation group name if one is available.</returns>
        private static ODataException CreateExceptionForInvalidAnnotationInsideAnnotationGroup(string propertyName, ODataJsonLightAnnotationGroup annotationGroup)
        {
            if (string.IsNullOrEmpty(annotationGroup.Name))
            {
                return new ODataException(Strings.JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideAnnotationGroup(propertyName));
            }

            // Throw a more specific message if we have the annotation group name.
            return new ODataException(Strings.JsonLightAnnotationGroupDeserializer_InvalidAnnotationFoundInsideNamedAnnotationGroup(annotationGroup.Name, propertyName));
        }

        /// <summary>
        /// Reads an annotation group reference and returns the existing annotation group instance with that name.
        /// </summary>
        /// <returns>The annotation group which was referenced.</returns>
        /// <remarks>This method will throw if no matching annotation group is found.</remarks>
        private ODataJsonLightAnnotationGroup ReadAnnotationGroupReference()
        {
            string annotationGroupName = this.JsonReader.ReadStringValue(ODataAnnotationNames.ODataAnnotationGroupReference);

            ODataJsonLightAnnotationGroup annotationGroup;
            if (this.annotationGroups.TryGetValue(annotationGroupName, out annotationGroup))
            {
                return annotationGroup;
            }

            throw new ODataException(Strings.JsonLightAnnotationGroupDeserializer_UndefinedAnnotationGroupReference(annotationGroupName));
        }

        /// <summary>
        /// Reads an annotation group declaration and returns a newly created annotation group instance.
        /// </summary>
        /// <param name="readPropertyAnnotationValue">Function which takes the name of an OData property annotation and reads and returns the value of the annotation.</param>
        /// <param name="readInstanceAnnotationValue">Function which takes the name of an OData instance annotation and reads and returns the value of the annotation.</param>
        /// <returns>The annotation group which was read.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.StartObject:     The property to consider as an annotion group declaration or reference.
        ///                 Any:                          Any other node type will throw an exception.
        /// Post-Condition: Any:                          The node after the annotation group property value.
        /// </remarks>
        private ODataJsonLightAnnotationGroup ReadAnnotationGroupDeclaration(Func<string, object> readPropertyAnnotationValue, Func<string, DuplicatePropertyNamesChecker, object> readInstanceAnnotationValue)
        {
            ODataJsonLightAnnotationGroup annotationGroup = new ODataJsonLightAnnotationGroup { Annotations = new Dictionary<string, object>(EqualityComparer<string>.Default) };

            this.JsonReader.ReadStartObject();
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ProcessProperty(
                    duplicatePropertyNamesChecker,
                    readPropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.PropertyWithValue:
                                VerifyDataPropertyIsAnnotationName(propertyName, annotationGroup);

                                VerifyAnnotationGroupNameNotYetFound(annotationGroup);
                                annotationGroup.Name = this.JsonReader.ReadStringValue(propertyName);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                Dictionary<string, object> propertyAnnotations = duplicatePropertyNamesChecker.GetODataPropertyAnnotations(propertyName);
                                if (propertyAnnotations != null)
                                {
                                    foreach (KeyValuePair<string, object> propertyAnnotation in propertyAnnotations)
                                    {
                                        annotationGroup.Annotations.Add(propertyName + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + propertyAnnotation.Key, propertyAnnotation.Value);
                                    }
                                }

                                break;

                            case PropertyParsingResult.ODataInstanceAnnotation:
                                annotationGroup.Annotations.Add(propertyName, readInstanceAnnotationValue(propertyName, duplicatePropertyNamesChecker));
                                break;

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                this.JsonReader.SkipValue();
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw CreateExceptionForInvalidAnnotationInsideAnnotationGroup(propertyName, annotationGroup);

                            case PropertyParsingResult.EndOfObject:
                                break;

                            default:
                                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataJsonLightAnnotationGroupDeserializer_ReadAnnotationGroupDeclaration));
                        }
                    });
            }

            VerifyAnnotationGroupNameFound(annotationGroup);
            this.JsonReader.ReadEndObject();
            this.AddAnnotationGroup(annotationGroup);
            return annotationGroup;
        }
    }
}
