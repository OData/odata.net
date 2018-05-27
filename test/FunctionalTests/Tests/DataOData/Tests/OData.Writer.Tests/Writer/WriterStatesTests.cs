//---------------------------------------------------------------------
// <copyright file="WriterStatesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
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
    /// Tests for correct handling of writer states.
    /// </summary>
    [TestClass, TestCase]
    public class WriterStatesTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        private enum WriterAction
        {
            StartResource,
            StartFeed,
            StartLink,
            End,
            Error
        };

        private class WriterStatesTestDescriptor
        {
            public WriterStatesTestDescriptor()
            {
            }

            public WriterStatesTestDescriptor(WriterStatesTestDescriptor other)
            {
                this.Setup = other.Setup;
                this.ExpectedResults = new Dictionary<WriterAction, ExpectedException>(other.ExpectedResults);
                this.SkipForTestConfiguration = other.SkipForTestConfiguration;
            }

            public Action<ODataMessageWriterTestWrapper, ODataWriter, TestStream> Setup { get; set; }
            public Dictionary<WriterAction, ExpectedException> ExpectedResults { get; set; }
            public Func<WriterTestConfiguration, bool> SkipForTestConfiguration { get; set; }
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates correct checking of states in ODataWriter for Feed implementations.")]
        public void FeedWriterStatesTest()
        {
            WriterStatesTestImplementation(true);
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates correct checking of states in ODataWriter for Entry implementations.")]
        public void EntryWriterStatesTest()
        {
            WriterStatesTestImplementation(false);
        }

        private void WriterStatesTestImplementation(bool feedWriter)
        {
            var testCases = new WriterStatesTestDescriptor[]
            {
                // Start
                new WriterStatesTestDescriptor {
                    Setup = null,
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, feedWriter ? ODataExpectedExceptions.ODataException("ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter") : (ExpectedException)null },
                        { WriterAction.StartFeed, feedWriter ? (ExpectedException)null : ODataExpectedExceptions.ODataException("ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromStart", "Start", "NavigationLink") },
                        { WriterAction.End, ODataExpectedExceptions.ODataException("ODataWriterCore_WriteEndCalledInInvalidState", "Start") },
                        { WriterAction.Error, null },
                    }
                },

                // Entry
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => { 
                        if (feedWriter) { w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); }
                        w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); 
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromResource", "Entry", "Entry") },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromResource", "Entry", "Feed") },
                        { WriterAction.StartLink, null },
                        { WriterAction.End, null },
                        { WriterAction.Error, null },
                    }
                },

                // Feed
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => { 
                        if (feedWriter) { w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); }
                        else { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.WriteStart(ObjectModelUtils.CreateDefaultCollectionLink()); w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); }
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, null },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromResourceSet", "Feed", "Feed") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromResourceSet", "Feed", "NavigationLink") },
                        { WriterAction.End, null },
                        { WriterAction.Error, null },
                    }
                },

                // Link - single
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => { 
                        if (feedWriter) { w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); }
                        w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); 
                        w.WriteStart(new ODataNestedResourceInfo { Name = ObjectModelUtils.DefaultLinkName, Url = ObjectModelUtils.DefaultLinkUrl, IsCollection = false });
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, null },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent", "http://odata.org/link") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidStateTransition", "NavigationLink", "NavigationLink") },
                        { WriterAction.End, null },
                        { WriterAction.Error, null },
                    },
                    SkipForTestConfiguration = tc => tc.IsRequest
                },

                // Link - collection
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => { 
                        if (feedWriter) { w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); }
                        w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); 
                        w.WriteStart(ObjectModelUtils.CreateDefaultCollectionLink());
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, ODataExpectedExceptions.ODataException("WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent", "http://odata.org/link") },
                        { WriterAction.StartFeed, null },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidStateTransition", "NavigationLink", "NavigationLink") },
                        { WriterAction.End, null },
                        { WriterAction.Error, null },
                    },
                    SkipForTestConfiguration = tc => tc.IsRequest
                },

                // Expanded link - there's no way to get to the expanded link state alone since the writer will always
                //   immediately transition to either entry or feed state instead.

                // Completed
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => {
                        if (feedWriter) { w.WriteStart(ObjectModelUtils.CreateDefaultFeed()); w.WriteEnd(); }
                        else { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.WriteEnd(); }
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromCompleted", "Completed", "Entry") },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromCompleted", "Completed", "Feed") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromCompleted", "Completed", "NavigationLink") },
                        { WriterAction.End, ODataExpectedExceptions.ODataException("ODataWriterCore_WriteEndCalledInInvalidState", "Completed") },
                        { WriterAction.Error, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromCompleted", "Completed", "Error") },
                    }
                },

                // ODataExceptionThrown
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => {
                        TestExceptionUtils.RunCatching(() => w.WriteStart(ObjectModelUtils.CreateDefaultCollectionLink()));
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "Entry") },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "Feed") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "NavigationLink") },
                        { WriterAction.End, ODataExpectedExceptions.ODataException("ODataWriterCore_WriteEndCalledInInvalidState", "Error") },
                        { WriterAction.Error, null },
                    },
                    SkipForTestConfiguration = tc => tc.IsRequest,
                },

                // Error
                new WriterStatesTestDescriptor {
                    Setup = (mw, w, s) => {
                        mw.WriteError(new ODataError(), false);
                    },
                    ExpectedResults = new Dictionary<WriterAction, ExpectedException> { 
                        { WriterAction.StartResource, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "Entry") },
                        { WriterAction.StartFeed, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "Feed") },
                        { WriterAction.StartLink, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "NavigationLink") },
                        { WriterAction.End, ODataExpectedExceptions.ODataException("ODataWriterCore_WriteEndCalledInInvalidState", "Error") },
                        { WriterAction.Error, ODataExpectedExceptions.ODataException("ODataMessageWriter_WriteErrorAlreadyCalled") },
                    },
                    SkipForTestConfiguration = tc => tc.IsRequest,
                },
            };

            ExpectedException errorNotAllowedException = ODataExpectedExceptions.ODataException("ODataMessageWriter_ErrorPayloadInRequest");

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                EnumExtensionMethods.GetValues<WriterAction>().Cast<WriterAction>(),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, writerAction, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testCase.SkipForTestConfiguration != null && testCase.SkipForTestConfiguration(testConfiguration))
                    {
                        return;
                    }

                    ExpectedException expectedExceptionOnError;
                    if (testCase.ExpectedResults.TryGetValue(WriterAction.Error, out expectedExceptionOnError))
                    {
                        if (testConfiguration.IsRequest)
                        {
                            testCase = new WriterStatesTestDescriptor(testCase);
                            testCase.ExpectedResults[WriterAction.Error] = errorNotAllowedException;
                        }
                    }

                    using (TestStream stream = new TestStream())
                    {
                        // We purposely don't use the using pattern around the messageWriter here. Disposing the message writer will
                        // fail here because the writer is not left in a valid state.
                        var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert);
                        ODataWriter writer = messageWriter.CreateODataWriter(feedWriter);

                        TestExceptionUtils.ExpectedException(
                            this.Assert,
                            () => 
                            {
                                if (testCase.Setup != null) testCase.Setup(messageWriter, writer, stream);
                                this.InvokeWriterAction(messageWriter, writer, writerAction); 
                            },
                            testCase.ExpectedResults[writerAction],
                            this.ExceptionVerifier);
                    }
                });
        }

        private void InvokeWriterAction(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, WriterAction writerAction)
        {
            switch (writerAction)
            {
                case WriterAction.StartResource:
                    writer.WriteStart(ObjectModelUtils.CreateDefaultEntry());
                    break;
                case WriterAction.StartFeed:
                    writer.WriteStart(ObjectModelUtils.CreateDefaultFeed());
                    break;
                case WriterAction.StartLink:
                    writer.WriteStart(ObjectModelUtils.CreateDefaultCollectionLink());
                    break;
                case WriterAction.End:
                    writer.WriteEnd();
                    break;
                case WriterAction.Error:
                    messageWriter.WriteError(new ODataError(), false);
                    break;
            }
        }
    }
}
