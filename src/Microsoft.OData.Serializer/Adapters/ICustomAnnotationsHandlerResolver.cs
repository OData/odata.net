namespace Microsoft.OData.Serializer.Adapters;

internal interface ICustomAnnotationsHandlerResolver<TCustomState>
{
    ICustomAnnotationsHandler<TCustomState>? Resolve(Type annotationsContainerType);
}
