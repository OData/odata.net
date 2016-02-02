//---------------------------------------------------------------------
// <copyright file="ClientAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientAnnotationTests
    {
        DefaultContainerPlus dsc = new DefaultContainerPlus(new Uri("http://odataService/"));
        IEnumerable<MergeOption> mergeOptions = Enum.GetValues(typeof(MergeOption)).Cast<MergeOption>();

        const string FeedPayloadWithInstanceAnnotationOnFeedandEntry = @"{
    ""@odata.context"":""http://odataService/$metadata#People"",
    ""@Microsoft.OData.SampleService.Models.TripPin.Gender"":""Male"",
    ""value"": [
        {
            ""@odata.type"": ""#Microsoft.OData.SampleService.Models.TripPin.Person"",
            ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
            [
                ""Bob1"",
                ""Cat1""
            ],
            ""UserName"":""BobCat"",
            ""FirstName"": ""Bob"",
            ""LastName"": ""Cat"",
            ""Location"": null,
            ""CompanyLocation"": null,
            ""Gender"":""Male"",
            ""Age"":""22"",
            ""Concurrency"":635548229917715070
        },
        {
            ""@odata.type"": ""#Microsoft.OData.SampleService.Models.TripPin.VipCustomer"",
            ""@Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions"":
            {
                ""Searchable"":false,
                ""UnsupportedExpressions"":""NOT,OR""
            },       
            ""UserName"": ""JillJones"",
            ""FirstName"": ""Jill"",
            ""LastName"": ""Jones"",
            ""Location"": null,
            ""CompanyLocation"": null,
            ""Gender"":""Male"",
            ""Age"":""22"",
            ""Concurrency"":635548229917715070
        }
    ]
}";

        const string EntryPayloadWithInstanceAnnotationOnEntry = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions"":
    {
        ""Searchable"":false,
        ""UnsupportedExpressions"":""NOT,OR""
    },
    ""UserName"":""aprilcline"",
    ""FirstName"":""April"",
    ""LastName"":""Cline"",
    ""Emails@Org.OData.Core.V1.Computed"":false,
    ""Emails@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
    [
        ""test""
    ],
    ""Emails"":[""Russell@abc.com""],
    ""Location"":
    {
        ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
        ""Address"":""1 Microsoft Way"",
        ""City"":
        {
            ""Name"":""Beijing"",
            ""CountryRegion"":""China""
        }
     },
    ""Gender"":""Female"",
    ""Concurrency@Org.OData.Core.V1.Computed"":""false"",
    ""Concurrency"":635549965782056080,
    ""Assets@Org.OData.Core.V1.Description"":""All assets"",
    ""Assets"":
    [
        {
            ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
            [
                ""PC1""
            ],
            ""Name"":""PC1"",
            ""Kind"":
            [
                ""PersonalAsset"",
                ""NormalAsset""
            ]
        },
        {
            ""@Org.OData.Core.V1.Description"": null,
            ""Name"":""PC2"",
            ""Kind@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
            [
                ""PersonalAsset""
            ],
            ""Kind"":
            [
                ""PersonalAsset"",
                ""NormalAsset""
            ]
        }
    ]
}";

        [TestMethod]
        public void TestGetAnnotationOnFeed()
        {
            foreach (var mergeOption in mergeOptions)
            {
                string response = FeedPayloadWithInstanceAnnotationOnFeedandEntry;
                TestAnnotation(response, () =>
                {
                    dsc.MergeOption = mergeOption;
                    var peopleQueryResponse1 = dsc.PeoplePlus.Execute();
                    var people1 = peopleQueryResponse1.ToList();

                    // Get Enum instance annotation on feed
                    PersonGenderPlus annotation;
                    bool result = dsc.TryGetAnnotation<PersonGenderPlus>(peopleQueryResponse1, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                    Assert.IsTrue(result);
                    Assert.AreEqual(PersonGenderPlus.MalePlus, annotation); ;

                    // Get collection instance annotation on one of the entities in the returned collection
                    ICollection<string> annotation2 = null;
                    result = dsc.TryGetAnnotation<ICollection<string>>(people1[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation2);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation2);
                    Assert.AreEqual("Bob1", annotation2.First());

                    // Set the response to a new value
                    response = response.Replace(
                        @"""@Microsoft.OData.SampleService.Models.TripPin.Gender"":""Male""",
                        @"""@Microsoft.OData.SampleService.Models.TripPin.Gender"":""Female""")
                        .Replace(@"""Bob1""", @"""Bob2""");
                    SetResponse(response);

                    // Query the entityset again
                    var peopleQueryResponse2 = dsc.PeoplePlus.Execute();
                    var people2 = peopleQueryResponse2.ToList();

                    // Verify the instance annotation for this feed has been changed
                    result = dsc.TryGetAnnotation<PersonGenderPlus>(peopleQueryResponse2, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                    Assert.IsTrue(result);
                    Assert.AreEqual(PersonGenderPlus.FemalePlus, annotation);

                    // Verify the instance annotation that is being tracked according to the merge option.
                    result = dsc.TryGetAnnotation<ICollection<string>>(people1[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation2);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation2);
                    if (mergeOption == MergeOption.OverwriteChanges || mergeOption == MergeOption.PreserveChanges)
                    {
                        Assert.AreEqual("Bob2", annotation2.First());
                    }
                    else
                    {
                        Assert.AreEqual("Bob1", annotation2.First());
                    }

                    result = dsc.TryGetAnnotation<ICollection<string>>(people2[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation2);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation2);
                    if (mergeOption == MergeOption.AppendOnly)
                    {
                        Assert.AreEqual("Bob1", annotation2.First());
                    }
                    else
                    {
                        Assert.AreEqual("Bob2", annotation2.First());
                    }                    
                });
            }
        }

        [TestMethod]
        public void TestGetAnnotationOnFeedWithFilter()
        {
            TestAnnotation(FeedPayloadWithInstanceAnnotationOnFeedandEntry, () =>
            {
                var peopleQueryResponse = ((DataServiceQuery<PersonPlus>)dsc.PeoplePlus.Where(p => p.FirstNamePlus == "Bob")).Execute();
                var people = peopleQueryResponse.ToList();

                ICollection<string> annotation2 = null;
                bool result = dsc.TryGetAnnotation<ICollection<string>>(people[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation2);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation2);
                Assert.AreEqual("Bob1", annotation2.First());

                // Verify the instance annotation for the colletion of entity
                PersonGenderPlus annotation;
                result = dsc.TryGetAnnotation<PersonGenderPlus>(peopleQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.MalePlus, annotation);
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnFeedWithProjection()
        {
            var response = @"{
    ""@odata.context"":""http://odataService/$metadata#People(UserName,FirstName,Emails,Location,Concurrency)"",
    ""@Microsoft.OData.SampleService.Models.TripPin.Gender"":""FemalePlus"",
    ""value"": [
        {
            ""@Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions"":
            {
                ""Searchable"":false,
                ""UnsupportedExpressions"":""NOT,OR""
            },
            ""UserName"":""BobCat"",
            ""FirstName"": ""Bob"",
            ""Emails@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
            [
                ""test""
            ],
            ""Emails"":[""Russell@abc.com""],
            ""Location"":
            {
                ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
                ""Address"":""1 Microsoft Way"",
                ""City"":
                {
                    ""Name"":""Beijing"",
                    ""CountryRegion"":""China""
                }
             },
            ""Concurrency@Org.OData.Core.V1.Computed"":""false"",
            ""Concurrency"":635548229917715070
        },
        {
            ""@Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions"":
            {
                ""Searchable"":false,
                ""UnsupportedExpressions"":""NOT,OR""
            },
            ""UserName"": ""JillJones"",
            ""FirstName"": ""Jill"",
            ""Emails"":[""Russell@abc.com""],
            ""Location"":
            {
                ""Address"":""1 Microsoft Way"",
                ""City"":
                {
                    ""Name"":""Beijing"",
                    ""CountryRegion"":""China""
                }
             },
            ""Concurrency"":635548229917715070
        }
    ]
}";
            TestAnnotation(response, () =>
            {
                var peopleQueryResponse = ((DataServiceQuery<PersonPlus>)dsc.PeoplePlus
                    .Select(p =>
                        new PersonPlus
                        {
                            UserNamePlus = p.UserNamePlus,
                            FirstNamePlus = p.FirstNamePlus,
                            ConcurrencyPlus = p.ConcurrencyPlus,
                            EmailsPlus = p.EmailsPlus,
                            LocationPlus = p.LocationPlus,
                        }))
                    .Execute();
                var people = peopleQueryResponse.ToList();

                PersonGenderPlus annotation;

                // Verify the instance annotation for the collection of entity
                bool result = dsc.TryGetAnnotation(peopleQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.FemalePlus, annotation);

                // Verify the instance annotation for one of the entities in the returned collection.
                SearchRestrictionsPlus searchAnnotation;
                result = dsc.TryGetAnnotation((object)people[0], "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out searchAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, searchAnnotation.UnsupportedExpressionsPlus);

                // Verify the instance annotation for the property in one entity
                ICollection<string> seoAnnotation = null;
                result = dsc.TryGetAnnotation<Func<ICollection<string>>, ICollection<string>>(() => people[0].EmailsPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out seoAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual("test", seoAnnotation.First());

                string cityNameAnnotation = null;
                result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => people.First().LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityNameAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", cityNameAnnotation);

                bool computedAnnotation;
                result = dsc.TryGetAnnotation<Func<long>, bool>(() => people.First().ConcurrencyPlus, "Org.OData.Core.V1.Computed", out computedAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual(false, computedAnnotation);
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnNavigationPropertyWithProjection()
        {
            var response = @"{
    ""@odata.context"":""http://odataService/$metadata#People(UserName,FirstName,Emails,Location,Concurrency,Partner)"",
    ""@Microsoft.OData.SampleService.Models.TripPin.Gender"":""FemalePlus"",
    ""value"": [
        {
            ""UserName"":""BobCat"",
            ""FirstName"": ""Bob"",
            ""Emails"":[""Russell@abc.com""],
            ""Location"":
            {
                ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
                ""Address"":""1 Microsoft Way"",
                ""City"":
                {
                    ""Name"":""Beijing"",
                    ""CountryRegion"":""China""
                }
             },
            ""Concurrency@Org.OData.Core.V1.Computed"":""false"",
            ""Concurrency"":635548229917715070,
            ""Partner"":
            {
                ""UserName"":""Marchcline"",
                ""FirstName"":""March"",
                ""LastName"":""Cline"",
                ""Emails"":[],
                ""Location"":null,
                ""Gender"":""Female"",
                ""Concurrency"":635549965782056081
            }
        },
        {
            ""UserName"": ""JillJones"",
            ""FirstName"": ""Jill"",
            ""Emails"":[""Russell@abc.com""],
            ""Location"":
            {
                ""Address"":""1 Microsoft Way"",
                ""City"":
                {
                    ""Name"":""Beijing"",
                    ""CountryRegion"":""China""
                }
             },
            ""Concurrency"":635548229917715070,
            ""Partner"":
            {
                ""@Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions"":
                {
                    ""Searchable"":false,
                    ""UnsupportedExpressions"":""NOT,OR""
                },
                ""UserName"":""Marchcline"",
                ""FirstName"":""March"",
                ""LastName"":""Cline"",
                ""Emails@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
                [
                    ""test""
                ],
                ""Emails"":[""Russell@abc.com""],
                ""Location"":
                {
                    ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
                    ""Address"":""1 Microsoft Way"",
                    ""City"":
                    {
                        ""Name"":""Beijing"",
                        ""CountryRegion"":""China""
                    }
                 },
                ""Gender"":""Female"",
                ""Concurrency@Org.OData.Core.V1.Computed"":""false"",
                ""Concurrency"":635548229917715070
            }
        }
    ]
}";
            TestAnnotation(response, () =>
            {
                var peopleQueryResponse = ((DataServiceQuery<PersonPlus>)dsc.PeoplePlus
                    .Select(p =>
                        new PersonPlus
                        {
                            UserNamePlus = p.UserNamePlus,
                            FirstNamePlus = p.FirstNamePlus,
                            ConcurrencyPlus = p.ConcurrencyPlus,
                            EmailsPlus = p.EmailsPlus,
                            LocationPlus = p.LocationPlus,
                            PartnerPlus = p.PartnerPlus
                        }))
                    .Execute();
                var people = peopleQueryResponse.ToList();

                PersonGenderPlus annotation;

                // Verify the instance annotation for the collection of entity
                bool result = dsc.TryGetAnnotation(peopleQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.FemalePlus, annotation);

                // Verify the instance annotation for the entity in the returned collection.
                SearchRestrictionsPlus searchAnnotation;
                result = dsc.TryGetAnnotation<Func<PersonPlus>, SearchRestrictionsPlus>(() => people[1].PartnerPlus, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out searchAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, searchAnnotation.UnsupportedExpressionsPlus);

                // Verify the instance annotation for the property in one entity
                ICollection<string> seoAnnotation = null;
                result = dsc.TryGetAnnotation<Func<ICollection<string>>, ICollection<string>>(() => people[1].PartnerPlus.EmailsPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out seoAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual("test", seoAnnotation.First());

                string cityNameAnnotation = null;
                result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => people[1].PartnerPlus.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityNameAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", cityNameAnnotation);

                bool computedAnnotation;
                result = dsc.TryGetAnnotation<Func<long>, bool>(() => people[1].PartnerPlus.ConcurrencyPlus, "Org.OData.Core.V1.Computed", out computedAnnotation);
                Assert.IsTrue(result);
                Assert.AreEqual(false, computedAnnotation);
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnSingleton()
        {
            foreach (var mergeOption in mergeOptions)
            {
                string response = EntryPayloadWithInstanceAnnotationOnEntry;
                TestAnnotation(response, () =>
                {
                    dsc.MergeOption = mergeOption;
                    var me = dsc.MePlus.GetValue();

                    SearchRestrictionsPlus annotation = null;

                    // Get complex type instance annotation on entity returned by querying Singleton
                    bool result = dsc.TryGetAnnotation(me, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                    Assert.IsTrue(result);
                    Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);

                    // Set the response to a new value
                    response = response.Replace(
            @"""Searchable"":false,
        ""UnsupportedExpressions"":""NOT,OR""",
            @"""Searchable"":true,
        ""UnsupportedExpressions"":""OR""");
                    SetResponse(response);

                    // Query again
                    var me2 = dsc.MePlus.GetValue();

                    // Get complex type instance annotation on entity according to the merge option
                    annotation = null;
                    result = dsc.TryGetAnnotation(me, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                    Assert.IsTrue(result);
                    if (mergeOption == MergeOption.OverwriteChanges || mergeOption == MergeOption.PreserveChanges)
                    {
                        Assert.AreEqual(SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
                    }
                    else
                    {
                        Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
                    }

                    // Get the new annotation on this entity
                    result = dsc.TryGetAnnotation(me2, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                    Assert.IsTrue(result);
                    if (mergeOption == MergeOption.AppendOnly)
                    {
                        Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
                    }
                    else
                    {
                        Assert.AreEqual(SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
                    }
                });
            }
        }

        [TestMethod]
        public void TestGetCollectionInstanceAnnotationOnODataEntryWhenPayloadisODataEntryButNotSingleton()
        {
            foreach (var mergeOption in mergeOptions)
            {
                string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
    [
        ""Russell1"",
        ""Whyte1""
    ],
    ""UserName"":""russellwhyte"",
    ""FirstName"":""Russell"",
    ""LastName"":""Whyte"",
    ""Emails"":[""Russell@example.com"",""Russell@contoso.com""],
    ""Location"":
    {
        ""Address"":""1 Microsoft Way"",
        ""City"":
        {
            ""Name"":""Nanjing"",
            ""CountryRegion"":""China""
        }
     },
     ""Gender"":""Male"",
     ""Age"":""22"",
     ""Concurrency"":635548229917715070
 }";
                TestAnnotation(response, () =>
                {
                    dsc.MergeOption = mergeOption;
                    var personQuery = dsc.PeoplePlus.ByKey("russellwhyte");
                    var person = personQuery.GetValue();
                    ICollection<string> annotation = null;

                    // Get instance annotation on a single entity
                    bool result = dsc.TryGetAnnotation<ICollection<string>>(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation);
                    Assert.AreEqual("Russell1", annotation.ElementAt(0));

                    response = response.Replace("Russell1", "Russell2");
                    SetResponse(response);

                    var personQuery2 = dsc.PeoplePlus.ByKey("russellwhyte");
                    var person2 = personQuery2.GetValue();

                    // Get instance annotation on a single entity, should be changed according to the merge options
                    result = dsc.TryGetAnnotation<ICollection<string>>(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation);
                    if (mergeOption == MergeOption.OverwriteChanges || mergeOption == MergeOption.PreserveChanges)
                    {
                        Assert.AreEqual("Russell2", annotation.ElementAt(0));
                    }
                    else
                    {
                        Assert.AreEqual("Russell1", annotation.ElementAt(0));
                    }

                    result = dsc.TryGetAnnotation<ICollection<string>>(person2, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation);
                    if (mergeOption == MergeOption.AppendOnly)
                    {
                        Assert.AreEqual("Russell1", annotation.ElementAt(0));
                    }
                    else
                    {
                        Assert.AreEqual("Russell2", annotation.ElementAt(0));
                    }

                    response = response.Replace("Russell2", "Russell3");
                    SetResponse(response);

                    var personQuery3 = dsc.PeoplePlus.ByKey("russellwhyte").Select(p => new { UN = p.UserNamePlus, FN = p.FirstNamePlus, CU = p.ConcurrencyPlus });
                    var person3 = personQuery3.GetValue();

                    // Get instance annotation on a single entity, it is not effected by the entity stored on client side.
                    result = dsc.TryGetAnnotation<ICollection<string>>(person2, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                    Assert.IsTrue(result);
                    Assert.IsNotNull(annotation);
                    if (mergeOption == MergeOption.AppendOnly)
                    {
                        Assert.AreEqual("Russell1", annotation.ElementAt(0));
                    }
                    else
                    {
                        Assert.AreEqual("Russell2", annotation.ElementAt(0));
                    }

                    // Get instance annotation on the projected single entity.
                    result = dsc.TryGetAnnotation<ICollection<string>>(person3, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                    Assert.IsNotNull(annotation);
                    Assert.AreEqual("Russell3", annotation.ElementAt(0));
                });
            }
        }

        [TestMethod]
        public void TestGetAnnotationOnDerivedEntity()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.VipCustomer"",
    ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
    [
        ""Russell1"",
        ""Whyte1""
    ],
    ""UserName"":""russellwhyte"",
    ""FirstName"":""Russell"",
    ""LastName"":""Whyte"",
    ""Emails"":[""Russell@example.com"",""Russell@contoso.com""],
    ""Location"":
    {
        ""Address"":""1 Microsoft Way"",
        ""City"":
        {
            ""Name"":""Nanjing"",
            ""CountryRegion"":""China""
        }
     },
     ""Gender"":""Male"",
     ""Age"":""22"",
     ""Concurrency"":635548229917715070,
     ""VipNumber"":1
 }";
            TestAnnotation(response, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();
                ICollection<string> annotation = null;

                // Get instance annotation on a single entity
                bool result = dsc.TryGetAnnotation<ICollection<string>>(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation);
                Assert.AreEqual("Russell1", annotation.ElementAt(0));
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnEntityWhichIsReturnedByFunction()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var PartnerQuery = dsc.PeoplePlus.ByKey("russellwhyte").GetPartnerPlus();
                var Partner = PartnerQuery.GetValue();

                // Get complex type instance annotation on returned entity
                SearchRestrictionsPlus annotation;
                bool result = dsc.TryGetAnnotation(Partner, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnCollectionOfEntityWhichIsReturnedByFunction()
        {
            TestAnnotation(FeedPayloadWithInstanceAnnotationOnFeedandEntry, () =>
            {
                var friendsQueryResponse = dsc.PeoplePlus.ByKey("russellwhyte").GetFriendsPlus().Execute();
                var friends = friendsQueryResponse.ToList();

                // Get complex type instance annotation on returned feed
                PersonGenderPlus annotation;
                bool result = dsc.TryGetAnnotation(friendsQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.MalePlus, annotation);

                // Get collection type instance annotation on one of the returned items
                ICollection<string> annotation1 = null;
                result = dsc.TryGetAnnotation(friends[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation1);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation1);
                Assert.AreEqual("Bob1", annotation1.ElementAt(0));

                // Get collection type instance annotation on one of the returned derived items
                SearchRestrictionsPlus annotation2 = null;
                result = dsc.TryGetAnnotation(friends[1], "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation2);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation2);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation2.UnsupportedExpressionsPlus);
            });
        }

        const string ComplexValuePayloadWithInstanceAnnotationOnComplexValue = @"{
    ""@odata.context"":""http://odataService/$metadata#Microsoft.OData.SampleService.Models.TripPin.Location"",
    ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
    ""Address"":""1 Microsoft Way"",
    ""City"":
    {
        ""Name"":""Nanjing"",
        ""CountryRegion"":""China""
    }
}";
        [TestMethod]
        public void TestGetInstanceAnnotationOnODataComplexValueWhichIsReturnedByFunction()
        {
            TestAnnotation(ComplexValuePayloadWithInstanceAnnotationOnComplexValue, () =>
            {
                var getLocationQuery = dsc.PeoplePlus.ByKey("Jason").GetLocationPlus();
                var location = getLocationQuery.GetValue();

                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(location, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", annotation);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnEntityWhichIsReturnedByAction()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var PartnerAction = dsc.PeoplePlus.ByKey("russellwhyte").SetPartnerPlus();
                var Partner = PartnerAction.GetValue();

                // Get complex type instance annotation on returned entity
                SearchRestrictionsTypePlus annotation = null;
                bool result = dsc.TryGetAnnotation(Partner, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(false, annotation.SearchablePlus);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnCollectionOfEntityWhichIsReturnedByAction()
        {
            TestAnnotation(FeedPayloadWithInstanceAnnotationOnFeedandEntry, () =>
            {
                var friendsQuery = dsc.PeoplePlus.ByKey("russellwhyte").SetFriendsPlus();
                var friendsResponse = friendsQuery.Execute();
                var friends = friendsResponse.ToList();

                // Get complex type instance annotation on returned feed
                PersonGenderPlus annotation;
                bool result = dsc.TryGetAnnotation(friendsResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.MalePlus, annotation);

                // Get collection type instance annotation on one of the returned items
                ICollection<string> annotation1 = null;
                result = dsc.TryGetAnnotation(friends[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation1);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation1);
                Assert.AreEqual("Bob1", annotation1.ElementAt(0));

                // Get collection type instance annotation on one of the returned derived items
                SearchRestrictionsPlus annotation2 = null;
                result = dsc.TryGetAnnotation(friends[1], "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation2);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation2);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation2.UnsupportedExpressionsPlus);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnODataComplexValueWhichIsReturnedByAction()
        {
            TestAnnotation(ComplexValuePayloadWithInstanceAnnotationOnComplexValue, () =>
            {
                var getLocationQuery = dsc.PeoplePlus.ByKey("Jason").SetLocationPlus();
                var location = getLocationQuery.GetValue();

                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(location, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", annotation);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnEntityWhichIsReturnedByFunctionImport()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var getPersonQuery = dsc.GetPersonPlus("Russell");
                var person = getPersonQuery.GetValue();

                // Get complex type instance annotation on returned entity
                SearchRestrictionsTypePlus annotation = null;
                bool result = dsc.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(false, annotation.SearchablePlus);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnCollectionOfEntityWhichIsReturnedByFunctionImport()
        {
            TestAnnotation(FeedPayloadWithInstanceAnnotationOnFeedandEntry, () =>
            {
                var getFriendsQueryResponse = dsc.GetFriendsPlus("Russell").Execute() as QueryOperationResponse<PersonPlus>;
                var friends = getFriendsQueryResponse.ToList();

                // Get complex type instance annotation on returned feed
                PersonGenderPlus annotation;
                bool result = dsc.TryGetAnnotation(getFriendsQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(PersonGenderPlus.MalePlus, annotation);

                // Get collection type instance annotation on one of the returned items
                ICollection<string> annotation1 = null;
                result = dsc.TryGetAnnotation(friends[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation1);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation1);
                Assert.AreEqual("Bob1", annotation1.ElementAt(0));

                // Get collection type instance annotation on one of the returned derived items
                SearchRestrictionsPlus annotation2 = null;
                result = dsc.TryGetAnnotation(friends[1], "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotation2);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation2);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation2.UnsupportedExpressionsPlus);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnComplexTypePropertyInAnEntity()
        {
            foreach (var mergeOption in mergeOptions)
            {
                string response = EntryPayloadWithInstanceAnnotationOnEntry;
                TestAnnotation(response, () =>
                {
                    dsc.MergeOption = mergeOption;
                    var personQuery = dsc.PeoplePlus.ByKey("russellwhyte");
                    var person1 = personQuery.GetValue();

                    // Get instance annotation on ComplexProperty
                    string annotation = null;
                    bool result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => person1.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                    Assert.IsTrue(result);
                    Assert.AreEqual("MyCity", annotation);

                    response = response.Replace("MyCity", "MyCity1");
                    SetResponse(response);

                    var person2 = personQuery.GetValue();

                    string annotationOnComplexInstance = null;
                    // Get instance annotation on ComplexProperty of the old entity by using propertyAccess
                    result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => person1.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                    Assert.IsTrue(result);

                    // Get instance annotation on ComplexProperty of the old entity by using property instance
                    result = dsc.TryGetAnnotation(person1.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotationOnComplexInstance);
                    Assert.IsTrue(result);
                    if (mergeOption == MergeOption.OverwriteChanges || mergeOption == MergeOption.PreserveChanges)
                    {
                        Assert.AreEqual("MyCity1", annotation);
                        Assert.AreEqual("MyCity1", annotationOnComplexInstance);
                    }
                    else
                    {
                        Assert.AreEqual("MyCity", annotation);
                        Assert.AreEqual("MyCity", annotationOnComplexInstance);
                    }

                    // Get instance annotation on ComplexProperty of the new entity by using propertyAccess
                    result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => person2.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                    Assert.IsTrue(result);

                    // Get instance annotation on ComplexProperty of the new entity by using property instance
                    result = dsc.TryGetAnnotation(person2.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotationOnComplexInstance);
                    Assert.IsTrue(result);
                    if (mergeOption == MergeOption.AppendOnly)
                    {
                        Assert.AreEqual("MyCity", annotation);
                        Assert.AreEqual("MyCity", annotationOnComplexInstance);
                    }
                    else
                    {
                        Assert.AreEqual("MyCity1", annotation);
                        Assert.AreEqual("MyCity1", annotationOnComplexInstance);
                    }
                });
            }
        }

        [TestMethod]
        public void TestGetAnnotationOnPrimitivePropertyInAnEntity()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();

                bool annotation;
                // Get instance annotation on primitive property
                bool result = dsc.TryGetAnnotation<Func<long>, bool>(() => person.ConcurrencyPlus, "Org.OData.Core.V1.Computed", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual(false, annotation);
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnCollectionPropertyInAnEntity()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();

                // Get instance annotation on collection property
                ICollection<string> annotation;
                bool result = dsc.TryGetAnnotation<Func<ObservableCollection<string>>, ICollection<string>>(() => person.EmailsPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation);
                Assert.AreEqual("test", annotation.FirstOrDefault());

                // Get instance annotation on the same collection property
                bool annotation1;
                result = dsc.TryGetAnnotation<Func<ObservableCollection<string>>, bool>(() => person.EmailsPlus, "Org.OData.Core.V1.Computed", out annotation1);
                Assert.IsTrue(result);
                Assert.AreEqual(false, annotation1);
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnCollectionOfComplexTypePropertyInAnEntity()
        {

            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();

                // Get instance annotation on collection property
                string annotation;
                bool result = dsc.TryGetAnnotation<Func<ObservableCollection<AssetPlus>>, string>(() => person.AssetsPlus, "Org.OData.Core.V1.Description", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("All assets", annotation);

                ICollection<string> annotation1;
                // Get instance annotation on one item
                result = dsc.TryGetAnnotation(person.AssetsPlus[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation1);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation1);
                Assert.AreEqual("PC1", annotation1.FirstOrDefault());

                // Get null instance annotation on one item
                result = dsc.TryGetAnnotation(person.AssetsPlus[1], "Org.OData.Core.V1.Description", out annotation);
                Assert.IsTrue(result);
                Assert.IsNull(annotation);

                // Get instance annotation on the collection in the complex property
                result = dsc.TryGetAnnotation<Func<ObservableCollection<AssetTypePlus>>, ICollection<string>>(
                    () => person.AssetsPlus[1].KindPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms",
                    out annotation1);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation1);
                Assert.AreEqual("PersonalAsset", annotation1.FirstOrDefault());
            });
        }

        [TestMethod]
        public void TestGetAnnotationOnPropertyInDerivedEntity()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.VipCustomer"",
    ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
    [
        ""Russell1"",
        ""Whyte1""
    ],
    ""UserName"":""russellwhyte"",
    ""FirstName"":""Russell"",
    ""LastName"":""Whyte"",
    ""Emails"":[""Russell@example.com"",""Russell@contoso.com""],
    ""Location"":
    {
        ""@Microsoft.OData.SampleService.Models.TripPin.CityName"" : ""Test1"",
        ""Address"":""1 Microsoft Way"",
        ""City"":
        {
            ""Name"":""Nanjing"",
            ""CountryRegion"":""China""
        }
     },
    ""CompanyLocation"":
    {
        ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.CompanyLocation"",
        ""@Microsoft.OData.SampleService.Models.TripPin.CityName"" : ""Test2"",
        ""Address"":""1 Microsoft Way"",
        ""City"":
        {
            ""Name"":""Nanjing"",
            ""CountryRegion"":""China""
        }
     },
     ""Gender"":""Male"",
     ""Age"":""22"",
     ""Concurrency"":635548229917715070,
     ""VipNumber"":1
 }";
            TestAnnotation(response, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();
                string annotation = null;

                // Get instance annotation on a single entity
                bool result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => person.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("Test1", annotation);

                result = dsc.TryGetAnnotation<Func<LocationPlus>, string>(() => person.CompanyLocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("Test2", annotation);
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnSingleNavigationProperty()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#Me"",
    ""UserName"":""aprilcline"",
    ""FirstName"":""April"",
    ""LastName"":""Cline"",
    ""Emails"":[],
    ""Location"":null,
    ""Gender"":""Male"",
    ""Concurrency"":635549965782056080,
    ""Partner"":
    {
        ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
        [
            ""Bob1"",
            ""Cat1""
        ],
        ""UserName"":""Marchcline"",
        ""FirstName"":""March"",
        ""LastName"":""Cline"",
        ""Emails"":[],
        ""Location"":null,
        ""Gender"":""Female"",
        ""Concurrency"":635549965782056081
    }
}";
            TestAnnotation(response, () =>
            {
                var person = dsc.PeoplePlus.ByKey("russellwhyte").Expand(p => p.PartnerPlus).GetValue();
                ICollection<string> annotation = null;
                bool result = dsc.TryGetAnnotation<Func<PersonPlus>, ICollection<string>>(() => person.PartnerPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation);
                Assert.AreEqual("Bob1", annotation.ElementAt(0));
                Assert.AreEqual("Cat1", annotation.ElementAt(1));

                annotation = null;
                result = dsc.TryGetAnnotation(person.PartnerPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                Assert.IsTrue(result);
                Assert.IsNotNull(annotation);
                Assert.AreEqual("Bob1", annotation.ElementAt(0));
                Assert.AreEqual("Cat1", annotation.ElementAt(1));
            });
        }

        [TestMethod]
        public void TestGetInstanceAnnotationOnODataComplexValueWhenPayloadisODataComplexValue()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People('Jason')/Location"",
    ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.Location"",
    ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
    ""Address"":""1 Microsoft Way"",
    ""City"":
    {
        ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.City"",
        ""Name"":""Nanjing"",
        ""CountryRegion"":""China""
    }
}";
            TestAnnotation(response, () =>
            {
                var location = dsc.PeoplePlus.ByKey("Jason").Select(p => p.LocationPlus).GetValue();
                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(location, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", annotation);
            });
        }

        [TestMethod]
        public void TestGetStringInstanceAnnotationOnODataComplexValueWhenPayloadisODataComplexValueofDerivedType()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People('Jason')/Location"",
    ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.CompanyLocation"",
    ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":""MyCity"",
    ""CompanyName@Org.OData.Core.V1.Description"":""My Company"",
    ""CompanyName"":""MATL"",
    ""Address"":""1 Microsoft Way"",
    ""City"":
    {
        ""Name"":""Nanjing"",
        ""CountryRegion"":""China""
    }
}";
            TestAnnotation(response, () =>
            {
                var location = dsc.PeoplePlus.ByKey("Jason").Select(p => p.LocationPlus).GetValue() as CompanyLocationPlus;
                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(location, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("MyCity", annotation);

                // Verify the instance annotaiton on property in derived type
                result = dsc.TryGetAnnotation<Func<string>, string>(() => location.CompanyNamePlus, "Org.OData.Core.V1.Description", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("My Company", annotation);
            });
        }

        [TestMethod]
        public void TestGetNullableIntegerInstanceAnnotationOnODataEntry()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@Microsoft.OData.SampleService.Models.TripPin.Age"":null,
    ""UserName"":""aprilcline"",
    ""FirstName"":""April"",
    ""LastName"":""Cline"",
    ""Emails"":[""aprilcline@abc.com""],
    ""Location"":
    {
        ""@Microsoft.OData.SampleService.Models.TripPin.CityName"":null,
        ""Address"":""Smoke-bag slanting street"",
        ""City"":
        {
            ""Name"":""Beijing"",
            ""CountryRegion"":""China""
        }
     },
    ""Gender"":""Female"",
    ""Concurrency@Org.OData.Core.V1.Computed"":null,
    ""Concurrency"":635549965782056080
}";
            TestAnnotation(response, () =>
            {
                var person = dsc.PeoplePlus.ByKey("aprilcline").GetValue();
                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(person.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.IsNull(annotation);

                Nullable<int> ageAnnotation = null;
                result = dsc.TryGetAnnotation<Nullable<int>>(person, "Microsoft.OData.SampleService.Models.TripPin.Age", out ageAnnotation);
                Assert.IsTrue(result);
                Assert.IsNull(ageAnnotation);
            });
        }

        [TestMethod]
        public void TestGetCollectionInstanceAnnotationWithOneNullItemOnEntity()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@Microsoft.OData.SampleService.Models.TripPin.SeoTerms"":
    [
        ""Russell1"",
        null,
        ""Whyte1""
    ],
    ""UserName"":""russellwhyte"",
    ""FirstName"":""Russell"",
    ""LastName"":""Whyte"",
    ""Emails"":[""Russell@example.com"",""Russell@contoso.com""],
    ""Location"": null,
     ""Gender"":""Male"",
     ""Age"":""22"",
     ""Concurrency"":635548229917715070
 }";
            TestAnnotation(response, () =>
            {
                var personQuery = dsc.PeoplePlus.ByKey("russellwhyte");
                var person = personQuery.GetValue();
                ICollection<string> annotation = null;

                bool result = dsc.TryGetAnnotation<ICollection<string>>(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);

                Assert.IsTrue(result);
                Assert.IsNotNull(annotation);
                Assert.AreEqual(3, annotation.Count);
                Assert.AreEqual("Russell1", annotation.ElementAt(0));
                Assert.IsNull(annotation.ElementAt(1));
                Assert.AreEqual("Whyte1", annotation.ElementAt(2));
            });
        }

        [TestMethod]
        public void TestGetDerivedInstanceWhileTheTermIsInBaseTypeAnnotationOnODataEntry()
        {
            string response = @"{
    ""@odata.context"":""http://odataService/$metadata#People/$entity"",
    ""@Microsoft.OData.SampleService.Models.TripPin.UnsupportedExpressions"":
    {
        ""@odata.type"":""#Microsoft.OData.SampleService.Models.TripPin.SearchRestrictionsType"",
        ""Searchable"":false,
        ""UnsupportedExpressions"":""NOT,OR""
    },
    ""UserName"":""aprilcline"",
    ""FirstName"":""April"",
    ""LastName"":""Cline"",
    ""Emails"":[""Russell@abc.com""],
    ""Location"":null,
    ""Gender"":""Female"",
    ""Concurrency"":635549965782056080
}";
            string unsupportExpressionTerm = "Microsoft.OData.SampleService.Models.TripPin.UnsupportedExpressions";
            TestAnnotation(response, () =>
            {
                dsc.MergeOption = MergeOption.OverwriteChanges;
                var person = dsc.PeoplePlus.ByKey("Jason").GetValue();
                SearchRestrictionsTypePlus annotation = null;
                bool result = dsc.TryGetAnnotation(person, unsupportExpressionTerm, out annotation);
                Assert.IsTrue(result);
                Assert.IsFalse(annotation.SearchablePlus.Value);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);

                SearchRestrictionsTypePlus unsupportExpressionAnnotation = null;
                result = dsc.TryGetAnnotation(person, unsupportExpressionTerm, out unsupportExpressionAnnotation);
                Assert.IsTrue(result);
                var typeCastedAnnotation = unsupportExpressionAnnotation as SearchRestrictionsTypePlus;
                Assert.IsNotNull(typeCastedAnnotation);
                Assert.IsFalse(typeCastedAnnotation.SearchablePlus.Value);

                SetResponse(response.Replace(@"""Searchable"":false,", @"""Searchable"":null,"));

                person = dsc.PeoplePlus.ByKey("Jason").GetValue();
                result = dsc.TryGetAnnotation(person, unsupportExpressionTerm, out annotation);
                Assert.IsTrue(result);
                Assert.IsNull(annotation.SearchablePlus);
                Assert.AreEqual(SearchExpressionsPlus.NOTPlus | SearchExpressionsPlus.ORPlus, annotation.UnsupportedExpressionsPlus);

                result = dsc.TryGetAnnotation(person, unsupportExpressionTerm, out unsupportExpressionAnnotation);
                Assert.IsTrue(result);
                typeCastedAnnotation = unsupportExpressionAnnotation as SearchRestrictionsTypePlus;
                Assert.IsNotNull(typeCastedAnnotation);
                Assert.IsNull(typeCastedAnnotation.SearchablePlus);
            });
        }

        [TestMethod]
        public void TestDisableInstanceAnnotationMaterializationOnODataFeed()
        {
            string response = FeedPayloadWithInstanceAnnotationOnFeedandEntry;
            TestAnnotation(response, () =>
            {
                dsc.DisableInstanceAnnotationMaterialization = true;
                var peopleQueryResponse = dsc.PeoplePlus.Execute();
                var people = peopleQueryResponse.ToList();

                // Get Enum annotation on feed, no instance annotation. 
                PersonGenderPlus annotation;
                bool result = dsc.TryGetAnnotation<PersonGenderPlus>(peopleQueryResponse, "Microsoft.OData.SampleService.Models.TripPin.Gender", out annotation);
                Assert.IsFalse(result);
                Assert.AreEqual(PersonGenderPlus.UnknownPlus, annotation);

                // Get annotaion on entry, no instance annotation, return metadata annotation.
                ICollection<string> annotation2 = null;
                result = dsc.TryGetAnnotation<ICollection<string>>(people[0], "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation2);
                Assert.IsTrue(result);
                Assert.AreEqual("Bob", annotation2.First());
                Assert.AreEqual("BobCat", annotation2.ElementAt(1));
            });
        }

        [TestMethod]
        public void TestDisableInstanceAnnotationMaterializationOnOnODataComplexValue()
        {
            TestAnnotation(ComplexValuePayloadWithInstanceAnnotationOnComplexValue, () =>
            {
                dsc.DisableInstanceAnnotationMaterialization = true;
                var getLocationQuery = dsc.PeoplePlus.ByKey("Jason").SetLocationPlus();
                var location = getLocationQuery.GetValue();

                string annotation = null;
                bool result = dsc.TryGetAnnotation<string>(location, "Microsoft.OData.SampleService.Models.TripPin.CityName", out annotation);
                Assert.IsTrue(result);
                Assert.AreEqual("Nanjing", annotation);
            });
        }

        [TestMethod]
        public void TestDisableInstanceAnnotationMaterializationOnCollectionPropertyInAnEntity()
        {
            TestAnnotation(EntryPayloadWithInstanceAnnotationOnEntry, () =>
            {
                dsc.DisableInstanceAnnotationMaterialization = true;
                var person = dsc.PeoplePlus.ByKey("russellwhyte").GetValue();

                // Get instance annotation on collection property
                ICollection<string> annotation;
                bool result = dsc.TryGetAnnotation<Func<ObservableCollection<string>>, ICollection<string>>(() => person.EmailsPlus, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out annotation);
                Assert.IsFalse(result);
                Assert.IsNull(annotation);

                // Get instance annotation on the same collection property
                bool annotation1;
                result = dsc.TryGetAnnotation<Func<ObservableCollection<string>>, bool>(() => person.EmailsPlus, "Org.OData.Core.V1.Computed", out annotation1);
                Assert.IsFalse(result);
                Assert.AreEqual(false, annotation1);
            });
        }
        
        [TestMethod]
        public void TestInstanceAnnotationsShouldBeEmptyIfNoInstanceAnnotationReturned()
        {
             const string FeedPayloadWithoutInstanceAnnotation = @"{
    ""@odata.context"":""http://odataService/$metadata#People"",
    ""value"": [
        {
            ""@odata.type"": ""#Microsoft.OData.SampleService.Models.TripPin.Person"",
            ""UserName"":""BobCat"",
            ""FirstName"": ""Bob"",
            ""LastName"": ""Cat"",
            ""Location"": null,
            ""CompanyLocation"": null,
            ""Gender"":""Male"",
            ""Age"":""22"",
            ""Concurrency"":635548229917715070
        },
        {
            ""@odata.type"": ""#Microsoft.OData.SampleService.Models.TripPin.VipCustomer"",
            ""UserName"": ""JillJones"",
            ""FirstName"": ""Jill"",
            ""LastName"": ""Jones"",
            ""Location"": null,
            ""CompanyLocation"": null,
            ""Gender"":""Male"",
            ""Age"":""22"",
            ""Concurrency"":635548229917715070
        }
    ]
}";

             TestAnnotation(FeedPayloadWithoutInstanceAnnotation, () =>
             {
                 var getFriendsQueryResponse = dsc.GetFriendsPlus("Russell").Execute() as QueryOperationResponse<PersonPlus>;
                 var friends = getFriendsQueryResponse.ToList();

                 Assert.AreEqual(0, dsc.InstanceAnnotations.Count);
             });
         }

        private void TestAnnotation(string response, Action testAction)
        {
            dsc.DisableInstanceAnnotationMaterialization = false;
            SetResponse(response);

            dsc.SendingRequest2 += (sender, args) =>
            {
                args.RequestMessage.SetHeader("Prefer", "odata.include-annotations=\"*\"");
            };
            testAction();
        }

        private void SetResponse(string response)
        {
            dsc.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                return new CustomizedHttpWebRequestMessage(args,
                    response,
                    new Dictionary<string, string>()
                    {
                        {"Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8"},
                        {"Preference-Applied", "odata.include-annotations=\"*\""}
                    });
            };
        }
    }
}