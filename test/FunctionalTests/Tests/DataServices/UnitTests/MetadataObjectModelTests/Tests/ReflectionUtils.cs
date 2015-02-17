//---------------------------------------------------------------------
// <copyright file="ReflectionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;

namespace AstoriaUnitTests.Tests
{
    class ReflectionUtils
    {
        public static object InvokeMethod(object instance, string methodName, params object[] args)
        {
            object obj2;

            try
            {
                obj2 = instance.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, instance, args);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
            return obj2;
        }

        public static object GetProperty(object instance, string propertyName)
        {
            object obj2;
            try
            {
                obj2 = instance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, instance, new object[0]);
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
            return obj2;
        }

        public static void SetProperty(object instance, string propertyName, object value)
        {
            try
            {
                instance.GetType().InvokeMember(propertyName, BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, instance, new object[] { value });
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }
        }
    }
}
