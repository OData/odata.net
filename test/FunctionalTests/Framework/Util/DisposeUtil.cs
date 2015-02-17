//---------------------------------------------------------------------
// <copyright file="DisposeUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

/// <summary>Partial utility class.  This set of methods are related to IDisposable.</summary>
internal static partial class Util
{
    /// <summary>Dispose an object if it implements IDisposable.</summary>
    /// <param name="disposable">Object to dispose.</param>
    public static void Dispose(object disposable)
    {
        Dispose(disposable as System.IDisposable);
    }

    /// <summary>Dispose of an IDisposable object.</summary>
    /// <typeparam name="T">Typeof <paramref name="disposable"/> that implements IDisposable.</typeparam>
    /// <param name="disposable">Object to dispose.</param>
    public static void Dispose<T>(T disposable) where T : class, System.IDisposable
    {
        if (null != disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>Dispose of an object and set its reference to null.</summary>
    /// <typeparam name="T">Typeof <paramref name="disposable"/> that implements IDisposable.</typeparam>
    /// <param name="disposable">Object to dispose.</param>
    public static void Dispose<T>(ref T disposable) where T : class, System.IDisposable
    {
        Dispose<T>(System.Threading.Interlocked.Exchange<T>(ref disposable, null));
    }
}
