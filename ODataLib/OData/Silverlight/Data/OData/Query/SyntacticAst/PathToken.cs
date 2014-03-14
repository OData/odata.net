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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces
    using System;
    using Microsoft.Data.OData.Query.SemanticAst;
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
