//---------------------------------------------------------------------
// <copyright file="FunctionReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to function defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class FunctionReference : Function
    {
        /// <summary>
        /// Initializes a new instance of the FunctionReference class with a given name.
        /// </summary>
        /// <param name="name">function name</param>
        public FunctionReference(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FunctionReference class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">function namespace.</param>
        /// <param name="name">function name</param>
        public FunctionReference(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }
    }
}
