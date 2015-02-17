//---------------------------------------------------------------------
// <copyright file="CollectionsModelFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// This model is used to test the Lists feature where particular entities have
    /// Collections of Primitive types and Collections of ComplexTypes
    /// </summary>
    public class CollectionsModelFixup : IEntityModelFixup 
    {
        private static DataType[] primitiveTypes = new DataType[]
        {
            EdmDataTypes.Binary(),
            EdmDataTypes.Boolean,
            EdmDataTypes.Byte,
            EdmDataTypes.DateTime(),
            EdmDataTypes.Decimal(),
            EdmDataTypes.Double,
            EdmDataTypes.Guid,
            EdmDataTypes.Int16,
            EdmDataTypes.Int32,
            EdmDataTypes.Int64,
            EdmDataTypes.Single,
            EdmDataTypes.String(),
        };
        
        /// <summary>
        /// Adds and updates existing types in the default model to use Primitive and Complex Collections
        /// </summary>
        /// <param name="model">Model to add fixup to.</param>
        public void Fixup(EntityModelSchema model)
        {
            // Create entityType with all PrimitiveTypes lists
            EntityType allPrimitiveCollectionTypesEntity = new EntityType("AllPrimitiveCollectionTypesEntity");
            allPrimitiveCollectionTypesEntity.Add(new MemberProperty("Key", EdmDataTypes.Int32));
            allPrimitiveCollectionTypesEntity.Properties[0].IsPrimaryKey = true;
            for (int i = 0; i < primitiveTypes.Length; i++)
            {
                DataType t = DataTypes.CollectionType.WithElementDataType(primitiveTypes[i]);
                allPrimitiveCollectionTypesEntity.Add(new MemberProperty("Property" + i, t));
            }

            model.Add(allPrimitiveCollectionTypesEntity);

             // Create a complexType  with all PrimitiveTypes Bags and primitive properties in it
            ComplexType additionalComplexType = new ComplexType("AdditionalComplexType");
            additionalComplexType.Add(new MemberProperty("Bag1", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.DateTime())));
            additionalComplexType.Add(new MemberProperty("Bag3", EdmDataTypes.String()));
            model.Add(additionalComplexType);

            // Create a complexType  with all PrimitiveTypes Bags in it
            ComplexType complexPrimitiveCollectionsType = new ComplexType("ComplexTypePrimitiveCollections");
            for (int i = 0; i < primitiveTypes.Length; i++)
            {
                DataType t = DataTypes.CollectionType.WithElementDataType(primitiveTypes[i]);
                complexPrimitiveCollectionsType.Add(new MemberProperty("Property" + i, t));
            }

            complexPrimitiveCollectionsType.Add(new MemberProperty("ComplexTypeBag", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithDefinition(additionalComplexType))));
            model.Add(complexPrimitiveCollectionsType);

            // Add the complexPrimitiveCollectionsType to an entity
            EntityType complexBagsEntity = new EntityType("ComplexBagsEntity");
            complexBagsEntity.Add(new MemberProperty("Key", EdmDataTypes.Int32));
            complexBagsEntity.Properties[0].IsPrimaryKey = true;
            DataType complexDataType = DataTypes.ComplexType.WithDefinition(complexPrimitiveCollectionsType);
            complexBagsEntity.Add(new MemberProperty("CollectionComplexTypePrimitiveCollections", DataTypes.CollectionType.WithElementDataType(complexDataType)));
            complexBagsEntity.Add(new MemberProperty("ComplexTypePrimitiveCollections", complexDataType));

            model.Add(complexBagsEntity);

            int numberOfComplexCollections = 0;
            
            // Update existing model so that every 3rd complexType is a Collection of ComplexTypes
            foreach (EntityType t in model.EntityTypes)
            {
                foreach (MemberProperty complexProperty in t.Properties.Where(p => p.PropertyType is ComplexDataType).ToList())
                {
                    // Remove existing one
                    t.Properties.Remove(complexProperty);

                    // Add new one 
                    t.Properties.Add(new MemberProperty(complexProperty.Name + "Collection", DataTypes.CollectionType.WithElementDataType(complexProperty.PropertyType)));
                    numberOfComplexCollections++;

                    if (numberOfComplexCollections > 4)
                    {
                        break;
                    }
                }

                if (numberOfComplexCollections > 4)
                {
                    break;
                }
            }

            new ResolveReferencesFixup().Fixup(model);

            // ReApply the previously setup namespace on to the new types
            new ApplyDefaultNamespaceFixup(model.EntityTypes.First().NamespaceName).Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);
            new SetDefaultCollectionTypesFixup().Fixup(model);
        }
    }
}
