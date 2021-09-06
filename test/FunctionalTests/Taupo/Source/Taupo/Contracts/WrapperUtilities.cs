//---------------------------------------------------------------------
// <copyright file="WrapperUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Utility methods used by generated wrapper code.
    /// </summary>
    public static class WrapperUtilities
    {
        private static readonly object[] emptyArray = new object[0];
        private static Dictionary<string, string> platformSpecificMethodMap;

        private static Dictionary<string, string> PlatformMethodMap
        {
            get
            {
                if (platformSpecificMethodMap == null)
                {
                    InitializePlatformSpecificMethodMap();
                }

                return platformSpecificMethodMap;
            }
        }

        /// <summary>
        /// Invokes the method on a wrapped object using reflection.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        public static void InvokeMethodWithoutResult(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments)
        {
            InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments);
        }

        /// <summary>
        /// Invokes the method without result.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        public static void InvokeMethodWithoutResult(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        /// <summary>
        /// Invokes the method without result.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        public static void InvokeMethodWithoutResult(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters)
        {
            InvokeMethodWithoutResult(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes);
        }

        /// <summary>
        /// Invokes the method without result.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        public static void InvokeMethodWithoutResult(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            InvokeMethodWithoutResult(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        /// <summary>
        /// Invokes the method and casts the result to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Result of the method cast to the specific type.</returns>
        public static TResult InvokeMethodAndCast<TResult>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters)
        {
            return InvokeMethodAndCast<TResult>(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes);
        }

        /// <summary>
        /// Invokes the method and cast.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of the method cast to the specific type.</returns>
        public static TResult InvokeMethodAndCast<TResult>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            return InvokeMethodAndCast<TResult>(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        /// <summary>
        /// Invokes the method and casts the result to the specified type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Result of the method cast to the specified type.</returns>
        public static TResult InvokeMethodAndCast<TResult>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments)
        {
            return (TResult)InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments);
        }

        /// <summary>
        /// Invokes the method and cast.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of the method cast to the specific type.</returns>
        public static TResult InvokeMethodAndCast<TResult>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            return (TResult)InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        /// <summary>
        /// Invokes the method and wraps the result using the specified wrapper type.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Result of the method wrapped using the specified wrapper.</returns>
        public static TWrapper InvokeMethodAndWrap<TWrapper>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters)
           where TWrapper : IWrappedObject
        {
            return InvokeMethodAndWrap<TWrapper>(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes);
        }

        /// <summary>
        /// Invokes the method and wrap.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of the method wrapped using the specified wrapper.</returns>
        public static TWrapper InvokeMethodAndWrap<TWrapper>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
           where TWrapper : IWrappedObject
        {
            return InvokeMethodAndWrap<TWrapper>(wrappedObject, methodInfo, parameters, PlatformHelper.EmptyTypes, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        /// <summary>
        /// Invokes the method and wraps the result using the specified wrapper type.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method handle.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Result of the method wrapped using the specified wrapper.</returns>
        public static TWrapper InvokeMethodAndWrap<TWrapper>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments)
           where TWrapper : IWrappedObject
        {
            return InvokeMethodAndWrap<TWrapper>(wrappedObject, methodInfo, parameters, typeArguments, null, null, null, null);
        }

        /// <summary>
        /// Invokes the method and wrap.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of the method wrapped using the specified wrapper.</returns>
        public static TWrapper InvokeMethodAndWrap<TWrapper>(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
           where TWrapper : IWrappedObject
        {
            object unwrappedResult = InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);

            return wrappedObject.Scope.Wrap<TWrapper>(unwrappedResult);
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        public static object GetPropertyValue(IWrappedObject wrappedObject, string propertyName)
        {
            var propertyInfo = wrappedObject.Product.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new TaupoInvalidOperationException("Property '" + propertyName + "' was not found in '" + wrappedObject.Product.GetType() + "'");
            }

            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            return InvokeMethod(wrappedObject, methodInfo, emptyArray);
        }

        /// <summary>
        /// Sets the property value on a wrapped object.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        public static void SetPropertyValue(IWrappedObject wrappedObject, string propertyName, object newValue)
        {
            var propertyInfo = wrappedObject.Product.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new TaupoInvalidOperationException("Property '" + propertyName + "' was not found in '" + wrappedObject.Product.GetType() + "'");
            }

            MethodInfo methodInfo = propertyInfo.GetSetMethod();

            InvokeMethod(wrappedObject, methodInfo, new[] { newValue });
        }

        /// <summary>
        /// Gets the MethodInfo based on the given signature.
        /// </summary>
        /// <param name="type">The type where the method is.</param>
        /// <param name="methodInfoCache">The cache of method infos.</param>
        /// <param name="signature">The signature of the method to look up.</param>
        /// <returns>Method info.</returns>
        public static MethodInfo GetMethodInfo(Type type, IDictionary<string, MethodInfo> methodInfoCache, string signature)
        {
            MethodInfo methodInfo;
            if (!methodInfoCache.TryGetValue(signature, out methodInfo))
            {
                methodInfo = WrapperUtilities.GetMethodInfo(type, signature);
                methodInfoCache[signature] = methodInfo;
            }

            return methodInfo;
        }

        /// <summary>
        /// Gets the method handle for a method with a given generic signature.
        /// </summary>
        /// <param name="type">The type (definition in case of generics).</param>
        /// <param name="signature">The signature of the method.</param>
        /// <returns>Method info.</returns>
        public static MethodInfo GetMethodInfo(Type type, string signature)
        {
            ExceptionUtilities.CheckObjectNotNull(PlatformMethodMap, "Platform method map was not initialized");
            if (PlatformMethodMap.ContainsKey(signature))
            {
                signature = PlatformMethodMap[signature];
            }

            foreach (var method in type.GetMethods())
            {
                if (MethodMatches(signature, method))
                {
                    return method;
                }
            }

            throw new TaupoInvalidOperationException("Could not find method matching signature: '" + signature + "'.");
        }

        /// <summary>
        /// Gets the type handle for a given name of the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeCache">The cache of known type handles</param>
        /// <returns>Runtime type handle.</returns>
        public static Type GetTypeFromCache(string typeName, string assemblyName, IDictionary<string, Type> typeCache)
        {
            string cacheKey = assemblyName + "." + typeName;
            Type type;
            if (!typeCache.TryGetValue(cacheKey, out type))
            {
                type = WrapperUtilities.GetTypeFromAssembly(typeName, assemblyName);
                typeCache[cacheKey] = type;
            }

            return type;
        }

        /// <summary>
        /// Gets the type for a given name of the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The requested type.</returns>
        public static Type GetTypeFromAssembly(string typeName, string assemblyName)
        {
            Type type = null;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Substring(0, assembly.FullName.IndexOf(",", StringComparison.Ordinal)) == assemblyName)
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        break;
                    }
                }
            }

            ExceptionUtilities.CheckObjectNotNull(type, "Type {0} not found", typeName);
            return type;
        }

        /// <summary>
        /// Invokes the method on a wrapped object using reflection.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Result of method invocation.</returns>
        internal static object InvokeMethod(IWrappedObject wrappedObject, MethodBase methodInfo, object[] parameters)
        {
            return InvokeMethod(wrappedObject, methodInfo, parameters, null, null, null, null);
        }

        /// <summary>
        /// Invokes the method on a wrapped object using reflection.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of method invocation.</returns>
        internal static object InvokeMethod(IWrappedObject wrappedObject, MethodBase methodInfo, object[] parameters, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            object[] unwrappedParameters = UnwrapParameters(methodInfo.GetParameters(), parameters);
            int callId = wrappedObject.Scope.BeginTraceCall(methodInfo, wrappedObject.Product, unwrappedParameters);

            try
            {
                // add the call for the before event
                if (beforeEvent != null)
                {
                    object eventTracker = wrappedObject.GetType().GetProperty("TrackEvents").GetValue(wrappedObject, null);
                    ExceptionUtilities.CheckObjectNotNull(eventTracker, "TrackEvents property is null");

                    object beforeArgs = beforeEventArgs.Invoke(parameters);

                    beforeEvent.Invoke(eventTracker, new object[] { beforeArgs });
                }

                // Product call
                object result = methodInfo.Invoke(wrappedObject.Product, unwrappedParameters);

                // add the call for the after event
                if (afterEvent != null)
                {
                    List<object> afterParams = parameters.ToList();

                    if (result != null)
                    {
                        afterParams.Add(result);
                    }

                    object afterArgs = afterEventArgs.Invoke(afterParams.ToArray());
                    object eventTracker = wrappedObject.GetType().GetProperty("TrackEvents").GetValue(wrappedObject, null);
                    ExceptionUtilities.CheckObjectNotNull(eventTracker, "TrackEvents property is null");
                    afterEvent.Invoke(eventTracker, new object[] { afterArgs });
                }

                if (callId != 0)
                {
                    wrappedObject.Scope.TraceResult(callId, methodInfo, wrappedObject.Product, unwrappedParameters, ref result);
                }

                int index = 0;
                foreach (ParameterInfo param in methodInfo.GetParameters())
                {
                    // Note: we cannot wrap out/ref parameters here as we simply don't know the type of the wrapper.
                    if (param.ParameterType.IsByRef)
                    {
                        parameters[index] = unwrappedParameters[index];
                    }

                    index++;
                }

                return result;
            }
            catch (TargetInvocationException exception)
            {
                if (callId != 0)
                {
                    wrappedObject.Scope.TraceException(callId, methodInfo, wrappedObject.Product, exception.InnerException);
                }

                throw;
            }
        }

        /// <summary>
        /// Invokes the method on a wrapped object using reflection.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Result of method invocation.</returns>
        internal static object InvokeMethod(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments)
        {
            return InvokeMethod(wrappedObject, methodInfo, parameters, typeArguments, null, null, null, null);
        }

        /// <summary>
        /// Invokes the method on a wrapped object using reflection.
        /// </summary>
        /// <param name="wrappedObject">The wrapped object.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="beforeEvent">The before event.</param>
        /// <param name="afterEvent">The after event.</param>
        /// <param name="beforeEventArgs">The before event args.</param>
        /// <param name="afterEventArgs">The after event args.</param>
        /// <returns>Result of method invocation.</returns>
        internal static object InvokeMethod(IWrappedObject wrappedObject, MethodInfo methodInfo, object[] parameters, Type[] typeArguments, MethodInfo beforeEvent, MethodInfo afterEvent, ConstructorInfo beforeEventArgs, ConstructorInfo afterEventArgs)
        {
            ExceptionUtilities.Assert(methodInfo != null, "Method was not found");

            MethodInfo matchingMethod = null;
            Type actualType = wrappedObject.Product.GetType();
            for (Type t = actualType; t != null; t = t.GetBaseType())
            {
                var typeArgumentMapping = CreateTypeArgumentMapping(t);
                matchingMethod = FindMatchingMethod(methodInfo, actualType.GetMethods(), typeArgumentMapping);
                if (matchingMethod != null)
                {
                    break;
                }
            }

            if (matchingMethod == null)
            {
                foreach (Type i in actualType.GetInterfaces())
                {
                    var argumentMapping = CreateTypeArgumentMapping(i);
                    matchingMethod = FindMatchingMethod(methodInfo, i.GetMethods(), argumentMapping);
                    if (matchingMethod != null)
                    {
                        break;
                    }
                }
            }

            ExceptionUtilities.Assert(matchingMethod != null, "Could not resolve method properly. Method: " + methodInfo.Name + ". Type: " + actualType.ToString());

            if (matchingMethod.IsGenericMethod)
            {
                methodInfo = matchingMethod.MakeGenericMethod(typeArguments);
            }
            else
            {
                methodInfo = matchingMethod;
            }

            return InvokeMethod(wrappedObject, methodInfo, parameters, beforeEvent, afterEvent, beforeEventArgs, afterEventArgs);
        }

        internal static object Unwrap(Type expectedType, object value)
        {
            var wrappedArray = value as WrappedArray;
            if (wrappedArray != null)
            {
                var productArray = (Array)wrappedArray.Product;
                var elementType = expectedType.GetElementType();

                var unwrappedArray = Array.CreateInstance(elementType, productArray.Length);
                for (int i = 0; i < productArray.Length; i++)
                {
                    unwrappedArray.SetValue(Unwrap(elementType, productArray.GetValue(i)), i);
                }

                return unwrappedArray;
            }

            var wrapper = value as IWrappedObject;
            if (wrapper != null && !(value is IProxyObject))
            {
                return wrapper.Product;
            }

            return value;
        }

        internal static object[] UnwrapParameters(ParameterInfo[] parameterInfos, params object[] parameters)
        {
            ExceptionUtilities.CheckArgumentNotNull(parameterInfos, "parameterInfos");
            ExceptionUtilities.Assert(parameterInfos.Length == parameters.Length, "Unexpected number of parameters");

            if (parameters.Length == 0)
            {
                return emptyArray;
            }

            var unwrappedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                unwrappedParameters[i] = Unwrap(parameterInfos[i].ParameterType, parameters[i]);
            }

            return unwrappedParameters;
        }

        internal static bool MethodMatches(string methodSignature, MethodInfo methodInfo)
        {
            if (methodInfo.IsGenericMethod)
            {
                methodInfo = methodInfo.GetGenericMethodDefinition();
            }

            if (methodInfo.DeclaringType.IsGenericType())
            {
                var typeArgumentMapping = CreateTypeArgumentMapping(methodInfo.DeclaringType);
                var declaringTypeGenericDefinition = methodInfo.DeclaringType.GetGenericTypeDefinition();
                foreach (var candidate in declaringTypeGenericDefinition.GetMethods())
                {
                    if (AreMatchingMethods(methodInfo, candidate, typeArgumentMapping))
                    {
                        methodInfo = candidate;
                        break;
                    }
                }
            }

            return methodSignature.Equals(methodInfo.ToString(), StringComparison.Ordinal);
        }

        private static Dictionary<Type, Type> CreateTypeArgumentMapping(Type type)
        {
            if (type.IsGenericType())
            {
                return type.GetGenericTypeDefinition().GetGenericArguments().Zip(type.GetGenericArguments(), (g, a) => new { g, a }).ToDictionary(k => k.g, e => e.a);
            }
            else
            {
                return new Dictionary<Type, Type>();
            }
        }

        private static Type MapTypeArguments(Type type, Dictionary<Type, Type> argumentMapping)
        {
            if (type.IsGenericType())
            {
                var mappedGenericArguments = type.GetGenericArguments().Select(ga => MapTypeArguments(ga, argumentMapping)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(mappedGenericArguments);
            }
            else
            {
                Type mappedValue;
                if (argumentMapping.TryGetValue(type, out mappedValue))
                {
                    return mappedValue;
                }
                else
                {
                    return type;
                }
            }
        }

        private static bool AreMatchingMethods(MethodInfo concreteMethod, MethodInfo genericMethod, Dictionary<Type, Type> typeArgumentMapping)
        {
            if (concreteMethod.IsGenericMethod)
            {
                concreteMethod.GetGenericMethodDefinition();
            }

            if (concreteMethod.Name != genericMethod.Name)
            {
                return false;
            }

            var concreteMethodParameters = concreteMethod.GetParameters();
            var genericMethodParameters = genericMethod.GetParameters();

            if (concreteMethodParameters.Length != genericMethodParameters.Length)
            {
                return false;
            }

            bool allParametersMatch = true;
            for (int i = 0; i < genericMethodParameters.Length; i++)
            {
                if (concreteMethodParameters[i].ParameterType != MapTypeArguments(genericMethodParameters[i].ParameterType, typeArgumentMapping))
                {
                    allParametersMatch = false;
                    break;
                }
            }

            if (!allParametersMatch)
            {
                return false;
            }

            if (concreteMethod.ReturnType != MapTypeArguments(genericMethod.ReturnType, typeArgumentMapping))
            {
                return false;
            }

            return true;
        }

        private static MethodInfo FindMatchingMethod(MethodInfo expected, IEnumerable<MethodInfo> candidates, Dictionary<Type, Type> argumentMapping)
        {
            foreach (var candidate in candidates)
            {
                if (AreMatchingMethods(candidate, expected, argumentMapping))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static void InitializePlatformSpecificMethodMap()
        {
            platformSpecificMethodMap = new Dictionary<string, string>();
        }
    }
}
