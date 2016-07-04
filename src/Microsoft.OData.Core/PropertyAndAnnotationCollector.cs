//---------------------------------------------------------------------
// <copyright file="PropertyAndAnnotationCollector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Json;
    using Microsoft.OData.JsonLight;
    #endregion Namespaces

    using Annotation = System.Collections.Generic.KeyValuePair<string, object>;

    /// <summary>
    /// This class has the following responsibilities:
    ///   1) Validates that no duplicate OData scope/property annotations exist.
    ///      Duplicate custom scope/property annotations are allowed.
    ///   2) Collects OData and custom scope/property annotations.
    ///   3) Validates that no duplicate properties exist.
    ///   4) Validates that property annotations come in group and immediately precede the annotated property.
    /// </summary>
    /// <remarks>
    /// Scope annotations are those that do not apply to specific properties, and start directly with "@".
    /// </remarks>
    internal sealed class PropertyAndAnnotationCollector
    {
        private static readonly IDictionary<string, object> emptyDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Whether to enable duplicate property validation so that an exception is thrown when detected.
        /// </summary>
        /// <remarks>
        /// If disabled and duplicate properties exist, the behavior is unspecified.
        /// </remarks>
        private readonly bool throwOnDuplicateProperty;

        /// <summary>
        /// Caches OData scope annotations.
        /// </summary>
        private IDictionary<string, object> odataScopeAnnotations = new Dictionary<string, object>();

        /// <summary>
        /// Caches custom scope annotations.
        /// </summary>
        private IList<Annotation> customScopeAnnotations = new List<Annotation>();

        /// <summary>
        /// Caches property data.
        /// </summary>
        private IDictionary<string, PropertyData> propertyData = new Dictionary<string, PropertyData>();

        /// <summary>
        /// Creates a PropertyAndAnnotationCollector instance.
        /// </summary>
        /// <param name="throwOnDuplicateProperty">Whether to enable duplicate property validation.</param>
        internal PropertyAndAnnotationCollector(bool throwOnDuplicateProperty)
        {
            this.throwOnDuplicateProperty = throwOnDuplicateProperty;
        }

        /// <summary>
        /// Processing state of a property.
        /// </summary>
        /// <remarks>
        /// Models a state machine.
        /// </remarks>
        private enum PropertyState
        {
            /// <summary>
            /// Initial state or when property annotations have been processed.
            /// </summary>
            AnnotationSeen,

            /// <summary>
            /// Non-nested non-navigation property value has been processed.
            /// </summary>
            SimpleProperty,

            /// <summary>
            /// 1) Nested property value has been processed.
            /// 2) Navigation property value has been processed.
            /// 3) Association link has been processed.
            /// </summary>
            NavigationProperty
        }

        /// <summary>
        /// Validates that no duplicate property exists.
        /// </summary>
        /// <param name="property">The property to be validated.</param>
        internal void CheckForDuplicatePropertyNames(ODataProperty property)
        {
            Debug.Assert(property != null);

            if (!throwOnDuplicateProperty)
            {
                return;
            }

            string propertyName = property.Name;
            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = new PropertyData(PropertyState.SimpleProperty);
            }
            else if (data.State == PropertyState.AnnotationSeen)
            {
                data.State = PropertyState.SimpleProperty;
            }
            else
            {
                throw new ODataException(
                    Strings.DuplicatePropertyNamesNotAllowed(
                        propertyName));
            }
        }

        /// <summary>
        /// Validates that no duplicate property exists when the nested resource info has started,
        /// but we don't know yet if it's expanded or not.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to be validated.</param>
        internal void ValidatePropertyUniquenessOnNestedResourceInfoStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null);

            if (!throwOnDuplicateProperty)
            {
                return;
            }

            // Dry run, no write.
            string propertyName = nestedResourceInfo.Name;
            PropertyData data;
            if (propertyData.TryGetValue(propertyName, out data))
            {
                CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(propertyName, data);
            }
        }

        /// <summary>
        /// Validates that no duplicate property exists and gets the corresponding association link if available.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to be checked.</param>
        /// <returns>Corresponding association link if available.</returns>
        internal Uri ValidatePropertyUniquenessAndGetAssociationLink(ODataNestedResourceInfo nestedResourceInfo)
        {
            string propertyName = nestedResourceInfo.Name;
            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = new PropertyData(PropertyState.NavigationProperty)
                {
                    NestedResourceInfo = nestedResourceInfo
                };
                return null;
            }
            else
            {
                if (throwOnDuplicateProperty)
                {
                    CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(propertyName, data);
                }

                data.State = PropertyState.NavigationProperty;
                data.NestedResourceInfo = nestedResourceInfo;
                return data.AssociationLinkUrl;
            }
        }

        /// <summary>
        /// Validates that no duplicate "odata.assocationLink" annotation exists and gets the corresponding
        /// ODataNestedResourceInfo of the annotated property if available.
        /// </summary>
        /// <param name="propertyName">Name of the annotated property.</param>
        /// <param name="associationLinkUrl">Annotation value.</param>
        /// <returns>Corresponding ODataNestedResourceInfo of the annotated property if available.</returns>
        internal ODataNestedResourceInfo ValidatePropertyOpenForAssociationLinkAndGetNestedResourceInfo(string propertyName, Uri associationLinkUrl)
        {
            Debug.Assert(propertyName != null);

            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = new PropertyData(PropertyState.NavigationProperty)
                {
                    AssociationLinkUrl = associationLinkUrl,
                };
                return null;
            }
            else
            {
                if (data.State == PropertyState.AnnotationSeen
                    || (data.State == PropertyState.NavigationProperty
                        && data.AssociationLinkUrl == null))
                {
                    data.State = PropertyState.NavigationProperty;
                    data.AssociationLinkUrl = associationLinkUrl;
                }
                else
                {
                    throw new ODataException(
                        Strings.DuplicateAnnotationNotAllowed(
                            "odata.associationLink"));
                }

                return data.NestedResourceInfo;
            }
        }

        /// <summary>
        /// Resets to initial state for reuse.
        /// </summary>
        internal void Reset()
        {
            propertyData.Clear();
        }

        /// <summary>
        /// Adds an OData scope annotation.
        /// </summary>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        /// <remarks>
        /// Scope annotations are those that do not apply to specific properties, and start directly with "@".
        /// </remarks>
        internal void AddODataScopeAnnotation(string annotationName, object annotationValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName));
            Debug.Assert(JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName));

            if (annotationValue == null)
            {
                return;
            }

            try
            {
                odataScopeAnnotations.Add(annotationName, annotationValue);
            }
            catch (ArgumentException)
            {
                throw new ODataException(
                    Strings.DuplicateAnnotationNotAllowed(
                        annotationName));
            }
        }

        /// <summary>
        /// Adds a custom scope annotation.
        /// </summary>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        /// <remarks>
        /// Scope annotations are those that do not apply to specific properties, and start directly with "@".
        /// </remarks>
        internal void AddCustomScopeAnnotation(string annotationName, object annotationValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName));
            Debug.Assert(!JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName));

            if (annotationValue == null)
            {
                return;
            }

            customScopeAnnotations.Add(new Annotation(annotationName, annotationValue));
        }

        /// <summary>
        /// Gets OData scope annotations.
        /// </summary>
        /// <returns>OData scope annotation key value pairs.</returns>
        /// <remarks>
        /// Scope annotations are those that do not apply to specific properties, and start directly with "@".
        /// </remarks>
        internal IDictionary<string, object> GetODataScopeAnnotation()
        {
            return odataScopeAnnotations;
        }

        /// <summary>
        /// Gets custom scope annotations.
        /// </summary>
        /// <returns>Custom scope annotation key value pairs.</returns>
        /// <remarks>
        /// Scope annotations are those that do not apply to specific properties, and start directly with "@".
        /// </remarks>
        internal IEnumerable<Annotation> GetCustomScopeAnnotation()
        {
            return customScopeAnnotations;
        }

        /// <summary>
        /// Adds an OData property annotation.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddODataPropertyAnnotation(string propertyName, string annotationName, object annotationValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName));
            Debug.Assert(!string.IsNullOrEmpty(annotationName));
            Debug.Assert(JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName));

            if (annotationValue == null)
            {
                return;
            }

            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = data = new PropertyData(PropertyState.AnnotationSeen);
            }
            else
            {
                if (data.Processed)
                {
                    throw new ODataException(
                        Strings.PropertyAnnotationAfterTheProperty(
                            annotationName, propertyName));
                }
            }

            try
            {
                data.ODataAnnotations.Add(annotationName, annotationValue);
            }
            catch (ArgumentException)
            {
                throw new ODataException(
                    ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName)
                    ? Strings.DuplicateAnnotationForInstanceAnnotationNotAllowed(annotationName, propertyName)
                    : Strings.DuplicateAnnotationForPropertyNotAllowed(annotationName, propertyName));
            }
        }

        /// <summary>
        /// Adds a custom property annotation.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddCustomPropertyAnnotation(string propertyName, string annotationName, object annotationValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName));
            Debug.Assert(!string.IsNullOrEmpty(annotationName));
            Debug.Assert(!JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName));

            if (annotationValue == null)
            {
                return;
            }

            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = data = new PropertyData(PropertyState.AnnotationSeen);
            }
            else
            {
                if (data.Processed)
                {
                    throw new ODataException(
                        Strings.PropertyAnnotationAfterTheProperty(
                            annotationName, propertyName));
                }
            }

            data.CustomAnnotations.Add(new Annotation(annotationName, annotationValue));
        }

        /// <summary>
        /// Returns OData annotations for a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>OData property annotation name value pairs.</returns>
        internal IDictionary<string, object> GetODataPropertyAnnotations(string propertyName)
        {
            Debug.Assert(propertyName != null);

            PropertyData data;
            return propertyData.TryGetValue(propertyName, out data)
                   ? data.ODataAnnotations
                   : emptyDictionary;
        }

        /// <summary>
        /// Returns custom annotations for a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Custom property annotation name value pairs.</returns>
        internal IEnumerable<Annotation> GetCustomPropertyAnnotations(string propertyName)
        {
            Debug.Assert(propertyName != null);

            PropertyData data;
            return propertyData.TryGetValue(propertyName, out data)
                   ? data.CustomAnnotations
                   : Enumerable.Empty<Annotation>();
        }

        /// <summary>
        /// Marks a property to note that all its annotations should have been processed by now.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <remarks>
        /// It's an error if more annotations for a marked property are found later in the payload.
        /// </remarks>
        internal void MarkPropertyAsProcessed(string propertyName)
        {
            Debug.Assert(propertyName != null);

            PropertyData data;
            if (!propertyData.TryGetValue(propertyName, out data))
            {
                propertyData[propertyName] = data = new PropertyData(PropertyState.AnnotationSeen);
            }

            if (data.Processed)
            {
                throw new ODataException(
                    ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName)
                    && !ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName)
                    ? Strings.DuplicateAnnotationNotAllowed(propertyName)
                    : Strings.DuplicatePropertyNamesNotAllowed(propertyName));
            }

            data.Processed = true;
        }

        /// <summary>
        /// Validates that the property is open to be added more annotations.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <remarks>
        /// A property is no longer open for annotations when it has been marked as processed.
        /// </remarks>
        internal void CheckIfPropertyOpenForAnnotations(string propertyName, string annotationName)
        {
            PropertyData data;
            if (propertyData.TryGetValue(propertyName, out data)
                && data.Processed)
            {
                throw new ODataException(
                    Strings.PropertyAnnotationAfterTheProperty(
                        annotationName, propertyName));
            }
        }

        /// <summary>
        /// Validates that no duplicate property exists (dry run).
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyData">Corresponding property data.</param>
        private static void CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(string propertyName, PropertyData propertyData)
        {
            if (propertyData.State == PropertyState.NavigationProperty
                && propertyData.AssociationLinkUrl != null
                && propertyData.NestedResourceInfo == null)
            {
                // Association link has been processed, and this is the corresponding navigation property.
            }
            else if (propertyData.State != PropertyState.AnnotationSeen)
            {
                throw new ODataException(
                    Strings.DuplicatePropertyNamesNotAllowed(
                        propertyName));
            }
        }

        private sealed class PropertyData
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="propertyState">Initial property state.</param>
            internal PropertyData(PropertyState propertyState)
            {
                State = propertyState;
                ODataAnnotations = new Dictionary<string, object>();
                CustomAnnotations = new List<Annotation>();
            }

            /// <summary>
            /// Current processing state.
            /// </summary>
            internal PropertyState State { get; set; }

            /// <summary>
            /// The nested resource info of the property if found.
            /// </summary>
            internal ODataNestedResourceInfo NestedResourceInfo { get; set; }

            /// <summary>
            /// The association link for the property if found.
            /// </summary>
            internal Uri AssociationLinkUrl { get; set; }

            /// <summary>
            /// OData property annotations.
            /// </summary>
            /// <remarks>
            /// The key is the fully qualified annotation name like "odata.type".
            /// The value is the parsed value of the annotation, which is annotation specific.
            /// </remarks>
            internal IDictionary<string, object> ODataAnnotations { get; private set; }

            /// <summary>
            /// Custom property annotations.
            /// </summary>
            /// <remarks>
            /// The key is the fully qualified annotation name.
            /// The value is the parsed value of the annotation, which is annotation specific.
            /// </remarks>
            internal IList<Annotation> CustomAnnotations { get; private set; }

            /// <summary>
            /// Denotes whether the property has been marked as processed.
            /// </summary>
            internal bool Processed { get; set; }
        }
    }
}
