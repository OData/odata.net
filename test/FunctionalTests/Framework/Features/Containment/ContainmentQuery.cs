//---------------------------------------------------------------------
// <copyright file="ContainmentQuery.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Test.Astoria;
using System.Linq;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Xml;
using System.Net;

namespace System.Data.Test.Astoria
{
    public class ContainmentQuery
    {
        public QueryNode Query = null;
        public HttpStatusCode ExpectedStatusCode = HttpStatusCode.OK;
        public ResourceIdentifier ExpectedErrorIdentifier = null;
        public object[] ExpectedErrorArgs = null;
        public ResourceType Type = null;
        public ResourceContainer Container = null;
        public Workspace Workspace = null;
        public bool SetExpected = false;

        public bool ErrorExpected
        {
            get { return (ExpectedErrorIdentifier != null); }
        }
    }

    public class ContainmentQueryFactory
    {
        private class ContainmentQuerySegment
        {
            public ContainmentAttribute attribute;
            public bool abbreviateChild;
            public bool omitParent;

            private ContainmentQuerySegment(ContainmentAttribute att, bool abbreviate, bool omit)
            {
                attribute = att;
                abbreviateChild = abbreviate;
                omitParent = omit;
            }

            public ContainmentQuerySegment(ContainmentAttribute att, bool abbreviate)
                : this(att, abbreviate, false)
            { }

            public ContainmentQuerySegment(ContainmentQuerySegment toCopy)
                : this(toCopy.attribute, toCopy.abbreviateChild, toCopy.omitParent)
            { }
        }

        internal class ContainmentQueryPath
        {
            private ResourceContainer rootContainer;
            private List<ContainmentQuerySegment> segments;

            public int SegmentCount
            {
                get { return segments.Count; }
            }

            public void OmitAll()
            {
                segments.ForEach(s => s.omitParent = true);
            }

            private ContainmentQueryPath(ContainmentQueryPath toCopy)
                : this(toCopy.rootContainer)
            {
                foreach (ContainmentQuerySegment segment in toCopy.segments)
                    this.segments.Add(new ContainmentQuerySegment(segment));
            }

            public ContainmentQueryPath(ResourceContainer root)
            {
                this.rootContainer = root;
                this.segments = new List<ContainmentQuerySegment>();
            }

            public ContainmentQueryPath Clone()
            {
                return new ContainmentQueryPath(this);
            }

            public void Add(ContainmentAttribute att, bool abbrChild)
            {
                ContainmentQuerySegment segment = new ContainmentQuerySegment(att, abbrChild);

                if (segments.Count == 0)
                {
                    AstoriaTestLog.AreEqual(segment.attribute.ParentContainer, rootContainer,
                        "First segment's parent container does not match this path's root container");
                }
                else
                {
                    AstoriaTestLog.AreEqual(segment.attribute.ParentContainer, segments.Last().attribute.ChildContainer,
                        "New segment's parent does not match the current last container");
                }
                segments.Add(segment);
            }

            public ResourceContainer FinalContainer
            {
                get
                {
                    if (segments.Count > 0)
                        return segments[segments.Count - 1].attribute.ChildContainer;
                    else
                        return rootContainer;
                }
            }

            private List<KeyExpression> GetContainingKeys(KeyExpression keyExpression)
            {
                List<KeyExpression> keys = new List<KeyExpression>();

                // reverse the segments, and generate keys for each parent type
                KeyExpression currentKey = keyExpression;
                keys.Add(currentKey);
                segments.Reverse();
                foreach (ContainmentQuerySegment segment in segments)
                {
                    currentKey = segment.attribute.GetContainingKey(currentKey, segment.abbreviateChild);
                    keys.Add(currentKey);
                }
                segments.Reverse();
                keys.Reverse();

                return keys;
            }

            private static void SetTopLevelAccessError(ContainmentQuery query, ResourceType type)
            {
                query.ExpectedStatusCode = System.Net.HttpStatusCode.NotFound;
                //query.ExpectedErrorIdentifier = SystemDataServicesResourceIdentifiers.CreateFirstSegment_NoTopLevelAccess;
                query.ExpectedErrorArgs = new string[] { type.ClrType.Name }; // TODO: error?, for missing keys it is full name
            }

            private static void SetMissingKeysError(ContainmentQuery query, ResourceType type)
            {
                query.ExpectedStatusCode = System.Net.HttpStatusCode.BadRequest;
                query.ExpectedErrorIdentifier = SystemDataServicesResourceIdentifiers.BadRequest_KeyCountMismatch;
                query.ExpectedErrorArgs = new string[] { type.ClrType.FullName };
            }

