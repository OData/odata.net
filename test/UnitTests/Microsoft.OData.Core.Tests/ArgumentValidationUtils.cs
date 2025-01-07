//---------------------------------------------------------------------
// <copyright file="ArgumentValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    internal static class ArgumentValidationUtils
    {
        internal static void ShouldThrowOnNullArgument<T>(this Action<T> action, string argumentName) where T : class
        {
            Action withNullValue = () => action(null);
            Assert.Throws<ArgumentNullException>(argumentName, withNullValue);
        }

        internal static void ShouldThrowOnEmptyStringArgument<T>(this Action<string> action, string argumentName) where T : Exception
        {
            Action withEmptyValue = () => action(string.Empty);
            Exception exception = Assert.Throws<T>(withEmptyValue);
            Assert.Contains(argumentName, exception.Message);
        }

        internal static void ShouldThrowOnNullOrEmptyStringArgument(this Action<string> action, string argumentName)
        {
            action.ShouldThrowOnNullArgument(argumentName);
            action.ShouldThrowOnEmptyStringArgument<ArgumentNullException>(argumentName);
        }
    }
}