//---------------------------------------------------------------------
// <copyright file="OperationTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;

    public static class OperationTestModelBuilder
    {
        private const string DefaultNamespaceName = "DefaultNamespace";

        public static IEdmModel OperationStandAloneSchemasEdm()
        {
            var model = new EdmModel();

            foreach (var edmFunction in OperationTestModelBuilder.EdmFunctions())
            {
                model.AddElement(edmFunction);
            }

            return model;
        }

        public static IEdmModel OperationsWithReturnTypeOfPrimitiveDataTypeSchemasEdm(EdmVersion edmVersion)
        {
            var model = new EdmModel();
            foreach (var function in OperationTestModelBuilder.EdmFunctionsWithReturnTypePrimitiveDataType(edmVersion))
            {
                model.AddElement(function);
            }

            return model;
        }

        public static IEdmModel OperationsWith2ParametersSchemasEdm(EdmVersion edmVersion)
        {
            var model = new EdmModel();
            foreach (var operation in OperationTestModelBuilder.EdmOperationsWith2Parameters(edmVersion))
            {
                model.AddElement(operation);
            }

            return model;
        }

        public static IEdmModel OperationsWithNamedStructuralDataTypeSchemasEdm()
        {
            var model = ModelBuilder.TaupoDefaultModelEdm() as EdmModel;
            foreach (var operation in OperationTestModelBuilder.EdmOperationsWithNamedStructuralDataType(model))
            {
                model.AddElement(operation);
            }

            return model;
        }

        public static void AddOperationImports(EdmModel model)
        {
            var container = model.EntityContainer as EdmEntityContainer;
            if (container == null)
            {
                container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
                model.AddElement(container);
            }

            foreach (var function in model.SchemaElements.OfType<IEdmFunction>())
            {
                container.AddFunctionImport(function);
            }

            foreach (var action in model.SchemaElements.OfType<IEdmAction>())
            {
                container.AddActionImport(action);
            }
        }

        private static IList<IEdmPrimitiveTypeReference> MaxLengthEdmPrimitiveTypes()
        {
            return new List<IEdmPrimitiveTypeReference> 
            { 
                EdmCoreModel.Instance.GetBinary(isUnbounded:false, maxLength:int.MaxValue, isNullable:true),
                EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:0, isUnicode:null, isNullable:true),
            };
        }

        private static IList<IEdmPrimitiveTypeReference> PrecisionScaleFacetedEdmTypes()
        {
            return new List<IEdmPrimitiveTypeReference>
            {
                EdmCoreModel.Instance.GetDuration(false),
                EdmCoreModel.Instance.GetDecimal(precision:int.MaxValue, scale:1, isNullable:false),
                EdmCoreModel.Instance.GetDateTimeOffset(false),
            };
        }

        private static IList<IEdmFunction> EdmFunctions()
        {
            return new List<IEdmFunction>
            {
                new EdmFunction(DefaultNamespaceName, new string('a', 256), EdmCoreModel.Instance.GetInt16(false)),
            };
        }

        private static IList<IEdmFunction> EdmFunctionsWithReturnTypePrimitiveDataType(EdmVersion edmVersion)
        {
            var functions = new List<IEdmFunction>();
            int index = 0;
            foreach (var primitiveType in ModelBuilder.AllPrimitiveEdmTypes(edmVersion, false))
            {
                var function = new EdmFunction(DefaultNamespaceName, "FunctionsWithReturnTypePrimitiveDataType" + (++index), primitiveType);
                functions.Add(function);
            }

            return functions;
        }

        private static IList<IEdmFunction> EdmOperationsWith2Parameters(EdmVersion edmVersion)
        {
            var testData = new List<IEdmFunction>();

            var firstParamTypes = ModelBuilder.AllPrimitiveEdmTypes(edmVersion, true).Concat(MaxLengthEdmPrimitiveTypes()).Concat(PrecisionScaleFacetedEdmTypes()).ToArray();
            var secondParamTypes = MaxLengthEdmPrimitiveTypes().ToArray();
            var returnTypes = ModelBuilder.AllPrimitiveEdmTypes(edmVersion, false).ToArray();

            var numberOfOperations = Math.Max(firstParamTypes.Count(), secondParamTypes.Count());

            for (int n = 0; n < numberOfOperations; n++)
            {
                var function = new EdmFunction(DefaultNamespaceName, "FunctionWith2Parameters" + n, returnTypes[n%returnTypes.Count()]);
                function.AddParameter("Param1", firstParamTypes[n%firstParamTypes.Count()]);
                function.AddParameter("Param2", secondParamTypes[n%secondParamTypes.Count()]);
                testData.Add(function);
            }

            return testData;
        }

        private static IList<IEdmFunction> EdmOperationsWithNamedStructuralDataType(IEdmModel model)
        {
            var complexTypes = model.SchemaElements.OfType<IEdmComplexType>();
            var testData = new List<IEdmFunction>();
            int index = 0;
            var getName = new Func<string, string>((n) => n + (++index).ToString());
            var functions = complexTypes.Select(t =>
                {
                    var typeReference = new EdmComplexTypeReference(t, true);
                    var function = new EdmFunction(DefaultNamespaceName, getName("FunctionsWithNamedStructuralDataType"), typeReference);
                    function.AddParameter(getName("ComplexTypeParameter"), typeReference);
                    return function;
                });

            foreach (var edmFunction in functions)
            {
                testData.Add(edmFunction);
            }

            var entityTypes = model.SchemaElements.OfType<IEdmEntityType>();
            functions = entityTypes.Select(t =>
                {
                    var typeReference = new EdmEntityTypeReference(t, false);
                    var function = new EdmFunction(DefaultNamespaceName, getName("FunctionsWithNamedStructuralDataType"), typeReference);
                    function.AddParameter(getName("EntityTypeParameter"), typeReference);
                    return function;
                });

            foreach (var edmFunction in functions)
            {
                testData.Add(edmFunction);
            }

            return testData;
        }
    }
}
