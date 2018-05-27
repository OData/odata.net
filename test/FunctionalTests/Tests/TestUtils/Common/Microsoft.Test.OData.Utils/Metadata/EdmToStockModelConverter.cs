//---------------------------------------------------------------------
// <copyright file="EdmToStockModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Common;

    public interface IEdmToStockModelConverter
    {
        EdmModel ConvertToStockModel(IEdmModel edmModel);
    }

    public class EdmToStockModelConverter : IEdmToStockModelConverter
    {
        public EdmToStockModelConverter()
        {
            this.EdmCoreModel = EdmCoreModel.Instance;
        }

        public EdmCoreModel EdmCoreModel
        {
            get;
            set;
        }

        public EdmModel ConvertToStockModel(IEdmModel edmModel)
        {
            var stockModel = new EdmModel();
            this.CreateStocksInModel(edmModel, stockModel);
            this.FillStockContents(edmModel, stockModel);
            this.AddStockVocabularies(edmModel, stockModel);

            return stockModel;
        }

        private void SetImmediateAnnotations(IEdmElement edmAnnotatable, IEdmElement stockAnnotatable, IEdmModel edmModel, EdmModel stockModel)
        {
            IEnumerable<IEdmDirectValueAnnotation> annotations = edmModel.DirectValueAnnotations(edmAnnotatable);

            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                var annotationValue = annotation.Value;
                string annotationNamespace = annotation.NamespaceUri;
                string annotationName = annotation.Name;
                stockModel.SetAnnotationValue(stockAnnotatable, annotationNamespace, annotationName, annotationValue);
            }
        }

        private IEdmVocabularyAnnotatable ConvertToStockVocabularyAnnotatable(IEdmElement edmAnnotatable, EdmModel stockModel)
        {
            // TODO: Need to provide a better way to get more details on IEdmVocabularyAnnotation.Target.
            IEdmVocabularyAnnotatable stockAnnotatable = null;
            if (edmAnnotatable is IEdmEntityType)
            {
                var edmEntityType = edmAnnotatable as IEdmEntityType;
                stockAnnotatable = stockModel.FindType(edmEntityType.FullName()) as IEdmVocabularyAnnotatable;
                ExceptionUtilities.CheckObjectNotNull(stockAnnotatable, "The FindType method must be successful.");
            }
            else if (edmAnnotatable is IEdmProperty)
            {
                var edmProperty = edmAnnotatable as IEdmProperty;
                if (edmProperty.DeclaringType is IEdmSchemaElement)
                {
                    var stockSchemaElement = stockModel.FindType(((IEdmSchemaElement)edmProperty.DeclaringType).FullName());
                    ExceptionUtilities.CheckObjectNotNull(stockAnnotatable, "The FindType method must be successful.");
                    stockAnnotatable = ((IEdmStructuredType)stockSchemaElement).FindProperty(edmProperty.Name);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (edmAnnotatable is IEdmEntitySet)
            {
                var edmEntitySet = edmAnnotatable as IEdmEntitySet;
                // TODO: No backpointer to the Entity Container from EntitySet in the API. Is this OK?
                stockAnnotatable = stockModel.EntityContainer.EntitySets().Single(m => m.Name == edmEntitySet.Name);
            }
            else if (edmAnnotatable is IEdmEnumType)
            {
                var edmEnumType = edmAnnotatable as IEdmEnumType;
                stockAnnotatable = stockModel.FindType(edmEnumType.FullName());
            }
            else
            {
                throw new NotImplementedException();
            }

            return stockAnnotatable;
        }

        private void AddStockVocabularies(IEdmModel edmModel, EdmModel stockModel)
        {
            foreach (var valueTypeTerm in edmModel.SchemaElements.OfType<IEdmTerm>())
            {
                var stockValueTerm = new EdmTerm(valueTypeTerm.Namespace, valueTypeTerm.Name, this.ConvertToStockTypeReference(valueTypeTerm.Type, stockModel));
                stockModel.AddElement(stockValueTerm);
            }

            foreach (var edmAnnotation in edmModel.VocabularyAnnotations)
            {
                var stockAnnotation = new EdmVocabularyAnnotation(
                    this.ConvertToStockVocabularyAnnotatable(edmAnnotation.Target, stockModel),
                    stockModel.FindTerm(((IEdmSchemaElement)edmAnnotation.Term).FullName()),
                    edmAnnotation.Qualifier,
                    this.ConvertToStockExpression(edmAnnotation.Value, stockModel)
                    // TODO: Do we need FullName()?  
                    // TODO: FullName() is Namespace.Name, but should it be NamespaceUri.Name? 
                    // TODO: FullName() on Annotation.Term returns Vocabulary0.TermName. Vocabulary0 is the using Alias. Is this correct? 
                    // TODO: Namepsace on Annotation.Term returns Vocabulary0, which is the using Alias. Is this correct?
                );
                stockModel.AddVocabularyAnnotation(stockAnnotation);
            }
        }

        private void CreateStocksInModel(IEdmModel edmModel, EdmModel stockModel)
        {
            Dictionary<string, EdmComplexType> stockComplexTypes = new Dictionary<string, EdmComplexType>();
            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmComplexType>())
            {
                ConstructStockComplexTypeInModel(edmType, stockModel, stockComplexTypes);
            }

            Dictionary<string, EdmEntityType> stockEntityTypes = new Dictionary<string, EdmEntityType>();
            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                ConstructStockEntityTypeInModel(edmType, stockModel, stockEntityTypes);
            }

            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmEnumType>())
            {
                var stockType = new EdmEnumType(edmType.Namespace, edmType.Name, edmType.UnderlyingType, edmType.IsFlags);
                // TODO: IsBad, Documentation
                stockModel.AddElement(stockType);
            }

            var edmContainer = edmModel.EntityContainer;
            if (edmContainer != null)
            {
                var stockContainer = new EdmEntityContainer(edmContainer.Namespace, edmContainer.Name);
                // TODO: IsBad, Documentation
                stockModel.AddElement(stockContainer);
            }
        }

        private EdmEntityType ConstructStockEntityTypeInModel(IEdmEntityType entityType, EdmModel stockModel, Dictionary<string, EdmEntityType> stockEntityTypes)
        {
            EdmEntityType stockType;
            string fullName = entityType.FullName();
            if (!stockEntityTypes.TryGetValue(fullName, out stockType))
            {
                stockType = new EdmEntityType(
                    entityType.Namespace,
                    entityType.Name,
                    entityType.BaseType != null ? this.ConstructStockEntityTypeInModel((IEdmEntityType)entityType.BaseType, stockModel, stockEntityTypes) : null,
                    entityType.IsAbstract,
                    entityType.IsOpen);

                // TODO: IsBad, Documentation
                stockModel.AddElement(stockType);
                stockEntityTypes.Add(fullName, stockType);
            }

            return stockType;
        }

        private EdmComplexType ConstructStockComplexTypeInModel(IEdmComplexType complexType, EdmModel stockModel, Dictionary<string, EdmComplexType> stockComplexTypes)
        {
            EdmComplexType stockType;
            string fullName = complexType.FullName();
            if (!stockComplexTypes.TryGetValue(fullName, out stockType))
            {
                stockType = new EdmComplexType(
                    complexType.Namespace,
                    complexType.Name,
                    complexType.BaseType != null ? this.ConstructStockComplexTypeInModel((IEdmComplexType)complexType.BaseType, stockModel, stockComplexTypes) : null,
                    complexType.IsAbstract);

                // TODO: IsBad, Documentation
                stockModel.AddElement(stockType);
                stockComplexTypes.Add(fullName, stockType);
            }

            return stockType;
        }

        private void FillStockContents(IEdmModel edmModel, EdmModel stockModel)
        {
            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmComplexType>())
            {
                this.FillStockContentsForComplex(edmType, edmModel, stockModel);
                // TODO: IsBad, Documentation
            }

            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                this.FillStockContentsForEntityWithoutNavigation(edmType, edmModel, stockModel);
                // TODO: IsBad, Documentation 
            }

            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmEnumType>())
            {
                this.FillStockContentsForEnum(edmType, edmModel, stockModel);
                // TODO: IsBad, Documentation 
            }

            foreach (var edmType in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                this.CreateNavigationPropertiesForStockEntity(edmType, edmModel, stockModel);
            }

            this.CreateAndFillStockContentsForOperations(edmModel, stockModel);

            var edmContainer = edmModel.EntityContainer;
            if (edmContainer != null)
            {
                this.FillStockContentsForEntityContainer(edmContainer, edmModel, stockModel);
                // TODO: IsBad, Documentation
            }
        }

        private void CreateAndFillStockContentsForOperations(IEdmModel edmModel, EdmModel stockModel)
        {
            foreach (var edmOperation in edmModel.SchemaElements.OfType<IEdmOperation>())
            {
                EdmOperation stockOperation = null;                
                var edmAction = edmOperation as IEdmAction;
                if (edmAction != null)
                {
                    stockOperation = new EdmAction(
                        edmAction.Namespace,
                        edmAction.Name,
                        edmAction.ReturnType == null ? edmOperation.ReturnType : ConvertToStockTypeReference(edmOperation.ReturnType, stockModel),
                        edmAction.IsBound,
                        edmAction.EntitySetPath);
                }
                else
                {
                    IEdmFunction edmFunction = edmOperation as IEdmFunction;
                    ExceptionUtilities.CheckObjectNotNull(edmFunction, "edmFunction");
                    stockOperation = new EdmFunction(
                    edmFunction.Namespace,
                    edmFunction.Name,
                    edmFunction.ReturnType == null ? edmFunction.ReturnType : ConvertToStockTypeReference(edmFunction.ReturnType, stockModel),
                    edmFunction.IsBound,
                    edmFunction.EntitySetPath,
                    edmFunction.IsComposable);
                }

                foreach (var edmParameter in edmOperation.Parameters)
                {
                    stockOperation.AddParameter(new EdmOperationParameter(edmOperation, edmParameter.Name, ConvertToStockTypeReference(edmParameter.Type, stockModel)));
                }

                stockModel.AddElement(stockOperation);
            }
        }

        private void FillStockContentsForEntityWithoutNavigation(IEdmEntityType edmType, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockType = (EdmEntityType)stockModel.FindType(edmType.FullName());
            this.SetImmediateAnnotations(edmType, stockType, edmModel, stockModel);

            foreach (var edmProperty in edmType.DeclaredStructuralProperties())
            {
                ConvertToStockStructuralProperty((IEdmStructuralProperty)edmProperty, edmModel, stockModel);
            }

            if (edmType.DeclaredKey != null)
            {
                stockType.AddKeys(edmType.DeclaredKey.Select(n => stockType.FindProperty(n.Name) as IEdmStructuralProperty).ToArray());
            }
        }

        private void CreateNavigationPropertiesForStockEntity(IEdmEntityType edmType, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockType = (EdmEntityType)stockModel.FindType(edmType.FullName());

            foreach (var edmNavigation in edmType.DeclaredNavigationProperties())
            {
                var stockToRoleType = (EdmEntityType)stockModel.FindType(edmNavigation.ToEntityType().FullName());

                if (stockType.FindProperty(edmNavigation.Name) == null)
                {
                    Func<IEnumerable<IEdmStructuralProperty>, IEnumerable<IEdmStructuralProperty>> createDependentProperties = (dependentProps) =>
                    {
                        if (dependentProps == null)
                        {
                            return null;
                        }

                        var stockDependentProperties = new List<IEdmStructuralProperty>();
                        foreach (var dependentProperty in dependentProps)
                        {
                            var stockDepProp = edmNavigation.DependentProperties() != null ? stockType.FindProperty(dependentProperty.Name) : stockToRoleType.FindProperty(dependentProperty.Name);
                            stockDependentProperties.Add((IEdmStructuralProperty)stockDepProp);
                        }

                        return stockDependentProperties;
                    };

                    Func<IEdmReferentialConstraint, IEdmEntityType, IEnumerable<IEdmStructuralProperty>> createPrincipalProperties = (refConstraint, principalType) =>
                    {
                        if (refConstraint == null)
                        {
                            return null;
                        }

                        return refConstraint.PropertyPairs.Select(p => (IEdmStructuralProperty)principalType.FindProperty(p.PrincipalProperty.Name));
                    };

                    var propertyInfo = new EdmNavigationPropertyInfo()
                        {
                            Name = edmNavigation.Name,
                            Target = stockToRoleType,
                            TargetMultiplicity = edmNavigation.TargetMultiplicity(),
                            DependentProperties = createDependentProperties(edmNavigation.DependentProperties()),
                            PrincipalProperties = createPrincipalProperties(edmNavigation.ReferentialConstraint, stockToRoleType),
                            ContainsTarget = edmNavigation.ContainsTarget,
                            OnDelete = edmNavigation.OnDelete
                        };

                    bool bidirectional = edmNavigation.Partner != null && edmNavigation.ToEntityType().FindProperty(edmNavigation.Partner.Name) != null;
                    if (bidirectional)
                    {
                        var partnerInfo = new EdmNavigationPropertyInfo()
                        {
                            Name = edmNavigation.Partner.Name,
                            TargetMultiplicity = edmNavigation.Partner.TargetMultiplicity(), 
                            DependentProperties = createDependentProperties(edmNavigation.Partner.DependentProperties()),
                            PrincipalProperties = createPrincipalProperties(edmNavigation.Partner.ReferentialConstraint, stockType),
                            ContainsTarget = edmNavigation.Partner.ContainsTarget, 
                            OnDelete = edmNavigation.Partner.OnDelete
                        };

                        stockType.AddBidirectionalNavigation(propertyInfo, partnerInfo);
                    }
                    else
                    {
                        stockType.AddUnidirectionalNavigation(propertyInfo);
                    }
                }
            }
        }

        private void FillStockContentsForEntityContainer(IEdmEntityContainer edmContainer, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockContainer = (EdmEntityContainer)stockModel.FindEntityContainer(edmContainer.FullName());
            this.SetImmediateAnnotations(edmContainer, stockContainer, edmModel, stockModel);

            foreach (var edmNavigationSource in edmContainer.Elements.OfType<IEdmNavigationSource>())
            {
                var stockEntityType = (EdmEntityType)stockModel.FindType(GetFullName(edmNavigationSource.EntityType()));
                if (edmNavigationSource is IEdmSingleton)
                {
                    stockContainer.AddSingleton(edmNavigationSource.Name, stockEntityType);
                }
                else
                {
                    stockContainer.AddEntitySet(edmNavigationSource.Name, stockEntityType);
                }
            }

            foreach (var stockNavigationSource in stockContainer.Elements.OfType<EdmNavigationSource>())
            {
                var stockEntityType = (EdmEntityType)stockModel.FindType(GetFullName(stockNavigationSource.EntityType()));
                IEdmNavigationSource edmNavigationSource = edmContainer.FindEntitySet(stockNavigationSource.Name);
                if (edmNavigationSource == null)
                {
                    edmNavigationSource = edmContainer.FindSingleton(stockNavigationSource.Name);
                }

                var stockDerivedNavigations = GetAllNavigationFromDerivedTypesAndSelf(stockEntityType, stockModel);

                foreach (var stockNavigationProperty in stockDerivedNavigations)
                {
                    var edmNavigationProperty = edmNavigationSource.NavigationPropertyBindings.Select(n => n.NavigationProperty).SingleOrDefault(n => n.Name == stockNavigationProperty.Name);

                    if (edmNavigationProperty != null)
                    {
                        var targetEdmEntitySet = edmNavigationSource.FindNavigationTarget(edmNavigationProperty);

                        if (null != targetEdmEntitySet)
                        {
                            var targetEntitySetFromContainer = stockContainer.Elements.OfType<EdmEntitySet>().SingleOrDefault
                                (
                                    n =>
                                        GetBaseTypesAndSelf(((IEdmNavigationProperty)stockNavigationProperty).ToEntityType()).Select(m => GetFullName(m)).Contains(n.EntityType().FullName()) && n.Name == targetEdmEntitySet.Name
                                );

                            if (null == targetEntitySetFromContainer)
                            {
                                targetEntitySetFromContainer = stockContainer.Elements.OfType<EdmEntitySet>().SingleOrDefault
                                (
                                    n =>
                                        GetAllDerivedTypesAndSelf(((IEdmNavigationProperty)stockNavigationProperty).ToEntityType(), stockModel).Select(m => GetFullName(m)).Contains(n.EntityType().FullName()) && n.Name == targetEdmEntitySet.Name
                                );
                            }

                            stockNavigationSource.AddNavigationTarget(stockNavigationProperty, targetEntitySetFromContainer);
                        }
                    }
                }
            }
            
            foreach (var edmOperationImport in edmContainer.OperationImports())
            {
                EdmOperationImport stockEdmOperationImport = null;
                var edmActionImport = edmOperationImport as IEdmActionImport;

                if (edmActionImport != null)
                {
                    var newEdmAction = stockModel.FindDeclaredOperations(edmActionImport.Action.FullName()).OfType<IEdmAction>().FirstOrDefault() as EdmAction;
                    ExceptionUtilities.CheckObjectNotNull(newEdmAction, "cannot find action");
                    stockEdmOperationImport = stockContainer.AddActionImport(edmOperationImport.Name, newEdmAction, edmActionImport.EntitySet);
                }
                else
                {
                    IEdmFunctionImport edmFunctionImport = edmOperationImport as IEdmFunctionImport;
                    ExceptionUtilities.CheckArgumentNotNull(edmFunctionImport, "edmFunctionImport");

                    var newEdmFunction = edmModel.FindDeclaredOperations(edmFunctionImport.Function.FullName()).OfType<IEdmFunction>().FirstOrDefault();
                    ExceptionUtilities.CheckObjectNotNull(newEdmFunction, "Expected to find an function: " + edmFunctionImport.Function.FullName());
                    stockEdmOperationImport = stockContainer.AddFunctionImport(edmFunctionImport.Name, newEdmFunction, edmFunctionImport.EntitySet, edmFunctionImport.IncludeInServiceDocument);
                }

                this.SetImmediateAnnotations(edmOperationImport, stockEdmOperationImport, edmModel, stockModel);
            }
        }

        private IEnumerable<IEdmNavigationProperty> GetAllNavigationFromDerivedTypesAndSelf(IEdmEntityType entityType, EdmModel model)
        {
            var derivedTypes = this.GetAllDerivedTypesAndSelf(entityType, model);

            return derivedTypes.Select(n => n.NavigationProperties()).SelectMany(list => list).Distinct();
        }

        private IEnumerable<IEdmEntityType> GetAllDerivedTypesAndSelf(IEdmEntityType entityType, IEdmModel model)
        {
            var resultingTypes = new List<IEdmEntityType>();

            var toBeDerivedTypes = new List<IEdmEntityType> { entityType };
            var derivedTypes = new List<IEdmEntityType>();

            while (toBeDerivedTypes.Count() > 0)
            {
                foreach (var deriveTypes in toBeDerivedTypes)
                {
                    derivedTypes.AddRange(this.GetDirectlyDerivedTypes(deriveTypes, model));
                }

                resultingTypes.AddRange(toBeDerivedTypes.AsEnumerable());
                toBeDerivedTypes.Clear();
                toBeDerivedTypes.AddRange(derivedTypes.AsEnumerable());
                derivedTypes.Clear();
            }

            return resultingTypes;
        }

        private List<IEdmEntityType> GetDirectlyDerivedTypes(IEdmEntityType baseType, IEdmModel model)
        {
            return model.SchemaElements.OfType<IEdmEntityType>().Where(n => n.BaseType != null && n.BaseType.Equals(baseType)).ToList();
        }

        private List<IEdmStructuredType> GetBaseTypesAndSelf(IEdmStructuredType type)
        {
            var temp = new List<IEdmStructuredType>();
            temp.Add(type);
            if (type.BaseType == null)
            {
                return temp;
            }
            temp.AddRange(GetBaseTypesAndSelf(type.BaseType));
            return temp;
        }

        private void FillStockContentsForComplex(IEdmComplexType edmType, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockType = (EdmComplexType)stockModel.FindType(edmType.FullName());
            this.SetImmediateAnnotations(edmType, stockType, edmModel, stockModel);

            foreach (var edmProperty in edmType.DeclaredStructuralProperties())
            {
                ConvertToStockStructuralProperty((IEdmStructuralProperty)edmProperty, edmModel, stockModel);
            }
        }

        private void FillStockContentsForEnum(IEdmEnumType edmType, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockType = (IEdmEnumType)stockModel.FindType(edmType.FullName());
            this.SetImmediateAnnotations(edmType, stockType, edmModel, stockModel);

            foreach (var edmMember in edmType.Members)
            {
                ConvertToStockMember((IEdmEnumMember)edmMember, edmModel, stockModel);
            }
        }

        private IEdmTypeReference ConvertToStockTypeReference(IEdmTypeReference edmTypeReference, EdmModel stockModel)
        {
            IEdmTypeReference stockTypeReference = null;
            switch (edmTypeReference.Definition.TypeKind)
            {
                case EdmTypeKind.Entity:
                    var stockEntity = (IEdmEntityType)stockModel.FindType(edmTypeReference.FullName());
                    stockTypeReference = new EdmEntityTypeReference(stockEntity, edmTypeReference.IsNullable);
                    break;
                case EdmTypeKind.Complex:
                    var stockComplex = (IEdmComplexType)stockModel.FindType(edmTypeReference.FullName());
                    stockTypeReference = new EdmComplexTypeReference(stockComplex, edmTypeReference.IsNullable);
                    break;
                case EdmTypeKind.Primitive:
                    stockTypeReference = CreatePrimitveStockTypeReference(edmTypeReference, stockModel);
                    break;
                case EdmTypeKind.Collection:
                    stockTypeReference = CreateCollectionStockTypeReference(edmTypeReference, stockModel);
                    break;
                case EdmTypeKind.EntityReference:
                    stockTypeReference = CreateEntityReference(edmTypeReference, stockModel);
                    break;
                case EdmTypeKind.Enum:
                    var stockEnum = (IEdmEnumType)stockModel.FindType(edmTypeReference.FullName());
                    stockTypeReference = new EdmEnumTypeReference(stockEnum, edmTypeReference.IsNullable);
                    break;
                default:
                    throw new NotImplementedException("EdmTypeKind.None are not implemented.");
            }
            return stockTypeReference;
        }

        private IEdmTypeReference CreateEntityReference(IEdmTypeReference edmTypeReference, EdmModel stockModel)
        {
            var edmEntityReferenceTypeReference = edmTypeReference.AsEntityReference();
            var stockEntityReference = new EdmEntityReferenceType((IEdmEntityType)stockModel.FindType(edmEntityReferenceTypeReference.EntityType().FullName()));
            return new EdmEntityReferenceTypeReference(stockEntityReference, edmEntityReferenceTypeReference.IsNullable);
        }

        private IEdmTypeReference CreateCollectionStockTypeReference(IEdmTypeReference edmTypeReference, EdmModel stockModel)
        {
            var edmElementTypeReference = edmTypeReference.AsCollection().ElementType();
            if (edmElementTypeReference.Definition.TypeKind == EdmTypeKind.Collection)
            {
                return EdmCoreModel.GetCollection(CreateCollectionStockTypeReference(edmElementTypeReference, stockModel));
            }
            else
            {
                var stockElementTypeReference = ConvertToStockTypeReference(edmElementTypeReference, stockModel);
                return EdmCoreModel.GetCollection(stockElementTypeReference);
            }
        }

        private static IEdmTypeReference CreatePrimitveStockTypeReference(IEdmTypeReference edmTypeReference, EdmModel stockModel)
        {
            IEdmTypeReference stockTypeReference = null;
            switch (edmTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    var binaryTypeReference = edmTypeReference.AsBinary();
                    stockTypeReference = EdmCoreModel.Instance.GetBinary(binaryTypeReference.IsUnbounded, binaryTypeReference.MaxLength, binaryTypeReference.IsNullable);
                    break;
                case EdmPrimitiveTypeKind.String:
                    var stringTypeReference = edmTypeReference.AsString();
                    stockTypeReference = EdmCoreModel.Instance.GetString(stringTypeReference.IsUnbounded, stringTypeReference.MaxLength, stringTypeReference.IsUnicode ?? true, stringTypeReference.IsNullable);
                    break;
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                    var temporalTypeReference = edmTypeReference.AsTemporal();
                    stockTypeReference = EdmCoreModel.Instance.GetTemporal(temporalTypeReference.PrimitiveKind(), temporalTypeReference.Precision ?? 0, temporalTypeReference.IsNullable);
                    break;
                case EdmPrimitiveTypeKind.Decimal:
                    var decimalTypeReference = edmTypeReference.AsDecimal();
                    stockTypeReference = EdmCoreModel.Instance.GetDecimal(decimalTypeReference.Precision, decimalTypeReference.Scale, decimalTypeReference.IsNullable);
                    break;
                default:
                    stockTypeReference = EdmCoreModel.Instance.GetPrimitive(edmTypeReference.PrimitiveKind(), edmTypeReference.IsNullable);
                    break;
            }
            return stockTypeReference;
        }

        private string GetFullName(IEdmType edmType)
        {
            string name = string.Empty;
            switch (edmType.TypeKind)
            {
                case EdmTypeKind.Entity:
                    name = ((IEdmEntityType)edmType).FullName();
                    break;
                case EdmTypeKind.Primitive:
                    name = ((IEdmPrimitiveType)edmType).FullName();
                    break;
                case EdmTypeKind.Complex:
                    name = ((IEdmComplexType)edmType).FullName();
                    break;
                case EdmTypeKind.Enum:
                    name = ((IEdmEnumType)edmType).FullName();
                    break;
                default:
                    throw new NotImplementedException("Association, Collection, EntityRefererence, Row, None are not impelemented yet.");
            }
            return name;
        }


        private IEdmStructuralProperty ConvertToStockStructuralProperty(IEdmStructuralProperty edmProperty, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockPropertyDeclaringType = stockModel.FindType(GetFullName(edmProperty.DeclaringType)) as IEdmStructuredType;

            var stockProperty = new EdmStructuralProperty(
                                        stockPropertyDeclaringType,
                                        edmProperty.Name,
                                        ConvertToStockTypeReference(edmProperty.Type, stockModel),
                                        edmProperty.DefaultValueString
                                     );
            ((EdmStructuredType)stockPropertyDeclaringType).AddProperty(stockProperty);

            // TODO: Documentation
            this.SetImmediateAnnotations(edmProperty, stockProperty, edmModel, stockModel);

            return stockProperty;
        }

        private IEdmEnumMember ConvertToStockMember(IEdmEnumMember edmMember, IEdmModel edmModel, EdmModel stockModel)
        {
            var stockMemberDeclaringType = stockModel.FindType(GetFullName(edmMember.DeclaringType)) as IEdmEnumType;

            var stockMember = new EdmEnumMember(
                                        stockMemberDeclaringType,
                                        edmMember.Name,
                                        edmMember.Value
                                    );
            ((EdmEnumType)stockMemberDeclaringType).AddMember(stockMember);

            // TODO: Documentation
            this.SetImmediateAnnotations(edmMember, stockMember, edmModel, stockModel);

            return stockMember;
        }

        public IEdmExpression ConvertToStockExpression(IEdmExpression edmExpression, EdmModel stockModel)
        {
            IEdmExpression result = null;
            switch (edmExpression.ExpressionKind)
            {
                case EdmExpressionKind.Null:
                    result = EdmNullExpression.Instance;
                    break;
                case EdmExpressionKind.StringConstant:
                    var tempString = (IEdmStringConstantExpression)edmExpression;
                    result = new EdmStringConstant(tempString.Type != null ? this.ConvertToStockTypeReference(tempString.Type, stockModel).AsString() : null, tempString.Value);
                    break;
                case EdmExpressionKind.IntegerConstant:
                    var tempInteger = (IEdmIntegerConstantExpression)edmExpression;
                    result = new EdmIntegerConstant(tempInteger.Type != null ? this.ConvertToStockTypeReference(tempInteger.Type, stockModel).AsPrimitive() : null, tempInteger.Value);
                    break;
                case EdmExpressionKind.Record:
                    var tempRecord = (IEdmRecordExpression)edmExpression;
                    result = new EdmRecordExpression(
                        tempRecord.DeclaredType == null ? null : this.ConvertToStockTypeReference(tempRecord.DeclaredType, stockModel).AsStructured(),
                        tempRecord.Properties.Select(edmProperty => 
                            (IEdmPropertyConstructor)new EdmPropertyConstructor(edmProperty.Name, this.ConvertToStockExpression(edmProperty.Value, stockModel))));
                    break;
                case EdmExpressionKind.Collection:
                    var tempCollection = (IEdmCollectionExpression)edmExpression;
                    result = new EdmCollectionExpression(tempCollection.Elements.Select(element => this.ConvertToStockExpression(element, stockModel)));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }
    }
}
