//---------------------------------------------------------------------
// <copyright file="CamelCasedTypeMaterializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using NS.Models;
    using Xunit;

    public partial class CamelCasedTypeMaterializationTests
    {
        #region Edmx Constants
        private const string CamelCasedEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""NS.Models"">
      <EntityType Name=""shape"" OpenType=""true"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""area"" Type=""Edm.Double"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""rectangle"" BaseType=""NS.Models.shape"" OpenType=""true"">
		<Property Name=""length"" Type=""Edm.Double"" Nullable=""false"" />
        <Property Name=""width"" Type=""Edm.Double"" Nullable=""false"" />
        <Property Name=""attributes"" Type=""Collection(Edm.String)"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""shapes"" EntityType=""NS.Models.shape"" />
        <EntitySet Name=""rectangles"" EntityType=""NS.Models.rectangle"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        #endregion Edmx Constants

        private const string ServiceUri = "http://tempuri.org/";
        private ClientEdmModel clientModel;
        private DataServiceContext dataServiceContext;

        public CamelCasedTypeMaterializationTests()
        {
            this.InitializeEdmModel();
        }

        [Fact]
        public void MaterializationForEntityBoundToBaseEntityTypeCollection()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#shapes/$entity\"," +
                "\"@odata.type\":\"#NS.Models.Rectangle\"," + // Note: Rectangle as opposed to rectangle
                "\"id\":1," +
                "\"area\":28," +
                "\"length\":7," +
                "\"width\":4," +
                "\"attributes\":[]}";

            ConfigureOnMessageCreating(payload);
            var query = dataServiceContext.CreateQuery<Rectangle>("shapes");

            var rectangle = query.Execute().FirstOrDefault();

            Assert.NotNull(rectangle);
            Assert.Equal(7D, rectangle.Length);
            Assert.Equal(4D, rectangle.Width);
            Assert.Equal(28D, rectangle.Area);
            Assert.Empty(rectangle.Attributes);
        }

        [Fact]
        public void MaterializationForEntityBoundToEntityTypeCollection()
        {
            // No "@odata.type" property
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#rectangles/$entity\"," +
                "\"id\":1," +
                "\"area\":28," +
                "\"length\":7," +
                "\"width\":4," +
                "\"attributes\":[]}";

            ConfigureOnMessageCreating(payload);
            var query = dataServiceContext.CreateQuery<Rectangle>("rectangles");

            var rectangle = query.Execute().FirstOrDefault();

            Assert.NotNull(rectangle);
            Assert.Equal(7D, rectangle.Length);
            Assert.Equal(4D, rectangle.Width);
            Assert.Equal(28D, rectangle.Area);
            Assert.Empty(rectangle.Attributes);
        }

        [Fact]
        public void MaterializationForEntitySetBoundToBaseEntityTypeCollection()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#shapes\"," +
                "\"value\":[{" +
                "\"@odata.type\":\"#NS.Models.Rectangle\"," +
                "\"id\":1," +
                "\"area\":28," +
                "\"length\":7," +
                "\"width\":4," +
                "\"attributes\":[]" +
                "}]}";

            ConfigureOnMessageCreating(payload);
            var query = dataServiceContext.CreateQuery<Rectangle>("shapes");

            var rectangles = query.Execute().ToArray();

            Assert.NotNull(rectangles);
            Assert.Single(rectangles);
            Assert.Equal(7D, rectangles[0].Length);
            Assert.Equal(4D, rectangles[0].Width);
            Assert.Equal(28D, rectangles[0].Area);
            Assert.Empty(rectangles[0].Attributes);
        }

        private void ConfigureOnMessageCreating(string payload)
        {
            dataServiceContext.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new CustomizedHttpWebRequestMessage(args,
                    payload,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    });
            };
        }

        private void InitializeEdmModel()
        {
            this.clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.dataServiceContext = new DataServiceContext(new Uri(ServiceUri), ODataProtocolVersion.V4, this.clientModel);
            this.dataServiceContext.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            this.dataServiceContext.ResolveType = (typeName) =>
            {
                return this.dataServiceContext.DefaultResolveType(typeName, "NS.Models", "NS.Models");
            };

            this.dataServiceContext.ResolveName = (clientType) =>
            {
                var originalNameAttribute = (OriginalNameAttribute)Enumerable.SingleOrDefault(Utility.GetCustomAttributes(clientType, typeof(OriginalNameAttribute), true));
                if (clientType.Namespace.Equals("NS.Models", global::System.StringComparison.Ordinal))
                {
                    if (originalNameAttribute != null)
                    {
                        return string.Concat("NS.Models.", originalNameAttribute.OriginalName);
                    }

                    return string.Concat("NS.Models.", clientType.Name);
                }

                if (originalNameAttribute != null)
                {
                    return clientType.Namespace + "." + originalNameAttribute.OriginalName;
                }

                return clientType.FullName;
            };

            using (var reader = XmlReader.Create(new StringReader(CamelCasedEdmx)))
            {
                if (CsdlReader.TryParse(reader, out IEdmModel serviceModel, out _))
                {
                    this.dataServiceContext.Format.UseJson(serviceModel);
                }
            }
        }
    }
}

namespace NS.Models
{
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;

    [Key("id")]
    [EntitySet("shapes")]
    [OriginalName("shape")]
    public class Shape
    {
        [OriginalName("id")]
        public int Id { get; set; }
        [OriginalName("area")]
        public double Area { get; set; }
    }

    [Key("id")]
    [OriginalName("rectangle")]
    public class Rectangle : Shape
    {
        public Rectangle()
        {
            Attributes = new ObservableCollection<string>();
        }

        [OriginalName("length")]
        public double Length { get; set; }
        [OriginalName("width")]
        public double Width { get; set; }
        [OriginalName("attributes")]
        public ObservableCollection<string> Attributes { get; set; }
    }
}
