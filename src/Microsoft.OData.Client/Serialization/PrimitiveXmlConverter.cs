//---------------------------------------------------------------------
// <copyright file="PrimitiveXmlConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

/*  DESIGN NOTES (pqian):
 *  a note on the primitive type parser/materializer on the client side
 *  Since the client type system allows multiple CLR types mapping to
 *  a single EDM type (i.e., String, Char, Char[], XDocument and XElement
 *  all maps to Edm.String). We cannot handle materialization based on the
 *  wire Edm type. The correct behavior would be to "tokenize" the wire data
 *  using the wire type, and them "materialize" the token using the CLR type
 *  declared on the entity type. However, there is a V1/V2 behavior where
 *  we DO NOT fail if the wire data type doesn't match the wire data (for example,
 *  you can have <d:Property type="Edm.Int32">3.2</Property>, as long as the CLR type
 *  can handle decimal numbers). Therefore, for all existing V1/V2 primitive types,
 *  the "tokenize" step should simply be storing the textual representation
 *  in an instance of TextPrimitiveParserToken, and use Parse() to materialize it.
 *  For new types, the TokenizeFromXml/TokenizeFromText on the PrimitiveTypeConverter
 *  should be overriden to give the correct behavior.
 */
namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal class PrimitiveTypeConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected PrimitiveTypeConverter()
        {
        }

        /// <summary>
        /// Create a parser token from xml feed
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <remarks>The reader is expected to be placed at the beginning of the element, and after this method call, the reader will be placed
        /// at the EndElement, such that the next Element will be read in the next Read call.</remarks>
        /// <returns>token</returns>
        internal virtual PrimitiveParserToken TokenizeFromXml(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "Reader at element");
            string elementString = MaterializeAtom.ReadElementString(reader, true);
            if (elementString != null)
            {
                return new TextPrimitiveParserToken(elementString);
            }

            return null;
        }

        /// <summary>
        /// Create a parser token from text representation ($value end points)
        /// </summary>
        /// <param name="text">The text form representation</param>
        /// <returns>token</returns>
        internal virtual PrimitiveParserToken TokenizeFromText(String text)
        {
            return new TextPrimitiveParserToken(text);
        }

        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal virtual object Parse(String text)
        {
            return text;
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal virtual String ToString(object instance)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class BooleanTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToBoolean(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Boolean)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class ByteTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToByte(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Byte)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class ByteArrayTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return Convert.FromBase64String(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return Convert.ToBase64String((byte[])instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class BinaryTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>MethodInfo for the ToArray method on the Binary type.</summary>
        private MethodInfo convertToByteArrayMethodInfo;

        /// <summary>
        /// The delay loaded binary type
        /// </summary>
        internal static Type BinaryType
        {
            get;
            set;
        }

        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return Activator.CreateInstance(BinaryType, Convert.FromBase64String(text));
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return instance.ToString();
        }

        /// <summary>
        /// Converts the System.Data.Linq.Binary to byte[] by calling the ToArray method on the Binary type.
        /// </summary>
        /// <param name="instance">Instance of Binary type.</param>
        /// <returns>Byte[] instance containing the value of the Binary type.</returns>
        internal byte[] ToArray(object instance)
        {
            if (this.convertToByteArrayMethodInfo == null)
            {
                this.convertToByteArrayMethodInfo = instance.GetType().GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance);
            }

            return (byte[])this.convertToByteArrayMethodInfo.Invoke(instance, null);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class DecimalTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToDecimal(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Decimal)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class DoubleTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToDouble(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Double)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class GuidTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return new Guid(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class Int16TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToInt16(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Int16)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class Int32TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToInt32(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Int32)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class Int64TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToInt64(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Int64)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class SingleTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToSingle(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Single)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class StringTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return text;
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return (String)instance;
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class SByteTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToSByte(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((SByte)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class CharTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToChar(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((Char)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class CharArrayTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return text.ToCharArray();
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return new String((char[])instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class ClrTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return PlatformHelper.GetTypeOrThrow(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return ((Type)instance).AssemblyQualifiedName;
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class UriTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return UriUtil.CreateUri(text, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return UriUtil.UriToString((Uri)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class XDocumentTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return text.Length > 0 ? System.Xml.Linq.XDocument.Parse(text) : new System.Xml.Linq.XDocument();
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class XElementTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return System.Xml.Linq.XElement.Parse(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return instance.ToString();
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class DateTimeOffsetTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return PlatformHelper.ConvertStringToDateTimeOffset(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((DateTimeOffset)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class DateTimeTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return PlatformHelper.ConvertStringToDateTime(text);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class TimeSpanTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return EdmValueParser.ParseDuration(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return EdmValueWriter.DurationAsXml((TimeSpan)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class UInt16TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToUInt16(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((UInt16)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class UInt32TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToUInt32(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((UInt32)instance);
        }
    }

    /// <summary>
    /// Convert between primitive types to string and xml representation
    /// </summary>
    internal sealed class UInt64TypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            return XmlConvert.ToUInt64(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return XmlConvert.ToString((UInt64)instance);
        }
    }

    /// <summary>
    /// Convert between an instance of geography and its Xml representation
    /// </summary>
    internal sealed class GeographyTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from the xml reader
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <remarks>The reader is expected to be placed at the beginning of the element, and after this method call, the reader will be placed
        /// at the EndElement, such that the next Element will be read in the next Read call.</remarks>
        /// <returns>An instance of primitive type</returns>
        internal override PrimitiveParserToken TokenizeFromXml(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "reader at element");
            reader.ReadStartElement(); // <d:Property>
            var g = new InstancePrimitiveParserToken<Geography>(GmlFormatter.Create().Read<Geography>(reader));
            Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "reader at end of current element");

            return g;
        }
    }

    /// <summary>
    /// Convert between an instance of geometry and its Xml representation
    /// </summary>
    internal sealed class GeometryTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from the xml reader
        /// </summary>
        /// <param name="reader">The xml reader</param>
        /// <remarks>The reader is expected to be placed at the beginning of the element, and after this method call, the reader will be placed
        /// at the EndElement, such that the next Element will be read in the next Read call.</remarks>
        /// <returns>An instance of primitive type</returns>
        internal override PrimitiveParserToken TokenizeFromXml(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "reader at element");
            reader.ReadStartElement(); // <d:Property>
            var g = new InstancePrimitiveParserToken<Geometry>(GmlFormatter.Create().Read<Geometry>(reader));
            Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "reader at end of current element");

            return g;
        }
    }

    /// <summary>
    /// Convert between an instance of DataServiceStreamLink and its xml representation.
    /// There is never a scenario in client which requires to do this, but since a converter
    /// is required, adding one which does nothing for namedstream.
    /// </summary>
    internal sealed class NamedStreamTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(String text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override String ToString(object instance)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convert between primitive types Edm.Date and string
    /// </summary>
    internal sealed class DateTypeConverter : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(string text)
        {
            return PlatformHelper.ConvertStringToDate(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return ((Date)instance).ToString();
        }
    }

    /// <summary>
    /// Convert between primitive types Edm.TimeOfDay and string
    /// </summary>
    internal sealed class TimeOfDayConvert : PrimitiveTypeConverter
    {
        /// <summary>
        /// Create an instance of primitive type from a string representation
        /// </summary>
        /// <param name="text">The string representation</param>
        /// <returns>An instance of primitive type</returns>
        internal override object Parse(string text)
        {
            return PlatformHelper.ConvertStringToTimeOfDay(text);
        }

        /// <summary>
        /// Convert an instance of primitive type to string
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The string representation of the instance</returns>
        internal override string ToString(object instance)
        {
            return ((TimeOfDay)instance).ToString();
        }
    }
}
