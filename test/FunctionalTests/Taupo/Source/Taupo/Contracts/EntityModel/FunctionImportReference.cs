//---------------------------------------------------------------------
// <copyright file="FunctionImportReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to function import defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class FunctionImportReference : FunctionImport
    {
        /// <summary>
        /// Initializes a new instance of the FunctionImportReference class with a given name.
        /// </summary>
        /// <param name="name">function name</param>
        public FunctionImportReference(string name)
            : base(name)
        {
        }
    }
}
