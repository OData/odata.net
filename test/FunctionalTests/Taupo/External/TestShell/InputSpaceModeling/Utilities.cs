//---------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
    public static class Utilities
    {
        public static IList<object> GetEnumValues(Type enumType)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumType, "enumType");            
            return Enum.GetValues(enumType).Cast<object>().ToList();
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");              
            return TypeDescriptor.GetConverter(type);
        }
    }
}
