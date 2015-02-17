//---------------------------------------------------------------------
// <copyright file="PropertyInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Abstract base class for primitive, complex, and navigation properties
    /// </summary>
    public abstract class PropertyInstance : ODataPayloadElement
    {
        /// <summary>
        /// Initializes a new instance of the PropertyInstance class
        /// </summary>
        /// <param name="name">The property's name</param>
        protected PropertyInstance(string name)
            : base()
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the property
        /// </summary>
        public string Name { get; set; }
    }
}
