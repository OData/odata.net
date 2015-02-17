//---------------------------------------------------------------------
// <copyright file="LinqBuiltInFunctionKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    /// <summary>
    /// The type of built in function
    /// </summary>
    public enum LinqBuiltInFunctionKind
    {
        /// <summary>
        /// Instance property
        /// </summary>
        InstanceProperty,

        /// <summary>
        /// Instance method
        /// </summary>
        InstanceMethod,

        /// <summary>
        /// Static method
        /// </summary>
        StaticMethod,
    }
}
