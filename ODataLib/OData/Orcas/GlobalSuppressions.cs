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
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "resource", Target = "Microsoft.Data.OData.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.Data.OData.ODataUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Utils", Scope = "type", Target = "Microsoft.Data.OData.Query.ODataUriUtils")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "mediaresource", Scope = "resource", Target = "Microsoft.Data.OData.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "src", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "The name of the src attribute on the atom:content element.")]

// Violations triggered by using various terms comming from other specs
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "dataservices", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "Part of an XML namespace URL, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Rel", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "LinkRel is a valid identifier used in the public API.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "rel", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "ATOM attribute name, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Edm", Scope = "resource", Target = "Microsoft.Data.OData.resources")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "orderby", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "$orderby is a keyword in the URL query language.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "innererror", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "JSON property name, can't change.")]
[module: SuppressMessage("Microsoft.Naming", "CA1701:ResourceStringCompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace", Scope = "resource", Target = "Microsoft.Data.OData.resources", Justification = "XmlNodeType.Whitespace is an existing API which we can't fix..")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "URIs", Scope = "resource", Target = "Microsoft.Data.OData.resources")]

// Violations in the generated Resource file; can't prevent these from being generated.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#ArgumentNull(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#ArgumentOutOfRange(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#NotImplemented()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Error.#NotSupported()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#GetObject(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#Resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#GetString(System.String,System.Boolean&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.TextRes.#GetString(System.String,System.Object[])")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#get_ODataVersionChecker_ProtocolVersion3IsNotSupported()", Justification = "String is used if #INTERNAL_DROP is set.")]

// Violations that resource strings are not used (in the generated resource file); remove once we implemented the readers - the messages will be used then.
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#get_HttpUtils_ContentTypeMissing()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#HttpUtils_EncodingNotSupported(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Strings.#ODataMessageWriter_InvalidContentTypeForWritingRawValue(System.Object)")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "charset", Scope = "resource", Target = "Microsoft.Data.OData.resources")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils.#ReadContentType(System.String,System.String&,System.Text.Encoding&)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#EncodingFromName(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#get_FallbackEncoding()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#get_MediaTypeName()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#get_MissingEncoding()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#get_Parameters()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.HttpUtils+MediaType.#SelectEncoding()")]

// Violation caused by the Metadata namespace having too few public types.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Data.OData.Metadata")]

// Remove once new EPM features are implemented
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Atom.EpmTargetPathSegment.#get_IsAtomCategoryAttributeSegment()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.Atom.EpmTargetPathSegment.#get_IsAtomLinkAttributeSegment()")]

// Remove once the ODataBatchReader is implemented.
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#batchBoundary")]
[module: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#Dispose(System.Boolean)")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#encoding")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#readingResponse")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#settings")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#synchronous")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Data.OData.ODataBatchReader.#VerifyNotDisposed()")]

// Types forwarded from Astoria client to be shared with ODataLib.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Data.Services.Common", Justification = "Types are shared with Astoria client.")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Data.OData.Query", Justification = "Types are shared with Query project.")]
