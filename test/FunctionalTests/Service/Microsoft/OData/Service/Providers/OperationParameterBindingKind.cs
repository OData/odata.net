//---------------------------------------------------------------------
// <copyright file="OperationParameterBindingKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for classifying the different kinds of operation parameter binding.
    /// </summary>
    public enum OperationParameterBindingKind
    {
        /// <summary>
        /// Used when the first parameter of a service action is not a binding parameter.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and some or all instances of the binding parameter type 
        /// may be bound to the service action.
        /// </summary>
        Sometimes = 1,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and all instances of the binding parameter type 
        /// must be bound to the service action.
        /// </summary>
        /// <remarks>When this value is set, the <see cref="IDataServiceActionProvider.AdvertiseServiceAction"/> method will not be called for the service action."/> </remarks>
        Always = 2,
    }
}
