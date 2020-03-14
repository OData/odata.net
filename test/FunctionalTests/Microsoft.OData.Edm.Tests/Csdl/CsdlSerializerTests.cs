//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlSerializerTests
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


        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ValidateEdmxVersions(string odataVersion)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"" + odataVersion + "\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\"><edmx:DataServices /></edmx:Edmx>";

            var stringReader = new System.IO.StringReader(xml);
            var xmlReader = System.Xml.XmlReader.Create(stringReader);

            IEdmModel edmModel = null;
            IEnumerable<EdmError> edmErrors;

            // Read in the CSDL and verify the version
            CsdlReader.TryParse(xmlReader, out edmModel, out edmErrors);
            Assert.Equal(edmErrors.Count(), 0);
            Assert.Equal(edmModel.GetEdmVersion(), odataVersion == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401);
        }
    }
}