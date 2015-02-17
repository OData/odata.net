//---------------------------------------------------------------------
// <copyright file="AddRootServiceOperationsFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Common;

    /// <summary>
    /// Entity model fixup for adding a service operation for each base entity type
    /// </summary>
    public class AddRootServiceOperationsFixup : IEntityModelFixup
    {
        /// <summary>
        /// Build function body to return all entities of given entity type.
        /// </summary>
        /// <param name="entityTypeName">the entity type name</param>
        /// <returns>FunctionBodyAnnotation  respresenting the function body</returns>
        public static FunctionBodyAnnotation BuildReturnEntitySetFunctionBody(string entityTypeName)
        {
            return new FunctionBodyAnnotation
            {
                IsRoot = true,
                FunctionBodyGenerator = (schema) =>
                {
                    var entitySet = schema.GetDefaultEntityContainer().EntitySets.Single(es => es.EntityType.Name.Equals(entityTypeName));
                    return CommonQueryBuilder.Root(entitySet);
                },
            };
        }

        /// <summary>
        /// Adds a Function for each base entity type, representing a service operation
        /// </summary>
        /// <param name="model">the entity model</param>
        public void Fixup(EntityModelSchema model)
        {
            foreach (var entityType in model.EntityTypes.Where(type => type.BaseType == null))
            {
                var entityName = entityType.Name;

                model.Add(
                    new Function("Get" + entityName)
                    {
                        ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName(entityName)),
                        Annotations = 
                        {
                            new LegacyServiceOperationAnnotation 
                            { 
                                Method = HttpVerb.Get,
                                ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                            },
                            AddRootServiceOperationsFixup.BuildReturnEntitySetFunctionBody(entityName),
                        },
                    });
            }
        }
    }
}
