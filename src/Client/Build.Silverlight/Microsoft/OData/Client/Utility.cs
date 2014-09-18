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
