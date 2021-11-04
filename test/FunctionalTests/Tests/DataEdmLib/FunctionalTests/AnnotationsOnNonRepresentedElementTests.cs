//---------------------------------------------------------------------
// <copyright file="AnnotationsOnNonRepresentedElementTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotationsOnNonRepresentedElementTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ValidateAnnotationAttributeMustNotCollide()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"" Bogus:PropertyAnnotationAttribute=""Just kidding"" Bogus:PropertyAnnotationAttribute=""Just kidding"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Bogus:EntityTypeAnnotation Stuff=""Whatever"" />
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "Should fail to parse.");
            Assert.IsTrue(errors.Count() != 0, "Should report a failure");
        }

        [TestMethod]
        public void ValidateKeyAnnotationElementSupportInV40()
        {
            // [EdmLib] Key, ReturnType, Schema, Principle, Dependent, OnDelete do not support annotations. - won't fixed. 
            var expectedErrors = new EdmLibTestErrors();
            this.VerifySemanticValidation(ValidationTestModelBuilder.KeyAnnotationElementSupportInV40(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateEntityContainerAnnotationElementSupportInV40()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { 5, 6, EdmErrorCode.DuplicateDirectValueAnnotationFullName}, 
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityContainerAnnotationElementSupportInV40(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateAnnotationElementFullNameShouldBeUnique()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicateDirectValueAnnotationFullName}
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.AnnotationElementFullNameShouldBeUnique(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        public void ValidatePropertyRefAnnotationElementSupportInV40()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                // [EdmLib] Key, ReturnType, Schema, Principle, Dependent, OnDelete do not support annotations. - won't fixed. 
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.PropertyRefAnnotationElementSupportInV40(this.EdmVersion), expectedErrors);
        }

        [TestMethod]
        // [EdmLib] [Validator] Add more version related semantic rules for annotation elements. - postponed
        public void ValidateAnnotationElementRelatedRulesInV12OrLower()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
                // TODO: Need to update after the attached bug is fixed. 
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationImportAnnotationElementSupportInV40(EdmVersion.V40), EdmVersion.V40, expectedErrors);
            expectedErrors = new EdmLibTestErrors()
            {
                // TODO: Need to update after the attached bug is fixed. 
                { 5, 6, EdmErrorCode.DuplicateDirectValueAnnotationFullName }
            };
            this.VerifySemanticValidation(ValidationTestModelBuilder.EntityContainerAnnotationElementSupportInV40(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

        [TestMethod]
        public void ValidateFunctionImportAnnotationElementSupportInV40()
        {
            var expectedErrors = new EdmLibTestErrors()
            {
            };

            this.VerifySemanticValidation(ValidationTestModelBuilder.OperationImportAnnotationElementSupportInV40(EdmVersion.V40), EdmVersion.V40, expectedErrors);
        }

    }
}
