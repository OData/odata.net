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
    using System.Collections.Generic;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Query node representing a service operation which returns a single composable value.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class SingleValueServiceOperationQueryNode : SingleValueQueryNode
#else
    public sealed class SingleValueServiceOperationQueryNode : SingleValueQueryNode
#endif
    {
        /// <summary>
        /// The service operation this node represents.
        /// </summary>
        public IEdmFunctionImport ServiceOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Enumeration of parameter values for the service operation.
        /// </summary>
        public IEnumerable<QueryNode> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// The resouce type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                if (this.ServiceOperation == null)
                {
                    return null;
                }
                else
                {
                    return this.ServiceOperation.ReturnType;
                }
            }
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.SingleValueServiceOperation;
            }
        }
    }
}
