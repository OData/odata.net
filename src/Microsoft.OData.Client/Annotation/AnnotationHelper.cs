//---------------------------------------------------------------------
// <copyright file="AnnotationHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Provides functions to get instance annotation or metadata annotation, or to build metadata annotation dictionary.
    /// </summary>
    internal static class AnnotationHelper
    {
        /// <summary>
        /// Gets the CLR value of a term that has been applied to the specified object
        /// </summary>
        /// <typeparam name="TResult">The CLR type of the annotation to be returned.</typeparam>
        /// <param name="context">The data service context.</param>
        /// <param name="source">The specified annotated object instance.</param>
        /// <param name="term">The term name.</param>
        /// <param name="qualifier">The qualifier name.</param>
        /// <param name="annotationValue">Value of the term evaluated.</param>
        /// <returns>True if the annotation value can be evaluated, else false.</returns>
        internal static bool TryGetMetadataAnnotation<TResult>(DataServiceContext context, object source, string term, string qualifier, out TResult annotationValue)
        {
            IEdmValueAnnotation edmValueAnnotation = null;
            ClientEdmStructuredValue clientEdmValue = null;
            PropertyInfo propertyInfo = null;
            MethodInfo methodInfo = null;

            var keyValue = source as Tuple<object, MemberInfo>;
            if (keyValue != null)
            {
                // Get metadata annotation defined on property or Navigation property or Operation or OperationImport
                var instance = keyValue.Item1;
                var memberInfo = keyValue.Item2;
                propertyInfo = memberInfo as PropertyInfo;
                methodInfo = memberInfo as MethodInfo;
                if (instance != null)
                {
                    IEdmType edmType = context.Model.GetOrCreateEdmType(instance.GetType());
                    if (edmType is IEdmStructuredType)
                    {
                        ClientTypeAnnotation clientTypeAnnotation = context.Model.GetClientTypeAnnotation(edmType);
                        clientEdmValue = new ClientEdmStructuredValue(instance, context.Model, clientTypeAnnotation);
                    }
                }
            }
            else
            {
                if (propertyInfo == null)
                {
                    propertyInfo = source as PropertyInfo;
                }

                if (methodInfo == null)
                {
                    methodInfo = source as MethodInfo;
                }
            }

            if (propertyInfo != null)
            {
                edmValueAnnotation = GetOrInsertCachedMetadataAnnotationForPropertyInfo(context, propertyInfo, term, qualifier);
                return TryEvaluateMetadataAnnotation(context, edmValueAnnotation, clientEdmValue, out annotationValue);
            }

            if (methodInfo != null)
            {
                edmValueAnnotation = GetOrInsertCachedMetadataAnnotationForMethodInfo(context, methodInfo, term, qualifier);
                return TryEvaluateMetadataAnnotation(context, edmValueAnnotation, clientEdmValue, out annotationValue);
            }

            var type = source as Type;
            Type underlyingType = type;
            if (type == null)
            {
                type = source.GetType();
                underlyingType = Nullable.GetUnderlyingType(type) ?? type;

                // For complex type or entity type instance, try to convert the instance to ClientEdmStructuredValue for further evaluation.
                IEdmType edmType = context.Model.GetOrCreateEdmType(underlyingType);
                if (edmType is IEdmStructuredType)
                {
                    ClientTypeAnnotation clientTypeAnnotation = context.Model.GetClientTypeAnnotation(edmType);
                    clientEdmValue = new ClientEdmStructuredValue(source, context.Model, clientTypeAnnotation);
                }
            }

            edmValueAnnotation = GetOrInsertCachedMetadataAnnotationForType(context, underlyingType, term, qualifier);
            return TryEvaluateMetadataAnnotation(context, edmValueAnnotation, clientEdmValue, out annotationValue);
        }

        /// <summary>
        /// Get Edm operation according to the MethodInfo from current data service context.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="methodInfo">The specified MethodInfo</param>
        /// <returns>The related <see cref="IEdmOperation"/> will be returned if it is found, or return null.</returns>
        internal static IEdmOperation GetEdmOperation(DataServiceContext context, MethodInfo methodInfo)
        {
            var serviceModel = context.Format.ServiceModel;
            if (serviceModel == null)
            {
                return null;
            }

            var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            Type bindingType = null;
            IEnumerable<Type> clientParameters;
            if (methodInfo.IsDefined(typeof(ExtensionAttribute), false))
            {
                bindingType = parameterTypes.First();
                clientParameters = parameterTypes.Skip(1);
            }
            else
            {
                bindingType = methodInfo.DeclaringType;
                clientParameters = parameterTypes;
            }

            var declaringType = methodInfo.DeclaringType;

            string methodInfoNameSpacePrefix = declaringType.Namespace + ".";
            if (context.ResolveName != null)
            {
                string serverSideDeclaringTypeName = context.ResolveName(declaringType);
                if (serverSideDeclaringTypeName != null)
                {
                    int index = serverSideDeclaringTypeName.LastIndexOf('.');
                    methodInfoNameSpacePrefix = index > 0 ? serverSideDeclaringTypeName.Substring(0, index + 1) : "";
                }
            }

            var serverSideMethodName = ClientTypeUtil.GetServerDefinedName(methodInfo);
            var operations = serviceModel.FindOperations(methodInfoNameSpacePrefix + serverSideMethodName).Where(o => o.IsBound);

            while (bindingType != null)
            {
                foreach (var operation in operations)
                {
                    Type bindingTypeFromTypeReference;

                    if (TryGetClrTypeFromEdmTypeReference(
                        context,
                        operation.Parameters.First().Type,
                        methodInfo.IsDefined(typeof(ExtensionAttribute), false),
                        out bindingTypeFromTypeReference)
                        && bindingTypeFromTypeReference == bindingType
                        && clientParameters.SequenceEqual(GetNonBindingParameterTypeArray(context, operation.Parameters, true)))
                    {
                        return operation;
                    }
                }

                if (methodInfo.IsDefined(typeof(ExtensionAttribute), false) && bindingType.IsGenericType())
                {
                    var genericTypeDefinition = bindingType.GetGenericTypeDefinition();
                    var genericArguments = bindingType.GetGenericArguments().ToList();
                    if (genericArguments.Count == 1)
                    {
                        var genericArgumentBaseType = genericArguments[0].GetBaseType();
                        if (genericArgumentBaseType != null)
                        {
                            bindingType = genericTypeDefinition.MakeGenericType(genericArgumentBaseType);
                            continue;
                        }
                    }
                }

                return null;
            }

            return null;
        }

        /// <summary>
        /// Get Edm operation import according to the MethodInfo from current data service context.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="methodInfo">The specified MethodInfo</param>
        /// <returns>The related <see cref="IEdmOperationImport"/> will be returned if it is found, or return null.</returns>
        internal static IEdmOperationImport GetEdmOperationImport(DataServiceContext context, MethodInfo methodInfo)
        {
            var serviceModel = context.Format.ServiceModel;
            if (serviceModel == null)
            {
                return null;
            }

            var serversideName = ClientTypeUtil.GetServerDefinedName(methodInfo);
            var operationImports = serviceModel.FindDeclaredOperationImports(serversideName);
            var clientParameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            foreach (var operationImport in operationImports)
            {
                var operationParameters = operationImport.Operation.Parameters;
                var clientOperationParameters = GetNonBindingParameterTypeArray(context, operationParameters, false);
                if (clientParameters.SequenceEqual(clientOperationParameters))
                {
                    return operationImport;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets vocabulary annotation that binds to a term and a qualifier from the metadata annotation dictionary in current data service context for a specified type.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="type">The specified annotated type.</param>
        /// <param name="term">The term name.</param>
        /// <param name="qualifier">The qualifier name.</param>
        /// <returns>The vocabulary annotation that binds to a term and a qualifier for the specified annotated type.</returns>
        private static IEdmValueAnnotation GetOrInsertCachedMetadataAnnotationForType(DataServiceContext context, Type type, string term, string qualifier)
        {
            var serviceModel = context.Format.ServiceModel;
            if (serviceModel == null)
            {
                return null;
            }

            IEdmValueAnnotation edmValueAnnotation = GetCachedMetadataAnnotation(context, type, term, qualifier);
            if (edmValueAnnotation != null)
            {
                return edmValueAnnotation;
            }

            IEdmVocabularyAnnotatable edmVocabularyAnnotatable = null;
            if (type.IsSubclassOf(typeof(DataServiceContext)))
            {
                edmVocabularyAnnotatable = serviceModel.EntityContainer;
            }
            else
            {
                var serversideName = context.ResolveName == null ? type.FullName : context.ResolveName(type);
                if (!string.IsNullOrWhiteSpace(serversideName))
                {
                    edmVocabularyAnnotatable = serviceModel.FindDeclaredType(serversideName);
                    if (edmVocabularyAnnotatable == null)
                    {
                        return null;
                    }
                }
            }

            // Gets the annotations which exactly match the qualifier and target.
            var edmValueAnnotations = serviceModel.FindVocabularyAnnotations<IEdmValueAnnotation>(edmVocabularyAnnotatable, term, qualifier)
                .Where(a => a.Qualifier == qualifier && a.Target == edmVocabularyAnnotatable);

            if (edmValueAnnotations.Count() == 0)
            {
                edmValueAnnotation = GetOrInsertCachedMetadataAnnotationForType(context, type.GetBaseType(), term, qualifier);
            }
            else if (edmValueAnnotations.Count() == 1)
            {
                edmValueAnnotation = edmValueAnnotations.Single();
            }

            InsertMetadataAnnotation(context, type, edmValueAnnotation);
            return edmValueAnnotation;
        }

        /// <summary>
        /// Gets vocabulary annotation that binds to a term and a qualifier from the metadata annotation dictionary in current data service context for a specified propertyInfo.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="propertyInfo">The specified annotated propertyInfo.</param>
        /// <param name="term">The term name.</param>
        /// <param name="qualifier">The qualifier name.</param>
        /// <returns>The vocabulary annotation that binds to a term and a qualifier for the specified annotated propertyInfo.</returns>
        private static IEdmValueAnnotation GetOrInsertCachedMetadataAnnotationForPropertyInfo(DataServiceContext context, PropertyInfo propertyInfo, string term, string qualifier)
        {
            var serviceModel = context.Format.ServiceModel;
            if (serviceModel == null)
            {
                return null;
            }

            IEdmValueAnnotation edmValueAnnotation = GetCachedMetadataAnnotation(context, propertyInfo, term, qualifier);
            if (edmValueAnnotation != null)
            {
                return edmValueAnnotation;
            }

            var severSidePropertyName = ClientTypeUtil.GetServerDefinedName(propertyInfo);
            if (string.IsNullOrEmpty(severSidePropertyName))
            {
                return null;
            }

            var declaringType = propertyInfo.DeclaringType;
            IEnumerable<IEdmValueAnnotation> edmValueAnnotations = null;
            if (declaringType.IsSubclassOf(typeof(DataServiceContext)))
            {
                var entityContainer = serviceModel.EntityContainer;
                var edmEntityContainerElements = entityContainer.Elements.Where(e => e.Name == severSidePropertyName);
                if (edmEntityContainerElements != null && edmEntityContainerElements.Count() == 1)
                {
                    edmValueAnnotations = serviceModel.FindVocabularyAnnotations<IEdmValueAnnotation>(
                        edmEntityContainerElements.Single(), term, qualifier).Where(a => a.Qualifier == qualifier);
                }
            }
            else
            {
                var serversideTypeName = context.ResolveName == null ? declaringType.FullName : context.ResolveName(declaringType);
                var edmType = serviceModel.FindDeclaredType(serversideTypeName);
                if (edmType != null)
                {
                    var edmStructuredType = edmType as IEdmStructuredType;
                    if (edmStructuredType != null)
                    {
                        var edmProperty = edmStructuredType.FindProperty(severSidePropertyName);
                        if (edmProperty != null)
                        {
                            edmValueAnnotations = serviceModel.FindVocabularyAnnotations<IEdmValueAnnotation>(
                                edmProperty, term, qualifier).Where(a => a.Qualifier == qualifier);
                        }
                    }
                }
            }

            if (edmValueAnnotations != null && edmValueAnnotations.Count() == 1)
            {
                edmValueAnnotation = edmValueAnnotations.Single();
                InsertMetadataAnnotation(context, propertyInfo, edmValueAnnotation);
                return edmValueAnnotation;
            }

            return null;
        }

        /// <summary>
        /// Gets vocabulary annotation that binds to a term and a qualifier from the metadata annotation dictionary in current data service context for a specified methodInfo.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="methodInfo">The specified annotated methodInfo.</param>
        /// <param name="term">The term name.</param>
        /// <param name="qualifier">The qualifier name.</param>
        /// <returns>The vocabulary annotation that binds to a term and a qualifier for the specified annotated methodInfo.</returns>
        private static IEdmValueAnnotation GetOrInsertCachedMetadataAnnotationForMethodInfo(DataServiceContext context, MethodInfo methodInfo, string term, string qualifier)
        {
            IEdmModel serviceModel = context.Format.ServiceModel;
            if (serviceModel == null)
            {
                return null;
            }

            IEdmValueAnnotation edmValueAnnotation = GetCachedMetadataAnnotation(context, methodInfo, term, qualifier);
            if (edmValueAnnotation != null)
            {
                return edmValueAnnotation;
            }

            IEdmVocabularyAnnotatable edmVocabularyAnnotatable = context.GetEdmOperationOrOperationImport(methodInfo);
            if (edmVocabularyAnnotatable == null)
            {
                return null;
            }

            var edmOperationImport = edmVocabularyAnnotatable as IEdmOperationImport;
            IEnumerable<IEdmValueAnnotation> edmValueAnnotations = null;
            if (edmOperationImport != null)
            {
                edmValueAnnotations = serviceModel.FindVocabularyAnnotations<IEdmValueAnnotation>(edmOperationImport, term, qualifier)
                    .Where(a => a.Qualifier == qualifier);
                if (!edmValueAnnotations.Any())
                {
                    edmVocabularyAnnotatable = edmOperationImport.Operation;
                }
            }

            if (edmValueAnnotations == null || !edmValueAnnotations.Any())
            {
                edmValueAnnotations = serviceModel.FindVocabularyAnnotations<IEdmValueAnnotation>(edmVocabularyAnnotatable, term, qualifier)
                    .Where(a => a.Qualifier == qualifier);
            }

            if (edmValueAnnotations != null && edmValueAnnotations.Count() == 1)
            {
                edmValueAnnotation = edmValueAnnotations.Single();
                InsertMetadataAnnotation(context, methodInfo, edmValueAnnotation);
                return edmValueAnnotation;
            }

            return null;
        }

        /// <summary>
        /// Get a cached metadata annotation for a specified annotatable object from current context.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="key">The specified annotatable object.</param>
        /// <param name="term">The term name of the annotation.</param>
        /// <param name="qualifier">The qualifier to be applied.</param>
        /// <returns><see cref="IEdmValueAnnotation"/> to be returned if it is found or return null. </returns>
        private static IEdmValueAnnotation GetCachedMetadataAnnotation(DataServiceContext context, object key, string term, string qualifier = null)
        {
            if (key != null && context.MetadataAnnotationsDictionary.ContainsKey(key))
            {
                var annotations = context.MetadataAnnotationsDictionary[key]
                    .Where(a => a.Term.FullName().Equals(term) && a.Qualifier == qualifier);

                // If there are more than one annotation per term and qualifier returned, we will return null
                if (annotations.Count() == 1)
                {
                    return annotations.Single();
                }
            }

            return null;
        }

        /// <summary>
        /// Insert an metadata annotation with the provided key to the metadata annotation dictionary.
        /// </summary>
        /// <param name="context">The data service context</param>
        /// <param name="key">The specified key</param>
        /// <param name="edmValueAnnotation">The metadata annotation to be inserted.</param>
        private static void InsertMetadataAnnotation(DataServiceContext context, object key, IEdmValueAnnotation edmValueAnnotation)
        {
            if (edmValueAnnotation != null)
            {
                IList<IEdmValueAnnotation> edmValueAnnotations;
                if (!context.MetadataAnnotationsDictionary.TryGetValue(key, out edmValueAnnotations))
                {
                    edmValueAnnotations = new List<IEdmValueAnnotation>();
                    context.MetadataAnnotationsDictionary.Add(key, edmValueAnnotations);
                }

                edmValueAnnotations.Add(edmValueAnnotation);
            }
        }

        /// <summary>
        /// Evaluate IEdmValueAnnotation to an CLR object
        /// </summary>
        /// <typeparam name="TResult">The CLR type of the annotation to be returned.</typeparam>
        /// <param name="context">The data service context.</param>
        /// <param name="edmValueAnnotation">IEdmValueAnnotation to be evaluated.</param>
        /// <param name="clientEdmValue">Value to use as context in evaluating the expression.</param>
        /// <param name="annotationValue">Value of the term evaluated.</param>
        /// <returns>True if the annotation value can be evaluated, else false.</returns>
        private static bool TryEvaluateMetadataAnnotation<TResult>(DataServiceContext context, IEdmValueAnnotation edmValueAnnotation, ClientEdmStructuredValue clientEdmValue, out TResult annotationValue)
        {
            if (edmValueAnnotation == null)
            {
                annotationValue = default(TResult);
                return false;
            }

            EdmToClrEvaluator evaluator = CreateEdmToClrEvaluator(context);

            try
            {
                annotationValue = evaluator.EvaluateToClrValue<TResult>(edmValueAnnotation.Value, clientEdmValue);
            }
            catch (InvalidOperationException)
            {
                // When expression contains Path. if the clientEdmValue is null, or the related property is not valid property of the clientEdmValue.
                // TheEvaluateToClrValue might throw InvalidOperationException;
                annotationValue = default(TResult);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create an <see cref="EdmToClrEvaluator"/> instance used to evaluate an edm value.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <returns>The <see cref="EdmToClrEvaluator"/> instance.</returns>
        private static EdmToClrEvaluator CreateEdmToClrEvaluator(DataServiceContext context)
        {
            AnnotationMaterializeHelper helper = new AnnotationMaterializeHelper(context);

            EdmToClrEvaluator evaluator = new EdmToClrEvaluator(
                null,
                null,
                helper.GetAnnnotationExpressionForType,
                helper.GetAnnnotationExpressionForProperty,
                context.Model);

            evaluator.EdmToClrConverter = new EdmToClrConverter(
                helper.TryCreateObjectInstance,
                helper.TryGetClientPropertyInfo,
                helper.TryGetClrTypeName);

            return evaluator;
        }

        /// <summary>
        /// Gets CLR types for a collection of <see cref="IEdmOperationParameter"/>s.
        /// </summary>
        /// <param name="context">The data service context</param>
        /// <param name="parameters">The parameters to be converted </param>
        /// <param name="isBound">This flag indicates whether the operation that these parameters belongs to is bound operation</param>
        /// <returns>The CLR types for the <paramref name="parameters"/></returns>
        private static Type[] GetNonBindingParameterTypeArray(DataServiceContext context, IEnumerable<IEdmOperationParameter> parameters, bool isBound = false)
        {
            List<Type> parameterTypes = new List<Type>();

            for (int i = isBound ? 1 : 0; i < parameters.Count(); ++i)
            {
                Type parameterType;
                if (TryGetClrTypeFromEdmTypeReference(context, parameters.ElementAt(i).Type, false, out parameterType))
                {
                    parameterTypes.Add(parameterType);
                }
            }

            return parameterTypes.ToArray();
        }

        /// <summary>
        /// Gets the CLR type based on the <see cref="IEdmTypeReference"/> and the current data service context.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="edmTypeReference">The specified edm type reference.</param>
        /// <param name="isBindingParameter">This flag indicates whether the edm type reference is used for a binding parameter.</param>
        /// <param name="clrType">The output parameter to return the CLR type.</param>
        /// <returns>True if the CLR type is found, or false.</returns>
        private static bool TryGetClrTypeFromEdmTypeReference(DataServiceContext context, IEdmTypeReference edmTypeReference, bool isBindingParameter, out Type clrType)
        {
            EdmTypeKind typeKind = edmTypeReference.Definition.TypeKind;
            if (typeKind == EdmTypeKind.None)
            {
                clrType = null;
                return false;
            }

            if (typeKind == EdmTypeKind.Primitive)
            {
                PrimitiveType primitiveType = null;
                if (PrimitiveType.TryGetPrimitiveType(edmTypeReference.Definition.FullName(), out primitiveType))
                {
                    clrType = primitiveType.ClrType;

                    if (edmTypeReference.IsNullable && ClientTypeUtil.CanAssignNull(clrType))
                    {
                        clrType = typeof(Nullable<>).MakeGenericType(clrType);
                    }

                    return true;
                }
            }

            if (typeKind == EdmTypeKind.Collection)
            {
                Type elementType;
                if (TryGetClrTypeFromEdmTypeReference(context, ((IEdmCollectionTypeReference)edmTypeReference).ElementType(), false, out elementType))
                {
                    if (isBindingParameter)
                    {
                        clrType = typeof(DataServiceQuery<>).MakeGenericType(elementType);
                    }
                    else
                    {
                        clrType = typeof(List<>).MakeGenericType(elementType);
                    }

                    return true;
                }
            }

            if (typeKind == EdmTypeKind.Complex || typeKind == EdmTypeKind.Entity || typeKind == EdmTypeKind.Enum)
            {
                clrType = ResolveTypeFromName(context, edmTypeReference.FullName());
                if (clrType == null)
                {
                    return false;
                }

                if (isBindingParameter)
                {
                    clrType = typeof(DataServiceQuerySingle<>).MakeGenericType(clrType);
                }

                return true;
            }

            clrType = null;
            return false;
        }

        /// <summary>
        /// Get the client CLR type according to the qualified type name.
        /// </summary>
        /// <param name="context">The data service context.</param>
        /// <param name="qualifiedTypeName">The qualified type name.</param>
        /// <returns>The client CLR type.</returns>
        private static Type ResolveTypeFromName(DataServiceContext context, string qualifiedTypeName)
        {
            var typeInClientModel = context.ResolveTypeFromName(qualifiedTypeName);
            if (typeInClientModel == null)
            {
                var typeNamespaceIndex = qualifiedTypeName.LastIndexOf('.');
                if (typeNamespaceIndex > 0)
                {
                    string typeNamespace = qualifiedTypeName.Substring(0, typeNamespaceIndex);
                    typeInClientModel = context.DefaultResolveType(qualifiedTypeName, typeNamespace, typeNamespace);
                }
            }

            return typeInClientModel;
        }

        /// <summary>
        /// Provides functions to <see cref="EdmToClrConverter"/> to get client CLR information by using edm info.
        /// </summary>
        private class AnnotationMaterializeHelper
        {
            /// <summary>
            /// The data service context.
            /// </summary>
            private DataServiceContext dataServiceContext;

            /// <summary>
            /// Create an <see cref="AnnotationMaterializeHelper"/> instance.
            /// </summary>
            /// <param name="context">Current data service context.</param>
            internal AnnotationMaterializeHelper(DataServiceContext context)
            {
                this.dataServiceContext = context;
            }

            /// <summary>
            /// Gets CLR type name based on the edm type name and <see cref=" IEdmModel"/>.
            /// </summary>
            /// <param name="edmModel">The edm model.</param>
            /// <param name="qualifiedEdmTypeName">The edm type name.</param>
            /// <param name="typeNameInClientModel">The client clr type name.</param>
            /// <returns>True if the client clr type name can be found, else false.</returns>
            internal bool TryGetClrTypeName(IEdmModel edmModel, string qualifiedEdmTypeName, out string typeNameInClientModel)
            {
                var typeInClientModel = ResolveTypeFromName(dataServiceContext, qualifiedEdmTypeName);
                typeNameInClientModel = typeInClientModel == null ? null : typeInClientModel.FullName;
                return typeNameInClientModel != null;
            }

            /// <summary>
            /// Gets client property info of a specified property name from a Type.
            /// </summary>
            /// <param name="type">The type that contains the property.</param>
            /// <param name="propertyName">The name of the property.</param>
            /// <param name="propertyInfo">The specified property, or null if the property is not found</param>
            /// <returns>True if the property is found, else false.</returns>
            internal bool TryGetClientPropertyInfo(Type type, string propertyName, out PropertyInfo propertyInfo)
            {
                propertyInfo = ClientTypeUtil.GetClientPropertyInfo(type, propertyName, this.dataServiceContext.UndeclaredPropertyBehavior);
                return propertyInfo != null;
            }

            /// <summary>
            /// Create an instance of a CLR type based on <see cref="IEdmValue"/> and <see cref="Type"/>.
            /// </summary>
            /// <param name="edmValue">The <see cref="IEdmStructuredValue"/> for which the <paramref name="objectInstance"/> needs to be created.</param>
            /// <param name="clrType">The CLR type of the object instance.</param>
            /// <param name="converter">The converter instance calling this method.</param>
            /// <param name="objectInstance">A CLR object instance created for the <paramref name="edmValue"/></param>
            /// <param name="objectInstanceInitialized">True if all properties of the created <paramref name="objectInstance"/> are initialized.
            /// False if properties of the created instance should be initialized using the default <see cref="EdmToClrConverter"/> logic.</param>
            /// <returns>True if the <paramref name="objectInstance"/> is created, else false.</returns>
            internal bool TryCreateObjectInstance(IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized)
            {
                return TryCreateClientObjectInstance(this.dataServiceContext, edmValue, clrType, out objectInstance, out objectInstanceInitialized);
            }

            /// <summary>
            /// Gets the <see cref="IEdmExpression"/> of an annotation targeting a specified <paramref name="edmType"/>.
            /// </summary>
            /// <param name="edmModel">The edm model.</param>
            /// <param name="edmType">The specified edm type.</param>
            /// <param name="termName">The term name.</param>
            /// <param name="qualifier">Qualifier to apply</param>
            /// <returns>The <see cref=" IEdmExpression"/> of the annotation, or null if it is not found.</returns>
            internal IEdmExpression GetAnnnotationExpressionForType(IEdmModel edmModel, IEdmType edmType, string termName, string qualifier)
            {
                if (termName != null)
                {
                    var clientTypeAnnotation = edmModel.GetClientTypeAnnotation(edmType);
                    if (clientTypeAnnotation != null)
                    {
                        var annotation = GetOrInsertCachedMetadataAnnotationForType(this.dataServiceContext, clientTypeAnnotation.ElementType, termName, qualifier);
                        if (annotation != null)
                        {
                            return annotation.Value;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Gets the <see cref="IEdmExpression"/> of an annotation targeting a specified property or navigation property of an <paramref name="edmType"/>.
            /// </summary>
            /// <param name="edmModel">The edm model.</param>
            /// <param name="edmType">The specified edm type.</param>
            /// <param name="propertyName">The name of the specified property or navigation property.</param>
            /// <param name="termName">The term name.</param>
            /// <param name="qualifier">Qualifier to apply</param>
            /// <returns>The <see cref=" IEdmExpression"/> of the annotation, or null if it is not found.</returns>
            internal IEdmExpression GetAnnnotationExpressionForProperty(IEdmModel edmModel, IEdmType edmType, string propertyName, string termName, string qualifier)
            {
                if (termName != null)
                {
                    var clientTypeAnnotation = edmModel.GetClientTypeAnnotation(edmType);
                    if (clientTypeAnnotation != null)
                    {
                        var propertyInfo = ClientTypeUtil.GetClientPropertyInfo(clientTypeAnnotation.ElementType, propertyName, this.dataServiceContext.UndeclaredPropertyBehavior);
                        if (propertyInfo != null)
                        {
                            var annotation = GetOrInsertCachedMetadataAnnotationForPropertyInfo(this.dataServiceContext, propertyInfo, termName, qualifier);
                            if (annotation != null)
                            {
                                return annotation.Value;
                            }
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Create an instance of a CLR type based on <see cref="IEdmValue"/> and <see cref="Type"/>.
            /// </summary>
            /// <param name="context">The data service context.</param>
            /// /// <param name="edmValue">The <see cref="IEdmStructuredValue"/> for which the <paramref name="objectInstance"/> needs to be created.</param>
            /// <param name="clrType">The CLR type of the object instance.</param>
            /// <param name="objectInstance">A CLR object instance created for the <paramref name="edmValue"/></param>
            /// <param name="objectInstanceInitialized">True if all properties of the created <paramref name="objectInstance"/> are initialized.
            /// False if properties of the created instance should be initialized using the default <see cref="EdmToClrConverter"/> logic.</param>
            /// <returns>True if the <paramref name="objectInstance"/> is created, else false.</returns>
            private static bool TryCreateClientObjectInstance(DataServiceContext context, IEdmStructuredValue edmValue, Type clrType, out object objectInstance, out bool objectInstanceInitialized)
            {
                var clientStructuredValue = edmValue as ClientEdmStructuredValue;
                Type realClientType = clrType;
                if (clientStructuredValue != null)
                {
                    var clientTypeAnnotation = context.Model.GetClientTypeAnnotation(edmValue.Type.Definition.FullName());
                    realClientType = clientTypeAnnotation.ElementType;
                }

                if (realClientType.IsSubclassOf(clrType))
                {
                    objectInstance = Activator.CreateInstance(realClientType);
                }
                else
                {
                    objectInstance = Activator.CreateInstance(clrType);
                }

                objectInstanceInitialized = false;
                return true;
            }
        }
    }
}
