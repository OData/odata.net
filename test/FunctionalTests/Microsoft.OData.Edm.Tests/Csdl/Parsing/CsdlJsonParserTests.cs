//---------------------------------------------------------------------
// <copyright file="CsdlJsonParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Csdl.Reader;
using Microsoft.OData.Edm.Validation;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlJsonParserTests
    {
        [Fact]
        public void DeserializeCsdlTestWorks()
        {
            string csdl = @" {
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS1.Container"",
  ""NS1"": {
    ""Product"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32"",
        ""@Org.OData.Core.V1.Computed"": true
      },
      ""Name"": { },
      ""UpdatedTime"": {
        ""$Type"": ""Edm.Date"",
        ""@Org.OData.Core.V1.Computed"": true
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Products"": {
        ""$Collection"": true,
        ""$Type"": ""NS1.Product"",
        ""@Org.OData.Core.V1.OptimisticConcurrency"": [
          {
            ""$PropertyPath"": ""Id""
          },
          {
            ""$PropertyPath"": ""UpdatedTime""
          }
        ]
      }
    }
  }
}";

            var model = CsdlSerializer.Deserialize(csdl);

            Assert.NotNull(model);
        }

        [Fact]
        public void ParseReferencesWorksAsExpected()
        {
            string json = @" {
  ""http://vocabs.odata.org/capabilities/v1"": {
    ""$Include"": [
      {
        ""$Namespace"": ""Org.OData.Capabilities.V1"",
         ""$Alias"": ""Capabilities""
        }
      ]
    },
  ""http://vocabs.odata.org/core/v1"": {
    ""$IncludeAnnotations"": [
        {
           ""$TermNamespace"": ""org.example.validation""
        },
        {
           ""$TermNamespace"": ""org.example.display"",
          ""$Qualifier"": ""Tablet""
        }
      ]
   }
}";

            JsonObjectValue jsonObject = ReadAsObject(json);

            JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            IList<IEdmReference> references = CsdlJsonParser.ParseReferences(jsonObject, path, options);

            Assert.NotNull(references);
            Assert.Equal(2, references.Count);

            Assert.Single(references.First().Includes);
            Assert.Empty(references.First().IncludeAnnotations);

            Assert.Empty(references.Last().Includes);
            Assert.Equal(2, references.Last().IncludeAnnotations.Count());
        }

        [Fact]
        public void ParseIncludeWorksAsExpected()
        {
            string json = @" {
""$Namespace"": ""org.example.display"",
 ""$Alias"": ""UI""  }";

            JsonObjectValue jsonObject = ReadAsObject(json);

            JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            IEdmInclude include = CsdlJsonParser.ParseInclude(jsonObject, path, options);

            Assert.NotNull(include);
            Assert.Equal("UI", include.Alias);
            Assert.Equal("org.example.display", include.Namespace);
        }

        [Fact]
        public void ParseIncludeThrowsForUnknownMember()
        {
            string json = @" { 
""$Namespace"": ""org.example.display"",
""$Unknown"": ""UI""  }";

            JsonObjectValue jsonObject = ReadAsObject(json);

            JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions
            {
                IgnoreUnexpectedMembers = false
            };

            Action test = () => CsdlJsonParser.ParseInclude(jsonObject, path, options);

            CsdlParseException exception = Assert.Throws<CsdlParseException>(test);
            Assert.Equal(Strings.CsdlJsonParser_UnexpectedJsonMember("$.$Unknown", "JPrimitive"), exception.Message);
        }

        [Fact]
        public void ParseIncludeAnnotationsWorksAsExpected()
        {
            string json = @" {
  ""$TermNamespace"": ""org.example.hcm"",
  ""$Qualifier"": ""Tablet"",
  ""$TargetNamespace"":   ""com.example.Person""  }";

            JsonObjectValue jsonObject = ReadAsObject(json);

            JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            IEdmIncludeAnnotations includeAnnotations = CsdlJsonParser.ParseIncludeAnnotations(jsonObject, path, options);

            Assert.NotNull(includeAnnotations);
            Assert.Equal("org.example.hcm", includeAnnotations.TermNamespace);
            Assert.Equal("Tablet", includeAnnotations.Qualifier);
            Assert.Equal("com.example.Person", includeAnnotations.TargetNamespace);
        }

        private static JsonObjectValue ReadAsObject(string json)
        {
            using (TextReader txtReader = new StringReader(json))
            {
                IJsonReader jsonReader = new JsonReader(txtReader);
                return jsonReader.ReadAsObject();
            }
        }
    }
}