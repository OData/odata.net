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

namespace System.Data.OData.Atom
{
    /// <summary>
    /// Enumeration for classifying the different kinds of text content in ATOM metadata.
    /// </summary>
#if INTERNAL_DROP
    internal enum AtomTextConstructKind
#else
    public enum AtomTextConstructKind
#endif
    {
        /// <summary>Plain text.</summary>
        Text = 0,

        /// <summary>Html text.</summary>
        Html = 1,

        /// <summary>XHtml text.</summary>
        Xhtml = 2,
    }
}
