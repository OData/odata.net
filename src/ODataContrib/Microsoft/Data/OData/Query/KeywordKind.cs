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

namespace Microsoft.Data.OData.Query
{
    /// <summary>
    /// Keyword enum values related to the URI query syntax
    /// such as $metadata, $count, $value, etc.
    /// </summary>
    public enum KeywordKind
    {
        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        Metadata,

        /// <summary>A segment name in a URI that indicates a plain primitive value is being requested.</summary>
        Value,

        /// <summary>A segment name in a URI that indicates batch is being requested.</summary>
        Batch,

        /// <summary>A segment name in a URI that indicates that this is an entity reference link operation.</summary>
        Links,

        /// <summary>A segment name in a URI that indicates that this is a count operation.</summary>
        Count
    }
}
