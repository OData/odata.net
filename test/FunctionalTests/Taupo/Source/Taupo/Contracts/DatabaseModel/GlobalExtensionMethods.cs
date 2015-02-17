//---------------------------------------------------------------------
// <copyright file="GlobalExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.DatabaseModel;

    /// <summary>
    /// Global extension methods to simplify usage of DatabaseModel classes.
    /// </summary>
    /// <remarks>
    /// The class is declared within contracts to reduce the number of using statements that
    /// are necessary in user code. Layering test verifies that those methods can be called only 
    /// from the user code but not from Taupo code.
    /// </remarks>
    public static class GlobalExtensionMethods
    {
        /// <summary>
        /// Resolves named references within the schema.
        /// </summary>
        /// <param name="schema">Schema to resolve.</param>
        /// <returns>This instance (can be used to chain calls together)</returns>
        public static DatabaseSchema Resolve(this DatabaseSchema schema)
        {
            var fixup = new ResolveDatabaseModelReferencesFixup();
            fixup.Fixup(schema);
            return schema;
        }

        /// <summary>
        /// Sets the data generator for the given column.
        /// </summary>
        /// <param name="column">The column to set the data generator for.</param>
        /// <param name="dataGenerator">The data generator.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static Column WithDataGenerator(this Column column, IDataGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(column, "column");

            return column.WithDataGeneratorInternal(dataGenerator);
        }

        /// <summary>
        /// Sets the data generator for the given store function parameter.
        /// </summary>
        /// <param name="functionParameter">The database function parameter to set the data generator for.</param>
        /// <param name="dataGenerator">The data generator.</param>
        /// <returns>This instance (can be used to chain calls together).</returns>
        public static DatabaseFunctionParameter WithDataGenerator(this DatabaseFunctionParameter functionParameter, IDataGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionParameter, "functionParameter");

            return functionParameter.WithDataGeneratorInternal(dataGenerator);
        }

        private static TItem WithDataGeneratorInternal<TItem>(this TItem annotatedItem, IDataGenerator dataGenerator) where TItem : IAnnotatedStoreItem
        {
            ExceptionUtilities.CheckArgumentNotNull(dataGenerator, "dataGenerator");

            var dataGenAnnotation = annotatedItem.Annotations.OfType<StoreDataGeneratorAnnotation>().SingleOrDefault();
            if (dataGenAnnotation != null)
            {
                annotatedItem.Annotations.Remove(dataGenAnnotation);
            }

            annotatedItem.Annotations.Add(new StoreDataGeneratorAnnotation(dataGenerator));

            return annotatedItem;
        }
    }
}
