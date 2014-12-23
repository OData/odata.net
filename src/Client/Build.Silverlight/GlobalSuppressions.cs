//   OData .NET Libraries ver. 6.9
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

[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "Microsoft.OData.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "dataservices", Scope = "resource", Target = "Microsoft.OData.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "rel", Scope = "resource", Target = "Microsoft.OData.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Rel", Scope = "resource", Target = "Microsoft.OData.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "Microsoft.OData.Client.resources")]

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.Service.Common")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#ClientType_MissingOpenProperty(System.Object,System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#Clienttype_MultipleOpenProperty(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#ClientType_NullOpenProperties(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#Collection_NullCollectionReference(System.Object,System.Object)")]

// Violations in the generated Resource file; can't prevent these from being generated...
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "Microsoft.OData.Client.TextRes..cctor()")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "Microsoft.OData.Client.TextResDescriptionAttribute..ctor(System.String)")]

[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#ODataMetadataBuilder_MissingEntitySetUri(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(System.Object,System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.Client.Strings.#ODataMetadataBuilder_MissingEntityInstanceUri(System.Object)")]

#region Task 1268242:Address CodeAnalysis suppressions that were added when moving to FxCop for SDL 6.0
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "Microsoft.OData.Client.DataServiceCollection`1.#Microsoft.OData.Client.ICollectionSerializationAppendix.GetAppendix()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "Microsoft.OData.Client.DataServiceSerializationScope.#.cctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "Microsoft.OData.Client.KnownTypeTable.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "Microsoft.OData.Client.DataServiceSerializationAppendice.#.ctor(Microsoft.OData.Client.DataServiceContext,System.Collections.Generic.IEnumerable`1<System.Object>)")]
#endregion

// Since DataServiceClientRequestMessage is already a public API, suppress this issue until we decide to make breaking changes.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Scope = "type", Target = "Microsoft.OData.Client.DataServiceClientRequestMessage")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Microsoft.OData.Client.DataServiceQuery`1+DataServiceOrderedQuery")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.OData.Client.DataServiceQuery`1+DataServiceOrderedQuery")]

// The class actually has instances in a code piece just disabled by a macro.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "Microsoft.OData.Core.Metadata.EdmLibraryExtensions+EdmTypeEqualityComparer")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration.#sender")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Microsoft.OData.Client.Utility.#GetCustomAttributes(System.Type,System.Type,System.Boolean)")]

// Already public APIs and thus cannot be changed.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuery.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuery`1.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuerySingle`1.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#AppendRequestUri(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuerySingle`1.#AppendRequestUri(System.String)")]

// By design.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuerySingle`1.#GetValue()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuerySingle`1.#GetValueAsync()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#GetAllPagesAsync()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#GetAllPages()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuerySingle`1.#GetValue()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuerySingle`1.#GetValueAsync()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuery.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuery`1.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Microsoft.OData.Client.DataServiceActionQuerySingle`1.#.ctor(Microsoft.OData.Client.DataServiceContext,System.String,Microsoft.OData.Client.BodyOperationParameter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#GetKeyPath(System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.OData.Client.ClientEdmModel.#FindOperationImportsByNameNonBindingParameterType(System.String,System.Collections.Generic.IEnumerable`1<System.String>)")]
