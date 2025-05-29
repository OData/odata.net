using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrODataValueWriterProvider : IODataValueWriterProvider
{
    public IODataWriter<T> GetValueWriter<T>(IEdmType edmType, ODataWriterContext context)
    {
        var propertySelector = new ClrTypeEdmPropertySelector<T>();
        return new ODataConventionalJsonResourceWriter<T>(
            context.JsonWriter,
            new ClrTypeEdmPropertySelector<T>(),
            new ClrTypeEdmJsonPropertyWriter<T>());
    }

    public IODataWriter GetValueWriter(IEdmType edmType, Type valueType, ODataWriterContext context)
    {
        var type = this.GetType();
        type.GetMethod(nameof(GetValueWriter), [typeof(IEdmType), typeof(ODataWriterContext)])
            .MakeGenericMethod(valueType);
        var valueWriterType = typeof(ODataConventionalJsonResourceWriter<>).MakeGenericType(valueType);
        var propertySelectorType = typeof(ClrTypeEdmPropertySelector<>).MakeGenericType(valueType);
        var propertyWriterType = typeof(ClrTypeEdmJsonPropertyWriter<>).MakeGenericType(valueType);

        var propertySelector = Activator.CreateInstance(propertySelectorType);
        var propertyWriter = Activator.CreateInstance(propertyWriterType);
        var valueWriter = Activator.CreateInstance(
            valueWriterType, context.JsonWriter, propertySelector, propertyWriter);

        if (valueWriter == null)
        {
            throw new InvalidOperationException($"Failed to create value writer for type '{valueType}' with EDM type '{edmType}'.");
        }

        return valueWriter as IODataWriter;
    }
}
