//---------------------------------------------------------------------
// <copyright file="CodeGenerationTypeUsage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Marks whether a type will be used for declaration or instantiation when generating code
    /// </summary>
    public enum CodeGenerationTypeUsage
    {
        /// <summary>
        /// Indicates that the type will be used for a declaration
        /// </summary>
        Declaration,

        /// <summary>
        /// Indicates that the type will be used for an instantiation
        /// </summary>
        Instantiation
    }
}