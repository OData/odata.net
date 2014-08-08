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

#if ASTORIA_OPEN_OBJECT
namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// open-typed object
    /// </summary>
    [OpenObject("OpenProperties")]
    public class OpenObject
    {
        /// <summary>dictionary of properties</summary>
        private readonly Dictionary<string, object> propertySet = new Dictionary<string, object>();

        /// <summary>
        /// constructor
        /// </summary>
        public OpenObject()
        {
        }

        /// <summary>
        /// dictionary property for storing non-strongly typed properties
        /// </summary>
        public Dictionary<string, object> OpenProperties
        {
            get { return this.propertySet; }
        }

        /// <summary>
        /// shortcut access method into OpenProperties
        /// </summary>
        /// <param name="property">property name</param>
        /// <returns>value or null if the property doesn't exist</returns>
        public object this[string property]
        {
            get
            {
                object value;
                this.propertySet.TryGetValue(property, out value);
                return value;
            }

            set
            {
                this.propertySet[property] = value;
            }
        }

        /// <summary>
        /// shortcut typed access method into OpenProperties
        /// </summary>
        /// <typeparam name="T">user desired type</typeparam>
        /// <param name="property">property name</param>
        /// <returns>typed value</returns>
        /// <exception cref="InvalidOperationException">when unable to cast the value to the desired type</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "required for this feature")]
        public T Field<T>(string property)
        {
            return (T)ClientConvert.VerifyCast(typeof(T), this[property]);
        }
    }
}
#endif
