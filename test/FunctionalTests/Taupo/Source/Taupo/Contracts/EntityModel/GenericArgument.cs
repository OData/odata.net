//---------------------------------------------------------------------
// <copyright file="GenericArgument.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents an argument that supplies the value for a generic type parameter in a base type.
    /// </summary>
    public sealed class GenericArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArgument"/> class.
        /// </summary>
        /// <param name="typeParameterName">The name of the type parameter in the base type that
        /// this <see cref="GenericArgument"/> provides a <see cref="DataType"/> for.</param>
        /// <param name="dataType">The <see cref="DataType"/> used to fill the type parameter.</param>
        public GenericArgument(string typeParameterName, DataType dataType)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(typeParameterName, "typeParameterName");
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");

            this.TypeParameterName = typeParameterName;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the name of the type parameter in the base type that
        /// this <see cref="GenericArgument"/> provides a <see cref="DataType"/> for.
        /// </summary>
        public string TypeParameterName { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="DataType"/> used to fill the type parameter.
        /// </summary>
        public DataType DataType { get; set; }
    }
}
