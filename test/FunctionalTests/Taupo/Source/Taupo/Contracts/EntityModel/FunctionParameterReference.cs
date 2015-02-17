//---------------------------------------------------------------------
// <copyright file="FunctionParameterReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Reference to function parameter defined elsewhere.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class FunctionParameterReference : FunctionParameter
    {
        /// <summary>
        /// Initializes a new instance of the FunctionParameterReference class with a given name.
        /// </summary>
        /// <param name="name">the given name of function parameter</param>
        public FunctionParameterReference(string name)
            : base(name, null)
        {
        }
    }
}
