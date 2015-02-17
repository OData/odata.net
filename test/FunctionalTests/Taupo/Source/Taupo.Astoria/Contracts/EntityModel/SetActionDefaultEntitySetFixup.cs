//---------------------------------------------------------------------
// <copyright file="SetActionDefaultEntitySetFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Fixup for adding default EntitySet actions
    /// </summary>
    public class SetActionDefaultEntitySetFixup : IEntityModelFixup
    {
        /// <summary>
        /// Adds the EntitySet information to an action or validates that it has an entitySet Path
        /// </summary>
        /// <param name="model">The model to fix up</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (Function f in model.Functions.Where(f => f.IsAction() && f.ReturnType != null))
            {
                var serviceOperationAnnotation = f.Annotations.OfType<ServiceOperationAnnotation>().Single();

                // If someone has set the entitySetPath or the entitySet name then we don't need to update anything
                if (serviceOperationAnnotation.EntitySetPath != null || serviceOperationAnnotation.EntitySetName != null)
                {
                    continue;
                }

                EntityDataType entityDataType = f.ReturnType as EntityDataType;
                var collectionDataType = f.ReturnType as CollectionDataType;
                if (collectionDataType != null)
                {
                    entityDataType = collectionDataType.ElementDataType as EntityDataType;
                }

                if (entityDataType != null)
                {
                    var possibleMatch = model.EntityContainers.Single().EntitySets.Where(es => entityDataType.Definition.IsKindOf(es.EntityType)).ToList();
                    ExceptionUtilities.Assert(possibleMatch.Count == 1, string.Format(CultureInfo.InvariantCulture, "Cannot resolve function '{0}' to one EntitySet, possible matches were '{1}'", f.Name, string.Join(",", possibleMatch.Select(es => es.Name))));
                    serviceOperationAnnotation.EntitySetName = possibleMatch.Single().Name;
                }
            }
        }
    }
}