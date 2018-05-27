//---------------------------------------------------------------------
// <copyright file="EdmNamedElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System.Diagnostics;

    /// <summary>
    /// Common base class for all named EDM elements.
    /// </summary>
    [DebuggerDisplay("Name:{Name}")]
    public abstract class EdmNamedElement : EdmElement, IEdmNamedElement
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNamedElement"/> class.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        protected EdmNamedElement(string name)
        {
            EdmUtil.CheckArgumentNull(name, "name");

            this.name = name;
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }
}
