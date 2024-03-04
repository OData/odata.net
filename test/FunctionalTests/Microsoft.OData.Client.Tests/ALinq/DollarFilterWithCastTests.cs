//---------------------------------------------------------------------
// <copyright file="DollarFilterWithCastTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Tests.ALinq
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Client.Tests.Tracking;
    using Microsoft.OData.Edm.Csdl;
    using Xunit;

    /// <summary>
    /// Dollar Filter with cast tests
    /// </summary>
    public class DollarFilterWithCastTests
    {
        private Container DefaultContext;

        #region TestEdmx
        private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
        <edmx:DataServices>
            <Schema Namespace=""Microsoft.OData.Client.Tests.ALinq"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityType Name=""Product"">
                    <Key>
                        <PropertyRef Name=""Id""/>
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""/>
                    <Property Name=""Name"" Type=""Edm.String""/>
                </EntityType>
            </Schema>
            <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <EntityContainer Name=""Container"">
                    <EntitySet Name=""Products"" EntityType=""Microsoft.OData.Client.Tests.ALinq.Product""/>
                </EntityContainer>
            </Schema>
        </edmx:DataServices>
        </edmx:Edmx>";
        #endregion

        #region responses
        private const string Response = @"{""@odata.context"":""http://localhost:8000/$metadata#Products"",""value"":[{""Id"":1,""Name"":""Hat""},{""Id"":2,""Name"":""Socks""},{""Id"":3,""Name"":""Scarf""}]}";
        #endregion

        public DollarFilterWithCastTests()
        {
            var uri = new Uri("http://localhost:8000");
            DefaultContext = new Container(uri);
        }

        [Fact]
        public void PrimitiveTypeInRequestUrlTest()
        {
            SetupContextWithRequestPipelineForSaving(
            new DataServiceContext[] { DefaultContext }, Response, "http://localhost:8000/Products");
            const string stringOfCast = "cast(Id,Edm.Byte)";
            var result = DefaultContext.Products.Where(a=>(Byte)a.Id > 0);
            var stringOfQuery = result.ToString();
            Assert.Contains(stringOfCast, stringOfQuery);
            Assert.Equal(3, result.ToList().Count());
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
                this.Products = base.CreateQuery<Product>("Products");
            }
            public DataServiceQuery<Product> Products { get; private set; }
        }
    }

    [Key("Id")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
