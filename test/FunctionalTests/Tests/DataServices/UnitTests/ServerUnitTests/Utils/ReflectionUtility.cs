//---------------------------------------------------------------------
// <copyright file="ReflectionUtility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>Provides utility methods for Reflection types.</summary>
    public static class ReflectionUtility
    {
        public static void DefineCreateDataSourceForEdm(TypeBuilder typeBuilder, Type objectContext, string connectionString)
        {
            TestUtil.CheckArgumentNotNull(typeBuilder, "typeBuilder");
            TestUtil.CheckArgumentNotNull(objectContext, "objectContext");
            TestUtil.CheckArgumentNotNull(connectionString, "connectionString");

            MethodBuilder builder = typeBuilder.DefineMethod(
                "CreateDataSource",         // name
                MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                objectContext,              // return type
                System.Type.EmptyTypes);    // parameters
            ConstructorInfo constructor = objectContext.GetConstructors().Where(
                c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(string))
                .Single();

            ILGenerator generator = builder.GetILGenerator();
            generator.Emit(OpCodes.Ldstr, connectionString);
            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>Generates IL that pops the element on the top of the stack and logs it.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="type"><see cref="Type"/> at the top of the stack.</param>
        public static void GenerateDebuggerLogForStackTop(ILGenerator generator, Type type)
        {
            Debug.Assert(generator != null, "generator != null");
            Debug.Assert(type != null, "type != null");

            LocalBuilder theString = generator.DeclareLocal(typeof(string));

            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }

            GenerateObjectToString(generator, "null");
            generator.Emit(OpCodes.Stloc, theString);

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ldstr, "");
            generator.Emit(OpCodes.Ldloc, theString);
            generator.Emit(OpCodes.Call, typeof(Debugger).GetMethod("Log"));
        }

        /// <summary>Generates IL that loads a Type on the stack.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="type"><see cref="Type"/> to load.</param>
        public static void GenerateLoadType(ILGenerator generator, Type type)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(generator, "generator");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(type, "type");
            generator.Emit(OpCodes.Ldtoken, type);
            generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }

        /// <summary>
        /// Generates IL that pops the element on the top of the stack and 
        /// leaves a string in its place.
        /// </summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="nullText">Text to leave if the element is null.</param>
        public static void GenerateObjectToString(ILGenerator generator, string nullText)
        {
            // string theString = (element == null) ? "null" : element.ToString();
            Label nullLabel = generator.DefineLabel();
            Label afterTheStringLabel = generator.DefineLabel();
            generator.Emit(OpCodes.Dup);
            generator.Emit(OpCodes.Brfalse, nullLabel);
            generator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
            generator.Emit(OpCodes.Br, afterTheStringLabel);
            generator.MarkLabel(nullLabel);
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ldstr, nullText);
            generator.MarkLabel(afterTheStringLabel);
        }

        /// <summary>Generates IL that logs a string to the debugger.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="text">Text to log.</param>
        public static void GenerateDebuggerLogForString(ILGenerator generator, string text)
        {
            Debug.Assert(generator != null, "generator != null");
            Debug.Assert(text != null, "text != null");

            // System.Diagnostics.Debugger.Log(0, "", theString);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ldstr, "");
            generator.Emit(OpCodes.Ldstr, text);
            generator.Emit(OpCodes.Call, typeof(Debugger).GetMethod("Log"));
        }

        /// <summary>Generates IL that logs an object array.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="localBuilder"><see cref="LocalBuilder"/> for a local variable of type object[].</param>
        public static void GenerateDebuggerLogForObjectArray(ILGenerator generator, LocalBuilder localBuilder)
        {
            Debug.Assert(generator != null, "generator != null");
            Debug.Assert(localBuilder != null, "localBuilder != null");
            Debug.Assert(localBuilder.LocalType == typeof(object[]), "localBuilder.LocalType == typeof(object[])");

            LocalBuilder index = generator.DeclareLocal(typeof(int));
            LocalBuilder element = generator.DeclareLocal(typeof(object));
            LocalBuilder elementString = generator.DeclareLocal(typeof(string));

            // for (int i = 0; ...)
            Label loopStart = generator.DefineLabel();
            Label loopEnd = generator.DefineLabel();
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Stloc, index);

            // object element = o[i];
            generator.MarkLabel(loopStart);
            generator.Emit(OpCodes.Ldloc, localBuilder);
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(OpCodes.Stloc, element);

            // string theString = (element == null) ? "null" : element.ToString();
            Label nullLabel = generator.DefineLabel();
            Label afterTheStringLabel = generator.DefineLabel();
            generator.Emit(OpCodes.Ldloc, element);
            generator.Emit(OpCodes.Brfalse, nullLabel);
            generator.Emit(OpCodes.Ldloc, element);
            generator.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
            generator.Emit(OpCodes.Br, afterTheStringLabel);
            generator.MarkLabel(nullLabel);
            generator.Emit(OpCodes.Ldstr, "null");
            generator.MarkLabel(afterTheStringLabel);
            generator.Emit(OpCodes.Stloc, elementString);

            // System.Diagnostics.Debugger.Log(0, "", theString);
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ldstr, "");
            generator.Emit(OpCodes.Ldloc, elementString);
            generator.Emit(OpCodes.Call, typeof(Debugger).GetMethod("Log"));

            // for (...; ...; i++)
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Add);
            generator.Emit(OpCodes.Stloc, index);

            // for (...; i < o.Length; ...)
            generator.MarkLabel(loopEnd);
            generator.Emit(OpCodes.Ldloc, index);
            generator.Emit(OpCodes.Ldloc, localBuilder);
            generator.Emit(OpCodes.Ldlen);
            generator.Emit(OpCodes.Conv_I4);
            generator.Emit(OpCodes.Clt);
            generator.Emit(OpCodes.Brtrue, loopStart);
        }

        /// <summary>Writes a local variable to System.Diagnostics.Debug.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="localBuilder"><see cref="LocalBuilder"/> for variable to write.</param>
        /// <param name="localName">Name of variable to write.</param>
        public static void GenerateDebugWriteLocal(ILGenerator generator, LocalBuilder localBuilder, string localName)
        {
            Debug.Assert(generator != null, "generator != null");
            Debug.Assert(localBuilder != null, "localBuilder != null");

            generator.Emit(OpCodes.Ldloc, localBuilder);
            GenerateDebugWriteStackTop(generator, localBuilder.LocalType, localName);
        }

        /// <summary>Writes the element on the top of the stack to System.Diagnostics.Debug.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        /// <param name="elementType"><see cref="Type"/> at the top of the stack.</param>
        /// <param name="name">Name of element to write.</param>
        public static void GenerateDebugWriteStackTop(ILGenerator generator, Type elementType, string name)
        {
            Debug.Assert(generator != null, "generator != null");
            Debug.Assert(elementType != null, "elementType != null");

            if (!String.IsNullOrEmpty(name))
            {
                MethodInfo debugWriteMethod = typeof(Debug).GetMethod("Write", new Type[] { typeof(string) });
                generator.Emit(OpCodes.Ldstr, name + ": ");
                generator.Emit(OpCodes.Call, debugWriteMethod);
            }

            if (elementType.IsValueType)
            {
                generator.Emit(OpCodes.Box, elementType);
            }

            MethodInfo debugWriteLineMethod = typeof(Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
            generator.Emit(OpCodes.Call, debugWriteLineMethod);
        }

        /// <summary>Generates IL to return null from a method.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        public static void GenerateReturnNull(ILGenerator generator)
        {
            Debug.Assert(generator != null, "generator != null");
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>Generates IL to return from a void method.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        public static void GenerateReturnVoid(ILGenerator generator)
        {
            Debug.Assert(generator != null, "generator != null");
            generator.Emit(OpCodes.Ret);
        }

        /// <summary>Generates IL to throw a NotImplementException with no message.</summary>
        /// <param name="generator"><see cref="ILGenerator"/> to write to.</param>
        public static void GenerateThrowNotImplementedException(ILGenerator generator)
        {
            Debug.Assert(generator != null, "generator != null");
            generator.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Throw);
        }

        public static MethodInfo GetMethod(Type type, string name)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(type, "type");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            MethodInfo result = type.GetMethod(name,
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);
            if (result == null)
            {
                throw new InvalidOperationException("Unable to find method '" + name + "' on type '" + type + "'.");
            }
            return result;
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(type, "type");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            PropertyInfo result = type.GetProperty(name,
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);
            if (result == null)
            {
                throw new InvalidOperationException("Unable to find property '" + name + "' on type '" + type + "'.");
            }
            return result;
        }

        /// <summary>Invokes a static method.</summary>
        /// <param name="type">Type with static method.</param>
        /// <param name="name">Name of static method.</param>
        /// <param name="parameters">Parameters for the method.</param>
        /// <returns>The result of the method.</returns>
        public static object InvokeMethod(Type type, string name, params object[] parameters)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(type, "type");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            MethodInfo result = type.GetMethod(name,
                BindingFlags.Static | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);
            return result.Invoke(null, parameters);
        }

        /// <summary>Invokes an instance method.</summary>
        /// <param name="target">The instance to invoke the method on.</param>
        /// <param name="name">The name of the method to invoke.</param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns>The result of the method.</returns>
        public static object InvokeMethod(object target, string name, params object[] parameters)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(target, "target");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            return GetMethod(target.GetType(), name).Invoke(target, parameters);
        }

        /// <summary>Sets a static field value.</summary>
        /// <param name="type">Type with field.</param>
        /// <param name="name">Name of field.</param>
        /// <param name="value">Field value.</param>
        public static void SetField(Type type, string name, object value)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(type, "type");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            FieldInfo result = type.GetField(name,
                BindingFlags.Static | 
                BindingFlags.Public | BindingFlags.NonPublic);
            result.SetValue(null, value);
        }

        /// <summary>Sets an instance field value.</summary>
        /// <param name="target">The object to get the field from.</param>
        /// <param name="name">Name of field.</param>
        /// <returns>Field value.</returns>
        public static object GetFieldValue(object target, string name)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(target, "target");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            FieldInfo result = target.GetType().GetField(name,
                BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);
            return result.GetValue(target);
        }

        /// <summary>Sets an instance property value.</summary>
        /// <param name="target">Type with field.</param>
        /// <param name="name">Name of field.</param>
        /// <param name="value">Field value.</param>
        public static void SetPropertyValue(object target, string name, object value)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(target, "target");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            PropertyInfo result = target.GetType().GetProperty(name,
                BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.NonPublic);
            result.SetValue(target, value, TestUtil.EmptyObjectArray);
        }

        /// <summary>Gets an instance property value.</summary>
        /// <param name="target">The object instance to get the property of.</param>
        /// <param name="name">Name of the property to get.</param>
        /// <returns>The value of the property.</returns>
        public static object GetPropertyValue(object target, string name)
        {
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(target, "target");
            System.Data.Test.Astoria.TestUtil.CheckArgumentNotNull(name, "name");
            return GetProperty(target.GetType(), name).GetValue(target, null);
        }

        /// <summary>Returns the value of a property specified by a source path (separated by slash) from a given object.</summary>
        /// <param name="source">The source object to read the value from.</param>
        /// <param name="path">The path to the property to read.</param>
        /// <param name="createIfNull">If set to true the method will create complex instances along the way to the property if they were null.
        /// So in that case it might modify the source object.</param>
        /// <returns>The value of the property.</returns>
        public static object GetPropertyPathValue(object source, string path, bool createIfNull = false)
        {
            if (string.IsNullOrEmpty(path)) return source;
            return GetPropertyPathValue(source, path.Split('/'), 0, createIfNull);
        }

        private static object GetPropertyPathValue(object source, string[] path, int index, bool createIfNull)
        {
            if (index >= path.Length) return source;
            PropertyInfo pi = source.GetType().GetProperty(path[index]);
            object propertyValue = pi.GetValue(source, null);
            if (propertyValue == null && createIfNull && !UnitTestsUtil.IsPrimitiveType(pi.PropertyType))
            {
                propertyValue = Activator.CreateInstance(pi.PropertyType);
                pi.SetValue(source, propertyValue, null);
            }

            return GetPropertyPathValue(propertyValue, path, index + 1, createIfNull);
        }

        /// <summary>Sets the value of a property specified by a source path (separated by slash) to a given object.</summary>
        /// <param name="source">The source object to set the value to.</param>
        /// <param name="path">The path to the property to read.</param>
        /// <param name="value">The value to set for the property. The method will create comple instances on the path if they were null.</param>
        public static void SetPropertyPathValue(object source, string path, object value)
        {
            if (string.IsNullOrEmpty(path)) return;
            SetPropertyPathValue(source, path.Split('/'), 0, value);
        }

        private static void SetPropertyPathValue(object source, string[] path, int index, object value)
        {
            if (index >= path.Length) return;
            PropertyInfo pi = source.GetType().GetProperty(path[index]);
            if (UnitTestsUtil.IsPrimitiveType(pi.PropertyType))
            {
                pi.SetValue(source, value, null);
            }
            else
            {
                object propertyValue = pi.GetValue(source, null);
                if (propertyValue == null)
                {
                    propertyValue = Activator.CreateInstance(pi.PropertyType);
                    pi.SetValue(source, propertyValue, null);
                }
                SetPropertyPathValue(propertyValue, path, index + 1, value);
            }
        }
    }
}
