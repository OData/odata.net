//---------------------------------------------------------------------
// <copyright file="OasisRelationshipChangesAcceptanceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.ScenarioTests
{
    public class OasisRelationshipChangesAcceptanceTests
    {
        private const string RepresentativeEdmxDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyProperty"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""navigation"" Type=""Collection(Test.EntityType)"" Partner=""NAVIGATION"" ContainsTarget=""true"">
          <OnDelete Action=""Cascade"" />
        </NavigationProperty>
        <NavigationProperty Name=""NAVIGATION"" Type=""Test.EntityType"" Partner=""navigation"">
          <ReferentialConstraint Property=""ForeignKeyId2"" ReferencedProperty=""ID2"" />
          <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
        <NavigationProperty Name=""NonKeyPrincipalNavigation"" Type=""Test.EntityType"" Partner=""OtherNavigation"">
          <ReferentialConstraint Property=""ForeignKeyProperty"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
        <NavigationProperty Name=""OtherNavigation"" Type=""Collection(Test.EntityType)"" Partner=""NonKeyPrincipalNavigation"" />
      </EntityType>
      <EntityType Name=""DerivedEntityType"" BaseType=""Test.EntityType"">
        <NavigationProperty Name=""DerivedNavigation"" Type=""Test.DerivedEntityType"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet1"" EntityType=""Test.EntityType"">
          <NavigationPropertyBinding Path=""navigation"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""NAVIGATION"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""NonKeyPrincipalNavigation"" Target=""EntitySet1"" />
          <NavigationPropertyBinding Path=""Test.DerivedEntityType/DerivedNavigation"" Target=""EntitySet1"" />
        </EntitySet>
        <EntitySet Name=""EntitySet2"" EntityType=""Test.EntityType"">
          <NavigationPropertyBinding Path=""navigation"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""NAVIGATION"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""NonKeyPrincipalNavigation"" Target=""EntitySet2"" />
          <NavigationPropertyBinding Path=""Test.DerivedEntityType/DerivedNavigation"" Target=""EntitySet2"" />
        </EntitySet>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private readonly IEdmModel representativeModel;
        private readonly IEdmEntitySet entitySet1;
        private readonly IEdmEntitySet entitySet2;
        private readonly IEdmEntityType entityType;
        private readonly IEdmNavigationProperty navigation1;
        private readonly IEdmNavigationProperty navigation2;
        private readonly IEdmNavigationProperty nonKeyPrincipalNavigation;
        private readonly IEdmNavigationProperty derivedNavigation;

        public OasisRelationshipChangesAcceptanceTests()
        {
            this.representativeModel = CsdlReader.Parse(XElement.Parse(RepresentativeEdmxDocument).CreateReader());
            var container = this.representativeModel.EntityContainer;
            this.entitySet1 = container.FindEntitySet("EntitySet1");
            this.entitySet2 = container.FindEntitySet("EntitySet2");
            this.entityType = this.representativeModel.FindType("Test.EntityType") as IEdmEntityType;

            this.entitySet1.Should().NotBeNull();
            this.entitySet2.Should().NotBeNull();
            this.entityType.Should().NotBeNull();

            this.navigation1 = this.entityType.FindProperty("navigation") as IEdmNavigationProperty;
            this.navigation2 = this.entityType.FindProperty("NAVIGATION") as IEdmNavigationProperty;
            nonKeyPrincipalNavigation = this.entityType.FindProperty("NonKeyPrincipalNavigation") as IEdmNavigationProperty;

            var derivedType = this.representativeModel.FindType("Test.DerivedEntityType") as IEdmEntityType;
            derivedType.Should().NotBeNull();
            this.derivedNavigation = derivedType.FindProperty("DerivedNavigation") as IEdmNavigationProperty;

            this.navigation1.Should().NotBeNull();
            this.navigation2.Should().NotBeNull();
            this.derivedNavigation.Should().NotBeNull();
        }

        [Fact]
        public void RepresentativeModelShouldBeValid()
        {
            IEnumerable<EdmError> errors;
            this.representativeModel.Validate(out errors).Should().BeTrue();
            errors.Should().BeEmpty();
        }

        [Fact]
        public void FindNavigationTargetShouldUseBinding()
        {
            this.entitySet2.FindNavigationTarget(this.navigation2).Should().BeSameAs(this.entitySet2);
            this.entitySet1.FindNavigationTarget(this.derivedNavigation).Should().BeSameAs(this.entitySet1);
        }

        [Fact]
        public void ReferenceNavigationPropertyTypeShouldContinueToWork()
        {
            this.navigation2.Type.Definition.Should().BeSameAs(this.entityType);
        }

        [Fact]
        public void CollectionNavigationPropertyTypeShouldContinueToWork()
        {
            this.navigation1.Type.TypeKind().Should().Be(EdmTypeKind.Collection);
            this.navigation1.Type.AsCollection().ElementType().Definition.Should().BeSameAs(this.entityType);
        }

        [Fact]
        public void OnDeleteShouldContinueToWork()
        {
            this.navigation1.OnDelete.Should().Be(EdmOnDeleteAction.Cascade);
            this.navigation2.OnDelete.Should().Be(EdmOnDeleteAction.None);
        }

        [Fact]
        public void ReferentialConstraintShouldContinueToWork()
        {
            this.navigation1.DependentProperties().Should().BeNull();
            this.navigation2.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyId1") as IEdmStructuralProperty);
            this.navigation2.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyId2") as IEdmStructuralProperty);
        }

        [Fact]
        public void ReferentialConstraintShouldWorkForNonKeyPrincipalProperties()
        {
            this.nonKeyPrincipalNavigation.DependentProperties().Should().Contain(this.entityType.FindProperty("ForeignKeyProperty") as IEdmStructuralProperty);
        }

        [Fact]
        public void IsPrincipalShouldContinueToWork()
        {
            this.navigation1.IsPrincipal().Should().BeTrue();
            this.navigation2.IsPrincipal().Should().BeFalse();
        }

        [Fact]
        public void PartnerShouldContinueToWork()
        {
            this.navigation1.Partner.Should().BeSameAs(this.navigation2);
            this.navigation2.Partner.Should().BeSameAs(this.navigation1);
        }

        [Fact]
        public void ContainsTargetShouldContinueToWork()
        {
            this.navigation1.ContainsTarget.Should().BeTrue();
            this.navigation2.ContainsTarget.Should().BeFalse();
        }

        [Fact]
        public void NavigationTargetMappingsShouldContainAllBindings()
        {
            this.entitySet1.NavigationPropertyBindings.Should()
                .HaveCount(4)
                .And.Contain(m => m.NavigationProperty == this.navigation1 && m.Target == this.entitySet1)
                .And.Contain(m => m.NavigationProperty == this.navigation2 && m.Target == this.entitySet1)
                .And.Contain(m => m.NavigationProperty == this.derivedNavigation && m.Target == this.entitySet1);
            this.entitySet2.NavigationPropertyBindings.Should()
                .HaveCount(4)
                .And.Contain(m => m.NavigationProperty == this.navigation1 && m.Target == this.entitySet2)
                .And.Contain(m => m.NavigationProperty == this.navigation2 && m.Target == this.entitySet2)
                .And.Contain(m => m.NavigationProperty == this.derivedNavigation && m.Target == this.entitySet2);
        }

        [Fact]
        public void WriterShouldContinueToWork()
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                IEnumerable<EdmError> errors;
                CsdlWriter.TryWriteCsdl(this.representativeModel, writer, CsdlTarget.OData, out errors).Should().BeTrue();
                errors.Should().BeEmpty();
                writer.Flush();
            }

            string actual = builder.ToString();
            var actualXml = XElement.Parse(actual);
            var actualNormalized = actualXml.ToString();

            actualNormalized.Should().Be(RepresentativeEdmxDocument);
        }

        [Fact]
        public void ValidationShouldFailIfABindingToANonExistentPropertyIsFound()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""NonExistent"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("NonExistent", "Test.EntityType"));
        }

        [Fact]
        public void ValidationShouldFailIfABindingToANonExistentSetIsFound()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""NonExistent"" />",
                EdmErrorCode.BadUnresolvedEntitySet,
                ErrorStrings.Bad_UnresolvedEntitySet("NonExistent"));
        }

        [Fact]
        public void ValidationShouldFailIfAContainerQualifiedNameIsUsedForTheTargetOfABinding()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""Container.EntitySet"" />",
                EdmErrorCode.BadUnresolvedEntitySet,
                ErrorStrings.Bad_UnresolvedEntitySet("Container.EntitySet"));
        }

        [Fact]
        public void ValidationShouldFailIfADerivedPropertyIsUsedWithoutATypeCast()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""DerivedNavigation"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("DerivedNavigation", "Test.EntityType"));
        }

        [Fact]
        public void ValidationShouldFailIfATypeCastIsFollowedByANonExistentProperty()
        {
            this.ValidateBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Test.DerivedEntityType/NonExistent"" Target=""EntitySet"" />",
                EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                ErrorStrings.Bad_UnresolvedNavigationPropertyPath("Test.DerivedEntityType/NonExistent", "Test.EntityType"));
        }

        [Fact]
        public void ParsingShouldFailIfABindingIsMissingTarget()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Target", "NavigationPropertyBinding"));
        }

        [Fact]
        public void ParsingShouldFailIfABindingIsMissingPath()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Target=""EntitySet"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Path", "NavigationPropertyBinding"));
        }

        [Fact]
        public void ParsingShouldFailIfABindingHasExtraAttributes()
        {
            this.ParseBindingWithExpectedErrors(
                @"<NavigationPropertyBinding Path=""Navigation"" Target=""EntitySet"" Something=""else"" Foo=""bar"" />",
                EdmErrorCode.UnexpectedXmlAttribute,
                ErrorStrings.XmlParser_UnexpectedAttribute("Something"),
                ErrorStrings.XmlParser_UnexpectedAttribute("Foo"));
        }

        [Fact]
        public void ParsingShouldNotFailIfABindingHasAnnotations()
        {
            const string validBinding = @"
              <NavigationPropertyBinding Path=""Navigation"" Target=""EntitySet"">
                <Annotation Term=""FQ.NS.Term""/>
              </NavigationPropertyBinding>";

            this.ParseBindingWithExpectedErrors(validBinding);
        }

        [Fact]
        public void ParsingShouldFailIfAConstraintIsMissingProperty()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint ReferencedProperty=""ID1"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Property", "ReferentialConstraint"));
        }

        [Fact]
        public void ParsingShouldFailIfAConstraintIsMissingReferencedProperty()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""ForeignKeyId1"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("ReferencedProperty", "ReferentialConstraint"));
        }

        [Fact]
        public void ParsingShouldFailIfAConstraintHasExtraAttributes()
        {
            this.ParseReferentialConstraintWithExpectedErrors(
                @"
              <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"" Something=""else"" Foo=""bar"" />",
                EdmErrorCode.UnexpectedXmlAttribute,
                ErrorStrings.XmlParser_UnexpectedAttribute("Something"),
                ErrorStrings.XmlParser_UnexpectedAttribute("Foo"));
        }

        [Fact]
        public void ParsingShouldNotFailIfAConstraintHasAnnotations()
        {
            const string validConstraint = @"
              <ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""ID1"">
                <Annotation Term=""FQ.NS.Term""/>
              </ReferentialConstraint>";

            this.ParseReferentialConstraint(validConstraint);
        }

        [Fact]
        public void ValidationShouldFailIfAConstraintOnANonExistentPropertyIsFound()
        {
            this.ValidateReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""NonExistent"" ReferencedProperty=""ID1"" />",
                EdmErrorCode.BadUnresolvedProperty,
                ErrorStrings.Bad_UnresolvedProperty("NonExistent")
                );
        }

        [Fact]
        public void ValidationShouldFailIfAConstraintOnANonExistentReferencedPropertyIsFound()
        {
            this.ValidateReferentialConstraintWithExpectedErrors(
                @"<ReferentialConstraint Property=""ForeignKeyId1"" ReferencedProperty=""NonExistent"" />",
                EdmErrorCode.BadUnresolvedProperty,
                ErrorStrings.Bad_UnresolvedProperty("NonExistent"));
        }

        [Fact]
        public void ParsingShouldFailIfANavigationHasMultipleOnDeleteElements()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"">
                    <OnDelete Action=""Cascade"" />
                    <OnDelete Action=""None"" />
                  </NavigationProperty>",
                EdmErrorCode.UnexpectedXmlElement,
                ErrorStrings.XmlParser_UnusedElement("OnDelete"));
        }

        [Fact]
        public void ParsingShouldFailIfANavigationHasAnInvalidOnDeleteAction()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"">
                    <OnDelete Action=""Foo"" />
                  </NavigationProperty>",
                EdmErrorCode.InvalidOnDelete,
                ErrorStrings.CsdlParser_InvalidDeleteAction("Foo"));
        }

        [Fact]
        public void ParsingShouldFailIfANavigationIsMissingType()
        {
            this.ParseNavigationExpectedErrors(
                @"<NavigationProperty Name=""Navigation"" />",
                EdmErrorCode.MissingAttribute,
                ErrorStrings.XmlParser_MissingAttribute("Type", "NavigationProperty"));
        }

        [Fact]
        public void ParsingShouldNotFailIfANavigationIsMissingPartner()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" />");
        }

        [Fact]
        public void ParsingShouldFailIfNavigationTypeIsEmpty()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type="""" />",
                EdmErrorCode.InvalidTypeName,
                ErrorStrings.CsdlParser_InvalidTypeName(""));
        }

        [Fact]
        public void ParsingShouldFailIfNavigationNullableIsEmpty()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""""/>",
                EdmErrorCode.InvalidBoolean,
                ErrorStrings.ValueParser_InvalidBoolean(""));
        }

        [Fact]
        public void ParsingShouldFailIfNavigationNullableIsNotTrueOrFalse()
        {
            this.ParseNavigationExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""foo""/>",
                EdmErrorCode.InvalidBoolean,
                ErrorStrings.ValueParser_InvalidBoolean("foo"));
        }

        [Fact]
        public void ValidationShouldFailIfNavigationNullableIsSpecifiedOnCollection()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" Nullable=""false""/>",
                EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute,
                ErrorStrings.CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType);
        }

        [Fact]
        public void ValidationShouldFailIfNavigationTypeIsAPrimitiveType()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Edm.Int32"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Edm.Int32"));
        }

        [Fact]
        public void ValidationShouldFailIfNavigationTypeIsPrimitiveCollectionType()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Edm.Int32)"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Edm.Int32"));
        }

        [Fact]
        public void ValidationShouldFailIfNavigationTypeDoesNotExist()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Fake.Nonexistent"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Fake.Nonexistent"));
        }

        [Fact]
        public void ValidationShouldFailIfNavigationTypeIsACollectionButElementTypeDoesNotExist()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Collection(Fake.Nonexistent)"" />",
                EdmErrorCode.BadUnresolvedEntityType,
                ErrorStrings.Bad_UnresolvedEntityType("Fake.Nonexistent"));
        }

        [Fact]
        public void ValidationShouldFailIfNavigationParterIsSpecifiedButCannotBeFound()
        {
            this.ValidateNavigationWithExpectedErrors(@"<NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Partner=""Nonexistent"" />",
                new[]
                {
                    EdmErrorCode.BadUnresolvedNavigationPropertyPath,
                    EdmErrorCode.UnresolvedNavigationPropertyPartnerPath
                },
                new[]
                {
                    ErrorStrings.Bad_UnresolvedNavigationPropertyPath("Nonexistent", "Test.EntityType"),
                    string.Format("Cannot resolve partner path for navigation property '{0}'.", "Navigation")
                });
        }

        [Fact]
        public void ValidationShouldFailIfEnumMemberIsSpecifiedButCannotBeFound()
        {
            IEdmModel model = GetEdmModel(@"<EnumMember>TestNS2.UnknownColor/Blue</EnumMember>");
            IEnumerable<EdmError> errors;
            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(1);
            errors.Should().Contain(e => e.ErrorCode == EdmErrorCode.BadUnresolvedEnumMember && e.ErrorMessage == ErrorStrings.Bad_UnresolvedEnumMember("Blue"));
        }

        [Fact]
        public void ValidationShouldFailIfEnumMemberIsSpecifiedButCannotBeFoundTheMember()
        {
            IEdmModel model = GetEdmModel(@"<EnumMember>TestNS2.Color/UnknownMember</EnumMember>");
            IEnumerable<EdmError> errors;
            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(2);
            errors.Should().Contain(e => e.ErrorCode == EdmErrorCode.InvalidEnumMemberPath &&
            e.ErrorMessage == ErrorStrings.CsdlParser_InvalidEnumMemberPath("TestNS2.Color/UnknownMember"));
        }

        [Fact]
        public void ValidationShouldSuccessIfEnumMemberIsSpecifiedWithCorrectType()
        {
            IEdmModel model = GetEdmModel(@"<EnumMember>TestNS2.Color/Blue</EnumMember>");
            IEnumerable<EdmError> errors;
            model.Validate(out errors).Should().BeTrue();
        }

        private IEdmModel GetEdmModel(string bindingText)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Nullable=""false"" Type=""Edm.Int32""/>
        <Annotation Term=""TestNS.OutColor"">
          {0}
        </Annotation>
      </EntityType>
      <Term Name=""OutColor"" Type=""TestNS2.Color"" />
    </Schema>
    <Schema Namespace=""TestNS2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""Color"" IsFlags=""true"">
        <Member Name=""Cyan"" Value=""1"" />
        <Member Name=""Blue"" Value=""2"" />
      </EnumType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, bindingText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();
            return model;
        }

        private void ValidateBindingWithExpectedErrors(string bindingText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet"" EntityType=""Test.EntityType"">
          {0}
        </EntitySet>
      </EntityContainer>
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Nullable=""false"" Type=""Edm.Int32""/>
        <NavigationProperty Name=""Navigation"" Type=""Collection(Test.EntityType)"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, bindingText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();

            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ValidateReferentialConstraintWithExpectedErrors(string referentialConstraintText, EdmErrorCode errorCode, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""true"">
          {0}
        </NavigationProperty>
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, referentialConstraintText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();

            model.Validate(out errors).Should().BeFalse();
            errors.Should().HaveCount(messages.Length);
            foreach (var message in messages)
            {
                errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
            }
        }

        private void ValidateNavigationWithExpectedErrors(string navigationText, EdmErrorCode? errorCode = null, string message = null)
        {
            if (errorCode != null)
            {
                ValidateNavigationWithExpectedErrors(navigationText, new[] { errorCode.Value }, new[] { message });
            }
            else
            {
                ValidateNavigationWithExpectedErrors(navigationText, new EdmErrorCode[0], new string[0]);
            }
        }

        private void ValidateNavigationWithExpectedErrors(string navigationText, EdmErrorCode[] errorCodes, string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {0}
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, navigationText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors).Should().BeTrue();

            bool result = model.Validate(out errors);

            if (errorCodes.Length > 0)
            {
                result.Should().BeFalse();

                errors.Should().HaveCount(messages.Length);
                for (int i = 0; i < messages.Length; i++)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCodes[i] && e.ErrorMessage == messages[i]);
                }
            }
            else
            {
                result.Should().BeTrue();
                errors.Should().BeEmpty();
            }
        }

        private void ParseBindingWithExpectedErrors(string bindingText, EdmErrorCode? errorCode = null, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""EntitySet"" EntityType=""Test.EntityType"">
          {0}
        </EntitySet>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, bindingText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            if (errorCode != null)
            {
                result.Should().BeFalse();
                errors.Should().HaveCount(messages.Length);
                foreach (var message in messages)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
                }
            }
        }

        private void ParseReferentialConstraint(string referentialConstraintText, EdmErrorCode? errorCode = null, params string[] messages)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ForeignKeyId1"" Type=""Edm.Int32"" />
        <Property Name=""ForeignKeyId2"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""Navigation"" Type=""Test.EntityType"" Nullable=""true"">
          {0}
        </NavigationProperty>
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, referentialConstraintText);

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            if (errorCode != null)
            {
                result.Should().BeFalse();
                errors.Should().HaveCount(messages.Length);
                foreach (var message in messages)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCode && e.ErrorMessage == message);
                }
            }
        }

        private void ParseReferentialConstraintWithExpectedErrors(string referentialConstraintText, EdmErrorCode errorCode, params string[] messages)
        {
            ParseReferentialConstraint(referentialConstraintText, errorCode, messages);
        }

        private void ParseNavigationExpectedErrors(string navigationText, EdmErrorCode[] errorCodes, string[] messages)
        {
            errorCodes.Length.Should().Be(messages.Length);
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {0}
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, navigationText);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            if (errorCodes.Length > 0)
            {
                result.Should().BeFalse();

                errors.Should().HaveCount(messages.Length);
                for (int i = 0; i < messages.Length; i++)
                {
                    errors.Should().Contain(e => e.ErrorCode == errorCodes[i] && e.ErrorMessage == messages[i]);
                }
            }
            else
            {
                result.Should().BeTrue();
                errors.Should().BeEmpty();
            }
        }

        private void ParseNavigationExpectedErrors(string navigationText, EdmErrorCode? errorCode = null, string message = null)
        {
            if (errorCode != null)
            {
                ParseNavigationExpectedErrors(navigationText, new[] { errorCode.Value }, new[] { message });
            }
            else
            {
                ParseNavigationExpectedErrors(navigationText, new EdmErrorCode[0], new string[0]);
            }
        }
    }
}
