//---------------------------------------------------------------------
// <copyright file="MissingSystemTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Platforms
{
#if WINDOWS_PHONE
    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate TResult Func<T1, T2, T3, T4, T5,TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
#endif
    /// <summary>
    /// Dummy class to declare missing Func and Action types
    /// </summary>
    public class MissingSystemTypes
    {
    }
}