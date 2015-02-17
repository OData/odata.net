//---------------------------------------------------------------------
// <copyright file="AtomParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Client.Xml;
    using Microsoft.OData.Service.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    #endregion Namespaces

    /// <summary>Parser for DataService payloads (mostly ATOM).</summary>
    /// <remarks>
    /// There are four types of documents parsed:
    /// 1. A single non-entity value.
    /// 2. A list of non-entity values.
    /// 3. An entity.
    /// 4. A feed.
    /// 
    /// In case of (1), the parser will go through these states:
    ///  None -> Custom -> Finished
    ///  
    /// In case of (2), the parser will go through these states:
    ///  None -> Custom -> Custom -> Finished
    /// 
    /// In case of (3), the parser will go through these states:
    ///  None -> Entry -> Finished
    /// 
    /// In case of (4), the parser will go through these states:
    ///  None -> (FeedData | Entry) -> (FeedData | Entry) -> Finished
    /// 
    /// Note that the parser will always stop on 'top-level' packets; all recursive data
    /// structures are handled internally.
    /// </remarks>
    [DebuggerDisplay("AtomParser {kind} {reader}")]
    internal class AtomParser : IDisposable
    {
        #region Private fields

        /// <summary>The maximum protocol version the client should support (send and receive).</summary>
        internal readonly DataServiceProtocolVersion MaxProtocolVersion;

        /// <summary>Callback invoked each time an ATOM entry is found.</summary>
        /// <remarks>
        /// This callback takes the current XmlReader and returns a 
        /// subtree XmlReader and an object that is assigned to the 
        /// entry's Tag property.
        /// </remarks>
        private readonly Func<XmlWrappingReader, KeyValuePair<XmlWrappingReader, AtomTag>> entryCallback;

        /// <summary>Row count value representing the initial state (-1)</summary>
        private const long CountStateInitial = -1;

        /// <summary>Row count value representing the failure state (-2)</summary>
        private const long CountStateFailure = -2;

        /// <summary>Stack of available XmlWrappingReader.</summary>
        private readonly Stack<XmlWrappingReader> readers;

        /// <summary>Scheme used to find type information on ATOM category elements.</summary>
        private readonly string typeScheme;

        /// <summary>The data namespace</summary>
        private readonly string currentDataNamespace;

        /// <summary>gets the appropriate uri. This is used to convert relative uri's in the response payload into absolute uri's.</summary>
        private readonly UriResolver baseUriResolver;

        /// <summary>The count tag's value, if requested</summary>
        private long countValue;

        /// <summary>ATOM entry being parsed.</summary>
        private AtomEntry entry;

        /// <summary>ATOM feed being parsed.</summary>
        private AtomFeed feed;

        /// <summary>Current data kind (nothing, entry, feed, custom-top-level-thingy, etc).</summary>
        private AtomDataKind kind;

        /// <summary>Current <see cref="XmlWrappingReader"/>.</summary>
        private XmlWrappingReader reader;

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new <see cref="AtomParser"/> instance.</summary>
        /// <param name="reader"><see cref="XmlReader"/> to parse content from.</param>
        /// <param name="entryCallback">
        /// Callback invoked each time an ATOM entry is found; see the comments
        /// on the entryCallback field.
        /// </param>
        /// <param name="typeScheme">
        /// Scheme used to find type information on ATOM category elements.
        /// </param>
        /// <param name="currentDataNamespace">The xml document's DataWeb Namespace</param>
        /// <param name="baseUriResolver">Interface to retrieve the baseUri to use for this EntitySetName - this will be used to convert relative uri's in the response payload to absolute uri's.</param>
        /// <param name="maxProtocolVersion">max protocol version that the client understands.</param>
        internal AtomParser(XmlReader reader, Func<XmlWrappingReader, KeyValuePair<XmlWrappingReader, AtomTag>> entryCallback, string typeScheme, string currentDataNamespace, UriResolver baseUriResolver, DataServiceProtocolVersion maxProtocolVersion)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(typeScheme != null, "typeScheme != null");
            Debug.Assert(entryCallback != null, "entryCallback != null");
            Debug.Assert(!String.IsNullOrEmpty(currentDataNamespace), "currentDataNamespace is empty or null");
            Debug.Assert(baseUriResolver != null, "baseUriResolver != null");

            if (reader.Settings.NameTable != null)
            {
                // NOTE: dataNamespace is used for reference equality, and while it looks like
                // a variable, it appears that it will only get set to XmlConstants.DataWebNamespace
                // at runtime. Therefore we remove string dataNamespace as a field here.
                // this.dataNamespace = reader != null ? reader.Settings.NameTable.Add(context.DataNamespace) : null;
                reader.Settings.NameTable.Add(currentDataNamespace);
            }

            this.reader = new Microsoft.OData.Service.Client.Xml.XmlAtomErrorReader(reader);
            this.readers = new Stack<XmlWrappingReader>();
            this.entryCallback = entryCallback;
            this.typeScheme = typeScheme;
            this.currentDataNamespace = currentDataNamespace;
            this.baseUriResolver = baseUriResolver;
            this.countValue = CountStateInitial;
            this.MaxProtocolVersion = maxProtocolVersion;
            Debug.Assert(this.kind == AtomDataKind.None, "this.kind == AtomDataKind.None -- otherwise not initialized correctly");
        }

        /// <summary>
        /// Private default ctor for ResultsWrapper
        /// </summary>
        private AtomParser()
        {
        }

        #endregion Constructors

        #region Internal properties

        /// <summary>Entry being materialized; possibly null.</summary>
        internal AtomEntry CurrentEntry
        {
            get
            {
                return this.entry;
            }
        }

        /// <summary>Feed being materialized; possibly null.</summary>
        internal AtomFeed CurrentFeed
        {
            get
            {
                return this.feed;
            }
        }

        /// <summary>Kind of ATOM data available on the parser.</summary>
        internal AtomDataKind DataKind
        {
            get
            {
                return this.kind;
            }
        }

        /// <summary>
        /// Returns true if the current element is in the data web namespace
        /// </summary>
        internal virtual bool IsDataWebElement
        {
            get { return this.reader.NamespaceURI == this.currentDataNamespace; }
        }

        #endregion Internal properties

        #region Public Methods

        /// <summary>
        /// Dispose of all the disposeable fields in this object
        /// </summary>
        public void Dispose()
        {
            if (null != this.reader)
            {
                ((IDisposable)this.reader).Dispose();
            }
        }

        #endregion Public Methods

        #region Internal methods.

        /// <summary>
        /// Creates a wrapper around already parsed AtomEntry(s)
        /// </summary>
        /// <param name="entries">the entries to wrap</param>
        /// <returns>The wrapped parser</returns>
        internal static AtomParser CreateWrapper(IEnumerable<AtomEntry> entries)
        {
            return new ResultsWrapper(entries);
        }

        /// <summary>
        /// Creates an <see cref="XElement"/> instance for ATOM entries.
        /// </summary>
        /// <param name="reader">Reader being used.</param>
        /// <returns>
        /// A pair of an XmlReader instance and an object to be assigned 
        /// to the Tag on the entry (available for materialization callbacks
        /// later in the pipeline).
        /// </returns>
        /// <remarks>
        /// A no-op implementation would do this instead:
        /// 
        /// return new KeyValuePair&lt;XmlReader, object&gt;(reader.ReadSubtree(), null);
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "not required")]
        internal static KeyValuePair<XmlWrappingReader, AtomTag> XElementBuilderCallback(XmlWrappingReader reader)
        {
            Debug.Assert(reader != null, "reader != null");

            XElement element = XElement.Load(reader.ReadSubtree(), LoadOptions.None);

            // The XElement created here will be passed to users through the DataServiceContext.ReadingEntity event.  If xml:base is on the parent
            // element, there is no way to get to the base uri from the event handler.  Links in the atom payload are relative uris, we are storing the
            // xml base in the tag and pass it to ReadingWritingEntityEventArgs.BaseUri so the full uris for links can be constructed.
            //
            // ReadingWritingEntityEventArgs.BaseUri contains the base uri from the current element.  If the current XElement also contains a xml:base
            // attribute, it will be part of the uri we are passing to ReadingWritingEntityEventArgs.BaseUri.
            //
            // [Client-ODataLib-Integration] ReadingEntityEvent in astoria client gives the incorrect base uri in the event args
            // We will pass in the base uri of the parent element to the readingentity event.
            AtomTag tag = new AtomTag(element, reader.ParentBaseURI);

            return new KeyValuePair<XmlWrappingReader, AtomTag>(XmlWrappingReader.CreateReader(reader.ParentBaseURI, element.CreateReader()), tag);
        }

        /// <summary>
        /// This method is for parsing CUD operation payloads which are expected to contain one of the following:
        ///
        ///     - Single AtomEntry
        ///         This is the typical response we expect in the non-error case.
        ///     - Feed with a single AtomEntry
        ///         This is not valid per OData protocol, but we allowed this in V1/V2 and will continue to accept it since we can still treat it like a single entry.
        ///     - Error
        ///         Parser handles this case as we read the payload, it's not explicitly handled here.
        ///
        /// Since we don't control the payload, it may contain something that doesn't fit these requirements, in which case we will throw.
        /// </summary>
        /// <param name="reader">the reader for the payload</param>
        /// <param name="responseInfo">The current ResponseInfo object</param>
        /// <returns>the AtomEntry that was read</returns>
        internal static AtomEntry ParseSingleEntityPayload(XmlReader reader, ResponseInfo responseInfo)
        {
            using (AtomParser parser = new AtomParser(reader, AtomParser.XElementBuilderCallback, CommonUtil.UriToString(responseInfo.TypeScheme), responseInfo.DataNamespace, responseInfo.BaseUriResolver, responseInfo.MaxProtocolVersion))
            {
                Debug.Assert(parser.DataKind == AtomDataKind.None, "the parser didn't start in the right state");
                AtomEntry entry = null;
                while (parser.Read())
                {
                    if (parser.DataKind != AtomDataKind.Feed && parser.DataKind != AtomDataKind.Entry)
                    {
                        throw new InvalidOperationException(Strings.AtomParser_SingleEntry_ExpectedFeedOrEntry);
                    }

                    if (parser.DataKind == AtomDataKind.Entry)
                    {
                        if (entry != null)
                        {
                            throw new InvalidOperationException(Strings.AtomParser_SingleEntry_MultipleFound);
                        }

                        entry = parser.CurrentEntry;
                    }
                }

                if (entry == null)
                {
                    throw new InvalidOperationException(Strings.AtomParser_SingleEntry_NoneFound);
                }

                Debug.Assert(parser.DataKind == AtomDataKind.Finished, "the parser didn't end in the right state");
                return entry;
            }
        }

        #endregion Internal methods.

        #region Internal methods.

        /// <summary>Consumes the next chunk of content from the underlying XML reader.</summary>
        /// <returns>
        /// true if another piece of content is available, identified by DataKind.
        /// false if there is no more content.
        /// </returns>
        internal virtual bool Read()
        {
            // When an external caller 'insists', we'll come all the way down (which is the 'most local'
            // scope at which this is known), and unwind as a no-op.
            if (this.DataKind == AtomDataKind.Finished)
            {
                return false;
            }

            while (this.reader.Read())
            {
                if (ShouldIgnoreNode(this.reader))
                {
                    continue;
                }

                Debug.Assert(
                    this.reader.NodeType == XmlNodeType.Element || this.reader.NodeType == XmlNodeType.EndElement,
                    "this.reader.NodeType == XmlNodeType.Element || this.reader.NodeType == XmlNodeType.EndElement -- otherwise we should have ignored or thrown");

                AtomDataKind readerData = ParseStateForReader(this.reader);

                if (this.reader.NodeType == XmlNodeType.EndElement)
                {
                    // The only case in which we expect to see an end-element at the top level
                    // is for a feed. Custom elements and entries should be consumed by
                    // their own parsing methods. However we are tolerant of additional EndElements,
                    // which at this point mean we have nothing else to consume.
                    break;
                }

                switch (readerData)
                {
                    case AtomDataKind.Custom:
                        if (this.DataKind == AtomDataKind.None)
                        {
                            this.kind = AtomDataKind.Custom;
                            return true;
                        }
                        else
                        {
                            CommonUtil.SkipToEndAtDepth(this.reader, this.reader.Depth);
                            continue;
                        }

                    case AtomDataKind.Entry:
                        this.kind = AtomDataKind.Entry;
                        this.ParseCurrentEntry(out this.entry);
                        return true;

                    case AtomDataKind.Feed:
                        if (this.DataKind == AtomDataKind.None)
                        {
                            this.feed = new AtomFeed();
                            this.kind = AtomDataKind.Feed;
                            return true;
                        }

                        throw new InvalidOperationException(Strings.AtomParser_FeedUnexpected);

                    case AtomDataKind.FeedCount:
                        this.ParseCurrentFeedCount();
                        break;

                    case AtomDataKind.PagingLinks:
                        if (this.feed == null)
                        {
                            // paging link outside of feed?
                            throw new InvalidOperationException(Strings.AtomParser_PagingLinkOutsideOfFeed);
                        }

                        this.kind = AtomDataKind.PagingLinks;
                        this.ParseCurrentFeedPagingLinks();

                        // need to take care of both patterns - if the element is empty as
                        // well as when there is an end element specified.
                        CommonUtil.SkipToEndAtDepth(this.reader, this.reader.Depth);
                        return true;

                    default:
                        Debug.Assert(false, "Atom Parser is in a wrong state...Did you add a new AtomDataKind?");
                        break;
                }
            }

            this.kind = AtomDataKind.Finished;
            this.entry = null;
            return false;
        }

        /// <summary>Reads the current property value from the reader.</summary>
        /// <returns>A structured property instance.</returns>
        /// <remarks>
        /// This method should only be called for top-level complex properties.
        /// 
        /// For top-level primitive values, <see cref="TokenizeFromXml"/>
        /// should be used to preserve V1 behavior in which mixed-content
        /// XML elements are allowed.
        /// </remarks>
        internal AtomContentProperty ReadCurrentPropertyValue()
        {
            Debug.Assert(
                this.kind == AtomDataKind.Custom,
                "this.kind == AtomDataKind.Custom -- otherwise caller shouldn't invoke ReadCurrentPropertyValue");
            return this.ReadPropertyValue();
        }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <returns>The count value returned from the server</returns>
        internal long CountValue()
        {
            if (this.countValue == CountStateInitial)
            {
                this.ReadCountValue();
            }
            else if (this.countValue == CountStateFailure)
            {
                throw new InvalidOperationException(Strings.MaterializeFromAtom_CountNotPresent);
            }

            return this.countValue;
        }

        /// <summary>
        /// Read value from the reader and convert it into a PrimitiveParserToken.
        /// </summary>
        /// <param name="type">PrimitiveType to use for the conversion.</param>
        /// <returns>PrimitiveParserToken for the value.</returns>
        internal PrimitiveParserToken TokenizeFromXml(PrimitiveType type)
        {
            return type.TypeConverter.TokenizeFromXml(this.reader);
        }

        #endregion Internal methods.

        #region Private methods.

        /// <summary>
        /// Determines what the parse state should be for the specified 
        /// <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Reader to check.</param>
        /// <returns>The data kind derived from the current element.</returns>
        /// <remarks>
        /// Note that no previous state is considered, so state transitions
        /// aren't handled by the method - instead, certain known elements
        /// are mapped to parser states.
        /// </remarks>
        private static AtomDataKind ParseStateForReader(XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(
                reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement,
                "reader.NodeType == XmlNodeType.Element || EndElement -- otherwise can't determine");

            AtomDataKind result = AtomDataKind.Custom;
            string elementName = reader.LocalName;
            string namespaceURI = reader.NamespaceURI;
            if (Util.AreSame(XmlConstants.AtomNamespace, namespaceURI))
            {
                if (Util.AreSame(XmlConstants.AtomEntryElementName, elementName))
                {
                    result = AtomDataKind.Entry;
                }
                else if (Util.AreSame(XmlConstants.AtomFeedElementName, elementName))
                {
                    result = AtomDataKind.Feed;
                }
                else if (Util.AreSame(XmlConstants.AtomLinkElementName, elementName) &&
                    Util.AreSame(XmlConstants.AtomLinkNextAttributeString, reader.GetAttribute(XmlConstants.AtomLinkRelationAttributeName)))
                {
                    result = AtomDataKind.PagingLinks;
                }
            }
            else if (Util.AreSame(XmlConstants.DataWebMetadataNamespace, namespaceURI))
            {
                if (Util.AreSame(XmlConstants.RowCountElement, elementName))
                {
                    result = AtomDataKind.FeedCount;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads from the specified <paramref name="reader"/> and moves to the 
        /// child element which should match the specified name.
        /// </summary>
        /// <param name="reader">Reader to consume.</param>
        /// <param name="localName">Expected local name of child element.</param>
        /// <param name="namespaceUri">Expected namespace of child element.</param>
        /// <returns>
        /// true if the <paramref name="reader"/> is left position on a child
        /// with the given name; false otherwise.
        /// </returns>
        private static bool ReadChildElement(XmlReader reader, string localName, string namespaceUri)
        {
            Debug.Assert(localName != null, "localName != null");
            Debug.Assert(namespaceUri != null, "namespaceUri != null");
            Debug.Assert(!reader.IsEmptyElement, "!reader.IsEmptyElement");
            Debug.Assert(reader.NodeType != XmlNodeType.EndElement, "reader.NodeType != XmlNodeType.EndElement");

            return reader.Read() && reader.IsStartElement(localName, namespaceUri);
        }

        /// <summary>
        /// Reads the text inside the element on the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Reader to get text from.</param>
        /// <returns>The text inside the specified <paramref name="reader"/>.</returns>
        /// <remarks>
        /// This method was designed to be compatible with the results
        /// of evaluating the text of an XElement.
        /// 
        /// In short, this means that nulls are never returned, and
        /// that all non-text nodes are ignored (but elements are
        /// recursed into).
        /// </remarks>
        private static string ReadElementStringForText(XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");
            if (reader.IsEmptyElement)
            {
                return String.Empty;
            }

            StringBuilder result = new StringBuilder();
            int depth = reader.Depth;
            while (reader.Read())
            {
                if (reader.Depth == depth)
                {
                    Debug.Assert(
                        reader.NodeType == XmlNodeType.EndElement,
                        "reader.NodeType == XmlNodeType.EndElement -- otherwise XmlReader is acting odd");
                    break;
                }

                if (reader.NodeType == XmlNodeType.SignificantWhitespace ||
                    reader.NodeType == XmlNodeType.Text)
                {
                    result.Append(reader.Value);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Checks whether the current node on the specified <paramref name="reader"/> 
        /// should be ignored.
        /// </summary>
        /// <param name="reader">Reader to check.</param>
        /// <returns>true if the node should be ignored; false if it should be processed.</returns>
        /// <remarks>
        /// This method will throw an exception on unexpected content (CDATA, entity references,
        /// text); therefore it should not be used if mixed content is allowed.
        /// </remarks>
        private static bool ShouldIgnoreNode(XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");

            switch (reader.NodeType)
            {
                case XmlNodeType.CDATA:
                case XmlNodeType.EntityReference:
                case XmlNodeType.EndEntity:
                    Error.ThrowInternalError(InternalError.UnexpectedXmlNodeTypeWhenReading);
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.SignificantWhitespace:
                    // With the ODataLib integration, ODataLib ignores Text and other elements
                    // when reading m:properties and other elements where mixed content is not allowed
                    // throw Error.InvalidOperation(Strings.Deserialize_MixedContent(currentType.ElementTypeName));
                    // Error.ThrowInternalError(InternalError.UnexpectedXmlNodeTypeWhenReading);
                    break;
                case XmlNodeType.Element:
                case XmlNodeType.EndElement:
                    return false;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks if the given content type string matches with 'application/xml' or 
        /// 'application/atom+xml' case insensitively.
        /// </summary>
        /// <param name="contentType">Input content type.</param>
        /// <returns>true if match found, false otherwise.</returns>
        private static bool IsAllowedContentType(string contentType)
        {
            return (String.Equals(XmlConstants.MimeApplicationXml, contentType, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(XmlConstants.MimeApplicationAtom, contentType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the given link type matches 'application/xml' case insensitively.
        /// </summary>
        /// <param name="linkType">Input link type.</param>
        /// <returns>true if match found, false otherwise.</returns>
        private static bool IsAllowedAssociationLinkType(string linkType)
        {
            return String.Equals(XmlConstants.MimeApplicationXml, linkType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the given link type matches 'application/atom+xml;type=feed' or
        /// 'application/atom+xml;type=entry' case insensitively.
        /// </summary>
        /// <param name="linkType">Input link type.</param>
        /// <param name="isFeed">Output parameter indicating whether we are reading a feed or an entry inline.</param>
        /// <returns>true if match found, false otherwise.</returns>
        private static bool IsAllowedNavigationLinkType(string linkType, out bool isFeed)
        {
            isFeed = String.Equals(XmlConstants.LinkMimeTypeFeed, linkType, StringComparison.OrdinalIgnoreCase);
            return isFeed ? true : String.Equals(XmlConstants.LinkMimeTypeEntry, linkType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the content on the reader into the specified <paramref name="targetEntry"/>.
        /// </summary>
        /// <param name="targetEntry">Target to read values into.</param>
        private void ParseCurrentContent(AtomEntry targetEntry)
        {
            Debug.Assert(targetEntry != null, "targetEntry != null");
            Debug.Assert(this.reader.NodeType == XmlNodeType.Element, "this.reader.NodeType == XmlNodeType.Element");

            string propertyValue = this.reader.GetAttributeEx(XmlConstants.AtomContentSrcAttributeName, XmlConstants.AtomNamespace);
            if (propertyValue != null)
            {
                // This is a media link entry
                if (!CommonUtil.ReadEmptyElement(this.reader))
                {
                    throw Error.InvalidOperation(Strings.Deserialize_ExpectedEmptyMediaLinkEntryContent);
                }

                targetEntry.MediaLinkEntry = true;

                // We should not use Util.CreateUri method to convert relative uri's into absolute uris/
                // Look at ConvertHRefAttributeValueIntoURI method in this class - this makes it consistent
                // with other href attributes that we read from the response payload.
                // can we treat this as similar to an href attribute and use ConvertHRefAttributeValueIntoURI method
                Debug.Assert(targetEntry.EntityDescriptor != null, "EntityDescriptor has not been created yet.");
                targetEntry.EntityDescriptor.ReadStreamUri = this.ProcessUriFromPayload(propertyValue);
            }
            else
            {
                // This is a regular (non-media link) entry
                if (targetEntry.MediaLinkEntry.HasValue && targetEntry.MediaLinkEntry.Value)
                {
                    // This means we saw a <m:Properties> element but now we have a Content element
                    // that's not just a media link entry pointer (src)
                    throw Error.InvalidOperation(Strings.Deserialize_ContentPlusPropertiesNotAllowed);
                }

                targetEntry.MediaLinkEntry = false;

                propertyValue = this.reader.GetAttributeEx(XmlConstants.AtomTypeAttributeName, XmlConstants.AtomNamespace);
                if (AtomParser.IsAllowedContentType(propertyValue))
                {
                    if (this.reader.IsEmptyElement)
                    {
                        return;
                    }

                    if (ReadChildElement(this.reader, XmlConstants.AtomPropertiesElementName, XmlConstants.DataWebMetadataNamespace))
                    {
                        this.ReadCurrentProperties(targetEntry.DataValues);
                    }
                    else if (this.reader.NodeType != XmlNodeType.EndElement)
                    {
                        throw Error.InvalidOperation(Strings.Deserialize_NotApplicationXml);
                    }
                }
            }
        }

        /// <summary>
        /// read the m2:count tag in the feed
        /// </summary>
        private void ReadCountValue()
        {
            Debug.Assert(this.countValue == CountStateInitial, "Count value is not in the initial state");

            if (this.CurrentFeed != null &&
                this.CurrentFeed.Count.HasValue)
            {
                this.countValue = this.CurrentFeed.Count.Value;
                return;
            }

            // find the first element tag
            while (this.reader.NodeType != XmlNodeType.Element && this.reader.Read())
            {
            }

            if (this.reader.EOF)
            {
                throw new InvalidOperationException(Strings.MaterializeFromAtom_CountNotPresent);
            }

            // the tag Should only be <feed> or <links> tag:
            Debug.Assert(
                (Util.AreSame(XmlConstants.AtomNamespace, this.reader.NamespaceURI) &&
                Util.AreSame(XmlConstants.AtomFeedElementName, this.reader.LocalName)) ||
                (Util.AreSame(XmlConstants.DataWebNamespace, this.reader.NamespaceURI) &&
                Util.AreSame(XmlConstants.LinkCollectionElementName, this.reader.LocalName)),
                "<feed> or <links> tag expected");

            // Create the XElement for look-ahead
            // DEVNOTE(pqian):
            // This is not streaming friendly!
            XElement element = XElement.Load(this.reader);
            this.reader.Close();

            // Read the count value from the xelement
            XElement countNode = element.Descendants(XNamespace.Get(XmlConstants.DataWebMetadataNamespace) + XmlConstants.RowCountElement).FirstOrDefault();

            if (countNode == null)
            {
                throw new InvalidOperationException(Strings.MaterializeFromAtom_CountNotPresent);
            }
            else
            {
                if (!long.TryParse(countNode.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out this.countValue))
                {
                    throw new FormatException(Strings.MaterializeFromAtom_CountFormatError);
                }
            }

            this.reader = new Microsoft.OData.Service.Client.Xml.XmlAtomErrorReader(element.CreateReader());
        }

        /// <summary>Parses a link for the specified <paramref name="targetEntry"/>.</summary>
        /// <param name="targetEntry">Entry to update with link information.</param>
        private void ParseCurrentLink(AtomEntry targetEntry)
        {
            Debug.Assert(targetEntry != null, "targetEntry != null");
            Debug.Assert(
                this.reader.NodeType == XmlNodeType.Element,
                "this.reader.NodeType == XmlNodeType.Element -- otherwise we shouldn't try to parse a link");
            Debug.Assert(
                this.reader.LocalName == "link",
                "this.reader.LocalName == 'link' -- otherwise we shouldn't try to parse a link");

            string propertyName = null;

            string relation = this.reader.GetAttribute(XmlConstants.AtomLinkRelationAttributeName);
            if (relation == null)
            {
                return;
            }

            if ((relation == XmlConstants.AtomEditRelationAttributeValue ||
                XmlConstants.AtomEditRelationAttributeValue == UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.IanaLinkRelationsNamespace))
                && targetEntry.EditLink == null)
            {
                // Only process the first link that has @rel='edit'.
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(Strings.Context_MissingEditLinkInResponseBody);
                }

                targetEntry.EditLink = this.ProcessUriFromPayload(href);
            }
            else if ((relation == XmlConstants.AtomSelfRelationAttributeValue ||
                XmlConstants.AtomSelfRelationAttributeValue == UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.IanaLinkRelationsNamespace))
                && targetEntry.QueryLink == null)
            {
                // Only process the first link that has @rel='self'.
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(Strings.Context_MissingSelfLinkInResponseBody);
                }

                targetEntry.QueryLink = this.ProcessUriFromPayload(href);
            }
            else if ((relation == XmlConstants.AtomEditMediaRelationAttributeValue ||
                XmlConstants.AtomEditMediaRelationAttributeValue == UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.IanaLinkRelationsNamespace))
                && targetEntry.MediaEditUri == null)
            {
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(Strings.Context_MissingEditMediaLinkInResponseBody);
                }

                targetEntry.MediaEditUri = this.ProcessUriFromPayload(href);
                targetEntry.EntityDescriptor.StreamETag = this.reader.GetAttribute(XmlConstants.AtomETagAttributeName, XmlConstants.DataWebMetadataNamespace);
            }
            else if ((propertyName = UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.DataWebRelatedNamespace)) != null)
            {
                #region Read the navigation link from the href attribute and handle expanded entities, if any
                bool isFeed = false;
                string propertyValueText = this.reader.GetAttribute(XmlConstants.AtomTypeAttributeName);
                if (!IsAllowedNavigationLinkType(propertyValueText, out isFeed))
                {
                    return;
                }

                // Get the link for the navigation property from the href attribute
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(Strings.Context_MissingRelationshipLinkInResponseBody(propertyName));
                }

                // Add the link to the target entry so that we do use the link while querying the relationship
                Uri uri = this.ProcessUriFromPayload(href);
                targetEntry.AddNavigationLink(propertyName, uri, isFeed);

                if (!this.reader.IsEmptyElement)
                {
                    this.HandleExpandedNavigationProperties(targetEntry, propertyName, isFeed);
                }
                #endregion Read the navigation link from the href attribute and handle expanded entities, if any
            }
            else if ((propertyName = UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.DataWebRelatedLinkNamespace)) != null)
            {
                string propertyValueText = this.reader.GetAttribute(XmlConstants.AtomTypeAttributeName);

                // check type="application/xml"
                if (!IsAllowedAssociationLinkType(propertyValueText))
                {
                    return;
                }

                // Get the related link for the navigation property from the href attribute
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(Strings.Context_MissingRelationshipLinkInResponseBody(propertyName));
                }

                // Add the link to the target entry so that we do use the link while querying the association
                Uri uri = this.ProcessUriFromPayload(href);
                targetEntry.AddAssociationLink(propertyName, uri);
            }
            else if ((propertyName = UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.DataWebMediaResourceEditNamespace)) != null)
            {
                this.ReadNamedStreamInfoFromLinkElement(targetEntry, propertyName, true/*editLink*/);
            }
            else if ((propertyName = UriUtil.GetNameFromAtomLinkRelationAttribute(relation, XmlConstants.DataWebMediaResourceNamespace)) != null)
            {
                this.ReadNamedStreamInfoFromLinkElement(targetEntry, propertyName, false/*editLink*/);
            }
        }

        /// <summary>
        /// Reads a property value and adds it as a text or a sub-property of 
        /// the specified <paramref name="property"/>.
        /// </summary>
        /// <param name="property">Property to read content into.</param>
        private void ReadPropertyValueIntoResult(AtomContentProperty property)
        {
            Debug.Assert(this.reader != null, "reader != null");
            Debug.Assert(property != null, "property != null");

            switch (this.reader.NodeType)
            {
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    if (property.PrimitiveToken != null)
                    {
                        throw Error.InvalidOperation(Strings.Deserialize_MixedTextWithComment);
                    }

                    // this is a string token
                    property.PrimitiveToken = new TextPrimitiveParserToken(this.reader.Value);
                    break;
                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.EndElement:
                    // Do nothing.
                    // ProcessingInstruction, Whitespace would have thrown before
                    break;
                case XmlNodeType.Element:
                    // We found an element while reading a property value. This should be
                    // a complex type.
                    if (property.PrimitiveToken != null)
                    {
                        throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
                    }

                    // Complex types:
                    property.EnsureProperties();
                    AtomContentProperty prop = this.ReadPropertyValue();

                    if (prop != null)
                    {
                        property.Properties.Add(prop);
                    }

                    break;

                default:
                    throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
            }
        }

        /// <summary>This method will read a string or a complex type.</summary>
        /// <returns>The property value read.</returns>
        /// <remarks>Always checks for null attribute.</remarks>
        private AtomContentProperty ReadPropertyValue()
        {
            Debug.Assert(this.reader != null, "reader != null");
            Debug.Assert(
                this.reader.NodeType == XmlNodeType.Element,
                "reader.NodeType == XmlNodeType.Element -- otherwise caller is confused as to where the reader is");

            if (!this.IsDataWebElement)
            {
                // we expect <d:PropertyName>...</d:PropertyName> only
                CommonUtil.SkipToEndAtDepth(this.reader, this.reader.Depth);
                return null;
            }

            AtomContentProperty result = new AtomContentProperty();
            result.Name = this.reader.LocalName;
            result.TypeName = this.reader.GetAttributeEx(XmlConstants.AtomTypeAttributeName, XmlConstants.DataWebMetadataNamespace);
            result.IsNull = Util.DoesNullAttributeSayTrue(this.reader);

            // simple optimization for empty and obviously null properties
            if (result.IsNull && this.reader.IsEmptyElement)
            {
                return result;
            }

            PrimitiveType type;
            if (result.TypeName != null && PrimitiveType.TryGetPrimitiveType(result.TypeName, out type))
            {
                // primitive type - tokenize it
                // DEVNOTE(pqian):
                // 1. If the typeName is null, it can be either a complex type or edm.string
                // We must drill into the element to find out
                // 2. We should throw if the Edm type is unrecognized, but this was not in V1/V2 
                // So we need to ignore unknown types
                result.PrimitiveToken = this.TokenizeFromXml(type);
            }
            else
            {
                // complex or collection type - recursive parse and store into result
                if (!this.reader.IsEmptyElement)
                {
                    int depth = this.reader.Depth;
                    while (this.reader.Read())
                    {
                        this.ReadPropertyValueIntoResult(result);
                        if (this.reader.Depth == depth)
                        {
                            break;
                        }
                    }
                }
            }

            if (result.PrimitiveToken == null && !result.IsNull)
            {
                // Empty String or actual null property can both cause PrimitiveToken to be null
                // NOTE: Ideally we should leave this null when parsing complex types or collection
                // But the V1/V2 behavior is to set them as Empty
                result.PrimitiveToken = TextPrimitiveParserToken.Empty;
            }

            return result;
        }

        /// <summary>
        /// Reads properties from the current reader into the 
        /// specified <paramref name="values"/> collection.
        /// </summary>
        /// <param name="values">Values to read into.</param>
        private void ReadCurrentProperties(List<AtomContentProperty> values)
        {
            Debug.Assert(values != null, "values != null");
            Debug.Assert(this.reader.NodeType == XmlNodeType.Element, "this.reader.NodeType == XmlNodeType.Element");

            while (this.reader.Read())
            {
                if (ShouldIgnoreNode(this.reader))
                {
                    continue;
                }

                if (this.reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }

                if (this.reader.NodeType == XmlNodeType.Element)
                {
                    AtomContentProperty prop = this.ReadPropertyValue();

                    if (prop != null)
                    {
                        values.Add(prop);
                    }
                }
            }
        }

        /// <summary>
        /// Parses the current reader into a new <paramref name="targetEntry"/> 
        /// instance.
        /// </summary>
        /// <param name="targetEntry">
        /// After invocation, the target entry that was created as a result
        /// of parsing the current reader.
        /// </param>
        private void ParseCurrentEntry(out AtomEntry targetEntry)
        {
            Debug.Assert(this.reader.NodeType == XmlNodeType.Element, "this.reader.NodeType == XmlNodeType.Element");

            // Push reader.
            var callbackResult = this.entryCallback(this.reader);
            Debug.Assert(callbackResult.Key != null, "callbackResult.Key != null");
            this.readers.Push(this.reader);
            this.reader = callbackResult.Key;

            this.reader.Read();
            Debug.Assert(this.reader.LocalName == "entry", "this.reader.LocalName == 'entry' - otherwise we're not reading the subtree");

            targetEntry = new AtomEntry(this.MaxProtocolVersion);
            targetEntry.DataValues = new List<AtomContentProperty>();
            targetEntry.Tag = callbackResult.Value;
            targetEntry.EntityDescriptor.ETag = this.reader.GetAttribute(XmlConstants.AtomETagAttributeName, XmlConstants.DataWebMetadataNamespace);

            while (this.reader.Read())
            {
                if (ShouldIgnoreNode(this.reader))
                {
                    continue;
                }

                if (this.reader.NodeType == XmlNodeType.Element)
                {
                    int depth = this.reader.Depth;
                    string elementName = this.reader.LocalName;
                    string namespaceURI = this.reader.NamespaceURI;
                    if (namespaceURI == XmlConstants.AtomNamespace)
                    {
                        if (elementName == XmlConstants.AtomCategoryElementName && targetEntry.TypeName == null)
                        {
                            string text = this.reader.GetAttributeEx(XmlConstants.AtomCategorySchemeAttributeName, XmlConstants.AtomNamespace);
                            if (text == this.typeScheme && !targetEntry.TypeNameHasBeenSet)
                            {
                                targetEntry.TypeNameHasBeenSet = true;
                                targetEntry.TypeName = this.reader.GetAttributeEx(XmlConstants.AtomCategoryTermAttributeName, XmlConstants.AtomNamespace);
                            }
                        }
                        else if (elementName == XmlConstants.AtomContentElementName)
                        {
                            this.ParseCurrentContent(targetEntry);
                        }
                        else if (elementName == XmlConstants.AtomIdElementName && targetEntry.Identity == null)
                        {
                            // The .Identity == null check ensures that only the first id element is processed.
                            string idText = ReadElementStringForText(this.reader);
                            WebUtil.ValidateIdentityValue(idText);
                            targetEntry.Identity = idText;
                        }
                        else if (elementName == XmlConstants.AtomLinkElementName)
                        {
                            this.ParseCurrentLink(targetEntry);
                        }
                    }
                    else if (namespaceURI == XmlConstants.DataWebMetadataNamespace)
                    {
                        if (elementName == XmlConstants.AtomPropertiesElementName)
                        {
                            if (targetEntry.MediaLinkEntry.HasValue && !targetEntry.MediaLinkEntry.Value)
                            {
                                // This means we saw a non-empty <atom:Content> element but now we have a Properties element
                                // that also carries properties
                                throw Error.InvalidOperation(Strings.Deserialize_ContentPlusPropertiesNotAllowed);
                            }

                            targetEntry.MediaLinkEntry = true;

                            if (!this.reader.IsEmptyElement)
                            {
                                this.ReadCurrentProperties(targetEntry.DataValues);
                            }
                        }
                        else if (elementName == XmlConstants.ActionElementName || elementName == XmlConstants.FunctionElementName)
                        {
                            this.ReadOperationDescriptor(targetEntry);
                        }
                    }

                    CommonUtil.SkipToEndAtDepth(this.reader, depth);
                }
            }

            if (targetEntry.Identity == null)
            {
                throw Error.InvalidOperation(Strings.Deserialize_MissingIdElement);
            }

            this.reader = this.readers.Pop();
        }

        /// <summary>Parses the value for the current feed count.</summary>
        /// <remarks>This method will update the value on the current feed.</remarks>
        private void ParseCurrentFeedCount()
        {
            if (this.feed == null)
            {
                throw new InvalidOperationException(Strings.AtomParser_FeedCountNotUnderFeed);
            }

            if (this.feed.Count.HasValue)
            {
                throw new InvalidOperationException(Strings.AtomParser_ManyFeedCounts);
            }

            long count;
            if (!long.TryParse(MaterializeAtom.ReadElementString(this.reader, true), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out count))
            {
                throw new FormatException(Strings.MaterializeFromAtom_CountFormatError);
            }

            this.feed.Count = count;
        }

        /// <summary>
        /// Parsing paging links
        /// </summary>
        private void ParseCurrentFeedPagingLinks()
        {
            // feed should never be null here since there is an outer check
            // just need to assert
            Debug.Assert(this.feed != null, "Trying to parser paging links but feed is null.");

            if (this.feed.NextLink != null)
            {
                // we have set next link before, this is a duplicate
                // atom spec does not allow duplicated next links
                throw new InvalidOperationException(Strings.AtomMaterializer_DuplicatedNextLink);
            }

            string nextLink = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);

            if (nextLink == null)
            {
                throw new InvalidOperationException(Strings.AtomMaterializer_LinksMissingHref);
            }
            else
            {
                this.feed.NextLink = this.ProcessUriFromPayload(nextLink);
            }
        }

        /// <summary>
        /// creates a new uri instance which takes into account the base uri of the reader.
        /// </summary>
        /// <param name="uriString">the uri attribute value as a string.</param>
        /// <param name="getErrorMessage">the error message to throw if the newly created uri was not an absolute uri.</param>
        /// <returns>a new instance of uri as refered by the <paramref name="uriString"/></returns>
        private Uri ProcessUriFromPayload(string uriString, Func<String> getErrorMessage)
        {
            Uri uri = Util.CreateUri(uriString, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri && !String.IsNullOrEmpty(this.reader.BaseURI))
            {
                Uri xmlBaseUri = Util.CreateUri(this.reader.BaseURI, UriKind.RelativeOrAbsolute);

                // The reason why we can't use Util.CreateUri function here, is that the util method
                // checks for trailing slashes in the baseuri and starting forward slashes in the request uri
                // and does some tricks which is not consistent with the uri class behaviour. Hence using the
                // uri class directly here.
                uri = new Uri(xmlBaseUri, uri);
            }

            if (!uri.IsAbsoluteUri)
            {
                // If the uri is not an absolute uri (after applying the xml:base), then we need to use the context
                // base uri to convert into an absolute uri.
                // Again the same comment as above applies : Util.CreateUri does a bunch of magic with '/' and doesn't
                // follow the right uri behavior
                uri = new Uri(this.baseUriResolver.GetBaseUriWithSlash(getErrorMessage), uri);
            }

            return uri;
        }

        /// <summary>
        /// creates a new uri instance which takes into account the base uri of the reader.
        /// </summary>
        /// <param name="uriString">the uri attribute value as a string.</param>
        /// <returns>a new instance of uri as refered by the <paramref name="uriString"/></returns>
        private Uri ProcessUriFromPayload(string uriString)
        {
            return this.ProcessUriFromPayload(uriString, () => Strings.Context_BaseUriRequired);
        }

        /// <summary>
        /// Reads an OData operation and constructs an OperationDescriptor representing the OData operation.
        /// </summary>
        /// <param name="targetEntry">the entry containing the OData operation. The constructed operation descriptor is added to the list of operation descriptors of the entry.</param>
        private void ReadOperationDescriptor(AtomEntry targetEntry)
        {
            Debug.Assert(targetEntry != null, "targetEntry != null");
            Debug.Assert(this.reader.NodeType == XmlNodeType.Element, "this.reader.NodeType == XmlNodeType.Element");
            Debug.Assert(this.reader.NamespaceURI == XmlConstants.DataWebMetadataNamespace, "The NamespaceURI was expected to be pointing to the metadata namespace.");
            Debug.Assert(this.reader.LocalName == XmlConstants.ActionElementName || this.reader.LocalName == XmlConstants.FunctionElementName, "An xml element named 'action' or 'function' was expected.");

            OperationDescriptor operationDescriptor;
            string operationTypeName = this.reader.LocalName;

            if (operationTypeName == XmlConstants.ActionElementName)
            {
                operationDescriptor = new ActionDescriptor();
            }
            else
            {
                Debug.Assert(operationTypeName == XmlConstants.FunctionElementName, "An xml element named 'function' was expected.");
                operationDescriptor = new FunctionDescriptor();
            }

            Debug.Assert(operationDescriptor != null, "operationDescriptor != null");

            // read all the attributes.
            while (this.reader.MoveToNextAttribute())
            {
                if (object.ReferenceEquals(this.reader.NamespaceURI, XmlConstants.EmptyNamespace))
                {
                    string attributeValue = this.reader.Value;
                    string localName = this.reader.LocalName;

                    if (localName == XmlConstants.ActionMetadataAttributeName)
                    {
                        // For metadata, if the URI is relative we don't attempt to make it absolute using the service
                        // base URI, because the ODataOperation metadata URI is relative to $metadata.
                        operationDescriptor.Metadata = Util.CreateUri(attributeValue, UriKind.RelativeOrAbsolute);
                    }
                    else if (localName == XmlConstants.ActionTargetAttributeName)
                    {
                        operationDescriptor.Target = this.ProcessUriFromPayload(attributeValue, () => Strings.AtomParser_OperationTargetUriIsNotAbsolute(operationTypeName, attributeValue));
                    }
                    else if (localName == XmlConstants.ActionTitleAttributeName)
                    {
                        operationDescriptor.Title = attributeValue;
                    }

                    // skip unknown attributes
                }
            }

            if (operationDescriptor.Metadata == null)
            {
                throw new InvalidOperationException(Strings.AtomParser_OperationMissingMetadataAttribute(operationTypeName));
            }

            if (operationDescriptor.Target == null)
            {
                throw new InvalidOperationException(Strings.AtomParser_OperationMissingTargetAttribute(operationTypeName));
            }

            targetEntry.EntityDescriptor.AddOperationDescriptor(operationDescriptor);

            // move back to the element node.
            this.reader.MoveToElement();
        }

        /// <summary>
        /// Handle the expanded navigation property entities for the given target entry
        /// </summary>
        /// <param name="targetEntry">parent entity containing the navigation property.</param>
        /// <param name="propertyName">name of the navigation property.</param>
        /// <param name="isFeed">whether the navigation property is a feed or entry. This information is obtained from the type attribute of the link element.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "not required to dispose of the nested XmlReader")]
        private void HandleExpandedNavigationProperties(AtomEntry targetEntry, string propertyName, bool isFeed)
        {
            Debug.Assert(!this.reader.IsEmptyElement, "the current element has some child content");

            if (!ReadChildElement(this.reader, XmlConstants.AtomInlineElementName, XmlConstants.DataWebMetadataNamespace))
            {
                return;
            }

            bool emptyInlineCollection = this.reader.IsEmptyElement;
            object propertyValue = null;

            if (!emptyInlineCollection)
            {
                AtomFeed nestedFeed = null;
                AtomEntry nestedEntry = null;
                List<AtomEntry> feedEntries = null;

                Debug.Assert(this.reader is Xml.XmlWrappingReader, "reader must be a instance of XmlWrappingReader");
                Uri readerBaseUri = string.IsNullOrEmpty(this.reader.BaseURI) ? null : new Uri(this.reader.BaseURI, UriKind.Absolute);
                XmlReader nestedReader = Xml.XmlWrappingReader.CreateReader(readerBaseUri, this.reader.ReadSubtree());
                nestedReader.Read();
                Debug.Assert(nestedReader.LocalName == "inline", "nestedReader.LocalName == 'inline'");

                AtomParser nested = new AtomParser(nestedReader, this.entryCallback, this.typeScheme, this.currentDataNamespace, this.baseUriResolver, this.MaxProtocolVersion);
                while (nested.Read())
                {
                    switch (nested.DataKind)
                    {
                        case AtomDataKind.Feed:
                            feedEntries = new List<AtomEntry>();
                            nestedFeed = nested.CurrentFeed;
                            propertyValue = nestedFeed;
                            break;
                        case AtomDataKind.Entry:
                            nestedEntry = nested.CurrentEntry;
                            if (feedEntries != null)
                            {
                                feedEntries.Add(nestedEntry);
                            }
                            else
                            {
                                propertyValue = nestedEntry;
                            }

                            break;
                        case AtomDataKind.PagingLinks:
                            // Here the inner feed parser found a paging link, and stored it on nestedFeed.NextPageLink
                            // we are going to add it into a link table and associate
                            // with the collection at AtomMaterializer::Materialize()
                            // Do nothing for now.
                            break;
                        default:
                            throw new InvalidOperationException(Strings.AtomParser_UnexpectedContentUnderExpandedLink);
                    }
                }

                if (nestedFeed != null)
                {
                    Debug.Assert(
                        nestedFeed.Entries == null,
                        "nestedFeed.Entries == null -- otherwise someone initialized this for us");
                    nestedFeed.Entries = feedEntries;
                }
            }

            AtomContentProperty property = new AtomContentProperty();
            property.Name = propertyName;

            if (emptyInlineCollection || propertyValue == null)
            {
                property.IsNull = true;
                if (isFeed)
                {
                    property.Feed = new AtomFeed();
                    property.Feed.Entries = Enumerable.Empty<AtomEntry>();
                }
                else
                {
                    property.Entry = new AtomEntry(this.MaxProtocolVersion);
                    property.Entry.IsNull = true;
                }
            }
            else
            {
                property.Feed = propertyValue as AtomFeed;
                property.Entry = propertyValue as AtomEntry;
            }

            targetEntry.DataValues.Add(property);
        }

        /// <summary>
        /// Read the named stream info from the current link element.
        /// </summary>
        /// <param name="targetEntry">parent entity containing the named stream.</param>
        /// <param name="name">name of the stream.</param>
        /// <param name="editLink">whether the current link element represents the edit link of the stream.</param>
        private void ReadNamedStreamInfoFromLinkElement(AtomEntry targetEntry, string name, bool editLink)
        {
            StreamDescriptor streamInfo = targetEntry.EntityDescriptor.AddStreamInfoIfNotPresent(name);

            // In ParseCurrentLink method, we only read href from the first link element. If the link element is repeated
            // with the same rel attribute value, the corresponding ones are ignored.
            if ((editLink && streamInfo.EditLink == null) || (!editLink && streamInfo.SelfLink == null))
            {
                // Get the related link for the navigation property from the href attribute
                string href = this.reader.GetAttribute(XmlConstants.AtomHRefAttributeName);
                if (String.IsNullOrEmpty(href))
                {
                    throw Error.InvalidOperation(editLink ? Strings.Context_MissingHRefInNamedStreamEditLinkElement(name) : Strings.Context_MissingHRefInNamedStreamSelfLinkElement(name));
                }

                if (editLink)
                {
                    streamInfo.EditLink = this.ProcessUriFromPayload(href);
                    streamInfo.ETag = this.reader.GetAttribute(XmlConstants.AtomETagAttributeName, XmlConstants.DataWebMetadataNamespace);
                }
                else
                {
                    streamInfo.SelfLink = this.ProcessUriFromPayload(href);
                }

                string contentType = this.reader.GetAttribute(XmlConstants.AtomTypeAttributeName);
                if (String.IsNullOrEmpty(streamInfo.ContentType))
                {
                    streamInfo.ContentType = contentType;
                }
                else
                {
                    if (!streamInfo.ContentType.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                    {
                        throw Error.InvalidOperation(Strings.Context_NamedStreamDeclareDifferentContentType(name));
                    }
                }
            }
            else
            {
                // Duplicated edit and self link is now blocked
                throw Error.InvalidOperation(editLink ? Strings.Context_StreamPropertyWithMultipleEditLinks(name) : Strings.Context_StreamPropertyWithMultipleSelfLinks(name));
            }
        }
        #endregion Private methods.

        /// <summary>
        /// Responsible for pretending to be an AtomParser, but simply 
        /// handing out the pre parsed AtomEntry(s)
        /// </summary>
        private class ResultsWrapper : AtomParser
        {
            /// <summary>
            /// results to wrap
            /// </summary>
            private IEnumerator<AtomEntry> resultEnumerator;

            /// <summary>
            /// ctor for creating the wrapper around the results
            /// </summary>
            /// <param name="results">the results to be wraped</param>
            internal ResultsWrapper(IEnumerable<AtomEntry> results)
            {
                Debug.Assert(results != null, "send an empty collection instead of null");

                this.resultEnumerator = results.GetEnumerator();
            }

            /// <summary>
            /// Reads the next wrapped result
            /// </summary>
            /// <returns>true if more results are avialable, false otherwise</returns>
            /// <summary>
            /// Returns true if the current element is in the data web namespace
            /// </summary>
            internal override bool IsDataWebElement
            {
                get
                {
                    // we won't hit this because we never have a AtomDataKind.Custom
                    return false;
                }
            }

            /// <summary>Consumes the next chunk of content from the underlying XML reader.</summary>
            /// <returns>
            /// true if another piece of content is available, identified by DataKind.
            /// false if there is no more content.
            /// </returns>
            internal override bool Read()
            {
                if (this.resultEnumerator.MoveNext())
                {
                    this.kind = AtomDataKind.Entry;
                    this.entry = this.resultEnumerator.Current;
                    return true;
                }
                else
                {
                    this.kind = AtomDataKind.Finished;
                    this.entry = null;
                    return false;
                }
            }
        }
    }

    /// <summary>Object to be assigned to the Tag on the AtomEntry object.</summary>
    internal class AtomTag
    {
        /// <summary>Constructs a new AtomTag instance.</summary>
        /// <param name="entry">XML data of the ATOM entry.</param>
        /// <param name="baseUri">The xml base of the feed or entry containing the current ATOM entry.</param>
        public AtomTag(XElement entry, Uri baseUri)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(baseUri == null || baseUri.IsAbsoluteUri, "baseUri == null || baseUri.IsAbsoluteUri");

            this.Entry = entry;
            this.BaseUri = baseUri;
        }

        /// <summary>XML data of the ATOM entry.</summary>
        public XElement Entry
        {
            get;
            private set;
        }

        /// <summary>The xml base of the feed or entry containing the current ATOM entry.</summary>
        public Uri BaseUri
        {
            get;
            private set;
        }
    }
}