            public ContainmentQuery ToQuery(KeyExpression keyExpression)
            {
                return ToQuery(keyExpression, false);
            }

            public ContainmentQuery ToQuery(KeyExpression keyExpression, bool setOnly)
            {
                ResourceContainer finalContainer = FinalContainer;
                AstoriaTestLog.AreEqual(keyExpression.ResourceContainer, finalContainer,
                    "ToQuery given a KeyExpression to an unexpected resource container");

                List<KeyExpression> keys = GetContainingKeys(keyExpression);
                List<ResourceContainer> omittedContainers =
                    segments.Where(s => s.omitParent).Select(s => s.attribute.ParentContainer).ToList();

                ContainmentQuery query = new ContainmentQuery();
                query.Query = null;
                query.Container = finalContainer;
                query.Type = query.Container.BaseType;
                query.Workspace = query.Container.Workspace;
                query.SetExpected = setOnly;

                ResourceContainer parentContainer = null;
                foreach (KeyExpression key in keys)
                {
                    if (omittedContainers.Contains(key.ResourceContainer))
                        continue;

                    bool omitKey = (setOnly && key == keyExpression);

                    if (parentContainer == null)
                    {
                        if (!query.ErrorExpected && !key.ResourceContainer.BaseType.Facets.TopLevelAccess)
                            SetTopLevelAccessError(query, key.ResourceType);

                        if (!omitKey)
                        {
                            if (!query.ErrorExpected && key.IncludeInUri.Contains(false))
                                SetMissingKeysError(query, key.ResourceType);
                        }
                        
                        query.Query = Query.From(Exp.Variable(key.ResourceContainer));
                    }
                    else
                    {
                        ResourceProperty navProp = segments
                            .Where(s => s.attribute.ParentContainer == parentContainer)
                            .Select(s => s.attribute.ParentNavigationProperty)
                            .FirstOrDefault();
                        
                        query.Query = query.Query.Nav(navProp.Property());
                    }

                    parentContainer = key.ResourceContainer;

                    // if we just want the set, leave off the last where clause
                    if(!omitKey)
                        query.Query = query.Query.Where(key);
                }

                return query;
            }

            public bool FinalSetAbbreviated
            {
                // there will be two versions of the last segment, abbreviated and not
                get
                {
                    if (segments.Count > 0)
                        return segments[segments.Count - 1].abbreviateChild;
                    else
                        return false;
                }
            }

            public bool AnyOmitted
            {
                get
                {
                    return segments.Any(s => s.omitParent);
                }
            }

            public override string ToString()
            {
                Text.StringBuilder builder = new Text.StringBuilder();
                if (segments.Count == 0)
                    builder.Append(rootContainer.Name);
                else
                {
                    foreach (ContainmentQuerySegment segment in segments)
                    {
                        if (builder.Length > 0)
                            builder.Append(", ");

                        builder.Append(segment.attribute.ParentContainer.Name);
                        
                        if (segment.omitParent)
                            builder.Append("(o)");
                        
                        builder.Append("->");
                        builder.Append(segment.attribute.ChildContainer.Name);

                        if (segment.abbreviateChild)
                            builder.Append("(a)");
                    }
                }
                return builder.ToString();
            }
        }

        private ServiceContainer serviceContainer;
        private List<ContainmentQueryPath> rootQueryPaths;

        public ContainmentQueryFactory(ServiceContainer container)
        {
            serviceContainer = container;
            rootQueryPaths = new List<ContainmentQueryPath>();

            List<ContainmentAttribute> attributes = container.Facets.Attributes.OfType<ContainmentAttribute>().ToList();

            IEnumerable<ResourceContainer> rootContainers =
                attributes.Select(ca => ca.ParentContainer)
                .Except(attributes.Select(ca => ca.ChildContainer));

            Queue<ContainmentQueryPath> buildQueue = new Queue<ContainmentQueryPath>();

            foreach (ResourceContainer rootContainer in rootContainers)
            {
                buildQueue.Enqueue(new ContainmentQueryPath(rootContainer));
            }

            while (buildQueue.Count > 0)
            {
                ContainmentQueryPath path = buildQueue.Dequeue();

                rootQueryPaths.Add(path);

                ResourceContainer currentContainer = path.FinalContainer;
                ContainmentQueryPath copy;

                IEnumerable<ContainmentAttribute> currentAttributes = attributes.Where(ca => ca.ParentContainer == currentContainer);
                foreach (bool omit in new bool[] { false, true })
                {
                    if (omit && (path.FinalSetAbbreviated || path.AnyOmitted))
                        continue;

                    foreach (bool abbreviate in new bool[] { true, false })
                    {
                        foreach (ContainmentAttribute att in currentAttributes)
                        {
                            bool canAbbreviate = (att.ChildContainer.BaseType.Key.PropertyCount > 1);
                            if (abbreviate && !canAbbreviate)
                                continue;

                            copy = path.Clone();
                            copy.Add(att, abbreviate);
                            if (omit)
                                copy.OmitAll();
                            buildQueue.Enqueue(copy);
                        }
                    }
                }
            }
        }

