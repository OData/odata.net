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
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Validation
{
    /// <summary>
    /// Context that records errors reported by validation rules.
    /// </summary>
    public sealed class ValidationContext
    {
        private readonly ValidationRuleSet ruleSet;
        private readonly List<EdmError> errors = new List<EdmError>();
        private readonly IEdmModel model;

        internal ValidationContext(ValidationRuleSet ruleSet, IEdmModel model)
        {
            this.ruleSet = ruleSet;
            this.IsValid = true;
            this.model = model;
        }

        internal IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }

        internal bool IsValid
        {
            get; 
            private set;
        }

        internal ValidationRuleSet RuleSet 
        { 
            get { return this.ruleSet; } 
        }

        /// <summary>
        /// Gets the model being validated.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Finds a type from the provided model, or from the set of primitive types
        /// </summary>
        /// <param name="qualifiedName">Name of the type being found</param>
        /// <returns>The requested type if it exists, otherwise null.</returns>
        public IEdmType FindTypeInModel (string qualifiedName)
        {
            return this.Model.FindType(qualifiedName) ?? EdmCoreModel.Instance.FindType(qualifiedName);
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="location">Location of the error.</param>
        /// <param name="errorCode">Value representing the error.</param>
        /// <param name="errorMessage">Message text discribing the error.</param>
        public void AddError(EdmLocation location, EdmErrorCode errorCode, string errorMessage)
        {
            this.IsValid = false;
            this.errors.Add(new EdmError(location, errorCode, errorMessage));
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(EdmError error)
        {
            this.IsValid = false;
            this.errors.Add(error);
        }
    }
}
