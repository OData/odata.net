//---------------------------------------------------------------------
// <copyright file="BlobsPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Holds and manages request or response MLE payloads.
    //---------------------------------------------------------------------
    public abstract class BlobsPayload
    {
        // Whole payload conversions.
        public abstract override string ToString();
        public abstract string AdjustedForVerify();

        // Individual MLE property access.
        public abstract string GetProperty(string name);
        public abstract void SetProperty(string name, string value);

        //---------------------------------------------------------------------
        public static BlobsPayload Parse(SerializationFormatKind format, AstoriaResponse response)
        {
            switch (format)
            {
                case SerializationFormatKind.Atom: return new BlobsPayload.Atom(response.Payload);
                case SerializationFormatKind.JSON: return new BlobsPayload.JSON(response.Payload);
                default:
                    AstoriaTestLog.FailAndThrow("Unknown serialization format kind: " + format.ToString());
                    return null;
            }
        }

        //---------------------------------------------------------------------
        // JSON payload parser.
        //---------------------------------------------------------------------
        public class JSON : BlobsPayload
        {
            private const string jsonPropertyRegex = @"(,?""name"":"")(.*?)""([,}])";
            private string payload;

            //---------------------------------------------------------------------
            public JSON(string payload)
            {
                // Remove outermost "d".
                var prefix = "{\"d\":";
                int metadataPosition = payload.IndexOf(prefix);
                if (metadataPosition != -1)
                {
                    payload = payload.Substring(metadataPosition + prefix.Length);  // {"d":
                    payload = payload.Remove(payload.LastIndexOf("}")); // }
                }

                // Remove "etag".
                string etagRegex = jsonPropertyRegex.Replace("name", "etag");
                Match m = Regex.Match(payload, etagRegex);
                if (m.Success)
                    payload = Regex.Replace(payload, etagRegex, m.Groups[3].Value);

                // Save and remove "media_etag".
                string mediaEtagRegex = jsonPropertyRegex.Replace("name", "media_etag");
                m = Regex.Match(payload, mediaEtagRegex);
                if (m.Success)
                {
                    BlobsRequest.ETagMRR = m.Groups[2].Value.Replace("\\'", "'").Replace("\\\"", "\"");
                    payload = Regex.Replace(payload, mediaEtagRegex, m.Groups[3].Value);
                }

                this.payload = payload;
            }

            //---------------------------------------------------------------------
            // Locates MLE property and returns its value.
            //---------------------------------------------------------------------
            public override string GetProperty(string name)
            {
                Match m = Regex.Match(payload, jsonPropertyRegex.Replace("name", name));
                return m.Groups[2].Value;
            }

            //---------------------------------------------------------------------
            // Modifies MLE by assigning value to property.
            //---------------------------------------------------------------------
            public override void SetProperty(string name, string newValue)
            {
                Match m = Regex.Match(payload, jsonPropertyRegex.Replace("name", name));
                string prefix = m.Groups[1].Value;
                string suffix = m.Groups[3].Value;
                payload = Regex.Replace(payload, jsonPropertyRegex.Replace("name", name), prefix + newValue + "\"" + suffix);
            }

            //---------------------------------------------------------------------
            public override string ToString() { return payload; }

            //---------------------------------------------------------------------
            public override string AdjustedForVerify() { return payload; }
        }
    }
}

