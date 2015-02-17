//---------------------------------------------------------------------
// <copyright file="ODataAtomMetadataReaderTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Base class for test cases which deal with ATOM metadata reading.
    /// </summary>
    public class ODataAtomMetadataReaderTestCase : ODataReaderTestCase
    {
        private PayloadReaderTestDescriptor.Settings settings;

        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public Microsoft.Test.Taupo.Contracts.IDependencyInjector Injector { get; set; }

        /// <summary>
        /// Accesses the test descriptor settings, which are set up to use ATOM-specific ObjectModel to PayloadElement conversion.
        /// </summary>
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

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomPersonConstruct ("author" and "contributor").
        /// </summary>
        /// <typeparam name="T">The type of payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload element to start with.</param>
        /// <param name="payloadElementName">The name of the payload element as it appears in ATOM.</param>
        /// <param name="personElementName">The name of the ATOM person contruct metadata element to create test descriptors for.</param>
        /// <param name="addPersonAtomMetadataAnnotation">A Func which takes an entity set instance, a name of a person, a string URI for the person,
        /// and an email address and produces the entity set instance with the added person construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        protected IEnumerable<PayloadReaderTestDescriptor> CreatePersonConstructTestDescriptors<T>(
            T payloadElement,
            string payloadElementName,
            string personElementName,
            Func<T, string, string, string, T> addPersonAtomMetadataAnnotation) where T : ODataPayloadElement
        {
            return new PayloadReaderTestDescriptor[]
            {
                // Single person construct with name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addPersonAtomMetadataAnnotation(payloadElement.DeepCopy(), "John Smith", null, null)
                },
                // Single person construct with name, uri, and email
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addPersonAtomMetadataAnnotation(payloadElement.DeepCopy(), "John Smith", "http://johnsmith.com", "john@smith.com")
                },
                // Multiple person constructs
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addPersonAtomMetadataAnnotation(
                                        addPersonAtomMetadataAnnotation(payloadElement.DeepCopy(), "John Smith", null, null),
                                        "Jane Doe", null, null),
                },
                // Person construct element in non-ATOM namespace ignored
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                            @"<{0} xmlns:cn='http://customnamespace.com'>
                                                <cn:{1}>
                                                    <name>John Smith</name>
                                                </cn:{1}>
                                            </{0}>", payloadElementName, personElementName)),
                },
            };
        }

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomTextConstruct ("rights", "title", and "summary").
        /// </summary>
        /// <typeparam name="T">The type of payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload element to start with.</param>
        /// <param name="payloadElementName">The name of the payload element as it appears in ATOM.</param>
        /// <param name="textConstructElementName">The name of the ATOM text construct metadata element to create test descriptors for.</param>
        /// <param name="addTextAtomMetadataAnnotation">A Func which takes an entity instance, the value of the text construct element, 
        /// and the name of the text construct kind and produces the entity instance with the added text construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        protected IEnumerable<PayloadReaderTestDescriptor> CreateTextConstructTestDescriptors<T>(
            T payloadElement,
            string payloadElementName,
            string textConstructElementName,
            Func<T, string, string, T> addTextAtomMetadataAnnotation) where T : ODataPayloadElement
        {
            return new PayloadReaderTestDescriptor[]
            {
                // Text construct with "text" type (implicit).
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addTextAtomMetadataAnnotation(payloadElement.DeepCopy(), "Copyright (c) 1999, Some Company", TestAtomConstants.AtomTextConstructTextKind)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><{1}>Copyright (c) 1999, Some Company</{1}></{0}>", payloadElementName, textConstructElementName))
                },
                // Text construct with "text" type (explicit).
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addTextAtomMetadataAnnotation(payloadElement.DeepCopy(), "Copyright (c) 1999, Some Company", TestAtomConstants.AtomTextConstructTextKind)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><{1} type='text'>Copyright (c) 1999, Some Company</{1}></{0}>", payloadElementName, textConstructElementName))
                },
                // Text construct with "html" type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addTextAtomMetadataAnnotation(payloadElement.DeepCopy(), "<b>Copyright</b> (c) 1999, <i>Some Company</i>", TestAtomConstants.AtomTextConstructHtmlKind)
                },
                // Text construct with "xhtml" type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    // Note: extra whitespace is added by System.XmlTextReader's implementation of ReadInnerXml.  As long as it's parseable XML, doesn't really matter.
                    PayloadElement = addTextAtomMetadataAnnotation(payloadElement.DeepCopy(), "\n    <div xmlns=\"http://www.w3.org/2005/Atom\">Copyright (c) 1999, Some Company</div>\n  ", TestAtomConstants.AtomTextConstructXHtmlKind)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><{1} type='xhtml'><div>Copyright (c) 1999, Some Company</div></{1}></{0}>", payloadElementName, textConstructElementName))
                },
                // All three text construct elements (rights, summary, and title) can appear at most once, so fail if multiple appear (and ATOM metadata reading is on).
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        @"<{0}>
                            <{1}>Copyright (c) 1999, Some Company</{1}>
                            <{1}>An additional text construct</{1}>
                          </{0}>", payloadElementName, textConstructElementName)),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements", textConstructElementName, payloadElementName)
                }
            };
        }

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomDateConstruct ("updated" and "published").
        /// </summary>
        /// <typeparam name="T">The type of payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload elemet to start with.</param>
        /// <param name="payloadElementName">The name of the payload element as it appears in ATOM.</param>
        /// <param name="dateConstructElementName">The name of the ATOM date construct metadata element to create test descriptors for.</param>
        /// <param name="addDateAtomMetadataAnnotation">A Func which takes a payload element and a date time string 
        /// and produces the payload element with the added date construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        protected IEnumerable<PayloadReaderTestDescriptor> CreateDateConstructTestDescriptors<T>(
            T payloadElement,
            string payloadElementName,
            string dateConstructElementName,
            Func<T, string, T> addDateAtomMetadataAnnotation) where T : ODataPayloadElement
        {
            return new PayloadReaderTestDescriptor[]
            {
                // Basic date time in GMT
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addDateAtomMetadataAnnotation(payloadElement.DeepCopy(), "2011-09-08T08:55:00Z")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><{1}>2011-09-08T08:55:00Z</{1}></{0}>", payloadElementName, dateConstructElementName))
                },
                // Date time in different time zone
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addDateAtomMetadataAnnotation(payloadElement.DeepCopy(), "2011-09-08T16:55:00Z")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><{1}>2011-09-08T08:55:00-08:00</{1}></{0}>", payloadElementName, dateConstructElementName))
                },
                // More than one "published" or "updated" element should cause failure if we're reading ATOM metadata.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                                            @"<{0}>
                                                <{1}>2011-09-08T08:55:00Z</{1}>
                                                <{1}>2010-09-08T08:55:00Z</{1}>
                                            </{0}>", payloadElementName, dateConstructElementName)),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements", dateConstructElementName, payloadElementName)
                },
            };
        }

        /// <summary>
        /// Constructs test descriptors which test category ATOM metadata.
        /// </summary>
        /// <typeparam name="T">The type of payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload element to start with.</param>
        /// <param name="payloadElementName">The name of the payload element as it appears in ATOM.</param>
        /// <returns>A list of test descriptors.</returns>
        protected IEnumerable<PayloadReaderTestDescriptor> CreateCategoryTestDescriptors<T>(
            T payloadElement,
            string payloadElementName) where T : ODataPayloadElement
        {
            return new PayloadReaderTestDescriptor[]
            {
                // Category with no attributes.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy().AtomCategory(null, null, null)
                },
                // Category with scheme, term, and label filled in.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy().AtomCategory("someTerm", "http://some.scheme", "someLabel")
                },
                // Category with attributes in wrong namespace.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy().AtomCategory(null, null, null)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><category cn:term='someTerm' cn:scheme='http://some.scheme' cn:label='someLabel' xmlns:cn='http://custom.namespace.com'/></{0}>", payloadElementName))
                },
                // Category with attributes not defined by the ATOM spec.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy().AtomCategory(null, null, null)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, 
                        "<{0}><category bla='something' baz='bar' /></{0}>", payloadElementName))
                },
                // Multiple (different) categories
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .AtomCategory("someTerm", "http://some.scheme", "someLabel")
                        .AtomCategory("otherTerm", "http://other.scheme", "otherLabel")
                },
                // Multiple (identical) categories
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .AtomCategory("someTerm", "http://some.scheme", "someLabel")
                        .AtomCategory("someTerm", "http://some.scheme", "someLabel")
                },
            };
        }

        /// <summary>
        /// Runs each test descriptor with each ATOM reader test configuration and with ATOM metadata both enabled and disabled.
        /// </summary>
        /// <param name="testDescriptors">The test descriptors to run</param>
        /// <param name="runOnlyWithMetadataReadingOn">If true, then the test descriptors are only run with ATOM metadata reading enabled; false means run each descriptor with ATOM metadata reading both enabled and disabled.</param>
        protected void RunAtomMetadataReaderTests(IEnumerable<PayloadReaderTestDescriptor> testDescriptors, bool runOnlyWithMetadataReadingOn = false)
        {
            bool[] enableMetadataReadingOptions = runOnlyWithMetadataReadingOn
                ? new bool[] { true }
                : new bool[] { true, false };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                enableMetadataReadingOptions,
                (testDescriptor, testConfiguration, enableMetadataReading) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.EnableAtomMetadataReading = enableMetadataReading;

                    testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);

                    // Normalize the payload elements so that if an ATOM metadata property is set, the corresponding ATOM metadata 
                    // annotation is created, and vice versa.
                    testDescriptor.ExpectedResultNormalizers.Add(tc => ODataPayloadElementAtomMetadataNormalizer.GenerateNormalizer(tc));

                    if (!enableMetadataReading)
                    {
                        // If we are running with ATOM metadata reading turned off, strip off all ATOM metadata annotations and 
                        // properties from the expected result.
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveAtomMetadataFromPayloadElementVisitor.Visit);

                        // Association links are only recognized in response and MPV >= V3
                        if (testConfiguration.IsRequest)
                        {
                            testDescriptor.ExpectedResultNormalizers.Add(tc =>
                                (payloadElement => RemoveAssociationLinkPayloadElementNormalizer.Normalize(payloadElement)));
                        }

                        // Stream properties are only recognized in response and >=V3
                        if (testConfiguration.IsRequest)
                        {
                            testDescriptor.ExpectedResultNormalizers.Add(tc =>
                                (payloadElement => RemoveStreamPropertyPayloadElementNormalizer.Normalize(payloadElement)));
                        }

                        // In this test class, expected exceptions apply only when ATOM metadata reading is on.
                        testDescriptor.ExpectedException = null;
                    }
                    else
                    {
                        // In requests when metadata reading is enabled we have to turn stream properties and association links
                        // into Atom metadata (and XmlTree annotation instances)
                        testDescriptor.ExpectedResultNormalizers.Add(tc => (payloadElement) => ConvertAtomMetadataForConfigurationPayloadElementNormalizer.Normalize(payloadElement, tc));
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
