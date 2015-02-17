//---------------------------------------------------------------------
// <copyright file="ArgumentPropertyType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    /// <summary>
    /// Represents the different types of ArgumentPropertyAttributes
    /// </summary>
    public enum ArgumentPropertyType
    {
        /// <summary>
        /// Standard property.
        /// </summary>
        Standard,

        /// <summary>
        /// Required property.
        /// </summary>
        Required,

        /// <summary>
        /// Property is a bool switch.
        /// </summary>
        Switch
    }
}
