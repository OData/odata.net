//---------------------------------------------------------------------
// <copyright file="ODataPropertySerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataWriter"/> for an <see cref="ODataProperty"/>.
    /// </summary>
    public sealed class ODataPropertySerializationInfo
    {
        /// <summary>
        /// The kind of the property
        /// </summary>
        public ODataPropertyKind PropertyKind
        {
            get;
            set;
        }
    }
}