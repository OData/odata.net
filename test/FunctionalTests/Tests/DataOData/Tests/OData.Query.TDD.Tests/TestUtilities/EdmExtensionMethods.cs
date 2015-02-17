//---------------------------------------------------------------------
// <copyright file="EdmExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.TestUtilities
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extension methods used for test work building on EdmLib types.
    /// </summary>
    public static class EdmExtensionMethods
    {
        /// <summary>
        /// Returns all the entity types in a model.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to get the entity types for (must not be null).</param>
        /// <returns>An enumerable of all <see cref="IEdmEntityType"/> instances in the <paramref name="model"/>.</returns>
        internal static IEnumerable<IEdmEntityType> EntityTypes(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
            if (schemaElements != null)
            {
                return schemaElements.OfType<IEdmEntityType>();
            }

            return null;
        }
    }
}
