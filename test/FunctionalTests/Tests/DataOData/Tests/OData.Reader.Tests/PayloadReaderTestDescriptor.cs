//---------------------------------------------------------------------
// <copyright file="PayloadReaderTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Batch;
    using Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight;
    using Microsoft.OData.Edm.Csdl;

    #endregion Namespaces

    /// <summary>
    /// Reader test descriptor for tests which specify the input as ODataPayloadElement.
    /// </summary>
    public class PayloadReaderTestDescriptor : ReaderTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public EntityModelSchemaSerializer EntityModelSchemaSerializer { get; set; }

            [InjectDependency(IsRequired = true)]
            public IPayloadElementToXmlConverter PayloadElementToXmlConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings ExpectedResultSettings { get; set; }

            [InjectDependency(IsRequired = true)]
            public BatchReaderTestExpectedResult.BatchReaderTestExpectedResultSettings BatchExpectedResultSettings { get; set; }

            [InjectDependency(IsRequired = true)]
            public TextValueProtocolFormatStrategy TextValuePayloadElementConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public BinaryValueProtocolFormatStrategy BinaryValuePayloadElementConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public IPayloadElementToJsonConverter PayloadElementToJsonConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public IPayloadElementToJsonLightConverter PayloadElementToJsonLightConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public IPayloadTransformFactory PayloadTransformFactory { get; set; }

            [InjectDependency(IsRequired = true)]
            public IBatchPayloadSerializer BatchSerializer { get; set; }
        }

        /// <summary>
        /// All the metadata information to be used when reading the actual payload.
        /// </summary>
        public sealed class ReaderMetadata : IReaderMetadata
        {
            /// <summary>An empty set of reader metadata.</summary>
            public static readonly ReaderMetadata None = new ReaderMetadata();

            /// <summary>The type to use when reading payloads to specify as the expected type for the reader.</summary>
            private readonly IEdmTypeReference expectedType;

            /// <summary>The entity set to use when reading entry or feed payloads.</summary>
            private readonly IEdmEntitySet entitySet;

            /// <summary>The structural property to use when reading property payloads.</summary>
            private readonly IEdmStructuralProperty structuralProperty;

            /// <summary>The navigation property to use when reading entity reference link payloads.</summary>
            private readonly IEdmNavigationProperty navigationProperty;

            /// <summary>The function import metadata used when reading a collection or parameter payload, 
            /// or when reading a property payload produced by an operation.</summary>
            private readonly IEdmOperationImport functionImport;

            /// <summary>
            /// Constructor
            /// </summary>
            private ReaderMetadata()
            {
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="expectedType">The expected type reader metadata.</param>
            public ReaderMetadata(IEdmTypeReference expectedType)
            {
                this.expectedType = expectedType;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="structuralProperty">The expected structural property reader metadata.</param>
            public ReaderMetadata(IEdmStructuralProperty structuralProperty)
            {
                this.expectedType = structuralProperty.Type;
                this.structuralProperty = structuralProperty;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="model">The model used to resolve the metadata.</param>
            /// <param name="payloadElement">The payload element to get the reader metadata for.</param>
            public ReaderMetadata(IEdmModel model, ODataPayloadElement payloadElement)
            {
                var expectedTypeAnnotation = payloadElement.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();

                // NOTE: we don't require a model for the computation of the expected type (since the expected one might be a primitive type).
                this.expectedType = GetExpectedType(expectedTypeAnnotation, model);

                // We need a model for all the other expected reader metadata
                if (model == null)
                {
                    // If the annotation specified some model dependent data (basically anything but primitive expected type)
                    // and we don't have a model, we wouldn't be able to correctly represent it here (since we need the model to resolve these)
                    // and thus we should not pass in the expected type alone, as that would be changing the intent of the annotation.
                    if (expectedTypeAnnotation != null &&
                        (expectedTypeAnnotation.EntitySet != null ||
                         expectedTypeAnnotation.EdmEntitySet != null ||
                         expectedTypeAnnotation.FunctionImport != null ||
                         expectedTypeAnnotation.ProductFunctionImport != null ||
                         expectedTypeAnnotation.MemberProperty != null ||
                         expectedTypeAnnotation.EdmProperty != null ||
                         expectedTypeAnnotation.NavigationProperty != null ||
                         expectedTypeAnnotation.EdmNavigationProperty != null ||
                         expectedTypeAnnotation.OpenMemberPropertyName != null ||
                         expectedTypeAnnotation.OwningType != null ||
                         expectedTypeAnnotation.EdmOwningType != null))
                    {
                        this.expectedType = null;
                    }

                    return;
                }

                ODataPayloadElementType elementType = payloadElement.ElementType;
                switch (elementType)
                {
                    case ODataPayloadElementType.EntityInstance:    // fall through
                    case ODataPayloadElementType.EntitySetInstance:
                        this.entitySet = GetExpectedEntitySet(expectedTypeAnnotation, model, payloadElement);
                        break;

                    case ODataPayloadElementType.DeferredLink:      // fall through
                    case ODataPayloadElementType.LinkCollection:
                        this.navigationProperty = GetExpectedNavigationProperty(expectedTypeAnnotation, model);
                        break;

                    case ODataPayloadElementType.PrimitiveMultiValueProperty:   // fall through
                    case ODataPayloadElementType.PrimitiveProperty:             // fall through
                    case ODataPayloadElementType.ComplexProperty:               // fall through
                    case ODataPayloadElementType.ComplexMultiValueProperty:     // fall through
                    case ODataPayloadElementType.EmptyCollectionProperty:
                        this.structuralProperty = GetExpectedStructuralProperty(expectedTypeAnnotation, model);
                        this.functionImport = GetExpectedFunctionImport(expectedTypeAnnotation, model);
                        break;

                    case ODataPayloadElementType.ComplexInstanceCollection:     // fall through
                    case ODataPayloadElementType.PrimitiveCollection:           // fall through
                    case ODataPayloadElementType.EmptyUntypedCollection:
                        this.functionImport = GetExpectedFunctionImport(expectedTypeAnnotation, model);
                        break;

                    case ODataPayloadElementType.ComplexInstance:
                        // NOTE: this is how we model parameter payloads
                        this.functionImport = GetExpectedFunctionImport(expectedTypeAnnotation, model);
                        break;
                }
            }

            /// <summary>
            /// The type to use when reading payloads to specify as the expected type for the reader.
            /// </summary>
            public IEdmTypeReference ExpectedType { get { return this.expectedType; } }

            /// <summary>
            /// The entity set to use when reading entry or feed payloads.
            /// </summary>
            public IEdmEntitySet EntitySet { get { return this.entitySet; } }

            /// <summary>
            /// The structural property to use when reading property payloads.
            /// </summary>
            public IEdmStructuralProperty StructuralProperty { get { return this.structuralProperty; } }

            /// <summary>
            /// The navigation property to use when reading entity reference link payloads.
            /// </summary>
            public IEdmNavigationProperty NavigationProperty { get { return this.navigationProperty; } }

            /// <summary>
            /// The function import metadata used when reading a collection or parameter payload, 
            /// or when reading a property payload produced by an operation.
            /// </summary>
            public IEdmOperationImport FunctionImport { get { return this.functionImport; } }

            /// <summary>
            /// Returns the function import for a parameters payload.
            /// </summary>
            /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
            /// <param name="model">The model to get the function import from.</param>
            /// <returns>Returns the function import for a parameters payload.</returns>
            private static IEdmOperationImport GetExpectedFunctionImport(ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, IEdmModel model)
            {
                ExceptionUtilities.Assert(model != null, "model != null");

                if (expectedTypeAnnotation != null)
                {
                    if (expectedTypeAnnotation.ProductFunctionImport != null)
                    {
                        return expectedTypeAnnotation.ProductFunctionImport;
                    }
                    FunctionImport functionImport = expectedTypeAnnotation.FunctionImport;
                    if (functionImport != null)
                    {
                        var container = model.FindEntityContainer(functionImport.Container.FullName);
                        var functionImports = container.FindOperationImports(functionImport.Name);
                        if (functionImports != null && functionImports.Any())
                        {
                            // Note that we don't support overload for Actions. Single() will throw if the model is invalid.
                            return functionImports.Single();
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Returns the expected type for the given payload element.
            /// </summary>
            /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
            /// <param name="model">The model to get the type from.</param>
            /// <returns>The expected type for the specified payload element.</returns>
            private static IEdmTypeReference GetExpectedType(ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, IEdmModel model)
            {
                if (expectedTypeAnnotation != null)
                {
                    if (expectedTypeAnnotation.EdmExpectedType != null)
                    {
                        return expectedTypeAnnotation.EdmExpectedType;
                    }
                    if (expectedTypeAnnotation.ExpectedType != null)
                    {
                        DataType expectedDataType = expectedTypeAnnotation.ExpectedType;
                        return EdmModelUtils.ResolveEntityModelSchemaType(model, expectedDataType);
                    }
                }

                return null;
            }

            /// <summary>
            /// Returns the entity set for the given payload element (only for entries and feeds).
            /// </summary>
            /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
            /// <param name="model">The model to get the type from.</param>
            /// <param name="payloadElement">The payload element to get the expected type for.</param>
            /// <returns>The expected type for the specified payload element.</returns>
            private static IEdmEntitySet GetExpectedEntitySet(ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, IEdmModel model, ODataPayloadElement payloadElement)
            {
                ExceptionUtilities.Assert(model != null, "model != null");
                ExceptionUtilities.Assert(payloadElement != null, "payloadElement != null");

                if (payloadElement.GetAnnotation<IgnoreEntitySetAnnotation>() != null)
                {
                    // Entity set information is explicitly ignored
                    return null;
                }

                if (expectedTypeAnnotation != null)
                {
                    if (expectedTypeAnnotation.EdmEntitySet != null)
                    {
                        return expectedTypeAnnotation.EdmEntitySet;
                    }
                    EntitySet entitySet = expectedTypeAnnotation.EntitySet;
                    if (entitySet != null)
                    {
                        return model.EntityContainersAcrossModels().Select(m => m.FindEntitySet(entitySet.Name)).FirstOrDefault(s => s != null);
                    }
                }

                EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                if (typeAnnotation != null)
                {
                    var edmEntityType = typeAnnotation.EdmModelType;
                    return model.EntityContainersAcrossModels().First().EntitySets().SingleOrDefault(es => es.EntityType().FullName() == edmEntityType.FullName());
                }

                return null;
            }

            /// <summary>
            /// Returns the navigation property for the given payload element (only for entity reference links).
            /// </summary>
            /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
            /// <param name="model">The model to get the navigation property from.</param>
            /// <returns>The expected navigation property for the specified payload element.</returns>
            private static IEdmNavigationProperty GetExpectedNavigationProperty(ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, IEdmModel model)
            {
                ExceptionUtilities.Assert(model != null, "model != null");

                if (expectedTypeAnnotation != null)
                {
                    if (expectedTypeAnnotation.EdmNavigationProperty != null)
                    {
                        return expectedTypeAnnotation.EdmNavigationProperty as IEdmNavigationProperty;
                    }
                    NavigationProperty expectedNavigationProperty = expectedTypeAnnotation.NavigationProperty;
                    if (expectedNavigationProperty != null)
                    {
                        NamedStructuralType expectedOwningType = expectedTypeAnnotation.OwningType;
                        ExceptionUtilities.Assert(expectedOwningType != null, "Need an owning type if a navigation property is specified.");

                        IEdmEntityType owningEntityType = model.FindType(expectedOwningType.FullName) as IEdmEntityType;
                        ExceptionUtilities.Assert(owningEntityType != null, "Did not find expected entity type in the model.");

                        IEdmNavigationProperty navigationProperty = owningEntityType.FindProperty(expectedNavigationProperty.Name) as IEdmNavigationProperty;
                        ExceptionUtilities.Assert(navigationProperty != null, "Did not find expected navigation property in the model.");

                        return navigationProperty;
                    }
                }

                return null;
            }

            /// <summary>
            /// Returns the structural property for the given payload element (only for properties).
            /// </summary>
            /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
            /// <param name="model">The model to get the structural property from.</param>
            /// <returns>The expected structural property for the specified payload element.</returns>
            private static IEdmStructuralProperty GetExpectedStructuralProperty(ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, IEdmModel model)
            {
                ExceptionUtilities.Assert(model != null, "model != null");

                if (expectedTypeAnnotation != null)
                {
                    if (expectedTypeAnnotation.EdmProperty != null)
                    {
                        return expectedTypeAnnotation.EdmProperty as IEdmStructuralProperty;
                    }
                    MemberProperty expectedStructuralProperty = expectedTypeAnnotation.MemberProperty;
                    if (expectedStructuralProperty != null)
                    {
                        NamedStructuralType expectedOwningType = expectedTypeAnnotation.OwningType;
                        ExceptionUtilities.Assert(expectedOwningType != null, "Need an owning type if a structural property is specified.");

                        IEdmStructuredType owningType = model.FindType(expectedOwningType.FullName) as IEdmStructuredType;
                        ExceptionUtilities.Assert(owningType != null, "Did not find expected structured type in the model.");

                        IEdmStructuralProperty structuralProperty = owningType.FindProperty(expectedStructuralProperty.Name) as IEdmStructuralProperty;
                        ExceptionUtilities.Assert(structuralProperty != null, "Did not find expected structural property in the model.");

                        return structuralProperty;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        protected readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public PayloadReaderTestDescriptor(Settings settings)
        {
            this.settings = settings;
            this.PayloadDescriptor = new PayloadTestDescriptor();

            var nullFunction = (Func<ODataPayloadElement, ODataPayloadElement>)null;
            this.PayloadNormalizers = new List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>>
            {
                (tc) => tc.Format == ODataFormat.Json ? (payloadElement) => JsonLightPayloadElementNormalizer.Normalize(payloadElement, tc) : nullFunction,

            };

            var nullAction = (Action<PayloadReaderTestDescriptor>)null;
            this.TestDescriptorNormalizers = new List<Func<ReaderTestConfiguration, Action<PayloadReaderTestDescriptor>>>
             {
                 (tc) => tc.Format == ODataFormat.Json ? (descriptor) => JsonLightPayloadElementFixup.Fixup(descriptor) : nullAction,
             };

            this.ExpectedResultNormalizers = new List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>>
            {
                (tc) => nullFunction,
                (tc) => tc.Format == ODataFormat.Json ? RemoveCollectionNameAnnotationForCollectionPayloadElementVisitor.Visit : nullFunction,
                (tc) => tc.Format == ODataFormat.Json ? (payloadElement) => JsonLightExpectedPayloadElementNormalizer.Normalize(payloadElement, tc) : nullFunction,
            };
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other payload test descriptor to copy</param>
        public PayloadReaderTestDescriptor(PayloadReaderTestDescriptor other)
            : base(other)
        {
            this.settings = other.settings;
            this.PayloadDescriptor = new PayloadTestDescriptor(other.PayloadDescriptor);
            this.ExpectedResultPayloadElement = other.ExpectedResultPayloadElement;
            this.ExpectedException = other.ExpectedException;
            this.PayloadNormalizers = new List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>>(other.PayloadNormalizers);
            this.ExpectedResultNormalizers = new List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>>(other.ExpectedResultNormalizers);
            this.UrlResolver = other.UrlResolver;
            this.TestMessageWrapper = other.TestMessageWrapper;
            this.IgnorePropertyOrder = other.IgnorePropertyOrder;
            this.TestDescriptorNormalizers = new List<Func<ReaderTestConfiguration, Action<PayloadReaderTestDescriptor>>>(other.TestDescriptorNormalizers);
        }

        /// <summary>
        /// true if this test descriptor was produced via a payload generator; otherwise false.
        /// </summary>
        public bool IsGeneratedPayload
        {
            get
            {
                return this.PayloadDescriptor.IsGeneratedPayload;
            }
            set
            {
                this.PayloadDescriptor.IsGeneratedPayload = value;
            }
        }

        /// <summary>
        /// The description of the payload and model to run the test against
        /// </summary>
        public PayloadTestDescriptor PayloadDescriptor { get; set; }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// Can be null in which case the payload will execute without metadata.
        /// </summary>
        public IEdmModel PayloadEdmModel
        {
            get
            {
                return this.PayloadDescriptor.PayloadEdmModel;
            }
            set
            {
                this.PayloadDescriptor.PayloadEdmModel = value;
            }
        }

        /// <summary>
        /// The payload element to write as the input for the reader.
        /// And if the expected result is not specified differently, this will also be the expected result of the reader.
        /// </summary>
        public ODataPayloadElement PayloadElement
        {
            get
            {
                return this.PayloadDescriptor.PayloadElement;
            }
            set
            {
                this.PayloadDescriptor.PayloadElement = value;
            }
        }

        /// <summary>
        /// A func to create the payload element to compare the results to based on the current test configuration. If this is null the PayloadElement will be used.
        /// </summary>
        public Func<ReaderTestConfiguration, ODataPayloadElement> ExpectedResultPayloadElement { get; set; }

        /// <summary>
        /// List of funcs which can normalize the payload element.
        /// Each item in the list is a func which get the test configuration, if it needs to normalize the payload element for that test configuration
        /// it should return another func which gets the payload element and can either modify it or return a new one.
        /// </summary>
        public List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>> PayloadNormalizers { get; set; }

        /// <summary>
        /// List of funcs that can normalize the test descriptor, based on the test configuration.
        /// </summary>
        public List<Func<ReaderTestConfiguration, Action<PayloadReaderTestDescriptor>>> TestDescriptorNormalizers { get; set; }

        /// <summary>
        /// List of funcs which can normalize the expected result element.
        /// Each item in the list is a func which get the test configuration, if it needs to normalize the payload for that test configuration
        /// it should return another func which gets the expected result payload element and can either modify it or return a new one.
        /// </summary>
        public List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>> ExpectedResultNormalizers { get; set; }

        /// <summary>
        /// The expected exception if the test is expected to fail.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// Optional URL resolver to put on the message.
        /// </summary>
        public IODataPayloadUriConverter UrlResolver { get; set; }

        /// <summary>true to ignore the order of properties in the payload during comparison; otherwise false.</summary>
        public bool IgnorePropertyOrder { get; set; }

        /// <summary>
        /// Set to override the PayloadTransformFactory's ApplyTransform property value.
        /// </summary>
        public bool? ApplyPayloadTransformations { get; set; }

        /// <summary>
        /// Func which can wrap the test message before it's used by the test.
        /// </summary>
        public Func<TestMessage, TestMessage> TestMessageWrapper { get; set; }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public override ODataPayloadKind PayloadKind
        {
            get
            {
                return this.PayloadDescriptor.PayloadKind;
            }

            set
            {
                this.PayloadDescriptor.PayloadKind = value;
            }
        }

        /// <summary>
        /// Returns description of the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test. Used for debugging.</returns>
        public override string ToString()
        {
            string result = base.ToString();
            if (this.PayloadElement != null)
            {
                result += "\r\nPayload element: " + this.PayloadElement.StringRepresentation;
            }

            if (this.PayloadEdmModel != null)
            {
                // There's no short way of serializing the payload model. Instead if the test fails the model
                // will be serialized as CSDL and dumped to the test output.
                result += "\r\nPayload model present";
            }

            return result;
        }

        /// <summary>
        /// Creates a copy of this PayloadReaderTestDescriptor
        /// </summary>
        public override object Clone()
        {
            return new PayloadReaderTestDescriptor(this);
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
        {
            return TestReaderUtils.CreateInputMessage(testConfiguration, this, this.settings, this.ApplyPayloadTransformations);
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            ReaderTestExpectedResult expectedResult = base.GetExpectedResult(testConfiguration);
            if (expectedResult == null)
            {
                IEdmModel model = this.GetMetadataProvider(testConfiguration);
                ReaderMetadata readerMetadata = new ReaderMetadata(model, this.PayloadElement);

                // First use the input payload as the expected result.
                ODataPayloadElement expectedResultPayloadElement = this.PayloadElement;

                // If we have an override specifically for expected result, use that instead.
                if (this.ExpectedResultPayloadElement != null)
                {
                    expectedResultPayloadElement = this.ExpectedResultPayloadElement(testConfiguration);
                }

                if (expectedResultPayloadElement.ElementType == ODataPayloadElementType.BatchRequestPayload || expectedResultPayloadElement.ElementType == ODataPayloadElementType.BatchResponsePayload)
                {
                    if (this.ExpectedResultNormalizers.Count > 0)
                    {
                        expectedResultPayloadElement = new BatchPayloadExpectedResultNormalizer(
                            testConfiguration,
                            this.ApplyExpectedResultNormalizers).Normalize(expectedResultPayloadElement);
                    }

                    IEdmModel payloadModel = this.GetMetadataProvider(testConfiguration);
                    return new BatchReaderTestExpectedResult(this.settings.BatchExpectedResultSettings)
                    {
                        ExpectedBatchPayload = expectedResultPayloadElement,
                        PayloadModel = payloadModel,
                        ExpectedException = this.ExpectedException,
                    };
                }
                else
                {
                    expectedResultPayloadElement = this.ApplyExpectedResultNormalizers(expectedResultPayloadElement, testConfiguration);
                }

                return new PayloadReaderTestExpectedResult(this.settings.ExpectedResultSettings)
                {
                    ExpectedPayloadElement = expectedResultPayloadElement,
                    ExpectedException = this.ExpectedException,
                    ReaderMetadata = readerMetadata,
                    IgnorePropertyOrder = this.IgnorePropertyOrder,
                };
            }
            else
            {
                PayloadReaderTestExpectedResult payloadReaderTestExpectedResult = expectedResult as PayloadReaderTestExpectedResult;
                if (payloadReaderTestExpectedResult != null && payloadReaderTestExpectedResult.ReaderMetadata == null)
                {
                    IEdmModel model = this.GetMetadataProvider(testConfiguration);
                    payloadReaderTestExpectedResult.ReaderMetadata = new ReaderMetadata(model, this.PayloadElement);
                }

                return expectedResult;
            }
        }

        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected override bool ShouldSkipForTestConfiguration(ReaderTestConfiguration testConfiguration)
        {
            if (this.PayloadDescriptor.SkipTestConfiguration != null && this.PayloadDescriptor.SkipTestConfiguration(testConfiguration))
            {
                return true;
            }
            else if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets The model to use for the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The model to use for the test.</returns>
        protected override IEdmModel GetMetadataProvider(ReaderTestConfiguration testConfiguration)
        {
            IEdmModel model = base.GetMetadataProvider(testConfiguration);

            if (model == null)
            {
                // Try to set the cached model if it is null. If the PayloadEdmModel is null cached model will stay null.
                if (this.cachedModel == null)
                {
                    this.cachedModel = this.PayloadEdmModel;
                }

                if (this.cachedModel != null)
                {
                    if (this.Annotations != null)
                    {
                        this.cachedModel = EdmModelBuilder.BuildAnnotationModel(this.Annotations, this.cachedModel);
                    }
                }

                model = this.cachedModel;
            }

            return model;
        }

        /// <summary>
        /// If overridden dumps the content of an input message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the input message.</returns>
        protected override string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
        {
            byte[] payload = TestReaderUtils.GetPayload(testConfiguration, this.PayloadNormalizers, this.settings, this.PayloadElement);
            return Encoding.UTF8.GetString(payload, 0, payload.Length);
        }

        /// <summary>
        /// If overriden dumps additional description of the test descriptor for the specified testConfiguration.
        /// This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>String description of the test.</returns>
        protected override string DumpAdditionalTestDescriptions(ReaderTestConfiguration testConfiguration)
        {
            StringBuilder result = new StringBuilder(base.DumpAdditionalTestDescriptions(testConfiguration));

            if (this.PayloadEdmModel != null)
            {
                result.AppendLine();
                result.AppendLine("PayloadModel CSDL:");

                List<StringBuilder> stringBuilders = new List<StringBuilder>();
                List<XmlWriter> xmlWriters = new List<XmlWriter>();
                IEnumerable<EdmError> errors;
                this.PayloadEdmModel.SetEdmVersion(Microsoft.OData.Edm.EdmConstants.EdmVersionLatest);
                this.PayloadEdmModel.TryWriteSchema(
                    s =>
                    {
                        stringBuilders.Add(new StringBuilder());
                        xmlWriters.Add(XmlWriter.Create(stringBuilders.Last()));

                        return xmlWriters.Last();
                    }, out errors);

                for (int i = 0; i < stringBuilders.Count; i++)
                {
                    xmlWriters[i].Close();
                }

                foreach (var sb in stringBuilders)
                {
                    result.AppendLine(sb.ToString());
                }

                if (this.Annotations != null)
                {
                    result.AppendLine();
                    result.AppendLine("Annotations:");
                    string annotationSchema =
                        "<Schema Namespace='TestModelStandardAnnotations' xmlns='http://docs.oasis-open.org/odata/ns/edm'>" +
                            this.Annotations +
                        "</Schema>";

                    result.AppendLine(XElement.Parse(annotationSchema).ToString());
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Applies expected result normalizers to the specified payload element.
        /// </summary>
        /// <param name="expectedResultPayloadElement">The payload element to apply the normalizers to.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result after normalization.</returns>
        private ODataPayloadElement ApplyExpectedResultNormalizers(ODataPayloadElement expectedResultPayloadElement, ReaderTestConfiguration testConfiguration)
        {
            // If we have some normalizers and some of them do apply to this test configuration
            // make a copy and apply the normalizer.
            if (this.ExpectedResultNormalizers != null)
            {
                ODataPayloadElement expectedResultPayloadElementCopy = null;
                foreach (var getNormalizerFunc in this.ExpectedResultNormalizers)
                {
                    var normalizer = getNormalizerFunc(testConfiguration);
                    if (normalizer != null)
                    {
                        if (expectedResultPayloadElementCopy == null)
                        {
                            expectedResultPayloadElementCopy = expectedResultPayloadElement.DeepCopy();
                        }

                        expectedResultPayloadElementCopy = normalizer(expectedResultPayloadElementCopy);
                    }
                }

                expectedResultPayloadElement = expectedResultPayloadElementCopy ?? expectedResultPayloadElement;
            }

            return expectedResultPayloadElement;

        }

        #region BatchPayloadExpectedResultNormalizer
        /// <summary>
        /// Visitor to apply expected result normalizers to batch payloads.
        /// </summary>
        private sealed class BatchPayloadExpectedResultNormalizer : ODataPayloadElementVisitorBase
        {
            /// <summary>The test configuration for the entire batch payload.</summary>
            private ReaderTestConfiguration batchTestConfiguration;

            /// <summary>The func which applies expected result normalizers to a payload element.</summary>
            private Func<ODataPayloadElement, ReaderTestConfiguration, ODataPayloadElement> applyExpectedResultNormalizers;

            /// <summary>
            /// Normalizes the specified payload element.
            /// </summary>
            /// <param name="payloadElement">The payload element to normalize.</param>
            /// <returns>The normalized payload element.</returns>
            public ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
            {
                // Allways make a deep copy since we will modify all the batch parts.
                payloadElement = payloadElement.DeepCopy();
                this.Recurse(payloadElement);
                return payloadElement;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchTestConfiguration">The test configuration for the entire batch payload.</param>
            /// <param name="applyExpectedResultNormalizers">The func which applies expected result normalizers to a payload element.</param>
            public BatchPayloadExpectedResultNormalizer(
                ReaderTestConfiguration batchTestConfiguration,
                Func<ODataPayloadElement, ReaderTestConfiguration, ODataPayloadElement> applyExpectedResultNormalizers)
            {
                this.batchTestConfiguration = batchTestConfiguration;
                this.applyExpectedResultNormalizers = applyExpectedResultNormalizers;
            }

            /// <summary>
            /// Visits a request payload
            /// </summary>
            /// <param name="operation">Operation to visit</param>
            protected override void VisitRequestOperation(IHttpRequest operation)
            {
                base.VisitRequestOperation(operation);

                var requestOperation = operation as ODataRequest;
                if (requestOperation.Body != null && requestOperation.Body.RootElement != null)
                {
                    requestOperation.Body = new ODataPayloadBody(
                            requestOperation.Body.SerializedValue,
                            this.applyExpectedResultNormalizers(requestOperation.Body.RootElement, this.GetOperationTestConfiguration(operation))
                        );
                }
            }

            /// <summary>
            /// Replaces operation if payload is updated
            /// </summary>
            /// <param name="operation">Operation to visit</param>
            protected override void VisitResponseOperation(HttpResponseData operation)
            {
                base.VisitResponseOperation(operation);

                var responseOperation = operation as ODataResponse;
                if (responseOperation.RootElement != null)
                {
                    responseOperation.RootElement = this.applyExpectedResultNormalizers(responseOperation.RootElement, this.GetOperationTestConfiguration(operation));
                }
            }

            /// <summary>
            /// Creates a test configuration for an operation in the batch.
            /// </summary>
            /// <param name="operation">The operation to get the configuration for.</param>
            /// <returns>Test configuration based on the batch test configuration with the format modified to apply to the operation in question.</returns>
            private ReaderTestConfiguration GetOperationTestConfiguration(IHttpMessage operation)
            {
                string contentType = operation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);

                ODataFormat format = this.batchTestConfiguration.Format;
                if (IsJsonMimeType(contentType))
                {
                    format = ODataFormat.Json;
                }

                return new ReaderTestConfiguration(
                    format,
                    this.batchTestConfiguration.MessageReaderSettings,
                    this.batchTestConfiguration.IsRequest,
                    this.batchTestConfiguration.Synchronous,
                    this.batchTestConfiguration.Version);
            }

            /// <summary>
            /// Determines if the specified content type is an ATOM content type.
            /// </summary>
            /// <param name="contentType">The content type to inspect.</param>
            /// <returns>true if the content type is ATOM; false otherwise</returns>
            private static bool IsAtomMimeType(string contentType)
            {
                return contentType.StartsWith(MimeTypes.ApplicationAtomXml, StringComparison.OrdinalIgnoreCase)
                    || contentType.StartsWith(MimeTypes.ApplicationXml, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Determines if the specified content type is an JSON content type.
            /// </summary>
            /// <param name="contentType">The content type to inspect.</param>
            /// <returns>true if the content type is JSON; false otherwise</returns>
            private static bool IsJsonMimeType(string contentType)
            {
                return contentType.StartsWith(MimeTypes.ApplicationJson, StringComparison.OrdinalIgnoreCase);
            }
        }
        #endregion
    }
}
