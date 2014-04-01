//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
