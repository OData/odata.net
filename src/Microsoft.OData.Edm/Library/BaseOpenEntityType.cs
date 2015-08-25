using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Edm.Library
{
    public sealed class BaseOpenEntityType : IEdmEntityType
    {
        public IEdmStructuredType BaseType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IEdmProperty> DeclaredProperties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasStream
        {
            get
            {
                return false;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public bool IsOpen
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return null;// return "AggregationWrapper";
            }
        }

        public string Namespace
        {
            get
            {
                return null;// return string.Empty;
            }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EdmTermKind TermKind
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public EdmTypeKind TypeKind
        {
            get
            {
                return EdmTypeKind.Entity;
            }
        }

        public IEdmProperty FindProperty(string name)
        {
            return new EdmStructuralProperty(this, name, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Int32, true));
        }
    }

    public static class Extension
    {
        public static IEdmEntityType GetDynamicEntityType()
        {
            return new BaseOpenEntityType();
        }
    }

}
