//---------------------------------------------------------------------
// <copyright file="RelationshipLinkVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A class for response relationship link verifier.
    /// </summary>
    public class RelationshipLinkVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private EntityModelSchema model;

        /// <summary>
        /// Initializes a new instance of the RelationshipLinkVerifier class.
        /// </summary>
        /// <param name="model">The conceptual model of the service</param>
        public RelationshipLinkVerifier(EntityModelSchema model)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            this.model = model;
        }

        /// <summary>
        /// Returns true for all requests rather than trying to guess which requests have responses with links in the payload
        /// </summary>
        /// <param name="request">The request that was issued</param>
        /// <returns>Whether or not this verifier applies to responses for this kind of request</returns>
        public bool Applies(ODataRequest request)
        {
            // filter based only on the response
            return true;
        }

        /// <summary>
        /// Return true for all Atom+Xml responses
        /// </summary>
        /// <param name="response">The response that might need to be verified</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return response.StatusCode != HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Verifies that any navigation or relationship/association links in the payload are correct.
        /// </summary>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            // Do not need to verify relationship links in metadata response. Skip it to avoid NullReferenceException in GetExpectedEntitySet call
            // TODO: Make the Json Light decision based on the request URI. Skip for now, since the links are not serialized by default.
            if (request.Uri.IsMetadata() || this.IsJsonLightResponse(response))
            {
                return;
            }

            // gather up all the entities to verify
            List<EntityInstance> entities = new List<EntityInstance>();
            if (response.RootElement.ElementType == ODataPayloadElementType.EntityInstance)
            {
                entities.Add(response.RootElement as EntityInstance);
            }
            else if (response.RootElement.ElementType == ODataPayloadElementType.EntitySetInstance)
            {
                foreach (EntityInstance ei in response.RootElement as EntitySetInstance)
                {
                    entities.Add(ei as EntityInstance);
                }
            }

            this.VerifyEntityLinks(request, response, entities);
        }

        /// <summary>
        /// Verifies that any navigation or relationship/association links in the entities are correct.
        /// </summary>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        /// <param name="entities">List of entities to verify links</param>
        private void VerifyEntityLinks(ODataRequest request, ODataResponse response, List<EntityInstance> entities)
        {
            // service behavior for telling whether or not the links should be there
            var serviceBehavior = this.model.GetDefaultEntityContainer().GetDataServiceBehavior();

            foreach (var entity in entities)
            {
                foreach (var navigation in entity.Properties.OfType<NavigationPropertyInstance>())
                {
                    string expectedNavigationUri = this.BuildExpectedNavigationUri(entity, navigation);
                    
                    // verify navigation link
                    if (navigation.Value.ElementType == ODataPayloadElementType.DeferredLink)
                    {
                        this.VerifyNavigationLink(navigation, expectedNavigationUri, request, response);
                    }
                    else
                    {
                        this.VerifyNavigationExpandedLink(navigation, expectedNavigationUri, request, response);
                    }

                    // verify relationship link
                    var associationLink = navigation.AssociationLink;
                    string expectedAssociationUri = this.BuildExpectedAssociationUri(entity, navigation);
                    if (serviceBehavior.IncludeAssociationLinksInResponse == true)
                    {
                        this.VerifyAssociationLink(navigation, associationLink, expectedAssociationUri, request, response);
                    }
                    else if (associationLink != null)
                    {
                        this.Logger.WriteLine(LogLevel.Verbose, "Property '{0}' had association link '{1}'", navigation.Name, associationLink);
                        this.Logger.WriteLine(LogLevel.Error, "Association link unexpectedly non-null for current service configuration");
                        this.ReportFailure(request, response);
                        throw new ResponseVerificationException();
                    }
                }
            }
        }

        /// <summary>
        /// Construct expected navigation link value.
        /// </summary>
        /// <param name="entity">entity that the link belongs to</param>
        /// <param name="navigation">navigation property instance for the navigation link</param>
        /// <returns>expected navigation link value</returns>
        private string BuildExpectedNavigationUri(EntityInstance entity, NavigationPropertyInstance navigation)
        {
            return UriHelpers.ConcatenateUriSegments(entity.GetEditLink(), Uri.EscapeDataString(navigation.Name));
        }

        /// <summary>
        /// Construct expected relationship link value.
        /// </summary>
        /// <param name="entity">entity that the link belongs to</param>
        /// <param name="navigation">navigation property instance for the relationship link</param>
        /// <returns>expected relationship link value</returns>
        private string BuildExpectedAssociationUri(EntityInstance entity, NavigationPropertyInstance navigation)
        {
            return UriHelpers.ConcatenateUriSegments(entity.GetEditLink(), Endpoints.Ref, Uri.EscapeDataString(navigation.Name));
        }

        /// <summary>
        /// Verify the relationship link is as expected.
        /// </summary>
        /// <param name="navigation">navigation property instance for the navigation link</param>
        /// <param name="associationLink">relationship link to verify</param>
        /// <param name="expectedAssociationUri">expected link value</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response to check the content type.</param>
        private void VerifyAssociationLink(NavigationPropertyInstance navigation, DeferredLink associationLink, string expectedAssociationUri, ODataRequest request, ODataResponse response)
        {
            bool linkShouldBeNull = false;
            if (!this.IsAtomResponse(response) && navigation.IsExpanded)
            {
                var expandedValue = ((ExpandedLink)navigation.Value).ExpandedElement;
                linkShouldBeNull = expandedValue == null;
            }

            if (linkShouldBeNull)
            {   
                // json navigations are null, then there are no links
                if (associationLink != null)
                {
                    this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Expected association link == null, observed '{0}'", associationLink.UriString);
                    this.ReportFailure(request, response);
                    throw new ResponseVerificationException();
                }
            }
            else
            {
                this.VerifyLinkUriValue(associationLink.UriString, expectedAssociationUri, request, response);

                // extra verifications for atom payload
                if (this.IsAtomResponse(response))
                {
                    this.VerifyAtomAssociationLinkTypeAttribute(associationLink, request, response);
                    this.VerifyAtomTitleAttribute(navigation, associationLink, request, response);
                }
            }
        }

        /// <summary>
        /// Verify the navigation link is as expected.
        /// </summary>
        /// <param name="navigation">navigation link to verify</param>
        /// <param name="expectedNavigationUri">expected link value</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response to check the content type.</param>
        private void VerifyNavigationLink(NavigationPropertyInstance navigation, string expectedNavigationUri, ODataRequest request, ODataResponse response)
        {
            if (navigation.IsExpanded)
            {
                this.VerifyLinkUriValue(((ExpandedLink)navigation.Value).UriString, expectedNavigationUri, request, response);               
            }
            else
            {
                this.VerifyLinkUriValue(((DeferredLink)navigation.Value).UriString, expectedNavigationUri, request, response);
            }

            // extra verifications for atom payload
            if (this.IsAtomResponse(response))
            {
                this.VerifyAtomNavigationLinkTypeAttribute(navigation.Value, request, response);
                this.VerifyAtomTitleAttribute(navigation, navigation.Value, request, response);
            }
        }

        /// <summary>
        /// Verify the expanded navigation link is as expected.
        /// </summary>
        /// <param name="navigation">expanded navigation link to verify</param>
        /// <param name="expectedNavigationUri">expected link value</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response to check the content type.</param>
        private void VerifyNavigationExpandedLink(NavigationPropertyInstance navigation, string expectedNavigationUri, ODataRequest request, ODataResponse response)
        {
            if (this.IsAtomResponse(response))
            {
                this.VerifyNavigationLink(navigation, expectedNavigationUri, request, response);
            }
            else
            {
                // JSON never has navigation links for expanded elements
                string expandedLinkUri = ((ExpandedLink)navigation.Value).UriString;
                if (expandedLinkUri != null)
                {
                    this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Expected expanded link uri == null, observed {0}", expandedLinkUri);
                    this.ReportFailure(request, response);
                    throw new ResponseVerificationException();
                }
            }

            List<EntityInstance> expandedEntities = new List<EntityInstance>();
            ODataPayloadElement expandedElement = ((ExpandedLink)navigation.Value).ExpandedElement;
            if (expandedElement != null)
            {
                EntityInstance entityInstance = expandedElement as EntityInstance;
                if (entityInstance != null)
                {
                    expandedEntities.Add(entityInstance);
                }
                else
                {
                    EntitySetInstance entitySetInstance = expandedElement as EntitySetInstance;

                    foreach (EntityInstance ei in entitySetInstance)
                    {
                        expandedEntities.Add(ei);
                    }
                }

                this.VerifyEntityLinks(request, response, expandedEntities);
            }
        }

        /// <summary>
        /// Returns true if response content is Atom feed/entry.
        /// </summary>
        /// <param name="response">The response to check the content type.</param>
        /// <returns>True if the content type is Atom, otherwise False.</returns>
        private bool IsAtomResponse(ODataResponse response)
        {
            return response.Headers[HttpHeaders.ContentType].StartsWith(MimeTypes.ApplicationAtomXml, StringComparison.CurrentCulture);
        }    

        /// <summary>
        /// Returns true if response content is Json Light format.
        /// </summary>
        /// <param name="response">The response to check the content type.</param>
        /// <returns>True if the content type is Json Light, otherwise False.</returns>
        private bool IsJsonLightResponse(ODataResponse response)
        {
            string contentType = response.Headers[HttpHeaders.ContentType];
            return contentType.StartsWith(MimeTypes.ApplicationJsonODataLightNonStreaming, StringComparison.CurrentCulture) ||
                   contentType.StartsWith(MimeTypes.ApplicationJsonODataLightStreaming, StringComparison.CurrentCulture);
        }    
        
        /// <summary>
        /// Verify that the navigation/relationship link value is consistent with expected value.
        /// </summary>
        /// <param name="link">The navigation/relationship link to verify.</param>
        /// <param name="expectedLinkValue">The expected link.</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response needed for error report.</param>
        private void VerifyLinkUriValue(string link, string expectedLinkValue, ODataRequest request, ODataResponse response)
        {
            if (link == null ||
                !link.Equals(expectedLinkValue, StringComparison.Ordinal))
            {
                this.Logger.WriteLine(LogLevel.Error, CultureInfo.InvariantCulture, "Expected link {0}, observed {1}", expectedLinkValue, link == null ? "Link not Found." : link);
                this.ReportFailure(request, response);
                throw new ResponseVerificationException();
            }
        }

        /// <summary>
        /// Verify that the type attribute of navigation link is not null and has the expected value.
        /// </summary>
        /// <param name="link">The relationship link to verify.</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response needed for error report.</param>
        private void VerifyAtomNavigationLinkTypeAttribute(ODataPayloadElement link, ODataRequest request, ODataResponse response)
        {
            ContentTypeAnnotation typeAnnotation = link.Annotations.OfType<ContentTypeAnnotation>().SingleOrDefault();

            if (typeAnnotation == null ||
                (!typeAnnotation.Value.Equals("application/atom+xml;type=feed", StringComparison.Ordinal) &&
                !typeAnnotation.Value.Equals("application/atom+xml;type=entry", StringComparison.Ordinal)))
            {
                this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Expected navigation link type attribute application/atom+xml;type=feed or application/atom+xml;type=entry, observed '{0}'", typeAnnotation == null ? "null" : typeAnnotation.Value);
                this.ReportFailure(request, response);
                throw new ResponseVerificationException();
            }
        }

        /// <summary>
        /// Verify that the type attribute of relationship link is not null and has the expected value "application/xml".
        /// </summary>
        /// <param name="link">The navigation link to verify.</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response needed for error report.</param>
        private void VerifyAtomAssociationLinkTypeAttribute(ODataPayloadElement link, ODataRequest request, ODataResponse response)
        {
            ContentTypeAnnotation typeAnnotation = link.Annotations.OfType<ContentTypeAnnotation>().SingleOrDefault();
            string expectedTypeAttributeValue = "application/xml";

            if (typeAnnotation == null || string.Compare(typeAnnotation.Value, expectedTypeAttributeValue, StringComparison.CurrentCulture) != 0)
            {
                this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Expected relationship link type attribute '{0}', observed '{1}'", expectedTypeAttributeValue, typeAnnotation == null ? "null" : typeAnnotation.Value);
                this.ReportFailure(request, response);
                throw new ResponseVerificationException();
            }
        }

        /// <summary>
        /// Verify that the title attribute of navigation/relationship link is not null and has the value consistent with the navigation property name.
        /// </summary>
        /// <param name="navigation">navigation property instance for the navigation link</param>
        /// <param name="link">The navigation/relationship link to verify.</param>
        /// <param name="request">The request needed for error report.</param>
        /// <param name="response">The response needed for error report.</param>
        private void VerifyAtomTitleAttribute(NavigationPropertyInstance navigation, ODataPayloadElement link, ODataRequest request, ODataResponse response)
        {
            TitleAnnotation titleAnnotation = link.Annotations.OfType<TitleAnnotation>().SingleOrDefault();
            if (titleAnnotation == null || string.Compare(titleAnnotation.Value, navigation.Name, StringComparison.CurrentCulture) != 0)
            {
                this.Logger.WriteLine(LogLevel.Verbose, CultureInfo.InvariantCulture, "Expected navigation property Title attribute '{0}', observed '{1}'", navigation.Name, titleAnnotation == null ? "null" : titleAnnotation.Value);
                this.ReportFailure(request, response);
                throw new ResponseVerificationException();
            }
        }
    }
}
