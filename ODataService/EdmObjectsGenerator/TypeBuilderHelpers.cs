namespace EdmObjectsGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class TypeBuilderHelper
    {
        /*
        /// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
        /// forwards its arguments to the base constructor, and matches the base constructor's signature.
        /// Supports optional values, and custom attributes on constructors and parameters.
        /// Does not support n-ary (variadic) constructors</summary>
        public static void CreatePassThroughConstructors(this TypeBuilder builder, Type baseType)
        {
            foreach (var constructor in baseType.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
                {
                    //throw new InvalidOperationException("Variadic constructors are not supported");
                    continue;
                }

                var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
                var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

                var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                    if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                    {
                        parameterBuilder.SetConstant(parameter.RawDefaultValue);
                    }

                    foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                    {
                        parameterBuilder.SetCustomAttribute(attribute);
                    }
                }

                foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData()))
                {
                    ctor.SetCustomAttribute(attribute);
                }

                var emitter = ctor.GetILGenerator();
                emitter.Emit(OpCodes.Nop);

                // Load `this` and call base constructor with arguments
                emitter.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parameters.Length; ++i)
                {
                    emitter.Emit(OpCodes.Ldarg, i);
                }
                emitter.Emit(OpCodes.Call, constructor);

                emitter.Emit(OpCodes.Ret);
            }
        }
        */

        public static void CreateDefaultConstructor(this TypeBuilder builder, Type baseType, string paramValue)
        {
            var constructor = baseType.GetConstructor(new Type[] { typeof(string) });
            var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, new Type[] { });

            var emitter = ctor.GetILGenerator();
            emitter.Emit(OpCodes.Nop);

            // Load `this` and call base constructor with arguments
            emitter.Emit(OpCodes.Ldarg_0);
            emitter.Emit(OpCodes.Ldstr, paramValue);
            emitter.Emit(OpCodes.Call, constructor);

            emitter.Emit(OpCodes.Ret);
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute => {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}