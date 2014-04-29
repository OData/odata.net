//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Represents a parameter passed to a service action, service function or a service operation when it is Executed.</summary>
    public abstract class OperationParameter
    {
        /// <summary>The name of the operation parameter.</summary>
        private String parameterName;

        /// <summary>The value of the operation parameter.</summary>
        private Object parameterValue;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.OperationParameter" /> class.</summary>
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
