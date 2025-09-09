using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class ODataTypeInfoFactory<TCustomState>
{
    public static ODataTypeInfo<T, TCustomState>? CreateTypeInfo<T>(IEdmModel? model, IODataTypeMapper typeMapper)
    {
        var type = typeof(T);

        if (type.IsValueType)
        {
            // TODO: Add support for structs later. Need for boxing, by val,etc.
            return null;
        }

        var typeInfo = new ODataTypeInfo<T, TCustomState>()
        {
            // TODO: we may need to support dynamic EDM type resolution, i.e. typeInfo.GetEdmType func property
            EdmType = GetEdmType(type, model, typeMapper)
        };
        

        var customStateType = typeof(TCustomState);
        var stateType = typeof(ODataWriterState<>).MakeGenericType(customStateType);

        var properties = type.GetProperties();
        var propertyInfos = new List<ODataPropertyInfo<T, TCustomState>>(properties.Length);
        foreach (var property in properties)
        {
            // By default, skip properties not mapped to OData
            if (typeInfo.EdmType is IEdmStructuredType structuredType)
            {
                if (structuredType.FindProperty(property.Name) == null)
                {
                    continue;
                }
            }

            var propertyInfo = CreateODataPropertyInfo(type, property, stateType, customStateType);
            propertyInfos.Add((ODataPropertyInfo<T, TCustomState>)propertyInfo);
        }
        
        typeInfo.Properties = propertyInfos;
        return typeInfo;
    }

    private static ODataPropertyInfo CreateODataPropertyInfo(Type instanceType, PropertyInfo property, Type stateType, Type customStateType)
    {
        var propertyType = property.PropertyType;
        var getterFunc = CreateGetValueDelegate(CreateGetter(instanceType, property, stateType), instanceType, propertyType, stateType);


        // ODataPropertyInfo<TDeclaringType, TProperty, TCustomState>
        var propertyInfoType = typeof(ODataPropertyInfo<,,>).MakeGenericType(instanceType, propertyType, customStateType);
        var propertyInfo = Activator.CreateInstance(propertyInfoType);
        propertyInfoType.GetProperty("Name")!.SetValue(propertyInfo, property.Name);
        propertyInfoType.GetProperty("GetValue")!.SetValue(propertyInfo, getterFunc);

        Debug.Assert(propertyInfo is ODataPropertyInfo, "propertyInfo is ODataPropertyInfo");
        return (ODataPropertyInfo)propertyInfo;
    }

    private static Delegate CreateGetValueDelegate(DynamicMethod dynamicMethod, Type instanceType, Type propertyType, Type stateType)
    {
        // Func<TDeclaringType, ODataWriterState<TCustomState>, TProperty>
        var funcType = typeof(Func<,,>).MakeGenericType(instanceType, stateType, propertyType);
        return dynamicMethod.CreateDelegate(funcType);
    }

    private static Func<TDeclaringType, TCustomState, TProperty> CreateDelegate<TDeclaringType, TProperty>(DynamicMethod dynamicMethod)
    {
        return (Func<TDeclaringType, TCustomState, TProperty>)dynamicMethod.CreateDelegate(typeof(Func<TDeclaringType, TCustomState, TProperty>));
    }

    private static DynamicMethod CreateGetter(Type instanceType, PropertyInfo property, Type stateType)
    {
        // Borrowed from: https://source.dot.net/#System.Text.Json/System/Text/Json/Serialization/Metadata/ReflectionEmitMemberAccessor.cs,95714f469f4e501d
        var getMethod = property.GetGetMethod();
        Debug.Assert(getMethod != null, $"Property {property.Name} does not have a getter.");
        var dynamicMethod = new DynamicMethod(
            property.Name + "Getter",
            property.PropertyType,
            [instanceType, stateType],
            typeof(ODataTypeInfoFactory<TCustomState>).Module,
            skipVisibility: true);


        ILGenerator generator = dynamicMethod.GetILGenerator();

        // TODO: support for value types
        // DO we really need to cast?
       // ((T)this).Property_get()
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Castclass, instanceType);
        generator.Emit(OpCodes.Callvirt, getMethod);

        // TODO handling of nullable structs?
        
        generator.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    private static IEdmType? GetEdmType(Type type, IEdmModel? model, IODataTypeMapper typeMapper)
    {
        if (model == null)
        {
            return null;
        }

        return typeMapper.GetEdmType(type, model);
    }
}
