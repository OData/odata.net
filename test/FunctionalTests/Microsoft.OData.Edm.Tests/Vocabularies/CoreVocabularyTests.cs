//---------------------------------------------------------------------
// <copyright file="CoreVocabularyTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    /// <summary>
    /// Test core vocabulary
    /// </summary>
    public class CoreVocabularyTests
    {
        private readonly IEdmModel coreVocModel = CoreVocabularyModel.Instance;

        [Fact]
        public void TestBaseCoreVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Core.V1"" Alias=""Core"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Tag"" UnderlyingType=""Edm.Boolean"">
    <Annotation Term=""Core.Description"" String=""This is the type to use for all tagging terms"" />
  </TypeDefinition>
  <ComplexType Name=""OptionalParameterType"">
    <Property Name=""DefaultValue"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Default value for an optional parameter, using the same rules for the default value facet of a property."" />
    </Property>
  </ComplexType>
  <EnumType Name=""Permission"" IsFlags=""true"">
    <Member Name=""None"" Value=""0"" />
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""ReadWrite"" Value=""3"" />
  </EnumType>
  <Term Name=""Description"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A brief description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
  </Term>
  <Term Name=""LongDescription"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A lengthy description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
  </Term>
  <Term Name=""IsLanguageDependent"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Term Property"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term are language-dependent"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""RequiresType"" Type=""Edm.String"" AppliesTo=""Term"">
    <Annotation Term=""Core.Description"" String=""Terms annotated with this term can only be applied to elements that have a type that is identical to or derived from the given type name"" />
  </Term>
  <Term Name=""ResourcePath"" Type=""Edm.String"" AppliesTo=""EntitySet Singleton ActionImport FunctionImport"">
    <Annotation Term=""Core.Description"" String=""Resource path for entity container child, can be relative to xml:base and the request URL"" />
    <Annotation Term=""Core.IsUrl"" Bool=""true"" />
  </Term>
  <Term Name=""DereferenceableIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Entity-ids are URLs that locate the identified entity"" />
  </Term>
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions"" />
  </Term>
  <Term Name=""Permissions"" Type=""Core.Permission"" AppliesTo=""Property"">
    <Annotation Term=""Core.Description"" String=""Permissions available for a property.The value of 2 is reserved for future use."" />
  </Term>
  <Term Name=""Immutable"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"">
    <Annotation Term=""Core.Description"" String=""A value for this non-key property can be provided on insert and remains unchanged on update"" />
  </Term>
  <Term Name=""Computed"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"">
    <Annotation Term=""Core.Description"" String=""A value for this property is generated on both insert and update"" />
  </Term>
  <Term Name=""IsURL"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property Term"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term MUST contain a valid URL"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""AcceptableMediaTypes"" Type=""Collection(Edm.String)"" AppliesTo=""EntityType Property"">
    <Annotation Term=""Core.Description"" String=""Lists the MIME types acceptable for the annotated entity type marked with HasStream=&quot;true&quot; or the annotated stream property"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
  </Term>
  <Term Name=""MediaType"" Type=""Edm.String"" AppliesTo=""Property"">
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.Binary"" />
  </Term>
  <Term Name=""IsMediaType"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property Term"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term MUST contain a valid MIME type"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""OptimisticConcurrency"" Type=""Collection(Edm.PropertyPath)"" AppliesTo=""EntitySet"">
    <Annotation Term=""Core.Description"" String=""Data modification requires the use of Etags. A non-empty collection contains the set of properties that are used to compute the ETag"" />
  </Term>
  <Term Name=""OptionalParameter"" Type=""Core.OptionalParameterType"" AppliesTo=""Parameter"">
    <Annotation Term=""Core.Description"" String=""Supplying a value for the parameter is optional."" />
  </Term>
</Schema>";

            var s = coreVocModel.FindDeclaredTerm("Org.OData.Core.V1.OptimisticConcurrency");
            Assert.NotNull(s);
            Assert.Equal("Org.OData.Core.V1", s.Namespace);
            Assert.Equal("OptimisticConcurrency", s.Name);

            var type = s.Type;
            Assert.Equal("Collection(Edm.PropertyPath)", type.FullName());
            Assert.Equal(EdmTypeKind.Collection, type.Definition.TypeKind);

            var descriptionTerm = coreVocModel.FindTerm("Org.OData.Core.V1.Description");
            Assert.NotNull(descriptionTerm);
            var descriptionType = descriptionTerm.Type.Definition as IEdmPrimitiveType;
            Assert.NotNull(descriptionType);
            Assert.Equal(EdmPrimitiveTypeKind.String, descriptionType.PrimitiveKind);

            var longDescriptionTerm = coreVocModel.FindTerm("Org.OData.Core.V1.LongDescription");
            Assert.NotNull(longDescriptionTerm);
            var longDescriptionType = longDescriptionTerm.Type.Definition as IEdmPrimitiveType;
            Assert.NotNull(longDescriptionType);
            Assert.Equal(EdmPrimitiveTypeKind.String, longDescriptionType.PrimitiveKind);

            var isLanguageDependentTerm = coreVocModel.FindTerm("Org.OData.Core.V1.IsLanguageDependent");
            Assert.NotNull(isLanguageDependentTerm);
            var isLanguageDependentType = isLanguageDependentTerm.Type.Definition as IEdmTypeDefinition;
            Assert.NotNull(isLanguageDependentType);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, isLanguageDependentType.UnderlyingType.PrimitiveKind);

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            IEnumerable<EdmError> errors;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            coreVocModel.TryWriteSchema(xw, out errors);
            xw.Flush();
#if NETCOREAPP1_0
            xw.Dispose();
#else
            xw.Close();
#endif
            string output = sw.ToString();
            Assert.False(errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }
    }
}
