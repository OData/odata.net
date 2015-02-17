//---------------------------------------------------------------------
// <copyright file="IEntityModelObjectServicesFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Entity model object services factory
    /// </summary>
    [ImplementationSelector("EntityModelObjectServicesFactory", DefaultImplementation = "Default")] 
    public interface IEntityModelObjectServicesFactory
    {
        /// <summary>
        /// Creates the object services for the specified entity model schema.
        /// </summary>
        /// <param name="modelSchema">The entity model schema.</param>
        /// <param name="conceptualDataServices">The conceptual data services.</param>
        /// <param name="assemblies">The assemblies that contain data classes for the specified entity model schema.</param>
        /// <returns>Entity model object services.</returns>
        IEntityModelObjectServices CreateObjectServices(EntityModelSchema modelSchema, IEntityModelConceptualDataServices conceptualDataServices, IEnumerable<Assembly> assemblies);
    }
}
