//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm.Validation.Internal;

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
