//---------------------------------------------------------------------
// <copyright file="ODataEntityMaterializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Used to materialize entities from a <see cref="ODataResource"/> objects.
    /// </summary>
    internal abstract class ODataEntityMaterializer : ODataMaterializer
    {
        /// <summary>The value of the current materialized entity.</summary>
        protected object currentValue;

        /// <summary>The materializer plan.</summary>
        private readonly ProjectionPlan materializeEntryPlan;

        /// <summary> The entry value materializer policy. </summary>
        private readonly EntryValueMaterializationPolicy entryValueMaterializationPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataEntityMaterializer" /> class.
        /// </summary>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="entityTrackingAdapter">The entity tracking adapter.</param>
        /// <param name="queryComponents">The query components.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="materializeEntryPlan">The materialize entry plan.</param>
        public ODataEntityMaterializer(
            IODataMaterializerContext materializerContext,
            EntityTrackingAdapter entityTrackingAdapter,
            QueryComponents queryComponents,
            Type expectedType,
            ProjectionPlan materializeEntryPlan)
            : base(materializerContext, expectedType)
        {
            this.materializeEntryPlan = materializeEntryPlan ?? CreatePlan(queryComponents);
            this.EntityTrackingAdapter = entityTrackingAdapter;
            DSClient.SimpleLazy<PrimitivePropertyConverter> converter = new DSClient.SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter());

            this.entryValueMaterializationPolicy = new EntryValueMaterializationPolicy(this.MaterializerContext, this.EntityTrackingAdapter, converter, nextLinkTable);
            this.entryValueMaterializationPolicy.CollectionValueMaterializationPolicy = this.CollectionValueMaterializationPolicy;
            this.entryValueMaterializationPolicy.InstanceAnnotationMaterializationPolicy = this.InstanceAnnotationMaterializationPolicy;
        }

        /// <summary>
        /// Gets the Entity Materializer Context
        /// </summary>
        internal EntityTrackingAdapter EntityTrackingAdapter { get; private set; }

        /// <summary>
        /// Target instance that the materializer expects to update.
        /// </summary>
        internal object TargetInstance
        {
            get
            {
                return this.EntityTrackingAdapter.TargetInstance;
            }

            set
            {
                Debug.Assert(value != null, "value != null -- otherwise we have no instance target.");
                this.EntityTrackingAdapter.TargetInstance = value;
            }
        }

        /// <summary>
        /// Current value being materialized; possibly null.
        /// </summary>
        internal sealed override object CurrentValue
        {
            get { return this.currentValue; }
        }

        /// <summary>
        /// Function to materialize an entry and produce a value.
        /// </summary>
        internal sealed override ProjectionPlan MaterializeEntryPlan
        {
            get { return this.materializeEntryPlan; }
        }

        /// <summary>
        /// Gets the entry value materialization policy.
        /// </summary>
        /// <value>
        /// The entry value materialization policy.
        /// </value>
        protected EntryValueMaterializationPolicy EntryValueMaterializationPolicy
        {
            get { return this.entryValueMaterializationPolicy; }
        }

        #region Projection support.

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
            Debug.Assert(source != null, "source != null");

            IEnumerable<T> typedSource = source as IEnumerable<T>;
            if (typedSource != null)
            {
                return typedSource;
            }
            else
            {
                return EnumerateAsElementTypeInternal<T>(source);
            }
        }

        /// <summary>Enumerates casting each element to a type.</summary>
        /// <typeparam name="T">Element type to enumerate over.</typeparam>
        /// <param name="source">Element source.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; that iterates over the specified <paramref name="source"/>.
        /// </returns>
        /// <remarks>
        /// This method should be unnecessary with .NET 4.0 covariance support.
        /// </remarks>
        internal static IEnumerable<T> EnumerateAsElementTypeInternal<T>(IEnumerable source)
        {
            Debug.Assert(source != null, "source != null");

            foreach (object item in source)
            {
                yield return (T)item;
            }
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
        internal static List<TTarget> ListAsElementType<T, TTarget>(ODataEntityMaterializer materializer, IEnumerable<T> source) where T : TTarget
        {
            Debug.Assert(materializer != null, "materializer != null");
            Debug.Assert(source != null, "source != null");

            List<TTarget> typedSource = source as List<TTarget>;
            if (typedSource != null)
            {
                return typedSource;
            }

            List<TTarget> list;
            IList sourceList = source as IList;
            if (sourceList != null)
            {
                list = new List<TTarget>(sourceList.Count);
            }
            else
            {
                list = new List<TTarget>();
            }

            foreach (T item in source)
            {
                list.Add((TTarget)item);
            }

            // We can flow the same continuation becaues they're immutable, and
            // we don't need to set the continuation property because List<T> doesn't
            // have one.
            DataServiceQueryContinuation continuation;
            if (materializer.nextLinkTable.TryGetValue(source, out continuation))
            {
                materializer.nextLinkTable[list] = continuation;
            }

            return list;
        }

        /// <summary>Creates an entry materialization plan that is payload-driven.</summary>
        /// <param name="lastSegmentType">Segment type for the entry to materialize (typically last of URI in query).</param>
        /// <returns>A payload-driven materialization plan.</returns>
        internal static ProjectionPlan CreatePlanForDirectMaterialization(Type lastSegmentType)
        {
            ProjectionPlan result = new ProjectionPlan();
            result.Plan = ODataEntityMaterializerInvoker.DirectMaterializePlan;
            result.ProjectedType = lastSegmentType;
            result.LastSegmentType = lastSegmentType;
            return result;
        }

        /// <summary>Creates an entry materialization plan that is payload-driven and does not traverse expanded links.</summary>
        /// <param name="lastSegmentType">Segment type for the entry to materialize (typically last of URI in query).</param>
        /// <returns>A payload-driven materialization plan.</returns>
        internal static ProjectionPlan CreatePlanForShallowMaterialization(Type lastSegmentType)
        {
            ProjectionPlan result = new ProjectionPlan();
            result.Plan = ODataEntityMaterializerInvoker.ShallowMaterializePlan;
            result.ProjectedType = lastSegmentType;
            result.LastSegmentType = lastSegmentType;
            return result;
        }

        /// <summary>Checks whether the entity on the specified <paramref name="path"/> is null.</summary>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="path">Path to pull value for.</param>
        /// <returns>Whether the specified <paramref name="path"/> is null.</returns>
        /// <remarks>
        /// This method will not instantiate entity types on the path.
        /// Note that if the target is a collection, the result is always false,
        /// as the model does not allow null feeds (but instead gets an empty
        /// collection, possibly with continuation tokens and such).
        /// </remarks>
        internal static bool ProjectionCheckValueForPathIsNull(
            MaterializerEntry entry,
            Type expectedType,
            ProjectionPath path)
        {
            Debug.Assert(path != null, "path != null");

            if (path.Count == 0 || path.Count == 1 && path[0].Member == null)
            {
                return entry.Entry == null;
            }

            bool result = false;
            MaterializerNavigationLink atomProperty = default(MaterializerNavigationLink);
            IEnumerable<ODataNestedResourceInfo> properties = entry.NestedResourceInfos;
            ClientEdmModel model = entry.EntityDescriptor.Model;
            for (int i = 0; i < path.Count; i++)
            {
                var segment = path[i];
                if (segment.Member == null)
                {
                    continue;
                }

                bool segmentIsLeaf = i == path.Count - 1;
                string propertyName = segment.Member;

                if (segment.SourceTypeAs != null)
                {
                    // (p as Employee).Manager
                    // The property might not be defined on the expectedType but is always defined on the TypeAs type which is a more derived type.
                    expectedType = segment.SourceTypeAs;

                    if (!properties.Any(p => p.Name == propertyName))
                    {
                        // We are projecting a property defined on a derived type and the entry is of the base type. The property doesn't exist, return null.
                        return true;
                    }
                }

                IEdmType expectedEdmType = model.GetOrCreateEdmType(expectedType);
                ClientPropertyAnnotation property = model.GetClientTypeAnnotation(expectedEdmType).GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);
                atomProperty = ODataEntityMaterializer.GetPropertyOrThrow(properties, propertyName);
                EntryValueMaterializationPolicy.ValidatePropertyMatch(property, atomProperty.Link);
                if (atomProperty.Feed != null)
                {
                    Debug.Assert(segmentIsLeaf, "segmentIsLeaf -- otherwise the path generated traverses a feed, which should be disallowed");
                    result = false;
                }
                else if (atomProperty.Entry != null)
                {
                    if (segmentIsLeaf)
                    {
                        result = atomProperty.Entry.Entry == null;
                    }
                    else
                    {
                        entry = atomProperty.Entry;
                        properties = entry.NestedResourceInfos;
                    }
                }
                else
                {
                    return true;
                }

                expectedType = property.PropertyType;
            }

            return result;
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
            ODataEntityMaterializer materializer,
            MaterializerEntry entry,
            Type expectedType,
            Type resultType,
            ProjectionPath path,
            Func<object, object, Type, object> selector)
        {
            ClientEdmModel edmModel = materializer.MaterializerContext.Model;
            ClientTypeAnnotation entryType = entry.ActualType ?? edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(expectedType));
            IEnumerable list = (IEnumerable)Util.ActivatorCreateInstance(typeof(List<>).MakeGenericType(resultType));
            MaterializerNavigationLink atomProperty = default(MaterializerNavigationLink);
            ClientPropertyAnnotation property = null;
            for (int i = 0; i < path.Count; i++)
            {
                var segment = path[i];
                if (segment.SourceTypeAs != null)
                {
                    entryType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(segment.SourceTypeAs));
                }

                if (segment.Member == null)
                {
                    continue;
                }

                string propertyName = segment.Member;
                property = entryType.GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);

                // If we are projecting a property defined on a derived type and the entry is of the base type, get property would throw. The user need to check for null in the query.
                // e.g. Select(p => new MyEmployee { ID = p.ID, Manager = (p as Employee).Manager == null ? null : new MyManager { ID = (p as Employee).Manager.ID } })
                atomProperty = ODataEntityMaterializer.GetPropertyOrThrow(entry.NestedResourceInfos, propertyName);

                if (atomProperty.Entry != null)
                {
                    entry = atomProperty.Entry;
                    entryType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(property.PropertyType));
                }
            }

            EntryValueMaterializationPolicy.ValidatePropertyMatch(property, atomProperty.Link);
            MaterializerFeed sourceFeed = MaterializerFeed.GetFeed(atomProperty.Feed);
            Debug.Assert(
                sourceFeed.Feed != null,
                "sourceFeed != null -- otherwise ValidatePropertyMatch should have thrown or property isn't a collection (and should be part of this plan)");

            Action<object, object> addMethod = ClientTypeUtil.GetAddToCollectionDelegate(list.GetType());
            foreach (var paramEntry in sourceFeed.Entries)
            {
                object projected = selector(materializer, paramEntry, property.EntityCollectionItemType /* perhaps nested? */);
                addMethod(list, projected);
            }

            ProjectionPlan plan = new ProjectionPlan();
            plan.LastSegmentType = property.EntityCollectionItemType;
            plan.Plan = selector;
            plan.ProjectedType = resultType;

            materializer.EntryValueMaterializationPolicy.FoundNextLinkForCollection(list, sourceFeed.NextPageLink, plan);

            return list;
        }

        /// <summary>Provides support for getting payload entries during projections.</summary>
        /// <param name="entry">Entry to get sub-entry from.</param>
        /// <param name="name">Name of sub-entry.</param>
        /// <returns>The sub-entry (never null).</returns>
        internal static ODataResource ProjectionGetEntry(MaterializerEntry entry, string name)
        {
            Debug.Assert(entry.Entry != null, "entry != null -- ProjectionGetEntry never returns a null entry, and top-level materialization shouldn't pass one in");

            // If we are projecting a property defined on a derived type and the entry is of the base type, get property would throw. The user need to check for null in the query.
            // e.g. Select(p => new MyEmployee { ID = p.ID, Manager = (p as Employee).Manager == null ? null : new MyManager { ID = (p as Employee).Manager.ID } })
            MaterializerNavigationLink property = ODataEntityMaterializer.GetPropertyOrThrow(entry.NestedResourceInfos, name);
            MaterializerEntry result = property.Entry;
            if (result == null)
            {
                throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_PropertyNotExpectedEntry(name));
            }

            CheckEntryToAccessNotNull(result, name);
            return result.Entry;
        }

        /// <summary>Initializes a projection-driven entry (with a specific type and specific properties).</summary>
        /// <param name="materializer">Materializer under which projection is taking place.</param>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="resultType">Expected result type.</param>
        /// <param name="properties">Properties to materialize.</param>
        /// <param name="propertyValues">Functions to get values for functions.</param>
        /// <returns>The initialized entry.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to throw the type that the expression would throw with other providers")]
        internal static object ProjectionInitializeEntity(
            ODataEntityMaterializer materializer,
            MaterializerEntry entry,
            Type expectedType,
            Type resultType,
            string[] properties,
            Func<object, object, Type, object>[] propertyValues)
        {
            if (entry.Entry == null)
            {
                throw new NullReferenceException(DSClient.Strings.AtomMaterializer_EntryToInitializeIsNull(resultType.FullName));
            }

            if (!entry.EntityHasBeenResolved)
            {
                ODataEntityMaterializer.ProjectionEnsureEntryAvailableOfType(materializer, entry, resultType);
            }
            else if (!resultType.IsAssignableFrom(entry.ActualType.ElementType))
            {
                string message = DSClient.Strings.AtomMaterializer_ProjectEntityTypeMismatch(
                    resultType.FullName,
                    entry.ActualType.ElementType.FullName,
                    entry.Id);
                throw new InvalidOperationException(message);
            }

            object result = entry.ResolvedObject;

            for (int i = 0; i < properties.Length; i++)
            {
                string propertyName = properties[i];

                // We get here if we have an entity member init in the projection. For example Select(p => new MyEmployee { Manager = (p as Employee).Manager }).
                // The entry.ActualType in the example would be MyEmployee and the Manager property always exist on it or else the linq statement would not compile.
                var property = entry.ActualType.GetProperty(propertyName, materializer.MaterializerContext.UndeclaredPropertyBehavior);
                Debug.Assert(property != null, "property != null");

                // NOTE:
                // 1. The delegate calls into methods such as ProjectionValueForPath or ProjectionCheckValueForPathIsNull where the projection path is given. Those methods
                //    will throw if property is missing from the Atom entry and there is no TypeAs before the property access. I.e. if the Manager property is missing in the
                //    Atom entry for (p as Employee).Manager, we return null. But we would throw for e.Manager because we are not accessing a derived property.
                // 2. If Manager is missing in the Atom entry for (p as Employee).Manager.Name, the delegate would throw because we are accessing the Name property on Manager
                //    which is null.  We require user to do a null check, e.g. "(p as Employee).Manager == null ? null (p as Employee).Manager.Name".
                object value = propertyValues[i](materializer, entry.Entry, expectedType);

                // If the property is missing in the Atom entry, we are projecting a derived property and entry is of the base type which the property is not defined on.
                // We don't want to set the property value, which is null, for the non-existing property on the base type.
                // Take the example Select(p => new MyEmployee { Manager = (p as Employee).Manager }), if p is of Person type, the Manager navigation property would not
                // be on its Atom payload from the server. Thus we don't need to set the MyEmployee.Manager link.
                StreamDescriptor streamInfo;
                var odataProperty = entry.Entry.Properties.Where(p => p.Name == propertyName).FirstOrDefault();
                var link = odataProperty == null && entry.NestedResourceInfos != null ? entry.NestedResourceInfos.Where(l => l.Name == propertyName).FirstOrDefault() : null;

                if (link == null && odataProperty == null && !entry.EntityDescriptor.TryGetNamedStreamInfo(propertyName, out streamInfo))
                {
                    continue;
                }

                if (entry.ShouldUpdateFromPayload)
                {
                    if (property.EdmProperty.Type.TypeKind() == EdmTypeKind.Entity)
                    {
                        materializer.EntityTrackingAdapter.MaterializationLog.SetLink(entry, property.PropertyName, value);
                    }

                    if (!property.IsEntityCollection)
                    {
                        // Collection properties cannot be just set like primitive or complex properties. For collectionValue we have a special initialization logic in the
                        // ApplyDataValue (called from ProjectionValueForPath invoked above with: propertyValues[i](materializer, entry, expectedType)) method
                        // that ensures that we either re-use existing collectionValue or create an instance using the right type for the collectionValue.  As a result at this
                        // point the value of the collectionValue must already be set to a non-null value.
                        if (!property.IsPrimitiveOrEnumOrComplexCollection)
                        {
                            property.SetValue(result, value, property.PropertyName, false);
                        }
                        else
                        {
                            Debug.Assert(property.GetValue(result) != null, "Collection should have already been initialized to a non null value");
                        }
                    }
                    else
                    {
                        Debug.Assert(value != null, "value != null");
                        IEnumerable valueAsEnumerable = (IEnumerable)value;
                        DataServiceQueryContinuation continuation = materializer.nextLinkTable[valueAsEnumerable];
                        Uri nextLinkUri = continuation == null ? null : continuation.NextLinkUri;
                        ProjectionPlan plan = continuation == null ? null : continuation.Plan;
                        materializer.MergeLists(entry, property, valueAsEnumerable, nextLinkUri, plan);
                    }
                }
                else if (property.IsEntityCollection)
                {
                    materializer.EntryValueMaterializationPolicy.FoundNextLinkForUnmodifiedCollection(property.GetValue(entry.ResolvedObject) as IEnumerable);
                }
            }

            return result;
        }

        /// <summary>
        /// Ensures that an entry of <paramref name="requiredType"/> is
        /// available on the specified <paramref name="entry"/>.
        /// </summary>
        /// <param name="materializer">Materilizer used for logging. </param>
        /// <param name="entry">Entry to ensure.</param>
        /// <param name="requiredType">Required type.</param>
        /// <remarks>
        /// As the 'Projection' suffix suggests, this method should only
        /// be used during projection operations; it purposefully avoid
        /// "source tree" type usage and POST reply entry resolution.
        /// </remarks>
        internal static void ProjectionEnsureEntryAvailableOfType(ODataEntityMaterializer materializer, MaterializerEntry entry, Type requiredType)
        {
            Debug.Assert(materializer != null, "materializer != null");
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(!entry.EntityHasBeenResolved, "should never get here for resolved entities.");
            Debug.Assert(
                materializer.EntityTrackingAdapter.TargetInstance == null,
                "materializer.targetInstance == null -- projection shouldn't have a target instance set; that's only used for POST replies");

            // TODO : Need to handle complex type with no tracking and entity with tracking but no id.
            if (entry.Id == null || !materializer.EntityTrackingAdapter.TryResolveAsExistingEntry(entry, requiredType))
            {
                // The type is always required, so skip ResolveByCreating.
                materializer.EntryValueMaterializationPolicy.ResolveByCreatingWithType(entry, requiredType);
            }
            else
            {
                if (!requiredType.IsAssignableFrom(entry.ResolvedObject.GetType()))
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_Current(requiredType, entry.ResolvedObject.GetType()));
                }
            }
        }

        /// <summary>Materializes an entry with no special selection.</summary>
        /// <param name="materializer">Materializer under which materialization should take place.</param>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="expectedEntryType">Expected type for the entry.</param>
        /// <returns>The materialized instance.</returns>
        internal static object DirectMaterializePlan(ODataEntityMaterializer materializer, MaterializerEntry entry, Type expectedEntryType)
        {
            materializer.entryValueMaterializationPolicy.Materialize(entry, expectedEntryType, true);
            return entry.ResolvedObject;
        }

        /// <summary>Materializes an entry without including in-lined expanded links.</summary>
        /// <param name="materializer">Materializer under which materialization should take place.</param>
        /// <param name="entry">Entry with object to materialize.</param>
        /// <param name="expectedEntryType">Expected type for the entry.</param>
        /// <returns>The materialized instance.</returns>
        internal static object ShallowMaterializePlan(ODataEntityMaterializer materializer, MaterializerEntry entry, Type expectedEntryType)
        {
            materializer.entryValueMaterializationPolicy.Materialize(entry, expectedEntryType, false);
            return entry.ResolvedObject;
        }

        /// <summary>Projects a simple value from the specified <paramref name="path"/>.</summary>
        /// <param name="entry">Root entry for paths.</param>
        /// <param name="expectedType">Expected type for <paramref name="entry"/>.</param>
        /// <param name="path">Path to pull value for.</param>
        /// <returns>The value for the specified <paramref name="path"/>.</returns>
        /// <remarks>
        /// This method will not instantiate entity types, except to satisfy requests
        /// for payload-driven feeds or leaf entities.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "cyclomatic complexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:MethodCoupledWithTooManyTypesFromDifferentNamespaces", Justification = "should refactor the method in the future.")]
        internal object ProjectionValueForPath(MaterializerEntry entry, Type expectedType, ProjectionPath path)
        {
            Debug.Assert(this != null, "materializer != null");
            Debug.Assert(entry.Entry != null, "entry.Entry != null");
            Debug.Assert(path != null, "path != null");

            // An empty path indicates that we do a regular materialization.
            if (path.Count == 0 || path.Count == 1 && path[0].Member == null)
            {
                if (!entry.EntityHasBeenResolved)
                {
                    this.EntryValueMaterializationPolicy.Materialize(entry, expectedType, /* includeLinks */ false);
                }

                return entry.ResolvedObject;
            }

            object result = null;
            ODataNestedResourceInfo link = null;
            ODataProperty odataProperty = null;
            ICollection<ODataNestedResourceInfo> links = entry.NestedResourceInfos;
            IEnumerable<ODataProperty> properties = entry.Entry.Properties;
            ClientEdmModel edmModel = this.MaterializerContext.Model;
            for (int i = 0; i < path.Count; i++)
            {
                var segment = path[i];
                if (segment.Member == null)
                {
                    continue;
                }

                bool segmentIsLeaf = i == path.Count - 1;
                string propertyName = segment.Member;

                // (p as Employee).Manager
                // The property might not be defined on the expectedType but is always defined on the TypeAs type which is a more derived type.
                expectedType = segment.SourceTypeAs ?? expectedType;
                ClientPropertyAnnotation property = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(expectedType)).GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);
                if (property.IsStreamLinkProperty)
                {
                    // projecting a DataServiceStreamLink property
                    // the stream link does not come inside <Properties>
                    // instead, it's materialized and attached to the entity descriptor on the MaterializerEntry struct
                    var streamDescriptor = entry.EntityDescriptor.StreamDescriptors.Where(sd => sd.Name == propertyName).SingleOrDefault();
                    if (streamDescriptor == null)
                    {
                        // We are projecting a named stream on a derived type and entry is of a base type which the named stream is not defined on. Return null.
                        if (segment.SourceTypeAs != null)
                        {
                            result = WebUtil.GetDefaultValue<DataServiceStreamLink>();
                            break;
                        }
                        else
                        {
                            // the named stream projected did not come back as part of the property
                            throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_PropertyMissing(propertyName));
                        }
                    }

                    result = streamDescriptor.StreamLink;
                }
                else
                {
                    // Note that we should only return the default value if the current segment is leaf.
                    // Take for example, select(new { M = (p as Employee).Manager }). If p is Person and Manager is null, we should return null here.
                    // On the other hand select(new { MID = (p as Employee).Manager.ID }) should throw if p is Person and Manager is null.
                    if (segment.SourceTypeAs != null && !links.Any(p => p.Name == propertyName) && !properties.Any(p => p.Name == propertyName) && segmentIsLeaf)
                    {
                        // We are projecting a derived property and entry is of a base type which the property is not defined on. Return null.
                        result = WebUtil.GetDefaultValue(property.PropertyType);
                        break;
                    }

                    odataProperty = properties.Where(p => p.Name == propertyName).FirstOrDefault();
                    link = odataProperty == null && links != null ? links.Where(p => p.Name == propertyName).FirstOrDefault() : null;
                    if (link == null && odataProperty == null)
                    {
                        throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_PropertyMissing(propertyName));
                    }

                    if (link != null)
                    {
                        EntryValueMaterializationPolicy.ValidatePropertyMatch(property, link);

                        MaterializerNavigationLink linkState = MaterializerNavigationLink.GetLink(link);

                        if (linkState.Feed != null)
                        {
                            MaterializerFeed feedValue = MaterializerFeed.GetFeed(linkState.Feed);

                            Debug.Assert(segmentIsLeaf, "segmentIsLeaf -- otherwise the path generated traverses a feed, which should be disallowed");

                            // When we're materializing a feed as a leaf, we actually project each element.
                            Type collectionType = ClientTypeUtil.GetImplementationType(segment.ProjectionType, typeof(ICollection<>));
                            if (collectionType == null)
                            {
                                collectionType = ClientTypeUtil.GetImplementationType(segment.ProjectionType, typeof(IEnumerable<>));
                            }

                            Debug.Assert(
                                collectionType != null,
                                "collectionType != null -- otherwise the property should never have been recognized as a collection");

                            Type nestedExpectedType = collectionType.GetGenericArguments()[0];
                            Type feedType = segment.ProjectionType;
                            if (DSClient.PlatformHelper.IsInterface(feedType) || ClientTypeUtil.IsDataServiceCollection(feedType))
                            {
                                feedType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(nestedExpectedType);
                            }

                            IEnumerable list = (IEnumerable)Util.ActivatorCreateInstance(feedType);
                            MaterializeToList(this, list, nestedExpectedType, feedValue.Entries);

                            if (ClientTypeUtil.IsDataServiceCollection(segment.ProjectionType))
                            {
                                Type dataServiceCollectionType = WebUtil.GetDataServiceCollectionOfT(nestedExpectedType);
                                list = (IEnumerable)Util.ActivatorCreateInstance(
                                    dataServiceCollectionType,
                                    list, // items
                                    TrackingMode.None); // tracking mode
                            }

                            ProjectionPlan plan = CreatePlanForShallowMaterialization(nestedExpectedType);
                            this.EntryValueMaterializationPolicy.FoundNextLinkForCollection(list, feedValue.Feed.NextPageLink, plan);
                            result = list;
                        }
                        else if (linkState.Entry != null)
                        {
                            MaterializerEntry linkEntry = linkState.Entry;

                            // If this is a leaf, then we'll do a tracking, payload-driven
                            // materialization. If this isn't the leaf, then we'll
                            // simply traverse through its properties.
                            if (segmentIsLeaf)
                            {
                                if (linkEntry.Entry != null && !linkEntry.EntityHasBeenResolved)
                                {
                                    this.EntryValueMaterializationPolicy.Materialize(linkEntry, property.PropertyType, /* includeLinks */ false);
                                    if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization && linkEntry.ShouldUpdateFromPayload)
                                    {
                                        this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(linkEntry.Entry, linkEntry.ResolvedObject);
                                    }
                                }
                            }
                            else
                            {
                                // if entry is null, no further property access can be done.
                                CheckEntryToAccessNotNull(linkEntry, propertyName);
                            }

                            // apply instance annotation for navigation property
                            if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization && linkEntry.ShouldUpdateFromPayload)
                            {
                                this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(propertyName, linkEntry.Entry, expectedType, entry.ResolvedObject);
                            }

                            properties = linkEntry.Properties;
                            links = linkEntry.NestedResourceInfos;
                            result = linkEntry.ResolvedObject;
                            entry = linkEntry;
                        }
                    }
                    else
                    {
                        if (odataProperty.Value is ODataStreamReferenceValue)
                        {
                            result = null;
                            links = ODataMaterializer.EmptyLinks;
                            properties = ODataMaterializer.EmptyProperties;
                            continue;
                        }

                        EntryValueMaterializationPolicy.ValidatePropertyMatch(property, odataProperty);

                        // So the payload is for non-entity types. If we encounter an entity in the client side, we should throw
                        // This is a breaking change from V1/V2 where we allowed materialization of entities into non-entities and vice versa
                        if (ClientTypeUtil.TypeOrElementTypeIsEntity(property.PropertyType))
                        {
                            throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidEntityType(property.EntityCollectionItemType ?? property.PropertyType));
                        }

                        if (property.IsPrimitiveOrEnumOrComplexCollection)
                        {
                            object instance = result ?? entry.ResolvedObject ?? this.CollectionValueMaterializationPolicy.CreateNewInstance(property.EdmProperty.Type.Definition.ToEdmTypeReference(true), expectedType);
                            this.entryValueMaterializationPolicy.ApplyDataValue(edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(instance.GetType())), odataProperty, instance);

                            links = ODataMaterializer.EmptyLinks;
                            properties = ODataMaterializer.EmptyProperties;
                        }
                        else if (odataProperty.Value is ODataEnumValue)
                        {
                            this.EnumValueMaterializationPolicy.MaterializeEnumTypeProperty(property.PropertyType, odataProperty);
                            links = ODataMaterializer.EmptyLinks;
                            properties = ODataMaterializer.EmptyProperties;
                        }
                        else
                        {
                            if (odataProperty.Value == null && !ClientTypeUtil.CanAssignNull(property.NullablePropertyType))
                            {
                                throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_CannotAssignNull(odataProperty.Name, property.NullablePropertyType));
                            }

                            this.entryValueMaterializationPolicy.MaterializePrimitiveDataValue(property.NullablePropertyType, odataProperty);

                            links = ODataMaterializer.EmptyLinks;
                            properties = ODataMaterializer.EmptyProperties;
                        }

                        result = odataProperty.GetMaterializedValue();

                        // TODO: projection with anonymous type is not supported now.
                        // apply instance annotation for property
                        if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization)
                        {
                            this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(odataProperty, expectedType, entry.ResolvedObject);
                        }
                    }
                }

                expectedType = property.PropertyType;
            }

            return result;
        }

        #endregion Projection support.

        /// <summary>Clears the materialization log of activity.</summary>
        internal sealed override void ClearLog()
        {
            this.EntityTrackingAdapter.MaterializationLog.Clear();
        }

        /// <summary>Applies the materialization log to the context.</summary>
        internal sealed override void ApplyLogToContext()
        {
            this.EntityTrackingAdapter.MaterializationLog.ApplyToContext();
        }

        /// <summary>Helper method for constructor of DataServiceCollection.</summary>
        /// <typeparam name="T">Element type for collection.</typeparam>
        /// <param name="from">The enumerable which has the continuation on it.</param>
        /// <param name="to">The DataServiceCollection to apply the continuation to.</param>
        internal void PropagateContinuation<T>(IEnumerable<T> from, DataServiceCollection<T> to)
        {
            DataServiceQueryContinuation continuation;
            if (this.nextLinkTable.TryGetValue(from, out continuation))
            {
                this.nextLinkTable.Add(to, continuation);
                Util.SetNextLinkForCollection(to, continuation);
            }
        }

        /// <summary>
        /// Implementation of Read/>.
        /// </summary>
        /// <returns>
        /// Return value of Read/>
        /// </returns>
        protected override bool ReadImplementation()
        {
            // links from last entry should be cleared
            this.nextLinkTable.Clear();

            bool setFeedInstanceAnnotation = this.CurrentFeed == null;
            if (this.ReadNextFeedOrEntry())
            {
                if (this.CurrentEntry == null)
                {
                    this.currentValue = null;
                    return true;
                }

                Debug.Assert(this.CurrentEntry != null, "Read successfully without finding an entry.");

                MaterializerEntry entryAndState = MaterializerEntry.GetEntry(this.CurrentEntry);
                entryAndState.ResolvedObject = this.TargetInstance;
                this.currentValue = this.materializeEntryPlan.Run(this, this.CurrentEntry, this.ExpectedType);

                if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization)
                {
                    // apply instance annotations for feed
                    if (setFeedInstanceAnnotation && this.CurrentFeed != null && this.SetInstanceAnnotations != null)
                    {
                        this.SetInstanceAnnotations(
                            this.InstanceAnnotationMaterializationPolicy.ConvertToClrInstanceAnnotations(this.CurrentFeed.InstanceAnnotations));
                    }

                    // 1. When using projection with anonymous type, the resolved object is null, ShouldUpdateFromPayload is false.
                    // 2. When using projection with a specific type, or in other circumstances,
                    // the resolved object is not null; the ShouldUpdateFromPayload is true or false according to the merge option.
                    if (this.CurrentEntry != null && entryAndState.ResolvedObject == null || entryAndState.ShouldUpdateFromPayload)
                    {
                        this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(this.CurrentEntry, this.currentValue);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the next feed or entry.
        /// </summary>
        /// <returns>True if an entry was read, otherwise false</returns>
        protected abstract bool ReadNextFeedOrEntry();

        #region Private methods.

        /// <summary>
        /// Checks that the specified <paramref name="entry"/> isn't null.
        /// </summary>
        /// <param name="entry">Entry to check.</param>
        /// <param name="name">Name of entry being accessed.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Need to throw the type that the expression would throw with other providers")]
        private static void CheckEntryToAccessNotNull(MaterializerEntry entry, string name)
        {
            Debug.Assert(name != null, "name != null");

            if (entry.Entry == null)
            {
                throw new NullReferenceException(DSClient.Strings.AtomMaterializer_EntryToAccessIsNull(name));
            }
        }

        /// <summary>Creates an entry materialization plan for a given projection.</summary>
        /// <param name="queryComponents">Query components for plan to materialize.</param>
        /// <returns>A materialization plan.</returns>
        private static ProjectionPlan CreatePlan(QueryComponents queryComponents)
        {
            // Can we have a primitive property as well?
            LambdaExpression projection = queryComponents.Projection;
            ProjectionPlan result;
            if (projection == null)
            {
                result = CreatePlanForDirectMaterialization(queryComponents.LastSegmentType);
            }
            else
            {
                result = ProjectionPlanCompiler.CompilePlan(projection, queryComponents.NormalizerRewrites);
                result.LastSegmentType = queryComponents.LastSegmentType;
            }

            return result;
        }

        /// <summary>Materializes the result of a projection into a list.</summary>
        /// <param name="materializer">Materializer to use for the operation.</param>
        /// <param name="list">Target list.</param>
        /// <param name="nestedExpectedType">Expected type for nested object.</param>
        /// <param name="entries">Entries to materialize from.</param>
        /// <remarks>
        /// This method supports projections and as such does shallow payload-driven
        /// materialization of entities.
        /// </remarks>
        private static void MaterializeToList(
            ODataEntityMaterializer materializer,
            IEnumerable list,
            Type nestedExpectedType,
            IEnumerable<ODataResource> entries)
        {
            Debug.Assert(materializer != null, "materializer != null");
            Debug.Assert(list != null, "list != null");

            Action<object, object> addMethod = ClientTypeUtil.GetAddToCollectionDelegate(list.GetType());
            foreach (ODataResource feedEntry in entries)
            {
                MaterializerEntry feedEntryState = MaterializerEntry.GetEntry(feedEntry);
                if (!feedEntryState.EntityHasBeenResolved)
                {
                    materializer.EntryValueMaterializationPolicy.Materialize(feedEntryState, nestedExpectedType, /* includeLinks */ false);
                }

                addMethod(list, feedEntryState.ResolvedObject);
            }
        }

        /// <summary>Gets a property from the specified <paramref name="links"/> list, throwing if not found.</summary>
        /// <param name="links">List to get value from.</param>
        /// <param name="propertyName">Property name to look up.</param>
        /// <returns>The specified property (never null).</returns>
        private static MaterializerNavigationLink GetPropertyOrThrow(IEnumerable<ODataNestedResourceInfo> links, string propertyName)
        {
            ODataNestedResourceInfo link = null;
            if (links != null)
            {
                link = links.Where(p => p.Name == propertyName).FirstOrDefault();
            }

            if (link == null)
            {
                throw new InvalidOperationException(DSClient.Strings.AtomMaterializer_PropertyMissing(propertyName));
            }

            return MaterializerNavigationLink.GetLink(link);
        }

        /// <summary>Merges a list into the property of a given <paramref name="entry"/>.</summary>
        /// <param name="entry">Entry to merge into.</param>
        /// <param name="property">Property on entry to merge into.</param>
        /// <param name="list">List of materialized values.</param>
        /// <param name="nextLink">Next link for feed from which the materialized values come from.</param>
        /// <param name="plan">Projection plan for the list.</param>
        /// <remarks>
        /// This method will handle entries that shouldn't be updated correctly.
        /// </remarks>
        private void MergeLists(MaterializerEntry entry, ClientPropertyAnnotation property, IEnumerable list, Uri nextLink, ProjectionPlan plan)
        {
            Debug.Assert(entry.Entry != null, "entry != null");
            Debug.Assert(entry.ResolvedObject != null, "entry.ResolvedObject != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(plan != null || nextLink == null, "plan != null || nextLink == null");

            object leftCollection = property.GetValue(entry.ResolvedObject);

            // Simple case: the list is of the target type, and the resolved entity
            // has null; we can simply assign the collection. No merge required.
            // Another case: the collection is not null but of zero elements and has
            // not been tracked already; we simply assign the collection too.
            if (entry.ShouldUpdateFromPayload &&
                property.NullablePropertyType == list.GetType() &&
                (leftCollection == null || NeedToAssignCollectionDirectly(leftCollection)))
            {
                property.SetValue(entry.ResolvedObject, list, property.PropertyName, false /* allowAdd */);
                this.EntryValueMaterializationPolicy.FoundNextLinkForCollection(list, nextLink, plan);

                foreach (object item in list)
                {
                    this.EntityTrackingAdapter.MaterializationLog.AddedLink(entry, property.PropertyName, item);
                }

                return;
            }

            this.EntryValueMaterializationPolicy.ApplyItemsToCollection(
                entry,
                property,
                list,
                nextLink,
                plan,
                false);
        }

        /// <summary>
        /// Returns if the left collection needs to be directly assigned from the right collection.
        /// </summary>
        /// <param name="collection">The given collection.</param>
        /// <returns>If the left collection needs to be directly assigned from the right collection.</returns>
        private static bool NeedToAssignCollectionDirectly(object collection)
        {
            Type type = collection.GetType();
            PropertyInfo countProp = type.GetPublicProperties(true).SingleOrDefault(property => property.Name == "Count");
            PropertyInfo isTrackingProp = type.GetNonPublicProperties(true, false /*declaredOnly*/).SingleOrDefault(property => property.Name == "IsTracking");

            if (countProp == null)
            {
                return false;
            }

            int count = (int)countProp.GetValue(collection, null);

            if (isTrackingProp == null)
            {
                return false;
            }

            bool isTracking = (bool)isTrackingProp.GetValue(collection, null);

            return count == 0 && !isTracking;
        }

        #endregion Private methods.
    }
}
