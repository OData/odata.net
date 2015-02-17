//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
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
  <Term Name=""ChangeTracking"" Type=""Capabilities.ChangeTrackingType"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""Change tracking capabilities of this service or entity set"" />
  </Term>
</Schema>";

            var s = this.capVocModel.FindDeclaredValueTerm("Org.OData.Capabilities.V1.ChangeTracking");
            Assert.IsNotNull(s);
            Assert.AreEqual("Org.OData.Capabilities.V1", s.Namespace);
            Assert.AreEqual("ChangeTracking", s.Name);
            Assert.AreEqual(EdmTermKind.Value, s.TermKind);

            var type = s.Type;
            Assert.AreEqual("Org.OData.Capabilities.V1.ChangeTrackingType", type.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.IsNotNull(complexType);
            var p = complexType.FindProperty("Supported");
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("FilterableProperties");
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("ExpandableProperties");
            Assert.AreEqual(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

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
    }
}
