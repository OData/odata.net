//---------------------------------------------------------------------
// <copyright file="FormatVerification.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public enum FormatVerifierSerializationKind { Atom, JSON };

    public class FormatVerifier
    {
        private List<IVerifier> _verifierList = new List<IVerifier>();

        public static FormatVerifier CreateFormatVerifier(FormatVerifierSerializationKind kind)
        {
            return FormatVerifier.CreateFormatVerifier(kind, false);
        }

        public static FormatVerifier CreateFormatVerifier(FormatVerifierSerializationKind kind, bool isBatch)
        {
            FormatVerifier formatVerifier = new FormatVerifier();

            if (isBatch)
            {
                // do something interesting
            }
            else
            {
                switch (kind)
                {
                    case FormatVerifierSerializationKind.JSON:
                        formatVerifier.AddRule(new JsonFormatVerifier());
                        break;

                    case FormatVerifierSerializationKind.Atom:
                        formatVerifier.XmlNamespaceManager = FormatVerifyUtils.CreateNamespaceManager();

                        formatVerifier.AddRule(new AtomFeedVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomEntryVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomCategoryVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomAuthorVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomContentVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomContributorVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomLinkVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomTitleVerifier(formatVerifier));
                        formatVerifier.AddRule(new AtomUpdatedVerifier(formatVerifier));
                        break;

                    default:
                        break;
                }
            }

            return formatVerifier;
        }

        public AstoriaRequest Request
        {
            get;
            set;
        }

        public List<IVerifier> VerifierList
        {
            get;
            set;
        }

        public XmlNamespaceManager XmlNamespaceManager
        {
            get;
            set;
        }

        public void AddRule(IVerifier verifier)
        {
            _verifierList.Add(verifier);
        }

        public void Verify(AstoriaResponse response)
        {
            foreach (IVerifier verifier in _verifierList)
            {
                verifier.Verify(response);
            }
        }
    }



    public class JsonFormatVerifier : IVerifier
    {
        public void Verify(AstoriaResponse response)
        {
            //AstoriaTestLog.TraceInfo(payload);

            try
            {
                JSONPayload jsonPayload = new JSONPayload(response);
            }
            catch (Exception e)
            {
                AstoriaTestLog.FailAndContinue(new TestFailedException("Failed to parse JSON payload : " + e.ToString()));
            }
            //AstoriaTestLog.TraceInfo(jsonPayload.ToString());

            return;
        }




    }


    public interface IVerifier
    {
        void Verify(AstoriaResponse response);
    }

    public abstract class EntryAndElementVerifier : IVerifier
    {
        private FormatVerifier _formatVerifier;

        protected abstract void VerifyEntry(XmlNode node);

        public EntryAndElementVerifier(FormatVerifier FormatVerifier)
        {
            _formatVerifier = FormatVerifier;
        }

        public FormatVerifier FormatVerifier
        {
            get { return _formatVerifier; }
        }

        public virtual void Verify(AstoriaResponse response)
        {
            string payload = response.Payload;
            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
            {
                XmlNodeList entries = xmlData.SelectNodes("atom:entry", _formatVerifier.XmlNamespaceManager);

                foreach (XmlNode node in entries)
                    VerifyEntry(node);
            }
            else if (xmlData.Name == "entry")
                VerifyEntry(xmlData);
        }
    }

    // 4.1 Container Elements

    // 4.1.1 Feed element
    public class AtomFeedVerifier : IVerifier
    {
        private FormatVerifier _formatVerifier;

        public AtomFeedVerifier(FormatVerifier FormatVerifier)
        {
            _formatVerifier = FormatVerifier;
        }

        public void Verify(AstoriaResponse response)
        {
            string payload = response.Payload;

            XmlNodeList childNodes = null;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
            {
                /* atom:feed elements MUST contain one or more atom:author elements,
                    unless all of the atom:feed element's child atom:entry elements
                    contain at least one atom:author element. 
                 */
                childNodes = xmlData.SelectNodes("atom:author", _formatVerifier.XmlNamespaceManager);
                XmlNodeList entries = xmlData.SelectNodes("atom:entry", _formatVerifier.XmlNamespaceManager);

                if (childNodes.Count == 0)
                {
                    if (entries.Count > 0)
                    {
                        foreach (XmlNode node in entries)
                        {
                            XmlNode authorNode = node.SelectSingleNode("atom:author[1]", _formatVerifier.XmlNamespaceManager);
                            if( authorNode == null)
                                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: No author node in entry of feed without author node."));
                        }
                    }
                    else
                        AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: No author node in feed with no entries."));
                }

                // atom:feed elements MUST NOT contain more than one atom:generator element.
                childNodes = xmlData.SelectNodes("atom:generator", _formatVerifier.XmlNamespaceManager);
                if(childNodes.Count > 1 )
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: More than 1 generator node found for feed."));

                // atom:feed elements MUST NOT contain more than one atom:icon element.
                childNodes = xmlData.SelectNodes("atom:icon", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count > 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: More than 1 icon node found for feed."));

                // atom:feed elements MUST NOT contain more than one atom:logo element.
                childNodes = xmlData.SelectNodes("atom:logo", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count > 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: More than 1 logo node found for feed."));

                // atom:feed elements MUST contain exactly one atom:id element
                childNodes = xmlData.SelectNodes("atom:id", _formatVerifier.XmlNamespaceManager);
                if( childNodes.Count != 1 )
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: Not exactly 1 id node found for feed."));

                /* atom:feed elements SHOULD contain one atom:link element with a rel
                    attribute value of "self".  This is the preferred URI for
                    retrieving Atom Feed Documents representing this Atom feed.
                
                   atom:feed elements MUST NOT contain more than one atom:link
                    element with a rel attribute value of "alternate" that has the
                    same combination of type and hreflang attribute values.
                */
                int selfLinkCount = 0, altLinkCount = 0;
                childNodes = xmlData.SelectNodes("atom:link", _formatVerifier.XmlNamespaceManager);
                foreach (XmlNode node in childNodes)
                {
                    if (node.Attributes["rel"] != null && node.Attributes["rel"].Value == "self")
                        selfLinkCount++;
                    else if (node.Attributes["rel"] != null && node.Attributes["rel"].Value == "alternate")
                        altLinkCount++;
                }

                AstoriaTestLog.AreEqual(1, selfLinkCount, "4.1.1 Feed element: Not exactly 1 self link node found for feed");
                AstoriaTestLog.AreEqual(false, altLinkCount > 1, "4.1.1 Feed element: More than 1 alternate link node found for feed");

                // atom:feed elements MUST NOT contain more than one atom:rights element.
                childNodes = xmlData.SelectNodes("atom:rights", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count > 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: More than 1 rights node found for feed."));

                // atom:feed elements MUST NOT contain more than one atom:subtitle element.
                childNodes = xmlData.SelectNodes("atom:subtitle", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count > 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: More than 1 subtitle node found for feed."));

                // atom:feed elements MUST contain exactly one atom:title element.
                childNodes = xmlData.SelectNodes("atom:title", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count != 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: Not exactly 1 title node found for feed."));

                // atom:feed elements MUST contain exactly one atom:updated element.
                childNodes = xmlData.SelectNodes("atom:updated", _formatVerifier.XmlNamespaceManager);
                if (childNodes.Count != 1)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.1 Feed element: Not exactly 1 updated node found for feed."));
            }
        }
    }

    // 4.1.2 Entry element
    public class AtomEntryVerifier : EntryAndElementVerifier
    {
        public AtomEntryVerifier(FormatVerifier FormatVerifier) : base(FormatVerifier)
        {
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            XmlNodeList childNodes = null;

            /* atom:entry elements MUST contain one or more atom:author elements,
                unless the atom:entry contains an atom:source element that
                contains an atom:author element or, in an Atom Feed Document, the
                atom:feed element contains an atom:author element itself.
            */
            childNodes = entry.SelectNodes("atom:author", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count == 0)
            {
                bool authorFound = false;
                XmlNodeList sources = entry.SelectNodes("atom:source", this.FormatVerifier.XmlNamespaceManager);
                if (sources.Count == 1)
                {
                    XmlNodeList authors = sources[0].SelectNodes("atom:author", this.FormatVerifier.XmlNamespaceManager);
                    if (authors.Count != 0)
                        authorFound = true;
                }

                if (!authorFound)
                {
                    XmlNode parent = entry.ParentNode;

                    if (parent.Name == "feed")
                    {
                        XmlNodeList authors = parent.SelectNodes("atom:author", this.FormatVerifier.XmlNamespaceManager);
                        if (authors.Count != 0)
                            authorFound = true;
                    }
                }

                if( !authorFound )
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: No author element found for entry"));
            }

            // atom:entry elements MUST NOT contain more than one atom:content element.
            childNodes = entry.SelectNodes("atom:content", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: More than 1 content node found for entry."));

            // atom:entry elements MUST contain exactly one atom:id element.
            childNodes = entry.SelectNodes("atom:id", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count != 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: Not exactly 1 id node found for entry."));

            /* atom:entry elements that contain no child atom:content element
                MUST contain at least one atom:link element with a rel attribute
                value of "alternate".
            */
            childNodes = entry.SelectNodes("atom:content", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count == 0)
            {
                bool relAltAttribFound = false;

                childNodes = entry.SelectNodes("atom:link", this.FormatVerifier.XmlNamespaceManager);
                foreach (XmlNode node in childNodes)
                {
                    if (node.Attributes["rel"] != null && node.Attributes["rel"].Value == "alternate")
                        relAltAttribFound = true;
                }

                if( !relAltAttribFound )
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: No link node with rel='alternate' found for entry with no content."));
            }

            /*  atom:entry elements MUST NOT contain more than one atom:link
                element with a rel attribute value of "alternate" that has the
                same combination of type and hreflang attribute values.
            */
            childNodes = entry.SelectNodes("atom:link", this.FormatVerifier.XmlNamespaceManager);

            int relAltLinkCount = 0;
            foreach (XmlNode node in childNodes)
            {
                if (node.Attributes["rel"] != null && node.Attributes["rel"].Value == "alternate")
                    relAltLinkCount++;
            }

            if (relAltLinkCount > 1)
            {
                //TODO: track and compare type and hreflang attribs
            }

            // atom:entry elements MUST NOT contain more than one atom:published element.
            childNodes = entry.SelectNodes("atom:published", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: More than 1 published node found for entry."));

            // atom:entry elements MUST NOT contain more than one atom:rights element.
            childNodes = entry.SelectNodes("atom:rights", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: More than 1 rights node found for entry."));

            // atom:entry elements MUST NOT contain more than one atom:source element.
            childNodes = entry.SelectNodes("atom:source", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: More than 1 source node found for entry."));

            /*  atom:entry elements MUST contain an atom:summary element in either
                of the following cases:
                    -  the atom:entry contains an atom:content that has a "src"
                        attribute (and is thus empty).
                    -  the atom:entry contains content that is encoded in Base64;
                        i.e., the "type" attribute of atom:content is a MIME media type
                        but is not an XML media type [<a href="./rfc3023" title='"XML Media Types"'>RFC3023</a>], does not
                        begin with "text/", and does not end with "/xml" or "+xml".
            */
            bool contentWithSrcFound = false, contentWithBase64Found = false;

            childNodes = entry.SelectNodes("atom:content", this.FormatVerifier.XmlNamespaceManager);
            foreach (XmlNode node in childNodes)
            {
                if (node.Attributes["src"] != null)
                {
                    contentWithSrcFound = true;
                    break;
                }
            }

            childNodes = entry.SelectNodes("atom:content", this.FormatVerifier.XmlNamespaceManager);
            foreach (XmlNode node in childNodes)
            {
                if (node.Attributes["type"] != null && !(node.Attributes["type"].Value.EndsWith("/xml")
                    || node.Attributes["type"].Value.EndsWith("+xml") || node.Attributes["type"].Value.StartsWith("text/")))
                {
                    contentWithBase64Found = true;
                    break;
                }
            }

            if (contentWithSrcFound || contentWithBase64Found)
            {
                XmlNodeList summaryNodes = entry.SelectNodes("atom:summary", this.FormatVerifier.XmlNamespaceManager);
                if( summaryNodes.Count == 0 )
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: No summary node found with Base64 or src'd content for entry."));
            }

            // atom:entry elements MUST NOT contain more than one atom:summary element.
            childNodes = entry.SelectNodes("atom:summary", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: More than 1 summary node found for entry."));

            // atom:entry elements MUST contain exactly one atom:title element.
            childNodes = entry.SelectNodes("atom:title", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count != 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: Not exactly 1 title node found for entry."));

            // atom:entry elements MUST contain exactly one atom:updated element.
            childNodes = entry.SelectNodes("atom:title", this.FormatVerifier.XmlNamespaceManager);
            if (childNodes.Count != 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.2 Entry element: Not exactly 1 updated node found for entry."));

        }
    }

    // 4.1.3 Content element
    public class AtomContentVerifier : EntryAndElementVerifier
    {
        public AtomContentVerifier(FormatVerifier FormatVerifier) : base(FormatVerifier)
        {
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            //TODO: Astoria always returns type="application/xml" so this section is incomplete wrt spec and assunes xml content

            XmlNodeList contentNodes = entry.SelectNodes("atom:content", this.FormatVerifier.XmlNamespaceManager);
            if (contentNodes.Count != 1)
            {
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.3 Content element: is missing"));
            }
            else
            {
                XmlNode contentNode = contentNodes[0];

                bool srcFound = contentNode.Attributes["src"] != null;

                if (contentNode.Attributes["type"] != null && contentNode.Attributes["type"].Value != "application/xml")
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.3 Content element: type is not 'application/xml' (only supported value in v1 astoria)"));

                if (contentNode.ChildNodes.Count != 1 && !srcFound)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.3.3 Content element: More than node found for conent root node."));

            }
        }
    }

    // 4.2 Metadata Elements

    // 4.2.1 Author element (entry, feed)
    public class AtomAuthorVerifier : EntryAndElementVerifier
    {
        public AtomAuthorVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            // author contains one atomPersonConstruct - verify this

            XmlNodeList authorNodes = entry.SelectNodes("atom:author", this.FormatVerifier.XmlNamespaceManager);
            if (authorNodes.Count != 1)
            {
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.1.3 Author element: count is not exactly 1"));
            }
            else
            {
                XmlNode authorNode = authorNodes[0];

                AtomPersonConstructVerifier atomPersonConstructVerifier = new AtomPersonConstructVerifier(this.FormatVerifier);
                atomPersonConstructVerifier.Verify(authorNode.OuterXml);

            }
        }
    }

    // 4.2.2 Category element (entry, feed)
    public class AtomCategoryVerifier : EntryAndElementVerifier
    {
        public AtomCategoryVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        public override void Verify(AstoriaResponse response)
        {
            base.Verify(response);

            string payload = response.Payload;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
                this.VerifyEntry(xmlData);
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            XmlNodeList categoryNodes = entry.SelectNodes("atom:category", this.FormatVerifier.XmlNamespaceManager);
            foreach( XmlNode categoryNode in categoryNodes )
            {
                XmlAttribute termAttrib = categoryNode.Attributes["term"];
                if (termAttrib == null)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("No term attribute found for Category node"));
                /*else
                {
                    bool typeFound = false;
                    foreach (NodeType nodeType in _formatVerifier.Request.Workspace.Types.Where(t => t.ClrType.FullName == termAttrib.Value))
                    {
                        typeFound = true;
                        break;
                    }

                    if( !typeFound )
                        AstoriaTestLog.FailAndContinue(new TestFailedException("term attribute value (" + termAttrib.Value + ")is not valid type"));
                }*/
            }
        }
    }

    // 4.2.3 atom:contributor (entry, feed)
    public class AtomContributorVerifier : EntryAndElementVerifier
    {
        public AtomContributorVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        public override void Verify(AstoriaResponse response)
        {
            base.Verify(response);

            string payload = response.Payload;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
                this.VerifyEntry(xmlData);
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            // contributor contains one atomPersonConstruct - verify this

            XmlNodeList contributorNodes = entry.SelectNodes("atom:contributor", this.FormatVerifier.XmlNamespaceManager);
            if (contributorNodes.Count > 0)
            {
                foreach (XmlNode contributorNode in contributorNodes)
                {
                    AtomPersonConstructVerifier atomPersonConstructVerifier = new AtomPersonConstructVerifier(this.FormatVerifier);
                    atomPersonConstructVerifier.Verify(contributorNode.OuterXml);
                }
            }
        }
    }

    // 4.2.4 atom:generator (feed)
    //  Nothing to do

    // 4.2.5 atom:icon (feed)
    //  Nothing to do

    // 4.2.6 atom:id (entry, feed)
    //  Nothing to do

    // 4.2.7 atom:link (entry, feed)
    public class AtomLinkVerifier : EntryAndElementVerifier
    {
        public AtomLinkVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        public override void Verify(AstoriaResponse response)
        {
            base.Verify(response);

            string payload = response.Payload;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
                this.VerifyEntry(xmlData);
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            XmlNodeList linkNodes = entry.SelectNodes("atom:link", this.FormatVerifier.XmlNamespaceManager);

            foreach( XmlNode linkNode in linkNodes)
            {
                XmlAttribute hrefAttrib = linkNode.Attributes["href"];
                if (hrefAttrib == null)
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.7.1: No href attribute found for link element"));

                XmlAttribute relAttrib = linkNode.Attributes["rel"];
                if (relAttrib != null)
                {
                    if (relAttrib.Value != "self" && relAttrib.Value != "edit" && !relAttrib.Value.Contains("http:"))
                        AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.7.2: wrong value for rel attrib of link element."));
                }
            }
        }
    }

    // 4.2.8 atom:logo (feed)
    //  Nothing to do

    // 4.2.9 atom:published (entry)
    //  Nothing to do

    // 4.2.10 atom:rights (entry, feed)
    //  Nothing to do

    // 4.2.11 atom:source (entry)
    //  Nothing to do

    // 4.2.12 atom:subtitle (feed)
    //  Nothing to do

    // 4.2.13 atom:summary (entry)
    //  Nothing to do

    // 4.2.14 atom:title (entry, feed)
    public class AtomTitleVerifier : EntryAndElementVerifier
    {
        public AtomTitleVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        public override void Verify(AstoriaResponse response)
        {
            base.Verify(response);

            string payload = response.Payload;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
                this.VerifyEntry(xmlData);
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            XmlNodeList titleNodes = entry.SelectNodes("atom:title", this.FormatVerifier.XmlNamespaceManager);
            if (titleNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.14 Title Element: More than 1 element"));

            else if (titleNodes.Count == 1)
            {
                XmlNode titleNode = titleNodes[0];

                XmlAttribute typeAttrib = titleNode.Attributes["type"];
                if (typeAttrib != null)
                {
                    if (typeAttrib.Value != "text" && typeAttrib.Value != "xhtml" && typeAttrib.Value != "html")
                        AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.14: Title element has invalid type value"));
                }
            }
        }
    }

    // 4.2.15 atom:updated (entry, feed)
    public class AtomUpdatedVerifier : EntryAndElementVerifier
    {
        public AtomUpdatedVerifier(FormatVerifier FormatVerifier)
            : base(FormatVerifier)
        {
        }

        public override void Verify(AstoriaResponse response)
        {
            base.Verify(response);

            string payload = response.Payload;

            XmlNode xmlData = FormatVerifyUtils.FindFirstChild(payload);
            if (xmlData.Name == "feed")
                this.VerifyEntry(xmlData);
        }

        protected override void VerifyEntry(XmlNode entry)
        {
            XmlNodeList updatedNodes = entry.SelectNodes("atom:updated", this.FormatVerifier.XmlNamespaceManager);
            if (updatedNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.15 Updated Element: More than 1 element"));

            else if (updatedNodes.Count == 1)
            {
                XmlNode updatedNode = updatedNodes[0];

                string dateValue = updatedNode.InnerXml;
                try
                {
                    DateTime d = XmlConvert.ToDateTime(dateValue, XmlDateTimeSerializationMode.Unspecified);
                }
                catch
                {
                    AstoriaTestLog.FailAndContinue(new TestFailedException("4.2.15 Updated Element: invalid date format."));
                }
            }
        }
    }

    public class AtomPersonConstructVerifier
    {
        private FormatVerifier _formatVerifier;

        public AtomPersonConstructVerifier(FormatVerifier FormatVerifier)
        {
            _formatVerifier = FormatVerifier;
        }

        public void Verify(string payload)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(payload);

            XmlNode authorNode = xmlDocument.SelectNodes("atom:author", _formatVerifier.XmlNamespaceManager)[0];

            // atom:author elements MUST contain exactly one atom:name element.
            XmlNodeList childNodes = authorNode.SelectNodes("atom:name", _formatVerifier.XmlNamespaceManager);
            if (childNodes.Count != 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("3.2.1 atomPersonConstruct element: Not exactly 1 name node found for atomPersonConstruct."));

            // atom:author elements MAY contain exactly one atom:uri element.
            childNodes = authorNode.SelectNodes("atom:uri", _formatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("3.2.1 atomPersonConstruct element: Greater than 1 uri node found for atomPersonConstruct."));

            // atom:author elements MAY contain exactly one atom:email element.
            childNodes = authorNode.SelectNodes("atom:email", _formatVerifier.XmlNamespaceManager);
            if (childNodes.Count > 1)
                AstoriaTestLog.FailAndContinue(new TestFailedException("3.2.1 atomPersonConstruct element: Greater than 1 email node found for atomPersonConstruct."));
        }        
    }

    public class FormatVerifyUtils
    {
        public static XmlNode FindFirstChild(string payload)
        {
            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(payload);
            }
            catch
            {
                AstoriaTestLog.FailAndThrow("payload was not valid xml.");
            }

            return xmlDocument.FirstChild.NextSibling;
        }

        public static XmlNamespaceManager CreateNamespaceManager()
        {
            XmlNamespaceManager namespaceManger = new XmlNamespaceManager(new NameTable());
            namespaceManger.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            namespaceManger.AddNamespace("d", "http://docs.oasis-open.org/odata/ns/data");
            namespaceManger.AddNamespace("m", "http://docs.oasis-open.org/odata/ns/metadata");

            return namespaceManger;
        }
    }
}
