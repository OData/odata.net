//---------------------------------------------------------------------
// <copyright file="JsonValueType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Edm.Csdl.Json.Reader
{
    /// <summary>
    /// Enumeration of all JSON value type.
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
    public enum JsonValueType
    {
        /// <summary>
        /// No node - invalid value.
        /// </summary>
        None,

        /// <summary>
        /// Null value
        /// </summary>
        Null,

        /// <summary>
        /// Integer value
        /// </summary>
        Integer,

        /// <summary>
        /// String value
        /// </summary>
        String,

        /// <summary>
        /// Boolean value
        /// </summary>
        Boolean,

        /// <summary>
        /// Decimal value
        /// </summary>
        Decimal,

        /// <summary>
        /// Double value
        /// </summary>
        Double
    }
}
