//---------------------------------------------------------------------
// <copyright file="IExpandedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    /// <summary>
    /// This interface declares the methods required to support enumerators for results and for
    /// associated segments on a WCF Data Service $expand query option.
    /// </summary>
    public interface IExpandedResult
    {
        /// <summary>Gets the element with expanded properties.</summary>
        /// <returns>The object in a property expanded by <see cref="T:Microsoft.OData.Service.IExpandedResult" />.</returns>
        object ExpandedElement
        { 
            get;
        }

        /// <summary>Gets the value for a named property of the result.</summary>
        /// <returns>The value of the property.</returns>
        /// <param name="name">The name of the property for which to get enumerable results.</param>
        /// <remarks>
        /// If the element returned in turn has properties which are expanded out-of-band
        /// of the object model, then the result will also be of type <see cref="IExpandedResult"/>,
        /// and the value will be available through <see cref="ExpandedElement"/>.
        /// </remarks>
        object GetExpandedPropertyValue(string name);
    }
}
