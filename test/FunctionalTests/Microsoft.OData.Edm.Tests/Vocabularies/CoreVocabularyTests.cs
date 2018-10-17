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
  <TypeDefinition Name=""QualifiedTermName"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The qualified name of a term in scope."" />
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
  <ComplexType Name=""AlternateKey"">
    <Property Name=""Key"" Type=""Collection(Core.PropertyRef)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The set of properties that make up this key"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""PropertyRef"">
    <Property Name=""Name"" Type=""Edm.PropertyPath"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""A path expression resolving to a primitive property of the entity type itself or to a primitive property of a complex or navigation property (recursively) of the entity type. The names of the properties in the path are joined together by forward slashes."" />
    </Property>
    <Property Name=""Alias"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""A SimpleIdentifier that MUST be unique within the set of aliases, structural and navigation properties of the containing entity type that MUST be used in the key predicate of URLs"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""Dictionary"" OpenType=""true"">
    <Annotation Term=""Core.Description"" String=""A dictionary of name-value pairs. Names must be valid property names, values may be restricted to a list of types via an annotation with term `Validation.OpenPropertyTypeConstraint`."" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;Property|Type&#xA;:-------|:---&#xA;Any simple identifier | Any type listed in `Validation.OpenPropertyTypeConstraint`, or any type if there is no constraint&#xA;"" />
  </ComplexType>
  <ComplexType Name=""OptionalParameterType"">
    <Property Name=""DefaultValue"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Default value for an optional parameter of primitive or enumeration type, using the same rules as the `cast` function in URLs."" />
    </Property>
  </ComplexType>
  <EnumType Name=""RevisionKind"">
    <Member Name=""Added"">
      <Annotation Term=""Core.Description"" String=""Model element was added"" />
    </Member>
    <Member Name=""Modified"">
      <Annotation Term=""Core.Description"" String=""Model element was modified"" />
    </Member>
    <Member Name=""Deprecated"">
      <Annotation Term=""Core.Description"" String=""Model element was deprecated"" />
    </Member>
  </EnumType>
  <EnumType Name=""Permission"" IsFlags=""true"">
    <Member Name=""None"" Value=""0"">
      <Annotation Term=""Core.Description"" String=""No permissions"" />
    </Member>
    <Member Name=""Read"" Value=""1"">
      <Annotation Term=""Core.Description"" String=""Read permission"" />
    </Member>
    <Member Name=""Write"" Value=""2"">
      <Annotation Term=""Core.Description"" String=""Write permission"" />
    </Member>
    <Member Name=""ReadWrite"" Value=""3"">
      <Annotation Term=""Core.Description"" String=""Read and write permission"" />
    </Member>
    <Member Name=""Invoke"" Value=""4"">
      <Annotation Term=""Core.Description"" String=""Permission to invoke actions"" />
    </Member>
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
    <Annotation Term=""Core.Description"" String=""A value for this non-key property can be provided by the client on insert and remains unchanged on update"" />
  </Term>
  <Term Name=""Computed"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A value for this property is generated on both insert and update"" />
  </Term>
  <Term Name=""ComputedDefaultValue"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A value for this property can be provided by the client on insert and update. If no value is provided on insert, a non-static default value is generated"" />
  </Term>
  <Term Name=""IsURL"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term MUST contain a valid URL"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""AcceptableMediaTypes"" Type=""Collection(Edm.String)"" AppliesTo=""EntityType Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Lists the MIME types acceptable for the annotated entity type marked with HasStream=&quot;true&quot; or the annotated stream property"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
  </Term>
  <Term Name=""MediaType"" Type=""Edm.String"" AppliesTo=""Property"">
    <Annotation Term=""Core.Description"" String=""The media type of a binary resource"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.Binary"" />
  </Term>
  <Term Name=""IsMediaType"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term MUST contain a valid MIME type"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""OptimisticConcurrency"" Type=""Collection(Edm.PropertyPath)"" AppliesTo=""EntitySet"" Nullable=""false"">
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
  <Term Name=""AlternateKeys"" Type=""Collection(Core.AlternateKey)"" AppliesTo=""EntityType EntitySet NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Communicates available alternate keys"" />
  </Term>
  <Term Name=""OptionalParameter"" Type=""Core.OptionalParameterType"" AppliesTo=""Parameter"">
    <Annotation Term=""Core.Description"" String=""Supplying a value for the parameter is optional."" />
    <Annotation Term=""Core.LongDescription"" String=""All parameters marked as optional must come after any parameters not marked as optional. The binding parameter must not be marked as optional."" />
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

        [Theory]
        [InlineData("OptionalParameterType", "DefaultValue")]
        [InlineData("PropertyRef", "Name|Alias")]
        [InlineData("AlternateKey", "Key")]
        [InlineData("MessageType", "code|message|severity|target|details")]
        [InlineData("Link", "rel|href")]
        [InlineData("RevisionType", "Version|Kind|Description")]
        public void TestCoreVocabularyComplexType(string typeName, string properties)
        {
            var schemaType = this.coreVocModel.FindDeclaredType("Org.OData.Core.V1." + typeName);
            Assert.NotNull(schemaType);

            Assert.Equal(EdmTypeKind.Complex, schemaType.TypeKind);
            IEdmComplexType complex = (IEdmComplexType)(schemaType);

            Assert.False(complex.IsAbstract);
            Assert.False(complex.IsOpen);
            Assert.Null(complex.BaseType);

            Assert.NotEmpty(complex.DeclaredProperties);
            Assert.Equal(properties, string.Join("|", complex.DeclaredProperties.Select(e => e.Name)));
            Assert.Empty(complex.DeclaredNavigationProperties());
        }

        [Theory]
        [InlineData("RevisionKind", "Added|Modified|Deprecated", false)]
        [InlineData("Permission", "None|Read|Write|ReadWrite|Invoke", true)]
        public void TestCoreVocabularyEnumType(string typeName, string members, bool isFlags)
        {
            var schemaType = this.coreVocModel.FindDeclaredType("Org.OData.Core.V1." + typeName);
            Assert.NotNull(schemaType);

            Assert.Equal(EdmTypeKind.Enum, schemaType.TypeKind);
            IEdmEnumType enumType = (IEdmEnumType)(schemaType);

            Assert.Equal(isFlags, enumType.IsFlags);

            Assert.NotEmpty(enumType.Members);
            Assert.Equal(members, string.Join("|", enumType.Members.Select(e => e.Name)));
        }

        [Theory]
        [InlineData("OperationAvailable", "Edm.Boolean", "Action Function")]
        [InlineData("OptionalParameter", "Org.OData.Core.V1.OptionalParameterType", "Parameter")]
        [InlineData("AlternateKeys", "Collection(Org.OData.Core.V1.AlternateKey)", "EntityType EntitySet NavigationProperty")]
        [InlineData("PositionalInsert", "Org.OData.Core.V1.Tag", "Property NavigationProperty EntitySet")]
        [InlineData("Ordered", "Org.OData.Core.V1.Tag", "Property NavigationProperty EntitySet ReturnType")]
        [InlineData("MayImplement", "Collection(Org.OData.Core.V1.QualifiedTypeName)", null)]
        [InlineData("AutoExpandReferences", "Org.OData.Core.V1.Tag", "NavigationProperty")]
        [InlineData("AutoExpand", "Org.OData.Core.V1.Tag", "NavigationProperty")]
        [InlineData("AdditionalProperties", "Org.OData.Core.V1.Tag", "EntityType ComplexType")]
        [InlineData("OptimisticConcurrency", "Collection(Edm.PropertyPath)", "EntitySet")]
        [InlineData("IsMediaType", "Org.OData.Core.V1.Tag", "Property Term")]
        [InlineData("MediaType", "Edm.String", "Property")]
        [InlineData("AcceptableMediaTypes", "Collection(Edm.String)", "EntityType Property")]
        [InlineData("IsURL", "Org.OData.Core.V1.Tag", "Property Term")]
        [InlineData("ComputedDefaultValue", "Org.OData.Core.V1.Tag", "Property")]
        [InlineData("Computed", "Org.OData.Core.V1.Tag", "Property")]
        [InlineData("Immutable", "Org.OData.Core.V1.Tag", "Property")]
        [InlineData("DefaultNamespace", "Org.OData.Core.V1.Tag", "Schema Include")]
        [InlineData("ContentID", "Edm.String", null)]
        [InlineData("Permissions", "Org.OData.Core.V1.Permission", "Property ComplexType TypeDefinition EntityType EntitySet NavigationProperty Action Function")]
        [InlineData("ConventionalIDs", "Org.OData.Core.V1.Tag", "EntityContainer")]
        [InlineData("DereferenceableIDs", "Org.OData.Core.V1.Tag", "EntityContainer")]
        [InlineData("ResourcePath", "Edm.String", "EntitySet Singleton ActionImport FunctionImport")]
        [InlineData("RequiresType", "Edm.String", "Term")]
        [InlineData("IsLanguageDependent", "Org.OData.Core.V1.Tag", "Term Property")]
        [InlineData("Messages", "Collection(Org.OData.Core.V1.MessageType)", null)]
        [InlineData("Links", "Collection(Org.OData.Core.V1.Link)", null)]
        [InlineData("LongDescription", "Edm.String", null)]
        [InlineData("Description", "Edm.String", null)]
        [InlineData("Revisions", "Collection(Org.OData.Core.V1.RevisionType)", null)]
        [InlineData("SchemaVersion", "Edm.String", "Schema Reference")]
        [InlineData("ODataVersions", "Edm.String", "EntityContainer")]
        public void TestCoreVocabularyTermType(string termName, string typeName, string appliesTo)
        {
            var termType = this.coreVocModel.FindDeclaredTerm("Org.OData.Core.V1." + termName);
            Assert.NotNull(termType);

            Assert.Equal(typeName, termType.Type.FullName());

            if (appliesTo != null)
            {
                Assert.Equal(appliesTo, termType.AppliesTo);
            }
            else
            {
                Assert.Null(termType.AppliesTo);
            }
        }
    }
}
