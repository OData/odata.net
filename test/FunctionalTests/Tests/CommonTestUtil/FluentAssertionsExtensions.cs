//---------------------------------------------------------------------
// <copyright file="FluentAssertionsExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Specialized;

    public static class FluentAssertionsExtensions
    {
#if !NETCOREAPP1_0
        [CLSCompliant(false)]
#endif
        public static ExceptionAssertions<TException> ShouldThrow<TException, TReturn>(this Func<TReturn> func) where TException : Exception
        {
            Action action = () => func();
            return action.ShouldThrow<TException>();
        }
    }
}