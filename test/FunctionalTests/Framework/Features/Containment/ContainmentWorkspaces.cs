//---------------------------------------------------------------------
// <copyright file="ContainmentWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public class ContainmentWorkspaces : FeatureWorkspaces
    {
        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // TODO: need a way to make sure all containers exist

            if (!attribute.Name.Equals("Aruba", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // TODO: allow inmemorylinq once it is working
            if (attribute.DataLayerProviderKind != DataLayerProviderKind.Edm)
                return false;

            return true;
        }
        protected override void WorkspaceCallback(Workspace workspace)
        {
            ServiceContainer sc = workspace.ServiceContainer;
            Nodes<ResourceContainer> containers = sc.ResourceContainers;
            ResourceContainer builds = containers["Builds"];
            ResourceContainer run1s = containers["Run1s"];
            ResourceContainer run2s = containers["Run2s"];
            ResourceContainer run3s = containers["Run3s"];
            ResourceContainer test1s = containers["Test1s"];
            ResourceContainer test2s = containers["Test2s"];
            ResourceContainer test3s = containers["Test3s"];
            ResourceContainer test4s = containers["Test4s"];
            ResourceContainer test5s = containers["Test5s"];
            ResourceContainer test6s = containers["Test6s"];
            ResourceContainer test7s = containers["Test7s"];
            ResourceContainer test8s = containers["Test8s"];
            ResourceContainer test9s = containers["Test9s"];

            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(builds, run1s, true)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run1s, test1s, true)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run1s, test2s)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run1s, test3s, false)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(builds, run2s)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run2s, test4s, true)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run2s, test5s)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run2s, test6s, false)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(builds, run3s, false)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run3s, test7s, true)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run3s, test8s)));
            sc.Facets.Add(NodeFacet.Attribute(new ContainmentAttribute(run3s, test9s, true)));
        }
    }
}
