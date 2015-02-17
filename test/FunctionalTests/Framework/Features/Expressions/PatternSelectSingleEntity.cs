//---------------------------------------------------------------------
// <copyright file="PatternSelectSingleEntity.cs" company="Microsoft">
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
    /// Actual verification class for Select Single entity Query Pattern
    /// Randomly chooses one entity eg: Customers("ALFKI") and verifies the expression tree
    /// 
    /// </summary>
    public class PatternSelectSingleEntity : AbstractExprTreePattern
    {
        //Dictionary to hold the nodes and its values
        Dictionary<String, List<String>> nodeValues = new Dictionary<String, List<String>>();
        //Hold the reference keys that will appear as orderby/thenby nodes in the tree
        List<ResourceProperty> refKeys = new List<ResourceProperty>();

        public override void Build(QueryTreeInfo queryTreeInfo)
        {
            queryTreeInfo.queryNodeTree = queryTreeInfo.rootQueryNodeTree.Where(queryTreeInfo.wkspc.GetRandomExistingKey(queryTreeInfo.resourceContainer));
            //TODO: What if there is no entity that you can select?
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

            if (queryTreeInfo.currentTree.Contains(queryTreeInfo.rootTree))
            {
                // Obtain the diff tree. The +1 is for the "." that is the node separator
                queryTreeInfo.diffTree = queryTreeInfo.currentTree.Substring(queryTreeInfo.rootTree.Length + 1);
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
            List<String> whereList = new List<String>();
            
            

            //Look through each node and construct the dictionary
            foreach (String s in result)
            {
                if (s.StartsWith("Where"))
                {
                    //Can have multiple where nodes, so just create the list of values here and add it to dictionary later
                    
                    String tempStr = s.Split(new String[] { "Where(" }, StringSplitOptions.RemoveEmptyEntries)[0];

                    //Dont add where(p => true) occurances
                    //TODO: Confirm if its ok to ignore where(p=>true) nodes.
                    if (!(tempStr.Contains("p => True")))
                    {
                        whereList.Add(tempStr);
                    }
                    else
                    {
                        AstoriaTestLog.WriteLine("Where(p => true) node found, ignoring the node");
                    }
                }
            }

                       

            //Add the where list to the dictionary
            if (!nodeValues.ContainsKey("Where"))
            {
                if (whereList.Count > 0)
                {
                    nodeValues.Add("Where", whereList);
                }
                if ((refKeys.Count) != whereList.Count)
                {
                    
                    //Where node count doesnt match
                    AstoriaTestLog.WriteLine("Where nodes count doesnt match");
                    return false;
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
            List<String> outVal = null;
            bool result = nodeValues.TryGetValue("Where", out outVal);
            
            //Check for the where key values
            for (int i = 0; i < refKeys.Count; i++)
            {
                bool valuePresent = false;
                foreach (String s in outVal)
                {
                    if (s.Contains("element." + refKeys[i].Name))
                    {
                        valuePresent = true;
                    }
                }
                if (!valuePresent)
                {
                    //RefKey not found in the list
                    AstoriaTestLog.WriteLine("RefKey not found in the  list");
                    return false;
                }
            }
            return true;
        }
    }

  
}
