//---------------------------------------------------------------------
// <copyright file="CharMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Net;

namespace System
{
    public class CharMethods
    {
        public static bool IsHighSurrogate(char c)
        {
            return (c >= 0xd800) && (c <= 0xdbff);
        }

        public static bool IsLowSurrogate(char c)
        {
            return (c >= 0xdc00) && (c <= 0xdfff);
        }
    }
}
