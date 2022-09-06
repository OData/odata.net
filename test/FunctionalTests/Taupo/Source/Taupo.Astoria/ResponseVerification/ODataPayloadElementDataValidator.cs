//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementDataValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Visits the payload element given and compares its data to the given query value
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementValidator), "Default")]
    public class ODataPayloadElementDataValidator : IODataPayloadElementValidator
    {
        /// <summary>
        /// Gets or sets the assertion handler
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public StackBasedAssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the odata literal converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets the convention-based link generator to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataConventionBasedLinkGenerator LinkGenerator { get; set; }

        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public IClrToPrimitiveDataTypeConverter PrimitiveDataTypeConverter { get; set; }

        /// <summary>
        /// Gets or sets the test case's query evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryEvaluationStrategy QueryEvaluationStrategy { get; set; }

        /// <summary>
        /// Gets or sets the expected protocol version of the payload
        /// </summary>
        public DataServiceProtocolVersion ExpectedProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the primitive comparer to use
        /// </summary>
        public IQueryScalarValueToClrValueComparer PrimitiveValueComparer { get; set; }

        /// <summary>
        /// Gets or sets the expected payload options
        /// </summary>
        public ODataPayloadOptions ExpectedPayloadOptions { get; set; }

        /// <summary>
        /// Validates a payload with the given root element against the given expected value
        /// </summary>
        /// <param name="rootElement">The root element of the payload</param>
        /// <param name="expectedValue">The expected value</param>
        public void Validate(ODataPayloadElement rootElement, QueryValue expectedValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.CheckArgumentNotNull(expectedValue, "expectedValue");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.Assert.IsNull(expectedValue.EvaluationError, string.Format(CultureInfo.InvariantCulture, "Query evaluation error: {0}", expectedValue.EvaluationError));

            var visitor = new PayloadDataValidatingVisitor(this);
            visitor.Validate(rootElement, expectedValue);
        }

        /// <summary>
        /// Internal visitor for validating the data in the payload
        /// Note that cases where ExceptionUtilities is used in place of the AssertionHandler are where the payload *structure* differs from the 
        /// expected structure. This is not the concern of this visitor, and so is treated as a framework error, not as a failed assertion about the payload
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Will need to be refactored at some point")]
        internal class PayloadDataValidatingVisitor : IODataPayloadElementVisitor
        {
            // because we need to visit two trees in parallel, we store the expected values in a stack as we walk the payload tree
            private Stack<QueryValue> expectedValueStack = new Stack<QueryValue>();
            private Stack<ODataPayloadElement> payloadStack = new Stack<ODataPayloadElement>();
            private ODataPayloadElementDataValidator parent;

            /// <summary>
            /// Initializes a new instance of the PayloadDataValidatingVisitor class
            /// </summary>
            /// <param name="parent">The validator that created this visitor</param>
            internal PayloadDataValidatingVisitor(ODataPayloadElementDataValidator parent)
            {
                ExceptionUtilities.CheckArgumentNotNull(parent, "parent");
                this.parent = parent;
            }

            /// <summary>
            /// Validates a payload with the given root element against the given expected value
            /// </summary>
            /// <param name="rootElement">The root element of the payload</param>
            /// <param name="expected">The expected value</param>
            public void Validate(ODataPayloadElement rootElement, QueryValue expected)
            {
                ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
                ExceptionUtilities.CheckArgumentNotNull(expected, "expected");

                this.expectedValueStack.Push(expected);
                this.payloadStack.Push(rootElement);

                rootElement.Accept(this);
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchRequestPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(BatchRequestPayload payloadElement)
            {
                throw new TaupoNotSupportedException("Batch payloads are not supported");
            }

            /// <summary>
            /// Visits a payload element whose root is a BatchResponsePayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(BatchResponsePayload payloadElement)
            {
                throw new TaupoNotSupportedException("Batch payloads are not supported");
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryStructuralValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a structural instance. Value was: '{0}'", current.ToString());

                this.VisitStructuralInstance(payloadElement, value);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexInstanceCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexInstanceCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.parent.Assert.AreEqual(value.Elements.Count, payloadElement.Count, "Complex instance collection count did not match expectation");
                for (int i = 0; i < value.Elements.Count; i++)
                {
                    this.RecurseWithMessage(payloadElement[i], value.Elements[i], "Complex instance at position {0} did not match expectation", i + 1);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.VerifyTypeName(value, payloadElement.FullTypeName, "Type name did not match expectation for complex multi-value");

                this.parent.Assert.AreEqual(value.Elements.Count, payloadElement.Count, "Complex multi-value count did not match expectation");
                for (int i = 0; i < value.Elements.Count; i++)
                {
                    this.RecurseWithMessage(payloadElement[i], value.Elements[i], "Complex instance at position {0} did not match expectation", i + 1);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexMultiValueProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexMultiValueProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.RecurseWithMessage(payloadElement.Value, value, "Complex multi-value property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is a ComplexProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ComplexProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryStructuralValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a structural instance. Value was: '{0}'", current.ToString());

                this.RecurseWithMessage(payloadElement.Value, value, "Complex property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is a DeferredLink.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(DeferredLink payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                // if the link is deferred, there is no data to validate
                // TODO: add mising verification
                var originalXmlPayloadElementAnnotation = payloadElement.Annotations.OfType<XmlPayloadElementRepresentationAnnotation>().SingleOrDefault();
                if (originalXmlPayloadElementAnnotation != null)
                {
                    // Validate that the namespace used is not the metadata namespace
                    var element = (XElement)originalXmlPayloadElementAnnotation.XmlNodes.Single();
                    this.parent.Assert.AreEqual(element.Name.Namespace.NamespaceName, ODataConstants.DataServicesNamespaceName, "Producing uri in Metadata Namespace is invalid OData");
                }
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyCollectionProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyCollectionProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.RecurseWithMessage(payloadElement.Value, value, "Empty Untyped collection property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyPayload payloadElement)
            {
                // using this.assert.Fail causes code coverage issue
                throw new AssertionFailedException("Payload should not have been empty");
            }

            /// <summary>
            /// Visits a payload element whose root is an EmptyUntypedCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EmptyUntypedCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.parent.Assert.AreEqual(value.Elements.Count, 0, "Collection was unexpectly empty");
            }

            /// <summary>
            /// Visits a payload element whose root is an EntityInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EntityInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryStructuralValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a structural instance. Value was: '{0}'", current.ToString());

                var entityType = value.Type as QueryEntityType;
                ExceptionUtilities.CheckObjectNotNull(entityType, "Value was not an entity instance. Value was: '{0}'", current.ToString());

                if (!payloadElement.IsNull)
                {
                    this.VerifyEntityMetadata(entityType, payloadElement, value);
                }

                foreach (var serviceOperationDescriptor in payloadElement.ServiceOperationDescriptors)
                {
                    this.Visit(serviceOperationDescriptor);
                }

                this.VisitStructuralInstance(payloadElement, value);
            }

            /// <summary>
            /// Visits a payload element whose root is an EntitySetInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(EntitySetInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());
                var entityType = value.Type.ElementType as QueryEntityType;
                ExceptionUtilities.CheckObjectNotNull(entityType, "Value was not a collection of entities. Value was: '{0}'", current.ToString());

                this.parent.Assert.AreEqual(value.Elements.Count, payloadElement.Count, "Entity set count did not match expectation");

                // apply different verification methods based on whether order is predictable.
                if (value.IsSorted)
                {
                    this.CompareSortedCollection(payloadElement, value);
                }
                else
                {
                    this.CompareUnsortedCollection(payloadElement, value);
                }

                // next-link verification now performed by seperate component. See NextLinkResponseVerifier.
            }

            /// <summary>
            /// Visits a payload element whose root is an ExpandedLink.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ExpandedLink payloadElement)
            {
                if (payloadElement.ExpandedElement != null)
                {
                    this.RecurseWithMessage(payloadElement.ExpandedElement, this.expectedValueStack.Peek(), "Expanded item did not match expectation");
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a LinkCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(LinkCollection payloadElement)
            {
                foreach (var link in payloadElement)
                {
                    var deferredLink = link as DeferredLink;
                    this.parent.Assert.IsNotNull(link, "Expected link to be a deferred link from a $ref collection");

                    // There is no expected link right now, call validates payload
                    // from an OData correctness point of view, additional validation will be done at somepoint
                    this.Visit(deferredLink);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NamedStreamProperty.  this will validate the content types and etags
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NamedStreamInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var queryStreamValue = current as AstoriaQueryStreamValue;
                ExceptionUtilities.CheckObjectNotNull(queryStreamValue, "Value was not a stream. Value was: '{0}'", current.ToString());

                using (this.parent.Assert.WithMessage("Named stream '{0}' did not match expectation", payloadElement.Name))
                {
                    this.CompareStreamETag(queryStreamValue, payloadElement.ETag);
                    if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeSelfOrEditLink))
                    {
                        this.parent.Assert.IsFalse(payloadElement.EditLink == null && payloadElement.SourceLink == null, "At least one link (self/edit) should be present in the response payload");
                    }

                    if (payloadElement.EditLink != null)
                    {
                        this.CompareUri(queryStreamValue.EditLink, payloadElement.EditLink, "Edit link did not match");
                        this.parent.Assert.AreEqual(queryStreamValue.ContentType, payloadElement.EditLinkContentType, "Edit link content-type did not match");
                    }

                    if (payloadElement.SourceLink != null)
                    {
                        if (!this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.ConventionallyProducedNamedStreamSelfLink))
                        {
                            this.CompareUri(queryStreamValue.SelfLink, payloadElement.SourceLink, "Self link did not match");
                            this.parent.Assert.AreEqual(queryStreamValue.ContentType, payloadElement.SourceLinkContentType, "Self link content-type did not match");
                        }
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a NavigationProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NavigationPropertyInstance payloadElement)
            {
                // the type in the method signature is fully qualified due to a collision with the EntityModel namespace
                this.RecurseWithMessage(payloadElement.Value, this.expectedValueStack.Peek(), "Navigation property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is a NullPropertyInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(NullPropertyInstance payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                ExceptionUtilities.CheckObjectNotNull(current, "Value was unexpectedly null");
                this.parent.Assert.IsTrue(current.IsNull, string.Format(CultureInfo.InvariantCulture, "Property '{0}' was unexpectedly non-null", payloadElement.Name));
                this.VerifyTypeName(current, payloadElement.FullTypeName, "Type name did not match expectation for null property");
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataErrorPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ODataErrorPayload payloadElement)
            {
                throw new AssertionFailedException("Payload should not have contained an error");
            }

            /// <summary>
            /// Visits a payload element whose root is an ODataInternalExceptionPayload.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ODataInternalExceptionPayload payloadElement)
            {
                throw new AssertionFailedException("Payload should not have contained an internal exception");
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveCollection.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveCollection payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.parent.Assert.AreEqual(value.Elements.Count, payloadElement.Count, "Primitive collection count did not match expectation");
                for (int i = 0; i < value.Elements.Count; i++)
                {
                    this.RecurseWithMessage(payloadElement[i], value.Elements[i], "Primitive value at position {0} did not match expectation", i + 1);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.VerifyTypeName(value, payloadElement.FullTypeName, "Type name did not match expectation for primitive multi-value");

                this.parent.Assert.AreEqual(value.Elements.Count, payloadElement.Count, "Primitive multi-value count did not match expectation");
                for (int i = 0; i < value.Elements.Count; i++)
                {
                    this.RecurseWithMessage(payloadElement[i], value.Elements[i], "Primitive value at position {0} did not match expectation", i + 1);
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveMultiValueProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveMultiValueProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();
                var value = current as QueryCollectionValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a collection. Value was: '{0}'", current.ToString());

                this.RecurseWithMessage(payloadElement.Value, value, "Primitive multi-value property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveProperty.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveProperty payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();

                var value = current as QueryScalarValue;
                ExceptionUtilities.CheckObjectNotNull(value, "Value was not a primitive. Value was: '{0}'", current.ToString());

                this.RecurseWithMessage(payloadElement.Value, value, "Primitive property '{0}' did not match expectation", payloadElement.Name);
            }

            /// <summary>
            /// Visits a payload element whose root is a PrimitiveValue.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(PrimitiveValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

                var current = this.expectedValueStack.Peek();

                var value = current as QueryScalarValue;

                if (value == null)
                {
                    // handle named stream value
                    var streamValue = current as AstoriaQueryStreamValue;
                    ExceptionUtilities.CheckObjectNotNull(streamValue, "Value was not a primitive value or a stream. Value was: '{0}'", current.ToString());

                    // TODO: verify the binary content of the stream is correct
                }
                else
                {
                    this.VerifyTypeName(value, payloadElement.FullTypeName, "Type name did not match expectation for primitive value");

                    if (value.IsNull)
                    {
                        this.parent.Assert.IsTrue(payloadElement.IsNull, "Primitive value unexpectedly non-null");
                    }
                    else
                    {
                        this.parent.Assert.IsFalse(payloadElement.IsNull, "Primitive value unexpectedly null");

                        if (value.Value is DateTime && ((DateTime)value.Value).Kind == DateTimeKind.Local && this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.UseOldDateTimeFormat))
                        {
                            // The old DateTime format cannot represent Local DateTimes, so convert the expected value to Utc
                            value = new QueryScalarValue(value.Type, TimeZoneInfo.ConvertTime((DateTime)value.Value, TimeZoneInfo.Utc), value.EvaluationError, this.parent.QueryEvaluationStrategy);
                        }

                        ExceptionUtilities.CheckObjectNotNull(this.parent.PrimitiveValueComparer, "Cannot compare primitive values without primitive comparer");
                        this.parent.PrimitiveValueComparer.Compare(value, payloadElement.ClrValue, this.parent.Assert);
                    }
                }
            }

            /// <summary>
            /// Visits a payload element whose root is a ResourceCollectionInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ResourceCollectionInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Not implemented.");
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceDocumentInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ServiceDocumentInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Not supported.");
            }

            /// <summary>
            /// Visits a payload element whose root is a ServiceOperationDescriptor.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(ServiceOperationDescriptor payloadElement)
            {
                // TODO: queryvalues don't hold descriptor info, need to update
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            }

            /// <summary>
            /// Visits a payload element whose root is a WorkspaceInstance.
            /// </summary>
            /// <param name="payloadElement">The root node of payload element being visited.</param>
            public void Visit(WorkspaceInstance payloadElement)
            {
                throw new TaupoNotSupportedException("Not implemented.");
            }

            /// <summary>
            /// Gets the expected type name for the given clr type
            /// </summary>
            /// <param name="clrType">The clr type</param>
            /// <param name="converter">The converter to use</param>
            /// <returns>The expected type name</returns>
            internal static string GetExpectedTypeName(Type clrType, IClrToPrimitiveDataTypeConverter converter)
            {
                ExceptionUtilities.CheckObjectNotNull(converter, "Cannot infer edm type from clr type without converter");
                var dataType = converter.ToDataType(clrType);

                ExceptionUtilities.CheckObjectNotNull(dataType, "Could not convert clr type '{0}' to a data type", clrType);
                return dataType.GetEdmTypeName();
            }

            /// <summary>
            /// Gets the expected type name for the given query type
            /// </summary>
            /// <param name="type">The query type</param>
            /// <returns>The expected type name</returns>
            internal string GetExpectedTypeName(QueryType type)
            {
                ExceptionUtilities.CheckArgumentNotNull(type, "type");

                var collection = type as QueryCollectionType;
                if (collection != null)
                {
                    return string.Concat(
                        ODataConstants.BeginMultiValueTypeIdentifier,
                        this.GetExpectedTypeName(collection.ElementType),
                        ODataConstants.EndMultiValueTypeNameIdentifier);
                }

                var complex = type as QueryComplexType;
                if (complex != null)
                {
                    return complex.ComplexType.FullName;
                }

                var entity = type as QueryEntityType;
                if (entity != null)
                {
                    return entity.EntityType.FullName;
                }

                ExceptionUtilities.Assert(type is QueryScalarType, "Type given was not a collection, complex, entity, or primitive type. Type was: '{0}'", type.StringRepresentation);
                var primitive = type as IQueryClrType;
                ExceptionUtilities.CheckObjectNotNull(primitive, "Primitive type did not have a CLR type. Type was: '{0}'", type.StringRepresentation);

                var clrType = Nullable.GetUnderlyingType(primitive.ClrType);
                if (clrType == null)
                {
                    clrType = primitive.ClrType;
                }

                return GetExpectedTypeName(clrType, this.parent.PrimitiveDataTypeConverter);
            }

            private void VisitStructuralInstance(ComplexInstance payloadElement, QueryStructuralValue value)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.CheckArgumentNotNull(value, "value");

                var entityType = value.Type as QueryEntityType;
                var complexType = value.Type as QueryComplexType;
                ExceptionUtilities.Assert(entityType != null || complexType != null, "Value was neither an entity type nor a complex type");

                bool isEntity = entityType != null;
                string errorType = isEntity ? "Entity" : "Complex";

                if (value.IsNull)
                {
                    this.parent.Assert.IsTrue(payloadElement.IsNull, errorType + " instance unexpectedly non-null");
                    return;
                }
                else
                {
                    this.parent.Assert.IsFalse(payloadElement.IsNull, errorType + " instance unexpectedly null");

                    this.VerifyTypeName(value, payloadElement.FullTypeName, errorType + " instance type name did not match expectation.");
                }

                // get all the payload properties, and remove them as we go to detect any extras
                var payloadProperties = payloadElement.Properties.ToList();

                // this is data-driven to deal with open types, but we need to skip over the 'default stream' property if it exists
                foreach (var propertyName in value.MemberNames.Where(m => m != AstoriaQueryStreamType.DefaultStreamPropertyName))
                {
                    var propertyInstance = payloadProperties.SingleOrDefault(p => p.Name == propertyName);
                    this.parent.Assert.IsNotNull(propertyInstance, string.Format(CultureInfo.InvariantCulture, "Could not find property '{0}' in payload", propertyName));
                    payloadProperties.Remove(propertyInstance);

                    var propertyValue = value.GetValue(propertyName);
                    this.RecurseWithMessage(propertyInstance, propertyValue, "{0} instance did not match expectation", errorType);
                }

                string extraPropertyNames = string.Join(", ", payloadProperties.Select(p => '\'' + p.Name + '\'').ToArray());
                this.parent.Assert.IsTrue(payloadProperties.Count == 0, string.Format(CultureInfo.InvariantCulture, "{0} instance contained unexpected properties: {1}", errorType, extraPropertyNames));
            }

            /// <summary>
            /// Helper method for recursively validating elements and catching exceptions from the assertion handler
            /// </summary>
            /// <param name="element">The element to validate</param>
            /// <param name="value">The expected value</param>
            /// <returns>Null if not exception was caught, or the caught exception</returns>
            private TestFailedException RecurseAndCatch(ODataPayloadElement element, QueryValue value)
            {
                this.expectedValueStack.Push(value);
                this.payloadStack.Push(element);
                try
                {
                    this.Recurse(element, value);
                    return null;
                }
                catch (TestFailedException e)
                {
                    return e;
                }
                finally
                {
                    this.expectedValueStack.Pop();
                    this.payloadStack.Pop();
                }
            }

            /// <summary>
            /// Helper method for recursively validating elements and wrapping exceptions from the assertion handler.
            /// If validation fails, the exception type will be maintained, but it will be wrapped in an exception with the given message.
            /// </summary>
            /// <param name="element">The element to validate</param>
            /// <param name="value">The expected value</param>
            /// <param name="errorMessage">The error message template to wrap any exceptions in</param>
            /// <param name="args">The arguments to the error message template</param>
            private void RecurseWithMessage(ODataPayloadElement element, QueryValue value, string errorMessage, params object[] args)
            {
                using (this.parent.Assert.WithMessage(errorMessage, args))
                {
                    this.Recurse(element, value);
                }
            }

            private void Recurse(ODataPayloadElement element, QueryValue value)
            {
                this.expectedValueStack.Push(value);
                this.payloadStack.Push(element);
                try
                {
                    element.Accept(this);
                }
                finally
                {
                    this.expectedValueStack.Pop();
                    this.payloadStack.Pop();
                }
            }

            /// <summary>
            /// verification method to verify entity collection with predictable order.
            /// </summary>
            /// <param name="payloadElement">entity set instance</param>
            /// <param name="value">expected collection value</param>
            private void CompareSortedCollection(EntitySetInstance payloadElement, QueryCollectionValue value)
            {
                for (int i = 0; i < value.Elements.Count; i++)
                {
                    this.RecurseWithMessage(payloadElement[i], value.Elements[i], "Element at position {0} did not match expectation", i + 1);
                }
            }

            /// <summary>
            /// verification method to verify entity collection without verifying order.
            /// </summary>
            /// <param name="payloadElement">entity set instance</param>
            /// <param name="value">expected collection value</param>
            private void CompareUnsortedCollection(EntitySetInstance payloadElement, QueryCollectionValue value)
            {
                for (int i = 0; i < payloadElement.Count; i++)
                {
                    var actual = payloadElement[i];
                    ExceptionUtilities.CheckObjectNotNull(actual, "Element at position {0} was unexpectedly null", i);

                    bool match = false;
                    foreach (var expected in value.Elements)
                    {
                        var ex = this.RecurseAndCatch(actual, expected);
                        if (ex == null)
                        {
                            match = true;
                            break;
                        }
                    }

                    this.parent.Assert.IsTrue(match, "Element at position {0} did not match any expected value", i + 1);
                }
            }

            /// <summary>
            /// Verify ETag and Id values of given payload element.
            /// </summary>
            /// <param name="entityType">type of the element</param>
            /// <param name="payloadElement">payload element to verify</param>
            /// <param name="value">expected values</param>
            private void VerifyEntityMetadata(QueryEntityType entityType, EntityInstance payloadElement, QueryStructuralValue value)
            {
                if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeETags))
                {
                    if (entityType.EntityType.HasETag())
                    {
                        // TODO: are ETags always based on property values?
                        var expectedETag = this.parent.LiteralConverter.ConstructWeakETag(value);
                        this.parent.Assert.AreEqual(expectedETag, payloadElement.ETag, "Entity's ETag did not match");
                    }
                    else
                    {
                        this.parent.Assert.IsNull(payloadElement.ETag, "Entity should not have had an ETag");
                    }
                }

                if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeEntityIdentifier))
                {
                    this.parent.Assert.IsNotNull(payloadElement.Id, "Entity's ID unexpectedly null");

                    if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.UseConventionBasedIdentifiers))
                    {
                        var expectedId = this.parent.LinkGenerator.GenerateEntityId(value);

                        this.parent.Assert.AreEqual(expectedId, payloadElement.Id, "Entity's ID did not match");
                    }
                }
                else
                {
                    this.parent.Assert.IsNull(payloadElement.Id, "Entity's ID unexpectedly non-null");
                }

                if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.UseConventionBasedLinks))
                {
                    var expectedEditLink = this.parent.LinkGenerator.GenerateEntityEditLink(value);
                    this.CompareUri(expectedEditLink, payloadElement.EditLink, "Entity's edit-link did not match");
                }

                if (entityType.EntityType.GetBaseTypesAndSelf().Any(t => t.HasStream()))
                {
                    using (this.parent.Assert.WithMessage("Entity's stream metadata did not match"))
                    {
                        var defaultStreamValue = value.GetDefaultStreamValue();
                        this.parent.Assert.AreEqual(defaultStreamValue.ContentType, payloadElement.StreamContentType, "Content type did not match");

                        this.CompareStreamETag(defaultStreamValue, payloadElement.StreamETag);

                        if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeMediaResourceEditLinks))
                        {
                            this.CompareUri(defaultStreamValue.EditLink, payloadElement.StreamEditLink, "Edit link did not match");
                        }
                        else
                        {
                            this.parent.Assert.IsNull(payloadElement.StreamEditLink, "Edit link unexpectedly non-null");
                        }

                        if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeMediaResourceSourceLinks))
                        {
                            this.CompareUri(defaultStreamValue.SelfLink, payloadElement.StreamSourceLink, "Source link did not match");
                        }
                        else
                        {
                            this.parent.Assert.IsNull(payloadElement.StreamSourceLink, "Source link unexpectedly non-null");
                        }
                    }
                }
            }

            private void CompareUri(string expected, string actual, string message)
            {
                Uri expectedUri = null;
                if (expected != null)
                {
                    expectedUri = new Uri(expected, UriKind.RelativeOrAbsolute);
                }

                this.CompareUri(expectedUri, actual, message);
            }

            private void CompareUri(Uri expectedUri, string actual, string message)
            {
                string currentBaseUri = UriHelpers.ConcatenateUriSegments(this.payloadStack.SelectMany(p => p.Annotations.OfType<XmlBaseAnnotation>()).Select(b => b.Value).ToArray());

                if (expectedUri == null)
                {
                    this.parent.Assert.IsNull(actual, message);
                    return;
                }

                string expected = expectedUri.OriginalString;
                if (!string.IsNullOrEmpty(currentBaseUri) && expected.StartsWith(currentBaseUri, StringComparison.Ordinal))
                {
                    expected = expected.Substring(currentBaseUri.Length);
                }

                this.parent.Assert.AreEqual(expected, actual, message);
            }

            private void VerifyTypeName(QueryValue value, string actualTypeName, string message)
            {
                var type = value.Type;
                string expectedTypeName = this.GetExpectedTypeName(type);

                bool expectTypeName = false;
                if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeTypeNames))
                {
                    expectTypeName = true;

                    // if type names are omitted within multivalues, we need to figure out if we're within one
                    if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.OmitTypeNamesWithinMultiValues))
                    {
                        bool inMultiValue = false;
                        if (this.payloadStack.Count > 2)
                        {
                            var elementType = this.payloadStack.ElementAt(2).ElementType;
                            inMultiValue = elementType == ODataPayloadElementType.ComplexMultiValueProperty || elementType == ODataPayloadElementType.PrimitiveMultiValueProperty;
                        }

                        if (inMultiValue)
                        {
                            var multivalue = (ITypedValue)this.payloadStack.ElementAt(1);
                            string multiValueElementName;
                            if (ODataUtilities.TryGetMultiValueElementTypeName(multivalue.FullTypeName, out multiValueElementName))
                            {
                                expectTypeName = expectedTypeName != multiValueElementName;
                            }
                        }
                    }

                    // if this is a primitive type, it may not have a type name
                    var primitiveType = type as QueryScalarType;
                    if (expectTypeName && primitiveType != null)
                    {
                        if (this.payloadStack.Count == 1)
                        {
                            // top-level raw $value payloads don't have a place to write the type name
                            expectTypeName = false;
                        }
                        else if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.OmitTypeNamesForAllPrimitives))
                        {
                            // no type name is expected for any un-structured primitive (JSON does not have a place for this, for instance)
                            // TODO: have a knob for services which choose not to write the type for spatial
                            expectTypeName = false;
                        }
                        else if (this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.OmitTypeNamesForStrings))
                        {
                            // string is the default type and is not always specified
                            var clrType = primitiveType as IQueryClrType;
                            ExceptionUtilities.CheckObjectNotNull(clrType, "Primitive type did not have a clr type. Type was: {0}", primitiveType);
                            expectTypeName = clrType.ClrType != typeof(string);
                        }
                    }

                    if (type is QueryClrSpatialType)
                    {
                        expectTypeName |= this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.AlwaysIncludeTypeNamesForSpatialPrimitives);
                    }
                }

                if (expectTypeName)
                {
                    // special cases for null values
                    var typedValue = this.payloadStack.Peek() as TypedValue;
                    if (typedValue != null && typedValue.IsNull)
                    {
                        if (!this.parent.ExpectedPayloadOptions.HasFlag(ODataPayloadOptions.IncludeTypeNamesForNullValues))
                        {
                            // if nulls are not typed, then do not expect it
                            expectTypeName = false;
                        }
                        else if (this.payloadStack.Count < 3)
                        {
                            // Astoria-ODataLib-Integration: For null top-level properties, ODataLib does not write the type attribute
                            expectTypeName = false;
                        }
                        else if (value.IsDynamicPropertyValue())
                        {
                            // dynamic nulls do not have type information, but if this is a sub-property of a complex type, then it will have a type name
                            var parentElement = this.payloadStack.ElementAt(2);
                            if (parentElement.ElementType == ODataPayloadElementType.EntityInstance)
                            {
                                expectTypeName = false;
                            }
                        }
                    }
                }

                if (expectTypeName)
                {
                    this.parent.Assert.AreEqual(expectedTypeName, actualTypeName, message);
                }
                else
                {
                    this.parent.Assert.IsNull(actualTypeName, "{0}. Unexpected type name '{1}' found.", message, actualTypeName);
                }
            }

            private void CompareStreamETag(AstoriaQueryStreamValue queryStreamValue, string actualETag)
            {
                this.parent.Assert.AreEqual(queryStreamValue.GetExpectedETag(), actualETag, "ETag did not match");
            }
        }
    }
}
