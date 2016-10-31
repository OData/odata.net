//   OData .NET Libraries ver. 5.6.3
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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation.Internal;

namespace Microsoft.Data.Edm.Validation
{
    /// <summary>
    /// Collection of validation methods.
    /// </summary>
    public static class EdmValidator
    {
        /// <summary>
        /// Validate the <see cref="IEdmModel"/> and all of its properties using the current version of the model.
        /// If the model has no version, <see cref="EdmConstants.EdmVersionLatest"/> is used.
        /// </summary>
        /// <param name="root">The root of the model to be validated.</param>
        /// <param name="errors">Errors encountered while validating the model.</param>
        /// <returns>True if model is valid, otherwise false.</returns>
        public static bool Validate(this IEdmModel root, out IEnumerable<EdmError> errors)
        {
            return Validate(root, root.GetEdmVersion() ?? EdmConstants.EdmVersionLatest, out errors);
        }

        /// <summary>
        /// Validate the <see cref="IEdmModel"/> and all of its properties given certain version.
        /// </summary>
        /// <param name="root">The root of the model to be validated.</param>
        /// <param name="version">Version of Edm to validate against.</param>
        /// <param name="errors">Errors encountered while validating the model.</param>
        /// <returns>True if model is valid, otherwise false.</returns>
        public static bool Validate(this IEdmModel root, Version version, out IEnumerable<EdmError> errors)
        {
            return Validate(root, ValidationRuleSet.GetEdmModelRuleSet(version), out errors);
        }

        /// <summary>
        /// Validate the <see cref="IEdmModel"/> and all of its properties given certain version.
        /// </summary>
        /// <param name="root">The root of the model to be validated.</param>
        /// <param name="ruleSet">Custom rule set to validate against.</param>
        /// <param name="errors">Errors encountered while validating the model.</param>
        /// <returns>True if model is valid, otherwise false.</returns>
        public static bool Validate(this IEdmModel root, ValidationRuleSet ruleSet, out IEnumerable<EdmError> errors)
        {
            EdmUtil.CheckArgumentNull(root, "root");
            EdmUtil.CheckArgumentNull(ruleSet, "ruleSet");

            errors = InterfaceValidator.ValidateModelStructureAndSemantics(root, ruleSet);
            return errors.FirstOrDefault() == null;
        }
    }
}
