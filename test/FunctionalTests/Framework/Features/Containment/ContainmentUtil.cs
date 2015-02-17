//---------------------------------------------------------------------
// <copyright file="ContainmentUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator
using System.Data.Test.Astoria.ReflectionProvider;

// TODO: different folder? Reorganize all the _____Util.cs files?

namespace System.Data.Test.Astoria
{
    public static class ContainmentUtil
    {
        public static bool AbbreviateCanonicalPath = false;

        public static KeyExpression GetContainingKey(this ContainmentAttribute att, KeyExpression childKey)
        {
            return att.GetContainingKey(childKey, att.ParentContainer.BaseType);
        }

        public static KeyExpression GetContainingKey(this ContainmentAttribute att, KeyExpression childKey, bool abbreviate)
        {
            return att.GetContainingKey(childKey, att.ParentContainer.BaseType, abbreviate);
        }

        public static KeyExpression GetContainingKey(this ContainmentAttribute att, KeyExpression childKey, ResourceType parentType)
        {
            return att.GetContainingKey(childKey, parentType, AbbreviateCanonicalPath);
        }

        public static KeyExpression GetContainingKey(this ContainmentAttribute att, KeyExpression childKey, ResourceType parentType, bool abbreviate)
        {
            AstoriaTestLog.Compare(childKey.ResourceContainer == att.ChildContainer,
                String.Format("ChildKey does not belong to expected set (Expected '{0}', got '{1}'", att.ChildContainer.Name, childKey.ResourceContainer.Name));

            List<PropertyExpression> parentProperties = new List<PropertyExpression>();
            List<ConstantExpression> parentValues = new List<ConstantExpression>();

            foreach (NodeProperty p_prop in att.ParentContainer.BaseType.Key.Properties)
            {
                string c_name;
                if (!att.KeyMapping.TryGetValue(p_prop.Name, out c_name))
                    AstoriaTestLog.FailAndThrow(String.Format("Parent key property {0} does not appear in derived key mapping", p_prop.Name));

                // need to get the offset now
                int c_offset = 0;
                for (; c_offset < childKey.Properties.Length; c_offset++)
                {
                    if (childKey.Properties[c_offset].Name == c_name)
                        break;
                }
                if (c_offset >= childKey.Properties.Length)
                    AstoriaTestLog.FailAndThrow(String.Format("Could not find property '{0}' in child key", c_name));

                NodeProperty c_prop = childKey.Properties[c_offset];

                parentProperties.Add(p_prop.Property());
                parentValues.Add(new ConstantExpression(childKey.Values[c_offset]));

                if (abbreviate)
                    childKey.IncludeInUri[c_offset] = false;
            }

            return new KeyExpression(att.ParentContainer, parentType, parentProperties.ToArray(), parentValues.ToArray());
        }

        public struct AccessPathSegment
        {
            public ContainmentAttribute Attribute;
            public KeyExpression ParentKey;
        }

        public static IEnumerable<ContainmentAttribute> GetContainmentAttributes(ServiceContainer sc)
        {
            return sc.Facets.Attributes.OfType<ContainmentAttribute>();
        }

        public static IEnumerable<ContainmentAttribute> GetContainmentAttributes(KeyExpression keyExp)
        {
            return GetContainmentAttributes(keyExp.ResourceContainer.Workspace.ServiceContainer);
        }

        public static IEnumerable<ContainmentAttribute> GetContainmentAttributes(ServiceContainer sc, Func<ContainmentAttribute,bool> predicate)
        {
            return GetContainmentAttributes(sc).Where(predicate);
        }

        public static IEnumerable<ContainmentAttribute> GetContainmentAttributes(KeyExpression keyExp, Func<ContainmentAttribute,bool> predicate)
        {
            return GetContainmentAttributes(keyExp.ResourceContainer.Workspace.ServiceContainer, predicate);
        }

        public static List<AccessPathSegment> BuildAccessPath(KeyExpression keyExp)
        {
            return BuildAccessPath(keyExp, true);
        }

