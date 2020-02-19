//---------------------------------------------------------------------
// <copyright file="ObjectLocation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Defines an object as a location of itself.
    /// </summary>
    public class ObjectLocation : EdmLocation
    {
        internal ObjectLocation(object obj)
        {
            this.Object = obj;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        public object Object { get; private set; }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return "(" + this.Object.ToString() + ")";
        }
    }
}
