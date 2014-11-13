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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
        /// <param name="atomOutputContext">The output context currently in use.</param>
        private EpmSyndicationWriter(EpmTargetTree epmTargetTree, ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
            this.epmTargetTree = epmTargetTree;
            this.entryMetadata = new AtomEntryMetadata();
        }

        /// <summary>
        /// Writes the syndication part of EPM for an entry into ATOM metadata OM.
        /// </summary>
        /// <param name="epmTargetTree">The EPM target tree to use.</param>
        /// <param name="epmValueCache">The entry properties value cache to use to access the properties.</param>
        /// <param name="type">The type of the entry.</param>
        /// <param name="atomOutputContext">The output context currently in use.</param>
        /// <returns>The ATOM metadata OM with the EPM values populated.</returns>
        internal static AtomEntryMetadata WriteEntryEpm(
            EpmTargetTree epmTargetTree,
            EntryPropertiesValueCache epmValueCache,
            IEdmEntityTypeReference type,
            ODataAtomOutputContext atomOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(epmTargetTree != null, "epmTargetTree != null");
            Debug.Assert(epmValueCache != null, "epmValueCache != null");
            Debug.Assert(type != null, "For any EPM to exist the metadata must be available.");

            EpmSyndicationWriter epmWriter = new EpmSyndicationWriter(epmTargetTree, atomOutputContext);
            return epmWriter.WriteEntryEpm(epmValueCache, type);
        }

        /// <summary>
        /// Creates a text ATOM value.
        /// </summary>
        /// <param name="textValue">The text value to use.</param>
        /// <param name="contentKind">The content kind of the value.</param>
        /// <returns>The Atom text value.</returns>
        private static AtomTextConstruct CreateAtomTextConstruct(string textValue, SyndicationTextContentKind contentKind)
        {
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
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_CreateAtomTextConstruct));
            }

            return new AtomTextConstruct
            {
                Kind = kind,
                Text = textValue,
            };
        }

        /// <summary>
        /// Given an object returns the corresponding DateTimeOffset value through conversions.
        /// </summary>
        /// <param name="propertyValue">Object containing property value.</param>
        /// <param name="targetProperty">The target syndication property for the mapping (used for exception messages).</param>
        /// <param name="writerBehavior">The current settings to control the behavior of the writer.</param>
        /// <returns>DateTimeOffset after conversion.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writerBehavior", Justification = "Used in debug assert.")]
        private static DateTimeOffset CreateDateTimeValue(object propertyValue, SyndicationItemProperty targetProperty, ODataWriterBehavior writerBehavior)
        {
            Debug.Assert(
                writerBehavior.FormatBehaviorKind != ODataBehaviorKind.WcfDataServicesClient,
                "CreateDateTimeValue should not be used in WCF DS client mode.");

            if (propertyValue == null)
            {
                return DateTimeOffset.Now;
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
                        throw new ODataException(ODataErrorStrings.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(targetProperty.ToString()));
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

                throw new ODataException(ODataErrorStrings.EpmSyndicationWriter_DateTimePropertyCanNotBeConverted(targetProperty.ToString()));
            }
        }

        /// <summary>
        /// Given an object returns the corresponding string representation of the value.
        /// </summary>
        /// <param name="propertyValue">Object containing property value.</param>
        /// <param name="writerBehavior">The current settings to control the behavior of the writer.</param>
        /// <returns>String representation of the property value.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "writerBehavior", Justification = "Used in debug assert.")]
        private static string CreateDateTimeStringValue(object propertyValue, ODataWriterBehavior writerBehavior)
        {
            Debug.Assert(
                writerBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient,
                "CreateDateTimeStringValue should only be used in WCF DS client mode.");

            if (propertyValue == null)
            {
                propertyValue = DateTimeOffset.Now;
            }

            if (propertyValue is DateTime)
            {
                // DateTimeOffset takes care of DateTimes of Unspecified kind so we won't end up 
                // with datetime without timezone info mapped to atom:updated or atom:published element.
                propertyValue = new DateTimeOffset((DateTime)propertyValue);
            }

            // For DateTimeOffset values we need to use the ATOM format when translating them
            // to strings since the value will be used in ATOM metadata.
            if (propertyValue is DateTimeOffset)
            {
                return ODataAtomConvert.ToAtomString((DateTimeOffset)propertyValue);
            }

            return EpmWriterUtils.GetPropertyValueAsText(propertyValue);
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
                if (targetSegment.HasContent)
                {
                    EntityPropertyMappingInfo epmInfo = targetSegment.EpmInfo;
                    Debug.Assert(
                        epmInfo != null && epmInfo.Attribute != null,
                        "If the segment has content it must have EpmInfo which in turn must have the EPM attribute");

                    object propertyValue = this.ReadEntryPropertyValue(
                        epmInfo,
                        epmValueCache,
                        entityType);
                    string textPropertyValue = EpmWriterUtils.GetPropertyValueAsText(propertyValue);

                    switch (epmInfo.Attribute.TargetSyndicationItem)
                    {
                        case SyndicationItemProperty.Updated:
                            if (this.WriterBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
                            {
                                this.entryMetadata.UpdatedString = EpmSyndicationWriter.CreateDateTimeStringValue(propertyValue, this.WriterBehavior);
                            }
                            else
                            {
                                this.entryMetadata.Updated = EpmSyndicationWriter.CreateDateTimeValue(propertyValue, SyndicationItemProperty.Updated, this.WriterBehavior);
                            }

                            break;
                        case SyndicationItemProperty.Published:
                            if (this.WriterBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
                            {
                                this.entryMetadata.PublishedString = EpmSyndicationWriter.CreateDateTimeStringValue(propertyValue, this.WriterBehavior);
                            }
                            else
                            {
                                this.entryMetadata.Published = EpmSyndicationWriter.CreateDateTimeValue(propertyValue, SyndicationItemProperty.Published, this.WriterBehavior);
                            }

                            break;
                        case SyndicationItemProperty.Rights:
                            this.entryMetadata.Rights = EpmSyndicationWriter.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        case SyndicationItemProperty.Summary:
                            this.entryMetadata.Summary = EpmSyndicationWriter.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        case SyndicationItemProperty.Title:
                            this.entryMetadata.Title = EpmSyndicationWriter.CreateAtomTextConstruct(textPropertyValue, epmInfo.Attribute.TargetTextContentKind);
                            break;
                        default:
                            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteEntryEpm_ContentTarget));
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
        /// <param name="typeReference">The type of the entry or collection item.</param>
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
            else
            {
                // Unhandled EpmTargetPathSegment.SegmentName.
                throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WriteParentSegment_TargetSegmentName));
            }
        }

        /// <summary>
        /// Writes EPM value to a person construct (author or contributor).
        /// </summary>
        /// <param name="targetSegment">The target segment which points to either author or contributor element.</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or collection item.</param>
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
                    // In V2 we write the mapped properties always in-content when the value is null.
                    continue;
                }

                // Initialize the person element only if we actually need to write something to it.
                Debug.Assert(subSegment.EpmInfo != null && subSegment.EpmInfo.Attribute != null, "The author subsegment must have EPM info and EPM attribute.");
                switch (subSegment.EpmInfo.Attribute.TargetSyndicationItem)
                {
                    case SyndicationItemProperty.AuthorName:
                    case SyndicationItemProperty.ContributorName:
                        if (textPropertyValue != null)
                        {
                            if (personMetadata == null)
                            {
                                personMetadata = new AtomPersonMetadata();
                            }

                            personMetadata.Name = textPropertyValue;
                        }

                        break;
                    case SyndicationItemProperty.AuthorEmail:
                    case SyndicationItemProperty.ContributorEmail:
                        if (textPropertyValue != null && textPropertyValue.Length > 0)
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
                        if (textPropertyValue != null && textPropertyValue.Length > 0)
                        {
                            if (personMetadata == null)
                            {
                                personMetadata = new AtomPersonMetadata();
                            }

                            personMetadata.UriFromEpm = textPropertyValue;
                        }

                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationWriter_WritePersonEpm));
                }
            }

            return personMetadata;
        }

        /// <summary>
        /// Given a target segment the method returns the text value of the property mapped to that segment to be used in EPM.
        /// </summary>
        /// <param name="targetSegment">The target segment to read the value for.</param>
        /// <param name="epmValueCache">EPM value cache to use to get property values, or a primitive value</param>
        /// <param name="typeReference">The type of the entry or collection item.</param>
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

            if (entryPropertiesValueCache != null)
            {
                propertyValue = this.ReadEntryPropertyValue(
                    targetSegment.EpmInfo,
                    entryPropertiesValueCache,
                    typeReference.AsEntity());
            }
            else
            { 
                propertyValue = epmValueCache;
                ValidationUtils.ValidateIsExpectedPrimitiveType(propertyValue, typeReference);
            }

            return EpmWriterUtils.GetPropertyValueAsText(propertyValue);
        }
    }
}
