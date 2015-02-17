//---------------------------------------------------------------------
// <copyright file="ResponseVerification.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public static class ResponseVerification
    {
        /// <summary>Verifies the result when an unknown verb is sent to the server.</summary>
        public static void VerifyMetadata(AstoriaResponse response)
        {
#if !ClientSKUFramework

            response.Workspace.VerifyMetadata(response.Payload);
#endif
        }

        public static void Verify(this AstoriaResponse response)
        {
            ResponseVerification.Verify(response, false);
        }

        public static void Verify(this AstoriaResponse response, bool throwOnError)
        {
            try
            {
                if (response.Request.URI.Contains("$value") &&
                    response.ActualStatusCode == System.Net.HttpStatusCode.NotFound &&
                    response.Request.ExpectedStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Skip statuscode verification if $value request returns unexpected NotFound (means that value was null)
                    // This should be verified by ExtraVerification step (VerifyGet)
                }
                else if (response.ActualStatusCode == System.Net.HttpStatusCode.NoContent && response.Request.Verb == RequestVerb.Get)
                {
                    // Skip if nav prop is null. Expected status code would be OK but NoContent will be returned
                }
                else
                    VerifyStatusCode(response);

                if ((int)response.ActualStatusCode >= 400 || response.Request.ErrorExpected)
                    VerifyError(response);
                else if (response is BatchResponse)
                {
                    //Batching in V1 had a bug in this area, so only check it its a V2 server
                    if(Versioning.Server.SupportsV2Features)
                        VerifySpecificResponseVersion(response, 1, 0);

                    foreach (AstoriaResponse subResponse in (response as BatchResponse).Responses)
                        subResponse.Verify();
                }
                else
                {
                    if (response.Request.MetadataOnly)
                        VerifyMetadata(response);
                    else
                    {
                        VerifyConcurrency(response);

                        if (InsertVerifier.Applies(response))
                            InsertVerifier.Verify(response);

                        if (UpdateVerifier.Applies(response))
                            UpdateVerifier.Verify(response);

                        if (DeleteVerifier.Applies(response))
                            DeleteVerifier.Verify(response);

                        // always do this last in case it has side-effects
                        if (response.Request.ExtraVerification != null)
                            response.Request.ExtraVerification(response);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFailure(response, ex, throwOnError);
            }
        }

        public static void LogFailure(AstoriaResponse response, Exception ex)
        {
            LogFailure(response, ex, true);
        }

        public static void LogFailure(AstoriaResponse response, Exception ex, bool shouldThrow)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Response verification failed");
            builder.AppendLine("----------------------------------------");
            response.Request.LogRequest(builder, true, true);
            builder.AppendLine("----------------------------------------");
            response.LogResponse(builder, true, true);
            builder.AppendLine("----------------------------------------");

            string log = builder.ToString();
            ex = new TestFailedException(log, null, null, ex);
            if (shouldThrow)
                throw ex;
            else
                AstoriaTestLog.FailAndContinue(ex);
        }

        public static void DefaultVerify(AstoriaResponse response)
        {
            // special case for ETags
            if (response.ActualStatusCode == System.Net.HttpStatusCode.NotModified)
                return;

            AstoriaRequest request = response.Request;

            RequestVerb verb = request.EffectiveVerb;
            
            switch (verb)
            {
                case RequestVerb.Get:
                    VerifyGet(response);
                    break;

                case RequestVerb.Post:
                case RequestVerb.Patch:
                case RequestVerb.Put:
                case RequestVerb.Delete:
                    // other verbs now handled by default elsewhere
                    break;

                default:
                    VerifyUnknownVerb(response);
                    break;
            }
        }

        public static void VerifyStatusCode(AstoriaResponse response)
        {
            VerifyStatusCode(response, response.Request.ExpectedStatusCode);
        }

        public static void VerifyStatusCode(AstoriaResponse response, System.Net.HttpStatusCode expected)
        {
            bool areEqual = expected == response.ActualStatusCode;
            if (!areEqual)
            {
                string errorMessage = string.Format("Expecting status code {0}, but received status code {1}",
                     expected, response.ActualStatusCode);
                throw new TestFailedException(errorMessage);
            }
        }

        public static void VerifySpecificResponseVersion(AstoriaResponse response, int major, int minor)
        {
            if (response.DataServiceVersion == null)
                AstoriaTestLog.FailAndThrow("Data service version should always be specified in response");

            AstoriaTestLog.IsTrue(response.DataServiceVersion.StartsWith(major + "." + minor),
                String.Format("Unexpected value in response's DataServiceVersion header: {0}, expected: {1}.{2}", response.DataServiceVersion, major, minor));
        }

        public static void VerifyGet(AstoriaResponse response)
        {
            AstoriaRequest request = response.Request;

            if (request.Query != null)
            {
                LinqQueryBuilder linqBuilder = new LinqQueryBuilder(request.Workspace);
                linqBuilder.Build(request.Query);
                if (request.URI.Contains("$value"))
                {
                    CommonPayload payload;

                    // $value queries will sometimes return unexpected 404s due to null values
                    // if we get a 404 unexpectedly, the underlying result must be null
                    if (response.ActualStatusCode == System.Net.HttpStatusCode.NotFound)
                        payload = request.CommonPayload;

                    IEnumerable enumerable = linqBuilder.QueryResult;
                    object expectedResult = CommonPayload.GetSingleValueFromIQueryable(linqBuilder.QueryResult, false);

                    ResourceProperty rp = request.GetPropertyFromQuery();
                    CommonPayload.ComparePrimitiveValuesObjectAndString(expectedResult, rp.Type.ClrType, response.Payload, true, request.Format, false);
                }
                else if (request.URI.Contains("$count") || request.URI.Contains("$inlinecount"))
                {
                    if (request.URI.Contains("$count"))
                    {
                        // Versioning: make sure the server always returns 2.0.
                        VerifySpecificResponseVersion(response, 2, 0);

                        // PlainText payload.
                        try
                        {
                            VerifyStatusCode(response);
                            if (response.Request.ExpectedStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                int countPayload = Int32.Parse(response.Payload);
                                int countBaseline = CommonPayload.CreateList(linqBuilder.QueryResult).Count;
                                AstoriaTestLog.AreEqual(countPayload, countBaseline, "Counts in payload (" + countPayload + ") and baseline (" + countBaseline + ") differ.");
                            }
                        }
                        catch (TestFailedException tfe)
                        {
                            //When the underlying resource has zero elements, the server now throws a 404 error , causing false negatives
                            ValidateCountException(response, request, tfe);
                        }
                    }
                    else
                    {
                        VerifySpecificResponseVersion(response, 2, 0);
                        
                        try
                        {
                            // JSON, Atom or PlainXml ($ref) payload.
                            VerifyStatusCode(response);
                            if (response.Request.ExpectedStatusCode == System.Net.HttpStatusCode.OK)
                            {
                                CommonPayload payload = response.CommonPayload;
                                LinqQueryBuilder baselineLinqBuilder = new LinqQueryBuilder(request.Workspace);
                                baselineLinqBuilder.CountingMode = true;
                                baselineLinqBuilder.Build(request.Query);

                                object baselineElementsCount = CommonPayload.CreateList(baselineLinqBuilder.QueryResult).Count;
                                payload.CompareCountInline(linqBuilder.QueryResult, baselineElementsCount);
                                AstoriaTestLog.WriteLine(linqBuilder.QueryExpression);
                                if (request.URI.Contains("$ref"))
                                    VerifyLinksPayload(request.Workspace, payload, linqBuilder);
                            }
                        }
                        catch (TestFailedException tfe)
                        {
                            //When the underlying resource has zero elements, the server now throws a 404 error , causing false negatives
                            ValidateCountException(response, request, tfe);
                        }
                    }
                }
                else
                {
                    if (response.ActualStatusCode == System.Net.HttpStatusCode.NoContent && response.Payload == "")
                        response.Payload = null; // allow result from Linq query and payload to be compared (NoContent is a get to a null Nav prop)

                    if (response.ActualStatusCode == System.Net.HttpStatusCode.NotFound)
                        return;

                    CommonPayload payload = response.CommonPayload;
                    if (request.URI.Contains("$ref"))
                    {
                        VerifyLinksPayload(request.Workspace, payload, linqBuilder);
                    }
                    else if (payload.Resources == null)
                    {
                        if (request.Format == SerializationFormatKind.JSON)
                            payload.CompareValue(linqBuilder.QueryResult, true, false);
                        else
                            payload.CompareValue(linqBuilder.QueryResult, false, false);
                    }
                    else
                        payload.Compare(linqBuilder.QueryResult);
                }
            }
        }

        private static bool PayloadHasNamedStreams(List<PayloadObject> entities)
        {
            return entities.Any(e => e.NamedStreams.Any() || PayloadHasNamedStreams(e.PayloadObjects));
        }

        public static void ValidateCountException(AstoriaResponse response, AstoriaRequest request, TestFailedException tfe)
        {
            // Errors are expected for older versions.
            if (!Versioning.Server.SupportsV2Features)
            {
                if (request.URI.Contains("$inlinecount=allpages"))
                {
                    // Query parameter not recognized.
                    if (response.ActualStatusCode == System.Net.HttpStatusCode.BadRequest)
                        return;
                }
                //anything other than http 404 or 400 is an invalid status code for a count query 
                else if (
                    response.ActualStatusCode != System.Net.HttpStatusCode.BadRequest
                    || response.ActualStatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw tfe;
                }
                else
                {
                    // Resource segment $count not found.
                    if (response.ActualStatusCode == System.Net.HttpStatusCode.NotFound || response.ActualStatusCode == System.Net.HttpStatusCode.BadRequest)
                        return;
                }
            }

            LinqQueryBuilder linqBuilder = new LinqQueryBuilder(request.Workspace);
            linqBuilder.CountingMode = request.URI.Contains("$inlinecount=allpages");
            try
            {
                linqBuilder.Build(request.Query);
            }
            catch (KeyNotFoundException kfe)
            {
                if (!AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr))
                {
                    throw kfe;
                }
            }
            bool isInvalidError = true;
            if (response.ActualStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                if (request.Query.Input is CountExpression)
                {
                    ExpNode internalQuery = ((CountExpression)request.Query.Input).ScanNode;
                    if (internalQuery is KeyExpression || internalQuery is PredicateExpression)
                    {
                        isInvalidError = false;
                    }
                }
                if (isInvalidError)
                {
                    if (CommonPayload.CreateList(linqBuilder.QueryResult).Count == 0)
                    {
                        isInvalidError = false;
                    }
                }
            }
            if (response.ActualStatusCode == System.Net.HttpStatusCode.BadRequest)
            {

                if (request.Query.Input is CountExpression)
                {
                    ExpNode internalQuery = ((CountExpression)request.Query.Input).ScanNode;
                    if (internalQuery is KeyExpression || internalQuery is PredicateExpression)
                    {
                        isInvalidError = false;
                    }
                }
                else if (request.Query.Input is KeyExpression)
                {
                    isInvalidError = false;
                }
            }

            if (isInvalidError)
            {
                throw tfe;
            }
        }

        public static void VerifyUnknownVerb(AstoriaResponse webResponse)
        {
            AstoriaTestLog.AreEqual(System.Net.HttpStatusCode.NotImplemented, webResponse.ActualStatusCode,
                "Unknown verbs should receive a 'Not Implemented' reply.");

            if (webResponse.ContentType != null)
            {
                AstoriaTestLog.IsTrue(webResponse.ContentType.StartsWith(SerializationFormatKinds.JsonMimeType, StringComparison.Ordinal),
                    "Error messages should have no content type or an application/json content type.");
            }
        }

        private static void VerifyLinksPayload(Workspace w, CommonPayload payload, LinqQueryBuilder linqBuilder)
        {
            ArrayList expectedEntities = CommonPayload.CreateList(linqBuilder.QueryResult);
            if (payload == null)
                AstoriaTestLog.AreEqual(expectedEntities.Count, 0, "Unexpected null $ref payload");
            else
            {
                KeyExpressions expectedKeys = new KeyExpressions();
                foreach (object o in expectedEntities)
                {
                    KeyExpression keyExp = w.CreateKeyExpressionFromProviderObject(o);
                    expectedKeys.Add(keyExp);
                }

                List<string> linksFound = new List<string>();

                if (payload.Resources == null)
                {
                    linksFound.Add(payload.Value);
                }
                else
                {
                    foreach (PayloadObject o in (payload.Resources as List<PayloadObject>))
                    {
                        if(o.PayloadProperties.Any(p => p.Name == "uri"))
                            linksFound.Add((o["uri"] as PayloadSimpleProperty).Value);
                    }
                }

                AstoriaTestLog.AreEqual(expectedKeys.Count, linksFound.Count, "Number of expected entities does not match number of links found");

                foreach (string link in linksFound)
                {
                    KeyExpression match = null;
                    foreach (KeyExpression expectedKey in expectedKeys)
                    {
                        if (compareKeyURI(link, expectedKey))
                        {
                            match = expectedKey;
                            break;
                        }
                    }

                    if (match != null)
                    {
                        expectedKeys.Remove(match);
                    }
                    else
                    {
                        AstoriaTestLog.WriteLineIgnore("Unexpected URI: '" + link + "'");
                        AstoriaTestLog.FailAndThrow("Unexpected URI found in $ref payload");
                    }
                }
            }
        }

        private static bool compareKeyURI(string uriFound, KeyExpression keyExpected)
        {
            UriQueryBuilder builder = new UriQueryBuilder(keyExpected.ResourceContainer.Workspace, keyExpected.ResourceContainer.Workspace.ServiceUri);
            builder.EscapeUriValues = true;
            builder.CleanUpSpecialCharacters = false;
            builder.UseBinaryFormatForDates = false;

            switch (keyExpected.IncludeInUri.Count(i => i))
            {
                case 0:
                    AstoriaTestLog.FailAndThrow("Cannot compare KeyExpression to URI, key has no included values");
                    return false;

                case 1:
                    // TODO: stop ignoring case
                    QueryNode query = ContainmentUtil.BuildCanonicalQuery(keyExpected);
                    string expected = builder.Build(query);

                    expected = expected.Replace(".0f", "f"); //this is kinda a hack, but TypeData.FormatForKey is going to add the .0, so we need to remove it
                    expected = expected.Replace(".0D", "D"); //this is kinda a hack, but TypeData.FormatForKey is going to add the .0, so we need to remove it
                    bool match = uriFound.Equals(expected, StringComparison.InvariantCultureIgnoreCase);
                    if (!match)
                        AstoriaTestLog.WriteLineIgnore("Link did not match key, expected '" + expected + "'");
                    return match;

                default:
                    QueryNode setQuery = ContainmentUtil.BuildCanonicalQuery(keyExpected, true);

                    Workspace w = keyExpected.ResourceContainer.Workspace;
                    string setUri = builder.Build(setQuery);

                    string keySegment = uriFound.Substring(setUri.Length);

                    string expectedKeySegment = "(" + UriQueryBuilder.CreateKeyString(keyExpected, false) + ")";

                    if (keySegment.Equals(expectedKeySegment, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                    // if not explicitely equal, need to make sure its not due to a re-ordering of the properties
                    //
                    List<KeyValuePair<string, int>> predicateLocations = new List<KeyValuePair<string, int>>();
                    for (int i = 0; i < keyExpected.Values.Length; i++)
                    {
                        string predicate = builder.CreateKeyStringPair(keyExpected.Properties[i].Name, keyExpected.Values[i].ClrValue);
                        int offset = keySegment.IndexOf(predicate);
                        if (offset < 0)
                            return false;

                        predicateLocations.Add(new KeyValuePair<string, int>(predicate, offset));
                    }

                    predicateLocations.Sort(delegate(KeyValuePair<string, int> pair1, KeyValuePair<string, int> pair2)
                    {
                        return pair1.Value.CompareTo(pair2.Value);
                    });

                    expectedKeySegment = "(" + String.Join(",", predicateLocations.Select(pair => pair.Key).ToArray()) + ")";

                    return keySegment.Equals(expectedKeySegment, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public static void VerifyConcurrency(AstoriaResponse response)
        {
            if (response.Request.ETagHeaderExpected)
            {
                if (!response.ETagHeaderFound && !RequestUtil.ExpectEmptyHeaders(response))
                    AstoriaTestLog.FailAndThrow("Missing ETag header in response");
            }
            else if (response.ETagHeaderFound)
            {
                if (string.IsNullOrEmpty(response.ETagHeader))
                {
                    if (Versioning.Server.BugFixed_NullETagWhenTypeHasNoConcurrency)
                        AstoriaTestLog.FailAndThrow("Unexpected null ETag header in response");
                    else
                        AstoriaTestLog.WriteLine("Ignoring unexpected null ETag header in response due to known bug");
                }
                else
                    AstoriaTestLog.FailAndThrow("Unexpected ETag header in response: '" + response.ETagHeader + "'");
            }
        }

        public static void VerifyResponseFormat(AstoriaResponse response)
        {
            if ((response.Request.Format == SerializationFormatKind.JSON && response.ContentType.StartsWith(SerializationFormatKinds.JsonMimeType, StringComparison.Ordinal)) ||
                (response.Request.Format == SerializationFormatKind.Atom && response.ContentType != "application/xml"))
                throw new TestFailedException(String.Format("Error content-type does not match request content-type - expected: {0}, actual: {1}", response.Request.Format.ToString(), response.ContentType));
        }

        #region error verification

        #region error parsers

        // will match from <error> to </error> (note that more than one error tag will disrupt this up), takes namespace into account
        // first subcapture is the entire <error> tag, second subcapture is the namespace prefix
        private static Regex XmlInStreamErrorRegex = new Regex(@".*(<(?:(.*):)?error[^>]*>.*</(?:.*:)?error>)\s*", RegexOptions.Singleline);

        private static string ParseResponseErrorXML(AstoriaResponse response, bool inStream)
        {
            string payloadString = response.Payload;

            ServiceError serviceError = new ServiceError();

            XmlNode error = null;
            if (inStream)
            {
                Match match = XmlInStreamErrorRegex.Match(payloadString);
                if (!match.Success)
                {
                    AstoriaTestLog.TraceLine(payloadString);
                    AstoriaTestLog.FailAndThrow("Payload did not contain expected in-stream error");
                }
                payloadString = match.Groups[1].Value;

                // if there was a namespace prefix, we need to inject a wrapping element with the namespace defined
                //
                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    payloadString = "<wrapper xmlns:" + match.Groups[2].Value + "=\"" + AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace + "\">" +
                        payloadString +
                        "</wrapper>";
                }
                else
                {
                    // just for consistency later when we pull out the <error> tag
                    //
                    payloadString = "<wrapper>" + payloadString + "</wrapper>";
                }
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(payloadString);
                }
                catch (XmlException ex)
                {
                    AstoriaTestLog.FailAndContinue(ex);
                    return null;
                }

                // pull out the <error> tag, assuming that there is a <wrapper> tag around it
                //
                error = xmlDoc.FirstChild.FirstChild;
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(payloadString);
                }
                catch (XmlException ex)
                {
                    AstoriaTestLog.FailAndContinue(ex);
                    return null;
                }

                error = xmlDoc.FirstChild.NextSibling;
            }

            if (error != null)
            {
                XmlNode code = error.FirstChild;
                XmlNode message = code.NextSibling;
                XmlNode innerError = message.NextSibling;

                serviceError.code = code.InnerXml;
                serviceError.message = message.InnerXml;
                if (message.Attributes["xml:lang"] != null)
                    serviceError.language = message.Attributes["xml:lang"].Value;
                else
                    serviceError.language = null;

                if (innerError != null)
                {
                    XmlNode innerMessage = null;
                    XmlNode innerType = null;
                    XmlNode innerStackTrace = null;

                    innerMessage = innerError.FirstChild;
                    if (innerMessage != null)
                    {
                        serviceError.InnerServiceError.message = innerMessage.InnerXml;
                        innerType = innerMessage.NextSibling;
                        if (innerType != null)
                        {
                            serviceError.InnerServiceError.type = innerType.InnerXml;
                            innerStackTrace = innerType.NextSibling;
                            if (innerStackTrace != null)
                            {
                                serviceError.InnerServiceError.stacktrace = innerStackTrace.InnerXml;
                            }
                        }
                    }
                }
            }

            return serviceError.message;
        }

        // will match from '{ "error"' to the end of the content
        private static Regex JsonInStreamErrorRegex = new Regex(".*({\\s*\"error\".*)", RegexOptions.Singleline);

        private static string ParseResponseErrorJSON(AstoriaResponse response, bool inStream)
        {
            // error should be something like
            //{
            //  "error": {
            //    "code": "", "message":  "Error message"
            //  }
            //}
            ServiceError serviceError = new ServiceError();
            string payloadString = response.Payload;

            CommonPayload payload;
            if (inStream)
            {
                Match match = JsonInStreamErrorRegex.Match(payloadString);
                if (!match.Success)
                {
                    AstoriaTestLog.TraceLine(payloadString);
                    AstoriaTestLog.FailAndThrow("Payload did not contain expected in-stream error");
                }
                response.Payload = match.Groups[1].Value;
            }
            payload = response.CommonPayload;

            PayloadProperty prop;
            PayloadComplexProperty complex = payload.Resources as PayloadComplexProperty;

            if (complex != null)
            {
                if (complex.PayloadProperties.TryGetValue("message", out prop))
                {
                    if (prop is PayloadComplexProperty)
                    {
                        if ((prop as PayloadComplexProperty).PayloadProperties.TryGetValue("value", out prop))
                        {
                            serviceError.message = (prop as PayloadSimpleProperty).Value;
                        }
                    }
                }
            }

            return serviceError.message;
        }

        // TODO: do full verification of code, language and maybe InnerServiceError as well
        struct ServiceError
        {
            public string code;
            public string language;
            public string message;
            public InnerServiceError InnerServiceError;
        }

        struct InnerServiceError
        {
            public string message;
            public string type;
            public string stacktrace;
        }

        #endregion

        #region helper methods

        public static string GetLocalizedResourceString(Workspace w, ResourceIdentifier identifier, bool isLocal, object[] args)
        {
            try
            {
                if (isLocal)
                    return ResourceUtil.GetLocalizedResourceString(identifier, args);
                else
                    return w.GetLocalizedResourceString(identifier.Id, args);
            }
            catch (ArgumentException e)
            {
                if (e.Message == ResourceUtil.MissingLocalizeResourceString)
                {
                    AstoriaTestLog.FailAndThrow(String.Format("Resource identifier '{0}' could not be found {1}",
                        identifier.Id, (isLocal ? "locally" : "remotely")));
                }
                throw;
            }
        }

        public static bool InStreamErrorExpected(AstoriaResponse response)
        {
            // for now, in stream errors are expected for status codes < 400 and not PreconditionFailed
            //
            return (int)response.ActualStatusCode < 400 && response.ActualStatusCode != System.Net.HttpStatusCode.PreconditionFailed;
        }

        #endregion

        #region VerifyError overloads

        // order: most to least specific

        public static void VerifyError(AstoriaResponse response)
        {
            // general error verification, called directly by response.Verify()

            if (!InStreamErrorExpected(response))
            {
                // Error responses should ALWAYS be 1.0 for top-level errors
                VerifySpecificResponseVersion(response, 1, 0);
            }

            if (response.Request.ExpectedErrorIdentifier != null)
                VerifyError(response, response.Request.ExpectedErrorIdentifier, response.Request.ExpectedErrorArguments);
            // leaving this off until we're ready to fix it everywhere
            //else
            //    AstoriaTestLog.Warning(false, "Error condition with ExpectedErrorIdentifier unset");

            if (response.Exception != null)
                AstoriaTestLog.HandleException(response.Exception);
        }

        public static void VerifyError(AstoriaResponse response, ResourceIdentifier resourceIdentifier, params object[] args)
        {
            if (response.ContentType.Contains(SerializationFormatKinds.JsonMimeType))
                VerifyError(ParseResponseErrorJSON, response, resourceIdentifier, false, args);
            else
                VerifyError(ParseResponseErrorXML, response, resourceIdentifier, false, args);
        }

        public static void VerifyError(AstoriaResponse response, string pattern, params object[] args)
        {
            if (response.ContentType.Contains(SerializationFormatKinds.JsonMimeType))
                VerifyError(ParseResponseErrorJSON, response, ResourceUtil.FormatResourceString(pattern, args), ComparisonFlag.Full);
            else
                VerifyError(ParseResponseErrorXML, response, ResourceUtil.FormatResourceString(pattern, args), ComparisonFlag.Full);
        }

        public static void VerifyError(Func<AstoriaResponse, string> responseParser, AstoriaResponse response, ResourceIdentifier resourceIdentifier, bool localResource, params object[] args)
        {
            VerifyError((payload, inStream) => responseParser(payload), response, resourceIdentifier, localResource, args);
        }

        private static void VerifyError(Func<AstoriaResponse, bool, string> responseParser, AstoriaResponse response, ResourceIdentifier resourceIdentifier, bool localResource, params object[] args)
        {
            string resourceText = GetLocalizedResourceString(response.Workspace, resourceIdentifier, localResource, args);
            VerifyError(responseParser, response, resourceText, resourceIdentifier.ComparisonFlag);
        }

        private static void VerifyError(Func<AstoriaResponse, bool, string> responseParser, AstoriaResponse response, string expectedErrorString, ComparisonFlag comparison)
        {
            bool inStream = InStreamErrorExpected(response);

            // actual error returned
            string actualErrorString = responseParser(response, inStream);

            // expected error string (from service op in service)
            AstoriaTestLog.TraceInfo("Actual Error String --> " + actualErrorString);
            AstoriaTestLog.TraceInfo("Expected Error String --> " + expectedErrorString);
            ResourceUtil.VerifyMessage(actualErrorString, expectedErrorString, comparison);
        }

        #endregion
        #endregion
    }
}

