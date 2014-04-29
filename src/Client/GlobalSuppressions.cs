//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Diagnostics.CodeAnalysis;

#if ASTORIA_LIGHT
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#System.Collections.IEnumerable.GetEnumerator()")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.OData.Client.DataServiceQuery`1.#System.Collections.Generic.IEnumerable`1<!0>.GetEnumerator()")]
#endif
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
