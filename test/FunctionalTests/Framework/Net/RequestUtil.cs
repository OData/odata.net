//---------------------------------------------------------------------
// <copyright file="RequestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator
using System.Net;
using System.Text;
using System.Diagnostics;

namespace System.Data.Test.Astoria
{
    public static class RequestUtil
    {
        #region GetAndVerifyStatusCode
        public static void GetAndVerifyStatusCode(Workspace w, QueryNode query)
        {
            AstoriaRequest request = w.CreateRequest(query);
            AstoriaResponse response = request.GetResponse();
            ResponseVerification.VerifyStatusCode(response);
        }

        public static void GetAndVerifyStatusCode(Workspace w, QueryNode query, HttpStatusCode expectedStatusCode)
        {
            AstoriaRequest request = w.CreateRequest(query);
            request.ExpectedStatusCode = expectedStatusCode;
            AstoriaResponse response = request.GetResponse();
            ResponseVerification.VerifyStatusCode(response);
        }

        public static void GetAndVerifyStatusCode(Workspace w, string uri, HttpStatusCode expectedStatusCode)
        {
            string responsePayload;
            GetAndVerifyStatusCode(w, uri, expectedStatusCode, out responsePayload);
        }

        public static void GetAndVerifyStatusCode(Workspace w, string uri, HttpStatusCode expectedStatusCode, out string responsePayload)
        {
            AstoriaResponse response;
            GetAndVerifyStatusCode(w, uri, expectedStatusCode, out response);
            responsePayload = response.Payload;
        }

        public static void GetAndVerifyStatusCode(Workspace w, string uri, HttpStatusCode expectedStatusCode, out CommonPayload responsePayload)
        {
            AstoriaResponse response;
            GetAndVerifyStatusCode(w, uri, expectedStatusCode, out response);
            responsePayload = response.CommonPayload;
        }

        public static void GetAndVerifyStatusCode(Workspace w, string uri, HttpStatusCode expectedStatusCode, out AstoriaResponse response)
        {
            GetAndVerifyStatusCode(w, uri, expectedStatusCode, out response, null);
        }

        public static void GetAndVerifyStatusCode(Workspace w, string uri, HttpStatusCode expectedStatusCode, out AstoriaResponse response, WebHeaderCollection requestHeaders)
        {
            AstoriaRequest request = w.CreateRequest();

            request.URI = uri;
            request.ExpectedStatusCode = expectedStatusCode;
            request.Accept = "*/*";

            if (requestHeaders != null)
            {
                foreach (string header in requestHeaders.AllKeys)
                {
                    request.Headers[header] = requestHeaders[header];
                }
            }
            response = request.GetResponse();
            try
            {
                ResponseVerification.VerifyStatusCode(response);
            }
            catch (Exception e)
            {
                ResponseVerification.LogFailure(response, e);
            }
        }
        #endregion

        #region BuildRandomRequest
        public static AstoriaRequest BuildRandomRequest(Workspace workspace, RequestVerb verb, SerializationFormatKind format)
        {
            IEnumerable<ResourceContainer> safeContainers =
                workspace.ServiceContainer.ResourceContainers
                    .Where(c => IsSafeOperation(verb, c));

            if (verb != RequestVerb.Get)
                safeContainers = safeContainers.Where(c => c.ResourceTypes.Any(t => IsSafeOperation(verb, c, t)));

            fxList<ResourceContainer> containers = new fxList<ResourceContainer>(safeContainers);
            if (!containers.Any())
                return null;

            AstoriaRequest request = null;

            while (request == null && containers.Any())
            {
                ResourceContainer container = null;
                ResourceType type = null;
                KeyExpression key = null;

                while (container == null && containers.Any())
                {
                    container = containers.Choose();

                    if (verb == RequestVerb.Get)
                    {
                        key = workspace.GetRandomExistingKey(container);
                        if (key == null)
                        {
                            containers.Remove(container);
                            container = null;
                        }
                    }
                    else
                    {
                        fxList<ResourceType> types = new fxList<ResourceType>(container.ResourceTypes.Where(t => IsSafeOperation(verb, container, t)));

                        if (!types.Any())
                        {
                            containers.Remove(container);
                            container = null;
                        }

                        if (verb != RequestVerb.Delete)
                            type = types.Choose();
                        else
                        {
                            while (key == null && types.Any())
                            {
                                type = types.Choose();
                                key = workspace.GetRandomExistingKey(container, type);
                                if (key == null)
                                    types.Remove(type);
                            }

                            if (key == null)
                            {
                                containers.Remove(container);
                                container = null;
                            }
                        }
                    }
                }

                // if we ran out of containers before finding one that would work
                //
                if (container == null)
                    return null;

                // if the Build___ method returns null, we'll come back around with a different container/key
                //
                switch (verb)
                {
                    case RequestVerb.Get:
                        request = BuildGet(workspace, key, HttpStatusCode.OK, format);
                        break;

                    case RequestVerb.Post:
                        request = BuildInsert(workspace, container, type, HttpStatusCode.Created, format);
                        break;

                    case RequestVerb.Put:
                    case RequestVerb.Patch:
                        request = BuildUpdate(workspace, container, type, (verb == RequestVerb.Put), HttpStatusCode.NoContent, format);
                        break;

                    case RequestVerb.Delete:
                        request = BuildDelete(workspace, key, HttpStatusCode.NoContent, format);
                        break;

                    default:
                        throw new ArgumentException("Unsupported verb: " + verb.ToString());
                }
            }

            // might be null, but we did our best
            //
            return request;
        }

