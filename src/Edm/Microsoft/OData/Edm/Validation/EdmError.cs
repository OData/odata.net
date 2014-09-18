//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
