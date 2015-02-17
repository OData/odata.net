//---------------------------------------------------------------------
// <copyright file="ExceptionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Data.Test.Astoria;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests
{
    class ExceptionUtils
    {
        public static void ExpectedException<TException>(Action action, string expectedExceptionMessage, string description = null)
        {
            Exception exception = UnwrapAggregateException(TestUtil.RunCatching(action));
            ExceptionUtils.IsExpectedException<TException>(exception, expectedExceptionMessage, description);
        }

        public static void IsExpectedException<TException>(Exception exception, string expectedExceptionMessage, string description = null)
        {
            if (expectedExceptionMessage == null)
            {
                Assert.IsNull(exception, "No exception was expected, but it occured. " + (description ?? string.Empty) + "\r\n" + ((exception == null) ? string.Empty : exception.ToString()), new object[0]);
            }
            else
            {
                Assert.IsNotNull(exception, "Expected " + typeof(TException).FullName + " but it was not thrown. " + (description ?? string.Empty), new object[0]);
                Assert.IsTrue(exception is TException, "Exception had unexpected type " + exception.GetType().FullName + ", expected type is " + typeof(TException).FullName + ". " + description, new object[0]);
                Assert.AreEqual<string>(expectedExceptionMessage, exception.Message, "Unexpected exception message. " + (description ?? string.Empty), new object[0]);
            }
        }

        public static void CheckInvalidConstructorParameters(Type type, string errorMessage, params object[] parameters)
        {
            try
            {
                type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single<ConstructorInfo>().Invoke(parameters);
                Assert.Fail(errorMessage, new object[0]);
            }
            catch (TargetInvocationException exception)
            {
                Assert.IsTrue(exception.InnerException is ArgumentException, "Expecting argument exception", new object[0]);
                Assert.IsTrue(exception.InnerException.Message.Contains(errorMessage), "The exception message doesn't contain the expected string '" + errorMessage + "'.", new object[0]);
            }
        }

        public static void ThrowsException<TException>(Action action, string errorMessage, params object[] errorMessageArguments) where TException : Exception
        {
            bool flag;
            bool flag2;
            try
            {
                action();
                flag = false;
                flag2 = false;
            }
            catch (Exception exception)
            {
                flag2 = true;
                if (exception is TException)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            if (!flag2)
            {
                Assert.Fail("Expected " + typeof(TException).FullName + " but it was not thrown.", new object[0]);
            }
            else
            {
                Assert.IsTrue(flag, FormatErrorMessage(errorMessage, errorMessageArguments), new object[0]);
            }
        }

        private static Exception UnwrapAggregateException(Exception exception)
        {
            AggregateException exception2 = exception as AggregateException;
            if (exception2 == null)
            {
                return exception;
            }
            exception2 = exception2.Flatten();
            Assert.AreEqual<int>(1, exception2.InnerExceptions.Count, "Expected exception count does not match.", new object[0]);
            return exception2.InnerExceptions[0];
        }

        private static string FormatErrorMessage(string errorMessage, params object[] errorMessageArguments)
        {
            string str = errorMessage;
            if ((errorMessageArguments != null) && (errorMessageArguments.Length > 0))
            {
                str = string.Format(CultureInfo.InvariantCulture, errorMessage, errorMessageArguments);
            }
            return str;
        }
    }
}
