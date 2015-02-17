//---------------------------------------------------------------------
// <copyright file="AtomMetadataReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of ATOM metadata not specific to feeds, entries, or service documents.
    /// </summary>
    [TestClass, TestCase]
    public class AtomMetadataReaderTests : ODataAtomMetadataReaderTestCase
    {
        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that invalid URI values in ATOM metadata fail.")]
        public void TestInvalidUriInAtomMetadata()
        {
            // A string which breaks URI syntax, as defined by RFC 2396
            const string invalidUri = "http://::";

            // Other unimportant values to use for creating payloads to test.
            const string personName = "Name";
            const string personEmail = "email@domain.com";
            const string collectionTitle = "Collection";
            const string validUri = "http://odata.org";
            const string generatorName = "Test Generator";
            const string generatorVersion = "1.0";
            const string nonOdataRelValue = "someRel";
            const string linkType = "text/html";

            // Create a test descriptor for each possible place the spec says a Uri can appear and is exposed as ATOM metadata.
            // Note: This excludes the two spots where the spec says the value is a Uri, but we expose it as a string: 
            //       scheme attributes of category and categories elements, and the id of the source feed of an entry.
            //       This also excludes any Uri values that are always parsed, whether or not ATOM metadata reading is on,
            //       such as the href attribute in a collection element.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region uri attribute in a generator
                // atom:feed/atom:generator/@uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator(generatorName, invalidUri, generatorVersion),
                },
                // atom:entry/atom:source/atom:generator/@uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomGenerator(generatorName, invalidUri, generatorVersion)),
                },
                #endregion uri attribute in a generator

                #region uri element in person constructs
                // atom:feed/atom:author/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomAuthor(personName, invalidUri, personEmail),
                },
                // atom:feed/atom:contributor/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomContributor(personName, invalidUri, personEmail),
                },
                // atom:entry/atom:author/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomAuthor(personName, invalidUri, personEmail),
                },
                // atom:entry/atom:contributor/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomContributor(personName, invalidUri, personEmail),
                },
                // atom:entry/atom:source/atom:author/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomAuthor(personName, invalidUri, personEmail)),
                },
                // atom:entry/atom:source/atom:contributor/atom:uri
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomContributor(personName, invalidUri, personEmail)),
                },
                #endregion uri element in person constructs

                #region content of icon element
                // atom:feed/atom:icon
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomIcon(invalidUri),
                },
                // atom:entry/atom:source/atom:icon
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomIcon(invalidUri)),
                },
                #endregion content of icon element
                
                #region content of logo element
                // atom:feed/atom:logo
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomLogo(invalidUri),
                },
                // atom:entry/atom:source/atom:logo
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomLogo(invalidUri)),
                },
                #endregion content of logo element

                #region href attribute in a link
                // atom:feed/atom:link/@href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomLink(invalidUri, nonOdataRelValue, linkType),
                },
                // atom:entry/atom:link/@href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomLink(invalidUri, nonOdataRelValue, linkType),
                },
                // atom:entry/atom:source/atom:link/@href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet().AtomLink(invalidUri, nonOdataRelValue, linkType)),
                },
                #endregion href attribute in a link

                #region href attribute in an out-of-line categories element
                // app:service/app:workspace/app:collection/app:categories/@href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument()
                        .Workspace(PayloadBuilder.Workspace()
                            .ResourceCollection(PayloadBuilder.ResourceCollection(collectionTitle, validUri)
                                .AppOutOfLineCategories(invalidUri))),
                    SkipTestConfiguration = tc => tc.IsRequest,
                },
                #endregion href attribute in an out-of-line categories element
            };

            // Add the expected exception to all test descriptors.
            testDescriptors = testDescriptors.Select(td =>
                new PayloadReaderTestDescriptor(td)
                {
                    ExpectedException = new ExpectedException(typeof(System.UriFormatException)),
                });

            // This method is set up so that expected exceptions are only expected when ATOM metadata reading is on.
            this.RunAtomMetadataReaderTests(testDescriptors);
        }
    }
}
