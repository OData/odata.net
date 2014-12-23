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

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    
    /// <summary>
    /// Helper class for T4 Template, provide uniform API for different platforms
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// When overridden in a derived class, returns an array of custom attributes
        /// applied to this member and identified by System.Type.
        /// </summary>
        /// <param name="type">
        /// The type to query custom attributes on.
        /// </param>
        /// <param name="attributeType">
        /// The type of attribute to search for. Only attributes that are assignable
        /// to this type are returned.
        /// </param>
        /// <param name="inherit">
        /// true to search this member's inheritance chain to find the attributes; otherwise,
        /// false. This parameter is ignored for properties and events; see Remarks.
        /// </param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of custom attributes applied to this member, or an array with zero
        /// elements if no attributes assignable to attributeType have been applied.
        /// </returns>
        public static IEnumerable<object> GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
#if WINRT
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
#else
            return type.GetCustomAttributes(attributeType, inherit);
#endif
        }
    }
}
