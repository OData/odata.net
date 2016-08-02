//---------------------------------------------------------------------
// <copyright file="CsdlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public void SetNavigationPropertyPartnerTest()
        {
            // build model
            var model = new EdmModel();
            var entityType1 = new EdmEntityType("NS", "EntityType1");
            var entityType2 = new EdmEntityType("NS", "EntityType2");
            var complexType1 = new EdmComplexType("NS", "ComplexType1");
            var complexType2 = new EdmComplexType("NS", "ComplexType2");
            model.AddElements(new IEdmSchemaElement[] { entityType1, entityType2, complexType1, complexType2 });
            entityType1.AddKeys(entityType1.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            entityType2.AddKeys(entityType2.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            var outterNav1 = entityType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OutterNav",
                Target = entityType2,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var outterNav2 = entityType2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OutterNav",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.One
            });
            var outterNav3 = entityType2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "OutterNav2",
                Target = entityType1,
                TargetMultiplicity = EdmMultiplicity.One
            });
            entityType1.SetNavigationPropertyPartner(outterNav1, "OutterNav", outterNav2, "OutterNav");
            entityType1.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType1, false));
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
            entityType2.SetNavigationPropertyPartner(outterNav3, "OutterNav2", innerNav1, "ComplexProp/InnerNav");

            Assert.Equal(
                GetCsdl(model, CsdlTarget.OData),
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                        "<edmx:DataServices>" +
                            "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"EntityType1\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"NS.ComplexType1\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OutterNav\" Type=\"NS.EntityType2\" Nullable=\"false\" Partner=\"OutterNav\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"EntityType2\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"NS.ComplexType2\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OutterNav\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"OutterNav\" />" +
                                    "<NavigationProperty Name=\"OutterNav2\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"ComplexProp/InnerNav\" />" +
                                "</EntityType>" +
                                "<ComplexType Name=\"ComplexType1\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType2\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                                "<ComplexType Name=\"ComplexType2\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType1\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                            "</Schema>" +
                        "</edmx:DataServices>" +
                    "</edmx:Edmx>");
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