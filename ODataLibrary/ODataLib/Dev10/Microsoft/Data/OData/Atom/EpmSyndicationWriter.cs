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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Writer for the EPM syndication-only. Writes the EPM properties into ATOM metadata OM.
    /// </summary>
    internal sealed class EpmSyndicationWriter : EpmWriter
    {
        /// <summary>The EPM target tree to use.</summary>
        private readonly EpmTargetTree epmTargetTree;

        /// <summary>Atom entry metadata to write to.</summary>
        private readonly AtomEntryMetadata entryMetadata;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="epmTargetTree">The EPM target tree to use.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        private EpmSyndicationWriter(
            EpmTargetTree epmTargetTree,
            IEdmModel model,
            ODataVersion version,
            ODataWriterBehavior writerBehavior)
            : base(model, version, writerBehavior)
        {
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            this.epmTargetTree = epmTargetTree;
            this.entryMetadata = new AtomEntryMetadata();
        }

        /// <summary>
        /// Writes the syndication part of EPM for an entry into ATOM metadata OM.
        /// </summary>
        /// <param name="epmTargetTree">The EPM target tree to use.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="type">The type of the entry.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <returns>The ATOM metadata OM with the EPM values populated.</returns>
        internal static AtomEntryMetadata WriteEntryEpm(
            EpmTargetTree epmTargetTree,
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference type,
            IEdmModel model,
            ODataVersion version,
            ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmTargetTree != null, "epmTargetTree != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(type != null, "For any EPM to exist the metadata must be available.");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            EpmSyndicationWriter epmWriter = new EpmSyndicationWriter(epmTargetTree, model, version, writerBehavior);
            return epmWriter.WriteEntryEpm(epmValueCache, type);
        }

        /// <summary>
        /// Writes the syndication part of EPM for an entry into ATOM metadata OM.
        /// </summary>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="entityType">The type of the entry.</param>
        /// <returns>The ATOM metadata OM with the EPM values populated.</returns>
        private AtomEntryMetadata WriteEntryEpm(
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference entityType)
        {
            // If there are no syndication mappings, just return null.
            EpmTargetPathSegment syndicationRootSegment = this.epmTargetTree.SyndicationRoot;
            Debug.Assert(syndicationRootSegment != null, "EPM Target tree must always have syndication root.");
            if (syndicationRootSegment.SubSegments.Count == 0)
            {
                return null;
            }

            foreach (EpmTargetPathSegment targetSegment in syndicationRootSegment.SubSegments)
            {
                if (targetSegment.IsMultiValueProperty)
                {
                    Debug.Assert(
                        targetSegment.EpmInfo != null && targetSegment.EpmInfo.Attribute != null,
                        "MultiValue property target segment must have EpmInfo and the EPM attribute.");

                    ODataVersionChecker.CheckMultiValueProperties(this.Version, targetSegment.EpmInfo.Attribute.SourcePath);

                    this.WriteMultiValue(targetSegment, epmValueCache, entityType);
                }
                else if (targetSegment.HasContent)
                {
                    EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
                    Debug.Assert(
                        epmInfo != null && epmInfo.Attribute != null,
                        "If the segment has content it must have EpmInfo which in turn must have the EPM attribute");

                    // We can safely ignore both nullOnParentProperty and valueTypeReference since those are only useful for multivalues
                    // but here we expect primitive values only. For primitive values null is always treated the same and the type
                    // is not needed since the actual CLR type of the value is enough.
                    bool nullOnParentProperty;
                    IEdmTypeReference valueTypeReference;
                    object propertyValue = this.ReadEntryPropertyValue(
                        epmInfo,
                        epmValueCache,
                        entityType,
                        out valueTypeReference,
                        out nullOnParentProperty);
                    string textPropertyValue = EpmWriterUtils.GetPropertyValueAsText(propertyValue);

                    switch (epmInfo.Attribute.TargetSyndicationItem)
                    {
                        case SyndicationItemProperty.Updated:
                            this.entryMetadata.Updated = this.CreateDateTimeValue(propertyValue, SyndicationItemProperty.Updated);
                            break;
                        case SyndicationItemProperty.Published:
                            this.entryMetadata.Published = this.CreateDateTimeValue(propertyValue, SyndicationItemProperty.Published);
                            break;
                        case SyndicationItemProperty.Rights:
                            this.entryMetadata.Rights = this.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        case SyndicationItemProperty.Summary:
                            this.entryMetadata.Summary = this.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        case SyndicationItemProperty.Title:
                            this.entryMetadata.Title = this.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        default:
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteEntryEpm_ContentTarget));
                    }
                }
                else
                {
                    this.WriteParentSegment(targetSegment, epmValueCache, entityType);
                }
            }

            return this.entryMetadata;
        }

        /// <summary>
        /// Writes a non-leaf segment which has sub segments.
        /// </summary>
        /// <param name="targetSegment">The segment being written</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or multivalue item.</param>
        private void WriteParentSegment(EpmTargetPathSegment targetSegment, object epmValueCache, IEdmTypeReference typeReference)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");

            if (targetSegment.SegmentName == AtomConstants.AtomAuthorElementName)
            {
                AtomPersonMetadata authorMetadata = this.WritePersonEpm(targetSegment, epmValueCache, typeReference);

                if (authorMetadata != null)
                {
                    List<AtomPersonMetadata> authors = (List<AtomPersonMetadata>)this.entryMetadata.Authors;
                    if (authors == null)
                    {
                        authors = new List<AtomPersonMetadata>();
                        this.entryMetadata.Authors = authors;
                    }

                    authors.Add(authorMetadata);
                }
            }
            else if (targetSegment.SegmentName == AtomConstants.AtomContributorElementName)
            {
                AtomPersonMetadata contributorMetadata = this.WritePersonEpm(targetSegment, epmValueCache, typeReference);

                if (contributorMetadata != null)
                {
                    List<AtomPersonMetadata> contributors = (List<AtomPersonMetadata>)this.entryMetadata.Contributors;
                    if (contributors == null)
                    {
                        contributors = new List<AtomPersonMetadata>();
                        this.entryMetadata.Contributors = contributors;
                    }

                    contributors.Add(contributorMetadata);
                }
            }
            else if (targetSegment.SegmentName == AtomConstants.AtomLinkElementName)
            {
                AtomLinkMetadata linkMetadata = this.WriteLinkEpm(targetSegment, epmValueCache, typeReference);

                if (targetSegment.CriteriaValue != null)
                {
                    linkMetadata.Relation = targetSegment.CriteriaValue;
                }

                List<AtomLinkMetadata> links = (List<AtomLinkMetadata>)this.entryMetadata.Links;
                if (this.entryMetadata.Links == null)
                {
                    links = new List<AtomLinkMetadata>();
                    this.entryMetadata.Links = links;
                }

                links.Add(linkMetadata);
            }
            else if (targetSegment.SegmentName == AtomConstants.AtomCategoryElementName)
            {
                AtomCategoryMetadata categoryMetadata = this.WriteCategoryEpm(targetSegment, epmValueCache, typeReference);
                if (targetSegment.CriteriaValue != null)
                {
                    categoryMetadata.Scheme = targetSegment.CriteriaValue;
                }

                List<AtomCategoryMetadata> categories = (List<AtomCategoryMetadata>)this.entryMetadata.Categories;
                if (categories == null)
                {
                    categories = new List<AtomCategoryMetadata>();
                    this.entryMetadata.Categories = categories;
                }

                categories.Add(categoryMetadata);
            }
            else
            {
                // Unhandled EpmTargetPathSegment.SegmentName.
                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteParentSegment_TargetSegmentName));
            }
        }

        /// <summary>
        /// Writes a MultiValue property value.
        /// </summary>
        /// <param name="targetSegment">The segment being written.</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values.</param>
        /// <param name="entityType">The type of the entry.</param>
        private void WriteMultiValue(
            EpmTargetPathSegment targetSegment,
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference entityType)
        {
            Debug.Assert(
                targetSegment.EpmInfo != null && targetSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueProperty,
                "This method can only be called for multivalue property segment.");

            // The nullOnParentProperty is set to true if the multivalue "doesn't exist" meaning that some parent property of the multiValue property was null
            // in which case we should not serialize the multiValue at all
            // Note that in that case if the EPM for the multiValue property is targetting the atom:author we will fail below, because
            // atom:author element is mandatory and not writing anything out would break that.
            bool nullOnParentProperty;
            IEdmTypeReference valueTypeReference;
            object value = this.ReadEntryPropertyValue(
                targetSegment.EpmInfo,
                epmValueCache,
                entityType,
                out valueTypeReference,
                out nullOnParentProperty);
            ODataMultiValue multiValue = (ODataMultiValue)value;

            if (!nullOnParentProperty)
            {
                IEnumerable enumerable = EpmValueCache.GetMultiValueItems(epmValueCache, multiValue, false);

                // Treat null Items as empty collection.
                if (enumerable != null)
                {
                    IEdmTypeReference multiValueItemTypeReference = targetSegment.EpmInfo.MultiValueItemTypeReference;
                    Debug.Assert(multiValueItemTypeReference != null, "Each multiValue property EpmInfo must have a valid multiValue item type.");
                    Debug.Assert(
                        multiValueItemTypeReference.IsODataPrimitiveTypeKind() || multiValueItemTypeReference.IsODataComplexTypeKind(),
                        "Only multiValue of primitive or complex types are supported.");
                    Debug.Assert(
                        valueTypeReference != null && ((IEdmCollectionType)valueTypeReference.Definition).ElementType.ODataFullName() == multiValueItemTypeReference.ODataFullName(),
                        "The type of the value must match the type from metadata, we should have verified this already.");

                    // Note that foreach will call Dispose on the used IEnumerator in a finally block
                    foreach (object itemValue in enumerable)
                    {
                        object item;
                        EpmMultiValueItemCache epmItemCache = itemValue as EpmMultiValueItemCache;
                        if (epmItemCache != null)
                        {
                            item = epmItemCache.ItemValue;
                        }
                        else
                        {
                            item = itemValue;
                        }

                        ValidationUtils.ValidateMultiValueItem(item);

                        ODataComplexValue complexValue = item as ODataComplexValue;
                        if (complexValue != null)
                        {
                            Debug.Assert(epmItemCache != null, "If the value is a complex value, we need the EPM item cache to read its properties");
                            this.WriteParentSegment(targetSegment, epmItemCache, multiValueItemTypeReference);
                        }
                        else
                        {
                            this.WriteParentSegment(targetSegment, item, multiValueItemTypeReference);
                        }
                    }
                }
            }

            // We must fail if an empty multiValue was mapped to an author element
            if (targetSegment.SegmentName == AtomConstants.AtomAuthorElementName)
            {
                List<AtomPersonMetadata> authors = (List<AtomPersonMetadata>)this.entryMetadata.Authors;
                
                if (authors == null || authors.Count == 0)
                {
                    throw new ODataException(Strings.EpmSyndicationWriter_EmptyMultiValueMappedToAuthor(targetSegment.EpmInfo.Attribute.SourcePath));
                }
            }
        }

        /// <summary>
        /// Writes EPM value to a person construct (author or contributor).
        /// </summary>
        /// <param name="targetSegment">The target segment which points to either author or contributor element.</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or multivalue item.</param>
        /// <returns>The person metadata or null if no person metadata should be written for this mapping.</returns>
        private AtomPersonMetadata WritePersonEpm(EpmTargetPathSegment targetSegment, object epmValueCache, IEdmTypeReference typeReference)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(
                targetSegment.SegmentName == AtomConstants.AtomAuthorElementName || targetSegment.SegmentName == AtomConstants.AtomContributorElementName, 
                "targetSegment must be author or contributor.");

            AtomPersonMetadata personMetadata = null;
            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                Debug.Assert(subSegment.HasContent, "sub segment of author segment must have content, there are no subsegments which don't have content under author.");
                Debug.Assert(
                    subSegment.EpmInfo != null && subSegment.EpmInfo.Attribute != null && subSegment.EpmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty,
                    "We should never find a subsegment without EPM attribute or for custom mapping when writing syndication person EPM.");
                string textPropertyValue = this.GetPropertyValueAsText(subSegment, epmValueCache, typeReference);

                if (textPropertyValue == null)
                {
                    // In Multi-Values or in V3 mapping nulls to author/contributor subelements is not legal since there's no way to express it.
                    // author/contributor subelements don't allow extension attributes, so we can't add the m:null attribute.
                    // In V2 we write the mapped properties always in-content when the value is null.
                    if (this.Version >= ODataVersion.V3)
                    {
                        throw new ODataException(Strings.EpmSyndicationWriter_NullPropertyMappedToElementWhichDoesntAllowIt(subSegment.EpmInfo.Attribute.TargetSyndicationItem.ToString()));
                    }

                    continue;
                }

                // Initialize the person element only if we actually need to write something to it.
                Debug.Assert(subSegment.EpmInfo != null && subSegment.EpmInfo.Attribute != null, "The author subsegment must have EPM info and EPM attribute.");
                switch (subSegment.EpmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorName:
                    case SyndicationItemProperty.ContributorName:
                        if (personMetadata == null)
                        {
                            personMetadata = new AtomPersonMetadata();
                        }

                        personMetadata.Name = textPropertyValue;
                        break;
                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.ContributorEmail:
                        this.ValidatePersonEmailOrUriValue(subSegment.EpmInfo.Attribute.TargetSyndicationItem, textPropertyValue);

                        if (textPropertyValue.Length > 0)
                        {
                            if (personMetadata == null)
                            {
                                personMetadata = new AtomPersonMetadata();
                            }

                            personMetadata.Email = textPropertyValue;
                        }

                        break;
                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorUri:
                        this.ValidatePersonEmailOrUriValue(subSegment.EpmInfo.Attribute.TargetSyndicationItem, textPropertyValue);

                        if (textPropertyValue.Length > 0)
                        {
                            if (personMetadata == null)
                            {
                                personMetadata = new AtomPersonMetadata();
                            }
                            
                            personMetadata.UriFromEpm = textPropertyValue;
                        }

                        break;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WritePersonEpm));
                }
            }

            return personMetadata;
        }

        /// <summary>
        /// Write EPM to AtomLinkMetadata
        /// </summary>
        /// <param name="targetPathSegment">The target segment which points to a Link element</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or multivalue item.</param>
        /// <returns>An AtomLinkMetadata representing targetPathSegment</returns>
        private AtomLinkMetadata WriteLinkEpm(
            EpmTargetPathSegment targetPathSegment, 
            object epmValueCache, 
            IEdmTypeReference typeReference)
        {
            AtomLinkMetadata linkMetadata = new AtomLinkMetadata();
            foreach (EpmTargetPathSegment subSegment in targetPathSegment.SubSegments)
            {
                EntityPropertyMappingInfo epmInfo = subSegment.EpmInfo;

                string textPropertyValue = this.GetPropertyValueAsText(subSegment, epmValueCache, typeReference);
                if (textPropertyValue == null)
                {
                    throw new ODataException(Strings.EpmSyndicationWriter_NullValueForAttributeTarget(
                        epmInfo.Attribute.SourcePath,
                        epmInfo.DefiningType.ODataFullName(),
                        epmInfo.Attribute.TargetPath));
                }

                switch (epmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.LinkHref:
                        linkMetadata.HrefFromEpm = textPropertyValue;
                        break;
                    case SyndicationItemProperty.LinkHrefLang:
                        linkMetadata.HrefLang = textPropertyValue;
                        break;
                    case SyndicationItemProperty.LinkLength:
                        {
                            int length;
                            if (int.TryParse(textPropertyValue, NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out length))
                            {
                                linkMetadata.Length = length;
                            }
                            else
                            {
                                throw new ODataException(Strings.EpmSyndicationWriter_InvalidLinkLengthValue(textPropertyValue));
                            }
                        }

                        break;
                    case SyndicationItemProperty.LinkRel:
                        if (!EntityPropertyMappingInfo.IsValidLinkRelCriteriaValue(textPropertyValue))
                        {
                            throw new ODataException(Strings.EpmSyndicationWriter_InvalidValueForLinkRelCriteriaAttribute(
                                textPropertyValue,
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.ODataFullName()));
                        }

                        if (this.epmTargetTree.HasLinkRelCriteriaValue(textPropertyValue))
                        {
                            throw new ODataException(Strings.EpmSyndicationWriter_UpdateNonConditionalCriteriaAttributeValueSameAsCondition(
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.ODataFullName(),
                                epmInfo.Attribute.TargetPath,
                                textPropertyValue));
                        }

                        linkMetadata.Relation = textPropertyValue;
                        break;
                    case SyndicationItemProperty.LinkTitle:
                        linkMetadata.Title = textPropertyValue;
                        break;
                    case SyndicationItemProperty.LinkType:
                        linkMetadata.MediaType = textPropertyValue;
                        break;
                    default:
                        // Unhandled SyndicationItemProperty enum value - should never get here.
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteLinkEpm_TargetSyndicationItem));
                }
            }

            return linkMetadata;
        }

        /// <summary>
        /// Write EPM to AtomCategoryMetadata
        /// </summary>
        /// <param name="targetPathSegment">The target segment which points to a Category element</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or multivalue item.</param>
        /// <returns>An AtomCategoryMetadata representing <paramref name="targetPathSegment"/></returns>
        private AtomCategoryMetadata WriteCategoryEpm(
            EpmTargetPathSegment targetPathSegment,
            object epmValueCache,
            IEdmTypeReference typeReference)
        {
            AtomCategoryMetadata categoryMetadata = new AtomCategoryMetadata();
            foreach (EpmTargetPathSegment subSegment in targetPathSegment.SubSegments)
            {
                EntityPropertyMappingInfo epmInfo = subSegment.EpmInfo;

                string textPropertyValue = this.GetPropertyValueAsText(subSegment, epmValueCache, typeReference);
                if (textPropertyValue == null)
                {
                    throw new ODataException(Strings.EpmSyndicationWriter_NullValueForAttributeTarget(
                        epmInfo.Attribute.SourcePath,
                        epmInfo.DefiningType.ODataFullName(),
                        epmInfo.Attribute.TargetPath));
                }

                switch (epmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.CategoryLabel:
                        categoryMetadata.Label = textPropertyValue;
                        break;
                    case SyndicationItemProperty.CategoryScheme:
                        if (!EntityPropertyMappingInfo.IsValidCategorySchemeCriteriaValue(textPropertyValue))
                        {
                            throw new ODataException(Strings.EpmSyndicationWriter_InvalidValueForCategorySchemeCriteriaAttribute(
                                textPropertyValue,
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.ODataFullName()));
                        }

                        if (this.epmTargetTree.HasCategorySchemeCriteriaValue(textPropertyValue))
                        {
                            throw new ODataException(Strings.EpmSyndicationWriter_UpdateNonConditionalCriteriaAttributeValueSameAsCondition(
                                epmInfo.Attribute.SourcePath,
                                epmInfo.DefiningType.ODataFullName(),
                                epmInfo.Attribute.TargetPath,
                                textPropertyValue));
                        }

                        categoryMetadata.Scheme = textPropertyValue;
                        break;
                    case SyndicationItemProperty.CategoryTerm:
                        categoryMetadata.Term = textPropertyValue;
                        break;
                    default:
                        // Unhandled SyndicationItemProperty enum value - should never get here.
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteCategoryEpm_TargetSyndicationItem));
                }
            }

            return categoryMetadata;
        }

        /// <summary>
        /// Given a target segment the method returns the text value of the property mapped to that segment to be used in EPM.
        /// </summary>
        /// <param name="targetSegment">The target segment to read the value for.</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or multivalue item.</param>
        /// <returns>The test representation of the value, or the method throws if the text representation was not possible to obtain.</returns>
        private string GetPropertyValueAsText(
            EpmTargetPathSegment targetSegment,
            object epmValueCache,
            IEdmTypeReference typeReference)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.HasContent, "The target segment to read property for must have content.");
            Debug.Assert(targetSegment.EpmInfo != null, "The EPM info must be available on the target segment to read its property.");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(typeReference != null, "typeReference != null");

            object propertyValue;
            EntryPropertiesValueCache entryPropertiesValueCache = epmValueCache as EntryPropertiesValueCache;

            // Note that it's safe to ignore the valueTypeReference since we only handle primitive values here
            // and the primitive types can be determined from the actual CLR type of the value.
            IEdmTypeReference valueTypeReference;
            if (entryPropertiesValueCache != null)
            {
                // It's OK to ignore the nullOnParentProperty since for primitive types (the only thing we handle here) null is always treated the same.
                bool nullOnParentProperty;
                propertyValue = this.ReadEntryPropertyValue(
                    targetSegment.EpmInfo,
                    entryPropertiesValueCache,
                    typeReference.AsEntity(),
                    out valueTypeReference,
                    out nullOnParentProperty);
            }
            else
            { 
                EpmMultiValueItemCache multiValueItemCache = epmValueCache as EpmMultiValueItemCache;
                if (multiValueItemCache != null)
                {
                    propertyValue = this.ReadMultiValueItemPropertyValue(
                        targetSegment.EpmInfo,
                        multiValueItemCache,
                        typeReference,
                        out valueTypeReference);
                }
                else
                {
                    propertyValue = epmValueCache;
                    ValidationUtils.ValidateIsExpectedPrimitiveType(propertyValue, typeReference);
                }
            }

            return EpmWriterUtils.GetPropertyValueAsText(propertyValue);
        }

        /// <summary>
        /// Creates a text ATOM value.
        /// </summary>
        /// <param name="textValue">The text value to use.</param>
        /// <param name="contentKind">The content kind of the value.</param>
        /// <returns>The Atom text value.</returns>
        private AtomTextConstruct CreateAtomTextConstruct(string textValue, SyndicationTextContentKind contentKind)
        {
            bool requiresMetadataNullAttribute = false;

            // If V3 serialization is enabled write null values by adding the m:null="true" attribute.
            if (textValue == null && this.Version >= ODataVersion.V3)
            {
                // Add the m:null attribute on the value
                requiresMetadataNullAttribute = true;
            }

            AtomTextConstructKind kind;
            switch (contentKind)
            {
                case SyndicationTextContentKind.Plaintext:
                    kind = AtomTextConstructKind.Text;
                    break;
                case SyndicationTextContentKind.Html:
                    kind = AtomTextConstructKind.Html;
                    break;
                case SyndicationTextContentKind.Xhtml:
                    kind = AtomTextConstructKind.Xhtml;
                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_CreateAtomTextConstruct));
            }

            return new AtomTextConstruct
            {
                Kind = kind,
                Text = textValue,
                HasMetadataNullAttribute = requiresMetadataNullAttribute
            };
        }
        
        /// <summary>
        /// Given an object returns the corresponding DateTimeOffset value through conversions.
        /// </summary>
        /// <param name="propertyValue">Object containing property value.</param>
        /// <param name="targetProperty">The target syndication property for the mapping (used for exception messages).</param>
        /// <returns>DateTimeOffset after conversion.</returns>
        private DateTimeOffset CreateDateTimeValue(object propertyValue, SyndicationItemProperty targetProperty)
        {
            if (propertyValue == null)
            {
                // In V3 we are to use m:null extension attribute to mark null values. Due to that we need to fail
                // because syndication API doesn't allow us to put extension attributes on publish or updated ATOM elements 
                // (even though the spec allows it) and the product used syndication APIs. In future version of the protocol we might be able
                // to lift the restriction, but for now we have to keep this for backward compat.
                if (this.Version >= ODataVersion.V3)
                {
                    throw new ODataException(Strings.EpmSyndicationWriter_DateTimePropertyHasNullValue(targetProperty.ToString()));
                }
                else
                {
                    return DateTimeOffset.Now;
                }
            }

            if (propertyValue is DateTimeOffset)
            {
                return (DateTimeOffset)propertyValue;
            }
            
            if (propertyValue is DateTime)
            {
                // DateTimeOffset takes care of DateTimes of Unspecified kind so we won't end up 
                // with datetime without timezone info mapped to atom:updated or atom:published element.
                return new DateTimeOffset((DateTime)propertyValue);
            }

            string stringValue = propertyValue as string;
            if (stringValue != null)
            {
                DateTimeOffset date;
                if (!DateTimeOffset.TryParse(stringValue, out date))
                {
                    DateTime result;
                    if (!DateTime.TryParse(stringValue, out result))
                    {
                        throw new ODataException(Strings.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(targetProperty.ToString()));
                    }

                    return new DateTimeOffset(result);
                }

                return date;
            }
            
            try
            {
                return new DateTimeOffset(Convert.ToDateTime(propertyValue, CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                throw new ODataException(Strings.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(targetProperty.ToString()));
            }
        }

        /// <summary>
        /// Validates a value to be written to ATOM person email or uri element.
        /// </summary>
        /// <param name="propertyToCheck">The syndication item property the property is to be written to.</param>
        /// <param name="textPropertyValue">The text value of the property.</param>
        private void ValidatePersonEmailOrUriValue(SyndicationItemProperty propertyToCheck, string textPropertyValue)
        {
            Debug.Assert(
                propertyToCheck == SyndicationItemProperty.AuthorEmail || propertyToCheck == SyndicationItemProperty.AuthorUri ||
                propertyToCheck == SyndicationItemProperty.ContributorEmail || propertyToCheck == SyndicationItemProperty.ContributorUri,
                "This method can only be called for person email or uri targets");

            // In V3 we validate the values and don't allow empty strings to be mapped to person/uri or person/email
            // because ATOM doesn't allow for these elements to have empty content.
            // In lower version we allow these by writing the values in content (backward compat)
            if (this.Version >= ODataVersion.V3 && textPropertyValue != null && textPropertyValue.Length == 0)
            {
                throw new ODataException(Strings.EpmSyndicationWriter_EmptyValueForAtomPersonEmailOrUri(propertyToCheck.ToString()));
            }
        }
    }
}
