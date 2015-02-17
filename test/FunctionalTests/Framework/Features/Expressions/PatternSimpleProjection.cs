//---------------------------------------------------------------------
// <copyright file="PatternSimpleProjection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Data.Test.Astoria.Expressions
{
    
  
    /// <summary>
    /// Actual verification class for Simple projection Query Pattern
    /// Randomly chooses a property and projects on that property. eg: $select=name
    /// 
    /// </summary>
    public class PatternSimpleProjection : AbstractExprTreePattern
    {
        //Dictionary to hold the nodes and its values
        Dictionary<String, List<String>> nodeValues = new Dictionary<String, List<String>>();
        //Hold the reference keys that will appear as orderby/thenby nodes in the tree
        List<ResourceProperty> refKeys = new List<ResourceProperty>();

        public override void Build(QueryTreeInfo queryTreeInfo)
        {
            List<ResourceType> resourceTypes = queryTreeInfo.resourceContainer.ResourceTypes.ToList();
            List<ResourceProperty> validProperties = resourceTypes
                        .SelectMany(rt => rt.Properties.OfType<ResourceProperty>())
                        .ToList();

            queryTreeInfo.queryNodeTree = queryTreeInfo.rootQueryNodeTree.New(validProperties[0].Property());
        }

        protected override bool VerifyNodes(QueryTreeInfo queryTreeInfo)
        {
            //Find the list of keys for this resource set that will appear as where nodes in the tree

            foreach (ResourceType type in queryTreeInfo.resourceContainer.ResourceTypes)
            {
                foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>())
                {
                    if (property.PrimaryKey != null)
                    {
                        refKeys.Add(property);
                        AstoriaTestLog.WriteLine(property.Name);
                    }
                }
            }

            return true;
        }
        protected override bool VerifyMetaData(QueryTreeInfo queryTreeInfo)
        {
            // No metadata to verify for this pattern
            return true;
        }
        protected override bool VerifyLambda(QueryTreeInfo queryTreeInfo)
        {
            XmlDocument currentTreeXml = queryTreeInfo.currentTreeXml;
            XmlNodeList projectedNodes = null;
            
            // Find the nodes that contain the projected properties
            if (queryTreeInfo.wkspc.DataLayerProviderKind == DataLayerProviderKind.Edm)
            {
                projectedNodes = currentTreeXml.SelectNodes("/Call/Arguments/Quote/Lambda/Body/MemberInit/MemberAssignment");
            }
            else if (queryTreeInfo.wkspc.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq)
            {
                projectedNodes = currentTreeXml.SelectNodes("/Call/Arguments/Quote/Lambda/Body/Conditional/False/MemberInit/MemberAssignment");
            }
            if (projectedNodes.Count == 0)
            {
                AstoriaTestLog.WriteLine("Projected nodes not found");
                return false;
            }
            
            // Verify all the projected properties in the node
            foreach (XmlNode node in projectedNodes)
            {
                if (node.Attributes[0].Value == "PropertyNameList")
                {
                    List<String> projectedProps = node.SelectSingleNode("./Constant").InnerText.Split(new char[] { ',' }).ToList();
                    if (projectedProps.Count != refKeys.Count)
                    {
                        AstoriaTestLog.WriteLine("Projected props count does not match refkeys");
                        return false;
                    }
                    foreach (ResourceProperty rp in refKeys)
                    {
                        if (!projectedProps.Contains(rp.Name))
                        {
                            AstoriaTestLog.WriteLine("Projected props is not a refkey");
                            return false;
                        }

                    }
                }
            }           
            return true;
        }
    }
}
