using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer.Adapters;

// TODO: this interface assumes a 1:1 mapping between CLR type and EDM type. This is not always the case.
// For example, if CLR type is a dictionary or some other dynamic type, we may need more info, like
// the current value or some state to determine the correct EDM type. We can consider
// adding an overload that accepts the value and perhaps state. But we want to
// know when the mapping is 1:1 and when it's not, so that we can cache/optimize
// in the 1:1 case.
/// <summary>
/// Maps CLR types to EDM types.
/// </summary>
public interface IODataTypeMapper
{
    IEdmType? GetEdmType(Type type, IEdmModel model);
}
