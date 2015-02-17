//---------------------------------------------------------------------
// <copyright file="FeedPayloadOrderReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests payload order reading of feeds in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class FeedPayloadOrderReaderAtomTests : ODataPayloadOrderReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct payload order of items in a feed.")]
        public void FeedPayloadOrderTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Nothing
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .XmlRepresentation("<feed></feed>")
                        .PayloadOrderItems("__StartFeed__")
                },
                // No entries
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomId("urn:feedid")
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .XmlRepresentation("<feed><id>urn:feedid</id><m:count>42</m:count><link rel='next' href='http://odata.org/next'/></feed>")
                        .PayloadOrderItems("Id, Count, NextPageLink, __StartFeed__")
                },
                // Single entry, properties after the entry
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomId("urn:feedid")
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .Append(PayloadBuilder.Entity()
                            .Id("urn:entryid")
                            .PayloadOrderItems("Id", "__StartEntry__"))
                        .XmlRepresentation("<feed><entry><id>urn:entryid</id></entry><id>urn:feedid</id><m:count>42</m:count><link rel='next' href='http://odata.org/next'/></feed>")
                        .PayloadOrderItems("__StartFeed__, Entry_urn:entryid, Id, Count, NextPageLink")
                },
                // Single entry, properties before the entry
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomId("urn:feedid")
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .Append(PayloadBuilder.Entity()
                            .Id("urn:entryid")
                            .PayloadOrderItems("Id", "__StartEntry__"))
                        .XmlRepresentation("<feed><id>urn:feedid</id><m:count>42</m:count><link rel='next' href='http://odata.org/next'/><entry><id>urn:entryid</id></entry></feed>")
                        .PayloadOrderItems("Id, Count, NextPageLink, __StartFeed__, Entry_urn:entryid")
                },
                // Two entries, properties in between the entries
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomId("urn:feedid")
                        .InlineCount(42)
                        .NextLink("http://odata.org/next")
                        .Append(PayloadBuilder.Entity()
                            .Id("urn:entryid")
                            .PayloadOrderItems("Id", "__StartEntry__"))
                        .Append(PayloadBuilder.Entity()
                            .Id("urn:entryid2")
                            .PayloadOrderItems("Id", "__StartEntry__"))
                        .XmlRepresentation("<feed><entry><id>urn:entryid</id></entry><id>urn:feedid</id><m:count>42</m:count><link rel='next' href='http://odata.org/next'/><entry><id>urn:entryid2</id></entry></feed>")
                        .PayloadOrderItems("__StartFeed__, Entry_urn:entryid, Id, Count, NextPageLink, Entry_urn:entryid2")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
