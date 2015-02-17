//---------------------------------------------------------------------
// <copyright file="MakeNamesUniqueFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Entity model fixup for making the ContainerName, EntitySetNames and Namespace names unique in order to track down IIS issue with StaticState persistance
    /// </summary>
    public class MakeNamesUniqueFixup : IEntityModelFixup
    {    
        /// <summary>
        /// Initializes a new instance of the MakeNamesUniqueFixup class that chooses the first container if more than one are present
        /// </summary>
        public MakeNamesUniqueFixup()
        {
            this.MakeContainerNameUnique = false;
            this.MakeEntitySetsUnique = false;
            this.MakeNamespacesUnique = false;            
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to fuzz the Container name
        /// </summary>
        [InjectTestParameter("MakeContainerNameUnique")]
        public bool MakeContainerNameUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to fuzz the EntitySet names
        /// </summary>
        [InjectTestParameter("MakeEntitySetsUnique")]
        public bool MakeEntitySetsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to fuzz the NameSpaces
        /// </summary>
        [InjectTestParameter("MakeNamespacesUnique")]
        public bool MakeNamespacesUnique { get; set; }

        /// <summary>
        /// Apply the fixup to the first container
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            // Make the container name unique
            EntityContainer container = model.EntityContainers.FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(container, "Expected to find a container");

            if (this.MakeContainerNameUnique)
            {
                container.Name = container.Name + "_" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(16);
            }

            // Make the EntitySet name unique
            if (this.MakeEntitySetsUnique)
            { 
                foreach (EntitySet es in container.EntitySets)
                {
                    // Do something to the names
                    es.Name = es.Name + "_" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(16);
                }
            }

            // Make the namespace unique
            if (this.MakeNamespacesUnique)
            {
                string namespaceGuid = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(16);
                foreach (INamedItem item in this.GetAllNamedItems(model))
                {                    
                    if (item.NamespaceName == null)
                    {
                        item.NamespaceName = "DefaultNamespace";
                    }

                    item.NamespaceName += namespaceGuid; 
                }
            }            
        }

        private IEnumerable<INamedItem> GetAllNamedItems(EntityModelSchema model)
        {
            return model.EntityTypes.Cast<INamedItem>()
                .Concat(model.ComplexTypes.Cast<INamedItem>())
                .Concat(model.Associations.Cast<INamedItem>())
                .Concat(model.Functions.Cast<INamedItem>())
                .Concat(model.EnumTypes.Cast<INamedItem>());
        }
    }
}