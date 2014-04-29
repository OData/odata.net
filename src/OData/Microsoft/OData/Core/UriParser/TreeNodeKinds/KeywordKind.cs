//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.TreeNodeKinds
{
    /// <summary>
    /// Keyword enum values related to the URI query syntax
    /// such as $metadata, $count, $value, etc.
    /// </summary>
    internal enum KeywordKind
    {
        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        Metadata = 0,

        /// <summary>A segment name in a URI that indicates a plain primitive value is being requested.</summary>
        Value = 1,

        /// <summary>A segment name in a URI that indicates batch is being requested.</summary>
        Batch = 2,

        /// <summary>A segment name in a URI that indicates that this is an entity reference link operation.</summary>
        Ref = 3,

        /// <summary>A segment name in a URI that indicates that this is a count operation.</summary>
        Count = 4
    }
}
