//---------------------------------------------------------------------
// <copyright file="AtomUpdatePayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using AstoriaUnitTests.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class AtomUpdatePayloadBuilder : UpdatePayloadBuilder
    {
        public static Action<XmlElement> EntryElementCallback = null;

        //Common required hardcoded strings
        public const string DataWebXmlNamespace = "http://docs.oasis-open.org/odata/ns/data";
        public const string DataWebMetadataXmlNamespace = "http://docs.oasis-open.org/odata/ns/metadata";
        public const string DataWebRelatedXmlNamespace = "http://docs.oasis-open.org/odata/ns/related/";
        public const string DataWebSchemeNamespace = "http://docs.oasis-open.org/odata/ns/scheme";
        public const string DataWebXmlNamespacePrefix = "ads";
        public const string DataWebMetadataXmlNamespacePrefix = "adsm";
        public const string DataWebBaseNamespacePrefix = "base";
        public const string AtomXmlNamespace = "http://www.w3.org/2005/Atom";
        
        private XmlDocument document;
        private Stack<ExpNode> nodeStack;

        public AtomUpdatePayloadBuilder(Workspace workspace, RequestVerb requestVerb)
            : base(workspace, requestVerb)
        {

        }

        /// <summary>Gets the XML element for a category element in the specified <paramref name="scheme"/>.</summary>
        /// <param name="term">Category term.</param>
        /// <param name="scheme">Category scheme.</param>
        /// <returns>The XML element for the category element.</returns>
        public static System.Xml.Linq.XElement GetCategoryXElement(string term, string scheme)
        {
            System.Xml.Linq.XNamespace ns = AtomXmlNamespace;
            return new System.Xml.Linq.XElement(
                ns + "category",
                new System.Xml.Linq.XAttribute("term", term),
                new System.Xml.Linq.XAttribute("scheme", scheme));
        }

        /// <summary>Gets the XML text for a category element in the default scheme.</summary>
        /// <param name="term">Category term.</param>
        /// <returns>The XML text for the category element.</returns>
        public static string GetCategoryXml(string term)
        {
            return "<category term='" + term + "' scheme='" + DataWebSchemeNamespace + "' />";
        }

        public override string Build(ExpNode node)
        {
            nodeStack = new Stack<ExpNode>();
            document = new XmlDocument();
           // document.AppendChild(document.CreateXmlDeclaration("1.0", Encoding.UTF8.WebName, "yes"));

            this.Visit(null, node, null);
            
            StringBuilder builder = new StringBuilder();

            // settings taken from server
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            XmlWriter writer = XmlWriter.Create(builder, settings);

            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + Encoding.UTF8.WebName + "\" standalone=\"yes\"");
            document.Save(writer);
            

            return builder.ToString();
        }

        private void AddNamespacesToTopElement(XmlElement topElement)
        {
            //add xmlns:ads="http://docs.oasis-open.org/odata/ns/data"
            XmlAttribute dataWebNamespaceAttribute = document.CreateAttribute("xmlns:ads");
            dataWebNamespaceAttribute.Value = DataWebXmlNamespace;
            topElement.Attributes.Append(dataWebNamespaceAttribute);

            //add xmlns:adsm="http://docs.oasis-open.org/odata/ns/metadata"
            XmlAttribute dataWebNamespaceMetadataAttribute = document.CreateAttribute("xmlns:adsm");
            dataWebNamespaceMetadataAttribute.Value = DataWebMetadataXmlNamespace;
            topElement.Attributes.Append(dataWebNamespaceMetadataAttribute);

            //add ATOM namespace
            XmlAttribute atomNamespaceAttribute = document.CreateAttribute("xmlns");
            atomNamespaceAttribute.Value = AtomXmlNamespace;
            topElement.Attributes.Append(atomNamespaceAttribute);
        }

        private XmlAttribute CreateAtomAttribute(string name)
        {
            XmlAttribute attribute = document.CreateAttribute(name);
            return attribute;
        }
        private XmlAttribute CreateDataMetadataAttribute(string name)
        {
            //return _xmlPayloadDocument.CreateAttribute(DataWebXmlNamespacePrefix, name,DataWebXmlNamespace);
            XmlAttribute attribute = document.CreateAttribute(DataWebMetadataXmlNamespacePrefix, name, DataWebMetadataXmlNamespace);
            return attribute;
        }
        private XmlAttribute CreateDataWebMetadataAttribute(string name)
        {
            XmlAttribute attribute = document.CreateAttribute(DataWebMetadataXmlNamespacePrefix, name, DataWebMetadataXmlNamespace);
            return attribute;
        }
        private XmlElement CreateAtomElement(string name)
        {
            XmlElement element = document.CreateElement(null, name, AtomXmlNamespace);
            return element;
        }
        private XmlElement CreateDataWebElement(string name)
        {
            XmlElement element = document.CreateElement(DataWebXmlNamespacePrefix, name, DataWebXmlNamespace);
            return element;
        }
        private XmlElement CreateDataWebMetadataElement(string name)
        {
            XmlElement element = document.CreateElement(DataWebMetadataXmlNamespacePrefix, name, DataWebMetadataXmlNamespace);
            return element;
        }
        protected void Visit(ExpNode caller, ExpNode node, XmlElement parentNode)
        {
            nodeStack.Push(node);
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node is KeyedResourceInstance && (!(node is AssociationResourceInstance)))
            {
                KeyedResourceInstance e = (KeyedResourceInstance)node;
                CreateEntryElement(e, parentNode);
            }
            else if (node is AssociationResourceInstance)
            {
                AssociationResourceInstance e = (AssociationResourceInstance)node;
                if (caller == null)
                    CreateLinkPayload(e, parentNode);
                else
                    CreateBinding(e, parentNode);
            }
            //Below are two special cases
            else if (caller == null && node is ResourceInstanceSimpleProperty)
            {
                this.VisitResourceInstanceSimpleProperty(node as ResourceInstanceSimpleProperty, parentNode);
            }
            else if (caller == null && node is ResourceInstanceComplexProperty)
            {
                this.VisitResourceInstanceComplexProperty(node as ResourceInstanceComplexProperty, parentNode);
            }
            else if (caller == null && node is ResourceInstanceNavRefProperty)
            {
                ResourceInstanceNavRefProperty navRef = node as ResourceInstanceNavRefProperty;
                AssociationResourceInstance associationResourceInstance = navRef.TreeNode as AssociationResourceInstance;
                if (associationResourceInstance != null && associationResourceInstance.Operation == AssociationOperation.Remove)
                {
                    this.CreateUnBinding(parentNode);
                }
                else
                    throw new Exception("Unknown node type:" + navRef.TreeNode.GetType());
            }
            else if (caller == null && node is ResourceInstanceCollection)
            {
                ResourceInstanceCollection collection = node as ResourceInstanceCollection;
                CreateLinksElement(collection, null);

            }
            else
            {
                throw new Exception("Unknown node type: " + node.GetType());
            }
            nodeStack.Pop();
        }

        private XmlElement CreateBasicEntryElement(KeyedResourceInstance keyedResourceInstance, XmlNode parentNode)
        {
            XmlElement entryElement = CreateAtomElement("entry");
            if (parentNode == null)
            {
                AddNamespacesToTopElement(entryElement);
                document.AppendChild(entryElement);
            }
            else
            {
                parentNode.AppendChild(entryElement);
            }

            //add type attribute
            if (keyedResourceInstance.IncludeTypeMetadataHint)
            {
                string typeName = null;

                //if( this.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr )
                //    typeName = keyedResourceInstance.ResourceTypeName;
                //else
                typeName = this.Workspace.ContextNamespace + "." + keyedResourceInstance.TypeName;

                XmlElement typeElement = CreateAtomElement("category");
                typeElement.SetAttribute("scheme", DataWebSchemeNamespace);
                typeElement.SetAttribute("term", typeName);
                entryElement.AppendChild(typeElement);
            }

            return entryElement;
        }

        private void CreateBinding(AssociationResourceInstance keyedResourceInstance, XmlNode parentNode)
        {
            XmlElement entryElement = CreateBasicEntryElement(keyedResourceInstance, parentNode);

            XmlElement idNode = CreateIdElement(keyedResourceInstance);
            entryElement.AppendChild(idNode);
        }
        /// <summary>
        /// Creates binding for $ref scenario
        /// </summary>
        /// <param name="associationResourceInstance"></param>
        /// <returns></returns>
        private void CreateLinkPayload(AssociationResourceInstance associationResourceInstance, XmlElement parentNode)
        {
            XmlElement bindElement;
            bindElement = this.CreateDataWebMetadataElement("ref");

            if (associationResourceInstance.Operation == AssociationOperation.Add)
                bindElement.InnerText = CreateCanonicalUri(associationResourceInstance.ResourceInstanceKey);
            else
            {
                XmlAttribute nullAttribute = CreateDataWebMetadataAttribute("null");
                nullAttribute.Value = "true";
                bindElement.Attributes.Append(nullAttribute);
            }

            if (parentNode == null)
                document.AppendChild(bindElement);
            else
                parentNode.AppendChild(bindElement);
        }
        private void CreateLinksElement(ResourceInstanceCollection collection, XmlElement parentNode)
        {
            XmlElement linksElement = this.CreateDataWebMetadataElement("links");

            if (collection.NodeList.Count == 0)
            {
                Console.WriteLine("got here");
            }
            foreach (ResourceBodyTree tree in collection.NodeList)
            {
                AssociationResourceInstance associationResourceInstance = tree as AssociationResourceInstance;
                if (associationResourceInstance == null)
                {
                    throw new Microsoft.Test.ModuleCore.TestFailedException("Tree should only be Associations, assuming this tree is doing a bind");
                }
                this.CreateLinkPayload(associationResourceInstance, linksElement);
            }
            if (parentNode == null)
                document.AppendChild(linksElement);
        }
        private void CreateUnBinding(XmlNode parentNode)
        {
            XmlElement entryElement = CreateAtomElement("entry");

            if (parentNode == null)
            {
                AddNamespacesToTopElement(entryElement);
                document.AppendChild(entryElement);
            }
            else
            {
                parentNode.AppendChild(entryElement);
            }
            XmlAttribute nullAttribute = CreateDataWebMetadataAttribute("null");
            nullAttribute.Value = "true";
            entryElement.Attributes.Append(nullAttribute);

        }
        private XmlElement CreateIdElement(KeyedResourceInstance keyedResourceInstance)
        {
            XmlElement idNode = CreateAtomElement("id");
            idNode.InnerText = CreateCanonicalUri(keyedResourceInstance.ResourceInstanceKey);
            return idNode;
        }
        private KeyedResourceInstance currentResource = null;
        private void CreateEntryElement(KeyedResourceInstance keyedResourceInstance, XmlNode parentNode)
        {
            currentResource = keyedResourceInstance;
            XmlElement entryElement = CreateBasicEntryElement(keyedResourceInstance, parentNode);

            //string relativeParentKey = null;
            //Add the Id if there is one

            if (this.RequestVerb != RequestVerb.Post)
            {
                if (keyedResourceInstance.ResourceInstanceKey != null)
                {
                    XmlElement idNode = CreateIdElement(keyedResourceInstance);
                    entryElement.AppendChild(idNode);
                }
            }

            
            ResourceType type = Workspace.ServiceContainer.ResourceTypes.Single(rt => rt.Name == keyedResourceInstance.TypeName);

            XmlElement propertiesNode = CreateDataWebMetadataElement("properties");

            IEnumerable<ResourceInstanceProperty> properties = keyedResourceInstance.Properties;
            if (this.RequestVerb == RequestVerb.Post && keyedResourceInstance.ResourceInstanceKey != null)
                properties = keyedResourceInstance.ResourceInstanceKey.KeyProperties.Union(properties);

            foreach (ResourceInstanceProperty property in properties)
            {
                if (property is ResourceInstanceSimpleProperty)
                {
                    ResourceInstanceSimpleProperty simpleResourceProperty = property as ResourceInstanceSimpleProperty;
                    VisitResourceInstanceSimpleProperty(simpleResourceProperty, propertiesNode);
                }
                else if (property is ResourceInstanceComplexProperty)
                {
                    ResourceInstanceComplexProperty complexResourceProperty = property as ResourceInstanceComplexProperty;

                    if (complexResourceProperty.ComplexResourceInstance == null)
                    {
                        VisitResourceInstanceComplexProperty(complexResourceProperty, propertiesNode);
                    }
                    else
                    {
                        VisitResourceInstanceComplexProperty(complexResourceProperty, propertiesNode);
                    }
                }
                else if (property is ResourceInstanceNavProperty)
                {
                    ResourceInstanceNavProperty navigationProperty = property as ResourceInstanceNavProperty;
                    VisitResourceNavigationProperty(navigationProperty, entryElement);
                }
            }

            if (propertiesNode.ChildNodes.Count > 0)
            {
                if (!type.Facets.HasStream)
                {
                    XmlElement contentNode = CreateAtomElement("content");
                    XmlAttribute contentTypeAttribute = CreateAtomAttribute("type");
                    contentTypeAttribute.Value = RequestUtil.RandomizeContentTypeCapitalization("application/xml");
                    contentNode.Attributes.Append(contentTypeAttribute);
                    contentNode.AppendChild(propertiesNode);
                    entryElement.AppendChild(contentNode);
                }
                else
                {
                    entryElement.AppendChild(propertiesNode);
                }
            }

            if (EntryElementCallback != null)
                EntryElementCallback(entryElement);
        }
        private XmlElement CreateAtomLinkElement(ResourceInstanceNavProperty navProperty, XmlElement parentNode, string hrefValue)
        {
            XmlElement linkNode = CreateAtomElement("link");

            XmlAttribute relAttribute = CreateAtomAttribute("rel");
            relAttribute.Value = DataWebRelatedXmlNamespace + navProperty.Name;
            linkNode.Attributes.Append(relAttribute);
            /*
            XmlAttribute titleAttribute = CreateAtomAttribute("title");
            titleAttribute.Value = navProperty.Name;
            linkNode.Attributes.Append(titleAttribute);
            */
            if (hrefValue != null)
            {
                XmlAttribute hrefAttribute = CreateAtomAttribute("href");
                hrefAttribute.Value = hrefValue;
                linkNode.Attributes.Append(hrefAttribute);
            }
            return linkNode;
        }
        public void VisitResourceNavigationCollectionProperty(ResourceInstanceNavColProperty navColProperty, XmlElement parentNode)
        {
            foreach (ResourceBodyTree node in navColProperty.Collection.NodeList)
            {
                VisitResourceNavigationRefProperty(navColProperty, node, parentNode);
            }
        }
        public void VisitResourceNavigationRefProperty(ResourceInstanceNavRefProperty navRefProperty, XmlElement parentNode)
        {
            VisitResourceNavigationRefProperty(navRefProperty, navRefProperty.TreeNode, parentNode);
        }
        public void VisitResourceNavigationRefProperty(ResourceInstanceNavProperty navProperty, ResourceBodyTree treeNode, XmlElement parentNode)
        {
            string type = "";
            string hrefKey = null;
            XmlElement inlineElement = null;
            if (treeNode is AssociationResourceInstance)
            {
                AssociationResourceInstance instance = treeNode as AssociationResourceInstance;
                hrefKey = CreateCanonicalUri(instance.ResourceInstanceKey).Replace(Workspace.ServiceUri + @"\", "");
            }
            else
            {
                inlineElement = CreateDataWebMetadataElement("inline");
                if (navProperty is ResourceInstanceNavColProperty)
                {
                    XmlElement feedNode = CreateAtomElement("feed");
                    this.Visit(navProperty, treeNode, feedNode);
                    inlineElement.AppendChild(feedNode);
                    type = "feed";
                }
                else
                {
                    this.Visit(navProperty, treeNode, inlineElement);
                    type = "entry";
                }
            }
            XmlElement linkElement = CreateAtomLinkElement(navProperty, parentNode, hrefKey);
            XmlAttribute typeAttribute = CreateAtomAttribute("type");
            typeAttribute.Value = "application/atom+xml;type=" + type;
            linkElement.Attributes.Append(typeAttribute);
            if (inlineElement != null)
                linkElement.AppendChild(inlineElement);
            parentNode.AppendChild(linkElement);
        }
        public void VisitResourceNavigationProperty(ResourceInstanceNavProperty navProperty, XmlElement parentNode)
        {
            if (navProperty is ResourceInstanceNavColProperty)
                VisitResourceNavigationCollectionProperty(navProperty as ResourceInstanceNavColProperty, parentNode);
            else
                VisitResourceNavigationRefProperty(navProperty as ResourceInstanceNavRefProperty, parentNode);
        }
        public void VisitResourceInstanceSimpleProperty(ResourceInstanceSimpleProperty simplePropertyNode, XmlElement parentNode)
        {
            XmlElement simplePropertyElement = null;

            simplePropertyElement = CreateDataWebElement(simplePropertyNode.Name);
            
            if (simplePropertyNode.IncludeTypeMetadataHint)
            {
                XmlAttribute typeAttribute = CreateDataMetadataAttribute("type");
#if !ClientSKUFramework

                if (simplePropertyNode.ClrType == typeof(byte[]) || simplePropertyNode.ClrType == typeof(System.Data.Linq.Binary))
                    typeAttribute.Value = "Edm.Binary";
                else
#endif
                    typeAttribute.Value = simplePropertyNode.ClrType.ToString().Replace("System.", "Edm.");
                simplePropertyElement.Attributes.Append(typeAttribute);
            }
            //TODO, need to do the correct serialization per type
            //TODO: What do we do if null?
            if (simplePropertyNode.PropertyValue != null)
            {
                string xmlValue = TypeData.XmlValueFromObject(simplePropertyNode.PropertyValue);
                simplePropertyElement.InnerText = xmlValue;
                if (simplePropertyNode.PropertyValue is string)
                {
                    if ((simplePropertyNode.PropertyValue as string).ToCharArray().Any(c => Char.IsWhiteSpace(c)))
                    {
                        XmlAttribute space = document.CreateAttribute("xml", "space", null);
                        space.Value = "preserve";
                        simplePropertyElement.Attributes.Append(space);
                    }
                }
            }
            else
            {
                XmlAttribute isnullAttribute = CreateDataWebMetadataAttribute("null");
                isnullAttribute.Value = "true";
                simplePropertyElement.Attributes.Append(isnullAttribute);
            }

            if (parentNode == null)
            {
                AddNamespacesToTopElement(simplePropertyElement);
                document.AppendChild(simplePropertyElement);
            }
            else
            {
                parentNode.AppendChild(simplePropertyElement);
            }

        }


        private XmlElement CreateComplexPropertyNode(ResourceInstanceComplexProperty complexPropertyNode)
        {
            XmlElement propertyNodeElement = CreateDataWebElement(complexPropertyNode.Name);
            if (complexPropertyNode.IncludeTypeMetadataHint)
            {
                XmlAttribute typeAttribute = CreateDataMetadataAttribute("type");
                typeAttribute.Value = this.Workspace.ContextNamespace + "." + complexPropertyNode.TypeName;
                propertyNodeElement.Attributes.Append(typeAttribute);
            }

            if (complexPropertyNode.ComplexResourceInstance == null)
            {
                XmlAttribute isnullAttribute = CreateDataWebMetadataAttribute("null");
                isnullAttribute.Value = "true";
                propertyNodeElement.Attributes.Append(isnullAttribute);
            }
            return propertyNodeElement;
        }

        public void VisitResourceInstanceComplexProperty(ResourceInstanceComplexProperty complexPropertyNode, XmlElement parentNode)
        {
            XmlElement propertyNodeElement = CreateComplexPropertyNode(complexPropertyNode);

            if (complexPropertyNode.ComplexResourceInstance != null)
            {
                foreach (ResourceInstanceSimpleProperty simpleProperty in complexPropertyNode.ComplexResourceInstance.Properties.OfType<ResourceInstanceSimpleProperty>())
                {
                    VisitResourceInstanceSimpleProperty(simpleProperty, propertyNodeElement);
                }
                foreach (ResourceInstanceComplexProperty complexProperty in complexPropertyNode.ComplexResourceInstance.Properties.OfType<ResourceInstanceComplexProperty>())
                {
                    VisitResourceInstanceComplexProperty(complexProperty, propertyNodeElement);
                }
            }

            if (parentNode == null)
            {
                AddNamespacesToTopElement(propertyNodeElement);
                document.AppendChild(propertyNodeElement);
            }
            else
            {
                parentNode.AppendChild(propertyNodeElement);
            }
        }
    }
}

// "{ __metadata: { type: \"northwind.Region\" }, RegionId: 55, RegionDesctripyion: \"foo\" }"
