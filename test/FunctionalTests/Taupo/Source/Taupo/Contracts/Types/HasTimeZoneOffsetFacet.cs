//---------------------------------------------------------------------
// <copyright file="HasTimeZoneOffsetFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Determines whether a primitive type has time zone offset information.
    /// </summary>
    public class HasTimeZoneOffsetFacet : PrimitiveDataTypeFacet<bool>
    {
        /// <summary>
        /// Initializes a new instance of the HasTimeZoneOffsetFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public HasTimeZoneOffsetFacet(bool value)
            : base(value)
        {
        }
    }
}
