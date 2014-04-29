//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Context that records errors reported by validation rules.
    /// </summary>
    public sealed class ValidationContext
    {
        private readonly List<EdmError> errors = new List<EdmError>();
        private readonly IEdmModel model;
        private readonly Func<object, bool> isBad;

        internal ValidationContext(IEdmModel model, Func<object, bool> isBad)
        {
            this.model = model;
            this.isBad = isBad;
        }

        /// <summary>
        /// Gets the model being validated.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.model; }
        }

        internal IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }

        /// <summary>
        /// Method returns true if the <paramref name="element"/> is known to have structural errors associated with it.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if the <paramref name="element"/> has structural errors associated with it.</returns>
        public bool IsBad(IEdmElement element)
        {
            return this.isBad(element);
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="location">Location of the error.</param>
        /// <param name="errorCode">Value representing the error.</param>
        /// <param name="errorMessage">Message text discribing the error.</param>
        public void AddError(EdmLocation location, EdmErrorCode errorCode, string errorMessage)
        {
            this.AddError(new EdmError(location, errorCode, errorMessage));
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(EdmError error)
        {
            this.errors.Add(error);
        }
    }
}
