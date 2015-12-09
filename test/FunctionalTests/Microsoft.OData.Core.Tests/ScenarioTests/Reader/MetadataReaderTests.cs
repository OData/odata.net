//---------------------------------------------------------------------
// <copyright file="MetadataReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Core.Metadata;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader
{
    public class MetadataReaderTests
    {
        [Fact]
        public void ReadSingleMetadataDocument()
        {
            string payload =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""C1"" BaseType=""demo.B1"">
        </ComplexType>      
        <ComplexType Name=""B1"">
        </ComplexType>    
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string mainUrl = "main";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {mainUrl, payload}
            };

            IEdmModel model = this.ReadMetadataDocument(map, mainUrl);
            var c1 = model.FindDeclaredType("demo.C1").As<IEdmComplexType>();
            c1.Should().NotBeNull();
            var b1 = model.FindDeclaredType("demo.B1").As<IEdmComplexType>();
            b1.Should().NotBeNull();
            b1.IsAssignableFrom(c1).Should().BeTrue();
        }

        [Fact]
        public void ReadMetadataDocumentWithReference()
        {
            string csdlMain =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:Reference Uri=""Ref1"">
    <edmx:Include Namespace=""demo"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""C1"" BaseType=""demo.B1"">
        </ComplexType>    
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string csdlR1 =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""demo"">    
        <ComplexType Name=""B1"">
        </ComplexType>    
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                {"main", csdlMain},
                {"Ref1", csdlR1}
            };

            IEdmModel model = this.ReadMetadataDocument(map, "main");
            var c1 = model.FindDeclaredType("demo.C1").As<IEdmComplexType>();
            c1.Should().NotBeNull();
            var b1 = model.FindType("demo.B1").As<IEdmComplexType>();
            b1.Should().NotBeNull();
            b1.IsAssignableFrom(c1).Should().BeTrue();
        }

        private IEdmModel ReadMetadataDocument(Dictionary<string, string> map, string mainUrl)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(map[mainUrl]));
            IODataResponseMessage responseMessage = new InMemoryMessage() { StatusCode = 200, Stream = stream };
            responseMessage.SetHeader("Content-Type", "application/xml");
            ODataMessageReader reader = new ODataMessageReader(responseMessage);
            return reader.ReadMetadataDocument((uri) =>
            {
                string uriStr = uri.ToString();
                if (map.ContainsKey(uriStr))
                {
                    return new XmlTextReader(new StringReader(map[uriStr]));
                }

                return null;
            });
        }
    }
}
