using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Dummy class that allows virtual property $count 
    /// to work like any other aggregation method.
    /// </summary>
    public sealed class CountVirtualPropertyNode : SingleValueNode
    {
        public CountVirtualPropertyNode() { }

        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.None;
            }
        }

        public override IEdmTypeReference TypeReference {
            get {
                return EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
            }
        }
    }
}
