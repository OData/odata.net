//---------------------------------------------------------------------
// <copyright file="ODataServiceCollectionExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;
using System;
using Xunit;

namespace Microsoft.OData.Tests
{

    /// <summary>
    /// Tests methods related to registering OData services in Dependency Injection containers.
    /// </summary>
    public class ODataServiceCollectionExtensionsTests
    {

        /// <summary>
        /// Tests whether the AddDefaultODataServices method registers the correct services.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_RegistersServicesCorrectly()
        {
            var services = new ServiceCollection();
            Assert.Empty(services);

            services.AddDefaultODataServices();
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for registered Singletons.
            Assert.NotNull(provider.GetService<IJsonReaderFactory>());
            Assert.NotNull(provider.GetService<IJsonWriterFactory>());
            Assert.NotNull(provider.GetService<ODataMediaTypeResolver>());
            Assert.NotNull(provider.GetService<ODataPayloadValueConverter>());
            Assert.NotNull(provider.GetService<IEdmModel>());
            Assert.NotNull(provider.GetService<ODataUriResolver>());

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            Assert.NotNull(scope.ServiceProvider.GetService<ODataMessageInfo>());
            Assert.NotNull(scope.ServiceProvider.GetService<UriPathParser>());
            Assert.NotNull(scope.ServiceProvider.GetService<ODataMessageReaderSettings>());
            Assert.NotNull(scope.ServiceProvider.GetService<ODataMessageWriterSettings>());
            Assert.NotNull(scope.ServiceProvider.GetService<ODataUriParserSettings>());
        }

        /// <summary>
        /// Tests whether the AddDefaultODataServices method registers the correct services for the default OData 
        /// version (<see cref="ODataVersion.V4" />).
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_ODataVersion_V4_IsDefault()
        {
            var services = new ServiceCollection();
            services.AddDefaultODataServices();
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            var readerSettings = scope.ServiceProvider.GetService<ODataMessageReaderSettings>();
            Assert.NotNull(readerSettings);
            Assert.Equal(ODataVersion.V4, readerSettings.Version);
            Assert.Equal(ODataVersion.V4, readerSettings.MaxProtocolVersion);

            var writerSettings = scope.ServiceProvider.GetService<ODataMessageWriterSettings>();
            Assert.NotNull(writerSettings);
            Assert.Equal(ODataVersion.V4, writerSettings.Version);
        }

        /// <summary>
        /// Tests whether the AddDefaultODataServices method registers the correct services when the using <see cref="ODataVersion.V401" />.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_ODataVersion_V401_IsSetCorrectly()
        {
            var services = new ServiceCollection();
            services.AddDefaultODataServices(ODataVersion.V401);
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            var readerSettings = scope.ServiceProvider.GetService<ODataMessageReaderSettings>();
            Assert.NotNull(readerSettings);
            Assert.Equal(ODataVersion.V401, readerSettings.Version);
            Assert.Equal(ODataVersion.V401, readerSettings.MaxProtocolVersion);

            var writerSettings = scope.ServiceProvider.GetService<ODataMessageWriterSettings>();
            Assert.NotNull(writerSettings);
            Assert.Equal(ODataVersion.V401, writerSettings.Version);
        }

        /// <summary>
        /// Tests whether the <see cref="ODataMessageReaderSettings" /> can be configured using the <see cref="Action{ODataMessageReaderSettings}" />.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_ReaderSettings_CanConfigure()
        {
            var services = new ServiceCollection();
            // @robertmclaws: Configure a setting that defaults to false.
            services.AddDefaultODataServices(configureReaderAction: (reader) => reader.EnableCharactersCheck = true);
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            var readerSettings = scope.ServiceProvider.GetService<ODataMessageReaderSettings>();
            Assert.NotNull(readerSettings);
            Assert.True(readerSettings.EnableCharactersCheck);
        }

        [Fact]
        public void AddDefaultODataServices_ReaderSettings_InstancesAreScoped()
        {
            var services = new ServiceCollection();
            services.AddDefaultODataServices();
            var provider = services.BuildServiceProvider();
            using var scope1 = provider.CreateScope();
            var settings = scope1.ServiceProvider.GetRequiredService<ODataMessageReaderSettings>();
            settings.EnableCharactersCheck = true;

            var settingsFromSameScope = scope1.ServiceProvider.GetRequiredService<ODataMessageReaderSettings>();

            using var scope2 = provider.CreateScope();
            var settingsFromOtherScope = scope2.ServiceProvider.GetRequiredService<ODataMessageReaderSettings>();
            
            // Instances from the same scope should be the same
            Assert.True(object.ReferenceEquals(settings, settingsFromSameScope));
            Assert.True(settingsFromSameScope.EnableCharactersCheck);
            // Instances from different scopes should be different
            Assert.False(object.ReferenceEquals(settings, settingsFromOtherScope));
            Assert.False(settingsFromOtherScope.EnableCharactersCheck);
        }

