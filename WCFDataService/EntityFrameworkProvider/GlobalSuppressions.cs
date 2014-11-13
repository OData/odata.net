//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "etag", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "JSON", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "allpages", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "inlinecount", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "dataservices", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "skiptoken", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "rel", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Rel", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Edm", Scope = "resource", Target = "System.Data.Services.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Advertisable", Scope = "resource", Target = "System.Data.Services.resources")]

#if !ASTORIA_OPEN_OBJECT
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#BadRequest_ValuesCanBeReturnedForPrimitiveTypesOnly")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#DataService_InvalidResourceType(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#DataService_TypeNotValidForDeleteOperation(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#InvalidUri_OpenCollectionPropertiesMustHaveParenthesis(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#LateBoundMethods_ExpressionTypeNotApplicableToNull(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#LateBoundMethods_ExpressionTypeNotApplicableToNulls(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#LateBoundMethods_GetSequenceValueNull(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#LateBoundMethods_KeyTypeHasNoKeys(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#OpenTypeAttribute_PropertyNameCannotBeNull")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#OpenTypeAttribute_PropertyNameIsNotValidIdentifier(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#RequestQueryParser_BooleanExpressionsExpectedFor(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Strings.#ResourceType_IncorrectOpenProperty(System.Object)")]
#endif

// Violations in the generated Resource file; can't prevent these from being generated...
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.TextRes.#GetString(System.String,[out]System.Boolean)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "System.Data.Services.TextRes..cctor()")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "System.Data.Services.TextResDescriptionAttribute..ctor(System.String)")]