        #region GET

        public static AstoriaRequest BuildGet(Workspace workspace, ResourceContainer container)
        {
            return BuildGet(workspace, container, HttpStatusCode.OK, SerializationFormatKind.Default);
        }

        public static AstoriaRequest BuildGet(Workspace workspace, KeyExpression key)
        {
            return BuildGet(workspace, key, HttpStatusCode.OK, SerializationFormatKind.Default);
        }

        public static AstoriaRequest BuildGet(Workspace workspace, ResourceContainer container, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            if (!container.Facets.TopLevelAccess && expectedStatusCode == HttpStatusCode.OK)
                expectedStatusCode = HttpStatusCode.BadRequest;

            QueryNode query = Query.From(Exp.Variable(container)).Select();

            return BuildGet(workspace, query, expectedStatusCode, format);
        }

        public static AstoriaRequest BuildGet(Workspace workspace, KeyExpression key, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            QueryNode query = ContainmentUtil.BuildCanonicalQuery(key);

            string keyString = UriQueryBuilder.CreateKeyString(key, false);
            if ((expectedStatusCode == System.Net.HttpStatusCode.OK) && (keyString.Contains("/") || keyString.Contains(Uri.EscapeDataString("/"))))
                expectedStatusCode = System.Net.HttpStatusCode.BadRequest;

            return BuildGet(workspace, query, expectedStatusCode, format);
        }

        public static AstoriaRequest BuildGet(Workspace workspace, QueryNode query, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            AstoriaRequest request = workspace.CreateRequest();

            request.Verb = RequestVerb.Get;
            request.Query = query;
            request.ExpectedStatusCode = expectedStatusCode;
            request.Format = format;

            return request;
        }
        #endregion

        #region INSERT
        public static AstoriaRequest BuildInsert(Workspace workspace, ResourceContainer container, ResourceType type)
        {
            return BuildInsert(workspace, container, type, HttpStatusCode.Created, SerializationFormatKind.Default);
        }

        public static AstoriaRequest BuildInsert(Workspace workspace, ResourceContainer container, ResourceType type,
            HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            KeyExpression createdKey;
            return BuildInsert(workspace, container, type, expectedStatusCode, format, out createdKey);
        }

        public static AstoriaRequest BuildInsert(Workspace workspace, ResourceContainer container, ResourceType type,
            HttpStatusCode expectedStatusCode, SerializationFormatKind format, out KeyExpression createdKey)
        {
            KeyedResourceInstance newResource = type.CreateRandomResource(container);
            if (newResource == null)
            {
                newResource = ResourceInstanceUtil.CreateKeyedResourceInstanceByClone(container, type);

                if(newResource == null)
                {
                    createdKey = null;
                    return null;
                }
            }

            QueryNode query;
            if (!type.Key.Properties.Any(p => p.Facets.ServerGenerated) && newResource.ResourceInstanceKey != null)
            {
                createdKey = newResource.ResourceInstanceKey.CreateKeyExpression(container, type);
                query = ContainmentUtil.BuildCanonicalQuery(createdKey, true);
            }
            else
            {
                createdKey = null;
                // the key is unknown, must be server generated
                // in this case, lets hope that containment is a non-issue
                query =
                    Query.From(Exp.Variable(container))
                    .Select();
                if (!container.Facets.TopLevelAccess && expectedStatusCode == HttpStatusCode.Created)
                    expectedStatusCode = HttpStatusCode.BadRequest;
            }

            AstoriaRequest request = workspace.CreateRequest();

            request.Verb = RequestVerb.Post;
            request.Query = query;
            request.UpdateTree = newResource;
            request.ExpectedStatusCode = expectedStatusCode;
            request.Format = format;

            return request;
        }
        #endregion

