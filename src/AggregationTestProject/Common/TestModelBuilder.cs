using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core.Aggregation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;


namespace AggregationTestProject.Common
{
    public class TestModelBuilder
    {
        /// <summary>
        /// Create a model
        /// </summary>
        /// <param name="entityTyps"></param>
        /// <returns></returns>
        public static EdmModel CreateModel(Type[] entityTyps)
        {
            var model = new EdmModel();


            foreach (var clrType in entityTyps)
            {
                EdmStructuredType edmType;
                if (clrType == entityTyps.Last())
                    edmType = new EdmEntityType(clrType.Namespace, clrType.Name);
                else
                {
                    edmType = new EdmComplexType(clrType.Namespace, clrType.Name);
                }
                foreach (var pi in clrType.GetProperties())
                {
                    
                    if (pi.PropertyType.IsPrimitive || pi.PropertyType.FullName == "System.String" || pi.PropertyType.IsEnum)
                    {
                        edmType.AddStructuralProperty(
                            pi.Name,
                            GetPrimitiveTypeKind(pi.PropertyType),
                            true);
                    }
                    else
                    {
                        var propEdmType = model.FindDeclaredType(pi.PropertyType.FullName);
                        if (propEdmType != null)
                        {
                            edmType.AddStructuralProperty(
                                pi.Name,
                                propEdmType.ToEdmTypeReference(true));
                        }
                    }
                }
                model.AddElement(edmType as IEdmSchemaElement);
            }
            
            return model;
        }


        private static EdmPrimitiveTypeKind GetPrimitiveTypeKind(Type t)
        {
            Contract.Assert(t != null);
            switch (t.Name)
            {
                case "Boolean": return EdmPrimitiveTypeKind.Boolean;
                case "Byte": return EdmPrimitiveTypeKind.Byte;
                case "DateTime": return EdmPrimitiveTypeKind.DateTimeOffset;
                case "DateTimeOffset": return EdmPrimitiveTypeKind.DateTimeOffset;
                case "Decimal": return EdmPrimitiveTypeKind.Decimal;
                case "Double": return EdmPrimitiveTypeKind.Double;
                case "Int16": return EdmPrimitiveTypeKind.Int16;
                case "Int32": return EdmPrimitiveTypeKind.Int32;
                case "Int64": return EdmPrimitiveTypeKind.Int64;
                case "SByte": return EdmPrimitiveTypeKind.SByte;
                case "String": return EdmPrimitiveTypeKind.String;
                case "Single": return EdmPrimitiveTypeKind.Single;
                case "Guid": return EdmPrimitiveTypeKind.Guid;
                case "Duration": return EdmPrimitiveTypeKind.Duration;
            }

            if (t.IsEnum)
            {
                return EdmPrimitiveTypeKind.Int32;
            }

            throw new InvalidOperationException("unsupported type");
        }
    }
}
