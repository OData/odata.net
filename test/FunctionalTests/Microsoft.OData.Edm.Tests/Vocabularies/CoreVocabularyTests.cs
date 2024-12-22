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
    public partial class CoreVocabularyTests
    {
        private readonly IEdmModel coreVocModel = CoreVocabularyModel.Instance;

#region expectedText
        private const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
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
  <TypeDefinition Name=""QualifiedActionName"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The qualified name of an action in scope."" />
  </TypeDefinition>
  <TypeDefinition Name=""QualifiedBoundOperationName"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The qualified name of a bound action or function in scope."" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;&#x9;&#x9;&#x9;&#x9;Either&#xA;&#x9;&#x9;&#x9;&#x9;- the qualified name of an action, to indicate the single bound overload with the specified binding parameter type,&#xA;&#x9;&#x9;&#x9;&#x9;- the qualified name of a function, to indicate all bound overloads with the specified binding parameter type, or&#xA;&#x9;&#x9;&#x9;&#x9;- the qualified name of a function followed by parentheses containing a comma-separated list of parameter types, in the order of their definition, to identify a single function overload with the first (binding) parameter matching the specified parameter type.&#xA;&#x9;&#x9;&#x9;"" />
  </TypeDefinition>
  <TypeDefinition Name=""LocalDateTime"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A string representing a Local Date-Time value with no offset."" />
    <Annotation Term=""Validation.Pattern"" String=""^[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T([01][0-9]|2[0-3]):[0-5][0-9](:[0-5][0-9](\\.[0-9]+)?)?$"" />
  </TypeDefinition>
  <TypeDefinition Name=""SimpleIdentifier"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A [simple identifier](https://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_SimpleIdentifier)"" />
    <Annotation Term=""Validation.Pattern"" String=""^[\p{L}\p{Nl}_][\p{L}\p{Nl}\p{Nd}\p{Mn}\p{Mc}\p{Pc}\p{Cf}]{0,}$"" />
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
      <Annotation Term=""Core.IsURL"" />
      <Annotation Term=""Core.Description"" String=""URL of related information"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""The Link term is inspired by the `atom:link` element, see [RFC4287](https://tools.ietf.org/html/rfc4287#section-4.2.7), and the `Link` HTTP header, see [RFC5988](https://tools.ietf.org/html/rfc5988)"" />
  </ComplexType>
  <ComplexType Name=""ExampleValue"">
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Description of the example value"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""PrimitiveExampleValue"" BaseType=""Core.ExampleValue"">
    <Property Name=""Value"" Type=""Edm.PrimitiveType"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Example value for the custom parameter"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ComplexExampleValue"" BaseType=""Core.ExampleValue"">
    <Property Name=""Value"" Type=""Edm.ComplexType"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Example value for the custom parameter"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""EntityExampleValue"" BaseType=""Core.ExampleValue"">
    <NavigationProperty Name=""Value"" Type=""Edm.EntityType"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Example value for the custom parameter"" />
    </NavigationProperty>
  </ComplexType>
  <ComplexType Name=""ExternalExampleValue"" BaseType=""Core.ExampleValue"">
    <Property Name=""ExternalValue"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Url reference to the value in its literal format"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""MessageType"">
    <Property Name=""code"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Machine-readable, language-independent message code"" />
    </Property>
    <Property Name=""message"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Human-readable, language-dependent message text"" />
      <Annotation Term=""Core.IsLanguageDependent"" />
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
  <ComplexType Name=""ExceptionType"" Abstract=""true"">
    <Property Name=""info"" Type=""Core.MessageType"">
      <Annotation Term=""Core.Description"" String=""Information about the exception"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ValueExceptionType"" BaseType=""Core.ExceptionType"">
    <Property Name=""value"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""String representation of the exact value"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ResourceExceptionType"" BaseType=""Core.ExceptionType"">
    <Property Name=""retryLink"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A GET request to this URL retries retrieving the problematic instance"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""DataModificationExceptionType"" BaseType=""Core.ExceptionType"">
    <Property Name=""failedOperation"" Type=""Core.DataModificationOperationKind"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The kind of modification operation that failed"" />
    </Property>
    <Property Name=""responseCode"" Type=""Edm.Int16"">
      <Annotation Term=""Core.Description"" String=""Response code of the failed operation, e.g. 424 for a failed dependency"" />
      <Annotation Term=""Validation.Minimum"" Decimal=""100"" />
      <Annotation Term=""Validation.Maximum"" Decimal=""599"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ContentDispositionType"">
    <Property Name=""Type"" Type=""Edm.String"" DefaultValue=""attachment"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The disposition type of the binary or stream value, see [RFC 6266, Disposition Type](https://datatracker.ietf.org/doc/html/rfc6266#section-4.2)"" />
    </Property>
    <Property Name=""Filename"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""The proposed filename for downloading the binary or stream value, see [RFC 6266, Disposition Parameter: 'Filename'](https://datatracker.ietf.org/doc/html/rfc6266#section-4.3)"" />
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
    <Property Name=""Alias"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A SimpleIdentifier that MUST be unique within the set of aliases, structural and navigation properties of the containing entity type that MUST be used in the key predicate of URLs"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""Dictionary"" OpenType=""true"">
    <Annotation Term=""Core.Description"" String=""A dictionary of name-value pairs. Names must be valid property names, values may be restricted to a list of types via an annotation with term `Validation.OpenPropertyTypeConstraint`."" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;&#x9;&#x9;&#x9;&#x9;Property|Type&#xA;&#x9;&#x9;&#x9;&#x9;:-------|:---&#xA;&#x9;&#x9;&#x9;&#x9;Any simple identifier | Any type listed in `Validation.OpenPropertyTypeConstraint`, or any type if there is no constraint&#xA;&#x9;&#x9;&#x9;"" />
  </ComplexType>
  <ComplexType Name=""OptionalParameterType"">
    <Property Name=""DefaultValue"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Default value for an optional parameter of primitive or enumeration type, using the same rules as the `cast` function in URLs."" />
      <Annotation Term=""Core.LongDescription"" String=""If no explicit DefaultValue is specified, the service is free on how to interpret omitting the parameter from the request. For example, a service might interpret an omitted optional parameter `KeyDate` as having the current date."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""GeometryFeatureType"">
    <Property Name=""geometry"" Type=""Edm.Geometry"">
      <Annotation Term=""Core.Description"" String=""Location of the Feature"" />
    </Property>
    <Property Name=""properties"" Type=""Core.Dictionary"">
      <Annotation Term=""Core.Description"" String=""Properties of the Feature"" />
    </Property>
    <Property Name=""id"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Commonly used identifer for a Feature"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""A [Feature Object](https://datatracker.ietf.org/doc/html/rfc7946#section-3.2) represents a spatially bounded thing"" />
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
  <EnumType Name=""DataModificationOperationKind"">
    <Member Name=""insert"">
      <Annotation Term=""Core.Description"" String=""Insert new instance"" />
    </Member>
    <Member Name=""update"">
      <Annotation Term=""Core.Description"" String=""Update existing instance"" />
    </Member>
    <Member Name=""upsert"">
      <Annotation Term=""Core.Description"" String=""Insert new instance or update it if it already exists"" />
    </Member>
    <Member Name=""delete"">
      <Annotation Term=""Core.Description"" String=""Delete existing instance"" />
    </Member>
    <Member Name=""invoke"">
      <Annotation Term=""Core.Description"" String=""Invoke action or function"" />
    </Member>
    <Member Name=""link"">
      <Annotation Term=""Core.Description"" String=""Add link between entities"" />
    </Member>
    <Member Name=""unlink"">
      <Annotation Term=""Core.Description"" String=""Remove link between entities"" />
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
  <Term Name=""ODataVersions"" Type=""Edm.String"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A space-separated list of supported versions of the OData Protocol. Note that 4.0 is implied by 4.01 and does not need to be separately listed."" />
  </Term>
  <Term Name=""SchemaVersion"" Type=""Edm.String"" AppliesTo=""Schema Reference"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Service-defined value representing the version of the schema. Services MAY use semantic versioning, but clients MUST NOT assume this is the case."" />
  </Term>
  <Term Name=""Revisions"" Type=""Collection(Core.RevisionType)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""List of revisions of a model element"" />
  </Term>
  <Term Name=""Description"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A brief description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" />
  </Term>
  <Term Name=""LongDescription"" Type=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""A long description of a model element"" />
    <Annotation Term=""Core.IsLanguageDependent"" />
  </Term>
  <Term Name=""Links"" Type=""Collection(Core.Link)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Link to related information"" />
  </Term>
  <Term Name=""Example"" Type=""Core.ExampleValue"" AppliesTo=""EntityType ComplexType TypeDefinition Term Property NavigationProperty Parameter ReturnType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Example for an instance of the annotated model element"" />
    <Annotation Term=""Core.Example"">
      <Record>
        <PropertyValue Property=""Description"" String=""The value of Core.Example is a record/object containing the example value and/or annotation examples."" />
      </Record>
    </Annotation>
  </Term>
  <Term Name=""Messages"" Type=""Collection(Core.MessageType)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Instance annotation for warning and info messages"" />
  </Term>
  <Term Name=""ValueException"" Type=""Core.ValueExceptionType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The annotated value is problematic"" />
  </Term>
  <Term Name=""ResourceException"" Type=""Core.ResourceExceptionType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The annotated instance within a success payload is problematic"" />
  </Term>
  <Term Name=""DataModificationException"" Type=""Core.DataModificationExceptionType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A modification operation failed on the annotated instance or collection within a success payload"" />
  </Term>
  <Term Name=""IsLanguageDependent"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Term Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term are language-dependent"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""RequiresType"" Type=""Edm.String"" AppliesTo=""Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Terms annotated with this term can only be applied to elements that have a type that is identical to or derived from the given type name"" />
  </Term>
  <Term Name=""AppliesViaContainer"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The target path of an annotation with the tagged term MUST start with an entity container or the annotation MUST be embedded within an entity container, entity set or singleton"" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;&#x9;&#x9;&#x9;&#x9;Services MAY additionally annotate a container-independent model element (entity type, property, navigation property) if allowed by the `AppliesTo` property of the term&#xA;&#x9;&#x9;&#x9;&#x9;and the annotation applies to all uses of that model element.&#xA;&#x9;&#x9;&#x9;"" />
  </Term>
  <Term Name=""ResourcePath"" Type=""Edm.String"" AppliesTo=""EntitySet Singleton ActionImport FunctionImport"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Resource path for entity container child, can be relative to xml:base and the request URL"" />
    <Annotation Term=""Core.IsURL"" />
  </Term>
  <Term Name=""DereferenceableIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Entity-ids are URLs that locate the identified entity"" />
  </Term>
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions"" />
  </Term>
  <Term Name=""Permissions"" Type=""Core.Permission"" AppliesTo=""Property ComplexType TypeDefinition EntityType EntitySet NavigationProperty Action Function"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Permissions for accessing a resource"" />
  </Term>
  <Term Name=""ContentID"" Type=""Edm.String"" Nullable=""false"">
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
  <Term Name=""AcceptableMediaTypes"" Type=""Collection(Edm.String)"" AppliesTo=""EntityType Property Term TypeDefinition Parameter ReturnType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Lists the MIME types acceptable for the annotated entity type marked with HasStream=&quot;true&quot; or the annotated binary, stream, or string property or term"" />
    <Annotation Term=""Core.LongDescription"" String=""The annotation of a TypeDefinition propagates to the model elements having this type"" />
    <Annotation Term=""Core.IsMediaType"" />
  </Term>
  <Term Name=""MediaType"" Type=""Edm.String"" AppliesTo=""EntityType Property Term TypeDefinition Parameter ReturnType"">
    <Annotation Term=""Core.Description"" String=""The media type of the media stream of the annotated entity type marked with HasStream=&quot;true&quot; or the annotated binary, stream, or string property or term"" />
    <Annotation Term=""Core.LongDescription"" String=""The annotation of a TypeDefinition propagates to the model elements having this type"" />
    <Annotation Term=""Core.IsMediaType"" />
  </Term>
  <Term Name=""IsMediaType"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Properties and terms annotated with this term MUST contain a valid MIME type"" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""ContentDisposition"" Type=""Core.ContentDispositionType"" AppliesTo=""EntityType Property Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The content disposition of the media stream of the annotated entity type marked with HasStream=&quot;true&quot; or the annotated binary, stream, or string property or term"" />
  </Term>
  <Term Name=""OptimisticConcurrency"" Type=""Collection(Edm.PropertyPath)"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Data modification requires the use of ETags. A non-empty collection contains the set of properties that are used to compute the ETag. An empty collection means that the service won't tell how it computes the ETag"" />
  </Term>
  <Term Name=""AdditionalProperties"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityType ComplexType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Instances of this type may contain properties in addition to those declared in $metadata"" />
    <Annotation Term=""Core.LongDescription"" String=""If specified as false clients can assume that instances will not contain dynamic properties, irrespective of the value of the OpenType attribute."" />
  </Term>
  <Term Name=""AutoExpand"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityType NavigationProperty Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The service will automatically expand this stream property, navigation property, or the media stream of this media entity type even if not requested with $expand"" />
  </Term>
  <Term Name=""AutoExpandReferences"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The service will automatically expand this navigation property as entity references even if not requested with $expand=.../$ref"" />
  </Term>
  <Term Name=""MayImplement"" Type=""Collection(Core.QualifiedTypeName)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A collection of qualified type names outside of the type hierarchy that instances of this type might be addressable as by using a type-cast segment."" />
  </Term>
  <Term Name=""Ordered"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property NavigationProperty EntitySet ReturnType Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Collection has a stable order. Ordered collections of primitive or complex types can be indexed by ordinal."" />
  </Term>
  <Term Name=""PositionalInsert"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property NavigationProperty EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Items can be inserted at a given ordinal index."" />
  </Term>
  <Term Name=""AlternateKeys"" Type=""Collection(Core.AlternateKey)"" AppliesTo=""EntityType EntitySet NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Communicates available alternate keys"" />
  </Term>
  <Term Name=""OptionalParameter"" Type=""Core.OptionalParameterType"" AppliesTo=""Parameter"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supplying a value for the action or function parameter is optional."" />
    <Annotation Term=""Core.LongDescription"" String=""All parameters marked as optional must come after any parameters not marked as optional. The binding parameter must not be marked as optional."" />
  </Term>
  <Term Name=""OperationAvailable"" Type=""Edm.Boolean"" DefaultValue=""true"" AppliesTo=""Action Function"">
    <Annotation Term=""Core.Description"" String=""Action or function is available"" />
    <Annotation Term=""Core.LongDescription"" String=""The annotation value will usually be an expression, e.g. using properties of the binding parameter type for instance-dependent availability, or using properties of a singleton for global availability. The static value `null` means that availability cannot be determined upfront and is instead expressed as an operation advertisement."" />
  </Term>
  <Term Name=""RequiresExplicitBinding"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Action Function"">
    <Annotation Term=""Core.Description"" String=""This bound action or function is only available on model elements annotated with the ExplicitOperationBindings term."" />
  </Term>
  <Term Name=""ExplicitOperationBindings"" Type=""Collection(Core.QualifiedBoundOperationName)"">
    <Annotation Term=""Core.Description"" String=""The qualified names of explicitly bound operations that are supported on the target model element. These operations are in addition to any operations not annotated with RequiresExplicitBinding that are bound to the type of the target model element."" />
  </Term>
  <Term Name=""SymbolicName"" Type=""Core.SimpleIdentifier"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A symbolic name for a model element"" />
  </Term>
  <Term Name=""GeometryFeature"" Type=""Core.GeometryFeatureType"">
    <Annotation Term=""Core.Description"" String=""A [Feature Object](https://datatracker.ietf.org/doc/html/rfc7946#section-3.2) represents a spatially bounded thing"" />
  </Term>
  <Term Name=""AnyStructure"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityType ComplexType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Instances of a type are annotated with this tag if they have no common structure in a given response payload"" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;&#x9;&#x9;&#x9;&#x9;The select-list of a context URL MUST be `(@Core.AnyStructure)` if it would otherwise be empty,&#xA;&#x9;&#x9;&#x9;&#x9;but this instance annotation SHOULD be omitted from the response value.&#xA;&#x9;&#x9;&#x9;"" />
  </Term>
  <Term Name=""IsDelta"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""ReturnType Parameter"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The annotated Action or Function Parameter or Return Type is represented as a Delta payload"" />
    <Annotation Term=""Core.LongDescription"" String=""&#xA;&#x9;&#x9;&#x9;&#x9;The parameter or result is represented as a delta payload, which may include deleted entries as well as changes to related&#xA;&#x9;&#x9;&#x9;&#x9;entities and relationships, according to the format-specific delta representation.&#xA;&#x9;&#x9;&#x9;"" />
  </Term>
</Schema>";
#endregion

        [Fact]
        public void TestBaseCoreVocabularyModel()
        {
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

            var requiresExplicitBindingTerm = coreVocModel.FindTerm("Org.OData.Core.V1.RequiresExplicitBinding");
            Assert.NotNull(requiresExplicitBindingTerm);
            var requiresExplicitBindingType = requiresExplicitBindingTerm.Type.Definition as IEdmTypeDefinition;
            Assert.NotNull(requiresExplicitBindingType);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, requiresExplicitBindingType.UnderlyingType.PrimitiveKind);

            var explicitOperationBindingsTerm = coreVocModel.FindTerm("Org.OData.Core.V1.ExplicitOperationBindings");
            Assert.NotNull(explicitOperationBindingsTerm);
            var explicitOperationBindingsType = explicitOperationBindingsTerm.Type.Definition;
            Assert.NotNull(explicitOperationBindingsType);
            Assert.Equal("Collection(Org.OData.Core.V1.QualifiedBoundOperationName)", explicitOperationBindingsType.FullTypeName());
            Assert.Equal(EdmTypeKind.Collection, explicitOperationBindingsType.TypeKind);

            var qualifiedBoundOperationNameType = coreVocModel.FindType("Org.OData.Core.V1.QualifiedBoundOperationName");
            Assert.NotNull(qualifiedBoundOperationNameType);
            Assert.Equal(qualifiedBoundOperationNameType, explicitOperationBindingsType.AsElementType());

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            IEnumerable<EdmError> errors;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            coreVocModel.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            string output = sw.ToString();
            Assert.False(errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }

        [Theory]
        [InlineData("OptionalParameterType", "DefaultValue", null)]
        [InlineData("PropertyRef", "Name|Alias", null)]
        [InlineData("AlternateKey", "Key", null)]
        [InlineData("MessageType", "code|message|severity|target|details", null)]
        [InlineData("Link", "rel|href", null)]
        [InlineData("RevisionType", "Version|Kind|Description", null)]
        [InlineData("ExampleValue", "Description", null)]
        [InlineData("PrimitiveExampleValue", "Value", "ExampleValue")]
        [InlineData("ComplexExampleValue", "Value", "ExampleValue")]
        [InlineData("EntityExampleValue", "Value", "ExampleValue")]
        [InlineData("ExternalExampleValue", "ExternalValue", "ExampleValue")]
        public void TestCoreVocabularyComplexType(string typeName, string properties, string baseType)
        {
            var schemaType = this.coreVocModel.FindDeclaredType("Org.OData.Core.V1." + typeName);
            Assert.NotNull(schemaType);

            Assert.Equal(EdmTypeKind.Complex, schemaType.TypeKind);
            IEdmComplexType complex = (IEdmComplexType)(schemaType);

            Assert.False(complex.IsAbstract);
            Assert.False(complex.IsOpen);
            if (baseType == null)
            {
                Assert.Null(complex.BaseType);
            }
            else
            {
                Assert.Equal("Org.OData.Core.V1." + baseType, complex.BaseType.FullTypeName());
            }

            Assert.NotEmpty(complex.DeclaredProperties);
            Assert.Equal(properties, string.Join("|", complex.DeclaredProperties.Select(e => e.Name)));

            if (typeName != "EntityExampleValue")
            {
                Assert.Empty(complex.DeclaredNavigationProperties());
            }
            else
            {
                Assert.Equal(properties, string.Join("|", complex.DeclaredNavigationProperties().Select(e => e.Name)));
            }
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
        [InlineData("ExplicitOperationBindings", "Collection(Org.OData.Core.V1.QualifiedBoundOperationName)", null)]
        [InlineData("RequiresExplicitBinding", "Org.OData.Core.V1.Tag", "Action Function")]
        [InlineData("OperationAvailable", "Edm.Boolean", "Action Function")]
        [InlineData("OptionalParameter", "Org.OData.Core.V1.OptionalParameterType", "Parameter")]
        [InlineData("AlternateKeys", "Collection(Org.OData.Core.V1.AlternateKey)", "EntityType EntitySet NavigationProperty")]
        [InlineData("PositionalInsert", "Org.OData.Core.V1.Tag", "Property NavigationProperty EntitySet")]
        [InlineData("Ordered", "Org.OData.Core.V1.Tag", "Property NavigationProperty EntitySet ReturnType Term")]
        [InlineData("MayImplement", "Collection(Org.OData.Core.V1.QualifiedTypeName)", null)]
        [InlineData("AutoExpandReferences", "Org.OData.Core.V1.Tag", "NavigationProperty")]
        [InlineData("AutoExpand", "Org.OData.Core.V1.Tag", "EntityType NavigationProperty Property")]
        [InlineData("AdditionalProperties", "Org.OData.Core.V1.Tag", "EntityType ComplexType")]
        [InlineData("OptimisticConcurrency", "Collection(Edm.PropertyPath)", "EntitySet")]
        [InlineData("IsMediaType", "Org.OData.Core.V1.Tag", "Property Term")]
        [InlineData("MediaType", "Edm.String", "EntityType Property Term TypeDefinition Parameter ReturnType")]
        [InlineData("AcceptableMediaTypes", "Collection(Edm.String)", "EntityType Property Term TypeDefinition Parameter ReturnType")]
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
        [InlineData("Example", "Org.OData.Core.V1.ExampleValue", "EntityType ComplexType TypeDefinition Term Property NavigationProperty Parameter ReturnType")]
        [InlineData("IsDelta","Org.OData.Core.V1.Tag","ReturnType Parameter")]
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

        [Theory]
        [InlineData("QualifiedBoundOperationName", "Edm.String")]
        [InlineData("MessageSeverity", "Edm.String")]
        [InlineData("Tag", "Edm.Boolean")]
        [InlineData("QualifiedTermName", "Edm.String")]
        [InlineData("QualifiedTypeName", "Edm.String")]
        [InlineData("LocalDateTime", "Edm.String")]
        public void TestCoreVocabularyTypeDefinition(string typeName, string underlyingTypeName)
        {
            var declaredType = this.coreVocModel.FindDeclaredType("Org.OData.Core.V1." + typeName);
            Assert.NotNull(declaredType);

            Assert.Equal(EdmTypeKind.TypeDefinition, declaredType.TypeKind);

            IEdmTypeDefinition typeDefinition = (IEdmTypeDefinition)declaredType;
            Assert.Equal(underlyingTypeName, typeDefinition.UnderlyingType.FullName());
        }

        [Fact]
        public void TestRevisionsTerm()
        {
            var revisionsTerm = CoreVocabularyModel.RevisionsTerm;
            Assert.NotNull(revisionsTerm);
            Assert.Equal("Org.OData.Core.V1.Revisions", revisionsTerm.FullName());
            Assert.Equal("Collection(Org.OData.Core.V1.RevisionType)", revisionsTerm.Type.FullName());
        }
    }
}
