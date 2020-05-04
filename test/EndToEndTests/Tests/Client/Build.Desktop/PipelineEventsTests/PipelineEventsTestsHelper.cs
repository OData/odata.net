//---------------------------------------------------------------------
// <copyright file="PipelineEventsTestsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PipelineEventsTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReferenceModifiedClientTypes;
    using Xunit;

    public static class PipelineEventsTestsHelper
    {
        public static Order CreateNewOrder(int id = 999)
        {
            return new Order() { OrderId = id };
        }

        public static Car CreateNewCar()
        {
            return new Car() { VIN = 999, Description = "create" };
        }

        public static Customer CreateNewCustomer(int id = 999)
        {
            return new Customer()
            {
                CustomerId = id,
                Name = "Name",
                PrimaryContactInfo = new ContactDetails() { EmailBag = { "abc", "def" } },
                Auditing = new AuditInfo() { ModifiedBy = "create", ModifiedDate = new DateTimeOffset() }
            };
        }

        public static void VerifyModfiedCustomerEntry(DataServiceContextWrapper<DefaultContainer> contextWrapper, Customer e)
        {
            Assert.True(e.Name.EndsWith("ModifyPropertyValueCustomer_Materialized"), "Property value not updated");
            EntityDescriptor descriptor = contextWrapper.GetEntityDescriptor(e);
            Assert.True(descriptor.Identity.OriginalString.Contains("ModifyEntryId"), "Wrong Id");
            Assert.True(descriptor.EditLink.AbsoluteUri.Contains("http://myeditlink/ModifyEntryEditLink"), "Wrong EditLink");
            Assert.True(descriptor.OperationDescriptors.Where(op => op.Title == "ModifyEntryAction").Any(), "Action not added");
            foreach (var linkInfo in descriptor.LinkInfos)
            {
                if (linkInfo.NavigationLink != null)
                {
                    // In Jsonlight, navigation link is calculated using edit link after the reading delegates
                    Assert.True(linkInfo.NavigationLink.AbsoluteUri.StartsWith("http://myeditlink/ModifyEntryEditLink"), "Wrong navigation link");
                    //AssociationLink not updated
                    Assert.Equal("http://modifyassociationlinkurl/", linkInfo.AssociationLink.AbsoluteUri);
                }
            }
        }

        /// <summary>
        /// Modify entry Id
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyEntryId_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null
                        &&
                        ( args.Entry.TypeName.EndsWith("Customer")
                        || args.Entry.TypeName.EndsWith("Car")
                        || args.Entry.TypeName.EndsWith("Order")))
                    {
                        args.Entry.Id = new Uri(((args.Entry.Id == null ? string.Empty : args.Entry.Id.OriginalString) + "ModifyEntryId"), UriKind.RelativeOrAbsolute);
                    }
                };
            }
        }

        /// <summary>
        /// Modify nullable entry Id
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyNullableEntryId_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null && (args.Entry.TypeName.Contains("Customer") || args.Entry.TypeName.Contains("License")))
                    {
                        args.Entry.Id = new Uri(((args.Entry.Id == null ? string.Empty : args.Entry.Id.OriginalString) + "ModifyEntryId"), UriKind.RelativeOrAbsolute);
                    }
                };
            }
        }

        /// <summary>
        /// Modify entry EditLink when reading begins
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyEntryEditLink_ReadingStart
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null)
                    {
                        args.Entry.EditLink = new Uri("http://myeditlink/entry", UriKind.Absolute);
                    }
                };
            }
        }

        /// <summary>
        /// Modify entry edit link when reading ends
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyEntryEditLink_ReadingEnd
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null)
                    {
                        args.Entry.EditLink = new Uri(args.Entry.EditLink, "ModifyEntryEditLink");
                    }
                };
            }
        }

        /// <summary>
        /// Modify nullable entry edit link when reading ends
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyNullalbeEntryEditLink_ReadingEnd
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null)
                    {
                        args.Entry.EditLink = new Uri("http://odata.org/ModifyEntryEditLink");
                    }
                };
            }
        }

        /// <summary>
        /// Add property/navigation that is not selected, remove property that is selected in Message entry.
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyMessageEntry_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Message"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        properties.Add(new ODataProperty() { Name = "Body", Value = "ModifyMessageEntry_Reading" });
                        args.Entry.Properties = properties.Where(p => p.Name != "ToUsername");
                    }
                };
            }
        }

        /// <summary>
        /// Modify Computer entry type name and property value in reading pipeline
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyTypeName_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Computer") || args.Entry.TypeName.EndsWith("Machine"))
                    {
                        args.Entry.TypeName = args.Entry.TypeName.Replace("Computer", "Machine");
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty property = properties.Single(p => p.Name == "Name");
                        string value = property.Value == null ? string.Empty : (string)property.Value;
                        property.Value = value + "ModifyTypeName_Reading";
                    }
                };
            }
        }

        /// <summary>
        /// Modify Computer entry type name and property value in writing pipeline
        /// </summary>
        public static Action<WritingEntryArgs> ModifyTypeName_Writing
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Computer") || args.Entry.TypeName.EndsWith("Machine"))
                    {
                        args.Entry.TypeName = args.Entry.TypeName.Replace("Machine", "Computer");
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty property = properties.Single(p => p.Name == "Name");
                        string value = property.Value == null ? string.Empty : (string)property.Value;
                        property.Value = value + "ModifyTypeName_Writing";
                    }
                };
            }
        }

        /// <summary>
        /// Modify property value of Message entity.
        /// </summary>
        public static Action<MaterializedEntityArgs> ModifyMessageEntry_Materialized
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Message"))
                    {
                        (args.Entity as Message).Body += "ModifyMessageEntry_Materialized";
                    }
                };
            }
        }

        /// <summary>
        /// Add additional action to entry
        /// </summary>
        public static Action<ReadingEntryArgs> ModifyEntryAction_Reading
        {
            get
            {
                return args =>
                    {
                        if (args.Entry != null)
                        {
                            args.Entry.AddAction(new ODataAction() { Title = "ModifyEntryAction", Metadata = new Uri("#ModifyEntryAction", UriKind.Relative), Target = new Uri("http://svc/Target", UriKind.Absolute) });
                        }
                    };
            }
        }

        /// <summary>
        /// Modify association link name 
        /// </summary>
        public static Action<ReadingNestedResourceInfoArgs> ModifyLinkName_ReadingNavigationLink
        {
            get { return args => args.Link.Name += "ModifyLinkName"; }
        }

        /// <summary>
        /// modify association link value
        /// </summary>
        public static Action<ReadingNestedResourceInfoArgs> ModifyAssociationLinkUrl_ReadingNavigationLink
        {
            get
            {
                return args =>
                    {
                        args.Link.AssociationLinkUrl = new Uri("http://ModifyAssociationLinkUrl", UriKind.Absolute);
                    };
            }
        }

        /// <summary>
        /// Modify customer entity name
        /// </summary>
        public static Action<MaterializedEntityArgs> ModifyPropertyValueCustomer_Materialized
        {
            get
            {
                return args =>
                {
                    var customer = args.Entity as Customer;
                    if (customer != null)
                    {
                        customer.Name += "ModifyPropertyValueCustomer_Materialized";
                    }
                };
            }

        }

        /// <summary>
        /// Add a property to the entry, also remove a property from SpecialEmployee entry.
        /// </summary>
        public static Action<ReadingEntryArgs> AddRemovePropertySpecialEmployeeEntry_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null && args.Entry.TypeName.EndsWith("SpecialEmployee"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        properties.Add(new ODataProperty() { Name = "CarsLicensePlate", Value = "AddRemovePropertySpecialEmployeeEntry_Reading" });
                        args.Entry.Properties = properties.AsEnumerable();

                        args.Entry.Properties = properties.Where(p => p.Name != "CarsVIN");

                        // add an instance annotation for a test later
                        args.Entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("CustomInstanceAnnotations.Term1", new ODataPrimitiveValue("Value")));
                    }
                };
            }
        }

        /// <summary>
        /// Modify Cusomter entry to have null complex property, null action
        /// </summary>
        public static Action<ReadingEntryArgs> ChangeEntryPropertyToNull_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null && args.Entry.TypeName.EndsWith("Customer"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty property = properties.Single(p => p.Name == "Auditing");
                        property.Value = null;
                    }
                };
            }
        }

        /// <summary>
        /// Retrieve instance annotation, and set new property of Enum type in SpecialEmployee entity
        /// </summary>
        public static Action<MaterializedEntityArgs> AddEnumPropertySpecialEmployeeEntity_Materialized
        {
            get
            {
                return args =>
                    {
                        if (args.Entry != null && args.Entry.TypeName.EndsWith("SpecialEmployee"))
                        {
                            if (args.Entry.InstanceAnnotations.Where(a => a.Name == "CustomInstanceAnnotations.Term1").Any())
                            {
                                (args.Entity as SpecialEmployee).BonusLevel = 1;
                            }
                        }
                    };
            }
        }

        /// <summary>
        /// Modify primitive/complex/collection property values in Customer entity
        /// </summary>
        public static Action<MaterializedEntityArgs> ModifyPropertyValueCustomerEntity_Materialized
        {
            get
            {
                return args =>
                {
                    if (args.Entry != null && args.Entry.TypeName.EndsWith("Customer"))
                    {
                        Customer customer = args.Entity as Customer;
                        customer.PrimaryContactInfo.EmailBag.Add("ModifyPropertyValueCustomerEntity_Materialized");
                        customer.Auditing = new AuditInfo() { ModifiedDate = new DateTimeOffset(), ModifiedBy = "ModifyPropertyValueCustomerEntity_Materialized", };
                        customer.Name += "ModifyPropertyValueCustomerEntity_Materialized";
                    }
                };
            }
        }

        /// <summary>
        /// Modify feed id
        /// </summary>
        public static Action<ReadingFeedArgs> ModifyFeedId_ReadingFeed
        {
            get { return args => args.Feed.Id = new Uri("http://ModifyFeedId"); }
        }

        /// <summary>
        /// Modify feed next link
        /// </summary>
        public static Action<ReadingFeedArgs> ModifyNextlink_ReadingFeed
        {
            get { return args => args.Feed.NextPageLink = new Uri(args.Feed.Id.OriginalString + "ModifyNextlink", UriKind.Absolute); }
        }

        /// <summary>
        /// Modify Customer entity property value of primitive/complex/collection types in writing pipeline delegates.
        /// </summary>
        public static Action<WritingEntryArgs> ModifyPropertyValueCustomerEntity_Writing
        {
            get
            {
                return args =>
                {
                    var propertyValue = "UpdatedODataEntryPropertyValue";
                    if (args.Entry.TypeName.EndsWith("Customer"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty propertyName = properties.Where(p => p.Name == "Name").Single();
                        propertyName.Value = ((string)propertyName.Value) + propertyValue;

                        args.Entry.Properties = properties.AsEnumerable();

                        // The following update should have no effect
                        var notUsedValue = "ModifyPropertyValueCustomerEntity_Writing";
                        Customer customer = args.Entity as Customer;
                        customer.PrimaryContactInfo.EmailBag.Add(notUsedValue);
                        customer.Auditing = new AuditInfo() { ModifiedDate = new DateTimeOffset(), ModifiedBy = notUsedValue, };
                        customer.Name += notUsedValue;
                    }

                    if (args.Entry.TypeName.EndsWith("ContactDetails"))
                    {
                        var propertyEmailBag = args.Entry.Properties.Single(p => p.Name == "EmailBag");
                        (propertyEmailBag.Value as ODataCollectionValue).Items = new string[] { propertyValue };
                    }

                    if (args.Entry.TypeName.EndsWith("AuditInfo"))
                    {
                        args.Entry.Properties.Single(p => p.Name == "ModifiedBy").Value = propertyValue;
                    }
                };
            }
        }

        /// <summary>
        /// Modify Customer entry property value in writing pipeline delegates.
        /// </summary>
        public static Action<WritingEntryArgs> ModifyPropertyValueCustomerEntry_Writing
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Customer"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty property = properties.Where(p => p.Name == "Name").Single();
                        property.Value = ((string)property.Value) + "ModifyPropertyValueCustomerEntry_Writing";
                        args.Entry.Properties = properties.AsEnumerable();
                    }
                };
            }
        }

        /// <summary>
        /// In AddObject-SetLink scenario, modify the new entity property and association link in the payload
        /// </summary>
        public static Action<WritingNestedResourceInfoArgs> ModifyNavigationLink_WritingStart
        {
            get
            {
                return args =>
                {
                    args.Link.Url = new Uri(args.Link.Url.AbsoluteUri.Replace("(400)", "(401)"));
                    args.Link.Url = new Uri(args.Link.Url.AbsoluteUri.Replace("(1300)", "(1301)"));

                    // This update should have no effect
                    ((Order)args.Source).OrderId += 1;
                };
            }
        }

        /// <summary>
        /// In AddObject-SetLink scenario, modify the new entity property and association link in the payload
        /// </summary>
        public static Action<WritingNestedResourceInfoArgs> ModifyNavigationLink_WritingEnd
        {
            get
            {
                return args =>
                {
                    args.Link.Url = new Uri(args.Link.Url.AbsoluteUri.Replace("(402)", "(403)"));
                    args.Link.Url = new Uri(args.Link.Url.AbsoluteUri.Replace("(1302)", "(1303)"));

                    // This update should have no effect
                    ((Order)args.Source).OrderId += 1;
                };
            }
        }

        /// <summary>
        /// In AddObject-SetLink scenario, modify the new entity property and association link in the payload
        /// </summary>
        public static Action<WritingEntityReferenceLinkArgs> ModifyReferenceLink
        {
            get
            {
                return args =>
                {
                    args.EntityReferenceLink.Url = new Uri(args.EntityReferenceLink.Url.AbsoluteUri.Replace("(401)", "(402)"));
                    args.EntityReferenceLink.Url = new Uri(args.EntityReferenceLink.Url.AbsoluteUri.Replace("(1301)", "(1302)"));

                    // This update should have no effect
                    ((Order)args.Source).OrderId += 1;
                };
            }
        }

        /// <summary>
        /// Modify Car entity property
        /// </summary>
        public static Action<WritingEntryArgs> ModifyPropertyValueCarEntity_Writing
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Car"))
                    {
                        Car car = args.Entity as Car;
                        car.Description += "ModifyPropertyValueCarEntity_Writing";
                    }
                };
            }
        }

        /// <summary>
        /// Modify car entry property
        /// </summary>
        public static Action<WritingEntryArgs> ModifyPropertyValueCarEntry_Writing
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Car"))
                    {
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        ODataProperty property = properties.Where(p => p.Name == "Description").Single();
                        property.Value = ((string)property.Value) + "ModifyPropertyValueCarEntry_Writing";
                        args.Entry.Properties = properties.AsEnumerable();
                    }
                };
            }
        }

        /// <summary>
        /// A writing pipeline delegate that throws exception
        /// </summary>
        public static Action<WritingEntryArgs> ThrowException_Writing
        {
            get
            {
                return args =>
                {
                    throw new Exception("ThrowException_Writing");
                };
            }
        }

        /// <summary>
        /// A reading pipeline delegate that throws exception
        /// </summary>
        public static Action<ReadingEntryArgs> ThrowException_Reading
        {
            get
            {
                return args =>
                {
                    throw new Exception("ThrowException_Reading");
                };
            }
        }

        /// <summary>
        /// Add additional collection property to Login entry (V2 types only) in response
        /// </summary>
        public static Action<ReadingEntryArgs> AddCollectionProperty_Reading
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Login"))
                    {
                        ODataProperty property = new ODataProperty() { Name = "Logs", Value = new ODataCollectionValue() { TypeName = "Collection(Edm.String)", Items = new[] { "a", "b" } } };
                        List<ODataProperty> properties = args.Entry.Properties.ToList();
                        properties.Add(property);
                        args.Entry.Properties = properties.AsEnumerable();
                    }
                };
            }
        }

        /// <summary>
        /// Add additional collection property to Computer entry (V2 types only) in request
        /// </summary>
        public static Action<WritingEntryArgs> AddCollectionProperty_Writing
        {
            get
            {
                return args =>
                {
                    if (args.Entry.TypeName.EndsWith("Computer"))
                    {
                        ODataProperty property = new ODataProperty() { Name = "Name", Value = new ODataCollectionValue() { TypeName = "Collection(Edm.String)", Items = new[] { "a", "b" } } };
                        List<ODataProperty> properties = args.Entry.Properties.Where(p => p.Name != "Name").ToList();
                        properties.Add(property);
                        args.Entry.Properties = properties.AsEnumerable();
                    }
                };
            }
        }

        private static string IncrementIdInUri(string originalUri)
        {
            // modify the originalUri like http://service.svc/Customer(100) to be http://service.svc/Customer(101)
            const string pattern = @"\(([0-9]+)\)";
            return Regex.Replace(originalUri, pattern,
                                 (match) => "(" + (int.Parse(match.Value.Substring(1, match.Length - 2)) + 1) + ")");
        }
    }
}


