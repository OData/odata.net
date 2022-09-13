//---------------------------------------------------------------------
// <copyright file="ProtocolTestServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A contract to encapsulate components and methods useful for writing protocol tests
    /// </summary>
    public class ProtocolTestServices
    {
        /// <summary>
        /// Gets or sets the formatter to use when constructing ETags
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets the OData request manager to use during the test
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Gets or sets the response verifier factory to use during the test
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerifierFactory ResponseVerifierFactory { get; set; }

        /// <summary>
        /// Gets or sets the visitor to use for building uri's out of queries
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriConverter QueryToUriConverter { get; set; }

        /// <summary>
        /// Gets or sets the converter to use when building system Uris out of OData Uris
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriToStringConverter { get; set; }

        /// <summary>
        /// Gets or sets the selector to use for getting protocol format information for a mime type
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector ProtocolFormatStrategySelector { get; set; }

        /// <summary>
        /// Gets or sets the protocol implementation details of the service
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolImplementationDetails ProtocolImplementationDetails { get; set; }

        /// <summary>
        /// Gets or sets the response verification context
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IResponseVerificationContext ResponseVerificationContext { get; set; }

        /// <summary>
        /// Gets or sets the payload builder
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadBuilder PayloadBuilder { get; set; }
    }
}
