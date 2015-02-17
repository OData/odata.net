//---------------------------------------------------------------------
// <copyright file="SimpleModelDescription.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    #region Simple model description.

    public class ResourceContainerList : List<SimpleResourceContainer>
    {
        public ResourceContainerList() : base() { }
        public ResourceContainerList(IEnumerable<SimpleResourceContainer> collection) : base(collection) { }
        public SimpleResourceContainer this[string name]
        {
            get { return this.Where(container => container.Name == name).Single(); }
        }
    }

    public class SimpleProperty
    {
        public SimpleProperty(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }

    public class SimpleResourceContainer
    {
        public SimpleResourceContainer() { }
        public SimpleResourceContainer(string name, IEnumerable<SimpleResourceType> types)
        {
            this.Name = name;
            this.ResourceTypes = new List<SimpleResourceType>(types);
        }

        public string Name { get; set; }
        public List<SimpleResourceType> ResourceTypes { get; set; }
    }

    public class SimpleServiceContainer
    {
        public string Name { get; set; }
        public ResourceContainerList ResourceContainers { get; set; }
    }

    public class SimpleResourceType
    {
        public SimpleResourceType(string name, IEnumerable<SimpleProperty> properties)
        {
            this.Name = name;
            this.Properties = new List<SimpleProperty>(properties);
        }

        public string Name { get; set; }
        public List<SimpleProperty> Properties { get; set; }
    }

    public class SimpleWorkspace
    {
        public string ServiceEndPoint { get; set; }
        public SimpleServiceContainer ServiceContainer { get; set; }
    }

    #endregion Simple model description.
}