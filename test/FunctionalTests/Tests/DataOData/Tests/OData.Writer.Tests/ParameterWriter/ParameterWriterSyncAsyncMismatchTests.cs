//---------------------------------------------------------------------
// <copyright file="ParameterWriterSyncAsyncMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.ParameterWriter
{
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for verifies that mismatch between synchronous and asynchronous calls is not allowed.
    /// </summary>
    [TestClass, TestCase]
    public class ParameterWriterSyncAsyncMismatchTests : ODataWriterTestCase
    {
        private class SyncAsyncCall
        {
            public Action<ODataParameterWriterTestWrapper> Sync { get; set; }
            public Action<ODataParameterWriterTestWrapper> Async { get; set; }
        }

        private SyncAsyncCall[] TestCalls = new SyncAsyncCall[]
        {
            // Flush
            new SyncAsyncCall
            {
                Sync = (w) => w.ParameterWriter.Flush(),
                Async = (w) => w.ParameterWriter.FlushAsync().Wait()
            },

            // WriteStart
            new SyncAsyncCall
            {
                Sync = (w) => w.ParameterWriter.WriteStart(),
                Async = (w) => w.ParameterWriter.WriteStartAsync().Wait()
            },

            // WriteValue
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(); w.ParameterWriter.WriteValue("p1", 1); },
                Async = (w) => { w.WriteStart(); w.ParameterWriter.WriteValueAsync("p1", 1).Wait(); }
            },

            // CreateCollectionWriter
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(); w.ParameterWriter.CreateCollectionWriter("p1"); },
                Async = (w) => { w.WriteStart(); w.ParameterWriter.CreateCollectionWriterAsync("p1").WaitForResult(); }
            },

            // WriteEnd
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(); w.ParameterWriter.WriteEnd(); },
                Async = (w) => { w.WriteStart(); w.ParameterWriter.WriteEndAsync().Wait(); }
            },
        };

        [TestMethod, Variation(Description = "Verifies that sync and async calls cannot be mixed on a single parameter writer.")]
        public void SyncAsyncMismatchTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestCalls,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                new bool[] { false, true },
                (testCall, testConfiguration, testSynchronousCall) =>
                {
                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataParameterWriter parameterWriter = messageWriter.CreateODataParameterWriter(null /*functionImport*/);
                        ODataParameterWriterTestWrapper writer = (ODataParameterWriterTestWrapper)parameterWriter;

                        this.Assert.ExpectedException(
                            () =>
                            {
                                if (testSynchronousCall)
                                {
                                    testCall.Sync(writer);
                                }
                                else
                                {
                                    testCall.Async(writer);
                                }
                            },
                            testConfiguration.Synchronous == testSynchronousCall ?
                                null :
                                testSynchronousCall 
                                    ? ODataExpectedExceptions.ODataException("ODataParameterWriterCore_SyncCallOnAsyncWriter")
                                    : ODataExpectedExceptions.ODataException("ODataParameterWriterCore_AsyncCallOnSyncWriter"),
                            this.ExceptionVerifier);
                    }
                });
        }
    }
}
