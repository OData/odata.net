//---------------------------------------------------------------------
// <copyright file="ArgumentValidationUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;

namespace Microsoft.OData.Core.Tests
{
    internal static class ArgumentValidationUtils
    {
        internal static void ShouldThrowOnNullArgument<T>(this Action<T> action, string argumentName) where T : class
        {
            Action withNullValue = () => action(null);
            withNullValue.ShouldThrow<ArgumentNullException>().WithMessage(argumentName, ComparisonMode.Substring);
        }

        internal static void ShouldThrowOnEmptyStringArgument(this Action<string> action, string argumentName)
        {
            Action withEmptyValue = () => action(string.Empty);
            withEmptyValue.ShouldThrow<ArgumentException>().WithMessage(argumentName, ComparisonMode.Substring);
        }

        internal static void ShouldThrowOnNullOrEmptyStringArgument(this Action<string> action, string argumentName)
        {
            action.ShouldThrowOnNullArgument(argumentName);
            action.ShouldThrowOnEmptyStringArgument(argumentName);
        }
    }
}