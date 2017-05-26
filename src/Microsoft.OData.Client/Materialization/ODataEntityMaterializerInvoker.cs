//---------------------------------------------------------------------
// <copyright file="ODataEntityMaterializerInvoker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Use this class to invoke projection methods from <see cref="ODataEntityMaterializer"/>.
    /// </summary>
    internal static class ODataEntityMaterializerInvoker
    {
        /// <summary>Enumerates casting each element to a type.</summary>
        /// <typeparam name="T">Element type to enumerate over.</typeparam>
        /// <param name="source">Element source.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; that iterates over the specified <paramref name="source"/>.
        /// </returns>
        /// <remarks>
        /// This method should be unnecessary with .NET 4.0 covariance support.
        /// </remarks>
        internal static IEnumerable<T> EnumerateAsElementType<T>(IEnumerable source)
        {
            return ODataEntityMaterializer.EnumerateAsElementType<T>(source);
        }

        /// <summary>Creates a list to a target element type.</summary>
        /// <param name="materializer">Materializer used to flow link tracking.</param>
        /// <typeparam name="T">Element type to enumerate over.</typeparam>
        /// <typeparam name="TTarget">Element type for list.</typeparam>
        /// <param name="source">Element source.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; that iterates over the specified <paramref name="source"/>.
        /// </returns>
        /// <remarks>
        /// This method should be unnecessary with .NET 4.0 covariance support.
        /// </remarks>
        internal static List<TTarget> ListAsElementType<T, TTarget>(object materializer, IEnumerable<T> source) where T : TTarget
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            return ODataEntityMaterializer.ListAsElementType<T, TTarget>((ODataEntityMaterializer)materializer, source);
        }

        /// <summary>Checks whether the entity on the specified <paramref name="path"/> is null.</summary>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="path">Path to pull value for.</param>
        /// <returns>Whether the specified <paramref name="path"/> is null.</returns>
        /// <remarks>
        /// This method will not instantiate entity types on the path.
        /// </remarks>
        internal static bool ProjectionCheckValueForPathIsNull(
            object entry,
            Type expectedType,
            object path)
        {
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return ODataEntityMaterializer.ProjectionCheckValueForPathIsNull(MaterializerEntry.GetEntry((ODataResource)entry), expectedType, (ProjectionPath)path);
        }

        /// <summary>Provides support for Select invocations for projections.</summary>
        /// <param name="materializer">Materializer under which projection is taking place.</param>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="resultType">Expected result type.</param>
        /// <param name="path">Path to traverse.</param>
        /// <param name="selector">Selector callback.</param>
        /// <returns>An enumerable with the select results.</returns>
        internal static IEnumerable ProjectionSelect(
            object materializer,
            object entry,
            Type expectedType,
            Type resultType,
            object path,
            Func<object, object, Type, object> selector)
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return ODataEntityMaterializer.ProjectionSelect((ODataEntityMaterializer)materializer, MaterializerEntry.GetEntry((ODataResource)entry), expectedType, resultType, (ProjectionPath)path, selector);
        }

        /// <summary>Provides support for getting payload entries during projections.</summary>
        /// <param name="entry">Entry to get sub-entry from.</param>
        /// <param name="name">Name of sub-entry.</param>
        /// <returns>The sub-entry (never null).</returns>
        internal static object ProjectionGetEntry(object entry, string name)
        {
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            return ODataEntityMaterializer.ProjectionGetEntry(MaterializerEntry.GetEntry((ODataResource)entry), name);
        }

        /// <summary>Initializes a projection-driven entry (with a specific type and specific properties).</summary>
        /// <param name="materializer">Materializer under which projection is taking place.</param>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="resultType">Expected result type.</param>
        /// <param name="properties">Properties to materialize.</param>
        /// <param name="propertyValues">Functions to get values for functions.</param>
        /// <returns>The initialized entry.</returns>
        internal static object ProjectionInitializeEntity(
            object materializer,
            object entry,
            Type expectedType,
            Type resultType,
            string[] properties,
            Func<object, object, Type, object>[] propertyValues)
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            return ODataEntityMaterializer.ProjectionInitializeEntity((ODataEntityMaterializer)materializer, MaterializerEntry.GetEntry((ODataResource)entry), expectedType, resultType, properties, propertyValues);
        }

        /// <summary>Projects a simple value from the specified <paramref name="path"/>.</summary>
        /// <param name="materializer">Materializer under which projection is taking place.</param>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="path">Path to pull value for.</param>
        /// <returns>The value for the specified <paramref name="path"/>.</returns>
        /// <remarks>
        /// This method will not instantiate entity types, except to satisfy requests
        /// for payload-driven feeds or leaf entities.
        /// </remarks>
        internal static object ProjectionValueForPath(object materializer, object entry, Type expectedType, object path)
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return ((ODataEntityMaterializer)materializer).ProjectionValueForPath(MaterializerEntry.GetEntry((ODataResource)entry), expectedType, (ProjectionPath)path);
        }

        /// <summary>Materializes an entry with no special selection.</summary>
        /// <param name="materializer">Materializer under which materialization should take place.</param>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="expectedEntryType">Expected type for the entry.</param>
        /// <returns>The materialized instance.</returns>
        internal static object DirectMaterializePlan(object materializer, object entry, Type expectedEntryType)
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            return ODataEntityMaterializer.DirectMaterializePlan((ODataEntityMaterializer)materializer, MaterializerEntry.GetEntry((ODataResource)entry), expectedEntryType);
        }

        /// <summary>Materializes an entry without including in-lined expanded links.</summary>
        /// <param name="materializer">Materializer under which materialization should take place.</param>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="expectedEntryType">Expected type for the entry.</param>
        /// <returns>The materialized instance.</returns>
        internal static object ShallowMaterializePlan(object materializer, object entry, Type expectedEntryType)
        {
            Debug.Assert(typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType()), "typeof(ODataEntityMaterializer).IsAssignableFrom(materializer.GetType())");
            Debug.Assert(entry.GetType() == typeof(ODataResource), "entry.GetType() == typeof(ODataResource)");
            return ODataEntityMaterializer.ShallowMaterializePlan((ODataEntityMaterializer)materializer, MaterializerEntry.GetEntry((ODataResource)entry), expectedEntryType);
        }
    }
}
