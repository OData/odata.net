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


        [Fact]
        public void CanDeserializeCategoryAndCategoriesExample()
        {
            string json = @"
{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""ODataDemo.DemoService"",
  ""$Reference"": {
    ""https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Core.V1.json"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Core.V1"",
          ""$Alias"": ""Core"",
          ""@Core.DefaultNamespace"": true
        }
      ]
    },
    ""https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Measures.V1.json"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Measures.V1"",
          ""$Alias"": ""Measures""
        }
      ]
    }
  },
  ""ODataDemo"": {
    ""$Alias"": ""self"",
    ""@Core.DefaultNamespace"": true,
    ""Product"": {
      ""$Kind"": ""EntityType"",
      ""$HasStream"": true,
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {},
      ""Description"": {
        ""$Nullable"": true,
        ""@Core.IsLanguageDependent"": true
      },
      ""ReleaseDate"": {
        ""$Nullable"": true,
        ""$Type"": ""Edm.Date""
      },
      ""DiscontinuedDate"": {
        ""$Nullable"": true,
        ""$Type"": ""Edm.Date""
      },
      ""Rating"": {
        ""$Nullable"": true,
        ""$Type"": ""Edm.Int32""
      },
      ""Price"": {
        ""$Nullable"": true,
        ""$Type"": ""Edm.Decimal"",
        ""@Measures.ISOCurrency"": {
          ""$Path"": ""Currency""
        }
      },
      ""Currency"": {
        ""$Nullable"": true,
        ""$MaxLength"": 3
      },
      ""Category"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""self.Category"",
        ""$Partner"": ""Products""
      },
      ""Supplier"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Nullable"": true,
        ""$Type"": ""self.Supplier"",
        ""$Partner"": ""Products""
      }
    },
    ""Category"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {
        ""@Core.IsLanguageDependent"": true
      },
      ""Products"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Partner"": ""Category"",
        ""$Collection"": true,
        ""$Type"": ""self.Product"",
        ""$OnDelete"": ""Cascade""
      }
    },
    ""Supplier"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {},
      ""Name"": {
        ""$Nullable"": true
      },
      ""Address"": {
        ""$Type"": ""self.Address""
      },
      ""Concurrency"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Products"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Partner"": ""Supplier"",
        ""$Collection"": true,
        ""$Type"": ""self.Product""
      }
    },
    ""Country"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Code""
      ],
      ""Code"": {
        ""$MaxLength"": 2
      },
      ""Name"": {
        ""$Nullable"": true
      }
    },
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""Street"": {
        ""$Nullable"": true
      },
      ""City"": {
        ""$Nullable"": true
      },
      ""State"": {
        ""$Nullable"": true
      },
      ""ZipCode"": {
        ""$Nullable"": true
      },
      ""CountryName"": {
        ""$Nullable"": true
      },
      ""Country"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Nullable"": true,
        ""$Type"": ""self.Country"",
        ""$ReferentialConstraint"": {
          ""CountryName"": ""Name""
        }
      }
    },
    ""ProductsByRating"": [
      {
        ""$Kind"": ""Function"",
        ""$Parameter"": [
          {
            ""$Name"": ""Rating"",
            ""$Nullable"": true,
            ""$Type"": ""Edm.Int32""
          }
        ],
        ""$ReturnType"": {
          ""$Collection"": true,
          ""$Type"": ""self.Product""
        }
      }
    ],
    ""DemoService"": {
      ""$Kind"": ""EntityContainer"",
      ""Products"": {
        ""$Collection"": true,
        ""$Type"": ""self.Product"",
        ""$NavigationPropertyBinding"": {
          ""Category"": ""Categories""
        }
      },
      ""Categories"": {
        ""$Collection"": true,
        ""$Type"": ""self.Category"",
        ""$NavigationPropertyBinding"": {
          ""Products"": ""Products""
        },
        ""@Core.Description"": ""Product Categories""
      },
      ""Suppliers"": {
        ""$Collection"": true,
        ""$Type"": ""self.Supplier"",
        ""$NavigationPropertyBinding"": {
          ""Products"": ""Products"",
          ""Address/Country"": ""Countries""
        },
        ""@Core.OptimisticConcurrency"": [
          ""Concurrency""
        ]
      },
      ""Countries"": {
        ""$Collection"": true,
        ""$Type"": ""self.Country""
      },
      ""MainSupplier"": {
        ""$Type"": ""self.Supplier"",
        ""$NavigationPropertyBinding"": {
          ""Products"": ""Products""
        },
        ""@Core.Description"": ""Primary Supplier""
      },
      ""ProductsByRating"": {
        ""$EntitySet"": ""Products"",
        ""$Function"": ""self.ProductsByRating""
      }
    }
  }
}";

            IEdmModel model = CsdlSerializer.Deserialize(json);

            Assert.NotNull(model);

            string csdlXml = CsdlWriterTests.GetCsdl(model, CsdlTarget.OData);

            Assert.Equal("", csdlXml);
        }
    }
}