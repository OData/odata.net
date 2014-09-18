//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