        public IEnumerable<ContainmentQuery> GetRandomRootQueries(bool setOnly)
        {
            //UriQueryBuilder uriBuilder = new UriQueryBuilder(serviceContainer.Workspace, serviceContainer.Workspace.ServiceEndPoint);
            foreach (ContainmentQueryPath path in rootQueryPaths)
            {
                // if we're just looking at the set, then only return the unabreviated version (to avoid duplicate uris)
                if (setOnly && path.FinalSetAbbreviated)
                    continue;

                //KeyExpressions keys = serviceContainer.Workspace.GetAllExistingKeys(path.FinalContainer);
                //KeyExpression keyExpression = keys.First;
                KeyExpression keyExpression = serviceContainer.Workspace.GetRandomExistingKey(path.FinalContainer);

                ContainmentQuery query = path.ToQuery(keyExpression, setOnly);
                
                //string uri = uriBuilder.Build(query.Query).Replace(serviceContainer.Workspace.ServiceEndPoint, "");
                //string pathString = path.ToString().PadRight(40, ' ');
                //AstoriaTestLog.WriteLineIgnore(list.Count + ". " + pathString + ":     " + uri);

                yield return query;
            }
        }
    }


    public enum ContainmentQuerySuffix
    {
        None = 0,
        SingleNavigation = 1,
        MultipleNavigation = 2,
        SingleNavigationTwice = 3,
        SimpleType = 4,
        LinkedSingleNavigation = 5,
        LinkedMultipleNavigation = 6
    };

    public static class ContainmentQueryAppender
    {

        private static class VerbMap
        {
            private static Dictionary<ContainmentQuerySuffix, RequestVerb[]> map;

            static VerbMap()
            {
                map = new Dictionary<ContainmentQuerySuffix, RequestVerb[]>();
                Array suffixes = Enum.GetValues(typeof(ContainmentQuerySuffix));
                foreach (ContainmentQuerySuffix suffix in suffixes)
                {
                    RequestVerb[] verbs;
                    #region fill the array
                    switch (suffix)
                    {
                        // entities
                        case ContainmentQuerySuffix.None:
                        case ContainmentQuerySuffix.SingleNavigation:
                        case ContainmentQuerySuffix.SingleNavigationTwice:
                            verbs = new RequestVerb[] 
                            { 
                                RequestVerb.Get,
                                RequestVerb.Delete,
                                RequestVerb.Patch,
                                RequestVerb.Put
                            };
                            break;

                        // sets
                        case ContainmentQuerySuffix.MultipleNavigation:
                            verbs = new RequestVerb[] 
                            { 
                                RequestVerb.Get,
                                RequestVerb.Post
                            };
                            break;

                        // values
                        case ContainmentQuerySuffix.SimpleType:
                            verbs = new RequestVerb[]
                            {
                                RequestVerb.Get,
                                RequestVerb.Patch,
                                RequestVerb.Put
                            };
                            break;

                        // $ref single - not sure about these
                        case ContainmentQuerySuffix.LinkedSingleNavigation:
                            verbs = new RequestVerb[]
                            {
                                RequestVerb.Get,
                                RequestVerb.Patch,
                                RequestVerb.Put
                            };
                            break;

                        // $ref multiple - not sure about these
                        case ContainmentQuerySuffix.LinkedMultipleNavigation:
                            verbs = new RequestVerb[]
                            {
                                RequestVerb.Get,
                                RequestVerb.Patch,
                                RequestVerb.Put
                            };
                            break;

                        default:
                            verbs = new RequestVerb[] { };
                            break;
                    }
                    #endregion
                    map[suffix] = verbs;
                }
            }

            public static bool Valid(ContainmentQuerySuffix suffix, RequestVerb verb)
            {
                return map[suffix].Contains(verb);
            }
        }

        private static bool MethodAllowed(ContainmentQuery query, ContainmentQuerySuffix suffix, RequestVerb verb)
        {
            if (query.SetExpected)
            {
                if (suffix != ContainmentQuerySuffix.None)
                {
                    AstoriaTestLog.FailAndThrow("Only the NONE suffix can be used when a set is expected");
                }

                return verb == RequestVerb.Get || verb == RequestVerb.Post;
            }
            else
            {
                return VerbMap.Valid(suffix, verb);
            }
        }

