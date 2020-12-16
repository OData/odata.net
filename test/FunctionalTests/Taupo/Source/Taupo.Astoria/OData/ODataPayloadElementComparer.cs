//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default implementation of the payload element comparer contract
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementComparer), "Default")]
    public class ODataPayloadElementComparer : IODataPayloadElementComparer
    {
        /// <summary>true to ignore the order of properties; otherwise false.</summary>
        /// <remarks>When using this flag we assume that no duplicate properties exist (and fail otherwise).</remarks>
        private readonly bool ignoreOrder;

        /// <summary>true if we are comparing a Json Light payload, false otherwise.</summary>
        private readonly bool expectMetadataToBeComputedByConvention;

        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementComparer class.
        /// </summary>
        public ODataPayloadElementComparer()
            : this(ignoreOrder:false, expectMetadataToBeComputedByConvention:false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementComparer class.
        /// </summary>
        /// <param name="ignoreOrder">true to ignore the order of properties; otherwise false.</param>
        /// <param name="expectMetadataToBeComputedByConvention">true if we are expecting metadata to be computed by convention, e.g. comparing a JSON Light reponse payload; otherwise false.</param>
        /// <remarks>When using the <paramref name="ignoreOrder"/> flag we assume that no duplicate properties exist (and fail otherwise).</remarks>
        protected ODataPayloadElementComparer(bool ignoreOrder, bool expectMetadataToBeComputedByConvention)
        {
            this.ignoreOrder = ignoreOrder;
            this.expectMetadataToBeComputedByConvention = expectMetadataToBeComputedByConvention;
        }

        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public StackBasedAssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IBatchPayloadComparer BatchComparer { get; set; }

        /// <summary>
        /// Compares the two payload elements
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="actual">The actual element</param>
        public void Compare(ODataPayloadElement expected, ODataPayloadElement actual)
        {
            var comparingVisitor = new PayloadComparingVisitor(this.BatchComparer, this.ignoreOrder, this.expectMetadataToBeComputedByConvention);
            comparingVisitor.Assert = this.Assert;
            comparingVisitor.Compare(expected, actual);
        }

        /// <summary>
        /// Helper class for visiting two payloads and comparing them.
        /// Stores observed elements in a stack so that both trees can be walked in parallel without breaking the double-dispatch visitor pattern.
        /// </summary>
        private sealed class PayloadComparingVisitor : IODataPayloadElementVisitor
        {
            /// <summary>true to ignore the order of properties; otherwise false.</summary>
            private readonly bool ignoreOrder;

            /// <summary>true if we are comparing a JSON Light reponse payload; otherwise false.</summary>
            private readonly bool comparingJsonLightResponse;

            private Stack<ODataPayloadElement> observedElementStack = new Stack<ODataPayloadElement>();
            private IBatchPayloadComparer batchComparer = null;
            
            /// <summary>
            /// Initializes a new instance of the PayloadComparingVisitor class.
            /// </summary>
            /// <param name="comparer">Specifies a batch comparer if needed</param>
            /// <param name="ignoreOrder">true to ignore the order of properties; otherwise false.</param>
            /// <param name="comparingJsonLightResponse">true if we are comparing a JSON Light reponse payload; otherwise false.</param>
            public PayloadComparingVisitor(IBatchPayloadComparer comparer = null, bool ignoreOrder = false, bool comparingJsonLightResponse = false)
            {
                this.batchComparer = comparer;
                this.ignoreOrder = ignoreOrder;
                this.comparingJsonLightResponse = comparingJsonLightResponse;
            }

            /// <summary>
            /// Gets or sets the assertion handler to use
            /// </summary>
            public StackBasedAssertionHandler Assert { get; set; }

            /// <summary>
            /// Entry point for the visitor, recursively compares the given elements
            /// </summary>
            /// <param name="expected">The expected element</param>
            /// <param name="actual">The actual element</param>
            public void Compare(ODataPayloadElement expected, ODataPayloadElement actual)
            {
                ExceptionUtilities.CheckObjectNotNull(this.Assert, "Assertion handler must be set");
                this.WrapAccept(expected, actual);
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchRequestPayload.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(BatchRequestPayload expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<BatchRequestPayload>();

                using (this.Assert.WithMessage("Batch request did not match expectation"))
                {
                    this.batchComparer.CompareBatchPayload(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchResponsePayload.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(BatchResponsePayload expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<BatchResponsePayload>();

                using (this.Assert.WithMessage("Batch response did not match expectation"))
                {
                    this.batchComparer.CompareBatchPayload(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ComplexInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement();

                if (observed.ElementType == ODataPayloadElementType.EntityInstance
                    || observed.ElementType == ODataPayloadElementType.ComplexInstance)
                {

                    using (this.Assert.WithMessage("Complex instance did not match expectation"))
                    {
                        this.CompareComplexInstance(expected, (ComplexInstance)observed);
                    }
                }
                else
                {
                    var expandedLink = (ExpandedLink)observed;
                    this.WrapAccept(expected, expandedLink.ExpandedElement);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ComplexInstanceCollection expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ComplexInstanceCollection>();

                using (this.Assert.WithMessage("Complex instance collection did not match expectation"))
                {
                    this.CompareCollection(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValue.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValue expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement();
                if (observed.ElementType == ODataPayloadElementType.ExpandedLink)
                {
                    using (this.Assert.WithMessage("Complex multi-value did not match expectation"))
                    {
                        var expandedLink = (ExpandedLink)observed;
                        this.CompareList<ComplexInstance, ODataPayloadElement>(expected, expandedLink.ExpandedElement as EntitySetInstance);
                    }
                }
                else if (observed.ElementType == ODataPayloadElementType.EntitySetInstance)
                {
                    using (this.Assert.WithMessage("Complex multi-value did not match expectation"))
                    {
                        this.CompareList<ComplexInstance, ODataPayloadElement>(expected, (EntitySetInstance)observed);
                    }
                }
                else
                {
                    using (this.Assert.WithMessage("Complex multi-value did not match expectation"))
                    {
                        ComplexMultiValue complexMultiValue = (ComplexMultiValue)observed;
                        this.Assert.AreEqual(expected.IsNull, complexMultiValue.IsNull, "Null flag did not match expectation");
                        this.Assert.AreEqual(expected.FullTypeName, complexMultiValue.FullTypeName, "Full type name did not match expectation");

                        this.CompareCollection(expected, complexMultiValue);
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValueProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValueProperty expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement();

                if (observed.ElementType == ODataPayloadElementType.NavigationPropertyInstance)
                {
                    using (this.Assert.WithMessage("Complex multi-value property '{0}' did not match expectation", expected.Name))
                    {
                        var navigationPropertyInstance = (NavigationPropertyInstance)observed;

                        this.Assert.AreEqual(expected.Name, navigationPropertyInstance.Name, "Property name did not match expectation");
                        this.WrapAccept(expected.Value, navigationPropertyInstance.Value);
                    }

                }
                else if (observed.ElementType == ODataPayloadElementType.EntitySetInstance)
                {
                    using (this.Assert.WithMessage("Complex multi-value property '{0}' did not match expectation", expected.Name))
                    {
                        this.WrapAccept(expected.Value, (EntitySetInstance)observed);
                    }
                }
                else
                {
                    using (this.Assert.WithMessage("Complex multi-value property '{0}' did not match expectation", expected.Name))
                    {
                        var complexMultiValue = (ComplexMultiValueProperty)observed;
                        this.Assert.AreEqual(expected.Name, complexMultiValue.Name, "Property name did not match expectation");

                        this.CompareAnnotations(expected.Annotations, complexMultiValue.Annotations);
                        this.WrapAccept(expected.Value, complexMultiValue.Value);
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ComplexProperty expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                //var observed = this.GetNextObservedElement<ComplexProperty>();
                var observed = this.GetNextObservedElement();
                if (observed.ElementType == ODataPayloadElementType.EntityInstance
                    || observed.ElementType == ODataPayloadElementType.ComplexInstance)
                {
                    using (this.Assert.WithMessage("Complex property '{0}' did not match expectation", expected.Name))
                    {
                        this.WrapAccept(expected.Value, observed);
                    }
                }
                else if (observed.ElementType == ODataPayloadElementType.ComplexProperty)
                {
                    var complexProperty = (ComplexProperty)observed;
                    this.Assert.AreEqual(expected.Name, complexProperty.Name, "Property name did not match expectation");
                    this.WrapAccept(expected.Value, complexProperty.Value);

                }
                else
                {
                    var navigationProperty = (NavigationPropertyInstance)observed;
                    this.Assert.AreEqual(expected.Name, navigationProperty.Name, "Property name did not match expectation");
                    this.WrapAccept(expected.Value, navigationProperty.Value);
                }

            }

            /// <summary>
            /// Visits a payload element whose root is a DeferredLink.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(DeferredLink expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<DeferredLink>();

                using (this.Assert.WithMessage("Deferred link did not match expectation"))
                {
                    if (this.comparingJsonLightResponse && expected.UriString == null)
                    {
                        this.Assert.IsNotNull(observed.UriString, "Conventional template evaluation should compute the Uri string of the deferred link.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.UriString, observed.UriString, "Uri string did not match expectation");
                    }

                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyCollectionProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(EmptyCollectionProperty expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<EmptyCollectionProperty>();

                using (this.Assert.WithMessage("Empty collection property '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Property name did not match expectation");
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.WrapAccept(expected.Value, observed.Value);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyPayload.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(EmptyPayload expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<EmptyPayload>();

                using (this.Assert.WithMessage("Empty payload did not match expectation"))
                {
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyUntypedCollection.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(EmptyUntypedCollection expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<EmptyUntypedCollection>();

                using (this.Assert.WithMessage("Empty untyped collection did not match expectation"))
                {
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EntityInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(EntityInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<EntityInstance>();

                using (this.Assert.WithMessage("Entity instance did not match expectation"))
                {
                    if (expected.IsNull && observed.IsNull)
                    {
                        return;
                    }

                    this.Assert.AreEqual(expected.ETag, observed.ETag, "ETag did not match expectation");

                    if (this.comparingJsonLightResponse && expected.Id == null && !expected.IsComplex)
                    {
                        this.Assert.IsNotNull(observed.Id, "Conventional template evaluation should compute the Id.");
                    }
                    else if (expected.Id != null)
                    {
                        this.Assert.AreEqual(expected.Id, observed.Id, "ID did not match expectation");
                    }

                    if (this.comparingJsonLightResponse && expected.EditLink == null && !expected.IsComplex)
                    {
                        this.Assert.IsNotNull(observed.EditLink, "Conventional template evaluation should compute the EditLink.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.EditLink, observed.EditLink, "Edit link did not match expectation");
                    }

                    if (!expected.IsComplex)
                    {
                        this.Assert.AreEqual(expected.StreamETag, observed.StreamETag, "Stream ETag did not match expectation");
                        this.Assert.AreEqual(expected.StreamContentType, observed.StreamContentType, "Stream content type did not match expectation");

                        var isMle = expected.Annotations.OfType<IsMediaLinkEntryAnnotation>().FirstOrDefault();
                        if (this.comparingJsonLightResponse && isMle != null)
                        {
                            this.Assert.IsNotNull(observed.StreamSourceLink, "Conventional template evaluation should compute the StreamSourceLink.");
                            this.Assert.IsNotNull(observed.StreamEditLink, "Conventional template evaluation should compute the StreamEditLink.");
                        }
                        else
                        {
                            this.Assert.AreEqual(expected.StreamSourceLink, observed.StreamSourceLink, "Stream source link did not match expectation");
                            this.Assert.AreEqual(expected.StreamEditLink, observed.StreamEditLink, "Stream edit link did not match expectation");
                        }
                    }
                    this.CompareList(expected.ServiceOperationDescriptors, observed.ServiceOperationDescriptors);

                    this.CompareComplexInstance(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EntitySetInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(EntitySetInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<EntitySetInstance>();

                using (this.Assert.WithMessage("Entity set instance did not match expectation"))
                {
                    this.Assert.AreEqual(expected.NextLink, observed.NextLink, "Next link did not match expectation");
                    this.Assert.AreEqual(expected.InlineCount, observed.InlineCount, "Inline count did not match expectation");

                    this.CompareCollection(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an ExpandedLink.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ExpandedLink expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ExpandedLink>();

                using (this.Assert.WithMessage("Expanded link did not match expectation"))
                {
                    if (expected.ExpandedElement == null)
                    {
                        this.Assert.IsNull(observed.ExpandedElement, "Value unexpectedly non-null");
                        this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    }
                    else
                    {
                        this.Assert.IsNotNull(observed.ExpandedElement, "Value unexpectedly null");
                        this.CompareAnnotations(expected.Annotations, observed.Annotations);
                        this.WrapAccept(expected.ExpandedElement, observed.ExpandedElement);
                    }

                    if (this.comparingJsonLightResponse && expected.UriString == null)
                    {
                        this.Assert.IsNotNull(observed.UriString, "Conventional template evaluation should compute the Uri string of the expanded link.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.UriString, observed.UriString, "Uri string did not match expectation");
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a LinkCollection.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(LinkCollection expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<LinkCollection>();

                using (this.Assert.WithMessage("Deferred link collection did not match expectation"))
                {
                    this.CompareCollection(expected, observed);
                    this.Assert.AreEqual(expected.InlineCount, observed.InlineCount, "Inline count did not match expectation");
                    this.Assert.AreEqual(expected.NextLink, observed.NextLink, "Next link did not match expectation");
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NamedStreamProperty.  this will validate the content types and etags
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(NamedStreamInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<NamedStreamInstance>();

                using (this.Assert.WithMessage("Named stream '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Stream name did not match expectation");

                    if (this.comparingJsonLightResponse && expected.EditLink == null)
                    {
                        this.Assert.IsNotNull(observed.EditLink, "Conventional template evaluation should compute the EditLink.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.EditLink, observed.EditLink, "Edit link did not match expectation");
                    }

                    if (this.comparingJsonLightResponse && (expected.EditLinkContentType != null || expected.SourceLinkContentType != null))
                    {
                        this.Assert.IsNotNull(observed.EditLinkContentType, "Conventional template evaluation should compute the edit link content type.");
                        this.Assert.IsNotNull(observed.SourceLinkContentType, "Conventional template evaluation should compute the source link content type.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.EditLinkContentType, observed.EditLinkContentType, "Edit link content type did not match expectation");
                        this.Assert.AreEqual(expected.SourceLinkContentType, observed.SourceLinkContentType, "Source link content type did not match expectation");                        
                    }

                    this.Assert.AreEqual(expected.ETag, observed.ETag, "ETag did not match expectation");

                    if (this.comparingJsonLightResponse && expected.SourceLink == null)
                    {
                        this.Assert.IsNotNull(observed.SourceLink, "Conventional template evaluation should compute the SourceLink.");
                    }
                    else
                    {
                        this.Assert.AreEqual(expected.SourceLink, observed.SourceLink, "Source link did not match expectation");
                    }

                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NavigationProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(NavigationPropertyInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<NavigationPropertyInstance>();

                using (this.Assert.WithMessage("Navigation property '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Property name did not match expectation");

                    if (expected.Value == null)
                    {
                        this.Assert.IsNull(observed.Value, "Value unexpectedly non-null");
                        this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    }
                    else
                    {
                        this.Assert.IsNotNull(observed.Value, "Value unexpectedly null");
                        this.CompareAnnotations(expected.Annotations, observed.Annotations);
                        this.WrapAccept(expected.Value, observed.Value);
                    }

                    // association links
                    if (expected.AssociationLink == null)
                    {
                        this.Assert.IsNull(observed.AssociationLink, "Association link unexpectedly non-null.");
                    }
                    else
                    {
                        this.Assert.IsNotNull(observed.AssociationLink, "Association link unexpectedly null.");
                        this.WrapAccept(expected.AssociationLink, observed.AssociationLink);
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(NullPropertyInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<NullPropertyInstance>();

                using (this.Assert.WithMessage("Null property '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Property name did not match expectation");
                    this.Assert.AreEqual(expected.FullTypeName, observed.FullTypeName, "Full type name did not match expectation");
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataErrorPayload.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ODataErrorPayload expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ODataErrorPayload>();

                using (this.Assert.WithMessage("Error did not match expectation"))
                {
                    this.Assert.AreEqual(expected.Message, observed.Message, "Message did not match expectation");
                    this.Assert.AreEqual(expected.Code, observed.Code, "Code did not match expectation");
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);

                    if (expected.InnerError != null)
                    {
                        this.Assert.IsNotNull(observed.InnerError, "Inner error unexpectedly null");
                        this.WrapAccept(expected.InnerError, observed.InnerError);
                    }
                    else
                    {
                        this.Assert.IsNull(observed.InnerError, "Inner error unexpectedly non-null");
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataInternalExceptionPayload.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ODataInternalExceptionPayload expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ODataInternalExceptionPayload>();

                // create an error message based on the current depth
                // ie: "Inner inner inner error did not match expectation"
                ExceptionUtilities.Assert(this.observedElementStack.Count > 1, "Inner error cannot appear as root element");
                string message = "Inner " + string.Concat(Enumerable.Repeat("inner ", this.observedElementStack.Count - 2)) + "error did not match expectation";

                using (this.Assert.WithMessage(message))
                {
                    this.Assert.AreEqual(expected.Message, observed.Message, "Message did not match expectation");
                    this.Assert.AreEqual(expected.StackTrace, observed.StackTrace, "Stack trace did not match expectation");
                    this.Assert.AreEqual(expected.TypeName, observed.TypeName, "Type name did not match expectation");
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);

                    if (expected.InternalException != null)
                    {
                        this.Assert.IsNotNull(observed.InternalException, "Inner error unexpectedly null");
                        this.WrapAccept(expected.InternalException, observed.InternalException);
                    }
                    else
                    {
                        this.Assert.IsNull(observed.InternalException, "Inner error unexpectedly non-null");
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(PrimitiveCollection expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<PrimitiveCollection>();

                using (this.Assert.WithMessage("Primitive collection did not match expectation"))
                {
                    this.CompareCollection(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValue.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValue expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<PrimitiveMultiValue>();

                using (this.Assert.WithMessage("Primitive multi-value did not match expectation"))
                {
                    this.Assert.AreEqual(expected.IsNull, observed.IsNull, "Null flag did not match expectation");
                    this.Assert.AreEqual(expected.FullTypeName, observed.FullTypeName, "Full type name did not match expectation");

                    this.CompareCollection(expected, observed);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollectionProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValueProperty expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<PrimitiveMultiValueProperty>();

                using (this.Assert.WithMessage("Primitive multi-value property '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Property name did not match expectation");

                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.WrapAccept(expected.Value, observed.Value);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(PrimitiveProperty expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");

                // This is a bug of this test framework that it treat null complex as primitiveValue.
                if (expected.Value.ElementType == ODataPayloadElementType.PrimitiveValue &&  expected.Value != null && expected.Value.ClrValue == null)
                {
                    var nullValue = this.GetNextObservedElement();
                    if (nullValue.ElementType == ODataPayloadElementType.NavigationPropertyInstance)
                    {
                        this.Assert.IsTrue(string.IsNullOrEmpty(((NavigationPropertyInstance)nullValue).Value.StringRepresentation.Trim(' ', '{', '}')), "read null value");
                        return;
                    }
                    
                }

                var observed = this.GetNextObservedElement<PrimitiveProperty>();

                using (this.Assert.WithMessage("Primitive property '{0}' did not match expectation", expected.Name))
                {
                    this.Assert.AreEqual(expected.Name, observed.Name, "Name did not match expectation");
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.WrapAccept(expected.Value, observed.Value);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(PrimitiveValue expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<PrimitiveValue>();

                using (this.Assert.WithMessage("Primitive value did not match expectation"))
                {
                    if (expected.IsNull)
                    {
                        this.Assert.IsTrue(observed.IsNull, "Unexpectedly non-null");
                    }
                    else
                    {
                        this.Assert.IsFalse(observed.IsNull, "Unexpectedly null");
                    }

                    this.Assert.AreEqual(expected.ClrValue, observed.ClrValue, "Clr value did not match expectation");
                    if (expected.ClrValue != null)
                    {
                        this.Assert.IsTrue(expected.ClrValue.GetType().IsAssignableFrom(observed.ClrValue.GetType()), "Type did not match expectation");
                    }

                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ResourceCollectionInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ResourceCollectionInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ResourceCollectionInstance>();

                using (this.Assert.WithMessage("Resource collection instance did not match expectation"))
                {
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.Assert.AreEqual(expected.Title, observed.Title, "Title did not match expectation");
                    this.Assert.AreEqual(expected.Href, observed.Href, "Href did not match expectation");
                    this.Assert.AreEqual(expected.Name, observed.Name, "Name did not match expectation");
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceDocumentInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ServiceDocumentInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ServiceDocumentInstance>();

                using (this.Assert.WithMessage("Service document did not match expectation"))
                {
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.CompareList(expected.Workspaces, observed.Workspaces);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ActionDescriptor.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(ServiceOperationDescriptor expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<ServiceOperationDescriptor>();

                this.Assert.AreEqual(expected.Metadata, observed.Metadata, "Metadata did not match expectation");
                this.Assert.AreEqual(expected.Target, observed.Target, "Target did not match expectation");
                this.Assert.AreEqual(expected.Title, observed.Title, "Title did not match expectation");
            }

            /// <summary>
            /// Visits a payload element whose root is a WorkspaceInstance.
            /// </summary>
            /// <param name="expected">The root node of payload element being visited.</param>
            public void Visit(WorkspaceInstance expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                var observed = this.GetNextObservedElement<WorkspaceInstance>();

                using (this.Assert.WithMessage("Workspace instance did not match expectation"))
                {
                    this.CompareAnnotations(expected.Annotations, observed.Annotations);
                    this.Assert.AreEqual(expected.Title, observed.Title, "Title did not match expectation");
                    this.CompareList(expected.ResourceCollections, observed.ResourceCollections);
                }
            }

            private void CompareComplexInstance(ComplexInstance expected, ComplexInstance actual)
            {
                if (expected.IsNull)
                {
                    this.Assert.IsTrue(actual.IsNull, "Instance unexpectedly non-null");
                }
                else
                {
                    this.Assert.IsFalse(actual.IsNull, "Instance unexpectedly null");
                }

                this.Assert.AreEqual(expected.FullTypeName, actual.FullTypeName, "Full type name did not match expectation");

                //[TODO]:layliu Compare Annotations
                //this.CompareAnnotations(expected.Annotations, actual.Annotations);

                var expectedProperties = expected.Properties.ToList();
                var actualProperties = actual.Properties.ToList();

                var propertyMismatchErrorBuilder = new StringBuilder();
                if (expectedProperties.Count != actualProperties.Count)
                {
                    propertyMismatchErrorBuilder.AppendLine("Number of properties did not match expectation");

                    var expectedPropertyNames = expectedProperties.Select(p => p.Name).ToList();
                    var actualPropertyNames = actualProperties.Select(p => p.Name).ToList();

                    var extraPropertyNames = actualPropertyNames.Except(expectedPropertyNames).ToList();
                    var missingPropertyNames = expectedPropertyNames.Except(actualPropertyNames).ToList();

                    foreach (var expectedProperty in expectedProperties)
                    {
                        if (missingPropertyNames.Contains(expectedProperty.Name))
                        {
                            propertyMismatchErrorBuilder.AppendLine("Missing property: " + expectedProperty.StringRepresentation.Replace("{", "{{").Replace("}", "}}"));
                        }
                    }

                    foreach (var actualProperty in actualProperties)
                    {
                        if (extraPropertyNames.Contains(actualProperty.Name))
                        {
                            propertyMismatchErrorBuilder.AppendLine("Extra property: " + actualProperty.StringRepresentation.Replace("{", "{{").Replace("}", "}}"));
                        }
                    }
                }

                this.Assert.AreEqual(expectedProperties.Count, actualProperties.Count, propertyMismatchErrorBuilder.ToString());

                // If we do not care about property order, align the actual properties with the expected ones
                if (this.ignoreOrder)
                {
                    actualProperties = this.OrderActualProperties(expectedProperties, actualProperties);
                }

                for (int i = 0; i < expectedProperties.Count; i++)
                {
                    using (this.Assert.WithMessage("Property at position {0} did not match expectation", i + 1))
                    {
                        this.WrapAccept(expectedProperties[i], actualProperties[i]);
                    }
                }
            }

            private List<PropertyInstance> OrderActualProperties(List<PropertyInstance> expectedProperties, List<PropertyInstance> actualProperties)
            {
                List<PropertyInstance> orderedProperties = new List<PropertyInstance>(expectedProperties.Count);
                for (int i = 0; i < expectedProperties.Count; ++i)
                {
                    // NOTE: we assume that no duplicate properties exist otherwise aligning the property values is hard.
                    PropertyInstance property = actualProperties.Where(p => p.Name == expectedProperties[i].Name).SingleOrDefault();
                    if (property != null)
                    {
                        orderedProperties.Add(property);
                        actualProperties.Remove(property);
                    }
                }

                // Append any unexpected properties at the end.
                if (actualProperties.Count > 0)
                {
                    orderedProperties.AddRange(actualProperties);
                }

                return orderedProperties;
            }

            private void CompareCollection<TElement>(ODataPayloadElementCollection<TElement> expected, ODataPayloadElementCollection<TElement> actual) where TElement : ODataPayloadElement
            {
                this.CompareAnnotations(expected.Annotations, actual.Annotations);
                this.CompareList<TElement, TElement>(expected, actual);
            }

            //private void CompareList<TElement>(IList<TElement> expected, IList<TElement> actual) where TElement : ODataPayloadElement
            //{
            //    if (expected == null && actual == null)
            //    {
            //        return;
            //    }

            //    this.Assert.IsFalse(expected == null || actual == null, "One of the lists is null.");
            //    this.Assert.AreEqual(expected.Count, actual.Count, "Count did not match expectation");
            //    for (int i = 0; i < expected.Count; i++)
            //    {
            //        using (this.Assert.WithMessage("Value at position {0} did not match expectation", i + 1))
            //        {
            //            this.WrapAccept(expected[i], actual[i]);
            //        }
            //    }
            //}

            private void CompareList<T1, T2>(IList<T1> expected, IList<T2> actual)
                where T1 : ODataPayloadElement
                where T2 : ODataPayloadElement
            {
                if (expected == null && actual == null)
                {
                    return;
                }

                this.Assert.IsFalse(expected == null || actual == null, "One of the lists is null.");
                this.Assert.AreEqual(expected.Count, actual.Count, "Count did not match expectation");
                for (int i = 0; i < expected.Count; i++)
                {
                    using (this.Assert.WithMessage("Value at position {0} did not match expectation", i + 1))
                    {
                        this.WrapAccept(expected[i], actual[i]);
                    }
                }
            }

            private void CompareAnnotations(IList<ODataPayloadElementAnnotation> expected, IList<ODataPayloadElementAnnotation> observed)
            {
                var expectedFiltered = expected.OfType<ODataPayloadElementEquatableAnnotation>().Where(o => !o.IgnoreDuringPayloadComparison).ToList();
                var observedFiltered = observed.OfType<ODataPayloadElementEquatableAnnotation>().Where(o => !o.IgnoreDuringPayloadComparison).ToList();

                ODataPayloadElementEquatableAnnotation match;
                foreach (ODataPayloadElementEquatableAnnotation expectedAnnotation in expectedFiltered)
                {
                    match = observedFiltered.FirstOrDefault(a => expectedAnnotation.Equals(a));
                    this.Assert.IsNotNull(
                        match,
                        "Could not find expected annotation: \r\n{0}\r\nActual annotations: \r\n{1}",
                        expectedAnnotation.StringRepresentation,
                        string.Join("\r\n", observedFiltered.Select(o => o.StringRepresentation).ToArray()));
                    observedFiltered.Remove(match);
                }

                if (this.comparingJsonLightResponse)
                {
                    match = observedFiltered.OfType<SelfLinkAnnotation>().FirstOrDefault();
                    if (match != null)
                    {
                        observedFiltered.Remove(match);
                    }
                }

                this.Assert.IsFalse(
                    observedFiltered.Count > 0,
                    "Should not have been any left over annotations: \r\n{0}", 
                    string.Join("\r\n", observedFiltered.Select(o => o.StringRepresentation).ToArray()));
            }

            private ODataPayloadElement GetNextObservedElement()
            {
                return this.observedElementStack.Peek();
            }
            private TElement GetNextObservedElement<TElement>() where TElement : ODataPayloadElement
            {
                var observed = this.observedElementStack.Peek();
                //if (ODataPayloadElement.GetElementType<TElement>() == ODataPayloadElementType.ComplexProperty)
                //{
                //    this.Assert.AreEqual(ODataPayloadElementType.EntityInstance, observed.ElementType, "Unexpected element type");
                //}
                //else
                //{
                //    

                var elementType = ODataPayloadElement.GetElementType<TElement>();
                this.Assert.IsTrue(elementType == observed.ElementType
                    || (elementType == ODataPayloadElementType.ComplexInstance && observed.ElementType == ODataPayloadElementType.EntityInstance), "Unexpected element type");
                //}
                return (TElement)observed;
            }

            private void WrapAccept(ODataPayloadElement expected, ODataPayloadElement observed)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                ExceptionUtilities.CheckArgumentNotNull(observed, "observed");

                this.observedElementStack.Push(observed);
                try
                {
                    expected.Accept(this);
                }
                finally
                {
                    this.observedElementStack.Pop();
                }
            }
        }
    }
}
