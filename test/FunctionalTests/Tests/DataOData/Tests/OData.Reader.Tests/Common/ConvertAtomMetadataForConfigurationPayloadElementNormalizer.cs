//---------------------------------------------------------------------
// <copyright file="ConvertAtomMetadataForConfigurationPayloadElementNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    #endregion Namespaces

    /// <summary>
    /// Converts ATOM metadata from the given payload element and its descendants into
    /// XmlTree annotations suitable for payloads when ATOM metadata reading is enabled.
    /// This happens for stream properties and association links that are read as atom:link elements
    /// but otherwise ignored in requests.
    /// </summary>
    public class ConvertAtomMetadataForConfigurationPayloadElementNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// The test configuration for the payload.
        /// </summary>
        private ReaderTestConfiguration readerTestConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="readerTestConfiguration">The reader test configuration for the payload.</param>
        private ConvertAtomMetadataForConfigurationPayloadElementNormalizer(ReaderTestConfiguration readerTestConfiguration)
        {
            this.readerTestConfiguration = readerTestConfiguration;
        }

        /// <summary>
        /// Converts ATOM metadata from the given payload element and its descendants into
        /// XmlTree annotations suitable for payloads when ATOM metadata reading is enabled.
        /// This happens for stream properties and association links that are read as atom:link elements
        /// but otherwise ignored in some cases.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <param name="readerTestConfiguration">The reader test configuration to use.</param>
        /// <returns>The <paramref name="payloadElement"/> after it has been visited.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement, ReaderTestConfiguration readerTestConfiguration)
        {
            new ConvertAtomMetadataForConfigurationPayloadElementNormalizer(readerTestConfiguration).Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Converts the ATOM metadata annotation of stream properties and assocation links
        /// into XmlTree annotations and removes the stream properties/association links from the payload.
        /// </summary>
        /// <param name="payloadElement">The entity instance to visit.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            IEnumerable<PropertyInstance> properties = payloadElement.Properties;
            if (properties != null)
            {
                // First remove all the stream properties in request payloads
                if (this.readerTestConfiguration.IsRequest)
                {
                    List<NamedStreamInstance> streamProperties = properties.OfType<NamedStreamInstance>().ToList();
                    foreach (var streamProperty in streamProperties)
                    {
                        // Edit link
                        string editRelationValue = TestAtomConstants.ODataStreamPropertyEditMediaRelatedLinkRelationPrefix + streamProperty.Name;
                        XmlTreeAnnotation annotation = GetStreamPropertyLinkXmlTreeAnnotation(streamProperty, editRelationValue);
                        if (annotation != null)
                        {
                            payloadElement.AddAnnotation(annotation);
                        }

                        // Self link
                        string sourceRelationValue = TestAtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix + streamProperty.Name;
                        annotation = GetStreamPropertyLinkXmlTreeAnnotation(streamProperty, sourceRelationValue);
                        if (annotation != null)
                        {
                            payloadElement.AddAnnotation(annotation);
                        }

                        payloadElement.Remove(streamProperty);
                    }
                }

                // Then also convert the association links of navigation properties and 
                // remove all navigation properties that do not have a URI - for request
                if (this.readerTestConfiguration.IsRequest)
                {
                    List<NavigationPropertyInstance> navigationProperties = properties.OfType<NavigationPropertyInstance>().ToList();
                    foreach (var navProperty in navigationProperties)
                    {
                        XmlTreeAnnotation annotation = GetAssociationLinkXmlTreeAnnotation(navProperty.AssociationLink);
                        navProperty.AssociationLink = null;

                        if (navProperty.Value == null)
                        {
                            payloadElement.Remove(navProperty);
                        }

                        if (annotation != null)
                        {
                            payloadElement.AddAnnotation(annotation);
                        }
                    }
                }
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Computes the XmlTreeAnnotation for a stream property from the <see cref="NamedStreamAtomLinkMetadataAnnotation"/>.
        /// </summary>
        /// <param name="streampProperty">The stream property to compute the Xml annotation for.</param>
        /// <param name="relation">The relation of the link we are converting</param>
        /// <returns>The <see cref="XmlTreeAnnotation"/> for the link with the specified <paramref name="relation"/>.</returns>
        private XmlTreeAnnotation GetStreamPropertyLinkXmlTreeAnnotation(NamedStreamInstance streampProperty, string relation)
        {
            // Look it up again since it was created above.
            NamedStreamAtomLinkMetadataAnnotation linkAnnotation = streampProperty.Annotations.OfType<NamedStreamAtomLinkMetadataAnnotation>().SingleOrDefault(a => a.Relation == relation);
            if (linkAnnotation == null)
            {
                return null;
            }

            List<XmlTreeAnnotation> attributes = new List<XmlTreeAnnotation>();

            if (linkAnnotation.Href != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkHrefAttributeName, linkAnnotation.Href));
            }

            if (linkAnnotation.HrefLang != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkHrefLangAttributeName, linkAnnotation.HrefLang));
            }

            if (linkAnnotation.Length != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkLengthAttributeName, linkAnnotation.Length));
            }

            if (linkAnnotation.Relation != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkRelationAttributeName, linkAnnotation.Relation));
            }

            if (linkAnnotation.Title != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkTitleAttributeName, linkAnnotation.Title));
            }

            if (linkAnnotation.Type != null)
            {
                attributes.Add(XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkTypeAttributeName, linkAnnotation.Type));
            }

            return XmlTreeAnnotation.Atom(TestAtomConstants.AtomLinkElementName, null, attributes.ToArray());
        }

        /// <summary>
        /// Computes the XmlTreeAnnotation for an association link.
        /// </summary>
        /// <param name="associationLink">The association link to compute the stream property for.</param>
        /// <returns>The <see cref="XmlTreeAnnotation"/> for the link specified in <paramref name="associationLink"/>.</returns>
        private XmlTreeAnnotation GetAssociationLinkXmlTreeAnnotation(DeferredLink associationLink)
        {
            if (associationLink == null)
            {
                return null;
            }

            // Add all the attributes that are already stored on the association link as annotations
            List<XmlTreeAnnotation> attributes = new List<XmlTreeAnnotation>();
            attributes.AddRange(associationLink.Annotations.OfType<XmlTreeAnnotation>().Where(a => a.IsAttribute));
            return XmlTreeAnnotation.Atom(TestAtomConstants.AtomLinkElementName, null, attributes.ToArray());
        }

    }
}