        /// <summary>
        /// Tests whether the <see cref="ODataMessageReaderSettings" /> can be configured using the <see cref="Action{ODataMessageWriterSettings}" />.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_WriterSettings_CanConfigure()
        {
            var services = new ServiceCollection();
            // @robertmclaws: Configure a setting that defaults to false.
            services.AddDefaultODataServices(configureWriterAction: (writer) => writer.EnableCharactersCheck = true);
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            var writerSettings = scope.ServiceProvider.GetService<ODataMessageWriterSettings>();
            Assert.NotNull(writerSettings);
            Assert.True(writerSettings.EnableCharactersCheck);
        }

        [Fact]
        public void AddDefaultODataServices_WriterSettings_InstancesAreScoped()
        {
            var services = new ServiceCollection();
            services.AddDefaultODataServices();
            var provider = services.BuildServiceProvider();
            using var scope1 = provider.CreateScope();
            var settings = scope1.ServiceProvider.GetRequiredService<ODataMessageWriterSettings>();
            settings.EnableCharactersCheck = true;

            var settingsFromSameScope = scope1.ServiceProvider.GetRequiredService<ODataMessageWriterSettings>();

            using var scope2 = provider.CreateScope();
            var settingsFromOtherScope = scope2.ServiceProvider.GetRequiredService<ODataMessageWriterSettings>();

            // Instances from the same scope should be the same
            Assert.True(object.ReferenceEquals(settings, settingsFromSameScope));
            Assert.True(settingsFromSameScope.EnableCharactersCheck);
            // Instances from different scopes should be different
            Assert.False(object.ReferenceEquals(settings, settingsFromOtherScope));
            Assert.False(settingsFromOtherScope.EnableCharactersCheck);
        }

        /// <summary>
        /// Tests whether the <see cref="ODataUriParserSettings" /> can be configured using the <see cref="Action{ODataUriParserSettings}" />.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_ODataUriParserSettings_CanConfigure()
        {
            var services = new ServiceCollection();
            // @robertmclaws: Configure a setting that defaults to false.
            services.AddDefaultODataServices(configureUriParserAction: (parser) => parser.MaximumExpansionCount = 1);
            Assert.True(services.Count == 11);

            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider);

            // @robertmclaws: Test for request-scoped services.
            var scope = provider.CreateScope();
            Assert.NotNull(scope);
            Assert.NotNull(scope.ServiceProvider);

            var parserSettings = scope.ServiceProvider.GetService<ODataUriParserSettings>();
            Assert.NotNull(parserSettings);
            Assert.Equal(1, parserSettings.MaximumExpansionCount);
        }

        [Fact]
        public void AddDefaultODataServices_ODataUriParserSettings_InstancesAreScoped()
        {
            var services = new ServiceCollection();
            services.AddDefaultODataServices();
            var provider = services.BuildServiceProvider();
            using var scope1 = provider.CreateScope();
            var settings = scope1.ServiceProvider.GetRequiredService<ODataUriParserSettings>();
            settings.EnableParsingKeyAsSegment = false;

            var settingsFromSameScope = scope1.ServiceProvider.GetRequiredService<ODataUriParserSettings>();

            using var scope2 = provider.CreateScope();
            var settingsFromOtherScope = scope2.ServiceProvider.GetRequiredService<ODataUriParserSettings>();

            // Instances from the same scope should be the same
            Assert.True(object.ReferenceEquals(settings, settingsFromSameScope));
            Assert.False(settingsFromSameScope.EnableParsingKeyAsSegment);
            // Instances from different scopes should be different
            Assert.False(object.ReferenceEquals(settings, settingsFromOtherScope));
            Assert.True(settingsFromOtherScope.EnableParsingKeyAsSegment);
        }

        /// <summary>
        /// Tests whether the correct exception is thrown when no <see cref="IServiceCollection" /> is provided.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_NullServiceCollection_ThrowsNullReferenceException()
        {
            Action extension = () => ODataServiceCollectionExtensions.AddDefaultODataServices(null);

            var exception = Assert.Throws<ArgumentNullException>(extension);
            Assert.Equal("services", exception.ParamName);
        }

        /// <summary>
        /// Tests whether specifically passing a null <see cref="Action{ODataMessageReaderSettings}" /> does not throw an exception.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_NullReaderAction_DoesNotThrow()
        {
            var exception = Record.Exception(() => new ServiceCollection().AddDefaultODataServices(configureReaderAction: null));
            Assert.Null(exception);
        }

        /// <summary>
        /// Tests whether specifically passing a null <see cref="Action{ODataMessageWriterSettings}" /> does not throw an exception.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_NullWriterAction_DoesNotThrow()
        {
            var exception = Record.Exception(() => new ServiceCollection().AddDefaultODataServices(configureWriterAction: null));
            Assert.Null(exception);
        }

        /// <summary>
        /// Tests whether specifically passing a null <see cref="Action{ODataUriParserSettings}" /> does not throw an exception.
        /// </summary>
        [Fact]
        public void AddDefaultODataServices_NullUriParserAction_DoesNotThrow()
        {
            var exception = Record.Exception(() => new ServiceCollection().AddDefaultODataServices(configureUriParserAction: null));
            Assert.Null(exception);
        }

    }
}
