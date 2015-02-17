//---------------------------------------------------------------------
// <copyright file="ServiceDocumentReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various ATOM service document payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ServiceDocumentReaderAtomTests : ODataReaderTestCase
    {
        private const string baseUri = "http://odata.org/";

        private PayloadReaderTestDescriptor.Settings settings;

        /// <summary>
        /// Gets or sets the dependency injector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public Microsoft.Test.Taupo.Contracts.IDependencyInjector Injector { get; set; }

        public PayloadReaderTestDescriptor.Settings Settings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = new PayloadReaderTestDescriptor.Settings();
                    this.Injector.InjectDependenciesInto(this.settings);
                    this.settings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new AtomObjectModelToPayloadElementConverter();
                }

                return this.settings;
            }
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the reading of ATOM service document payloads with ATOM metadata.")]
        public void ServiceDocumentReaderWithAtomMetadataTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region tests related to atom:title
                
                // Verify that a title inside a collection is read into the Name property and (if ATOM metadata reading is enabled) the Title property of the metadata annotation.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection("Products", baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                            xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                            <workspace>
                                                <collection href='" + baseUri + @"Products'>
                                                    <atom:title>Products</atom:title>
                                                </collection>
                                            </workspace>
                                            </service>"),
                },

                // Title inside a workspace should be read when ATOM metadata reading is on, but ignored otherwise.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .AtomTitle("Default", TestAtomConstants.AtomTextConstructTextKind)
                            .ResourceCollection("Products", baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <atom:title>Default</atom:title>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                    </collection>
                                                </workspace>
                                             </service>"),
                },

                // Multiple atom:title elements inside the collection element should throw when metadata reading is both on and off.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                    <workspace>
                                                        <collection href='" + baseUri + @"Products'>
                                                            <atom:title>Products</atom:title>
                                                            <atom:title>Products</atom:title>   
                                                        </collection>
                                                    </workspace>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound", "collection"),
                },
                
                #endregion tests related to atom:title

                #region app:collection element

                // Collection element with no children.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                    </collection>
                                                </workspace>
                                                </service>"),
                },
                
                // Empty collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products' />
                                                </workspace>
                                                </service>"),
                },

                // Multiple collections
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .AtomTitle("Default", TestAtomConstants.AtomTextConstructTextKind)
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Collection1", baseUri + "Collection1")
                                    .AppAccept("image/png")
                                    .AppOutOfLineCategories("http://somecategories.com"))
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Collection2", baseUri + "Collection2")
                                    .AppAccept("image/gif")
                                    .AppOutOfLineCategories("http://othercategories.com")))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <atom:title>Default</atom:title>
                                                    <collection href='" + baseUri + @"Collection1'>
                                                        <atom:title>Collection1</atom:title>
                                                        <accept>image/png</accept>
                                                        <categories href='http://somecategories.com'/>
                                                    </collection>
                                                    <collection href='" + baseUri + @"Collection2'>
                                                        <atom:title>Collection2</atom:title>
                                                        <accept>image/gif</accept>
                                                        <categories href='http://othercategories.com'/>
                                                    </collection>
                                                </workspace>
                                             </service>"),
                },

                #endregion app:collection element

                #region app:accept and app:categories element 

                // app:accept and app:categories element inside the collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppAccept("application/atom+xml;type=entry")
                                    .AppInlineCategories("yes", null,
                                        AtomMetadataBuilder.AtomCategory("SomeTerm", "http://someschema.org/", null))))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <accept>application/atom+xml;type=entry</accept>
                                                        <categories fixed='yes'>
                                                            <atom:category scheme='http://someschema.org/' term='SomeTerm'/>
                                                        </categories>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },

                // Out of line app:categories element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .AtomTitle("Default", TestAtomConstants.AtomTextConstructTextKind)
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppOutOfLineCategories("http://example.com/categories")))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <atom:title>Default</atom:title>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <categories href='http://example.com/categories' />
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },

                // Attributes of the "categories" element in a non-empty namespace should be ignored.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppInlineCategories(null, null,
                                        AtomMetadataBuilder.AtomCategory("SomeTerm", "http://someschema.org", "SomeLabel"))))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'
                                                xmlns:foo='http://foo.org'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <categories foo:fixed='yes' atom:scheme='http://baseschema.org'>
                                                            <atom:category term='SomeTerm' scheme='http://someschema.org' label='SomeLabel'/>
                                                        </categories>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },
                
                #endregion app:accept and app:categories element

                #region atom:category element

                // Attributes of the "category" element in a non-empty namespace should be ignored.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppInlineCategories("yes", "http://baseschema.org", 
                                        AtomMetadataBuilder.AtomCategory(null, null, null))))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'
                                                xmlns:foo='http://foo.org'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <categories fixed='yes' scheme='http://baseschema.org'>
                                                            <atom:category foo:term='SomeTerm' app:scheme='http://someschema.org' atom:label='SomeLabel'/>
                                                        </categories>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },

                // Multiple atom:category elements.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .AtomTitle("Default", TestAtomConstants.AtomTextConstructTextKind)
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppInlineCategories("no", "http://baseschema.org", 
                                        AtomMetadataBuilder.AtomCategory("FirstCategory", null, null),
                                        AtomMetadataBuilder.AtomCategory("SecondCategory", null, "SomeLabel"))))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <atom:title>Default</atom:title>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <categories fixed='no' scheme='http://baseschema.org'>
                                                            <atom:category term='FirstCategory'/>
                                                            <atom:category term='SecondCategory' label='SomeLabel'/>
                                                        </categories>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },

                #endregion atom:category element

                #region extra nodes

                // comments at different places in the service document.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <!-- some comments -->
                                                <workspace>
                                                    <!-- more comments -->
                                                    <collection href='" + baseUri + @"Products'>
                                                       <!-- more comments -->                                                    
                                                    </collection>
                                                    <!-- more comments -->
                                                </workspace>
                                                <!-- more comments -->            
                                              </service>"),
                },

                // elements having additional irrelevant attributes 
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom' foo2='bar2'>
                                                <workspace foo='bar'>
                                                    <collection href='" + baseUri + @"Products' foo1='bar1'>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                },

                // irrelevant child elements inside a workspace or collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(null, baseUri + "Products")
                            .ResourceCollection(null, baseUri + "Orders"))
                        .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <cn:foo foo1='bar1' />
                                                    <collection href='" + baseUri + @"Products'>
                                                        <cn:foo href='" + baseUri + @"Products'>
                                                        </cn:foo>
                                                    </collection>
                                                    <cn:foo2 foo3='bar3' />
                                                    <cn:foo2 foo3='bar3' />
                                                    <collection href='" + baseUri + @"Orders'>
                                                        <atom:foo href='" + baseUri + @"Orders'>
                                                        </atom:foo>
                                                    </collection>
                                                </workspace>
                                             </service>"),
                },

                // irrelevant element before and after the workspace 
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                       .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <cn:some_element>some text</cn:some_element>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                    </collection>
                                                </workspace>
                                                <cn:some_element>some text</cn:some_element>
                                             </service>"),
                },

                #endregion extra elements
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                new bool[] { true, false },
                (testDescriptor, testConfiguration, enableAtomMetadataReading) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.EnableAtomMetadataReading = enableAtomMetadataReading;

                    testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);

                    if (!enableAtomMetadataReading)
                    {
                        // If we are running with ATOM metadata reading turned off, strip off all ATOM metadata annotations and 
                        // properties from the expected result.
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveAtomMetadataFromPayloadElementVisitor.Visit);
                    }

                    // Normalize the payload elements so that if an ATOM metadata property is set, the corresponding ATOM metadata 
                    // annotation is created, and vice versa.
                    testDescriptor.ExpectedResultNormalizers.Add(tc => ODataPayloadElementAtomMetadataNormalizer.GenerateNormalizer(testConfiguration));

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the error reporting of invalid ATOM Metadata, and verfies that none of these cases cause errors when ATOM metadata reading is off.")]
        public void ServiceDocumentReaderInvalidAtomMetadataTest()
        {
            // The ExpectedExceptions in the following test descriptors only apply when ATOM metadata reading is on.
            // When ATOM metadata reading is off, invalid ATOM metadata should cause no failures, and ExpectedException is ignored.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Multiple atom:title elements inside the workspace element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection("Products", baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                    <workspace>
                                                        <atom:title>Default</atom:title>
                                                        <atom:title>Default</atom:title>
                                                        <collection href='" + baseUri + @"Products'>
                                                            <atom:title>Products</atom:title>   
                                                        </collection>
                                                    </workspace>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentMetadataDeserializer_MultipleTitleElementsFound", "workspace"),
                },

                // Invalid value for "fixed" attribute on a categories element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection("Products", baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                    <workspace>
                                                        <collection href='" + baseUri + @"Products'>
                                                            <atom:title>Products</atom:title>
                                                            <categories fixed='foo' scheme='http://baseschema.org'>
                                                                <atom:category term='Some Term'/>
                                                            </categories> 
                                                        </collection>
                                                    </workspace>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentMetadataDeserializer_InvalidFixedAttributeValue", "foo"),
                },

                // Multiple app:accept attributes inside a collection
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace()
                            .ResourceCollection(
                                PayloadBuilder.ResourceCollection("Products", baseUri + "Products")
                                    .AppAccept("application/atom+xml;type=entry")
                                    .AppInlineCategories("yes", null,
                                        AtomMetadataBuilder.AtomCategory("SomeTerm", "http://someschema.org/", null))))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                        <atom:title>Products</atom:title>
                                                        <accept>application/atom+xml;type=entry</accept>
                                                        <categories fixed='yes'>
                                                            <atom:category scheme='http://someschema.org/' term='SomeTerm'/>
                                                        </categories>
                                                        <accept>application/atom+xml;type=entry</accept>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                       ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentMetadataDeserializer_MultipleAcceptElementsFoundInCollection"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                new bool[] { true, false },
                (testDescriptor, testConfiguration, enableAtomMetadataReading) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.EnableAtomMetadataReading = enableAtomMetadataReading;

                    testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);

                    if (!enableAtomMetadataReading)
                    {
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveAtomMetadataFromPayloadElementVisitor.Visit);
                        testDescriptor.ExpectedException = null;
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the reading of ATOM service document payloads without ATOM metadata.")]
        public void ServiceDocumentReaderWithoutAtomMetadataTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> manualDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region extra nodes
                // collection element in a different namespace.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <cn:collection href='" + baseUri + @"Products'>
                                                    </cn:collection>
                                                </workspace>
                                             </service>"),
                },

                // collection element in a ATOM namespace.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <atom:collection href='" + baseUri + @"Products'>
                                                    </atom:collection>
                                                </workspace>
                                             </service>"),
                },

                // irrelevant child element inside a workspace element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <cn:foo href='" + baseUri + @"Products'>
                                                    </cn:foo>
                                                </workspace>
                                             </service>"),
                },
                #endregion extra nodes

                // Invalid reader tests
                #region Empty service document.
                
                // Service document with no children.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument()
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                              </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement"),
                },
                
                // Empty service.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument()
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom' />"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement"),
                },

                #endregion Empty service document.

                #region service element with wrong name or namespace

                // Service element not named as 'service'.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<myservice xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <workspace/>
                                                 </myservice>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace", "myservice", TestAtomConstants.AtomApplicationNamespace),
                },

                // Service element not in the 'app' namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<cn:service xmlns:cn='http://custom_namespace' xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <workspace/>
                                                 </cn:service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_ServiceDocumentRootElementWrongNameOrNamespace", "service", "http://custom_namespace"),
                },

                #endregion root element with wrong name or namespace

                #region workspace element with wrong name or namespace

                // Workspace element is not named as 'workspace'.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <myworkspace>
                                                      <collection href='" + baseUri + @"Products'/>
                                                   </myworkspace>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument", "myworkspace", TestAtomConstants.AtomApplicationNamespace),
                },

                // Workspace element not in the 'app' namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:cn='http://custom_namespace' xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <cn:workspace/>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_MissingWorkspaceElement"),
                },

                #endregion workspace element wrong name or namespace

                #region missing href
                
                // href in different namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:cn='http://customnamespace' xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection cn:href='" + baseUri + @"Products'>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ServiceDocumentElementUrlMustNotBeNull")
                },

                #endregion missing href

                #region multiple workspaces
                // multiple workspaces
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                    <collection href='" + baseUri + @"Products'>
                                                    </collection>
                                                </workspace>
                                                <workspace>
                                                    <collection href='Orders'>
                                                    </collection>
                                                </workspace>
                                              </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_MultipleWorkspaceElementsFound"),
                },

                #endregion multiple workspaces

                #region extra elements in ATOM publishing namespace

                // Element in 'app' namespace after the workspace element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <workspace/>
                                                   <foo baz='bar2'>somedata</foo>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument", "foo", TestAtomConstants.AtomApplicationNamespace),
                },

                // Element in 'app' namespace before the workspace document.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace())
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                    xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                   <foo baz='bar2'>somedata</foo>
                                                   <workspace/>
                                                 </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInServiceDocument", "foo", TestAtomConstants.AtomApplicationNamespace),
                },

                // Element in 'app' namespace before the collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                   <foo baz='bar2'>somedata</foo>
                                                   <collection href='" + baseUri + @"Products'>
                                                   </collection>
                                                </workspace> 
                                             </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace", "foo", TestAtomConstants.AtomApplicationNamespace),
                },

                // Element in 'app' namespace after the collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                   <collection href='" + baseUri + @"Products'>
                                                   </collection>
                                                   <foo baz='bar2'>somedata</foo>
                                                </workspace> 
                                             </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace", "foo", TestAtomConstants.AtomApplicationNamespace),
                },

                // Element in 'app' namespace inside the collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument().Workspace(
                        PayloadBuilder.Workspace().ResourceCollection(null, baseUri + "Products"))
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                   <collection href='" + baseUri + @"Products'>
                                                       <foo baz='bar2'>somedata</foo>                                                                     
                                                   </collection>
                                                </workspace> 
                                             </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInResourceCollection", "foo", TestAtomConstants.AtomApplicationNamespace),
                },

                // Nested workspace element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument()
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                   <workspace />
                                                </workspace> 
                                             </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInWorkspace", "workspace", TestAtomConstants.AtomApplicationNamespace),
                },

                // Nested collection element.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ServiceDocument()
                        .XmlRepresentation(@"<service xmlns:app='http://www.w3.org/2007/app' 
                                                xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>
                                                <workspace>
                                                   <collection href='" + baseUri + @"Products'>
                                                        <collection href='" + baseUri + @"Customers' />
                                                   </collection> 
                                                </workspace> 
                                             </service>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomServiceDocumentDeserializer_UnexpectedElementInResourceCollection", "collection", TestAtomConstants.AtomApplicationNamespace),
                },
                #endregion extra elements in ATOM publishing namespace
            };

            this.CombinatorialEngineProvider.RunCombinations(
            manualDescriptors,
            this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
            new bool[] { true, false },
            (testDescriptor, testConfiguration, enableAtomMetadataReading) =>
            {
                testConfiguration = new ReaderTestConfiguration(testConfiguration);
                testConfiguration.MessageReaderSettings.EnableAtomMetadataReading = enableAtomMetadataReading;
                testDescriptor.RunTest(testConfiguration);
            });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the reading of ATOM service document payloads without whitespace.")]
        public void ServiceDocumentNoWhitespaceTest()
        {
            var testCases = new[]
            {
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<atom:title>Products</atom:title>" +
                        "</collection>" +
                        "<collection href='http://odata.org/Orders'>" +
                            "<atom:title>Orders</atom:title>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<atom:title>Products</atom:title>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                        "</collection>" +
                        "<collection href='http://odata.org/Orders'>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'/>" +
                        "<collection href='http://odata.org/Orders'/>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'/>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace/>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<atom:title>Products</atom:title>" +
                            "<accept>image/png</accept>" +
                            "<categories href='http://somecategories.com'/>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<accept>image/png</accept>" +
                            "<categories href='http://somecategories.com'/>" +
                            "<atom:title>Products</atom:title>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<accept>image/png</accept>" +
                            "<categories href='http://somecategories.com'/>" +
                            "<atom:title>Products</atom:title>" +
                        "</collection>" +
                        "<collection href='http://odata.org/Orders'>" +
                            "<accept>image/png</accept>" +
                            "<categories href='http://somecategories.com'/>" +
                            "<atom:title>Orders</atom:title>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<cf:cf xmlns:cf='uri'/>" +
                        "</collection>" +
                        "<collection href='http://odata.org/Orders'>" +
                            "<cf:cf xmlns:cf='uri'/>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<cf:cf xmlns:cf='uri'/>" +
                        "</collection>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<collection href='http://odata.org/Products'>" +
                            "<cf:cf xmlns:cf='uri'/>" +
                        "</collection>" +
                        "<cf:cf xmlns:cf='uri'/>" +
                        "<collection href='http://odata.org/Orders'>" +
                            "<cf:cf xmlns:cf='uri'/>" +
                        "</collection>" +
                        "<cf:cf xmlns:cf='uri'/>" +
                    "</workspace>" +
                "</service>",
                "<service xmlns:app='http://www.w3.org/2007/app' " +
                     "xmlns='http://www.w3.org/2007/app' xmlns:atom='http://www.w3.org/2005/Atom'>" +
                     "<workspace>" +
                        "<cf:cf xmlns:cf='uri'/>" +
                    "</workspace>" +
                "</service>",
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                testCase =>
                {
                    // Just validate that the input strings above don't include any whitespaces.
                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(testCase)))
                    {
                        while (xmlReader.Read())
                        {
                            this.Assert.AreNotEqual(XmlNodeType.Whitespace, xmlReader.NodeType, "No whitespaces please");
                            this.Assert.AreNotEqual(XmlNodeType.SignificantWhitespace, xmlReader.NodeType, "No whitespaces please");
                        }
                    }

                    // Run the payload through the reader and make sure it doesn't fail.
                    TestResponseMessage message = new TestResponseMessage(new MemoryStream(Encoding.UTF8.GetBytes(testCase)));
                    message.SetContentType(ODataFormat.Atom, ODataPayloadKind.ServiceDocument);
                    ODataMessageReaderSettings settings = new ODataMessageReaderSettings { EnableAtom = true };

                    using (ODataMessageReader messageReader = new ODataMessageReader(message, settings))
                    {
                        messageReader.ReadServiceDocument();
                    }
                });
        }
    }
}
