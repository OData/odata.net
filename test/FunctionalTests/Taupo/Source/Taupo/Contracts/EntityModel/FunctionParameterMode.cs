//---------------------------------------------------------------------
// <copyright file="FunctionParameterMode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// The Mode of a function parameter
    /// </summary>
    public enum FunctionParameterMode
    {
        /// <summary>
        /// Input parameter
        /// </summary>
        In,

        /// <summary>
        /// Output parameter
        /// </summary>
        Out,

        /// <summary>
        /// Input and Output parameter
        /// </summary>
        InOut,
    }
}
