//---------------------------------------------------------------------
// <copyright file="CsdlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlWriterTests
    {
        #region Reference
        [Fact]
        public void ShouldWriteEdmReference()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmReference reference1 = new EdmReference(new Uri("https://example.com/Org.OData.Authorization.V1.xml"));
            reference1.AddInclude(new EdmInclude("Auth", "Org.OData.Authorization.V1"));
            reference1.AddIncludeAnnotations(new EdmIncludeAnnotations("org.example.validation", null, null));
            reference1.AddIncludeAnnotations(new EdmIncludeAnnotations("org.example.display", "Tablet", null));

            EdmReference reference2 = new EdmReference(new Uri("https://example.com/Org.OData.Core.V1.xml"));
            reference2.AddInclude(new EdmInclude("Core", "Org.OData.Core.V1"));
            reference2.AddIncludeAnnotations(new EdmIncludeAnnotations("org.example.hcm", null, "com.example.Sales"));
            reference2.AddIncludeAnnotations(new EdmIncludeAnnotations("org.example.hcm", "Tablet", "com.example.Person"));
            model.SetEdmReferences(new[] { reference1, reference2 });

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:Reference Uri=\"https://example.com/Org.OData.Authorization.V1.xml\">" +
                "<edmx:Include Namespace=\"Org.OData.Authorization.V1\" Alias=\"Auth\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.validation\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.display\" Qualifier=\"Tablet\" />" +
              "</edmx:Reference>" +
              "<edmx:Reference Uri=\"https://example.com/Org.OData.Core.V1.xml\">" +
                "<edmx:Include Namespace=\"Org.OData.Core.V1\" Alias=\"Core\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.hcm\" TargetNamespace=\"com.example.Sales\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.hcm\" Qualifier=\"Tablet\" TargetNamespace=\"com.example.Person\" />" +
              "</edmx:Reference>" +
              "<edmx:DataServices />" +
            "</edmx:Edmx>", csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$Reference"": {
    ""https://example.com/Org.OData.Authorization.V1.xml"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Authorization.V1"",
          ""$Alias"": ""Auth""
        }
      ],
      ""$IncludeAnnotations"": [
        {
          ""$TermNamespace"": ""org.example.validation""
        },
        {
          ""$TermNamespace"": ""org.example.display"",
          ""$Qualifier"": ""Tablet""
        }
      ]
    },
    ""https://example.com/Org.OData.Core.V1.xml"": {
      ""$Include"": [
        {
          ""$Namespace"": ""Org.OData.Core.V1"",
          ""$Alias"": ""Core""
        }
      ],
      ""$IncludeAnnotations"": [
        {
          ""$TermNamespace"": ""org.example.hcm"",
          ""$TargetNamespace"": ""com.example.Sales""
        },
        {
          ""$TermNamespace"": ""org.example.hcm"",
          ""$Qualifier"": ""Tablet"",
          ""$TargetNamespace"": ""com.example.Person""
        }
      ]
    }
  }
}", csdlStr);
        }
        #endregion

        #region Annotation - Computed, OptimisticConcurrency

        [Fact]
        public void VerifyAnnotationComputedConcurrency()
        {
            // Arrange
            var model = new EdmModel();
            var entity = new EdmEntityType("NS1", "Product");
            var entityId = entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            entity.AddKeys(entityId);
            EdmStructuralProperty name1 = entity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty timeVer = entity.AddStructuralProperty("UpdatedTime", EdmCoreModel.Instance.GetDate(false));
            model.AddElement(entity);

            SetComputedAnnotation(model, entityId);  // semantic meaning is V3's 'Identity' for Key property
            SetComputedAnnotation(model, timeVer);   // semantic meaning is V3's 'Computed' for non-key property

            var entityContainer = new EdmEntityContainer("NS1", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet set1 = new EdmEntitySet(entityContainer, "Products", entity);
            model.SetOptimisticConcurrencyAnnotation(set1, new IEdmStructuralProperty[] { entityId, timeVer });
            entityContainer.AddElement(set1);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "<edmx:DataServices>" +
                    "<Schema Namespace=\"NS1\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                      "<EntityType Name=\"Product\">" +
                        "<Key>" +
                          "<PropertyRef Name=\"Id\" />" +
                        "</Key>" +
                        "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\">" +
                          "<Annotation Term=\"Org.OData.Core.V1.Computed\" Bool=\"true\" />" +
                        "</Property>" +
                        "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                        "<Property Name=\"UpdatedTime\" Type=\"Edm.Date\" Nullable=\"false\">" +
                          "<Annotation Term=\"Org.OData.Core.V1.Computed\" Bool=\"true\" />" +
                        "</Property>" +
                      "</EntityType>" +
                      "<EntityContainer Name=\"Container\">" +
                        "<EntitySet Name=\"Products\" EntityType=\"NS1.Product\">" +
                          "<Annotation Term=\"Org.OData.Core.V1.OptimisticConcurrency\">" +
                            "<Collection>" +
                              "<PropertyPath>Id</PropertyPath>" +
                              "<PropertyPath>UpdatedTime</PropertyPath>" +
                            "</Collection>" +
                          "</Annotation>" +
                        "</EntitySet>" +
                      "</EntityContainer>" +
                    "</Schema>" +
                  "</edmx:DataServices>" +
                "</edmx:Edmx>", csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
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
        ""$Kind"": ""EntitySet"",
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
}", csdlStr);
        }

        [Fact]
        public void WriteNavigationPropertyInComplexType()
        {
            // Arrange
            var model = new EdmModel();

            var person = new EdmEntityType("DefaultNs", "Person");
            var entityId = person.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(entityId);

            var city = new EdmEntityType("DefaultNs", "City");
            var cityId = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            city.AddKeys(cityId);

            var countryOrRegion = new EdmEntityType("DefaultNs", "CountryOrRegion");
            var countryId = countryOrRegion.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            countryOrRegion.AddKeys(countryId);

            var complex = new EdmComplexType("DefaultNs", "Address");
            complex.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var derivedComplex = new EdmComplexType("DefaultNs", "WorkAddress", complex);
            var navP2 = derivedComplex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CountryOrRegion",
                    Target = countryOrRegion,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            person.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("WorkAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

            model.AddElement(person);
            model.AddElement(city);
            model.AddElement(countryOrRegion);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
            EdmEntitySet cities = new EdmEntitySet(entityContainer, "City", city);
            EdmEntitySet countriesOrRegions = new EdmEntitySet(entityContainer, "CountryOrRegion", countryOrRegion);
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("HomeAddress/City"));
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Addresses/City"));
            people.AddNavigationTarget(navP2, countriesOrRegions, new EdmPathExpression("WorkAddress/DefaultNs.WorkAddress/CountryOrRegion"));
            entityContainer.AddElement(people);
            entityContainer.AddElement(cities);
            entityContainer.AddElement(countriesOrRegions);

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            Assert.Equal(actualErrors.Count(), 0);

            // Act & Assert for XML
            string actual = GetCsdl(model, CsdlTarget.OData);
            string expected =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                "<Schema Namespace=\"DefaultNs\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                "<EntityType Name=\"Person\">" +
                    "<Key><PropertyRef Name=\"UserName\" /></Key>" +
                    "<Property Name=\"UserName\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<Property Name=\"HomeAddress\" Type=\"DefaultNs.Address\" Nullable=\"false\" />" +
                    "<Property Name=\"WorkAddress\" Type=\"DefaultNs.Address\" Nullable=\"false\" />" +
                    "<Property Name=\"Addresses\" Type=\"Collection(DefaultNs.Address)\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<EntityType Name=\"City\">" +
                    "<Key><PropertyRef Name=\"Name\" /></Key>" +
                    "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<EntityType Name=\"CountryOrRegion\">" +
                    "<Key><PropertyRef Name=\"Name\" /></Key>" +
                    "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<ComplexType Name=\"Address\">" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<NavigationProperty Name=\"City\" Type=\"DefaultNs.City\" Nullable=\"false\" />" +
                "</ComplexType>" +
                "<ComplexType Name=\"WorkAddress\" BaseType=\"DefaultNs.Address\">" +
                    "<NavigationProperty Name=\"CountryOrRegion\" Type=\"DefaultNs.CountryOrRegion\" Nullable=\"false\" /><" +
                "/ComplexType>" +
                "<EntityContainer Name=\"Container\">" +
                "<EntitySet Name=\"People\" EntityType=\"DefaultNs.Person\">" +
                    "<NavigationPropertyBinding Path=\"Addresses/City\" Target=\"City\" />" +
                    "<NavigationPropertyBinding Path=\"HomeAddress/City\" Target=\"City\" />" +
                    "<NavigationPropertyBinding Path=\"WorkAddress/DefaultNs.WorkAddress/CountryOrRegion\" Target=\"CountryOrRegion\" />" +
                "</EntitySet>" +
                "<EntitySet Name=\"City\" EntityType=\"DefaultNs.City\" />" +
                "<EntitySet Name=\"CountryOrRegion\" EntityType=\"DefaultNs.CountryOrRegion\" />" +
                "</EntityContainer></Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            Assert.Equal(expected, actual);

            // Act & Assert for JSON
            actual = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""DefaultNs.Container"",
  ""DefaultNs"": {
    ""Person"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""UserName""
      ],
      ""UserName"": { },
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
      ""Name"": { }
    },
    ""CountryOrRegion"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Name""
      ],
      ""Name"": { }
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
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""DefaultNs.Person"",
        ""$NavigationPropertyBinding"": {
          ""Addresses/City"": ""City"",
          ""HomeAddress/City"": ""City"",
          ""WorkAddress/DefaultNs.WorkAddress/CountryOrRegion"": ""CountryOrRegion""
        }
      },
      ""City"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""DefaultNs.City""
      },
      ""CountryOrRegion"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""DefaultNs.CountryOrRegion""
      }
    }
  }
}", actual);
        }

        [Fact]
        public void WriteCollectionOfNavigationOnComplex()
        {
            // Arrange
            var model = new EdmModel();

            var entity = new EdmEntityType("DefaultNs", "EntityType");
            var entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            entity.AddKeys(entityId);

            var navEntity = new EdmEntityType("DefaultNs", "NavEntityType");
            var navEntityId = navEntity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            navEntity.AddKeys(navEntityId);

            var complex = new EdmComplexType("DefaultNs", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CollectionOfNav",
                    Target = navEntity,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            entity.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, false));

            model.AddElement(entity);
            model.AddElement(navEntity);
            model.AddElement(complex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet entites = new EdmEntitySet(entityContainer, "Entities", entity);
            EdmEntitySet navEntities = new EdmEntitySet(entityContainer, "NavEntities", navEntity);
            entites.AddNavigationTarget(navP, navEntities, new EdmPathExpression("Complex/CollectionOfNav"));
            entityContainer.AddElement(entites);
            entityContainer.AddElement(navEntities);

            // Act & Assert for XML
            string actual = GetCsdl(model, CsdlTarget.OData);
            string expected = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                              "<edmx:DataServices><Schema Namespace=\"DefaultNs\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                              "<EntityType Name=\"EntityType\">" +
                                  "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                  "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                                  "<Property Name=\"Complex\" Type=\"DefaultNs.ComplexType\" Nullable=\"false\" />" +
                              "</EntityType>" +
                              "<EntityType Name=\"NavEntityType\">" +
                                  "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                  "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                              "</EntityType>" +
                              "<ComplexType Name=\"ComplexType\">" +
                                  "<Property Name=\"Prop1\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                  "<NavigationProperty Name=\"CollectionOfNav\" Type=\"Collection(DefaultNs.NavEntityType)\" />" +
                              "</ComplexType>" +
                              "<EntityContainer Name=\"Container\">" +
                              "<EntitySet Name=\"Entities\" EntityType=\"DefaultNs.EntityType\">" +
                                "<NavigationPropertyBinding Path=\"Complex/CollectionOfNav\" Target=\"NavEntities\" />" +
                              "</EntitySet>" +
                              "<EntitySet Name=\"NavEntities\" EntityType=\"DefaultNs.NavEntityType\" />" +
                              "</EntityContainer>" +
                              "</Schema>" +
                              "</edmx:DataServices>" +
                              "</edmx:Edmx>";

            Assert.Equal(expected, actual);

            // Act & Assert for JSON
            actual = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""DefaultNs.Container"",
  ""DefaultNs"": {
    ""EntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": { },
      ""Complex"": {
        ""$Type"": ""DefaultNs.ComplexType""
      }
    },
    ""NavEntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": { }
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
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""DefaultNs.EntityType"",
        ""$NavigationPropertyBinding"": {
          ""Complex/CollectionOfNav"": ""NavEntities""
        }
      },
      ""NavEntities"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""DefaultNs.NavEntityType""
      }
    }
  }
}", actual);
        }

        [Fact]
        public void ContainedUnderComplexTest()
        {
            // Arrange
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "EntityType");
            var entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            entity.AddKeys(entityId);

            var containedEntity = new EdmEntityType("NS", "ContainedEntityType");
            var containedEntityId = containedEntity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            containedEntity.AddKeys(containedEntityId);

            var complex = new EdmComplexType("NS", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

            var containedUnderComplex = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "ContainedUnderComplex",
                    Target = containedEntity,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true
                });

            var navUnderContained = containedEntity.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NavUnderContained",
                    Target = entity,
                    TargetMultiplicity = EdmMultiplicity.Many
                });

            entity.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, false));

            model.AddElement(entity);
            model.AddElement(containedEntity);
            model.AddElement(complex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet entites1 = new EdmEntitySet(entityContainer, "Entities1", entity);
            EdmEntitySet entites2 = new EdmEntitySet(entityContainer, "Entities2", entity);
            entites1.AddNavigationTarget(navUnderContained, entites2,
                new EdmPathExpression("Complex/ContainedUnderComplex/NavUnderContained"));
            entityContainer.AddElement(entites1);
            entityContainer.AddElement(entites2);

            var entitySet1 = model.EntityContainer.FindEntitySet("Entities1");
            var entitySet2 = model.EntityContainer.FindEntitySet("Entities2");
            var containedEntitySet = entitySet1.FindNavigationTarget(containedUnderComplex,
                new EdmPathExpression("Complex/ContainedUnderComplex"));
            Assert.Equal(containedEntitySet.Name, "ContainedUnderComplex");
            var entitySetUnderContained = containedEntitySet.FindNavigationTarget(navUnderContained);
            Assert.Equal(entitySetUnderContained, entitySet2);

            // Act & Assert for XML
            string actual = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "<edmx:DataServices><Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                    "<EntityType Name=\"EntityType\">" +
                      "<Key><PropertyRef Name=\"ID\" /></Key>" +
                      "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"Complex\" Type=\"NS.ComplexType\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<EntityType Name=\"ContainedEntityType\">" +
                      "<Key><PropertyRef Name=\"ID\" /></Key>" +
                      "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<NavigationProperty Name=\"NavUnderContained\" Type=\"Collection(NS.EntityType)\" />" +
                    "</EntityType>" +
                    "<ComplexType Name=\"ComplexType\">" +
                      "<Property Name=\"Prop1\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                      "<NavigationProperty Name=\"ContainedUnderComplex\" Type=\"Collection(NS.ContainedEntityType)\" ContainsTarget=\"true\" />" +
                    "</ComplexType>" +
                    "<EntityContainer Name=\"Container\">" +
                      "<EntitySet Name=\"Entities1\" EntityType=\"NS.EntityType\">" +
                        "<NavigationPropertyBinding Path=\"Complex/ContainedUnderComplex/NavUnderContained\" Target=\"Entities2\" />" +
                      "</EntitySet>" +
                      "<EntitySet Name=\"Entities2\" EntityType=\"NS.EntityType\" />" +
                    "</EntityContainer>" +
                  "</Schema>" +
                "</edmx:DataServices>" +
              "</edmx:Edmx>", actual);

            // Act & Assert for JSON
            actual = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Container"",
  ""NS"": {
    ""EntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": { },
      ""Complex"": {
        ""$Type"": ""NS.ComplexType""
      }
    },
    ""ContainedEntityType"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""ID""
      ],
      ""ID"": { },
      ""NavUnderContained"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.EntityType""
      }
    },
    ""ComplexType"": {
      ""$Kind"": ""ComplexType"",
      ""Prop1"": {
        ""$Type"": ""Edm.Int32""
      },
      ""ContainedUnderComplex"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.ContainedEntityType"",
        ""$ContainsTarget"": true
      }
    },
    ""Container"": {
      ""$Kind"": ""EntityContainer"",
      ""Entities1"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""NS.EntityType"",
        ""$NavigationPropertyBinding"": {
          ""Complex/ContainedUnderComplex/NavUnderContained"": ""Entities2""
        }
      },
      ""Entities2"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""NS.EntityType""
      }
    }
  }
}", actual);
        }

        [Fact]
        public void SetNavigationPropertyPartnerTest()
        {
            // build model
            var model = new EdmModel();
            var entityType1 = new EdmEntityType("NS", "EntityType1");
            var entityType2 = new EdmEntityType("NS", "EntityType2");
            var entityType3 = new EdmEntityType("NS", "EntityType3", entityType2);
            var complexType1 = new EdmComplexType("NS", "ComplexType1");
            var complexType2 = new EdmComplexType("NS", "ComplexType2");
            model.AddElements(new IEdmSchemaElement[] { entityType1, entityType2, entityType3, complexType1, complexType2 });
            entityType1.AddKeys(entityType1.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            entityType2.AddKeys(entityType2.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var outerNav1A = entityType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OuterNavA",
                Target = entityType2,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var outerNav2A = entityType2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OuterNavA",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.One
            });
            entityType1.SetNavigationPropertyPartner(
                outerNav1A, new EdmPathExpression("OuterNavA"), outerNav2A, new EdmPathExpression("OuterNavA"));
            entityType1.AddStructuralProperty(
                "ComplexProp",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmComplexTypeReference(complexType1, false))));
            entityType2.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType2, false));
            var innerNav1 = complexType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "InnerNav",
                Target = entityType2,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var innerNav2 = complexType2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "InnerNav",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var outerNav2B = entityType2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OuterNavB",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.One
            });
            entityType2.SetNavigationPropertyPartner(
                outerNav2B, new EdmPathExpression("OuterNavB"), innerNav1, new EdmPathExpression("ComplexProp/InnerNav"));
            var outerNav2C = entityType3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OuterNavC",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            var outerNav1B = entityType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OuterNavB",
                Target = entityType2,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            entityType1.SetNavigationPropertyPartner(
                outerNav1B, new EdmPathExpression("OuterNavB"), outerNav2C, new EdmPathExpression("NS.EntityType3/OuterNavC"));

            // Act & Assert for XML
            var str = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                        "<edmx:DataServices>" +
                            "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"EntityType1\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"Collection(NS.ComplexType1)\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OuterNavA\" Type=\"NS.EntityType2\" Nullable=\"false\" Partner=\"OuterNavA\" />" +
                                    "<NavigationProperty Name=\"OuterNavB\" Type=\"Collection(NS.EntityType2)\" Partner=\"NS.EntityType3/OuterNavC\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"EntityType2\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"NS.ComplexType2\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OuterNavA\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"OuterNavA\" />" +
                                    "<NavigationProperty Name=\"OuterNavB\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"ComplexProp/InnerNav\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"EntityType3\" BaseType=\"NS.EntityType2\">" +
                                    "<NavigationProperty Name=\"OuterNavC\" Type=\"Collection(NS.EntityType1)\" Partner=\"OuterNavB\" />" +
                                "</EntityType>" +
                                "<ComplexType Name=\"ComplexType1\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType2\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                                "<ComplexType Name=\"ComplexType2\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType1\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                            "</Schema>" +
                        "</edmx:DataServices>" +
                    "</edmx:Edmx>",
                str);

            // Act & Assert for JSON
            str = GetJsonCsdl(model);
            Assert.Equal(@"{
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
}", str);
        }

        [Fact]
        public void SetNavigationPropertyPartnerTypeHierarchyTest()
        {
            // Arrange
            var model = new EdmModel();
            var entityTypeA1 = new EdmEntityType("NS", "EntityTypeA1");
            var entityTypeA2 = new EdmEntityType("NS", "EntityTypeA2", entityTypeA1);
            var entityTypeA3 = new EdmEntityType("NS", "EntityTypeA3", entityTypeA2);
            var entityTypeB = new EdmEntityType("NS", "EntityTypeB");
            model.AddElements(new IEdmSchemaElement[] { entityTypeA1, entityTypeA2, entityTypeA3, entityTypeB });
            entityTypeA1.AddKeys(entityTypeA1.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var a1Nav = entityTypeA1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "A1Nav",
                Target = entityTypeB,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var a3Nav = entityTypeA3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "A3Nav",
                Target = entityTypeB,
                TargetMultiplicity = EdmMultiplicity.One
            });
            entityTypeB.AddKeys(entityTypeB.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var bNav1 = entityTypeB.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "BNav1",
                Target = entityTypeA2,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var bNav2 = entityTypeB.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "BNav2",
                Target = entityTypeA3,
                TargetMultiplicity = EdmMultiplicity.One
            });
            entityTypeA2.SetNavigationPropertyPartner(a1Nav, new EdmPathExpression("A1Nav"), bNav1, new EdmPathExpression("BNav1"));
            entityTypeA2.SetNavigationPropertyPartner(a3Nav, new EdmPathExpression("NS.EntityTypeA3/A3Nav"), bNav2, new EdmPathExpression("BNav2"));

            // Act & Assert for XML
            var str = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                    "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                            "<EntityType Name=\"EntityTypeA1\">" +
                                "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                "<NavigationProperty Name=\"A1Nav\" Type=\"NS.EntityTypeB\" Nullable=\"false\" Partner=\"BNav1\" />" +
                            "</EntityType>" +
                            "<EntityType Name=\"EntityTypeA2\" BaseType=\"NS.EntityTypeA1\" />" +
                            "<EntityType Name=\"EntityTypeA3\" BaseType=\"NS.EntityTypeA2\">" +
                                "<NavigationProperty Name=\"A3Nav\" Type=\"NS.EntityTypeB\" Nullable=\"false\" Partner=\"BNav2\" />" +
                            "</EntityType>" +
                            "<EntityType Name=\"EntityTypeB\">" +
                                "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                "<NavigationProperty Name=\"BNav1\" Type=\"NS.EntityTypeA2\" Nullable=\"false\" Partner=\"A1Nav\" />" +
                                "<NavigationProperty Name=\"BNav2\" Type=\"NS.EntityTypeA3\" Nullable=\"false\" Partner=\"NS.EntityTypeA3/A3Nav\" />" +
                            "</EntityType>" +
                        "</Schema>" +
                    "</edmx:DataServices>" +
                "</edmx:Edmx>",
                str);

            // Act & Assert for JSON
            str = GetJsonCsdl(model);
            Assert.Equal(@"{
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
}", str);
        }

        public static void SetComputedAnnotation(EdmModel model, IEdmProperty target)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(target, "target");

            IEdmBooleanConstantExpression val = new EdmBooleanConstant(true);
            IEdmTerm term = CoreVocabularyModel.ComputedTerm;

            Debug.Assert(term != null, "term!=null");
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(target, term, val);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);
        }

        #endregion

        #region Optional Parameters

        [Fact]
        public void ShouldWriteInLineOptionalParameters()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"test\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"TestFunction\">" +
                    "<Parameter Name=\"requiredParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<Parameter Name=\"optionalParam\" Type=\"Edm.String\" Nullable=\"false\">" +
                        "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" />" +
                    "</Parameter>" +
                    "<Parameter Name=\"optionalParamWithDefault\" Type=\"Edm.String\" Nullable=\"false\">" +
                        "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                          "<Record>" +
                            "<PropertyValue Property=\"DefaultValue\" String=\"Smith\" />" +
                          "</Record>" +
                        "</Annotation>" +
                    "</Parameter>" +
                    "<ReturnType Type=\"Edm.String\" Nullable=\"false\" />" +
                  "</Function>" +
                  "<EntityContainer Name=\"Default\">" +
                    "<FunctionImport Name=\"TestFunction\" Function=\"test.TestFunction\" />" +
                  "</EntityContainer>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            var stringTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var model = new EdmModel();
            var function = new EdmFunction("test", "TestFunction", stringTypeReference);
            var requiredParam = new EdmOperationParameter(function, "requiredParam", stringTypeReference);
            var optionalParam = new EdmOptionalParameter(function, "optionalParam", stringTypeReference, null);
            var optionalParamWithDefault = new EdmOptionalParameter(function, "optionalParamWithDefault", stringTypeReference, "Smith");
            function.AddParameter(requiredParam);
            function.AddParameter(optionalParam);
            function.AddParameter(optionalParamWithDefault);
            model.AddElement(function);
            model.AddEntityContainer("test", "Default").AddFunctionImport("TestFunction", function);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""test.Default"",
  ""test"": {
    ""TestFunction"": {
      ""$Kind"": ""Function"",
      ""$Parameter"": [
        {
          ""$Name"": ""requiredParam""
        },
        {
          ""$Name"": ""optionalParam"",
          ""@Org.OData.Core.V1.OptionalParameter"": { }
        },
        {
          ""$Name"": ""optionalParamWithDefault"",
          ""@Org.OData.Core.V1.OptionalParameter"": {
            ""DefaultValue"": ""Smith""
          }
        }
      ],
      ""$ReturnType"": { }
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""TestFunction"": {
        ""$Kind"": ""FunctionImport"",
        ""$Function"": ""test.TestFunction""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void ShouldWriteOutofLineOptionalParameters()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"TestFunction\">" +
                    "<Parameter Name=\"requiredParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<Parameter Name=\"optionalParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<Parameter Name=\"optionalParamWithDefault\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<ReturnType Type=\"Edm.String\" Nullable=\"false\" />" +
                  "</Function>" +
                  "<Annotations Target=\"NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParam\">" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                     "<Record />" +
                  "</Annotation>" +
                 "</Annotations>" +
                 "<Annotations Target=\"NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParamWithDefault\">" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                     "<Record Type=\"Org.OData.Core.V1.OptionalParameterType\">" +
                       "<PropertyValue Property=\"DefaultValue\" String=\"Smith\" />" +
                     "</Record>" +
                  "</Annotation>" +
                 "</Annotations>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            var stringTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var model = new EdmModel();
            var function = new EdmFunction("NS", "TestFunction", stringTypeReference);
            var requiredParam = new EdmOperationParameter(function, "requiredParam", stringTypeReference);
            var optionalParam = new EdmOptionalParameter(function, "optionalParam", stringTypeReference, null);
            var optionalParamWithDefault = new EdmOptionalParameter(function, "optionalParamWithDefault", stringTypeReference, "Smith");
            function.AddParameter(requiredParam);
            function.AddParameter(optionalParam);
            function.AddParameter(optionalParamWithDefault);
            model.AddElement(function);

            // parameter without default value
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(optionalParam, CoreVocabularyModel.OptionalParameterTerm, new EdmRecordExpression());
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            // parameter with default value
            IEdmComplexType optionalParameterType = CoreVocabularyModel.Instance.FindDeclaredType("Org.OData.Core.V1.OptionalParameterType") as IEdmComplexType;
            Assert.NotNull(optionalParameterType);

            IEdmRecordExpression optionalParameterRecord = new EdmRecordExpression(
                    new EdmComplexTypeReference(optionalParameterType, false),
                    new EdmPropertyConstructor("DefaultValue", new EdmStringConstant("Smith")));
            annotation = new EdmVocabularyAnnotation(optionalParamWithDefault, CoreVocabularyModel.OptionalParameterTerm, optionalParameterRecord);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": {
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
      ""$ReturnType"": { }
    },
    ""$Annotations"": {
      ""NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParam"": {
        ""@Org.OData.Core.V1.OptionalParameter"": { }
      },
      ""NS.TestFunction(Edm.String, Edm.String, Edm.String)/optionalParamWithDefault"": {
        ""@Org.OData.Core.V1.OptionalParameter"": {
          ""$Type"": ""Org.OData.Core.V1.OptionalParameterType"",
          ""DefaultValue"": ""Smith""
        }
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void ShouldWriteOutOfLineOptionalParametersOverwriteInLineOptionalParameter()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"TestFunction\">" +
                    "<Parameter Name=\"optionalParamWithDefault\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<ReturnType Type=\"Edm.String\" Nullable=\"false\" />" +
                  "</Function>" +
                  "<Annotations Target=\"NS.TestFunction(Edm.String)/optionalParamWithDefault\">" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                     "<Record Type=\"Org.OData.Core.V1.OptionalParameterType\">" +
                       "<PropertyValue Property=\"DefaultValue\" String=\"OutofLineValue\" />" +
                     "</Record>" +
                  "</Annotation>" +
                 "</Annotations>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            var stringTypeReference = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false);
            var model = new EdmModel();
            var function = new EdmFunction("NS", "TestFunction", stringTypeReference);
            var optionalParamWithDefault = new EdmOptionalParameter(function, "optionalParamWithDefault", stringTypeReference, "InlineDefaultValue");
            function.AddParameter(optionalParamWithDefault);
            model.AddElement(function);

            // parameter with default value
            IEdmComplexType optionalParameterType = CoreVocabularyModel.Instance.FindDeclaredType("Org.OData.Core.V1.OptionalParameterType") as IEdmComplexType;
            Assert.NotNull(optionalParameterType);

            IEdmRecordExpression optionalParameterRecord = new EdmRecordExpression(
                    new EdmComplexTypeReference(optionalParameterType, false),
                    new EdmPropertyConstructor("DefaultValue", new EdmStringConstant("OutofLineValue")));
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(optionalParamWithDefault, CoreVocabularyModel.OptionalParameterTerm, optionalParameterRecord);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": {
      ""$Kind"": ""Function"",
      ""$Parameter"": [
        {
          ""$Name"": ""optionalParamWithDefault""
        }
      ],
      ""$ReturnType"": { }
    },
    ""$Annotations"": {
      ""NS.TestFunction(Edm.String)/optionalParamWithDefault"": {
        ""@Org.OData.Core.V1.OptionalParameter"": {
          ""$Type"": ""Org.OData.Core.V1.OptionalParameterType"",
          ""DefaultValue"": ""OutofLineValue""
        }
      }
    }
  }
}", csdlStr);
        }

        #endregion

        [Fact]
        public void ShouldWriteInLineReturnTypeAnnotation()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"TestFunction\">" +
                    "<ReturnType Type=\"Edm.PrimitiveType\" Nullable=\"false\">" +
                        "<Annotation Term=\"Org.OData.Validation.V1.DerivedTypeConstraint\">" +
                          "<Collection>" +
                            "<String>Edm.Int32</String>" +
                            "<String>Edm.Boolean</String>" +
                          "</Collection>" +
                        "</Annotation>" +
                    "</ReturnType>" +
                  "</Function>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            // Act & Assert for XML
            Assert.Equal(expected, WriteReturnTypeAnnotation(EdmVocabularyAnnotationSerializationLocation.Inline));

            // Act & Assert for JSON
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": {
      ""$Kind"": ""Function"",
      ""$ReturnType"": {
        ""$Type"": ""Edm.PrimitiveType"",
        ""@Org.OData.Validation.V1.DerivedTypeConstraint"": [
          ""Edm.Int32"",
          ""Edm.Boolean""
        ]
      }
    }
  }
}", WriteReturnTypeAnnotation(EdmVocabularyAnnotationSerializationLocation.Inline, xml: false));
        }

        [Fact]
        public void ShouldWriteOutofLineReturnTypeAnnotation()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"TestFunction\">" +
                    "<ReturnType Type=\"Edm.PrimitiveType\" Nullable=\"false\" />" +
                  "</Function>" +
                  "<Annotations Target=\"NS.TestFunction()/$ReturnType\">" +
                    "<Annotation Term=\"Org.OData.Validation.V1.DerivedTypeConstraint\">" +
                      "<Collection>" +
                        "<String>Edm.Int32</String>" +
                        "<String>Edm.Boolean</String>" +
                      "</Collection>" +
                    "</Annotation>" +
                  "</Annotations>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            // Act & Assert for XML
            Assert.Equal(expected, WriteReturnTypeAnnotation(EdmVocabularyAnnotationSerializationLocation.OutOfLine));

            // Act & Assert for JSON
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""TestFunction"": {
      ""$Kind"": ""Function"",
      ""$ReturnType"": {
        ""$Type"": ""Edm.PrimitiveType"",
        ""@Org.OData.Validation.V1.DerivedTypeConstraint"": [
          ""Edm.Int32"",
          ""Edm.Boolean""
        ]
      }
    }
  }
}", WriteReturnTypeAnnotation(EdmVocabularyAnnotationSerializationLocation.Inline, xml: false));
        }

        private string WriteReturnTypeAnnotation(EdmVocabularyAnnotationSerializationLocation location, bool xml = true)
        {
            var primitiveTypeRef = EdmCoreModel.Instance.GetPrimitiveType(false);
            var model = new EdmModel();
            var termType = model.FindTerm("Org.OData.Validation.V1.DerivedTypeConstraint");
            Assert.NotNull(termType);

            var function = new EdmFunction("NS", "TestFunction", primitiveTypeRef);
            model.AddElement(function);

            IEdmCollectionExpression collectionExpression = new EdmCollectionExpression(new EdmStringConstant("Edm.Int32"), new EdmStringConstant("Edm.Boolean"));
            IEdmOperationReturn returnType = function.GetReturn();
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(returnType, termType, collectionExpression);
            annotation.SetSerializationLocation(model, location);
            model.SetVocabularyAnnotation(annotation);

            return xml ? GetCsdl(model, CsdlTarget.OData) : GetJsonCsdl(model);
        }

        [Fact]
        public void ShouldWriteEdmComplexTypeProperty()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<Property Name=\"ComplexProperty\" Type=\"Edm.ComplexType\" />" +
                  "</EntityType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("ComplexProperty", EdmCoreModel.Instance.GetComplexType(true));
            model.AddElement(customer);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""ComplexProperty"": {
        ""$Type"": ""Edm.ComplexType"",
        ""$Nullable"": true
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void ShouldWriteEdmEntityTypeProperty()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<NavigationProperty Name=\"EntityNavigationProperty\" Type=\"Edm.EntityType\" />" +
                  "</EntityType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = EdmCoreModel.Instance.GetEntityType(true).EntityDefinition(),
                ContainsTarget = false,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                Name = "EntityNavigationProperty"
            });
            model.AddElement(customer);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""EntityNavigationProperty"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Type"": ""Edm.EntityType""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void ShouldWriteEdmPathTypeProperty()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<ComplexType Name=\"SelectType\">" +
                    "<Property Name=\"DefaultSelect\" Type=\"Collection(Edm.PropertyPath)\" />" +
                    "<Property Name=\"DefaultHidden\" Type=\"Collection(Edm.NavigationPropertyPath)\" Nullable=\"false\" />" +
                  "</ComplexType>" +
                  "<Term Name=\"MyTerm\" Type=\"NS.SelectType\" />" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("NS", "SelectType");
            complexType.AddStructuralProperty("DefaultSelect", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetPropertyPath(true))));
            complexType.AddStructuralProperty("DefaultHidden", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetNavigationPropertyPath(false))));
            model.AddElement(complexType);
            EdmTerm term = new EdmTerm("NS", "MyTerm", new EdmComplexTypeReference(complexType, true));
            model.AddElement(term);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""SelectType"": {
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
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmSingletonWithEdmEntityTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityContainer Name=\"Default\">" +
                    "<Singleton Name=\"VIP\" Type=\"Edm.EntityType\" />" +
                  "</EntityContainer>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmSingleton singleton = new EdmSingleton(container, "VIP", EdmCoreModel.Instance.GetEntityType());
            container.AddElement(singleton);
            model.AddElement(container);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(1, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""VIP"": {
        ""$Kind"": ""Singleton"",
        ""$Type"": ""Edm.EntityType""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmEntitySetWithEdmEntityTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityContainer Name=\"Default\">" +
                    "<EntitySet Name=\"Customers\" EntityType=\"Edm.EntityType\" />" +
                  "</EntityContainer>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", EdmCoreModel.Instance.GetEntityType());
            container.AddElement(entitySet);
            model.AddElement(container);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(2, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""Edm.EntityType""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmEntityTypeWithEdmPrimitiveTypeKeyButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.PrimitiveType\" Nullable=\"false\" />" +
                  "</EntityType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetPrimitiveType(false)));
            model.AddElement(customer);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(1, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.PrimitiveType""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmEntityTypeWithCollectionAbstractTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<Property Name=\"Primitive\" Type=\"Collection(Edm.PrimitiveType)\" />" +
                    "<Property Name=\"Complex\" Type=\"Collection(Edm.ComplexType)\" />" +
                  "</EntityType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Primitive",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetPrimitiveType(true))));
            customer.AddStructuralProperty("Complex",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetComplexType(true))));
            model.AddElement(customer);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(2, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Primitive"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PrimitiveType"",
        ""$Nullable"": true
      },
      ""Complex"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.ComplexType"",
        ""$Nullable"": true
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmStructuredTypeWithAbstractBaseTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\" BaseType=\"Edm.EntityType\" />" +
                  "<ComplexType Name=\"Address\" BaseType=\"Edm.ComplexType\" />" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer", EdmCoreModel.Instance.GetEntityType());
            model.AddElement(customer);
            EdmComplexType address = new EdmComplexType("NS", "Address", EdmCoreModel.Instance.GetComplexType());
            model.AddElement(address);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(2, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""Edm.EntityType""
    },
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""$BaseType"": ""Edm.ComplexType""
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmTypeDefinitionWithEdmPrimitiveTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<TypeDefinition Name=\"MyType\" UnderlyingType=\"Edm.PrimitiveType\" />" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmTypeDefinition definition = new EdmTypeDefinition("NS", "MyType", EdmPrimitiveTypeKind.PrimitiveType);
            model.AddElement(definition);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(1, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""MyType"": {
      ""$Kind"": ""TypeDefinition"",
      ""$UnderlyingType"": ""Edm.PrimitiveType""
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteEdmFunctioneWithCollectionAbstractTypeButValidationFailed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<Function Name=\"GetCustomer\">" +
                    "<ReturnType Type=\"Collection(Edm.PrimitiveType)\" />" +
                  "</Function>" +
                  "<Function Name=\"GetSomething\">" +
                    "<ReturnType Type=\"Collection(Edm.ComplexType)\" Nullable=\"false\" />" +
                  "</Function>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmFunction function = new EdmFunction("NS", "GetCustomer", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetPrimitiveType(true))));
            model.AddElement(function);
            function = new EdmFunction("NS", "GetSomething", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetComplexType(false))));
            model.AddElement(function);
            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.Equal(2, errors.Count());

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""GetCustomer"": {
      ""$Kind"": ""Function"",
      ""$ReturnType"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.PrimitiveType"",
        ""$Nullable"": true
      }
    },
    ""GetSomething"": {
      ""$Kind"": ""Function"",
      ""$ReturnType"": {
        ""$Collection"": true,
        ""$Type"": ""Edm.ComplexType""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void ShouldWriteAnnotationForEnumMember()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EnumType Name=\"Appliance\" UnderlyingType=\"Edm.Int64\" IsFlags=\"true\">" +
                    "<Member Name=\"Stove\" Value=\"1\">" +
                      "<Annotation Term=\"Org.OData.Core.V1.LongDescription\" String=\"Stove Inline LongDescription\" />" +
                    "</Member>" +
                    "<Member Name=\"Washer\" Value=\"2\">" +
                      "<Annotation Term=\"NS.MyTerm\" String=\"Washer Inline MyTerm Value\" />" +
                    "</Member>" +
                  "</EnumType>" +
                  "<Term Name=\"MyTerm\" Type=\"Edm.String\" />" +
                  "<Annotations Target=\"NS.Appliance/Stove\">" +
                    "<Annotation Term=\"NS.MyTerm\" String=\"Stove OutOfLine MyTerm Value\" />" +
                  "</Annotations>" +
                  "<Annotations Target=\"NS.Appliance/Washer\">" +
                    "<Annotation Term=\"Org.OData.Core.V1.LongDescription\" String=\"Washer OutOfLine LongDescription\" />" +
                  "</Annotations>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEnumType appliance = new EdmEnumType("NS", "Appliance", EdmPrimitiveTypeKind.Int64, isFlags: true);
            model.AddElement(appliance);

            var stove = new EdmEnumMember(appliance, "Stove", new EdmEnumMemberValue(1));
            appliance.AddMember(stove);

            var washer = new EdmEnumMember(appliance, "Washer", new EdmEnumMemberValue(2));
            appliance.AddMember(washer);

            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(stove, CoreVocabularyModel.LongDescriptionTerm, new EdmStringConstant("Stove Inline LongDescription"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(washer, CoreVocabularyModel.LongDescriptionTerm, new EdmStringConstant("Washer OutOfLine LongDescription"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            annotation = new EdmVocabularyAnnotation(stove, term, new EdmStringConstant("Stove OutOfLine MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(washer, term, new EdmStringConstant("Washer Inline MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Appliance"": {
      ""$Kind"": ""EnumType"",
      ""$UnderlyingType"": ""Edm.Int64"",
      ""$IsFlags"": true,
      ""Stove"": 1,
      ""Stove@Org.OData.Core.V1.LongDescription"": ""Stove Inline LongDescription"",
      ""Washer"": 2,
      ""Washer@NS.MyTerm"": ""Washer Inline MyTerm Value""
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""$Annotations"": {
      ""NS.Appliance/Stove"": {
        ""Stove@NS.MyTerm"": ""Stove OutOfLine MyTerm Value""
      },
      ""NS.Appliance/Washer"": {
        ""Washer@Org.OData.Core.V1.LongDescription"": ""Washer OutOfLine LongDescription""
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWritePropertyWithCoreTypeDefinitionAndValidationPassed()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<ComplexType Name=\"Complex\">" +
                    "<Property Name=\"ModifiedDate\" Type=\"Org.OData.Core.V1.LocalDateTime\" />" +
                    "<Property Name=\"QualifiedName\" Type=\"Org.OData.Core.V1.QualifiedTypeName\" />" +
                  "</ComplexType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            var localDateTime = model.FindType("Org.OData.Core.V1.LocalDateTime") as IEdmTypeDefinition;
            Assert.NotNull(localDateTime);

            var qualifiedTypeName = model.FindType("Org.OData.Core.V1.QualifiedTypeName") as IEdmTypeDefinition;
            Assert.NotNull(qualifiedTypeName);

            EdmComplexType type = new EdmComplexType("NS", "Complex");
            type.AddStructuralProperty("ModifiedDate", new EdmTypeDefinitionReference(localDateTime, true));
            type.AddStructuralProperty("QualifiedName", new EdmTypeDefinitionReference(qualifiedTypeName, true));

            model.AddElement(type);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""ModifiedDate"": {
        ""$Type"": ""Org.OData.Core.V1.LocalDateTime"",
        ""$Nullable"": true
      },
      ""QualifiedName"": {
        ""$Type"": ""Org.OData.Core.V1.QualifiedTypeName"",
        ""$Nullable"": true
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteNavigationPropertyBindingWithTargetPathOnContainmentOnSingleton()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Customer\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" />" +
                    "<NavigationProperty Name=\"ContainedOrders\" Type=\"Collection(NS.Order)\" ContainsTarget=\"true\" />" +
                    "<NavigationProperty Name=\"ContainedOrderLines\" Type=\"Collection(NS.OrderLine)\" ContainsTarget=\"true\" />" +
                  "</EntityType>" +
                  "<EntityType Name=\"Order\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" />" +
                    "<NavigationProperty Name=\"OrderLines\" Type=\"Collection(NS.OrderLine)\" />" +
                  "</EntityType>" +
                  "<EntityType Name=\"OrderLine\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" />" +
                  "</EntityType>" +
                  "<EntityContainer Name=\"Default\">" +
                     "<Singleton Name=\"Me\" Type=\"NS.Customer\" />" +
                     "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\">" +
                       "<NavigationPropertyBinding Path=\"ContainedOrders/OrderLines\" Target=\"Me/ContainedOrderLines\" />" +
                     "</EntitySet>" +
                  "</EntityContainer>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            EdmEntityType order = new EdmEntityType("NS", "Order");
            order.AddKeys(order.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            EdmEntityType orderLine = new EdmEntityType("NS", "OrderLine");
            orderLine.AddKeys(orderLine.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            // Customer
            //        -> ContainedOrders (Contained)
            //        -> ContainedOrderLines (Contained)
            customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "ContainedOrders",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = order,
                ContainsTarget = true
            });

            var orderLinesContainedNav = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "ContainedOrderLines",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = orderLine,
                ContainsTarget = true
            });

            // Order
            //    -> OrderLines
            var orderLinesNav = order.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OrderLines",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = orderLine
            });

            model.AddElement(customer);
            model.AddElement(order);
            model.AddElement(orderLine);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmSingleton me = new EdmSingleton(container, "Me", customer);
            container.AddElement(me);
            EdmEntitySet customers = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(customers);
            model.AddElement(container);

            // Navigation property binding to the containment of the singleton
            EdmContainedEntitySet containedEntitySet = new EdmContainedEntitySet(me, orderLinesContainedNav);
            customers.AddNavigationTarget(orderLinesNav, containedEntitySet, new EdmPathExpression("ContainedOrders/OrderLines"));

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            csdlStr = GetJsonCsdl(model);
            Assert.Equal(@"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""ContainedOrders"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.Order"",
        ""$ContainsTarget"": true
      },
      ""ContainedOrderLines"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.OrderLine"",
        ""$ContainsTarget"": true
      }
    },
    ""Order"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      },
      ""OrderLines"": {
        ""$Kind"": ""NavigationProperty"",
        ""$Collection"": true,
        ""$Type"": ""NS.OrderLine""
      }
    },
    ""OrderLine"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32"",
        ""$Nullable"": true
      }
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Me"": {
        ""$Kind"": ""Singleton"",
        ""$Type"": ""NS.Customer""
      },
      ""Customers"": {
        ""$Kind"": ""EntitySet"",
        ""$Type"": ""NS.Customer"",
        ""$NavigationPropertyBinding"": {
          ""ContainedOrders/OrderLines"": ""Me/ContainedOrderLines""
        }
      }
    }
  }
}", csdlStr);
        }

        [Fact]
        public void CanWriteUrlEscapeFunction()
        {
            // Arrange
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<EntityType Name=\"Entity\">" +
                    "<Key>" +
                      "<PropertyRef Name=\"Id\" />" +
                    "</Key>" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                  "</EntityType>" +
                  "<Function Name=\"Function\" IsBound=\"true\">" +
                    "<Parameter Name=\"entity\" Type=\"NS.Entity\" />" +
                    "<Parameter Name=\"path\" Type=\"Edm.String\" />" +
                    "<ReturnType Type=\"Edm.Int32\" />" +
                    "<Annotation Term=\"Org.OData.Community.V1.UrlEscapeFunction\" Bool=\"true\" />" +
                  "</Function>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            EdmFunction function = new EdmFunction("NS", "Function", EdmCoreModel.Instance.GetInt32(true), true, null, false);
            function.AddParameter("entity", new EdmEntityTypeReference(entityType, true));
            function.AddParameter("path", EdmCoreModel.Instance.GetString(true));
            model.AddElement(entityType);
            model.AddElement(function);
            model.SetUrlEscapeFunction(function);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            // Act & Assert for XML
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);

            // Act & Assert for JSON
            string jsonStr = GetJsonCsdl(model);
            Assert.Equal(@"{
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
    ""Function"": {
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
      },
      ""@Org.OData.Community.V1.UrlEscapeFunction"": true
    }
  }
}", jsonStr);
        }

        [Fact]
        public void CanWriteAnnotationPathExpression()
        {
            string expected =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<ComplexType Name=\"Complex\">" +
                    "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                    "<Annotation Term=\"NS.MyNavigationPathTerm\" NavigationPropertyPath=\"123/456.t\" />" +
                  "</ComplexType>" +
                  "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                  "<Term Name=\"MyNavigationPathTerm\" Type=\"Edm.NavigationPropertyPath\" Nullable=\"false\" />" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            EdmModel model = new EdmModel();
            EdmComplexType complex = new EdmComplexType("NS", "Complex");
            model.AddElement(complex);
            EdmTerm term1 = new EdmTerm("NS", "MyAnnotationPathTerm", EdmCoreModel.Instance.GetAnnotationPath(false));
            EdmTerm term2 = new EdmTerm("NS", "MyNavigationPathTerm", EdmCoreModel.Instance.GetNavigationPropertyPath(false));
            model.AddElement(term1);
            model.AddElement(term2);

            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complex, term1, new EdmAnnotationPathExpression("abc/efg"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(complex, term2, new EdmNavigationPropertyPathExpression("123/456.t"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);
        }

        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ValidateEdmxVersions(string odataVersion)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"" + odataVersion + "\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\"><edmx:DataServices /></edmx:Edmx>";

            // Specify the model
            EdmModel edmModel = new EdmModel(false);
            edmModel.SetEdmVersion(odataVersion == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401);

            // Validate the CSDL for the specified version
            Assert.Equal(GetCsdl(edmModel, CsdlTarget.OData), xml);
        }

        private string GetCsdl(IEdmModel model, CsdlTarget target)
        {
            string edmx = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = System.Text.Encoding.UTF8;

                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                {
                    IEnumerable<EdmError> errors;
                    CsdlWriter.TryWriteCsdl(model, xw, target, out errors);
                    xw.Flush();
                }

                edmx = sw.ToString();
            }

            return edmx;
        }

        private string GetJsonCsdl(IEdmModel model, bool indent = true, bool isIeee754Compatible = false)
        {
            string edmx = string.Empty;

            CsdlJsonWriterSettings settings = CsdlJsonWriterSettings.Default;
            settings.Indent = indent;
            settings.IsIeee754Compatible = isIeee754Compatible;

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                IEnumerable<EdmError> errors;
                bool ok = CsdlWriter.TryWriteJson(model, writer, settings, out errors);
                Assert.True(ok);

                stream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}