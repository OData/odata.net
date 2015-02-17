//---------------------------------------------------------------------
// <copyright file="Property.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    /// <summary>
    /// The property class
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets or sets property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets property value
        /// </summary>
        public dynamic Value { get; set; }
    }
}
