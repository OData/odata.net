namespace Microsoft.OData.Serializer;

internal interface ICustomAnnotationsHandlerResolver<TCustomState>
{
    ICustomAnnotationsHandler<TCustomState>? Resolve(Type annotationsContainerType);
}
