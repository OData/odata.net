//---------------------------------------------------------------------
// <copyright file="TripPinInMemoryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.TripPin
{
    using System;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Services.ODataWCFService.Vocabularies;

    public static class TripPinInMemoryModel
    {
        public static IEdmModel CreateTripPinServiceModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "DefaultContainer");
            model.AddElement(defaultContainer);

            var genderType = new EdmEnumType(ns, "PersonGender", isFlags: false);
            genderType.AddMember("Male", new EdmEnumMemberValue(0));
            genderType.AddMember("Female", new EdmEnumMemberValue(1));
            genderType.AddMember("Unknown", new EdmEnumMemberValue(2));
            model.AddElement(genderType);

            var cityType = new EdmComplexType(ns, "City");
            cityType.AddProperty(new EdmStructuralProperty(cityType, "CountryRegion", EdmCoreModel.Instance.GetString(false)));
            cityType.AddProperty(new EdmStructuralProperty(cityType, "Name", EdmCoreModel.Instance.GetString(false)));
            cityType.AddProperty(new EdmStructuralProperty(cityType, "Region", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(cityType);

            var locationType = new EdmComplexType(ns, "Location", null, false, true);
            locationType.AddProperty(new EdmStructuralProperty(locationType, "Address", EdmCoreModel.Instance.GetString(false)));
            locationType.AddProperty(new EdmStructuralProperty(locationType, "City", new EdmComplexTypeReference(cityType, false)));
            model.AddElement(locationType);

            var eventLocationType = new EdmComplexType(ns, "EventLocation", locationType, false, true);
            eventLocationType.AddProperty(new EdmStructuralProperty(eventLocationType, "BuildingInfo", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(eventLocationType);

            var airportLocationType = new EdmComplexType(ns, "AirportLocation", locationType, false, true);
            airportLocationType.AddProperty(new EdmStructuralProperty(airportLocationType, "Loc", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false)));
            model.AddElement(airportLocationType);

            var photoType = new EdmEntityType(ns, "Photo", null, false, false, true);
            var photoIdProperty = new EdmStructuralProperty(photoType, "Id", EdmCoreModel.Instance.GetInt64(false));
            photoType.AddProperty(photoIdProperty);
            photoType.AddKeys(photoIdProperty);
            photoType.AddProperty(new EdmStructuralProperty(photoType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(photoType);

            var photoSet = new EdmEntitySet(defaultContainer, "Photos", photoType);
            defaultContainer.AddElement(photoSet);

            var personType = new EdmEntityType(ns, "Person", null, false, /* isOpen */ true);
            var personIdProperty = new EdmStructuralProperty(personType, "UserName", EdmCoreModel.Instance.GetString(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(personIdProperty);
            personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "LastName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Emails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "AddressInfo", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(locationType, true)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "Gender", new EdmEnumTypeReference(genderType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Concurrency", EdmCoreModel.Instance.GetInt64(false)));
            model.AddElement(personType);

            var personSet = new EdmEntitySet(defaultContainer, "People", personType);
            defaultContainer.AddElement(personSet);

            var airlineType = new EdmEntityType(ns, "Airline");
            var airlineCodeProp = new EdmStructuralProperty(airlineType, "AirlineCode", EdmCoreModel.Instance.GetString(false));
            airlineType.AddKeys(airlineCodeProp);
            airlineType.AddProperty(airlineCodeProp);
            airlineType.AddProperty(new EdmStructuralProperty(airlineType, "Name", EdmCoreModel.Instance.GetString(false)));

            model.AddElement(airlineType);
            var airlineSet = new EdmEntitySet(defaultContainer, "Airlines", airlineType);
            defaultContainer.AddElement(airlineSet);

            var airportType = new EdmEntityType(ns, "Airport");
            var airportIdType = new EdmStructuralProperty(airportType, "IcaoCode", EdmCoreModel.Instance.GetString(false));
            airportType.AddProperty(airportIdType);
            airportType.AddKeys(airportIdType);
            airportType.AddProperty(new EdmStructuralProperty(airportType, "Name", EdmCoreModel.Instance.GetString(false)));
            airportType.AddProperty(new EdmStructuralProperty(airportType, "IataCode", EdmCoreModel.Instance.GetString(false)));
            airportType.AddProperty(new EdmStructuralProperty(airportType, "Location", new EdmComplexTypeReference(airportLocationType, false)));
            model.AddElement(airportType);

            var airportSet = new EdmEntitySet(defaultContainer, "Airports", airportType);
            defaultContainer.AddElement(airportSet);

            var planItemType = new EdmEntityType(ns, "PlanItem");
            var planItemIdType = new EdmStructuralProperty(planItemType, "PlanItemId", EdmCoreModel.Instance.GetInt32(false));
            planItemType.AddProperty(planItemIdType);
            planItemType.AddKeys(planItemIdType);
            planItemType.AddProperty(new EdmStructuralProperty(planItemType, "ConfirmationCode", EdmCoreModel.Instance.GetString(true)));
            planItemType.AddProperty(new EdmStructuralProperty(planItemType, "StartsAt", EdmCoreModel.Instance.GetDateTimeOffset(true)));
            planItemType.AddProperty(new EdmStructuralProperty(planItemType, "EndsAt", EdmCoreModel.Instance.GetDateTimeOffset(true)));
            planItemType.AddProperty(new EdmStructuralProperty(planItemType, "Duration", EdmCoreModel.Instance.GetDuration(true)));
            model.AddElement(planItemType);

            var publicTransportationType = new EdmEntityType(ns, "PublicTransportation", planItemType);
            publicTransportationType.AddProperty(new EdmStructuralProperty(publicTransportationType, "SeatNumber", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(publicTransportationType);

            var flightType = new EdmEntityType(ns, "Flight", publicTransportationType);
            var flightNumberType = new EdmStructuralProperty(flightType, "FlightNumber", EdmCoreModel.Instance.GetString(false));
            flightType.AddProperty(flightNumberType);
            model.AddElement(flightType);

            var eventType = new EdmEntityType(ns, "Event", planItemType, false, true);
            eventType.AddProperty(new EdmStructuralProperty(eventType, "Description", EdmCoreModel.Instance.GetString(true)));
            eventType.AddProperty(new EdmStructuralProperty(eventType, "OccursAt", new EdmComplexTypeReference(eventLocationType, false)));
            model.AddElement(eventType);

            var tripType = new EdmEntityType(ns, "Trip");
            var tripIdType = new EdmStructuralProperty(tripType, "TripId", EdmCoreModel.Instance.GetInt32(false));
            tripType.AddProperty(tripIdType);
            tripType.AddKeys(tripIdType);
            tripType.AddProperty(new EdmStructuralProperty(tripType, "ShareId", EdmCoreModel.Instance.GetGuid(true)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "Description", EdmCoreModel.Instance.GetString(true)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "Name", EdmCoreModel.Instance.GetString(false)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "Budget", EdmCoreModel.Instance.GetSingle(false)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "StartsAt", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "EndsAt", EdmCoreModel.Instance.GetDateTimeOffset(false)));
            tripType.AddProperty(new EdmStructuralProperty(tripType, "Tags", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            model.AddElement(tripType);

            var friendsnNavigation = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Friends",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var personTripNavigation = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Trips",
                Target = tripType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true
            });

            var personPhotoNavigation = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Photo",
                Target = photoType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
            });

            var tripPhotosNavigation = tripType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Photos",
                Target = photoType,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

            var tripItemNavigation = tripType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "PlanItems",
                Target = planItemType,
                ContainsTarget = true,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var flightFromAirportNavigation = flightType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "From",
                Target = airportType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var flightToAirportNavigation = flightType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "To",
                Target = airportType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var flightAirlineNavigation = flightType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Airline",
                Target = airlineType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var me = new EdmSingleton(defaultContainer, "Me", personType);
            defaultContainer.AddElement(me);

            personSet.AddNavigationTarget(friendsnNavigation, personSet);
            me.AddNavigationTarget(friendsnNavigation, personSet);

            var path = new EdmPathExpression("Trips/PlanItems/Microsoft.OData.SampleService.Models.TripPin.Flight/Airline");
            personSet.AddNavigationTarget(flightAirlineNavigation, airlineSet, path);
            me.AddNavigationTarget(flightAirlineNavigation, airlineSet, path);

            path = new EdmPathExpression("Trips/PlanItems/Microsoft.OData.SampleService.Models.TripPin.Flight/From");
            personSet.AddNavigationTarget(flightFromAirportNavigation, airportSet, path);
            me.AddNavigationTarget(flightFromAirportNavigation, airportSet, path);

            path = new EdmPathExpression("Trips/PlanItems/Microsoft.OData.SampleService.Models.TripPin.Flight/To");
            personSet.AddNavigationTarget(flightToAirportNavigation, airportSet, path);
            me.AddNavigationTarget(flightToAirportNavigation, airportSet, path);

            personSet.AddNavigationTarget(personPhotoNavigation, photoSet);
            me.AddNavigationTarget(personPhotoNavigation, photoSet);

            path = new EdmPathExpression("Trips/Photos");
            personSet.AddNavigationTarget(tripPhotosNavigation, photoSet, path);
            me.AddNavigationTarget(tripPhotosNavigation, photoSet, path);

            var getFavoriteAirlineFunction = new EdmFunction(ns, "GetFavoriteAirline",
                new EdmEntityTypeReference(airlineType, false), true,
                new EdmPathExpression("person/Trips/PlanItems/Microsoft.OData.SampleService.Models.TripPin.Flight/Airline"), true);
            getFavoriteAirlineFunction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            model.AddElement(getFavoriteAirlineFunction);

            var getInvolvedPeopleFunction = new EdmFunction(ns, "GetInvolvedPeople",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(personType, false))), true, null, true);
            getInvolvedPeopleFunction.AddParameter("trip", new EdmEntityTypeReference(tripType, false));
            model.AddElement(getInvolvedPeopleFunction);

            var getFriendsTripsFunction = new EdmFunction(ns, "GetFriendsTrips",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(tripType, false))),
                true, new EdmPathExpression("person/Friends/Trips"), true);
            getFriendsTripsFunction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            getFriendsTripsFunction.AddParameter("userName", EdmCoreModel.Instance.GetString(false));
            model.AddElement(getFriendsTripsFunction);

            var getNearestAirport = new EdmFunction(ns, "GetNearestAirport",
                new EdmEntityTypeReference(airportType, false),
                false, null, true);
            getNearestAirport.AddParameter("lat", EdmCoreModel.Instance.GetDouble(false));
            getNearestAirport.AddParameter("lon", EdmCoreModel.Instance.GetDouble(false));
            model.AddElement(getNearestAirport);
            var getNearestAirportFunctionImport = (IEdmFunctionImport)defaultContainer.AddFunctionImport("GetNearestAirport", getNearestAirport, new EdmPathExpression("Airports"), true);

            var resetDataSourceAction = new EdmAction(ns, "ResetDataSource", null, false, null);
            model.AddElement(resetDataSourceAction);
            defaultContainer.AddActionImport(resetDataSourceAction);

            var shareTripAction = new EdmAction(ns, "ShareTrip", null, true, null);
            shareTripAction.AddParameter("person", new EdmEntityTypeReference(personType, false));
            shareTripAction.AddParameter("userName", EdmCoreModel.Instance.GetString(false));
            shareTripAction.AddParameter("tripId", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(shareTripAction);

            model.SetDescriptionAnnotation(defaultContainer, "TripPin service is a sample service for OData V4.");
            model.SetOptimisticConcurrencyAnnotation(personSet, personType.StructuralProperties().Where(p => p.Name == "Concurrency"));
            // TODO: currently singleton does not support ETag feature
            // model.SetOptimisticConcurrencyAnnotation(me, personType.StructuralProperties().Where(p => p.Name == "Concurrency"));
            model.SetResourcePathCoreAnnotation(personSet, "People");
            model.SetResourcePathCoreAnnotation(me, "Me");
            model.SetResourcePathCoreAnnotation(airlineSet, "Airlines");
            model.SetResourcePathCoreAnnotation(airportSet, "Airports");
            model.SetResourcePathCoreAnnotation(photoSet, "Photos");
            model.SetResourcePathCoreAnnotation(getNearestAirportFunctionImport, "Microsoft.OData.SampleService.Models.TripPin.GetNearestAirport");
            model.SetDereferenceableIDsCoreAnnotation(defaultContainer, true);
            model.SetConventionalIDsCoreAnnotation(defaultContainer, true);
            model.SetPermissionsCoreAnnotation(personType.FindProperty("UserName"), CorePermission.Read);
            model.SetPermissionsCoreAnnotation(airlineType.FindProperty("AirlineCode"), CorePermission.Read);
            model.SetPermissionsCoreAnnotation(airportType.FindProperty("IcaoCode"), CorePermission.Read);
            model.SetPermissionsCoreAnnotation(planItemType.FindProperty("PlanItemId"), CorePermission.Read);
            model.SetPermissionsCoreAnnotation(tripType.FindProperty("TripId"), CorePermission.Read);
            model.SetPermissionsCoreAnnotation(photoType.FindProperty("Id"), CorePermission.Read);
            model.SetImmutableCoreAnnotation(airportType.FindProperty("IataCode"), true);
            model.SetComputedCoreAnnotation(personType.FindProperty("Concurrency"), true);
            model.SetAcceptableMediaTypesCoreAnnotation(photoType, new[] { "image/jpeg" });
            model.SetConformanceLevelCapabilitiesAnnotation(defaultContainer, CapabilitiesConformanceLevelType.Advanced);
            model.SetSupportedFormatsCapabilitiesAnnotation(defaultContainer, new[] { "application/json;odata.metadata=full;IEEE754Compatible=false;odata.streaming=true", "application/json;odata.metadata=minimal;IEEE754Compatible=false;odata.streaming=true", "application/json;odata.metadata=none;IEEE754Compatible=false;odata.streaming=true" });
            model.SetAsynchronousRequestsSupportedCapabilitiesAnnotation(defaultContainer, true);
            model.SetBatchContinueOnErrorSupportedCapabilitiesAnnotation(defaultContainer, false);
            model.SetNavigationRestrictionsCapabilitiesAnnotation(personSet, CapabilitiesNavigationType.None, new[] { new Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>(friendsnNavigation, CapabilitiesNavigationType.Recursive) });
            model.SetFilterFunctionsCapabilitiesAnnotation(defaultContainer, new[] { "contains", "endswith", "startswith", "length", "indexof", "substring", "tolower", "toupper", "trim", "concat", "year", "month", "day", "hour", "minute", "second", "round", "floor", "ceiling", "cast", "isof" });
            model.SetSearchRestrictionsCapabilitiesAnnotation(personSet, true, CapabilitiesSearchExpressions.None);
            model.SetSearchRestrictionsCapabilitiesAnnotation(airlineSet, true, CapabilitiesSearchExpressions.None);
            model.SetSearchRestrictionsCapabilitiesAnnotation(airportSet, true, CapabilitiesSearchExpressions.None);
            model.SetSearchRestrictionsCapabilitiesAnnotation(photoSet, true, CapabilitiesSearchExpressions.None);
            model.SetInsertRestrictionsCapabilitiesAnnotation(personSet, true, new[] { personTripNavigation, friendsnNavigation });
            model.SetInsertRestrictionsCapabilitiesAnnotation(airlineSet, true, null);
            model.SetInsertRestrictionsCapabilitiesAnnotation(airportSet, false, null);
            model.SetInsertRestrictionsCapabilitiesAnnotation(photoSet, true, null);
            // TODO: model.SetUpdateRestrictionsCapabilitiesAnnotation();
            model.SetDeleteRestrictionsCapabilitiesAnnotation(airportSet, false, null);
            model.SetISOCurrencyMeasuresAnnotation(tripType.FindProperty("Budget"), "USD");
            model.SetScaleMeasuresAnnotation(tripType.FindProperty("Budget"), 2);

            return model;
        }
    }
}
