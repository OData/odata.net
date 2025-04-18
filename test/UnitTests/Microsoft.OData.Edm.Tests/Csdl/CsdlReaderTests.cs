﻿//---------------------------------------------------------------------
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
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.SRResources;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlReaderTests
    {
        private const string ValidEdmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        private const string InvalidXml = "<fake/>";
        private const string ErrorMessage = "UnexpectedXmlElement : The element 'fake' was unexpected for the root element. The root element should be Edmx. : (0, 0)";

        private XmlReader validReader;
        private XmlReader invalidReader;

        public CsdlReaderTests()
        {
            this.validReader = XElement.Parse(ValidEdmx).CreateReader();
            this.invalidReader = XElement.Parse(InvalidXml).CreateReader();
        }

        [Fact]
        public void ShouldReadEdmReference()
        {
            // Act & Assert for XML
            string csdl = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:Reference Uri=\"https://example.com/Org.OData.Authorization.V1.xml\">" +
                "<edmx:Include Namespace=\"Org.OData.Authorization.V1\" Alias=\"Auth\" >" +
                  "<Annotation Term=\"Core.Description\" String=\"Default Include.\" />" +
                "</edmx:Include>" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.validation\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.display\" Qualifier=\"Tablet\" />" +
              "</edmx:Reference>" +
              "<edmx:Reference Uri=\"https://example.com/Org.OData.Core.V1.xml\">" +
                "<edmx:Include Namespace=\"Org.OData.Core.V1\" Alias=\"Core\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.hcm\" TargetNamespace=\"com.example.Sales\" />" +
                "<edmx:IncludeAnnotations TermNamespace=\"org.example.hcm\" Qualifier=\"Tablet\" TargetNamespace=\"com.example.Person\" />" +
                "<Annotation Term=\"Core.Description\" String=\"Default Reference.\" />" +
               "</edmx:Reference>" +
              "<edmx:DataServices />" +
            "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());

            var references = model.GetEdmReferences().ToList();
            foreach (var reference in references)
            {
                CsdlSemanticsReference edmRef = Assert.IsType<CsdlSemanticsReference>(reference);
                Assert.NotNull(edmRef);

                IEdmReference edmReference = Assert.IsAssignableFrom<IEdmReference>(reference);
                Assert.NotNull(edmReference);
            }

            Assert.Equal(2, model.VocabularyAnnotations.Count());

            IEdmReference autheRef = references.First(f => f.Uri.OriginalString.EndsWith("Org.OData.Authorization.V1.xml"));
            var annotations = model.FindVocabularyAnnotations(autheRef);
            Assert.Empty(annotations);

            // annotation on include
            IEdmInclude authInclude = Assert.Single(autheRef.Includes);
            annotations = model.FindVocabularyAnnotations(authInclude);
            IEdmVocabularyAnnotation annotation = Assert.Single(annotations);
            Assert.Equal("Org.OData.Core.V1.Description", annotation.Term.FullName());
            Assert.Same(authInclude, annotation.Target);
            Assert.Equal("Default Include.", (annotation.Value as IEdmStringConstantExpression).Value);

            // Annotation on reference
            IEdmReference coreRef = references.First(f => f.Uri.OriginalString.EndsWith("Org.OData.Core.V1.xml"));
            annotations = model.FindVocabularyAnnotations(coreRef);
            annotation = Assert.Single(annotations);
            Assert.Equal("Org.OData.Core.V1.Description", annotation.Term.FullName());
            Assert.Same(coreRef, annotation.Target);
            Assert.Equal("Default Reference.", (annotation.Value as IEdmStringConstantExpression).Value);
        }

        [Fact]
        public void ReadNavigationPropertyPartnerTest()
        {
            var csdl =
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
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
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
        public void ReadNavigationPropertyPartnerOnlyOnOneSideTest()
        {
            var csdl =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                        "<edmx:DataServices>" +
                            "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"Product\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<NavigationProperty Name=\"Category\" Type=\"NS.Category\" Nullable=\"false\" Partner=\"Products\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"Category\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<NavigationProperty Name=\"Products\" Type=\"Collection(NS.Product)\" Nullable=\"false\" />" +
                                "</EntityType>" +
                            "</Schema>" +
                        "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var product = (IEdmEntityType)model.FindDeclaredType("NS.Product");
            var category = (IEdmEntityType)model.FindDeclaredType("NS.Category");
            IEdmNavigationProperty categoryNavigationProperty = (IEdmNavigationProperty)product.FindProperty("Category");
            IEdmNavigationProperty productsNavigationProperty = (IEdmNavigationProperty)category.FindProperty("Products");

            Assert.Equal("Products", categoryNavigationProperty.GetPartnerPath().Path);
            Assert.Null(productsNavigationProperty.GetPartnerPath());

            Assert.Same(productsNavigationProperty, categoryNavigationProperty.Partner);
            Assert.Same(categoryNavigationProperty, productsNavigationProperty.Partner);
        }

        [Fact]
        public void ReadNavigationPropertyPartnerFromBigCsdlTestNoExceptionThrows()
        {
            var model = CsdlReader.Parse(XElement.Parse(BigCsdl).CreateReader());

            Action test = () =>
            {
                foreach (var entityType in model.SchemaElements.OfType<IEdmEntityType>())
                {
                    foreach (var nav in entityType.NavigationProperties())
                    {
                        IEdmNavigationProperty partner = nav.Partner;
                        Assert.Null(partner);
                    }
                }
            };

            var exception = Record.Exception(test);
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathInvalidTypeCast()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityA\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                              "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                              "<Property Name=\"Complex\" Type=\"NS.ComplexA\" Nullable=\"false\" />" +
                            "</EntityType>" +
                          "<EntityType Name=\"EntityB\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<ComplexType Name=\"ComplexA\" />" +
                          "<ComplexType Name=\"ComplexB\">" +
                            "<NavigationProperty Name=\"ComplexBNav\" Type=\"NS.EntityB\" Nullable=\"false\" />" +
                          "</ComplexType>" +
                          "<EntityContainer Name=\"Container\">" +
                            "<EntitySet Name=\"Set1\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"Complex/NS.ComplexB/ComplexBNav\" Target=\"Set2\" />" +
                            "</EntitySet>" +
                            "<EntitySet Name=\"Set2\" EntityType=\"NS.EntityB\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var set1 = model.FindDeclaredNavigationSource("Set1");
            Assert.True(set1.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTypeCast()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityA\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                              "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                              "<Property Name=\"Complex\" Type=\"NS.Complex\" Nullable=\"false\" />" +
                            "</EntityType>" +
                          "<ComplexType Name=\"Complex\" />" +
                          "<ComplexType Name=\"ComplexB\" BaseType=\"NS.Complex\">" +
                            "<NavigationProperty Name=\"ComplexBNav\" Type=\"NS.EntityB\" Nullable=\"false\" />" +
                          "</ComplexType>" +
                          "<EntityContainer Name=\"Container\">" +
                            "<EntitySet Name=\"Set1\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"Complex/NS.ComplexB/ComplexBNav\" Target=\"Set2\" />" +
                            "</EntitySet>" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var set1 = model.FindDeclaredNavigationSource("Set1");
            var navProp = (model.FindType("NS.ComplexB") as IEdmStructuredType).FindProperty("ComplexBNav");
            Assert.Equal(navProp, set1.NavigationPropertyBindings.First().NavigationProperty);
        }

        [InlineData("4.0")]
        [InlineData("4.01")]
        [Theory]
        public void ValidateNavigationPropertyBindingPathEndingInTypeCast(string odataVersion)
        {
            var entitySetWithNavProperties =
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"Nav/NS.EntityX1\" Target=\"SetX1\" />" +
                              "<NavigationPropertyBinding Path=\"Nav/NS.EntityX2\" Target=\"SetX2\" />" +
                            "</EntitySet>";
            var entitySetWithoutNavProperties =
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\" />";
            var csdl = 
                  "<edmx:Edmx Version=\"{0}\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityA\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                              "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                              "<NavigationProperty Name=\"Nav\" Type=\"NS.EntityX\" Nullable=\"false\" />" +
                            "</EntityType>" +
                          "<EntityType Name=\"EntityX\" Abstract=\"true\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityX1\" BaseType=\"NS.EntityX\" />" +
                          "<EntityType Name=\"EntityX2\" BaseType=\"NS.EntityX\" />" +
                          "<EntityContainer Name=\"Container\">" +
                          "{1}" +
                            "<EntitySet Name=\"SetX1\" EntityType=\"NS.EntityX1\" />" +
                            "<EntitySet Name=\"SetX2\" EntityType=\"NS.EntityX2\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(String.Format(csdl, odataVersion, entitySetWithNavProperties)).CreateReader());
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));
            Assert.Empty(errors);

            var setA = model.FindDeclaredNavigationSource("SetA");
            Assert.NotNull(setA);

            var navProp = setA.EntityType.FindProperty("Nav") as IEdmNavigationProperty;
            Assert.NotNull(navProp);
            Assert.Equal(2, setA.NavigationPropertyBindings.Count());

            // NavPropBinding for for EntityX1 should be SetX1
            var X1NavPropBinding = setA.NavigationPropertyBindings.FirstOrDefault(b => b.Path.PathSegments.Last() == "NS.EntityX1");
            Assert.NotNull(X1NavPropBinding);
            var setX1 = model.FindDeclaredNavigationSource("SetX1");
            Assert.Same(setX1, X1NavPropBinding.Target);
            Assert.Same(setX1, setA.FindNavigationTarget(navProp, new EdmPathExpression("Nav/NS.EntityX1")));

            // NavPropBinding for EntityX2 should be SetX2
            var X2NavPropBinding = setA.NavigationPropertyBindings.FirstOrDefault(b => b.Path.PathSegments.Last() == "NS.EntityX2");
            Assert.NotNull(X2NavPropBinding);
            var setX2 = model.FindDeclaredNavigationSource("SetX2");
            Assert.Same(setX2, X2NavPropBinding.Target);
            Assert.Same(setX2, setA.FindNavigationTarget(navProp, new EdmPathExpression("Nav/NS.EntityX2")));

            // Navigation Property without path should return UnknownEntitySet
            Assert.True(setA.FindNavigationTarget(navProp) is EdmUnknownEntitySet);

            // Verify writing model
            model.SetEdmVersion(Version.Parse(odataVersion));
            VerifyXmlModel(model, odataVersion == "4.0" ? entitySetWithoutNavProperties : entitySetWithNavProperties);

        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesNoNonContainmentNavigationProperties()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityBase\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityA\" BaseType=\"NS.EntityBase\">" +
                            "<NavigationProperty Name=\"EntityAToB\" Type=\"NS.EntityB\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityB\" BaseType=\"NS.EntityBase\">" +
                            "<NavigationProperty Name=\"EntityBToC\" Type=\"NS.EntityC\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityC\" BaseType=\"NS.EntityBase\" />" +
                          "<EntityContainer Name=\"Container\">" +
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"EntityAToB/EntityBToC\" Target=\"SetC\" />" +
                            "</EntitySet>" +
                            "<EntitySet Name=\"SetB\" EntityType=\"NS.EntityB\" />" +
                            "<EntitySet Name=\"SetC\" EntityType=\"NS.EntityC\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var setA = model.FindDeclaredNavigationSource("SetA");
            Assert.True(setA.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesContainmentNavigationProperties()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"RootEntity\">" +
                            "<NavigationProperty Name=\"SetA\" Type=\"Collection(NS.EntityA)\" ContainsTarget=\"true\" />" +
                            "<NavigationProperty Name=\"SetB\" Type=\"Collection(NS.EntityB)\" ContainsTarget=\"true\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityA\">" +
                            "<NavigationProperty Name=\"EntityAToB\" Type=\"Collection(NS.EntityB)\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityB\"/>" +
                          "<EntityContainer Name=\"Container\">" +
                            "<Singleton Name=\"Root\" Type=\"NS.RootEntity\">" +
                              "<NavigationPropertyBinding Path=\"EntityA/EntityAToB\" Target=\"Root/SetB\" />" +
                            "</Singleton>" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
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

        [Theory]
        [InlineData("Sites/Plants", "Sites")]
        [InlineData("NS.MyApi.Sites/Plants", "Sites")]
        [InlineData("NS.MyApi/Sites/Plants", "Sites")]
        public void ValidateNavigationPropertyBindingTargetOnContainmentNavigationProperty(string target, string setName)
        {
            var csdl = string.Format(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
   <edmx:DataServices>
      <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""NS"">
         <EntityType Name=""Area"">
            <Key>
               <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.String"" />
            <NavigationProperty Name=""Plant"" Type=""NS.Plant"" Nullable=""false"" />
         </EntityType>
         <EntityType Name=""Plant"">
            <Key>
               <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.String"" />
            <NavigationProperty Name=""Areas"" Type=""Collection(NS.Area)"" ContainsTarget=""true"" />
         </EntityType>
         <EntityType Name=""Site"">
            <Key>
               <PropertyRef Name=""Id"" />
            </Key>
            <Property Name=""Id"" Type=""Edm.String"" />
            <NavigationProperty Name=""Plants"" Type=""Collection(NS.Plant)"" ContainsTarget=""true"" />
         </EntityType>
         <EntityContainer Name=""MyApi"" Extends=""NS.OtherApi"" >
            <EntitySet Name=""Sites"" EntityType=""NS.Site"" />
            <EntitySet Name=""Areas"" EntityType=""NS.Area"">
               <NavigationPropertyBinding Target=""{0}"" Path=""Plant""/>
            </EntitySet>
         </EntityContainer>
      </Schema>
   </edmx:DataServices>
</edmx:Edmx>", target);

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var setAreas = model.FindDeclaredNavigationSource("Areas");
            var navBinding = Assert.Single(setAreas.NavigationPropertyBindings);

            // Path
            Assert.Equal("Plant", navBinding.NavigationProperty.Name);

            // Target
            EdmContainedEntitySet containedEntitySet = Assert.IsType<EdmContainedEntitySet>(navBinding.Target);
            Assert.Equal(setName, containedEntitySet.ParentNavigationSource.Name);
            Assert.Equal("Plants", containedEntitySet.NavigationProperty.Name);
        }

        [Fact]
        public void ValidateEducationModel()
        {
            var csdl
                = "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "  <edmx:DataServices>" +
                  "    <Schema Namespace=\"Test.NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "      <EntityType Name=\"educationRoot\">" +
                  "        <NavigationProperty Name=\"classes\" Type=\"Collection(Test.NS.class)\" ContainsTarget=\"true\"/>" +
                  "        <NavigationProperty Name=\"users\" Type=\"Collection(Test.NS.user)\" ContainsTarget=\"true\"/>" +
                  "      </EntityType>" +
                  "      <EntityType Name=\"user\">" +
                  "        <Key><PropertyRef Name=\"name\"/></Key>" +
                  "        <Property Name =\"name\" Type=\"Edm.String\" Nullable=\"False\"/>" +
                  "        <NavigationProperty Name=\"classes\" Type=\"Collection(Test.NS.class)\"/>" +
                  "      </EntityType>" +
                  "      <EntityType Name=\"class\">" +
                  "        <Key><PropertyRef Name=\"classNumber\"/></Key>" +
                  "        <Property Name=\"classNumber\" Type=\"Edm.String\" Nullable=\"False\"/>" +
                  "        <Property Name=\"displayName\" Type=\"Edm.String\"/>" +
                  "        <Property Name=\"description\" Type=\"Edm.String\"/>" +
                  "        <Property Name=\"period\" Type=\"Edm.String\"/>" +
                  "        <NavigationProperty Name=\"members\" Type=\"Collection(Test.NS.user)\"/>" +
                  "      </EntityType>" +
                  "      <EntityContainer Name=\"EducationService\">" +
                  "        <Singleton Name=\"education\" Type=\"Test.NS.educationRoot\">" +
                  "          <NavigationPropertyBinding Path=\"classes/members\" Target=\"education/users\" />" +
                  "        </Singleton>" +
                  "      </EntityContainer>" +
                  "    </Schema>" +
                  "  </edmx:DataServices>" +
                  "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            var result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
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
        public void ReadNavigationPropertyPartnerTypeHierarchyTest()
        {
            var csdl =
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
                "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
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
        public void ReadAnnotationWithoutSpecifiedValueAndUseDefault()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                         "<Annotation Term=\"NS.MyDefaultTerm\" />" + // does not specify value
                       "</ComplexType>" +
                       "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                     "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>";
            // Parse into CSDL

            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmTerm defaultTerm = model.FindTerm("NS.MyDefaultTerm");
            Assert.Equal("This is a test", defaultTerm.DefaultValue);
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(2, annotations.Length);

            IEdmVocabularyAnnotation annotationWithSpecifiedValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[0]);
            Assert.NotNull(annotationWithSpecifiedValue.Value);
            Assert.IsAssignableFrom<IEdmPathExpression>(annotationWithSpecifiedValue.Value);

            IEdmVocabularyAnnotation annotationWithDefaultValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[1]);
            Assert.NotNull(annotationWithDefaultValue.Value);
            IEdmStringConstantExpression stringConstantExp = Assert.IsAssignableFrom<IEdmStringConstantExpression>(annotationWithDefaultValue.Value);
            Assert.Equal("This is a test", stringConstantExp.Value);

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            CsdlWriterTests.WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                     "<Annotation Term=\"NS.MyDefaultTerm\" />" + // no longer has value
                   "</ComplexType>" +
                   "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                   "<Term Name=\"MyDefaultTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>");
        }

        [Fact]
        public void ReadWriteAnnotationUsingDefaultValueWithContinuity()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyDefaultBoolTerm1\" />" + // does not specify value
                         "<Annotation Term=\"NS.MyDefaultBoolTerm2\" Bool=\"true\" />" + // specifies default
                         "<Annotation Term=\"NS.MyDefaultBoolTerm3\" Bool=\"false\" />" + // specifies non-default
                       "</ComplexType>" +
                       "<Term Name=\"MyDefaultBoolTerm1\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm2\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm3\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                      "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>";
            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmTerm defaultTerm = model.FindTerm("NS.MyDefaultBoolTerm1");
            Assert.True(bool.Parse(defaultTerm.DefaultValue));
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(3, annotations.Length);
            IEdmVocabularyAnnotation annotation1 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[0]);
            IEdmBooleanConstantExpression annotationValue1 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation1.Value);
            Assert.True(annotationValue1.Value);

            IEdmVocabularyAnnotation annotation2 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[1]);
            IEdmBooleanConstantExpression annotationValue2 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation2.Value);
            Assert.True(annotationValue2.Value);

            IEdmVocabularyAnnotation annotation3 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[2]);
            IEdmBooleanConstantExpression annotationValue3 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation3.Value);
            Assert.False(annotationValue3.Value);

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            CsdlWriterTests.WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyDefaultBoolTerm1\" />" + // does not specify value
                         "<Annotation Term=\"NS.MyDefaultBoolTerm2\" Bool=\"true\" />" + // for back compatiable, write the default value again.
                         "<Annotation Term=\"NS.MyDefaultBoolTerm3\" Bool=\"false\" />" + // specifies non-default
                       "</ComplexType>" +
                       "<Term Name=\"MyDefaultBoolTerm1\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm2\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm3\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                     "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>");
        }

        [Fact]
        public void ReadAnnotationWithoutSpecifiedValueAndDoesntValidate()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyNonDefaultTerm\" />" + // does not specify value
                       "</ComplexType>" +
                       "<Term Name=\"MyDefaultTerm\" Type=\"Edm.String\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                     "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>";
            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.False(validated);
        }

        [Fact]
        public void ReadAnnotationWithoutSpecifiedValueAndUsePrimitiveDefaultValues()
        {
            CsdlXmlWriterSettings writerSettings = new CsdlXmlWriterSettings
            {
                LibraryCompatibility = EdmLibraryCompatibility.UseLegacyVariableCasing
            };

            var csdl = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.DefaultBinaryTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDecimalTerm\" />" +
                     "<Annotation Term=\"NS.DefaultStringTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDurationTerm\" />" +
                     "<Annotation Term=\"NS.DefaultTODTerm\" />" +
                     "<Annotation Term=\"NS.DefaultBoolTerm\" />" +
                     "<Annotation Term=\"NS.DefaultSingleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDoubleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultGuidTerm\" />" +
                     "<Annotation Term=\"NS.DefaultInt16Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt32Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt64Term\" />" +
                     "<Annotation Term=\"NS.DefaultDateTerm\" />" +
                   "</ComplexType>" +
                 "<Term Name=\"DefaultBinaryTerm\" Type=\"Edm.Binary\" DefaultValue=\"01\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDecimalTerm\" Type=\"Edm.Decimal\" DefaultValue=\"0.34\" AppliesTo=\"Property Term\" Nullable=\"false\" Scale=\"Variable\" />" +
                 "<Term Name=\"DefaultStringTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDurationTerm\" Type=\"Edm.Duration\" DefaultValue=\"P11DT23H59M59.999999999999S\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultTODTerm\" Type=\"Edm.TimeOfDay\" DefaultValue=\"21:45:00\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultBoolTerm\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultSingleTerm\" Type=\"Edm.Single\" DefaultValue=\"0.2\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDoubleTerm\" Type=\"Edm.Double\" DefaultValue=\"3.94\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultGuidTerm\" Type=\"Edm.Guid\" DefaultValue=\"21EC2020-3AEA-1069-A2DD-08002B30309D\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt16Term\" Type=\"Edm.Int16\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt32Term\" Type=\"Edm.Int32\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt64Term\" Type=\"Edm.Int64\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDateTerm\" Type=\"Edm.Date\" DefaultValue=\"2000-12-10\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>";
            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(13, annotations.Length);
            for (int i = 0; i < annotations.Length; i++)
            {
                IEdmVocabularyAnnotation annotationWithDefaultValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[i]);
                Assert.NotNull(annotationWithDefaultValue.Value);
            }

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            CsdlWriterTests.WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.DefaultBinaryTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDecimalTerm\" />" +
                     "<Annotation Term=\"NS.DefaultStringTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDurationTerm\" />" +
                     "<Annotation Term=\"NS.DefaultTODTerm\" />" +
                     "<Annotation Term=\"NS.DefaultBoolTerm\" />" +
                     "<Annotation Term=\"NS.DefaultSingleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDoubleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultGuidTerm\" />" +
                     "<Annotation Term=\"NS.DefaultInt16Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt32Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt64Term\" />" +
                     "<Annotation Term=\"NS.DefaultDateTerm\" />" +
                   "</ComplexType>" +
                 "<Term Name=\"DefaultBinaryTerm\" Type=\"Edm.Binary\" DefaultValue=\"01\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDecimalTerm\" Type=\"Edm.Decimal\" DefaultValue=\"0.34\" AppliesTo=\"Property Term\" Nullable=\"false\" Scale=\"Variable\" />" +
                 "<Term Name=\"DefaultStringTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDurationTerm\" Type=\"Edm.Duration\" DefaultValue=\"P11DT23H59M59.999999999999S\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultTODTerm\" Type=\"Edm.TimeOfDay\" DefaultValue=\"21:45:00\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultBoolTerm\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultSingleTerm\" Type=\"Edm.Single\" DefaultValue=\"0.2\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDoubleTerm\" Type=\"Edm.Double\" DefaultValue=\"3.94\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultGuidTerm\" Type=\"Edm.Guid\" DefaultValue=\"21EC2020-3AEA-1069-A2DD-08002B30309D\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt16Term\" Type=\"Edm.Int16\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt32Term\" Type=\"Edm.Int32\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt64Term\" Type=\"Edm.Int64\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDateTerm\" Type=\"Edm.Date\" DefaultValue=\"2000-12-10\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>", writerSettings);
        }

        [Fact]
        public void ParsingValidXmlWithNoReferencesShouldSucceed()
        {
            this.RunValidTest(CsdlReader.Parse);
        }

        [Fact]
        public void ParsingInvalidXmlWithNoReferencesShouldThrow()
        {
            this.RunInvalidTest(CsdlReader.Parse);
        }

        [Fact]
        public void ParsingvalidXmlWithNavigationPropertyInComplex()
        {
            string complexWithNav =
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
                    "<NavigationPropertyBinding Path=\"HomeAddress/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"Addresses/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"WorkAddress/DefaultNs.WorkAddress/CountryOrRegion\" Target=\"CountriesOrRegions\" />" +
                "</EntitySet>" +
                "<EntitySet Name=\"Cities\" EntityType=\"DefaultNs.City\" />" +
                "<EntitySet Name=\"CountriesOrRegions\" EntityType=\"DefaultNs.CountryOrRegion\" />" +
                "</EntityContainer></Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(complexWithNav).CreateReader());
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
        public void ParsingXmlWithCollectionOfNavOnComplex()
        {
            string complexWithNav = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
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

            var model = CsdlReader.Parse(XElement.Parse(complexWithNav).CreateReader());
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
            string csdl =
        "<edmx:Edmx xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\" Version=\"4.0\">" +
          "<edmx:DataServices>" +
            "<Schema xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" Namespace=\"test\">" +
              "<Function Name=\"TestFunction\">" +
                "<Parameter Name = \"requiredParam\" Type=\"Edm.String\"/>" +
                "<Parameter Name = \"optionalParam\" Type=\"Edm.String\">" +
                    "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\"/>" +
                "</Parameter>" +
                "<Parameter Name = \"optionalParamWithDefault\" Type=\"Edm.String\">" +
                    "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                      "<Record>" +
                        "<PropertyValue Property=\"DefaultValue\" String=\"Smith\"/>" +
                      "</Record>" +
                    "</Annotation>" +
                "</Parameter>" +
                "<ReturnType Type=\"Edm.Boolean\"/>" +
              "</Function>" +
              "<EntityContainer Name=\"Default\">" +
                "<FunctionImport Name=\"TestFunction\" Function=\"test.TestFunction\"/>" +
              "</EntityContainer>" +
            "</Schema>" +
          "</edmx:DataServices>" +
        "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
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

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void ParsingReturnTypeAnnotationShouldSucceed(EdmVocabularyAnnotationSerializationLocation location)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Function Name=""TestFunction"">
        <ReturnType Type=""Edm.PrimitiveType"" Nullable=""false"" >
          {0}
        </ReturnType>
      </Function>
      <Annotations Target=""NS.TestFunction()/$ReturnType"">
         {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string annotationString =
                "<Annotation Term =\"Org.OData.Validation.V1.DerivedTypeConstraint\">" +
                  "<Collection>" +
                     "<String>Edm.Int32</String>" +
                     "<String>Edm.Boolean</String>" +
                   "</Collection>" +
                 "</Annotation>";
            string inline = location == EdmVocabularyAnnotationSerializationLocation.Inline ? annotationString : "";
            string outLine = location == EdmVocabularyAnnotationSerializationLocation.Inline ? "" : annotationString;
            string csdl = String.Format(template, inline, outLine);

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
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
        public void ParsingValidXmlWithOneReferencesShouldSucceed()
        {
            this.RunValidTest(r => CsdlReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingInvalidXmlWithOneReferencesShouldThrow()
        {
            this.RunInvalidTest(r => CsdlReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingValidXmlWithManyReferencesShouldSucceed()
        {
            this.RunValidTest(r => CsdlReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [Fact]
        public void ParsingInvalidXmlWithManyReferencesShouldThrow()
        {
            this.RunInvalidTest(r => CsdlReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [Fact]
        public void ParsingInvalidXmlWithMultipleEntityContainersShouldThrow()
        {
            string EdmxwithMultipleEntityContainers = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container1"" />
      <EntityContainer Name=""Container2"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            Action parseAction = () => CsdlReader.Parse(XElement.Parse(EdmxwithMultipleEntityContainers).CreateReader());
            var exception = Assert.Throws<EdmParseException>(parseAction);
            Assert.Contains(SRResources.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer, exception.Message);
            Assert.Single(exception.Errors);
        }

        [Fact]
        public void ParsingNavigationPropertyWithEdmEntityTypeWorks()
        {
            string navigationProperty =
                @"<NavigationProperty Name=""EntityNavigationProperty"" Type=""Edm.EntityType"" />";

            IEdmModel model = GetEdmModel(properties: navigationProperty);
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
        public void ParsingPropertyWithEdmComplexTypeWorks()
        {
            string complexProperty =
                @"<Property Name=""ComplexProperty"" Type=""Edm.ComplexType"" />";

            IEdmModel model = GetEdmModel(properties: complexProperty);
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
        public void ParsingPropertyWithEdmPrimitiveTypeWorks()
        {
            string complexProperty =
                @"<Property Name=""PrimitiveProperty"" Type=""Edm.PrimitiveType"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: complexProperty);
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
        public void ParsingPropertyWithDecimalTypeWorks()
        {
            string decimalProperty =
                @"<Property Name=""DecimalProperty"" Type=""Edm.Decimal"" Precision=""6"" Scale=""2"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: decimalProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Equal(2, property.Type.AsDecimal().Scale);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Equal(2, property.Type.AsDecimal().Scale);
        }

        [Fact]
        public void ParsingPropertyWithZeroScaleDecimalTypeWorks()
        {
            string decimalProperty =
                @"<Property Name=""DecimalProperty"" Type=""Edm.Decimal"" Precision=""6"" Scale=""0"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: decimalProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Equal(0, property.Type.AsDecimal().Scale);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Equal(0, property.Type.AsDecimal().Scale);
        }

        [Fact]
        public void ParsingPropertyWithDecimalTypeDefaultsToNullScale()
        {
            string decimalProperty =
                @"<Property Name=""DecimalProperty"" Type=""Edm.Decimal"" Precision=""6"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: decimalProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Null(property.Type.AsDecimal().Scale);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Null(property.Type.AsDecimal().Scale);
        }

        [Fact]
        public void ParsingPropertyWithVariableScaleDecimalTypeWorks()
        {
            string decimalProperty =
                @"<Property Name=""DecimalProperty"" Type=""Edm.Decimal"" Precision=""6"" Scale=""Variable"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: decimalProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Null(property.Type.AsDecimal().Scale);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "DecimalProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetDecimal(false).Definition, property.Type.Definition);
            Assert.Equal(6, property.Type.AsDecimal().Precision);
            Assert.Null(property.Type.AsDecimal().Scale);
        }

        [Fact]
        public void ParsingTermPropertyWithEdmPathTypeWorks()
        {
            string termTypes =
                @"<ComplexType Name=""SelectType"" >
                    <Property Name=""DefaultSelect"" Type=""Collection(Edm.PropertyPath)"" />
                    <Property Name=""DefaultHidden"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"" />
                  </ComplexType>
                  <Term Name=""MyTerm"" Type=""NS.SelectType"" />";

            IEdmModel model = GetEdmModel(types: termTypes);
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
        public void ParsingPropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<Property Name=""PathProperty"" Type=""Collection(Edm.PropertyPath)"" />";

            IEdmModel model = GetEdmModel(properties: properties);

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
        public void ParsingRecursivePropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<Property Name=""PathProperty"" Type=""Collection(Edm.PropertyPath)"" />
                  <Property Name=""ComplexProperty"" Type=""NS.Address"" />";

            IEdmModel model = GetEdmModel(properties: properties);

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
        public void ParsingNavigationPropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<NavigationProperty Name=""NavigationProperty"" Type=""Collection(NS.Customer)"" />
                  <Property Name=""PathProperty"" Type=""Collection(Edm.AnnotationPath)"" />";

            IEdmModel model = GetEdmModel(properties: properties);

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
        public void ParsingEnumMemberWithAnnotationsWorks()
        {
            string types =
@"<EnumType Name=""Color"" >
  <Member Name=""Red"" Value=""1"" >
    <Annotation String=""Inline Description"" Term=""Org.OData.Core.V1.LongDescription""/>
    <Annotation String=""Inline MyTerm Value"" Term=""NS.MyTerm""/>
  </Member>
  <Member Name=""Blue"" Value=""2"" />
</EnumType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Color/Blue"" >
  <Annotation String=""OutOfLine Description"" Term=""Org.OData.Core.V1.LongDescription""/>
  <Annotation String=""OutOfLine MyTerm Value"" Term=""NS.MyTerm""/>
</Annotations>";

            IEdmModel model = GetEdmModel(types: types);
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

        private void RunValidTest(Func<XmlReader, IEdmModel> parse)
        {
            var result = parse(this.validReader);
            Assert.NotNull(result);
            Assert.Equal("Test.Container", result.EntityContainer.FullName());
        }

        private void RunInvalidTest(Func<XmlReader, IEdmModel> parse)
        {
            Action parseAction = () => parse(this.invalidReader);

            EdmParseException exception = Assert.Throws<EdmParseException>(parseAction);
            Assert.Equal(Error.Format(ErrorStrings.EdmParseException_ErrorsEncounteredInEdmx, ErrorMessage), exception.Message);
            Assert.Single(exception.Errors, e => e.ToString() == ErrorMessage);
        }

        private static IEdmModel GetEdmModel(string types = "", string properties = "")
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {1}
      </EntityType>
      <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" />
        {1}
      </ComplexType>
      {0}
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, types, properties);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingInLineOptionalParameterWorks()
        {
            string expected =
           "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
           "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
             "<edmx:DataServices>" +
               "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
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
               "</Schema>" +
             "</edmx:DataServices>" +
           "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(expected).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            VerifyOptionalParameter(function);
        }

        [Theory]
        [InlineData("NS.TestFunction")] // non-overloads
        [InlineData("NS.TestFunction(Edm.String, Edm.String, Edm.String)")] // overloads
        public void ParsingOutOfLineOptionalParameterWorks(string fullName)
        {
            string expected = String.Format(
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
                 "<EntityContainer Name=\"Default\">" +
                   "<FunctionImport Name=\"TestFunction\" Function=\"test.TestFunction\" />" +
                 "</EntityContainer>" +
                 "<Annotations Target=\"{0}/optionalParam\" >" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" >" +
                     "<Record />" +
                  "</Annotation> " +
                 "</Annotations>" +
                 "<Annotations Target=\"{0}/optionalParamWithDefault\" >" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" >" +
                     "<Record Type=\"Org.OData.Core.V1.OptionalParameterType\">" +
                       "<PropertyValue Property=\"DefaultValue\" String=\"Smith\" />" +
                     "</Record>" +
                  "</Annotation> " +
                 "</Annotations>" +
               "</Schema>" +
             "</edmx:DataServices>" +
           "</edmx:Edmx>", fullName);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(expected).CreateReader(), out model, out errors);
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
        public void ParsingUrlEscapeFunctionWorks(bool escaped)
        {
            string format = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" >
               <edmx:DataServices>
                <Schema Namespace=""NS"" xmlns =""http://docs.oasis-open.org/odata/ns/edm"" >
                   <EntityType Name=""Entity"" >
                    <Key>
                      <PropertyRef Name=""Id"" />
                    </Key>
                    <Property Name=""Id"" Type =""Edm.Int32"" Nullable =""false"" />
                  </EntityType>
                  <Function Name=""Function"" IsBound =""true"" >
                    <Parameter Name=""entity"" Type =""NS.Entity"" />
                    <Parameter Name=""path"" Type =""Edm.String"" />
                    <ReturnType Type=""Edm.Int32"" />
                    {0}
                  </Function>
                </Schema>
              </edmx:DataServices>
            </edmx:Edmx>";

            string annotation = escaped ? "<Annotation Term=\"Org.OData.Community.V1.UrlEscapeFunction\" Bool=\"true\" />" : "";
            string csdl = String.Format(format, annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            Assert.Equal(escaped, model.IsUrlEscapeFunction(function));
        }

        [Fact]
        public void ParsingBaseAndDerivedTypeWithSameAnnotationWorksButValidationSuccessful()
        {
            string annotations = @"
            <Annotations Target=""NS.Base"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Base description"" />
            </Annotations>
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description"" />
            </Annotations>";

            IEdmModel model = GetInheritanceEdmModel(annotations);

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
        public void ParsingDerivedTypeWithDuplicatedAnnotationsWorksButValidationFailed()
        {
            string annotations = @"
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description 1"" />
            </Annotations>
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description 2"" />
            </Annotations>";

            IEdmModel model = GetInheritanceEdmModel(annotations);

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

        private static IEdmModel GetInheritanceEdmModel(string annotation)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Base"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""Derived"" BaseType=""NS.Base"" />
      {0}
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingPropertyWithCoreTypeDefinitionAsPropertyTypeWorks()
        {
            string csdl =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<ComplexType Name=\"Complex\">" +
                    "<Property Name=\"ModifiedDate\" Type=\"Org.OData.Core.V1.LocalDateTime\" />" +
                  "</ComplexType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
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
        public void ParsingAnnotationPathExpressionWorks()
        {
            string csdl =
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

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
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
        public void ParsingUntypedPropertiesWorks()
        {
            string csdl =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "<edmx:DataServices>" +
                    "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                      "<ComplexType Name=\"Complex\">" +
                        "<Property Name=\"Value\" Type=\"Edm.Untyped\" />" +
                        "<Property Name=\"Data\" Type=\"Collection(Edm.Untyped)\" />" +
                      "</ComplexType>" +
                    "</Schema>" +
                  "</edmx:DataServices>" +
                "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
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

        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ValidateEdmxVersions(string odataVersion)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"" + odataVersion + "\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\"><edmx:DataServices /></edmx:Edmx>";

            var stringReader = new StringReader(xml);
            var xmlReader = System.Xml.XmlReader.Create(stringReader); 

            IEdmModel edmModel = null;
            IEnumerable<EdmError> edmErrors;

            // Read in the CSDL and verify the version
            CsdlReader.TryParse(xmlReader, out edmModel, out edmErrors);
            Assert.Empty(edmErrors);
            Assert.Equal(edmModel.GetEdmVersion(), odataVersion == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401);
        }

        [Fact]
        public void ImportsAnnotationsFromExernalReferencedModel()
        {
            string permissionsCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimpleModel.xml"">
    <edmx:Include Namespace=""Default""/>
    <edmx:Include Namespace=""Example.Types"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Permissions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""BasicType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <Annotations Target=""Default.Container/Products"">
        <Annotation Term=""NotIncludedNS.MyAnnotationPathTerm"" AnnotationPath=""abc/efg"" />
        <Annotation Term=""Org.OData.Capabilities.V1.InsertRestrictions"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Create"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
        <Annotation Term=""Org.OData.Capabilities.V1.DeleteRestrictions"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Delete"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";
            string mainCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimplePermissions.xml"">
    <edmx:IncludeAnnotations  TermNamespace=""Org.OData.Capabilities.V1""/>
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Types"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Price"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""Example.Types.Product"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";

            Func<Uri, XmlReader> getReferenceModelReader = uri =>
            {
                string csdl = uri.OriginalString == "SimplePermissions.xml" ? permissionsCsdl : mainCsdl;
                var stringReader = new StringReader(csdl);
                return XmlReader.Create(stringReader);
            };


            var reader = XmlReader.Create(new StringReader(mainCsdl));
            IEdmModel model = CsdlReader.Parse(reader, getReferencedModelReaderFunc: getReferenceModelReader);

            var entitySet = model.FindDeclaredEntitySet("Products");
            var annotations = model.FindVocabularyAnnotations(entitySet);
            Assert.Equal(2, annotations.Count());

            // only imports annotations terms in the Org.OData.Capabilities.V1 namespace
            IEdmVocabularyAnnotation insertRestrictions = annotations.FirstOrDefault(a => a.Term.Name == "InsertRestrictions");
            Assert.NotNull(insertRestrictions);
            IEdmVocabularyAnnotation deleteRestrictions = annotations.FirstOrDefault(a => a.Term.Name == "DeleteRestrictions");
            Assert.NotNull(deleteRestrictions);

            IEdmRecordExpression record = insertRestrictions.Value as IEdmRecordExpression;
            Assert.NotNull(record);
            var insertPermissions = record.FindProperty("Permissions").Value as IEdmCollectionExpression;
            Assert.NotNull(insertPermissions);

            var permission = insertPermissions.Elements.FirstOrDefault() as IEdmRecordExpression;
            Assert.NotNull(permission);
            var scheme = permission.FindProperty("SchemeName");
            Assert.NotNull(scheme);
            Assert.Equal("Scheme", ((IEdmStringConstantExpression)scheme.Value).Value);

            // do not import other schema elements since there's
            // no explicit <edmx:Include Namespace="Example.Permissions" /> declaration
            var basicType = model.FindType("Example.Permissions.BasicType");
            Assert.Null(basicType);
        }

        [Fact]
        public void ImportsAnnotationsFromReferencedModelsOnlyIfTargetNamespaceAndQualifierMatch()
        {
            string permissionsCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimpleModel.xml"">
    <edmx:Include Namespace=""Default""/>
    <edmx:Include Namespace=""Example.Types"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Permissions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Annotations Target=""Example.Types.Product"">
         <Annotation Term=""OtherIncludedNS.SomeAnnotationPath"" AnnotationPath=""abc/efg"" />
      </Annotations>
      <Annotations Target=""Default.Container/Products"">
        <Annotation Term=""OtherIncludedNS.SomeAnnotationPath"" AnnotationPath=""abc/efg"" />
        <Annotation Term=""NotIncludedNS.MyAnnotationPathTerm"" AnnotationPath=""abc/efg"" />
        <Annotation Term=""Org.OData.Capabilities.V1.InsertRestrictions"" Qualifier=""Insert"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Create"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
        <Annotation Term=""Org.OData.Capabilities.V1.DeleteRestrictions"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Delete"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";
            string mainCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimplePermissions.xml"">
    <edmx:IncludeAnnotations  TermNamespace=""Org.OData.Capabilities.V1"" Qualifier=""Insert""/>
    <edmx:IncludeAnnotations  TermNamespace=""OtherIncludedNS"" TargetNamespace=""Example.Types""/>
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Types"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Price"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""ODataAuthorizationDemo.Models.Product"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";

            Func<Uri, XmlReader> getReferenceModelReader = uri =>
            {
                string csdl = uri.OriginalString == "SimplePermissions.xml" ? permissionsCsdl : mainCsdl;
                var stringReader = new StringReader(csdl);
                return XmlReader.Create(stringReader);
            };

            var reader = XmlReader.Create(new StringReader(mainCsdl));
            IEdmModel model = CsdlReader.Parse(reader, getReferencedModelReaderFunc: getReferenceModelReader);

            var entitySet = model.FindDeclaredEntitySet("Products");
            var entitySetAnnotations = model.FindVocabularyAnnotations(entitySet);
            Assert.Single(entitySetAnnotations);


            // imports annotation terms in the Org.OData.Capabilities.V1 namespace that match the qualifier Insert
            IEdmVocabularyAnnotation insertRestrictions = entitySetAnnotations.FirstOrDefault(a => a.Term.Name == "InsertRestrictions");
            Assert.NotNull(insertRestrictions);

            // imports annotation terms in the OtherIncludedNS namespace that match the target namespace Example.Types
            var product = model.FindDeclaredType("Example.Types.Product");
            var productAnnotations = model.FindVocabularyAnnotations(product);
            Assert.Single(productAnnotations);
            IEdmVocabularyAnnotation productAnnotation = productAnnotations.FirstOrDefault(a => a.Term.Name == "SomeAnnotationPath");
            Assert.NotNull(productAnnotation);
        }

        [Fact]
        public void ImportAnnotationsFromExternalNamespaceWithAlias()
        {
            string permissionsCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimpleModel.xml"">
    <edmx:Include Namespace=""Default""/>
    <edmx:Include Namespace=""Example.Types"" Alias=""examples"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Permissions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Annotations Target=""examples.Product"">
         <Annotation Term=""MyNS.SomeAnnotationPath"" AnnotationPath=""abc/efg"" />
      </Annotations>
      <Annotations Target=""Default.Container/Products"">
        <Annotation Term=""MyNS.MyAnnotationPathTerm"" AnnotationPath=""abc/efg"" />
        <Annotation Term=""Org.OData.Capabilities.V1.InsertRestrictions"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Create"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";
            string mainCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimplePermissions.xml"">
    <edmx:IncludeAnnotations TermNamespace=""MyNS"" TargetNamespace=""Example.Types"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Types"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Price"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""Example.Types.Product"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";

            Func<Uri, XmlReader> getReferenceModelReader = uri =>
            {
                string csdl = uri.OriginalString == "SimplePermissions.xml" ? permissionsCsdl : mainCsdl;
                var stringReader = new StringReader(csdl);
                return XmlReader.Create(stringReader);
            };

            var reader = XmlReader.Create(new StringReader(mainCsdl));
            IEdmModel model = CsdlReader.Parse(reader, getReferencedModelReaderFunc: getReferenceModelReader);

            var entity = model.FindDeclaredType("Example.Types.Product");
            var entityAnnotations = model.FindVocabularyAnnotations(entity);
            Assert.Single(entityAnnotations);

            IEdmVocabularyAnnotation annotation = entityAnnotations.First(a => a.Term.Name == "SomeAnnotationPath");
            Assert.NotNull(annotation);
        }

        [Fact]
        public void ImportSchemaElementsOnlyFromReferencedNamespacesl()
        {
            string permissionsCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimpleModel.xml"">
    <edmx:Include Namespace=""Default""/>
    <edmx:Include Namespace=""Example.Types"" Alias=""examples"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Permissions"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Annotations Target=""examples.Product"">
         <Annotation Term=""MyNS.SomeAnnotationPath"" AnnotationPath=""abc/efg"" />
      </Annotations>
      <Annotations Target=""Default.Container/Products"">
        <Annotation Term=""MyNS.MyAnnotationPathTerm"" AnnotationPath=""abc/efg"" />
        <Annotation Term=""Org.OData.Capabilities.V1.InsertRestrictions"">
          <Record>
            <PropertyValue Property=""Permissions"">
              <Collection>
                <Record>
                  <PropertyValue Property=""SchemeName"" String=""Scheme"" />
                  <PropertyValue Property=""Scopes"">
                    <Collection>
                      <Record>
                        <PropertyValue Property=""Scope"" String=""Product.Create"" />
                      </Record>
                    </Collection>
                  </PropertyValue>
                </Record>
              </Collection>
            </PropertyValue>
          </Record>
        </Annotation>
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";

            string mainCsdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference  Uri=""SimplePermissions.xml"">
    <edmx:IncludeAnnotations TermNamespace=""MyNS"" TargetNamespace=""Example.Types"" />
  </edmx:Reference >
  <edmx:DataServices>
    <Schema Namespace=""Example.Types"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Price"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema Namespace=""Other.Types"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Person"">
          <Key>
            <PropertyRef Name=""Id"" />
          </Key>
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </EntityType> 
    </Schema>
    <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""Example.Types.Product"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";

            Func<Uri, XmlReader> getReferenceModelReader = uri =>
            {
                string csdl = uri.OriginalString == "SimplePermissions.xml" ? permissionsCsdl : mainCsdl;
                var stringReader = new StringReader(csdl);
                return XmlReader.Create(stringReader);
            };

            var reader = XmlReader.Create(new StringReader(permissionsCsdl));
            IEdmModel model = CsdlReader.Parse(reader, getReferencedModelReaderFunc: getReferenceModelReader);

            var productType = model.FindType("Example.Types.Product");
            Assert.NotNull(productType);
            var container = model.FindEntityContainer("Default.Container");
            Assert.NotNull(container);

            // this is not imported because the Other.Types namespace is not referenced by the permissionsCsdl model
            var personType = model.FindType("Other.Types.Person");
            Assert.Null(personType);
        }

        // We currently do not initialize the Scale and SpatialReferenceIdentifier of the respective Edm type references when reading the scale and srid values if the value is variable or Variable 
        // The IEdmDecimalTypeReference'Scale value should have the value `variable` or `Variable` depending on what is in the CSDL.
        // Also, the IEdmSpatialTypeReference's SpatialReferenceIdentifier should have the value `variable` or `Variable` depending on what is in the Csdl. 
        [Theory]
        [InlineData("Variable")]
        [InlineData("variable")]
        public void ReadingScaleAndSridValue_VaribaleValue_NotInitializedOnRespectiveEdms(string variableValue)
        {
            // Build the CSDL template with the provided variable value
            string csdlTemplate = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                "<ComplexType Name=\"Complex\">" +
                $"<Property Name=\"GeographyPoint\" Type=\"Edm.GeographyPoint\" SRID=\"{variableValue}\" />" +
                $"<Property Name=\"DecimalProperty\" Type=\"Edm.Decimal\" Nullable=\"false\" Scale=\"{variableValue}\" />" +
                "</ComplexType>" +
                "</Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            // Parse the CSDL template
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdlTemplate).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }

            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            IEdmComplexType complexType = model.FindDeclaredType("NS.Complex") as IEdmComplexType;
            IEdmDecimalTypeReference decimalPropertyReference = complexType.FindProperty("DecimalProperty").Type as IEdmDecimalTypeReference;

            Assert.Null(decimalPropertyReference.Scale);

            IEdmSpatialTypeReference geographyPointProperty = complexType.FindProperty("GeographyPoint").Type as IEdmSpatialTypeReference;
            Assert.Null(geographyPointProperty.SpatialReferenceIdentifier);
        }

        [Theory]
        [InlineData("", "The root element has no namespace.")]
        [InlineData(" ", "The root element has no namespace.")]
        [InlineData("xmlns=\"\"", "The root element has no namespace.")]
        [InlineData("xmlns=\"http://docs.oasis-open.org/odata/ns/edm-invalid\"", "The namespace 'http://docs.oasis-open.org/odata/ns/edm-invalid' is invalid.")]
        [InlineData("xmlns=\"''\"", "The namespace '''' is invalid.")]
        [InlineData("xmlns=\"null\"", "The namespace 'null' is invalid.")]
        [InlineData(null, "The root element has no namespace.")]
        public void ShouldReportUnexpectedRootNamespaceWithSchemaTagMissingXMLNamespace(string xmlns, string firstPartErrorMessage)
        {
            // Arrange
            string csdl =
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
                  <edmx:DataServices>
                    <Schema Namespace="NS" Alias="self" {xmlns}>
                      <EnumType Name="someEnum">
                        <Member Name="notApplicable" Value="0" />
                        <Member Name="enabled" Value="1" />
                      </EnumType>
                    </Schema>
                  </edmx:DataServices>
                </edmx:Edmx>
                """;

            var errorMessage = $"{firstPartErrorMessage} The root element is expected to belong to one of the following namespaces: 'http://docs.oasis-open.org/odata/ns/edm, http://docs.oasis-open.org/odata/ns/edm'.";

            // Act
            using var reader = XmlReader.Create(new StringReader(csdl));
            var success = CsdlReader.TryParse(reader, out var model, out var errors);

            // Assert
            Assert.False(success);
            Assert.NotEmpty(errors);

            var error = errors.FirstOrDefault();
            Assert.Equal(EdmErrorCode.UnexpectedXmlElement, error?.ErrorCode);
            Assert.Equal(errorMessage, error.ErrorMessage);
            Assert.Equal(4, (error?.ErrorLocation as CsdlLocation)?.LineNumber);
            Assert.Equal(6, (error?.ErrorLocation as CsdlLocation)?.LinePosition);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("xmlns=\"\"")]
        [InlineData("xmlns=\"http://docs.oasis-open.org/odata/ns/edm\"")]
        [InlineData("xmlns=\"http://docs.oasis-open.org/odata/ns/edmx-invalid\"")]
        [InlineData("xmlns=\"''\"")]
        [InlineData("xmlns=\"null\"")]
        [InlineData(null)]
        public void ShouldReportUnexpectedXmlElementWithEdmxTagMissingXMLNamespace(string xmlns)
        {
            // Arrange
            string csdl =
                $"""
                <Edmx Version="4.0" {xmlns}>
                  <edmx:DataServices>
                    <Schema Namespace="NS" Alias="self" xmlns="http://docs.oasis-open.org/odata/ns/edm">
                      <EnumType Name="someEnum">
                        <Member Name="notApplicable" Value="0" />
                        <Member Name="enabled" Value="1" />
                      </EnumType>
                    </Schema>
                  </edmx:DataServices>
                </Edmx>
                """;

            // Act
            using var reader = XmlReader.Create(new StringReader(csdl));
            var success = CsdlReader.TryParse(reader, out var model, out var errors);

            // Assert
            Assert.False(success);
            Assert.NotEmpty(errors);

            var error = errors.FirstOrDefault();
            Assert.Equal(EdmErrorCode.UnexpectedXmlElement, error?.ErrorCode);
            Assert.Contains("The element 'Edmx' was unexpected for the root element. The root element should be Edmx.", error.ErrorMessage);
            Assert.Equal(1, (error?.ErrorLocation as CsdlLocation)?.LineNumber);
            Assert.Equal(2, (error?.ErrorLocation as CsdlLocation)?.LinePosition);
        }

        [Fact]
        public void ShouldReportCorrectLineNumbersWithMultiLineElements()
        {
            string csdl =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
                  <edmx:DataServices>
                    <Schema Namespace="name.space" Alias="self" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
                      <EnumType

                        Name="someEnum">
                        <Member Name="notApplicable" Value="0" />

                        <Member Name="enabled" Value="1" />
                        <Member Name="disabled" Value="2" />
                        <Member Name="unknownFutureValue" Value="3" />
                      </EnumType>

                      <EnumType Name="otherEnum">
                        <Member Name="success" Value="0" />
                        <Member Name="failure" Value="1" />
                        <Member Name="unknownFutureValue" Value="2" />
                      </EnumType>
                    </Schema>
                  </edmx:DataServices>
                </edmx:Edmx>
                """;



            using var reader = XmlReader.Create(new StringReader(csdl));
            var model = CsdlReader.Parse(reader);

            var someEnum = model.FindDeclaredType("name.space.someEnum") as IEdmEnumType;
            AssertLineLocation(someEnum, 5, 8);

            AssertLineLocation(
                someEnum.Members.FirstOrDefault(m => m.Name == "disabled"), 11, 10);

            var otherEnum = model.FindDeclaredType("name.space.otherEnum") as IEdmElement;
            AssertLineLocation(otherEnum, 15, 8);
        }


        [Fact]
        public void ShouldReportCorrectLineNumbersWithMultiLineReferenceElement()
        {
            string csdl =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
                  <edmx:Reference
                    Uri="https://docs.oasis-open.org/odata/odata-data-aggregation-ext/v4.0/cs03/vocabularies/Org.OData.Aggregation.V1.xml"
                  >
                    <edmx:Include
                      Namespace="Org.OData.Aggregation.V1" Alias="Aggregation"/>
                  </edmx:Reference>
                  <edmx:DataServices>
                    <Schema Namespace="name.space" Alias="self" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
                      <EnumType

                        Name="someEnum">
                        <Member Name="notApplicable" Value="0" />

                        <Member Name="enabled" Value="1" />
                        <Member Name="disabled" Value="2" />
                        <Member Name="unknownFutureValue" Value="3" />
                      </EnumType>

                      <EnumType Name="otherEnum">
                        <Member Name="success" Value="0" />
                        <Member Name="failure" Value="1" />
                        <Member Name="unknownFutureValue" Value="2" />
                      </EnumType>
                    </Schema>
                  </edmx:DataServices>
                </edmx:Edmx>
                """;



            using var reader = XmlReader.Create(new StringReader(csdl));
            var model = CsdlReader.Parse(reader);

            var reference = model.GetEdmReferences().FirstOrDefault();
            var edmInclude = reference.Includes.FirstOrDefault(i => i.Alias == "Aggregation");
            AssertLineLocation(edmInclude, 6, 6);

            var someEnum = model.FindDeclaredType("name.space.someEnum") as IEdmEnumType;
            AssertLineLocation(someEnum, 11, 8);

            AssertLineLocation(
                someEnum.Members.FirstOrDefault(m => m.Name == "disabled"), 17, 10);

            var otherEnum = model.FindDeclaredType("name.space.otherEnum") as IEdmElement;
            AssertLineLocation(otherEnum, 21, 8);
        }


        [Fact]
        public void ShouldReportCorrectLineNumbersWithMultiLineElementsAndMultipleSchemas()
        {
            string csdl =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
                  <edmx:DataServices>
                    <Schema Namespace="ns.one" Alias="self" xmlns="http://docs.oasis-open.org/odata/ns/edm" xmlns:ags="http://aggregator.microsoft.com/internal">
                      <EnumType
                        Name="someEnum">
                        <Member Name="notApplicable" Value="0" />
                        <Member Name="enabled" Value="1" />
                        <Member Name="disabled" Value="2" />
                        <Member Name="unknownFutureValue" Value="3" />
                      </EnumType>
                      <EnumType Name="otherEnum">
                        <Member Name="success" Value="0" />
                        <Member Name="failure"
                          Value="1" />
                        <Member Name="unknownFutureValue" Value="2" />
                      </EnumType>
                    </Schema>
                    <Schema Namespace="ns.two" xmlns="http://docs.oasis-open.org/odata/ns/edm"
                      xmlns:ags="http://aggregator.microsoft.com/internal">
                      <EntityType
                        Name="task">
                        <Key>
                          <PropertyRef Name="id" />
                        </Key>
                        <Property Name="id" Type="Edm.String" />
                        <Property 
                            Name="status"
                            Type="ns.one.someEnum" />
                        <Property Name="title" Type="Edm.String" />
                      </EntityType>
                        <EntityContainer Name="defaultContainer">
                        <EntitySet Name="tasks" EntityType="ns.two.task"/>
                      </EntityContainer>
                    </Schema>
                  </edmx:DataServices>
                </edmx:Edmx>
                """;



            using var reader = XmlReader.Create(new StringReader(csdl));
            var model = CsdlReader.Parse(reader);

            AssertLineLocation(
                model.FindDeclaredType("ns.one.someEnum"),
                5,
                8);

            var otherEnum = model.FindDeclaredType("ns.one.otherEnum") as IEdmEnumType;

            AssertLineLocation(
                otherEnum,
                12,
                8);

            AssertLineLocation(
                otherEnum.Members.FirstOrDefault(m => m.Name == "unknownFutureValue"),
                16,
                10);

            var task = model.FindDeclaredType("ns.two.task") as IEdmEntityType;

            AssertLineLocation(
                task,
                21,
                8);

            AssertLineLocation(
                task.DeclaredProperties.FirstOrDefault(p => p.Name == "title"),
                30,
                10);

            AssertLineLocation(
                model.EntityContainer,
                32,
                10);

            AssertLineLocation(
                model.FindDeclaredEntitySet("defaultContainer.tasks"),
                33,
                10);
        }

        static void VerifyXmlModel(IEdmModel model, string csdl)
        {
            var stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            IEnumerable<EdmError> errors;
            Assert.True(CsdlWriter.TryWriteCsdl(model, xmlWriter, CsdlTarget.OData, out errors));
            Assert.Empty(errors);
            stream.Position = 0;
            Assert.Contains(csdl, new StreamReader(stream).ReadToEnd());
        }

        private static void AssertLineLocation(IEdmElement element, int lineNumber, int position)
        {
            Assert.NotNull(element);
            var location = element.Location() as CsdlLocation;
            Assert.NotNull(location);
            Assert.Equal(lineNumber, location.LineNumber);
            Assert.Equal(position, location.LinePosition);
        }
    }
}