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
    #region Namespaces
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Query node representing a parameter for expressions.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ParameterQueryNode : SingleValueQueryNode
#else
    public sealed class ParameterQueryNode : SingleValueQueryNode
#endif
    {
        /// <summary>
        /// The type of the value the parameter represents.
        /// </summary>
        public IEdmTypeReference ParameterType
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return this.ParameterType;
            }
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.Parameter;
            }
        }
    }
}
