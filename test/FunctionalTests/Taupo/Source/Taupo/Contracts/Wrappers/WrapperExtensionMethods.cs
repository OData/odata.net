//---------------------------------------------------------------------
// <copyright file="WrapperExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extensio methods for wrappers.
    /// </summary>
    public static class WrapperExtensionMethods
    {
        /// <summary>
        /// Creates and wraps the entity instance for a given CLR type.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="entityClrType">Type of the entity CLR.</param>
        /// <returns>Wrapper for newly created entity instance.</returns>
        public static WrappedEntityInstance CreateEntityInstance(this IWrapperScope wrapperScope, Type entityClrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckArgumentNotNull(entityClrType, "entityClrType");

            object entityInstance = Activator.CreateInstance(entityClrType);
            return wrapperScope.Wrap<WrappedEntityInstance>(entityInstance);
        }
    }
}