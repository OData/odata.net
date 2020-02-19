//---------------------------------------------------------------------
// <copyright file="ValidationExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Contains IsBad() and Errors() extension methods.
    /// </summary>
    public static class ValidationExtensionMethods
    {
        /// <summary>
        /// Returns true if this element contains errors returned by the <see cref="Errors"/> method.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            return element.Errors().FirstOrDefault() != null;
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or elements that this element contains. For example errors for a structural type include the errors of the type itself and errors of its declared properties.
        /// The method does not analyze elements referenced by this element. For example errors of a property do not include errors from its type.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or elements that element contains.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            return InterfaceValidator.GetStructuralErrors(element);
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this type reference or its definition.
        /// </summary>
        /// <param name="type">The type reference.</param>
        /// <returns>Any errors that belong to this type reference or its definition.</returns>
        public static IEnumerable<EdmError> TypeErrors(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return InterfaceValidator.GetStructuralErrors(type).Concat(InterfaceValidator.GetStructuralErrors(type.Definition));
        }
    }
}
