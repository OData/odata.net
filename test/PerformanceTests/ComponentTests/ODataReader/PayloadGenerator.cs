//---------------------------------------------------------------------
// <copyright file="PayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class PayloadGenerator
    {
        private static string prologueOfPayload = "{\"@odata.context\":\"http://odata.org/Perf.svc/$metadata#Product\",\"value\":[";

        /// <summary>
        /// Read part of payload (an entry) from template file
        /// </summary>
        /// <param name="payloadTemplate">The payload template file name</param>
        /// <returns>The file content as string</returns>
        private static string ReadTemplate(string payloadTemplate)
        {
            var bytes = TestUtils.ReadTestResource(payloadTemplate);
            return System.Text.Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// Generate a full payload for test
        /// </summary>
        /// <param name="payloadTemplate">The payload template file name</param>
        /// <param name="entryCount">Number of entries in the feed</param>
        /// <returns></returns>
        public static byte[] GenerateFeed(string payloadTemplate, int entryCount)
        {
            var sb = new StringBuilder(prologueOfPayload);
            string entry =  ReadTemplate(payloadTemplate);

            for (int i = 0; i < entryCount - 1; i++)
            {
                sb.Append(entry);
                sb.Append(",");
            }

            sb.Append(entry);

            // Epilog of the payload
            sb.Append("]}");

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
