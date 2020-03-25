//---------------------------------------------------------------------
// <copyright file="SchemaTermBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Term is built after all types header built.
    /// </summary>
    internal class SchemaOperationBuilder
    {
        private IDictionary<CsdlModel, EdmModel> _modelMapping;
        //private AliasNamespaceHelper _aliasNameMapping;
        //private readonly IDictionary<string, EdmStructuredType> _structuredTypes;
        //private readonly IDictionary<string, EdmEnumType> _enumTypes;
        //private readonly IDictionary<string, EdmTypeDefinition> _typeDefinitions;

        private IList<EdmOperation> _builtOperations = new List<EdmOperation>();

        internal SchemaOperationBuilder(IDictionary<CsdlModel, EdmModel> modelMapping, AliasNamespaceHelper aliasNsMapping,
            IDictionary<string, EdmStructuredType> structuredTypes,
            IDictionary<string, EdmEnumType> enumTypes,
            IDictionary<string, EdmTypeDefinition> typeDefinitions)
        {
            _modelMapping = modelMapping;
            //_aliasNameMapping = aliasNsMapping;
            //_structuredTypes = structuredTypes;
            //_enumTypes = enumTypes;
            //_typeDefinitions = typeDefinitions;
        }


        public void BuildSchemaOperation()
        {
            _builtOperations.Clear();

            foreach (var modelItem in _modelMapping)
            {
                foreach (var csdlSchema in modelItem.Key.Schemata)
                {
                    foreach (var csdlOperation in csdlSchema.Operations)
                    {
                        CsdlAction csdlAction = csdlOperation as CsdlAction;
                        if (csdlAction != null)
                        {
                            BuildSchemaAction(csdlAction, modelItem.Key, csdlSchema, modelItem.Value);
                        }
                        else
                        {
                            BuildSchemaFunction(csdlOperation as CsdlFunction, modelItem.Key, csdlSchema, modelItem.Value);
                        }
                    }
                }
            }

        }

        private static void BuildSchemaAction(CsdlAction csdlAction, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference returnType = BuildReturnType(csdlAction.Return);

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression();

            EdmAction edmAction = new EdmAction(csdlSchema.Namespace, csdlAction.Name, returnType, csdlAction.IsBound, entitySetPathExpression);

            edmModel.AddElement(edmAction);
        }

        private static void BuildSchemaFunction(CsdlFunction csdlFunction, CsdlModel csdlModel, CsdlSchema csdlSchema, EdmModel edmModel)
        {
            IEdmTypeReference returnType = BuildReturnType(csdlFunction.Return);

            IEdmPathExpression entitySetPathExpression = BuildEntitySetPathExpression();

            EdmFunction edmFunction = new EdmFunction(csdlSchema.Namespace, csdlFunction.Name, returnType, csdlFunction.IsBound, entitySetPathExpression, csdlFunction.IsComposable);

            edmModel.AddElement(edmFunction);
        }

        private static IEdmPathExpression BuildEntitySetPathExpression()
        {
            return null;
        }

        private static IEdmTypeReference BuildReturnType(CsdlOperationReturn returnType)
        {
            return null;
        }
    }
}
