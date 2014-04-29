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
    /// Query node representing an access to a property value.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class PropertyAccessQueryNode : SingleValueQueryNode
#else
    public sealed class PropertyAccessQueryNode : SingleValueQueryNode
#endif
    {
        /// <summary>
        /// The value to access the property on.
        /// </summary>
        public SingleValueQueryNode Source
        {
            get;
            set;
        }

        /// <summary>
        /// The EDM property which is to be accessed.
        /// </summary>
        /// <remarks>Only non-entity properties are supported by this node.</remarks>
        public IEdmProperty Property
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
                if (this.Property == null)
                {
                    // TODO: We should probably have a separate OpenPropertyAccessQueryNode for open properties
                    //   in which case null is illegal here.
                    return null;
                }
                else
                {
                    return this.Property.Type;
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
                return QueryNodeKind.PropertyAccess;
            }
        }
    }
}
