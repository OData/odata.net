//---------------------------------------------------------------------
// <copyright file="TripPinServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.SampleService.Models.TripPin;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class TripPinServiceOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<TripPinServiceDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        public Airline GetFavoriteAirline(Person person)
        {
            IEnumerable<Airline> airlines = GetRootQuery("Airlines") as IEnumerable<Airline>;

            Dictionary<string, int> countDict = new Dictionary<string, int>();
            foreach (var a in airlines)
            {
                countDict.Add(a.AirlineCode, 0);
            }

            foreach (var t in person.Trips)
            {
                foreach (var p in t.PlanItems)
                {
                    Flight f = p as Flight;
                    if (f != null)
                    {
                        countDict[f.Airline.AirlineCode]++;
                    }
                }
            }

            int max = -1;
            string favoriteAirlineCode = null;
            foreach (var record in countDict)
            {
                if (max < record.Value)
                {
                    favoriteAirlineCode = record.Key;
                    max = record.Value;
                }
            }
            return airlines.Single(a => a.AirlineCode.Equals(favoriteAirlineCode));
        }

        public Collection<Person> GetInvolvedPeople(Trip trip)
        {
            var shareID = trip.ShareId;

            IEnumerable<Person> persons = GetRootQuery("People") as IEnumerable<Person>;

            Collection<Person> sharingPersons = new Collection<Person>();

            foreach (var person in persons)
            {
                foreach (var t in person.Trips)
                {
                    if (shareID.Equals(t.ShareId))
                    {
                        sharingPersons.Add(person);
                        break;
                    }
                }
            }

            var dataSource = DataSourceManager.GetCurrentDataSource<TripPinServiceDataSource>();
            EdmEntityContainer defaultContainer = (dataSource.Model.SchemaElements.Single(e => e.Name.Equals("DefaultContainer")) as EdmEntityContainer);
            QueryContext.OperationResultSource = defaultContainer.Elements.Single(e => e.Name.Equals("People")) as EdmEntitySet;

            return sharingPersons;
        }

        public Collection<Trip> GetFriendsTrips(Person person, string userName)
        {
            var friends = person.Friends.Where(p => p.UserName.Equals(userName)).ToArray();
            string parentPath = "People('" + userName + "')";
            string relativePath = "Trips";
            QueryContext.CanonicalUri = new Uri(ServiceConstants.ServiceBaseUri, parentPath + "/" + relativePath);
            if (friends.Count() == 0)
            {
                //todo: in this case it should throw a 404 not found error.
                return new Collection<Trip>();
            }
            else
            {
                return friends[0].Trips;
            }
        }

        public Airport GetNearestAirport(double lat, double lon)
        {
            GeographyPoint startPoint = GeographyPoint.Create(lat, lon);

            var airports = GetRootQuery("Airports") as IEnumerable<Airport>;

            double minDistance = 2;
            Airport nearestAirport = null;

            foreach (Airport airport in airports)
            {
                double distance = CalculateDistance(startPoint, airport.Location.Loc);
                if (distance < minDistance)
                {
                    nearestAirport = airport;
                    minDistance = distance;
                }
            }
            return nearestAirport;
        }

        public void ShareTrip(Person personInstance, string userName, int tripId)
        {
            if (personInstance == null)
            {
                throw new ArgumentNullException("personInstance");
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if (tripId < 0)
            {
                throw new ArgumentNullException("tripId");
            }

            var tripInstance = personInstance.Trips.FirstOrDefault(item => item.TripId == tripId);

            if (tripInstance == null)
            {
                throw new Exception(string.Format("Can't get trip with ID '{0}' in person '{1}'", tripId, personInstance.UserName));
            }

            var friendInstance = personInstance.Friends.FirstOrDefault(item => item.UserName == userName);

            if (friendInstance == null)
            {
                throw new Exception(string.Format("Can't get friend with userName '{0}' in person '{1}'", userName, personInstance.UserName));
            }

            if (friendInstance.Trips != null && friendInstance.Trips.All(item => item.TripId != tripId))
            {
                //TODO, should return 201 if we add new entity, those behavior should be update in handler.
                var newTrip = tripInstance.Clone() as Trip;
                var maxTripId = friendInstance.Trips.Select(item => item.TripId).Max();
                newTrip.TripId = maxTripId + 1;
                friendInstance.Trips.Add(newTrip);
            }
        }

        private static double CalculateDistance(GeographyPoint p1, GeographyPoint p2)
        {
            // using Haversine formula
            // refer to http://en.wikipedia.org/wiki/Haversine_formula
            double lat1 = Math.PI * p1.Latitude / 180;
            double lat2 = Math.PI * p2.Latitude / 180;
            double lon1 = Math.PI * p1.Longitude / 180;
            double lon2 = Math.PI * p2.Longitude / 180;
            double item1 = (Math.Sin((lat1 - lat2) / 2)) * (Math.Sin((lat1 - lat2) / 2));
            double item2 = Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin((lon1 - lon2) / 2) * Math.Sin((lon1 - lon2) / 2);
            return Math.Asin(Math.Sqrt(item1 + item2));
        }

        private static object GetRootQuery(string propertyName)
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<TripPinServiceDataSource>();
            return dataSource.GetType().GetProperty(propertyName).GetValue(dataSource, null);
        }
    }
}
