//-----------------------------------------------------------------------------
// <copyright file="BroadCoverageTestsDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.OData.E2E.TestCommon.Common.Attributes;
using Microsoft.Spatial;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests;

public enum PersonGender
{
    Male = 0,
    Female = 1,
    Unknown = 2,
}

public class Person
{
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

    public Collection<Trip> Trips { get; set; } = new Collection<Trip>();

    public Collection<Person> Friends { get; set; } = new Collection<Person>();

    // the field is used by ETag
    [ETagField]
    public long Concurrency { get; set; } = DateTime.UtcNow.Ticks;

    //Open properties
    public Dictionary<string, object> OpenProperties { get; set; } = new Dictionary<string, object>();
}

public class  SpecialPerson : Person
{
    public string SpecialID { get; set; }
}

public class Trip
{
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

    public Collection<PlanItem> PlanItems { get; set; }

    public Collection<Photo> Photos { get; set; }

    public DateTime UpdatedTime { get; set; }

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
                newTrip.Tags ??= new Collection<string>();
                newTrip.Tags.Add(tag);
            }
        }

        if (this.Photos != null)
        {
            foreach (var photo in this.Photos)
            {
                newTrip.Photos ??= new Collection<Photo>();
                newTrip.Photos.Add(photo);
            }
        }

        foreach (var planItem in this.PlanItems)
        {
            newTrip.PlanItems ??= new Collection<PlanItem>();
            newTrip.PlanItems.Add(planItem.Clone() as PlanItem);
        }
        return newTrip;
    }
}

public class PlanItem
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

    public DateTime UpdatedTime { get; set; }

    //Open properties
    public Dictionary<string, object> OpenProperties { get; set; } = new Dictionary<string, object>();

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

public class PublicTransportation : PlanItem
{
    [SearchField]
    public string SeatNumber { get; set; }
}

public class Event : PlanItem
{
    [SearchField]
    public EventLocation OccursAt { get; set; }

    [SearchField]
    public string Description { get; set; }
}

public class Flight : PublicTransportation
{
    [SearchField]
    public string FlightNumber { get; set; }

    public Airline Airline { get; set; }

    public Airport From { get; set; }

    public Airport To { get; set; }
}

public class Airport
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

    public DateTime UpdatedTime { get; set; }
}

public class City
{
    [SearchField]
    public string Name { get; set; }

    [SearchField]
    public string CountryRegion { get; set; }

    [SearchField]
    public string Region { get; set; }

    public DateTime UpdatedTime { get; set; }
}

public class Airline
{
    [SearchField]
    [ReadOnlyField]
    public string AirlineCode { get; set; }

    [SearchField]
    public string Name { get; set; }

    public DateTime UpdatedTime { get; set; }
}

public class Location
{
    [SearchField]
    public string Address { get; set; }

    [SearchField]
    public City City { get; set; }

    //Open properties
    public Dictionary<string, object> OpenProperties { get; set; } = new Dictionary<string, object>();
}

public class AirportLocation : Location
{
    public GeographyPoint Loc { get; set; }
}

public class EventLocation : Location
{
    [SearchField]
    public string BuildingInfo { get; set; }
}


public class Photo : MediaEntity
{
    public string Name { get; set; }
}

public abstract class MediaEntity
{
    public virtual long Id { get; set; }

    public virtual string ContentType { get; set; }

    public virtual Stream Stream { get; set; }

    public virtual long ETagValue { get; set; } = DateTime.UtcNow.Ticks;

    public virtual string ETag
    {
        get { return string.Format("W/\"{0}\"", this.ETagValue.ToString("X16")); }
    }

    public DateTime UpdatedTime { get; set; }
}
