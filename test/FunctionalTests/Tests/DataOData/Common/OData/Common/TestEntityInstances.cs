//---------------------------------------------------------------------
// <copyright file="TestEntityInstances.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting entity instances used in payloads.
    /// </summary>
    public static class TestEntityInstances
    {
        /// <summary>
        /// Creates a set of interesting entity instances along with metadata.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <param name="model">If non-null, the method creates complex types for the complex values and adds them to the model.</param>
        /// <param name="withTypeNames">true if the payloads should specify type names.</param>
        /// <returns>List of test descriptors with interesting entity instances as payload.</returns>
        public static IEnumerable<PayloadTestDescriptor> CreateEntityInstanceTestDescriptors(
            EdmModel model,
            bool withTypeNames)
        {
            IEnumerable<PrimitiveValue> primitiveValues = TestValues.CreatePrimitiveValuesWithMetadata(fullSet: false);
            IEnumerable<ComplexInstance> complexValues = TestValues.CreateComplexValues(model, withTypeNames, fullSet: false);
            IEnumerable<NamedStreamInstance> streamReferenceValues = TestValues.CreateStreamReferenceValues(fullSet: false);
            IEnumerable<PrimitiveMultiValue> primitiveMultiValues = TestValues.CreatePrimitiveCollections(withTypeNames, fullSet: false);
            IEnumerable<ComplexMultiValue> complexMultiValues = TestValues.CreateComplexCollections(model, withTypeNames, fullSet: false);
            IEnumerable<NavigationPropertyInstance> navigationProperties = TestValues.CreateDeferredNavigationLinks();

            // NOTE we have to copy the EntityModelTypeAnnotation on the primitive value to the NullPropertyInstance for null values since the 
            //      NullPropertyInstance does not expose a value. We will later copy it back to the value we generate for the null property.
            IEnumerable<PropertyInstance> primitiveProperties =
                primitiveValues.Select((pv, ix) => PayloadBuilder.Property("PrimitiveProperty" + ix, pv).CopyAnnotation<PropertyInstance, EntityModelTypeAnnotation>(pv));
            IEnumerable<PropertyInstance> complexProperties = complexValues.Select((cv, ix) => PayloadBuilder.Property("ComplexProperty" + ix, cv));
            IEnumerable<PropertyInstance> primitiveMultiValueProperties = primitiveMultiValues.Select((pmv, ix) => PayloadBuilder.Property("PrimitiveMultiValueProperty" + ix, pmv));
            IEnumerable<PropertyInstance> complexMultiValueProperties = complexMultiValues.Select((cmv, ix) => PayloadBuilder.Property("ComplexMultiValueProperty" + ix, cmv));

            PropertyInstance[][] propertyMatrix = new PropertyInstance[6][];
            propertyMatrix[0] = primitiveProperties.ToArray();
            propertyMatrix[1] = complexProperties.ToArray();
            propertyMatrix[2] = streamReferenceValues.ToArray();
            propertyMatrix[3] = primitiveMultiValueProperties.ToArray();
            propertyMatrix[4] = complexMultiValueProperties.ToArray();
            propertyMatrix[5] = navigationProperties.ToArray();

            IEnumerable<PropertyInstance[]> propertyCombinations = propertyMatrix.ColumnCombinations(0, 1, 6);

            int count = 0;
            foreach (PropertyInstance[] propertyCombination in propertyCombinations)
            {
                // build the entity type, add it to the model
                EdmEntityType generatedEntityType = null;
                string typeName = "PGEntityType" + count;
                EdmEntityContainer container = null;
                EdmEntitySet entitySet = null;
                if (model != null)
                {
                    // generate a new type with the auto-generated name, check that no type with this name exists and add the default key property to it.
                    Debug.Assert(model.FindDeclaredType(typeName) == null, "Entity type '" + typeName + "' already exists.");
                    generatedEntityType = new EdmEntityType("TestModel", typeName);
                    generatedEntityType.AddKeys(generatedEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    model.AddElement(generatedEntityType);
                    container = model.EntityContainer as EdmEntityContainer;

                    if (container == null)
                    {
                        container = new EdmEntityContainer("TestModel", "DefaultNamespace");
                        model.AddElement(container);
                    }

                    entitySet = container.AddEntitySet(typeName, generatedEntityType);
                }

                EntityInstance entityInstance = PayloadBuilder.Entity("TestModel." + typeName)
                    .Property("Id", PayloadBuilder.PrimitiveValue(count).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(false)));

                for (int i = 0; i < propertyCombination.Length; ++i)
                {
                    PropertyInstance currentProperty = propertyCombination[i];
                    entityInstance.Add(currentProperty);

                    if (model != null)
                    {
                        if (entitySet == null)
                        {
                            entitySet = container.FindEntitySet(typeName) as EdmEntitySet;
                        }

                        switch (currentProperty.ElementType)
                        {
                            case ODataPayloadElementType.ComplexProperty:
                                ComplexProperty complexProperty = (ComplexProperty)currentProperty;
                                generatedEntityType.AddStructuralProperty(complexProperty.Name,
                                    complexProperty.Value.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                                break;

                            case ODataPayloadElementType.PrimitiveProperty:
                                PrimitiveProperty primitiveProperty = (PrimitiveProperty)currentProperty;
                                if (primitiveProperty.Value == null)
                                {
                                    generatedEntityType.AddStructuralProperty(
                                       primitiveProperty.Name,
                                       PayloadBuilder.PrimitiveValueType(null));
                                }
                                else
                                {
                                    generatedEntityType.AddStructuralProperty(primitiveProperty.Name,
                                        primitiveProperty.Value.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                                }
                                break;
                            case ODataPayloadElementType.NamedStreamInstance:
                                NamedStreamInstance streamProperty = (NamedStreamInstance)currentProperty;
                                generatedEntityType.AddStructuralProperty(streamProperty.Name, EdmPrimitiveTypeKind.Stream);
                                break;

                            case ODataPayloadElementType.EmptyCollectionProperty:
                                throw new NotImplementedException();

                            case ODataPayloadElementType.NavigationPropertyInstance:
                                NavigationPropertyInstance navigationProperty = (NavigationPropertyInstance)currentProperty;
                                var navProperty = generatedEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
                                {
                                    ContainsTarget = false,
                                    Name = navigationProperty.Name,
                                    Target = generatedEntityType,
                                    TargetMultiplicity = EdmMultiplicity.One
                                });
                                entitySet.AddNavigationTarget(navProperty, entitySet);
                                break;

                            case ODataPayloadElementType.ComplexMultiValueProperty:
                                ComplexMultiValueProperty complexMultiValueProperty = (ComplexMultiValueProperty)currentProperty;
                                generatedEntityType.AddStructuralProperty(complexMultiValueProperty.Name,
                                    complexMultiValueProperty.Value.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                                break;

                            case ODataPayloadElementType.PrimitiveMultiValueProperty:
                                PrimitiveMultiValueProperty primitiveMultiValueProperty = (PrimitiveMultiValueProperty)currentProperty;
                                generatedEntityType.AddStructuralProperty(primitiveMultiValueProperty.Name,
                                     primitiveMultiValueProperty.Value.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                                break;

                            default:
                                throw new NotSupportedException("Unsupported element type found : " + propertyCombination[i].ElementType);
                        }
                    }
                }

                if (generatedEntityType != null)
                {
                    entityInstance.AddAnnotation(new EntityModelTypeAnnotation(generatedEntityType.ToTypeReference(true)));
                }

                yield return new PayloadTestDescriptor() { PayloadElement = entityInstance, PayloadEdmModel = model };

                count++;
            }
        }
    }
}
