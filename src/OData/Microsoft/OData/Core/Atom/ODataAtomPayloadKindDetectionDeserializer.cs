//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for detecting the payload kind of an ATOM payload.
    /// </summary>
    internal sealed class ODataAtomPayloadKindDetectionDeserializer : ODataAtomPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomPayloadKindDetectionDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
        }

        /// <summary>
        /// Detects the payload kind(s) of the payload.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero or more payload kinds depending on what payload kinds were detected.</returns>
        /// <remarks>This method decides the payload kind based on the fully-qualified element name of the top-level Xml element 
        /// in the payload for entry, feed, entity reference link, error and service document payload kinds. It performs more checks
        /// for properties and collection payloads as follows:
        /// * If an m:type attribute is found => property
        /// * If an m:null attribute is found => property
        /// Otherwise the shape of the payload decides:
        /// * If we only find m:element child nodes => collection or property
        /// * If we find no child nodes => primitive property
        /// * If we find anything else => complex property
        /// </remarks>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            this.XmlReader.DisableInStreamErrorDetection = true;
            try
            {
                if (this.XmlReader.TryReadToNextElement())
                {
                    if (string.CompareOrdinal(AtomConstants.AtomNamespace, this.XmlReader.NamespaceURI) == 0)
                    {
                        // ATOM namespace for <entry> and <feed>
                        if (string.CompareOrdinal(AtomConstants.AtomEntryElementName, this.XmlReader.LocalName) == 0)
                        {
                            return new ODataPayloadKind[] { ODataPayloadKind.Entry };
                        }

                        if (this.ReadingResponse && string.CompareOrdinal(AtomConstants.AtomFeedElementName, this.XmlReader.LocalName) == 0)
                        {
                            if (this.XmlReader.XmlBaseUri != null
                                && this.XmlReader.XmlBaseUri.AbsoluteUri.Contains(ODataConstants.EntityReferenceSegmentName))
                            {
                                return new ODataPayloadKind[] { ODataPayloadKind.EntityReferenceLinks };
                            }
                            else
                            {
                                return new ODataPayloadKind[] { ODataPayloadKind.Feed };
                            }
                        }
                    }
                    else if (string.CompareOrdinal(AtomConstants.ODataNamespace, this.XmlReader.NamespaceURI) == 0)
                    {
                        // Will figure out whether we need to remove this part of logic, since in the top level, the format has been changed to <m:value
                        // OData namespace for entity reference links, properties, collections
                        // NOTE: everything in the OData namespace is considered a property (or collection). Some of them
                        //       may have a potential other payload kind as well.
                        //
                        // Only check for property or collection if these payload kinds are even included in the possible payload kinds
                        IEnumerable<ODataPayloadKind> possiblePayloadKinds = detectionInfo.PossiblePayloadKinds;
                        IEnumerable<ODataPayloadKind> payloadKinds =
                            possiblePayloadKinds.Any(k => k == ODataPayloadKind.Property || k == ODataPayloadKind.Collection)
                            ? this.DetectPropertyOrCollectionPayloadKind()
                            : Enumerable.Empty<ODataPayloadKind>();

                        return payloadKinds;
                    }
                    else if (string.CompareOrdinal(AtomConstants.ODataMetadataNamespace, this.XmlReader.NamespaceURI) == 0)
                    {
                        // OData metadata namespace for collections
                        // Only check for property or collection if these payload kinds are even included in the possible payload kinds
                        if (string.CompareOrdinal(AtomConstants.ODataValueElementName, this.XmlReader.LocalName) == 0)
                        {
                            IEnumerable<ODataPayloadKind> possiblePayloadKinds = detectionInfo.PossiblePayloadKinds;
                            IEnumerable<ODataPayloadKind> payloadKinds =
                                possiblePayloadKinds.Any(k => k == ODataPayloadKind.Property || k == ODataPayloadKind.Collection)
                                ? this.DetectPropertyOrCollectionPayloadKind()
                                : Enumerable.Empty<ODataPayloadKind>();
                            return payloadKinds;
                        }

                        // OData metadata namespace for errors and <m:ref>
                        if (this.ReadingResponse && string.CompareOrdinal(AtomConstants.ODataErrorElementName, this.XmlReader.LocalName) == 0)
                        {
                            return new ODataPayloadKind[] { ODataPayloadKind.Error };
                        }

                        if (string.CompareOrdinal(AtomConstants.ODataRefElementName, this.XmlReader.LocalName) == 0)
                        {
                            return new ODataPayloadKind[] { ODataPayloadKind.EntityReferenceLink };
                        }
                    }
                    else if (string.CompareOrdinal(AtomConstants.AtomPublishingNamespace, this.XmlReader.NamespaceURI) == 0)
                    {
                        // AtomPub namespace for service doc
                        if (this.ReadingResponse && string.CompareOrdinal(AtomConstants.AtomPublishingServiceElementName, this.XmlReader.LocalName) == 0)
                        {
                            return new ODataPayloadKind[] { ODataPayloadKind.ServiceDocument };
                        }
                    }

                    //// If none of the above namespaces is found, we don't understand the payload in the ATOM format.
                }
            }
            catch (XmlException)
            {
                // If we are not able to read the payload as XML
                // return no detected payload kind below.
            }
            finally
            {
                this.XmlReader.DisableInStreamErrorDetection = false;
            }

            return Enumerable.Empty<ODataPayloadKind>();
        }

        /// <summary>
        /// Detects whether the current element represents a property payload, a collection payload or neither.
        /// </summary>
        /// <returns>An enumerable of zero, one or two payload kinds depending on whether a property, collection, both or neither were detected.</returns>
        private IEnumerable<ODataPayloadKind> DetectPropertyOrCollectionPayloadKind()
        {
            string typeName;
            bool isNull;
            this.ReadNonEntityValueAttributes(out typeName, out isNull);

            // If the payloads has either m:null or m:type attributes, it must be a top-level property payload (since collections don't allow either).
            if (isNull || typeName != null)
            {
                return new ODataPayloadKind[] { ODataPayloadKind.Property };
            }

            // Determine the type from the payload shape.
            EdmTypeKind typeKind = this.GetNonEntityValueKind();

            // If we find a collection type we report property or collection; otherwise it has to be a property
            return typeKind == EdmTypeKind.Collection && this.ReadingResponse
                ? new ODataPayloadKind[] { ODataPayloadKind.Property, ODataPayloadKind.Collection }
                : new ODataPayloadKind[] { ODataPayloadKind.Property };
        }
    }
}
