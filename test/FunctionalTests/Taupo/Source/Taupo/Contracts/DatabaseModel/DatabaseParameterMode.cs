//---------------------------------------------------------------------
// <copyright file="DatabaseParameterMode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Parameter passing mode for <see cref="DatabaseFunctionParameter"/>
    /// </summary>
    public enum DatabaseParameterMode
    {
        /// <summary>
        /// Input parameter.
        /// </summary>
        In,

        /// <summary>
        /// Output parameter.
        /// </summary>
        Out,

        /// <summary>
        /// Input/output parameter.
        /// </summary>
        InOut,

        /// <summary>
        /// Return value.
        /// </summary>
        ReturnValue,
    }
}
