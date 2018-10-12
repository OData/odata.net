//---------------------------------------------------------------------
// <copyright file="ODataClientBuilderExtensionsTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ODataClient.Tests.Netcore;
using Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Extensions.ODataClient.Tests.UnitTests
{
    public class ODataClientBuilderExtensionsTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("Verification")]
        public void TestAddHttpClient(string clientName)
        {
            var sc = new ServiceCollection();
            var builder = sc.AddODataClient(clientName).AddHttpClient();
            builder.Name.Should().Be(clientName);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();

            var client = factory.CreateClient<DataServiceContext>(new Uri("http://localhost"), clientName);
            client.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("Verification")]
        public void TestConfigureODataClient(string clientName)
        {
            var sc = new ServiceCollection();
            int count = 0;
            var builder = sc.AddODataClient(clientName).ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost2"); });
            builder.Name.Should().Be(clientName);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();

            var client = factory.CreateClient<DataServiceContext>(new Uri("http://localhost"), clientName);
            client.Should().NotBeNull();
            client.BaseUri.Should().Be(new Uri("http://localhost2"));
            count.Should().Be(1);
        }

        [Theory]
        [InlineData("", "TestString")]
        [InlineData("", 123)]
        [InlineData("", true)]
        [InlineData("Verification", "TestString")]
        [InlineData("Verification", 123)]
        [InlineData("Verification", true)]
        public async Task TestSharePropertiesWithHttpClient(string clientName, object testProperty)
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<VerificationCounter>();
            sc.AddTransient<PropertyHttpClientHandler>();

            var builder = sc
                .AddODataClient(clientName)
                .AddProperty("property", testProperty)
                .ConfigureODataClient(dsc => dsc.Configurations.Properties.Add("TestProperty", testProperty))
                .AddHttpClient()
                .AddHttpMessageHandler<PropertyHttpClientHandler>();
            builder.Name.Should().Be(clientName);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();
            var counter = sp.GetRequiredService<VerificationCounter>();

            var client = factory.CreateClient<DefaultContainer>(new Uri("http://InvalidHost"), clientName);
            client.Should().NotBeNull();

            client.AddToPerson(new Person());

            Func<Task> action = async () => await client.SaveChangesAsync();
            action.ShouldThrow<InvalidOperationException>("Host is invalid and could not connect");

            counter.HttpRequestProperties.Count.Should().Be(2);
            counter.HttpRequestProperties["property"].Should().Be(testProperty);
            counter.HttpRequestProperties["TestProperty"].Should().Be(testProperty);
        }

        [Theory]
        [InlineData("", "InvalidName")]
        [InlineData("InvalidName", "")]
        [InlineData("Verification", "InvalidName")]
        public void TestAddODataClientNameNotRegistered(string registerName, string createName)
        {
            var sc = new ServiceCollection();
            int count = 0;
            var builder = sc.AddODataClient(registerName).ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost2"); });
            builder.Name.Should().Be(registerName);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();

            var client = factory.CreateClient<DataServiceContext>(new Uri("http://localhost"), createName);
            client.Should().NotBeNull();
            client.BaseUri.Should().Be(new Uri("http://localhost"));
        }

        [Theory]
        [InlineData("", "InvalidName")]
        [InlineData("InvalidName", "")]
        [InlineData("Verification", "InvalidName")]
        public void Factory_MultipleCalls(string clientName1, string clientName2)
        {
            var sc = new ServiceCollection();
            int count = 0;
            var builder1 = sc.AddODataClient(clientName1).ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost1"); });
            builder1.Name.Should().Be(clientName1);

            var builder2 = sc.AddODataClient(clientName2).ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost2"); });
            builder2.Name.Should().Be(clientName2);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();

            var client1 = factory.CreateClient<DataServiceContext>(new Uri("http://localhost"), clientName1);
            client1.Should().NotBeNull();
            client1.BaseUri.Should().Be(new Uri("http://localhost1"));

            var client2 = factory.CreateClient<DefaultContainer>(new Uri("http://localhost"), clientName2);
            client2.Should().NotBeNull();
            client2.BaseUri.Should().Be(new Uri("http://localhost2"));

            clientName2.Should().NotBeSameAs(clientName1);
        }

        [Theory]
        [InlineData("", "InvalidName")]
        [InlineData("InvalidName", "")]
        [InlineData("Verification", "InvalidName")]
        public void Factory_Verification(string clientName1, string clientName2)
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<VerificationCounter>();
            sc.AddTransient<VerificationController>();

            sc.AddTransient<VerificationODataClientHandler>();
            sc.AddTransient<VerificationHttpClientHandler>();

            int count = 0;
            var builder1 = sc
                .AddODataClient(clientName1)
                .ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost1"); })
                .AddODataClientHandler<VerificationODataClientHandler>()
                .AddHttpClient()
                .AddHttpMessageHandler<VerificationHttpClientHandler>();

            builder1.Name.Should().Be(clientName1);
            count.Should().Be(0);

            var builder2 = sc
                .AddODataClient(clientName2)
                .ConfigureODataClient((dsc) => { count++; dsc.BaseUri = new Uri("http://localhost2"); })
                .AddODataClientHandler<VerificationODataClientHandler>()
                .AddHttpClient()
                .AddHttpMessageHandler<VerificationHttpClientHandler>();

            builder1.Name.Should().Be(clientName1);
            count.Should().Be(0);

            var sp = sc.BuildServiceProvider();
            var factory = sp.GetRequiredService<IODataClientFactory>();
            var counter = sp.GetRequiredService<VerificationCounter>();
            counter.ODataInvokeCount.Should().Be(0);

            var client1 = factory.CreateClient<DataServiceContext>(new Uri("http://localhost"), clientName1);
            client1.Should().NotBeNull();
            client1.BaseUri.Should().Be(new Uri("http://localhost1"));
            counter.ODataInvokeCount.Should().Be(1);
            counter.HttpInvokeCount.Should().Be(0);

            var client2 = factory.CreateClient<DefaultContainer>(new Uri("http://localhost"), clientName2);
            client2.Should().NotBeNull();
            client2.BaseUri.Should().Be(new Uri("http://localhost2"));
            counter.ODataInvokeCount.Should().Be(2);
            counter.HttpInvokeCount.Should().Be(0);
        }
    }
}
