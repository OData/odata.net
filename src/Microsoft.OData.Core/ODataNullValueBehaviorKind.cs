//---------------------------------------------------------------------
// <copyright file="ODataNullValueBehaviorKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary> Represents the behavior of readers when reading property with null value. </summary>
    public enum ODataNullValueBehaviorKind
    {
        /// <summary>
        /// The default behavior - this means validate the null value against the declared type
        /// and then report the null value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// This means to not report the value and not validate it against the model.
        /// </summary>
        /// <remarks>
        /// This setting can be used to correctly work with clients that send null values
        /// for uninitialized properties in requests instead of omitting them altogether.
        /// </remarks>
        IgnoreValue = 1,

        /// <summary>
        /// This means to report the value, but not validate it against the model.
        /// </summary>
        DisableValidation = 2,
    }
}
