//---------------------------------------------------------------------
// <copyright file="ODataDeltaResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
    /// <summary>
    /// Describes a delta response.
    /// </summary>
    public sealed class ODataDeltaResourceSet : ODataResourceSetBase
    {
        /// <summary>
        /// The type name of the resource set.
        /// </summary>
        private string typeName;

        /// <summary>Gets the resource set type name.</summary>
        /// <returns>The resource set type name.</returns>
        public override string TypeName
        {
            get
            {
                if (typeName == null && this.SerializationInfo != null && this.SerializationInfo.ExpectedTypeName != null)
                {
                    typeName = EdmLibraryExtensions.GetCollectionTypeName(this.SerializationInfo.ExpectedTypeName);
                }

                return typeName;
            }

            set
            {
                this.typeName = value;
            }
        }
    }
}
