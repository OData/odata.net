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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Globalization;
    #endregion Namespaces.

    /// <summary>
    /// Writer for the EPM syndication-only. Writes the EPM properties into ATOM metadata OM.
    /// </summary>
    internal static class EpmSyndicationWriter
    {
        /// <summary>
        /// Writes the syndication part of EPM for an entry into ATOM metadata OM.
        /// </summary>
        /// <param name="epmTargetTree">The EPM target tree to use.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="resourceType">The resource type of the entry.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <param name="version">The version of OData protocol to use.</param>
        /// <returns>The ATOM metadata OM with the EPM values populated.</returns>
        internal static AtomEntryMetadata WriteEntryEpm(
            EpmTargetTree epmTargetTree, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata,
            ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmTargetTree != null, "epmTargetTree != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(resourceType != null, "For any EPM to exist the metadata must be available.");

            // If there are no syndication mappings, just return null.
            EpmTargetPathSegment syndicationRootSegment = epmTargetTree.SyndicationRoot;
            Debug.Assert(syndicationRootSegment != null, "EPM Target tree must always have syndication root.");
            if (syndicationRootSegment.SubSegments.Count == 0)
            {
                return null;
            }

            AtomEntryMetadata entryMetadata = new AtomEntryMetadata();

            foreach (EpmTargetPathSegment targetSegment in syndicationRootSegment.SubSegments)
            {
                if (targetSegment.IsMultiValueProperty)
                {
                    Debug.Assert(
                        targetSegment.EpmInfo != null && targetSegment.EpmInfo.Attribute != null,
                        "MultiValue property target segment must have EpmInfo and the Epm Attribute.");
                    
                    ODataVersionChecker.CheckMultiValueProperties(version, targetSegment.EpmInfo.Attribute.SourcePath);

                    // WriteMultiValueEpm(entry, targetSegment, epmValueCache);
                    throw new NotImplementedException();
                }
                else if (targetSegment.HasContent)
                {
                    EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
                    Debug.Assert(
                        epmInfo != null && epmInfo.Attribute != null, 
                        "If the segment has content it must have EpmInfo which in turn must have the Epm attribute");

                    bool nullOnParentProperty;
                    object propertyValue = epmInfo.ReadEntryPropertyValue(epmValueCache, resourceType, metadata, out nullOnParentProperty);
                    string textPropertyValue = EpmWriterUtils.GetPropertyValueAsText(propertyValue);

                    switch (epmInfo.Attribute.TargetSyndicationItem)
                    {
                        case SyndicationItemProperty.Updated:
                            entryMetadata.Updated = CreateDateTimeValue(propertyValue, SyndicationItemProperty.Updated, version);
                            break;
                        case SyndicationItemProperty.Published:
                            entryMetadata.Published = CreateDateTimeValue(propertyValue, SyndicationItemProperty.Published, version);
                            break;
                        case SyndicationItemProperty.Rights:
                            entryMetadata.Rights = CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind, version);
                            break;
                        case SyndicationItemProperty.Summary:
                            entryMetadata.Summary = CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind, version);
                            break;
                        case SyndicationItemProperty.Title:
                            entryMetadata.Title = CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind, version);
                            break;
                        default:
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteEntryEpm_ContentTarget));
                    }
                }
                else if (targetSegment.SegmentName == AtomConstants.AtomAuthorElementName)
                {
                    AtomPersonMetadata authorMetadata = WritePersonEpm(targetSegment, epmValueCache, resourceType, metadata);

                    Debug.Assert(entryMetadata.Authors == null, "Found two mappings to author, that is invalid.");
                    if (authorMetadata != null)
                    {
                        entryMetadata.Authors = CreateSinglePersonList(authorMetadata);
                    }
                }
                else if (targetSegment.SegmentName == AtomConstants.AtomContributorElementName)
                {
                    AtomPersonMetadata contributorMetadata = WritePersonEpm(targetSegment, epmValueCache, resourceType, metadata);

                    Debug.Assert(entryMetadata.Contributors == null, "Found two mappings to contributor, that is invalid.");
                    if (contributorMetadata != null)
                    {
                        entryMetadata.Contributors = CreateSinglePersonList(contributorMetadata);
                    }
                }
                else if (targetSegment.SegmentName == AtomConstants.AtomLinkElementName)
                {
                    AtomLinkMetadata linkMetadata = new AtomLinkMetadata();
                    //// WriteLinkEpm(entry, targetSegment, epmValueCache);

                    Debug.Assert(targetSegment.CriteriaValue != null, "Mapping to link must be conditional.");
                    linkMetadata.Relation = targetSegment.CriteriaValue;
                    List<AtomLinkMetadata> links;
                    if (entryMetadata.Links == null)
                    {
                        links = new List<AtomLinkMetadata>();
                        entryMetadata.Links = links;
                    }
                    else
                    {
                        links = entryMetadata.Links as List<AtomLinkMetadata>;
                        Debug.Assert(links != null, "AtomEntryMetadata.Links must be of type List<AtomLinkMetadata> since we create it like that.");
                    }

                    links.Add(linkMetadata);

                    throw new NotImplementedException();
                }
                else if (targetSegment.SegmentName == AtomConstants.AtomCategoryElementName)
                {
                    AtomCategoryMetadata categoryMetadata = new AtomCategoryMetadata();
                    //// WriteCategoryEpm(entry, targetSegment, epmValueCache)

                    Debug.Assert(targetSegment.CriteriaValue != null, "Mapping to category must be conditional.");
                    categoryMetadata.Scheme = targetSegment.CriteriaValue;
                    List<AtomCategoryMetadata> categories;
                    if (entryMetadata.Categories == null)
                    {
                        categories = new List<AtomCategoryMetadata>();
                        entryMetadata.Categories = categories;
                    }
                    else
                    {
                        categories = entryMetadata.Links as List<AtomCategoryMetadata>;
                        Debug.Assert(categories != null, "AtomEntryMetadata.Categories must be of type List<AtomCategoryMetadata> since we create it like that.");
                    }

                    categories.Add(categoryMetadata);

                    throw new NotImplementedException();
                }
                else
                {
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteEntryEpm_TargetSegment));
                }
            }

            return entryMetadata;
        }

        /// <summary>
        /// Given a target segment the method returns the text value of the property mapped to that segment to be used in EPM.
        /// </summary>
        /// <param name="targetSegment">The target segment to read the value for.</param>
        /// <param name="epmValueCache">The entry EPM value cache to use.</param>
        /// <param name="resourceType">The resource type of the entry being processed.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <returns>The test representation of the value, or the method throws if the text representation was not possible to obtain.</returns>
        private static string GetEntryPropertyValueAsText(
            EpmTargetPathSegment targetSegment, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(targetSegment.HasContent, "The target segment to read property for must have content.");
            Debug.Assert(targetSegment.EpmInfo != null, "The EPM info must be available on the target segment to read its property.");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(resourceType != null, "resourceType != null");

            bool nullOnParentProperty;
            object propertyValue = targetSegment.EpmInfo.ReadEntryPropertyValue(epmValueCache, resourceType, metadata, out nullOnParentProperty);
            return EpmWriterUtils.GetPropertyValueAsText(propertyValue);
        }

        /// <summary>
        /// Writes EPM value to a person construct (author or contributor).
        /// </summary>
        /// <param name="targetSegment">The target segment which points to either author or contributor element.</param>
        /// <param name="epmValueCache">The EPM value cache to use to get property values.</param>
        /// <param name="resourceType">The resource type of the entry being processed.</param>
        /// <param name="metadata">The metadata provider to use.</param>
        /// <returns>The person metadata or null if no person metadata should be written for this mapping.</returns>
        private static AtomPersonMetadata WritePersonEpm(
            EpmTargetPathSegment targetSegment, 
            EntryPropertiesValueCache epmValueCache, 
            ResourceType resourceType,
            DataServiceMetadataProviderWrapper metadata)
        {
            Debug.Assert(targetSegment != null, "targetSegment != null");
            Debug.Assert(
                targetSegment.SegmentName == AtomConstants.AtomAuthorElementName || targetSegment.SegmentName == AtomConstants.AtomContributorElementName, 
                "targetSegment must be author or contributor.");

            AtomPersonMetadata personMetadata = null;
            foreach (EpmTargetPathSegment subSegment in targetSegment.SubSegments)
            {
                Debug.Assert(subSegment.HasContent, "sub segment of author segment must have content, there are no subsegments which don't have content under author.");
                string textPropertyValue = GetEntryPropertyValueAsText(subSegment, epmValueCache, resourceType, metadata);

                if (textPropertyValue == null)
                {
                    // TODO: In Multi-Values or in V3 mapping nulls to author/contributor subelements is not legal since there's no way to express it.
                    // author/contributor subelements don't allow extension attributes, so we can't add the m:null attribute.
                    continue;
                }

                // Initialize the person element only if we actually need to write something to it.
                if (personMetadata == null)
                {
                    personMetadata = new AtomPersonMetadata();
                }

                Debug.Assert(subSegment.EpmInfo != null && subSegment.EpmInfo.Attribute != null, "The author subsegment must have EPM info and EPM attribute.");
                switch (subSegment.EpmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorName:
                    case SyndicationItemProperty.ContributorName:
                        personMetadata.Name = textPropertyValue;
                        break;
                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.ContributorEmail:
                        // TODO: Validate the email value. In V3 or in multi-values the email value must not be null and it must not be empty
                        // since the syndication API doesn't allow empty email values (see below) and it's questionable if ATOM allows it itself.

                        // In case the value is empty the syndication API will actually omit the email element from the payload
                        // we have to simulate that behavior here by not setting the property in that case.
                        if (textPropertyValue.Length > 0)
                        {
                            personMetadata.Email = textPropertyValue;
                        }

                        break;
                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorUri:
                        // TODO: Validate the uri value. In V3 or in multi-values the uri value must not be null and it must not be empty
                        // since the syndication API doesn't allow empty uri values (see below) and it's questionable if ATOM allows it itself.

                        // In case the value is empty the syndication API will actually omit the uri element from the payload
                        // we have to simulate that behavior here by not setting the property in that case.
                        if (textPropertyValue.Length > 0)
                        {
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
        /// Creates a text ATOM value.
        /// </summary>
        /// <param name="textValue">The text value to use.</param>
        /// <param name="contentKind">The content kind of the value.</param>
        /// <param name="version">The protocol version to use.</param>
        /// <returns>The Atom text value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "contentKind", Justification = "Will be used once the ATOM text construct is available in metadata.")]
        private static AtomTextConstruct CreateAtomTextConstruct(string textValue, SyndicationTextContentKind contentKind, ODataVersion version)
        {
            // If V3 serialization is enabled write null values by adding the m:null="true" attribute.
            if (textValue == null && version >= ODataVersion.V3)
            {
                // Add the m:null attribute on the value
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

            return new AtomTextConstruct() 
            { 
                Kind = kind, 
                Text = textValue 
            };
        }

        /// <summary>
        /// Creates a list of ATOM person metadata with a single item.
        /// </summary>
        /// <param name="person">The person metadata to include.</param>
        /// <returns>The list of ATOM person metadata.</returns>
        private static List<AtomPersonMetadata> CreateSinglePersonList(AtomPersonMetadata person)
        {
            List<AtomPersonMetadata> personList = new List<AtomPersonMetadata>();
            personList.Add(person);
            return personList;
        }

        /// <summary>
        /// Given an object returns the corresponding DateTimeOffset value through conversions.
        /// </summary>
        /// <param name="propertyValue">Object containing property value.</param>
        /// <param name="targetProperty">The target syndication property for the mapping (used for exception messages).</param>
        /// <param name="version">The version of the OData protocol to use.</param>
        /// <returns>DateTimeOffset after conversion.</returns>
        private static DateTimeOffset CreateDateTimeValue(object propertyValue, SyndicationItemProperty targetProperty, ODataVersion version)
        {
            if (propertyValue == null)
            {
                // In V3 we are to use m:null extension attribute to mark null values. Due to that we need to fail
                // because syndication API doesn't allow us to put extension attributes on publish or updated ATOM elements 
                // (even though the spec allows it) and the product used syndication APIs. In future version of the protocol we might be able
                // to lift the restriction, but for now we have to keep this for backward compat.
                if (version >= ODataVersion.V3)
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
            else if (propertyValue is DateTime)
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
            else
            {
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
        }
    }
}
