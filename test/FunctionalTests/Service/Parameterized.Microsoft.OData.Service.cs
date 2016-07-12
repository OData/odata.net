//---------------------------------------------------------------------
// <copyright file="Parameterized.Microsoft.OData.Service.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//
//      GENERATED FILE.  DO NOT MODIFY.
//
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings
    {
        /// <summary>
        /// A string like "The expression type {0} is not supported."
        /// </summary>
        internal static string ALinq_UnsupportedExpression(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ALinq_UnsupportedExpression, p0);
        }

        /// <summary>
        /// A string like "The value for the given enum type '{0}' is not valid. Please specify a valid enum value."
        /// </summary>
        internal static string InvalidEnumValue(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.InvalidEnumValue, p0);
        }

        /// <summary>
        /// A string like "Open navigation properties are not supported on OpenTypes. Property name: '{0}'."
        /// </summary>
        internal static string OpenNavigationPropertiesNotSupportedOnOpenTypes(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.OpenNavigationPropertiesNotSupportedOnOpenTypes, p0);
        }

        /// <summary>
        /// A string like "Type '{0}' has a method '{1}' which is a generic IEnumerable but is marked with a SingleResultAttribute. Only IQueryable methods support this attribute."
        /// </summary>
        internal static string BaseServiceProvider_IEnumerableAlwaysMultiple(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_IEnumerableAlwaysMultiple, p0, p1);
        }

        /// <summary>
        /// A string like "Overloading is not supported but type '{0}' has an overloaded method '{1}'."
        /// </summary>
        internal static string BaseServiceProvider_OverloadingNotSupported(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_OverloadingNotSupported, p0, p1);
        }

        /// <summary>
        /// A string like "Method '{0}' has a parameter '{1}' which is not an [in] parameter."
        /// </summary>
        internal static string BaseServiceProvider_ParameterNotIn(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_ParameterNotIn, p0, p1);
        }

        /// <summary>
        /// A string like "Method '{0}' has a parameter '{1}' of type '{2}' which is not supported for service operations. Only primitive types are supported as parameters."
        /// </summary>
        internal static string BaseServiceProvider_ParameterTypeNotSupported(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_ParameterTypeNotSupported, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Service operation '{0}' produces instances of type '{1}', but having a single entity set for that type is required."
        /// </summary>
        internal static string BaseServiceProvider_ServiceOperationMissingSingleEntitySet(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_ServiceOperationMissingSingleEntitySet, p0, p1);
        }

        /// <summary>
        /// A string like "Unable to load metadata for return type '{1}' of method '{0}'."
        /// </summary>
        internal static string BaseServiceProvider_UnsupportedReturnType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_UnsupportedReturnType, p0, p1);
        }

        /// <summary>
        /// A string like "The given resource type instance for the type '{0}' is not known to the metadata provider."
        /// </summary>
        internal static string BaseServiceProvider_UnknownResourceTypeInstance(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BaseServiceProvider_UnknownResourceTypeInstance, p0);
        }

        /// <summary>
        /// A string like "Description for expanded properties has not been initialized."
        /// </summary>
        internal static string BasicExpandProvider_ExpandedPropertiesNotInitialized
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BasicExpandProvider_ExpandedPropertiesNotInitialized);
            }
        }

        /// <summary>
        /// A string like "Description for projected properties has not been initialized."
        /// </summary>
        internal static string BasicExpandProvider_ProjectedPropertiesNotInitialized
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BasicExpandProvider_ProjectedPropertiesNotInitialized);
            }
        }

        /// <summary>
        /// A string like "$expand does not support '{0}' properties expanded simultaneously on the same segment."
        /// </summary>
        internal static string BasicExpandProvider_UnsupportedExpandBreadth(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BasicExpandProvider_UnsupportedExpandBreadth, p0);
        }

        /// <summary>
        /// A string like "$expand does not support '{0}' ordering expressions simultaneously on the top level type. Ordering expressions include top level $orderby expressions and the key columns in the top level entity type."
        /// </summary>
        internal static string BasicExpandProvider_SDP_UnsupportedOrderingExpressionBreadth(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BasicExpandProvider_SDP_UnsupportedOrderingExpressionBreadth, p0);
        }

        /// <summary>
        /// A string like "$expand does not support open properties."
        /// </summary>
        internal static string BasicExpandProvider_ExpandNotSupportedForOpenProperties
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BasicExpandProvider_ExpandNotSupportedForOpenProperties);
            }
        }

        /// <summary>
        /// A string like "The requested media type '{0}' is not compatible with the $callback query option."
        /// </summary>
        internal static string CallbackQueryOptionHandler_UnsupportedContentType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.CallbackQueryOptionHandler_UnsupportedContentType, p0);
        }

        /// <summary>
        /// A string like "$callback can only be specified on GET requests."
        /// </summary>
        internal static string CallbackQueryOptionHandler_GetRequestsOnly
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.CallbackQueryOptionHandler_GetRequestsOnly);
            }
        }

        /// <summary>
        /// A string like "The binding parameter for service action '{0}' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() is null. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type."
        /// </summary>
        internal static string DataServiceActionProviderWrapper_ServiceActionBindingParameterNull(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceActionProviderWrapper_ServiceActionBindingParameterNull, p0);
        }

        /// <summary>
        /// A string like "The service action '{0}' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() has a binding parameter of type '{1}' that is not bindable to the resource type '{2}'. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type."
        /// </summary>
        internal static string DataServiceActionProviderWrapper_ResourceTypeMustBeAssignableToBindingParameterResourceType(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceActionProviderWrapper_ResourceTypeMustBeAssignableToBindingParameterResourceType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The IDataServiceActionProvider.AdvertiseServiceAction() method must return a non-null value for the actionToSerialize parameter if it returns true."
        /// </summary>
        internal static string DataServiceActionProviderWrapper_AdvertiseServiceActionCannotReturnNullActionToSerialize
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceActionProviderWrapper_AdvertiseServiceActionCannotReturnNullActionToSerialize);
            }
        }

        /// <summary>
        /// A string like "The resource set '{0}' returned by the provider is not read-only. Please make sure that all the resource sets are set to read-only."
        /// </summary>
        internal static string DataServiceProviderWrapper_ResourceContainerNotReadonly(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ResourceContainerNotReadonly, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' returned by the provider is not read-only. Please make sure that all the types are set to read-only."
        /// </summary>
        internal static string DataServiceProviderWrapper_ResourceTypeNotReadonly(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ResourceTypeNotReadonly, p0);
        }

        /// <summary>
        /// A string like "The service operation '{0}' returned by the provider is not read-only. Please make sure that all the service operations are set to read-only."
        /// </summary>
        internal static string DataServiceProviderWrapper_ServiceOperationNotReadonly(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ServiceOperationNotReadonly, p0);
        }

        /// <summary>
        /// A string like "The value returned by IDataServiceMetadataProvider.ContainerNamespace must not be null or empty."
        /// </summary>
        internal static string DataServiceProviderWrapper_ContainerNamespaceMustNotBeNullOrEmpty
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ContainerNamespaceMustNotBeNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "The value returned by IDataServiceMetadataProvider.ContainerName must not be null or empty."
        /// </summary>
        internal static string DataServiceProviderWrapper_ContainerNameMustNotBeNullOrEmpty
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ContainerNameMustNotBeNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "IDataServiceQueryProvider.CurrentDataSource must return an object of type that is assignable to T in DataService&lt;T&gt;."
        /// </summary>
        internal static string DataServiceProviderWrapper_DataSourceTypeMustBeAssignableToContextType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_DataSourceTypeMustBeAssignableToContextType);
            }
        }

        /// <summary>
        /// A string like "More than one entity set with the name '{0}' was found. Entity set names must be unique."
        /// </summary>
        internal static string DataServiceProviderWrapper_MultipleEntitySetsWithSameName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_MultipleEntitySetsWithSameName, p0);
        }

        /// <summary>
        /// A string like "More than one resource type with the name '{0}' was found. Resource type names must be unique."
        /// </summary>
        internal static string DataServiceProviderWrapper_MultipleResourceTypesWithSameName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_MultipleResourceTypesWithSameName, p0);
        }

        /// <summary>
        /// A string like "More than one service operation with the name '{0}' was found. Service operation names must be unique."
        /// </summary>
        internal static string DataServiceProviderWrapper_MultipleServiceOperationsWithSameName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_MultipleServiceOperationsWithSameName, p0);
        }

        /// <summary>
        /// A string like "Complex type '{0}' has derived types and is used as the item type in a collection property. Only collection properties containing complex types without derived types are supported."
        /// </summary>
        internal static string DataServiceProviderWrapper_CollectionOfComplexTypeWithDerivedTypes(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_CollectionOfComplexTypeWithDerivedTypes, p0);
        }

        /// <summary>
        /// A string like "The IDataServiceQueryProvider.GetQueryRootForResourceSet({0}) method must return an IQueryable instance that can be converted to the type '{1}'."
        /// </summary>
        internal static string DataServiceProviderWrapper_InvalidQueryRootType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_InvalidQueryRootType, p0, p1);
        }

        /// <summary>
        /// A string like "The collection returned by DataServiceConfiguration.AnnotationsBuilder must not contain null elements."
        /// </summary>
        internal static string DataServiceProviderWrapper_AnnotationsBuilderCannotReturnNullModels
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_AnnotationsBuilderCannotReturnNullModels);
            }
        }

        /// <summary>
        /// A string like "The service action '{0}' has the binding parameter of type '{1}', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type '{1}' should be made visible."
        /// </summary>
        internal static string DataServiceProviderWrapper_ActionHasNoBindableSet(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ActionHasNoBindableSet, p0, p1);
        }

        /// <summary>
        /// A string like "The service action '{0}' has the resource set path expression '{1}', but there is no visible resource set that can be reached from the binding parameter through the path expression. The service action should be made hidden or a resource set targeted by the path expression should be made visible."
        /// </summary>
        internal static string DataServiceProviderWrapper_ActionHasNoVisibleSetReachableFromPathExpression(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderWrapper_ActionHasNoVisibleSetReachableFromPathExpression, p0, p1);
        }

        /// <summary>
        /// A string like "A service action with the name '{0}' already exists. Please make sure that the list returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() contains unique service action names."
        /// </summary>
        internal static string DataServiceActionProviderWrapper_DuplicateAction(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceActionProviderWrapper_DuplicateAction, p0);
        }

        /// <summary>
        /// A string like "The 'ProviderBehavior' property for IDataServiceProviderBehavior interface must not return a null value."
        /// </summary>
        internal static string DataServiceProviderBehavior_ProviderBehaviorMustBeNonNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceProviderBehavior_ProviderBehaviorMustBeNonNull);
            }
        }

        /// <summary>
        /// A string like "An error occurred while trying to write an error payload."
        /// </summary>
        internal static string ErrorHandler_ErrorWhileWritingError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ErrorHandler_ErrorWhileWritingError);
            }
        }

        /// <summary>
        /// A string like "ETag attribute must specify at least one property name."
        /// </summary>
        internal static string ETagAttribute_MustSpecifyAtleastOnePropertyName
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ETagAttribute_MustSpecifyAtleastOnePropertyName);
            }
        }

        /// <summary>
        /// A string like "The filter for an expand segment should be a lambda type, but the specified filter is of type '{0}'."
        /// </summary>
        internal static string ExpandSegment_FilterShouldBeLambda(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ExpandSegment_FilterShouldBeLambda, p0);
        }

        /// <summary>
        /// A string like "The filter for an expand segment should return a boolean value, but the specified filter returns '{0}'."
        /// </summary>
        internal static string ExpandSegment_FilterBodyShouldReturnBool(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ExpandSegment_FilterBodyShouldReturnBool, p0);
        }

        /// <summary>
        /// A string like "The filter for an expand segment should take a single parameter, but the specified filter takes '{0}'."
        /// </summary>
        internal static string ExpandSegment_FilterBodyShouldTakeOneParameter(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ExpandSegment_FilterBodyShouldTakeOneParameter, p0);
        }

        /// <summary>
        /// A string like "Query parameter item '{0}' expected, but both '{1}' and '{2}' are inexact matches for it."
        /// </summary>
        internal static string HttpContextServiceHost_AmbiguousItemName(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_AmbiguousItemName, p0, p1, p2);
        }

        /// <summary>
        /// A string like "There is no System.ServiceModel.Web.WebOperationContext.Current instance."
        /// </summary>
        internal static string HttpContextServiceHost_WebOperationContextCurrentMissing
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_WebOperationContextCurrentMissing);
            }
        }

        /// <summary>
        /// A string like "Fragment parts are not supported in template matches but the base URI '{0}' has one."
        /// </summary>
        internal static string HttpContextServiceHost_IncomingTemplateMatchFragment(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_IncomingTemplateMatchFragment, p0);
        }

        /// <summary>
        /// A string like "Query parts are not supported in template matches but the base URI '{0}' has one."
        /// </summary>
        internal static string HttpContextServiceHost_IncomingTemplateMatchQuery(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_IncomingTemplateMatchQuery, p0);
        }

        /// <summary>
        /// A string like "Malformed value in request header."
        /// </summary>
        internal static string HttpContextServiceHost_MalformedHeaderValue
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_MalformedHeaderValue);
            }
        }

        /// <summary>
        /// A string like "The X-HTTP-Method header should have a single value, but has '{0}' instead."
        /// </summary>
        internal static string HttpContextServiceHost_XMethodIncorrectCount(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_XMethodIncorrectCount, p0);
        }

        /// <summary>
        /// A string like "The X-HTTP-Method header should be 'PUT', 'PATCH' or 'DELETE', but is '{0}' instead."
        /// </summary>
        internal static string HttpContextServiceHost_XMethodIncorrectValue(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_XMethodIncorrectValue, p0);
        }

        /// <summary>
        /// A string like "The X-HTTP-Method header can only be used with POST requests."
        /// </summary>
        internal static string HttpContextServiceHost_XMethodNotUsingPost
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_XMethodNotUsingPost);
            }
        }

        /// <summary>
        /// A string like "Query parameter '{0}' is specified, but it should be specified exactly once."
        /// </summary>
        internal static string HttpContextServiceHost_QueryParameterMustBeSpecifiedOnce(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_QueryParameterMustBeSpecifiedOnce, p0);
        }

        /// <summary>
        /// A string like "The query parameter '{0}' begins with a system-reserved '$' character but is not recognized."
        /// </summary>
        internal static string HttpContextServiceHost_UnknownQueryParameter(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_UnknownQueryParameter, p0);
        }

        /// <summary>
        /// A string like "The incoming message property '{0}' on the current operation context must be a System.Uri object."
        /// </summary>
        internal static string HttpContextServiceHost_IncomingMessagePropertyMustBeValidUriInstance(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpContextServiceHost_IncomingMessagePropertyMustBeValidUriInstance, p0);
        }

        /// <summary>
        /// A string like "Content-Type header value missing."
        /// </summary>
        internal static string HttpProcessUtility_ContentTypeMissing
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_ContentTypeMissing);
            }
        }

        /// <summary>
        /// A string like "Media type is missing a parameter value."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeMissingValue
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_MediaTypeMissingValue);
            }
        }

        /// <summary>
        /// A string like "Media type requires a ';' character before a parameter definition."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter);
            }
        }

        /// <summary>
        /// A string like "Media type requires a '/' character."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSlash
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_MediaTypeRequiresSlash);
            }
        }

        /// <summary>
        /// A string like "Media type requires a subtype definition."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeRequiresSubType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_MediaTypeRequiresSubType);
            }
        }

        /// <summary>
        /// A string like "Media type is unspecified."
        /// </summary>
        internal static string HttpProcessUtility_MediaTypeUnspecified
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_MediaTypeUnspecified);
            }
        }

        /// <summary>
        /// A string like "Character set '{0}' is not supported."
        /// </summary>
        internal static string HttpProcessUtility_EncodingNotSupported(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_EncodingNotSupported, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because it contained escape characters even though it was not quoted."
        /// </summary>
        internal static string HttpProcessUtility_EscapeCharWithoutQuotes(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_EscapeCharWithoutQuotes, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because it terminated with escape character. Escape characters must always be followed by a character in a parameter value."
        /// </summary>
        internal static string HttpProcessUtility_EscapeCharAtEnd(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_EscapeCharAtEnd, p0);
        }

        /// <summary>
        /// A string like "Value for MIME type parameter '{0}' is incorrect because the closing quote character could not be found while the parameter value started with a quote character."
        /// </summary>
        internal static string HttpProcessUtility_ClosingQuoteNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.HttpProcessUtility_ClosingQuoteNotFound, p0);
        }

        /// <summary>
        /// A string like "The property name specified in the IgnoreProperties attribute cannot be null or empty. Please specify a valid property name."
        /// </summary>
        internal static string IgnorePropertiesAttribute_PropertyNameCannotBeNullOrEmpty
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.IgnorePropertiesAttribute_PropertyNameCannotBeNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' is not a valid property name for type '{1}'. Please specify a valid property name."
        /// </summary>
        internal static string IgnorePropertiesAttribute_InvalidPropertyName(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.IgnorePropertiesAttribute_InvalidPropertyName, p0, p1);
        }

        /// <summary>
        /// A string like "Expecting XML element '{0}', but found '{1}' instead."
        /// </summary>
        internal static string PlainXml_IncorrectElementName(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.PlainXml_IncorrectElementName, p0, p1);
        }

        /// <summary>
        /// A string like "Digit expected."
        /// </summary>
        internal static string RequestQueryParser_DigitExpected
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_DigitExpected);
            }
        }

        /// <summary>
        /// A string like "The '{0}' is not allowed. Member access or specifying a type identifier on a resource set reference is not allowed."
        /// </summary>
        internal static string RequestQueryParser_DisallowMemberAccessForResourceSetReference(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_DisallowMemberAccessForResourceSetReference, p0);
        }

        /// <summary>
        /// A string like "Expression of type '{0}' expected."
        /// </summary>
        internal static string RequestQueryParser_ExpressionTypeMismatch(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_ExpressionTypeMismatch, p0);
        }

        /// <summary>
        /// A string like "Identifier expected."
        /// </summary>
        internal static string RequestQueryParser_IdentifierExpected
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_IdentifierExpected);
            }
        }

        /// <summary>
        /// A string like "Operator '{0}' incompatible with operand type '{1}'."
        /// </summary>
        internal static string RequestQueryParser_IncompatibleOperand(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_IncompatibleOperand, p0, p1);
        }

        /// <summary>
        /// A string like "Operator '{0}' incompatible with operand types '{1}' and '{2}'."
        /// </summary>
        internal static string RequestQueryParser_IncompatibleOperands(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_IncompatibleOperands, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Syntax error '{0}'."
        /// </summary>
        internal static string RequestQueryParser_InvalidCharacter(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_InvalidCharacter, p0);
        }

        /// <summary>
        /// A string like "No applicable function found for '{0}' with the specified arguments. The functions considered are: {1}."
        /// </summary>
        internal static string RequestQueryParser_NoApplicableFunction(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_NoApplicableFunction, p0, p1);
        }

        /// <summary>
        /// A string like "'not' operator does not support type '{0}'."
        /// </summary>
        internal static string RequestQueryParser_NotDoesNotSupportType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_NotDoesNotSupportType, p0);
        }

        /// <summary>
        /// A string like "The operator '{0}' is not supported for the 'null' literal; only equality checks are supported."
        /// </summary>
        internal static string RequestQueryParser_NullOperatorUnsupported(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_NullOperatorUnsupported, p0);
        }

        /// <summary>
        /// A string like "Ordering does not support expressions of type '{0}'."
        /// </summary>
        internal static string RequestQueryParser_OrderByDoesNotSupportType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_OrderByDoesNotSupportType, p0);
        }

        /// <summary>
        /// A string like "Syntax error."
        /// </summary>
        internal static string RequestQueryParser_SyntaxError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Unknown function '{0}'."
        /// </summary>
        internal static string RequestQueryParser_UnknownFunction(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_UnknownFunction, p0);
        }

        /// <summary>
        /// A string like "Unrecognized '{0}' literal '{1}'."
        /// </summary>
        internal static string RequestQueryParser_UnrecognizedLiteral(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_UnrecognizedLiteral, p0, p1);
        }

        /// <summary>
        /// A string like "Unterminated literal in '{0}'."
        /// </summary>
        internal static string RequestQueryParser_UnterminatedLiteral(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_UnterminatedLiteral, p0);
        }

        /// <summary>
        /// A string like "Unterminated string literal in '{0}'."
        /// </summary>
        internal static string RequestQueryParser_UnterminatedStringLiteral(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_UnterminatedStringLiteral, p0);
        }

        /// <summary>
        /// A string like "Skip token values are expected to be literals. Unrecognized literal '{0}' found."
        /// </summary>
        internal static string RequsetQueryParser_ExpectingLiteralInSkipToken(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequsetQueryParser_ExpectingLiteralInSkipToken, p0);
        }

        /// <summary>
        /// A string like "The method 'all' without a supplied predicate is not supported."
        /// </summary>
        internal static string RequestQueryParser_AllWithoutAPredicateIsNotSupported
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_AllWithoutAPredicateIsNotSupported);
            }
        }

        /// <summary>
        /// A string like "This data service endpoint does not support spatial literal values in the URI."
        /// </summary>
        internal static string RequestQueryParser_SpatialNotSupported
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_SpatialNotSupported);
            }
        }

        /// <summary>
        /// A string like "Numeric string '{0}' is not a valid Int32/Int64/Double/Decimal."
        /// </summary>
        internal static string RequestQueryParser_InvalidNumericString(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryParser_InvalidNumericString, p0);
        }

        /// <summary>
        /// A string like "Incorrect format for {0} argument '{1}'."
        /// </summary>
        internal static string RequestQueryProcessor_IncorrectArgumentFormat(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_IncorrectArgumentFormat, p0, p1);
        }

        /// <summary>
        /// A string like "Query option $expand cannot be applied to the requested resource."
        /// </summary>
        internal static string RequestQueryProcessor_QueryExpandOptionNotApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QueryExpandOptionNotApplicable);
            }
        }

        /// <summary>
        /// A string like "Query option $filter cannot be applied to the requested resource."
        /// </summary>
        internal static string RequestQueryProcessor_QueryFilterOptionNotApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QueryFilterOptionNotApplicable);
            }
        }

        /// <summary>
        /// A string like "Query option $format is not supported on a $batch request."
        /// </summary>
        internal static string RequestQueryProcessor_FormatNotApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_FormatNotApplicable);
            }
        }

        /// <summary>
        /// A string like "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource."
        /// </summary>
        internal static string RequestQueryProcessor_QueryNoOptionsApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QueryNoOptionsApplicable);
            }
        }

        /// <summary>
        /// A string like "Query options $orderby, $count, $skip and $top cannot be applied to the requested resource."
        /// </summary>
        internal static string RequestQueryProcessor_QuerySetOptionsNotApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QuerySetOptionsNotApplicable);
            }
        }

        /// <summary>
        /// A string like "Query option $select cannot be applied to the requested resource."
        /// </summary>
        internal static string RequestQueryProcessor_QuerySelectOptionNotApplicable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QuerySelectOptionNotApplicable);
            }
        }

        /// <summary>
        /// A string like "Unknown $count option, only "true" and "false" are supported"
        /// </summary>
        internal static string RequestQueryProcessor_InvalidCountOptionError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_InvalidCountOptionError);
            }
        }

        /// <summary>
        /// A string like "Segment /$count and query count $count=true(false)) only apply to an HTTP GET request."
        /// </summary>
        internal static string RequestQueryProcessor_RequestVerbCannotCountError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_RequestVerbCannotCountError);
            }
        }

        /// <summary>
        /// A string like "Query $count cannot be applied to the resource segment $count"
        /// </summary>
        internal static string RequestQueryProcessor_QueryCountWithSegmentCount
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QueryCountWithSegmentCount);
            }
        }

        /// <summary>
        /// A string like "Property {1} on type {0} is of primitive type and cannot be used as a navigation property."
        /// </summary>
        internal static string RequestQueryProcessor_PrimitivePropertyUsedAsNavigationProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_PrimitivePropertyUsedAsNavigationProperty, p0, p1);
        }

        /// <summary>
        /// A string like "$select doesn't support projecting properties of complex type. Type {0}, property {1}."
        /// </summary>
        internal static string RequestQueryProcessor_ComplexPropertyAsInnerSelectSegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_ComplexPropertyAsInnerSelectSegment, p0, p1);
        }

        /// <summary>
        /// A string like "The projection from property '{1}' of type '{0}' is not valid. The $select query option does not support projecting items from a collection property."
        /// </summary>
        internal static string RequestQueryProcessor_CollectionPropertyAsInnerSelectSegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_CollectionPropertyAsInnerSelectSegment, p0, p1);
        }

        /// <summary>
        /// A string like "Only properties specified in $expand can be traversed in $select query options. Property {0}."
        /// </summary>
        internal static string RequestQueryProcessor_ProjectedPropertyWithoutMatchingExpand(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_ProjectedPropertyWithoutMatchingExpand, p0);
        }

        /// <summary>
        /// A string like "A skip token can only be provided in a query request against an entity set when the entity set has a paging limit set."
        /// </summary>
        internal static string RequestQueryProcessor_SkipTokenSupportedOnPagedSets
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_SkipTokenSupportedOnPagedSets);
            }
        }

        /// <summary>
        /// A string like "Skip tokens can only be provided for requests that return collections of entities."
        /// </summary>
        internal static string RequestQueryProcessor_SkipTokenNotAllowed
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_SkipTokenNotAllowed);
            }
        }

        /// <summary>
        /// A string like "The named stream '{0}' must be the last segment in a $select path."
        /// </summary>
        internal static string RequestQueryProcessor_NamedStreamMustBeLastSegmentInSelect(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_NamedStreamMustBeLastSegmentInSelect, p0);
        }

        /// <summary>
        /// A string like "The service action '{0}' must be the last segment in a $select path."
        /// </summary>
        internal static string RequestQueryProcessor_ServiceActionMustBeLastSegmentInSelect(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_ServiceActionMustBeLastSegmentInSelect, p0);
        }

        /// <summary>
        /// A string like "Both the $select and $expand query options could not be specified for the '{0}' property because the type qualifiers do not match. The type qualifier specified in the $expand query option must be the same as the one specified in the $select query option."
        /// </summary>
        internal static string RequestQueryProcessor_SelectAndExpandCannotBeSpecifiedTogether(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_SelectAndExpandCannotBeSpecifiedTogether, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' resolves to an open property, but the same property is already a declared property on type '{2}'. In a query URI, the same property cannot belong to two different types in the same type hierarchy. You must either remove one property from the query URI or use the same type identifier."
        /// </summary>
        internal static string RequestQueryProcessor_CannotSpecifyOpenPropertyAndDeclaredPropertyAtTheSameTime(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_CannotSpecifyOpenPropertyAndDeclaredPropertyAtTheSameTime, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The {0} query option is not valid because it contains a property path which ends with the type identifier '{1}'. Property paths in query options cannot end with a type identifier. Please make sure that property paths specified in query options end with a property name."
        /// </summary>
        internal static string RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier, p0, p1);
        }

        /// <summary>
        /// A string like "The AbsoluteRequestUri property of the data service host cannot be null."
        /// </summary>
        internal static string RequestUriProcessor_AbsoluteRequestUriCannotBeNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_AbsoluteRequestUriCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The AbsoluteRequestUri property of the data service host must be an absolute URI."
        /// </summary>
        internal static string RequestUriProcessor_AbsoluteRequestUriMustBeAbsolute
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_AbsoluteRequestUriMustBeAbsolute);
            }
        }

        /// <summary>
        /// A string like "The AbsoluteServiceUri property of the data service host cannot be null."
        /// </summary>
        internal static string RequestUriProcessor_AbsoluteServiceUriCannotBeNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_AbsoluteServiceUriCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The AbsoluteServiceUri property of the data service host must be an absolute URI."
        /// </summary>
        internal static string RequestUriProcessor_AbsoluteServiceUriMustBeAbsolute
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_AbsoluteServiceUriMustBeAbsolute);
            }
        }

        /// <summary>
        /// A string like "Forbidden"
        /// </summary>
        internal static string RequestUriProcessor_Forbidden
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_Forbidden);
            }
        }

        /// <summary>
        /// A string like "Segments with multiple key values must specify them in 'name=value' form."
        /// </summary>
        internal static string RequestUriProcessor_KeysMustBeNamed
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_KeysMustBeNamed);
            }
        }

        /// <summary>
        /// A string like "Method Not Allowed"
        /// </summary>
        internal static string RequestUriProcessor_MethodNotAllowed
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_MethodNotAllowed);
            }
        }

        /// <summary>
        /// A string like "Type '{0}' does not have a property named '{1}'; there is no service action named '{1}' that is bindable to the type '{0}'; and there is no type with the name '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_PropertyNotFound(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_PropertyNotFound, p0, p1);
        }

        /// <summary>
        /// A string like "Resource not found for the segment '{0}'."
        /// </summary>
        internal static string RequestUriProcessor_ResourceNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_ResourceNotFound, p0);
        }

        /// <summary>
        /// A string like "Bad Request - Error in query syntax."
        /// </summary>
        internal static string RequestUriProcessor_SyntaxError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_SyntaxError);
            }
        }

        /// <summary>
        /// A string like "Empty segment encountered in request URL. Please make sure that a valid request URL is specified."
        /// </summary>
        internal static string RequestUriProcessor_EmptySegmentInRequestUrl
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_EmptySegmentInRequestUrl);
            }
        }

        /// <summary>
        /// A string like "The segment '{1}' in the request URI is not valid. Since the segment '{0}' refers to a primitive type property, the only supported value from the next segment is '$value'."
        /// </summary>
        internal static string RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment, p0, p1);
        }

        /// <summary>
        /// A string like "Resource Not Found - '{0}' refers to a service operation which does not allow further composition."
        /// </summary>
        internal static string RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. Since the segment '{0}' refers to a collection, this must be the last segment in the request URI or it must be followed by an function or action that can be bound to it otherwise all intermediate segments must refer to a single resource."
        /// </summary>
        internal static string RequestUriProcessor_CannotQueryCollections(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_CannotQueryCollections, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must be the last segment in the URI because it is one of the following: $batch, $value, $metadata, a collection property, a named media resource, an action, a noncomposable function, an action import, or a noncomposable function import."
        /// </summary>
        internal static string RequestUriProcessor_MustBeLeafSegment(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_MustBeLeafSegment, p0);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' must refer to a navigation property since the previous segment identifier is '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment, p0, p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' is not valid. Since the uri contains the '{1}' segment, there must be only one segment specified after that."
        /// </summary>
        internal static string RequestUriProcessor_CannotSpecifyAfterPostLinkSegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_CannotSpecifyAfterPostLinkSegment, p0, p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid, the segment $count cannot be applied to the root of the service."
        /// </summary>
        internal static string RequestUriProcessor_CountOnRoot
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_CountOnRoot);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid. $count cannot be applied to the segment '{0}' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type."
        /// </summary>
        internal static string RequestUriProcessor_CountNotSupported(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_CountNotSupported, p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' refers to a named stream and is not valid for '{1}' operation."
        /// </summary>
        internal static string RequestUriProcessor_InvalidHttpMethodForNamedStream(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_InvalidHttpMethodForNamedStream, p0, p1);
        }

        /// <summary>
        /// A string like "The request URI is not valid. The segment '{0}' cannot include key predicates, however it may end with empty parenthesis."
        /// </summary>
        internal static string RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates, p0);
        }

        /// <summary>
        /// A string like "The type name '{0}' specified in the URI is not a valid type. Please make sure that the type name is valid and that it derives from the type '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_InvalidTypeIdentifier_MustBeASubType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_InvalidTypeIdentifier_MustBeASubType, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' specified in the URI is neither a base type nor a sub-type of the previously-specified type '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType, p0, p1);
        }

        /// <summary>
        /// A string like "Type identifier '{0}' in the URI cannot be specified immediately after '{1}' type identifier. There must be a property name specified between 2 type identifiers."
        /// </summary>
        internal static string RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier, p0, p1);
        }

        /// <summary>
        /// A string like "The operation '{0}' is not bindable and must be called at the root level."
        /// </summary>
        internal static string RequestUriProcessor_UnbindableOperationsMustBeCalledAtRootLevel(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_UnbindableOperationsMustBeCalledAtRootLevel, p0);
        }

        /// <summary>
        /// A string like "Service action '{0}' requires a binding parameter, but it was invoked unbound."
        /// </summary>
        internal static string RequestUriProcessor_MissingBindingParameter(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_MissingBindingParameter, p0);
        }

        /// <summary>
        /// A string like "The binding parameter for '{0}' is not assignable from the result of the uri segment '{1}'."
        /// </summary>
        internal static string RequestUriProcessor_BindingParameterNotAssignableFromPreviousSegment(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_BindingParameterNotAssignableFromPreviousSegment, p0, p1);
        }

        /// <summary>
        /// A string like "Batched service action '{0}' cannot be invoked because it was bound to an entity created in the same changeset."
        /// </summary>
        internal static string RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset, p0);
        }

        /// <summary>
        /// A string like "An action cannot be composed with a service operation that uses the WebInvokeAttribute."
        /// </summary>
        internal static string RequestUriProcessor_ActionComposedWithWebInvokeServiceOperationNotAllowed
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.RequestUriProcessor_ActionComposedWithWebInvokeServiceOperationNotAllowed);
            }
        }

        /// <summary>
        /// A string like "The request URI is not valid. $ref cannot be applied to the segment '{0}' since $ref can only follow a navigational property."
        /// </summary>
        internal static string PathParser_EntityReferenceNotSupported(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.PathParser_EntityReferenceNotSupported, p0);
        }

        /// <summary>
        /// A string like "Multiple Service Operations with the name '{0}' were found. There can only be one Service Operation with a given name in a model."
        /// </summary>
        internal static string PathParser_ServiceOperationsWithSameName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.PathParser_ServiceOperationsWithSameName, p0);
        }

        /// <summary>
        /// A string like "The ResourceTypeKind property of a ResourceType instance that is associated with a ResourceSet must have a value of 'EntityType'."
        /// </summary>
        internal static string ResourceContainer_ContainerMustBeAssociatedWithEntityType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceContainer_ContainerMustBeAssociatedWithEntityType);
            }
        }

        /// <summary>
        /// A string like "The resource set '{0}' cannot be modified since it is already set to read-only."
        /// </summary>
        internal static string ResourceSet_Sealed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSet_Sealed, p0);
        }

        /// <summary>
        /// A string like "The path expression '{0}' is not a valid path expression. A valid path expression must start with the binding parameter name '{1}'."
        /// </summary>
        internal static string ResourceSetPathExpression_PathExpressionMustStartWithBindingParameterName(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSetPathExpression_PathExpressionMustStartWithBindingParameterName, p0, p1);
        }

        /// <summary>
        /// A string like "The path expression '{0}' is not a valid expression because the segment '{1}' is not a type identifier or a property on the resource type '{2}'."
        /// </summary>
        internal static string ResourceSetPathExpression_PropertyNotFound(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSetPathExpression_PropertyNotFound, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The path expression '{0}' is not a valid expression because the segment '{1}' is a property of type '{2}'. A valid path expression must only contain properties of entity type."
        /// </summary>
        internal static string ResourceSetPathExpression_PropertyMustBeEntityType(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSetPathExpression_PropertyMustBeEntityType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The path expression '{0}' is not a valid expression because it ends with the type identifier '{1}'. A valid path expression must not end in a type identifier."
        /// </summary>
        internal static string ResourceSetPathExpression_PathCannotEndWithTypeIdentifier(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSetPathExpression_PathCannotEndWithTypeIdentifier, p0, p1);
        }

        /// <summary>
        /// A string like "The path expression '{0}' is not a valid expression because it contains an empty segment or it ends with '/'."
        /// </summary>
        internal static string ResourceSetPathExpression_EmptySegment(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceSetPathExpression_EmptySegment, p0);
        }

        /// <summary>
        /// A string like "Key properties cannot be defined in derived types."
        /// </summary>
        internal static string ResourceType_NoKeysInDerivedTypes
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_NoKeysInDerivedTypes);
            }
        }

        /// <summary>
        /// A string like "Key properties can only be added to ResourceType instances with a ResourceTypeKind equal to 'EntityType'."
        /// </summary>
        internal static string ResourceType_KeyPropertiesOnlyOnEntityTypes
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_KeyPropertiesOnlyOnEntityTypes);
            }
        }

        /// <summary>
        /// A string like "ETag properties can only be added to ResourceType instances with a ResourceTypeKind equal to 'EntityType'."
        /// </summary>
        internal static string ResourceType_ETagPropertiesOnlyOnEntityTypes
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_ETagPropertiesOnlyOnEntityTypes);
            }
        }

        /// <summary>
        /// A string like "ResourceTypeKind.Primitive, ResourceTypeKind.Collection and ResourceTypeKind.EntityCollection are not valid values for the '{0}' parameter."
        /// </summary>
        internal static string ResourceType_InvalidValueForResourceTypeKind(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_InvalidValueForResourceTypeKind, p0);
        }

        /// <summary>
        /// A string like "The CLR type for the resource type cannot be a value type."
        /// </summary>
        internal static string ResourceType_TypeCannotBeValueType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_TypeCannotBeValueType);
            }
        }

        /// <summary>
        /// A string like "The resource type '{0}' cannot be modified since it is already set to read-only."
        /// </summary>
        internal static string ResourceType_Sealed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_Sealed, p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' does not have any key properties. Please make sure that one or more key properties are defined for this entity type."
        /// </summary>
        internal static string ResourceType_MissingKeyPropertiesForEntity(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_MissingKeyPropertiesForEntity, p0);
        }

        /// <summary>
        /// A string like "Adding named streams to the type '{0}' is not allowed. Named streams can only be added to entity types."
        /// </summary>
        internal static string ResourceType_NamedStreamsOnlyApplyToEntityType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_NamedStreamsOnlyApplyToEntityType, p0);
        }

        /// <summary>
        /// A string like "A property with same name '{0}' already exists in type '{1}'. Please make sure that there is no property with the same name defined in one of the base types."
        /// </summary>
        internal static string ResourceType_PropertyWithSameNameAlreadyExists(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_PropertyWithSameNameAlreadyExists, p0, p1);
        }

        /// <summary>
        /// A string like "Complex types can not be marked as 'Open'. Error occurred for type '{0}'."
        /// </summary>
        internal static string ResourceType_ComplexTypeCannotBeOpen(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_ComplexTypeCannotBeOpen, p0);
        }

        /// <summary>
        /// A string like "Only collection properties that contain primitive types or complex types are supported."
        /// </summary>
        internal static string ResourceType_CollectionItemCanBeOnlyPrimitiveOrComplex
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_CollectionItemCanBeOnlyPrimitiveOrComplex);
            }
        }

        /// <summary>
        /// A string like "Only collections of an entity type are supported."
        /// </summary>
        internal static string ResourceType_CollectionItemCanBeOnlyEntity
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_CollectionItemCanBeOnlyEntity);
            }
        }

        /// <summary>
        /// A string like "A resource type of kind '{0}' cannot derive from a base resource type of kind '{1}'. Inheritance is only supported when resource types are of the same kind."
        /// </summary>
        internal static string ResourceType_InvalidResourceTypeKindInheritance(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_InvalidResourceTypeKindInheritance, p0, p1);
        }

        /// <summary>
        /// A string like "The ItemType of a collection resource type cannot be of type '{0}'."
        /// </summary>
        internal static string ResourceType_CollectionItemCannotBeStream(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_CollectionItemCannotBeStream, p0);
        }

        /// <summary>
        /// A string like "A spatial property '{0}' cannot be used as key or ETag for the entity type '{1}'."
        /// </summary>
        internal static string ResourceType_SpatialKeyOrETag(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceType_SpatialKeyOrETag, p0, p1);
        }

        /// <summary>
        /// A string like "The resource property '{0}' cannot be modified since it is already set to read-only."
        /// </summary>
        internal static string ResourceProperty_Sealed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_Sealed, p0);
        }

        /// <summary>
        /// A string like "MimeType for property '{0}' is not valid. Please make sure that the mime type is not empty."
        /// </summary>
        internal static string ResourceProperty_MimeTypeAttributeEmpty(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_MimeTypeAttributeEmpty, p0);
        }

        /// <summary>
        /// A string like "The MimeType attribute is specified for property '{0}', which is of kind '{1}'. A MimeType can only be specified on properties that are of kind ResourcePropertyKind.Primitive."
        /// </summary>
        internal static string ResourceProperty_MimeTypeAttributeOnNonPrimitive(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_MimeTypeAttributeOnNonPrimitive, p0, p1);
        }

        /// <summary>
        /// A string like "MIME type '{0}' for property '{1}' is not in 'type/subtype' format. Please specify a valid value for mime type."
        /// </summary>
        internal static string ResourceProperty_MimeTypeNotValid(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_MimeTypeNotValid, p0, p1);
        }

        /// <summary>
        /// A string like "'{0}' parameter does not match with the type of the resource type in parameter '{1}'."
        /// </summary>
        internal static string ResourceProperty_PropertyKindAndResourceTypeKindMismatch(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_PropertyKindAndResourceTypeKindMismatch, p0, p1);
        }

        /// <summary>
        /// A string like "Key Properties cannot be of nullable type. Please make sure the type of this property is not of Nullable&lt;&gt; type."
        /// </summary>
        internal static string ResourceProperty_KeyPropertiesCannotBeNullable
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_KeyPropertiesCannotBeNullable);
            }
        }

        /// <summary>
        /// A string like "The property 'CanReflectOnInstanceTypeProperty' on a NamedStream property is not settable."
        /// </summary>
        internal static string ResourceProperty_NamedStreamCannotReflect
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_NamedStreamCannotReflect);
            }
        }

        /// <summary>
        /// A string like "Invalid ResourcePropertyKind, ResourcePropertyKind.Stream must not be combined with any other flag."
        /// </summary>
        internal static string ResourceProperty_NamedStreamKindMustBeUsedAlone
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceProperty_NamedStreamKindMustBeUsedAlone);
            }
        }

        /// <summary>
        /// A string like "The resource contains value '{0}' which cannot be serialized."
        /// </summary>
        internal static string Serializer_CannotConvertValue(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_CannotConvertValue, p0);
        }

        /// <summary>
        /// A string like "The serialized resource has a null value in key member '{0}'. Null values are not supported in key members."
        /// </summary>
        internal static string Serializer_NullKeysAreNotSupported(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_NullKeysAreNotSupported, p0);
        }

        /// <summary>
        /// A string like "The response exceeds the maximum {0} results per collection."
        /// </summary>
        internal static string Serializer_ResultsExceedMax(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_ResultsExceedMax, p0);
        }

        /// <summary>
        /// A string like "The top-level type '{0}' for this request is not supported for serialization."
        /// </summary>
        internal static string Serializer_UnsupportedTopLevelType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_UnsupportedTopLevelType, p0);
        }

        /// <summary>
        /// A string like "The etag value in the request header does not match with the current etag value of the object."
        /// </summary>
        internal static string Serializer_ETagValueDoesNotMatch
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_ETagValueDoesNotMatch);
            }
        }

        /// <summary>
        /// A string like "If-Match or If-None-Match headers cannot be specified if the target type does not have etag properties defined."
        /// </summary>
        internal static string Serializer_NoETagPropertiesForType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_NoETagPropertiesForType);
            }
        }

        /// <summary>
        /// A string like "The collection property '{0}' has a null value. Collection properties that return a null value are not supported."
        /// </summary>
        internal static string Serializer_CollectionCanNotBeNull(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_CollectionCanNotBeNull, p0);
        }

        /// <summary>
        /// A string like "The collection type returned by the property '{0}' does not implement IEnumerable. Collection properties must be of a type that implements IEnumerable."
        /// </summary>
        internal static string Serializer_CollectionPropertyValueMustImplementIEnumerable(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_CollectionPropertyValueMustImplementIEnumerable, p0);
        }

        /// <summary>
        /// A string like "A circular loop was detected while serializing the property '{0}'. You must make sure that loops are not present in properties that return a collection property or complex type."
        /// </summary>
        internal static string Serializer_LoopsNotAllowedInComplexTypes(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Serializer_LoopsNotAllowedInComplexTypes, p0);
        }

        /// <summary>
        /// A string like "The MIME type '{0}' for service operation '{1}' is not in 'type/subtype' format."
        /// </summary>
        internal static string ServiceOperation_MimeTypeNotValid(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_MimeTypeNotValid, p0, p1);
        }

        /// <summary>
        /// A string like "The MIME type specified for service operation '{0}' is not valid. The MimeType property cannot be set to null or empty."
        /// </summary>
        internal static string ServiceOperation_MimeTypeCannotBeEmpty(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_MimeTypeCannotBeEmpty, p0);
        }

        /// <summary>
        /// A string like "The service operation '{0}' cannot be modified since it is already set to read-only."
        /// </summary>
        internal static string ServiceOperation_Sealed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_Sealed, p0);
        }

        /// <summary>
        /// A string like "The '{1}' parameter must be null when the '{0}' parameter value is '{2}', however the '{1}' parameter cannot be null when the '{0}' parameter is of any value other than '{2}'. Please make sure that the '{0}' parameter value is set according to the '{1}' parameter value."
        /// </summary>
        internal static string ServiceOperation_ResultTypeAndKindMustMatch(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ResultTypeAndKindMustMatch, p0, p1, p2);
        }

        /// <summary>
        /// A string like "'{0}' must be null when '{1}' is null or not an EntityType."
        /// </summary>
        internal static string ServiceOperation_ResultSetMustBeNullForGivenResultType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ResultSetMustBeNullForGivenResultType, p0, p1);
        }

        /// <summary>
        /// A string like "'{0}' must be null when '{1}' is null, not an entity type or not an entity collection type."
        /// </summary>
        internal static string ServiceOperation_ResultSetMustBeNullForGivenReturnType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ResultSetMustBeNullForGivenReturnType, p0, p1);
        }

        /// <summary>
        /// A string like "When '{0}' is an entity type, '{1}' cannot be null and the resource type of '{1}' must be assignable from '{0}'."
        /// </summary>
        internal static string ServiceOperation_ResultTypeAndResultSetMustMatch(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ResultTypeAndResultSetMustMatch, p0, p1);
        }

        /// <summary>
        /// A string like "When '{0}' is an entity type or an entity collection type, '{1}' and '{2}' cannot be both null and the resource type of the result set must be assignable from '{0}'."
        /// </summary>
        internal static string ServiceOperation_ReturnTypeAndResultSetMustMatch(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ReturnTypeAndResultSetMustMatch, p0, p1, p2);
        }

        /// <summary>
        /// A string like "An invalid HTTP method '{0}' was specified for the service operation '{1}'. Only the HTTP 'POST' and 'GET' methods are supported for service operations."
        /// </summary>
        internal static string ServiceOperation_NotSupportedProtocolMethod(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_NotSupportedProtocolMethod, p0, p1);
        }

        /// <summary>
        /// A string like "A parameter with the name '{0}' already exists. Please make sure that every parameter has a unique name."
        /// </summary>
        internal static string ServiceOperation_DuplicateParameterName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_DuplicateParameterName, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' is not a type that can be returned by a service operation. A service operation can only return values of an entity type, a complex type or any primitive type, other than the stream type."
        /// </summary>
        internal static string ServiceOperation_InvalidResultType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_InvalidResultType, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' is not a type that can be returned by a function or action. A function or action can only return values of an entity type, an entity collection type, a complex type, a collection type or any primitive type, other than the stream type."
        /// </summary>
        internal static string ServiceOperation_InvalidReturnType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_InvalidReturnType, p0);
        }

        /// <summary>
        /// A string like "An action's binding parameter must be of type Entity or EntityCollection."
        /// </summary>
        internal static string ServiceOperation_ActionBindingMustBeEntityOrEntityCollection
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_ActionBindingMustBeEntityOrEntityCollection);
            }
        }

        /// <summary>
        /// A string like "Bindable actions or functions must have at least one parameter, where the first parameter is the binding parameter."
        /// </summary>
        internal static string ServiceOperation_BindableOperationMustHaveAtLeastOneParameter
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_BindableOperationMustHaveAtLeastOneParameter);
            }
        }

        /// <summary>
        /// A string like "The operation must be bindable when '{0}' is not null."
        /// </summary>
        internal static string ServiceOperation_MustBeBindableToUsePathExpression(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_MustBeBindableToUsePathExpression, p0);
        }

        /// <summary>
        /// A string like "The binding parameter type must be an entity type or an entity collection type when '{0}' is not null."
        /// </summary>
        internal static string ServiceOperation_BindingParameterMustBeEntityToUsePathExpression(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_BindingParameterMustBeEntityToUsePathExpression, p0);
        }

        /// <summary>
        /// A string like "The '{0}' parameter is of resource type kind '{1}' and it is not the binding parameter. Parameter of type kind '{1}' is only supported for the binding parameter."
        /// </summary>
        internal static string ServiceOperation_NonBindingParametersCannotBeEntityorEntityCollection(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperation_NonBindingParametersCannotBeEntityorEntityCollection, p0, p1);
        }

        /// <summary>
        /// A string like "The operation '{0}' is not supported because a bound operation cannot specify an entity set value and must specify and entity set path."
        /// </summary>
        internal static string Opereration_BoundOperationsMustNotSpecifyEntitySetOnlyEntitySetPath(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Opereration_BoundOperationsMustNotSpecifyEntitySetOnlyEntitySetPath, p0);
        }

        /// <summary>
        /// A string like "The service operation parameter '{0}' of type '{1}' is not supported."
        /// </summary>
        internal static string ServiceOperationParameter_TypeNotSupported(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperationParameter_TypeNotSupported, p0, p1);
        }

        /// <summary>
        /// A string like "A single resource was expected for the result, but multiple resources were found."
        /// </summary>
        internal static string SingleResourceExpected
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.SingleResourceExpected);
            }
        }

        /// <summary>
        /// A string like "Error reading syndication item: '{0}'."
        /// </summary>
        internal static string Syndication_ErrorReadingEntry(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Syndication_ErrorReadingEntry, p0);
        }

        /// <summary>
        /// A string like "The open property '{0}' has an value which is not of valid type.. Please make sure that the property type is supported."
        /// </summary>
        internal static string Syndication_InvalidOpenPropertyType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Syndication_InvalidOpenPropertyType, p0);
        }

        /// <summary>
        /// A string like "One or more open properties of the given instance of type '{0}' has a null or empty name specified."
        /// </summary>
        internal static string Syndication_InvalidOpenPropertyName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.Syndication_InvalidOpenPropertyName, p0);
        }

        /// <summary>
        /// A string like "The required extended attribute '{0}' is missing for resource type '{1}'."
        /// </summary>
        internal static string ObjectContext_MissingExtendedAttributeType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_MissingExtendedAttributeType, p0, p1);
        }

        /// <summary>
        /// A string like "A resource type named '{0}' does not exist in the metadata."
        /// </summary>
        internal static string ObjectContext_ResourceTypeNameNotExist(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_ResourceTypeNameNotExist, p0);
        }

        /// <summary>
        /// A string like "An entity resource type is expected, however the resource type '{0}' is of type kind '{1}'."
        /// </summary>
        internal static string ObjectContext_EntityTypeExpected(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_EntityTypeExpected, p0, p1);
        }

        /// <summary>
        /// A string like "A complex resource type is expected, however the resource type '{0}' is of type kind '{1}'."
        /// </summary>
        internal static string ObjectContext_ComplexTypeExpected(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_ComplexTypeExpected, p0, p1);
        }

        /// <summary>
        /// A string like "The clr type '{0}' is an unknown resource type to the metadata provider."
        /// </summary>
        internal static string ObjectContext_UnknownResourceTypeForClrType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_UnknownResourceTypeForClrType, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' does not define a property that is named '{1}'."
        /// </summary>
        internal static string ObjectContext_PropertyNotDefinedOnType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_PropertyNotDefinedOnType, p0, p1);
        }

        /// <summary>
        /// A string like "The CLR type '{0}' does not define a public property named '{1}'. All properties declared on types in the object model must be public."
        /// </summary>
        internal static string ObjectContext_PublicPropertyNotDefinedOnType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_PublicPropertyNotDefinedOnType, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' must be a navigation property defined on the resource type '{1}'."
        /// </summary>
        internal static string ObjectContext_PropertyMustBeNavigationProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_PropertyMustBeNavigationProperty, p0, p1);
        }

        /// <summary>
        /// A string like "If-None-Match HTTP header cannot be specified for update and delete operations."
        /// </summary>
        internal static string ObjectContext_IfNoneMatchHeaderNotSupportedInUpdateAndDelete
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_IfNoneMatchHeaderNotSupportedInUpdateAndDelete);
            }
        }

        /// <summary>
        /// A string like "The operation '{0}' has the resource set '{1}' that is not visible. The operation '{0}' should be made hidden or the resource set '{1}' should be made visible."
        /// </summary>
        internal static string OperationWrapper_OperationResourceSetNotVisible(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.OperationWrapper_OperationResourceSetNotVisible, p0, p1);
        }

        /// <summary>
        /// A string like "The service action '{0}' has the resource set path expression '{1}'. However there is no visible target resource set when it is bind to the resource set '{2}'."
        /// </summary>
        internal static string OperationWrapper_TargetSetFromPathExpressionNotNotVisible(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.OperationWrapper_TargetSetFromPathExpressionNotNotVisible, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The operation '{0}' contains a path expression and cannot be invoked as a top level operation."
        /// </summary>
        internal static string OperationWrapper_PathExpressionRequiresBindingSet(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.OperationWrapper_PathExpressionRequiresBindingSet, p0);
        }

        /// <summary>
        /// A string like "Unable to create ServiceOperationProvider. Type '{0}' is abstract."
        /// </summary>
        internal static string ServiceOperationProvider_TypeIsAbstract(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ServiceOperationProvider_TypeIsAbstract, p0);
        }

        /// <summary>
        /// A string like "The batch request exceeds the maximum {0} operations per request."
        /// </summary>
        internal static string DataService_BatchExceedMaxBatchCount(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_BatchExceedMaxBatchCount, p0);
        }

        /// <summary>
        /// A string like "The batch request operation exceeds the maximum {0} changes per change set."
        /// </summary>
        internal static string DataService_BatchExceedMaxChangeSetCount(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_BatchExceedMaxChangeSetCount, p0);
        }

        /// <summary>
        /// A string like "The /$batch resource only supports POST method requests."
        /// </summary>
        internal static string DataService_BatchResourceOnlySupportsPost
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_BatchResourceOnlySupportsPost);
            }
        }

        /// <summary>
        /// A string like "Method '{0}' on type '{1}' is marked as a change interceptor method but has {2} parameters. Two parameters are expected."
        /// </summary>
        internal static string DataService_ChangeInterceptorIncorrectParameterCount(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ChangeInterceptorIncorrectParameterCount, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Unable to create data provider. Type '{0}' for data source in '{1}' is abstract."
        /// </summary>
        internal static string DataService_ContextTypeIsAbstract(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ContextTypeIsAbstract, p0, p1);
        }

        /// <summary>
        /// A string like "The method CreateDataService has been overridden but it returned a null value; a valid instance is required."
        /// </summary>
        internal static string DataService_CreateDataSourceNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_CreateDataSourceNull);
            }
        }

        /// <summary>
        /// A string like "The request includes {0} $expand segment(s), but the maximum allowed is {1}."
        /// </summary>
        internal static string DataService_ExpandCountExceeded(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ExpandCountExceeded, p0, p1);
        }

        /// <summary>
        /// A string like "Method '{0}' on type '{1}' is marked as a query interceptor method but has {2} parameters. Query interceptors should take no parameters."
        /// </summary>
        internal static string DataService_QueryInterceptorIncorrectParameterCount(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_QueryInterceptorIncorrectParameterCount, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Entity set '{0}' declared on attribute for method '{1}' of type '{2}' was not found."
        /// </summary>
        internal static string DataService_AttributeEntitySetNotFound(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AttributeEntitySetNotFound, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Method '{0}' on type '{1}' us marked as an authorization method but has a return type of '{2}' when no return value is expected."
        /// </summary>
        internal static string DataService_AuthorizationMethodNotVoid(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationMethodNotVoid, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Method '{0}' on type '{1}' returns no value, but a '{2}' return type is required for a query interceptor method."
        /// </summary>
        internal static string DataService_AuthorizationMethodVoid(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationMethodVoid, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Parameter '{0}' of method '{1}' on type '{2}' is of type '{3}' but a type assignable from '{4}' is required."
        /// </summary>
        internal static string DataService_AuthorizationParameterNotAssignable(object p0, object p1, object p2, object p3, object p4)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationParameterNotAssignable, p0, p1, p2, p3, p4);
        }

        /// <summary>
        /// A string like "Parameter '{0}' of method '{1}' on type '{2}' is of type '{3}' but should be of type System.Data.Web.ResourceAction."
        /// </summary>
        internal static string DataService_AuthorizationParameterNotResourceAction(object p0, object p1, object p2, object p3)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationParameterNotResourceAction, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "Method '{0}' on type '{1}' returned a null expression."
        /// </summary>
        internal static string DataService_AuthorizationReturnedNullQuery(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationReturnedNullQuery, p0, p1);
        }

        /// <summary>
        /// A string like "Return type of method '{0}' on type '{1}' is of type '{2}' but a type assignable to '{3}' is required for a query interceptor."
        /// </summary>
        internal static string DataService_AuthorizationReturnTypeNotAssignable(object p0, object p1, object p2, object p3)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_AuthorizationReturnTypeNotAssignable, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "There is no host attached to this service. Call AttachHost to provide a valid host that can provide a request to process."
        /// </summary>
        internal static string DataService_HostNotAttached
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_HostNotAttached);
            }
        }

        /// <summary>
        /// A string like "Not Implemented"
        /// </summary>
        internal static string DataService_NotImplementedException
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_NotImplementedException);
            }
        }

        /// <summary>
        /// A string like "Type '{0}' has a method '{1}' with parameter '{2}' declared as an [Out] parameter. The parameter should be declared as [In] only."
        /// </summary>
        internal static string DataService_ParameterIsOut(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ParameterIsOut, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Both If-Match and If-None-Match HTTP headers cannot be specified at the same time. Please specify either one of the headers or none of them."
        /// </summary>
        internal static string DataService_BothIfMatchAndIfNoneMatchHeaderSpecified
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_BothIfMatchAndIfNoneMatchHeaderSpecified);
            }
        }

        /// <summary>
        /// A string like "If-Match or If-None-Match HTTP headers cannot be specified since the URI '{0}' refers to a collection of resources or has a $count or $link segment or has a $expand as one of the query parameters."
        /// </summary>
        internal static string DataService_ETagCannotBeSpecified(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ETagCannotBeSpecified, p0);
        }

        /// <summary>
        /// A string like "If-Match or If-None-Match HTTP headers cannot be specified for POST operations."
        /// </summary>
        internal static string DataService_ETagSpecifiedForPost
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ETagSpecifiedForPost);
            }
        }

        /// <summary>
        /// A string like "If-Match or If-None-Match HTTP headers cannot be specified for service actions."
        /// </summary>
        internal static string DataService_ETagSpecifiedForServiceAction
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ETagSpecifiedForServiceAction);
            }
        }

        /// <summary>
        /// A string like "If-None-Match HTTP header cannot be specified for DELETE operations."
        /// </summary>
        internal static string DataService_IfNoneMatchHeaderNotSupportedInDelete
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_IfNoneMatchHeaderNotSupportedInDelete);
            }
        }

        /// <summary>
        /// A string like "If-None-Match HTTP header cannot be specified for PUT operations."
        /// </summary>
        internal static string DataService_IfNoneMatchHeaderNotSupportedInPut
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_IfNoneMatchHeaderNotSupportedInPut);
            }
        }

        /// <summary>
        /// A string like "If-Match or If-None-Match HTTP headers cannot be specified for DELETE operations to $link end points."
        /// </summary>
        internal static string DataService_ETagNotSupportedInUnbind
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ETagNotSupportedInUnbind);
            }
        }

        /// <summary>
        /// A string like "Since entity type '{0}' has one or more etag properties, If-Match HTTP header must be specified for DELETE/PUT operations on this type."
        /// </summary>
        internal static string DataService_CannotPerformOperationWithoutETag(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_CannotPerformOperationWithoutETag, p0);
        }

        /// <summary>
        /// A string like "'{0}' is not a valid value for the 'Content-ID' header. In batch requests, the 'Content-ID' header must be an integer value."
        /// </summary>
        internal static string DataService_ContentIdMustBeAnInteger(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ContentIdMustBeAnInteger, p0);
        }

        /// <summary>
        /// A string like "'{0}' is not a unique 'Content-ID' in the given batch requests. In batch requests, every 'Content-ID' must be a unique value."
        /// </summary>
        internal static string DataService_ContentIdMustBeUniqueInBatch(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ContentIdMustBeUniqueInBatch, p0);
        }

        /// <summary>
        /// A string like "The URL representing the root of the service only supports GET requests."
        /// </summary>
        internal static string DataService_OnlyGetOperationSupportedOnServiceUrl
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_OnlyGetOperationSupportedOnServiceUrl);
            }
        }

        /// <summary>
        /// A string like "Request version '{0}' specified for header '{1}' is not valid. Please specify a valid version value."
        /// </summary>
        internal static string DataService_VersionCannotBeParsed(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_VersionCannotBeParsed, p0, p1);
        }

        /// <summary>
        /// A string like "Request version '{0}' is not a valid request version. The only supported versions are {1}."
        /// </summary>
        internal static string DataService_InvalidRequestVersion(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_InvalidRequestVersion, p0, p1);
        }

        /// <summary>
        /// A string like "The OData-Version '{0}' is too low for the request. The lowest supported version is '{1}.{2}'."
        /// </summary>
        internal static string DataService_DSVTooLow(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_DSVTooLow, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The OData-MaxVersion '{0}' is too low for the response. The lowest supported version is '{1}.{2}'."
        /// </summary>
        internal static string DataService_MaxDSVTooLow(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_MaxDSVTooLow, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Request version '{0}' is too high for the server. Please specify a request version less than or equal to '{1}'."
        /// </summary>
        internal static string DataService_RequestVersionMustBeLessThanMPV(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_RequestVersionMustBeLessThanMPV, p0, p1);
        }

        /// <summary>
        /// A string like "Since '{0}' is a key property, it cannot be updated."
        /// </summary>
        internal static string DataService_CannotUpdateKeyProperties(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_CannotUpdateKeyProperties, p0);
        }

        /// <summary>
        /// A string like "The etag value '{0}' specified in one of the request headers is not valid. Please make sure only one etag value is specified and is valid."
        /// </summary>
        internal static string DataService_ETagValueNotValid(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_ETagValueNotValid, p0);
        }

        /// <summary>
        /// A string like "Specification of entity set page size is mutually exclusive with the specification of MaxResultsPerCollection property in configuration."
        /// </summary>
        internal static string DataService_SDP_PageSizeWithMaxResultsPerCollection
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_SDP_PageSizeWithMaxResultsPerCollection);
            }
        }

        /// <summary>
        /// A string like "The given page size {0} for entity set '{1}' should have a non-negative value."
        /// </summary>
        internal static string DataService_SDP_PageSizeMustbeNonNegative(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_SDP_PageSizeMustbeNonNegative, p0, p1);
        }

        /// <summary>
        /// A string like "The number of keys '{0}' in skip token with value '{1}' did not match the number of ordering constraints '{2}' for the resource type."
        /// </summary>
        internal static string DataService_SDP_SkipTokenNotMatchingOrdering(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_SDP_SkipTokenNotMatchingOrdering, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The request could not be processed because the data service implements IExpandProvider, which does not support the paging of top-level results."
        /// </summary>
        internal static string DataService_SDP_TopLevelPagedResultWithOldExpandProvider
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_SDP_TopLevelPagedResultWithOldExpandProvider);
            }
        }

        /// <summary>
        /// A string like "Adding types through RegisterKnownType() is not supported for providers instantiated by the user."
        /// </summary>
        internal static string DataService_RegisterKnownTypeNotAllowedForIDSP
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_RegisterKnownTypeNotAllowedForIDSP);
            }
        }

        /// <summary>
        /// A string like "The request could not be processed because the data service implements IExpandProvider, which does not support projection."
        /// </summary>
        internal static string DataService_Projections_ProjectionsWithOldExpandProvider
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_Projections_ProjectionsWithOldExpandProvider);
            }
        }

        /// <summary>
        /// A string like "For custom providers, if GetService returns non-null for IDataServiceMetadataProvider, it must not return null for IDataServiceQueryProvider."
        /// </summary>
        internal static string DataService_IDataServiceQueryProviderNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_IDataServiceQueryProviderNull);
            }
        }

        /// <summary>
        /// A string like "Update operations are not supported for '$ref' end points that refer to collection properties."
        /// </summary>
        internal static string DataService_CannotUpdateSetReferenceLinks
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_CannotUpdateSetReferenceLinks);
            }
        }

        /// <summary>
        /// A string like "The request could not be processed because the data service implements IExpandProvider, which does not support expansions on derived navigation properties."
        /// </summary>
        internal static string DataService_DerivedExpansions_OldExpandProvider
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_DerivedExpansions_OldExpandProvider);
            }
        }

        /// <summary>
        /// A string like "The resourceProperty parameter must be a navigation property on the resource type specified by the resourceType parameter."
        /// </summary>
        internal static string ResourceAssociationSetEnd_ResourcePropertyMustBeNavigationPropertyOnResourceType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSetEnd_ResourcePropertyMustBeNavigationPropertyOnResourceType);
            }
        }

        /// <summary>
        /// A string like "The resourceType parameter must be a type that is assignable to the resource set specified by the resourceSet parameter."
        /// </summary>
        internal static string ResourceAssociationSetEnd_ResourceTypeMustBeAssignableToResourceSet
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSetEnd_ResourceTypeMustBeAssignableToResourceSet);
            }
        }

        /// <summary>
        /// A string like "The resource type '{0}' specified for the ResourceAssociationSetEnd is not valid for resource property '{1}'. The resource type must be the declaring type of the property."
        /// </summary>
        internal static string ResourceAssociationSetEnd_ResourceTypeMustBeTheDeclaringType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSetEnd_ResourceTypeMustBeTheDeclaringType, p0, p1);
        }

        /// <summary>
        /// A string like "The ResourceProperty of the ResourceAssociationEnds cannot both be null."
        /// </summary>
        internal static string ResourceAssociationSet_ResourcePropertyCannotBeBothNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSet_ResourcePropertyCannotBeBothNull);
            }
        }

        /// <summary>
        /// A string like "When the ResourceAssociationSet is bidirectional, IDataServiceMetadataProvider.GetResourceAssociationSet() must return the same ResourceAssociationSet when call from both ends."
        /// </summary>
        internal static string ResourceAssociationSet_BidirectionalAssociationMustReturnSameResourceAssociationSetFromBothEnd
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSet_BidirectionalAssociationMustReturnSameResourceAssociationSetFromBothEnd);
            }
        }

        /// <summary>
        /// A string like "The ends of a ResourceAssociationSet cannot both have the same ResourceType and ResourceProperty values. If this is a self-referencing association, the ResourceAssociationSet must be unidirectional with the ResourceProperty on one of the ends set to null."
        /// </summary>
        internal static string ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSet_SelfReferencingAssociationCannotBeBiDirectional);
            }
        }

        /// <summary>
        /// A string like "ResourceAssociationSets '{0}' and '{1}' have a ResourceAssociationSetEnd referring to the same EntitySet '{2}' through the same AssociationType. Make sure that if two or more AssociationSets refer to the same AssociationType, the ends must not refer to the same EntitySet. For CLR context, this could happen if multiple entity sets have entity types that have a common ancestor and the ancestor has a property of derived entity types."
        /// </summary>
        internal static string ResourceAssociationSet_MultipleAssociationSetsForTheSameAssociationTypeMustNotReferToSameEndSets(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ResourceAssociationSet_MultipleAssociationSetsForTheSameAssociationTypeMustNotReferToSameEndSets, p0, p1, p2);
        }

        /// <summary>
        /// A string like "No visible ResourceAssociationSet found for navigation property '{0}' on type '{1}'. There must be at least one ResourceAssociationSet for each navigation property."
        /// </summary>
        internal static string MetadataSerializer_NoResourceAssociationSetForNavigationProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataSerializer_NoResourceAssociationSetForNavigationProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Resource type '{0}' was registered by DataServiceConfiguration.EnableAccess(), however it no longer exists in the data service provider."
        /// </summary>
        internal static string MetadataSerializer_AccessEnabledTypeNoLongerExists(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataSerializer_AccessEnabledTypeNoLongerExists, p0);
        }

        /// <summary>
        /// A string like "An IEdmModel instance was found that failed validation. The following errors were reported:\r\n{0}"
        /// </summary>
        internal static string MetadataSerializer_ModelValidationErrors(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataSerializer_ModelValidationErrors, p0);
        }

        /// <summary>
        /// A string like "There is no HTTP method specified by the host."
        /// </summary>
        internal static string DataServiceHost_EmptyHttpMethod
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceHost_EmptyHttpMethod);
            }
        }

        /// <summary>
        /// A string like "The RequestHeaders property of the data service host cannot be null."
        /// </summary>
        internal static string DataServiceHost_RequestHeadersCannotBeNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceHost_RequestHeadersCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The ResponseHeaders property of the data service host cannot be null."
        /// </summary>
        internal static string DataServiceHost_ResponseHeadersCannotBeNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceHost_ResponseHeadersCannotBeNull);
            }
        }

        /// <summary>
        /// A string like "The data service cannot access the requested information. To be able to access this information, the process that hosts the data service must implement the IDataServiceHost2 interface."
        /// </summary>
        internal static string DataServiceHost_FeatureRequiresIDataServiceHost2
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceHost_FeatureRequiresIDataServiceHost2);
            }
        }

        /// <summary>
        /// A string like "IDataServiceStreamProvider.GetWriteStream() must return a valid writable stream."
        /// </summary>
        internal static string DataService_InvalidStreamFromGetWriteStream
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_InvalidStreamFromGetWriteStream);
            }
        }

        /// <summary>
        /// A string like "IDataServiceStreamProvider.GetReadStream() must return a valid readable stream."
        /// </summary>
        internal static string DataService_InvalidStreamFromGetReadStream
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataService_InvalidStreamFromGetReadStream);
            }
        }

        /// <summary>
        /// A string like "No changes are allowed to the configuration after '{0}' is invoked."
        /// </summary>
        internal static string DataServiceConfiguration_NoChangesAllowed(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_NoChangesAllowed, p0);
        }

        /// <summary>
        /// A string like "The given name '{0}' was not found in the entity sets."
        /// </summary>
        internal static string DataServiceConfiguration_ResourceSetNameNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_ResourceSetNameNotFound, p0);
        }

        /// <summary>
        /// A string like "The given name '{0}' was not found in the entity types."
        /// </summary>
        internal static string DataServiceConfiguration_ResourceTypeNameNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_ResourceTypeNameNotFound, p0);
        }

        /// <summary>
        /// A string like "The given name '{0}' was not found in the service operations."
        /// </summary>
        internal static string DataServiceConfiguration_ServiceNameNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_ServiceNameNotFound, p0);
        }

        /// <summary>
        /// A string like "The given type '{0}' is not a complex type."
        /// </summary>
        internal static string DataServiceConfiguration_NotComplexType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_NotComplexType, p0);
        }

        /// <summary>
        /// A string like "The ability of the data service to return row count information is disabled. To enable this functionality, set the DataServiceConfiguration.AcceptCountRequests property to true."
        /// </summary>
        internal static string DataServiceConfiguration_CountNotAccepted
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_CountNotAccepted);
            }
        }

        /// <summary>
        /// A string like "The ability to use the $select query option to define a projection in a data service query is disabled. To enable this functionality, set the DataServiceConfiguration. AcceptProjectionRequests property to true."
        /// </summary>
        internal static string DataServiceConfiguration_ProjectionsNotAccepted
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_ProjectionsNotAccepted);
            }
        }

        /// <summary>
        /// A string like "The response requires that version {0} of the protocol be used, but the MaxProtocolVersion of the data service is set to {1}."
        /// </summary>
        internal static string DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion, p0, p1);
        }

        /// <summary>
        /// A string like "An error occurred while processing this request."
        /// </summary>
        internal static string DataServiceException_GeneralError
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceException_GeneralError);
            }
        }

        /// <summary>
        /// A string like "Unsupported media type requested."
        /// </summary>
        internal static string DataServiceException_UnsupportedMediaType
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceException_UnsupportedMediaType);
            }
        }

        /// <summary>
        /// A string like "More than one query parameter specified with the name '{0}' in request URI '{1}'."
        /// </summary>
        internal static string DataServiceHost_MoreThanOneQueryParameterSpecifiedWithTheGivenName(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceHost_MoreThanOneQueryParameterSpecifiedWithTheGivenName, p0, p1);
        }

        /// <summary>
        /// A string like "Value cannot be null or empty."
        /// </summary>
        internal static string WebUtil_ArgumentNullOrEmpty
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.WebUtil_ArgumentNullOrEmpty);
            }
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid for POST operation. For POST operations, the URI must refer to a service operation or an entity set."
        /// </summary>
        internal static string BadRequest_InvalidUriForPostOperation(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriForPostOperation, p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid for PUT operation. The URI must point to a single resource for PUT operations."
        /// </summary>
        internal static string BadRequest_InvalidUriForPutOperation(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriForPutOperation, p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid for PATCH operation. The URI must point to a single resource for PATCH operations. PATCH operation against a media resource is not supported."
        /// </summary>
        internal static string BadRequest_InvalidUriForPatchOperation(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriForPatchOperation, p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid for DELETE operation. The URI must refer to a single resource from an entity set or from a property referring to a set of resources."
        /// </summary>
        internal static string BadRequest_InvalidUriForDeleteOperation(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriForDeleteOperation, p0);
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid. The segment before '$value' must be a Media Link Entry or a primitive property."
        /// </summary>
        internal static string BadRequest_InvalidUriForMediaResource(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriForMediaResource, p0);
        }

        /// <summary>
        /// A string like "Error in processing request - there is no request body available."
        /// </summary>
        internal static string BadRequest_NullRequestStream
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_NullRequestStream);
            }
        }

        /// <summary>
        /// A string like "The number of keys specified in the URI does not match number of key properties for the resource '{0}'."
        /// </summary>
        internal static string BadRequest_KeyCountMismatch(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_KeyCountMismatch, p0);
        }

        /// <summary>
        /// A string like "The request is not valid. To set a value to null, the URI must refer to a property whose type is not of value type."
        /// </summary>
        internal static string BadRequest_CannotNullifyValueTypeProperty
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CannotNullifyValueTypeProperty);
            }
        }

        /// <summary>
        /// A string like "$id or any other query option must not be specified for DELETE operation on a single entity reference."
        /// </summary>
        internal static string BadRequest_QueryOptionsShouldNotBeSpecifiedForDeletingSingleEntityReference
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_QueryOptionsShouldNotBeSpecifiedForDeletingSingleEntityReference);
            }
        }

        /// <summary>
        /// A string like "$id query option must be specified for DELETE operation on a collection of entity references."
        /// </summary>
        internal static string BadRequest_IdMustBeSpecifiedForDeletingCollectionOfEntityReference
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_IdMustBeSpecifiedForDeletingCollectionOfEntityReference);
            }
        }

        /// <summary>
        /// A string like "The data source must implement IUpdatable, IDataServiceUpdateProvider or IDataServiceUpdateProvider2 to support updates."
        /// </summary>
        internal static string UpdatableWrapper_MissingIUpdatableForV1Provider
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.UpdatableWrapper_MissingIUpdatableForV1Provider);
            }
        }

        /// <summary>
        /// A string like "The data source must implement IDataServiceUpdateProvider or IDataServiceUpdateProvider2 to support updates."
        /// </summary>
        internal static string UpdatableWrapper_MissingUpdateProviderInterface
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.UpdatableWrapper_MissingUpdateProviderInterface);
            }
        }

        /// <summary>
        /// A string like "To support service actions, the data service must implement IServiceProvider.GetService() to return an implementation of IDataServiceUpdateProvider2 or the data source must implement IDataServiceUpdateProvider2."
        /// </summary>
        internal static string UpdatableWrapper_MustImplementDataServiceUpdateProvider2ToSupportServiceActions
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.UpdatableWrapper_MustImplementDataServiceUpdateProvider2ToSupportServiceActions);
            }
        }

        /// <summary>
        /// A string like "The property '{0}' referred in the request URI is a open property and refers to a collection of resources. Open properties are not allowed to contain collections."
        /// </summary>
        internal static string InvalidUri_OpenPropertiesCannotBeCollection(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.InvalidUri_OpenPropertiesCannotBeCollection, p0);
        }

        /// <summary>
        /// A string like "Property '{0}' requires a non-negative value."
        /// </summary>
        internal static string PropertyRequiresNonNegativeNumber(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.PropertyRequiresNonNegativeNumber, p0);
        }

        /// <summary>
        /// A string like "To support streaming, the data service must implement IServiceProvider.GetService() to return an implementation of IDataServiceStreamProvider or the data source must implement IDataServiceStreamProvider."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_MustImplementIDataServiceStreamProviderToSupportStreaming
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_MustImplementIDataServiceStreamProviderToSupportStreaming);
            }
        }

        /// <summary>
        /// A string like "To support named streams, the data service must implement IServiceProvider.GetService() to return an implementation of IDataServiceStreamProvider2 or the data source must implement IDataServiceStreamProvider2."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_MustImplementDataServiceStreamProvider2ToSupportNamedStreams
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_MustImplementDataServiceStreamProvider2ToSupportNamedStreams);
            }
        }

        /// <summary>
        /// A string like "The method '{0}' must not set the HTTP response headers 'Content-Type' and 'ETag'."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_MustNotSetContentTypeAndEtag(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_MustNotSetContentTypeAndEtag, p0);
        }

        /// <summary>
        /// A string like "The method 'IDataServiceStreamProvider.GetStreamContentType' must not return a null or empty string."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_GetStreamContentTypeReturnsEmptyOrNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_GetStreamContentTypeReturnsEmptyOrNull);
            }
        }

        /// <summary>
        /// A string like "The method 'IDataServiceStreamProvider.GetReadStreamUri' must return an absolute Uri or null."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_GetReadStreamUriMustReturnAbsoluteUriOrNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_GetReadStreamUriMustReturnAbsoluteUriOrNull);
            }
        }

        /// <summary>
        /// A string like "The method 'IDataServiceStreamProvider.GetStreamETag' returned an entity tag with invalid format."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_GetStreamETagReturnedInvalidETagFormat
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_GetStreamETagReturnedInvalidETagFormat);
            }
        }

        /// <summary>
        /// A string like "The method 'IDataServiceStreamProvider.ResolveType' must return a valid resource type name."
        /// </summary>
        internal static string DataServiceStreamProviderWrapper_ResolveTypeMustReturnValidResourceTypeName
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceStreamProviderWrapper_ResolveTypeMustReturnValidResourceTypeName);
            }
        }

        /// <summary>
        /// A string like "Recursion reached allowed limit: '{0}'."
        /// </summary>
        internal static string BadRequest_DeepRecursion(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_DeepRecursion, p0);
        }

        /// <summary>
        /// A string like "Recursion reached allowed limit."
        /// </summary>
        internal static string BadRequest_DeepRecursion_General
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_DeepRecursion_General);
            }
        }

        /// <summary>
        /// A string like "The URI '{0}' is not valid since it is not based on '{1}'."
        /// </summary>
        internal static string BadRequest_RequestUriDoesNotHaveTheRightBaseUri(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_RequestUriDoesNotHaveTheRightBaseUri, p0, p1);
        }

        /// <summary>
        /// A string like "Failed to create absolute URI. The URI '{0}' is not valid because it cannot be based on '{1}'. When the URI for an operation is relative, you must remove all preceding forward slashes."
        /// </summary>
        internal static string BadRequest_RequestUriCannotBeBasedOnBaseUri(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_RequestUriCannotBeBasedOnBaseUri, p0, p1);
        }

        /// <summary>
        /// A string like "A constructor which accepts no parameters was not found for type '{0}'. Every entity type must have a constructor which accepts no parameters."
        /// </summary>
        internal static string NoEmptyConstructorFoundForType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.NoEmptyConstructorFoundForType, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' is an abstract type. For PUT, POST and DELETE operations, the type specified must be a concrete type with a constructor that accepts no parameters."
        /// </summary>
        internal static string CannotCreateInstancesOfAbstractType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.CannotCreateInstancesOfAbstractType, p0);
        }

        /// <summary>
        /// A string like "Could not find a property named '{0}' on type '{1}'."
        /// </summary>
        internal static string BadRequest_InvalidPropertyNameSpecified(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidPropertyNameSpecified, p0, p1);
        }

        /// <summary>
        /// A string like "Error processing request stream. The value of the property '{0}' specified is null and hence you cannot dereference the property."
        /// </summary>
        internal static string BadRequest_DereferencingNullPropertyValue(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_DereferencingNullPropertyValue, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. The property '{0}' represents a collection of resources and hence cannot be set to null."
        /// </summary>
        internal static string BadRequest_CannotSetCollectionsToNull(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CannotSetCollectionsToNull, p0);
        }

        /// <summary>
        /// A string like "$value must be only specified for primitive values. Please make sure that property is a primitive type property."
        /// </summary>
        internal static string BadRequest_ValuesCanBeReturnedForPrimitiveTypesOnly
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_ValuesCanBeReturnedForPrimitiveTypesOnly);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. The URI specified is not valid."
        /// </summary>
        internal static string BadRequest_InvalidUriSpecified
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidUriSpecified);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Deep updates are not supported in PUT or PATCH operations."
        /// </summary>
        internal static string BadRequest_DeepUpdateNotSupported
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_DeepUpdateNotSupported);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Type information must be specified for types that take part in inheritance."
        /// </summary>
        internal static string BadRequest_TypeInformationMustBeSpecifiedForInhertiance
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_TypeInformationMustBeSpecifiedForInhertiance);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. In batch mode, a resource can be cross-referenced only for bind/unbind operations."
        /// </summary>
        internal static string BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Since URI '{0}' points directly to a top level resource, it cannot be set to null."
        /// </summary>
        internal static string BadRequest_CannotSetTopLevelResourceToNull(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CannotSetTopLevelResourceToNull, p0);
        }

        /// <summary>
        /// A string like "Missing URI element. For link operations, URI element must be specified."
        /// </summary>
        internal static string BadRequest_MissingUriForLinkOperation
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_MissingUriForLinkOperation);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Error encountered in setting value for property '{0}'. Please verify that the value is correct."
        /// </summary>
        internal static string BadRequest_ErrorInSettingPropertyValue(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_ErrorInSettingPropertyValue, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. Property '{0}' is a read-only property and cannot be updated. Please make sure that this property is not present in the request payload."
        /// </summary>
        internal static string BadRequest_PropertyValueCannotBeSet(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_PropertyValueCannotBeSet, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. Binding to new entities is not supported in PUT operations."
        /// </summary>
        internal static string BadRequest_CannotUpdateRelatedEntitiesInPut
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CannotUpdateRelatedEntitiesInPut);
            }
        }

        /// <summary>
        /// A string like "The request exceeds the maximum {0} resources that can be referenced in a single POST request."
        /// </summary>
        internal static string BadRequest_ExceedsMaxObjectCountOnInsert(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_ExceedsMaxObjectCountOnInsert, p0);
        }

        /// <summary>
        /// A string like "Collection properties of a collection type are not supported."
        /// </summary>
        internal static string BadRequest_CollectionOfCollectionNotSupported
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CollectionOfCollectionNotSupported);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' specified on collection property '{1}' is not a collection type. Only a collection type can be specified on a collection property."
        /// </summary>
        internal static string BadRequest_CollectionTypeExpected(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CollectionTypeExpected, p0, p1);
        }

        /// <summary>
        /// A string like "An open collection property '{0}' was found. In OData, open collection properties are not supported."
        /// </summary>
        internal static string BadRequest_OpenCollectionProperty(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_OpenCollectionProperty, p0);
        }

        /// <summary>
        /// A string like "The value of a collection property can only be enumerated once."
        /// </summary>
        internal static string CollectionCanOnlyBeEnumeratedOnce
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.CollectionCanOnlyBeEnumeratedOnce);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. JSON text specified is not valid."
        /// </summary>
        internal static string BadRequestStream_InvalidContent
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_InvalidContent);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. The type name '{0}' is not valid."
        /// </summary>
        internal static string BadRequest_InvalidTypeName(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidTypeName, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. The type name '{0}' is not valid for the given URI it represents. The expected type must be '{1}' or one of its derived types."
        /// </summary>
        internal static string BadRequest_InvalidTypeSpecified(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_InvalidTypeSpecified, p0, p1);
        }

        /// <summary>
        /// A string like "Error processing request stream. Missing ',' between array elements."
        /// </summary>
        internal static string BadRequestStream_MissingArrayMemberSeperator
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_MissingArrayMemberSeperator);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. '{0}' is not a valid JSON literal. 'true', 'false' and 'null' are only valid JSON literals. Make sure they are all lower-case."
        /// </summary>
        internal static string BadRequestStream_InvalidKeyword(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_InvalidKeyword, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. In JSON, every object is made up of name/value pairs and the name must be specified."
        /// </summary>
        internal static string BadRequestStream_MissingMemberName
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_MissingMemberName);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. In JSON, every object is made up of name/value pairs and every name/value pair must be separated by ','."
        /// </summary>
        internal static string BadRequestStream_MissingMemberSeperator
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_MissingMemberSeperator);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Missing ':' after '{0}'."
        /// </summary>
        internal static string BadRequestStream_MissingNameValueSeperator(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_MissingNameValueSeperator, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. Either an extra comma is specified at the end or name is an empty string."
        /// </summary>
        internal static string BadRequestStream_InvalidJsonNameSpecifiedOrExtraComma
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_InvalidJsonNameSpecifiedOrExtraComma);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Unrecognized escape sequence found in JSON string."
        /// </summary>
        internal static string BadRequestStream_InvalidJsonUnrecognizedEscapeSequence
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequestStream_InvalidJsonUnrecognizedEscapeSequence);
            }
        }

        /// <summary>
        /// A string like "Error processing request stream. Element '{0}' cannot have child elements or a value specified if it has a null attribute with true value."
        /// </summary>
        internal static string BadRequest_CannotSpecifyValueOrChildElementsForNullElement(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_CannotSpecifyValueOrChildElementsForNullElement, p0);
        }

        /// <summary>
        /// A string like "Error processing request stream. Since the property '{0}' refers to a reference property, the URI specified in the href attribute must refer to a single resource."
        /// </summary>
        internal static string BadRequest_LinkHrefMustReferToSingleResource(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadRequest_LinkHrefMustReferToSingleResource, p0);
        }

        /// <summary>
        /// A string like "Entity set '{0}' has rights '{1}' applied to it, but entity set '{2}' has rights '{3}'. Entity sets of the same type must have the same rights."
        /// </summary>
        internal static string ObjectContext_DifferentContainerRights(object p0, object p1, object p2, object p3)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_DifferentContainerRights, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "HasStream attribute for entity type '{0}' cannot be empty."
        /// </summary>
        internal static string ObjectContext_HasStreamAttributeEmpty(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_HasStreamAttributeEmpty, p0);
        }

        /// <summary>
        /// A string like "Navigation property '{0}' on type '{1}' cannot be used because the entity set '{2}' does not have an association set specified for it."
        /// </summary>
        internal static string ObjectContext_NavigationPropertyUnbound(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_NavigationPropertyUnbound, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Unable to load metadata for type '{0}'. The mapped OSpace type was not found. Please make sure you register the mapped OSpace type."
        /// </summary>
        internal static string ObjectContext_UnableToLoadMetadataForType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_UnableToLoadMetadataForType, p0);
        }

        /// <summary>
        /// A string like "Unsupported value '{0}' for HasStream attribute on entity type '{1}'."
        /// </summary>
        internal static string ObjectContext_UnsupportedStreamProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_UnsupportedStreamProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is of type '{2}' which is not a supported primitive type."
        /// </summary>
        internal static string ObjectContext_PrimitiveTypeNotSupported(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContext_PrimitiveTypeNotSupported, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Property '{0}' and '{1}' are IQueryable of types '{2}' and '{3}' and type '{2}' is an ancestor for type '{3}'. Please make sure that there is only one IQueryable property for each type hierarchy."
        /// </summary>
        internal static string ReflectionProvider_MultipleEntitySetsForSameType(object p0, object p1, object p2, object p3)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_MultipleEntitySetsForSameType, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is not a valid property. Make sure that the type of the property is a public type and a supported primitive type or a entity type with a valid key or a complex type."
        /// </summary>
        internal static string ReflectionProvider_InvalidProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_InvalidProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Type '{0}' has a key defined through an explicit DataServiceKeyAttribute, but it is not exposed through a top-level IQueryable property on the data context."
        /// </summary>
        internal static string ReflectionProvider_EntityTypeHasKeyButNoEntitySet(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_EntityTypeHasKeyButNoEntitySet, p0);
        }

        /// <summary>
        /// A string like "On data context type '{1}', there is a top IQueryable property '{0}' whose element type is not an entity type. Make sure that the IQueryable property is of entity type or specify the IgnoreProperties attribute on the data context type to ignore this property."
        /// </summary>
        internal static string ReflectionProvider_InvalidEntitySetProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_InvalidEntitySetProperty, p0, p1);
        }

        /// <summary>
        /// A string like "Type '{0}' has property '{1}' of entity type. Either this property has no corresponding entity set in the data context or one of its inherited types has a corresponding entity set. Specify IgnoreProperties attribute on the entity type for this property or use a property type that has a corresponding entity set in the data context."
        /// </summary>
        internal static string ReflectionProvider_EntityPropertyWithNoEntitySet(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_EntityPropertyWithNoEntitySet, p0, p1);
        }

        /// <summary>
        /// A string like "Key properties cannot be ignored using the IgnoreProperties attribute. For type '{0}', please make sure that there is a key property which is not ignored."
        /// </summary>
        internal static string ReflectionProvider_KeyPropertiesCannotBeIgnored(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_KeyPropertiesCannotBeIgnored, p0);
        }

        /// <summary>
        /// A string like "The property name'{0}' specified in the ETagAttribute on type '{1}' is not a valid property name. Please specify a valid property name."
        /// </summary>
        internal static string ReflectionProvider_ETagPropertyNameNotValid(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_ETagPropertyNameNotValid, p0, p1);
        }

        /// <summary>
        /// A string like "Cannot apply the HasStreamAttribute to type '{0}'. HasStreamAttribute is only applicable to entity types."
        /// </summary>
        internal static string ReflectionProvider_HasStreamAttributeOnlyAppliesToEntityType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_HasStreamAttributeOnlyAppliesToEntityType, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' on a complex type '{1}' is not a valid property. Navigation properties are not supported on complex types."
        /// </summary>
        internal static string ReflectionProvider_ComplexTypeWithNavigationProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_ComplexTypeWithNavigationProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on a type '{1}' is not a valid property. A collection property that contains collection types is not supported."
        /// </summary>
        internal static string ReflectionProvider_CollectionOfCollectionProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_CollectionOfCollectionProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' on type '{1}' is a collection property with unsupported item type '{2}'. Only primitive types and complex types are valid item types for a collection property."
        /// </summary>
        internal static string ReflectionProvider_CollectionOfUnsupportedTypeProperty(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_CollectionOfUnsupportedTypeProperty, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The CLR Type '{0}' has no public properties and is not a supported resource type."
        /// </summary>
        internal static string ReflectionProvider_ResourceTypeHasNoPublicallyVisibleProperties(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ReflectionProvider_ResourceTypeHasNoPublicallyVisibleProperties, p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' that the URI refers to does not match with the expected entity type '{1}'."
        /// </summary>
        internal static string TargetElementTypeOfTheUriSpecifiedDoesNotMatchWithTheExpectedType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.TargetElementTypeOfTheUriSpecifiedDoesNotMatchWithTheExpectedType, p0, p1);
        }

        /// <summary>
        /// A string like "A corresponding ObjectSpace type was not found for the type '{0}'. Please ensure that the ObjectContext or DbContext is valid."
        /// </summary>
        internal static string ObjectContextServiceProvider_OSpaceTypeNotFound(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ObjectContextServiceProvider_OSpaceTypeNotFound, p0);
        }

        /// <summary>
        /// A string like "Internal Server Error. ResolveResource returned null value. Please contact the provider."
        /// </summary>
        internal static string BadProvider_ResolveResourceReturnedNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_ResolveResourceReturnedNull);
            }
        }

        /// <summary>
        /// A string like "Internal Server Error. CreateResource returned null value. Please contact the provider."
        /// </summary>
        internal static string BadProvider_CreateResourceReturnedNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_CreateResourceReturnedNull);
            }
        }

        /// <summary>
        /// A string like "Internal Server Error. ResetResource returned null value. Please contact the provider."
        /// </summary>
        internal static string BadProvider_ResetResourceReturnedNull
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_ResetResourceReturnedNull);
            }
        }

        /// <summary>
        /// A string like "Internal Server Error. The type '{0}' is not a complex type or an entity type."
        /// </summary>
        internal static string BadProvider_InvalidTypeSpecified(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_InvalidTypeSpecified, p0);
        }

        /// <summary>
        /// A string like "Internal Server Error. Unable to find property '{1}' on type '{0}'. Please make sure that the property name is correct."
        /// </summary>
        internal static string BadProvider_UnableToGetPropertyInfo(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnableToGetPropertyInfo, p0, p1);
        }

        /// <summary>
        /// A string like "Internal Server Error. The type '{0}' is not supported."
        /// </summary>
        internal static string BadProvider_UnsupportedType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnsupportedType, p0);
        }

        /// <summary>
        /// A string like "Internal Server Error. The type '{0}' derives from '{1}' which is an unsupported type."
        /// </summary>
        internal static string BadProvider_UnsupportedAncestorType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnsupportedAncestorType, p0, p1);
        }

        /// <summary>
        /// A string like "Internal Server Error. The property '{0}' is of type '{1}' which is an unsupported type."
        /// </summary>
        internal static string BadProvider_UnsupportedPropertyType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnsupportedPropertyType, p0, p1);
        }

        /// <summary>
        /// A string like "Internal Server Error. The type '{0}' has inconsistent metadata and runtime type info."
        /// </summary>
        internal static string BadProvider_InconsistentEntityOrComplexTypeUsage(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_InconsistentEntityOrComplexTypeUsage, p0);
        }

        /// <summary>
        /// A string like "The resource set '{0}' is not known to the provider."
        /// </summary>
        internal static string BadProvider_UnknownResourceSet(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnknownResourceSet, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' is not known to the provider."
        /// </summary>
        internal static string BadProvider_UnknownResourceType(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_UnknownResourceType, p0);
        }

        /// <summary>
        /// A string like "The resource type '{0}' must contain the resource property instance '{1}'."
        /// </summary>
        internal static string BadProvider_ResourceTypeMustBeDeclaringTypeForProperty(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_ResourceTypeMustBeDeclaringTypeForProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The resource property '{0}' must be a navigation property on the resource type '{1}'."
        /// </summary>
        internal static string BadProvider_PropertyMustBeNavigationPropertyOnType(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.BadProvider_PropertyMustBeNavigationPropertyOnType, p0, p1);
        }

        /// <summary>
        /// A string like "The request URI and service URI cannot be modified after the OnStartProcessingRequest method has returned."
        /// </summary>
        internal static string AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly);
            }
        }

        /// <summary>
        /// A string like "The query string of the request URI cannot be modified in the OnStartProcessingRequest method."
        /// </summary>
        internal static string AstoriaRequestMessage_CannotChangeQueryString
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.AstoriaRequestMessage_CannotChangeQueryString);
            }
        }

        /// <summary>
        /// A string like "The service URI cannot be modified when processing a request inside a $batch payload. The ProcessRequestArgs.IsBatchOperation property will be true when inside a $batch payload."
        /// </summary>
        internal static string DataServiceOperationContext_CannotModifyServiceUriInsideBatch
        {
            get
            {
                return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.DataServiceOperationContext_CannotModifyServiceUriInsideBatch);
            }
        }

        /// <summary>
        /// A string like "The primitive type kind '{0}' is not supported."
        /// </summary>
        internal static string MetadataProviderUtils_UnsupportedPrimitiveTypeKind(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderUtils_UnsupportedPrimitiveTypeKind, p0);
        }

        /// <summary>
        /// A string like "Found invalid value for facet '{0}'. Cannot convert a value of type '{1}' to the expected type '{2}'."
        /// </summary>
        internal static string MetadataProviderUtils_ConversionError(object p0, object p1, object p2)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderUtils_ConversionError, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The name '{0}' of a property is invalid; property names must not contain any of the reserved characters {1}."
        /// </summary>
        internal static string MetadataProviderUtils_PropertiesMustNotContainReservedChars(object p0, object p1)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderUtils_PropertiesMustNotContainReservedChars, p0, p1);
        }

        /// <summary>
        /// A string like "An attempt was made to convert a type of kind '{0}' to a schema type. This is invalid since the type kind '{0}' is not valid for schema types."
        /// </summary>
        internal static string MetadataProviderEdmModel_UnsupportedSchemaTypeKind(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderEdmModel_UnsupportedSchemaTypeKind, p0);
        }

        /// <summary>
        /// A string like "The resource type kind '{0}' is not supported."
        /// </summary>
        internal static string MetadataProviderEdmModel_UnsupportedResourceTypeKind(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderEdmModel_UnsupportedResourceTypeKind, p0);
        }

        /// <summary>
        /// A string like "The resource type kind '{0}' is not supported as the type kind of a collection item type. Only 'Primitive' and 'ComplexType' kinds are supported for collection item types."
        /// </summary>
        internal static string MetadataProviderEdmModel_UnsupportedCollectionItemType_PrimitiveOrComplex(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderEdmModel_UnsupportedCollectionItemType_PrimitiveOrComplex, p0);
        }

        /// <summary>
        /// A string like "The resource type kind '{0}' is not supported as the type kind of a collection item type. Only 'Primitive', 'ComplexType' and 'EntityType' kinds are supported for collection item types."
        /// </summary>
        internal static string MetadataProviderEdmModel_UnsupportedCollectionItemType_EntityPrimitiveOrComplex(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderEdmModel_UnsupportedCollectionItemType_EntityPrimitiveOrComplex, p0);
        }

        /// <summary>
        /// A string like "The resource property kind '{0}' is not supported."
        /// </summary>
        internal static string MetadataProviderEdmModel_UnsupportedResourcePropertyKind(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.MetadataProviderEdmModel_UnsupportedResourcePropertyKind, p0);
        }

        /// <summary>
        /// A string like "An internal error '{0}' occurred."
        /// </summary>
        internal static string General_InternalError(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.General_InternalError, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid duration value."
        /// </summary>
        internal static string ValueParser_InvalidDuration(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.ValueParser_InvalidDuration, p0);
        }

        /// <summary>
        /// A string like "The time zone information is missing on the DateTimeOffset value '{0}'. A DateTimeOffset value must contain the time zone information."
        /// </summary>
        internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0)
        {
            return Microsoft.OData.Service.TextRes.GetString(Microsoft.OData.Service.TextRes.PlatformHelper_DateTimeOffsetMustContainTimeZone, p0);
        }

    }

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {

        /// <summary>
        /// The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.
        /// </summary>
        internal static Exception ArgumentNull(string paramName)
        {
            return new ArgumentNullException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
        /// </summary>
        internal static Exception ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the author has not yet implemented the logic at this point in the program. This can act as an exception based TODO tag.
        /// </summary>
        internal static Exception NotImplemented()
        {
            return new NotImplementedException();
        }

        /// <summary>
        /// The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality.
        /// </summary>
        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }
    }
}
