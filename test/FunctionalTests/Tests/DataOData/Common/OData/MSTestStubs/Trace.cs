//---------------------------------------------------------------------
// <copyright file="Trace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SILVERLIGHT || WINDOWS_PHONE
namespace System.Diagnostics
{
    using System;

    /// <summary>
    /// Stub class so that MStest attributed files compile
    /// </summary>
    public static class Trace
    {
        public static void WriteLine(string traceString)
        {
        }

        public static void WriteLine(Exception error)
        {
        }
    }
}
#endif