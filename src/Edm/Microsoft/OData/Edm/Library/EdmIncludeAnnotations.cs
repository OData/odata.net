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
    /// The includeAnnotation information for referenced model.
    /// </summary>
    public class EdmIncludeAnnotations : IEdmIncludeAnnotations
    {
        private readonly string termNamespace;
        private readonly string qualifier;
        private readonly string targetNamespace;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="termNamespace">The term namespace.</param>
        /// <param name="qualifier">The qualifier.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        public EdmIncludeAnnotations(string termNamespace, string qualifier, string targetNamespace)
        {
            this.termNamespace = termNamespace;
            this.qualifier = qualifier;
            this.targetNamespace = targetNamespace;
        }

        /// <summary>
        /// Get the term namespace.
        /// </summary>
        public string TermNamespace
        {
            get
            {
                return this.termNamespace;
            }
        }

        /// <summary>
        /// Gets the qualifier.
        /// </summary>
        public string Qualifier
        {
            get
            {
                return this.qualifier;
            }
        }

        /// <summary>
        /// Gets the target namespace.
        /// </summary>
        public string TargetNamespace
        {
            get
            {
                return this.targetNamespace;
            }
        }
    }
}