        #region DELETE
        public static AstoriaRequest BuildDelete(Workspace workspace, KeyExpression toDelete, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            QueryNode query = ContainmentUtil.BuildCanonicalQuery(toDelete);

            AstoriaRequest request = workspace.CreateRequest();

            request.Verb = RequestVerb.Delete;
            request.Query = query;
            request.Format = format;
            request.ExpectedStatusCode = expectedStatusCode;

            if (toDelete.ResourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
                request.Headers[ConcurrencyUtil.IfMatchHeader] = toDelete.ETag;

            return request;
        }

        public static AstoriaRequest BuildDelete(Workspace workspace, KeyExpressions existingKeys, HttpStatusCode expectedStatusCode, SerializationFormatKind format, out KeyExpression deletedKey)
        {
            deletedKey = existingKeys.Choose();
            if (deletedKey == null)
            {
                AstoriaTestLog.WriteLine("Cannot build DELETE request, no existing keys");
                return null;
            }
            existingKeys.Remove(deletedKey);

            return BuildDelete(workspace, deletedKey, expectedStatusCode, format);
        }

        public static AstoriaRequest BuildDelete(Workspace workspace, KeyExpressions existingKeys, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            KeyExpression deletedKey;
            return BuildDelete(workspace, existingKeys, expectedStatusCode, format, out deletedKey);
        }

        public static AstoriaRequest BuildDelete(Workspace workspace, KeyExpressions existingKeys, HttpStatusCode expectedStatusCode)
        {
            return BuildDelete(workspace, existingKeys, expectedStatusCode, SerializationFormatKind.Default);
        }
        #endregion

        #region UPDATE
        public static AstoriaRequest BuildUpdate(Workspace workspace, KeyExpression modifiedKey, bool replace, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            if (modifiedKey == null)
                return null;

            ResourceContainer container = modifiedKey.ResourceContainer;
            ResourceType resourceType = modifiedKey.ResourceType;

            if (replace && resourceType.Properties.Any(p => p.Facets.IsIdentity))
                return null;

            string keyString = UriQueryBuilder.CreateKeyString(modifiedKey, false);
            if (expectedStatusCode == HttpStatusCode.NoContent && (keyString.Contains("/") || keyString.Contains(Uri.EscapeDataString("/"))))
                expectedStatusCode = HttpStatusCode.BadRequest;

            QueryNode query = ContainmentUtil.BuildCanonicalQuery(modifiedKey);

            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();

            string[] propertiesToSkip;
            //Skip because setting the birthdate to a random Datetime won't work due to contraints
            //if (resourceType.Name == "Employees")
            //    propertiesToSkip = new string[] { "BirthDate" };
            ////Skipping because it has some weird constraint on it
            //else if (resourceType.Name == "Order_Details")
            //    propertiesToSkip = new string[] { "Discount" };
            //else
            //    propertiesToSkip = new string[] { };

            foreach (ResourceProperty resourceProperty in resourceType.Properties.OfType<ResourceProperty>()
                .Where(p => !p.IsNavigation
                    && p.PrimaryKey == null
                    && !p.Facets.IsIdentity))
                    //&& !p.IsComplexType
                    //&& !propertiesToSkip.Contains(p.Name)))
            {
                properties.Add(resourceProperty.CreateRandomResourceInstanceProperty());
            }

            if (!properties.Any())
                return null;

            KeyedResourceInstance resourceInstance = new KeyedResourceInstance(
                ResourceInstanceKey.ConstructResourceInstanceKey(modifiedKey),
                properties.ToArray());

            AstoriaRequest request = workspace.CreateRequest();

            request.Verb = replace ? RequestVerb.Put : RequestVerb.Patch;
            request.Query = query;
            request.UpdateTree = resourceInstance;
            request.ExpectedStatusCode = expectedStatusCode;
            request.Format = format;

            if (modifiedKey.ResourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
            {
                request.Headers[ConcurrencyUtil.IfMatchHeader] = modifiedKey.ETag;
                request.ETagHeaderExpected = true;
            }

            return request;
        }

        public static AstoriaRequest BuildUpdate(Workspace workspace, ResourceContainer container, ResourceType resourceType,
            bool replace, HttpStatusCode expectedStatusCode, SerializationFormatKind format, out KeyExpression modifiedKey)
        {
            modifiedKey = workspace.GetRandomExistingKey(container, resourceType);
            return BuildUpdate(workspace, modifiedKey, replace, expectedStatusCode, format);
        }

        public static AstoriaRequest BuildUpdate(Workspace workspace, ResourceContainer container, ResourceType resourceType,
            bool replace, HttpStatusCode expectedStatusCode, SerializationFormatKind format)
        {
            KeyExpression modifiedKey;
            return BuildUpdate(workspace, container, resourceType, replace, expectedStatusCode, format, out modifiedKey);
        }
        #endregion
        #endregion

        #region IsSafeOperation
        public static bool IsSafeOperation(RequestVerb verb, ResourceContainer container)
        {
            if (container is ServiceOperation)
            {
                ServiceOperation op = container as ServiceOperation;
                return verb == op.Verb && verb == RequestVerb.Get; //for now POST always means insert
            }

            if(container.Name == "Invoices" && (container.Workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr || container.Workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq))
                return false;

            switch (verb)
            {
                case RequestVerb.Delete:
                    if (container.Name.Equals("DataKey_Bit"))
                        return false;
                    if (container.Name.Equals("ComputerDetails"))
                        return false;
                    if (!container.ResourceTypes.All(type => IsSafeOperation(verb, container, type)))
                        return false;
                    break;

                case RequestVerb.Post:
                    if (container.Name.Equals("Shippers"))
                        return false;
                    if (container.Name.Equals("Employees"))
                        return false;
                    if (container.Name.Equals("Order_Details"))
                        return false;
                    if (container.Name.Equals("ProjectSet"))
                        return false;
                    if (container.Name.Equals("DataKey_Bit"))
                        return false;
                    if (container.Name.Equals("Vehicles"))
                        return false;
                    if (container.Name.Equals("People"))
                        return false;
                    if (container.Name.Equals("Computers"))
                        return false;
                    if (container.Name.Equals("Workers"))
                        return false;
                    if (!container.ResourceTypes.All(type => IsSafeOperation(verb, container, type)))
                        return false;
                    break;
            }

            return true;
        }

        public static bool IsSafeOperation(RequestVerb verb, ResourceContainer container, ResourceType type)
        {
            switch (verb)
            {
                case RequestVerb.Delete:
                    if (!type.IsInsertable)
                        return false;
                    if (type.Associations.Any())
                        return false;
                    if (type.Properties.Any(p => p.Facets.ConcurrencyModeFixed && p.Facets.IsStoreBlob))
                        return false;
                    return true;

                case RequestVerb.Patch:
                    if (type.Facets.AbstractType)
                        return false;
                    if (!type.IsInsertable)
                        return false;
                    if (type.IsAssociationEntity)
                        return false;
                    if (type.Properties.Any(p => p.Facets.ConcurrencyModeFixed && p.Facets.IsStoreBlob))
                        return false;
                    return true;

                case RequestVerb.Put:
                    if (type.Facets.AbstractType)
                        return false;
                    if (!type.IsInsertable)
                        return false;
                    if (type.IsAssociationEntity)
                        return false;
                    if (type.Properties.Any(p => p.Facets.IsIdentity))
                        return false;
                    if (type.Properties.Any(p => p.Facets.ConcurrencyModeFixed && p.Facets.IsStoreBlob))
                        return false;
                    return true;

                case RequestVerb.Post:
                    if (type.Facets.AbstractType)
                        return false;
                    if (!type.IsInsertable)
                        return false;
                    if (type.IsAssociationEntity)
                        return false;
                    if (type.IsChildRefEntity)
                        return false;
                    if (type.Associations.Any(ra => ra.Ends.Any(end => end.ResourceType.IsAssociationEntity)))
                        return false;
                    return true;
            }
            return true;
        }


        public static bool IsSafeLinkOperation(RequestVerb verb, ResourceContainer container, ResourceProperty property)
        {
            if (!property.IsNavigation)
                return false;

            if (property.Facets.UnsafeLinkOperations.Contains(verb))
                return false;

            if (verb == RequestVerb.Get)
                return true;

            // astoria-level check
            switch(verb)
            {
                case RequestVerb.Patch:
                case RequestVerb.Put:
                    if (property.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                        return false;
                    break;

                case RequestVerb.Post:
                    if (property.OtherAssociationEnd.Multiplicity != Multiplicity.Many)
                        return false;
                    break;

                case RequestVerb.Delete:
                    if (property.OtherAssociationEnd.Multiplicity == Multiplicity.One)
                        return false;
                    break;

                default:
                    return false;
            }

            //// EF/DB level check
            if (container.Workspace.Database != null)
            {
                // would changing this nav-prop affect any foreign keys
                List<NodeProperty> affectedProperties = new List<NodeProperty>();
                foreach (NodeProperty p in property.ResourceType.Properties)
                {
                    if (!p.ForeignKeys.Any())
                        continue;

                    ResourceType relatedType = (p.ForeignKeys.First().PrimaryKey.Properties.First() as ResourceProperty).ResourceType;
                    if (relatedType == property.OtherAssociationEnd.ResourceType)
                        affectedProperties.Add(p);
                }

                foreach (NodeProperty p in property.OtherAssociationEnd.ResourceType.Properties)
                {
                    if (!p.ForeignKeys.Any())
                        continue;

                    ResourceType relatedType = (p.ForeignKeys.First().PrimaryKey.Properties.First() as ResourceProperty).ResourceType;
                    if (relatedType == property.ResourceType)
                        affectedProperties.Add(p);
                }

                // can't change primary key by changing nav-prop
                if (affectedProperties.Any(p => p.PrimaryKey != null))
                    return false;

                if (verb == RequestVerb.Delete && affectedProperties.Any(p => !p.Facets.Nullable))
                    return false;

                if (container.Workspace.DataLayerProviderKind == DataLayerProviderKind.Edm && (verb == RequestVerb.Put || verb == RequestVerb.Patch))
                {
                    // EF: updating self-reference causes a null-reference in the mapping layer
                    if (property.Type is ResourceType && property.OtherAssociationEnd.ResourceType == property.ResourceType)
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region header utils
        public static void SetDefaultHost(AstoriaRequest request)
        {
            Uri rootUri = request.Workspace.ServiceRoot;
            string host = rootUri.Scheme + "://" + rootUri.Host;
            if (!rootUri.IsDefaultPort)
                host += ":" + rootUri.Port;

            request.Host = host;
        }

        public static bool ExpectEmptyHeaders(AstoriaResponse response)
        {
            // it seems that for all the 'real' verbs, XmlHttpRequest does not give headers back on 204's
            if (response.Request.Verb != RequestVerb.Patch
                && AstoriaTestProperties.Client == ClientEnum.XMLHTTP
                && response.ActualStatusCode == HttpStatusCode.NoContent)
                return true;
            return false;
        }

        public static string RandomizeContentTypeCapitalization(string contentType)
        {
            if (contentType == null || !Versioning.Server.BugFixed_ServiceRejectsUppercaseContentTypes)
                return contentType;

            // TODO: refactor to use System.Net.Mime.ContentType?
            if (contentType.Contains("boundary="))
            {
                string[] pieces = contentType.Split(new string[] { "boundary=" }, StringSplitOptions.RemoveEmptyEntries);
                if (pieces.Length > 1)
                    return RandomizeContentTypeCapitalization(pieces[0] + "boundary=") + pieces[1];
                else //fall through
                    contentType = pieces[0] + "boundary=";
            }

            string lower = contentType.ToLowerInvariant();
            string upper = contentType.ToUpperInvariant();

            // if this happens, just bail since the universe has stopped making sense
            if (lower.Length != upper.Length)
                return contentType;

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < lower.Length; i++)
            {
                if (AstoriaTestProperties.Random.Next(2) % 2 == 0)
                    result.Append(lower[i]);
                else
                    result.Append(upper[i]);
            }

            return result.ToString();
        }
        #endregion
    }
}
