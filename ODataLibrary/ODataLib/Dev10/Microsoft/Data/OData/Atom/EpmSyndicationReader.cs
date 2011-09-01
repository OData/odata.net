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
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Reader for the EPM syndication-only. Read the EPM properties from ATOM metadata OM.
    /// </summary>
    internal sealed class EpmSyndicationReader : EpmReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <param name="messageReaderSettings">The reader settings to use.</param>
        private EpmSyndicationReader(
            IODataAtomReaderEntryState entryState,
            ODataVersion version,
            ODataMessageReaderSettings messageReaderSettings)
            : base(entryState, version, messageReaderSettings)
        {
        }

        /// <summary>
        /// Reads the syndication EPM for an entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="version">The OData protocol version to use.</param>
        /// <param name="messageReaderSettings">The reader settings to use.</param>
        internal static void ReadEntryEpm(
            IODataAtomReaderEntryState entryState,
            ODataVersion version,
            ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.CachedEpm != null, "To read entry EPM, the entity type must have an EPM annotation.");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            EpmSyndicationReader epmSyndicationReader = new EpmSyndicationReader(entryState, version, messageReaderSettings);
            epmSyndicationReader.ReadEntryEpm();
        }

        /// <summary>
        /// Reads an EPM for the entire entry.
        /// </summary>
        private void ReadEntryEpm()
        {
            // Note that this property access will create empty entry metadata if none were read yet.
            AtomEntryMetadata entryMetadata = this.EntryState.AtomEntryMetadata;
            Debug.Assert(entryMetadata != null, "entryMetadata != null");

            // If there are no syndication mappings, just return null.
            EpmTargetPathSegment syndicationRootSegment = this.EntryState.CachedEpm.EpmTargetTree.SyndicationRoot;
            Debug.Assert(syndicationRootSegment != null, "EPM Target tree must always have syndication root.");
            if (syndicationRootSegment.SubSegments.Count == 0)
            {
                return;
            } 

            foreach (EpmTargetPathSegment targetSegment in syndicationRootSegment.SubSegments)
            {
                if (targetSegment.IsMultiValueProperty)
                {
                    this.ReadMultiValueSegment(targetSegment, entryMetadata);
                }
                else if (targetSegment.HasContent)
                {
                    this.ReadPropertyValueSegment(targetSegment, entryMetadata);
                }
                else
                {
                    this.ReadParentSegment(targetSegment, entryMetadata);
                }
            }
        }

        /// <summary>
        /// Reads a segment which maps to a multivalue property.
        /// </summary>
        /// <param name="targetSegment">The segment being read.</param>
        /// <param name="entryMetadata">The ATOM entry metadata to read from.</param>
        private void ReadMultiValueSegment(EpmTargetPathSegment targetSegment, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(entryMetadata != null, "entryMetadata != null");

            EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
            Debug.Assert(
                epmInfo != null && epmInfo.Attribute != null,
                "MultiValue property target segment must have EpmInfo and the Epm Attribute.");

            List<object> items = this.CreateEntryEpmMultiValue(epmInfo);
            IEdmTypeReference itemTypeReference = epmInfo.MultiValueItemTypeReference;
            switch (targetSegment.SegmentName)
            {
                case AtomConstants.AtomAuthorElementName:
                    foreach (AtomPersonMetadata personMetadata in entryMetadata.Authors)
                    {
                        this.ReadPersonEpm(EpmReader.CreateMultiValueItemTargetList(epmInfo, items), itemTypeReference, targetSegment, personMetadata);
                    }

                    break;
                case AtomConstants.AtomContributorElementName:
                    foreach (AtomPersonMetadata personMetadata in entryMetadata.Contributors)
                    {
                        this.ReadPersonEpm(EpmReader.CreateMultiValueItemTargetList(epmInfo, items), itemTypeReference, targetSegment, personMetadata);
                    }

                    break;
                case AtomConstants.AtomCategoryElementName:
                    foreach (AtomCategoryMetadata categoryMetadata in this.FilterCategoriesForTargetPath(targetSegment, entryMetadata.Categories, this.EntryState.CachedEpm.EpmTargetTree))
                    {
                        this.ReadCategoryEpm(EpmReader.CreateMultiValueItemTargetList(epmInfo, items), itemTypeReference, targetSegment, categoryMetadata);
                    }

                    break;
                case AtomConstants.AtomLinkElementName:
                    foreach (AtomLinkMetadata linkMetadata in this.FilterLinksForTargetPath(targetSegment, entryMetadata.Links, this.EntryState.CachedEpm.EpmTargetTree))
                    {
                        this.ReadLinkEpm(EpmReader.CreateMultiValueItemTargetList(epmInfo, items), itemTypeReference, targetSegment, linkMetadata);
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadEntryEpm_MultiValueTarget));
            }
        }

        /// <summary>
        /// Reads a leaf segment which maps to a property value.
        /// </summary>
        /// <param name="targetSegment">The segment being read.</param>
        /// <param name="entryMetadata">The ATOM entry metadata to read from.</param>
        private void ReadPropertyValueSegment(EpmTargetPathSegment targetSegment, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(entryMetadata != null, "entryMetadata != null");

            EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
            Debug.Assert(
                epmInfo != null && epmInfo.Attribute != null,
                "If the segment has content it must have EpmInfo which in turn must have the Epm attribute");

            switch (epmInfo.Attribute.TargetSyndicationItem)
            {
                case SyndicationItemProperty.Updated:
                    if (entryMetadata.Updated.HasValue)
                    {
                        this.SetEntryEpmValue(targetSegment.EpmInfo, XmlConvert.ToString(entryMetadata.Updated.Value));
                    }

                    break;
                case SyndicationItemProperty.Published:
                    if (entryMetadata.Published.HasValue)
                    {
                        this.SetEntryEpmValue(targetSegment.EpmInfo, XmlConvert.ToString(entryMetadata.Published.Value));
                    }

                    break;
                case SyndicationItemProperty.Rights:
                    this.ReadTextConstructEpm(targetSegment, entryMetadata.Rights);
                    break;
                case SyndicationItemProperty.Summary:
                    this.ReadTextConstructEpm(targetSegment, entryMetadata.Summary);
                    break;
                case SyndicationItemProperty.Title:
                    this.ReadTextConstructEpm(targetSegment, entryMetadata.Title);
                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadEntryEpm_ContentTarget));
            }
        }

        /// <summary>
        /// Reads a non-leaf segment which has sub segments.
        /// </summary>
        /// <param name="targetSegment">The segment being read.</param>
        /// <param name="entryMetadata">The ATOM entry metadata to read from.</param>
        private void ReadParentSegment(EpmTargetPathSegment targetSegment, AtomEntryMetadata entryMetadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(entryMetadata != null, "entryMetadata != null");
            bool found = false;
            switch (targetSegment.SegmentName)
            {
                case AtomConstants.AtomAuthorElementName:
                    // If a singleton property (non-multivalue) is mapped to author and there are multiple authors
                    // EPM uses the first author.
                    AtomPersonMetadata authorMetadata = entryMetadata.Authors.FirstOrDefault();
                    if (authorMetadata != null)
                    {
                        this.ReadPersonEpm(
                            ReaderUtils.GetPropertiesList(this.EntryState.Entry.Properties),
                            this.EntryState.EntityType.ToTypeReference(),
                            targetSegment,
                            authorMetadata);
                    }

                    break;
                case AtomConstants.AtomContributorElementName:
                    // If a singleton property (non-multivalue) is mapped to contributor and there are multiple contributors
                    // EPM uses the first contributor.
                    AtomPersonMetadata contributorMetadata = entryMetadata.Contributors.FirstOrDefault();
                    if (contributorMetadata != null)
                    {
                        this.ReadPersonEpm(
                            ReaderUtils.GetPropertiesList(this.EntryState.Entry.Properties),
                            this.EntryState.EntityType.ToTypeReference(),
                            targetSegment,
                            contributorMetadata);
                    }

                    break;
                case AtomConstants.AtomLinkElementName:
                    foreach (var linkMetadata in this.FilterLinksForTargetPath(targetSegment, entryMetadata.Links, this.EntryState.CachedEpm.EpmTargetTree))
                    {
                        // this is a non-MultiValue property so we do not allow multiple values
                        if (!found)
                        {
                            this.ReadLinkEpm(
                                ReaderUtils.GetPropertiesList(this.EntryState.Entry.Properties),
                                this.EntryState.EntityType.ToTypeReference(),
                                targetSegment,
                                linkMetadata);
                            found = true;
                        }
                        else
                        {
                            throw new ODataException(Strings.EpmSyndicationReader_MultipleValuesForNonMultiValueProperty(
                                targetSegment.SubSegments[0].EpmInfo.Attribute.SourcePath,
                                targetSegment.SubSegments[0].EpmInfo.DefiningType.Name,
                                targetSegment.SubSegments[0].EpmInfo.Attribute.TargetPath));
                        }
                    }

                    break;
                case AtomConstants.AtomCategoryElementName:
                    foreach (var categoryMetadata in this.FilterCategoriesForTargetPath(targetSegment, entryMetadata.Categories, this.EntryState.CachedEpm.EpmTargetTree))
                    {
                        // this is a non-MultiValue property so we do not allow multiple values
                        if (!found)
                        {
                            this.ReadCategoryEpm(
                                ReaderUtils.GetPropertiesList(this.EntryState.Entry.Properties),
                                this.EntryState.EntityType.ToTypeReference(),
                                targetSegment,
                                categoryMetadata);
                            found = true;
                        }
                        else
                        {
                            throw new ODataException(Strings.EpmSyndicationReader_MultipleValuesForNonMultiValueProperty(
                                targetSegment.SubSegments[0].EpmInfo.Attribute.SourcePath,
                                targetSegment.SubSegments[0].EpmInfo.DefiningType.Name,
                                targetSegment.SubSegments[0].EpmInfo.Attribute.TargetPath));
                        }
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadParentSegment_TargetSegmentName));
            }
        }

        /// <summary>
        /// Reads EPM values from a person construct (author or contributor).
        /// </summary>
        /// <param name="targetList">The target list, this can be either a list of properties (on entry or complex value),
        /// or a list of items (for multivalue of primitive types).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity, complex or primitive).</param>
        /// <param name="targetSegment">The target segment which points to either author or contributor element.</param>
        /// <param name="personMetadata">The person ATOM metadata to read from.</param>
        private void ReadPersonEpm(IList targetList, IEdmTypeReference targetTypeReference, EpmTargetPathSegment targetSegment, AtomPersonMetadata personMetadata)
        {
            Debug.Assert(targetList != null, "targetList != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(
                targetSegment.SegmentName == AtomConstants.AtomAuthorElementName || targetSegment.SegmentName == AtomConstants.AtomContributorElementName,
                "targetSegment must be author or contributor.");
            Debug.Assert(personMetadata != null, "personMetadata != null");

            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                Debug.Assert(subSegment.HasContent, "sub segment of author segment must have content, there are no subsegments which don't have content under author.");
                Debug.Assert(
                    subSegment.EpmInfo != null && subSegment.EpmInfo.Attribute != null && subSegment.EpmInfo.Attribute.TargetSyndicationItem != SyndicationItemProperty.CustomProperty,
                    "We should never find a subsegment without EPM attribute or for custom mapping when writing syndication person EPM.");
                switch (subSegment.EpmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorName:
                    case SyndicationItemProperty.ContributorName:
                        // Note that person name can never specify true null in EPM, since it can't have the m:null attribute on it.
                        string personName = personMetadata.Name;
                        if (personName != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, subSegment.EpmInfo, personName);
                        }

                        break;

                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.ContributorEmail:
                        string personEmail = personMetadata.Email;
                        if (personEmail != null)
                        {
                            this.ValidatePersonEmailOrUriValue(subSegment, personEmail);
                            this.SetEpmValue(targetList, targetTypeReference, subSegment.EpmInfo, personEmail);
                        }

                        break;

                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorUri:
                        string personUri = personMetadata.UriFromEpm;
                        if (personUri != null)
                        {
                            this.ValidatePersonEmailOrUriValue(subSegment, personUri);
                            this.SetEpmValue(targetList, targetTypeReference, subSegment.EpmInfo, personUri);
                        }

                        break;

                    default:
                        // Unhandled EpmTargetPathSegment.SegmentName.
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadPersonEpm));
                }
            }
        }

        /// <summary>
        /// Validates a value to be written to ATOM person email or uri element.
        /// </summary>
        /// <param name="targetPathSegment">The target EPM path for the property.</param>
        /// <param name="stringValue">The string value read from the payload.</param>
        private void ValidatePersonEmailOrUriValue(EpmTargetPathSegment targetPathSegment, string stringValue)
        {
            Debug.Assert(
                targetPathSegment.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.AuthorEmail ||
                targetPathSegment.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.AuthorUri ||
                targetPathSegment.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.ContributorEmail ||
                targetPathSegment.EpmInfo.Attribute.TargetSyndicationItem == SyndicationItemProperty.ContributorUri,
                "This method can only be called for author/email, author/uri, contributor/email or contributor/uri mappings.");

            // In V2 (for backward compatibility) empty strings are allowed to be mapped to person/email or person/uri. In V3 these are not allowed
            // since ATOM spec doesn't allow these elements to have empty content. Also if the source of the mapping is a multivalue we don't allow
            // the empty string either (MultiValue is a V3 feature and thus no backward compatibility burden). We do allow reading multivalues 
            // even in V1 (relaxed reading), so we need to check for those explicitely.
            if ((targetPathSegment.EpmInfo.MultiValueStatus == EntityPropertyMappingMultiValueStatus.MultiValueItemProperty || this.Version >= ODataVersion.V3) && 
                stringValue != null && stringValue.Length == 0)
            {
                throw new ODataException(Strings.EpmSyndicationReader_EmptyValueForAtomPersonEmailOrUri(targetPathSegment.EpmInfo.Attribute.TargetSyndicationItem.ToString()));
            }
        }

        /// <summary>
        /// Reads the value of the ATOM text construct and sets it to the EPM.
        /// </summary>
        /// <param name="targetSegment">The EPM target segment for the value to read.</param>
        /// <param name="textConstruct">The text construct to read it from (can be null).</param>
        private void ReadTextConstructEpm(EpmTargetPathSegment targetSegment, AtomTextConstruct textConstruct)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.HasContent, "We can only read text construct values into segments which are mapped to a leaf property.");

            if (textConstruct != null)
            {
                if (textConstruct.HasMetadataNullAttribute)
                {
                    Debug.Assert(this.Version >= ODataVersion.V3, "The m:null should have been read only if the payload version is V3 or higher.");
                    this.SetEntryEpmValue(targetSegment.EpmInfo, null);
                }
                else if (textConstruct.Text != null)
                {
                    this.SetEntryEpmValue(targetSegment.EpmInfo, textConstruct.Text);
                }
            }
        }

        /// <summary>
        /// Reads EPM values from a link.
        /// </summary>
        /// <param name="targetList">The target list, this can be either a list of properties (on entry or complex value),
        /// or a list of items (for multivalue of primitive types).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity, complex or primitive).</param>
        /// <param name="targetSegment">The target segment which points to link element.</param>
        /// <param name="linkMetadata">The link ATOM metadata to read from.</param>
        private void ReadLinkEpm(IList targetList, IEdmTypeReference targetTypeReference, EpmTargetPathSegment targetSegment, AtomLinkMetadata linkMetadata)
        {
            Debug.Assert(targetList != null, "targetList != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.SegmentName == AtomConstants.AtomLinkElementName, "Only link EPM is supported by this method.");
            Debug.Assert(linkMetadata != null, "linkMetadata != null");

            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                Debug.Assert(subSegment.HasContent, "Subsegments of atom:link segment must be mapped to property, they can't be intermediary.");
                EntityPropertyMappingInfo epmInfo = subSegment.EpmInfo;
                switch (epmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.LinkHref:
                        string linkHref = linkMetadata.HrefFromEpm;
                        if (linkHref != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkHref);
                        }

                        break;
                    case SyndicationItemProperty.LinkHrefLang:
                        string linkHrefLang = linkMetadata.HrefLang;
                        if (linkHrefLang != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkHrefLang);
                        }

                        break;
                    case SyndicationItemProperty.LinkLength:
                        int? linkLength = linkMetadata.Length;
                        if (linkLength.HasValue)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkLength.Value.ToString(CultureInfo.InvariantCulture));
                        }

                        break;
                    case SyndicationItemProperty.LinkRel:
                        string linkRel = linkMetadata.Relation;
                        if (linkRel != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkRel);
                        }

                        break;
                    case SyndicationItemProperty.LinkTitle:
                        string linkTitle = linkMetadata.Title;
                        if (linkTitle != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkTitle);
                        }

                        break;
                    case SyndicationItemProperty.LinkType:
                        string linkType = linkMetadata.MediaType;
                        if (linkType != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, linkType);
                        }

                        break;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadLinkEpm));
                }
            }
        }

        /// <summary>
        /// Reads EPM values from a category.
        /// </summary>
        /// <param name="targetList">The target list, this can be either a list of properties (on entry or complex value),
        /// or a list of items (for multivalue of primitive types).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity, complex or primitive).</param>
        /// <param name="targetSegment">The target segment which points to category element.</param>
        /// <param name="categoryMetadata">The category ATOM metadata to read from.</param>
        private void ReadCategoryEpm(IList targetList, IEdmTypeReference targetTypeReference, EpmTargetPathSegment targetSegment, AtomCategoryMetadata categoryMetadata)
        {
            Debug.Assert(targetList != null, "targetList != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.SegmentName == AtomConstants.AtomCategoryElementName, "Only category EPM is supported by this method.");
            Debug.Assert(categoryMetadata != null, "categoryMetadata != null");

            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                Debug.Assert(subSegment.HasContent, "Subsegments of atom:category segment must be mapped to property, they can't be intermediary.");
                EntityPropertyMappingInfo epmInfo = subSegment.EpmInfo;
                switch (epmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.CategoryLabel:
                        string categoryLabel = categoryMetadata.Label;
                        if (categoryLabel != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, categoryLabel);
                        }

                        break;
                    case SyndicationItemProperty.CategoryScheme:
                        string categoryScheme = categoryMetadata.Scheme;
                        if (categoryScheme != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, categoryScheme);
                        }

                        break;
                    case SyndicationItemProperty.CategoryTerm:
                        string categoryTerm = categoryMetadata.Term;
                        if (categoryTerm != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, epmInfo, categoryTerm);
                        }

                        break;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadCategoryEpm));
                }
            }
        }

        /// <summary>
        /// Filter <paramref name="links"/> by criteria specified on <paramref name="targetPathSegment"/>
        /// </summary>
        /// <param name="targetPathSegment">The EPM target path segment.</param>
        /// <param name="links">Enumeration of AtomLinkMetadata objects to filter.</param>
        /// <param name="targetTree">Target tree of mapping.</param>
        /// <returns>Enumeration of AtomLinkMetadata objects by criteria defined on targetPathSegment.</returns>
        private IEnumerable<AtomLinkMetadata> FilterLinksForTargetPath(EpmTargetPathSegment targetPathSegment, IEnumerable<AtomLinkMetadata> links, EpmTargetTree targetTree)
        {
            Debug.Assert(targetPathSegment != null, "targetPathSegment != null");
            Debug.Assert(links != null, "links != null");
            Debug.Assert(targetTree != null, "targetTree != null");

            string criteriaValue = targetPathSegment.CriteriaValue;
            if (criteriaValue != null)
            {
                Debug.Assert(targetPathSegment.Criteria == EpmSyndicationCriteria.LinkRel, "We are filtering links and there is a criteria so it has to be EpmSyndicationCriteria.LinkRel");
                foreach (AtomLinkMetadata linkMetadata in
                    links.Where(linkMetadata => criteriaValue.Equals(linkMetadata.Relation, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return linkMetadata;
                }
            }
            else
            {
                Debug.Assert(targetPathSegment.Criteria == EpmSyndicationCriteria.None, "criteriaValue == null, so criteria must be None");
                foreach (AtomLinkMetadata linkMetadata in links)
                {
                    string rel = linkMetadata.Relation;
                    if (EntityPropertyMappingInfo.IsValidLinkRelCriteriaValue(rel) && !targetTree.HasLinkRelCriteriaValue(rel))
                    {
                        yield return linkMetadata;
                    }
                }
            }
        }

        /// <summary>
        /// Filter <paramref name="categories"/> by criteria specified on <paramref name="targetPathSegment"/>
        /// </summary>
        /// <param name="targetPathSegment">The EPM target path segment.</param>
        /// <param name="categories">Enumeration of AtomCategoryMetadata objects to filter.</param>
        /// <param name="targetTree">Target tree of mapping.</param>
        /// <returns>Enumeration of AtomCategoryMetadata objects by criteria defined on targetPathSegment.</returns>
        private IEnumerable<AtomCategoryMetadata> FilterCategoriesForTargetPath(EpmTargetPathSegment targetPathSegment, IEnumerable<AtomCategoryMetadata> categories, EpmTargetTree targetTree)
        {
            Debug.Assert(targetPathSegment != null, "targetPathSegment != null");
            Debug.Assert(categories != null, "categories != null");
            Debug.Assert(targetTree != null, "targetTree != null");

            string criteriaValue = targetPathSegment.CriteriaValue;
            if (criteriaValue != null)
            {
                Debug.Assert(targetPathSegment.Criteria == EpmSyndicationCriteria.CategoryScheme, "We are filtering categories and there is a criteria so it has to be EpmSyndicationCriteria.CategoryScheme");
                foreach (AtomCategoryMetadata categoryMetadata in categories)
                {
                    string scheme = categoryMetadata.Scheme;
                    if (scheme != null && scheme.Equals(criteriaValue, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return categoryMetadata;
                    }
                }
            }
            else
            {
                Debug.Assert(targetPathSegment.Criteria == EpmSyndicationCriteria.None, "criteriaValue == null, so criteria must be None");
                foreach (AtomCategoryMetadata categoryMetadata in categories)
                {
                    string scheme = categoryMetadata.Scheme;

                    // scheme is not mandatory for non-conditional mapping to atom:category
                    if (scheme == null || EntityPropertyMappingInfo.IsValidCategorySchemeCriteriaValue(scheme) && !targetTree.HasCategorySchemeCriteriaValue(scheme))
                    {
                        yield return categoryMetadata;
                    }
                }
            }
        }
    }
}
