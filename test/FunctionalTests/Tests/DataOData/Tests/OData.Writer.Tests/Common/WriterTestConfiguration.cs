//---------------------------------------------------------------------
// <copyright file="WriterTestConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Class which wraps all the configurations for a writer test
    /// </summary>
    public sealed class WriterTestConfiguration : TestConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format used for the test.</param>
        /// <param name="messageWriterSettings">The message writer settings used for the test.</param>
        /// <param name="IsRequest">True if the test is writing a request. Otherwise false if it's writing a response.</param>
        /// <param name="synchronous">True if the test should be ran using synchronous API. Otherwise false if it should be ran using asynchronous APIs.</param>
        public WriterTestConfiguration(ODataFormat format, ODataMessageWriterSettings messageWriterSettings, bool IsRequest, bool synchronous)
            : base(format, messageWriterSettings.Version.Value, IsRequest, TestODataBehaviorKind.Default)
        {
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");
            this.Synchronous = synchronous;
            this.MessageWriterSettings = messageWriterSettings;
        }

        /// <summary>
        /// True if the test should be ran using synchronous API. Otherwise false if it should be ran using asynchronous APIs.
        /// </summary>
        public bool Synchronous { get; private set; }

        /// <summary>
        /// The message writer settings used for the test.
        /// </summary>
        public ODataMessageWriterSettings MessageWriterSettings { get; private set; }

        /// <summary>
        /// Returns text represenation of the configuration.
        /// </summary>
        /// <returns>Humanly readable text representation of the configuration. Used for debugging.</returns>
        public override string ToString()
        {
            return string.Format(
                "Format: {0}, Version: {1}, WriterSettings.EnableMessageStreamDisposal: {2}, IsRequest: {3}, Synchronous: {4}",
                this.Format,
                this.Version.ToString(),
                this.MessageWriterSettings.EnableMessageStreamDisposal.ToString(),
                this.IsRequest,
                this.Synchronous);
        }

        /// <summary>
        /// Clones the test configuration.
        /// </summary>
        /// <returns>The copied test configuration.</returns>
        public WriterTestConfiguration Clone()
        {
            return new WriterTestConfiguration(this.Format, this.MessageWriterSettings.Clone(), this.IsRequest, this.Synchronous);
        }

        /// <summary>
        /// Applies the specified behavior with default settings.
        /// For detailed settings the caller should use Clone and direct application of behavior to the settings.
        /// </summary>
        /// <param name="behaviorKind">The behavior kind to apply.</param>
        /// <returns>A copy of this configuration with the new behavior applied.</returns>
        public WriterTestConfiguration CloneAndApplyBehavior(TestODataBehaviorKind behaviorKind)
        {
            WriterTestConfiguration testConfiguration = this.Clone();
            switch (behaviorKind)
            {
                case TestODataBehaviorKind.Default:
                    break;
                case TestODataBehaviorKind.WcfDataServicesClient:
                    break;
                case TestODataBehaviorKind.WcfDataServicesServer:
                    testConfiguration.MessageWriterSettings.Validations &= ~ValidationKinds.ThrowOnDuplicatePropertyNames;
                    break;
            }

            return testConfiguration;
        }
    }
}
