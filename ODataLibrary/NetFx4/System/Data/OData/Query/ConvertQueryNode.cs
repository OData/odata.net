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

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Data.Services.Providers;
    #endregion Namespaces.

    /// <summary>
    /// Query node representing a conversion of primitive type.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ConvertQueryNode : SingleValueQueryNode
#else
    public sealed class ConvertQueryNode : SingleValueQueryNode
#endif
    {
        /// <summary>
        /// The source value to convert.
        /// </summary>
        public SingleValueQueryNode Source
        {
            get;
            set;
        }

        /// <summary>
        /// The resouce type of the single value this node represents.
        /// </summary>
        public override ResourceType ResourceType
        {
            get
            {
                return this.TargetResourceType;
            }
        }

        /// <summary>
        /// The resource to convert to.
        /// </summary>
        public ResourceType TargetResourceType
        {
            get;
            set;
        }

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.Convert;
            }
        }
    }
}
