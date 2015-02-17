//---------------------------------------------------------------------
// <copyright file="InsertVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.Net;
using AstoriaUnitTests.Data;
using System.Text.RegularExpressions;

namespace System.Data.Test.Astoria
{
    // TODO: deep insert verification, links verification

    public class InsertVerifier : PayloadVerifier
    {
        public static void Verify(AstoriaResponse response)
        {
            InsertVerifier verifier = new InsertVerifier(response);
            verifier.Verify();
        }

        public static bool Applies(AstoriaResponse response)
        {
            if (response.Request.EffectiveVerb != RequestVerb.Post)
                return false;

            if (response.Request.IsBlobRequest)
                return false;

            if (string.IsNullOrEmpty(response.Request.Payload))
            {
                // must be a service op or something
                return false;
            }
            if (response.Request.URI.Contains("$ref") || response.Request.URI.Contains("$batch"))
            {
                // not an insert
                return false;
            }

            // we don't know how to handle batched updates yet
            if (response.Request.Batched)
                return false;

            return true;
        }

        private InsertVerifier(AstoriaResponse response)
            : base(response)
        { }

        protected override void Verify()
        {
            if (!Applies(Response))
                return;

            // build a common payload for the insert
            //
            CommonPayload insertPayload = Response.Request.CommonPayload;

            // get the entity that was inserted
            PayloadObject inserted;
            if (!TryGetSingleObjectFromPayload(insertPayload, out inserted))
                ResponseVerification.LogFailure(Response, new Exception("Insert request payload did not contain a single entity"));

            // determine the type based on what was inserted
            ResourceType type = Response.Workspace.ServiceContainer.ResourceTypes.Single(rt => inserted.Type.Equals(rt.Namespace + "." + rt.Name));

            // get the entity that was returned
            PayloadObject returned;
            if (!TryGetSingleObjectFromPayload(Response.CommonPayload, out returned))
            {
                if (Versioning.Server.SupportsLiveFeatures)
                {
                    string preferHeader;
                    if (Response.Request.Headers.TryGetValue("prefer", out preferHeader) && preferHeader == "return=minimal")
                    {
                        return;
                    }
                }

                ResponseVerification.LogFailure(Response, new Exception("Insert response payload did not contain a single entity"));
            }

            // verify that the inserted and returned entities are equivalent
            VerifyInsertResponse(type, inserted, returned);

            // re-query the entity
            Workspace workspace = Response.Workspace;
            AstoriaRequest queryRequest = workspace.CreateRequest();
            if (type.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
                queryRequest.ETagHeaderExpected = true;

            if (type.Key.Properties.Any(p => p.Type == Clr.Types.DateTime))
            {
                // this will blow up for MEST, but we don't currently have any datetime key + MEST types
                ResourceContainer container = Response.Workspace.ServiceContainer.ResourceContainers.Single(rc => !(rc is ServiceOperation) && rc.ResourceTypes.Contains(type));
                queryRequest.Query = ContainmentUtil.BuildCanonicalQuery(ConcurrencyUtil.ConstructKey(container, returned));
            }
            else
            {
                queryRequest.URI = Uri.UnescapeDataString(returned.AbsoluteUri);
                if (queryRequest.URI.Contains("E+"))
                    queryRequest.URI = queryRequest.URI.Replace("E+", "E");
                if (queryRequest.URI.Contains("e+"))
                    queryRequest.URI = queryRequest.URI.Replace("e+", "e");
            }

            AstoriaResponse queryResponse = queryRequest.GetResponse();
            if (queryResponse.ActualStatusCode == HttpStatusCode.BadRequest)
            {
                // try it as a filter instead (possibly caused by the URI being too long)
                // this will blow up for MEST
                ResourceContainer container = Response.Workspace.ServiceContainer.ResourceContainers
                    .Single(rc => !(rc is ServiceOperation) && rc.ResourceTypes.Contains(type));
                KeyExpression key = ConcurrencyUtil.ConstructKey(container, returned);
                queryRequest = workspace.CreateRequest(Query.From(Exp.Variable(container)).Where(key.Predicate));
                queryResponse = queryRequest.GetResponse();
            }
            queryResponse.Verify();

            // get the entity from the query
            PayloadObject queried;
            if (!TryGetSingleObjectFromPayload(queryResponse.CommonPayload, out queried))
                ResponseVerification.LogFailure(queryResponse, new Exception("Query response payload did not contain a single entity"));

            // ensure that the entity did not change between the insert and the re-query
            VerifyQueryResponse(type, returned, queried);
        }

        private void VerifyInsertResponse(ResourceType type, PayloadObject inserted, PayloadObject returned)
        {
            // need to 'follow' deferred links to ensure that they're correct
            // doesn't need to match exactly
            AstoriaTestLog.AreEqual(inserted.Type, returned.Type, "Types do not match");

            VerifyProperties(type, inserted, returned, true, (rp => !rp.Facets.ServerGenerated));
        }

        private void VerifyProperties(ResourceType type, PayloadObject inserted, PayloadObject returned, bool missingInsertPropertiesOk, Func<ResourceProperty, bool> strictEquality)
        {
            List<string> propertyNames = inserted.PayloadProperties.Union(returned.PayloadProperties).Select(p => p.Name).ToList();
            propertyNames.AddRange(inserted.CustomEpmMappedProperties.Keys);
            propertyNames.AddRange(returned.CustomEpmMappedProperties.Keys);
            
            foreach (string propertyName in propertyNames.Distinct())
            {
                PayloadProperty insertedProperty;
                if(inserted.Format == SerializationFormatKind.JSON) //in JSON the first one wins
                    insertedProperty = inserted.PayloadProperties.FirstOrDefault(p => p.Name == propertyName);
                else //in xml the last one wins
                    insertedProperty = inserted.PayloadProperties.LastOrDefault(p => p.Name == propertyName);
                bool insertHadProperty = insertedProperty != null;

                PayloadProperty returnedProperty = returned.PayloadProperties.LastOrDefault(p => p.Name == propertyName); ;
                bool returnedHadProperty = returnedProperty != null;

                ResourceProperty property = type.Properties.OfType<ResourceProperty>().SingleOrDefault(p => p.Name == propertyName);

                if (property == null || !property.Facets.IsDeclaredProperty)
                {
                    if (!type.Facets.IsOpenType)
                    {
                        string error = "included dynamic property '" + propertyName + "' despite '" + type.Name + "' not being an open type";
                        if (insertHadProperty && returnedHadProperty)
                            AstoriaTestLog.FailAndThrow("Both the inserted and returned objects " + error);
                        else if (insertHadProperty)
                            AstoriaTestLog.FailAndThrow("The inserted object " + error);
                        else if (returnedHadProperty)
                            AstoriaTestLog.FailAndThrow("The returned object " + error);
                    }
                    else
                    {
                        if (insertHadProperty && returnedHadProperty)
                        {
                            CompareDynamicPropertyValues(insertedProperty, returnedProperty);
                        }
                        else if (insertHadProperty)
                        {
                            // only acceptable if inserted value was null
                            if (insertedProperty.IsNull)
                                AstoriaTestLog.FailAndThrow("Returned object missing non-null dynamic property '" + propertyName + "'");
                        }
                    }
                }
                else
                {
                    if (property.IsNavigation)
                    {
                        // the insert payload may not specify this property
                        //PayloadObject insertedObject = inserted.PayloadObjects.SingleOrDefault(o => o.Name == property.Name);
                        PayloadObject returnedObject = returned.PayloadObjects.Single(o => o.Name == property.Name);

                        // returned thing should be deferred whether its a reference or not
                        AstoriaTestLog.AreEqual(true, returnedObject.Deferred, "!returnedObject.Deferred", false);

                        // TODO: verify only expected links are present (based on uri and payload values as well)
                    }
                    else
                    {
                        if (insertHadProperty && returnedHadProperty)
                        {
                            try
                            {
                                ComparePropertyValues(property, insertedProperty, returnedProperty, strictEquality(property));
                            }
                            catch (Exception e)
                            {
                                throw new TestFailedException("Value of property '" + property.Name + "' does not match inserted value", null, null, e);
                            }
                        }
                        else if (insertHadProperty)
                        {
                            AstoriaTestLog.FailAndThrow("Returned object unexpectedly missing declared property '" + propertyName + "'");
                        }
                        else if (returnedHadProperty)
                        {
                            if (missingInsertPropertiesOk)
                                AstoriaTestLog.WriteLineIgnore("Ignoring missing declared property '" + propertyName + "' in insert payload");
                            else
                                AstoriaTestLog.FailAndThrow("Inserted object unexpectedly missing declared property '" + propertyName + "'");
                        }
                    }
                }
            }
        }

        private void VerifyQueryResponse(ResourceType type, PayloadObject fromInsert, PayloadObject fromQuery)
        {
            bool keyShouldMatch = true;
            if (type.Key.Properties.Any(p => p.Facets.FixedLength))
            {
                foreach (NodeProperty property in type.Key.Properties.Where(p => p.Facets.FixedLength))
                {
                    // this may not always be correct for all types, but it seems to work most of the time;
                    int length;
                    if (property.Facets.MaxSize.HasValue)
                        length = property.Facets.MaxSize.Value;
                    else if (property.Type == Clr.Types.Decimal)
                        length = property.Facets.Precision.Value + property.Facets.Scale.Value;
                    else
                        continue;

                    string value = null;
                    if (fromInsert.PayloadProperties.Any(p => p.Name == property.Name))
                    {
                        value = (fromInsert[property.Name] as PayloadSimpleProperty).Value;
                    }
                    else
                    {
                        value = fromInsert.CustomEpmMappedProperties[property.Name];
                    }

                    if (value != null && value.Length < length)
                        keyShouldMatch = false;
                }
            }

            if (keyShouldMatch)
                AstoriaTestLog.AreEqual(fromInsert.AbsoluteUri, fromQuery.AbsoluteUri, "fromInsert.AbsoluteUri != fromQuery.AbsoluteUri", false);
            else
                AstoriaTestLog.IsFalse(fromInsert.AbsoluteUri == fromQuery.AbsoluteUri, "fromInsert.AbsoluteUri == fromQuery.AbsoluteUri, despite fixed-length string");

            // leave it to the concurrency tests to check the more involved etags
            if (!type.Properties.Any(p => p.Facets.ConcurrencyModeFixed && p.Facets.FixedLength))
                AstoriaTestLog.AreEqual(fromInsert.ETag, fromQuery.ETag, "fromInsert.ETag != fromQuery.ETag", false);

            VerifyProperties(type, fromInsert, fromQuery, false, rp => true);
        }
    }
}
