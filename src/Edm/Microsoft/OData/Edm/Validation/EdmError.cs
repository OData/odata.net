//   OData .NET Libraries ver. 6.9
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
using System.Globalization;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Represents a reportable error in EDM
    /// </summary>
    public class EdmError
    {
        /// <summary>
        /// Initializes a new instance of the EdmError class.
        /// </summary>
        /// <param name="errorLocation">The location where the error occurred.</param>
        /// <param name="errorCode">An integer code representing the error.</param>
        /// <param name="errorMessage">A human readable message describing the error.</param>
        public EdmError(EdmLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
        {
            this.ErrorLocation = errorLocation;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the location of the error in the file in which it occurred.
        /// </summary>
        public EdmLocation ErrorLocation { get; private set; }

        /// <summary>
        /// Gets an integer code representing the error.
        /// </summary>
        public EdmErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// Gets a human readable string describing the error.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gets a string representation of the error.
        /// </summary>
        /// <returns>A string representation of the error.</returns>
        public override string ToString()
        {
            if (this.ErrorLocation != null && !(this.ErrorLocation is ObjectLocation))
            {
                return Convert.ToString(this.ErrorCode, CultureInfo.InvariantCulture) + " : " + this.ErrorMessage + " : " + this.ErrorLocation.ToString();
            }

            return Convert.ToString(this.ErrorCode, CultureInfo.InvariantCulture) + " : " + this.ErrorMessage;
        }
    }
}
