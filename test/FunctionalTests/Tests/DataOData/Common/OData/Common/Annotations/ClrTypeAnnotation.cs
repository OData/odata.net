//---------------------------------------------------------------------
// <copyright file="ClrTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Annotations
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation to store the instance type on test metadata items
    /// </summary>
    public class ClrTypeAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Creates or initializes an instance of the ClrTypeAnnotation type
        /// </summary>
        /// <param name="clrType">The Clr type of the metadata item</param>
        public ClrTypeAnnotation(Type clrType)
        {
            this.ClrType = clrType;
        }

        /// <summary>
        /// Gets the clr type of the metadata item this annotation is declared on.
        /// </summary>
        public Type ClrType
        {
            get;
            private set;
        }
    }
}