        // gets the path down to the set containing the given keyexpression
        public static List<AccessPathSegment> BuildAccessPath(KeyExpression keyExp, bool requireCanonical)
        {
            // grab all the canonical path attributes
            IEnumerable<ContainmentAttribute> attributes;
            if(requireCanonical)
                attributes = GetContainmentAttributes(keyExp, ca => ca.Canonical);
            else
                attributes = GetContainmentAttributes(keyExp);

            // now we need to link them together, first by getting the SINGLE canonical path attribute with the current
            // resource container as the child, and moving up
            List<AccessPathSegment> path = new List<AccessPathSegment>();
            ResourceContainer current = keyExp.ResourceContainer;
            bool done = !attributes.Any();
            while (!done)
            {
                IEnumerable<ContainmentAttribute> applicable = attributes.Where(ca => ca.ChildContainer == current);
                if (!applicable.Any())
                    done = true;
                else
                {
                    ContainmentAttribute att = applicable.Where(ca => ca.Canonical).FirstOrDefault();
                    if (att == null)
                        att = applicable.FirstOrDefault();

                    if (att == null || (att.TopLevelAccess && !requireCanonical))
                        done = true;
                    else
                    {
                        AccessPathSegment segment = new AccessPathSegment();
                        segment.Attribute = att;
                        keyExp = att.GetContainingKey(keyExp);
                        segment.ParentKey = keyExp;
                        path.Add(segment);

                        current = att.ParentContainer;
                    }
                }
            }

            path.Reverse();

            return path;
        }


        
        public static QueryNode BuildCanonicalQuery(KeyExpression keyExp)
        {
            return BuildQuery(keyExp, true);
        }

        public static QueryNode BuildCanonicalQuery(KeyExpression keyExp, bool setOnly)
        {
            return BuildQuery(keyExp, setOnly, true);
        }

        public static QueryNode BuildQuery(KeyExpression keyExp, bool requireCanonical)
        {
            return BuildQuery(keyExp, BuildAccessPath(keyExp, requireCanonical));
        }

        public static QueryNode BuildQuery(KeyExpression keyExp, bool setOnly, bool requireCanonical)
        {
            return BuildQuery(keyExp, BuildAccessPath(keyExp, requireCanonical), setOnly);
        }

        // TODO: rename or add parameters, not necessarily canonical all the time
        public static QueryNode BuildQuery(KeyExpression keyExp, List<AccessPathSegment> path)
        {
            return BuildQuery(keyExp, path, false);
        }

        public static QueryNode BuildQuery(KeyExpression keyExp, List<AccessPathSegment> path, bool setOnly)
        {
            QueryNode query = null;
            if (path.Count() == 0)
                query = Query.From(Exp.Variable(keyExp.ResourceContainer));
            else
            {
                query = Query.From(Exp.Variable(path.First().Attribute.ParentContainer));
                foreach (AccessPathSegment segment in path)
                {
                    query = query.Where(segment.ParentKey);
                    query = query.Nav(segment.Attribute.ParentNavigationProperty.Property());
                }
            }
            if(!setOnly)
                query = query.Where(keyExp);

            return query;
        }

        public static KeyExpressions GetAllExistingKeys(Workspace w, ResourceContainer resourceContainer, ResourceType resourceType)
        {
            IEnumerable<ContainmentAttribute> attributes = GetContainmentAttributes(w.ServiceContainer,
                ca => ca.ChildContainer == resourceContainer);

            // should we pick one path at random? just use the first? determine which is shortest?
            ContainmentAttribute att = attributes.FirstOrDefault();

            if (att == null)
            {
                QueryNode query = Query.From(
                                      Exp.Variable(resourceContainer))
                                     .Select();
                if (resourceType != null)
                    query = query.OfType(resourceType);
                return w.GetAllExistingKeys(query, resourceContainer);
            }

            // recursively get all the parent container's keys
            // since we don't know the exact parent type, don't use one
            //
            KeyExpressions parentKeys = GetAllExistingKeys(w, att.ParentContainer, null);

            // we're doing some extra work by re-determining the parent key's access path,
            // but it would be hard to keep it around
            //
            KeyExpressions childKeys = new KeyExpressions();
            foreach (KeyExpression parentKey in parentKeys)
            {
                // we don't necessarily need a canonical path
                QueryNode q = BuildQuery(parentKey, false);
                q = q.Nav(att.ParentNavigationProperty.Property()).Select();
                if (resourceType != null)
                    q = q.OfType(resourceType);
                foreach (KeyExpression childKey in w.GetAllExistingKeys(q, resourceContainer))
                    childKeys.Add(childKey);
            }
            return childKeys;
        }
    }
}
