//---------------------------------------------------------------------
// <copyright file="ODataRequestVersionResourceErrorCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Component that Calculates the version based on the ODataUri provided
    /// </summary>
    [ImplementationName(typeof(IODataRequestVersionResourceErrorCalculator), "Default")]
    public class ODataRequestVersionResourceErrorCalculator : IODataRequestVersionResourceErrorCalculator
    {
        private const string DataServiceGeneralError = "DataServiceException_GeneralError";
        private const string DataServiceVersionTooLow = "DataService_DSVTooLow";
        private const string MaxDataServiceVersionTooLow = "DataService_MaxDSVTooLow";
        private const string DataServiceRequestVersionMustBeLessThanMPV = "DataService_RequestVersionMustBeLessThanMPV";
        private const string DataServiceConfigurationResponseVersionIsBiggerThanProtocolVersion = "DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion";

        /// <summary>
        /// Gets or sets the Min Version Required Calculator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriMinimumVersionRequiredCalculator MinimumVersionRequiredCalculator { get; set; }

        /// <summary>
        /// Gets or sets the string resource verifiers to use.
        /// </summary>
        [InjectDependency]
        public IAstoriaStringResourceVerifiers StringResourceVerifiers { get; set; }

        /// <summary>
        /// Gets or sets the metadata resolver.
        /// </summary>
        [InjectDependency]
        public IODataPayloadElementMetadataResolver MetadataResolver { get; set; }

        /// <summary>
        /// Calculates the ResourceStringInformation based on the ODataRequest and maxProtocol version to determine if this i
        /// </summary>
        /// <param name="request">Request to calculate from</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <param name="expectedErrorMessage">Calculated Version Error information</param>
        /// <returns>boolean value of if a Error ResourceString Information was calculated or not</returns>
        public bool TryCalculateError(ODataRequest request, DataServiceProtocolVersion maxProtocolVersion, out ExpectedErrorMessage expectedErrorMessage)
        {
            ExceptionUtilities.CheckArgumentNotNull(request, "request");

            expectedErrorMessage = null;

            DataServiceProtocolVersion dataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.DataServiceVersion));
            DataServiceProtocolVersion maxDataServiceVersion = VersionHelper.ConvertToDataServiceProtocolVersion(request.GetHeaderValueIfExists(HttpHeaders.MaxDataServiceVersion));
            if (dataServiceVersion == DataServiceProtocolVersion.Unspecified)
            {
                dataServiceVersion = VersionHelper.CalculateDataServiceVersionIfNotSpecified(maxProtocolVersion, maxDataServiceVersion);
            }

            if (TryCalculateDataServiceVersionTooHighError(dataServiceVersion, maxProtocolVersion, out expectedErrorMessage))
            {
                return true;
            }

            string contentType = request.GetHeaderValueIfExists(HttpHeaders.ContentType);
            string acceptType = request.GetHeaderValueIfExists(HttpHeaders.Accept);

            if (contentType == null)
            {
                contentType = MimeTypes.Any;
            }

            if (acceptType == null)
            {
                acceptType = MimeTypes.Any;
            }

            // Do Uri processing first
            if (this.TryCalculateODataUriProcessingError(request, maxDataServiceVersion, maxProtocolVersion, acceptType, out expectedErrorMessage))
            {
                return true;
            }

            // Now check metadata of what the request should look like, will it cause a version bump greater than the DSV
            EntitySet expectedEntitySet = null;
            if (request.Uri.TryGetExpectedEntitySet(out expectedEntitySet))
            {
                DataServiceProtocolVersion entitySetUriVersion = CalculateEntitySetUriSegmentRequestVersion(request, expectedEntitySet, maxProtocolVersion, maxDataServiceVersion, contentType);
                if (TryCalculateVersionError(entitySetUriVersion, dataServiceVersion, maxProtocolVersion, out expectedErrorMessage))
                {
                    return true;
                }
            }

            // First ensure that the version is greater than mpv
            if (maxProtocolVersion < dataServiceVersion)
            {
                expectedErrorMessage = new ExpectedErrorMessage(DataServiceRequestVersionMustBeLessThanMPV, dataServiceVersion.ConvertToHeaderFormat(), maxProtocolVersion.ConvertToHeaderFormat());
                return true;
            }

            // Now review the version of the payload and create the right errors
            if (this.TryCalculateODataResponseError(request, dataServiceVersion, maxDataServiceVersion, maxProtocolVersion, out expectedErrorMessage))
            {
                return true;
            }

            // Now check for any errors from reading the payload
            return TryCalculateReaderError(request, dataServiceVersion, this.StringResourceVerifiers, this.MetadataResolver, out expectedErrorMessage);
        }

        internal static bool TryCalculateDataServiceVersionTooHighError(DataServiceProtocolVersion dataServiceVersion, DataServiceProtocolVersion maxProtocolVersion, out ExpectedErrorMessage expectedErrorMessage)
        {
            if (dataServiceVersion > maxProtocolVersion)
            {
                expectedErrorMessage = new ExpectedErrorMessage(DataServiceRequestVersionMustBeLessThanMPV, dataServiceVersion.ConvertToHeaderFormat(), maxProtocolVersion.ConvertToHeaderFormat());
                return true;
            }

            expectedErrorMessage = null;
            return false;
        }

        internal static DataServiceProtocolVersion CalculateEntitySetUriSegmentRequestVersion(ODataRequest request, EntitySet entitySet, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion, string contentType)
        {
            bool processTypeMetadata = false;

            DataServiceProtocolVersion entitySetSegmentVersion = DataServiceProtocolVersion.V4;

            IEnumerable<EntityType> entityTypes = VersionHelper.GetEntityTypes(entitySet);

            if (request.GetEffectiveVerb().IsUpdateVerb() || request.GetEffectiveVerb() == HttpVerb.Post)
            {
                // Typically for all posts there is some type of payload so we should analyze the metadata
                if (request.GetEffectiveVerb() == HttpVerb.Post)
                {
                    processTypeMetadata = true;
                }
                else if (request.PreferHeaderApplies(maxProtocolVersion))
                {
                    processTypeMetadata = true;
                }

                // Whenever there is an update operation and EPM is involved we need to check the metadata version
                if (entityTypes.SelectMany(et => et.Annotations.OfType<PropertyMappingAnnotation>()).Where(fma => fma.KeepInContent == false).Any())
                {
                    processTypeMetadata = true;
                }
            }

            if (processTypeMetadata)
            {
                entitySetSegmentVersion = entitySetSegmentVersion.IncreaseVersionIfRequired(entitySet.CalculateMinEntityPropertyMappingVersion(VersionCalculationType.Request, contentType, maxProtocolVersion, maxDataServiceVersion));
            }

            return entitySetSegmentVersion;
        }

        internal static bool TryCalculateVersionError(DataServiceProtocolVersion calculatedUriSegmentVersion, DataServiceProtocolVersion dataServiceVersion, DataServiceProtocolVersion maxProtocolVersion, out ExpectedErrorMessage expectedErrorMessage)
        {
            expectedErrorMessage = null;

            // First ensure that the version is greater than mpv
            if (calculatedUriSegmentVersion > maxProtocolVersion)
            {
                expectedErrorMessage = new ExpectedErrorMessage(DataServiceRequestVersionMustBeLessThanMPV, calculatedUriSegmentVersion.ConvertToHeaderFormat(), maxProtocolVersion.ConvertToHeaderFormat());
                return true;
            }

            // Now ensure its not greater than than DSV
            if (calculatedUriSegmentVersion > dataServiceVersion)
            {
                expectedErrorMessage = new ExpectedErrorMessage(DataServiceVersionTooLow, dataServiceVersion.ConvertToHeaderFormat(), calculatedUriSegmentVersion.ConvertToIntegerFormat(), "0");
                return true;
            }

            return false;
        }

        internal static bool TryCalculateReaderError(ODataRequest request, DataServiceProtocolVersion dataServiceVersion, IAstoriaStringResourceVerifiers verifiers, IODataPayloadElementMetadataResolver metadataResolver, out ExpectedErrorMessage expectedErrorMessage)
        {
            var requestBody = request.Body;

            if (requestBody != null && requestBody.RootElement != null)
            {
                var payload = requestBody.RootElement;

                if (metadataResolver != null && !IsStreamRequest(request) && !request.Uri.IsAction())
                {
                    metadataResolver.ResolveMetadata(payload, request.Uri);
                }

                if (TryCalculateReaderError(payload, dataServiceVersion, verifiers, out expectedErrorMessage))
                {
                    return true;
                }
            }

            expectedErrorMessage = null;
            return false;
        }

        internal static IEnumerable<MemberProperty> ExtractPropertyPath(ODataUri requestUri)
        {
            // take all property segments in the uri and get the properties
            return requestUri.Segments.OfType<PropertySegment>().Select(p => p.Property);
        }

        internal static bool TryCalculateReaderError(ODataPayloadElement payload, DataServiceProtocolVersion dataServiceVersion, IAstoriaStringResourceVerifiers verifiers, out ExpectedErrorMessage expectedErrorMessage)
        {
            if (new PayloadReaderVersionErrorCalculator(verifiers).TryCalculateError(payload, dataServiceVersion, out expectedErrorMessage))
            {
                return true;
            }

            expectedErrorMessage = null;
            return false;
        }

        internal static bool IsStreamRequest(ODataRequest request)
        {
            if (request.Uri.IsNamedStream() || request.Uri.IsMediaResource())
            {
                return true;
            }

            if (request.GetEffectiveVerb() == HttpVerb.Post && request.Uri.IsEntitySet())
            {
                EntitySet entitySet;
                ExceptionUtilities.Assert(request.Uri.TryGetExpectedEntitySet(out entitySet), "Could not infer entity set from uri");
                return entitySet.EntityType.HasStream();
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not really high.")]
        internal bool TryCalculateODataUriProcessingError(ODataRequest request, DataServiceProtocolVersion maxDataServiceVersion, DataServiceProtocolVersion maxProtocolVersion, string acceptType, out ExpectedErrorMessage expectedErrorMessage)
        {
            expectedErrorMessage = null;

            if (request.Uri.IsEntity() || request.Uri.IsEntitySet() || request.Uri.IsServiceOperation())
            {
                bool isEntityFromNonOperationQuery = request.GetEffectiveVerb() == HttpVerb.Get && !request.Uri.IsServiceOperation();
                bool isEntityFromOperation = request.Uri.IsServiceOperation() && (request.Uri.IsEntity() || request.Uri.IsEntitySet());

                // Do an EPM on all returning sets for the response
                if (isEntityFromNonOperationQuery || isEntityFromOperation)
                {
                    EntitySet startingEntitySet = null;
                    if (request.Uri.TryGetExpectedEntitySet(out startingEntitySet))
                    {
                        DataServiceProtocolVersion minVersion = startingEntitySet.CalculateEntitySetProtocolVersion(acceptType, VersionCalculationType.Response, maxProtocolVersion, maxDataServiceVersion);

                        // Check Epm First
                        if (maxDataServiceVersion != DataServiceProtocolVersion.Unspecified && minVersion > maxDataServiceVersion)
                        {
                            expectedErrorMessage = new ExpectedErrorMessage(MaxDataServiceVersionTooLow, maxDataServiceVersion.ConvertToHeaderFormat(), minVersion.ConvertToIntegerFormat(), "0");
                            return true;
                        }
                    }

                    // we need to check the root Entity Set first, then any/all, and later the Expand
                    foreach (EntitySet entitySet in request.Uri.GetIncludingExpandsSets())
                    {
                        DataServiceProtocolVersion minVersion = entitySet.CalculateEntitySetProtocolVersion(acceptType, VersionCalculationType.Response, maxProtocolVersion, maxDataServiceVersion);

                        // Check Epm First
                        if (maxDataServiceVersion != DataServiceProtocolVersion.Unspecified && minVersion > maxDataServiceVersion)
                        {
                            expectedErrorMessage = new ExpectedErrorMessage(MaxDataServiceVersionTooLow, maxDataServiceVersion.ConvertToHeaderFormat(), minVersion.ConvertToIntegerFormat(), "0");
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool TryCalculateODataResponseError(ODataRequest request, DataServiceProtocolVersion dataServiceVersion, DataServiceProtocolVersion maxDataServiceVersion, DataServiceProtocolVersion maxProtocolVersion, out ExpectedErrorMessage expectedErrorMessage)
        {
            expectedErrorMessage = null;

            string preferHeader = request.GetHeaderValueIfExists(HttpHeaders.Prefer);
            if (preferHeader != null &&
                maxProtocolVersion > DataServiceProtocolVersion.V4 &&
                dataServiceVersion > DataServiceProtocolVersion.V4 &&
                maxDataServiceVersion != DataServiceProtocolVersion.Unspecified &&
                maxDataServiceVersion < DataServiceProtocolVersion.V4 &&
                request.Verb != HttpVerb.Delete)
            {
                expectedErrorMessage = new ExpectedErrorMessage(MaxDataServiceVersionTooLow, maxDataServiceVersion.ConvertToHeaderFormat(), DataServiceProtocolVersion.V4.ConvertToIntegerFormat(), "0");
                return true;
            }

            // Now do payload processing
            DataServiceProtocolVersion expectedResponseVersion = this.MinimumVersionRequiredCalculator.CalculateMinResponseVersion(request, maxProtocolVersion);

            // Now ensure its not greater than than MDSV
            if (maxDataServiceVersion != DataServiceProtocolVersion.Unspecified && expectedResponseVersion > maxDataServiceVersion)
            {
                expectedErrorMessage = new ExpectedErrorMessage(MaxDataServiceVersionTooLow, maxDataServiceVersion.ConvertToHeaderFormat(), expectedResponseVersion.ConvertToIntegerFormat(), "0");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Visitor for calculating errors from reading payloads with version restrictions
        /// </summary>
        private class PayloadReaderVersionErrorCalculator : ODataPayloadElementVisitorBase
        {
            private readonly Stack<NamedStructuralType> structuralTypeStack = new Stack<NamedStructuralType>();
            private readonly Stack<MemberProperty> memberPropertyStack = new Stack<MemberProperty>();
            private readonly IAstoriaStringResourceVerifiers verifiers;
            private DataServiceProtocolVersion payloadVersion;
            private ExpectedErrorMessage expectedError;

            /// <summary>
            /// Initializes a new instance of the <see cref="PayloadReaderVersionErrorCalculator"/> class.
            /// </summary>
            /// <param name="verifiers">The string resource verifiers</param>
            public PayloadReaderVersionErrorCalculator(IAstoriaStringResourceVerifiers verifiers)
            {
                this.verifiers = verifiers;
            }

            /// <summary>
            /// Returns whether or not to expect an error when reading the given payload with the given version.
            /// </summary>
            /// <param name="requestPayload">The payload which will be read.</param>
            /// <param name="version">The version the reader expects.</param>
            /// <param name="error">The error if one is expected.</param>
            /// <returns>
            /// Whether an error is expected
            /// </returns>
            public bool TryCalculateError(ODataPayloadElement requestPayload, DataServiceProtocolVersion version, out ExpectedErrorMessage error)
            {
                error = null;

                if (requestPayload == null)
                {
                    return false;
                }

                // Simplifying assumption. The rest of this class assumes that the version is < 3 and doesn't check further.
                if (version >= DataServiceProtocolVersion.V4)
                {
                    return false;
                }

                ExceptionUtilities.Assert(version != DataServiceProtocolVersion.Unspecified, "Version parameter cannot be unspecified");
                this.payloadVersion = version;

                return error != null;
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(ComplexInstance payloadElement)
            {
                this.VisitStructuralInstance<ComplexInstance, ComplexDataType>(payloadElement, d => d.Definition);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(ComplexMultiValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.Assert(this.payloadVersion < DataServiceProtocolVersion.V4, "Code should not be called for V3 or greater version");

                this.ExpectErrorForCollection();

                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(EntityInstance payloadElement)
            {
                this.VisitStructuralInstance<EntityInstance, EntityDataType>(payloadElement, d => d.Definition);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(PrimitiveMultiValue payloadElement)
            {
                this.ExpectErrorForCollection();
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Visits the payload element
            /// </summary>
            /// <param name="payloadElement">The payload element to visit</param>
            public override void Visit(PrimitiveValue payloadElement)
            {
                this.ExpectErrorIfSpatial(payloadElement);
                base.Visit(payloadElement);
            }

            /// <summary>
            /// Helper method for visiting properties
            /// </summary>
            /// <param name="payloadElement">The property to visit</param>
            /// <param name="value">The value of the property</param>
            protected override void VisitProperty(PropertyInstance payloadElement, ODataPayloadElement value)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                var memberProperty = payloadElement.Annotations.OfType<MemberPropertyAnnotation>().Select(p => p.Property).SingleOrDefault();

                if (memberProperty != null)
                {
                    this.memberPropertyStack.Push(memberProperty);
                }

                try
                {
                    base.VisitProperty(payloadElement, value);
                }
                finally
                {
                    if (memberProperty != null)
                    {
                        ExceptionUtilities.Assert(object.ReferenceEquals(memberProperty, this.memberPropertyStack.Pop()), "Stack was in bad state, unexpected property was popped");
                    }
                }
            }

            /// <summary>
            /// Wrapper for recursively visiting the given element. Used with the callback property to make unit tests easier.
            /// </summary>
            /// <param name="element">The element to visit</param>
            protected override void Recurse(ODataPayloadElement element)
            {
                // only recurse if we haven't yet found an error
                if (this.expectedError == null)
                {
                    base.Recurse(element);
                }
            }

            private static bool IsSpatialValue(PrimitiveValue primitiveValue)
            {
                // if the metadata is provided and is known to be spatial, expect an error
                var dataTypeAnnotation = primitiveValue.Annotations.OfType<DataTypeAnnotation>().SingleOrDefault();
                if (dataTypeAnnotation != null && dataTypeAnnotation.DataType is SpatialDataType)
                {
                    return true;
                }

                // if the object is non-null and a spatial value, expect an error
                SpatialTypeKind? kind;
                if (primitiveValue.ClrValue.IfValid(false, o => SpatialUtilities.TryInferSpatialTypeKind(o.GetType(), out kind)))
                {
                    return true;
                }

                return false;
            }

            private void VisitStructuralInstance<TInstance, TDataType>(TInstance payloadElement, Func<TDataType, NamedStructuralType> getDefinition)
                where TInstance : ComplexInstance
                where TDataType : DataType
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                var structuralType = payloadElement.Annotations
                    .OfType<DataTypeAnnotation>()
                    .Select(d => d.DataType)
                    .OfType<TDataType>()
                    .Select(getDefinition)
                    .SingleOrDefault();

                if (structuralType != null)
                {
                    this.structuralTypeStack.Push(structuralType);
                }

                try
                {
                    base.Visit(payloadElement);
                }
                finally
                {
                    if (structuralType != null)
                    {
                        ExceptionUtilities.Assert(object.ReferenceEquals(structuralType, this.structuralTypeStack.Pop()), "Stack was in bad state, unexpected type was popped");
                    }
                }
            }

            private void ExpectErrorIfSpatial(PrimitiveValue payloadElement)
            {
                ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
                ExceptionUtilities.Assert(this.payloadVersion < DataServiceProtocolVersion.V4, "Code should not be called for V3 or greater version");

                if (IsSpatialValue(payloadElement))
                {
                    if (this.InDynamicProperty())
                    {
                        if (payloadElement.IsNull)
                        {
                            return;
                        }

                        if (payloadElement.FullTypeName == null)
                        {
                            // A value without a type name was found and no expected type is available. When the model is specified, each value in the payload must have a type which can be either specified in the payload, explicitly by the caller or implicitly inferred from the parent value.
                            this.expectedError = new ExpectedErrorMessage("ReaderValidationUtils_ValueWithoutType");
                        }
                        else
                        {
                            // A type named '{0}' could not be resolved by the model. When a model is available, each type name must resolve to a valid type.
                            this.expectedError = new ExpectedErrorMessage("ValidationUtils_UnrecognizedTypeName", payloadElement.FullTypeName);
                        }
                    }
                    else
                    {
                        // Geography and Geometry types are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}.
                        this.expectedError = new ExpectedErrorMessage("ODataVersionChecker_GeographyAndGeometryNotSupported", this.payloadVersion.ConvertToHeaderFormat());
                    }

                    ExceptionUtilities.CheckObjectNotNull(this.verifiers, "Cannot construct expected error without verifiers");
                    this.expectedError.Verifier = this.verifiers.MicrosoftDataODataStringVerifier;
                }
            }

            private bool InDynamicProperty()
            {
                ExceptionUtilities.Assert(this.structuralTypeStack.Count > 0, "Type stack cannot be empty");
                var currentType = this.structuralTypeStack.Peek();

                if (!currentType.IsOpen)
                {
                    return false;
                }

                if (this.memberPropertyStack.Count == 0)
                {
                    return true;
                }

                var currentProperty = this.memberPropertyStack.Peek();
                if (currentType.AllProperties.Any(p => object.ReferenceEquals(p, currentProperty)) && !currentProperty.IsMetadataDeclaredProperty())
                {
                    return true;
                }

                return false;
            }
            
            private void ExpectErrorForCollection()
            {
                ExceptionUtilities.Assert(this.payloadVersion < DataServiceProtocolVersion.V4, "Code should not be called for V3 or greater version");

                // Collection types are only supported in version 3.0 of the OData protocol and higher versions. They are not supported in version {0}.
                this.expectedError = new ExpectedErrorMessage("ODataVersionChecker_CollectionNotSupported", this.payloadVersion.ConvertToHeaderFormat());

                ExceptionUtilities.CheckObjectNotNull(this.verifiers, "Cannot construct expected error without verifiers");
                this.expectedError.Verifier = this.verifiers.MicrosoftDataODataStringVerifier;
            }
        }
    }
}
