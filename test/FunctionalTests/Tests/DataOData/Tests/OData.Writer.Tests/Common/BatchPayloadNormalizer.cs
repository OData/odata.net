//---------------------------------------------------------------------
// <copyright file="BatchPayloadNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces.

    /// <summary>
    /// Payload element normalizer for batch format payloads. 
    /// </summary>
    public class BatchPayloadNormalizer
    {
        /// <summary>
        /// Gets or sets the normalizer selector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatNormalizerSelector NormalizerSelector { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Normalizes the observed batch payload element.
        /// </summary>
        /// <param name="expectedPayload">The expected payload element.</param>
        /// <param name="observedPayload">The observed payload element.</param>
        /// <returns>The normalized payload element.</returns>
        public ODataPayloadElement Normalize(ODataPayloadElement expectedPayload, ODataPayloadElement observedPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedPayload, "expectedPayload");

            ODataPayloadElement copyOfObservedPayload = observedPayload.DeepCopy();

            var visitor = new BatchPayloadNormalizingVisitor 
            {   
                Assert = this.Assert, 
                NormalizerSelector = this.NormalizerSelector,
            };
            
            visitor.Normalize(expectedPayload, copyOfObservedPayload);

            return copyOfObservedPayload;
        }

        /// <summary>
        /// Visits two batch payload elements in parallel, performing normalization on the observed payload.
        /// </summary>
        /// <remarks>This visitor modifies the observed payload element.</remarks>
        private class BatchPayloadNormalizingVisitor : ODataPayloadElementVisitorBase
        {
            private Stack<object> observedElementStack = new Stack<object>();

            /// <summary>
            /// Gets or sets the assertion handler.
            /// </summary>
            public AssertionHandler Assert { get; set; }

            /// <summary>
            /// Gets or sets the normalizer selector.
            /// </summary>
            public IProtocolFormatNormalizerSelector NormalizerSelector { get; set; }

            /// <summary>
            /// Normalizes the observed payload.
            /// </summary>
            /// <param name="expectedPayload">The expected payload element.</param>
            /// <param name="observedPayload">The observed payload element.</param>
            public void Normalize(ODataPayloadElement expectedPayload, ODataPayloadElement observedPayload)
            {
                this.WrapAccept(expectedPayload, observedPayload);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(BatchRequestPayload payloadElement)
            {
                var observed = this.GetNextObservedElement<BatchRequestPayload>();

                this.WrapAccept(
                    payloadElement, 
                    observed, 
                    (e) => this.VisitBatch<IHttpRequest, BatchRequestChangeset, BatchRequestPayload>((BatchRequestPayload)e, this.VisitRequestOperation));
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(BatchResponsePayload payloadElement)
            {
                var observed = this.GetNextObservedElement<BatchResponsePayload>();

                this.WrapAccept(
                    payloadElement, 
                    observed,
                    (e) => this.VisitBatch<HttpResponseData, BatchResponseChangeset, BatchResponsePayload>((BatchResponsePayload)e, this.VisitResponseOperation));
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(ComplexInstance payloadElement)
            {
                var observed = this.GetNextObservedElement<ComplexInstance>();
                this.VisitEnumerable(payloadElement.Properties, observed.Properties);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(ComplexInstanceCollection payloadElement)
            {
                var observed = this.GetNextObservedElement<ComplexInstanceCollection>();
                this.VisitEnumerable(payloadElement, observed);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(ComplexMultiValue payloadElement)
            {
                var observed = this.GetNextObservedElement<ComplexMultiValue>();
                this.VisitEnumerable(payloadElement, observed);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(ComplexMultiValueProperty payloadElement)
            {
                var observed = this.GetNextObservedElement<ComplexMultiValueProperty>();
                this.WrapAccept(payloadElement.Value, observed.Value);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(ComplexProperty payloadElement)
            {
                var observed = this.GetNextObservedElement<ComplexProperty>();
                this.WrapAccept(payloadElement.Value, observed.Value);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(EntityInstance payloadElement)
            {
                // Mark corresponding element with the EntityTypeAnnotation so that the 
                // normalizer will convert it if the type is incorrect.
                var observed = this.GetNextObservedElement<ComplexInstance>();
                observed.AddAnnotation(new EntitySetAnnotation());

                this.VisitEnumerable(payloadElement.Properties, observed.Properties);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(EntitySetInstance payloadElement)
            {
                var observed = this.GetNextObservedElement<EntitySetInstance>();
                this.VisitEnumerable(payloadElement, observed);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(PrimitiveCollection payloadElement)
            {
                var observed = this.GetNextObservedElement<PrimitiveCollection>();
                this.VisitEnumerable(payloadElement, observed);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(PrimitiveMultiValue payloadElement)
            {
                var observed = this.GetNextObservedElement<PrimitiveMultiValue>();
                this.VisitEnumerable(payloadElement, observed);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(PrimitiveMultiValueProperty payloadElement)
            {
                var observed = this.GetNextObservedElement<PrimitiveMultiValueProperty>();
                this.WrapAccept(payloadElement.Value, observed.Value);
            }

            /// <summary>
            /// Visits the payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(PrimitiveProperty payloadElement)
            {
                var observed = this.GetNextObservedElement<PrimitiveProperty>();
                this.WrapAccept(payloadElement.Value, observed.Value);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit.</param>
            public override void Visit(PrimitiveValue payloadElement)
            {
                var observed = this.GetNextObservedElement<PrimitiveValue>();
                if (!observed.Annotations.OfType<DataTypeAnnotation>().Any())
                {
                    observed.Add(new DataTypeAnnotation { DataType = DataTypes.Integer.WithPrimitiveClrType(payloadElement.ClrValue.GetType()) });
                }
            }

            /// <summary>
            /// Visits the batch payload.
            /// </summary>
            /// <typeparam name="TOperation">The operation type.</typeparam>
            /// <typeparam name="TChangeset">The changeset type.</typeparam>
            /// <typeparam name="TBatch">The batch payload type.</typeparam>
            /// <param name="batchPayload">The batch payload we are visiting.</param>
            /// <param name="visitOperation">The action to use to visit the operation.</param>
            protected override void VisitBatch<TOperation, TChangeset, TBatch>(TBatch batchPayload, Action<TOperation> visitOperation)
            {
                var observed = this.GetNextObservedElement<TBatch>();

                this.Assert.AreEqual(batchPayload.Parts.Count(), observed.Parts.Count(), "Parts count for expected and observed do not match");
                for (int i = 0; i < batchPayload.Parts.Count(); ++i)
                {
                    var expectedChangeSet = batchPayload.Parts.ElementAt(i) as TChangeset;
                    if (expectedChangeSet != null)
                    {
                        var observedChangeSet = observed.Parts.ElementAt(i) as TChangeset;
                        this.Assert.IsNotNull(observedChangeSet, "Observed batch part at " + i + " does not match type of expected");
                        this.WrapAccept(expectedChangeSet, observedChangeSet, (e) => this.VisitChangeset((TChangeset)e, visitOperation));
                    }
                    else
                    {
                        var expectedOperation = (MimePartData<TOperation>)batchPayload.Parts.ElementAt(i);
                        var observedOperation = (MimePartData<TOperation>)observed.Parts.ElementAt(i);
                        this.WrapAccept(expectedOperation.Body, observedOperation.Body, (e) => visitOperation(e));
                    }
                }
            }

            /// <summary>
            /// Visits the batch changeset.
            /// </summary>
            /// <typeparam name="TOperation">Type of operation in the changeset.</typeparam>
            /// <typeparam name="TChangeset">Type of changeset.</typeparam>
            /// <param name="changeset">Changeset to visit.</param>
            /// <param name="visitOperation">Action to use when visiting operations.</param>
            protected override void VisitChangeset<TOperation, TChangeset>(TChangeset changeset, Action<TOperation> visitOperation)
            {
                var observed = this.GetNextObservedElement<TChangeset>();
                this.VisitEnumerable(changeset.Operations, observed.Operations, (e) => visitOperation(e));
            }

            /// <summary>
            /// Visits a request operation payload
            /// </summary>
            /// <param name="operation">Operation to visit</param>
            protected override void VisitRequestOperation(IHttpRequest operation)
            {
                var requestOperation = operation as ODataRequest;
                this.Assert.IsNotNull(requestOperation, "Request operation has unexpected type.");

                var observed = this.GetNextObservedElement<ODataRequest>();

                if (requestOperation.Body != null && requestOperation.Body.SerializedValue.Length > 0)
                {
                    this.Assert.IsNotNull(observed.Body, "Expected non-null body on request operation.");

                    var requestOperationRootElement = requestOperation.Body.RootElement;
                    if (requestOperationRootElement != null)
                    {
                        var observedRootElement = observed.Body.RootElement;
                        this.Assert.IsNotNull(observedRootElement, "Expected non-null root element on request operation.");

                        this.WrapAccept(requestOperationRootElement, observedRootElement);

                        observed.Body = new ODataPayloadBody(observed.Body.SerializedValue, this.ContentTypeSpecificNormalize(observedRootElement, operation.Headers));
                    }
                }
            }

            /// <summary>
            /// Visits a response operation payload
            /// </summary>
            /// <param name="operation">Operation to visit</param>
            protected override void VisitResponseOperation(HttpResponseData operation)
            {
                var responseOperation = operation as ODataResponse;
                this.Assert.IsNotNull(responseOperation, "Response operation has unexpected type.");

                var observed = this.GetNextObservedElement<ODataResponse>();

                if (responseOperation.RootElement != null)
                {
                    this.Assert.IsNotNull(observed.RootElement, "Expected non-null body on response operation.");

                    this.WrapAccept(responseOperation.RootElement, observed.RootElement);

                    observed.RootElement = this.ContentTypeSpecificNormalize(observed.RootElement, operation.Headers);
                }
            }

            private ODataPayloadElement ContentTypeSpecificNormalize(ODataPayloadElement payloadElement, IDictionary<string, string> batchOperationHeaders)
            {
                string contentType;
                if (batchOperationHeaders.TryGetValue(HttpHeaders.ContentType, out contentType))
                {
                    var normalizer = this.NormalizerSelector.GetNormalizer(contentType);
                    return normalizer.Normalize(payloadElement);
                }

                return payloadElement;
            }

            private TElement GetNextObservedElement<TElement>()
            {
                var observed = this.observedElementStack.Peek();
                this.Assert.IsTrue(typeof(TElement).IsAssignableFrom(observed.GetType()), "Unexpected element type: " + observed.GetType().Name);
                return (TElement)observed;
            }

            private void VisitEnumerable<TElement>(IEnumerable<TElement> expected, IEnumerable<TElement> observed) where TElement : ODataPayloadElement
            {
                this.VisitEnumerable(expected, observed, (e) => e.Accept(this));
            }

            private void VisitEnumerable<TElement>(IEnumerable<TElement> expected, IEnumerable<TElement> observed, Action<TElement> acceptAction)
            {
                if (expected == null && observed == null)
                {
                    return;
                }

                this.Assert.IsFalse(expected == null || observed == null, "One of the lists is null.");
                this.Assert.AreEqual(expected.Count(), observed.Count(), "Count did not match expectation");

                for (int i = 0; i < expected.Count(); i++)
                {
                    this.WrapAccept(expected.ElementAt(i), observed.ElementAt(i), acceptAction);
                }
            }

            private void WrapAccept(ODataPayloadElement expected, ODataPayloadElement observed)
            {
                this.WrapAccept(expected, observed, (e) => e.Accept(this));
            }

            private void WrapAccept<TElement>(TElement expected, TElement observed, Action<TElement> acceptAction)
            {
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
                ExceptionUtilities.CheckArgumentNotNull(observed, "observed");

                this.observedElementStack.Push(observed);
                try
                {
                    acceptAction(expected);
                }
                finally
                {
                    this.observedElementStack.Pop();
                }
            }
        }
    }
}
