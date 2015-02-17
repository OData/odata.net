//---------------------------------------------------------------------
// <copyright file="ServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.ServiceModel.Web;
    using System.Threading;
    #endregion

    /// <summary>
    /// Provides the service writers capability to specify the type which implements
    /// the service operations.
    /// </summary>
    public sealed class ServiceOperationProvider
    {
        /// <summary>
        /// Type implementing service operations.
        /// </summary>
        private Type type;

        /// <summary>
        /// Resolver that gives a <see cref="ResourceType"/> corresponding to given CLR type.
        /// </summary>
        private Func<Type, ResourceType> resourceTypeResolver;

        /// <summary>
        /// Resolver that gives a <see cref="ResourceSet"/> corresponding to given <see cref="ResourceType"/> and <see cref="MethodInfo"/>.
        /// </summary>
        private Func<ResourceType, MethodInfo, ResourceSet> resourceSetResolver;

        /// <summary>
        /// Lazily one-time initialized collection of service operations.
        /// </summary>
        private SimpleLazy<List<ServiceOperation>> serviceOperations;

        /// <summary>
        /// Constructs a new instance of ServiceOperationProvider.
        /// </summary>
        /// <param name="type">Type implementing service operations.</param>
        /// <param name="resourceTypeResolver">Resolver that gives a <see cref="ResourceSet"/> corresponding to given <see cref="ResourceType"/> and <see cref="MethodInfo"/>.</param>
        /// <param name="resourceSetResolver">Resolver that gives a <see cref="ResourceType"/> corresponding to given CLR type.</param>
        public ServiceOperationProvider(Type type, Func<Type, ResourceType> resourceTypeResolver, Func<ResourceType, MethodInfo, ResourceSet> resourceSetResolver)
        {
            WebUtil.CheckArgumentNull(type, "type");
            WebUtil.CheckArgumentNull(resourceTypeResolver, "resourceTypeResolver");
            WebUtil.CheckArgumentNull(resourceSetResolver, "resourceSetResolver");

            if (type.IsAbstract)
            {
                throw new InvalidOperationException(Strings.ServiceOperationProvider_TypeIsAbstract(type));
            }

            this.type = type;
            this.resourceTypeResolver = resourceTypeResolver;
            this.resourceSetResolver = resourceSetResolver;
            this.serviceOperations = new SimpleLazy<List<ServiceOperation>>(this.FindServiceOperations, true);
        }

        /// <summary>
        /// Returns all the service operations exposed on the registered types.
        /// </summary>
        /// <returns>Collection of service operations.</returns>
        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                return this.serviceOperations.Value;
            }
        }

        /// <summary>
        /// Iterates over all the interesting methods on the type passed in the constructor and infers
        /// all the service operations from it.
        /// </summary>
        /// <returns>A list of service operations inferred from the type provided in the constructor.</returns>
        private List<ServiceOperation> FindServiceOperations()
        {
            List<ServiceOperation> serviceOps = new List<ServiceOperation>();

            foreach (MethodInfo methodInfo in this.type.GetMethods(WebUtil.PublicInstanceBindingFlags | BindingFlags.FlattenHierarchy))
            {
                if (methodInfo.GetCustomAttributes(typeof(WebGetAttribute), true).Length != 0)
                {
                    serviceOps.Add(this.GetServiceOperationForMethod(methodInfo, XmlConstants.HttpMethodGet));
                }
                else if (methodInfo.GetCustomAttributes(typeof(WebInvokeAttribute), true).Length != 0)
                {
                    serviceOps.Add(this.GetServiceOperationForMethod(methodInfo, XmlConstants.HttpMethodPost));
                }
            }

            return serviceOps;
        }

        /// <summary>
        /// Returns a new <see cref="ServiceOperation"/> based on the specified <paramref name="method"/>
        /// instance.
        /// </summary>
        /// <param name="method">Method to expose as a service operation.</param>
        /// <param name="protocolMethod">Protocol (for example HTTP) method the service operation responds to.</param>
        /// <returns>Service operation corresponding to give <paramref name="method"/>.</returns>
        private ServiceOperation GetServiceOperationForMethod(MethodInfo method, string protocolMethod)
        {
            Debug.Assert(method != null, "method != null");
            Debug.Assert(!method.IsAbstract, "!method.IsAbstract - if method is abstract, the type is abstract - already checked");

            bool hasSingleResult = BaseServiceProvider.MethodHasSingleResult(method);
            ServiceOperationResultKind resultKind;
            ResourceType resourceType = null;
            if (method.ReturnType == typeof(void))
            {
                resultKind = ServiceOperationResultKind.Void;
            }
            else
            {
                // Load the metadata of the resource type on the fly.
                // For Edm provider, it might not mean anything, but for reflection service provider, we need to
                // load the metadata of the type if its used only in service operation case
                Type resultType;
                if (WebUtil.IsPrimitiveType(method.ReturnType))
                {
                    resultKind = ServiceOperationResultKind.DirectValue;
                    resultType = method.ReturnType;
                    resourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(resultType);
                }
                else
                {
                    Type queryableElement = BaseServiceProvider.GetIQueryableElement(method.ReturnType);
                    if (queryableElement != null)
                    {
                        resultKind = hasSingleResult ?
                            ServiceOperationResultKind.QueryWithSingleResult :
                            ServiceOperationResultKind.QueryWithMultipleResults;
                        resultType = queryableElement;
                    }
                    else
                    {
                        Type enumerableElement = BaseServiceProvider.GetIEnumerableElement(method.ReturnType);
                        if (enumerableElement != null)
                        {
                            resultKind = ServiceOperationResultKind.Enumeration;
                            resultType = enumerableElement;
                        }
                        else
                        {
                            resultType = method.ReturnType;
                            resultKind = ServiceOperationResultKind.DirectValue;
                        }
                    }

                    Debug.Assert(resultType != null, "resultType != null");
                    resourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(resultType);
                    if (resourceType == null)
                    {
                        resourceType = this.ResolveResourceType(resultType);
                    }
                }

                if (resourceType == null)
                {
                    throw new InvalidOperationException(Strings.BaseServiceProvider_UnsupportedReturnType(method, method.ReturnType));
                }

                if (resultKind == ServiceOperationResultKind.Enumeration && hasSingleResult)
                {
                    throw new InvalidOperationException(Strings.BaseServiceProvider_IEnumerableAlwaysMultiple(type, method));
                }
            }

            ParameterInfo[] parametersInfo = method.GetParameters();
            ServiceOperationParameter[] parameters = new ServiceOperationParameter[parametersInfo.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parametersInfo[i];
                if (parameterInfo.IsOut || parameterInfo.IsRetval)
                {
                    throw new InvalidOperationException(Strings.BaseServiceProvider_ParameterNotIn(method, parameterInfo));
                }

                ResourceType parameterType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(parameterInfo.ParameterType);
                if (parameterType == null)
                {
                    throw new InvalidOperationException(
                        Strings.BaseServiceProvider_ParameterTypeNotSupported(method, parameterInfo, parameterInfo.ParameterType));
                }

                string parameterName = parameterInfo.Name ?? "p" + i.ToString(CultureInfo.InvariantCulture);
                parameters[i] = new ServiceOperationParameter(parameterName, parameterType);
            }

            ResourceSet resourceSet = null;
            if (resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                resourceSet = this.ResolveResourceSet(resourceType, method);

                if (resourceSet == null)
                {
                    throw new InvalidOperationException(
                        Strings.BaseServiceProvider_ServiceOperationMissingSingleEntitySet(method, resourceType.FullName));
                }
            }

            ServiceOperation operation = new ServiceOperation(method.Name, resultKind, resourceType, resourceSet, protocolMethod, parameters);

            operation.CustomState = method;

            MimeTypeAttribute attribute = BaseServiceProvider.GetMimeTypeAttribute(method);
            if (attribute != null)
            {
                operation.MimeType = attribute.MimeType;
            }

            return operation;
        }

        /// <summary>
        /// Method for obtaining a <see cref="ResourceType"/> corresponding to the given CLR type.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <returns><see cref="ResourceType"/> correspoding to <paramref name="type"/>.</returns>
        private ResourceType ResolveResourceType(Type type)
        {
            Debug.Assert(this.resourceTypeResolver != null, "ResourceType resolver must be initialized.");
            return this.resourceTypeResolver(type);
        }

        /// <summary>
        /// Method for obtaining a <see cref="ResourceSet"/> corresponding to given resource type.
        /// </summary>
        /// <param name="resourceType">Given resource type.</param>
        /// <param name="methodInfo">MethodInfo for a service operation.</param>
        /// <returns><see cref="ResourceSet"/> corresponding to <paramref name="resourceType"/>.</returns>
        private ResourceSet ResolveResourceSet(ResourceType resourceType, MethodInfo methodInfo)
        {
            Debug.Assert(this.resourceSetResolver != null, "ResourceSet resolver must be initialized.");
            return this.resourceSetResolver(resourceType, methodInfo);
        }
    }
}
