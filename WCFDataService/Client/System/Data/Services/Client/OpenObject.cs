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
