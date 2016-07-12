//---------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

// Violations triggered by using terms from our spec could not solve them by adding the word(s) to the FxCop custom dictionary
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "resource", Target = "Microsoft.OData.Core.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.ODataUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.ODataUriUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.OData.UriParser.ODataQueryUtils")]
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
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Error.#ArgumentNull(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Error.#ArgumentOutOfRange(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Error.#NotImplemented()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Error.#NotSupported()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.TextRes.#GetString(System.String,System.Object[])")]

// Violations caused by namespaces having too few public types.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Evaluation")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.JsonLight")]

// Types forwarded from Astoria client to be shared with ODataLib.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Service.Common", Justification = "Types are shared with Astoria client.")]

//// FxCop does not recognize the suppress message attributes on the members; have to make them global suppressions.
[module: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.UriParser.BinaryOperator.#ctor(System.String,System.Int16,System.Boolean)", Scope = "member", Target = "Microsoft.OData.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eq", Scope = "member", Target = "Microsoft.OData.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "gt", Scope = "member", Target = "Microsoft.OData.UriParser.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "mul", Scope = "member", Target = "Microsoft.OData.UriParser.BinaryOperator.#.cctor()")]

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "NonEntity", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// Normalize strings to uppercase
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.ODataPreferenceHeader.#ReturnContent")]
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ExpandOptionParser.#BuildExpandTermToken(Microsoft.OData.UriParser.PathSegmentToken,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionCallBinder.#BindAsUriFunction(Microsoft.OData.UriParser.FunctionCallToken,System.Collections.Generic.List`1<Microsoft.OData.UriParser.QueryNode>)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ExpandOptionParser.#BuildStarExpandTermToken(Microsoft.OData.UriParser.PathSegmentToken)")]

[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "NonEntity", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// By design and already public APIs thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.ODataInputContext.#ReadProperty(Microsoft.OData.Edm.IEdmStructuralProperty,Microsoft.OData.Edm.IEdmTypeReference)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.ODataInputContext.#ReadPropertyAsync(Microsoft.OData.Edm.IEdmStructuralProperty,Microsoft.OData.Edm.IEdmTypeReference)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.ODataOutputContext.#WriteProperty(Microsoft.OData.ODataProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Scope = "member", Target = "Microsoft.OData.ODataOutputContext.#WritePropertyAsync(Microsoft.OData.ODataProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Scope = "member", Target = "Microsoft.OData.ODataOutputContext.#WriteError(Microsoft.OData.ODataError,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Scope = "member", Target = "Microsoft.OData.ODataOutputContext.#WriteErrorAsync(Microsoft.OData.ODataError,System.Boolean)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Scope = "member", Target = "Microsoft.OData.UriUtils.#CreateUriAsEntryOrFeedId(System.String,System.UriKind,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.UriParser.EnumBinder.#bindMethod")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Evaluation.ODataResourceMetadataContext+ODataResourceMetadataContextWithoutModel.#serializationInfo")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Evaluation.ODataMetadataContext.#metadataLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightReader.#readingParameter")]

// Already public APIs thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.ODataBatchOperationRequestMessage.#ContentId")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.ODataBatchOperationResponseMessage.#ContentId")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.ODataMediaType.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.UriParser.ODataUnresolvedFunctionParameterAlias.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.ODataTypeAnnotation.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Microsoft.OData.Json.JsonValueUtils.#.cctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter.#CreateDeltaResourceSetScope(Microsoft.OData.ODataItem,Microsoft.OData.Edm.IEdmNavigationSource,Microsoft.OData.Edm.IEdmEntityType,Microsoft.OData.SelectedPropertiesNode,Microsoft.OData.ODataUri)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.ODataPathParser.#ThrowIfMustBeLeafSegment(Microsoft.OData.UriParser.ODataPathSegment)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectExpandBinder.#BuildDefaultSubExpand()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectExpandBinder.#ParseLevels(System.Nullable`1<System.Int64>,Microsoft.OData.Edm.IEdmType,Microsoft.OData.Edm.IEdmNavigationProperty)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectExpandSemanticBinder.#Bind(Microsoft.OData.Edm.IEdmStructuredType,Microsoft.OData.Edm.IEdmNavigationSource,Microsoft.OData.UriParser.ExpandToken,Microsoft.OData.UriParser.SelectToken,Microsoft.OData.UriParser.ODataUriParserConfiguration)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectTreeNormalizer.#NormalizeSelectTree(Microsoft.OData.UriParser.SelectToken)")]

// Current design actually can save cast effort.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.DottedIdentifierBinder.#BindDottedIdentifier(Microsoft.OData.UriParser.DottedIdentifierToken)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionCallBinder.#TryBindIdentifier(System.String,System.Collections.Generic.IEnumerable`1<Microsoft.OData.UriParser.FunctionParameterToken>,Microsoft.OData.UriParser.QueryNode,Microsoft.OData.UriParser.BindingState,Microsoft.OData.UriParser.QueryNode&)")]

// In lambda expression.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.SelectPropertyVisitor.#ProcessTokenAsPath(Microsoft.OData.UriParser.NonSystemToken)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter.#StartDeltaLink(Microsoft.OData.ODataDeltaLinkBase)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.ODataAsynchronousReader.#ParseResponseLine(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaReader.#IsTopLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter.#IsTopLevel")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionSignatureWithReturnType.#ArgumentTypes")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.OData.UriParser.UriPrimitiveTypeParser.#TryUriStringToPrimitive(System.String,Microsoft.OData.Edm.IEdmTypeReference,System.Object&,Microsoft.OData.UriParser.UriLiteralParsingException&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`2(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`1(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`2(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime,System.Func`2<System.IServiceProvider,!!1>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ODataUriParser.#IsODataQueryOption(System.String)")]
