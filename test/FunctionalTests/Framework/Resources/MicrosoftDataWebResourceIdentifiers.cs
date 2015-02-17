//---------------------------------------------------------------------
// <copyright file="MicrosoftDataWebResourceIdentifiers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class SystemDataServicesResourceIdentifiers
    {
        public static List<ResourceIdentifier> ResourceIdentifiers;

        public static ResourceIdentifier Create(string id, Type expectedException)
        {
            if (ResourceIdentifiers == null)
                ResourceIdentifiers = new List<ResourceIdentifier>();
#if !ClientSKUFramework

            ResourceIdentifier resourceIdentifier = new ResourceIdentifier(typeof(Microsoft.OData.Service.DataServiceHost).Assembly, id, ComparisonFlag.Full, expectedException);
            ResourceIdentifiers.Add(resourceIdentifier);

            return resourceIdentifier;
#endif
#if ClientSKUFramework
	return null;
#endif
        }

        public static ResourceIdentifier Create(string id)
        {
            return Create(id, null);
        }

        // TODO: try to keep these organized
        // they should be alphabetical by prefix (ie, before the underscore), then by the remaining portion
        // those without a prefix come first
        public static ResourceIdentifier CannotCreateInstancesOfAbstractType = Create("CannotCreateInstancesOfAbstractType");

        public static ResourceIdentifier BadRequest_CannotNullifyValueTypeProperty = Create("BadRequest_CannotNullifyValueTypeProperty");
        public static ResourceIdentifier BadRequest_CannotSetCollectionsToNull = Create("BadRequest_CannotSetCollectionsToNull");
        public static ResourceIdentifier BadRequest_CannotUpdateRelatedEntitiesInPut = Create("BadRequest_CannotUpdateRelatedEntitiesInPut");
        public static ResourceIdentifier BadRequest_ErrorInConvertingPropertyValue = Create("BadRequest_ErrorInConvertingPropertyValue");
        public static ResourceIdentifier BadRequest_ErrorInSettingPropertyValue = Create("BadRequest_ErrorInSettingPropertyValue");
        public static ResourceIdentifier BadRequest_ExceedsMaxObjectCountOnInsert = Create("BadRequest_ExceedsMaxObjectCountOnInsert");
        public static ResourceIdentifier BadRequest_InvalidContentTypeForOpenProperty = Create("BadRequest_InvalidContentTypeForOpenProperty");
        public static ResourceIdentifier BadRequest_InvalidTypeName = Create("BadRequest_InvalidTypeName");
        public static ResourceIdentifier BadRequest_InvalidTypeSpecified = Create("BadRequest_InvalidTypeSpecified");
        public static ResourceIdentifier BadRequest_InvalidUriForDeleteOperation = Create("BadRequest_InvalidUriForDeleteOperation");
        public static ResourceIdentifier BadRequest_InvalidUriForPostOperation = Create("BadRequest_InvalidUriForPostOperation");
        public static ResourceIdentifier BadRequest_InvalidUriForPutOperation = Create("BadRequest_InvalidUriForPutOperation");
        public static ResourceIdentifier BadRequest_InvalidUriForMediaResource = Create("BadRequest_InvalidUriForMediaResource");
        public static ResourceIdentifier BadRequest_InvalidValue = Create("BadRequest_InvalidValue");
        public static ResourceIdentifier BadRequest_PropertyMustBeArray = Create("BadRequest_PropertyMustBeArray");
        public static ResourceIdentifier BadRequest_KeyCountMismatch = Create("BadRequest_KeyCountMismatch");
        public static ResourceIdentifier BadRequest_DeepRecursion = Create("BadRequest_DeepRecursion");
        
        public static ResourceIdentifier BadRequestStream_InvalidContent = Create("BadRequestStream_InvalidContent");
        public static ResourceIdentifier BadRequestStream_InvalidJsonUnrecognizedEscapeSequence = Create("BadRequestStream_InvalidJsonUnrecognizedEscapeSequence");
        public static ResourceIdentifier BadRequestStream_InvalidResourceEntity = Create("BadRequestStream_InvalidResourceEntity");
        public static ResourceIdentifier BadRequestStream_MissingTypeInformationForOpenTypeProperties = Create("BadRequestStream_MissingTypeInformationForOpenTypeProperties");

        public static ResourceIdentifier BadProvider_InvalidTypeSpecified = Create("BadProvider_InvalidTypeSpecified");
        
        public static ResourceIdentifier HttpContextServiceHost_UnknownQueryParameter = Create("HttpContextServiceHost_UnknownQueryParameter");

        public static ResourceIdentifier DataService_CannotPerformOperationWithoutETag = Create("DataService_CannotPerformOperationWithoutETag");
        public static ResourceIdentifier DataService_CannotUpdateSetReferenceLinks = Create("DataService_CannotUpdateSetReferenceLinks");
        public static ResourceIdentifier DataService_ContentIdMustBeAnInteger = Create("DataService_ContentIdMustBeAnInteger");
        public static ResourceIdentifier DataService_ETagCannotBeSpecified = Create("DataService_ETagCannotBeSpecified");
        public static ResourceIdentifier DataService_ETagNotSupportedInUnbind = Create("DataService_ETagNotSupportedInUnbind");
        public static ResourceIdentifier DataService_ETagSpecifiedForPost = Create("DataService_ETagSpecifiedForPost");
        public static ResourceIdentifier DataService_ETagValueNotValid = Create("DataService_ETagValueNotValid");
        public static ResourceIdentifier DataService_IfNoneMatchHeaderNotSupportedInDelete = Create("DataService_IfNoneMatchHeaderNotSupportedInDelete");
        public static ResourceIdentifier DataService_IfNoneMatchHeaderNotSupportedInPut = Create("DataService_IfNoneMatchHeaderNotSupportedInPut");
        public static ResourceIdentifier DataService_SDP_TopLevelPagedResultWithOldExpandProvider = Create("DataService_SDP_TopLevelPagedResultWithOldExpandProvider");
        public static ResourceIdentifier DataService_Projections_ProjectionsWithOldExpandProvider = Create("DataService_Projections_ProjectionsWithOldExpandProvider");
        public static ResourceIdentifier DataService_InvalidRequestVersion = Create("DataService_InvalidRequestVersion");
        public static ResourceIdentifier DataService_DSVTooLow = Create("DataService_DSVTooLow");
        public static ResourceIdentifier DataService_MaxDSVTooLow = Create("DataService_MaxDSVTooLow");
        public static ResourceIdentifier DataService_NotImplementedException = Create("DataService_NotImplementedException");

        public static ResourceIdentifier DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion = Create("DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion");
        
        public static ResourceIdentifier DataServiceException_GeneralError = Create("DataServiceException_GeneralError");
        public static ResourceIdentifier DataServiceException_UnsupportedMediaType = Create("DataServiceException_UnsupportedMediaType");

        public static ResourceIdentifier OpenNavigationPropertiesNotSupportedOnOpenTypes = Create("OpenNavigationPropertiesNotSupportedOnOpenTypes");

        public static ResourceIdentifier RequestUriProcessor_MethodNotAllowed = Create("RequestUriProcessor_MethodNotAllowed");
        public static ResourceIdentifier RequestUriProcessor_PropertyNotFound = Create("RequestUriProcessor_PropertyNotFound");
        public static ResourceIdentifier RequestUriProcessor_ResourceNotFound = Create("RequestUriProcessor_ResourceNotFound");
        public static ResourceIdentifier RequestUriProcessor_SyntaxError = Create("RequestUriProcessor_SyntaxError");
        public static ResourceIdentifier RequestUriProcessor_InvalidHttpMethodForNamedStream = Create("RequestUriProcessor_InvalidHttpMethodForNamedStream");
        
        public static ResourceIdentifier RequestQueryParser_ExpressionTypeMismatch = Create("RequestQueryParser_ExpressionTypeMismatch"); 
        public static ResourceIdentifier RequestQueryParser_InvalidCharacter = Create("RequestQueryParser_InvalidCharacter");

        public static ResourceIdentifier RequestQueryProcessor_QueryNoOptionsApplicable = Create("RequestQueryProcessor_QueryNoOptionsApplicable");
        public static ResourceIdentifier RequestQueryProcessor_QuerySetOptionsNotApplicable = Create("RequestQueryProcessor_QuerySetOptionsNotApplicable");
        
        public static ResourceIdentifier Serializer_ETagValueDoesNotMatch = Create("Serializer_ETagValueDoesNotMatch");
        public static ResourceIdentifier Serializer_NoETagPropertiesForType = Create("Serializer_NoETagPropertiesForType");
        public static ResourceIdentifier Serializer_LoopsNotAllowedInComplexTypes = Create("Serializer_LoopsNotAllowedInComplexTypes");
        public static ResourceIdentifier Serializer_ResultsExceedMax = Create("Serializer_ResultsExceedMax");

        public static ResourceIdentifier ResourceAssociationSet_MultipleAssociationSetsForTheSameAssociationTypeMustNotReferToSameEndSets
            = Create("ResourceAssociationSet_MultipleAssociationSetsForTheSameAssociationTypeMustNotReferToSameEndSets");

        public static ResourceIdentifier ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional
            = Create("ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional");

        public static ResourceIdentifier ResourceAssociationSet_BidirectionalAssociationMustReturnSameResourceAssociationSetFromBothEnd
            = Create("ResourceAssociationSet_BidirectionalAssociationMustReturnSameResourceAssociationSetFromBothEnd");

        public static ResourceIdentifier DataServiceProviderWrapper_MultipleEntitySetsWithSameName = Create("DataServiceProviderWrapper_MultipleEntitySetsWithSameName");
        public static ResourceIdentifier DataServiceProviderWrapper_MultipleResourceTypesWithSameName = Create("DataServiceProviderWrapper_MultipleResourceTypesWithSameName");
        public static ResourceIdentifier DataServiceProviderWrapper_MultipleServiceOperationsWithSameName = Create("DataServiceProviderWrapper_MultipleServiceOperationsWithSameName");
    }
}
