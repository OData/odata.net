//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Reader;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class SchemaJsonReaderTests
    {
        [Fact]
        public void ReadAsObjectWorks()
        {
            string csdl = @" {

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
}";

            CsdlSchema objectValue;
            using (TextReader txtReader = new StringReader(csdl))
            {
                JsonReader jsonReader = new JsonReader(txtReader);
                objectValue = SchemaJsonReader.ParseSchemObject(jsonReader);
            }

            Assert.NotNull(objectValue);
        }

        [Fact]
        public void ParseEnumTypeWorks()
        {
            string csdl = @" {
  ""$Kind"": ""EnumType"",
  ""$UnderlyingType"": ""Edm.Int32"",
  ""$IsFlags"": true,
  ""Read"": 1,
  ""Write"": 2,
  ""Create"": 4,
  ""Delete"": 8,
   ""TwoDay"": 16,
  ""TwoDay@Core.Description"": ""Shipped within two days"",
  ""@Core.Description"": ""Method of shipping""
}";

            CsdlEnumType enumType;
            using (TextReader txtReader = new StringReader(csdl))
            {
                JsonReader jsonReader = new JsonReader(txtReader);
                JsonObjectValue objValue = jsonReader.ReadAsObject();
                enumType = SchemaJsonReader.BuildCsdlEnumType("ShippingMethod", objValue);
            }

            Assert.NotNull(enumType);
        }
    }
}