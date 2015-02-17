//---------------------------------------------------------------------
// <copyright file="PatternTopSkipOrderBy.cs" company="Microsoft">
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
    /// Actual verification class for the Top / Skip / Orderby Query Pattern
    /// These 3 are clubbed together since all 3 create the same expression tree 
    /// that contains orderby, thenby nodes and in addition take/skip node as applicable
    /// 
    /// </summary>
    public class PatternTopSkipOrderBy : AbstractExprTreePattern
    {
        //TODO: randomize this value
        public int takeSkipValue=1;
        //Dictionary to hold the nodes and its values
        Dictionary<String, List<String>> nodeValues = new Dictionary<String, List<String>>();
        //Hold the reference keys that will appear as orderby/thenby nodes in the tree
        List<ResourceProperty> refKeys = new List<ResourceProperty>();

        public override void Build(QueryTreeInfo queryTreeInfo)
        {
            switch (queryTreeInfo.queryComponent)
            {
                case QueryComponent.Top:
                    {
                        queryTreeInfo.queryNodeTree = queryTreeInfo.rootQueryNodeTree.Top(takeSkipValue);
                        break;
                    }
                case QueryComponent.Skip:
                    {
                        queryTreeInfo.queryNodeTree = queryTreeInfo.rootQueryNodeTree.Skip(takeSkipValue);
                        break;
                    }
                case QueryComponent.OrderBy:
                    {
                        //ORDERBY
                        List<PropertyExpression> keyProperties = new List<PropertyExpression>();
                        ResourceType resourceType = queryTreeInfo.resourceContainer.BaseType;
                        PropertyExpression[] propValues = resourceType.Key.Properties.Select(p => p.Property()).ToArray();
                        queryTreeInfo.queryNodeTree = queryTreeInfo.rootQueryNodeTree.Sort(propValues, true);
                        queryTreeInfo.queryComponent = QueryComponent.OrderBy;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        protected override bool VerifyNodes(QueryTreeInfo queryTreeInfo)
        {
            //Find the list of keys for this resource set that will appear as orderby/thenby nodes in the tree
            
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

            if (queryTreeInfo.currentTree.Contains(queryTreeInfo.rootTree))
            {
                // Obtain the diff tree. The +1 is for the "." that is the node separator
                queryTreeInfo.diffTree= queryTreeInfo.currentTree.Substring(queryTreeInfo.rootTree.Length + 1);
                AstoriaTestLog.WriteLine("Diff Tree : " + queryTreeInfo.diffTree);
            }
            //Construct the Tree Dictionary

            String[] splitString = new String[] { ")." };
            String[] result = queryTreeInfo.diffTree.Split(splitString, StringSplitOptions.None);

            AstoriaTestLog.WriteLine("Split string");
            foreach (String s in result) 
            { 
                AstoriaTestLog.WriteLine(s); 
            }

            
            //ThenBy and Where nodes can repeat, so create a list to capture the multiple values
            List <String> thenByList = new List<String>();
            List<String> whereList = new List<String>();
            String expectedNode = null;

            //Based on the queryComponent, identify the node we should look for in the tree
            if (queryTreeInfo.queryComponent == QueryComponent.Top)
            {
                expectedNode = "Take";
            }
            else if (queryTreeInfo.queryComponent == QueryComponent.Skip)
            {
                expectedNode = "Skip";
            }

            //Look through each node and construct the dictionary
            foreach (String s in result)
            {
                if (s.StartsWith("OrderBy"))
                {
                    if (!nodeValues.ContainsKey("OrderBy"))
                    {
                        nodeValues.Add("OrderBy", new List<String>(s.Split(new String[] { "OrderBy(" }, StringSplitOptions.RemoveEmptyEntries)));
                    }
                    else
                    {
                        //OrderBy found again, return error
                        AstoriaTestLog.WriteLine("More than 1 OrderBy node found");
                        return false;
                    }

                }
                if (s.StartsWith("ThenBy"))
                {
                    //Can have multiple thenby nodes, so just create the list of values here and add it to dictionary later
                    thenByList.Add((s.Split(new String[] { "ThenBy(" }, StringSplitOptions.RemoveEmptyEntries)[0]));
                }

                //Check for take/skip
                if (expectedNode != null)
                {
                    if (s.StartsWith(expectedNode))
                    {
                        if (!nodeValues.ContainsKey(expectedNode))
                        {
                            nodeValues.Add(expectedNode, new List<String>(s.Split(new String[] { expectedNode + "(", ")" }, StringSplitOptions.RemoveEmptyEntries)));
                        }
                        else
                        {
                            //Node found again, return error
                            AstoriaTestLog.WriteLine("More than 1 " + expectedNode + " node found");
                            return false;
                        }
                    }
                }
                if (s.StartsWith("Where"))
                {
                    //Can have multiple where nodes, so just create the list of values here and add it to dictionary later
                    whereList.Add((s.Split(new String[] { "Where(" }, StringSplitOptions.RemoveEmptyEntries)[0]));
                }
            }

            //Verify that the correct number of orderby/thenby/take nodes are present

            //Check for take/skip
            if (expectedNode != null)
            {
                //Is Take/Skip node present
                if (!nodeValues.ContainsKey(expectedNode))
                {
                    //Take/Skip node not found
                    AstoriaTestLog.WriteLine(expectedNode + " node not found");
                    return false;
                }
            }

            //Is OrderBy node present?
            if (!nodeValues.ContainsKey("OrderBy"))
            {
                //OrderBy node not found
                AstoriaTestLog.WriteLine("OrderBy node not found");
                return false;
            }

            //Add the theyby list to the dictionary and Verify the count of ThenBy nodes
            if (!nodeValues.ContainsKey("ThenBy"))
            {
                if (thenByList.Count > 0)
                {
                    nodeValues.Add("ThenBy", thenByList);
                }
                if ((refKeys.Count-1) != thenByList.Count)
                {
                    //ThenBy node count doesnt match
                    AstoriaTestLog.WriteLine("ThenBy nodes count doesnt match");
                    return false;
                }
            }

            //Add the where list to the dictionary
            if (!nodeValues.ContainsKey("Where"))
            {
                if (whereList.Count > 0)
                {
                    nodeValues.Add("Where", whereList);
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
            List <String> outVal = null;
            bool result = false;
            if (queryTreeInfo.queryComponent == QueryComponent.Top)
            {
                result = nodeValues.TryGetValue("Take", out outVal);
            }
            else if (queryTreeInfo.queryComponent == QueryComponent.Skip)
            {
                result = nodeValues.TryGetValue("Skip", out outVal);
            }
            

            //Check for take/skip value
            if (result)
            {
                if (!(takeSkipValue.ToString() == outVal[0]))
                {
                    //Take node value does not match
                    AstoriaTestLog.WriteLine("Take/Skip node value does not match");
                    return false;
                }
            }
            else
            {
                if (queryTreeInfo.queryComponent != QueryComponent.OrderBy)
                {
                    //Take node not found in the list
                    AstoriaTestLog.WriteLine("Take/Skip node not found");
                    return false;
                }
            }

            //Check for the orderby/thenby key values
            for (int i = 0; i < refKeys.Count ; i++ )
            {
                
                bool keyPresent;
                List<String> outOrderByVal = null;
                List<String> outThenByVal = null;
                //Must find orderby node
                keyPresent = nodeValues.TryGetValue("OrderBy", out outOrderByVal);
                if (outVal != null)
                {
                    outVal.Clear();
                    outVal.AddRange(outOrderByVal);
                }
                else
                {
                    outVal = outOrderByVal;
                }

                if (keyPresent)
                {
                    //thenby nodes are optional
                    nodeValues.TryGetValue("ThenBy", out outThenByVal);
                    if (outThenByVal != null)
                    {
                        outVal.AddRange(outThenByVal);
                    }
                }
                else
                {
                    //OrderBy/ThenBy node not found
                    AstoriaTestLog.WriteLine("OrderBy/ThenBy node not found");
                    return false;
                }
                
                //Verify the values
                if (keyPresent)
                {
                    bool valuePresent = false;
                    foreach (String s in outVal)
                    {
                        if(s.Contains("element." + refKeys[i].Name))
                        {
                            valuePresent = true;
                        }
                    }
                    if(!valuePresent)
                    {
                        //RefKey not found in the list
                        AstoriaTestLog.WriteLine("RefKey not found in the  list");
                        return false;
                    }
                }
                

            }
            return true;
        }
    }

}
