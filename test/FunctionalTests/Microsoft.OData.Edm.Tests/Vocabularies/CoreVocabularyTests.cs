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
  <TypeDefinition Name=""MessageSeverity"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Validation.AllowedValues"">
      <Collection>
        <Record>
          <PropertyValue Property=""Value"" String=""success"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""info"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""warning"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""error"" />
        </Record>
      </Collection>
    </Annotation>
  </TypeDefinition>
  <TypeDefinition Name=""Tag"" UnderlyingType=""Edm.Boolean"">
    <Annotation Term=""Core.Description"" String=""This is the type to use for all tagging terms"" />
  </TypeDefinition>
  <TypeDefinition Name=""QualifiedTypeName"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The qualified name of a type in scope."" />
  </TypeDefinition>
  <ComplexType Name=""RevisionType"">
    <Property Name=""Version"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""The schema version with which this revision was first published"" />
    </Property>
    <Property Name=""Kind"" Type=""Core.RevisionKind"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The kind of revision"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Text describing the reason for the revision"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""Link"">
    <Property Name=""rel"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Link relation type, see [IANA Link Relations](http://www.iana.org/assignments/link-relations/link-relations.xhtml)"" />
    </Property>
    <Property Name=""href"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
      <Annotation Term=""Core.Description"" String=""URL of related information"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""The Link term is inspired by the `atom:link` element, see [RFC4287](https://tools.ietf.org/html/rfc4287#section-4.2.7), and the `Link` HTTP header, see [RFC5988](https://tools.ietf.org/html/rfc5988)"" />
  </ComplexType>
  <ComplexType Name=""MessageType"">
    <Property Name=""code"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Machine-readable, language-independent message code"" />
    </Property>
    <Property Name=""message"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Human-readable, language-dependent message text"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""severity"" Type=""Core.MessageSeverity"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Severity of the message"" />
    </Property>
    <Property Name=""target"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A path to the target of the message detail, relative to the annotated instance"" />
    </Property>
    <Property Name=""details"" Type=""Collection(Core.MessageType)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of detail messages"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OptionalParameterType"">
    <Property Name=""DefaultValue"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Default value for an optional parameter, using the same rules for the default value facet of a property."" />
    </Property>
  </ComplexType>
  <EnumType Name=""RevisionKind"">
    <Member Name=""Added"" />
    <Member Name=""Modified"" />
    <Member Name=""Deprecated"" />
  </EnumType>
  <EnumType Name=""Permission"" IsFlags=""true"">
    <Member Name=""None"" Value=""0"" />
    <Member Name=""Read"" Value=""1"" />
    <Member Name=""Write"" Value=""2"" />
    <Member Name=""ReadWrite"" Value=""3"" />
    <Member Name=""Invoke"" Value=""4"" />
  </EnumType>
  <Term Name=""ODataVersions"" Type=""Edm.String"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""A space-separated list of supported versions of the OData Protocol. Note that 4.0 is implied by 4.01 and does not need to be separately listed."" />
  </Term>
  <Term Name=""SchemaVersion"" Type=""Edm.String"" AppliesTo=""Schema Reference"">
    <Annotation Term=""Core.Description"" String=""Service-defined value representing the version of the schema. Services MAY use semantic versioning, but clients MUST NOT assume this is the case."" />
  </Term>
  <Term Name=""Revisions"" Type=""Collection(Core.RevisionType)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""List of revisions of a model element"" />
  </Term>
  <Term Name=""Description"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A brief description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
  </Term>
  <Term Name=""LongDescription"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A lengthy description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
  </Term>
  <Term Name=""Links"" Type=""Collection(Core.Link)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Link to related information"" />
  </Term>
  <Term Name=""Messages"" Type=""Collection(Core.MessageType)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Instance annotation for warning and info messages"" />
  </Term>
  <Term Name=""IsLanguageDependent"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Term Property"" Nullable=""false"">
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
  <Term Name=""DereferenceableIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Entity-ids are URLs that locate the identified entity"" />
  </Term>
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions"" />
  </Term>
  <Term Name=""Permissions"" Type=""Core.Permission"" AppliesTo=""Property ComplexType TypeDefinition EntityType EntitySet NavigationProperty Action Function"">
    <Annotation Term=""Core.Description"" String=""Permissions for accessing a resource"" />
  </Term>
  <Term Name=""ContentID"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A unique identifier for nested entities within a request."" />
  </Term>
  <Term Name=""DefaultNamespace"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Schema Include"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Functions, actions and types in this namespace can be referenced in URLs with or without namespace- or alias- qualification."" />
    <Annotation Term=""Core.LongDescription"" String=""Data Modelers should ensure uniqueness of schema children across all default namespaces, and should avoid naming bound functions, actions, or derived types with the same name as a structural or navigational property of the type."" />
  </Term>
  <Term Name=""Immutable"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A value for this non-key property can be provided on insert and remains unchanged on update"" />
  </Term>
  <Term Name=""Computed"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"" Nullable=""false"">
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
    <Annotation Term=""Core.Description"" String=""Data modification requires the use of ETags. A non-empty collection contains the set of properties that are used to compute the ETag"" />
  </Term>
  <Term Name=""AdditionalProperties"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityType ComplexType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Instances of this type may contain properties in addition to those declared in $metadata"" />
    <Annotation Term=""Core.LongDescription"" String=""If specified as false clients can assume that instances will not contain dynamic properties, irrespective of the value of the OpenType attribute."" />
  </Term>
  <Term Name=""AutoExpand"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The service will automatically expand this navigation property even if not requested with $expand"" />
  </Term>
  <Term Name=""AutoExpandReferences"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The service will automatically expand this navigation property as entity references even if not requested with $expand=.../$ref"" />
  </Term>
  <Term Name=""MayImplement"" Type=""Collection(Core.QualifiedTypeName)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A collection of qualified type names outside of the type hierarchy that instances of this type might be addressable as by using a type-cast segment."" />
  </Term>
  <Term Name=""Ordered"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property NavigationProperty EntitySet ReturnType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Collection has a stable order. Ordered collections of primitive or complex types can be indexed by ordinal."" />
  </Term>
  <Term Name=""PositionalInsert"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property NavigationProperty EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Items can be inserted at a given ordinal index."" />
  </Term>
  <Term Name=""OptionalParameter"" Type=""Core.OptionalParameterType"" AppliesTo=""Parameter"">
    <Annotation Term=""Core.Description"" String=""Supplying a value for the parameter is optional."" />
  </Term>
  <Term Name=""OperationAvailable"" Type=""Edm.Boolean"" DefaultValue=""true"" AppliesTo=""Action Function"">
    <Annotation Term=""Core.Description"" String=""Action or function is available"" />
    <Annotation Term=""Core.LongDescription"" String=""The annotation value will usually be an expression, e.g. using properties of the binding parameter type for instance-dependent availability, or using properties of a singleton for global availability. The static value `null` means that availability cannot be determined upfront and is instead expressed as an operation advertisement."" />
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
