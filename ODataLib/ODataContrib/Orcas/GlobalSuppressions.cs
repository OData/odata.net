//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Diagnostics.CodeAnalysis;

// Violations triggered by using terms from our spec could not solve them by adding the word(s) to the FxCop custom dictionary
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.Data.OData.Query.ODataQueryUtils")]

// Remove once we finalize the namespaces and the API (or keep if necessary)
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Data.OData.Query.Metadata")]

// We are directly linking the DataServiceProviderMethods class from the Microsoft.Data.OData project so we cannot avoid these methods.
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.DataServiceProviderMethods.#ConvertMethodInfo")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.DataServiceProviderMethods.#GetSequenceValueMethodInfo")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.DataServiceProviderMethods.#GetValueMethodInfo")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.DataServiceProviderMethods.#TypeIsMethodInfo")]

// These methods are used in the Microsoft.Data.OData project. We are directly linking the DebugUtils class from the Microsoft.Data.OData project so we cannot avoid these methods.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.DebugUtils.#CheckNoExternalCallers(System.Boolean)")]

// These methods are used in the Microsoft.Data.OData project. We are directly linking the ExceptionUtils class from the Microsoft.Data.OData project so we cannot avoid these methods.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.ExceptionUtils.#CheckIntegerNotNegative(System.Int32,System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.ExceptionUtils.#CheckIntegerPositive(System.Int32,System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.ExceptionUtils.#IsCatchableExceptionType(System.Exception)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.ExceptionUtils.#CheckLongPositive(System.Int64,System.String)")]

// Remove once all OpenTypeMethods are used in ODataLib; this is not used because the whole class was copied from the product tree.
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.OpenTypeMethods.#GetValueOpenPropertyMethodInfo")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.OpenTypeMethods.#ConvertMethodInfo")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.Query.OpenTypeMethods.#TypeIsMethodInfo")]

// Violations in the generated Resource file; can't prevent these from being generated.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#ArgumentOutOfRange(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#NotImplemented()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#NotSupported()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#GetString(System.String,System.Boolean&)")]

// Remove once expression lexer is used in $filter and once uri query path parser is used
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Query.ExpressionLexer.#get_Position()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Query.ExpressionLexer.#ReadDottedIdentifier()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Query.ExpressionToken.#get_IsEqualityOperator()")]

// Remove once we implement semantic binding based on internal literal expression strings
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Query.LiteralQueryToken.#get_OriginalText()")]

// Violations triggered by using 'MultiValue', 'Utils' and '__mediaresource'; could not solve them by adding the word(s) to the FxCop custom dictionary
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "Microsoft.Data.OData.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "$orderby is a keyword in the URL query language.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "inlinecount", Scope = "resource", Target = "Microsoft.Data.OData.resources")]

// Naming - revisit once we settle on the OM names
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uncomposable", Scope = "member", Target = "Microsoft.Data.OData.Query.QueryNodeKind.#UncomposableServiceOperation")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uncomposable", Scope = "type", Target = "Microsoft.Data.OData.Query.UncomposableServiceOperationQueryNode")]

// These method are used by Microsoft.Data.OData.ExceptionUtils class. We are directly linking to that class so we cannot avoid this method.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#ExceptionUtils_CheckIntegerNotNegative(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#ExceptionUtils_CheckIntegerPositive(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#ExceptionUtils_CheckLongPositive(System.Object)")]

// FxCop does not recognize the suppress message attributes on the members; have to make them global suppressions.
[module: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Query.BinaryOperator.#ctor(System.String,System.Int16,System.Boolean)", Scope = "member", Target = "Microsoft.Data.OData.Query.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eq", Scope = "member", Target = "Microsoft.Data.OData.Query.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "gt", Scope = "member", Target = "Microsoft.Data.OData.Query.BinaryOperator.#.cctor()")]
[module: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "mul", Scope = "member", Target = "Microsoft.Data.OData.Query.BinaryOperator.#.cctor()")]
