using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.E2E.Profile111
{
    internal static class PlatformHelper
    {
        public static bool IsSubclassOf(this Type thisType, Type otherType)
        {
            if (thisType == otherType)
            {
                return false;
            }

            Type type = thisType.GetTypeInfo().BaseType;
            while (type != null)
            {
                if (type == otherType)
                {
                    return true;
                }

                type = type.GetTypeInfo().BaseType;
            }

            return false;
        }

        public static Type GetType(this object obj)
        {
            return obj.GetType();
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return type.GetRuntimeProperty(propertyName);
        }
    }
}
