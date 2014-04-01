//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    #region Namespaces
    using System;
    using Microsoft.OData.Core.UriParser.Semantic;
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a segment in a path.
    /// </summary>
    /// 
    internal abstract class PathToken : QueryToken
    {
        /// <summary>
        /// The NextToken in the path(can either be the parent or the child depending on whether the tree has
        /// been normalized for expand or not.
        /// TODO: need to revisit this and the rest of the syntactic parser to make it ready for public consumption.
        /// </summary>
        public abstract QueryToken NextToken { get; set; }

        /// <summary>
        /// The name of the property to access.
        /// </summary>
        public abstract string Identifier { get; }
    }
}
