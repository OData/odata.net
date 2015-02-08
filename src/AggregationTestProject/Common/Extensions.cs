using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace AggregationTestProject.Common
{
    public static class Extensions
    {
        private static readonly EdmCoreModel _coreModel = EdmCoreModel.Instance;
        public static IEdmTypeReference ToEdmTypeReference(this IEdmType edmType, bool isNullable)
        {
            Contract.Assert(edmType != null);

            switch (edmType.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference(edmType as IEdmCollectionType);
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference(edmType as IEdmComplexType, isNullable);
                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference(edmType as IEdmEntityType, isNullable);
                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference(edmType as IEdmEntityReferenceType, isNullable);
                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference(edmType as IEdmEnumType, isNullable);
                case EdmTypeKind.Primitive:
                    return _coreModel.GetPrimitive((edmType as IEdmPrimitiveType).PrimitiveKind, isNullable);
                default:
                    throw new Exception("EdmTypeNotSupported");
            }
        }
    }
}
