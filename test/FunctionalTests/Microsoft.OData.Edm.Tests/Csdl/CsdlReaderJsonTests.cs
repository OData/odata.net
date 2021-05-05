//---------------------------------------------------------------------
// <copyright file="CsdlReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlReaderJsonTests
    {
        [Fact]
        public void ReadNavigationPropertyPartnerInJsonTest()
        {
            var csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""EntityType1"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""ComplexProp"": {
        ""$Collection"": true,
        ""$Type"": ""NS.ComplexType1""
      },
      ""OuterNavA"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityType2"",
        ""$Partner"": ""OuterNavA""
      },
      ""OuterNavB"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityType2"",
        ""$Partner"": ""NS.EntityType3/OuterNavC""
      }
    },
    ""EntityType2"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""ComplexProp"": {
        ""$Type"": ""NS.ComplexType2""
      },
      ""OuterNavA"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityType1"",
        ""$Partner"": ""OuterNavA""
      },
      ""OuterNavB"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityType1"",
        ""$Partner"": ""ComplexProp/InnerNav""
      }
    },
    ""EntityType3"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityType2"",
      ""OuterNavC"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityType1"",
        ""$Partner"": ""OuterNavB""
      }
    },
    ""ComplexType1"": {
      ""$Kind"": ""ComplexType"",
      ""InnerNav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityType2""
      }
    },
    ""ComplexType2"": {
      ""$Kind"": ""ComplexType"",
      ""InnerNav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityType1""
      }
    }
  }
}";

            IEdmModel model = Parse(csdl);
            var entityType1 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType1");
            var entityType2 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType2");
            var entityType3 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType3");
            var complexType1 = (IEdmComplexType)model.FindDeclaredType("NS.ComplexType1");
            var complexType2 = (IEdmComplexType)model.FindDeclaredType("NS.ComplexType2");
            Assert.Equal("OuterNavA", ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavA")).GetPartnerPath().Path);
            Assert.Equal(
                entityType2.FindProperty("OuterNavA"),
                ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavA")).Partner);
            Assert.Equal("ComplexProp/InnerNav", ((IEdmNavigationProperty)entityType2.FindProperty("OuterNavB")).GetPartnerPath().Path);
            Assert.Equal(
                complexType1.FindProperty("InnerNav"),
                ((IEdmNavigationProperty)entityType2.FindProperty("OuterNavB")).Partner);
            Assert.Equal("NS.EntityType3/OuterNavC", ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavB")).GetPartnerPath().Path);
            Assert.Equal(
                entityType3.FindProperty("OuterNavC"),
                ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavB")).Partner);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTypeCastInJson()
        {
            var csdl = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""EntityA"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Complex"": {
        ""$Type"": ""NS.ComplexA""
      }
    },
    ""EntityB"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""ComplexA"": {
      ""$Kind"": ""ComplexType""
    },
    ""ComplexB"": {
      ""$Kind"": ""ComplexType"",
      ""ComplexBNav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityB""
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Set1"": {
        ""$Collection"": true,
        ""$Type"": ""NS.EntityA"",
        ""$NavigationPropertyBinding"": {
          ""Complex/NS.ComplexB/ComplexBNav"": ""Set2""
        }
      },
      ""Set2"": {
        ""$Collection"": true,
        ""$Type"": ""NS.EntityB""
      }
    }
  }
}";

            IEdmModel model = Parse(csdl);
            var set1 = model.FindDeclaredNavigationSource("Set1");
            Assert.True(set1.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesNoNonContainmentNavigationPropertiesInJson()
        {
            var csdl = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""EntityBase"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""EntityA"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityBase"",
      ""EntityAToB"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityB""
      }
    },
    ""EntityB"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityBase"",
      ""EntityBToC"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityC""
      }
    },
    ""EntityC"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityBase""
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""SetA"": {
        ""$Collection"": true,
        ""$Type"": ""NS.EntityA"",
        ""$NavigationPropertyBinding"": {
          ""EntityAToB/EntityBToC"": ""SetC""
        }
      },
      ""SetB"": {
        ""$Collection"": true,
        ""$Type"": ""NS.EntityB""
      },
      ""SetC"": {
        ""$Collection"": true,
        ""$Type"": ""NS.EntityC""
      }
    }
  }
}";
            IEdmModel model = Parse(csdl);
            var setA = model.FindDeclaredNavigationSource("SetA");
            Assert.True(setA.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesContainmentNavigationPropertiesInJson()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""RootEntity"": {
      ""$Kind"": ""EntityType"",
      ""SetA"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityA"",
        ""$ContainsTarget"": true
      },
      ""SetB"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityB"",
        ""$ContainsTarget"": true
      }
    },
    ""EntityA"": {
      ""$Kind"": ""EntityType"",
      ""EntityAToB"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityB""
      }
    },
    ""EntityB"": {
      ""$Kind"": ""EntityType""
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Root"": {
        ""$Type"": ""NS.RootEntity"",
        ""$NavigationPropertyBinding"": {
          ""EntityA/EntityAToB"": ""Root/SetB""
        }
      }
    }
  }
}";
            IEdmModel model = Parse(csdl);
            var setA = model.FindDeclaredNavigationSource("Root");
            var target = setA.NavigationPropertyBindings.First().Target;
            Assert.True(target is IEdmContainedEntitySet);
            Assert.Equal("SetB", target.Name);
            var targetSegments = target.Path.PathSegments.ToList();
            Assert.Equal(2, targetSegments.Count());
            Assert.Equal("Root", targetSegments[0]);
            Assert.Equal("SetB", targetSegments[1]);
            var pathSegments = setA.NavigationPropertyBindings.First().Path.PathSegments.ToList();
            Assert.Equal(2, pathSegments.Count());
            Assert.Equal("EntityA", pathSegments[0]);
            Assert.Equal("EntityAToB", pathSegments[1]);
        }

        [Fact]
        public void ValidateEducationModelInJson()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""Test.NS.EducationService"",
  ""Test.NS"": {
    ""educationRoot"": {
      ""$Kind"": ""EntityType"",
      ""classes"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""Test.NS.class"",
        ""$ContainsTarget"": true
      },
      ""users"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""Test.NS.user"",
        ""$ContainsTarget"": true
      }
    },
    ""user"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""name""
      ],
      ""name"": {},
      ""classes"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""Test.NS.class""
      }
    },
    ""class"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""classNumber""
      ],
      ""classNumber"": {},
      ""displayName"": {
        ""$Nullable"": true
      },
      ""description"": {
        ""$Nullable"": true
      },
      ""period"": {
        ""$Nullable"": true
      },
      ""members"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""Test.NS.user""
      }
    },
    ""EducationService"": {
      ""$Kind"": ""EntityContainer"",
      ""education"": {
        ""$Type"": ""Test.NS.educationRoot"",
        ""$NavigationPropertyBinding"": {
          ""classes/members"": ""education/users""
        }
      }
    }
  }
}";

            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.Empty(errors);
            model.Validate(out errors);
            Assert.Empty(errors);

            var educationSingleton = model.FindDeclaredNavigationSource("education");
            var navPropBinding = educationSingleton.NavigationPropertyBindings.First();
            var target = navPropBinding.Target;
            Assert.NotNull(target);
            Assert.True(target is IEdmContainedEntitySet);
            Assert.Equal("users", target.Name);
            var targetSegments = target.Path.PathSegments.ToList();
            Assert.Equal(2, targetSegments.Count());
            Assert.Equal("education", targetSegments[0]);
            Assert.Equal("users", targetSegments[1]);
            var pathSegments = navPropBinding.Path.PathSegments.ToList();
            Assert.Equal(2, pathSegments.Count());
            Assert.Equal("classes", pathSegments[0]);
            Assert.Equal("members", pathSegments[1]);
        }

        [Fact]
        public void ReadNavigationPropertyPartnerTypeHierarchyTestInJson()
        {
            var csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""EntityTypeA1"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""A1Nav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityTypeB"",
        ""$Partner"": ""BNav1""
      }
    },
    ""EntityTypeA2"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityTypeA1""
    },
    ""EntityTypeA3"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.EntityTypeA2"",
      ""A3Nav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityTypeB"",
        ""$Partner"": ""BNav2""
      }
    },
    ""EntityTypeB"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""BNav1"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityTypeA2"",
        ""$Partner"": ""A1Nav""
      },
      ""BNav2"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""NS.EntityTypeA3"",
        ""$Partner"": ""NS.EntityTypeA3/A3Nav""
      }
    }
  }
}";
            IEdmModel model = Parse(csdl);
            var entityTypeA1 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA1");
            var entityTypeA2 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA2");
            var entityTypeA3 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA3");
            var entityTypeB = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeB");
            Assert.Equal("BNav1", ((IEdmNavigationProperty)entityTypeA2.FindProperty("A1Nav")).GetPartnerPath().Path);
            Assert.Equal(entityTypeB.FindProperty("BNav1"), ((IEdmNavigationProperty)entityTypeA2.FindProperty("A1Nav")).Partner);
            Assert.Equal("BNav2", ((IEdmNavigationProperty)entityTypeA3.FindProperty("A3Nav")).GetPartnerPath().Path);
            Assert.Equal(entityTypeB.FindProperty("BNav2"), ((IEdmNavigationProperty)entityTypeA3.FindProperty("A3Nav")).Partner);
            Assert.Equal("A1Nav", ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav1")).GetPartnerPath().Path);
            Assert.Equal(entityTypeA2.FindProperty("A1Nav"), ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav1")).Partner);
            Assert.Equal("NS.EntityTypeA3/A3Nav", ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav2")).GetPartnerPath().Path);
            Assert.Equal(entityTypeA3.FindProperty("A3Nav"), ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav2")).Partner);
        }

        [Fact]
        public void ParsingValidJsonWithNoReferencesShouldSucceed()
        {
            var csdl = @"{ ""$Version"": ""4.01""}";

            IEdmModel model = Parse(csdl);

            Assert.NotNull(model);
            Version csdlVersion = model.GetEdmxVersion();
            Assert.Equal(EdmConstants.EdmVersion401, csdlVersion);
        }

        [Fact]
        public void ParsingInvalidJsonWithNoReferencesShouldThrow()
        {
            string expectedMessage = "UnexpectedValueKind : An unexpected 'Array' value kind was found when parsing the JSON path '$'. A 'Object' value kind was expected. : $";
            var csdl = @"[""fake""]";
            Action parseAction = () => Parse(csdl);

            EdmParseException exception = Assert.Throws<EdmParseException>(parseAction);
            Assert.Equal(ErrorStrings.EdmParseException_ErrorsEncounteredInEdmx(expectedMessage), exception.Message);
            Assert.Single(exception.Errors, e => e.ToString() == expectedMessage);
        }

        [Fact]
        public void ParsingValidJsonWithNavigationPropertyInComplex()
        {
            string complexWithNav = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""DefaultNs.Container"",
  ""DefaultNs"": {
                    ""Person"": {
                        ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""UserName""
      ],
      ""UserName"": {},
      ""HomeAddress"": {
        ""$Type"": ""DefaultNs.Address""
      },
      ""WorkAddress"": {
        ""$Type"": ""DefaultNs.Address""
      },
      ""Addresses"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.Address""
      }
    },
    ""City"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Name""
      ],
      ""Name"": {}
    },
    ""CountryOrRegion"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Name""
      ],
      ""Name"": {}
    },
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""City"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""DefaultNs.City""
      }
    },
    ""WorkAddress"": {
      ""$Kind"": ""ComplexType"",
      ""$BaseType"": ""DefaultNs.Address"",
      ""CountryOrRegion"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""DefaultNs.CountryOrRegion""
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""People"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.Person"",
        ""$NavigationPropertyBinding"": {
          ""HomeAddress/City"": ""Cities"",
          ""Addresses/City"": ""Cities"",
          ""WorkAddress/DefaultNs.WorkAddress/CountryOrRegion"": ""CountriesOrRegions""
        }
      },
      ""Cities"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.City""
      },
      ""CountriesOrRegions"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.CountryOrRegion""
      }
    }
  }
}";

            var model = Parse(complexWithNav);
            var people = model.EntityContainer.FindEntitySet("People");
            var address = model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var workAddress = model.FindType("DefaultNs.WorkAddress") as IEdmStructuredType;
            var city = address.FindProperty("City") as IEdmNavigationProperty;
            var countryOrRegion = workAddress.FindProperty("CountryOrRegion") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("Cities");
            var countriesOrRegions = model.EntityContainer.FindEntitySet("CountriesOrRegions");
            var navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("HomeAddress/City"));
            Assert.Equal(navigationTarget, cities);
            navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("Addresses/City"));
            Assert.Equal(navigationTarget, cities);
            navigationTarget = people.FindNavigationTarget(countryOrRegion, new EdmPathExpression("WorkAddress/DefaultNs.WorkAddress/CountryOrRegion"));
            Assert.Equal(navigationTarget, countriesOrRegions);
        }

        [Fact]
        public void ParsingJsonWithCollectionOfNavOnComplex()
        {
            string complexWithNav = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""DefaultNs.Container"",
  ""DefaultNs"": {
    ""EntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {},
      ""Complex"": {
        ""$Type"": ""DefaultNs.ComplexType""
      }
    },
    ""NavEntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {}
    },
    ""ComplexType"": {
      ""$Kind"": ""ComplexType"",
      ""Prop1"": {
        ""$Type"": ""Edm.Int32""
      },
      ""CollectionOfNav"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.NavEntityType""
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Entities"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.EntityType"",
        ""$NavigationPropertyBinding"": {
          ""Complex/CollectionOfNav"": ""NavEntities""
        }
      },
      ""NavEntities"": {
        ""$Collection"": true,
        ""$Type"": ""DefaultNs.NavEntityType""
      }
    }
  }
}";

            var model = Parse(complexWithNav);
            var people = model.EntityContainer.FindEntitySet("Entities");
            var address = model.FindType("DefaultNs.ComplexType") as IEdmStructuredType;
            var city = address.FindProperty("CollectionOfNav") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("NavEntities");
            var navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("Complex/CollectionOfNav"));
            Assert.Equal(navigationTarget, cities);
        }

        [Fact]
        public void ParsingOptionalParametersShouldSucceed()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""test.Default"",
  ""test"": {
    ""TestFunction"": [
      {
        ""$Kind"": ""Function"",
        ""$Parameter"": [
          {
            ""$Name"": ""requiredParam"",
            ""$Nullable"": true
          },
          {
            ""$Name"": ""optionalParam"",
            ""$Nullable"": true,
            ""@Org.OData.Core.V1.OptionalParameter"": {}
          },
          {
            ""$Name"": ""optionalParamWithDefault"",
            ""$Nullable"": true,
            ""@Org.OData.Core.V1.OptionalParameter"": {
              ""DefaultValue"": ""Smith""
            }
          }
        ],
        ""$ReturnType"": {
          ""$Type"": ""Edm.Boolean"",
          ""$Nullable"": true
        }
      }
    ],
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""TestFunction"": {
        ""$Kind"": ""FunctionImport"",
        ""$Function"": ""test.TestFunction""
      }
    }
  }
}";

            var model = Parse(csdl);
            var function = model.FindDeclaredOperations("test.TestFunction").FirstOrDefault();
            Assert.NotNull(function);
            var requiredParam = function.Parameters.Where(p => p.Name == "requiredParam").FirstOrDefault();
            Assert.NotNull(requiredParam);
            Assert.Null(requiredParam as IEdmOptionalParameter);
            IEdmOptionalParameter optionalParam = function.Parameters.Where(p => p.Name == "optionalParam").FirstOrDefault() as IEdmOptionalParameter;
            Assert.NotNull(optionalParam);
            Assert.True(String.IsNullOrEmpty(optionalParam.DefaultValueString));
            IEdmOptionalParameter optionalParamWithDefault = function.Parameters.Where(p => p.Name == "optionalParamWithDefault").FirstOrDefault() as IEdmOptionalParameter;
            Assert.NotNull(optionalParamWithDefault);
            Assert.Equal("Smith", optionalParamWithDefault.DefaultValueString);
        }

        [Fact]
        public void ParsingReturnTypeAnnotationInLineShouldSucceed()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": [
      {
        ""$Kind"": ""Function"",
        ""$ReturnType"": {
          ""$Type"": ""Edm.PrimitiveType"",
          ""@Org.OData.Validation.V1.DerivedTypeConstraint"": [
            ""Edm.Int32"",
            ""Edm.Boolean""
          ]
        }
      }
    ]
  }
}";
            var model = Parse(csdl);

            ValidateReturnTypeAnnotation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
        }

        [Fact]
        public void ParsingReturnTypeAnnotationOutOfLineShouldSucceed()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": [
      {
        ""$Kind"": ""Function"",
        ""$ReturnType"": {
          ""$Type"": ""Edm.PrimitiveType""
        }
      }
    ],
    ""$Annotations"": {
      ""NS.TestFunction()/$ReturnType"": {
        ""@Org.OData.Validation.V1.DerivedTypeConstraint"": [
          ""Edm.Int32"",
          ""Edm.Boolean""
        ]
      }
    }
  }
}";
            var model = Parse(csdl);

            ValidateReturnTypeAnnotation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
        }

        private static void ValidateReturnTypeAnnotation(IEdmModel model, EdmVocabularyAnnotationSerializationLocation location)
        {
            var function = model.FindDeclaredOperations("NS.TestFunction").FirstOrDefault();
            Assert.NotNull(function);
            Assert.NotNull(function.ReturnType);
            IEdmOperationReturn returnType = function.GetReturn();
            Assert.NotNull(returnType);
            Assert.Same(returnType.DeclaringOperation, function);
            Assert.Equal("Edm.PrimitiveType", returnType.Type.FullName());

            var termType = model.FindTerm("Org.OData.Validation.V1.DerivedTypeConstraint");
            Assert.NotNull(termType);

            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(returnType, termType).FirstOrDefault();
            Assert.NotNull(annotation);

            Assert.True(annotation.GetSerializationLocation(model) == location);
            IEdmCollectionExpression collectConstant = annotation.Value as IEdmCollectionExpression;
            Assert.NotNull(collectConstant);

            Assert.Equal(new[] { "Edm.Int32", "Edm.Boolean" }, collectConstant.Elements.Select(e => ((IEdmStringConstantExpression)e).Value));
        }

        [Fact]
        public void ParsingValidJsonWithOneReferencesShouldSucceed()
        {
            var csdl = @"{ ""$Version"": ""4.01""}";

            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            CsdlJsonReaderSettings setting = new CsdlJsonReaderSettings
            {
                ReferencedModels = new[] { EdmCoreModel.Instance }
            };
            IEdmModel model = CsdlReader.Parse(ref jsonReader, setting);

            Assert.NotNull(model);
            Assert.Equal(2, model.ReferencedModels.Where(c => object.ReferenceEquals(c, EdmCoreModel.Instance)).Count());
        }

        [Fact]
        public void ParsingInvalidXmlWithMultipleEntityContainersShouldThrow()
        {
            string csdl = @"
{
   ""$Version"": ""4.01"",
   ""Test"": {
       ""Container1"": { ""$Kind"": ""EntityContainer""},
       ""Container2"": { ""$Kind"": ""EntityContainer""}
    }
}";
            Action parseAction = () => Parse(csdl);
            var exception = Assert.Throws<EdmParseException>(parseAction);
            Assert.Contains("The schema object at '$.Test' cannot have more than one entity container.", exception.Message);
            EdmError error = Assert.Single(exception.Errors);
            Assert.Equal(EdmErrorCode.SchemaCannotHaveMoreThanOneEntityContainer, error.ErrorCode);
        }

        [Fact]
        public void ParsingNavigationPropertyWithEdmEntityTypeInJsonWorks()
        {
            string navigationProperty =
                @"""EntityNavigationProperty"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""Edm.EntityType""
      }";

            IEdmModel model = GetEdmModelFromJson(properties: navigationProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var navProperty = customer.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "EntityNavigationProperty");
            Assert.NotNull(navProperty);
            Assert.True(navProperty.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetEntityType(), navProperty.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            navProperty = address.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "EntityNavigationProperty");
            Assert.NotNull(navProperty);
            Assert.True(navProperty.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetEntityType(), navProperty.Type.Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmComplexTypeInJsonWorks()
        {
            string complexProperty = @"""ComplexProperty"": {
        ""$Type"": ""Edm.ComplexType"",
        ""$Nullable"": true
      }";

            IEdmModel model = GetEdmModelFromJson(properties: complexProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetComplexType(), property.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetComplexType(), property.Type.Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmPrimitiveTypeInJsonWorks()
        {
            string primitiveProperty = @"""PrimitiveProperty"": {
        ""$Type"": ""Edm.PrimitiveType""
      }";

            IEdmModel model = GetEdmModelFromJson(properties: primitiveProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "PrimitiveProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(), property.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "PrimitiveProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(), property.Type.Definition);
        }

        [Fact]
        public void ParsingTermPropertyWithEdmPathTypeInJsonWorks()
        {
            string termTypes = @"""SelectType"": {
      ""$Kind"": ""ComplexType"",
      ""DefaultSelect"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PropertyPath"",
        ""$Nullable"": true
      },
      ""DefaultHidden"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.NavigationPropertyPath""
      }
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""NS.SelectType"",
      ""$Nullable"": true
    }";

            IEdmModel model = GetEdmModelFromJson(types: termTypes);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var complex = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "SelectType");
            Assert.NotNull(complex);
            var property = complex.DeclaredProperties.FirstOrDefault(c => c.Name == "DefaultSelect");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.PropertyPath), property.Type.AsCollection().ElementType().Definition);

            property = complex.DeclaredProperties.FirstOrDefault(c => c.Name == "DefaultHidden");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.NavigationPropertyPath), property.Type.AsCollection().ElementType().Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmPathTypeInJsonWorksButValidationFailed()
        {
            string properties = @"""PathProperty"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PropertyPath"",
        ""$Nullable"": true
      }";

            IEdmModel model = GetEdmModelFromJson(properties: properties);

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "PathProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.PropertyPath), property.Type.AsCollection().ElementType().Definition);

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            var error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty, error.ErrorCode);
        }

        [Fact]
        public void ParsingRecursivePropertyWithEdmPathTypeInJsonWorksButValidationFailed()
        {
            string properties = @"""PathProperty"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PropertyPath"",
        ""$Nullable"": true
      },
      ""ComplexProperty"": {
        ""$Type"": ""NS.Address"",
        ""$Nullable"": true
      }";

            IEdmModel model = GetEdmModelFromJson(properties: properties);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            var property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Equal(EdmTypeKind.Complex, property.Type.TypeKind());

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            var error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty, error.ErrorCode);
        }

        [Fact]
        public void ParsingNavigationPropertyWithEdmPathTypeInJsonWorksButValidationFailed()
        {
            string properties =
                @"""PathProperty"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.AnnotationPath"",
        ""$Nullable"": true
      },
      ""NavigationProperty"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }";

            IEdmModel model = GetEdmModelFromJson(properties: properties);

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var navProperty = customer.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "NavigationProperty");
            Assert.NotNull(navProperty);

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.NotNull(errors);
            Assert.Equal(3, errors.Count());

            Assert.Equal(new[] {EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty,
                EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty,
                EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty}, errors.Select(e => e.ErrorCode));
        }

        [Fact]
        public void ParsingEnumMemberWithAnnotationsInJsonWorks()
        {
            string types =
@"""Color"": {
      ""$Kind"": ""EnumType"",
      ""Red"": 1,
      ""Red@Org.OData.Core.V1.LongDescription"": ""Inline Description"",
      ""Red@NS.MyTerm"": ""Inline MyTerm Value"",
      ""Blue"": 2
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""$Annotations"": {
      ""NS.Color/Blue"": {
        ""@Org.OData.Core.V1.LongDescription"": ""OutOfLine Description"",
        ""@NS.MyTerm"": ""OutOfLine MyTerm Value""
      }
    }";

            IEdmModel model = GetEdmModelFromJson(types: types);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var color = model.SchemaElements.OfType<IEdmEnumType>().FirstOrDefault(c => c.Name == "Color");
            Assert.NotNull(color);

            IEdmTerm fooBarTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(fooBarTerm);

            // Red
            var red = color.Members.FirstOrDefault(c => c.Name == "Red");
            Assert.NotNull(red);
            string redAnnotation = GetStringAnnotation(model, red, CoreVocabularyModel.LongDescriptionTerm, EdmVocabularyAnnotationSerializationLocation.Inline);
            Assert.Equal("Inline Description", redAnnotation);

            redAnnotation = GetStringAnnotation(model, red, fooBarTerm, EdmVocabularyAnnotationSerializationLocation.Inline);
            Assert.Equal("Inline MyTerm Value", redAnnotation);

            // Blue
            var blue = color.Members.FirstOrDefault(c => c.Name == "Blue");
            Assert.NotNull(blue);
            string blueAnnotation = GetStringAnnotation(model, blue, CoreVocabularyModel.LongDescriptionTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("OutOfLine Description", blueAnnotation);

            blueAnnotation = GetStringAnnotation(model, blue, fooBarTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("OutOfLine MyTerm Value", blueAnnotation);
        }

        private string GetStringAnnotation(IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, EdmVocabularyAnnotationSerializationLocation location)
        {
            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
            if (annotation != null)
            {
                Assert.True(annotation.GetSerializationLocation(model) == location);

                IEdmStringConstantExpression stringConstant = annotation.Value as IEdmStringConstantExpression;
                if (stringConstant != null)
                {
                    return stringConstant.Value;
                }
            }

            return null;
        }

        private static IEdmModel GetEdmModelFromJson(string types = null, string properties = null)
        {
            const string template = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      }
      PROPERTYTEMPLATE
    },
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""Street"": {
        ""$Nullable"": true
      }
      PROPERTYTEMPLATE
    },
    TYPETEMPLATE
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    }
  }
}";
            types = types != null ? types + "," : "";
            properties = properties != null ? "," + properties : "";
            string modelText = template.Replace("PROPERTYTEMPLATE", properties);
            modelText = modelText.Replace("TYPETEMPLATE", types);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(modelText);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingInLineOptionalParameterInJsonWorks()
        {
            string expected = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
                ""TestFunction"": [
                  {
        ""$Kind"": ""Function"",
                    ""$Parameter"": [
                      {
            ""$Name"": ""requiredParam""
          },
          {
            ""$Name"": ""optionalParam"",
            ""@Org.OData.Core.V1.OptionalParameter"": {}
          },
          {
            ""$Name"": ""optionalParamWithDefault"",
            ""@Org.OData.Core.V1.OptionalParameter"": {
              ""DefaultValue"": ""Smith""
            }
          }
        ],
        ""$ReturnType"": {}
      }
    ]
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(expected);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            VerifyOptionalParameter(function);
        }

        [Fact]
        public void ParsingOutOfLineOptionalParameterInJsonWorks()
        {
            string expected = @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""TestFunction"": [
      {
        ""$Kind"": ""Function"",
        ""$Parameter"": [
          {
            ""$Name"": ""requiredParam""
          },
          {
            ""$Name"": ""optionalParam""
          },
          {
            ""$Name"": ""optionalParamWithDefault""
          }
        ],
        ""$ReturnType"": {}
      }
    ],
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""TestFunction"": {
        ""$Kind"": ""FunctionImport"",
        ""$Function"": ""test.TestFunction""
      }
    },
    ""$Annotations"": {
      ""NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParam"": {
        ""@Org.OData.Core.V1.OptionalParameter"": {}
      },
      ""NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParamWithDefault"": {
        ""@Org.OData.Core.V1.OptionalParameter"": {
          ""$Type"": ""Org.OData.Core.V1.OptionalParameterType"",
          ""DefaultValue"": ""Smith""
        }
      }
    }
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(expected);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            VerifyOptionalParameter(function);
        }

        private static void VerifyOptionalParameter(IEdmFunction function)
        {
            Assert.NotNull(function);
            Assert.Equal("TestFunction", function.Name);

            var requiredParam = function.Parameters.First(c => c.Name == "requiredParam");
            Assert.True(requiredParam is IEdmOperationParameter);
            Assert.False(requiredParam is IEdmOptionalParameter);

            var optionalParam = function.Parameters.First(c => c.Name == "optionalParam");
            Assert.True(optionalParam is IEdmOptionalParameter);

            var optionalParamWithDefault = function.Parameters.First(c => c.Name == "optionalParamWithDefault");
            Assert.True(optionalParamWithDefault is IEdmOptionalParameter);
            var parameter = optionalParamWithDefault as IEdmOptionalParameter;
            Assert.Equal("Smith", parameter.DefaultValueString);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParsingUrlEscapeFunctionInJsonWorks(bool escaped)
        {
            string format = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Entity"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""Function"": [
      {
        ""$Kind"": ""Function"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""entity"",
            ""$Type"": ""NS.Entity"",
            ""$Nullable"": true
          },
          {
            ""$Name"": ""path"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Type"": ""Edm.Int32"",
          ""$Nullable"": true
        }
        ESCAPEFUNCTIONTEMPLATE
      }
    ]
  }
}";

            string annotation = escaped ? @",""@Org.OData.Community.V1.UrlEscapeFunction"": true" : "";
            string csdl = format.Replace("ESCAPEFUNCTIONTEMPLATE", annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            Assert.Equal(escaped, model.IsUrlEscapeFunction(function));
        }

        [Fact]
        public void ParsingBaseAndDerivedTypeWithSameAnnotationInJsonWorksButValidationSuccessful()
        {
            string annotations = @"""$Annotations"": {
      ""NS.Base"": {
        ""@Org.OData.Core.V1.Description"": ""Base description""
      },
      ""NS.Derived"": {
        ""@Org.OData.Core.V1.Description"": ""Derived description""
      }
    }";

            IEdmModel model = GetInheritanceEdmModelFromJson(annotations);

            var edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Base");
            Assert.NotNull(edmType);
            Assert.Equal("Base description", model.GetDescriptionAnnotation(edmType));

            edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Derived");
            Assert.NotNull(edmType);
            Assert.Equal("Derived description", model.GetDescriptionAnnotation(edmType));

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));
        }

        [Fact]
        public void ParsingDerivedTypeWithDuplicatedAnnotationsInJsonWorksButValidationFailed()
        {
            string annotations = @"""$Annotations"": {
      ""NS.Derived"": {
        ""@Org.OData.Core.V1.Description"": ""Derived description 1"",
        ""@Org.OData.Core.V1.Description"": ""Derived description 2""
      }
    }";

            IEdmModel model = GetInheritanceEdmModelFromJson(annotations);

            var edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Derived");
            Assert.NotNull(edmType);
            var descriptions = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(edmType, CoreVocabularyModel.DescriptionTerm);
            Assert.Equal(new[] { "Derived description 1", "Derived description 2" },
                descriptions.Select(d => d.Value as IEdmStringConstantExpression).Select(e => e.Value));

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            EdmError error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DuplicateAnnotation, error.ErrorCode);
            Assert.Equal("The annotated element 'NS.Derived' has multiple annotations with the term 'Org.OData.Core.V1.Description' and the qualifier ''.", error.ErrorMessage);
        }

        private static IEdmModel GetInheritanceEdmModelFromJson(string annotation)
        {
            const string template = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Base"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""Derived"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.Base""
    },
    ANNOTATION
  }
}";
            string modelText = template.Replace("ANNOTATION", annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            Utf8JsonReader jsonReader = GetJsonReader(modelText);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingPropertyWithCoreTypeDefinitionAsPropertyTypeInJsonWorks()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""ModifiedDate"": {
        ""$Type"": ""Org.OData.Core.V1.LocalDateTime"",
        ""$Nullable"": true
      }
    }
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var edmType = model.SchemaElements.OfType<IEdmComplexType>().First();
            Assert.NotNull(edmType);
            var property = edmType.Properties().FirstOrDefault(c => c.Name == "ModifiedDate");
            Assert.NotNull(property);

            Assert.Equal(EdmTypeKind.TypeDefinition, property.Type.TypeKind());
            Assert.Equal("Org.OData.Core.V1.LocalDateTime", property.Type.FullName());
        }

        [Fact]
        public void ParsingAnnotationPathExpressionInJsonWorks()
        {
            string csdl =
            @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""@NS.MyAnnotationPathTerm"": ""abc/efg"",
      ""@NS.MyNavigationPathTerm"": ""123/456.t""
    },
    ""MyAnnotationPathTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""Edm.AnnotationPath""
    },
    ""MyNavigationPathTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""Edm.NavigationPropertyPath""
    }
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var complexType = model.SchemaElements.OfType<IEdmComplexType>().First();
            Assert.NotNull(complexType);

            IEdmTerm term = model.SchemaElements.OfType<IEdmTerm>().First(t => t.Name == "MyAnnotationPathTerm");
            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(complexType, term).FirstOrDefault();
            Assert.Equal(EdmExpressionKind.AnnotationPath, annotation.Value.ExpressionKind);
            Assert.Equal("abc/efg", ((IEdmPathExpression)annotation.Value).Path);

            term = model.SchemaElements.OfType<IEdmTerm>().First(t => t.Name == "MyNavigationPathTerm");
            annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(complexType, term).FirstOrDefault();
            Assert.Equal(EdmExpressionKind.NavigationPropertyPath, annotation.Value.ExpressionKind);
            Assert.Equal("123/456.t", ((IEdmPathExpression)annotation.Value).Path);
        }

        [Fact]
        public void ParsingAnnotationEnumMemberExpressionInJsonWorks()
        {
            string csdl =
            @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""@NS.MyTerm"": ""Read, Write"",
      ""@NS.MyTerm#Tablet1"": ""13"",
      ""@NS.MyTerm#Tablet2"": ""10""
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""NS.Permission""
    },
    ""Permission"": {
        ""$Kind"": ""EnumType"",
        ""$IsFlags"": true,
        ""None"": 1,
        ""Read"": 2,
        ""Write"": 4,
        ""ReadWrite"": 6,
        ""Invoke"": 8,
        ""ReadInvoke"": 10
    }
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var complexType = model.SchemaElements.OfType<IEdmComplexType>().First();
            Assert.NotNull(complexType);

            IEdmTerm term = model.SchemaElements.OfType<IEdmTerm>().First(t => t.Name == "MyTerm");
            var annotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(complexType, term);
            Assert.Equal(3, annotations.Count());

            // #1, Using symbolic value
            IEdmVocabularyAnnotation annotation = annotations.First(c => c.Qualifier == null);
            Assert.Equal(EdmExpressionKind.EnumMember, annotation.Value.ExpressionKind);
            IEdmEnumMemberExpression enumMemberExpression = (IEdmEnumMemberExpression)annotation.Value;
            Assert.Equal(2, enumMemberExpression.EnumMembers.Count());
            Assert.Equal(new[] { "Read", "Write" }, enumMemberExpression.EnumMembers.Select(e => e.Name));

            // #2, Using numeric value: 13 = 1 + 4 + 8
            annotation = annotations.First(c => c.Qualifier == "Tablet1");
            Assert.Equal(EdmExpressionKind.EnumMember, annotation.Value.ExpressionKind);
            enumMemberExpression = (IEdmEnumMemberExpression)annotation.Value;
            Assert.Equal(3, enumMemberExpression.EnumMembers.Count());
            Assert.Equal(new[] { "None", "Write", "Invoke" }, enumMemberExpression.EnumMembers.Select(e => e.Name));

            // #3, Using numeric value: 10 = 2 + 8, but it's defined in the Enum type, so directly use it.
            annotation = annotations.First(c => c.Qualifier == "Tablet2");
            Assert.Equal(EdmExpressionKind.EnumMember, annotation.Value.ExpressionKind);
            enumMemberExpression = (IEdmEnumMemberExpression)annotation.Value;
            IEdmEnumMember enumMember = Assert.Single(enumMemberExpression.EnumMembers);
            Assert.Equal("ReadInvoke", enumMember.Name);
        }

        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ValidateCsdlVersions(string odataVersion)
        {
            string csdl = @"{""$Version"":""" + odataVersion + @"""}";

            Utf8JsonReader jsonReader = GetJsonReader(csdl);

            IEdmModel edmModel;
            IEnumerable<EdmError> edmErrors;

            // Read in the CSDL and verify the version
            bool result = CsdlReader.TryParse(ref jsonReader, out edmModel, out edmErrors);
            Assert.True(result);
            Assert.Empty(edmErrors);
            Assert.Equal(edmModel.GetEdmVersion(), odataVersion == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401);
        }

        [Fact]
        public void TryParseWorksForProductsAndCategoriesExample()
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

            Utf8JsonReader jsonReader = GetJsonReader(json);

            CsdlJsonReaderSettings setting = new CsdlJsonReaderSettings
            {
                JsonSchemaReaderFactory = GetMeasureJsonReader
            };

            bool ok = CsdlReader.TryParse(ref jsonReader, setting, out IEdmModel model, out IEnumerable<EdmError> errors);
            Assert.True(ok);

            // we have one unexpected annotation which is not supported yet in ODL.
            Assert.Single(errors);
            Assert.NotNull(model);

            // 8 built-in + 1 referenced outside
            Assert.Equal(9, model.ReferencedModels.Count());
            IEdmTerm isoCurrencyTerm = model.FindTerm("Org.OData.Measures.V1.ISOCurrency");
            Assert.NotNull(isoCurrencyTerm);

            // Schema elements
            Assert.Equal(7, model.SchemaElements.Count());

            var complexTypes = model.SchemaElements.OfType<IEdmComplexType>();
            Assert.Equal(new[] { "Address" }, complexTypes.Select(e => e.Name));

            var entityTypes = model.SchemaElements.OfType<IEdmEntityType>();
            Assert.Equal(new[] { "Product", "Category", "Supplier", "Country" }, entityTypes.Select(e => e.Name));

            // EntityContainer
            Assert.NotNull(model.EntityContainer);
            Assert.Equal("ODataDemo.DemoService", model.EntityContainer.FullName());

            var entitySets = model.EntityContainer.EntitySets();
            Assert.Equal(4, entitySets.Count());
            Assert.Equal(new[] { "Products", "Categories", "Suppliers", "Countries" }, entitySets.Select(e => e.Name));

            var singletons = model.EntityContainer.Singletons();
            IEdmSingleton mainSupplier = Assert.Single(singletons);
            Assert.Equal("MainSupplier", mainSupplier.Name);
        }

        [Fact]
        public void ParsingPropertyWithUntypedTypeAsPropertyTypeInJsonWorks()
        {
            string csdl = @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""Value"": {
        ""$Type"": ""Edm.Untyped"",
        ""$Nullable"": true
      },
      ""Data"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.Untyped"",
        ""$Nullable"": true
      }
    }
  }
}";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            bool result = CsdlReader.TryParse(ref jsonReader, out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            IEdmComplexType complexType = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault();
            Assert.NotNull(complexType);

            Assert.Equal(2, complexType.Properties().Count());

            IEdmProperty valueProperty = complexType.Properties().FirstOrDefault(p => p.Name == "Value");
            Assert.NotNull(valueProperty);
            Assert.Equal("Edm.Untyped", valueProperty.Type.FullName());

            IEdmProperty dataProperty = complexType.Properties().FirstOrDefault(p => p.Name == "Data");
            Assert.NotNull(dataProperty);
            Assert.Equal("Collection(Edm.Untyped)", dataProperty.Type.FullName());
        }

        private static Utf8JsonReader GetMeasureJsonReader(Uri uri, out bool skip)
        {
            string measures = @"{
    ""$Version"": ""4.0"",
    ""Org.OData.Measures.V1"": {
      ""$Alias"": ""Measures"",
      ""ISOCurrency"": {
         ""$Kind"": ""Term"",
         ""$AppliesTo"": [
                ""Property""
            ],
            ""@Core.Description"": ""The currency for this monetary amount as an ISO 4217 currency code""
        }
   }
}";

            if (uri == new Uri("https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Measures.V1.json"))
            {
                skip = false;
                ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(measures);
                return new Utf8JsonReader(jsonReadOnlySpan);
            }

            skip = true;
            return new Utf8JsonReader();
        }

        private static Utf8JsonReader GetJsonReader(string json)
        {
            ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(json);
            return new Utf8JsonReader(jsonReadOnlySpan);
        }

        private static IEdmModel Parse(string csdl)
        {
            Utf8JsonReader jsonReader = GetJsonReader(csdl);
            return CsdlReader.Parse(ref jsonReader);
        }
    }
}
#endif