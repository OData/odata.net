//---------------------------------------------------------------------
// <copyright file="StubEdmEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of EdmEntityContainer
    /// </summary>
    public class StubEdmEntityContainer : StubEdmElement, IEdmEntityContainer
    {
        private List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();

        /// <summary>
        /// Initializes a new instance of the StubEdmEntityContainer class.
        /// </summary>
        /// <param name="namespaceName">The namespace of the container</param>
        /// <param name="name">The name of the container</param>
        public StubEdmEntityContainer(string namespaceName, string name)
        {
            this.Namespace = namespaceName;
            this.Name = name;
        }

        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        /// <summary>
        /// Gets the elements
        /// </summary>
        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Finds an element by name
        /// </summary>
        /// <param name="setName">the name of the element</param>
        /// <returns>the element.</returns>
        public IEdmEntitySet FindEntitySet(string setName)
        {
            if (setName == null)
            {
                throw new ArgumentNullException("setName");
            }

            return (IEdmEntitySet)this.elements.FirstOrDefault(e => (e.Name == setName && e.ContainerElementKind == EdmContainerElementKind.EntitySet));
        }

        /// <summary>
        /// Searches for a singleton with the given name in this entity container and returns null if no such singleton exists.
        /// </summary>
        /// <param name="singletonName">The name of the singleton to search.</param>
        /// <returns>The requested singleton, or null if the singleton does not exist.</returns>
        public IEdmSingleton FindSingleton(string singletonName)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Searches for operation imports with the given name in this entity container and returns null if no such operation import exists.
        /// </summary>
        /// <param name="operationName">The name of the operation import being found.</param>
        /// <returns>A group of the requested operation imports, or null if no such operation import exists.</returns>
        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            if (operationName == null)
            {
                throw new ArgumentNullException("operationName");
            }

            return this.elements.Where(e => (e.Name == operationName && (e.ContainerElementKind == EdmContainerElementKind.ActionImport || e.ContainerElementKind == EdmContainerElementKind.FunctionImport))).Cast<IEdmOperationImport>();
        }

        /// <summary>
        /// Adds an element
        /// </summary>
        /// <param name="element">The element to add</param>
        public void Add(IEdmEntityContainerElement element)
        {
            this.elements.Add(element);
        }
    }
}
