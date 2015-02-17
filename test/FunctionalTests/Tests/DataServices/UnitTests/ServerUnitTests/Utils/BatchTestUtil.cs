//---------------------------------------------------------------------
// <copyright file="BatchTestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    #endregion Namespaces

    public static class BatchTestUtil
    {
        public static string CreateBatchText(out string requestContentType, params string[] changesets)
        {
            return CreateBatchText(out requestContentType, (IEnumerable<string>)changesets);
        }

        public static string CreateBatchText(out string requestContentType, IEnumerable<string> changesets)
        {
            requestContentType = "multipart/mixed;boundary=boundary1";
            StringBuilder result = new StringBuilder();
            string delim = "";
            foreach (string changeset in changesets)
            {
                result.Append(delim);
                delim = "\r\n";
                result.Append("--boundary1\r\n");
                result.Append(changeset);
            }
            result.Append("\r\n--boundary1--");
            return result.ToString();
        }

        public static string CreateChangeSetText(params string[] operations)
        {
            return CreateChangeSetText((IEnumerable<string>)operations);
        }

        public static string CreateChangeSetText(IEnumerable<string> operations)
        {
            StringBuilder result = new StringBuilder();
            result.Append("Content-Type: multipart/mixed; boundary=cs\r\n");
            result.Append("\r\n");
            string delim = "";
            foreach (string operation in operations)
            {
                result.Append(delim);
                delim = "\r\n";
                result.Append("--cs\r\n");
                result.Append(operation);
            }
            result.Append("\r\n--cs--");
            return result.ToString();
        }

        public static string CreateOperationRequestText(string requestMethod, string requestUri, string headers, string body)
        {
            return
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "\r\n" +
                requestMethod + " " + requestUri + " HTTP/1.1\r\n" +
                headers +
                "Content-Length: " + body.Length + "\r\n" +
                "\r\n" +
                body;
        }

        private static Regex StackTraceInfo = new Regex("^[ ]*at ");

        // 23587319-1f44-49ee-96f3-5acb2ac81ee7
        private static Regex GuidRegEx = new Regex(
            "[0-9a-fA-F]{8}-" +
            "[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-" +
            "[0-9a-fA-F]{12}");

        internal static string GenerateDummyGuid(int id)
        {
            return id.ToString("D8", System.Globalization.CultureInfo.InvariantCulture) +
                "-0000-0000-0000-" +
                "000000000000";
        }

        public static string PrepareResponseForFileCompare(TextReader reader, string requestBaseUri, string targetBaseUri)
        {
            int dummyGuidId = 0;
            Dictionary<string, string> dummyGuids = new Dictionary<string, string>();
            MemoryStream buffer = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(buffer))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // remove the file name information and line information
                    Match match = StackTraceInfo.Match(line);
                    if (match.Success)
                    {
                        // Ignore the whole line
                    }
                    else
                    {
                        int stackTraceStartIndex = line.IndexOf("<m:stacktrace>");
                        if (stackTraceStartIndex >= 0)
                        {
                            line = line.Substring(0, stackTraceStartIndex);
                        }

                        Match guidMatch = GuidRegEx.Match(line);
                        if (guidMatch.Success)
                        {
                            string replacementGuid;
                            if (!dummyGuids.TryGetValue(guidMatch.Captures[0].Value, out replacementGuid))
                            {
                                replacementGuid = GenerateDummyGuid(dummyGuidId++);
                                dummyGuids.Add(guidMatch.Captures[0].Value, replacementGuid);
                            }

                            line = line.Replace(guidMatch.Captures[0].Value, replacementGuid);
                        }

                        /* depending on the service location the base uri may or may not contain trailing '/' character
                        (e.g.  for WebServerLocation.InProcess base uri will be http://host/ while for WebServerLocation.InProcessStreamedWcf
                        it will be like http://localhost:{port}/TheTest). The regular expression below treats the trailing '/' as optional */
                        line = Regex.Replace(line, requestBaseUri + "/?", targetBaseUri);

                        writer.WriteLine(line);
                    }
                }

                writer.Flush();
                buffer.Position = 0;
                return new StreamReader(buffer).ReadToEnd();
            }
        }

        public static string GetResponse(string payload, Type contextType, WebServerLocation location)
        {
            return GetResponse(payload, contextType, location, string.Empty);
        }

        public static string GetResponse(string payload, Type contextType, WebServerLocation location, string requestVersion)
        {
            string[] segments = payload.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string boundary = segments[0].Substring(2);
            using (TestWebRequest request = TestWebRequest.CreateForLocation(location))
            {
                request.RequestVersion = requestVersion;
                request.DataServiceType = contextType;
              
                request.RequestUriString = "/$batch";
                request.Accept = UnitTestsUtil.MimeMultipartMixed;
                request.HttpMethod = "POST";
                request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                if (request.BaseUri != null)
                {
                    payload = Regex.Replace(payload, "\\$\\(BaseUri\\)", request.BaseUri.EndsWith("/") ? request.BaseUri : request.BaseUri + "/");
                }
                
                request.RequestStream = IOUtil.CreateStream(payload);
                request.SendRequest();
                Stream responseStream = request.GetResponseStream();
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return PrepareResponseForFileCompare(reader, request.BaseUri, "$(BaseUri)");
                }
            }
        }

        public static void CompareBatchResponse(string expectedResponseFileName, string actualResponse)
        {
            string expectedResponse = File.ReadAllText(expectedResponseFileName);

            if (expectedResponse.Length != actualResponse.Length)
            {
                
                string difference = DescribeDifference(expectedResponse, actualResponse);
                string newFileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), Path.GetFileNameWithoutExtension(expectedResponseFileName) + "_actual.txt");
                
                File.WriteAllText(newFileName, actualResponse);
                
                Assert.Fail(
                    "The response length (" + actualResponse.Length +
                    ") is not the same as the expected response length (" + expectedResponse.Length +
                    ")\r\n" + difference + "Compare expected to actual with\r\n" +
                    "odd " + expectedResponseFileName + " " + newFileName + "\r\n" +
                    "\r\nUpdate baseline with:\r\n" +
                    "copy /y " + newFileName + " " + expectedResponseFileName.Replace("GeneratedFiles\\..\\", "") + "\r\n\r\n");
            }
        }

        private static string DescribeDifference(string expected, string actual)
        {
            int len = Math.Min(expected.Length, actual.Length);

            if (len < 4)
            {
                return "";
            }
            
            int expectedLineNumber = 0;
            int rowNumber = 0;
            int difference = -1;
            for (int i = 0; i < len; i++)
            {
                string possibleNewLine = expected.Substring(i, 2);
                if (possibleNewLine == Environment.NewLine)
                {
                    expectedLineNumber++;
                    rowNumber = 0;
                }

                if (expected[i] != actual[i])
                {
                    difference = i;
                    break;
                }

                rowNumber++;
            }

            if (difference == -1)
            {
                return "For the length expectual and actual have in common, they match.\r\n";
            }
            else
            {
                string actualLine = actual.Split(new string[]{"\r\n"}, StringSplitOptions.None)[expectedLineNumber];
                string expectedLine = expected.Split(new string[]{Environment.NewLine}, StringSplitOptions.None)[expectedLineNumber];
                return string.Format(
                    CultureInfo.InvariantCulture, 
                    "Found difference on line number '{0}' row '{1}', {2}, expected: '{3}' {2} actual:'{4}'", 
                    expectedLineNumber, rowNumber, Environment.NewLine, expectedLine, actualLine);
            }
        }
    }

    public class OpenBatchDataService : DataService<CustomDataContext>
    {
        public static void InitializeService(DataServiceConfiguration configuration)
        {
            configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            configuration.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            configuration.UseVerboseErrors = OpenWebDataServiceHelper.ForceVerboseErrors;
            configuration.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        [WebGet]
        public IQueryable<Customer> GetCustomerByCity(string city)
        {
            return this.CurrentDataSource.Customers.Where(c => c.Address.City == city);
        }

        [WebInvoke]
        public void CreateCustomer(int id, string name)
        {
            CustomDataContext.AddResource(new Customer() { ID = id, Name = name }, true);
        }
    }

}
