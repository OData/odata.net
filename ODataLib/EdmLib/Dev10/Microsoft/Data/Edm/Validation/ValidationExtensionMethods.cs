//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Validation.Internal;

namespace Microsoft.Data.Edm.Validation
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
