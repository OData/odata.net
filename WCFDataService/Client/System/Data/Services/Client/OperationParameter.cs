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

namespace System.Data.Services.Client
{
    /// <summary>Represents a parameter passed to a service action, service function or a service operation when it is Executed.</summary>
    public abstract class OperationParameter
    {
        /// <summary>The name of the operation parameter.</summary>
        private String parameterName;

        /// <summary>The value of the operation parameter.</summary>
        private Object parameterValue;

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.Client.OperationParameter" /> class.</summary>
        /// <param name="name">The name of the operation parameter.</param>
        /// <param name="value">The value of the operation parameter.</param>
        protected OperationParameter(string name, Object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(Strings.Context_MissingOperationParameterName);
            }

            this.parameterName = name;
            this.parameterValue = value;
        }

        /// <summary>Gets the name of the operation parameter. </summary>
        /// <returns>The name of the operation parameter.</returns>
        public String Name
        {
            get
            {
                return this.parameterName;
            }
        }

        /// <summary>Gets the value of the operation parameter. </summary>
        /// <returns>The value of the operation parameter.</returns>
        public Object Value
        {
            get { return this.parameterValue; }
        }
    }
}
