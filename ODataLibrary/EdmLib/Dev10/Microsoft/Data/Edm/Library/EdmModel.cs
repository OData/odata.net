//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM model.
    /// </summary>
    public class EdmModel : EdmModelBase
    {
        private readonly List<IEdmSchemaElement> elements = new List<IEdmSchemaElement>();

        /// <summary>
        /// Initializes a new instance of the EdmModel class.
        /// </summary>
        public EdmModel()
        {
        }

        /// <summary>
        /// Adds a schema element to this model.
        /// </summary>
        /// <param name="element">Element to be added.</param>
        public void AddElement(IEdmSchemaElement element)
        {
            this.elements.Add(element);
            this.RegisterElement(element);
        }

        /// <summary>
        /// Adds a collection of schema elements to this model.
        /// </summary>
        /// <param name="newElements">Elements to be added.</param>
        public void AddElements(IEnumerable<IEdmSchemaElement> newElements)
        {
            EdmUtil.CheckArgumentNull(newElements, "newElements");
            foreach (IEdmSchemaElement element in newElements)
            {
                this.AddElement(element);
            }
        }

        /// <summary>
        /// Removes a schema element from this model.
        /// </summary>
        /// <param name="element">Element to be removed.</param>
        public void RemoveElement(IEdmSchemaElement element)
        {
            IEdmEntityType entityElement = element as IEdmEntityType;
            if (entityElement != null)
            {
                foreach (IEdmEntityContainer container in EntityContainers)
                {
                    EdmEntityContainer mutableContainer = container as EdmEntityContainer;
                    if (mutableContainer != null)
                    {
                        foreach (IEdmEntityContainerElement containerElement in container.Elements)
                        {
                            // We can only deal with mutable Sets. If this invalidates an immutable set, it will have to be checked during validation.
                            EdmEntitySet entitySet = containerElement as EdmEntitySet;
                            if (entitySet != null)
                            {
                                if (entitySet.ElementType == element)
                                {
                                    entitySet.ElementType = new BadEntityType(element.FullName(), new EdmError[]{new EdmError(null, EdmErrorCode.ConstructibleEntitySetTypeInvalidFromEntityTypeRemoval, Edm.Strings.Constructable_EntitySetTypeInvalidFromEntityTypeRemoval(entitySet.Name))});
                                }
                            }
                        }
                    }
                }
            }

            IDependent dependent = element as IDependent;
            if (dependent != null)
            {
                dependent.RespondToDependency(new HashSetInternal<IDependent>());
            }

            this.elements.Remove(element);
            UnregisterElement(element);
        }

        /// <summary>
        /// Removes an entity container from this model.
        /// </summary>
        /// <param name="container">Container to be removed</param>
        public void RemoveEntityContainer(IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            EntityContainersList.Remove(container);
            RegistrationHelper.RemoveElement(container, container.Name, this.ContainersDictionary);
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model.
        /// </summary>
        public override IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get
            {
                List<IEdmAssociation> associations = new List<IEdmAssociation>();

                foreach (IEdmSchemaElement element in this.elements)
                {
                    yield return element;
                    EdmEntityType entity = element as EdmEntityType;
                    if (entity != null)
                    {
                        foreach (IEdmNavigationProperty property in entity.DeclaredNavigationProperties())
                        {
                            EdmNavigationProperty navigation = property as EdmNavigationProperty;
                            if (navigation != null && navigation.IsFromPrincipal)
                            {
                                associations.Add(((IEdmNavigationProperty)navigation).To.DeclaringAssociation);
                            }
                        }
                    }
                }

                // Return associations after everything else.
                foreach (IEdmAssociation association in associations)
                {
                    yield return association;
                }
            }
        }
    }
}
