//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementAtomMetadataNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Atom
{
    #region Namespaces
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;

    #endregion Namespaces

    /// <summary>
    /// Normalizes atom metadata values between multiple properties/annotations where it may be defined.
    /// </summary>
    /// <remarks>
    /// This visitor modifies the payload element being visited - use deep clone if the original state needs
    /// to be retained. 
    /// An example of when this visitor should be used is when the EditLink property of an entry has been
    /// set, without adding the corresponding XML Tree annotation.
    /// </remarks>
    public class ODataPayloadElementAtomMetadataNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>Test configuration to apply during normalization.</summary>
        private readonly TestConfiguration testConfiguration;

        /// <summary>
        /// Generates a function which normalizes the given payload element and all of its descendants.
        /// </summary>
        /// <param name="testConfiguration">Test configuration to apply during normalization.</param>
        /// <returns>A function which normalizes the ATOM metadata of a payload element and returns the modified payload element.</returns>
        public static Func<ODataPayloadElement, ODataPayloadElement> GenerateNormalizer(TestConfiguration testConfiguration)
        {
            return new Func<ODataPayloadElement, ODataPayloadElement>(payloadElement =>
                {
                    new ODataPayloadElementAtomMetadataNormalizer(testConfiguration).Recurse(payloadElement);
                    return payloadElement;
                });
        }

        /// <summary>
        /// Creates a new ATOM metadata normalizer.
        /// </summary>
        /// <param name="testConfiguration">Test configuration to apply during normalization.</param>
        public ODataPayloadElementAtomMetadataNormalizer(TestConfiguration testConfiguration)
        {
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// Normalizes entry specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            // Self Link
            NormalizeLinkValue(
                payloadElement,
                TestAtomConstants.AtomSelfRelationAttributeValue,
                TestAtomConstants.AtomLinkHrefAttributeName,
                (entity) => entity.Annotations.OfType<SelfLinkAnnotation>().Select(a => a.Value).SingleOrDefault(),
                (entity, value) => entity.WithSelfLink(value));

            // Edit Link
            NormalizeLinkValue(
                payloadElement,
                TestAtomConstants.AtomEditRelationAttributeValue,
                TestAtomConstants.AtomLinkHrefAttributeName,
                (entity) => entity.EditLink,
                (entity, value) => entity.EditLink = value);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes feed specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            // Next Link
            NormalizeLinkValue(
                payloadElement,
                TestAtomConstants.AtomNextRelationAttributeValue,
                TestAtomConstants.AtomLinkHrefAttributeName,
                (entity) => entity.NextLink,
                (entity, value) => 
                {
                    if (this.testConfiguration.IsRequest)
                    {
                        // Next links aren't read in request or V1 payloads.
                        return;
                    }
                    else
                    {
                        entity.NextLink = value;
                    }
                });

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes link collection specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            // Next Link
            NormalizeLinkValue(
                payloadElement,
                TestAtomConstants.AtomNextRelationAttributeValue,
                TestAtomConstants.AtomLinkHrefAttributeName,
                (e) => e.NextLink,
                (e, v) => e.NextLink = v);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes named stream specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            // Edit Link
            string editRelationValue = TestAtomConstants.ODataStreamPropertyEditMediaRelatedLinkRelationPrefix + payloadElement.Name;
            NamedStreamAtomLinkMetadataAnnotation editLinkAnnotation = payloadElement.Annotations.OfType<NamedStreamAtomLinkMetadataAnnotation>().SingleOrDefault(a => a.Relation == editRelationValue);
            NormalizeNamedStreamLinkValue(
                payloadElement,
                editLinkAnnotation,
                (annotation) => annotation.Href,
                (annotation, value) => annotation.Href = value,
                (namedStream) => namedStream.EditLink,
                (namedStream, value) => namedStream.EditLink = value);

            NormalizeNamedStreamLinkValue(
                payloadElement,
                editLinkAnnotation,
                (annotation) => annotation.Type,
                (annotation, value) => annotation.Type = value,
                (namedStream) => namedStream.EditLinkContentType,
                (namedStream, value) => namedStream.EditLinkContentType = value);

            // Source Link
            string sourceRelationValue = TestAtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix + payloadElement.Name;
            NamedStreamAtomLinkMetadataAnnotation sourceLinkAnnotation = payloadElement.Annotations.OfType<NamedStreamAtomLinkMetadataAnnotation>().SingleOrDefault(a => a.Relation == sourceRelationValue);
            NormalizeNamedStreamLinkValue(
                payloadElement,
                sourceLinkAnnotation,
                (annotation) => annotation.Href,
                (annotation, value) => annotation.Href = value,
                (namedStream) => namedStream.SourceLink,
                (namedStream, value) => namedStream.SourceLink = value);

            NormalizeNamedStreamLinkValue(
                payloadElement,
                sourceLinkAnnotation,
                (annotation) => annotation.Type,
                (annotation, value) => annotation.Type = value,
                (namedStream) => namedStream.SourceLinkContentType,
                (namedStream, value) => namedStream.SourceLinkContentType = value);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes navigation property specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            DeferredLink deferredLink = payloadElement.Value as DeferredLink;
            if (deferredLink != null)
            {
                // If there is a type annotation specified as a XmlTreeAnnotion, copy its value over to a ContentTypeAnnotation as well.
                XmlTreeAnnotation typeAnnotation = deferredLink.Annotations.OfType<XmlTreeAnnotation>().SingleOrDefault(a => a.LocalName == TestAtomConstants.AtomLinkTypeAttributeName);
                if (typeAnnotation != null)
                {
                    deferredLink.WithContentType(typeAnnotation.PropertyValue);
                }
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes resoure collection specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(ResourceCollectionInstance payloadElement)
        {
            NormalizeTitleValue(
                payloadElement,
                payloadElement.Title,
                (value) => payloadElement.Title = value);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Normalizes workspace specific atom metadata.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(WorkspaceInstance payloadElement)
        {
            NormalizeTitleValue(
                payloadElement,
                payloadElement.Title,
                (value) => payloadElement.Title = value);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Retrieves the attribute annotation that is an immediate child of the tree annotation.
        /// </summary>
        /// <param name="annotation">The tree annotation to search.</param>
        /// <param name="attributeName">The name of the attribute to retrieve.</param>
        /// <returns>The specified attribute annotation, or null if no matching attributes found.</returns>
        private static XmlTreeAnnotation GetAttributeAnnotation(XmlTreeAnnotation annotation, string attributeName)
        {
            return annotation.Children.SingleOrDefault(a => a.IsAttribute && a.LocalName.Equals(attributeName));
        }

        /// <summary>
        /// Searches the tree annotation for an attribute of specific name and value.
        /// </summary>
        /// <param name="annotation">The tree annotation to search.</param>
        /// <param name="attributeName">The name of attribute to find.</param>
        /// <param name="attributeValue">The value of attribute to find.</param>
        /// <returns>true if the specified attribute name/value was found, false otherwise.</returns>
        private static bool HasAttribute(XmlTreeAnnotation annotation, string attributeName, string attributeValue)
        {
            return annotation.Children.Any(a => a.IsAttribute && a.LocalName == attributeName && a.PropertyValue == attributeValue);
        }

        /// <summary>
        /// Normalizes an Atom link property value between an XmlTreeAnnotation and another property.
        /// </summary>
        /// <typeparam name="T">The type of payload element.</typeparam>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <param name="relationValue">The relation value of the link to normalize.</param>
        /// <param name="attributeName">The name of the link attribute to normalize.</param>
        /// <param name="getOtherLinkValue">Function for retrieving the other value for the link property.</param>
        /// <param name="setOtherLinkValue">Delegate for setting the other value for the link property.</param>
        private void NormalizeLinkValue<T>(T payloadElement, string relationValue, string attributeName, Func<T, string> getOtherLinkValue, Action<T, string> setOtherLinkValue) where T : ODataPayloadElement
        {
            string otherLinkValue = getOtherLinkValue(payloadElement);
            XmlTreeAnnotation linkXmlTreeAnnotation = payloadElement.Annotations
                .OfType<XmlTreeAnnotation>()
                .SingleOrDefault(a => a.LocalName.Equals(TestAtomConstants.AtomLinkElementName) && HasAttribute(a, TestAtomConstants.AtomLinkRelationAttributeName, relationValue));

            if (otherLinkValue != null)
            {
                if (linkXmlTreeAnnotation != null)
                {
                    var linkAttribute = GetAttributeAnnotation(linkXmlTreeAnnotation, attributeName);
                    if (linkAttribute == null)
                    {
                        linkXmlTreeAnnotation.Children.Add(XmlTreeAnnotation.AtomAttribute(attributeName, otherLinkValue));
                    }
                    else if (linkAttribute.PropertyValue != otherLinkValue)
                    {
                        ExceptionUtilities.Assert(otherLinkValue.Equals(linkAttribute.PropertyValue), attributeName + " value for link '" + relationValue + "' does not match value on Xml Tree");
                    }
                }
                else
                {
                    // Add an xml tree annotation to match the value from the other source.
                    payloadElement.Add(
                        XmlTreeAnnotation.Atom(
                            TestAtomConstants.AtomLinkElementName,
                            null,
                            XmlTreeAnnotation.AtomAttribute(TestAtomConstants.AtomLinkRelationAttributeName, relationValue),
                            XmlTreeAnnotation.AtomAttribute(attributeName, otherLinkValue)));
                }
            }
            else if (linkXmlTreeAnnotation != null)
            {
                // Set the other source to match the atom:link href value of the annotation
                var linkAttribute = GetAttributeAnnotation(linkXmlTreeAnnotation, attributeName);
                if (linkAttribute != null)
                {
                    setOtherLinkValue(payloadElement, linkAttribute.PropertyValue);
                }
            }
        }

        /// <summary>
        /// Normalizes an named stream's Atom link property value between a NamedStreamAtomLinkMetadataAnnotation and another property.
        /// </summary>
        /// <param name="namedStream">The named stream to normalize.</param>
        /// <param name="annotation">The annotation to use for normalizing.</param>
        /// <param name="getAnnotationValue">Function for retrieving the annotation value for the link property.</param>
        /// <param name="setAnnotationValue">Delegate for setting the annotation value for the link property.</param>
        /// <param name="getPropertyValue">Function for retrieving the property value for the link property.</param>
        /// <param name="setPropertyValue">Delegate for setting the property value for the link property.</param>
        private void NormalizeNamedStreamLinkValue(
            NamedStreamInstance namedStream,
            NamedStreamAtomLinkMetadataAnnotation annotation,
            Func<NamedStreamAtomLinkMetadataAnnotation, string> getAnnotationValue,
            Action<NamedStreamAtomLinkMetadataAnnotation, string> setAnnotationValue,
            Func<NamedStreamInstance, string> getPropertyValue,
            Action<NamedStreamInstance, string> setPropertyValue)
        {
            string propertyValue = getPropertyValue(namedStream);

            if (propertyValue != null)
            {
                if (annotation != null)
                {
                    string annotationValue = getAnnotationValue(annotation);
                    if (annotationValue == null)
                    {
                        setAnnotationValue(annotation, propertyValue);
                    }
                    else
                    {
                        ExceptionUtilities.Assert(propertyValue == annotationValue, "Link '" + annotation.Relation + "' has different values : Property=" + propertyValue + ", Annotation=" + annotationValue);
                    }
                }
                else
                {
                    var newAnnotation = new NamedStreamAtomLinkMetadataAnnotation { Relation = annotation.Relation };
                    setAnnotationValue(newAnnotation, propertyValue);
                    namedStream.Add(newAnnotation);
                }
            }
            else if (annotation != null)
            {
                setPropertyValue(namedStream, getAnnotationValue(annotation));
            }
        }

        /// <summary>
        /// Normalizes a workspace or resource collection's Atom title property value so that it matches the element's title annotation.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize</param>
        /// <param name="titlePropertyValue">The value of the payload element's title property</param>
        /// <param name="setPropertyValue">Delegate for setting the payload element's title property</param>
        private void NormalizeTitleValue(ODataPayloadElement payloadElement, string titlePropertyValue, Action<string> setPropertyValue)
        {
            XmlTreeAnnotation titleXmlTreeAnnotation = payloadElement.Annotations
                .OfType<XmlTreeAnnotation>()
                .SingleOrDefault(a => a.LocalName.Equals(TestAtomConstants.AtomTitleElementName));

            if (titlePropertyValue != null)
            {
                if (titleXmlTreeAnnotation != null)
                {
                    string titleAnnotationValue = titleXmlTreeAnnotation.PropertyValue;
                    if (titleAnnotationValue == null)
                    {
                        titleXmlTreeAnnotation.PropertyValue = titlePropertyValue;
                    }
                    else
                    {
                        ExceptionUtilities.Assert(titlePropertyValue == titleAnnotationValue, "Title in workspace or resource collection has different values : Property=" + titlePropertyValue + ", Annotation=" + titleAnnotationValue);
                    }
                }
                else
                {
                    payloadElement.AtomTitle(titlePropertyValue, TestAtomConstants.AtomTextConstructTextKind);
                }
            }
            else if (titleXmlTreeAnnotation != null)
            {
                setPropertyValue(titleXmlTreeAnnotation.PropertyValue);
            }
        }
    }
}
