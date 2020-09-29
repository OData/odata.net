//---------------------------------------------------------------------
// <copyright file="TypeBuilderUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;


    internal static class TypeBuilderUtil
    {
        /// <summary>
        /// Dynamically builds a type from the key/value pairs in the dictionary 
        /// where key is the property name and value is the property type.
        /// </summary>
        /// <param name="typeName">The name of the created type.</param>
        /// <param name="properties">Dictionary containing properties and their respective types.</param>
        /// <returns></returns>
        public static TypeInfo CreateTypeInfo(string typeName, IDictionary<string, Type> properties)
        {
            TypeBuilder typeBuilder = GetTypeBuilder(typeName);

            foreach (KeyValuePair<string, Type> kvPair in properties)
            {
                CreateProperty(typeBuilder, kvPair.Key, kvPair.Value);
            }

            return typeBuilder.CreateTypeInfo();
        }

        /// <summary>
        /// Returns type builder.
        /// </summary>
        /// <param name="typeName">The typename.</param>
        /// <returns></returns>
        private static TypeBuilder GetTypeBuilder(string typeName)
        {
            string assembyName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);

            // Parameterize AssemblyBuilderAccess value if that ever becomes necessary
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(assembyName), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                name: typeName,
                attr: TypeAttributes.Public
                | TypeAttributes.Class
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoLayout,
                parent: null);

            return typeBuilder;
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField(
                "_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getterBuilder = typeBuilder.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);
            ILGenerator getterIlGenerator = getterBuilder.GetILGenerator();

            getterIlGenerator.Emit(OpCodes.Ldarg_0);
            getterIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIlGenerator.Emit(OpCodes.Ret);

            MethodBuilder setterBuilder = typeBuilder.DefineMethod(
                "set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                new[] { propertyType });
            ILGenerator setterIlGenerator = setterBuilder.GetILGenerator();
            Label modifyProperty = setterIlGenerator.DefineLabel();
            Label exitSet = setterIlGenerator.DefineLabel();

            setterIlGenerator.MarkLabel(modifyProperty);
            setterIlGenerator.Emit(OpCodes.Ldarg_0);
            setterIlGenerator.Emit(OpCodes.Ldarg_1);
            setterIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            setterIlGenerator.Emit(OpCodes.Nop);
            setterIlGenerator.MarkLabel(exitSet);
            setterIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);
        }
    }
}
