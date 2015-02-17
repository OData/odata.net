//---------------------------------------------------------------------
// <copyright file="TypeSpecificationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Annotation which holds original property type specification before data type was resolved.
    /// </summary>
    public class TypeSpecificationAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the TypeSpecificationAnnotation class.
        /// </summary>
        /// <param name="dataType">Data type.</param>
        public TypeSpecificationAnnotation(DataType dataType)
        {
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        /// <value>The type of the data.</value>
        public DataType DataType { get; private set; }
    }
}
