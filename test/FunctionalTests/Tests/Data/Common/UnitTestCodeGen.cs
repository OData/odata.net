//---------------------------------------------------------------------
// <copyright file="UnitTestCodeGen.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

//#define TEST_FRAMEWORK

#if TEST_FRAMEWORK
namespace System.Data.Test.Framework
#else
namespace Suites.Data.Test
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;
    using MethodDef = System.Collections.Generic.KeyValuePair<System.RuntimeMethodHandle, System.RuntimeTypeHandle>;

    public static class UnitTestCodeGen
    {
        /// <summary>delegate for a dynamic method to get the property value from a CLR instance</summary> 
        private delegate object Method(object instance, object[] parameters);

        #region InvokeMethod
        public static TTarget InvokeConstructor<TTarget>(Type[] parameterTypes, params object[] parameterValues)
        {
            object result = InvokeMethod(typeof(TTarget), ".ctor", parameterTypes, Type.EmptyTypes, BindingFlags.Default, null, parameterValues);
            return (TTarget)result;
        }
        public static object InvokeConstructor(Type targetType, Type[] parameterTypes, params object[] parameterValues)
        {
            object result = InvokeMethod(targetType, ".ctor", parameterTypes, Type.EmptyTypes, BindingFlags.Default, null, parameterValues);
            return result;
        }

        /// <summary>Get property value from target.</summary>
        public static object InvokePropertyGet(PropertyInfo propertyInfo, object target)
        {
            return InvokePropertyGet(propertyInfo, target, (object[])null);
        }
        /// <summary>Get indexed property value from target.</summary>
        public static object InvokePropertyGet(PropertyInfo propertyInfo, object target, params object[] parameterValues)
        {
            DelegateMethod method = GetGetProperty(propertyInfo);
            if (null == method)
            {
                ThrowMissingMethodException(propertyInfo.DeclaringType, propertyInfo.Name, Type.EmptyTypes, Type.EmptyTypes);
            }
            return method.Method(target, parameterValues);
        }
        /// <summary>Set property value on target; supports indexed properties.</summary>
        public static void InvokePropertySet(PropertyInfo propertyInfo, object target, params object[] parameterValues)
        {
            DelegateMethod method = GetSetProperty(propertyInfo);
            if (null == method)
            {
                ThrowMissingMethodException(propertyInfo.DeclaringType, propertyInfo.Name, Type.EmptyTypes, Type.EmptyTypes);
            }
            method.Method(target, parameterValues);
        }
        /// <summary>Invoke a parameterless method.</summary>
        public static TResult InvokeMethod<TTarget, TResult>(string memberName, object target)
        {
            object result = InvokeMethod(typeof(TTarget), memberName, Type.EmptyTypes, Type.EmptyTypes, BindingFlags.Default, target, (Object[])null);
            return (TResult)result;
        }
        /// <summary>Invoke a method.</summary>
        public static TResult InvokeMethod<TTarget, TResult>(string memberName, Type[] parameterTypes, Type[] genericArguments, object target, params object[] parameterValues)
        {
            object result = InvokeMethod(typeof(TTarget), memberName, parameterTypes, genericArguments, BindingFlags.Default, target, parameterValues);
            return (TResult)result;
        }
        /// <summary>Invoke a method.</summary>
        public static object InvokeMethod(Type methodTarget, string memberName, Type[] parameterTypes, Type[] genericArguments, object target, params object[] parameterValues)
        {
            return InvokeMethod(methodTarget, memberName, parameterTypes, genericArguments, BindingFlags.Default, target, parameterValues);
        }

        /// <summary>Invoke a method.</summary>
        public static object InvokeMethod(Type methodTarget, string memberName, Type[] parameterTypes, Type[] genericArguments, BindingFlags bindingFlags, object target, params object[] parameterValues)
        {
            if (null == memberName) { memberName = ".ctor"; }
            if (null == parameterTypes) { parameterTypes = Type.EmptyTypes; }
            if (null == genericArguments) { genericArguments = Type.EmptyTypes; }
            if (null == parameterValues) { parameterValues = EmptyValues; }

#if TEST_FRAMEWORK
            if (parameterTypes.Length != parameterValues.Length)
            {   // so that parameter values can be derived
                if (".ctor" == memberName) { bindingFlags |= BindingFlags.CreateInstance; }
                return methodTarget.InvokeMember(memberName, bindingFlags, null, target, parameterValues);
            }
#else
            if (parameterTypes.Length != parameterTypes.Length)
            {
                ThrowTargetParameterCountException();
            }
#endif
            DelegateMethod method = GetMethod(methodTarget, memberName, parameterTypes, genericArguments);
            if (null == method)
            {
                ThrowMissingMethodException(methodTarget, memberName, parameterTypes, genericArguments);
            }
            return method.Method(target, parameterValues);
        }
        private static void ThrowMissingMethodException(Type methodTarget, string memberName, Type[] parameterTypes, Type[] genericArguments)
        {
#if TEST_FRAMEWORK
            string[] typeNames = new string[parameterTypes.Length];
            for (int i = 0; i < parameterTypes.Length; i++)
                typeNames[i] = parameterTypes[i].ToString();

            string[] genericArgNames = new string[genericArguments.Length];
            for (int i = 0; i < genericArguments.Length; i++)
                genericArgNames[i] = genericArguments[i].ToString();

            string memberString = methodTarget.FullName + "." + memberName + (genericArgNames.Length > 0 ? "<" + string.Join(",", genericArgNames) + ">" : "") + "(" + string.Join(",", typeNames) + ")";
            throw new fxTestFailedException("Unable to find member: " + memberString);            
#else
            throw new MissingMethodException(methodTarget.FullName, memberName);
#endif
        }
        private static void ThrowTargetParameterCountException()
        {
            throw new TargetParameterCountException();
        }
        #endregion

        #region Get DelegateMethod
        private static DelegateMethod GetGetProperty(PropertyInfo propertyInfo)
        {
            MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
            if (null == methodInfo)
            {
                ThrowPropertyNoGetter();
            }
            return WrapMethod(methodInfo);
        }
        private static DelegateMethod GetSetProperty(PropertyInfo propertyInfo)
        {
            MethodInfo methodInfo = propertyInfo.GetSetMethod(true);
            if (null == methodInfo)
            {
                ThrowPropertyNoSetter();
            }
            return WrapMethod(methodInfo);
        }
        private static DelegateMethod GetMethod(Type targetType, string methodName, Type[] parametersTypes, Type[] genericArguments)
        {
            DelegateMethod method = FindDelegateMethod(targetType, methodName, parametersTypes, genericArguments);
            if (null == method)
            {
                MethodBase methodInfo = MatchMethod(targetType, methodName, parametersTypes, genericArguments);
                if (null != methodInfo)
                {
                    method = WrapMethod(methodInfo, parametersTypes, genericArguments);
                }
            }
            return method;
        }
        private static DelegateMethod FindDelegateMethod(Type targetType, string methodName, Type[] parametersTypes, Type[] genericArguments)
        {
            Debug.Assert(null != targetType, "null targetType");
            Debug.Assert(!String.IsNullOrEmpty(methodName), "empty methodName");

            List<DelegateMethod> methodList;
            if (DelegateMethodCache.TryGetValue(methodName, out methodList))
            {
                foreach (DelegateMethod member in methodList)
                {
                    if (member.AreEqual(targetType, parametersTypes, genericArguments))
                    {
                        return member;
                    }
                }
            }
            return null;
        }
        private static MethodBase MatchMethod(Type targetType, string memberName, Type[] parametersTypes, Type[] genericArguments)
        {
            MemberInfo[] members = targetType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MemberInfo memberInfo in members)
            {
                MethodBase methodInfo = null;
                if (memberName == memberInfo.Name)
                {
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Method:
                            methodInfo = AreParametersEqual((MethodInfo)memberInfo, parametersTypes, genericArguments);
                            break;
                        case MemberTypes.Property:
                            methodInfo = AreParametersEqual(((PropertyInfo)memberInfo).GetGetMethod(true), parametersTypes, genericArguments) ??
                                         AreParametersEqual(((PropertyInfo)memberInfo).GetSetMethod(true), parametersTypes, genericArguments);
                            break;
                        case MemberTypes.Event:
                            methodInfo = AreParametersEqual(((EventInfo)memberInfo).GetAddMethod(true), parametersTypes, genericArguments) ??
                                         AreParametersEqual(((EventInfo)memberInfo).GetRemoveMethod(true), parametersTypes, genericArguments);
                            break;
                        case MemberTypes.Constructor:
                            methodInfo = AreParametersEqual((ConstructorInfo)memberInfo, parametersTypes);
                            break;
                        case MemberTypes.Field:
                        default:
                            throw new NotSupportedException(memberInfo.MemberType.ToString());
                    }
                    if (null != methodInfo)
                    {
                        if ((null != genericArguments) && (0 < genericArguments.Length))
                        {
                            methodInfo = ((MethodInfo)methodInfo).MakeGenericMethod(genericArguments);
                        }
                        return methodInfo;
                    }
                }
            }
            return null;
        }
        private static DelegateMethod WrapMethod(MethodInfo methodInfo)
        {
            DelegateMethod methodWrapper;
            if (!DelegateMethodHandleCache.TryGetValue(new MethodDef(methodInfo.MethodHandle, methodInfo.DeclaringType.TypeHandle), out methodWrapper))
            {
                ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                Type[] parameterTypes = ((0 < parameterInfo.Length) ? new Type[parameterInfo.Length] : Type.EmptyTypes);
                for (int i = 0; i < parameterInfo.Length; ++i)
                {
                    parameterTypes[i] = parameterInfo[i].ParameterType;
                }
                Type[] genericArguments = ((methodInfo.IsGenericMethod) ? methodInfo.GetGenericArguments() : Type.EmptyTypes);

                methodWrapper = WrapMethodInfo(methodInfo, parameterTypes, genericArguments);
            }
            return methodWrapper;
        }
        private static DelegateMethod WrapMethod(MethodBase methodInfo, Type[] parameterTypes, Type[] genericArguments)
        {
            DelegateMethod methodWrapper;
            if (!DelegateMethodHandleCache.TryGetValue(new MethodDef(methodInfo.MethodHandle, methodInfo.DeclaringType.TypeHandle), out methodWrapper))
            {
                methodWrapper = WrapMethodInfo(methodInfo, parameterTypes, genericArguments);
            }
            return methodWrapper;
        }
        private static DelegateMethod WrapMethodInfo(MethodBase methodInfo, Type[] parameterTypes, Type[] genericArguments)
        {
            Method method = CreateMethod(methodInfo, parameterTypes);

            DelegateMethod methodWrapper = new DelegateMethod(methodInfo, parameterTypes, genericArguments, method);
            DelegateMethodHandleCache.Add(methodWrapper.MethodHandle, methodWrapper);

            List<DelegateMethod> methodList;
            if (!DelegateMethodCache.TryGetValue(methodWrapper.MethodName, out methodList))
            {
                methodList = new List<DelegateMethod>();
                DelegateMethodCache.Add(methodWrapper.MethodName, methodList);
            }
            methodList.Add(methodWrapper);
            return methodWrapper;
        }
        #endregion

        #region DelegateMethod
        private readonly static Dictionary<string, List<DelegateMethod>> DelegateMethodCache = new Dictionary<string, List<DelegateMethod>>();
        private readonly static Dictionary<MethodDef, DelegateMethod> DelegateMethodHandleCache = new Dictionary<MethodDef, DelegateMethod>();

        private sealed class DelegateMethod
        {
            internal readonly MethodDef MethodHandle;
            internal readonly string MethodName;
            internal readonly Type[] ParametersTypes;
            internal readonly Type[] GenericArguments;
            internal readonly Method Method;

            internal DelegateMethod(MethodBase methodInfo, Type[] parametersTypes, Type[] genericArguments, Method method)
            {
                this.MethodHandle = new MethodDef(methodInfo.MethodHandle, methodInfo.DeclaringType.TypeHandle);
                this.MethodName = methodInfo.Name;

                this.ParametersTypes = ((0 < (parametersTypes ?? Type.EmptyTypes).Length) ? (Type[])parametersTypes.Clone() : Type.EmptyTypes);
                this.GenericArguments = ((0 < (genericArguments ?? Type.EmptyTypes).Length) ? (Type[])genericArguments.Clone() : Type.EmptyTypes);
                this.Method = method;
            }

            internal bool AreEqual(Type targetType, Type[] parametersTypes, Type[] genericArguments)
            {
                return (targetType.TypeHandle.Equals(MethodHandle.Value) &&
                        AreTypesEqual(ParametersTypes, parametersTypes ?? Type.EmptyTypes) &&
                        AreTypesEqual(GenericArguments, genericArguments ?? Type.EmptyTypes));
            }
        }
        private static bool AreTypesEqual(Type[] a, Type[] b)
        {
            if (a.Length != b.Length) { return false; }
            for (int i = 0; i < a.Length; ++i) { if (a[i] != b[i]) { return false; } }
            return true;
        }
        private static bool AreTypesEqual(ParameterInfo[] a, Type[] b)
        {
            if (a.Length != b.Length) { return false; }
            for (int i = 0; i < a.Length; ++i) { if (a[i].ParameterType != b[i]) { return false; } }
            return true;
        }
        private static MethodBase AreParametersEqual(MethodInfo methodInfo, Type[] parameterTypes, Type[] genericArguments)
        {
            return (((null != methodInfo) &&
                     AreTypesEqual(methodInfo.GetParameters(), parameterTypes) &&
                     ((methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments() : Type.EmptyTypes).Length == genericArguments.Length))
                    ? methodInfo
                    : null);
        }
        private static MethodBase AreParametersEqual(ConstructorInfo methodInfo, Type[] parameterTypes)
        {
            return (((null != methodInfo) &&
                     AreTypesEqual(methodInfo.GetParameters(), parameterTypes))
                    ? methodInfo
                    : null);
        }




        #endregion

        #region get the delegate

        private static Method CreateMethod(MethodBase mi, Type[] parameterTypes)
        {
            if (null == mi)
            {
                ThrowPropertyNoMethod();
            }

            // because CreateDynamicMethod asserts ReflectionPermission, method is "elevated" and must be treated carefully
            DynamicMethod method = CreateDynamicMethod(mi.Name, typeof(object), new Type[] { typeof(object), typeof(object[]) });
            ILGenerator gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            if (mi.IsStatic || mi.IsConstructor)
            {   // instance must be null for static methods, else TargetException
                Label labelFalse = gen.DefineLabel();
                gen.Emit(OpCodes.Brfalse_S, labelFalse);
                gen.Emit(OpCodes.Ldsfld, typeof(UnitTestCodeGen).GetField("StaticInstanceNotNull", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField));
                gen.Emit(OpCodes.Throw);
                gen.MarkLabel(labelFalse);
            }
            else
            {   // verify correct instance type for instance method, else InvalidCastException
                Label labelTrue = gen.DefineLabel();
                gen.Emit(OpCodes.Brtrue_S, labelTrue);
                gen.Emit(OpCodes.Ldsfld, typeof(UnitTestCodeGen).GetField("InstanceIsNull", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField));
                gen.Emit(OpCodes.Throw);
                gen.MarkLabel(labelTrue);

                gen.Emit(OpCodes.Ldarg_0);
                if (mi.DeclaringType.IsValueType)
                {
                    LocalBuilder local = gen.DeclareLocal(mi.DeclaringType);
                    gen.Emit(OpCodes.Unbox_Any, mi.DeclaringType);
                    gen.Emit(OpCodes.Stloc_S, local);
                    gen.Emit(OpCodes.Ldloca_S, local);
                }
                else
                {
                    gen.Emit(OpCodes.Castclass, mi.DeclaringType);
                }
            }

            Type realType;
            for (int i = 0; i < parameterTypes.Length; ++i)
            {
                realType = parameterTypes[i];
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldelem_Ref);

                if (realType.IsValueType)
                {
                    if (realType.IsGenericType && (typeof(Nullable<>) == realType.GetGenericTypeDefinition()))
                    {
                        // need to convert type for better compatibility
                        LocalBuilder local = gen.DeclareLocal(realType);
                        Label labelTrue = gen.DefineLabel();
                        Label label = gen.DefineLabel();
                        // check if paremeter is null
                        gen.Emit(OpCodes.Brtrue_S, labelTrue);
                        // parameter is null
                        // convert it to default(realType)
                        gen.Emit(OpCodes.Ldloca_S, local);
                        gen.Emit(OpCodes.Initobj, realType);
                        gen.Emit(OpCodes.Ldloc_0);
                        gen.Emit(OpCodes.Br_S, label);

                        // parameter is not null, unbox it (new Nullable<T>((T)value))
                        gen.MarkLabel(labelTrue);
                        gen.Emit(OpCodes.Ldarg_1);
                        gen.Emit(OpCodes.Ldc_I4, i);
                        gen.Emit(OpCodes.Ldelem_Ref);
                        gen.Emit(OpCodes.Unbox_Any, realType.GetGenericArguments()[0]);
                        gen.Emit(OpCodes.Newobj, realType.GetConstructor(new Type[] { realType.GetGenericArguments()[0] }));
                        gen.MarkLabel(label);
                    }
                    else
                    {
                        // need to unbox
                        gen.Emit(OpCodes.Unbox_Any, realType);
                    }
                }
                else
                {
                    gen.Emit(OpCodes.Castclass, realType);
                }
            }

            if (mi.IsConstructor)
            {
                gen.Emit(OpCodes.Newobj, (ConstructorInfo)mi);
            }
            else
            {
                gen.Emit(mi.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, (MethodInfo)mi);
                realType = ((MethodInfo)mi).ReturnType;
                if (typeof(void) == realType)
                {
                    gen.Emit(OpCodes.Ldnull);
                }
                else if (realType.IsValueType)
                {
                    if (realType.IsGenericType && (typeof(Nullable<>) == realType.GetGenericTypeDefinition()))
                    {
                        Label lableFalse = gen.DefineLabel();
                        LocalBuilder local = gen.DeclareLocal(realType);
                        gen.Emit(OpCodes.Stloc_S, local);

                        gen.Emit(OpCodes.Ldloca_S, local);
                        gen.Emit(OpCodes.Call, realType.GetMethod("get_HasValue"));
                        gen.Emit(OpCodes.Brfalse_S, lableFalse);

                        gen.Emit(OpCodes.Ldloca_S, local);
                        gen.Emit(OpCodes.Call, realType.GetMethod("get_Value"));
                        gen.Emit(OpCodes.Box, realType.GetGenericArguments()[0]);
                        gen.Emit(OpCodes.Ret);

                        gen.MarkLabel(lableFalse);
                        gen.Emit(OpCodes.Ldnull);
                    }
                    else
                    {
                        // need to box to return value as object
                        gen.Emit(OpCodes.Box, realType);
                    }
                }
            }
            gen.Emit(OpCodes.Ret);

            Delegate function = method.CreateDelegate(typeof(Method));
#if DEBUG
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareDelegate(function);
#endif
            return (Method)function;
        }

        private static void ThrowPropertyNoGetter()
        {
            throw new InvalidOperationException("Property getter does not exist.");
        }
        private static void ThrowPropertyNoSetter()
        {
            throw new InvalidOperationException("Property setter does not exist.");
        }
        private static void ThrowPropertyNoMethod()
        {
            throw new InvalidOperationException("Method does not exist.");
        }

        #endregion

        #region Lightweight code generation

        private static System.Object[] EmptyValues = new System.Object[0];
        private static System.Reflection.TargetException StaticInstanceNotNull = new TargetException("static method target must be null");
        private static System.Reflection.TargetException InstanceIsNull = new TargetException("instance method target must not be null");

        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes)
        {
            return new DynamicMethod(name, returnType, parameterTypes, typeof(UnitTestCodeGen).Module, true);
        }

        #endregion
    }
}
