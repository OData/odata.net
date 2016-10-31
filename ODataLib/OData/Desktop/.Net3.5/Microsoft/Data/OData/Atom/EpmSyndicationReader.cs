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
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
        /// <param name="inputContext">The input context currently in use.</param>
        private EpmSyndicationReader(
            IODataAtomReaderEntryState entryState,
            ODataAtomInputContext inputContext)
            : base(entryState, inputContext)
        {
        }

        /// <summary>
        /// Reads the syndication EPM for an entry.
        /// </summary>
        /// <param name="entryState">The reader entry state for the entry to which the EPM is applied.</param>
        /// <param name="inputContext">The input context currently in use.</param>
        internal static void ReadEntryEpm(
            IODataAtomReaderEntryState entryState,
             ODataAtomInputContext inputContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryState != null, "entryState != null");
            Debug.Assert(entryState.CachedEpm != null, "To read entry EPM, the entity type must have an EPM annotation.");
            Debug.Assert(inputContext != null, "inputContext != null");

            EpmSyndicationReader epmSyndicationReader = new EpmSyndicationReader(entryState, inputContext);
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
                if (targetSegment.HasContent)
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
                    // TODO: need some way to handle m:null if we decide to support it.
                    if (this.MessageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
                    {
                        if (entryMetadata.UpdatedString != null)
                        {
                            this.SetEntryEpmValue(targetSegment.EpmInfo, entryMetadata.UpdatedString);
                        }
                    }
                    else if (entryMetadata.Updated.HasValue)
                    {
                        this.SetEntryEpmValue(targetSegment.EpmInfo, XmlConvert.ToString(entryMetadata.Updated.Value));
                    }
                    
                    break;
                case SyndicationItemProperty.Published:
                    // TODO: need some way to handle m:null if we decide to support it.
                    if (this.MessageReaderSettings.ReaderBehavior.FormatBehaviorKind == ODataBehaviorKind.WcfDataServicesClient)
                    {
                        if (entryMetadata.PublishedString != null)
                        {
                            this.SetEntryEpmValue(targetSegment.EpmInfo, entryMetadata.PublishedString);
                        }
                    }
                    else if (entryMetadata.Published.HasValue)
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
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadEntryEpm_ContentTarget));
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
            switch (targetSegment.SegmentName)
            {
                case AtomConstants.AtomAuthorElementName:
                    // If a singleton property (non-collection) is mapped to author and there are multiple authors
                    // EPM uses the first author.
                    AtomPersonMetadata authorMetadata = entryMetadata.Authors.FirstOrDefault();
                    if (authorMetadata != null)
                    {
                        this.ReadPersonEpm(
                            this.EntryState.Entry.Properties.ToReadOnlyEnumerable("Properties"),
                            this.EntryState.EntityType.ToTypeReference(),
                            targetSegment,
                            authorMetadata);
                    }

                    break;
                case AtomConstants.AtomContributorElementName:
                    // If a singleton property (non-collection) is mapped to contributor and there are multiple contributors
                    // EPM uses the first contributor.
                    AtomPersonMetadata contributorMetadata = entryMetadata.Contributors.FirstOrDefault();
                    if (contributorMetadata != null)
                    {
                        this.ReadPersonEpm(
                            this.EntryState.Entry.Properties.ToReadOnlyEnumerable("Properties"),
                            this.EntryState.EntityType.ToTypeReference(),
                            targetSegment,
                            contributorMetadata);
                    }

                    break;
                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadParentSegment_TargetSegmentName));
            }
        }

        /// <summary>
        /// Reads EPM values from a person construct (author or contributor).
        /// </summary>
        /// <param name="targetList">The target list, this can be either a list of properties (on entry or complex value),
        /// or a list of items (for a collection of primitive types).</param>
        /// <param name="targetTypeReference">The type of the value on which to set the property (can be entity, complex or primitive).</param>
        /// <param name="targetSegment">The target segment which points to either author or contributor element.</param>
        /// <param name="personMetadata">The person ATOM metadata to read from.</param>
        private void ReadPersonEpm(ReadOnlyEnumerable<ODataProperty> targetList, IEdmTypeReference targetTypeReference, EpmTargetPathSegment targetSegment, AtomPersonMetadata personMetadata)
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
                            this.SetEpmValue(targetList, targetTypeReference, subSegment.EpmInfo, personEmail);
                        }

                        break;

                    case SyndicationItemProperty.AuthorUri:
                    case SyndicationItemProperty.ContributorUri:
                        string personUri = personMetadata.UriFromEpm;
                        if (personUri != null)
                        {
                            this.SetEpmValue(targetList, targetTypeReference, subSegment.EpmInfo, personUri);
                        }

                        break;

                    default:
                        // Unhandled EpmTargetPathSegment.SegmentName.
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmSyndicationReader_ReadPersonEpm));
                }
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
                if (textConstruct.Text != null)
                {
                    this.SetEntryEpmValue(targetSegment.EpmInfo, textConstruct.Text);
                }
            }
        }
    }
}
