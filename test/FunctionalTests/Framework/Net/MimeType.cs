//---------------------------------------------------------------------
// <copyright file="MimeType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public enum SerializationFormatKind
    {
        Default=0,
        JSON,
        Atom,
        PlainXml,
        PlainText,
    }

    public class SerializationFormatKinds
    {
        private static readonly ReadOnlyCollection<SerializationFormatKind> SerializationTypes =
            new ReadOnlyCollection<SerializationFormatKind>(
                new SerializationFormatKind[] { SerializationFormatKind.JSON, SerializationFormatKind.Atom });

        public static ReadOnlyCollection<SerializationFormatKind> GetSerializationTypes()
        {
            return SerializationTypes;
        }

        /// <summary>Gets a valid MIME type for the specified <paramref name="kind"/>.</summary>
        /// <param name="kind"><see cref="SerializationFormatKind"/> to get MIME type for.</param>
        /// <returns>A valid MIME type for the specified <paramref name="kind"/>.</returns>
        public static string ContentTypeFromKind(SerializationFormatKind kind)
        {
            switch (kind)
            {
                case SerializationFormatKind.JSON: 
                    return JsonMimeType;
                case SerializationFormatKind.Atom: 
                    return AtomMimeType;
                case SerializationFormatKind.PlainXml:
                    return XmlMimeType;
                case SerializationFormatKind.PlainText:
                    return MimeTextPlain;
                default:
                    Debug.Assert(kind == SerializationFormatKind.Default);
                    return AtomMimeType;
            }
        }

        public static readonly string Web3SMimeType = "application/Web3S+xml";
        public static readonly string JsonMimeType = "application/json";
        public static readonly string AtomMimeType = "application/atom+xml";
        public static readonly string XmlMimeType = "text/xml";
        public static readonly string MimeAny = "*/*";
        public static readonly string MimeApplicationOctetStream = "application/octet-stream";
        public static readonly string MimeApplicationXml = "application/xml";
        public static readonly string MimeTextPlain = "text/plain";
        public static readonly string MimeMultipartMixed = "multipart/mixed";
    }
}