        private static void UpdateErrorState(ContainmentQuery query, ContainmentQuerySuffix suffix, RequestVerb verb)
        {
            if (!MethodAllowed(query, suffix, verb))
            {
                // TODO: set error identifier?
                query.ExpectedStatusCode = HttpStatusCode.MethodNotAllowed;
                switch (verb)
                {
                    case RequestVerb.Post:
                        query.ExpectedErrorIdentifier = SystemDataServicesResourceIdentifiers.BadRequest_InvalidUriForPostOperation;
                        UriQueryBuilder builder = new UriQueryBuilder(query.Workspace, query.Workspace.ServiceEndPoint);
                        query.ExpectedErrorArgs = new object[] { builder.Build(query.Query) };
                        break;
                }
            }
        }

        private static ResourceProperty getRandomProperty(ResourceType endType, Func<ResourceProperty, bool> test)
        {
            IEnumerable<ResourceProperty> properties = getProperties(endType, test);
            int count = properties.Count();
            if (count < 1)
                return null;

            count = 1;
            return properties.ElementAt(AstoriaTestProperties.Random.Next(count));
        }

        private static ResourceProperty getRandomNavigationProperty(ResourceType endType, Multiplicity mult)
        {
            return getRandomProperty(endType,
                (p => p.IsNavigation && p.OtherAssociationEnd.Multiplicity == mult));
        }

        private static IEnumerable<ResourceProperty> getProperties(ResourceType endType, Func<ResourceProperty, bool> test)
        {
            return endType.Properties.OfType<ResourceProperty>().Where(p => test(p));
        }

        private static bool ApplyNavigationProperty(ContainmentQuery query, Multiplicity mult, bool isLink)
        {
            ResourceProperty property = getRandomNavigationProperty(query.Type, mult);
            if (property == null)
                return false;

            query.Type = property.OtherAssociationEnd.ResourceType;
            query.Container = query.Container.FindDefaultRelatedContainer(property);
            query.Query = query.Query.Nav(property.Property(), isLink);
            return true;
        }

        private static bool ApplyNavigationProperties(ContainmentQuery query, params Multiplicity[] multiplicities)
        {
            foreach (Multiplicity mult in multiplicities)
            {
                if (!ApplyNavigationProperty(query, mult, false))
                    return false;
            }
            return true;
        }

        private static bool ApplyLink(ContainmentQuery query, Multiplicity mult)
        {
            return ApplyNavigationProperty(query, mult, true);
        }

        private static bool ApplySimpleType(ContainmentQuery query)
        {
            ResourceProperty property = getRandomProperty(query.Type,
                            (p) => (!p.IsComplexType && !p.IsNavigation &&
                                    p.ForeignKeys.Count() == 0 && !p.ResourceType.Key.Contains(p)));
            if (property == null)
                return false;

            query.Type = null;
            query.Container = null;
            query.Query = query.Query.Select(property.Property());
            return true;
        }

        private static bool UpdateQuery(ContainmentQuery query, ContainmentQuerySuffix suffix, RequestVerb verb)
        {
            switch (suffix)
            {
                case ContainmentQuerySuffix.SingleNavigationTwice:
                    return ApplyNavigationProperties(query, Multiplicity.One, Multiplicity.One);

                case ContainmentQuerySuffix.SingleNavigation:
                    return ApplyNavigationProperties(query, Multiplicity.One);

                case ContainmentQuerySuffix.MultipleNavigation:
                    return ApplyNavigationProperties(query, Multiplicity.Many);

                case ContainmentQuerySuffix.LinkedSingleNavigation:
                    return ApplyLink(query, Multiplicity.One);

                case ContainmentQuerySuffix.LinkedMultipleNavigation:
                    return ApplyLink(query, Multiplicity.Many);

                case ContainmentQuerySuffix.SimpleType:
                    return ApplySimpleType(query);

                case ContainmentQuerySuffix.None:
                    return true;

                default:
                    return false;
            }
        }

        public static bool AppendSuffix(this ContainmentQuery query, ContainmentQuerySuffix suffix)
        {
            return query.AppendSuffix(suffix, RequestVerb.Get);
        }

        public static bool AppendSuffix(this ContainmentQuery query, ContainmentQuerySuffix suffix, RequestVerb verb)
        {
            UpdateErrorState(query, suffix, verb);
            return UpdateQuery(query, suffix, verb);
        }
    }
}
