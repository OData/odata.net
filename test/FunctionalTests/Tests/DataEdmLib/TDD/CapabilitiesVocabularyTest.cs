//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.CsdlSemantics;

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
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
  <ComplexType Name=""CallbackType"">
    <Property Name=""CallbackProtocols"" Type=""Collection(Capabilities.CallbackProtocol)"" />
    <Annotation Term=""Core.Description"" String=""A non-empty collection lists the full set of supported protocols. A empty collection means 'only HTTP is supported'"" />
  </ComplexType>
  <ComplexType Name=""CallbackProtocol"">
    <Property Name=""Id"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Protcol Identifier"" />
    </Property>
    <Property Name=""UrlTemplate"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""URL Template including parameters. Parameters are enclosed in curly braces {} as defined in RFC6570"" />
    </Property>
    <Property Name=""DocumentationUrl"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Human readable description of the meaning of the URL Template parameters"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
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
  <ComplexType Name=""SearchRestrictionsType"">
    <Property Name=""Searchable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""$search is supported"" />
    </Property>
    <Property Name=""UnsupportedExpressions"" Type=""Capabilities.SearchExpressions"" DefaultValue=""none"">
      <Annotation Term=""Core.Description"" String=""Expressions supported in $search"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""InsertRestrictionsType"">
    <Property Name=""Insertable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""Entities can be inserted"" />
    </Property>
    <Property Name=""NonInsertableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow deep inserts"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""UpdateRestrictionsType"">
    <Property Name=""Updatable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""Entities can be updated"" />
    </Property>
    <Property Name=""NonUpdatableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow rebinding"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""DeleteRestrictionsType"">
    <Property Name=""Deletable"" Type=""Edm.Boolean"" DefaultValue=""true"">
      <Annotation Term=""Core.Description"" String=""Entities can be deleted"" />
    </Property>
    <Property Name=""NonDeletableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow DeleteLink requests"" />
    </Property>
  </ComplexType>
  <EnumType Name=""ConformanceLevelType"">
    <Member Name=""Minimal"" />
    <Member Name=""Intermediate"" />
    <Member Name=""Advanced"" />
  </EnumType>
  <EnumType Name=""IsolationLevel"" IsFlags=""true"">
    <Member Name=""Snapshot"" Value=""1"" />
  </EnumType>
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
  <EnumType Name=""SearchExpressions"" IsFlags=""true"">
    <Member Name=""none"" Value=""0"" />
    <Member Name=""AND"" Value=""1"" />
    <Member Name=""OR"" Value=""2"" />
    <Member Name=""NOT"" Value=""4"" />
    <Member Name=""phrase"" Value=""8"" />
    <Member Name=""group"" Value=""16"" />
  </EnumType>
  <Term Name=""ConformanceLevel"" Type=""Capabilities.ConformanceLevelType"" AppliesTo=""EntityContainer"" />
  <Term Name=""SupportedFormats"" Type=""Collection(Edm.String)"">
    <Annotation Term=""Core.Description"" String=""Media types of supported formats, including format parameters"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
  </Term>
  <Term Name=""AcceptableEncodings"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""List of acceptable compression methods for ($batch) requests, e.g. gzip"" />
  </Term>
  <Term Name=""AsynchronousRequestsSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Service supports the asynchronous request preference"" />
  </Term>
  <Term Name=""BatchContinueOnErrorSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Service supports the continue on error preference"" />
  </Term>
  <Term Name=""IsolationSupported"" Type=""Capabilities.IsolationLevel"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Supported odata.isolation levels"" />
  </Term>
  <Term Name=""CallbackSupported"" Type=""Capabilities.CallbackType"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""Supports callbacks for the specified protocols"" />
  </Term>
  <Term Name=""CrossJoinSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Supports cross joins for the entity sets in this container"" />
  </Term>
  <Term Name=""ChangeTracking"" Type=""Capabilities.ChangeTrackingType"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""Change tracking capabilities of this service or entity set"" />
  </Term>
  <Term Name=""CountRestrictions"" Type=""Capabilities.CountRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on /$count path suffix and $count=true system query option"" />
  </Term>
  <Term Name=""NavigationRestrictions"" Type=""Capabilities.NavigationRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on navigating properties according to OData URL conventions"" />
  </Term>
  <Term Name=""IndexableByKey"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Supports key values according to OData URL conventions"" />
  </Term>
  <Term Name=""TopSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Supports $top"" />
  </Term>
  <Term Name=""SkipSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Supports $skip"" />
  </Term>
  <Term Name=""BatchSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Supports $batch requests"" />
  </Term>
  <Term Name=""FilterFunctions"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""List of functions supported in $filter"" />
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
  <Term Name=""SearchRestrictions"" Type=""Capabilities.SearchRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on $search expressions"" />
  </Term>
  <Term Name=""InsertRestrictions"" Type=""Capabilities.InsertRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on insert operations"" />
  </Term>
  <Term Name=""UpdateRestrictions"" Type=""Capabilities.UpdateRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on update operations"" />
  </Term>
  <Term Name=""DeleteRestrictions"" Type=""Capabilities.DeleteRestrictionsType"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Restrictions on delete operations"" />
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
        public void TestCapabilitiesVocabularyReferenceCoreVocabularyTypesAndTerms()
        {
            foreach (string name in new[] { "AsynchronousRequestsSupported", "BatchContinueOnErrorSupported" })
            {
                var term = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1." + name);
                Assert.IsNotNull(term);

                // Core.Tag
                Assert.IsNotNull(term.Type);

                Assert.AreEqual(EdmTypeKind.TypeDefinition, term.Type.Definition.TypeKind);
                Assert.AreEqual("Org.OData.Core.V1.Tag", term.Type.Definition.FullTypeName());

                // Core.Description
                var annotations = this.capVocModel.FindDeclaredVocabularyAnnotations(term).ToList();
                Assert.AreEqual(1, annotations.Count());
                var description = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsValueTerm && a.Term.Name == "Description");
                Assert.IsNotNull(description);
                Assert.AreEqual("Org.OData.Core.V1", description.Term.Namespace);
            }
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyReferenceMultiCoreVocabularyTerms()
        {
            var supportedFormats = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.SupportedFormats");
            Assert.IsNotNull(supportedFormats);

            var annotations = this.capVocModel.FindDeclaredVocabularyAnnotations(supportedFormats).ToList();
            Assert.AreEqual(2, annotations.Count());

            // Core.Description
            var description = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsValueTerm && a.Term.Name == "Description");
            Assert.IsNotNull(description);
            Assert.AreEqual("Org.OData.Core.V1", description.Term.Namespace);

            // Core.IsMediaType
            var isMediaType = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsValueTerm && a.Term.Name == "IsMediaType");
            Assert.IsNotNull(isMediaType);
            Assert.AreEqual("Org.OData.Core.V1", isMediaType.Term.Namespace);
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
            var countTerm = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.CountRestrictions");
            Assert.IsNotNull(countTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", countTerm.Namespace);
            Assert.AreEqual("CountRestrictions", countTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, countTerm.TermKind);

            var type = countTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.CountRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Countable");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("NonCountableProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("NonCountableNavigationProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyNavigationRestrictions()
        {
            var navigationTerm = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.NavigationRestrictions");
            Assert.IsNotNull(navigationTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", navigationTerm.Namespace);
            Assert.AreEqual("NavigationRestrictions", navigationTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, navigationTerm.TermKind);

            var type = navigationTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.NavigationRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Navigability");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Enum, p.Type.Definition.TypeKind);

            var navType = p.Type.Definition as IEdmEnumType;
            Assert.IsNotNull(navType);
            Assert.AreEqual(3, navType.Members.Count());
            Assert.AreEqual("Recursive|Single|None", String.Join("|", navType.Members.Select(e => e.Name)));

            p = complexType.FindProperty("RestrictedProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            IEdmCollectionType collection = p.Type.Definition as IEdmCollectionType;
            Assert.IsNotNull(collection);
            var propertyResType = collection.ElementType.Definition as IEdmComplexType;
            Assert.IsNotNull(propertyResType);
            Assert.AreEqual("Org.OData.Capabilities.V1.NavigationPropertyRestriction", propertyResType.FullName());

            p = propertyResType.FindProperty("NavigationProperty");
            Assert.IsNotNull(p);

            p = propertyResType.FindProperty("Navigability");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Enum, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyFilterRestrictions()
        {
            var filterTerm = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.FilterRestrictions");
            Assert.IsNotNull(filterTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", filterTerm.Namespace);
            Assert.AreEqual("FilterRestrictions", filterTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, filterTerm.TermKind);

            var type = filterTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.FilterRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Filterable");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("RequiresFilter");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("RequiredProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("NonFilterableProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularySortRestrictions()
        {
            var sortTerm = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.SortRestrictions");
            Assert.IsNotNull(sortTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", sortTerm.Namespace);
            Assert.AreEqual("SortRestrictions", sortTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, sortTerm.TermKind);

            var type = sortTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.SortRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Sortable");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("AscendingOnlyProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("DescendingOnlyProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("NonSortableProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyExpandRestrictions()
        {
            var expandTerm = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.ExpandRestrictions");
            Assert.IsNotNull(expandTerm);
            Assert.AreEqual("Org.OData.Capabilities.V1", expandTerm.Namespace);
            Assert.AreEqual("ExpandRestrictions", expandTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, expandTerm.TermKind);

            var type = expandTerm.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.ExpandRestrictionsType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Expandable");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("NonExpandableProperties");
            Assert.IsNotNull(p);
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [TestMethod]
        public void TestCapabilitiesVocabularyConformanceLevel()
        {
            var confLevel = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.ConformanceLevel");
            Assert.IsNotNull(confLevel);
            Assert.AreEqual("Org.OData.Capabilities.V1", confLevel.Namespace);
            Assert.AreEqual("ConformanceLevel", confLevel.Name);
            Assert.AreEqual(EdmTermKind.Value, confLevel.TermKind);

            var type = confLevel.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.ConformanceLevelType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Enum, type.Definition.TypeKind);

            var enumType = type.Definition as IEdmEnumType;
            Assert.IsNotNull(enumType);
            Assert.AreEqual(3, enumType.Members.Count());
            Assert.AreEqual("Minimal|Intermediate|Advanced", String.Join("|", enumType.Members.Select(e => e.Name)));
        }

        [TestMethod]
        public void TestCapabilitiesVocabularySupportedFormats()
        {
            var supportedFormats = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.SupportedFormats");
            Assert.IsNotNull(supportedFormats);
            Assert.AreEqual("Org.OData.Capabilities.V1", supportedFormats.Namespace);
            Assert.AreEqual("SupportedFormats", supportedFormats.Name);
            Assert.AreEqual(EdmTermKind.Value, supportedFormats.TermKind);

            var type = supportedFormats.Type;
            Assert.AreEqual("Collection(Edm.String)", type.FullName());
        }
    }
}
