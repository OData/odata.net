//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    /// <summary>
    /// Test capabilities vocabulary
    /// </summary>
    public class CapabilitiesVocabularyTests
    {
        private readonly IEdmModel capVocModel = CapabilitiesVocabularyModel.Instance;

        [Fact]
        public void TestCapabilitiesVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Capabilities.V1"" Alias=""Capabilities"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""FilterExpressionType"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Validation.AllowedValues"">
      <Collection>
        <Record>
          <PropertyValue Property=""Value"" String=""SingleValue"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""MultiValue"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""SingleRange"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""MultiRange"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""SearchExpression"" />
        </Record>
        <Record>
          <PropertyValue Property=""Value"" String=""MultiRangeOrSearchExpression"" />
        </Record>
      </Collection>
    </Annotation>
  </TypeDefinition>
  <ComplexType Name=""CallbackType"">
    <Property Name=""CallbackProtocols"" Type=""Collection(Capabilities.CallbackProtocol)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of supported callback protocols, e.g. `http` or `wss`"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""A non-empty collection lists the full set of supported protocols. A empty collection means 'only HTTP is supported'"" />
  </ComplexType>
  <ComplexType Name=""CallbackProtocol"">
    <Property Name=""Id"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Protocol Identifier"" />
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
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""This entity set supports the odata.track-changes preference"" />
    </Property>
    <Property Name=""FilterableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Change tracking supports filters on these properties"" />
      <Annotation Term=""Core.LongDescription"" String=""If no properties are specified or FilterableProperties is omitted, clients cannot assume support for filtering on any properties in combination with change tracking."" />
    </Property>
    <Property Name=""ExpandableProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Change tracking supports these properties expanded"" />
      <Annotation Term=""Core.LongDescription"" String=""If no properties are specified or ExpandableProperties is omitted, clients cannot assume support for expanding any properties in combination with change tracking."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""CountRestrictionsType"">
    <Property Name=""Countable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be counted (only valid if targeting an entity set)"" />
    </Property>
    <Property Name=""NonCountableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of these collection properties cannot be counted"" />
    </Property>
    <Property Name=""NonCountableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of these navigation properties cannot be counted"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""NavigationRestrictionsType"">
    <Property Name=""Navigability"" Type=""Capabilities.NavigationType"">
      <Annotation Term=""Core.Description"" String=""Default navigability for all navigation properties of the annotation target. Individual navigation properties can override this value via `RestrictedProperties/Navigability`."" />
    </Property>
    <Property Name=""RestrictedProperties"" Type=""Collection(Capabilities.NavigationPropertyRestriction)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of navigation properties with restrictions"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""NavigationPropertyRestriction"">
    <Property Name=""NavigationProperty"" Type=""Edm.NavigationPropertyPath"">
      <Annotation Term=""Core.Description"" String=""Navigation properties can be navigated"" />
    </Property>
    <Property Name=""Navigability"" Type=""Capabilities.NavigationType"">
      <Annotation Term=""Core.Description"" String=""Supported navigability of this navigation property"" />
    </Property>
    <Property Name=""FilterFunctions"" Type=""Collection(Edm.String)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of functions and operators supported in filter expressions"" />
      <Annotation Term=""Core.LongDescription"" String=""If not specified, null, or empty, all functions and operators may be attempted."" />
    </Property>
    <Property Name=""FilterRestrictions"" Type=""Capabilities.FilterRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on filter expressions"" />
    </Property>
    <Property Name=""SearchRestrictions"" Type=""Capabilities.SearchRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on search expressions"" />
    </Property>
    <Property Name=""SortRestrictions"" Type=""Capabilities.SortRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on orderby expressions"" />
    </Property>
    <Property Name=""TopSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $top"" />
    </Property>
    <Property Name=""SkipSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $skip"" />
    </Property>
    <Property Name=""SelectSupport"" Type=""Capabilities.SelectSupportType"">
      <Annotation Term=""Core.Description"" String=""Support for $select"" />
    </Property>
    <Property Name=""IndexableByKey"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports key values according to OData URL conventions"" />
    </Property>
    <Property Name=""InsertRestrictions"" Type=""Capabilities.InsertRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on insert operations"" />
    </Property>
    <Property Name=""DeepInsertSupport"" Type=""Capabilities.DeepInsertSupportType"">
      <Annotation Term=""Core.Description"" String=""Deep Insert Support of the annotated resource (the whole service, an entity set, or a collection-valued resource)"" />
    </Property>
    <Property Name=""UpdateRestrictions"" Type=""Capabilities.UpdateRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on update operations"" />
    </Property>
    <Property Name=""DeepUpdateSupport"" Type=""Capabilities.DeepUpdateSupportType"">
      <Annotation Term=""Core.Description"" String=""Deep Update Support of the annotated resource (the whole service, an entity set, or a collection-valued resource)"" />
    </Property>
    <Property Name=""DeleteRestrictions"" Type=""Capabilities.DeleteRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on delete operations"" />
    </Property>
    <Property Name=""OptimisticConcurrencyControl"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Data modification (including insert) along this navigation property requires the use of ETags"" />
    </Property>
    <Property Name=""ReadRestrictions"" Type=""Capabilities.ReadRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions for retrieving entities"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""SelectSupportType"">
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $select"" />
    </Property>
    <Property Name=""InstanceAnnotationsSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports instance annotations in $select list"" />
    </Property>
    <Property Name=""Expandable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$expand within $select is supported"" />
    </Property>
    <Property Name=""Filterable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$filter within $select is supported"" />
    </Property>
    <Property Name=""Searchable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$search within $select is supported"" />
    </Property>
    <Property Name=""TopSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$top within $select is supported"" />
    </Property>
    <Property Name=""SkipSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$skip within $select is supported"" />
    </Property>
    <Property Name=""ComputeSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$compute within $select is supported"" />
    </Property>
    <Property Name=""Countable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$count within $select is supported"" />
    </Property>
    <Property Name=""Sortable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$orderby within $select is supported"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""BatchSupportType"">
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports requests to $batch"" />
    </Property>
    <Property Name=""ContinueOnErrorSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports the continue on error preference"" />
    </Property>
    <Property Name=""ReferencesInRequestBodiesSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports Content-ID referencing in request bodies"" />
    </Property>
    <Property Name=""ReferencesAcrossChangeSetsSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports Content-ID referencing across change sets"" />
    </Property>
    <Property Name=""EtagReferencesSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports referencing Etags from previous requests"" />
    </Property>
    <Property Name=""RequestDependencyConditionsSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Service supports the `if` member in JSON batch requests"" />
    </Property>
    <Property Name=""SupportedFormats"" Type=""Collection(Edm.String)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Media types of supported formats for $batch"" />
      <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
      <Annotation Term=""Validation.AllowedValues"">
        <Collection>
          <Record>
            <PropertyValue Property=""Value"" String=""multipart/mixed"" />
          </Record>
          <Record>
            <PropertyValue Property=""Value"" String=""application/json"" />
          </Record>
        </Collection>
      </Annotation>
    </Property>
  </ComplexType>
  <ComplexType Name=""FilterRestrictionsType"">
    <Property Name=""Filterable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$filter is supported"" />
    </Property>
    <Property Name=""RequiresFilter"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$filter is required"" />
    </Property>
    <Property Name=""RequiredProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These properties must be specified in the $filter clause (properties of derived types are not allowed here)"" />
    </Property>
    <Property Name=""NonFilterableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties cannot be used in filter expressions"" />
    </Property>
    <Property Name=""FilterExpressionRestrictions"" Type=""Collection(Capabilities.FilterExpressionRestrictionType)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These properties only allow a subset of filter expressions. A valid filter expression for a single property can be enclosed in parentheses and combined by `and` with valid expressions for other properties."" />
    </Property>
    <Property Name=""MaxLevels"" Type=""Edm.Int32"" DefaultValue=""-1"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The maximum number of levels (including recursion) that can be traversed in a filter expression. A value of -1 indicates there is no restriction."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""FilterExpressionRestrictionType"">
    <Property Name=""Property"" Type=""Edm.PropertyPath"">
      <Annotation Term=""Core.Description"" String=""Path to the restricted property"" />
    </Property>
    <Property Name=""AllowedExpressions"" Type=""Capabilities.FilterExpressionType"">
      <Annotation Term=""Core.Description"" String=""Allowed subset of expressions"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""SortRestrictionsType"">
    <Property Name=""Sortable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$orderby is supported"" />
    </Property>
    <Property Name=""AscendingOnlyProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These properties can only be used for sorting in Ascending order"" />
    </Property>
    <Property Name=""DescendingOnlyProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These properties can only be used for sorting in Descending order"" />
    </Property>
    <Property Name=""NonSortableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties cannot be used in orderby expressions"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ExpandRestrictionsType"">
    <Property Name=""Expandable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$expand is supported"" />
    </Property>
    <Property Name=""StreamsExpandable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$expand is supported for stream properties and media resources"" />
    </Property>
    <Property Name=""NonExpandableProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These properties cannot be used in expand expressions"" />
    </Property>
    <Property Name=""NonExpandableStreamProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These stream properties cannot be used in expand expressions"" />
      <Annotation Term=""Core.RequiresType"" String=""Edm.Stream"" />
    </Property>
    <Property Name=""MaxLevels"" Type=""Edm.Int32"" DefaultValue=""-1"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The maximum number of levels that can be expanded in a expand expression. A value of -1 indicates there is no restriction."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""SearchRestrictionsType"">
    <Property Name=""Searchable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""$search is supported"" />
    </Property>
    <Property Name=""UnsupportedExpressions"" Type=""Capabilities.SearchExpressions"" DefaultValue=""none"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Expressions not supported in $search"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""InsertRestrictionsType"">
    <Property Name=""Insertable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be inserted"" />
    </Property>
    <Property Name=""NonInsertableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties cannot be specified on insert"" />
    </Property>
    <Property Name=""NonInsertableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow deep inserts"" />
    </Property>
    <Property Name=""RequiredProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties must be specified on insert"" />
    </Property>
    <Property Name=""MaxLevels"" Type=""Edm.Int32"" DefaultValue=""-1"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The maximum number of navigation properties that can be traversed when addressing the collection to insert into. A value of -1 indicates there is no restriction."" />
    </Property>
    <Property Name=""TypecastSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities of a specific derived type can be created by specifying a type-cast segment"" />
    </Property>
    <Property Name=""Permissions"" Type=""Collection(Capabilities.PermissionType)"">
      <Annotation Term=""Core.Description"" String=""Required permissions. One of the specified sets of scopes is required to perform the insert."" />
    </Property>
    <Property Name=""QueryOptions"" Type=""Capabilities.ModificationQueryOptionsType"">
      <Annotation Term=""Core.Description"" String=""Support for query options with insert requests"" />
    </Property>
    <Property Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom headers"" />
    </Property>
    <Property Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom query options"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A brief description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""LongDescription"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A lengthy description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""PermissionType"">
    <Property Name=""SchemeName"" Type=""Auth.SchemeName"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Authorization flow scheme name"" />
    </Property>
    <Property Name=""Scopes"" Type=""Collection(Capabilities.ScopeType)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of scopes that can provide access to the resource"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ScopeType"">
    <Property Name=""Scope"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Name of the scope."" />
    </Property>
    <Property Name=""RestrictedProperties"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Comma-separated string value of all properties that will be included or excluded when using the scope."" />
      <Annotation Term=""Core.LongDescription"" String=""Possible string value identifiers when specifying properties are `*`, _PropertyName_, `-`_PropertyName_.&#xA;&#xA;`*` denotes all properties are accessible.&#xA;&#xA;`-`_PropertyName_ excludes that specific property.&#xA;&#xA;_PropertyName_ explicitly provides access to the specific property.&#xA;&#xA;The absence of `RestrictedProperties` denotes all properties are accessible using that scope."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""DeepInsertSupportType"">
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Annotation target supports deep inserts"" />
    </Property>
    <Property Name=""ContentIDSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Annotation target supports accepting and returning nested entities annotated with the `Core.ContentID` instance annotation."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""UpdateRestrictionsType"">
    <Property Name=""Updatable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be updated"" />
    </Property>
    <Property Name=""Upsertable"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be upserted"" />
    </Property>
    <Property Name=""DeltaUpdateSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be inserted, updated, and deleted via a PATCH request with a delta payload"" />
    </Property>
    <Property Name=""UpdateMethod"" Type=""Capabilities.HttpMethod"">
      <Annotation Term=""Core.Description"" String=""Supported HTTP Methods (PUT or PATCH) for updating an entity.  If null, PATCH SHOULD be supported and PUT MAY be supported."" />
    </Property>
    <Property Name=""FilterSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of collections can be updated via a PATCH request with a `/$filter(...)/$each` segment"" />
    </Property>
    <Property Name=""TypecastSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of collections can be updated via a PATCH request with a type-cast segment and a `/$each` segment"" />
    </Property>
    <Property Name=""NonUpdatableProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties cannot be specified on update"" />
    </Property>
    <Property Name=""NonUpdatableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow rebinding"" />
    </Property>
    <Property Name=""RequiredProperties"" Type=""Collection(Edm.PropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These structural properties must be specified on update"" />
    </Property>
    <Property Name=""MaxLevels"" Type=""Edm.Int32"" DefaultValue=""-1"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The maximum number of navigation properties that can be traversed when addressing the collection or entity to update. A value of -1 indicates there is no restriction."" />
    </Property>
    <Property Name=""Permissions"" Type=""Collection(Capabilities.PermissionType)"">
      <Annotation Term=""Core.Description"" String=""Required permissions. One of the specified sets of scopes is required to perform the update."" />
    </Property>
    <Property Name=""QueryOptions"" Type=""Capabilities.ModificationQueryOptionsType"">
      <Annotation Term=""Core.Description"" String=""Support for query options with update requests"" />
    </Property>
    <Property Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom headers"" />
    </Property>
    <Property Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom query options"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A brief description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""LongDescription"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A lengthy description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""DeepUpdateSupportType"">
    <Property Name=""Supported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Annotation target supports deep updates"" />
    </Property>
    <Property Name=""ContentIDSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Annotation target supports accepting and returning nested entities annotated with the `Core.ContentID` instance annotation."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""DeleteRestrictionsType"">
    <Property Name=""Deletable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be deleted"" />
    </Property>
    <Property Name=""NonDeletableNavigationProperties"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""These navigation properties do not allow DeleteLink requests"" />
    </Property>
    <Property Name=""MaxLevels"" Type=""Edm.Int32"" DefaultValue=""-1"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The maximum number of navigation properties that can be traversed when addressing the collection to delete from or the entity to delete. A value of -1 indicates there is no restriction."" />
    </Property>
    <Property Name=""FilterSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of collections can be updated via a PATCH request with a `/$filter(...)/$each` segment"" />
    </Property>
    <Property Name=""TypecastSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of collections can be updated via a PATCH request with a type-cast segment and a `/$each` segment"" />
    </Property>
    <Property Name=""Permissions"" Type=""Collection(Capabilities.PermissionType)"">
      <Annotation Term=""Core.Description"" String=""Required permissions. One of the specified sets of scopes is required to perform the delete."" />
    </Property>
    <Property Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom headers"" />
    </Property>
    <Property Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom query options"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A brief description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""LongDescription"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A lengthy description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""CollectionPropertyRestrictionsType"">
    <Property Name=""CollectionProperty"" Type=""Edm.PropertyPath"">
      <Annotation Term=""Core.Description"" String=""Restricted Collection-valued property"" />
    </Property>
    <Property Name=""FilterFunctions"" Type=""Collection(Edm.String)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""List of functions and operators supported in filter expressions"" />
      <Annotation Term=""Core.LongDescription"" String=""If not specified, null, or empty, all functions and operators may be attempted."" />
    </Property>
    <Property Name=""FilterRestrictions"" Type=""Capabilities.FilterRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on filter expressions"" />
    </Property>
    <Property Name=""SearchRestrictions"" Type=""Capabilities.SearchRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on search expressions"" />
    </Property>
    <Property Name=""SortRestrictions"" Type=""Capabilities.SortRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions on orderby expressions"" />
    </Property>
    <Property Name=""TopSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $top"" />
    </Property>
    <Property Name=""SkipSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $skip"" />
    </Property>
    <Property Name=""SelectSupport"" Type=""Capabilities.SelectSupportType"">
      <Annotation Term=""Core.Description"" String=""Support for $select"" />
    </Property>
    <Property Name=""Insertable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members can be inserted into this collection"" />
      <Annotation Term=""Core.LongDescription"" String=""If additionally annotated with [Core.PositionalInsert](Org.OData.Core.V1.md#PositionalInsert), members can be inserted at a specific position"" />
    </Property>
    <Property Name=""Updatable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of this ordered collection can be updated by ordinal"" />
    </Property>
    <Property Name=""Deletable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Members of this ordered collection can be deleted by ordinal"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OperationRestrictionsType"">
    <Property Name=""FilterSegmentSupported"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Bound action or function can be invoked on a collection-valued binding parameter path with a `/$filter(...)` segment"" />
    </Property>
    <Property Name=""Permissions"" Type=""Collection(Capabilities.PermissionType)"">
      <Annotation Term=""Core.Description"" String=""Required permissions. One of the specified sets of scopes is required to invoke an action or function"" />
    </Property>
    <Property Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom headers"" />
    </Property>
    <Property Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom query options"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ModificationQueryOptionsType"">
    <Property Name=""ExpandSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $expand with modification requests"" />
    </Property>
    <Property Name=""SelectSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $select with modification requests"" />
    </Property>
    <Property Name=""ComputeSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $compute with modification requests"" />
    </Property>
    <Property Name=""FilterSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $filter with modification requests"" />
    </Property>
    <Property Name=""SearchSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $search with modification requests"" />
    </Property>
    <Property Name=""SortSupported"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supports $orderby with modification requests"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ReadRestrictionsBase"" Abstract=""true"">
    <Property Name=""Readable"" Type=""Edm.Boolean"" DefaultValue=""true"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Entities can be retrieved"" />
    </Property>
    <Property Name=""Permissions"" Type=""Collection(Capabilities.PermissionType)"">
      <Annotation Term=""Core.Description"" String=""Required permissions. One of the specified sets of scopes is required to read."" />
    </Property>
    <Property Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom headers"" />
    </Property>
    <Property Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Supported or required custom query options"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A brief description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
    <Property Name=""LongDescription"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A lengthy description of the request"" />
      <Annotation Term=""Core.IsLanguageDependent"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ReadByKeyRestrictionsType"" BaseType=""Capabilities.ReadRestrictionsBase"">
    <Annotation Term=""Core.Description"" String=""Restrictions for retrieving an entity by key"" />
  </ComplexType>
  <ComplexType Name=""ReadRestrictionsType"" BaseType=""Capabilities.ReadRestrictionsBase"">
    <Property Name=""ReadByKeyRestrictions"" Type=""Capabilities.ReadByKeyRestrictionsType"">
      <Annotation Term=""Core.Description"" String=""Restrictions for retrieving an entity by key"" />
      <Annotation Term=""Core.LongDescription"" String=""Only valid when applied to a collection. If a property of `ReadByKeyRestrictions` is not specified, the corresponding property value of `ReadRestrictions` applies."" />
    </Property>
  </ComplexType>
  <ComplexType Name=""CustomParameter"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Name of the custom parameter"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Description of the custom parameter"" />
    </Property>
    <Property Name=""DocumentationURL"" Type=""Edm.String"">
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
      <Annotation Term=""Core.Description"" String=""URL of related documentation"" />
    </Property>
    <Property Name=""Required"" Type=""Edm.Boolean"" DefaultValue=""false"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""true: parameter is required, false or not specified: parameter is optional"" />
    </Property>
    <Property Name=""ExampleValues"" Type=""Collection(Core.PrimitiveExampleValue)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Example values for the custom parameter"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""A custom parameter is either a header or a query option"" />
    <Annotation Term=""Core.LongDescription"" String=""The type of a custom parameter is always a string. Restrictions on the parameter values can be expressed by annotating the record expression describing the parameter with terms from the Validation vocabulary, e.g. Validation.Pattern or Validation.AllowedValues."" />
  </ComplexType>
  <EnumType Name=""ConformanceLevelType"">
    <Member Name=""Minimal"">
      <Annotation Term=""Core.Description"" String=""Minimal conformance level"" />
    </Member>
    <Member Name=""Intermediate"">
      <Annotation Term=""Core.Description"" String=""Intermediate conformance level"" />
    </Member>
    <Member Name=""Advanced"">
      <Annotation Term=""Core.Description"" String=""Advanced conformance level"" />
    </Member>
  </EnumType>
  <EnumType Name=""IsolationLevel"" IsFlags=""true"">
    <Member Name=""Snapshot"" Value=""1"">
      <Annotation Term=""Core.Description"" String=""All data returned for a request, including multiple requests within a batch or results retrieved across multiple pages, will be consistent as of a single point in time"" />
    </Member>
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
    <Member Name=""none"" Value=""0"">
      <Annotation Term=""Core.Description"" String=""Single search term"" />
    </Member>
    <Member Name=""AND"" Value=""1"">
      <Annotation Term=""Core.Description"" String=""Multiple search terms separated by `AND`"" />
    </Member>
    <Member Name=""OR"" Value=""2"">
      <Annotation Term=""Core.Description"" String=""Multiple search terms separated by `OR`"" />
    </Member>
    <Member Name=""NOT"" Value=""4"">
      <Annotation Term=""Core.Description"" String=""Search terms preceded by `NOT`"" />
    </Member>
    <Member Name=""phrase"" Value=""8"">
      <Annotation Term=""Core.Description"" String=""Search phrases enclosed in double quotes"" />
    </Member>
    <Member Name=""group"" Value=""16"">
      <Annotation Term=""Core.Description"" String=""Precedence grouping of search expressions with parentheses"" />
    </Member>
  </EnumType>
  <EnumType Name=""HttpMethod"" IsFlags=""true"">
    <Member Name=""GET"" Value=""1"">
      <Annotation Term=""Core.Description"" String=""The HTTP GET Method"" />
    </Member>
    <Member Name=""PATCH"" Value=""2"">
      <Annotation Term=""Core.Description"" String=""The HTTP PATCH Method"" />
    </Member>
    <Member Name=""PUT"" Value=""4"">
      <Annotation Term=""Core.Description"" String=""The HTTP PUT Method"" />
    </Member>
    <Member Name=""POST"" Value=""8"">
      <Annotation Term=""Core.Description"" String=""The HTTP POST Method"" />
    </Member>
    <Member Name=""DELETE"" Value=""16"">
      <Annotation Term=""Core.Description"" String=""The HTTP DELETE Method"" />
    </Member>
    <Member Name=""OPTIONS"" Value=""32"">
      <Annotation Term=""Core.Description"" String=""The HTTP OPTIONS Method"" />
    </Member>
    <Member Name=""HEAD"" Value=""64"">
      <Annotation Term=""Core.Description"" String=""The HTTP HEAD Method"" />
    </Member>
  </EnumType>
  <Term Name=""ConformanceLevel"" Type=""Capabilities.ConformanceLevelType"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The conformance level achieved by this service"" />
  </Term>
  <Term Name=""SupportedFormats"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Media types of supported formats, including format parameters"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
  </Term>
  <Term Name=""SupportedMetadataFormats"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Media types of supported formats for $metadata, including format parameters"" />
    <Annotation Term=""Core.IsMediaType"" Bool=""true"" />
  </Term>
  <Term Name=""AcceptableEncodings"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""List of acceptable compression methods for ($batch) requests, e.g. gzip"" />
  </Term>
  <Term Name=""AsynchronousRequestsSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Service supports the asynchronous request preference"" />
  </Term>
  <Term Name=""BatchContinueOnErrorSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Service supports the continue on error preference. Supports $batch requests. Services that apply the BatchContinueOnErrorSupported term should also specify the ContinueOnErrorSupported property from the BatchSupport term."" />
  </Term>
  <Term Name=""IsolationSupported"" Type=""Capabilities.IsolationLevel"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supported odata.isolation levels"" />
  </Term>
  <Term Name=""CrossJoinSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports cross joins for the entity sets in this container"" />
  </Term>
  <Term Name=""CallbackSupported"" Type=""Capabilities.CallbackType"" AppliesTo=""EntityContainer EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports callbacks for the specified protocols"" />
  </Term>
  <Term Name=""ChangeTracking"" Type=""Capabilities.ChangeTrackingType"" AppliesTo=""EntitySet Singleton Function FunctionImport NavigationProperty"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Change tracking capabilities of this service or entity set"" />
  </Term>
  <Term Name=""CountRestrictions"" Type=""Capabilities.CountRestrictionsType"" AppliesTo=""EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on /$count path suffix and $count=true system query option"" />
  </Term>
  <Term Name=""NavigationRestrictions"" Type=""Capabilities.NavigationRestrictionsType"" AppliesTo=""EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on navigating properties according to OData URL conventions"" />
    <Annotation Term=""Core.LongDescription"" String=""Restrictions specified on an entity set are valid whether the request is directly to the entity set or through a navigation property bound to that entity set. Services can specify a different set of restrictions specific to a path, in which case the more specific restrictions take precedence."" />
  </Term>
  <Term Name=""IndexableByKey"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports key values according to OData URL conventions"" />
  </Term>
  <Term Name=""TopSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports $top"" />
  </Term>
  <Term Name=""SkipSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports $skip"" />
  </Term>
  <Term Name=""ComputeSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports $compute"" />
  </Term>
  <Term Name=""SelectSupport"" Type=""Capabilities.SelectSupportType"" AppliesTo=""EntityContainer EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Support for $select and nested query options within $select"" />
  </Term>
  <Term Name=""BatchSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports $batch requests. Services that apply the BatchSupported term should also apply the more comprehensive BatchSupport term."" />
  </Term>
  <Term Name=""BatchSupport"" Type=""Capabilities.BatchSupportType"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Batch Support for the service"" />
  </Term>
  <Term Name=""FilterFunctions"" Type=""Collection(Edm.String)"" AppliesTo=""EntityContainer EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""List of functions and operators supported in filter expressions"" />
    <Annotation Term=""Core.LongDescription"" String=""If not specified, null, or empty, all functions and operators may be attempted."" />
  </Term>
  <Term Name=""FilterRestrictions"" Type=""Capabilities.FilterRestrictionsType"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on filter expressions"" />
  </Term>
  <Term Name=""SortRestrictions"" Type=""Capabilities.SortRestrictionsType"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on orderby expressions"" />
  </Term>
  <Term Name=""ExpandRestrictions"" Type=""Capabilities.ExpandRestrictionsType"" AppliesTo=""EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on expand expressions"" />
  </Term>
  <Term Name=""SearchRestrictions"" Type=""Capabilities.SearchRestrictionsType"" AppliesTo=""EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on search expressions"" />
  </Term>
  <Term Name=""KeyAsSegmentSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports [key-as-segment convention](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#sec_KeyasSegmentConvention) for addressing entities within a collection"" />
  </Term>
  <Term Name=""QuerySegmentSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports [passing query options in the request body](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#sec_PassingQueryOptionsintheRequestBody)"" />
  </Term>
  <Term Name=""InsertRestrictions"" Type=""Capabilities.InsertRestrictionsType"" AppliesTo=""EntitySet EntityType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on insert operations"" />
  </Term>
  <Term Name=""DeepInsertSupport"" Type=""Capabilities.DeepInsertSupportType"" AppliesTo=""EntityContainer EntitySet"">
    <Annotation Term=""Core.Description"" String=""Deep Insert Support of the annotated resource (the whole service, an entity set, or a collection-valued resource)"" />
  </Term>
  <Term Name=""UpdateRestrictions"" Type=""Capabilities.UpdateRestrictionsType"" AppliesTo=""EntitySet Singleton EntityType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on update operations"" />
  </Term>
  <Term Name=""DeepUpdateSupport"" Type=""Capabilities.DeepUpdateSupportType"" AppliesTo=""EntityContainer EntitySet"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Deep Update Support of the annotated resource (the whole service, an entity set, or a collection-valued resource)"" />
  </Term>
  <Term Name=""DeleteRestrictions"" Type=""Capabilities.DeleteRestrictionsType"" AppliesTo=""EntitySet Singleton EntityType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions on delete operations"" />
  </Term>
  <Term Name=""CollectionPropertyRestrictions"" Type=""Collection(Capabilities.CollectionPropertyRestrictionsType)"" AppliesTo=""EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Describes restrictions on operations applied to collection-valued structural properties"" />
  </Term>
  <Term Name=""OperationRestrictions"" Type=""Capabilities.OperationRestrictionsType"" AppliesTo=""Action Function"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions for function or action operation"" />
  </Term>
  <Term Name=""AnnotationValuesInQuerySupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Supports annotation values within system query options"" />
  </Term>
  <Term Name=""ModificationQueryOptions"" Type=""Capabilities.ModificationQueryOptionsType"" AppliesTo=""EntityContainer Action ActionImport"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Support for query options with modification requests (insert, update, action invocation)"" />
  </Term>
  <Term Name=""ReadRestrictions"" Type=""Capabilities.ReadRestrictionsType"" AppliesTo=""EntitySet Singleton"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Restrictions for retrieving a collection of entities, retrieving a singleton instance."" />
  </Term>
  <Term Name=""CustomHeaders"" Type=""Collection(Capabilities.CustomParameter)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Custom headers that are supported/required for the annotated resource"" />
    <Annotation Term=""Core.Example"">
      <Record />
    </Annotation>
  </Term>
  <Term Name=""CustomQueryOptions"" Type=""Collection(Capabilities.CustomParameter)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Custom query options that are supported/required for the annotated resource"" />
    <Annotation Term=""Core.LongDescription"" String=""If the entity container is annotated, the query option is supported/required by all resources in that container."" />
    <Annotation Term=""Core.Example"">
      <Record />
    </Annotation>
  </Term>
  <Term Name=""MediaLocationUpdateSupported"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Property"" Nullable=""false"">
    <Annotation Term=""Core.RequiresType"" String=""Edm.Stream"" />
    <Annotation Term=""Core.Description"" String=""Stream property supports update of its media edit URL and/or media read URL"" />
  </Term>
</Schema>";

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            IEnumerable<EdmError> errors;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            this.capVocModel.TryWriteSchema(xw, out errors);
            xw.Flush();
#if NETCOREAPP1_1
            xw.Dispose();
#else
            xw.Close();
#endif
            string output = sw.ToString();
            Assert.True(!errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }

        [Fact]
        public void TestCapabilitiesVocabularyDependsOnCoreAuthorizationValidationVocabularies()
        {
            var referencedModels = this.capVocModel.ReferencedModels;

            Assert.NotNull(referencedModels);

            Assert.Equal(4, referencedModels.Count());

            Assert.Contains(EdmCoreModel.Instance, referencedModels);
            Assert.Contains(CoreVocabularyModel.Instance, referencedModels);
            Assert.Contains(ValidationVocabularyModel.Instance, referencedModels);
            Assert.Contains(AuthorizationVocabularyModel.Instance, referencedModels);
        }

        [Theory]
        [InlineData("AsynchronousRequestsSupported")]
        [InlineData("BatchContinueOnErrorSupported")]
        public void TestCapabilitiesVocabularyReferenceCoreVocabularyTypesAndTerms(string termName)
        {
            var term = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1." + termName);
            Assert.NotNull(term);

            // Core.Tag
            Assert.NotNull(term.Type);

            Assert.Equal(EdmTypeKind.TypeDefinition, term.Type.Definition.TypeKind);
            Assert.Equal("Org.OData.Core.V1.Tag", term.Type.Definition.FullTypeName());

            // Core.Description
            var annotations = this.capVocModel.FindDeclaredVocabularyAnnotations(term).ToList();
            Assert.Single(annotations);
            var description = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsTerm && a.Term.Name == "Description");
            Assert.NotNull(description);
            Assert.Equal("Org.OData.Core.V1", description.Term.Namespace);
        }

        [Fact]
        public void TestCapabilitiesVocabularyReferenceMultiCoreVocabularyTerms()
        {
            var supportedFormats = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.SupportedFormats");
            Assert.NotNull(supportedFormats);

            var annotations = this.capVocModel.FindDeclaredVocabularyAnnotations(supportedFormats).ToList();
            Assert.Equal(2, annotations.Count());

            // Core.Description
            var description = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsTerm && a.Term.Name == "Description");
            Assert.NotNull(description);
            Assert.Equal("Org.OData.Core.V1", description.Term.Namespace);

            // Core.IsMediaType
            var isMediaType = annotations.SingleOrDefault(a => a.Term is CsdlSemanticsTerm && a.Term.Name == "IsMediaType");
            Assert.NotNull(isMediaType);
            Assert.Equal("Org.OData.Core.V1", isMediaType.Term.Namespace);
        }

        [Fact]
        public void TestCapabilitiesVocabularyChangeTracking()
        {
            var changeTerm = this.capVocModel.FindDeclaredTerm(CapabilitiesVocabularyConstants.ChangeTracking);
            Assert.NotNull(changeTerm);
            Assert.Equal("Org.OData.Capabilities.V1", changeTerm.Namespace);
            Assert.Equal("ChangeTracking", changeTerm.Name);

            var type = changeTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.ChangeTrackingType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingSupported);
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingFilterableProperties);
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty(CapabilitiesVocabularyConstants.ChangeTrackingExpandableProperties);
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [Fact]
        public void TestCapabilitiesVocabularyCountRestrictions()
        {
            var countTerm = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.CountRestrictions");
            Assert.NotNull(countTerm);
            Assert.Equal("Org.OData.Capabilities.V1", countTerm.Namespace);
            Assert.Equal("CountRestrictions", countTerm.Name);

            var type = countTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.CountRestrictionsType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty("Countable");
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("NonCountableProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
            Assert.Equal(EdmTypeKind.Path, p.Type.AsCollection().ElementType().TypeKind());
            IEdmPathType pathType = p.Type.AsCollection().ElementType().AsPath().Definition as IEdmPathType;
            Assert.NotNull(pathType);
            Assert.Equal("PropertyPath", pathType.Name);
            Assert.Equal("Edm", pathType.Namespace);
            Assert.Equal("Edm.PropertyPath", pathType.FullName());

            p = complexType.FindProperty("NonCountableNavigationProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [Fact]
        public void TestCapabilitiesVocabularyNavigationRestrictions()
        {
            var navigationTerm = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.NavigationRestrictions");
            Assert.NotNull(navigationTerm);
            Assert.Equal("Org.OData.Capabilities.V1", navigationTerm.Namespace);
            Assert.Equal("NavigationRestrictions", navigationTerm.Name);

            var type = navigationTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.NavigationRestrictionsType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty("Navigability");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Enum, p.Type.Definition.TypeKind);

            var navType = p.Type.Definition as IEdmEnumType;
            Assert.NotNull(navType);
            Assert.Equal(3, navType.Members.Count());
            Assert.Equal("Recursive|Single|None", String.Join("|", navType.Members.Select(e => e.Name)));

            p = complexType.FindProperty("RestrictedProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            IEdmCollectionType collection = p.Type.Definition as IEdmCollectionType;
            Assert.NotNull(collection);
            var propertyResType = collection.ElementType.Definition as IEdmComplexType;
            Assert.NotNull(propertyResType);
            Assert.Equal("Org.OData.Capabilities.V1.NavigationPropertyRestriction", propertyResType.FullName());

            p = propertyResType.FindProperty("NavigationProperty");
            Assert.NotNull(p);

            p = propertyResType.FindProperty("Navigability");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Enum, p.Type.Definition.TypeKind);

            p = propertyResType.FindProperty("FilterFunctions");
            Assert.NotNull(p);
            Assert.Equal("Collection(Edm.String)", p.Type.FullName());

            p = propertyResType.FindProperty("FilterRestrictions");
            Assert.NotNull(p);
            Assert.Equal("Org.OData.Capabilities.V1.FilterRestrictionsType", p.Type.FullName());
        }

        [Fact]
        public void TestCapabilitiesVocabularyFilterRestrictions()
        {
            var filterTerm = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.FilterRestrictions");
            Assert.NotNull(filterTerm);
            Assert.Equal("Org.OData.Capabilities.V1", filterTerm.Namespace);
            Assert.Equal("FilterRestrictions", filterTerm.Name);

            var type = filterTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.FilterRestrictionsType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty("Filterable");
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("RequiresFilter");
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("RequiredProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("NonFilterableProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("FilterExpressionRestrictions");
            Assert.NotNull(p);
            Assert.Equal("Collection(Org.OData.Capabilities.V1.FilterExpressionRestrictionType)", p.Type.FullName());

            p = complexType.FindProperty("MaxLevels");
            Assert.NotNull(p);
            Assert.Equal("Edm.Int32", p.Type.FullName());
        }

        [Fact]
        public void TestCapabilitiesVocabularySortRestrictions()
        {
            var sortTerm = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.SortRestrictions");
            Assert.NotNull(sortTerm);
            Assert.Equal("Org.OData.Capabilities.V1", sortTerm.Namespace);
            Assert.Equal("SortRestrictions", sortTerm.Name);

            var type = sortTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.SortRestrictionsType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty("Sortable");
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("AscendingOnlyProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("DescendingOnlyProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            p = complexType.FindProperty("NonSortableProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);
        }

        [Fact]
        public void TestCapabilitiesVocabularyExpandRestrictions()
        {
            var expandTerm = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.ExpandRestrictions");
            Assert.NotNull(expandTerm);
            Assert.Equal("Org.OData.Capabilities.V1", expandTerm.Namespace);
            Assert.Equal("ExpandRestrictions", expandTerm.Name);

            var type = expandTerm.Type;
            Assert.Equal("Org.OData.Capabilities.V1.ExpandRestrictionsType", type.FullName());
            Assert.Equal(EdmTypeKind.Complex, type.Definition.TypeKind);

            var complexType = type.Definition as IEdmComplexType;
            Assert.NotNull(complexType);
            var p = complexType.FindProperty("Expandable");
            Assert.NotNull(p);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, p.Type.PrimitiveKind());

            p = complexType.FindProperty("NonExpandableProperties");
            Assert.NotNull(p);
            Assert.Equal(EdmTypeKind.Collection, p.Type.Definition.TypeKind);

            Assert.Equal(EdmTypeKind.Path, p.Type.AsCollection().ElementType().TypeKind());
            IEdmPathType pathType = p.Type.AsCollection().ElementType().AsPath().Definition as IEdmPathType;
            Assert.NotNull(pathType);
            Assert.Equal("NavigationPropertyPath", pathType.Name);
            Assert.Equal("Edm", pathType.Namespace);
            Assert.Equal("Edm.NavigationPropertyPath", pathType.FullName());

            p = complexType.FindProperty("MaxLevels");
            Assert.NotNull(p);
            Assert.Equal("Edm.Int32", p.Type.FullName());
        }

        [Theory]
        [InlineData("InsertRestrictionsType", true, true)]
        [InlineData("UpdateRestrictionsType", true, true)]
        [InlineData("DeleteRestrictionsType", false, true)]
        [InlineData("OperationRestrictionsType", false, false)]
        [InlineData("ReadRestrictionsBase", false, true)]
        public void TestCapabilitiesVocabularyComplexTypeWithPermissions(string typeName, bool hasQueryOptions, bool hasDescription)
        {
            var capType = this.capVocModel.FindDeclaredType("Org.OData.Capabilities.V1." + typeName);
            Assert.NotNull(capType);

            IEdmComplexType capComplexType = capType as IEdmComplexType;
            Assert.NotNull(capComplexType);

            var permissions = capComplexType.FindProperty("Permissions");
            Assert.NotNull(permissions);
            Assert.Equal("Collection(Org.OData.Capabilities.V1.PermissionType)", permissions.Type.FullName());

            var queryOptions = capComplexType.FindProperty("QueryOptions");
            if (hasQueryOptions)
            {
                Assert.NotNull(queryOptions);
                Assert.Equal("Org.OData.Capabilities.V1.ModificationQueryOptionsType", queryOptions.Type.FullName());
            }
            else
            {
                Assert.Null(queryOptions);
            }

            var customHeaders = capComplexType.FindProperty("CustomHeaders");
            Assert.NotNull(customHeaders);
            Assert.Equal("Collection(Org.OData.Capabilities.V1.CustomParameter)", customHeaders.Type.FullName());

            var customQueryOptions = capComplexType.FindProperty("CustomQueryOptions");
            Assert.NotNull(customQueryOptions);
            Assert.Equal("Collection(Org.OData.Capabilities.V1.CustomParameter)", customQueryOptions.Type.FullName());

            var description = capComplexType.FindProperty("Description");
            if (hasDescription)
            {
                Assert.NotNull(description);
                Assert.Equal("Edm.String", description.Type.FullName());
            }
            else
            {
                Assert.Null(description);
            }

            var longDescription = capComplexType.FindProperty("LongDescription");
            if (hasDescription)
            {
                Assert.NotNull(longDescription);
                Assert.Equal("Edm.String", longDescription.Type.FullName());
            }
            else
            {
                Assert.Null(longDescription);
            }
        }

        [Fact]
        public void TestCapabilitiesVocabularyConformanceLevel()
        {
            var confLevel = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.ConformanceLevel");
            Assert.NotNull(confLevel);
            Assert.Equal("Org.OData.Capabilities.V1", confLevel.Namespace);
            Assert.Equal("ConformanceLevel", confLevel.Name);

            var type = confLevel.Type;
            Assert.Equal("Org.OData.Capabilities.V1.ConformanceLevelType", type.FullName());
            Assert.Equal(EdmTypeKind.Enum, type.Definition.TypeKind);

            var enumType = type.Definition as IEdmEnumType;
            Assert.NotNull(enumType);
            Assert.Equal(3, enumType.Members.Count());
            Assert.Equal("Minimal|Intermediate|Advanced", String.Join("|", enumType.Members.Select(e => e.Name)));
        }

        [Fact]
        public void TestCapabilitiesVocabularySupportedFormats()
        {
            var supportedFormats = this.capVocModel.FindDeclaredTerm("Org.OData.Capabilities.V1.SupportedFormats");
            Assert.NotNull(supportedFormats);
            Assert.Equal("Org.OData.Capabilities.V1", supportedFormats.Namespace);
            Assert.Equal("SupportedFormats", supportedFormats.Name);

            var type = supportedFormats.Type;
            Assert.Equal("Collection(Edm.String)", type.FullName());
        }

        [Theory]
        [InlineData("AsynchronousRequestsSupported", "EntityContainer")]
        [InlineData("BatchContinueOnErrorSupported", "EntityContainer")]
        [InlineData("CrossJoinSupported", "EntityContainer")]
        [InlineData("TopSupported", "EntitySet")]
        [InlineData("SkipSupported", "EntitySet")]
        [InlineData("IndexableByKey", "EntitySet")]
        [InlineData("BatchSupported", "EntityContainer")]
        [InlineData("KeyAsSegmentSupported", "EntityContainer")]
        public void TestCapabilitiesVocabularySupportedTerm(string name, string appliesTo)
        {
            string qualifiedName = "Org.OData.Capabilities.V1." + name;
            var supported = this.capVocModel.FindDeclaredTerm(qualifiedName);
            Assert.NotNull(supported);
            Assert.Equal("Org.OData.Capabilities.V1", supported.Namespace);
            Assert.Equal(name, supported.Name);

            Assert.Equal(appliesTo, supported.AppliesTo);
            var type = supported.Type;
            Assert.Equal("Org.OData.Core.V1.Tag", type.FullName());

            Assert.Equal(EdmTypeKind.TypeDefinition, type.TypeKind());
            IEdmTypeDefinitionReference typeDefinitionReference = type.AsTypeDefinition();
            Assert.NotNull(typeDefinitionReference);

            Assert.Equal(EdmPrimitiveTypeKind.Boolean, typeDefinitionReference.TypeDefinition().UnderlyingType.PrimitiveKind);
        }

        [Fact]
        public void TestCapabilitiesVocabularyDependentOnValidationVocabulary()
        {
            var batchSupportType = this.capVocModel.FindDeclaredType("Org.OData.Capabilities.V1.BatchSupportType");
            Assert.NotNull(batchSupportType);
            Assert.Equal("Org.OData.Capabilities.V1", batchSupportType.Namespace);
            Assert.Equal("BatchSupportType", batchSupportType.Name);
            Assert.Equal(EdmTypeKind.Complex, batchSupportType.TypeKind);
            var complexType = batchSupportType as IEdmComplexType;

            IEdmProperty edmProperty = complexType.FindProperty("SupportedFormats");
            Assert.NotNull(edmProperty);

            var allowedValuesTerm = ValidationVocabularyModel.Instance.FindDeclaredTerm("Org.OData.Validation.V1.AllowedValues");
            Assert.NotNull(allowedValuesTerm);
            var annotation = this.capVocModel.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(edmProperty, allowedValuesTerm).FirstOrDefault();
            Assert.NotNull(annotation);
            Assert.NotNull(annotation.Value);

            IEdmCollectionExpression collectionExpression = annotation.Value as IEdmCollectionExpression;
            Assert.NotNull(collectionExpression);
            Assert.Equal(2, collectionExpression.Elements.Count());
            foreach(var item in collectionExpression.Elements)
            {
                IEdmRecordExpression recordExpress = item as IEdmRecordExpression;
                Assert.NotNull(recordExpress);

                var recordProperty = recordExpress.FindProperty("Value");
                Assert.NotNull(recordProperty);

                Assert.Contains(((IEdmStringConstantExpression)recordProperty.Value).Value, new[] { "multipart/mixed", "application/json" });
            }
        }
    }
}
