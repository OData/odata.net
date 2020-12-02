//---------------------------------------------------------------------
// <copyright file="GmlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;

    /// <summary>
    /// Gml Reader
    /// </summary>
    internal class GmlReader : SpatialReader<XmlReader>
    {
        /// <summary>
        /// Creates a reader that that will send messages to the destination during read.
        /// </summary>
        /// <param name="destination">The instance to message to during read.</param>
        public GmlReader(SpatialPipeline destination)
            : base(destination)
        {
        }

        /// <summary>
        /// Parses some serialized format that represents a geography value, passing the result down the pipeline.
        /// </summary>
        /// <param name = "input">The XmlReader instance to read from.</param>
        protected override void ReadGeographyImplementation(XmlReader input)
        {
            new Parser(input, new TypeWashedToGeographyLatLongPipeline(Destination)).Read();
        }

        /// <summary>
        /// Parses some serialized format that represents a geometry value, passing the result down the pipeline.
        /// </summary>
        /// <param name = "input">The XmlReader instance to read from.</param>
        protected override void ReadGeometryImplementation(XmlReader input)
        {
            new Parser(input, new TypeWashedToGeometryPipeline(Destination)).Read();
        }

        /// <summary>
        /// This class parses the xml and calls the pipeline based on what is parsed
        /// </summary>
        private class Parser
        {
            /// <summary>
            /// Delimiters used in position arrays. As per Xml spec white space characters is: #x20 | #x9 | #xD | #xA
            /// </summary>
            private static readonly char[] coordinateDelimiter = { ' ', '\t', '\r', '\n' };

            /// <summary>
            /// List of known gml elements that can be ignored by the parser
            /// </summary>
            private static readonly Dictionary<String, String> skippableElements = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { GmlConstants.Name, GmlConstants.Name },
                    { GmlConstants.Description, GmlConstants.Description },
                    { GmlConstants.MetadataProperty, GmlConstants.MetadataProperty },
                    { GmlConstants.DescriptionReference, GmlConstants.DescriptionReference },
                    { GmlConstants.IdentifierElement, GmlConstants.IdentifierElement }
                };

            #region Atomized Constants

            /// <summary>
            /// Atomized gml namespace
            /// </summary>
            private readonly string gmlNamespace;

            /// <summary>
            /// Atomized Full Globe namespace
            /// </summary>
            private readonly string fullGlobeNamespace;

            #endregion

            /// <summary>
            /// Output pipeline
            /// </summary>
            private readonly TypeWashedPipeline pipeline;

            /// <summary>
            /// Input reader
            /// </summary>
            private readonly XmlReader reader;

            /// <summary>
            /// Number of points in the current figure
            /// </summary>
            private int points; // number of points in current figure

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="reader">Input Reader</param>
            /// <param name="pipeline">Output pipeline</param>
            internal Parser(XmlReader reader, TypeWashedPipeline pipeline)
            {
                Debug.Assert(reader != null && pipeline != null, "reader or pipeline is null");
                this.reader = reader;
                this.pipeline = pipeline;

                var nametable = this.reader.NameTable;
                this.gmlNamespace = nametable.Add(GmlConstants.GmlNamespace);
                this.fullGlobeNamespace = nametable.Add(GmlConstants.FullGlobeNamespace);
            }

            /// <summary>
            /// Read
            /// </summary>
            public void Read()
            {
                this.ParseGmlGeometry(true);
            }

            /// <summary>
            /// Parses the top level element in the document
            /// </summary>
            /// <param name="readCoordinateSystem">Whether coordinate system is expected</param>
            private void ParseGmlGeometry(bool readCoordinateSystem)
            {
                if (!this.reader.IsStartElement())
                {
                    throw new FormatException(Strings.GmlReader_ExpectReaderAtElement);
                }

                // handle SRID here
                if (object.ReferenceEquals(this.reader.NamespaceURI, this.gmlNamespace))
                {
                    this.ReadAttributes(readCoordinateSystem);

                    switch (this.reader.LocalName)
                    {
                        case GmlConstants.Point:
                            this.ParseGmlPointShape();
                            break;
                        case GmlConstants.LineString:
                            this.ParseGmlLineStringShape();
                            break;
                        case GmlConstants.Polygon:
                            this.ParseGmlPolygonShape();
                            break;
                        case GmlConstants.MultiPoint:
                            this.ParseGmlMultiPointShape();
                            break;
                        case GmlConstants.MultiLineString:
                            this.ParseGmlMultiCurveShape();
                            break;
                        case GmlConstants.MultiPolygon:
                            this.ParseGmlMultiSurfaceShape();
                            break;
                        case GmlConstants.Collection:
                            this.ParseGmlMultiGeometryShape();
                            break;
                        default:
                            throw new FormatException(Strings.GmlReader_InvalidSpatialType(this.reader.LocalName));
                    }
                }
                else if (object.ReferenceEquals(this.reader.NamespaceURI, this.fullGlobeNamespace) && this.reader.LocalName.Equals(GmlConstants.FullGlobe, StringComparison.Ordinal))
                {
                    this.ReadAttributes(readCoordinateSystem);
                    this.ParseGmlFullGlobeElement();
                }
                else
                {
                    throw new FormatException(Strings.GmlReader_ExpectReaderAtElement);
                }
            }

            /// <summary>
            /// Set the CoordinateSystem
            /// </summary>
            /// <param name="expectSrsName">Should we allow CRS attributes</param>
            private void ReadAttributes(bool expectSrsName)
            {
                bool crsSet = false;

                // skip whitespace
                this.reader.MoveToContent();
                if (this.reader.MoveToFirstAttribute())
                {
                    do
                    {
                        if (this.reader.NamespaceURI.Equals(XmlConstants.XmlnsNamespace, StringComparison.Ordinal))
                        {
                            // xmlns namespace
                            continue;
                        }

                        string attributeName = this.reader.LocalName;
                        switch (attributeName)
                        {
                            case GmlConstants.AxisLabels:
                            case GmlConstants.UomLabels:
                            case GmlConstants.Count:
                            case GmlConstants.IdName:
                                break;
                            case GmlConstants.SrsName:
                                if (!expectSrsName)
                                {
                                    this.reader.MoveToElement();
                                    throw new FormatException(Strings.GmlReader_InvalidAttribute(attributeName, this.reader.Name));
                                }

                                // srsName="http://www.opengis.net/def/crs/EPSG/0/SRID"
                                string attributeValue = this.reader.Value;
                                if (attributeValue.StartsWith(GmlConstants.SrsPrefix, StringComparison.Ordinal))
                                {
                                    int id = XmlConvert.ToInt32(attributeValue.Substring(GmlConstants.SrsPrefix.Length));
                                    this.pipeline.SetCoordinateSystem(id);
                                    crsSet = true;
                                }
                                else
                                {
                                    throw new FormatException(Strings.GmlReader_InvalidSrsName(GmlConstants.SrsPrefix));
                                }

                                break;
                            default:
                                this.reader.MoveToElement();
                                throw new FormatException(Strings.GmlReader_InvalidAttribute(attributeName, this.reader.Name));
                        }
                    }
                    while (this.reader.MoveToNextAttribute());

                    this.reader.MoveToElement();
                }

                if (expectSrsName && !crsSet)
                {
                    this.pipeline.SetCoordinateSystem(null);
                }
            }

            /// <summary>
            /// creates a shape and parses the element.
            /// This is used to parse a top level Point element, as opposed to
            /// a point which is embedded in a linestring or a polygon.
            /// </summary>
            private void ParseGmlPointShape()
            {
                this.pipeline.BeginGeo(SpatialType.Point);
                this.PrepareFigure();
                this.ParseGmlPointElement(true /* allowEmpty */);
                this.EndFigure();
                this.pipeline.EndGeo();
            }

            /// <summary>
            ///  creates a shape and parses the element for top level LineString shapes
            /// </summary>
            private void ParseGmlLineStringShape()
            {
                this.pipeline.BeginGeo(SpatialType.LineString);
                this.PrepareFigure();
                this.ParseGmlLineString();
                this.EndFigure();
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the Polygon element.
            /// </summary>
            private void ParseGmlPolygonShape()
            {
                // GmlPolygonElement :=
                //     <Polygon>
                //         GmlExteriorLinearRingElement GmlInteriorLinearRingElement*
                //     </Polygon>
                //     | <Polygon/>
                this.pipeline.BeginGeo(SpatialType.Polygon);

                if (this.ReadStartOrEmptyElement(GmlConstants.Polygon))
                {
                    this.ReadSkippableElements();
                    if (!this.IsEndElement(GmlConstants.Polygon))
                    {
                        this.PrepareFigure();
                        this.ParseGmlRingElement(GmlConstants.ExteriorRing);
                        this.EndFigure();

                        this.ReadSkippableElements();
                        while (this.IsStartElement(GmlConstants.InteriorRing))
                        {
                            this.PrepareFigure();
                            this.ParseGmlRingElement(GmlConstants.InteriorRing);
                            this.EndFigure();
                            this.ReadSkippableElements();
                        }
                    }

                    this.ReadSkippableElements();
                    this.ReadEndElement();
                }

                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the MultiPoint element.
            /// </summary>
            private void ParseGmlMultiPointShape()
            {
                this.pipeline.BeginGeo(SpatialType.MultiPoint);
                this.ParseMultiItemElement(GmlConstants.MultiPoint, GmlConstants.PointMember, GmlConstants.PointMembers, this.ParseGmlPointShape);
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the MultiLineString(Gml MultiCurve) element.
            /// </summary>
            private void ParseGmlMultiCurveShape()
            {
                this.pipeline.BeginGeo(SpatialType.MultiLineString);
                this.ParseMultiItemElement(GmlConstants.MultiLineString, GmlConstants.LineStringMember, GmlConstants.LineStringMembers, this.ParseGmlLineStringShape);
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the MultiPolygon(Gml MultiSurface) element.
            /// </summary>
            private void ParseGmlMultiSurfaceShape()
            {
                this.pipeline.BeginGeo(SpatialType.MultiPolygon);
                this.ParseMultiItemElement(GmlConstants.MultiPolygon, GmlConstants.PolygonMember, GmlConstants.PolygonMembers, this.ParseGmlPolygonShape);
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the Collection(Gml MultiGeometry) element.
            /// </summary>
            private void ParseGmlMultiGeometryShape()
            {
                this.pipeline.BeginGeo(SpatialType.Collection);
                this.ParseMultiItemElement(GmlConstants.Collection, GmlConstants.CollectionMember, GmlConstants.CollectionMembers, () => this.ParseGmlGeometry(false));
                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Creates a shape and parses the FullGlobe element
            /// </summary>
            private void ParseGmlFullGlobeElement()
            {
                // FullGlobeElement :=
                //     <FullGlobe>
                //     </FullGlobe>
                this.pipeline.BeginGeo(SpatialType.FullGlobe);
                if (this.ReadStartOrEmptyElement(GmlConstants.FullGlobe))
                {
                    if (this.IsEndElement(GmlConstants.FullGlobe))
                    {
                        this.ReadEndElement();
                    }
                }

                this.pipeline.EndGeo();
            }

            /// <summary>
            /// Parses a simple point.
            /// </summary>
            /// <param name="allowEmpty">Allow Empty Point</param>
            private void ParseGmlPointElement(bool allowEmpty)
            {
                if (this.ReadStartOrEmptyElement(GmlConstants.Point))
                {
                    // <gml:Point>
                    //   <gml:name>foo</gml:name>
                    //   <gml:pos>1 2 3 4</gml:pos>
                    //   <gml:description>bar</gml:description>
                    // </gml:Point>
                    this.ReadSkippableElements();
                    this.ParseGmlPosElement(allowEmpty);
                    this.ReadSkippableElements();
                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// Parses the GmlLineStringElement.
            /// </summary>
            private void ParseGmlLineString()
            {
                // Linestrings are not allowed to contain empty points.
                //
                //        GmlLineStringElement :=
                //            <LineString>
                //                PosList
                //                | GmlPosListElement
                //            </LineString>
                if (this.ReadStartOrEmptyElement(GmlConstants.LineString))
                {
                    this.ReadSkippableElements();
                    if (this.IsPosListStart())
                    {
                        this.ParsePosList(false /* allowEmpty */);
                    }
                    else
                    {
                        this.ParseGmlPosListElement(true /* allowEmpty */);
                    }

                    this.ReadSkippableElements();
                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// Parses the GmlExteriorLinearRingElement
            /// </summary>
            /// <param name="ringTag">The type or ring</param>
            private void ParseGmlRingElement(string ringTag)
            {
                // GmlExteriorLinearRingElement :=
                //     <exterior>
                //         GmlLinearRingElement
                //     </exterior>
                //     | <exterior/>
                if (this.ReadStartOrEmptyElement(ringTag))
                {
                    if (!this.IsEndElement(ringTag))
                    {
                        this.ParseGmlLinearRingElement();
                    }

                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// ParseGmlLinearRingElement parses the GmlLinearRingElement
            /// </summary>
            private void ParseGmlLinearRingElement()
            {
                // GmlLinearRingElement :=
                //     <LinearRing>
                //         PosList
                //         | GmlPosListElement
                //     </LinearRing>
                //     | <LinearRing/>
                if (this.ReadStartOrEmptyElement(GmlConstants.LinearRing))
                {
                    if (this.IsEndElement(GmlConstants.LinearRing))
                    {
                        // Empty rings are not allowed
                        throw new FormatException(Strings.GmlReader_EmptyRingsNotAllowed);
                    }
                    else
                    {
                        if (this.IsPosListStart())
                        {
                            this.ParsePosList(false /* allowEmpty */);
                        }
                        else
                        {
                            this.ParseGmlPosListElement(false /* allowEmpty */);
                        }
                    }

                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// Common function for all item collections, since they are all parsed exactly the same way
            /// </summary>
            /// <param name="header">The wrapping header tag</param>
            /// <param name="member">The member tag</param>
            /// <param name="members">The members tag</param>
            /// <param name="parseItem">Parser for individual items</param>
            private void ParseMultiItemElement(string header, string member, string members, Action parseItem)
            {
                // Example: MultiPoint
                //        GmlMultiPointElement :=
                //            <MultiPoint>
                //                GmlPointMemberElement* GmlPointMembersElement?
                //            </MultiPoint>
                //            | <MultiPoint/>
                //
                //        GmlPointMemberElement :=
                //            <pointMember>
                //                GmlPointElement?
                //            </pointMember>
                //            | </pointMember>
                //
                //        GmlPointMembersElement :=
                //            <pointMembers>
                //                GmlPointElement*
                //            </pointMembers>
                //            | <pointMembers/>
                if (this.ReadStartOrEmptyElement(header))
                {
                    this.ReadSkippableElements();
                    if (!this.IsEndElement(header))
                    {
                        while (this.IsStartElement(member))
                        {
                            if (this.ReadStartOrEmptyElement(member))
                            {
                                if (!this.IsEndElement(member))
                                {
                                    parseItem();
                                    this.ReadEndElement(); // end of member
                                    this.ReadSkippableElements();
                                }
                            }
                        }

                        if (this.IsStartElement(members))
                        {
                            if (this.ReadStartOrEmptyElement(members))
                            {
                                while (reader.IsStartElement())
                                {
                                    parseItem();
                                }

                                this.ReadEndElement();
                            }
                        }
                    }

                    this.ReadSkippableElements();
                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// parses a pos element, which eventually is used in most other top level elements.
            /// This represents a single point location with either two or zero coordinates.
            /// </summary>
            /// <param name="allowEmpty">Allow empty pos</param>
            private void ParseGmlPosElement(bool allowEmpty)
            {
                // GmlPosListElement :=
                //     <pos>
                //         {Double}*
                //     </pos>
                //     | <pos/>
                this.ReadAttributes(false);
                if (this.ReadStartOrEmptyElement(GmlConstants.Position))
                {
                    double[] doubleList = this.ReadContentAsDoubleArray();

                    if (doubleList.Length != 0)
                    {
                        if (doubleList.Length < 2)
                        {
                            // When parsing a pos, we need at least two coordinates.
                            throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
                        }

                        this.AddPoint(doubleList[0], doubleList[1], doubleList.Length > 2 ? doubleList[2] : default(double?), doubleList.Length > 3 ? doubleList[3] : default(double?));
                    }
                    else if (!allowEmpty)
                    {
                        throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
                    }

                    this.ReadEndElement();
                }
                else if (!allowEmpty)
                {
                    // Read an empty "pos", and allowEmpty is false.
                    throw new FormatException(Strings.GmlReader_PosNeedTwoNumbers);
                }
            }

            /// <summary>
            /// Parses a sequence of 1 or more pos and pointProperty elements
            /// </summary>
            /// <param name="allowEmpty">Allow Empty Point</param>
            private void ParsePosList(bool allowEmpty)
            {
                // PosList := {GmlPosElement | GmlPointPropertyElement}*
                do
                {
                    if (this.IsStartElement(GmlConstants.Position))
                    {
                        this.ParseGmlPosElement(allowEmpty);
                    }
                    else
                    {
                        this.ParseGmlPointPropertyElement(allowEmpty);
                    }
                }
                while (this.IsPosListStart());
            }

            /// <summary>
            /// Parses a simple pointProperty.
            /// </summary>
            /// <param name="allowEmpty">Allow empty point</param>
            private void ParseGmlPointPropertyElement(bool allowEmpty)
            {
                // GmlPointPropertyElement :=
                //     <pointProperty>
                //         GmlPointElement
                //     </pointProperty>
                if (this.ReadStartOrEmptyElement(GmlConstants.PointProperty))
                {
                    this.ParseGmlPointElement(allowEmpty);
                    this.ReadEndElement();
                }
            }

            /// <summary>
            /// parses a GmlPosListElement.
            /// </summary>
            /// <param name="allowEmpty">Allow empty posList</param>
            private void ParseGmlPosListElement(bool allowEmpty)
            {
                int dimension = ReadSrsDimension();
                Debug.Assert(dimension == 2 || dimension == 3, "dimension is not 2D or 3D");

                // GmlPosListElement :=
                //     <posList>
                //         {Double}*
                //     </posList>
                //     | <posList/>
                if (this.ReadStartOrEmptyElement(GmlConstants.PositionList))
                {
                    if (!this.IsEndElement(GmlConstants.PositionList))
                    {
                        double[] doubleList = this.ReadContentAsDoubleArray();
                        if (doubleList.Length != 0)
                        {
                            if (doubleList.Length % dimension != 0)
                            {
                                // Odd number of doubles
                                throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
                            }

                            // posList supports only 2D points
                            for (int i = 0; i < doubleList.Length; i += dimension)
                            {
                                if (dimension == 2)
                                {
                                    this.AddPoint(doubleList[i], doubleList[i + 1], null, null);
                                }
                                else
                                {
                                    this.AddPoint(doubleList[i], doubleList[i + 1], doubleList[i + 2], null);
                                }
                            }
                        }
                        else
                        {
                            // Read a ParseGmlPosListElement with 0 elements.
                            throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
                        }
                    }
                    else if (!allowEmpty)
                    {
                        // Read  a posList with no contents.
                        throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
                    }

                    this.ReadEndElement();
                }
                else if (!allowEmpty)
                {
                    // Read an empty posList.
                    throw new FormatException(Strings.GmlReader_PosListNeedsEvenCount);
                }
            }

            /// <summary>
            /// Read SrsDimension integer value from SrsDimension attribute if presented.
            /// </summary>
            /// <returns>The SrsDimension value.</returns>
            private int ReadSrsDimension()
            {
                int dimension = 2; // by default
                string srsDimension = this.reader.GetAttribute(GmlConstants.SrsDimension);
                if (srsDimension != null)
                {
                    dimension = XmlConvert.ToInt32(srsDimension);
                }

                if (dimension != 2 && dimension != 3)
                {
                    throw new FormatException(Strings.GmlReader_InvalidSrsDimension);
                }

                return dimension;
            }

            /// <summary>
            /// Reads the current content in the xml element as a double array
            /// </summary>
            /// <remarks>
            /// XmlReader.ReadContentAs(typeof(double[])) basically does this but a lot slower, since it will handle a bunch of
            /// different splitters and formats. Here we simply parse it as a string and split in on one separator
            /// </remarks>
            /// <returns>The double array</returns>
            private double[] ReadContentAsDoubleArray()
            {
                String[] splitted = this.reader.ReadContentAsString().Split(coordinateDelimiter, StringSplitOptions.RemoveEmptyEntries);
                double[] array = new double[splitted.Length];
                for (int i = 0; i < splitted.Length; ++i)
                {
                    array[i] = XmlConvert.ToDouble(splitted[i]);
                }

                return array;
            }

            /// <summary>
            /// Main element reading function.
            /// Returns true if it read a non-empty start element of the given name.
            /// possibilities:
            ///     1- current element is not a start element named "element" - throw
            ///     2- current element is named "element" but is an empty element - return false
            ///     3- current element is named "element" and is not empty - return true
            /// If the function returns true, it means that a non-empty element of the given name
            /// was read, so the caller takes responsibility to read the corresponding end element.
            /// </summary>
            /// <param name="element">The element name</param>
            /// <returns>Returns true if it read a non-empty start element of the given name.</returns>
            private bool ReadStartOrEmptyElement(string element)
            {
                bool isEmptyElement = reader.IsEmptyElement;

                if (element != GmlConstants.FullGlobe)
                {
                    reader.ReadStartElement(element, this.gmlNamespace);
                }
                else
                {
                    reader.ReadStartElement(element, GmlConstants.FullGlobeNamespace);
                }

                return !isEmptyElement;
            }

            /// <summary>
            /// Is Start Element
            /// </summary>
            /// <param name="element">Expected Element Tag</param>
            /// <returns>True if reader is at the expected element</returns>
            private bool IsStartElement(string element)
            {
                return reader.IsStartElement(element, this.gmlNamespace);
            }

            /// <summary>
            /// Is End Element
            /// </summary>
            /// <param name="element">Expected Element Tag</param>
            /// <returns>True if reader is at the end of the expected element</returns>
            private bool IsEndElement(string element)
            {
                this.reader.MoveToContent();
                return reader.NodeType == XmlNodeType.EndElement && this.reader.LocalName.Equals(element, StringComparison.Ordinal);
            }

            /// <summary>
            /// Read End Element
            /// </summary>
            private void ReadEndElement()
            {
                this.reader.MoveToContent();
                if (this.reader.NodeType != XmlNodeType.EndElement)
                {
                    throw new FormatException(Strings.GmlReader_UnexpectedElement(this.reader.Name));
                }

                this.reader.ReadEndElement();
            }

            /// <summary>
            /// Call MoveToContent, then skip a known set of irrelevant elements (gml:name, gml:description)
            /// </summary>
            private void ReadSkippableElements()
            {
                bool shouldSkip = true;
                while (shouldSkip)
                {
                    this.reader.MoveToContent();
                    if (this.reader.NodeType == XmlNodeType.Element &&
                        Object.ReferenceEquals(this.reader.NamespaceURI, this.gmlNamespace))
                    {
                        // calling LocalName_get() multiple times should be avoided.
                        String localName = this.reader.LocalName;
                        shouldSkip = skippableElements.ContainsKey(localName);
                    }
                    else
                    {
                        shouldSkip = false;
                    }

                    if (shouldSkip)
                    {
                        this.reader.Skip();
                    }
                }
            }

            /// <summary>
            /// Is reader at the start of a pos or pointProperty
            /// </summary>
            /// <returns>True if reader is at the expected element</returns>
            private bool IsPosListStart()
            {
                return IsStartElement(GmlConstants.Position) || IsStartElement(GmlConstants.PointProperty);
            }

            /// <summary>
            /// Prepare for figure drawing
            /// </summary>
            private void PrepareFigure()
            {
                points = 0;
            }

            /// <summary>
            /// Draw a point in the current figure
            /// </summary>
            /// <param name="x">X coordinate</param>
            /// <param name="y">Y coordinate</param>
            /// <param name="z">Z coordinate</param>
            /// <param name="m">M coordinate</param>
            private void AddPoint(double x, double y, double? z, double? m)
            {
                if (z.HasValue && double.IsNaN(z.Value))
                {
                    z = null;
                }

                if (m.HasValue && double.IsNaN(m.Value))
                {
                    m = null;
                }

                if (points == 0)
                {
                    pipeline.BeginFigure(x, y, z, m);
                }
                else
                {
                    pipeline.LineTo(x, y, z, m);
                }

                points += 1;
            }

            /// <summary>
            /// End Current Figure
            /// </summary>
            private void EndFigure()
            {
                if (points > 0)
                {
                    pipeline.EndFigure();
                }
            }
        }
    }
}
