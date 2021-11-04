//---------------------------------------------------------------------
// <copyright file="VocabularyModelComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EdmLibTests.StubEdm;
using EdmLibTests.VocabularyStubs;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    [TestClass]
    public class VocabularyModelComparerTests
    {
        private StubEdmModel definitionModel;

        private StubEdmModel expectedModel;
        private StubEdmModel actualModel;
        private VocabularyModelComparer comparer;

        [TestInitialize]
        public void Initialize()
        {
            this.definitionModel = this.CreateDefinitionModel();

            this.expectedModel = this.CreateApplicationModel();
            this.actualModel = this.CreateApplicationModel();

            this.comparer = new VocabularyModelComparer();
        }

        [TestMethod]
        public void TheSameModel_Should_Succeed()
        {
            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreEqual(0, errors.Count, "Should not have errors!");
        }

        [TestMethod]
        public void Term_Count_Not_Match_Should_Error()
        {
            this.actualModel.Add(new StubTerm("", ""));

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("Wrong ValueTerm count")));
        }

        [TestMethod]
        public void Term_Name_Not_Match_Should_Error()
        {
            this.actualModel.SchemaElements.OfType<StubTerm>().First().Name = "_non_exist_";

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("Cannot find ValueTerm")));
        }

        [TestMethod]
        public void Term_Type_Nullable_Not_Match_Should_Error()
        {
            IEdmTypeReference nonMatchTypeReference = EdmCoreModel.Instance.GetInt32(false);
            this.actualModel.SchemaElements.OfType<StubTerm>().First().Type = nonMatchTypeReference;

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("TypeReference")));
            Assert.IsTrue(errors.Any(e => e.Contains("IsNullable")));
        }

        [TestMethod]
        public void Term_Type_Not_Match_Should_Error()
        {
            IEdmTypeReference nonMatchTypeReference = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
            this.actualModel.SchemaElements.OfType<StubTerm>().First().Type = nonMatchTypeReference;

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("TypeReference")));
        }

        [TestMethod]
        public void Term_Annotation_Count_Not_Match_Should_Error()
        {
            var bazValueTerm = this.definitionModel.SchemaElements.OfType<IEdmTerm>().FirstOrDefault(t => t.Name == "baz");
            var valueAnnotation = new StubVocabularyAnnotation() { Term = bazValueTerm, Value = new StubStringConstantExpression("zzz") };
            this.actualModel.SchemaElements.OfType<StubTerm>().First().AddVocabularyAnnotation(valueAnnotation);

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("Wrong TermAnnotations count")));
        }

        [TestMethod]
        public void Term_Annotation_Term_Not_Match_Should_Error()
        {
            var barValueTerm = this.definitionModel.SchemaElements.OfType<IEdmTerm>().FirstOrDefault(t => t.Name == "bar");
            var bazValueTerm = this.definitionModel.SchemaElements.OfType<IEdmTerm>().FirstOrDefault(t => t.Name == "baz");
            var valueAnnotation = new StubVocabularyAnnotation() { Term = bazValueTerm, Value = new StubStringConstantExpression("zzz") };

            var target = this.actualModel.SchemaElements.OfType<StubTerm>().First();
            target.RemoveAnnotationsForTerm(barValueTerm);
            target.AddVocabularyAnnotation(valueAnnotation);

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("Cannot find TermAnnotation")));
        }

        [TestMethod]
        public void Term_Annotation_Value_Not_Match_Should_Error()
        {
            var barValueTerm = this.definitionModel.SchemaElements.OfType<IEdmTerm>().FirstOrDefault(t => t.Name == "bar");
            var valueAnnotation = new StubVocabularyAnnotation() { Term = barValueTerm, Value = new StubStringConstantExpression("_not_exist_") };

            var target = this.actualModel.SchemaElements.OfType<StubTerm>().First();
            target.RemoveAnnotationsForTerm(barValueTerm);
            target.AddVocabularyAnnotation(valueAnnotation);

            var errors = this.comparer.CompareModels(this.expectedModel, this.actualModel);
            this.DumpErrors(errors);

            Assert.AreNotEqual(0, errors.Count, "Should have error!");
            Assert.IsTrue(errors.Any(e => e.Contains("Value expression mismatch")));
        }

        private void DumpErrors(IEnumerable<string> errors)
        {
            errors.ToList().ForEach(e => Console.WriteLine(e));
        }

        private StubEdmModel CreateDefinitionModel()
        {
            var model = new StubEdmModel();

            var barValueTerm = new StubTerm("", "bar") { Type = EdmCoreModel.Instance.GetInt32(true) };
            model.Add(barValueTerm);

            var bazValueTerm = new StubTerm("", "baz") { Type = EdmCoreModel.Instance.GetString(true) };
            model.Add(bazValueTerm);

            var p1 = new StubEdmStructuralProperty("p1") { Type = EdmCoreModel.Instance.GetString(true) };
            var p2 = new StubEdmStructuralProperty("p2") { Type = EdmCoreModel.Instance.GetString(true) };
            var foobazTypeTerm = new StubTypeTerm("", "foobaz") { p1, p2 };
            model.Add(foobazTypeTerm);

            var p10 = new StubEdmStructuralProperty("p10") { Type = EdmCoreModel.Instance.GetString(true) };
            var bazfooTypeTerm = new StubTypeTerm("", "bazfoo") { p10 };
            model.Add(bazfooTypeTerm);

            return model;
        }

        private StubEdmModel CreateApplicationModel()
        {
            var model = new StubEdmModel();

            var valueTerm = new StubTerm("", "foo") { Type = EdmCoreModel.Instance.GetInt32(true) };
            model.Add(valueTerm);

            var barValueTerm = this.definitionModel.SchemaElements.OfType<IEdmTerm>().FirstOrDefault(t => t.Name == "bar");
            var valueAnnotation = new StubVocabularyAnnotation() { Term = barValueTerm, Value = new StubStringConstantExpression("zzz") };
            valueTerm.AddVocabularyAnnotation(valueAnnotation);

            return model;
        }
    }
}
