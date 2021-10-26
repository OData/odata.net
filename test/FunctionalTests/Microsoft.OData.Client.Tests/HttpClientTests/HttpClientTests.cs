//---------------------------------------------------------------------
// <copyright file="HttpClientTests.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Client.Tests.Tracking;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.Tests.HttpClientTests
{
    public class HttpClientTests
    {
        private Container Context;
        private ContainerA ODataServiceContext;

        #region TestEdmx
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
        <edmx:DataServices>
            <Schema Namespace=""Microsoft.OData.Client.Tests.HttpClientTests"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityType Name=""Book"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
            </Schema>
            <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityContainer Name=""Container"">
                    <EntitySet Name=""Book"" EntityType=""Microsoft.OData.Client.Tests.HttpClientTests.Book""/>
                </EntityContainer>
            </Schema>
        </edmx:DataServices>
        </edmx:Edmx>";
        #endregion

        #region responses
        private const string Response = @"{
            ""@odata.context"":""http://localhost:8000/$metadata#Book/$entity"",
            ""Id"":1,
            ""Name"":""BookA""}";
        #endregion

        public HttpClientTests()
        {
            var uri = new Uri("http://localhost:8000");
            HttpClient httpClient = new HttpClient();
            Context = new Container(uri);
            ODataServiceContext = new ContainerA(uri, httpClient);
        }

        [Fact]
        public async Task HttpClient_IsDisposed_IfNotPassed()
        {
            SetupContextWithRequestPipelineForSaving(
            new DataServiceContext[] { Context }, Response, "http://localhost:8000/Books(1)");
            var book = new Book
            {
                Id = 0,
                Name = "BookA"
            };

            Context.AddObject("Books", book);
            await SaveContextChangesAsync(new DataServiceContext[] { Context });
            Context.Dispose();
            bool httpClientIsDisposed = (bool)Context.HttpClient.GetType().BaseType.GetField("disposed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Context.HttpClient);
            Assert.True(httpClientIsDisposed);
        }

        [Fact]
        public async Task HttpClient_IsNotDisposed_IfPassed()
        {
            SetupContextWithRequestPipelineForSaving(
            new DataServiceContext[] { ODataServiceContext }, Response, "http://localhost:8000/Books(1)");
            var book = new Book
            {
                Id = 0,
                Name = "BookA"
            };

            ODataServiceContext.AddObject("Books", book);
            await SaveContextChangesAsync(new DataServiceContext[] { ODataServiceContext });
            Context.Dispose();
            bool httpClientIsDisposed = (bool)ODataServiceContext.HttpClient.GetType().BaseType.GetField("disposed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ODataServiceContext.HttpClient);
            Assert.False(httpClientIsDisposed);
        }

        private async Task SaveContextChangesAsync(DataServiceContext[] dataServiceContexts)
        {
            foreach (var dataServiceContext in dataServiceContexts)
            {
                await dataServiceContext.SaveChangesAsync();
            }
        }

        private void SetupContextWithRequestPipelineForSaving(DataServiceContext[] dataServiceContexts, string response, string location)
        {
            foreach (var context in dataServiceContexts)
            {
                context.Configurations.RequestPipeline.OnMessageCreating =
                    (args) => new CustomizedRequestMessage(
                        args,
                        response,
                        new Dictionary<string, string>()
                        {
                            {"Content-Type", "application/json;charset=utf-8"},
                            { "Location", location },
                        });
            }
        }

        class Container : DataServiceContext
        {
            public Container(global::System.Uri serviceRoot) :
                base(serviceRoot, ODataProtocolVersion.V4)
            {
                this.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                this.Format.UseJson();
                this.Books = base.CreateQuery<Book>("Books");
            }
            public DataServiceQuery<Book> Books { get; private set; }
        }

        class ContainerA : DataServiceContext
        {
            public ContainerA(global::System.Uri serviceRoot, HttpClient httpClient) :
                base(serviceRoot, ODataProtocolVersion.V4, httpClient)
            {
                this.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                this.Format.UseJson();
                this.Books = base.CreateQuery<Book>("Books");
            }
            public DataServiceQuery<Book> Books { get; private set; }
        }
    }

    [Key("Id")]
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
