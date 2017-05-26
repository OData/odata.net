//---------------------------------------------------------------------
// <copyright file="EdmIncludeAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
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
