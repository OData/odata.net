//---------------------------------------------------------------------
// <copyright file="RemoveNavigationPropertiesFromDerivedTypesFixup.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// Entity model fixup for removing navigation properties defined on derived types
    /// </summary>
    public class RemoveNavigationPropertiesFromDerivedTypesFixup : IEntityModelFixup
    {
        /// <summary>
        /// Initializes a new instance of the RemoveNavigationPropertiesFromDerivedTypesFixup class
        /// </summary>
        public RemoveNavigationPropertiesFromDerivedTypesFixup()
        {
        }

        /// <summary>
        /// Remove navigation properties defined on derived types
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var et in model.EntityTypes)
            {
                if (et.BaseType == null)
                {
                    continue;
                }

                // ToList is to avoid modifying the collection while enumerating
                foreach (var property in et.NavigationProperties.Where(n => !et.BaseType.AllNavigationProperties.Contains(n)).ToList())
                {
                    et.NavigationProperties.Remove(property);
                }
            }
        }
    }
}