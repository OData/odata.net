//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for EPM readers.
    /// </summary>
    internal abstract class EpmReader
    {
        /// <summary>The reader entry state to use for the entry to which the EPM is applied.</summary>
        private readonly IODataAtomReaderEntryState entryState;

        /// <summary>The version of OData protocol to use.</summary>
        private readonly ODataVersion version;

        /// <summary>The reader settings to use.</summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entryState">The reader entry state to use for the entry to which the EPM is applied.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="messageReaderSettings">The reader settings to use.</param>
        protected EpmReader(
            IODataAtomReaderEntryState entryState,
            ODataVersion version,
            ODataMessageReaderSettings messageReaderSettings)
        {
            this.entryState = entryState;
            this.version = version;
            this.messageReaderSettings = messageReaderSettings;
        }

        /// <summary>The reader entry state to use for the entry to which the EPM is applied.</summary>
        protected IODataAtomReaderEntryState EntryState
        {
            get
            {
                return this.entryState;
            }
        }

        /// <summary>The version of OData protocol to use.</summary>
        protected ODataVersion Version
        {
            get
            {
                return this.version;
            }
        }

        /// <summary>
        /// Creates a new item in a multivalue and its target list.
        /// </summary>
        /// <param name="epmInfo">The EPM info describing the mapping of the MultiValue</param>
        /// <param name="items">The list of items for the multivalue (adds the new item there). This should be the value returned by CreateEntryEpmMultiValue.</param>
        /// <returns>The target list for the newly added item. Use this to set property values to.</returns>
        protected static IList CreateMultiValueItemTargetList(EntityPropertyMappingInfo epmInfo, List<object> items)
        {
            Debug.Assert(
                epmInfo != null && epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                "The epmInfo must specify the mapping for the multivalue property to create.");

            if (epmInfo.MultiValueItemTypeReference.IsODataPrimitiveTypeKind())
            {
                // If the multivalue is of primitive type, then we use the items list itself as the target list
                // the SetEpmValue method will notice that and add the value directly to the list
                // instead of creating a property for it.
                return items;
            }

            Debug.Assert(
                epmInfo.MultiValueItemTypeReference.IsODataComplexTypeKind(),
                "Only multivalue of primitive or complex types are supported.");

            // Create a new complex value for the item, read the EPM into it and add it to the list
            ODataComplexValue complexValue = new ODataComplexValue
            {
                TypeName = epmInfo.MultiValueItemTypeReference.ODataFullName(),
                Properties = new ReadOnlyEnumerable<ODataProperty>()
            };

            items.Add(complexValue);

            return ReaderUtils.GetPropertiesList(complexValue.Properties);
        }

        /// <summary>
        /// Sets the value read from EPM to a property on an entry.
        /// </summary>
        /// <param name="epmInfo">The EPM info for the mapping for which the value was read.</param>
        /// <param name="propertyValue">The property value read, if the value was specified as null then this should be null,
        /// if the value was missing the method should not be called at all.
        /// For primitive properties this should be the string value, for all other properties this should be the exact value type.</param>
        protected void SetEntryEpmValue(EntityPropertyMappingInfo epmInfo, object propertyValue)
        {
            this.SetEpmValue(
                ReaderUtils.GetPropertiesList(this.entryState.Entry.Properties),
                this.entryState.EntityType.ToTypeReference(),
                epmInfo,
                propertyValue);
        }

        /// <summary>
        /// Sets the value read from EPM to a property on an entry.
        /// </summary>
        /// <param name="targetList">The target list, this can be either a list of properties (on entry or complex value),
        /// or a list of items (for multivalue of primitive types).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity, complex or primitive).</param>
        /// <param name="epmInfo">The EPM info for the mapping for which the value was read.</param>
        /// <param name="propertyValue">The property value read, if the value was specified as null then this should be null,
        /// if the value was missing the method should not be called at all.
        /// For primitive properties this should be the string value, for all other properties this should be the exact value type.</param>
        protected void SetEpmValue(
            IList targetList,
            IEdmTypeReference targetTypeReference,
            EntityPropertyMappingInfo epmInfo,
            object propertyValue)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(targetList != null, "targetList != null");

            if (targetTypeReference.IsODataPrimitiveTypeKind())
            {
                // The target is the multivalue itself, this means the target is a multivalue of primitive types
                Debug.Assert(
                    epmInfo.MultiValueItemTypeReference.IsODataPrimitiveTypeKind(),
                    "Only multivalue of primitive types can set values to the list of items directly.");
                List<object> items = (List<object>)targetList;

                // Convert the string to the primitive value
                Debug.Assert(propertyValue != null, "null value is not allowed in multivalue, we should have failed on it already.");
                object primitiveValue = AtomValueUtils.ConvertStringToPrimitive((string)propertyValue, targetTypeReference.AsPrimitiveOrNull());
                items.Add(primitiveValue);
            }
            else
            {
                Debug.Assert(
                    targetTypeReference.IsODataEntityTypeKind() || targetTypeReference.IsODataComplexTypeKind(),
                    "Only entity and complex types can have an EPM value set on them.");
                this.SetEpmValueForSegment(
                    epmInfo,
                    0,
                    targetTypeReference.AsStructuredOrNull(),
                    (List<ODataProperty>)targetList,
                    propertyValue);
            }
        }

        /// <summary>
        /// Creates a new MultiValue for the specified EPM.
        /// </summary>
        /// <param name="epmInfo">The EPM info describing the mapping of the MultiValue.</param>
        /// <returns>The list for the MultiValue to fill with items.</returns>
        protected List<object> CreateEntryEpmMultiValue(EntityPropertyMappingInfo epmInfo)
        {
            Debug.Assert(
                epmInfo != null && epmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                "The epmInfo must specify the mapping for the multivalue property to create.");

            List<object> items = new List<object>();
            this.SetEntryEpmValue(epmInfo, items);
            return items;
        }

        /// <summary>
        /// Sets a property value for a segment of the EPM source path.
        /// </summary>
        /// <param name="epmInfo">The EPM info according to which we are mapping the value to properties.</param>
        /// <param name="propertyValuePathIndex">The index in the epmInfo.PropertyValuePath for the source segment for which to set the value.</param>
        /// <param name="segmentStructuralTypeReference">The structural type of the parent segment.</param>
        /// <param name="existingProperties">The list of properties of the parent segment, this method may add to this list.</param>
        /// <param name="propertyValue">The property value read, if the value was specified as null then this should be null,
        /// if the value was missing the method should not be called at all.
        /// For primitive properties this should be the string value, for all other properties this should be the exact value type.</param>
        private void SetEpmValueForSegment(
            EntityPropertyMappingInfo epmInfo,
            int propertyValuePathIndex,
            IEdmStructuredTypeReference segmentStructuralTypeReference,
            List<ODataProperty> existingProperties,
            object propertyValue)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(propertyValuePathIndex < epmInfo.PropertyValuePath.Length, "The propertyValuePathIndex is out of bounds.");
            Debug.Assert(existingProperties != null, "existingProperties != null");

            string propertyName = epmInfo.PropertyValuePath[propertyValuePathIndex].PropertyName;

            ODataProperty existingProperty = existingProperties.FirstOrDefault(p => string.CompareOrdinal(p.Name, propertyName) == 0);
            ODataComplexValue existingComplexValue = null;
            if (existingProperty != null)
            {
                // In case the property exists and it's a complex value we will try to merge.
                // Note that if the property is supposed to be complex, but it already has a null value, then the null wins.
                // Since in-content null complex value wins over any EPM complex value.
                existingComplexValue = existingProperty.Value as ODataComplexValue;
                if (existingComplexValue == null)
                {
                    return;
                }
            }

            IEdmProperty propertyMetadata = segmentStructuralTypeReference.FindProperty(propertyName);
            Debug.Assert(propertyMetadata != null || segmentStructuralTypeReference.IsOpen(), "We should have verified that if the property is not declared the type must be open.");

            if (propertyMetadata == null && propertyValuePathIndex != epmInfo.PropertyValuePath.Length - 1)
            {
                throw new ODataException(Strings.EpmReader_OpenComplexOrMultivalueEpmProperty(epmInfo.Attribute.SourcePath));
            }

            // Open properties in EPM are by default of type Edm.String - there's no way to specify a typename in EPM
            // consumer is free to do the conversion later on if it needs to.
            // Note that this effectively means that ODataMessageReaderSettings.DisablePrimitiveTypeConversion is as if it's turned on for open EPM properties.
            IEdmTypeReference propertyType;
            if (propertyMetadata == null ||
                (this.messageReaderSettings.DisablePrimitiveTypeConversion && propertyMetadata.Type.IsODataPrimitiveTypeKind()))
            {
                propertyType = EdmCoreModel.Instance.GetString(/*nullable*/true);
            }
            else
            {
                propertyType = propertyMetadata.Type;
            }

            // NOTE: WCF DS Server only applies the values when
            // - It's an open property
            // - It's not a key property
            // - It's a key property and it's a POST operation
            // ODataLib here will always set the property though.
            switch (propertyType.TypeKind())
            {
                case EdmTypeKind.Primitive:
                    {
                        if (propertyType.IsStream())
                        {
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmReader_SetEpmValueForSegment_StreamProperty));
                        }

                        object primitiveValue;
                        if (propertyValue == null)
                        {
                            ReaderValidationUtils.ValidateNullValue(propertyType);
                            primitiveValue = null;
                        }
                        else
                        {
                            // Convert the value to the desired target type
                            primitiveValue = AtomValueUtils.ConvertStringToPrimitive((string)propertyValue, propertyType.AsPrimitiveOrNull());
                        }

                        this.AddEpmPropertyValue(existingProperties, propertyName, primitiveValue, segmentStructuralTypeReference.IsODataEntityTypeKind());
                    }

                    break;

                case EdmTypeKind.Complex:
                    // Note: Unlike WCF DS we don't have a preexisting instance to override (since complex values are atomic, so we should not updated them)
                    // In our case the complex value either doesn't exist yet on the entry being reported (easy, create it)
                    // or it exists, but then it was created during reading of previous normal or EPM properties for this entry. It never exists before
                    // we ever get to see the entity. So in our case we will never recreate the complex value, we always start with new one
                    // and update it with new properties as they come. (Next time we will start over with a new complex value.)
                    Debug.Assert(
                        existingComplexValue == null || (existingProperty != null && existingProperty.Value == existingComplexValue),
                        "If we have existing complex value, we must have an existing property as well.");
                    Debug.Assert(
                        epmInfo.PropertyValuePath.Length > propertyValuePathIndex + 1,
                        "Complex value can not be a leaf segment in the source property path. We should have failed constructing the EPM trees for it.");

                    if (existingComplexValue == null)
                    {
                        Debug.Assert(existingProperty == null, "If we don't have an existing complex value, then we must not have an existing property at all.");

                        // Create a new complex value and set its type name to the type name of the property type (in case of EPM we never have type name from the payload)
                        existingComplexValue = new ODataComplexValue
                        {
                            TypeName = propertyType.ODataFullName(),
                            Properties = new ReadOnlyEnumerable<ODataProperty>()
                        };

                        this.AddEpmPropertyValue(existingProperties, propertyName, existingComplexValue, segmentStructuralTypeReference.IsODataEntityTypeKind());
                    }

                    // Get the properties list of the complex value and recursively set the next EPM segment value to it.
                    // Note that on inner complex value we don't need to check for duplicate properties
                    // because EPM will never add a property which already exists (see the start of this method).
                    IEdmComplexTypeReference complexPropertyTypeReference = propertyType.AsComplexOrNull();
                    Debug.Assert(complexPropertyTypeReference != null, "complexPropertyTypeReference != null");
                    this.SetEpmValueForSegment(
                        epmInfo,
                        propertyValuePathIndex + 1,
                        complexPropertyTypeReference,
                        ReaderUtils.GetPropertiesList(existingComplexValue.Properties),
                        propertyValue);

                    break;

                case EdmTypeKind.Collection:
                    Debug.Assert(propertyType.IsODataMultiValueTypeKind(), "Multi value types must be atomic collection types.");

                    // In this case the property value is the internal list of items.
                    // Create a new multi value and set the list as the list of items on it.
                    ODataMultiValue multiValue = new ODataMultiValue
                    {
                        TypeName = propertyType.ODataFullName(),
                        Items = new ReadOnlyEnumerable((List<object>)propertyValue)
                    };

                    this.AddEpmPropertyValue(existingProperties, propertyName, multiValue, segmentStructuralTypeReference.IsODataEntityTypeKind());

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmReader_SetEpmValueForSegment_TypeKind));
            }
        }

        /// <summary>
        /// Creates and adds a new property to the list of properties for an EPM.
        /// </summary>
        /// <param name="properties">The list of properties to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property to add.</param>
        /// <param name="checkDuplicateEntryPropertyNames">true if the new property should be checked for duplicates against the entry properties; false otherwise.
        /// This should be true if the <paramref name="properties"/> is the list of properties for the entry, and false in all other cases.</param>
        private void AddEpmPropertyValue(List<ODataProperty> properties, string propertyName, object propertyValue, bool checkDuplicateEntryPropertyNames)
        {
            Debug.Assert(properties != null, "properties != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "propertyName must not be null or empty.");

            // Create a new property object and add it.
            ODataProperty property = new ODataProperty
            {
                Name = propertyName,
                Value = propertyValue
            };

            if (checkDuplicateEntryPropertyNames)
            {
                this.entryState.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
            }

            properties.Add(property);
        }
    }
}
