//---------------------------------------------------------------------
// <copyright file="ODataUntypedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// OData representation of an untyped value.
    /// </summary>
    public sealed class ODataUntypedValue : ODataValue
    {
        /// <summary>Gets or sets the raw untyped value.</summary>
        /// <returns>The raw untyped value.</returns>
        /// <remarks>
        /// The caller of the setter is responsible for formatting the value for the
        /// data transmission protocol it will be used in.
        /// For instance, if the protocol is JSON, the caller must format this value as JSON.
        /// This library will not perform any formatting.
        /// </remarks>
        public string RawValue
        {
            get;
            set;
        }
    }
}
