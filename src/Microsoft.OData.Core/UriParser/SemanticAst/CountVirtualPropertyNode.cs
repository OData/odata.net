//---------------------------------------------------------------------
// <copyright file="CountVirtualPropertyNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Dummy class that allows virtual property $count
    /// to work like any other aggregation method.
    /// </summary>
    public sealed class CountVirtualPropertyNode : SingleValueNode
    {
        /// <summary>Constructor.</summary>
        public CountVirtualPropertyNode()
        {
        }

        /// <summary>Kind of the single value node.</summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.Count;
            }
        }

        /// <summary>Type returned by the $count virtual property.</summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                // Issue #758: CountDistinct and $Count should return type Edm.Decimal with Scale="0" and sufficient Precision.
                return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
            }
        }
    }
}
