//---------------------------------------------------------------------
// <copyright file="MissingPlatformFeatures.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
#if WINDOWS_PHONE
    #region Missing Types
    // a Func with 6 parameters is not in-built to the Windows Phone runtime
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    #endregion
#endif
    /// <summary>
    /// This class contains extension methods for missing platform features
    /// </summary>
    public static class MissingPlatformFeatures
    {
    }
}