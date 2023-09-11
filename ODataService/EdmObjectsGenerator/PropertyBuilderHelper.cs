namespace EdmObjectsGenerator
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class PropertyBuilderHelper
    {
        // null keyIndex means the property is not a key.
        // keyIndex of -1 means the key is the only key property.
        // keyIndex > -1 means the key is part of a multi-part key, as indicated by the index
        public static void BuildProperty(TypeBuilder typeBuilder,  string fieldName, Type fieldType, int? keyIndex = null)
        {
            FieldBuilder fieldBldr = typeBuilder.DefineField(fieldName,
                                                            fieldType,
                                                            FieldAttributes.Private);

           
            PropertyBuilder propBuilder = typeBuilder.DefineProperty(fieldName,
                                                             PropertyAttributes.HasDefault,
                                                             fieldType,
                                                             null);
            if (keyIndex != null)
            {
                // Set the [Key] attribute
                ConstructorInfo constructor = typeof(System.ComponentModel.DataAnnotations.KeyAttribute).GetConstructor(Type.EmptyTypes);
                CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(constructor, new object[] { });
                propBuilder.SetCustomAttribute(attributeBuilder);
                if (keyIndex > -1)
                {
                    // Set the [Column(Order={keyIndex})] attribute
                    Type columnAttribute = typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute);
                    constructor = columnAttribute.GetConstructor(Type.EmptyTypes);
                    PropertyInfo[] propertyInfos = columnAttribute.GetProperties().Where(p => p.Name == "Order").ToArray();
                    attributeBuilder = new CustomAttributeBuilder(constructor, new object[] { }, propertyInfos, new object[] { keyIndex });
                    propBuilder.SetCustomAttribute(attributeBuilder);
                }
            }

            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder propMethodBldr =
                typeBuilder.DefineMethod($"get_{fieldName}",
                                           getSetAttr,
                                           fieldType,
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = propMethodBldr.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, fieldBldr);
            custNameGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder propSetMethodBldr =
                typeBuilder.DefineMethod($"set_{fieldName}",
                                           getSetAttr,
                                           null,
                                           new Type[] { fieldType });

            ILGenerator custNameSetIL = propSetMethodBldr.GetILGenerator();

            custNameSetIL.Emit(OpCodes.Ldarg_0);
            custNameSetIL.Emit(OpCodes.Ldarg_1);
            custNameSetIL.Emit(OpCodes.Stfld, fieldBldr);
            custNameSetIL.Emit(OpCodes.Ret);
           
            propBuilder.SetGetMethod(propMethodBldr);
            propBuilder.SetSetMethod(propSetMethodBldr);
        }
    }
}
