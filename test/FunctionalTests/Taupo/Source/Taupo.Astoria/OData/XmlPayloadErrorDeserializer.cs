//---------------------------------------------------------------------
// <copyright file="XmlPayloadErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Deserializer for OData payloads for the atom/xml wire format
    /// </summary>
    public class XmlPayloadErrorDeserializer : IPayloadErrorDeserializer
    {
        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Deserializes given HTTP payload as an xml error payload or returns false
        /// </summary>
        /// <param name="serialized">The payload that was sent over HTTP</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream is disposed when the reader is disposed")]
        public virtual bool TryDeserializeErrorPayload(byte[] serialized, string encodingName, out ODataPayloadElement errorPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // get the encoding based on the given encoding name
            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);

            using (StreamReader streamReader = new StreamReader(new MemoryStream(serialized), encoding))
            {
                using (XmlReader reader = XmlReader.Create(streamReader))
                {
                    bool foundErrorPayload = false;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement(ODataConstants.ErrorElementName, ODataConstants.DataServicesMetadataNamespaceName))
                        {
                            foundErrorPayload = true;
                            break;
                        }
                    }

                    if (!foundErrorPayload)
                    {
                        errorPayload = null;
                        return false;
                    }

                    errorPayload = this.ParseErrorFragment(reader);
                    return true;
                }
            }
        }

        /// <summary>
        /// Parse the error tag from an Odata response.  It is assumed that the error portion is well formed xml.
        /// </summary>
        /// <param name="reader">Reader where the start error tag has already been parsed.</param>
        /// <returns>The resulting error payload.</returns>
        public ODataErrorPayload ParseErrorFragment(XmlReader reader)
        {
            // example xml error fragment
            // <m:error>
            //   <m:code>…</>
            //   <m:message xml:lang=”…”>…</>
            //   <m:innererror>
            //     <m:message>…</>
            //     <m:type>…</>
            //     <m:stacktrace>…</>
            //     <m:internalexception>
            //       … another inner error structure …
            //     </>
            //   </m:innererror>
            // </m:error>
            ODataErrorPayload topLevelError = new ODataErrorPayload();
            ODataInternalExceptionPayload innerError = null;

            int errorStartDepth = reader.Depth;
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.NamespaceURI == ODataConstants.DataServicesMetadataNamespaceName)
                {
                    if (reader.LocalName == ODataConstants.InnerErrorElementName)
                    {
                        this.Assert.IsNull(innerError, "Should not find inner error element on internal exceptions");
                        topLevelError.InnerError = innerError = new ODataInternalExceptionPayload();
                    }
                    else if (reader.LocalName == ODataConstants.InternalExceptionElementName)
                    {
                        this.Assert.IsNotNull(innerError, "Should not find internal exception element on top-level errors");
                        ExceptionUtilities.Assert(innerError.InternalException == null, "Expected internal error to be null");
                        innerError.InternalException = new ODataInternalExceptionPayload();
                        innerError = innerError.InternalException;
                    }
                    else if (reader.LocalName == ODataConstants.MessageElementName)
                    {
                        if (reader.HasAttributes)
                        {
                            this.Assert.IsNull(innerError, "Should not find attributes on inner errors");
                        }

                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();
                            if (innerError == null)
                            {
                                topLevelError.Message = reader.Value;
                            }
                            else
                            {
                                innerError.Message = reader.Value;
                            }
                        }
                    }
                    else if (reader.LocalName == ODataConstants.CodeElementName)
                    {
                        this.Assert.IsNull(innerError, "Should not find code element on inner errors");
                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();
                            topLevelError.Code = reader.Value;
                        }
                    }
                    else if (reader.LocalName == ODataConstants.TypeNameElementName)
                    {
                        this.Assert.IsNotNull(innerError, "Should not find type name element on top-level errors");
                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();
                            innerError.TypeName = reader.Value;
                        }
                    }
                    else if (reader.LocalName == ODataConstants.StackTraceElementName)
                    {
                        this.Assert.IsNotNull(innerError, "Should not find stack trace element on top-level errors");
                        if (!reader.IsEmptyElement)
                        {
                            reader.Read();
                            innerError.StackTrace = reader.Value;
                        }
                    }
                }

                // don't read past the end of <error> since there may be un-closed tags
                if (reader.Depth == errorStartDepth)
                {
                    break;
                }
            }

            return topLevelError;
        }
    }
}
