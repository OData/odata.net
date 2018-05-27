//---------------------------------------------------------------------
// <copyright file="TestConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Contracts;

    /// <summary>
    /// Configuration settings for a test.
    /// </summary>
    public class TestConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format used for the test.</param>
        /// <param name="version">The OData protocol version to be used for the payload.</param>
        /// <param name="request">True if the test is reading a request. Otherwise false if it's reading a response.</param>
        public TestConfiguration(ODataFormat format, ODataVersion version, bool request, TestODataBehaviorKind behaviorKind)
        {
            this.Format = format;
            this.Version = version;
            this.IsRequest = request;
            this.RunBehaviorKind = behaviorKind;
        }

        /// <summary>
        /// The format used for the test.
        /// </summary>
        public ODataFormat Format { get; protected set; }

        /// <summary>
        /// The version of the protocol used in the test.
        /// </summary>
        public ODataVersion Version { get; protected set; }

        /// <summary>
        /// True if the test is for a request. Otherwise false if it's for a response.
        /// </summary>
        public bool IsRequest { get; protected set; }

        /// <summary>
        /// The behavior to use when running the test.
        /// </summary>
        public TestODataBehaviorKind RunBehaviorKind { get; private set; }
    }
}
