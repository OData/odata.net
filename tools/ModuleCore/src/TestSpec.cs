//---------------------------------------------------------------------
// <copyright file="TestSpec.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;		//Hashtable
using System.Xml;				//XmlDocument

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestSpec
	//
	////////////////////////////////////////////////////////////////
	public class TestSpec
	{
		//Data
		internal  TestXmlDocument   _xmldoc;
		protected TestModule	    _testmodule;

		//Constructor
		public TestSpec(TestModule testmodule)
		{
			_testmodule = testmodule;
		}

		public void					ApplyFilter(TestItem testmodule, string xpathquery)
		{
            try
            {
                //Delegate to XPath
                XmlNodeList xmlnodes = this.XmlDocument.SelectNodes(xpathquery);
			    if(xmlnodes.Count > 0)
			    {
				    //Build a (object indexable) hashtable of all found items
				    Hashtable found = new Hashtable();
				    foreach(XmlNode xmlnode in xmlnodes)
				    {
					    TestItem item = FindMatchingNode(xmlnode);
					    if(item != null)
						    found[item] = xmlnode;
				    }

				    //If the entire testmodule was selected as part of the filter, were done.
				    //(as all children are implicitly included if the parent is selected)
				    if(!found.Contains(testmodule))
                        ApplyFilter(testmodule, found);
			    } 
			    else
			    {
			        //No results
				    testmodule.Children.Clear();
			    }
            }
			catch(Exception e)
			{
				//Make this easier to debug
				throw new TestFailedException(
                    testmodule.Name + " failed to apply filter: \"" + xpathquery + "\"",
                    xpathquery, 
                    null,
                    e
                    );
			}
		}
		
        public void                 ApplyFilter(TestItem testItem, Hashtable found)
        {
			// Remove all test items not found in the list.
			for(int i=0; i<testItem.Children.Count; i++)
			{
				//If the entire test item was selected as part of the filter, we are done.
				//(as all children are implicitly included if the parent is selected)
                TestItem child = (TestItem)testItem.Children[i];
                if (!found.Contains(child))
                {
                    //If this is a leaf, then remove
                    if (child.Children.Count <= 0)
                    {
                        testItem.Children.Remove(child);
                        i--;
                    }
                    else
                    {
                        //Otherwise, check its children
                        ApplyFilter(child, found);

                        //If no test item children are left, (and since the test item wasn't on the list),
                        //then the test item shouldn't be removed as well
                        if (child.Children.Count <= 0)
                        {
                            testItem.Children.Remove(child);
                            i--;
                        }
                    }
                }
            }
        }

		protected TestItem			FindMatchingNode(XmlNode xmlnode)
		{
			//Matching nodes are always elements (testmodule, testcase, variation)
            //Those nodes have item numbers...
            XmlNode parent = xmlnode;
			while(parent != null)
            {
                if(parent is TestXmlElement && ((TestXmlElement)parent).Item != null)
                    break;
				parent = parent.ParentNode;
            }

			//Note: We actually placed a uniqueid in the XmlElement node itself (derived node type)
			TestXmlElement found = parent as TestXmlElement;
			if(found != null)
                return found.Item;
			
            //Otherwise, not found
			return null;
		}
		
		public XmlDocument			XmlDocument
		{
			get
			{
				//Deferred creation
				if(_xmldoc == null)
					this.CreateSpec();
				return _xmldoc;
			}
		}
		
		protected void				CreateSpec()
		{
			//Note: We want both API and (xml) DataDriven tests to all to filter
			//We need to expose our attributes, (and testcase information) in a similar
			//xml structure so we can leverage xpath queries (not redesign our own filtering syntax),
			//plus this allows both teams to have a identical (similar as possible) queries/filters.
			
			//Xml Spec Format:
			//	Example:
			/*
				<TestModule Name="Functional" Created="10 October 2001" Modified="10 October 2001" Version="1">
				<-- Owner type is an enum, "test", "dev" or "pm" -->
				<Owner Alias="YourAlias" Type="test"/>
				<Description>XQuery conformance tests</Description>
					<Data filePath="\\webxtest\wdtest\managed\..." DBName="Northwind" Anything="whatever you want to be global">
						<!--My global data -->
						<xml>http://webdata/data/mytest/test.xml</xml>
						<xsd>http://webdata/data/mytest/test.xsd</xsd>
					</Data>
					<TestCase name="FLWR Expressions">
						   <Description>Tests for FLWR expressions</description>
						<Variation id="1" Implemented="true" Priority="2">
							<Description>Simple 1 FLWR expression</description>
							<FilterCriteria>  
											<!-- Recommended place for filter criteria -->
											<Os>NT</Os>
											<Language>English</Language>
							</FilterCriteria>
							<Data >
											<!-- Override global data -->
											<xml>http://webdata/data/mytest/special_test.xml</xml>
							</Data>
							<SoapData>  
											<!-- Additional data for SOAP tests -->
											<wsdl>http://webdata/data/mytest/test.wsdl</wsdl>
							</SoapData>  
	  					</Variation>
    					</TestCase>
				</TestModule>
			*/

			//Create the document
			_xmldoc	= new TestXmlDocument();
            
			//Add the module (from the root)
			this.AddChild(_xmldoc, _testmodule);
		}
		
		protected void			AddChild(XmlNode parent, TestItem node)
        {
            //<TestModule/TestCase/Variation>
            string name = node.Type.ToString();

            //Create the Element
			TestXmlElement element	= (TestXmlElement)_xmldoc.CreateElement(name);
			parent.AppendChild(element);
			
			//Add typed properties
			//TODO: Should these also be part of the properties collection, or not in the interface
			AddProperty(element, "Name",	node.Name,		0);
			AddProperty(element, "Desc",	node.Desc,		0);
			AddProperty(element, "Id",		node.Order,		0);
//			AddProperty(element, "Guid",	node.Guid,		0);
			AddProperty(element, "Priority",node.Priority,	0);
			AddProperty(element, "Owner",	node.Owner,		0);
			AddProperty(element, "Owners",	node.Owners,	TestPropertyFlags.MultipleValues);
			AddProperty(element, "Version",	node.Version,	0);

			//Add extended properties
			AddProperties(element, node);

			//Add our own uniqueid (for fast assoication later)
			//Note: We place this 'userdata' into our own version of the XmlElement, (derived class)
            element.Item = node;

			//Recursure through the children
			foreach(TestItem childnode in node.Children)
				this.AddChild(element, childnode);
		}
		
		protected void			AddProperties(XmlNode element, TestItem node)
		{
			//Obtain ALL the meta-data from the test.  This is stored in the properties
			//collection, which is at least all the attribute data, (plus potentially more)
			foreach(TestProperty property in node.Metadata)
				this.AddProperty(element, property.Name, property.Value, property.Flags);
		}

		protected void			AddProperty(XmlNode element, string name, object value, TestPropertyFlags flags)
		{
			//Ignore all the properties that have no name or value (as to not bloat the xml)
			if(name == null || value == null)
				return;
				
			//How to serialize (elements or attributes)
			if((flags & TestPropertyFlags.MultipleValues) != 0)
			{
				//Property as Element
				AddValue(element, name, value);
			}
			else
			{
				//Property as Attribute
				XmlAttribute xmlattribute  = _xmldoc.CreateAttribute(name);
				xmlattribute.Value         = StringEx.ToString(value);
				element.Attributes.Append( xmlattribute );
			}
		}

		protected void			AddValue(XmlNode element, string name, object value)
		{
			//Recurise through the value(s)
			if(value != null && value.GetType().HasElementType && value is IEnumerable)
			{
				//Recurse through the values
				foreach(object item in (IEnumerable)value)
				   AddValue(element, name, item);
			}
			else
			{
				//<node>value</node>
				XmlElement child = _xmldoc.CreateElement(name);
				child.InnerText = StringEx.ToString(value);
				element.AppendChild(child);
			}
		}
	}

	////////////////////////////////////////////////////////////////
	// TestXmlDocument
	//
	////////////////////////////////////////////////////////////////
	internal class TestXmlDocument : XmlDocument
	{
		public override XmlElement CreateElement(string prefix, string name, string namespaceURI)
		{
			return new TestXmlElement(prefix, name, namespaceURI, this);
		}		
	}
	
	////////////////////////////////////////////////////////////////
	// TestElement
	//
	////////////////////////////////////////////////////////////////
	internal class TestXmlElement : XmlElement
	{
		//Data
		internal TestItem     Item;
		
		//Constructor
		public TestXmlElement(string prefix, string name, string namespaceURI, TestXmlDocument xmldoc)
			: base(prefix, name, namespaceURI, xmldoc)
		{
		}		
	}
}