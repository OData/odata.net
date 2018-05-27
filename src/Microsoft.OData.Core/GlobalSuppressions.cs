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
[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace", Scope = "resource", Target = "Microsoft.OData.Core.resources", Justification = "XmlNodeType.Whitespace is an existing API which we can't fix..")]

// Normalize strings to uppercase
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.ODataPreferenceHeader.#ReturnContent")]
[module: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ExpandOptionParser.#BuildExpandTermToken(Microsoft.OData.UriParser.PathSegmentToken,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionCallBinder.#BindAsUriFunction(Microsoft.OData.UriParser.FunctionCallToken,System.Collections.Generic.List`1<Microsoft.OData.UriParser.QueryNode>)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ExpandOptionParser.#BuildStarExpandTermToken(Microsoft.OData.UriParser.PathSegmentToken)")]
[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "NonEntity", Scope = "resource", Target = "Microsoft.OData.Core.resources")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Evaluation.ODataResourceMetadataContext+ODataResourceMetadataContextWithoutModel.#serializationInfo")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Evaluation.ODataMetadataContext.#metadataLevel")]

// Already public APIs thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.ODataBatchOperationRequestMessage.#ContentId")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "Microsoft.OData.ODataBatchOperationResponseMessage.#ContentId")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.ODataMediaType.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Microsoft.OData.UriParser.ODataUnresolvedFunctionParameterAlias.#Type")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectExpandSemanticBinder.#Bind(Microsoft.OData.UriParser.ODataPathInfo,Microsoft.OData.UriParser.ExpandToken,Microsoft.OData.UriParser.SelectToken,Microsoft.OData.UriParser.ODataUriParserConfiguration)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.UriParser.SelectTreeNormalizer.#NormalizeSelectTree(Microsoft.OData.UriParser.SelectToken)")]

// Current design actually can save cast effort.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionCallBinder.#TryBindIdentifier(System.String,System.Collections.Generic.IEnumerable`1<Microsoft.OData.UriParser.FunctionParameterToken>,Microsoft.OData.UriParser.QueryNode,Microsoft.OData.UriParser.BindingState,Microsoft.OData.UriParser.QueryNode&)")]

// In lambda expression.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.SelectPropertyVisitor.#ProcessTokenAsPath(Microsoft.OData.UriParser.NonSystemToken)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter.#StartDeltaLink(Microsoft.OData.ODataDeltaLinkBase)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaReader.#IsTopLevel", Justification = "Used in debug builds in assertions.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter.#IsTopLevel", Justification = "Used in debug builds in assertions.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Microsoft.OData.UriParser.FunctionSignatureWithReturnType.#ArgumentTypes")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.OData.UriParser.UriPrimitiveTypeParser.#TryUriStringToPrimitive(System.String,Microsoft.OData.Edm.IEdmTypeReference,System.Object&,Microsoft.OData.UriParser.UriLiteralParsingException&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`2(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`1(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Microsoft.OData.ContainerBuilderExtensions.#AddService`2(Microsoft.OData.IContainerBuilder,Microsoft.OData.ServiceLifetime,System.Func`2<System.IServiceProvider,!!1>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.OData.UriParser.ODataUriParser.#IsODataQueryOption(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", Target = "Microsoft.OData.UriParser.InnerPathTokenBinder.#BindInnerPathSegment(Microsoft.OData.UriParser.InnerPathToken)")]
