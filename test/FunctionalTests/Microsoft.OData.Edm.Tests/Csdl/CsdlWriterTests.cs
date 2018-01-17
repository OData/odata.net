//---------------------------------------------------------------------
// <copyright file="CsdlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlWriterTests
    {
        #region Annotation - Computed, OptimisticConcurrency

        [Fact]
        public void VerifyAnnotationComputedConcurrency()
        {
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

            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx""><edmx:DataServices><Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""><EntityType Name=""Product""><Key><PropertyRef Name=""Id"" /></Key><Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""><Annotation Term=""Org.OData.Core.V1.Computed"" Bool=""true"" /></Property><Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" /><Property Name=""UpdatedTime"" Type=""Edm.Date"" Nullable=""false""><Annotation Term=""Org.OData.Core.V1.Computed"" Bool=""true"" /></Property></EntityType><EntityContainer Name=""Container""><EntitySet Name=""Products"" EntityType=""NS1.Product""><Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency""><Collection><PropertyPath>Id</PropertyPath><PropertyPath>UpdatedTime</PropertyPath></Collection></Annotation></EntitySet></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>", csdlStr);
        }

        [Fact]
        public void WriteNavigationPropertyInComplexType()
        {
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
                    "<NavigationPropertyBinding Path=\"HomeAddress/City\" Target=\"City\" />" +
                    "<NavigationPropertyBinding Path=\"Addresses/City\" Target=\"City\" />" +
                    "<NavigationPropertyBinding Path=\"WorkAddress/DefaultNs.WorkAddress/CountryOrRegion\" Target=\"CountryOrRegion\" />" +
                "</EntitySet>" +
                "<EntitySet Name=\"City\" EntityType=\"DefaultNs.City\" />" +
                "<EntitySet Name=\"CountryOrRegion\" EntityType=\"DefaultNs.CountryOrRegion\" />" +
                "</EntityContainer></Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteCollectionOfNavigationOnComplex()
        {
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
        }

        [Fact]
        public void ContainedUnderComplexTest()
        {
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

            string actual = GetCsdl(model, CsdlTarget.OData);

            var entitySet1 = model.EntityContainer.FindEntitySet("Entities1");
            var entitySet2 = model.EntityContainer.FindEntitySet("Entities2");
            var containedEntitySet = entitySet1.FindNavigationTarget(containedUnderComplex);
            Assert.Equal(containedEntitySet.Name, "ContainedUnderComplex");
            var entitySetUnderContained = containedEntitySet.FindNavigationTarget(navUnderContained,
                new EdmPathExpression("Complex/ContainedUnderComplex/NavUnderContained"));
            Assert.Equal(entitySetUnderContained, entitySet2);
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
        }

        [Fact]
        public void SetNavigationPropertyPartnerTypeHierarchyTest()
        {
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
        public void ShouldWriteOptionalParameters()
        {
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
            string csdlStr = GetCsdl(model, CsdlTarget.OData);
            Assert.Equal(expected, csdlStr);
        }

        #endregion

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
    }
}