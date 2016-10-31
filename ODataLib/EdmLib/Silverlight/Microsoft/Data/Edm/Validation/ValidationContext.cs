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

namespace Microsoft.Data.Edm.Validation
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
