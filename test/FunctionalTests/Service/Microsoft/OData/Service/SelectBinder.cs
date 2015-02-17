//---------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Query.SemanticAst;

    /// <summary>
    /// Temporary simple representation of a metadata-bound $select segment. Will be replaced with <see cref="Microsoft.OData.Core.Query.SemanticAst.Selection"/> when integration is complete.
    /// </summary>
    internal static class SelectBinder
    {
        /// <summary>
        /// Binds the paths from the request's $select query option to the sets/types/properties from the metadata provider of the service.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        /// <param name="dataService">The data service.</param>
        /// <param name="selectQueryOption">The raw value of the $select query option.</param>
        /// <returns>The bound select segments.</returns>
        internal static IList<IList<SelectItem>> BindSelectSegments(RequestDescription requestDescription, IDataService dataService, string selectQueryOption)
        {
            Debug.Assert(requestDescription != null, "requestDescription != null");
            Debug.Assert(dataService != null, "dataService != null");

            if (string.IsNullOrEmpty(selectQueryOption))
            {
                return new List<IList<SelectItem>>();
            }

            // Throw if $select requests have been disabled by the user
            Debug.Assert(dataService.Configuration != null, "dataService.Configuration != null");
            if (!dataService.Configuration.DataServiceBehavior.AcceptProjectionRequests)
            {
                throw DataServiceException.CreateBadRequestError(Strings.DataServiceConfiguration_ProjectionsNotAccepted);
            }

            IList<IList<string>> selectPathsAsText = SplitSelect(selectQueryOption, dataService.Provider);
            Debug.Assert(selectPathsAsText != null, "selectPathsAsText != null");

            List<IList<SelectItem>> boundSegments = new List<IList<SelectItem>>(selectPathsAsText.Count);
            if (selectPathsAsText.Count == 0)
            {
                return boundSegments;
            }

            ValidateSelectIsAllowedForRequest(requestDescription);

            MetadataProviderEdmModel metadataProviderEdmModel = dataService.Provider.GetMetadataProviderEdmModel();

            for (int i = selectPathsAsText.Count - 1; i >= 0; i--)
            {
                IList<string> path = selectPathsAsText[i];
                List<SelectItem> boundSegmentPath = new List<SelectItem>(path.Count);
                boundSegments.Add(boundSegmentPath);

                ResourceType targetResourceType = requestDescription.TargetResourceType;
                ResourceSetWrapper targetResourceSet = requestDescription.TargetResourceSet;

                // if we get to here, we're building a partial selection
                List<TypeSegment> typeSegments = new List<TypeSegment>();
                bool previousSegmentIsTypeSegment = false;
                for (int j = 0; j < path.Count; j++)
                {
                    string pathSegment = path[j];
                    bool lastPathSegment = (j == path.Count - 1);

                    // '*' is special, it means "Project all immediate properties on this level."
                    if (pathSegment == "*")
                    {
                        Debug.Assert(lastPathSegment, "A wildcard select segment must be the last one. This should have been checked already when splitting appart the paths.");
                        boundSegmentPath.Add(CreateWildcardSelection());
                        continue;
                    }

                    bool nameIsContainerQualified;
                    string nameFromContainerQualifiedName = dataService.Provider.GetNameFromContainerQualifiedName(pathSegment, out nameIsContainerQualified);
                    if (nameFromContainerQualifiedName == "*")
                    {
                        Debug.Assert(lastPathSegment, "A wildcard select segment must be the last one.  This should have been checked already when splitting appart the paths.");
                        Debug.Assert(nameIsContainerQualified, "nameIsContainerQualified == true");
                        boundSegmentPath.Add(CreateContainerQualifiedWildcardSelection(metadataProviderEdmModel));
                        continue;
                    }

                    ResourceProperty property = targetResourceType.TryResolvePropertyName(pathSegment);
                    if (property == null)
                    {
                        ResourceType resolvedResourceType = WebUtil.ResolveTypeIdentifier(dataService.Provider, pathSegment, targetResourceType, previousSegmentIsTypeSegment);
                        if (resolvedResourceType != null)
                        {
                            previousSegmentIsTypeSegment = true;
                            if (lastPathSegment)
                            {
                                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier(XmlConstants.HttpQueryStringSelect, resolvedResourceType.FullName));
                            }

                            targetResourceType = resolvedResourceType;

                            // Whenever we encounter the type segment, we need to only verify that the MPV is set to 3.0 or higher.
                            // There is no need to check for request DSV, request MinDSV or request MaxDSV since there are no protocol changes in
                            // the payload for uri's with type identifier.
                            requestDescription.VerifyProtocolVersion(VersionUtil.Version3Dot0, dataService);

                            IEdmSchemaType edmType = metadataProviderEdmModel.EnsureSchemaType(targetResourceType);
                            TypeSegment typeSegment = new TypeSegment(edmType);
                            typeSegments.Add(typeSegment);
                            continue;
                        }

                        previousSegmentIsTypeSegment = false;

                        // If the currentResourceType is an open type, we require the service action name to be fully qualified or else we treat it as an open property name.
                        if (!targetResourceType.IsOpenType || nameIsContainerQualified)
                        {
                            // Note that if the service does not implement IDataServiceActionProvider and the MaxProtocolVersion < V3, 
                            // GetActionsBoundToAnyTypeInResourceSet() would simply return an empty ServiceOperationWrapper collection.
                            Debug.Assert(dataService.ActionProvider != null, "dataService.ActionProvider != null");
                            IEnumerable<OperationWrapper> allOperationsInSet = dataService.ActionProvider.GetActionsBoundToAnyTypeInResourceSet(targetResourceSet);
                            List<OperationWrapper> selectedServiceActions = allOperationsInSet.Where(o => o.Name == nameFromContainerQualifiedName).ToList();

                            if (selectedServiceActions.Count > 0)
                            {
                                if (!lastPathSegment)
                                {
                                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_ServiceActionMustBeLastSegmentInSelect(pathSegment));
                                }

                                boundSegmentPath.Add(CreateOperationSelection(metadataProviderEdmModel, selectedServiceActions, typeSegments));
                                continue;
                            }
                        }

                        if (!targetResourceType.IsOpenType)
                        {
                            throw DataServiceException.CreateSyntaxError(Strings.RequestUriProcessor_PropertyNotFound(targetResourceType.FullName, pathSegment));
                        }

                        if (!lastPathSegment)
                        {
                            // Open navigation properties are not supported on OpenTypes.
                            throw DataServiceException.CreateBadRequestError(Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(pathSegment));
                        }

                        boundSegmentPath.Add(CreateOpenPropertySelection(pathSegment, typeSegments));
                    }
                    else
                    {
                        previousSegmentIsTypeSegment = false;

                        ValidateSelectedProperty(targetResourceType, property, lastPathSegment);

                        boundSegmentPath.Add(CreatePropertySelection(metadataProviderEdmModel, targetResourceType, property, typeSegments));

                        if (property.TypeKind == ResourceTypeKind.EntityType)
                        {
                            targetResourceSet = dataService.Provider.GetResourceSet(targetResourceSet, targetResourceType, property);
                            targetResourceType = property.ResourceType;
                        }
                    }
                }

                // Note that this check is also covering cases where a type segment is followed by a wildcard.
                if (previousSegmentIsTypeSegment)
                {
                    throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier(XmlConstants.HttpQueryStringSelect, targetResourceType.FullName));
                }
            }

            return boundSegments;
        }
        
        /// <summary>Reads a $select clause.</summary>
        /// <param name="value">Value to read.</param>
        /// <param name="dataServiceProviderWrapper">The provider wrapper for the service.</param>
        /// <returns>A list of paths, each of which is a list of identifiers.</returns>
        private static IList<IList<string>> SplitSelect(string value, DataServiceProviderWrapper dataServiceProviderWrapper)
        {
            Debug.Assert(!String.IsNullOrEmpty(value), "!String.IsNullOrEmpty(value)");

            List<IList<string>> result = new List<IList<string>>();
            List<string> currentPath = null;
            ExpressionLexer lexer = new ExpressionLexer(value);
            while (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                string identifier;
                bool lastSegment = false;
                if (lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
                {
                    identifier = lexer.CurrentToken.Text;
                    lexer.NextToken();
                    lastSegment = true;
                }
                else
                {
                    identifier = lexer.ReadDottedIdentifier(true /*allowEndWithDotStar*/);
                    bool nameIsContainerQualifed;
                    if (dataServiceProviderWrapper.GetNameFromContainerQualifiedName(identifier, out nameIsContainerQualifed) == "*")
                    {
                        lastSegment = true;
                    }
                }

                if (currentPath == null)
                {
                    currentPath = new List<string>();
                    result.Add(currentPath);
                }

                currentPath.Add(identifier);

                // Check whether we're at the end, whether we're drilling in,
                // or whether we're finishing with this path.
                ExpressionTokenKind tokenId = lexer.CurrentToken.Kind;
                if (tokenId != ExpressionTokenKind.End)
                {
                    if (lastSegment || tokenId != ExpressionTokenKind.Slash)
                    {
                        lexer.ValidateToken(ExpressionTokenKind.Comma);
                        currentPath = null;
                    }

                    lexer.NextToken();
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an instance of <see cref="ODataPath"/> with the given segments.
        /// </summary>
        /// <param name="typeSegments">The type segments seen so far. Will be cleared once they are used in the path.</param>
        /// <param name="lastSegment">The last segment for the path.</param>
        /// <returns>The newly created path.</returns>
        private static ODataPath CreatePath(ICollection<TypeSegment> typeSegments, Segment lastSegment)
        {
            Debug.Assert(typeSegments != null, "typeSegments != null");
            Debug.Assert(lastSegment != null, "lastSegment != null");

            if (typeSegments.Count > 0)
            {
                var path = new ODataPath(typeSegments.Concat(new[] { lastSegment }));
                typeSegments.Clear();
                return path;
            }

            return new ODataPath(lastSegment);
        }

        /// <summary>
        /// Creates an instance of <see cref="SelectItem"/> to represent the selection of the given property.
        /// </summary>
        /// <param name="metadataProviderEdmModel">The metadata provider-based edm model.</param>
        /// <param name="targetResourceType">The resource type the property is being selected for.</param>
        /// <param name="property">The property being selected.</param>
        /// <param name="typeSegments">Type segments seen in the path so far.</param>
        /// <returns>A new <see cref="SelectItem"/> to represent the selection of the given property.</returns>
        private static SelectItem CreatePropertySelection(MetadataProviderEdmModel metadataProviderEdmModel, ResourceType targetResourceType, ResourceProperty property, ICollection<TypeSegment> typeSegments)
        {
            var structuredType = (IEdmStructuredType)metadataProviderEdmModel.EnsureSchemaType(targetResourceType);
            IEdmProperty edmProperty = structuredType.FindProperty(property.Name);
            var edmStructuralProperty = edmProperty as IEdmStructuralProperty;

            Segment lastSegment;
            if (edmStructuralProperty != null)
            {
                lastSegment = new PropertySegment(edmStructuralProperty);
            }
            else
            {
                lastSegment = new NavigationPropertySegment((IEdmNavigationProperty)edmProperty);
            }

            var path = CreatePath(typeSegments, lastSegment);
            return new PathSelectItem(path);
        }

        /// <summary>
        /// Creates an instance of <see cref="SelectItem"/> to represent the selection of an open property.
        /// </summary>
        /// <param name="propertyName">The name of the open property being selected.</param>
        /// <param name="typeSegments">Type segments seen in the path so far.</param>
        /// <returns>A new <see cref="SelectItem"/> to represent the selection of the open property.</returns>
        private static SelectItem CreateOpenPropertySelection(string propertyName, ICollection<TypeSegment> typeSegments)
        {
            return new PathSelectItem(CreatePath(typeSegments, new OpenPropertySegment(propertyName)));
        }

        /// <summary>
        /// Creates an instance of <see cref="SelectItem"/> to represent the selection of an set of operations.
        /// </summary>
        /// <param name="metadataProviderEdmModel">The metadata provider-based edm model.</param>
        /// <param name="selectedServiceActions">The operations being selected.</param>
        /// <param name="typeSegments">Type segments seen in the path so far.</param>
        /// <returns>A new <see cref="SelectItem"/> to represent the selection of the operations.</returns>
        private static SelectItem CreateOperationSelection(MetadataProviderEdmModel metadataProviderEdmModel, IEnumerable<OperationWrapper> selectedServiceActions, ICollection<TypeSegment> typeSegments)
        {
            return new PathSelectItem(CreatePath(typeSegments, new OperationSegment(selectedServiceActions.Select(a => metadataProviderEdmModel.EnsureDefaultEntityContainer().EnsureFunctionImport(a)).ToList())));
        }

        /// <summary>
        /// Creates a new instance of <see cref="WildcardSelectItem"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="WildcardSelectItem"/>.</returns>
        private static WildcardSelectItem CreateWildcardSelection()
        {
            return new WildcardSelectItem();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ContainerQualifiedWildcardSelectItem"/>.
        /// </summary>
        /// <param name="metadataProviderEdmModel">The metadata provider-based edm model.</param>
        /// <returns>A new instance of <see cref="ContainerQualifiedWildcardSelectItem"/>.</returns>
        private static ContainerQualifiedWildcardSelectItem CreateContainerQualifiedWildcardSelection(MetadataProviderEdmModel metadataProviderEdmModel)
        {
            return new ContainerQualifiedWildcardSelectItem(metadataProviderEdmModel.EnsureDefaultEntityContainer());
        }

        /// <summary>
        /// Throws exceptions if the $select query option cannot be specified on this request.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        private static void ValidateSelectIsAllowedForRequest(RequestDescription requestDescription)
        {
            // We only allow $select on entity/entityset queries. Queries which return a primitive/complex value don't support $select.
            if (requestDescription.TargetResourceType == null || (requestDescription.TargetResourceType.ResourceTypeKind != ResourceTypeKind.EntityType))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }

            // $select can't be used on $links URIs as it doesn't make sense
            if (requestDescription.SegmentInfos.Any(si => si.TargetKind == RequestTargetKind.Link))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }
        }

        /// <summary>
        /// Validates that the given property can be selected.
        /// </summary>
        /// <param name="currentResourceType">The current resource type.</param>
        /// <param name="property">The property to validate.</param>
        /// <param name="lastPathSegment">Whether the property is the last path segment.</param>
        private static void ValidateSelectedProperty(ResourceType currentResourceType, ResourceProperty property, bool lastPathSegment)
        {
            Debug.Assert(property != null, "property != null");
            switch (property.TypeKind)
            {
                case ResourceTypeKind.Primitive:
                    if (!lastPathSegment)
                    {
                        if (property.IsOfKind(ResourcePropertyKind.Stream))
                        {
                            // Cannot compose after named stream
                            throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_NamedStreamMustBeLastSegmentInSelect(property.Name));
                        }

                        throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_PrimitivePropertyUsedAsNavigationProperty(currentResourceType.FullName, property.Name));
                    }

                    break;

                case ResourceTypeKind.ComplexType:
                    if (!lastPathSegment)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_ComplexPropertyAsInnerSelectSegment(currentResourceType.FullName, property.Name));
                    }

                    break;

                case ResourceTypeKind.Collection:
                    if (!lastPathSegment)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.RequestQueryProcessor_CollectionPropertyAsInnerSelectSegment(currentResourceType.FullName, property.Name));
                    }

                    break;

                case ResourceTypeKind.EntityType:
                    break;

                default:
                    Debug.Fail("Unexpected property type.");
                    break;
            }
        }
    }
}