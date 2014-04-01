//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
