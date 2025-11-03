using Microsoft.OData.Edm;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Microsoft.OData.Serializer;

internal static class ODataTypeInfoFactory<TCustomState>
{
    private static readonly Type CustomStateType = typeof(TCustomState);
    private static readonly Type StateType = typeof(ODataWriterState<>).MakeGenericType(CustomStateType);
    private static readonly Type ValueWriterType = typeof(IValueWriter<>).MakeGenericType(CustomStateType);
    private static readonly Type StreamValueWriterType = typeof(IStreamValueWriter<>).MakeGenericType(CustomStateType);
    private static readonly Type NullableOfTGenericDefinition = typeof(Nullable<>);

    private static Type ValueTaskType { get; } = typeof(ValueTask);
    private static Type ObjectType { get; } = typeof(object);

    private static ConcurrentDictionary<Type, object> CustomValueWriterInstanceCache { get; } = new();

    public static ODataTypeInfo<T, TCustomState>? CreateTypeInfo<T>(IEdmModel? model, IODataTypeMapper typeMapper)
    {
        var type = typeof(T);
        var typeName = type.Name; // for debugging

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

        var properties = type.GetProperties();
        var propertyInfos = new List<ODataPropertyInfo<T, TCustomState>>(properties.Length);
        PropertyInfo? openPropertiesContainerProperty = null;

        var ignoreProperties = type.GetCustomAttribute<ODataIgnorePropertiesAttribute>();
        ODataIgnoreCondition classLevelIgnoreCondition = ODataIgnoreCondition.Never;
        if (ignoreProperties != null)
        {
            classLevelIgnoreCondition = ignoreProperties.Condition;
        }

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<ODataOpenPropertiesAttribute>() != null)
            {
                if (openPropertiesContainerProperty != null)
                {
                    throw new Exception($"Found duplicate open property container on property {property.Name} of type {type.FullName}");
                }

                openPropertiesContainerProperty = property;
                continue;
            }

            if (!ShouldIncludeProperty(property, typeInfo, out IEdmProperty? edmProperty, out ODataIgnoreCondition? propertyIgnoreCondition))
            {
                continue;
            }

            var effectiveIgnoreCondition = GetEffectiveIgnoreCondition(classLevelIgnoreCondition, propertyIgnoreCondition);
            if (effectiveIgnoreCondition == ODataIgnoreCondition.Always)
            {
                continue;
            }

