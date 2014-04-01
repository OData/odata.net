//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM entity container.
    /// </summary>
    public class EdmEntityContainer : EdmElement, IEdmEntityContainer
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly List<IEdmEntityContainerElement> containerElements = new List<IEdmEntityContainerElement>();
        private readonly Dictionary<string, IEdmEntitySet> entitySetDictionary = new Dictionary<string, IEdmEntitySet>();
        private readonly Dictionary<string, object> functionImportDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityContainer"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the entity container.</param>
        /// <param name="name">Name of the entity container.</param>
        public EdmEntityContainer(string namespaceName, string name)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(name, "name");

            this.namespaceName = namespaceName;
            this.name = name;
        }

        /// <summary>
        /// Gets a collection of the elements of this entity container.
        /// </summary>
        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return this.containerElements; }
        }

        /// <summary>
        /// Gets the namespace of this entity container.
        /// </summary>
        public string Namespace
        {
            get { return this.namespaceName; }
        }

        /// <summary>
        /// Gets the name of this entity container.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }
        
        /// <summary>
        /// Adds an entity container element to this entity container.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void AddElement(IEdmEntityContainerElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");

            this.containerElements.Add(element);
            
            switch (element.ContainerElementKind)
            {
                case EdmContainerElementKind.EntitySet:
                    RegistrationHelper.AddElement((IEdmEntitySet)element, element.Name, this.entitySetDictionary, RegistrationHelper.CreateAmbiguousEntitySetBinding);
                    break;
                case EdmContainerElementKind.OperationImport:
                    RegistrationHelper.AddOperation((IEdmOperationImport)element, element.Name, this.functionImportDictionary);
                    break;
                case EdmContainerElementKind.None:
                    throw new InvalidOperationException(Edm.Strings.EdmEntityContainer_CannotUseElementWithTypeNone);
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind));
            }
        }

        /// <summary>
        /// Creates and adds an entity set to this entity container.
        /// </summary>
        /// <param name="name">Name of the entity set.</param>
        /// <param name="elementType">The entity type of the elements in this entity set.</param>
        /// <returns>Created entity set.</returns>
        public virtual EdmEntitySet AddEntitySet(string name, IEdmEntityType elementType)
        {
            EdmEntitySet entitySet = new EdmEntitySet(this, name, elementType);
            this.AddElement(entitySet);
            return entitySet;
        }

        /// <summary>
        /// Creates and adds a function import to this entity container.
        /// </summary>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <returns>Created function import.</returns>
        public virtual EdmFunctionImport AddFunctionImport(string name, IEdmTypeReference returnType)
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(this, name, returnType);
            this.AddElement(functionImport);
            return functionImport;
        }

        /// <summary>
        /// Creates and adds a action import to this entity container.
        /// </summary>
        /// <param name="name">Name of the action import.</param>
        /// <param name="returnType">Return type of the action import.</param>
        /// <returns>Created action import.</returns>
        public virtual EdmActionImport AddActionImport(string name, IEdmTypeReference returnType)
        {
            EdmActionImport actionImport = new EdmActionImport(this, name, returnType);
            this.AddElement(actionImport);
            return actionImport;
        }

        /// <summary>
        /// Creates and adds a function import to this entity container.
        /// </summary>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <returns>Created function import.</returns>
        public virtual EdmFunctionImport AddFunctionImport(string name, IEdmTypeReference returnType, IEdmExpression entitySet)
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(this, name, returnType, entitySet);
            this.AddElement(functionImport);
            return functionImport;
        }

        /// <summary>
        /// Creates and adds an action import to this entity container.
        /// </summary>
        /// <param name="name">Name of the action import.</param>
        /// <param name="returnType">Return type of the action import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this action import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <returns>Created action import.</returns>
        public virtual EdmActionImport AddActionImport(string name, IEdmTypeReference returnType, IEdmExpression entitySet)
        {
            EdmActionImport actionImport = new EdmActionImport(this, name, returnType, entitySet);
            this.AddElement(actionImport);
            return actionImport;
        }

        /// <summary>
        /// Creates and adds a function import to this entity container.
        /// </summary>
        /// <param name="name">Name of the function import.</param>
        /// <param name="returnType">Return type of the function import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this function import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <param name="composable">A value indicating whether this function import can be composed inside expressions.</param>
        /// <param name="bindable">A value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.</param>
        /// <returns>Created operation import.</returns>
        public virtual EdmOperationImport AddFunctionImport(string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool composable, bool bindable)
        {
            EdmOperationImport functionImport = new EdmFunctionImport(this, name, returnType, entitySet, bindable, composable);
            this.AddElement(functionImport);
            return functionImport;
        }

        /// <summary>
        /// Creates and adds an action import to this entity container.
        /// </summary>
        /// <param name="name">Name of the action import.</param>
        /// <param name="returnType">Return type of the action import.</param>
        /// <param name="entitySet">An entity set containing entities returned by this action import. 
        /// The two expression kinds supported are <see cref="IEdmEntitySetReferenceExpression"/> and <see cref="IEdmPathExpression"/>.</param>
        /// <param name="bindable">A value indicating whether this action import can be used as an extension method for the type of the first parameter of this action import.</param>
        /// <returns>Created action import.</returns>
        public virtual EdmActionImport AddActionImport(string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool bindable)
        {
            EdmActionImport actionImport = new EdmActionImport(this, name, returnType, entitySet, bindable);
            this.AddElement(actionImport);
            return actionImport;
        }

        /// <summary>
        /// Searches for an entity set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="setName">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        public virtual IEdmEntitySet FindEntitySet(string setName)
        {
            IEdmEntitySet element;
            return this.entitySetDictionary.TryGetValue(setName, out element) ? element : null;
        }

        /// <summary>
        /// Searches for operation imports with the given name in this entity container and returns null if no such operation import exists.
        /// </summary>
        /// <param name="operationName">The name of the operation to find.</param>
        /// <returns>A group of the requested operation imports, or an empty enumerable if no such operation import exists.</returns>
        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            object element;
            if (this.functionImportDictionary.TryGetValue(operationName, out element))
            {
                List<IEdmOperationImport> listElement = element as List<IEdmOperationImport>;
                if (listElement != null)
                {
                    return listElement;
                }

                return new IEdmOperationImport[] { (IEdmOperationImport)element };
            }

            return Enumerable.Empty<IEdmOperationImport>();
        }
    }
}
