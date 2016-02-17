//---------------------------------------------------------------------
// <copyright file="ODataUndeclaredPropertyValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    /// <summary>
    /// OData representation of an undeclared property value.
    /// </summary>
    /// <remarks>ODataUndeclaredPropertyValue and ODataUntypedValue may look similar 
    /// but ODataUntypedValue represents a declared Edm.Untyped value.</remarks>
    public sealed class ODataUndeclaredPropertyValue : ODataValue
    {
        /// <summary>Gets or sets the raw string value.</summary>
        /// <returns>The raw string value.</returns>
        public string RawValue
        {
            get;
            set;
        }
    }
}
