//---------------------------------------------------------------------
// <copyright file="ModelBuildingHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Class that provides methods to allow unit tests to quickly create various EDM model elements that are entirely independent
    /// of each other.
    /// </summary>
    static internal class ModelBuildingHelpers
    {
        /// <summary>
        /// Static counter for creating entity types to help guarentee uniqueness.
        /// </summary>
        private static int EntityTypeNumber = 0;

        /// <summary>
        /// Static counter for creating entity containers to help guarentee uniqueness.
        /// </summary>
        private static int EntityContainerNumber = 0;

        /// <summary>
        /// Static counter for creating entity sets to help guarentee uniqueness.
        /// </summary>
        private static int EntitySetNumber = 0;

        public static IEdmNavigationProperty BuildValidNavigationProperty()
        {
            return BuildValidEntityType().AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
                {Name = "Reference", Target = BuildValidEntityType(), TargetMultiplicity = EdmMultiplicity.One});
        }

        public static IEdmNavigationProperty BuildCollectionNavigationProperty()
        {
            return BuildValidEntityType().AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() 
                { Name = "Reference", Target = BuildValidEntityType(), TargetMultiplicity = EdmMultiplicity.Many });
        }


        public static EdmEntityType BuildValidEntityType()
        {
            return new EdmEntityType("Name.Space", "EntityType" + EntityTypeNumber++);
        }

        public static IEdmCollectionType BuildValidEntityCollectionType()
        {
            return new EdmCollectionType(new EdmEntityTypeReference(BuildValidEntityType(), false));
        }

        public static EdmEntityType BuildValidOpenEntityType()
        {
            return new EdmEntityType("Name.Space", "OpenEntityType" + EntityTypeNumber++, null, false, true);
        }

        public static EdmEntityContainer BuildValidEntityContainer()
        {
            return new EdmEntityContainer("Name.Space", "Container" + EntityContainerNumber++);
        }

        public static IEdmStructuralProperty BuildValidPrimitiveProperty()
        {
            return BuildValidEntityType().AddStructuralProperty("PrimitiveProperty", EdmPrimitiveTypeKind.Double);
        }

        public static IEdmEntitySet BuildValidEntitySet()
        {
            return new EdmEntitySet(BuildValidEntityContainer(), "EntitySet" + EntitySetNumber++, BuildValidEntityType());
        }

        public static IEdmType BuildValidCollectionType()
        {
            return new EdmCollectionType(new EdmEntityTypeReference(BuildValidEntityType(), false));
        }

        public static IEdmType BuildValidComplexType()
        {
            return new EdmComplexType("Name.Space", "ComplexType");
        }

        public static EdmModel GetTestModelForNavigationPropertyBinding()
        {
            // Create a model with three navigation properties:
            // 1. "Many" with no target entity set
            // 2. "ZeroOrOne" with no target entity set
            // 3. "One" with no target entity set
            EdmEntityType geneType = new EdmEntityType("Test", "Gene");
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);
            vegetableType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "GenesModified", Target = geneType, TargetMultiplicity = EdmMultiplicity.Many });
            vegetableType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "DefectiveGene", Target = geneType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            vegetableType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "KeyGene", Target = geneType, TargetMultiplicity = EdmMultiplicity.One });

            // Note how we do NOT call AddNavigationTarget(...) to create associations 
            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            container.AddEntitySet("Genes", geneType);
            container.AddEntitySet("Vegetables", vegetableType);

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(geneType);
            return model;
        }

        public static EdmModel GetModelWithServiceOperationWithMissingReturnSet()
        {
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);

            // Note how the entity set expression is null even though it returns an entity
            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            var function = new EdmFunction("Test", "Container", new EdmEntityTypeReference(vegetableType, true));
            container.AddFunctionImport("GetVegetableWithMissingSet", function, null);

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            return model;
        }

        public static EdmModel GetModelWithActionWithMissingReturnSet()
        {
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);

            EdmAction action = new EdmAction("Test", "ActionWithMissingReturnSet", new EdmEntityTypeReference(vegetableType, true));
            // Note how the entity set expression is null even though it returns an entity
            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            container.AddActionImport("ActionWithMissingReturnSet", action);

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(action);
            return model;
        }

        public static EdmModel GetModelWithIllegalActionOverloads()
        {
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);

            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            var set = container.AddEntitySet("Vegetables", vegetableType);
            
            var action1 = new EdmAction("Test", "Action", new EdmEntityTypeReference(vegetableType, true), true /*isBound*/, null /*entitySetPath*/);
            action1.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            container.AddActionImport("Action", action1, new EdmEntitySetReferenceExpression(set));
            
            var action2 = new EdmAction("Test", "Action", new EdmEntityTypeReference(vegetableType, true), true /*isBound*/, null /*entitySetPath*/);
            action2.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            action2.AddParameter("p2", EdmCoreModel.Instance.GetInt32(false));
            container.AddActionImport("Action", action2, new EdmEntitySetReferenceExpression(set));

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(action1);
            model.AddElement(action2);
            return model;
        }

        public static IEdmModel GetModelWithMixedActionsAndFunctionsWithSameName()
        {
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable"); 
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);

            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            var set = container.AddEntitySet("Vegetables", vegetableType);

            var action1 = new EdmAction("Test", "Foo", new EdmEntityTypeReference(vegetableType, true), false /*isBound*/, null /*entitySetPath*/);
            action1.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            container.AddActionImport("Foo", action1, new EdmEntitySetReferenceExpression(set));

            var function1 = new EdmFunction("Test", "Foo", new EdmEntityTypeReference(vegetableType, true), false /*isBound*/, null, true);
            function1.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            function1.AddParameter("p2", EdmCoreModel.Instance.GetInt32(false));
            container.AddFunctionImport("Foo", function1, new EdmEntitySetReferenceExpression(set));

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(action1);
            model.AddElement(function1);
            return model;
        }

        public static IEdmModel GetModelFunctionsOnNonEntityTypes()
        {
            EdmComplexType colorInfoType = new EdmComplexType("Test", "ColorInfo");
            colorInfoType.AddProperty(new EdmStructuralProperty(colorInfoType, "Red", EdmCoreModel.Instance.GetInt32(false)));
            colorInfoType.AddProperty(new EdmStructuralProperty(colorInfoType, "Green", EdmCoreModel.Instance.GetInt32(false)));
            colorInfoType.AddProperty(new EdmStructuralProperty(colorInfoType, "Blue", EdmCoreModel.Instance.GetInt32(false)));

            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);
            vegetableType.AddStructuralProperty("Color", new EdmComplexTypeReference(colorInfoType, false));

            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            var set = container.AddEntitySet("Vegetables", vegetableType);

            var function1 = new EdmFunction("Test", "IsPrime", EdmCoreModel.Instance.GetBoolean(false), true, null, true);
            function1.AddParameter("integer", EdmCoreModel.Instance.GetInt32(false));
            container.AddFunctionImport("IsPrime", function1);

            var action = new EdmAction("Test", "Subtract", EdmCoreModel.Instance.GetInt32(false), true /*isBound*/, null /*entitySetPath*/);
            action.AddParameter("integer", EdmCoreModel.Instance.GetInt32(false));
            container.AddActionImport(action);

            var function2 = new EdmFunction("Test", "IsDark", EdmCoreModel.Instance.GetBoolean(false), true, null, true);
            function2.AddParameter("color", new EdmComplexTypeReference(colorInfoType, false));
            container.AddFunctionImport("IsDark", function2);
            
            var function3 = new EdmFunction("Test", "IsDarkerThan", EdmCoreModel.Instance.GetBoolean(false), true, null, true);
            function3.AddParameter("color", new EdmComplexTypeReference(colorInfoType, false));
            function3.AddParameter("other", new EdmComplexTypeReference(colorInfoType, true));
            container.AddFunctionImport("IsDarkerThan", function3);

            var function4 = new EdmFunction("Test", "GetMostPopularVegetableWithThisColor", new EdmEntityTypeReference(vegetableType, true), true, new EdmPathExpression("color"), true);
            function4.AddParameter("color", new EdmComplexTypeReference(colorInfoType, false));
 
            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(action);
            model.AddElement(function1);
            model.AddElement(function2);
            model.AddElement(function3);
            model.AddElement(function4);
            return model;
        }

        public static IEdmModel GetModelWithFunctionWithDuplicateParameterNames()
        {
            // Can only build a model with functionWithDuplicateParams via CSDL...
            const string modelWithDuplicateParameterNames =
@"<?xml version=""1.0"" ?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
     
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
       
      <EntityType Name=""Vegetable"">
        <Key>
          <PropertyRef Name=""ID""/>
        </Key>
        <Property Name=""ID"" Nullable=""false"" Type=""Edm.Int32""/>
      </EntityType>
       
      <Function Name=""Foo"">
        <Parameter Name=""p2"" Type=""Edm.String""/>
        <Parameter Name=""p2"" Type=""Edm.Int32""/>
        <ReturnType Type=""Edm.Boolean"" />
      </Function>

      <EntityContainer Name=""Context"">
        <EntitySet EntityType=""Fully.Qualified.Namespace.Vegetable"" Name=""Vegetables""/>        
        <FunctionImport Name=""Foo"" Function=""Fully.Qualified.Namespace.Foo"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            return ReadEdmModel(modelWithDuplicateParameterNames);
        }

        private static IEdmModel ReadEdmModel(string edmxContent)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            using (StringReader stringReader = new StringReader(edmxContent))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    bool parsed = EdmxReader.TryParse(xmlReader, out model, out errors);
                    if (!parsed)
                    {
                        throw new Exception(errors.First().ErrorMessage);
                    }
                }
            }

            return model;
        }

        public static IEdmModel GetModelWithFunctionOverloadsWithSameParameterNames()
        {
            EdmEntityType vegetableType = new EdmEntityType("Test", "Vegetable");
            IEdmStructuralProperty id = vegetableType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            vegetableType.AddKeys(id);

            EdmEntityContainer container = new EdmEntityContainer("Test", "Container");
            var set = container.AddEntitySet("Vegetables", vegetableType); 
            
            var function1 = new EdmFunction("Test", "Foo", new EdmEntityTypeReference(vegetableType, true), true/*isBound*/, null, true /*isComposable*/);
            function1.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            function1.AddParameter("p2", EdmCoreModel.Instance.GetInt32(false));
            
            container.AddFunctionImport("Foo", function1, new EdmEntitySetReferenceExpression(set));

            var function2 = new EdmFunction("Test", "Foo", new EdmEntityTypeReference(vegetableType, true), true /*isBound*/, null, true /*isComposable*/);
            function2.AddParameter("p1", new EdmEntityTypeReference(vegetableType, false));
            function2.AddParameter("p2", EdmCoreModel.Instance.GetString(false));
            container.AddFunctionImport("Foo", function2, new EdmEntitySetReferenceExpression(set));

            EdmModel model = new EdmModel();
            model.AddElement(container);
            model.AddElement(vegetableType);
            model.AddElement(function1);
            model.AddElement(function2);
            return model;
        }
    }
}