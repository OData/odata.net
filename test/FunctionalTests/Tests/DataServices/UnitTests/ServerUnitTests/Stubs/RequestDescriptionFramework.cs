//---------------------------------------------------------------------
// <copyright file="RequestDescriptionFramework.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.RequestDescriptionFramework
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using Microsoft.OData.Client;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion

    #region NodeFacetHelpers
    /// <summary>Helper class adding more NodeFacets and method to manipulate them.</summary>
    public static class NodeFacetHelpers
    {
        private const string ExistingValueFacetName = "ExistingValue";

        /// <summary>Returns the value of "ExistingValue" node facet. Use to specify a value of a property which is defined in at least one entity.</summary>
        /// <param name="facets">The facets to get the value from.</param>
        /// <returns>A <see cref="NodeValue"/> with the value of the property.</returns>
        public static NodeValue ExistingValue(this NodeFacets facets)
        {
            NodeFacet facet = facets.FirstOrDefault(f => f.Name == ExistingValueFacetName);
            return facet == null ? null : facet.Value;
        }

        /// <summary>Creates new facet which specifies an existing value of a property.</summary>
        /// <param name="value">The value of the property which exists on at least one entity.</param>
        /// <returns>The new facet.</returns>
        public static NodeFacet ExistingValue(NodeValue value)
        {
            return new NodeFacet(ExistingValueFacetName, value);
        }

        private const string ServerDrivenPagingFacetName = "ServerDrivenPaging";

        /// <summary>Returnst the page size of Server Driven Paging facet or null if none is found.</summary>
        /// <param name="facets">The facets to look into.</param>
        /// <returns>The size of the page or null.</returns>
        public static int? ServerDrivenPaging(this NodeFacets facets)
        {
            NodeFacet facet = facets.FirstOrDefault(f => f.Name == ServerDrivenPagingFacetName);
            return facet == null ? null : (int?)(facet.Value.ClrValue);
        }

        /// <summary>Creates new facet which specifies that a given type has SDP on.</summary>
        /// <param name="pageSize">The size of the page in SDP.</param>
        /// <returns>The new facet.</returns>
        public static NodeFacet ServerDrivenPaging(int pageSize)
        {
            return new NodeFacet(ServerDrivenPagingFacetName, Clr.Value(pageSize));
        }

        public enum EntityPropertyMappingType
        {
            EverythingInContent,
            SomeV2NotInContent,
            SomeV3NotInContent,
        }

        private const string EntityPropertyMappingFacetName = "EntityPropertyMapping";

        /// <summary>Returns the EPM facet or null if none is found.</summary>
        /// <param name="facets">The facets to look into.</param>
        /// <returns>null - no EPM or the EPM type.</returns>
        public static EntityPropertyMappingType? EntityPropertyMapping(this NodeFacets facets)
        {
            NodeFacet facet = facets.FirstOrDefault(f => f.Name == EntityPropertyMappingFacetName);
            if (facet != null) return (EntityPropertyMappingType)(int)(facet.Value.ClrValue);
            else return null;
        }

        /// <summary>Returns the most restrictive EPM for a given resource type and all its derived types.</summary>
        /// <param name="resourceType">The resource type to inspect</param>
        /// <returns>null - no EPM or the EPM type.</returns>
        public static EntityPropertyMappingType? EntityPropertyMappingOnInheritanceTree(this ResourceType resourceType)
        {
            IEnumerable<ResourceType> inheritanceTreeTypesAndSelf = (new ResourceType[] { resourceType }).Concat(resourceType.DerivedTypes);
            EntityPropertyMappingType? result = null;
            foreach (var type in inheritanceTreeTypesAndSelf)
            {
                var epm = type.Facets.EntityPropertyMapping();
                if (result == null) result = epm;
                if (result == EntityPropertyMappingType.EverythingInContent && epm != EntityPropertyMappingType.EverythingInContent)
                    result = epm;
            }
            return result;
        }

        /// <summary>Creates new facet which specifies that the given type has EPM.</summary>
        /// <param name="entityPropertyMappingType">The type of EPM used for the resource type.</param>
        /// <returns>The new facet</returns>
        public static NodeFacet EntityPropertyMapping(EntityPropertyMappingType entityPropertyMappingType)
        {
            return new NodeFacet(EntityPropertyMappingFacetName, Clr.Value<int>((int)entityPropertyMappingType));
        }
    }
    #endregion

    #region AnnotatedBase
    /// <summary>Base class which adds annotations</summary>
    public class AnnotatedBase
    {
        /// <summary>Lazily initialized dictionary of annotations.</summary>
        private Dictionary<string, object> annotations;

        /// <summary>Constructor</summary>
        public AnnotatedBase() { }

        /// <summary>Copy constructor.</summary>
        /// <param name="source">The source to copy.</param>
        public AnnotatedBase(AnnotatedBase source)
        {
            if (source != null)
            {
                this.annotations = new Dictionary<string, object>(source.annotations);
            }
        }

        /// <summary>Sets annotation. This will overwrite any existing annotation with the same name.</summary>
        /// <param name="name">The name of the annotation to set.</param>
        /// <param name="value">The value of the annotation to set.</param>
        public void SetAnnotation(string name, object value)
        {
            EnsureAnnotations();
            this.annotations[name] = value;
        }

        /// <summary>Returns annotation value of null if no annotation was found.</summary>
        /// <param name="name">The name of the annotation to find.</param>
        /// <returns>The value of the annotation or null. Note that if the value of the annotation was null, there's no way
        /// to tell.</returns>
        public object GetAnnotation(string name)
        {
            if (this.annotations == null) return null;
            object value;
            if (!this.annotations.TryGetValue(name, out value)) return null;
            return value;
        }

        /// <summary>Returns annotation value or the defaultValue passed in if no annotation was found.</summary>
        /// <param name="name">The name of the annotation to find.</param>
        /// <returns>The value of the annotation or defaultValue that was passed in.</returns>
        /// <remarks>The method will throw if the annotation value can't be casted to the specified type.</remarks>
        public TAnnotation GetAnnotation<TAnnotation>(string name)
        {
            return (TAnnotation)this.GetAnnotation(name);
        }

        /// <summary>Returns annotation value of null if no annotation was found.</summary>
        /// <param name="name">The name of the annotation to find.</param>
        /// <param name="defaultValue">The value to use if the annotation has not been set.</param>
        /// <returns>The value of the annotation or null. Note that if the value of the annotation was null, there's no way
        /// to tell.</returns>
        /// <remarks>The method will throw if the annotation value can't be casted to the specified type.</remarks>
        public TAnnotation GetAnnotationOrDefault<TAnnotation>(string name, TAnnotation defaultValue)
        {
            if (!this.ContainsAnnotation(name))
            {
                return defaultValue;
            }

            return (TAnnotation)this.GetAnnotation(name);
        }
        
        
        /// <summary>
        /// Informs the caller about the precense of an Annotation
        /// </summary>
        /// <param name="name">the annotation name</param>
        /// <returns>true if the annotation exists, otherwise false</returns>
        public bool ContainsAnnotation(string name)
        {
            if (this.annotations == null)
                return false;

            return this.annotations.ContainsKey(name);
        }

        /// <summary>Combines annotations on this class with the ones on the source. This class always wins.</summary>
        /// <param name="source">The annotations to combine.</param>
        public void CombineAnnotations(AnnotatedBase source)
        {
            if (source.annotations == null) return;
            EnsureAnnotations();
            foreach (var kv in source.annotations)
            {
                if (!this.annotations.ContainsKey(kv.Key))
                {
                    this.annotations[kv.Key] = kv.Value;
                }
            }
        }

        /// <summary>Initializes annotations if they are not yet initialized.</summary>
        private void EnsureAnnotations()
        {
            if (this.annotations == null) { this.annotations = new Dictionary<string, object>(); }
        }
    }
    #endregion

    #region RequestDescription
    /// <summary>Class which described request. This is a base class defininf the interface.</summary>
    public abstract class RequestDescription : AnnotatedBase
    {
        /// <summary>Constructor</summary>
        public RequestDescription() { }

        /// <summary>Copy constructor.</summary>
        /// <param name="source">The source to copy.</param>
        public RequestDescription(AnnotatedBase source) : base(source) { }

        /// <summary>Method which returns the request URI (without the server base URI).</summary>
        /// <returns>The relative URI of the request.</returns>
        public abstract string GetRequestUri();

        /// <summary>Method which returns the format of the response.</summary>
        /// <returns>The mime/type of the response expected.</returns>
        public abstract string GetResponseFormat();

        /// <summary>Returns a client LINQ query for this request.</summary>
        /// <param name="context">The client context to build the query on.</param>
        /// <param name="queryNode">Query node to use to build the query.</param>
        /// <returns>The query if it's possible to build it.</returns>
        public abstract object GetClientLinqQuery(DataServiceContext context, QueryNode queryNode);

        /// <summary>Returns a client LINQ query for this request.</summary>
        /// <param name="context">The client context to build the query on.</param>
        /// <returns>The query if it's possible to build it. If the request describes a query with a single result (like .Count()) then
        /// this will return the result of the query directly (and will execute the query). Otherwise this returns the IQueryable.</returns>
        public object GetClientLinqQuery(DataServiceContext context)
        {
            return this.GetClientLinqQuery(context, this.GetClientLinqQueryNode());
        }

        /// <summary>Returns a query node for this request.</summary>
        /// <returns>The query node which represents the request.</returns>
        public abstract QueryNode GetClientLinqQueryNode();
    }
    #endregion

    #region ServiceContextDescription
    /// <summary>Desctiption of a service context.</summary>
    public class ServiceContextDescription : AnnotatedBase
    {
        /// <summary>Constructs new descrinption of a service.</summary>
        /// <param name="name">The name of the service (used only for reporting).</param>
        public ServiceContextDescription(string name)
        {
            this.Name = name;
            this.Usings = new List<Func<object>>();
            this.Setups = new List<Action>();
            this.ActionsBeforeDropChangesRequest = new List<Action<TestWebRequest>>();
            this.ActionsBeforePreserveChangesRequest = new List<Action<TestWebRequest>>();
        }

        /// <summary>Name of the service context (for shorter description).</summary>
        public string Name { get; set; }

        /// <summary>The type to create the service from.</summary>
        public Type DataServiceType { get; set; }

        /// <summary>Array of functions which will be executed before the service is created.
        /// If they return an IDisposable instance this instance will be disposed once the service is no longer needed.</summary>
        public List<Func<object>> Usings { get; set; }

        /// <summary>Action to execute before the service startup to setup the service.</summary>
        public List<Action> Setups { get; set; }

        /// <summary>The service container which describes the service.</summary>
        public ServiceContainer ServiceContainer { get; set; }

        /// <summary>These actions will be executed before any request which should drop any changes from the previous ones.</summary>
        public List<Action<TestWebRequest>> ActionsBeforeDropChangesRequest { get; set; }

        /// <summary>These actions will be executed before any request which should preserve changes from the previous ones.</summary>
        public List<Action<TestWebRequest>> ActionsBeforePreserveChangesRequest { get; set; }

        /// <summary>Infers the association graph from the existing connections. Call this once the ServiceContainer is filled in.</summary>
        public void InferAssociations()
        {
            foreach (ResourceContainer rc in this.ServiceContainer.ResourceContainers)
            {
                foreach (ResourceType t in rc.ResourceTypes)
                {
                    t.InferAssociations();
                }
            }
        }

        /// <summary>Creates IDisposable which should be disposed once the service is not needed anymore.</summary>
        /// <returns>The IDisposable to dispose.</returns>
        public IDisposable RestoreState() { return (new DisposableUsing(this.Usings)) as IDisposable; }

        /// <summary>Sets up environment for the service to run.</summary>
        public void SetupService()
        {
            foreach (var s in this.Setups)
            {
                s();
            }
        }

        /// <summary>Will prepare specified request such that it will drop all changes from the previous ones.</summary>
        /// <param name="request">the request which is about to execute</param>
        public void PrepareDropChangesRequest(TestWebRequest request)
        {
            foreach (var a in this.ActionsBeforeDropChangesRequest)
            {
                a(request);
            }
        }

        /// <summary>Will prepare specified request such that it will preserve changes from the previous ones.</summary>
        /// <param name="request">the request which is about to execute</param>
        public void PreparePreserveChangesRequest(TestWebRequest request)
        {
            foreach (var a in this.ActionsBeforePreserveChangesRequest)
            {
                a(request);
            }
        }

        /// <summary>Will prepare the client context for usage with this service. For example sets the resolvers.</summary>
        /// <param name="context">The context to prepare.</param>
        public void PrepareClientContext(DataServiceContext context)
        {
            context.ResolveType = this.ResolveClientType;
            context.ResolveName = this.ResolveClientName;
        }

        private Type ResolveClientType(string fullname)
        {
            NodeType nodeType = this.ServiceContainer.AllTypes.FirstOrDefault(r => r.FullName == fullname);
            return nodeType == null ? null : nodeType.ClientClrType;
        }

        private string ResolveClientName(Type type)
        {
            NodeType nodeType = this.ServiceContainer.AllTypes.FirstOrDefault(r => r.ClientClrType == type);
            return nodeType == null ? null : nodeType.FullName;
        }

        private class DisposableUsing : IDisposable
        {
            IEnumerable<object> usings;

            public DisposableUsing(IEnumerable<Func<object>> usings)
            {
                List<object> disposables = new List<object>();
                if (usings != null)
                {
                    foreach (var usingFunction in usings)
                    {
                        disposables.Add(usingFunction());
                    }
                }
                this.usings = disposables;
            }

            #region IDisposable Members

            public void Dispose()
            {
                foreach (var u in usings)
                {
                    IDisposable d = u as IDisposable;
                    if (d != null)
                    {
                        d.Dispose();
                    }
                }
            }

            #endregion
        }
    }
    #endregion

    #region ResourceContainerDescription
    /// <summary>Description of a service and a single resource container on the server.</summary>
    public class ResourceContainerDescription : RequestDescription
    {
        /// <summary>Constructor</summary>
        public ResourceContainerDescription() : base() { }
        /// <summary>Constructor</summary>
        /// <param name="source">Another request description to copy annotations from.</param>
        public ResourceContainerDescription(AnnotatedBase source) : base(source) { }

        /// <summary>The service context this container belongs to.</summary>
        public ServiceContextDescription ServiceContext { get; set; }

        /// <summary>The resource container.</summary>
        public ResourceContainer ResourceContainer { get; set; }

        public override string GetRequestUri()
        {
            return "/" + this.ResourceContainer.Name;
        }

        public override string GetResponseFormat()
        {
            throw new NotSupportedException("Format is not specified by the resource container.");
        }

        public override object GetClientLinqQuery(DataServiceContext context, QueryNode queryNode)
        {
            MethodInfo createQueryMI = typeof(DataServiceContext).GetMethod("CreateQuery", new Type[] { typeof(string) });
            createQueryMI = createQueryMI.MakeGenericMethod(this.ResourceContainer.BaseType.ClientClrType);
            // ctx.CreateQuery<T>("entitySetName")
            IQueryable resourceContainerQuery = (IQueryable)createQueryMI.Invoke(context, new object[] { this.ResourceContainer.Name });

            LinqQueryBuilder linqQueryBuilder = new LinqQueryBuilder(dummyWorkspace, resourceContainerQuery);
            linqQueryBuilder.Build(queryNode);

            return linqQueryBuilder.QueryResultSingle ?? linqQueryBuilder.QueryResult;
        }

        public override QueryNode GetClientLinqQueryNode()
        {
            return Query.From(Exp.Variable(this.ResourceContainer));
        }

        public override string ToString()
        {
            return "[" + this.ServiceContext.Name + "]" + this.GetRequestUri();
        }

        private class DummyWorkspace : Workspace
        {
            public DummyWorkspace()
                : base(DataLayerProviderKind.None, "DummyWorkspace")
            {
            }

            public override void ApplyFriendlyFeeds()
            {
                throw new NotImplementedException();
            }

            public override IQueryable ResourceContainerToQueryable(ResourceContainer container)
            {
                throw new NotImplementedException();
            }
        }

        private static DummyWorkspace dummyWorkspace = new DummyWorkspace();
    }
    #endregion

    #region PathAndFormatDescription
    /// <summary>Description of a request path (the URL portion) and the response format of a request.</summary>
    public class PathAndFormatDescription : RequestDescription
    {
        /// <summary>Creates new description</summary>
        /// <param name="resourceContainer">The resource container description to base the path on.</param>
        /// <param name="path">The path to append to the resource container (appended without any separators).</param>
        /// <param name="format">The format of the response expected.</param>
        public PathAndFormatDescription(ResourceContainerDescription resourceContainer, string path, string format)
            : base(resourceContainer)
        {
            this.ResourceContainer = resourceContainer;
            this.Path = path;
            this.Format = format;
        }

        /// <summary>The resource container this path applies to.</summary>
        public ResourceContainerDescription ResourceContainer { get; set; }

        /// <summary>The path after the resource container.</summary>
        public string Path { get; set; }

        /// <summary>The response format expected.</summary>
        public string Format { get; set; }

        /// <summary>Optional - creates a LINQ query on the client data context which represents the specified path.</summary>
        public Func<QueryNode> CreateClientLinqQueryNode { get; set; }

        /// <summary>The type of the root returned by the path (for a plain resource container 
        /// this would be the base resource type of that container)</summary>
        public NodeType RootType { get; set; }

        public override string GetRequestUri()
        {
            return this.ResourceContainer.GetRequestUri() + this.Path;
        }

        public override string GetResponseFormat()
        {
            return this.Format;
        }

        public override string ToString()
        {
            return "[" + this.ResourceContainer.ServiceContext.Name + "]" + this.GetRequestUri() + " ResponseFormat=[" + this.GetResponseFormat() + "]";
        }

        public override object GetClientLinqQuery(DataServiceContext context, QueryNode queryNode)
        {
            // Just forward to the resource container
            return this.ResourceContainer.GetClientLinqQuery(context, queryNode);
        }

        public override QueryNode GetClientLinqQueryNode()
        {
            if (CreateClientLinqQueryNode != null)
            {
                return CreateClientLinqQueryNode();
            }
            else
            {
                throw new NotImplementedException("This path '" + this.ToString() + "' doesn't implement the client query creation.");
            }
        }
    }
    #endregion

    #region QueryOption
    /// <summary>Class representing a single query option.</summary>
    public class QueryOption
    {
        /// <summary>Create new query option.</summary>
        /// <param name="name">The name of the option (this must include the $ if required)</param>
        /// <param name="value">The value of the query option.</param>
        public QueryOption(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>The name of the query options - this includes the $ if required.</summary>
        public string Name { get; set; }
        /// <summary>The value of the query option.</summary>
        public string Value { get; set; }
    }
    #endregion

    #region QueryOptionsDescription
    /// <summary>Describes a request with query options.</summary>
    public class QueryOptionsDescription : RequestDescription
    {
        /// <summary>Constructor</summary>
        /// <param name="path">The path description to base this off.</param>
        public QueryOptionsDescription(PathAndFormatDescription path)
            : base(path)
        {
            this.Path = path;
        }

        /// <summary>The path description the query options are applied to.</summary>
        public PathAndFormatDescription Path { get; set; }

        private List<QueryOption> options = new List<QueryOption>();
        /// <summary>List of query options applied to the path.</summary>
        public List<QueryOption> Options { get { return this.options; } }

        public override string GetRequestUri()
        {
            string queryOptions = this.GetQueryOptions();
            return this.Path.GetRequestUri() + ((queryOptions.Length > 0) ? "?" + queryOptions : "");
        }

        public override string GetResponseFormat()
        {
            return this.Path.GetResponseFormat();
        }

        public override string ToString()
        {
            return "[" + this.Path.ResourceContainer.ServiceContext.Name + "]" + this.GetRequestUri() + " ResponseFormat=[" + this.GetResponseFormat() + "]";
        }

        private string GetQueryOptions()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Options != null)
            {
                foreach (var qo in this.Options)
                {
                    if (sb.Length > 0) sb.Append("&");
                    sb.Append(qo.Name);
                    sb.Append("=");
                    sb.Append(qo.Value);
                }
            }
            return sb.ToString();
        }

        public override object GetClientLinqQuery(DataServiceContext context, QueryNode queryNode)
        {
            // Just forward to the path
            return this.Path.GetClientLinqQuery(context, queryNode);
        }

        public override QueryNode GetClientLinqQueryNode()
        {
            ResourceContainer resourceContainer = this.Path.ResourceContainer.ResourceContainer;
            QueryNode query = this.Path.GetClientLinqQueryNode();
            if (this.Options != null)
            {
                foreach (QueryOption queryOption in this.Options)
                {
                    switch(queryOption.Name)
                    {
                        case "$select":
                            query = query.Select(query.New(GetProperties(queryOption.Value, resourceContainer)));
                            break;
                        case "$filter":
                            query = query.Where(ProcessFilter(queryOption.Value, resourceContainer));
                            break;
                        case "$orderby":
                            // ascending/descending not supported
                            query = query.Sort(GetProperties(queryOption.Value, resourceContainer), true);
                            break;
                        case "$expand":
                            query = query.Expand(GetProperties(queryOption.Value, resourceContainer));
                            break;
                        case "$inlinecount":
                            query = query.Count(true);
                            break;
                        case "$top":
                            query = query.Top(int.Parse(queryOption.Value));
                            break;
                        case "$skip":
                            query = query.Skip(int.Parse(queryOption.Value));
                            break;
                        default:
                            throw new NotSupportedException(string.Format("queryOption: {0} (value: {1}) is invalid or not supported.", queryOption.Name, queryOption.Value));
                    }
                }
            }
            return query;
        }

        /// <summary>Given a list of property names in a string separated by commas, this method returns array
        /// of the PropertyExpressions for these properties.</summary>
        /// <param name="input">The comma separated list of property names.</param>
        /// <param name="resourceContainer">The resource container the properties belong to.</param>
        /// <returns>Array of property expressions.</returns>
        private static PropertyExpression[] GetProperties(string input, ResourceContainer resourceContainer)
        {
            string[] propertyNames = input.Split(',');

            return (from property in resourceContainer.BaseType.Properties
                    where propertyNames.Contains(property.Name)
                    select new PropertyExpression(property)).ToArray();
        }

        private static IEnumerable<string> SplitFilterExpression(string filterExp)
        {
            int pos = 0;
            char[] interestingCharacters = new char[] { ' ', '\'' };
            while (pos < filterExp.Length)
            {
                int i = filterExp.IndexOfAny(interestingCharacters, pos);
                if (i == -1) break;
                yield return filterExp.Substring(pos, i - pos);
                if (filterExp[i] == '\'')
                {
                    pos = i;
                    int j = filterExp.IndexOf('\'', i + 1);
                    if (j == -1) break;
                    yield return filterExp.Substring(i, j - i + 1);
                    pos = j + 1;
                }
                else
                {
                    pos = i + 1;
                }
            }
            string remaining = pos < filterExp.Length ? filterExp.Substring(pos) : "";
            if (remaining.Length > 0) yield return remaining;
        }

        // can process just primitve filter query options: [propertyName] [operator] [constValue]
        // due to a very primitive type conversion only string and int types are supported
        private static ComparisonExpression ProcessFilter(string filterExp, ResourceContainer resourceContainer)
        {
            string[] components = SplitFilterExpression(filterExp).Where(s => s.Length > 0).ToArray();
            if (components.Length != 3)
            {
                throw new Exception("filter expression too complicated.");
            }

            ComparisonOperator @operator;
            switch(components[1])
            {
                case "lt": @operator = ComparisonOperator.LessThan; break;
                case "le": @operator = ComparisonOperator.LessThanOrEqual; break;
                case "eq": @operator = ComparisonOperator.Equal; break;
                case "ge": @operator = ComparisonOperator.GreaterThanOrEqual; break;
                case "gt": @operator = ComparisonOperator.GreaterThan; break;
                case "ne": @operator = ComparisonOperator.NotEqual; break;
                default:
                    throw new NotSupportedException(string.Format("Operator {0} is either unknown or not supported", components[1]));                
            }
            
            PropertyExpression left = new PropertyExpression(resourceContainer.BaseType.Properties.Where(p => p.Name == components[0]).First());
            //the type on the right side has to be the same as the one on the left side
            object value = components[2][0] == '\'' ? components[2].Substring(1, components[2].Length - 2) : (object)(int.Parse(components[2]));
            ConstantExpression right = new ConstantExpression(new NodeValue(value, left.Type)); 
            return new ComparisonExpression(left, right, @operator);
        }
    }
    #endregion
}
