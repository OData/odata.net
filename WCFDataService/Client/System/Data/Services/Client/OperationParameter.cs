//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
