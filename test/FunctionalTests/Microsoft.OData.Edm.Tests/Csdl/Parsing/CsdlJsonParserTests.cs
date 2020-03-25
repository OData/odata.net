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
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Builder;
using Microsoft.OData.Edm.Csdl.Json.Parser;
using Microsoft.OData.Edm.Csdl.Json.Reader;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlJsonParserTests
    {
//        [Fact]
//        public void DeserializeCsdlTestWorks()
//        {
//            string csdl = @" {
//  ""$Version"": ""4.0"",
//  ""$EntityContainer"": ""NS1.Container"",
//  ""NS1"": {
//    ""Product"": {
//      ""$Kind"": ""EntityType"",
//      ""$Key"": [
//        ""Id""
//      ],
//      ""Id"": {
//        ""$Type"": ""Edm.Int32"",
//        ""@Org.OData.Core.V1.Computed"": true
//      },
//      ""Name"": { },
//      ""UpdatedTime"": {
//        ""$Type"": ""Edm.Date"",
//        ""@Org.OData.Core.V1.Computed"": true
//      }
//    },
//    ""Container"": {
//      ""$Kind"": ""EntityContainer"",
//      ""Products"": {
//        ""$Collection"": true,
//        ""$Type"": ""NS1.Product"",
//        ""@Org.OData.Core.V1.OptimisticConcurrency"": [
//          {
//            ""$PropertyPath"": ""Id""
//          },
//          {
//            ""$PropertyPath"": ""UpdatedTime""
//          }
//        ]
//      }
//    }
//  }
//}";

//            // var model = CsdlSerializer.Deserialize(csdl);

//            JsonPath path = new JsonPath();
//            CsdlSerializerOptions options = new CsdlSerializerOptions();
//            using (TextReader txtReader = new StringReader(csdl))
//            {
//                EdmModelBuilder builder = new EdmModelBuilder(txtReader, options);
//                IEdmModel model = builder.BuildEdmModel(path);
//                Assert.NotNull(model);
//            }
//        }

        [Fact]
        public void DeserializeCsdlTestWork2s()
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

            // var model = CsdlSerializer.Deserialize(csdl);

        //    JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            using (TextReader txtReader = new StringReader(csdl))
            {
                CsdlJsonModelBuilder csdlModelBuilder = new CsdlJsonModelBuilder(txtReader, options);
                CsdlJsonModel csdlModel = csdlModelBuilder.TryParseCsdlJsonModel();

                EdmModelBuilder builder = new EdmModelBuilder(options);
                IEdmModel model = builder.TryBuildEdmModel(csdlModel);
                Assert.NotNull(model);
            }
        }

        [Fact]
        public void DeserializeCsdlTestWork3333333()
        {
            string mainCsdl = @" {
  ""$Reference"": {
    ""http://localhost/samxu/v1"": {
      ""$Include"": [
        {
          ""$Namespace"": ""My.Namespace"",
          ""$Alias"": ""sam""
        }
      ]
    },
   ""https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Core.V1.json"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Core.V1"",
          ""$Alias"": ""Core"",
          ""@Core.DefaultNamespace"": true
        }
      ]
    }
  },
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS1.Container"",
  ""NS1"": {
    ""Product"": {
      ""$BaseType"": ""sam.ProductBase"",
      ""$Kind"": ""EntityType"",
      ""Name"": { },
      ""UpdatedTime"": {
        ""$Type"": ""Edm.Date"",
        ""@Core.Computed"": true
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Products"": {
        ""$Collection"": true,
        ""$Type"": ""NS1.Product"",
        ""@Core.OptimisticConcurrency"": [
           ""Id"",
           ""UpdatedTime""
        ]
      }
    }
  }
}";

            string csdl = @" {
  ""$Version"": ""4.0"",
  ""My.Namespace"": {
    ""$Alias"": ""sam"",
    ""ProductBase"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      }
    }
  }
}";

            // var model = CsdlSerializer.Deserialize(csdl);

            //    JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            options.ReferencedModelJsonFactory = (uri) =>
            {
                if (uri.OriginalString.Contains("samxu/v1"))
                {
                    return new StringReader(csdl);
                }

                return null;
            };

            using (TextReader txtReader = new StringReader(mainCsdl))
            {
                CsdlJsonModelBuilder csdlModelBuilder = new CsdlJsonModelBuilder(txtReader, options);
                CsdlJsonModel csdlModel = csdlModelBuilder.TryParseCsdlJsonModel();

                EdmModelBuilder builder = new EdmModelBuilder(options);
                IEdmModel model = builder.TryBuildEdmModel(csdlModel);
                Assert.NotNull(model);
            }
        }

        [Fact]
        public void DeserializeCsdlTestWorkUsingCsdlModel()
        {
            string mainCsdl = @" {
  ""$Reference"": {
    ""http://localhost/samxu/v1"": {
      ""$Include"": [
        {
          ""$Namespace"": ""My.Namespace"",
          ""$Alias"": ""sam""
        }
      ]
    },
   ""https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Core.V1.json"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Core.V1"",
          ""$Alias"": ""Core"",
          ""@Core.DefaultNamespace"": true
        }
      ]
    }
  },
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS1.Container"",
  ""NS1"": {
    ""Product"": {
      ""$BaseType"": ""sam.ProductBase"",
      ""$Kind"": ""EntityType"",
      ""Name"": { },
      ""UpdatedTime"": {
        ""$Type"": ""Edm.Date"",
        ""@Core.Computed"": true
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Products"": {
        ""$Collection"": true,
        ""$Type"": ""NS1.Product"",
        ""@Core.OptimisticConcurrency"": [
           ""Id"",
           ""UpdatedTime""
        ]
      }
    }
  }
}";

            string csdl = @" {
  ""$Version"": ""4.0"",
  ""My.Namespace"": {
    ""$Alias"": ""sam"",
    ""ProductBase"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      }
    }
  }
}";

            // var model = CsdlSerializer.Deserialize(csdl);

            //    JsonPath path = new JsonPath();
            CsdlSerializerOptions options = new CsdlSerializerOptions();
            options.ReferencedModelJsonFactory = (uri) =>
            {
                if (uri.OriginalString.Contains("samxu/v1"))
                {
                    return new StringReader(csdl);
                }

                return null;
            };

            using (TextReader txtReader = new StringReader(mainCsdl))
            {
                CsdlJsonModelBuilder csdlModelBuilder = new CsdlJsonModelBuilder(txtReader, options);
                CsdlModel csdlModel = csdlModelBuilder.TryParseCsdlModel();

                EdmModelBuilder builder = new EdmModelBuilder(options);
                IEdmModel model = builder.TryBuildEdmModel(csdlModel, csdlModel.ReferencedModels);
                Assert.NotNull(model);
            }
        }

#if false
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

#endif

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