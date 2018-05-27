//---------------------------------------------------------------------
// <copyright file="EdmValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
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
