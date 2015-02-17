//---------------------------------------------------------------------
// <copyright file="TripPinModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.TripPin
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    [Serializable]
    public enum PersonGender
    {
        Male = 0,
        Female = 1,
        Unknown = 2,
    }

    [Serializable]
    public class Person : OpenClrObject
    {
        private readonly EntityCollection<Person> friends;

        public Person()
        {
            Trips = new EntityCollection<Trip>();
            friends = new EntityCollection<Person>((DataSourceManager.GetCurrentDataSource<TripPinServiceDataSource>()).People);
            Emails = new Collection<string>();
            AddressInfo = new Collection<Location>();
            this.Concurrency = DateTime.UtcNow.Ticks;
        }

        [SearchField]
        [ReadOnlyField]
        public string UserName { get; set; }

        [SearchField]
        public string FirstName { get; set; }

        [SearchField]
        public string LastName { get; set; }

        [SearchField]
        public PersonGender Gender { get; set; }

        public Photo Photo { get; set; }

        [SearchField]
        public Collection<string> Emails { get; set; }

        [SearchField]
        public Collection<Location> AddressInfo { get; set; }

        public EntityCollection<Trip> Trips { get; private set; }

        public EntityCollection<Person> Friends
        {
            get
            {
                return this.friends.Cleanup();
            }
        }

        // the field is used by ETag
        [ETagField]
        public long Concurrency { get; set; }
    }

    [Serializable]
    public class Trip : ClrObject, ICloneable
    {
        public Trip()
        {
            PlanItems = new EntityCollection<PlanItem>();
            Photos = new EntityCollection<Photo>();
        }

        [SearchField]
        [ReadOnlyField]
        public int TripId { get; set; }

        [SearchField]
        public Guid ShareId { get; set; }

        [SearchField]
        public string Name { get; set; }

        [SearchField]
        public float Budget { get; set; }

        [SearchField]
        public string Description { get; set; }

        [SearchField]
        public Collection<string> Tags { get; set; }

        [SearchField]
        public DateTimeOffset StartsAt { get; set; }

        [SearchField]
        public DateTimeOffset EndsAt { get; set; }

        public EntityCollection<PlanItem> PlanItems { get; set; }

        public EntityCollection<Photo> Photos { get; set; }

        public object Clone()
        {
            Trip newTrip = new Trip()
            {
                TripId = 0,//Should reset the trip id value.
                Description = this.Description,
                EndsAt = this.EndsAt,
                Name = this.Name,
                Budget = this.Budget,
                ShareId = this.ShareId,
                StartsAt = this.StartsAt,
                Tags = null,
            };
            if (this.Tags != null)
            {
                newTrip.Tags = new Collection<string>();
                foreach (var tag in this.Tags)
                {
                    newTrip.Tags.Add(tag);
                }
            }
            if (this.Photos != null)
            {
                foreach (var photo in this.Photos)
                {
                    newTrip.Photos.Add(photo);
                }
            }
            foreach (var planItem in this.PlanItems)
            {
                newTrip.PlanItems.Add(planItem.Clone() as PlanItem);
            }
            return newTrip;
        }
    }

    [Serializable]
    public class PlanItem : OpenClrObject, ICloneable
    {
        [SearchField]
        [ReadOnlyField]
        public int PlanItemId { get; set; }

        [SearchField]
        public string ConfirmationCode { get; set; }

        [SearchField]
        public DateTimeOffset StartsAt { get; set; }

        [SearchField]
        public DateTimeOffset EndsAt { get; set; }

        [SearchField]
        public TimeSpan Duration { get; set; }

        public virtual object Clone()
        {
            var newPlan = new PlanItem()
            {
                ConfirmationCode = this.ConfirmationCode,
                Duration = this.Duration,
                EndsAt = this.EndsAt,
                PlanItemId = this.PlanItemId,
                StartsAt = this.StartsAt
            };
            return newPlan;
        }
    }

    [Serializable]
    public class PublicTransportation : PlanItem
    {
        [SearchField]
        public string SeatNumber { get; set; }

        public override object Clone()
        {
            var newPlan = new PublicTransportation()
            {
                ConfirmationCode = this.ConfirmationCode,
                Duration = this.Duration,
                EndsAt = this.EndsAt,
                PlanItemId = this.PlanItemId,
                StartsAt = this.StartsAt,
                SeatNumber = this.SeatNumber,
            };
            return newPlan;
        }
    }

    [Serializable]
    public class Event : PlanItem
    {
        [SearchField]
        public EventLocation OccursAt { get; set; }

        [SearchField]
        public string Description { get; set; }

        public override object Clone()
        {
            var newPlan = new Event()
            {
                ConfirmationCode = this.ConfirmationCode,
                Duration = this.Duration,
                EndsAt = this.EndsAt,
                PlanItemId = this.PlanItemId,
                StartsAt = this.StartsAt,
                Description = this.Description,
                OccursAt = new EventLocation()
                {
                    Address = this.OccursAt.Address,
                    BuildingInfo = this.OccursAt.BuildingInfo,
                    City = new City() {
                        CountryRegion = this.OccursAt.City.CountryRegion,
                        Name = this.OccursAt.City.Name,
                        Region = this.OccursAt.City.Region,
                    }
                },
            };
            return newPlan;
        }
    }

    [Serializable]
    public class Flight : PublicTransportation
    {
        [SearchField]
        public string FlightNumber { get; set; }

        public Airline Airline { get; set; }

        public Airport From { get; set; }

        public Airport To { get; set; }

        public override object Clone()
        {
            var newPlan = new Flight()
            {
                ConfirmationCode = this.ConfirmationCode,
                Duration = this.Duration,
                EndsAt = this.EndsAt,
                PlanItemId = this.PlanItemId,
                StartsAt = this.StartsAt,
                FlightNumber = this.FlightNumber,
                Airline = this.Airline,
                From = this.From,
                SeatNumber = this.SeatNumber,
                To = this.To,
            };
            return newPlan;
        }
    }

    [Serializable]
    public class Airport : ClrObject
    {
        [SearchField]
        public string Name { get; set; }

        [SearchField]
        public AirportLocation Location { get; set; }

        [SearchField]
        [ReadOnlyField]
        public string IcaoCode { get; set; }

        [SearchField]
        [ReadOnlyField]
        public string IataCode { get; set; }
    }

    [Serializable]
    public class City : ClrObject
    {
        [SearchField]
        public string Name { get; set; }

        [SearchField]
        public string CountryRegion { get; set; }

        [SearchField]
        public string Region { get; set; }
    }

    [Serializable]
    public class Airline : ClrObject
    {
        [SearchField]
        [ReadOnlyField]
        public string AirlineCode { get; set; }

        [SearchField]
        public string Name { get; set; }
    }

    [Serializable]
    public class Location : OpenClrObject
    {
        [SearchField]
        public string Address { get; set; }

        [SearchField]
        public City City { get; set; }
    }

    [Serializable]
    public class AirportLocation : Location
    {
        // TODO: the type of field does not support serialization
        public GeographyPoint Loc { get; set; }
    }

    [Serializable]
    public class EventLocation : Location
    {
        [SearchField]
        public string BuildingInfo { get; set; }
    }

    [Serializable]
    public class Photo : MediaEntity
    {
        public string Name { get; set; }
    }
}