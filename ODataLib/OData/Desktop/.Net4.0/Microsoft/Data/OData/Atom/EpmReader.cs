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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Base class for EPM readers.
    /// </summary>
    internal abstract class EpmReader
    {
        /// <summary>The input context currently in use.</summary>
        private readonly ODataAtomInputContext atomInputContext;

        /// <summary>The reader entry state to use for the entry to which the EPM is applied.</summary>
        private readonly IODataAtomReaderEntryState entryState;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entryState">The reader entry state to use for the entry to which the EPM is applied.</param>
        /// <param name="inputContext">The input context currently in use.</param>
        protected EpmReader(
            IODataAtomReaderEntryState entryState,
            ODataAtomInputContext inputContext)
        {
            this.entryState = entryState;
            this.atomInputContext = inputContext;
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
                return this.atomInputContext.Version;
            }
        }

        /// <summary>The reader settings to use.</summary>
        protected ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                return this.atomInputContext.MessageReaderSettings;
            }
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
                this.entryState.Entry.Properties.ToReadOnlyEnumerable("Properties"),
                this.entryState.EntityType.ToTypeReference(),
                epmInfo,
                propertyValue);
        }

        /// <summary>
        /// Sets the value read from EPM to a property on an entry.
        /// </summary>
        /// <param name="targetList">The target list, which is a list of properties (on entry or complex value).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity or complex).</param>
        /// <param name="epmInfo">The EPM info for the mapping for which the value was read.</param>
        /// <param name="propertyValue">The property value read, if the value was specified as null then this should be null,
        /// if the value was missing the method should not be called at all.
        /// For primitive properties this should be the string value, for all other properties this should be the exact value type.</param>
        protected void SetEpmValue(
            ReadOnlyEnumerable<ODataProperty> targetList,
            IEdmTypeReference targetTypeReference,
            EntityPropertyMappingInfo epmInfo,
            object propertyValue)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(targetList != null, "targetList != null");
            Debug.Assert(
                targetTypeReference.IsODataEntityTypeKind() || targetTypeReference.IsODataComplexTypeKind(),
                "Only entity and complex types can have an EPM value set on them.");

            this.SetEpmValueForSegment(
                epmInfo,
                0,
                targetTypeReference.AsStructuredOrNull(),
                targetList,
                propertyValue);
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to keep the logic together for better readability.")]
        private void SetEpmValueForSegment(
            EntityPropertyMappingInfo epmInfo,
            int propertyValuePathIndex,
            IEdmStructuredTypeReference segmentStructuralTypeReference,
            ReadOnlyEnumerable<ODataProperty> existingProperties,
            object propertyValue)
        {
            Debug.Assert(epmInfo != null, "epmInfo != null");
            Debug.Assert(propertyValuePathIndex < epmInfo.PropertyValuePath.Length, "The propertyValuePathIndex is out of bounds.");
            Debug.Assert(existingProperties != null, "existingProperties != null");

            string propertyName = epmInfo.PropertyValuePath[propertyValuePathIndex].PropertyName;

            // Do not set out-of-content values if the EPM is defined as KeepInContent=true.
            if (epmInfo.Attribute.KeepInContent)
            {
                return;
            }

            // Try to find the property in the existing properties
            // If the property value is atomic from point of view of EPM (non-streaming collection or primitive) then if it already exists
            // it must have been in-content, and thus we leave it as is (note that two EPMs can't map to the same property, we verify that upfront).
            // If the property value is non-atomic, then it is a complex value, we might want to merge the new value comming from EPM with it.
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

            // TODO: Server seems to have a bug where if there's an EPM for an open property where the EPM uses complex types (the source path is deeper than 1)
            // then it will actually materialize the property as entry level string property, with the value of the deep property value. If there are multiple deep
            // EPM for the same top-level open property it seems to set the entry level property multiple times with the values as they come from payload.
            // Client on the other hand doesn't have open properties, and always has a type, so no problem there.
            if (propertyMetadata == null && propertyValuePathIndex != epmInfo.PropertyValuePath.Length - 1)
            {
                throw new ODataException(ODataErrorStrings.EpmReader_OpenComplexOrCollectionEpmProperty(epmInfo.Attribute.SourcePath));
            }

            // Open properties in EPM are by default of type Edm.String - there's no way to specify a typename in EPM
            // consumer is free to do the conversion later on if it needs to.
            // Note that this effectively means that ODataMessageReaderSettings.DisablePrimitiveTypeConversion is as if it's turned on for open EPM properties.
            IEdmTypeReference propertyType;
            if (propertyMetadata == null ||
                (this.MessageReaderSettings.DisablePrimitiveTypeConversion && propertyMetadata.Type.IsODataPrimitiveTypeKind()))
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
                            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmReader_SetEpmValueForSegment_StreamProperty));
                        }

                        object primitiveValue;
                        if (propertyValue == null)
                        {
                            ReaderValidationUtils.ValidateNullValue(
                                this.atomInputContext.Model, 
                                propertyType, 
                                this.atomInputContext.MessageReaderSettings,
                                /*validateNullValue*/ true, 
                                this.atomInputContext.Version, 
                                propertyName);

                            primitiveValue = null;
                        }
                        else
                        {
                            // Convert the value to the desired target type
                            primitiveValue = AtomValueUtils.ConvertStringToPrimitive((string)propertyValue, propertyType.AsPrimitive());
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
                    IEdmComplexTypeReference complexPropertyTypeReference = propertyType.AsComplex();
                    Debug.Assert(complexPropertyTypeReference != null, "complexPropertyTypeReference != null");
                    this.SetEpmValueForSegment(
                        epmInfo,
                        propertyValuePathIndex + 1,
                        complexPropertyTypeReference,
                        existingComplexValue.Properties.ToReadOnlyEnumerable("Properties"),
                        propertyValue);

                    break;

                case EdmTypeKind.Collection:
                    Debug.Assert(propertyType.IsNonEntityCollectionType(), "Collection types in EPM must be atomic.");

                    // In this case the property value is the internal list of items.
                    // Create a new collection value and set the list as the list of items on it.
                    ODataCollectionValue collectionValue = new ODataCollectionValue
                    {
                        TypeName = propertyType.ODataFullName(),
                        Items = new ReadOnlyEnumerable((List<object>)propertyValue)
                    };

                    this.AddEpmPropertyValue(existingProperties, propertyName, collectionValue, segmentStructuralTypeReference.IsODataEntityTypeKind());

                    break;
                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmReader_SetEpmValueForSegment_TypeKind));
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
        private void AddEpmPropertyValue(ReadOnlyEnumerable<ODataProperty> properties, string propertyName, object propertyValue, bool checkDuplicateEntryPropertyNames)
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

            properties.AddToSourceList(property);
        }
    }
}
