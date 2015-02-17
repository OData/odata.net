//---------------------------------------------------------------------
// <copyright file="ODataWriterPayloadNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;

    /// <summary>
    /// Helps modify paylaods so that they are ODataLib compliant
    /// </summary>
    public class ODataWriterPayloadNormalizer : ODataPayloadElementReplacingVisitor, IODataPayloadElementVisitor<ODataPayloadElement>
    {
        /// <summary>
        /// Visits payloadElement to add default annotations and remove certain ones. 
        /// Then Visits its properties to replace properties with null values with NullProperty Instances.
        /// This is required in comparison because the Test Deserialiser converts properties that are null to NullProperty Instnaces.
        /// Also removes the annotations added by the Replacing Visitor and removes MultiValue properties if found
        /// </summary>
        /// <param name="payloadElement">the payload to potentially replace</param>
        /// <returns>The original or a copy if it has changed</returns>
        public override ODataPayloadElement Visit(EntitySetInstance payloadElement)
        {
            FixEpmAnnotationsUris(payloadElement.Annotations);
            EntitySetInstance entitySetInstance = (EntitySetInstance)base.Visit(payloadElement);

            // This has to happen last
            ReplaceAnnotationRemover replaceAnnotationRemover = new ReplaceAnnotationRemover();
            replaceAnnotationRemover.Visit(entitySetInstance);

            return entitySetInstance;
        }

        /// <summary>
        /// Visits payloadElement to add default annotations and remove certain ones. 
        /// Then Visits its properties to replace properties with null values with NullProperty Instances.
        /// This is required in comparison because the Test Deserialiser converts properties that are null to NullProperty Instnaces.
        /// Also removes the annotations added by the Replacing Visitor and removes MultiValue properties if found
        /// </summary>
        /// <param name="payloadElement">the payload to potentially replace</param>
        /// <returns>The original or a copy if it has changed</returns>
        public override ODataPayloadElement Visit(EntityInstance payloadElement)
        {
            FixMleAnnotation(payloadElement);
            FixEpmAnnotationsUris(payloadElement.Annotations);
            EntityInstance entry = (EntityInstance)base.Visit(payloadElement);
            
            // This has to happen last
            ReplaceAnnotationRemover replaceAnnotationRemover = new ReplaceAnnotationRemover();
            replaceAnnotationRemover.Visit(entry);

            return entry;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public override ODataPayloadElement Visit(PrimitiveProperty payloadElement)
        {
            if (payloadElement.Value == null || payloadElement.Value.ClrValue == null || payloadElement.Value.IsNull)
            {
                var nullProperty = new NullPropertyInstance()
                {
                    Name = payloadElement.Name,
                };
                foreach (ODataPayloadElementAnnotation annotation in payloadElement.Annotations)
                {
                    nullProperty.Add(annotation);
                }

                return nullProperty;
            }

            return payloadElement;
        }

        /// <summary>
        /// Entries might come with MLE annotation but might not satisfy all the conditions for MLEs.
        /// This method removes the MLE annotation if it is not an MLE or adds it if absent
        /// </summary>
        /// <param name="payloadElement">payloadElement to fix MLE annotation for</param>
        private void FixMleAnnotation(EntityInstance payloadElement)
        {
            // validate that if IsMediaLinkEntry returns true then indeed it is a valid MLE
            if (payloadElement.IsMediaLinkEntry())
            {
                var isMLEann = payloadElement.Annotations.OfType<IsMediaLinkEntryAnnotation>().Single();

                // MLE should have both of these, if not make it not an MLE
                if (payloadElement.StreamSourceLink == null || payloadElement.StreamContentType == null)
                {
                    if (isMLEann != null)
                    {
                        payloadElement.Annotations.Remove(isMLEann);
                    }

                    payloadElement.StreamSourceLink = null;
                    ExceptionUtilities.Assert(!payloadElement.IsMediaLinkEntry(), "should not be a media link entry");
                }
                else
                {
                    payloadElement.StreamSourceLink = FixUris(payloadElement.StreamSourceLink);
                    payloadElement.StreamEditLink = FixUris(payloadElement.StreamEditLink);

                    if (isMLEann == null)
                    {
                        payloadElement = payloadElement.AsMediaLinkEntry();
                    }
                }
            }
        }

        /// <summary>
        /// Fixes links in EpmAnnotations for payload
        /// </summary>
        /// <param name="payload">payload to fix Epm Annotations for</param>
        private void FixEpmAnnotationsUris(IEnumerable<ODataPayloadElementAnnotation> annotations)
        {
            var epmAnns = annotations;
            foreach (XmlTreeAnnotation epmAnn in annotations.OfType<XmlTreeAnnotation>())
            {
                this.FixEpmAnnotationsUris(epmAnn.Children.Cast<ODataPayloadElementAnnotation>().AsEnumerable());
            }
            foreach (XmlTreeAnnotation epmAnn in annotations.OfType<XmlTreeAnnotation>().Where(child => child.IsAttribute && child.LocalName == TestAtomConstants.AtomLinkHrefAttributeName))
            {
                epmAnn.PropertyValue = this.FixUris(epmAnn.PropertyValue);
            }
        }

        /// <summary>
        /// Correct the links used in MLEs to make sure they are valid URIs
        /// </summary>
        /// <param name="payload">payload whose links to correct</param>
        private string FixUris(string uri)
        {
            if (!string.IsNullOrEmpty(uri) && !Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                uri = new Uri("http://" + uri + ".com/").OriginalString;
            }
            return uri;
        }

        /// <summary>
        /// Visitor to remove ReplaceAnnotations
        /// </summary>
        internal class ReplaceAnnotationRemover : ODataPayloadElementVisitorBase, IODataPayloadElementVisitor
        {
            /// <summary>
            /// Visits a payload element whose root is a ComplexCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(ComplexMultiValueProperty payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(ComplexInstance payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(ComplexInstanceCollection payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(ComplexProperty payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(EmptyCollectionProperty payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is an EntityInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(EntityInstance payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is an EntitySetInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(EntitySetInstance payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(NullPropertyInstance payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(PrimitiveCollection payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(PrimitiveMultiValueProperty payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(PrimitiveProperty payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public override void Visit(PrimitiveValue payloadElement)
            {
                RemoveChangeAnnotations(payloadElement);
            }

            /// <summary>
            /// Remove ReplacedElementAnnotation from an ODataPayloadElement
            /// </summary>
            /// <param name="payloadElement">ODataPayloadElement from which to remove annotations.</param>
            private void RemoveChangeAnnotations(ODataPayloadElement payload)
            {
                payload.RemoveAnnotations(typeof(ReplacedElementAnnotation));
            }
        }
    }
}
