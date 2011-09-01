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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a URI-based qualified name.
    /// </summary>
    public class EdmTermName
    {
        private readonly string namespaceName;
        private readonly string localName;

        /// <summary>
        /// Initializes a new instance of the EdmTermName class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the item.</param>
        /// <param name="localName">Name of the item within the namespace.</param>
        public EdmTermName(string namespaceName, string localName)
        {
            this.namespaceName = namespaceName;
            this.localName = localName;
        }

        /// <summary>
        /// Gets the namespace of the item.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the name of the item within the namespace.
        /// </summary>
        public string LocalName
        {
            get { return this.localName; }
        }
    }
}
