//---------------------------------------------------------------------
// <copyright file="JsonLightPayloadElementFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using EdmConstants = Microsoft.Test.Taupo.OData.Common.EdmConstants;

    /// <summary>
    /// Modifies a payload element for use in Json Lite test configurations.
    /// </summary>
    public class JsonLightPayloadElementFixup : ODataPayloadElementVisitorBase
    {
        private readonly PayloadReaderTestDescriptor testDescriptor;
        private readonly Stack<ODataPayloadElement> payloadElementStack;

        private JsonLightPayloadElementFixup(PayloadReaderTestDescriptor testDescriptor)
        {
            this.testDescriptor = testDescriptor;
            this.payloadElementStack = new Stack<ODataPayloadElement>();
        }

        /// <summary>
        /// Modifies a test descriptor so that it can be used in a Json Lite test configuration.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to modify.</param>
        /// <remarks>The test descriptor is modified in place, though the payload element and model are cloned prior to change.</remarks>
        public static void Fixup(PayloadReaderTestDescriptor testDescriptor)
        {
            testDescriptor.PayloadElement = testDescriptor.PayloadElement.DeepCopy();
            new JsonLightPayloadElementFixup(testDescriptor).Recurse(testDescriptor.PayloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is a ComplexInstanceCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                this.AddExpectedFunctionImportToCollection(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a ComplexProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                Func<MemberProperty, bool> matchesProperty =
                    (p) =>
                    {
                        if (p.Name == payloadElement.Name && p.PropertyType is ComplexDataType)
                        {
                            var complexType = ((ComplexDataType)p.PropertyType).Definition;
                            return complexType.FullName == payloadElement.Value.FullTypeName;
                        }

                        return false;
                    };

                Func<IEdmProperty, bool> EdmMatchesProperty =
                        (p) =>
                        {
                            if (p.Name == payloadElement.Name && p.DeclaringType as IEdmComplexType != null)
                            {
                                var complexType = (IEdmComplexType)p.DeclaringType;
                                return complexType.FullName() == payloadElement.Value.FullTypeName;
                            }

                            return false;
                        };

                var valueTypeAnnotation = payloadElement.Value.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();
                if (valueTypeAnnotation != null)
                {
                    if (valueTypeAnnotation.EdmModelType != null)
                    {
                        var edmEntityType = valueTypeAnnotation.EdmModelType;
                        this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                    }
                }
                else
                {
                    var edmEntityType = this.ResolvePropertyEdmDataType(payloadElement.Value.FullTypeName);
                    this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                }
            }

            this.AnnotateIfOpenProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ComplexMultiValue payloadElement)
        {
            base.Visit(payloadElement);
            AddSerializationTypeAnnotationIfNone(payloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                Func<MemberProperty, bool> matchesProperty =
                    (p) =>
                    {
                        if (p.Name == payloadElement.Name && p.PropertyType is CollectionDataType)
                        {
                            var complexElementType = ((CollectionDataType)p.PropertyType).ElementDataType as ComplexDataType;
                            return complexElementType != null && payloadElement.Value.FullTypeName == "Collection(" + complexElementType.Definition.FullName + ")";
                        }

                        return false;
                    };

                Func<IEdmProperty, bool> EdmMatchesProperty =
                        (p) =>
                        {
                            if (p.Name == payloadElement.Name && p.DeclaringType as IEdmCollectionType != null)
                            {
                                var complexElementType = ((IEdmCollectionType)p.DeclaringType).ElementType as IEdmComplexType;
                                return complexElementType != null && payloadElement.Value.FullTypeName == "Collection(" + complexElementType.FullName() + ")";
                            }

                            return false;
                        };

                var valueTypeAnnotation = payloadElement.Value.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();

                if (valueTypeAnnotation != null)
                {
                    if (valueTypeAnnotation.EdmModelType != null)
                    {
                        var edmEntityType = valueTypeAnnotation.EdmModelType;
                        this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                    }
                }
                else
                {
                    var edmEntityType = this.ResolvePropertyEdmDataType(payloadElement.Value.FullTypeName);
                    this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                }
            }

            this.AnnotateIfOpenProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits a payload element whose root is a DeferredLink.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot() && payloadElement.GetAnnotation<JsonLightContextUriAnnotation>() == null)
            {
                var expectedTypeAnnotation = payloadElement.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
                if (expectedTypeAnnotation == null)
                {
                    // Any navigation property in the model should suffice for generating the context uri because there is no
                    // type information in the link.
                    if (this.testDescriptor.PayloadEdmModel != null)
                    {
                        IEdmEntityType edmEntityTypeWithNavProps = this.testDescriptor.PayloadEdmModel.EntityTypes().FirstOrDefault(e => e.NavigationProperties().Any());
                        ExceptionUtilities.CheckObjectNotNull(edmEntityTypeWithNavProps, "No navigation properties found in the model");
                        IEdmNavigationProperty edmNavProperty = edmEntityTypeWithNavProps.NavigationProperties().First();

                        payloadElement.AddAnnotation(new ExpectedTypeODataPayloadElementAnnotation { EdmOwningType = edmEntityTypeWithNavProps, EdmNavigationProperty = edmNavProperty });
                    }
                }
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a EntityInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot() && payloadElement.GetAnnotation<JsonLightContextUriAnnotation>() == null)
            {
                var typeAnnotation = payloadElement.Annotations.OfType<ExpectedTypeODataPayloadElementAnnotation>().SingleOrDefault();
                if (typeAnnotation == null && !string.IsNullOrEmpty(payloadElement.FullTypeName))
                {
                    if (this.testDescriptor.PayloadEdmModel != null)
                    {
                        var edmEntityType = this.testDescriptor.PayloadEdmModel.FindDeclaredType(payloadElement.FullTypeName);
                        var edmEntitySet = FindEntitySet(this.testDescriptor.PayloadEdmModel, edmEntityType);
                        payloadElement.ExpectedEntityType(edmEntityType, edmEntitySet);
                    }
                }
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a EntitySetInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            base.Visit(payloadElement);

            if (payloadElement.GetAnnotation<JsonLightContextUriAnnotation>() == null)
            {
                var typeAnnotation = payloadElement.Annotations.OfType<ExpectedTypeODataPayloadElementAnnotation>().SingleOrDefault();
                if (typeAnnotation == null && this.payloadElementStack.Count == 1)
                {
                    var annotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                    if (annotation != null)
                    {
                        if (this.testDescriptor.PayloadEdmModel != null && annotation.EdmModelType != null)
                        {
                            var edmEntityType = annotation.EdmModelType;
                            var edmEntitySet = FindEntitySet(this.testDescriptor.PayloadEdmModel, edmEntityType.Definition as IEdmSchemaType);
                            payloadElement.ExpectedEntityType(edmEntityType, edmEntitySet);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a LinkCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                var expectedTypeAnnotation = payloadElement.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
                if (expectedTypeAnnotation == null)
                {
                    // Any feed navigation property in the model should suffice for generating the context uri
                    if (this.testDescriptor.PayloadEdmModel != null)
                    {
                        IEdmEntityType edmEntityTypeWithNavProps = this.testDescriptor.PayloadEdmModel.EntityTypes().FirstOrDefault(e => e.NavigationProperties().Any(n => n.TargetMultiplicity() == EdmMultiplicity.Many));
                        ExceptionUtilities.CheckObjectNotNull(edmEntityTypeWithNavProps, "No navigation properties found in the model");
                        IEdmNavigationProperty edmNavProperty = edmEntityTypeWithNavProps.NavigationProperties().First(n => n.TargetMultiplicity() == EdmMultiplicity.Many);

                        payloadElement.AddAnnotation(new ExpectedTypeODataPayloadElementAnnotation { EdmOwningType = edmEntityTypeWithNavProps, EdmNavigationProperty = edmNavProperty });
                    }
                }
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            base.Visit(payloadElement);
            this.AddExpectedFunctionImportToCollection(payloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(PrimitiveMultiValue payloadElement)
        {
            base.Visit(payloadElement);
            AddSerializationTypeAnnotationIfNone(payloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                Func<MemberProperty, bool> matchesProperty =
                    (p) =>
                    {
                        if (p.Name == payloadElement.Name && p.PropertyType is CollectionDataType)
                        {
                            var primitiveElementType = ((CollectionDataType)p.PropertyType).ElementDataType as PrimitiveDataType;
                            return primitiveElementType != null && payloadElement.Value.FullTypeName == "Collection(" + primitiveElementType.FullEdmName() + ")";
                        }

                        return false;
                    };

                Func<IEdmProperty, bool> EdmMatchesProperty =
                        (p) =>
                        {
                            if (p.Name == payloadElement.Name && p.DeclaringType as IEdmCollectionType != null)
                            {
                                var complexElementType = ((IEdmCollectionType)p.DeclaringType).ElementType as IEdmPrimitiveType;
                                return complexElementType != null && payloadElement.Value.FullTypeName == "Collection(" + complexElementType.FullName() + ")";
                            }

                            return false;
                        };

                var valueTypeAnnotation = payloadElement.Value.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();
                if (valueTypeAnnotation != null)
                {
                    if (valueTypeAnnotation.EdmModelType != null)
                    {
                        var edmEntityType = valueTypeAnnotation.EdmModelType;
                        this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                    }
                }
                else
                {
                    var edmEntityType = this.ResolvePropertyEdmDataType(payloadElement.Value.FullTypeName);
                    this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                }
            }

            this.AnnotateIfOpenProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            base.Visit(payloadElement);

            if (this.CurrentElementIsRoot())
            {
                Func<MemberProperty, bool> matchesProperty =
                    (p) => p.Name == payloadElement.Name &&
                           p.PropertyType is PrimitiveDataType &&
                           ((PrimitiveDataType)p.PropertyType).FullEdmName() == payloadElement.Value.FullTypeName;
                
                Func<IEdmProperty, bool> EdmMatchesProperty =
                        (p) => p.Name == payloadElement.Name &&
                               p.DeclaringType as IEdmPrimitiveType != null &&
                               ((IEdmPrimitiveType)p).FullName() == payloadElement.Value.FullTypeName;

                var valueTypeAnnotation = payloadElement.Value.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();
                if (valueTypeAnnotation != null)
                {
                    if (valueTypeAnnotation.EdmModelType != null)
                    {
                        var edmEntityType = valueTypeAnnotation.EdmModelType;
                        this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                    }
                }
                else
                {
                    var edmEntityType = this.ResolvePropertyEdmDataType(payloadElement.Value.FullTypeName);
                    this.AddExpectedTypeToProperty(payloadElement, edmEntityType, EdmMatchesProperty);
                }
            }

            this.AnnotateIfOpenProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits a payload element whose root is a ServiceDocumentInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ServiceDocumentInstance payloadElement)
        {
            base.Visit(payloadElement);

            var contextUriAnnotation = payloadElement.GetAnnotation<JsonLightContextUriAnnotation>();
            if (contextUriAnnotation == null)
            {
                payloadElement.AddAnnotation(new JsonLightContextUriAnnotation { ContextUri = JsonLightConstants.DefaultMetadataDocumentUri.OriginalString });
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a WorkspaceInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(WorkspaceInstance payloadElement)
        {
            base.Visit(payloadElement);
            foreach (var resourceCollection in payloadElement.ResourceCollections)
            {
                if (string.IsNullOrEmpty(resourceCollection.Name))
                {
                    // Resource collection names are mandatory in JSON Light.
                    resourceCollection.Name = "ResourceCollection_" + Guid.NewGuid().ToString();
                }
            }
        }

        protected override void Recurse(ODataPayloadElement element)
        {
            try
            {
                this.payloadElementStack.Push(element);
                base.Recurse(element);
            }
            finally
            {
                this.payloadElementStack.Pop();
            }
        }
        
        /// <summary>
        /// Adds or modifies a property's ExpectedTypeODataPayloadElementAnnotation, to aid generation of the context uri.
        /// </summary>
        /// <param name="property">The property to annotate.</param>
        /// <param name="propertyValueType">The type of the property's value.</param>
        /// <param name="matchesProperty">Delegate for matching the property instance to a MemberProperty.</param>
        /// <remarks>
        /// If the method cannot resolve the parent type of the property, one will be created and added to the test descriptor's 
        /// PayloadModel. The descriptor's cached model will be reset.
        /// </remarks>
        private void AddExpectedTypeToProperty(PropertyInstance property, IEdmTypeReference propertyValueType, Func<IEdmProperty, bool> matchesProperty)
        {
            if (property.Annotations.OfType<JsonLightContextUriAnnotation>().Any())
            {
                return;
            }

            var typeAnnotation = property.Annotations.OfType<ExpectedTypeODataPayloadElementAnnotation>().SingleOrDefault();
            if (typeAnnotation == null || (typeAnnotation.MemberProperty == null && string.IsNullOrEmpty(typeAnnotation.OpenMemberPropertyName)))
            {
                ExpectedTypeODataPayloadElementAnnotation annotation = typeAnnotation ?? new ExpectedTypeODataPayloadElementAnnotation();

                IEdmModel model = this.testDescriptor.PayloadEdmModel;

                var entityType = model.EntityTypes().SingleOrDefault(t => t.Properties().Any(matchesProperty));
                if (entityType != null)
                {

                    annotation.EdmEntitySet = FindEntitySet(model, entityType);
                    annotation.EdmOwningType = entityType;
                    annotation.EdmProperty = entityType.Properties().FirstOrDefault(matchesProperty);
                }
                else
                {
                    var complexType = model.SchemaElements.OfType<IEdmComplexType>().SingleOrDefault(t => t.Properties().Any(matchesProperty));
                    if (complexType != null)
                    {
                        var complexProperty = complexType.Properties().Single(p => p.Name == property.Name);

                        annotation.EdmOwningType = complexType;
                        annotation.EdmProperty = complexProperty;
                        annotation.EdmExpectedType = complexProperty.Type;
                    }
                    else
                    {
                        // Add new entity type to the model and use that
                        IEdmTypeReference propertyType = annotation.EdmExpectedType ?? propertyValueType;
                        
                        EdmEntityType newEntityType = model.FindDeclaredType("TestModel.NewType") as EdmEntityType;
                        IEdmEntitySet newEntitySet = null;
                        IEdmProperty newProperty = null;
                        string newPorpertyName = property.Name ?? propertyType.FullName() ?? "EmptyName";

                        if (newEntityType == null)
                        {
                            newEntityType = new EdmEntityType("TestModel", "NewType");
                            newProperty = newEntityType.AddStructuralProperty(newPorpertyName, propertyType);
                            ((EdmModel)model).AddElement(newEntityType);
                            var container = model.EntityContainersAcrossModels().Single() as EdmEntityContainer;
                            newEntitySet = container.AddEntitySet("NewTypes", newEntityType);
                        }
                        else
                        {
                            newProperty = newEntityType.AddStructuralProperty(newPorpertyName, propertyType);
                            newEntitySet = FindEntitySet(model, newEntityType);
                        }

                        annotation.EdmEntitySet = newEntitySet;
                        annotation.EdmOwningType = newEntityType;
                        annotation.EdmProperty = newProperty;
                        annotation.EdmExpectedType = propertyType;

                        this.testDescriptor.PayloadEdmModel = model;
                        this.testDescriptor.ResetCachedModel();
                    }
                }

                property.SetAnnotation(annotation);
            }
        }

        private void AddExpectedFunctionImportToCollection(ODataPayloadElementCollection collection)
        {
            var expectedTypeAnnotation = collection.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
            if (expectedTypeAnnotation == null)
            {
                expectedTypeAnnotation = new ExpectedTypeODataPayloadElementAnnotation();
                collection.Add(expectedTypeAnnotation);
            }

            if (expectedTypeAnnotation.ProductFunctionImport == null)
            {
                var typeAnnotation = collection.GetAnnotation<EntityModelTypeAnnotation>();
                var collectionType = typeAnnotation.EdmModelType;

                if (this.testDescriptor.PayloadEdmModel != null)
                {
                    EdmModel model = this.testDescriptor.PayloadEdmModel as EdmModel;
                    EdmEntityContainer container = model.EntityContainer as EdmEntityContainer;
                    var functionImport = container.OperationImports().FirstOrDefault(f =>
                    { return f.Operation.ReturnType != null && f.Operation.ReturnType == collectionType; });
                    if (functionImport == null)
                    {
                        functionImport = container.OperationImports().FirstOrDefault(f =>
                        { return f.Operation.ReturnType != null && f.Operation.ReturnType.IsCollection(); });

                        if (functionImport == null)
                        {
                            var collectionNameAnnotation = collection.GetAnnotation<CollectionNameAnnotation>();
                            container.AddFunctionAndFunctionImport(model, collectionNameAnnotation == null ? "NewFunctionImport" : collectionNameAnnotation.Name,
                                collectionType);
                            this.testDescriptor.ResetCachedModel();
                        }
                    }

                    expectedTypeAnnotation.ProductFunctionImport = functionImport as EdmOperationImport;
                }
            }
        }

        private static IEdmEntitySet FindEntitySet(IEdmModel model, IEdmSchemaType entityType)
        {
            var entitySets = model.EntityContainer.EntitySets().Where(s => entityType.IsOrInheritsFrom(s.EntityType()));
            ExceptionUtilities.Assert(entitySets.Count() == 1, "Expected one entity set for entity type {0}. Found: {1}", entityType.Name, entitySets.Count());
            return entitySets.Single();
        }

        private bool CurrentElementIsRoot()
        {
            return this.payloadElementStack.Count == 1;
        }

        /// <summary>
        /// Adds a JSON-L odata.type property annotation to any properties of an open type that are
        /// not defined by the entity type.
        /// </summary>
        /// <param name="property">The property to annotate.</param>
        /// <param name="propertyValue">The property's value.</param>
        private void AnnotateIfOpenProperty(PropertyInstance property, ITypedValue propertyValue)
        {
            string propertyTypeName = propertyValue.FullTypeName;
            if (this.payloadElementStack.Count > 1 && !string.IsNullOrEmpty(propertyTypeName))
            {
                var parentEntity = this.payloadElementStack.Skip(1).First() as EntityInstance;
                if (parentEntity != null)
                {
                    if (this.testDescriptor.PayloadEdmModel != null)
                    {
                        var parentType = this.testDescriptor.PayloadEdmModel.EntityTypes().SingleOrDefault(t => t.TestFullName() == parentEntity.FullTypeName);
                        if (parentType != null &&
                        parentType.IsOpen &&
                        parentType.Properties().All(p => p.Name != property.Name) &&
                        property.Annotations.OfType<JsonLightPropertyAnnotationAnnotation>().All(a => a.AnnotationName != JsonLightConstants.ODataTypeAnnotationName))
                        {
                            property.WithPropertyAnnotation(JsonLightConstants.ODataTypeAnnotationName, propertyTypeName);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Resolves the specified Edm Data Type from the payload model.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type to resolve.</param>
        /// <returns>The EdmDataType that corresponds to the type name.</returns>
        private IEdmTypeReference ResolvePropertyEdmDataType(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
            {
                return null;
            }

            bool propertyIsCollection = false;
            if (fullTypeName.StartsWith(EdmConstants.CollectionTypeQualifier))
            {
                fullTypeName = EntityModelUtils.GetCollectionItemTypeName(fullTypeName);
                propertyIsCollection = true;
            }

            string namespaceName;
            string typeName;
            DataTypeUtils.ParseFullTypeName(fullTypeName, out typeName, out namespaceName);

            IEdmTypeReference edmDataType = null;

            var complexType = this.testDescriptor.PayloadEdmModel.FindType(fullTypeName);
            if (complexType != null)
            {
                edmDataType = complexType.ToTypeReference();
            }
            else
            {
                var primitiveType = EdmCoreModel.Instance.GetPrimitive(EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName), true);
                ExceptionUtilities.CheckObjectNotNull(primitiveType, "Failed to resolve type: " + fullTypeName);
                edmDataType = primitiveType;
            }

            if (propertyIsCollection)
            {
                edmDataType = EdmCoreModel.GetCollection(edmDataType);
            }

            return edmDataType;
        }

        /// <summary>
        /// Annotates the payload element with a SerializationTypeAnnotation if none are already present.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        private static void AddSerializationTypeAnnotationIfNone(ODataPayloadElement payloadElement)
        {
            var annotation = payloadElement.Annotations.OfType<SerializationTypeNameTestAnnotation>().SingleOrDefault();
            if (annotation == null)
            {
                payloadElement.Annotations.Add(new SerializationTypeNameTestAnnotation { TypeName = null });
            }
        }
    }
}
