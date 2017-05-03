//---------------------------------------------------------------------
// <copyright file="EdmOptionalParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM operation parameter.
    /// </summary>
    public class EdmOptionalParameter : EdmOperationParameter, IEdmOptionalParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationParameter"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmOptionalParameter(IEdmOperation declaringOperation, string name, IEdmTypeReference type)
            : this(declaringOperation, name, type, null)
        {
        }

        /// <summary>
         /// Initializes a new instance of the <see cref="EdmOperationParameter"/> class.
         /// </summary>
         /// <param name="declaringOperation">Declaring function of the parameter.</param>
         /// <param name="name">Name of the parameter.</param>
         /// <param name="type">Type of the parameter.</param>
         /// <param name="defaultValue">The default value of the optional parameter.</param>
        public EdmOptionalParameter(IEdmOperation declaringOperation, string name, IEdmTypeReference type, string defaultValue)
            : base(declaringOperation, name, type)
        {
            this.DefaultValueString = defaultValue;
        }

        /// <summary>
        /// Gets the type of this parameter.
        /// </summary>
        public string DefaultValueString { get; private set; }
    }
}