            var propertyInfo = CreateODataPropertyInfo(type, property, edmProperty, effectiveIgnoreCondition);
            propertyInfos.Add((ODataPropertyInfo<T, TCustomState>)propertyInfo);
        }
        
        typeInfo.Properties = propertyInfos;

        if (openPropertiesContainerProperty != null)
        {
            typeInfo.GetOpenProperties = (Func<T, ODataWriterState<TCustomState>, object?>)CreateGetValueDelegate(
                CreateGetter(type, openPropertiesContainerProperty),
                type,
                openPropertiesContainerProperty.PropertyType);
        }

        return typeInfo;
    }

    private static bool ShouldIncludeProperty<TResource>(
        PropertyInfo property,
        ODataTypeInfo<TResource, TCustomState> typeInfo,
        [NotNullWhen(true)] out IEdmProperty? edmProperty,
        out ODataIgnoreCondition? ignoreCondition)
    {
        edmProperty = null;
        ignoreCondition = null;

        // Checking attributes individually instead of using GetCustomAttributes()
        // via cause we're betting that ODataIgnoreAttribute will be the most common.
        var odataIgnore = property.GetCustomAttribute<ODataIgnoreAttribute>();
        if (odataIgnore != null && odataIgnore.Condition == ODataIgnoreCondition.Always)
        {
            ignoreCondition = ODataIgnoreCondition.Always;
            return false;
        }

        // Support for [IgnoreDataMember] due to its common usage in data contracts
        // TODO: should this be opt-in?
        if (property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
        {
            return false;
        }

        if (odataIgnore != null)
        {
            ignoreCondition = odataIgnore.Condition;
        }

        var propertyNameAttribute = property.GetCustomAttribute<ODataPropertyNameAttribute>();
        var propertyName = propertyNameAttribute != null ? propertyNameAttribute.Name : property.Name;

        if (typeInfo.EdmType is IEdmStructuredType structuredType)
        {
            edmProperty = structuredType.FindProperty(propertyName);
            // TODO: if property name was specified via [ODataPropertyName] and it doesn't exist
            // on the EDM type, we should throw.
            if (edmProperty == null)
            {
                return false;
            }

            return true;
        }


        return false;
    }

    private static ODataPropertyInfo CreateODataPropertyInfo(
        Type instanceType,
        PropertyInfo clrProperty,
        IEdmProperty edmProperty,
        ODataIgnoreCondition ignoreCondition)
    {
        var clrPropertyType = clrProperty.PropertyType;

        // ODataPropertyInfo<TDeclaringType, TProperty, TCustomState>
        var odataPropertyInfoType = typeof(ODataPropertyInfo<,,>).MakeGenericType(instanceType, clrPropertyType, CustomStateType);
        var odataPropertyInfo = Activator.CreateInstance(odataPropertyInfoType);
        Debug.Assert(odataPropertyInfo != null);

        odataPropertyInfoType.GetProperty(nameof(ODataPropertyInfo.Name))!.SetValue(odataPropertyInfo, edmProperty.Name);

        SetODataPropertyInfoValueHandler(odataPropertyInfo, odataPropertyInfoType, clrProperty, instanceType, ignoreCondition);

        Debug.Assert(odataPropertyInfo is ODataPropertyInfo, "propertyInfo is ODataPropertyInfo");
        return (ODataPropertyInfo)odataPropertyInfo;
    }

    private static void SetODataPropertyInfoValueHandler(
        object odataPropertyInfo,
        Type odataPropertyInfoType,
        PropertyInfo clrProperty,
        Type instanceType,
        ODataIgnoreCondition ignoreCondition)
    {
        var valueWriterAttribute = clrProperty.GetCustomAttribute<ODataValueWriterAttribute>();
        if (valueWriterAttribute?.WriterType != null)
        {
            var baseWriterTypeWithValue = typeof(ODataAsyncPropertyWriter<,,>).MakeGenericType(instanceType, clrProperty.PropertyType, CustomStateType);
            if (valueWriterAttribute.WriterType.IsAssignableTo(baseWriterTypeWithValue))
            {
                var writeValueAsyncDelegate = CreateWriteValueAsyncFromCustomWriterWithValueDelegate(
                    instanceType,
                    clrProperty,
                    valueWriterAttribute.WriterType);

                var writeValueWithCustomWriterAsyncProp = odataPropertyInfoType.GetProperty(
                    nameof(ODataPropertyInfo<bool, bool>.WriteValueWithCustomWriterAsync),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(writeValueWithCustomWriterAsyncProp != null, "WriteValueWithCustomWriterAsync property not found");

                writeValueWithCustomWriterAsyncProp.SetValue(odataPropertyInfo, writeValueAsyncDelegate);

                var customValueWriterProp = odataPropertyInfoType.GetProperty(
                    nameof(ODataPropertyInfo<bool, bool>.CustomPropertyValueWriter),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(customValueWriterProp != null, "CustomPropertyValueWriter property not found");

                // The custom writer type should be stateless. This expectation should be well documented.
                // Since it's stateless we can create one instance per type instead of per property it's used on.
                if (!CustomValueWriterInstanceCache.TryGetValue(valueWriterAttribute.WriterType, out object? customWriterInstance))
                {
                    // TODO: validate that custom writer type has parameterless constructor.
                    // that's somethign we require.
                    customWriterInstance = Activator.CreateInstance(valueWriterAttribute.WriterType);
                    Debug.Assert(customWriterInstance != null);
                    CustomValueWriterInstanceCache[valueWriterAttribute.WriterType] = customWriterInstance;
                }

                // Store the writer instance on the property info
                customValueWriterProp.SetValue(odataPropertyInfo, customWriterInstance);
            }
            else
            {
                throw new InvalidOperationException(
                    $"The WriterType {valueWriterAttribute.WriterType.FullName} specified in ODataPropertyValueWriterAttribute on property {clrProperty.Name} does not inherit from {baseWriterTypeWithValue.FullName}");
            }

            return;
        }
        // TODO: potentially handle writer type specified on the declaring type instead.

        Debug.Assert(ignoreCondition != ODataIgnoreCondition.Always, "Ignore condition should not be Always at this point.");

        if (ignoreCondition == ODataIgnoreCondition.WhenWritingNull && IsReferenceTypeOrNullableValueType(clrProperty.PropertyType))
        {
            var writeValueDelegate = CreatePropertyValueWriterThatIgnoresWhenNullDelegate(
                CreatePropertyValueWriterThatIgnoresWhenNull(instanceType, clrProperty),
                instanceType,
                clrProperty.PropertyType);
            odataPropertyInfoType.GetProperty(nameof(ODataPropertyInfo<bool,bool,bool>.WriteValue))!.SetValue(odataPropertyInfo, writeValueDelegate);
            return;
        }

        // No custom writer set, so we generate basic GetValue delegate
        var getValueDelegate = CreateGetValueDelegate(CreateGetter(instanceType, clrProperty), instanceType, clrProperty.PropertyType);
        // TODO: we use bool as placeholder since we can't leave the type param blank until C# 14.
        odataPropertyInfoType.GetProperty(nameof(ODataPropertyInfo<bool, bool, bool>.GetValue))!.SetValue(odataPropertyInfo, getValueDelegate);
    }

    private static Delegate CreateGetValueDelegate(DynamicMethod dynamicMethod, Type instanceType, Type propertyType)
    {
        // Func<TDeclaringType, ODataWriterState<TCustomState>, TProperty>
        var funcType = typeof(Func<,,>).MakeGenericType(instanceType, StateType, propertyType);
        return dynamicMethod.CreateDelegate(funcType);
    }

    private static DynamicMethod CreateGetter(Type instanceType, PropertyInfo property)
    {
        // Borrowed from: https://source.dot.net/#System.Text.Json/System/Text/Json/Serialization/Metadata/ReflectionEmitMemberAccessor.cs,95714f469f4e501d
        var getMethod = property.GetGetMethod();
        Debug.Assert(getMethod != null, $"Property {property.Name} does not have a getter.");
        var dynamicMethod = new DynamicMethod(
            property.Name + "Getter",
            property.PropertyType,
            [instanceType, StateType],
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

    private static Delegate CreatePropertyValueWriterThatIgnoresWhenNullDelegate(
        DynamicMethod dynamicMethod,
        Type instanceType,
        Type propertyType)
    {
        // Func<TDeclaringType, IValueWriter<TCustomState>, ODataWriterState<TCustomState>, bool>
        var funcType = typeof(Func<,,,>).MakeGenericType(instanceType, ValueWriterType, StateType, typeof(bool));
        return dynamicMethod.CreateDelegate(funcType);
    }

    private static DynamicMethod CreatePropertyValueWriterThatIgnoresWhenNull(
        Type instanceType,
        PropertyInfo property)
    {
        // bool PropertyValueWriter(InstanceType resource, IValueWriter<TCustomState> writer, ODataWriterState<TCustomState> state)
        var dynamicMethod = new DynamicMethod(
            $"{property.Name}ValueWriter",
            typeof(bool),
            [instanceType, ValueWriterType, StateType],
            typeof(ODataTypeInfoFactory<TCustomState>).Module,
            skipVisibility: true);


        // We should validate that getter is not null before calling this method.`
        var getMethod = property.GetGetMethod();
        Debug.Assert(getMethod != null, $"Property {property.Name} does not have a getter.");

        bool isNullableValueType = property.PropertyType.IsValueType
            && property.PropertyType.IsGenericType
            && property.PropertyType.GetGenericTypeDefinition() == NullableOfTGenericDefinition;

        Debug.Assert(isNullableValueType || !property.PropertyType.IsValueType, "Property type should either be Nullable<T> or a reference type.");

        MethodInfo writeValueMethod;
        if (!isNullableValueType)
        {
            writeValueMethod = ValueWriterType.GetMethod(nameof(IValueWriter<bool>.WriteValue))!.MakeGenericMethod(property.PropertyType);
        }
        else
        {
            // For Nullable<T>, we will use T as the type parameter for WriteValue<T>
            // If the serializer already has an ODataWriter<T> or ODataTypeInfo<T>,
            // then that will be used to write the value. Otherwise, the serializer would
            // try to generate a writer for Nullable<T> and perform null checks again, which
            // is an unnecessary cost since we're skipping the null-case.
            var valueType = property.PropertyType.GetGenericArguments()[0];
            Debug.Assert(valueType.IsValueType);
            writeValueMethod = ValueWriterType.GetMethod(nameof(IValueWriter<bool>.WriteValue))!.MakeGenericMethod(valueType);
        }

        ILGenerator il = dynamicMethod.GetILGenerator();

        /*
         * bool WritePropertyValue(InstanceType resource, IValueWriter<TCustomState> writer, ODataWriterState<TCustomState> state)
         * {
         *     PropertType value = resource.Property;
         *     if (value == null)
         *     {
         *        return true;
         *     }
         *
         *     return writer.WriteValue<TProperty>(value, state);
         * }
         */

        var value = il.DeclareLocal(property.PropertyType); // local variable to store property value

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Callvirt, getMethod); // load property value
        il.Emit(OpCodes.Stloc_S, value); // store property value in local variable

        if (isNullableValueType)
        {
            // for Nullable<T>, we need to check HasValue
            il.Emit(OpCodes.Ldloca_S, (byte)0); // load address of local variable.
                                                // Since this is a value type, we have to load the address explicitly before calling an instance method
            var hasValueMethod = property.PropertyType.GetProperty(nameof(Nullable<bool>.HasValue))!.GetGetMethod()!;
            il.Emit(OpCodes.Call, hasValueMethod); // call get_HasValue
        }
        else
        {
            il.Emit(OpCodes.Ldloc_S, value); // load property value
        }

        Label notNullLabel = il.DefineLabel();
        il.Emit(OpCodes.Brtrue_S, notNullLabel); // if value != null goto notNullLabel

        // property value is null
        il.Emit(OpCodes.Ldc_I4_1); // return true
        il.Emit(OpCodes.Ret);

        // property value is not null
        il.MarkLabel(notNullLabel);
        il.Emit(OpCodes.Ldarg_1); // load writer

        if (!isNullableValueType)
        {
            il.Emit(OpCodes.Ldloc_S, value); // load property value
        }
        else
        {
            // for Nullable<T>, we want to unwrap the Nullable the underlying T value and write it directly,
            // i.e. WriteValue<T>(resource.Property.Value)
            il.Emit(OpCodes.Ldloca_S, value); // load property value
            var valueGetter = property.PropertyType.GetProperty(nameof(Nullable<bool>.Value))!.GetGetMethod()!;
            il.Emit(OpCodes.Call, valueGetter);
        }

        il.Emit(OpCodes.Ldarg_2); // load state
        il.Emit(OpCodes.Callvirt, writeValueMethod); // call writer.WriteValue<TProperty>(value, state)
        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    // return Func<TDeclaringType, object, IStreamValueWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask>
    private static Delegate CreateWriteValueAsyncFromCustomWriterWithValueDelegate(
        Type instanceType,
        PropertyInfo property,
        Type writerType)
    {
        var dynamicMethod = CreateWriteValueAsyncFromCustomWriterWithValue(instanceType, property, writerType);
        // Func<TDeclaringType, object, IStreamValueWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask>
        var actionType = typeof(Func<,,,,>).MakeGenericType(
            instanceType,
            ObjectType,
            StreamValueWriterType,
            StateType,
            ValueTaskType);
        return dynamicMethod.CreateDelegate(actionType);
    }

    /// <summary>
    /// Creates an ODataPropertyInfo.WriteValueWithCustomWriterAsync delegate
    /// based on
    /// <see cref="ODataAsyncPropertyWriter{TResource, TValue, TCustomState}.WriteValueAsync(TResource, TValue, IStreamValueWriter{TCustomState}, ODataWriterState{TCustomState})"/>"/>
    /// </summary>
    /// <param name="instanceType"></param>
    /// <param name="property"></param>
    /// <param name="writerType">Must be <see cref="ODataAsyncPropertyWriter{TDeclaringType, TValue, TCustomState}"/></param>
    /// <returns></returns>
    private static DynamicMethod CreateWriteValueAsyncFromCustomWriterWithValue(
        Type instanceType,
        PropertyInfo property,
        Type writerType)
    {
        // ODataAyncPropertyValueWriter<TResource, TValue, TCustomState>
        Debug.Assert(writerType.IsAssignableTo(typeof(ODataAsyncPropertyWriter<,,>).MakeGenericType(instanceType, property.PropertyType, typeof(TCustomState))));
        var getMethod = property.GetGetMethod();
        Debug.Assert(getMethod != null, $"Property {property.Name} does not have a getter.");

        // TODO: using bool as type params since I can't leave them blank, I only need
        // the method name here. In C# 14 we'll be able to do
        // nameof(ODataAsyncPropertyWriter<,,>.WriteValueAsync)
        var writeAsyncMethod = writerType.GetMethod(nameof(ODataAsyncPropertyWriter<bool, bool, bool>.WriteValueAsync));
        Debug.Assert(writeAsyncMethod != null);

        // we need a way to access an instance of the writer within the method

        // How do we pass an instance of customValueWriter?
        // Easiet way is to pass it as an argument, but that would change the signature of the
        // WriteValueAsync delegate.
        // We could add another delegate that accepts the writer instance and make it internal.
        // Another option is to inject the writer instance in the state, and retrieve it
        // from there. But that will make the state larger. I think we can go with

        // ODataPropertyInfo<T>.WriteValueWithCustomeWriterAsync =
        // (TResource resource, object customValueWriter, IStreamValueWriter<TCustomState> writer, ODataWriterState<TCustomState> state) =>
        // {
        //    PropertyType value = resource.Property;
        //    return ((WriterType)customValueWriter.WriteValueAsync(resource, value, writer, state);
        // }


        var dynamicMethod = new DynamicMethod(
            $"Write{property.Name}WithCustomWriterAsync",
            ValueTaskType,
            [instanceType, ObjectType, StreamValueWriterType, StateType],
            typeof(ODataTypeInfoFactory<TCustomState>).Module,
            skipVisibility: true);


        ILGenerator il = dynamicMethod.GetILGenerator();

        // let's say the resource is person, and the property is Name
        // and the writer type is ODataAsyncPropertyWriter<Person, string, CustomState>
        // then the signature of the method we want to generate is
        // WriteValueWithCustomWriterAsync(Person person, object customWriter, IStreamValueWriter<CustomState> writer, ODataWriterState<TCustomState> state)
        // 
        // and this should be the body of the delegate:
        // return ((ODataAsyncPropertyWriter<Person, string, CustomState>)customValueWriter)
        //            .WriteValueAsync(person, person.Name, writer, state);

        il.DeclareLocal(property.PropertyType);
        

        // 1: Load the writer instance to prepare for the method cal
        il.Emit(OpCodes.Ldarg_1); // load customValueWriter
        // 1.a.cast since the arg type is object
        // TODO: we pass object since there different type of writers without
        // a common interface passed to the same delegate signature.
        // This is for simplicity of initial implementation. If cast has noticeable cost,
        // we can create different delegates for different writer types.
        il.Emit(OpCodes.Castclass, writerType); 

        // 2. load arguments for the method call
        il.Emit(OpCodes.Ldarg_0); // first arg is the resource

        // 2.a second arg is the value, we get the value by calling the property getter
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Callvirt, getMethod);

        // 2.a 3rd arg is the IStreamValueWriter
        il.Emit(OpCodes.Ldarg_2);

        // 3rd arg is the state
        il.Emit(OpCodes.Ldarg_3);

        // 4. call the method
        il.Emit(OpCodes.Callvirt, writeAsyncMethod);
        il.Emit(OpCodes.Ret);

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

    private static bool IsReferenceTypeOrNullableValueType(Type type) =>
        !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == NullableOfTGenericDefinition);

    /// <summary>
    /// Resolves the effective ignore condition for a property
    /// based on parent (class-level) and child (property-level) ignore conditions
    /// following precedence rules.
    /// </summary>
    /// <param name="parentCondition">Ignore condition from the parent-level configuration.</param>
    /// <param name="childCondition">Ignore condition from the child-level configuration.</param>
    /// <returns></returns>
    private static ODataIgnoreCondition GetEffectiveIgnoreCondition(
        ODataIgnoreCondition? parentCondition,
        ODataIgnoreCondition? childCondition)
    {
        if (parentCondition == null)
        {
            return childCondition ?? ODataIgnoreCondition.Never;
        }

        if (childCondition == null)
        {
            return parentCondition ?? ODataIgnoreCondition.Never;
        }

        // If child condition is conditional (e.g. WhenWritingNull) and parent is Always,
        // Then effective condition is Always since "ignoring the property all the time"
        // is consistent with "ignoring the property sometimes".
        if (parentCondition == ODataIgnoreCondition.Always)
        {
            if (childCondition == ODataIgnoreCondition.WhenWritingNull)
            {
                return ODataIgnoreCondition.Always;
            }
        }

        // Otherwise, child condition takes precedence.
        return childCondition.Value;
    }
}
