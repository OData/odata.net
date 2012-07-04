//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


#if ASTORIA_CLIENT
namespace System.Data.Services.Client
#else
namespace System.Data.Services
#endif
{
    using System.Linq;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal static class CommonUtil
    {
        private static readonly Type[] unsupportedTypes = new Type[]
        {
#if !ASTORIA_LIGHT
                typeof(System.Dynamic.IDynamicMetaObjectProvider),
                typeof(System.Tuple<>),
                typeof(System.Tuple<,>),
                typeof(System.Tuple<,,>),
                typeof(System.Tuple<,,,>),
                typeof(System.Tuple<,,,,>),
                typeof(System.Tuple<,,,,,>),
                typeof(System.Tuple<,,,,,,>),
                typeof(System.Tuple<,,,,,,,>)
#endif
        };

        internal static bool IsUnsupportedType(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (unsupportedTypes.Any(t => t.IsAssignableFrom(type)))
            {
                return true;
            }

            Debug.Assert(!type.FullName.StartsWith("System.Tuple", StringComparison.Ordinal), "System.Tuple is not blocked by unsupported type check");
            return false;
        }
    }
}