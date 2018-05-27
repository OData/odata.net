//---------------------------------------------------------------------
// <copyright file="ParameterWriterStatesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.ParameterWriter
{
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for ODataParameterWriter state machine.
    /// </summary>
    [TestClass, TestCase]
    public class ParameterWriterStatesTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Error test cases for the parameter writer state machine.")]
        public void ParameterWriterStateMachineErrorTests()
        {
            var testCases = new ParameterWriterStateMachineTestCase[]
            {
                // WriteStart can only be called once and it must be called before writing anything else.
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteStart();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteStart"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteValue("p1", null);
                        writer.WriteStart();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteStart"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.CreateCollectionWriter("p1");
                        writer.WriteStart();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteStart"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteEnd();
                        writer.WriteStart();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteStart"),
                },

                // WriteValue and CreateCollectionWriter can only be called after WriteStart and before WriteEnd. And they cannot be called until the previously created sub-writer is completed.
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteValue("p1", null);
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteParameter"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.CreateCollectionWriter("p1");
                        writer.WriteValue("p1", null);
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteParameter"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.CreateCollectionWriter("p1");
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteParameter"),
                },

                // WriteEnd can only be called after WriteStart and after the previously created sub-writer is completed.
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteEnd();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteEnd"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.CreateCollectionWriter("p1");
                        writer.WriteEnd();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteEnd"),
                },

                // The writer is in error or completed state. No further writes can be performed on this writer.
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteEnd();
                        writer.WriteValue("p1", null);
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteInErrorOrCompletedState"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteEnd();
                        writer.CreateCollectionWriter("p1");
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteInErrorOrCompletedState"),
                },
                new ParameterWriterStateMachineTestCase
                {
                    WriteActions = writer =>
                    {
                        writer.WriteStart();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteInErrorOrCompletedState"),
                },
                // TODO: Add test cases to cause the writer enter error state then verify that no writes can be performed afterwards.
            };

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException };
                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback);

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        ODataParameterWriter writer = messageWriter.CreateODataParameterWriter(null /*functionImport*/);
                        testCase.WriteActions(writer);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        private class ParameterWriterStateMachineTestCase
        {
            public Action<ODataParameterWriter> WriteActions { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}
