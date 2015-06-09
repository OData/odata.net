//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies.V1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test capabilities vocabulary
    /// </summary>
    [TestClass]
    public class CapabilitiesVocabularyTest
    {
        private readonly IEdmModel capVocModel = CapabilitiesVocabularyModel.Instance;

        [TestMethod]
        public void TestCapabilitiesVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Capabilities.V1"" Alias=""Capabilities"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ChangeTrackingType"">
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""This entity set supports the odata.track-changes preference"" />
    </Property>
    <Property Name=""FilterableProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""Change tracking supports filters on these properties"" />
    </Property>
    <Property Name=""ExpandableProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""Change tracking supports these properties expanded"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""CountRestrictionsType"">
    <Property Name=""Countable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""Entities can be counted"" />
    </Property>
    <Property Name=""NonCountableProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These collection properties do not allow /$count segments"" />
    </Property>
    <Property Name=""NonCountableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow /$count segments"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""NavigationRestrictionsType"">
    <Property Name=""Navigability"" Type=""Capabilities.NavigationType"">
      <Annotation Term=""Core.Description"" String=""Supported Navigability"" />
    </Property>
    <Property Name=""RestrictedProperties"" Type=""Collection(Capabilities.NavigationPropertyRestriction)"" />
  </ComplexType>
  <ComplexType Name=""NavigationPropertyRestriction"">
    <Property Name=""NavigationProperty"" Type=""Edm.NavigationPropertyPath"">
      <Annotation Term=""Core.Description"" String=""Navigation properties can be navigated"" />
    </Property>
    <Property Name=""Navigability"" Type=""Capabilities.NavigationType"">
      <Annotation Term=""Core.Description"" String=""Navigation properties can be navigated to this level"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""FilterRestrictionsType"">
    <Property Name=""Filterable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""$filter is supported"" />
    </Property>
    <Property Name=""RequiresFilter"" Type=""Edm.Boolean"">
      <Annotation Term=""Core.Description"" String=""$filter is required"" />
    </Property>
    <Property Name=""RequiredProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties must be specified in the $filter clause (properties of derived types are not allowed here)"" />
    </Property>
    <Property Name=""NonFilterableProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties cannot be used in $filter expressions"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""SortRestrictionsType"">
    <Property Name=""Sortable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""$orderby is supported"" />
    </Property>
    <Property Name=""AscendingOnlyProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties can only be used for sorting in Ascending order"" />
    </Property>
    <Property Name=""DescendingOnlyProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties can only be used for sorting in Descending order"" />
    </Property>
    <Property Name=""NonSortableProperties"" Type=""Collection(Edm.PropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties cannot be used in $orderby expressions"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ExpandRestrictionsType"">
    <Property Name=""Expandable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""$expand is supported"" />
    </Property>
    <Property Name=""NonExpandableProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These properties cannot be used in $expand expressions"" />
    </Property>
  </ComplexType>
  <EnumType Name=""NavigationType"">
    <Member Name=""Recursive"">
      <Annotation Term=""Core.Description"" String=""Navigation properties can be recursively navigated"" />
    </Member>
    <Member Name=""Single"">
      <Annotation Term=""Core.Description"" String=""Navigation properties can be navigated to a single level"" />
    </Member>
    <Member Name=""None"">
      <Annotation Term=""Core.Description"" String=""Navigation properties are not navigable"" />
    </Member>
  </EnumType>
  <Term Name=""ChangeTracking"" Type=""Capabilities.ChangeTrackingType"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""Change tracking capabilities of this service or entity set"" />
  </Term>
  <Term Name=""CountRestrictions"" Type=""Capabilities.CountRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on /$count path suffix and $count=true system query option"" />
  </Term>
  <Term Name=""NavigationRestrictions"" Type=""Capabilities.NavigationRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on navigating properties according to OData URL conventions"" />
  </Term>
  <Term Name=""FilterRestrictions"" Type=""Capabilities.FilterRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on $filter expressions"" />
  </Term>
  <Term Name=""SortRestrictions"" Type=""Capabilities.SortRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on $orderby expressions"" />
  </Term>
  <Term Name=""ExpandRestrictions"" Type=""Capabilities.ExpandRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on $expand expressions"" />
  </Term>
</Schema>";

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            IEnumerable<EdmError> errors;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            this.capVocModel.TryWriteCsdl(xw, out errors);
            xw.Flush();
            xw.Close();
            string output = sw.ToString();
            Assert.IsTrue(!errors.Any(), "No Errors");
            Assert.AreEqual(expectedText, output, "expectedText = output");
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyChangeTracking()
        {
            var changeTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ChangeTracking);
            Assert.IsNotNull(changeTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", changeTerm.Namespace);
            Assert.AreEqual("ChangeTracking", changeTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, changeTerm.TermKind);

            var type = changeTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.ChangeTrackingType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingSupported);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingFilterableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingExpandableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyCountRestrictions()
        {
            var countTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.CountRestrictions);
            Assert.IsNotNull(countTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", countTerm.Namespace);
            Assert.AreEqual("CountRestrictions", countTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, countTerm.TermKind);

            var type = countTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.CountRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.CountRestrictionsCountable);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.CountRestrictionsNonCountableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.CountRestrictionsNonCountableNavigationProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyNavigationRestrictions()
        {
            var navigationTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.NavigationRestrictions);
            Assert.IsNotNull(navigationTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", navigationTerm.Namespace);
            Assert.AreEqual("NavigationRestrictions", navigationTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, navigationTerm.TermKind);

            var type = navigationTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.NavigationRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.NavigationRestrictionsNavigability);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Enum, p.Type.Definition.TypeKind);

            var navType = p.Type.Definition as IEdmEnumType;
            Assert.AreSame(navType, CapabilitiesVocabularyModel.NavigationTypeType);
            Assert.IsNotNull(navType);
            Assert.AreEqual(3, navType.Members.Count());
            Assert.AreEqual("Recursive|Single|None", String.Join("|", navType.Members.Select(e => e.Name)));

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.NavigationRestrictionsRestrictedProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            IEdmCollectionType collection = p.Type.Definition as IEdmCollectionType;
            var propertyResType = collection.ElementType.Definition as IEdmComplexType;
            Assert.IsNotNull(propertyResType);
            Assert.AreEqual("Org.OData.Capabilities.V1.NavigationPropertyRestriction", propertyResType.FullName());

            p = propertyResType.FindProperty(CapabilitiesVocabularyConstants.NavigationPropertyRestrictionNavigationProperty);
            Assert.IsNotNull(p);

            p = propertyResType.FindProperty(CapabilitiesVocabularyConstants.NavigationRestrictionsNavigability);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Enum, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyFilterRestrictions()
        {
            var filterTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.FilterRestrictions);
            Assert.IsNotNull(filterTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", filterTerm.Namespace);
            Assert.AreEqual("FilterRestrictions", filterTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, filterTerm.TermKind);

            var type = filterTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.FilterRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.FilterRestrictionsFilterable);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.FilterRestrictionsRequiresFilter);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.FilterRestrictionsRequiredProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.FilterRestrictionsNonFilterableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularySortRestrictions()
        {
            var sortTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.SortRestrictions);
            Assert.IsNotNull(sortTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", sortTerm.Namespace);
            Assert.AreEqual("SortRestrictions", sortTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, sortTerm.TermKind);

            var type = sortTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.SortRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.SortRestrictionsSortable);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.SortRestrictionsAscendingOnlyProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.SortRestrictionsDescendingOnlyProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.SortRestrictionsNonSortableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyExpandRestrictions()
        {
            var expandTerm = this.capVocModel.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ExpandRestrictions);
            Assert.IsNotNull(expandTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", expandTerm.Namespace);
            Assert.AreEqual("ExpandRestrictions", expandTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, expandTerm.TermKind);

            var type = expandTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.ExpandRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.ExpandRestrictionsExpandable);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.ExpandRestrictionsNonExpandableProperties);
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }
    }
}
