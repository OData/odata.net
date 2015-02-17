//---------------------------------------------------------------------
// <copyright file="OperationParameterBindingKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Used in place of Microsoft.OData.Service.OperationParameterBindingKind
    /// </summary>
    public enum OperationParameterBindingKind
    {
        /// <summary>
        /// Always have a binding kind
        /// </summary>
        Always,

        /// <summary>
        /// Never have a binding Kind
        /// </summary>
        Never,

        /// <summary>
        /// Sometimes have a binding kind
        /// </summary>
        Sometimes,
    }
}