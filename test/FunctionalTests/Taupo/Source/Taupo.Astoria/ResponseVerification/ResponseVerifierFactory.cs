//---------------------------------------------------------------------
// <copyright file="ResponseVerifierFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Factory class for building response verifiers
    /// </summary>
    [ImplementationName(typeof(IResponseVerifierFactory), "Default")]
    public class ResponseVerifierFactory : IResponseVerifierFactory
    {
        /// <summary>
        /// Initializes a new instance of the ResponseVerifierFactory class
        /// </summary>
        public ResponseVerifierFactory()
        {
            this.BuildStandardVerifierFunc =
                expectedStatusCode => new CompositeResponseVerifier(
                this.BuildStatusCodeVerifier(expectedStatusCode),
                this.BuildPayloadTypeVerifier(),
                this.BuildQueryVerifier(),
                this.BuildDeleteVerifier(),
                this.BuildETagHeaderVerifier(),
                this.BuildUpdateVerifier(),
                this.BuildServiceOperationVerifier(),
                this.BuildActionResponseVerifier(),
                this.BuildRelationshipLinkVerifier(),
                this.BuildPreferHeaderVerifier(),
                this.BuildNoSniffHeaderVerifier(),
                this.BuildDataServiceVersionVerifier(),
                this.BuildContentTypeHeaderVerifier(),
                this.BuildNextLinkVerifier(),
                this.BuildJsonDateTimeVerifier());

            this.BuildErrorVerifierFunc =
                (statusCode, message, verifier) =>
                {
                    // TODO: add ContentTypeHeaderVerifier
                    return new CompositeResponseVerifier(
                        this.BuildStatusCodeVerifier(statusCode),
                        this.BuildDataServiceVersionVerifier(),
                        this.BuildNoSniffHeaderVerifier(),
                        this.BuildETagHeaderVerifier(),
                        this.BuildErrorResponseVerifier(message, verifier));
                };
        }

        /// <summary>
        /// Gets or sets the dependency injector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

        /// <summary>
        /// Gets or sets the model of the service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema Model { get; set; }

        /// <summary>
        /// Gets or sets the delegate to use for building standard verifiers. Factored this way for testability.
        /// </summary>
        internal Func<HttpStatusCode, CompositeResponseVerifier> BuildStandardVerifierFunc { get; set; }

        /// <summary>
        /// Gets or sets the delegate to use for building standard verifiers. Factored this way for testability.
        /// </summary>
        internal Func<HttpStatusCode, ExpectedErrorMessage, IStringResourceVerifier, CompositeResponseVerifier> BuildErrorVerifierFunc { get; set; }

        private DataServiceProtocolVersion MaxProtocolVersion
        {
            get { return this.Model.GetDefaultEntityContainer().GetDataServiceBehavior().MaxProtocolVersion; }
        }

        /// <summary>
        /// Returns a composite verifier with all standard verifiers attached
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code of the response</param>
        /// <returns>A composite verifier</returns>
        public IResponseVerifier GetStandardVerifier(HttpStatusCode expectedStatusCode)
        {
            ExceptionUtilities.CheckObjectNotNull(this.BuildStandardVerifierFunc, "Delegate for building standard verifier cannot be null");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var verifier = this.BuildStandardVerifierFunc(expectedStatusCode);
            this.Injector.InjectDependenciesInto(verifier);
            verifier.Verifiers.ForEach(v => this.Injector.InjectDependenciesInto(v));
            return verifier;
        }

        /// <summary>
        /// Returns a composite verifier with all standard verifiers attached
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code of the response</param>
        /// <param name="expectedMessage">The expected error message</param>
        /// <param name="resourceVerifier">The resource verifier for the error message</param>
        /// <returns>A composite verifier</returns>
        public IResponseVerifier GetErrorVerifier(HttpStatusCode expectedStatusCode, ExpectedErrorMessage expectedMessage, IStringResourceVerifier resourceVerifier)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedMessage, "expectedMessage");
            ExceptionUtilities.CheckArgumentNotNull(resourceVerifier, "resourceVerifier");
            ExceptionUtilities.CheckObjectNotNull(this.BuildErrorVerifierFunc, "Delegate for building standard verifier cannot be null");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var verifier = this.BuildErrorVerifierFunc(expectedStatusCode, expectedMessage, resourceVerifier);
            this.Injector.InjectDependenciesInto(verifier);
            verifier.Verifiers.ForEach(v => this.Injector.InjectDependenciesInto(v));
            return verifier;
        }

        private IResponseVerifier BuildStatusCodeVerifier(HttpStatusCode expectedStatusCode)
        {
            return new StatusCodeVerifier(expectedStatusCode);
        }

        private IResponseVerifier BuildPayloadTypeVerifier()
        {
            return new ExpectedPayloadTypeResponseVerifier();
        }

        private IResponseVerifier BuildQueryVerifier()
        {
            return new QueryResponseVerifier(this.MaxProtocolVersion);
        }

        private IResponseVerifier BuildRelationshipLinkVerifier()
        {
            return new RelationshipLinkVerifier(this.Model);
        }

        private IResponseVerifier BuildPreferHeaderVerifier()
        {
            return new PreferHeaderVerifier(this.MaxProtocolVersion);
        }

        private IResponseVerifier BuildNoSniffHeaderVerifier()
        {
            return new NoSniffHeaderVerifier();
        }

        private IResponseVerifier BuildDataServiceVersionVerifier()
        {
            return new DataServiceVersionVerifier(this.Model, this.MaxProtocolVersion);
        }

        private IResponseVerifier BuildErrorResponseVerifier(ExpectedErrorMessage expectedMessage, IStringResourceVerifier resourceVerifier)
        {
            return new ErrorResponseVerifier(expectedMessage, resourceVerifier);
        }

        private IResponseVerifier BuildContentTypeHeaderVerifier()
        {
            return new ContentTypeHeaderVerifier();
        }

        private IResponseVerifier BuildDeleteVerifier()
        {
            return new DeleteResponseVerifier();
        }

        private IResponseVerifier BuildETagHeaderVerifier()
        {
            return new ETagHeaderVerifier();
        }

        private IResponseVerifier BuildUpdateVerifier()
        {
            return new UpdateResponseVerifier(this.MaxProtocolVersion);
        }

        private IResponseVerifier BuildServiceOperationVerifier()
        {
            return new ServiceOperationDescriptorVerifier(this.Model);
        }

        private IResponseVerifier BuildActionResponseVerifier()
        {
            return new ActionResponseVerifier(this.Model, this.MaxProtocolVersion);
        }

        private IResponseVerifier BuildNextLinkVerifier()
        {
            return new NextLinkResponseVerifier();
        }

        private IResponseVerifier BuildJsonDateTimeVerifier()
        {
            return new JsonDateTimeFormattingResponseVerifier();
        }
    }
}
