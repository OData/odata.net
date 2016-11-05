//---------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

// Violations triggered by using terms from our spec could not solve them by adding the word(s) to the FxCop custom dictionary
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.Core.ODataUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.Core.UriParser.ODataUriUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.Core.UriParser.ODataQueryUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "mediaresource", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "src", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "The name of the src attribute on the atom:content element.")]

// Violations triggered by using various terms comming from other specs
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "dataservices", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "Part of an XML namespace URL, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Rel", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "LinkRel is a valid identifier used in the public API.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "rel", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "ATOM attribute name, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Edm", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "$orderby is a keyword in the URL query language.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "innererror", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "JSON property name, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "XmlNodeType.Whitespace is an existing API which we can't fix..")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "URIs", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "odata", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "setexpression", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "charset", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "yyyy-mm-dd", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Thh", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "zzzzzz", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "ss", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// Violations in the generated Resource file; can't prevent these from being generated.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.Error.#ArgumentNull(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.Error.#ArgumentOutOfRange(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.Error.#NotImplemented()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.Error.#NotSupported()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.TextRes.#GetString(System.String,System.Object[])")]

// Violations caused by namespaces having too few public types.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Core.Metadata")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Core.Evaluation")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Core.JsonLight")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Core.UriParser.Metadata")]

// Types forwarded from Astoria client to be shared with ODataLib.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Service.Common", Justification = "Types are shared with Astoria client.")]

//// FxCop does not recognize the suppress message attributes on the members; have to make them global suppressions.
[module: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Core.UriParser.BinaryOperator.#ctor(System.String,System.Int16,System.Boolean)", Scope = "member", Target = "Microsoft.OData.Core.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eq", Scope = "member", Target = "Microsoft.OData.Core.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "gt", Scope = "member", Target = "Microsoft.OData.Core.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "mul", Scope = "member", Target = "Microsoft.OData.Core.UriParser.BinaryOperator.#.cctor()")]

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "NonEntity", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// Normalize strings to uppercase
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.Core.ODataPreferenceHeader.#ReturnContent")]
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.ExpandOptionParser.#BuildExpandTermToken(Microsoft.OData.Core.UriParser.Syntactic.PathSegmentToken,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.FunctionCallBinder.#BindAsUriFunction(Microsoft.OData.Core.UriParser.Syntactic.FunctionCallToken,System.Collections.Generic.List`1<Microsoft.OData.Core.UriParser.Semantic.QueryNode>)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.ExpandOptionParser.#BuildStarExpandTermToken(Microsoft.OData.Core.UriParser.Syntactic.PathSegmentToken)")]

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "NonEntity", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// By design and already public APIs thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.Core.ODataInputContext.#ReadProperty(Microsoft.OData.Edm.IEdmStructuralProperty,Microsoft.OData.Edm.IEdmTypeReference)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.Core.ODataInputContext.#ReadPropertyAsync(Microsoft.OData.Edm.IEdmStructuralProperty,Microsoft.OData.Edm.IEdmTypeReference)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.Core.ODataOutputContext.#WriteProperty(Microsoft.OData.Core.ODataProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.Core.ODataOutputContext.#WritePropertyAsync(Microsoft.OData.Core.ODataProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Scope = "member", Target = "Microsoft.OData.Core.ODataOutputContext.#WriteError(Microsoft.OData.Core.ODataError,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Scope = "member", Target = "Microsoft.OData.Core.ODataOutputContext.#WriteErrorAsync(Microsoft.OData.Core.ODataError,System.Boolean)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Scope = "member", Target = "Microsoft.OData.Core.UriUtils.#CreateUriAsEntryOrFeedId(System.String,System.UriKind,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.EnumBinder.#bindMethod")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Core.Evaluation.ODataEntryMetadataContext+ODataEntryMetadataContextWithoutModel.#serializationInfo")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Core.Evaluation.ODataMetadataContext.#metadataLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Core.JsonLight.ODataJsonLightReader.#readingParameter")]

// Already public APIs thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.Core.ODataBatchOperationRequestMessage.#ContentId")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.Core.ODataBatchOperationResponseMessage.#ContentId")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.Core.ODataMediaType.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Semantic.ODataUnresolvedFunctionParameterAlias.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Microsoft.OData.Core.Json.JsonValueUtils.#.cctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.JsonLight.ODataJsonLightDeltaWriter.#CreateDeltaFeedScope(Microsoft.OData.Core.ODataItem,Microsoft.OData.Edm.IEdmNavigationSource,Microsoft.OData.Edm.IEdmEntityType,Microsoft.OData.Core.SelectedPropertiesNode,Microsoft.OData.Core.ODataUri)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.ODataPathParser.#ThrowIfMustBeLeafSegment(Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.SelectExpandBinder.#BuildDefaultSubExpand()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.SelectExpandBinder.#ParseLevels(System.Nullable`1<System.Int64>,Microsoft.OData.Edm.IEdmType,Microsoft.OData.Edm.IEdmNavigationProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.SelectExpandSemanticBinder.#Bind(Microsoft.OData.Edm.IEdmStructuredType,Microsoft.OData.Edm.IEdmNavigationSource,Microsoft.OData.Core.UriParser.Syntactic.ExpandToken,Microsoft.OData.Core.UriParser.Syntactic.SelectToken,Microsoft.OData.Core.UriParser.ODataUriParserConfiguration)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.SelectTreeNormalizer.#NormalizeSelectTree(Microsoft.OData.Core.UriParser.Syntactic.SelectToken)")]

// Current design actually can save cast effort.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.DottedIdentifierBinder.#BindDottedIdentifier(Microsoft.OData.Core.UriParser.Syntactic.DottedIdentifierToken)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.FunctionCallBinder.#TryBindIdentifier(System.String,System.Collections.Generic.IEnumerable`1<Microsoft.OData.Core.UriParser.Syntactic.FunctionParameterToken>,Microsoft.OData.Core.UriParser.Semantic.QueryNode,Microsoft.OData.Core.UriParser.Parsers.BindingState,Microsoft.OData.Core.UriParser.Semantic.QueryNode&)")]

// In lambda expression.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Visitors.SelectPropertyVisitor.#ProcessTokenAsPath(Microsoft.OData.Core.UriParser.Syntactic.NonSystemToken)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.Core.JsonLight.ODataJsonLightDeltaWriter.#StartDeltaLink(Microsoft.OData.Core.ODataDeltaLinkBase)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Core.ODataAsynchronousReader.#ParseResponseLine(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.JsonLight.ODataJsonLightDeltaReader.#IsTopLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Core.JsonLight.ODataJsonLightDeltaWriter.#IsTopLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Microsoft.OData.Core.UriParser.FunctionSignatureWithReturnType.#ArgumentTypes")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.OData.Core.UriParser.Parsers.UriPrimitiveTypeParser.#TryUriStringToPrimitive(System.String,Microsoft.OData.Edm.IEdmTypeReference,System.Object&,Microsoft.OData.Core.UriParser.Parsers.Common.UriLiteralParsingException&)")]
