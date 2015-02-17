//---------------------------------------------------------------------
// <copyright file="SR.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace System
{
    internal static class SR
    {
        public const string ArgumentOutOfRange_NeedNonNegNum = "ArgumentOutOfRange_NeedNonNegNum";
        public const string Arg_ArrayPlusOffTooSmall = "Arg_ArrayPlusOffTooSmall";
        public const string Arg_HSCapacityOverflow = "Arg_HSCapacityOverflow";
        public const string InvalidOperation_EnumFailedVersion = "InvalidOperation_EnumFailedVersion";
        public const string InvalidOperation_EnumOpCantHappen = "InvalidOperation_EnumOpCantHappen";

        public static string GetString(string constant)
        {
            return constant;
        }

        public static string GetString(string constant, params object[] parameters)
        {
            return constant;
        }
    }
}
