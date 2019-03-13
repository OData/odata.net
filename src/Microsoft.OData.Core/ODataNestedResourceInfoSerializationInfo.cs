//---------------------------------------------------------------------
// <copyright file="ODataNestedResourceInfoSerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataWriter"/> for an <see cref="ODataNestedResourceInfo"/>.
    /// </summary>
    public sealed class ODataNestedResourceInfoSerializationInfo
    {
        /// <summary>
        /// Gets or sets the value that indicates the nested resource for a complex property or collection of complex property.
        /// </summary>
        public bool IsComplex { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the nested resource is undeclared or not.
        /// </summary>
        public bool IsUndeclared { get; set; }
    }
}
