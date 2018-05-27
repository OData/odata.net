//---------------------------------------------------------------------
// <copyright file="ODataEnumValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// OData enum value
    /// </summary>
    public sealed class ODataEnumValue : ODataValue
    {
        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Cyan".</param>
        public ODataEnumValue(string value)
        {
            this.Value = value;
            this.TypeName = null;
        }

        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Cyan".</param>
        /// <param name="typeName">The type name in edm model.</param>
        public ODataEnumValue(string value, string typeName)
        {
            this.Value = value;
            this.TypeName = typeName;
        }

        /// <summary>Get backing type value, can be "3" or "White" or "Black,Yellow,Cyan".</summary>
        public string Value { get; private set; }

        /// <summary>Get the type name in edm model.</summary>
        public string TypeName { get; private set; }
    }
}
