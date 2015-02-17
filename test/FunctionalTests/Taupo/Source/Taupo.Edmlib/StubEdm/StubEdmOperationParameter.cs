//---------------------------------------------------------------------
// <copyright file="StubEdmOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.StubEdm
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of EdmOperationParameter
    /// </summary>
    public class StubEdmOperationParameter : StubEdmElement, IEdmOperationParameter
    {
        /// <summary>
        /// Initializes a new instance of the StubEdmOperationParameter class.
        /// </summary>
        /// <param name="name">the name of the operation parameter</param>
        public StubEdmOperationParameter(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public IEdmTypeReference Type { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the operation that declared this parameter.
        /// </summary>
        public IEdmOperation DeclaringOperation { get; set; }
    }
}
