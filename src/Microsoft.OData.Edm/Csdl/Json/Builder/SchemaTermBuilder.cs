//---------------------------------------------------------------------
// <copyright file="SchemaTermBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
#if false
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Term is built after all types header built.
    /// </summary>
    internal class SchemaTermBuilder
    {
        private IDictionary<CsdlJsonSchemaOperationItem, CsdlJsonModel> _termItemsToModelMapping;
        private IDictionary<string, IEdmSchemaType> _allSchemaTypes;

        internal SchemaTermBuilder(IDictionary<CsdlJsonSchemaOperationItem, CsdlJsonModel> termItemsToModelMapping,
            IDictionary<string, IEdmSchemaType> schemaTypes)
        {
            _termItemsToModelMapping = termItemsToModelMapping;
            _allSchemaTypes = schemaTypes;
        }

        internal string ReplaceAlias(CsdlJsonSchemaOperationItem operationItem, string name)
        {
            CsdlJsonModel declaredModel = _termItemsToModelMapping[termItem];
            return declaredModel.ReplaceAlias(name);
        }

        public static IDictionary<CsdlJsonSchemaOperationItem, EdmOperation> BuildSchemaOperations(
            IDictionary<CsdlJsonSchemaOperationItem, CsdlJsonModel> termItemsToModelMapping,
            IDictionary<string, IEdmSchemaType> allSchemaTypes)
        {
            SchemaOperationBuilder builder = new SchemaOperationBuilder(termItemsToModelMapping, allSchemaTypes);
            return builder.BuildSchemaOperations();
        }

        private IDictionary<CsdlJsonSchemaOperationItem, EdmOperation> BuildSchemaOperations()
        {
            IDictionary<CsdlJsonSchemaTermItem, IEdmTerm> terms = new Dictionary<CsdlJsonSchemaTermItem, IEdmTerm>();

            // Build the term after building the types
            foreach (var termJsonItem in _termItemsToModelMapping)
            {
                terms[termJsonItem.Key] = BuildTermType(termJsonItem.Key, termJsonItem.Value);
            }

            return terms;
        }

        private IEdmTerm BuildOperation(CsdlJsonSchemaTermItem operationItem, CsdlJsonModel declaredModel)
        {
            string typeName = declaredModel.ReplaceAlias(termItem.QualifiedTypeName);



            IEdmTypeReference termType = EdmTypeHelper.BuildEdmTypeReference(typeName,
                termItem.IsCollection,
                termItem.Nulable,
                false,
                termItem.MaxLength,
                termItem.Unicode,
                termItem.Precision,
                termItem.Scale,
                termItem.Srid,
                _allSchemaTypes);

            string appliesTo = null;
            if (termItem.AppliesTo != null)
            {
                appliesTo = string.Join(" ", termItem.AppliesTo);
            }

            return new EdmTerm(termItem.Namespace, termItem.Name, termType, appliesTo, termItem.DefaultValue);
        }
    }
#endif
}
