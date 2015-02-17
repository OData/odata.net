//---------------------------------------------------------------------
// <copyright file="NetworkUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

/// <summary>
/// Network related utility methods.
/// </summary>
public static partial class NetworkUtil
{
    // random port generator
    private static Random portGenerator = new Random();

    public static int GetRandomPortNumber()
    {
        // Generate a random port between 20000 and 40000
        return portGenerator.Next(20000, 40000);
    }
}
