//---------------------------------------------------------------------
// <copyright file="UriHandlingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;
    #endregion Namespaces

    /// <summary>
    /// Tests handling of URIs in payloads.
    /// </summary>
    [TestClass, TestCase]
    public class UriHandlingTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToXmlConverter PayloadElementToXmlConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementToJsonConverter PayloadElementToJsonConverter { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Gets or sets the dependency injector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public Microsoft.Test.Taupo.Contracts.IDependencyInjector Injector { get; set; }

        private PayloadReaderTestDescriptor.Settings noValidatorSettings;

        public PayloadReaderTestDescriptor.Settings NoValidatorSettings
        {
            get
            {
                if (this.noValidatorSettings == null)
                {
                    this.noValidatorSettings = new PayloadReaderTestDescriptor.Settings();
                    this.Injector.InjectDependenciesInto(this.noValidatorSettings);
                    this.noValidatorSettings.ExpectedResultSettings.DefaultODataObjectModelValidator = new Microsoft.Test.Taupo.OData.Reader.Tests.Common.AggregateODataObjectModelValidator();
                }

                return this.noValidatorSettings;
            }
        }

        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);

            // We have to register JSON converter for this test case explicitely since we're using the batch infrastructure
            // which doesn't inherit the JSON converter from the PayloadReaderTestDescriptor.Settings.
            container.Register<IPayloadElementToJsonConverter, AnnotatedPayloadElementToJsonConverter>();
            container.Register<IPayloadElementToJsonLightConverter, AnnotatedPayloadElementToJsonLightConverter>();
        }

        /// <summary>
        /// Set of absolute and relative URIs used in the tests as payload URIs.
        /// </summary>
        private static readonly Uri[] payloadUris = new Uri[]
        {
            new Uri("http://odata.org/absolute"),
            new Uri(string.Empty, UriKind.Relative),
            new Uri("relative", UriKind.Relative),
        };

        /// <summary>Default base URI from the xml:base attribute</summary>
        private static readonly Uri xmlBaseUri = new Uri("http://odata.org/xmlbase");

        /// <summary>Default base URI from the reader settings.</summary>
        private static readonly Uri readerSettingBaseUri = new Uri("http://odata.org/readersetting/");

        /// <summary>All interesting combinations of xml:base and reader settings-specified base URIs.</summary>
        private static readonly BaseUriValue[] baseUriValues = new BaseUriValue[]
        {
            // No base URIs
            new BaseUriValue { XmlBaseUri = null, ReaderSettingBaseUri = null, },
            // Only Xml base URI
            new BaseUriValue { XmlBaseUri = xmlBaseUri, ReaderSettingBaseUri = null, },
            // Only reader setting base URI
            new BaseUriValue { XmlBaseUri = null, ReaderSettingBaseUri = readerSettingBaseUri, },
            //Both base URIs
            new BaseUriValue { XmlBaseUri = xmlBaseUri, ReaderSettingBaseUri = readerSettingBaseUri, },
        };

        private static readonly Uri resolverResultRelative = new Uri("resolverelative", UriKind.Relative);
        private static readonly Uri resolverResultAbsolute = new Uri("http://odata.org/resolverabsolute");

        /// <summary>Resolvers to use.</summary>
        private static readonly KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>[] resolvers = new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>[]
        {
            // No resolver
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>(null, null),
            // Resolver which always returns relative URL
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>((inputUri, baseUri, payloadUri) =>
                {
                    return (inputUri.OriginalString == payloadUri.OriginalString) ? resolverResultRelative : null;
                },
                resolverResultRelative),
            // Resolver which always returns absolute URL
            new KeyValuePair<Func<Uri, Uri, Uri, Uri>, Uri>((inputUri, baseUri, payloadUri) =>
                {
                    return (inputUri.OriginalString == payloadUri.OriginalString) ? resolverResultAbsolute : null;
                },
                resolverResultAbsolute),
        };

        /// <summary>
        /// A base URI configuration specifying a base URI to use on the Xml reader 
        /// and a base URI to use on the reader settings.
        /// </summary>
        private sealed class BaseUriValue
        {
            public Uri XmlBaseUri { get; set; }
            public Uri ReaderSettingBaseUri { get; set; }

            public override string ToString()
            {
                string xmlBase = this.XmlBaseUri == null ? "<null>" : this.XmlBaseUri.OriginalString;
                string settingsBase = this.ReaderSettingBaseUri == null ? "<null>" : this.ReaderSettingBaseUri.OriginalString;
                return "XmlBaseUri: " + xmlBase + "; SettingsBaseUri: " + settingsBase;
            }
        }

        /// <summary>
        /// Class representing a test case.
        /// </summary>
        private sealed class BaseUriValueTestCase
        {
            public ODataPayloadElement PayloadElement { get; set; }
            public Uri ReaderSettingBaseUri { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private sealed class NullUriValueTestCase<T>
        {
            public Action<T, Uri, ReaderTestConfiguration> SetNullUriAction { get; set; }
            public Action<T, Uri, ReaderTestConfiguration> SetExpectedUriAction { get; set; }
        }
    }
}