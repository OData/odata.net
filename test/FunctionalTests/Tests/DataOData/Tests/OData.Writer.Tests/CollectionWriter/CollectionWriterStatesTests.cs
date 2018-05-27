//---------------------------------------------------------------------
// <copyright file="CollectionWriterStatesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.Platforms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for correct handling of collection writer states.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionWriterStatesTests : ODataWriterTestCase
    {
        private enum CollectionWriterAction
        {
            Start,
            Item,
            End,
            Error,
        }

        private class CollectionWriterStatesTestDescriptor
        {
            public string DebugDescription { get; set; }
            public Action<ODataMessageWriterTestWrapper, ODataCollectionWriter, TestStream> Setup { get; set; }
            public Dictionary<CollectionWriterAction, string> ExpectedResults { get; set; }
            public override string ToString() { return this.DebugDescription; }
            public Func<WriterTestConfiguration, bool> SkipForConfiguration { get; set; }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates correct checking of states in ODataCollectionWriter implementations.")]
        public void CollectionWriterStatesTest()
        {
            var testCases = new CollectionWriterStatesTestDescriptor[]
            {
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "Start",
                    Setup = null,
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, null },
                        { CollectionWriterAction.Item, "Cannot transition from state 'Start' to state 'Item'. The only valid actions in state 'Start' are to write the collection or to write nothing at all." },
                        { CollectionWriterAction.End, "ODataCollectionWriter.WriteEnd was called in an invalid state ('Start'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'." },
                        { CollectionWriterAction.Error, null },
                    }
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "Collection",
                    Setup = (mw, w, s) => {
                        w.WriteStart(new ODataCollectionStart { Name = "foo" });
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Collection' to state 'Collection'. The only valid actions in state 'Collection' are to write an item or to write the end of the collection." },
                        { CollectionWriterAction.Item, null },
                        { CollectionWriterAction.End, null },
                        { CollectionWriterAction.Error, null },
                    }
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "Item",
                    Setup = (mw, w, s) => {
                        w.WriteStart(new ODataCollectionStart { Name = "foo" });
                        w.WriteItem(42);
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Item' to state 'Collection'. The only valid actions in state 'Item' are to write an item or the end of the collection." },
                        { CollectionWriterAction.Item, null },
                        { CollectionWriterAction.End, null },
                        { CollectionWriterAction.Error, null },
                    }
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "Completed",
                    Setup = (mw, w, s) => {
                        w.WriteStart(new ODataCollectionStart { Name = "foo" });
                        w.WriteEnd();
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Completed' to state 'Collection'. Nothing further can be written once the writer has completed." },
                        { CollectionWriterAction.Item, "Cannot transition from state 'Completed' to state 'Item'. Nothing further can be written once the writer has completed." },
                        { CollectionWriterAction.End, "ODataCollectionWriter.WriteEnd was called in an invalid state ('Completed'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'." },
                        { CollectionWriterAction.Error, "Cannot transition from state 'Completed' to state 'Error'. Nothing further can be written once the writer has completed." },
                    }
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "ODataExceptionThrown",
                    Setup = (mw, w, s) => {
                        TestExceptionUtils.RunCatching(() => w.WriteItem(42));
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Error' to state 'Collection'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.Item, "Cannot transition from state 'Error' to state 'Item'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.End, "ODataCollectionWriter.WriteEnd was called in an invalid state ('Error'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'." },
                        { CollectionWriterAction.Error, null },
                    }
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "FatalExceptionThrown",
                    Setup = (mw, w, s) => {
                        // In JSON we can make the stream fail
                        s.FailNextCall = true;
                        w.WriteStart(new ODataCollectionStart { Name = "foo" });
                        TestExceptionUtils.RunCatching(() => w.Flush());
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Error' to state 'Collection'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.Item, "Cannot transition from state 'Error' to state 'Item'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.End, "ODataCollectionWriter.WriteEnd was called in an invalid state ('Error'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'." },
                        { CollectionWriterAction.Error, null },
                    },
                    // There's no simple way to make the writer go into a fatal exception state with XmlWriter underneath.
                    // XmlWriter will move to an Error state if anything goes wrong with it, and thus we can't write into it anymore.
                    // As a result for example the in-stream error case for this one can't work as it should.
                    SkipForConfiguration = (tc) => tc.Format != ODataFormat.Json
                },
                new CollectionWriterStatesTestDescriptor {
                    DebugDescription = "Error",
                    Setup = (mw, w, s) => {
                        mw.WriteError(new ODataError(), false);
                    },
                    ExpectedResults = new Dictionary<CollectionWriterAction,string> { 
                        { CollectionWriterAction.Start, "Cannot transition from state 'Error' to state 'Collection'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.Item, "Cannot transition from state 'Error' to state 'Item'. Nothing can be written once the writer entered the error state." },
                        { CollectionWriterAction.End, "ODataCollectionWriter.WriteEnd was called in an invalid state ('Error'); WriteEnd is only supported in states 'Start', 'Collection', and 'Item'." },
                        { CollectionWriterAction.Error, "The WriteError method or the WriteErrorAsync method on the ODataMessageWriter has already been called to write an error payload. Only a single error payload can be written with each ODataMessageWriter instance." },
                    }
                },
            };

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                EnumExtensionMethods.GetValues<CollectionWriterAction>().Cast<CollectionWriterAction>(),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testCase, writerAction, testConfiguration) =>
                {
                    using (TestStream stream = new TestStream())
                    {
                        if (testCase.SkipForConfiguration != null && testCase.SkipForConfiguration(testConfiguration)) return;

                        // We purposely don't use the using pattern around the messageWriter here. Disposing the message writer will
                        // fail here because the writer is not left in a valid state.
                        var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert);
                        ODataCollectionWriter writer = messageWriter.CreateODataCollectionWriter();
                        if (testCase.Setup != null) { testCase.Setup(messageWriter, writer, stream); }

                        string expectedException = testCase.ExpectedResults[writerAction];

                        this.Assert.ExpectedException<ODataException>(
                            () => InvokeCollectionWriterAction(messageWriter, writer, writerAction),
                            expectedException);
                    }
                });
        }

        private void InvokeCollectionWriterAction(ODataMessageWriterTestWrapper messageWriter, ODataCollectionWriter writer, CollectionWriterAction writerAction)
        {
            switch (writerAction)
            {
                case CollectionWriterAction.Start:
                    writer.WriteStart(new ODataCollectionStart { Name = "foo" });
                    break;
                case CollectionWriterAction.Item:
                    writer.WriteItem(42);
                    break;
                case CollectionWriterAction.End:
                    writer.WriteEnd();
                    break;
                case CollectionWriterAction.Error:
                    messageWriter.WriteError(new ODataError(), false);
                    break;
            }
        }
    }
}
