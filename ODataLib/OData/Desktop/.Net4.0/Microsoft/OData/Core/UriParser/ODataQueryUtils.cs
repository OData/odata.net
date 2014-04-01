//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Utility methods used with the OData Query library.
    /// </summary>
    internal static class ODataQueryUtils
    {
        /// <summary>
        /// Checks whether reflection over the property is allowed or not.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <returns>true if reflection over the property is allowed; otherwise false.</returns>
        public static bool GetCanReflectOnInstanceTypeProperty(this IEdmProperty property, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataQueryEdmPropertyAnnotation annotation =
                model.GetAnnotationValue<ODataQueryEdmPropertyAnnotation>(property);
            return annotation == null ? false : annotation.CanReflectOnProperty;
        }

        /// <summary>
        /// Sets whether reflection over the property is allowed or not.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <param name="canReflect">true if reflection over the property is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceTypeProperty(this IEdmProperty property, IEdmModel model, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataQueryEdmPropertyAnnotation annotation =
                model.GetAnnotationValue<ODataQueryEdmPropertyAnnotation>(property);
            if (annotation == null)
            {
                if (canReflect)
                {
                    annotation = new ODataQueryEdmPropertyAnnotation
                        {
                            CanReflectOnProperty = true
                        };
                    model.SetAnnotationValue(property, annotation);
                }
            }
            else
            {
                annotation.CanReflectOnProperty = canReflect;
            }
        }

        /// <summary>
        /// Gets the result kind of the <paramref name="serviceOperation"/>.
        /// </summary>
        /// <param name="serviceOperation">The <see cref="IEdmOperationImport"/> to check.</param>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <returns>The result kind of the <paramref name="serviceOperation"/> or null if no result kind annotation exists.</returns>
        public static ODataServiceOperationResultKind? GetServiceOperationResultKind(
            this IEdmOperationImport serviceOperation, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(serviceOperation, "operationImport");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataQueryEdmServiceOperationAnnotation annotation =
                model.GetAnnotationValue<ODataQueryEdmServiceOperationAnnotation>(serviceOperation);
            return annotation == null ? null : (ODataServiceOperationResultKind?)annotation.ResultKind;
        }

        /// <summary>
        /// Sets the result kind of the <paramref name="serviceOperation"/>.
        /// </summary>
        /// <param name="serviceOperation">The <see cref="IEdmOperationImport"/> to check.</param>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="resultKind">The result kind to set.</param>
        public static void SetServiceOperationResultKind(this IEdmOperationImport serviceOperation, IEdmModel model, ODataServiceOperationResultKind resultKind)
        {
            ExceptionUtils.CheckArgumentNotNull(serviceOperation, "serviceOperation");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            ODataQueryEdmServiceOperationAnnotation existingAnnotation =
                model.GetAnnotationValue<ODataQueryEdmServiceOperationAnnotation>(serviceOperation);
            if (existingAnnotation == null)
            {
                ODataQueryEdmServiceOperationAnnotation newAnnotation =
                    new ODataQueryEdmServiceOperationAnnotation
                        {
                            ResultKind = resultKind
                        };
                model.SetAnnotationValue(serviceOperation, newAnnotation);
            }
            else
            {
                existingAnnotation.ResultKind = resultKind;
            }
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmOperationImport"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="operationName">The name of the service operation to look up.</param>
        /// <returns>An <see cref="IEdmOperationImport"/> instance with the specified <paramref name="operationName"/>; if no such service operation exists the method throws.</returns>
        public static IEdmOperationImport ResolveServiceOperation(this IEdmModel model, string operationName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            IEdmOperationImport operationImport = model.TryResolveServiceOperation(operationName);
            if (operationImport == null)
            {
                throw new ODataException(ODataErrorStrings.ODataQueryUtils_DidNotFindServiceOperation(operationName));
            }

            return operationImport;
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmOperationImport"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="operationName">The name of the service operation to look up.</param>
        /// <returns>An <see cref="IEdmOperationImport"/> instance with the specified <paramref name="operationName"/> or null if no such service operation exists.</returns>
        public static IEdmOperationImport TryResolveServiceOperation(this IEdmModel model, string operationName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            IEdmOperationImport operationImport = null;
            foreach (IEdmOperationImport import in model.ResolveFunctionImports(operationName))
            {
                if (model.IsServiceOperation(import))
                {
                    if (operationImport == null)
                    {
                        operationImport = import;
                    }
                    else
                    {
                        throw new ODataException(
                            ODataErrorStrings.ODataQueryUtils_FoundMultipleServiceOperations(operationName));
                    }
                }
            }

            return operationImport;
        }

        /// <summary>
        /// Returns the instance type for the specified <paramref name="typeReference"/> or null if none exists.
        /// </summary>
        /// <param name="typeReference">The type reference to get the instance type for.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <returns>The instance type for the <paramref name="typeReference"/> or null if no instance type exists.</returns>
        /// <remarks>All primitive type references are guaranteed to have an instance type.</remarks>
        public static Type GetInstanceType(this IEdmTypeReference typeReference, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (typeReference.TypeKind() == EdmTypeKind.Primitive)
            {
                IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitive();

                return EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference);
            }

            ODataQueryEdmTypeAnnotation annotation =
                model.GetAnnotationValue<ODataQueryEdmTypeAnnotation>(typeReference.Definition);
            return annotation == null ? null : annotation.InstanceType;
        }

        /// <summary>
        /// Returns the instance type for the specified <paramref name="type"/> or null if none exists.
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <returns>The instance type for the <paramref name="type"/> or null if no instance type exists.</returns>
        public static Type GetInstanceType(this IEdmType type, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                return EdmLibraryExtensions.GetPrimitiveClrType((IEdmPrimitiveTypeReference)type.ToTypeReference(false));
            }

            ODataQueryEdmTypeAnnotation annotation = model.GetAnnotationValue<ODataQueryEdmTypeAnnotation>(type);
            return annotation == null ? null : annotation.InstanceType;
        }

        /// <summary>
        /// Sets the instance type for the specified <paramref name="type"/>; if null is specified an existing instance type will be removed.
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <param name="model">Model containing annotations.</param>
        /// <param name="instanceType">The instance type for the <paramref name="type"/> or null to remove an existing instance type.</param>
        public static void SetInstanceType(this IEdmType type, IEdmModel model, Type instanceType)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                throw new ODataException(ODataErrorStrings.ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }

            ODataQueryEdmTypeAnnotation existingAnnotation = model.GetAnnotationValue<ODataQueryEdmTypeAnnotation>(type);
            if (existingAnnotation == null)
            {
                if (instanceType != null)
                {
                    ODataQueryEdmTypeAnnotation newAnnotation = new ODataQueryEdmTypeAnnotation
                        {
                            InstanceType = instanceType,
                        };

                    model.SetAnnotationValue(type, newAnnotation);
                }
            }
            else
            {
                existingAnnotation.InstanceType = instanceType;
            }
        }

        /// <summary>
        /// Checks whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="typeReference">The type reference to check.</param>
        /// <param name="model">Model containing annotations.</param>
        /// <returns>true if reflection over the instance type is allowed; otherwise false.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "primitiveTypeReference",
            Justification = "Local used in debug assertion.")]
        public static bool GetCanReflectOnInstanceType(this IEdmTypeReference typeReference, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (typeReference.TypeKind() == EdmTypeKind.Primitive)
            {
                // we can reflect over all primitive types
                return true;
            }

            ODataQueryEdmTypeAnnotation annotation =
                model.GetAnnotationValue<ODataQueryEdmTypeAnnotation>(typeReference.Definition);
            return annotation == null ? false : annotation.CanReflectOnInstanceType;
        }

        /// <summary>
        /// Sets whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="typeReference">The type reference to check.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <param name="canReflect">true if reflection over the instance type is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceType(this IEdmTypeReference typeReference, IEdmModel model, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            SetCanReflectOnInstanceType(typeReference.Definition, model, canReflect);
        }

        /// <summary>
        /// Sets whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="model">Model containing annotations.</param>
        /// <param name="canReflect">true if reflection over the instance type is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceType(this IEdmType type, IEdmModel model, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                throw new ODataException(ODataErrorStrings.ODataQueryUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }

            ODataQueryEdmTypeAnnotation annotation = model.GetAnnotationValue<ODataQueryEdmTypeAnnotation>(type);
            if (annotation == null)
            {
                if (canReflect)
                {
                    annotation = new ODataQueryEdmTypeAnnotation { CanReflectOnInstanceType = true };
                    model.SetAnnotationValue(type, annotation);
                }
            }
            else
            {
                annotation.CanReflectOnInstanceType = canReflect;
            }
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmEntitySet"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="entitySetName">The name of the entity set to look up.</param>
        /// <returns>An <see cref="IEdmEntitySet"/> instance with the specified <paramref name="entitySetName"/>; if no such entity set exists the method throws.</returns>
        public static IEdmEntitySet ResolveEntitySet(this IEdmModel model, string entitySetName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, "entitySetName");

            IEdmEntitySet entitySet = model.TryResolveEntitySet(entitySetName);
            if (entitySet == null)
            {
                throw new ODataException(ODataErrorStrings.ODataQueryUtils_DidNotFindEntitySet(entitySetName));
            }

            return entitySet;
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmEntitySet"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="entitySetName">The name of the entity set to look up.</param>
        /// <returns>An <see cref="IEdmEntitySet"/> instance with the specified <paramref name="entitySetName"/> or null if no such entity set exists.</returns>
        public static IEdmEntitySet TryResolveEntitySet(this IEdmModel model, string entitySetName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, "entitySetName");

            IEnumerable<IEdmEntityContainer> entityContainers = model.EntityContainers();
            if (entityContainers == null)
            {
                return null;
            }

            IEdmEntitySet entitySet = null;
            foreach (IEdmEntityContainer container in entityContainers)
            {
                entitySet = container.FindEntitySet(entitySetName);
                if (entitySet != null)
                {
                    break;
                }
            }

            return entitySet;
        }

        /// <summary>
        /// Method that checks whether a operation import is a service operation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing annotations.</param>
        /// <param name="operationImport">The <see cref="IEdmOperationImport"/> to check.</param>
        /// <returns>true if the <paramref name="operationImport"/> represents a service operation; otherwise false.</returns>
        /// <remarks>
        /// A <see cref="IEdmOperationImport"/> is considered a service operation if it is annotated with an m:HttpMethod attribute.
        /// </remarks>
        internal static bool IsServiceOperation(this IEdmModel model, IEdmOperationImport operationImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operationImport != null, "operationImport != null");
            Debug.Assert(model != null, "model != null");

            // Check whether an annotation on the operation import that makes it a service operation exists
            return model.GetHttpMethod(operationImport) != null;
        }

        /// <summary>
        /// Method that checks whether a operation import is an action.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing annotations.</param>
        /// <param name="operationImport">The <see cref="IEdmOperationImport"/> to check.</param>
        /// <returns>true if the <paramref name="operationImport"/> represents an action; otherwise false.</returns>
        /// <remarks>
        /// A <see cref="IEdmOperationImport"/> is considered an action if it is side-effecting but not annotated with an m:HttpMethod attribute.
        /// </remarks>
        internal static bool IsAction(this IEdmModel model, IEdmOperationImport operationImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operationImport != null, "operationImport != null");
            Debug.Assert(model != null, "model != null");

            return !operationImport.IsComposable && operationImport.IsSideEffecting && !model.IsServiceOperation(operationImport);
        }
    }
}
