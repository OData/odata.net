//---------------------------------------------------------------------
// <copyright file="ReaderTestConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for a reader test.
    /// </summary>
    public class ReaderTestConfiguration : TestConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format used for the test.</param>
        /// <param name="messageReaderSettings">The message reader settings used for the test.</param>
        /// <param name="readerRequest">True if the test is reading a request. Otherwise false if it's reading a response.</param>
        /// <param name="synchronous">True if the test should be ran using synchronous API. Otherwise false if it should be ran using asynchronous APIs.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="skipStateValidationBeforeRead">True if test to skip reader state validation before reading.</param>
        public ReaderTestConfiguration(ODataFormat format, ODataMessageReaderSettings messageReaderSettings, bool IsRequest, bool synchronous,
            ODataVersion version = ODataVersion.V4, bool skipStateValidationBeforeRead = false)
            : base(format, version, IsRequest, TestODataBehaviorKind.Default)
        {
            Debug.Assert(messageReaderSettings != null, "readerSettings != null");

            this.MessageReaderSettings = messageReaderSettings;
            this.Synchronous = synchronous;
            this.SkipStateValidationBeforeRead = skipStateValidationBeforeRead;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other">The <see cref="ReaderTestConfiguration"/> instance used to initialize the new instance.</param>
        /// <param name="behaviorKind">The behavior to use when running this test.</param>
        private ReaderTestConfiguration(ReaderTestConfiguration other, TestODataBehaviorKind behaviorKind)
            : base(other.Format, other.Version, other.IsRequest, behaviorKind)
        {
            this.MessageReaderSettings = other.MessageReaderSettings.Clone();
            this.Synchronous = other.Synchronous;
            this.SkipStateValidationBeforeRead = other.SkipStateValidationBeforeRead;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other">The <see cref="ReaderTestConfiguration"/> instance used to initialize the new instance.</param>
        public ReaderTestConfiguration(ReaderTestConfiguration other)
            : this(other, other.RunBehaviorKind)
        {
        }

        /// <summary>
        /// The message reader settings used for the test.
        /// </summary>
        public ODataMessageReaderSettings MessageReaderSettings { get; private set; }

        /// <summary>
        /// True if the test should be ran using synchronous API. Otherwise false if it should be ran usin asynchronous APIs.
        /// </summary>
        public bool Synchronous { get; private set; }

        /// <summary>
        /// True if test to skip reader state validation before reading.
        /// </summary>
        public bool SkipStateValidationBeforeRead { get; private set; }

        /// <summary>
        /// Returns text representation of the configuration.
        /// </summary>
        /// <returns>Humanly readable text representation of the configuration. Used for debugging.</returns>
        public override string ToString()
        {
            return string.Format(
                "Format: {0}, Version: {1}, ReaderSettings: [{2}], IsRequest: {3}, Synchronous: {4}, SkipStateValidationBeforeRead: {5}",
                this.Format,
                this.Version.ToString(),
                this.MessageReaderSettings.ToDebugString(),
                this.IsRequest,
                this.Synchronous,
                this.SkipStateValidationBeforeRead);
        }

        /// <summary>
        /// Applies the specified behavior with default settings.
        /// </summary>
        /// <param name="behaviorKind">The behavior kind to apply.</param>
        /// <returns>A copy of this configuration with the new behavior applied.</returns>
        public ReaderTestConfiguration CloneAndApplyBehavior(TestODataBehaviorKind behaviorKind)
        {
            ReaderTestConfiguration testConfiguration = new ReaderTestConfiguration(this, behaviorKind);
            switch (behaviorKind)
            {
                case TestODataBehaviorKind.Default:
                    break;
                case TestODataBehaviorKind.WcfDataServicesClient:
                    testConfiguration.MessageReaderSettings.Validations &= ~ValidationKinds.ThrowOnDuplicatePropertyNames;
                    testConfiguration.MessageReaderSettings.ClientCustomTypeResolver = null;
                    testConfiguration.MessageReaderSettings.Validations &= ~ValidationKinds.ThrowIfTypeConflictsWithMetadata;
                    break;
                case TestODataBehaviorKind.WcfDataServicesServer:
                    testConfiguration.MessageReaderSettings.Validations &= ~ValidationKinds.ThrowOnDuplicatePropertyNames;
                    testConfiguration.MessageReaderSettings.ClientCustomTypeResolver = null;
                    testConfiguration.MessageReaderSettings.Validations &= ~ValidationKinds.ThrowIfTypeConflictsWithMetadata;
                    // EnableReadingEntryContentInEntryStartState == true
                    break;
            }

            return testConfiguration;
        }

        /// <summary>
        /// Applies the specified max protocol version to copy of the settings.
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version to apply.</param>
        /// <returns>A copy of this configuration with the max protocol version applied.</returns>
        public ReaderTestConfiguration CloneAndApplyMaxProtocolVersion(ODataVersion maxProtocolVersion)
        {
            ReaderTestConfiguration testConfiguration = new ReaderTestConfiguration(this);
            testConfiguration.MessageReaderSettings.MaxProtocolVersion = maxProtocolVersion;
            return testConfiguration;
        }
    }
}
