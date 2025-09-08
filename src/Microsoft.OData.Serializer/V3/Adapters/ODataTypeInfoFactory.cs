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
    public static ODataTypeInfo<T, TCustomState> CreateTypeInfo<T>()
    {
        var typeInfo = new ODataTypeInfo<T, TCustomState>();

        var type = typeof(T);
        var properties = type.GetProperties();
        var propertyInfos = new List<ODataPropertyInfo<T, TCustomState>>(properties.Length);
        foreach (var property in properties)
        {
            var propertyInfo = CreateODataPropertyInfo(type, property);
            propertyInfos.Add((ODataPropertyInfo<T, TCustomState>)propertyInfo);
        }
        
        typeInfo.Properties = propertyInfos;
        return typeInfo;
    }
    public static ODataTypeInfo CreateTypeInfo(Type type)
    {
        var typeInfoType = typeof(ODataTypeInfo<,>).MakeGenericType(type, typeof(TCustomState));
        Debug.Assert(typeInfoType != null);

        var typeInfo = Activator.CreateInstance(typeInfoType);
        Debug.Assert(typeInfo != null);

        var propertiesProp = typeInfoType.GetProperty("Properties")!;
        var propertyInfos = (IList<ODataPropertyInfo>)propertiesProp.GetValue(Activator.CreateInstance(typeInfoType))!;

        foreach (var prop in type.GetProperties())
        {
            var propertyInfo = CreateODataPropertyInfo(type, prop);
            propertyInfos.Add(propertyInfo);
        }

        return (ODataTypeInfo)typeInfo;
    }

    private static ODataPropertyInfo CreateODataPropertyInfo(Type instanceType, PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        var getterFunc = CreateGetter(instanceType, property);


        var propertyInfoType = typeof(ODataPropertyInfo<,,>).MakeGenericType(instanceType, propertyType, typeof(TCustomState));
        var propertyInfo = Activator.CreateInstance(propertyInfoType);
        propertyInfoType.GetProperty("Name")!.SetValue(propertyInfo, property.Name);
        propertyInfoType.GetProperty("GetValue")!.SetValue(propertyInfo, getterFunc);

        Debug.Assert(propertyInfo is ODataPropertyInfo, "propertyInfo is ODataPropertyInfo");
        return (ODataPropertyInfo)propertyInfo;
    }

    private static Delegate CreateDelegate<TDeclaringType, TProperty>(DynamicMethod dynamicMethod, Type instanceType, Type propertyType)
    {
        var funcType = typeof(Func<,,>).MakeGenericType(instanceType, typeof(TCustomState), propertyType);
        return dynamicMethod.CreateDelegate(funcType);
    }

    private static Func<TDeclaringType, TCustomState, TProperty> CreateDelegate<TDeclaringType, TProperty>(DynamicMethod dynamicMethod)
    {
        return (Func<TDeclaringType, TCustomState, TProperty>)dynamicMethod.CreateDelegate(typeof(Func<TDeclaringType, TCustomState, TProperty>));
    }

    private static DynamicMethod CreateGetter(Type instanceType, PropertyInfo property)
    {
        // Borrowed from: https://source.dot.net/#System.Text.Json/System/Text/Json/Serialization/Metadata/ReflectionEmitMemberAccessor.cs,95714f469f4e501d
        var getMethod = property.GetGetMethod();
        Debug.Assert(getMethod != null, $"Property {property.Name} does not have a getter.");
        var dynamicMethod = new DynamicMethod(
            property.Name + "Getter",
            property.PropertyType,
            [instanceType],
            typeof(ODataTypeInfoFactory<TCustomState>).Module,
            skipVisibility: true);


        ILGenerator generator = dynamicMethod.GetILGenerator();

        // TODO: support for value types
        // DO we need to cast?
        generator.Emit(OpCodes.Castclass, instanceType);
        generator.Emit(OpCodes.Callvirt, getMethod);

        // TODO handling of nullable structs?
        
        generator.Emit(OpCodes.Ret);

        return dynamicMethod;
    }
}
