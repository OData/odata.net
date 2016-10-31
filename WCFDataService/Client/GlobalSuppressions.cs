//   OData .NET Libraries ver. 5.6.3
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

#if ASTORIA_LIGHT
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Data.Services.Client.DataServiceQuery`1.#System.Collections.IEnumerable.GetEnumerator()")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Data.Services.Client.DataServiceQuery`1.#System.Collections.Generic.IEnumerable`1<!0>.GetEnumerator()")]
#endif
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "System.Data.Services.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "dataservices", Scope = "resource", Target = "System.Data.Services.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "rel", Scope = "resource", Target = "System.Data.Services.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Rel", Scope = "resource", Target = "System.Data.Services.Client.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "System.Data.Services.Client.resources")]

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Data.Services.Common")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#ClientType_MissingOpenProperty(System.Object,System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#Clienttype_MultipleOpenProperty(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#ClientType_NullOpenProperties(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#Collection_NullCollectionReference(System.Object,System.Object)")]

// Violations in the generated Resource file; can't prevent these from being generated...
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "System.Data.Services.Client.TextRes..cctor()")]
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "System.Data.Services.Client.TextResDescriptionAttribute..ctor(System.String)")]

[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#ODataMetadataBuilder_MissingEntitySetUri(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(System.Object,System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Data.Services.Client.Strings.#ODataMetadataBuilder_MissingEntityInstanceUri(System.Object)")]

#region Task 1268242:Address CodeAnalysis suppressions that were added when moving to FxCop for SDL 6.0
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "System.Data.Services.Client.DataServiceCollection`1.#System.Data.Services.Client.ICollectionSerializationAppendix.GetAppendix()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "System.Data.Services.Client.DataServiceSerializationScope.#.cctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "System.Data.Services.Client.KnownTypeTable.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Performance", "AC0002:HashSetCtorRule", Scope = "member", Target = "System.Data.Services.Client.DataServiceSerializationAppendice.#.ctor(System.Data.Services.Client.DataServiceContext,System.Collections.Generic.IEnumerable`1<System.Object>)")]
#endregion
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Scope = "member", Target = "System.Data.Services.Client.BaseSaveResult.#CreateRequest(System.Data.Services.Client.LinkDescriptor)")]
