
namespace Microsoft.OData.Serializer;

public interface ICustomAnnotationsHandler<TCustomState>
{
    // TODO: should we return bool instead and make these resumable as well?
    void WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataWriterState<TCustomState> state);
}
