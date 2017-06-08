//---------------------------------------------------------------------
// <copyright file="EdmStructuredType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base class for definitions of EDM structured types.
    /// </summary>
    public sealed class EdmUntypedStructuredType : EdmStructuredType, IEdmStructuredType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmStructuredType"/> class.
        /// </summary>
        public EdmUntypedStructuredType() 
            :base(/*isAbstract*/true, /*isOpen*/true, /*baseType*/ null)
        {
        }

        public override EdmTypeKind TypeKind
        {
            get
            {
                return EdmTypeKind.Untyped;
            }
        }
    }
}
