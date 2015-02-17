//---------------------------------------------------------------------
// <copyright file="Type.Ext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.Dallas.Test.Common
{
    public static class TypeExt
    {
        public static object ConvertToClr(this Type type, string value, bool escapeSingleQuote = true)
        {
            if (type.Equals(typeof(String)))
            {
                if(value == null)
                {
                    return null;
                }
                else if (value.StartsWith("'") && value.EndsWith("'") && value.Length >= 2 && escapeSingleQuote)
                {
                    return value.Substring(1, value.Length - 2);
                }
                else
                {
                    return value;
                }
            }
            else if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            else if (type.Equals(typeof(Int16)))
            {
                return Int16.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Int32)))
            {
                return Int32.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Double)))
            {
                return Double.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Boolean)))
            {
                return Boolean.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Single)))
            {
                return Single.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Byte)))
            {
                return Byte.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Int64)))
            {
                return Int64.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Decimal)))
            {
                return Decimal.Parse(value.ToString());
            }
            else if (type.Equals(typeof(DateTime)))
            {
                if (value.StartsWith("datetime", StringComparison.CurrentCultureIgnoreCase))
                {
                    value = value.Substring("datetime".Length).Trim(new char[] { '\'' });
                }
                return DateTime.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Guid)))
            {
                if(value.StartsWith("guid", StringComparison.CurrentCultureIgnoreCase))
                {
                    value = value.Substring("guid".Length).Trim(new char[]{'\''});
                }
                return Guid.Parse(value.ToString());
            }
            else if (type.Equals(typeof(DateTimeOffset)))
            {
                return DateTimeOffset.Parse(value.ToString());
            }
            else if (type.Equals(typeof(TimeSpan)))
            {
                return TimeSpan.Parse(value.ToString());
            }
            else if (type.Equals(typeof(SByte)))
            {
                return SByte.Parse(value.ToString());
            }
            else if (type.Equals(typeof(Byte[])))
            {
                return System.Text.Encoding.Unicode.GetBytes(value.ToString());
            }
            else
            {
                throw new NotImplementedException(
                    string.Format("Don't know how to parse type: {0}", type.Name));
            }
        }
    }
}
