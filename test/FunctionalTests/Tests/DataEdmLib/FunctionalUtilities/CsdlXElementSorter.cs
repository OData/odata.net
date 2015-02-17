//---------------------------------------------------------------------
// <copyright file="CsdlXElementSorter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Sorts the Csdl XElement
    ///   For any legal element, it yields a consistent order
    ///   If there are any abnormal order within an element, that abnormal order is preserved 
    /// </summary>
    public class CsdlXElementSorter
    {
        private string schemaNamespaceName = string.Empty;
        private Stack<string> elementNameStack = new Stack<string>();

        // parent -> (element name -> group)
        //   group 0 is reserved for 'Documentation'
        // Csdl has some interesting ordering requirements. For example, for EntityType, 
        //   Key must be before any property / navigation property; but the order of property / navigation property are not important. 
        // This is captured by the group number, basically if the group number are the same, the order is not important among them. 
        // Otherwise, smaller group number must come before larger group number, to be valid Csdl.
        private Dictionary<string, Dictionary<string, int>> elementOrderingGroupLookup = new Dictionary<string, Dictionary<string, int>>
        {
            { "Schema", new Dictionary<string, int> 
                        {
                            { "Association", 1 },
                            { "ComplexType", 1 },
                            { "EntityType", 1 },
                            { "EntityContainer", 1 },
                            { "Function", 1 },
                            { "EnumType", 1 },
                            { "Annotations", 1 },
                            { "Term", 1 },
                        }},
            { "Association", new Dictionary<string, int> 
                        {
                            { "End", 1 },
                            { "ReferentialConstraint", 2 },
                        }},
            { "End", new Dictionary<string, int> 
                        {
                            { "OnDelete", 1 },
                        }},
            { "ReferentialConstraint", new Dictionary<string, int> 
                        {
                            { "Principal", 1 },
                            { "Dependent", 2 },
                        }},
            { "Principal", new Dictionary<string, int> 
                        {
                            { "PropertyRef", 1 },
                        }},
            { "Dependent", new Dictionary<string, int> 
                        {
                            { "PropertyRef", 1 },
                        }},
            { "ComplexType", new Dictionary<string, int> 
                        {
                            { "Property", 1 },
                        }},
            { "EntityType", new Dictionary<string, int> 
                        {
                            { "Key", 1 },
                            { "Property", 2 },
                            { "NavigationProperty", 2 },
                            { "Annotation", 4 },
                        }},
            { "Key", new Dictionary<string, int> 
                        {
                            { "PropertyRef", 1 },
                        }},
            { "Function", new Dictionary<string, int> 
                        {
                            { "Parameter", 1 },
                            { "ReturnType", 1 },
                        }},
            { "EntityContainer", new Dictionary<string, int> 
                        {
                            { "EntitySet", 1 },
                            { "AssociationSet", 1 },
                            { "FunctionImport", 1 },
                        }},
            { "AssociationSet", new Dictionary<string, int> 
                        {
                            { "End", 1 },
                        }},
            { "FunctionImport", new Dictionary<string, int> 
                        {
                            { "Parameter", 1 },
                        }},
            { "Documentation", new Dictionary<string, int> 
                        {
                            { "Summary", 1 },
                            { "LongDescription", 2 },
                        }},
            { "RowType", new Dictionary<string, int> 
                        {
                            { "Property", 1 },
                        }},
            { "EnumType", new Dictionary<string, int> 
                        {
                            { "Member", 1 },
                        }},
            { "Annotations", new Dictionary<string, int> 
                        {
                            { "Annotation", 1 },
                        }},
            { "Record", new Dictionary<string, int> 
                        {
                            { "PropertyValue", 1 },
                        }},
            { "EntitySet", new Dictionary<string, int> 
                        {
                            { "NavigationPropertyBinding", 1 },
                        }},
        };

        private Dictionary<string, string> elementNameToSortingAttribute = new Dictionary<string, string>
        {
            { "End", "Role" },
            { "Annotation", "Term" },
            { "PropertyValue", "Property" },
            { "Annotations", "Target" },
            { "NavigationPropertyBinding", "Path" },
        };

        public XElement SortCsdl(XElement csdlElement)
        {
            if (csdlElement.Name.LocalName == "Schema")
            {
                this.schemaNamespaceName = csdlElement.Name.NamespaceName;
            }

            return this.SortChildren(csdlElement);
        }

        private XElement SortChildren(XElement parentElement)
        {
            // if not in regular Csdl xml namespace, do not sort any further
            if (parentElement.Name.NamespaceName != this.schemaNamespaceName)
            {
                return parentElement;
            }

            // sort attributes
            var sortedAttributes = parentElement.Attributes().OrderBy(a => a.Name.NamespaceName + ":" + a.Name.LocalName);

            // sort child elements
            List<XElement> sortedElements = new List<XElement>();
            foreach (var e in parentElement.Elements())
            {
                int index = this.FindInsertionIndex(e, sortedElements, parentElement.Name.LocalName);
                sortedElements.Insert(index, this.SortChildren(e));
            }

            var nonElements = parentElement.Nodes().Where(n => !(n is XElement));

            return new XElement(parentElement.Name, sortedAttributes, sortedElements, nonElements);
        }

        private int FindInsertionIndex(XElement elementToInsert, List<XElement> sortedElements, string parentElementName)
        {
            int currentIndex = sortedElements.Count - 1;
            while (currentIndex >= 0 && 
                   this.ShouldInsertBefore(elementToInsert, sortedElements[currentIndex], parentElementName))
            {
                currentIndex--;
            }

            return currentIndex + 1;
        }

        private bool ShouldInsertBefore(XElement elementToInsert, XElement elementInList, string parentElementName)
        {
            // if elementToInsert is not in regular Csdl namespace, it should always be inserted after
            if (elementToInsert.Name.NamespaceName != this.schemaNamespaceName)
            {
                return false;
            }

            // if elementToInsert is in regular Csdl namespace, but elementInList is not
            // this is abnormal order, need to insert after to preserve wrong order
            if (elementInList.Name.NamespaceName != this.schemaNamespaceName)
            {
                return false;
            }

            // now both elements are in regular Csdl namespace
            int groupOfToInsert = this.GetOrderingGroup(parentElementName, elementToInsert.Name.LocalName);
            int groupOfInList = this.GetOrderingGroup(parentElementName, elementInList.Name.LocalName);

            // for anything "unknown" (we don't want to handle), return false to perserve original ordering
            if (groupOfToInsert < 0 || groupOfInList < 0)
            {
                return false;
            }

            if (groupOfToInsert == groupOfInList)
            {
                return this.ShouldInsertBeforeWithinSameOrderingGroup(elementToInsert, elementInList);
            }

            // subtle: if groupOfToInsert < groupOfInList, it's abnormal order, need to insert after to perserve wrong order
            return false;
        }

        private int GetOrderingGroup(string parentElementName, string elementName)
        {
            if (elementName == "Documentation")
            {
                return 0;
            }

            if (!this.elementOrderingGroupLookup.ContainsKey(parentElementName))
            {
                return -1;
            }

            var subElements = elementOrderingGroupLookup[parentElementName];

            if (!subElements.ContainsKey(elementName))
            {
                return -1;
            }
            return subElements[elementName];
        }

        private bool ShouldInsertBeforeWithinSameOrderingGroup(XElement elementToInsert, XElement elementInList)
        {            
            int compareNameResult = string.CompareOrdinal(elementToInsert.Name.LocalName, elementInList.Name.LocalName);
            if (compareNameResult < 0)
            {
                return true;
            }
            else if (compareNameResult > 0)
            {
                return false;
            }

            return this.ShouldInsertBeforeBasedOnAttribute(elementToInsert, elementInList);
        }

        private bool ShouldInsertBeforeBasedOnAttribute(XElement elementToInsert, XElement elementInList)
        {
            string attributeName = this.GetSortingAttributeName(elementToInsert.Name.LocalName);

            XAttribute attributeOfToInsert = elementToInsert.Attribute(attributeName);
            XAttribute attributeOfInList = elementInList.Attribute(attributeName);
            Assert.IsNotNull(attributeOfToInsert, string.Format("{0} does not have attribute {1}", elementToInsert, attributeName));
            Assert.IsNotNull(attributeOfInList, string.Format("{0} does not have attribute {1}", elementInList, attributeName));

            int compareAttributeResult = string.CompareOrdinal(attributeOfToInsert.Value, attributeOfInList.Value);
            return compareAttributeResult <= 0;
        }

        private string GetSortingAttributeName(string elementName)
        {
            if (this.elementNameToSortingAttribute.ContainsKey(elementName))
            {
                return this.elementNameToSortingAttribute[elementName];
            }
            else
            {
                // by default, use attribute 'Name'
                return "Name";
            }
        }
    }
}
