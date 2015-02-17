//---------------------------------------------------------------------
// <copyright file="ReflectionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Platforms;
    #endregion Namespaces

    /// <summary>
    /// Helper method for using reflection.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Creates an instance of the specified through either public or non-public constructor.
        /// </summary>
        /// <param name="type">The type to create an instance for.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The newly created instance.</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            try
            {
                ConstructorInfo ctor = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    Type.DefaultBinder,
                    args.Select(a => a == null ? typeof(object) : a.GetType()).ToArray(),
                    null);
                ExceptionUtilities.CheckObjectNotNull(
                    ctor,
                    "Failed to create instance of type {0}. Constructor with types {1} not found.",
                    type.ToString(),
                    string.Join(", ", args.Select(a => a == null ? "System.Object" : a.GetType().ToString())));
                return ctor.Invoke(args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Creates an instance of the specified through either public or non-public constructor.
        /// </summary>
        /// <param name="type">The type to create an instance for.</param>
        /// <param name="argTypes">Argument types of the constructor to use.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The newly created instance.</returns>
        public static object CreateInstance(Type type, Type[] argTypes, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            try
            {
                ConstructorInfo ctor = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    Type.DefaultBinder,
                    argTypes,
                    null);
                ExceptionUtilities.CheckObjectNotNull(
                    ctor,
                    "Failed to create instance of type {0}. Constructor with types {1} not found.",
                    type.ToString(),
                    string.Join(", ", argTypes.Select(a => a.ToString())));
                return ctor.Invoke(args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes an instance method with the specified name.
        /// </summary>
        /// <param name="instance">The instance to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        public static object InvokeMethod(object instance, string methodName, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");

            try
            {
                return instance.GetType().InvokeMember(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    instance,
                    args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes an instance method with the specified name.
        /// </summary>
        /// <param name="instance">The instance to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="argTypes">Array of parameter types.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        /// <remarks>Use this method if possible ambiguity based on parameter types could occur.</remarks>
        public static object InvokeMethod(object instance, string methodName, Type[] argTypes, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");

            var method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                Type.DefaultBinder,
                argTypes,
                null);
            try
            {
                return method.Invoke(instance, args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes an instance generic method with the specified name.
        /// </summary>
        /// <param name="instance">The instance to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="genericParameters">Generic parameters.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        public static object InvokeGenericMethod(object instance, string methodName, Type[] genericParameters, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");
            ExceptionUtilities.CheckArgumentNotNull(genericParameters, "genericParameters");

            var method = instance.GetType().GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(genericParameters);

            try
            {
                return method.Invoke(instance, args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes a static method with the specified name.
        /// </summary>
        /// <param name="type">The type to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        public static object InvokeMethod(Type type, string methodName, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");

            try
            {
                return type.InvokeMember(
                    methodName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    null,
                    args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes a static method with the specified name.
        /// </summary>
        /// <param name="type">The type to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="argTypes">Array of parameter types.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        /// <remarks>Use this method if possible ambiguity based on parameter types could occur.</remarks>
        public static object InvokeMethod(Type type, string methodName, Type[] argTypes, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");

            var method = type.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                Type.DefaultBinder,
                argTypes,
                null);
            try
            {
                return method.Invoke(null, args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Invokes a static generic method with the specified name.
        /// </summary>
        /// <param name="type">The type to invoke the method on.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="genericParameters">Generic parameters.</param>
        /// <param name="args">Arguments to pass to the method.</param>
        /// <returns>The return value of the method (if any).</returns>
        public static object InvokeGenericMethod(Type type, string methodName, Type[] genericParameters, params object[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");
            ExceptionUtilities.CheckArgumentNotNull(genericParameters, "genericParameters");

            var method = type.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericParameters);

            try
            {
                return method.Invoke(null, args);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Gets a property with the specified name.
        /// </summary>
        /// <param name="instance">The instance to get the property on.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The value of the property.</returns>
        public static object GetProperty(object instance, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            try
            {
                return instance.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                    Type.DefaultBinder,
                    instance,
                    new object[0]);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Sets a property with the specified name.
        /// </summary>
        /// <param name="instance">The instance to set the property on.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            try
            {
                instance.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    instance,
                    new object[] { value });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Gets a field with the specified name.
        /// </summary>
        /// <param name="instance">The instance to get the field on.</param>
        /// <param name="fieldName">The name of the field to get.</param>
        /// <returns>The value of the field.</returns>
        public static object GetField(object instance, string fieldName)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(fieldName, "fieldName");

            try
            {
                return instance.GetType().InvokeMember(
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField,
                    Type.DefaultBinder,
                    instance,
                    new object[0]);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Sets a field with the specified name.
        /// </summary>
        /// <param name="instance">The instance to set the field on.</param>
        /// <param name="fieldName">The name of the field to set.</param>
        /// <param name="value">The value to set the field to.</param>
        public static void SetField(object instance, string fieldName, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(fieldName, "fieldName");

            try
            {
                instance.GetType().InvokeMember(
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                    Type.DefaultBinder,
                    instance,
                    new object[] { value });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }


        /// <summary>
        /// Gets the value of an enumeration option.
        /// </summary>
        /// <param name="enumerationType">The type of the enumeration.</param>
        /// <param name="value">The value to get from the enumeration.</param>
        /// <returns>The value of the option on the enumeration.</returns>
        public static object GetEnumerationValue(Type enumerationType, string value)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumerationType, "enumerationType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(value, "value");

            foreach (var enumValue in EnumExtensionMethods.GetValues(enumerationType))
            {
                if (enumValue.ToString() == value)
                {
                    return enumValue;
                }
            }

            throw new ArgumentException(String.Format("Failed to find value '{0}' in enum type '{1}", value, enumerationType.Name));
        }
    }
}
