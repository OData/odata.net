//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// The include information for referenced model.
    /// </summary>
    public class EdmInclude : IEdmInclude
    {
        private readonly string alias;
        private readonly string namespace_;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="namespace_">The namespace.</param>
        public EdmInclude(string alias, string namespace_)
        {
            this.alias = alias;
            this.namespace_ = namespace_;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return this.alias;
            }
        }

        /// <summary>
        /// Gets the namespace to include.
        /// </summary>
        public string Namespace
        {
            get
            {
                return this.namespace_;
            }
        }
    }
}
