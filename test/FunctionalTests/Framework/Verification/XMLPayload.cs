//---------------------------------------------------------------------
// <copyright file="XMLPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.ModuleCore;
    using System.Xml.Linq;
    using System.IO;

    public class XMLPayload : CommonPayload
    {
        private static readonly XNamespace atom = XNamespace.Get("http://www.w3.org/2005/Atom");
        private static readonly XNamespace d = XNamespace.Get("http://docs.oasis-open.org/odata/ns/data");
        private static readonly XNamespace m = XNamespace.Get("http://docs.oasis-open.org/odata/ns/metadata");

        private XDocument document;

        // DO NOT INSTANTIATE THIS DIRECTLY. Use either CommonPayload.CreateCommonPayload or AstoriaResponse.CommonPayload
        internal XMLPayload(AstoriaRequestResponseBase rr)
            : base(rr)
        {
            var request = rr as AstoriaRequest;
            var response = rr as AstoriaResponse;

            string uri = null;
            if (request != null)
            {
                uri = request.URI;    
            }
            else if(response != null)
            {
                uri = response.Request.URI;
            }

            // for $value requests, the payload is just a raw string, so do not attempt to parse it as xml
            if (uri != null && uri.EndsWith("$value", StringComparison.Ordinal))
            {
                this.Value = this.RawPayload;
                return;
            }

            ParseResults(this.RawPayload);
        }

        private void ParseResults(string payload)
        {
            if (payload == null)
            {
                this.Value = null;
                return;
            }

            try
            {
                document = XDocument.Parse(payload);
            }
            catch (Exception e)
            {
                this.Value = payload;
                return;
            }

            XElement xmlData = document.Root;

            if (xmlData.Name.LocalName == "feed")
                this.Resources = this.ParseFeed(xmlData);
            else if (xmlData.Name.LocalName == "entry")
                this.Resources = this.ParseSingleEntry(xmlData);
            else if (xmlData.Name.LocalName == "links")
                this.Resources = this.ParseLinks(xmlData);
            else if (xmlData.Name.LocalName == "uri")
                this.Resources = new List<PayloadObject>() { this.ParseUriNode(xmlData) };
            else
            {
                PayloadObject payloadObject = new PayloadObject(this);
                if (xmlData.HasElements)
                    this.Resources = this.ParseComplexProperty(payloadObject, xmlData);
                else
                    this.Resources = this.ParseSimpleProperty(payloadObject, xmlData);
            }
        }

        internal List<PayloadObject> ParseFeed(XElement xmlData)
        {
            List<PayloadObject> payloadObjects = new List<PayloadObject>();

            XElement count = xmlData.Element(m + "count");
            if (count != null)
            {
                ParseCount(count);
            }

            foreach (XElement entry in xmlData.Elements(atom + "entry"))
            {
                payloadObjects.Add(ParseEntry(entry));
            }

            XElement nextLink = xmlData.Elements(atom + "link")
                .SingleOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "next" && l.Attribute("href") != null);
            if (nextLink != null)
                ParseSkipTokenLink(nextLink);

            return payloadObjects;
        }

        internal void ParseSkipTokenLink(XElement nextLink)
        {
            XAttribute href = nextLink.Attribute("href");

            if (href == null)
                return;

            this.NextLink = href.Value;
        }

        internal void ParseCount(XElement count)
        {
            this.Count = Convert.ToInt64(count.Value);
        }

        internal List<PayloadObject> ParseSingleEntry(XElement xmlData)
        {
            List<PayloadObject> payloadObjects = new List<PayloadObject>();
            payloadObjects.Add(ParseEntry(xmlData));
            return payloadObjects;
        }

        internal List<PayloadObject> ParseLinks(XElement node)
        {
            List<PayloadObject> payloadObjects = new List<PayloadObject>();
            foreach (XElement uriNode in node.Elements())
            {
                PayloadObject po;
                if (uriNode.Name == m + "count")
                    ParseCount(uriNode);
                else
                    payloadObjects.Add(ParseUriNode(uriNode));
            }
            return payloadObjects;
        }

        internal PayloadObject ParseUriNode(XElement node)
        {
            PayloadObject po = new PayloadObject(this);
            PayloadSimpleProperty property = new PayloadSimpleProperty(po);
            property.Value = node.Value;
            property.Name = node.Name.LocalName;
            po.Name = node.Name.LocalName;
            po.PayloadProperties.Add(property);
            return po;
        }

        internal const string DataWebRelatedNamespace = "http://docs.oasis-open.org/odata/ns/related/";

        /// <summary>XML namespace for data service named media resources.</summary>
        internal const string DataWebMediaResourceNamespace = "http://docs.oasis-open.org/odata/ns/mediaresource/";

        /// <summary>XML namespace for data service edit-media link for named media resources.</summary>
        internal const string DataWebMediaResourceEditNamespace = "http://docs.oasis-open.org/odata/ns/edit-media/";

        internal PayloadObject ParseEntry(XElement xmlData)
        {
            PayloadObject payloadObject = new PayloadObject(this);

            #region edit link
            XElement linkEdit = xmlData.Elements(atom + "link").SingleOrDefault(l => l.Attribute("rel") != null && l.Attribute("rel").Value == "edit");
            if (linkEdit != null)
                payloadObject.Uri = linkEdit.Attribute("href").Value;
            #endregion

            #region id link
            XElement idNode = xmlData.Element(atom + "id");
            if (idNode != null)
                payloadObject.Uri = idNode.Value;
            // TODO: compare the edit link to the ID node's value
            #endregion

            #region name
            XElement titleNode = xmlData.Parent;
            if (titleNode != null && titleNode.Name == m + "inline")
            {
                XAttribute titleAttribute = titleNode.Parent.Attribute("title");
                if (titleAttribute != null)
                    payloadObject.Name = titleAttribute.Value;
            }
            else if (titleNode != null)
            {
                titleNode = titleNode.Element(atom + "title");
                if (titleNode != null)
                    payloadObject.Name = titleNode.Value;
            }
            else if (linkEdit != null)
            {
                XAttribute titleAttribute = linkEdit.Attribute("title");
                if (titleAttribute != null)
                    payloadObject.Name = titleAttribute.Value;
            }
            #endregion

            #region type
            bool isMediaLink = false;
            XElement category = xmlData.Element(atom + "category");
            ResourceType type = null;
            if (category != null)
            {
                XAttribute term = category.Attribute("term");
                if (term != null)
                {
                    payloadObject.Type = term.Value;
                    type = Workspace.ServiceContainer.ResourceTypes.Single(rt => rt.Namespace + "." + rt.Name == term.Value);
                    if (type.Facets.HasStream)
                        isMediaLink = true;
                }
            }
            #endregion

            #region friendly feeds
            ParseNullableSyndicationItemProperty(AtomSyndicationItemProperty.Title, xmlData, payloadObject);
            ParseNullableSyndicationItemProperty(AtomSyndicationItemProperty.Summary, xmlData, payloadObject);
            ParseNullableSyndicationItemProperty(AtomSyndicationItemProperty.Rights, xmlData, payloadObject);

            //This is the correct mapping for Friendly Feeds SyndicationItemProperty.Published
            XElement ffEntryPublishedNode = xmlData.Element(atom + "published");
            if (ffEntryPublishedNode != null)
                payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.Published] = ffEntryPublishedNode.Value;

            //This is the correct mapping for Friendly Feeds SyndicationItemProperty.Updated
            XElement ffEntryUpdatedNode = xmlData.Element(atom + "updated");
            if (ffEntryUpdatedNode != null)
                payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.Updated] = ffEntryUpdatedNode.Value;

            //This is the correct mapping for Friendly Feeds Author Node
            XElement ffEntryAuthorNode = xmlData.Element(atom + "author");
            if (ffEntryAuthorNode != null)
            {
                //Friendly Feeds SyndicationItemProperty.AuthorEmail
                XElement ffEmailNode = ffEntryAuthorNode.Element(atom + "email");
                if (ffEmailNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.AuthorEmail] = ffEmailNode.Value;

                //Friendly Feeds SyndicationItemProperty.AuthorName
                XElement ffNameNode = ffEntryAuthorNode.Element(atom + "name");
                if (ffNameNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.AuthorName] = ffNameNode.Value;

                //Friendly Feeds SyndicationItemProperty.AuthorUri
                XElement ffUriNode = ffEntryAuthorNode.Element(atom + "uri");
                if (ffUriNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.AuthorUri] = ffUriNode.Value;
            }

            //This is the correct mapping for Friendly Feeds Contributor Node
            XElement ffEntryContributorNode = xmlData.Element(atom + "contributor");
            if (ffEntryContributorNode != null)
            {
                //Friendly Feeds SyndicationItemProperty.ContributorEmail
                XElement ffEmailNode = ffEntryContributorNode.Element(atom + "email");
                if (ffEmailNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.ContributorEmail] = ffEmailNode.Value;

                //Friendly Feeds SyndicationItemProperty.ContributorName
                XElement ffNameNode = ffEntryContributorNode.Element(atom + "name");
                if (ffNameNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.ContributorName] = ffNameNode.Value;

                //Friendly Feeds SyndicationItemProperty.ContributorUri
                XElement ffUriNode = ffEntryContributorNode.Element(atom + "uri");
                if (ffUriNode != null)
                    payloadObject.SyndicationItemProperties[AtomSyndicationItemProperty.ContributorUri] = ffUriNode.Value;
            }

            #endregion

            #region etag
            if (xmlData.HasAttributes)
            {
                XAttribute etagNodeAttribute = xmlData.Attribute(m + "etag");
                if (etagNodeAttribute != null)
                    payloadObject.ETag = etagNodeAttribute.Value;
            }
            #endregion

            #region properties
            XElement content = xmlData.Element(atom + "content");
            XElement properties = null;
            if (content != null)
            {
                XAttribute contentType = content.Attribute("type");
                if (contentType != null)
                {
                    if (!isMediaLink)
                    {
                        StringComparison comp;
                        if (WasResponse)
                            comp = StringComparison.InvariantCulture;
                        else
                            comp = StringComparison.InvariantCultureIgnoreCase;
                        if (!contentType.Value.Equals("application/xml", comp))
                            AstoriaTestLog.AreEqual("application/xml", contentType.Value, "Unexpected type on content element", true);
                        properties = content.Element(m + "properties");
                    }
                    // TODO: save content type if its a media link
                }
            }

            if (isMediaLink)
                properties = xmlData.Element(m + "properties");

            if (properties != null)
            {
                foreach (XElement element in properties.Elements())
                {
                    PayloadProperty property;
                    if (element.HasElements)
                    {
                        property = ParseComplexProperty(payloadObject, element);
                    }
                    else
                    {
                        property = ParseSimpleProperty(payloadObject, element);
                    }
                    payloadObject.PayloadProperties.Add(property);
                }
            }
            #endregion

            #region links
            foreach (XElement link in xmlData.Elements(atom + "link"))
            {
                Uri linkRel = new Uri(link.Attribute("rel").Value, UriKind.RelativeOrAbsolute);
                if (linkRel.IsAbsoluteUri)
                {
                    string linkRelValue = linkRel.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
                    if (linkRelValue.StartsWith(DataWebRelatedNamespace, StringComparison.Ordinal))
                    {
                        XElement inlineTag = link.Element(m + "inline");
                        if (inlineTag != null)
                        {
                            PayloadObject nestedPayloadObject = new PayloadObject(this);
                            nestedPayloadObject.Name = linkRelValue.Substring(DataWebRelatedNamespace.Length);

                            string linkType = link.Attribute("type").Value;
                            if (linkType.Contains("feed"))
                            {
                                if (inlineTag.HasElements)
                                    nestedPayloadObject.PayloadObjects.AddRange(this.ParseFeed(inlineTag.Elements().Single()));
                            }
                            else if (linkType.Contains("entry"))
                            {
                                if (inlineTag.HasElements)
                                    nestedPayloadObject = this.ParseEntry(inlineTag.Elements().Single());

                                nestedPayloadObject.Reference = true;
                            }

                            payloadObject.PayloadObjects.Add(nestedPayloadObject);
                        }
                        else
                        {
                            // deferred links
                            PayloadObject deferredObject = new PayloadObject(this);
                            deferredObject.Name = linkRelValue.Substring(DataWebRelatedNamespace.Length);
                            if (link.Attribute("href") != null)
                                deferredObject.Uri = link.Attribute("href").Value;
                            deferredObject.Deferred = true;

                            payloadObject.PayloadObjects.Add(deferredObject);
                        }
                    }
                    else if (linkRelValue.StartsWith(DataWebMediaResourceNamespace))
                    {
                        string name = linkRelValue.Substring(DataWebMediaResourceNamespace.Length);
                        PayloadNamedStream namedStream = payloadObject.NamedStreams.SingleOrDefault(s => s.Name == name);
                        if (namedStream == null)
                        {
                            namedStream = new PayloadNamedStream() { Name = name };
                            payloadObject.NamedStreams.Add(namedStream);
                        }

                        UpdateNamedStreamMetadata(link, namedStream);
                        namedStream.SelfLink = link.Attribute("href").Value;
                    }
                    else if (linkRelValue.StartsWith(DataWebMediaResourceEditNamespace))
                    {
                        string name = linkRelValue.Substring(DataWebMediaResourceEditNamespace.Length);
                        PayloadNamedStream namedStream = payloadObject.NamedStreams.SingleOrDefault(s => s.Name == name);
                        if (namedStream == null)
                        {
                            namedStream = new PayloadNamedStream() { Name = name };
                            payloadObject.NamedStreams.Add(namedStream);
                        }

                        UpdateNamedStreamMetadata(link, namedStream);
                        namedStream.EditLink = link.Attribute("href").Value;
                    }
                }
            }
            #endregion

            return payloadObject;
        }

        private void ParseNullableSyndicationItemProperty(AtomSyndicationItemProperty itemProperty, XElement xmlData, PayloadObject payloadObject)
        {
            XElement element = xmlData.Element(atom + itemProperty.ToString().ToLowerInvariant());
            if (element != null)
            {
                var value = element.Value;

                XAttribute nullAttribute = element.Attribute(m + "null");
                if (nullAttribute != null && nullAttribute.Value == "true")
                {
                    value = null;
                }

                payloadObject.SyndicationItemProperties[itemProperty] = value;
            }
        }

        private static void UpdateNamedStreamMetadata(XElement link, PayloadNamedStream namedStream)
        {
            XAttribute typeAttribute = link.Attribute("type");
            XAttribute titleAttribute = link.Attribute("title");
            XAttribute etagAttribute = link.Attribute("etag");


            if (typeAttribute != null)
            {
                namedStream.ContentType = typeAttribute.Value;
            }

            if (titleAttribute != null)
            {
                namedStream.Title = titleAttribute.Value;
            }


            if (etagAttribute != null)
            {
                namedStream.ETag = etagAttribute.Value;
            }
        }

        internal PayloadSimpleProperty ParseSimpleProperty(PayloadObject parent, XElement xmlData)
        {
            PayloadSimpleProperty payloadProperty = new PayloadSimpleProperty(parent);
            payloadProperty.Name = xmlData.Name.LocalName;
            payloadProperty.Value = xmlData.Value;

            XAttribute nullAttribute = xmlData.Attribute(m + "null");
            if (nullAttribute != null && nullAttribute.Value == "true")
            {
                payloadProperty.Value = null;
                payloadProperty.IsNull = true;
            }

            XAttribute typeAttribute = xmlData.Attribute(m + "type");
            if (typeAttribute != null)
                payloadProperty.Type = typeAttribute.Value;

            return payloadProperty;
        }

        internal PayloadComplexProperty ParseComplexProperty(PayloadObject parent, XElement xmlData)
        {
            PayloadComplexProperty payloadProperty = new PayloadComplexProperty(parent);
            payloadProperty.Name = xmlData.Name.LocalName;

            XAttribute nullAttribute = xmlData.Attribute(m + "null");
            if (nullAttribute != null && nullAttribute.Value == "true")
                payloadProperty.IsNull = true;

            XAttribute typeAttribute = xmlData.Attribute(m + "type");
            if (typeAttribute != null)
                payloadProperty.Type = typeAttribute.Value;

            foreach (XElement property in xmlData.Elements())
            {
                PayloadProperty subProperty;
                if (property.HasElements)
                {
                    subProperty = ParseComplexProperty(parent, property);
                }
                else
                {
                    subProperty = ParseSimpleProperty(parent, property);
                }
                payloadProperty.PayloadProperties.Add(subProperty.Name, subProperty);
            }

            return payloadProperty;
        }

        public override void Compare(IQueryable baseline)
        {
            base.Compare(baseline);

            // TODO: verify namespace and ?xml data here?
        }
    }
}
