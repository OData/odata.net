//---------------------------------------------------------------------
// <copyright file="ExceptionUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
    internal static class ExceptionUtilities
    {
        public static void CheckArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
