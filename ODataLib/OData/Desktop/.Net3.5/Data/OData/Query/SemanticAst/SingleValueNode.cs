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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for all semantic metadata bound nodes which represent a single composable value.
    /// </summary>
    public abstract class SingleValueNode : QueryNode
    {
        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        public abstract IEdmTypeReference TypeReference
        {
            get;
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override QueryNodeKind Kind
        {  
            get { return (QueryNodeKind)this.InternalKind; }
        }
    }
}
