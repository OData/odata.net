//---------------------------------------------------------------------
// <copyright file="WriterArgumentValidationTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for correct API argument validation in writers
    /// </summary>
    // [TestClass, TestCase]
    public class WriterArgumentValidationTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies argument validation behavior for WriteStart(ODataResourceSet) method")]
        public void WriteStartFeedTest()
        {
            ForWriters(true, (writer) =>
            {
                this.VerifyArgumentNullException(() => writer.WriteStart((ODataResourceSet)null));
            });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies argument validation behavior for WriteStart(ODataEntry) method")]
        public void WriteStartResourceTest()
        {
            ForWriters(false, (writer) =>
            {
                this.VerifyArgumentNullException(() => writer.WriteStart((ODataResource)null));
            });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies argument validation behavior for WriteStart(ODataNestedResourceInfo) method")]
        public void WriteStartLinkTest()
        {
            ForWriters(false, (writer) =>
            {
                this.VerifyArgumentNullException(() => writer.WriteStart((ODataNestedResourceInfo)null));
            });
        }

        /// <summary>
        /// Runs the specified action for all interesting writers.
        /// </summary>
        /// <param name="feedWriter">True if writing a feed; false if writing an entry.</param>
        /// <param name="action">The action to run.</param>
        private void ForWriters(bool feedWriter, Action<ODataWriter> action)
        {
            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (TestStream stream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(feedWriter);
                        action(writer);
                    }
                });
        }

        /// <summary>
        /// Verifies that the specified action fails with argument being null exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void VerifyArgumentNullException(Action action)
        {
            // TODO: Support raw error message verification for non-product exceptions
            TestExceptionUtils.ExpectedException(this.Assert, action, new ExpectedException(typeof(ArgumentNullException)), this.ExceptionVerifier);
        }
    }
}
