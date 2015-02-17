//---------------------------------------------------------------------
// <copyright file="HtmlPayloadErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Deserializer for Html error payloads
    /// </summary>
    public class HtmlPayloadErrorDeserializer : IPayloadErrorDeserializer
    {
        /// <summary>
        /// Deserializes the given html payload a error payload or returns null
        /// </summary>
        /// <param name="serialized">The payload that was sent over HTTP</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        public bool TryDeserializeErrorPayload(byte[] serialized, string encodingName, out ODataPayloadElement errorPayload)
        {
            Encoding encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            string payload = encoding.GetString(serialized, 0, serialized.Length);
            errorPayload = null;

            // <html>
            //  <body>
            //      <div id="content">
            //          <p class="heading1">Request Error</p>
            //           <p>Error message goes here</p>
            //           <p>Stack trace goes here</p>
            //      </div>
            //  <body>
            // </html>
            // The server encountered an error processing the request. The exception message is '
            // trim the html to only contain the <body> </body> section of the HTML.
            if (payload.Contains("<html"))
            {
                errorPayload = new HtmlErrorPayload();
                var htmlPayload = errorPayload as HtmlErrorPayload;

                payload = payload.Substring(payload.IndexOf("<body>", StringComparison.OrdinalIgnoreCase)).Replace("</html>", string.Empty);
                payload = payload.Replace("The server encountered an error processing the request. The exception message is '", string.Empty);
                payload = payload.Replace("'. See server logs for more details. The exception stack trace is: ", string.Empty);
                XDocument errorDocument = XDocument.Parse(payload);
                XElement body = errorDocument.Element(XName.Get("body"));
                XElement errorContent = body.Element(XName.Get("div"));
                XElement errorMessage = errorContent.Elements(XName.Get("p")).Skip(1).First();
                XElement stackTrace = errorContent.Elements(XName.Get("p")).Skip(2).First();

                htmlPayload.Message = errorMessage.Value;
                htmlPayload.StackTrace = stackTrace.Value;

                return true;
            }

            return false;
        }
    }
}
