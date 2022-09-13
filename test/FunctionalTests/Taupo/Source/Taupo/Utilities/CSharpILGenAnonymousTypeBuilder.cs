//---------------------------------------------------------------------
// <copyright file="CSharpILGenAnonymousTypeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Creates and stores anonymous types.
    /// </summary>
    [ImplementationName(typeof(IAnonymousTypeBuilder), "CSharpILGen", HelpText = "C# anonymous types created using dynamic IL generation")]
    public class CSharpILGenAnonymousTypeBuilder : IAnonymousTypeBuilder
    {
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the CSharpILGenAnonymousTypeBuilder class.
        /// </summary>
        public CSharpILGenAnonymousTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName("AnonymousTypes" + Guid.NewGuid().ToString("N"));

            // We apply the SecurityTransparentAttribute because this dynamic assembly should not do anything
            // that is security-sensitive. Furthermore, emitting a non-transparent dynamic assembly from a
            // transparent method causes a demand for the emitting assembly's (i.e. Microsoft.Test.Taupo.dll)
            // permission set, which is full trust. Applying the attribute avoids this demand. Note that
            // all user code in Silverlight is transparent, so this is not necessary to do in Silverlight.
            var customAttributes = new List<CustomAttributeBuilder>
            {
                new CustomAttributeBuilder(typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes), new object[0])
            };

            // The dynamic assembly inherits the permission set of the AppDomain, so that when anonymous types
            // (which are non-public) in this assembly are created the demand for the AppDomain's
            // permission set will succeed. If the dynamic assembly is fully trusted and the current AppDomain
            // is at medium trust, then demands for the dynamic assembly's permission set (full trust)
            // will fail at the AppDomain boundary.
            this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run,
                customAttributes,
                SecurityContextSource.CurrentAppDomain);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        }

        /// <summary>
        /// Returns an anonymous type that contains members with given names. Returned type is actually a generic type definition.
        /// </summary>
        /// <param name="memberNames">Names of members of the anonymous type.</param>
        /// <returns>Anonymous type containing given members.</returns>
        public Type GetAnonymousType(params string[] memberNames)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberNames, "memberNames");

            Type result;
            string key = string.Join("|", memberNames);
            string hashString = Encoding.UTF8.GetBytes(key).ComputeHash().ToBase16String();
            string typeName = "<>f__AnonymousType" + hashString;

            result = this.assemblyBuilder.GetType(typeName);
            if (result == null)
            {
                result = this.CreateNewAnonymousType(typeName, memberNames);
            }

            return result;
        }

        private Type CreateNewAnonymousType(string typeName, string[] memberNames)
        {
            TypeBuilder typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.NotPublic);

            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                typeof(CompilerGeneratedAttribute).GetInstanceConstructor(true, PlatformHelper.EmptyTypes),
                new object[] { });

            typeBuilder.SetCustomAttribute(attributeBuilder);

            var typeArguments = typeBuilder.DefineGenericParameters(memberNames.Select(n => string.Format(CultureInfo.InvariantCulture, "<{0}>j__TPar", n)).ToArray());
            FieldBuilder[] fields = new FieldBuilder[memberNames.Length];
            for (int i = 0; i < memberNames.Length; i++)
            {
                string fieldName = string.Format(CultureInfo.InvariantCulture, "<{0}>i__Field", memberNames[i]);

                FieldBuilder field = typeBuilder.DefineField(fieldName, typeArguments[i], FieldAttributes.Private | FieldAttributes.InitOnly);
                fields[i] = field;

                this.BuildProperty(typeBuilder, memberNames[i], typeArguments[i], field);
            }

            this.BuildConstructor(typeBuilder, memberNames, typeArguments, fields);
            this.BuildEqualsMethod(typeBuilder);

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Builds property of the anonymous type using IL Generator.
        /// </summary>
        /// <param name="typeBuilder">Type builder for the anonymous type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="field">Backing field.</param>
        private void BuildProperty(TypeBuilder typeBuilder, string propertyName, Type type, FieldBuilder field)
        {
            MethodAttributes getSetAttribute = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, type, null);

            MethodBuilder propertyGetMethod = typeBuilder.DefineMethod("get_" + propertyName, getSetAttribute, type, PlatformHelper.EmptyTypes);
            ILGenerator propertyGetIlGenerator = propertyGetMethod.GetILGenerator();

            // return this.field
            propertyGetIlGenerator.Emit(OpCodes.Ldarg_0);
            propertyGetIlGenerator.Emit(OpCodes.Ldfld, field);
            propertyGetIlGenerator.Emit(OpCodes.Ret);

            MethodBuilder propertySetMethod = typeBuilder.DefineMethod("set_" + propertyName, getSetAttribute, null, new Type[] { type });
            ILGenerator propertySetIlGenerator = propertySetMethod.GetILGenerator();

            // this.field = value
            propertySetIlGenerator.Emit(OpCodes.Ldarg_0);
            propertySetIlGenerator.Emit(OpCodes.Ldarg_1);
            propertySetIlGenerator.Emit(OpCodes.Stfld, field);
            propertySetIlGenerator.Emit(OpCodes.Ret);

            // associalte get and set methods with the property
            property.SetGetMethod(propertyGetMethod);
            property.SetSetMethod(propertySetMethod);
        }

        /// <summary>
        /// Builds constructor for the anonymous type using IL Generator.
        /// </summary>
        /// <param name="typeBuilder">Type builder for the anonymous type.</param>
        /// <param name="parameterNames">Names of parameters for the anonymous type constructor.</param>
        /// <param name="parameterTypes">Types of parameters for the anonymous type constructor.</param>
        /// <param name="fields">Fields of the anonymous type.</param>
        private void BuildConstructor(TypeBuilder typeBuilder, string[] parameterNames, Type[] parameterTypes, FieldBuilder[] fields)
        {
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);

            ILGenerator ilGenerator = ctorBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(object).GetInstanceConstructor(true, PlatformHelper.EmptyTypes));

            for (int i = 0; i < fields.Length; i++)
            {
                ctorBuilder.DefineParameter(i + 1, ParameterAttributes.In, parameterNames[i]);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                ilGenerator.Emit(OpCodes.Stfld, fields[i]);
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Builds Equals method for the anonymous type using IL Generator. Method delegates to a helper that performs the a.Equals(b) operation.
        /// </summary>
        /// <param name="typeBuilder">Type builder for the anonymous type.</param>
        private void BuildEqualsMethod(TypeBuilder typeBuilder)
        {
            var method = typeBuilder.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(bool), new[] { typeof(object) });
            method.SetReturnType(typeof(bool));
            method.InitLocals = true;

            var ilgen = method.GetILGenerator();
            ilgen.DeclareLocal(typeBuilder);
            ilgen.DeclareLocal(typeof(bool));

            // call AnonymousTypeHelpers.EqualsHelper
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Call, ((Func<object, object, bool>)AnonymousTypeHelpers.EqualsHelper).Method);
            ilgen.Emit(OpCodes.Ret);
        }
    }
}
