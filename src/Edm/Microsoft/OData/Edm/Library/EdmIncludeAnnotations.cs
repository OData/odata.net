//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
