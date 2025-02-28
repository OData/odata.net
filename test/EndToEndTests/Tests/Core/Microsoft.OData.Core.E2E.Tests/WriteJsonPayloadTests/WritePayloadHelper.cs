//---------------------------------------------------------------------
// <copyright file="WritePayloadHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.Edm;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Core.E2E.Tests.WriteJsonPayloadTests
{
    /// <summary>
    /// Some helper methods to create various ODataResourceSet/Entry/values.
    /// </summary>
    public class WritePayloadTestsHelper
    {
        private readonly Uri _baseUri;
        private readonly IEdmModel _model;

        public WritePayloadTestsHelper(Uri baseUri, IEdmModel model)
        {
            _baseUri = baseUri;
            _model = model;
        }

        public const string NameSpace = "Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.";

        #region OrderFeedTestHelper

        public ODataResource CreateOrderEntry1(bool hasModel)
        {
            var orderEntry1 = CreateOrderEntry1NoMetadata(hasModel);
            orderEntry1.Id = new Uri(_baseUri.AbsoluteUri + "Order(-10)");
            orderEntry1.EditLink = new Uri(_baseUri.AbsoluteUri + "Order(-10)");

            return orderEntry1;
        }

        public ODataResourceWrapper CreateOrderEntry2(bool hasModel)
        {
            var orderEntry2Wrapper = CreateOrderEntry2NoMetadata(hasModel);
            orderEntry2Wrapper.Resource.Id = new Uri(_baseUri.AbsoluteUri + "Order(-9)");
            orderEntry2Wrapper.Resource.EditLink = new Uri(_baseUri.AbsoluteUri + "Order(-9)");

            return orderEntry2Wrapper;
        }

        #endregion OrderFeedTestHelper

        #region ExpandedCustomerEntryTestHelper

        public ODataResourceWrapper CreateCustomerEntry(bool hasModel)
        {
            var customerEntryWrapper = CreateCustomerResourceWrapperNoMetadata(hasModel);
            var customerEntry = customerEntryWrapper.Resource;
            customerEntry.Id = new Uri(_baseUri.AbsoluteUri + "Customers(-9)");

            var customerEntryP6 = new ODataProperty()
            {
                Name = "Video",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Video"),
                }
            };

            var customerEntryP7 = new ODataProperty()
            {
                Name = "Thumbnail",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Thumbnail"),
                }
            };

            var properties = customerEntry.Properties.ToList();
            properties.Add(customerEntryP6);
            properties.Add(customerEntryP7);
            customerEntry.Properties = properties.ToArray();
            customerEntry.EditLink = new Uri(_baseUri.AbsoluteUri + "Customers(-9)");

            return customerEntryWrapper;
        }

        public ODataResourceWrapper CreatePrimaryContactODataWrapper()
        {
            var emailBag = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                {
                    "c",
                    "vluxyßhmibqsbifocryvfhcßjmgkdagjßavhcelfjqazacnlmauprxhkcbjhrssdiyctbd",
                    "ぴダグマァァﾈぴﾈ歹黑ぺぺミミぞボ"
                }
            };

            var alternativeNames = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new[]
                {
                    "rmjhkvrovdnfeßqllqrehpogavcnlliqmoqsbvkinbtoyolqlmxobhhejihrnoqguzvzhssfrb"
                }
            };

            var contactAlias = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Aliases",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[]
                                {
                                    "uymiyzgjfbsrqfiqfprsscdxksykfizfztdxdifdnhsnamuutsscxyssrsmaijakagjyvzgkxnßgonnsvzsssshxejßipg"
                                }
                            }
                        }
                    }
                }
            };

            var homePhone = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "1234"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "5678"
                        },
                    }
                }
            };

            var workPhone = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "elvfevjyssuako"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "fltuu"
                        },
                    }
                }
            };

            var mobilePhoneBag = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet()
                {
                    TypeName = "Collection(" + NameSpace + "Phone)",
                },
                Resources = new List<ODataResourceWrapper>()
                {
                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value =
                                        "hkugxatukjjdimßytgkqyopßitßdyzexdkmmarpojjzqycqqvsuztzidxudieldnhnßrakyetgbkbßoyoglbtoiggdsxjlezu"
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value = "ypfuiuhrqevehzrziuckpf"
                                }
                            }
                        }
                    }
                }
            };

            return new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "ContactDetails",
                    Properties = new[]
                        {
                            new ODataProperty
                                {
                                    Name = "EmailBag",
                                    Value = emailBag
                                },
                            new ODataProperty
                                {
                                    Name = "AlternativeNames",
                                    Value = alternativeNames
                                },
                        }
                },

                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "ContactAlias",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = contactAlias
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "HomePhone",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = homePhone
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "WorkPhone",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = workPhone
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "MobilePhoneBag",
                            IsCollection = true
                        },
                        NestedResourceOrResourceSet = mobilePhoneBag
                    }
                }
            };
        }

        public ODataResourceSetWrapper CreateBackupContactODataWrapper()
        {
            var emailBag = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                {
                    "irbkxhydugvnsytkckx"
                }
            };

            var alternativeNames = new ODataCollectionValue()
            {
                TypeName = "Collection(Edm.String)",
                Items = new string[]
                {
                    "ezphrstutiyrmnoapgfmxnzojaobcpouzrsxgcjicvndoxvdlboxtkekalyqpmxuzssuubphxbfaaqzmuuqakchkqdvvd",
                    "ßjfhuakdntßpuakgmjmvyystgdupgviotqeqhpjuhjludxfqvnfydrvisneyxyssuqxx"
                }
            };

            var contactAliasWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Aliases",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[]
                                {
                                    "ァソソゼ黑ゾタｦダ亜弌ゾぺ畚せ歹ｚ黑欲ダタんゾソマたゼﾝ匚ボﾝハク裹黑ぺァマ弌ぁゾａをぞたまゼﾝ九マぁ黑ぞゼソяｦЯミ匚ぜダび裹亜べそんｚ珱タぼぞ匚ёハяァんゼ九ゼほせハせソｦゼ裹ぼんﾈяｦｦ九ゼグｚ",
                                    "xutt"
                                }
                            }
                        }
                    }
                }
            };

            var homePhoneWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = "zymn"
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "iußkgesaijemzupzrvuqmxmbjpassazrgcicfmcsseqtnetßoufpyjduhcrveteußbutfxmfhjyiavdkkjkxrjaci"
                        },
                    }
                }
            };

            var workPhoneWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpace + "Phone",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "PhoneNumber",
                            Value = null
                        },
                        new ODataProperty
                        {
                            Name = "Extension",
                            Value = "avsgfzrdpacjlosmybfp"
                        },
                    }
                }
            };

            var mobilePhoneBagWrapper = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet() { TypeName = "Collection(" + NameSpace + "Phone)" },
                Resources = new List<ODataResourceWrapper>()
                {
                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value = null
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value = "ximrqcriuazoktucrbpszsuikjpzuubcvgycogqcyeqmeeyzoakhpvtozkcbqtfhxr"
                                },
                            }
                        }
                    },

                    new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpace + "Phone",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "PhoneNumber",
                                    Value = "scvffqyenctjnoxgilyqdfbmregufyuakq"
                                },
                                new ODataProperty
                                {
                                    Name = "Extension",
                                    Value = "珱タほバミひソゾｚァせまゼミ亜タёゼяをバをを匚マポソ九ｚｚバ縷ソ九"
                                },
                            }
                        },
                    }
                }
            };

            var contactDetailsWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "ContactDetails",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "EmailBag",
                            Value = emailBag
                        },
                        new ODataProperty
                        {
                            Name = "AlternativeNames",
                            Value = alternativeNames
                        }
                    }
                },

                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "ContactAlias",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = contactAliasWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "HomePhone",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = homePhoneWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "WorkPhone",
                            IsCollection = false
                        },

                        NestedResourceOrResourceSet = workPhoneWrapper
                    },

                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "MobilePhoneBag",
                            IsCollection = true
                        },

                        NestedResourceOrResourceSet = mobilePhoneBagWrapper
                    }
                }
            };

            return new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet()
                {
                    TypeName = "Collection(" + NameSpace + "ContactDetails)"
                },

                Resources = new List<ODataResourceWrapper>()
                {
                    CreatePrimaryContactODataWrapper(),
                    contactDetailsWrapper
                }
            };
        }

        public static ODataResourceWrapper CreateAuditInforWrapper()
        {
            return new ODataResourceWrapper()
            {
                Resource = new ODataResource
                {
                    TypeName = NameSpace + "AuditInfo",
                    Properties = new[]
                    {
                        new ODataProperty
                            {
                                Name = "ModifiedDate",
                                Value = new DateTimeOffset()
                            },
                        new ODataProperty
                            {
                                Name = "ModifiedBy",
                                Value = "ボァゼあクゾ"
                            }
                    }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "Concurrency",
                        },

                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpace + "ConcurrencyInfo",
                                Properties = new[]
                                {
                                    new ODataProperty()
                                    {
                                        Name = "Token",
                                        Value =
                                            "tyoyfuhsbfzsnycgfciusrsucysxrdeamozidbrevbvfgpkhcgzlogyeuyqgilaxczbjzo"
                                    },
                                    new ODataProperty()
                                    {
                                        Name = "QueriedDateTime",
                                        Value = null
                                    },
                                }
                            }
                        }
                    }
                }
            };
        }

        public IEnumerable<ODataNestedResourceInfoWrapper> CreateCustomerNavigationLinks()
        {
            return new List<ODataNestedResourceInfoWrapper>()
            {
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Orders",
                        IsCollection = true,
                        Url = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Orders")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Husband",
                        IsCollection = false,
                        Url = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Husband")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Wife",
                        IsCollection = false,
                        Url = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Wife")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Info",
                        IsCollection = false,
                        Url = new Uri(_baseUri.AbsoluteUri + "Customers(-9)/Info")
                    }
                }
            };
        }

        public ODataResource CreateLoginEntry(bool hasModel)
        {
            var loginEntry = CreateLoginEntryNoMetadata(hasModel);
            loginEntry.Id = new Uri(_baseUri.AbsoluteUri + "Logins('2')");
            loginEntry.EditLink = new Uri(_baseUri.AbsoluteUri + "Logins('2')");

            return loginEntry;
        }

        public IEnumerable<ODataNestedResourceInfoWrapper> CreateLoginNavigationLinksWrapper()
        {
            return new List<ODataNestedResourceInfoWrapper>()
            {
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Customer",
                        IsCollection = false,
                        Url = new Uri(_baseUri.AbsoluteUri + "Login('2')/Customer")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "LastLogin",
                        IsCollection = false,
                        Url = new Uri(_baseUri.AbsoluteUri + "Login('2')/LastLogin")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "SentMessages",
                        IsCollection = true,
                        Url = new Uri(_baseUri.AbsoluteUri + "Login('2')/SentMessages")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "ReceivedMessages",
                        IsCollection = true,
                        Url = new Uri(_baseUri.AbsoluteUri + "Login('2')/ReceivedMessages")
                    }
                },
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Orders",
                        IsCollection = true,
                        Url = new Uri(_baseUri.AbsoluteUri + "Login('2')/Orders")
                    }
                },
            };
        }

        #endregion ExpandedCustomerEntryTestHelper

        #region PersonFeedTestHelper

        public ODataResource CreatePersonEntry(bool hasModel)
        {
            var personEntry = CreatePersonEntryNoMetadata(hasModel);
            personEntry.Id = new Uri(_baseUri.AbsoluteUri + "People(-5)");
            personEntry.EditLink = new Uri(_baseUri.AbsoluteUri + "People(-5)");

            return personEntry;
        }

        public ODataResource CreateEmployeeEntry(bool hasModel)
        {
            var employeeEntry = CreateEmployeeEntryNoMetadata(hasModel);
            employeeEntry.Id = new Uri(_baseUri.AbsoluteUri + "People(-3)");
            employeeEntry.EditLink = new Uri(_baseUri.AbsoluteUri + "People(-3)/" + NameSpace + "Employee", UriKind.Absolute);
            employeeEntry.AddAction(
                new ODataAction()
                {
                    Metadata = new Uri(_baseUri.AbsoluteUri + "$metadata#" + NameSpace + "Sack"),
                    Target = new Uri(_baseUri.AbsoluteUri + "People(-3)/" + NameSpace + "Employee" + "/Sack"),
                    Title = "Sack"
                });

            return employeeEntry;
        }

        public ODataResource CreateSpecialEmployeeEntry(bool hasModel)
        {
            var employeeEntry = CreateSpecialEmployeeEntryNoMetadata(hasModel);
            employeeEntry.Id = new Uri(_baseUri.AbsoluteUri + "People(-10)");
            employeeEntry.EditLink = new Uri("People(-10)/" + NameSpace + "SpecialEmployee", UriKind.Relative);
            employeeEntry.AddAction(
                new ODataAction()
                {
                    Metadata = new Uri(_baseUri.AbsoluteUri + "$metadata#" + NameSpace + "Sack"),
                    Target = new Uri(_baseUri.AbsoluteUri + "People(-10)/" + NameSpace + "SpecialEmployee" + "/Sack"),
                    Title = "Sack"
                });

            return employeeEntry;
        }

        #endregion PersonFeedTestHelper

        public ODataResource CreateCarEntry(bool hasModel)
        {
            var carEntry = CreateCarEntryNoMetadata(hasModel);
            carEntry.Id = new Uri(_baseUri.AbsoluteUri + "Cars(11)");
            carEntry.EditLink = new Uri(_baseUri.AbsoluteUri + "Cars(11)");
            carEntry.ReadLink = new Uri(_baseUri.AbsoluteUri + "Cars(11)");

            var carEntryP3 = new ODataProperty()
            {
                Name = "Photo",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(_baseUri.AbsoluteUri + "Cars(11)/Photo"),
                }
            };

            var carEntryP4 = new ODataProperty()
            {
                Name = "Video",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(_baseUri.AbsoluteUri + "Cars(11)/Video"),
                }
            };

            var properties = carEntry.Properties.ToList();
            properties.Add(carEntryP3);
            properties.Add(carEntryP4);

            carEntry.Properties = properties.AsEnumerable();

            // MLE
            carEntry.MediaResource = new ODataStreamReferenceValue()
            {
                EditLink = new Uri(_baseUri.AbsoluteUri + "Cars(11)/$value"),
            };

            return carEntry;
        }

        public static ODataResource CreateOrderEntry1NoMetadata(bool hasModel)
        {
            var orderEntry1 = new ODataResource()
            {
                TypeName = NameSpace + "Order",
            };

            var orderEntry1P1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderEntry1P2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderEntry1P3 = new ODataProperty { Name = "Concurrency", Value = null };
            orderEntry1.Properties = new[] { orderEntry1P1, orderEntry1P2, orderEntry1P3 };

            if (!hasModel)
            {
                orderEntry1P1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return orderEntry1;
        }

        public static ODataResourceWrapper CreateOrderEntry2NoMetadata(bool hasModel)
        {
            var orderEntry2 = new ODataResource()
            {
                TypeName = NameSpace + "Order"
            };

            var orderEntry2P1 = new ODataProperty { Name = "OrderId", Value = -9 };
            var orderEntry2P2 = new ODataProperty { Name = "CustomerId", Value = 78 };
            var Concurrency_nestedInfo = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Concurrency",
                    IsCollection = false
                },
                NestedResourceOrResourceSet = new ODataResourceWrapper()
                {
                    Resource = new ODataResource()
                    {
                        TypeName = NameSpace + "ConcurrencyInfo",
                        Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Token",
                                Value = "muunxfmcubaihvgnzoojgecdztyipapnxahnuibukrveamumfuokuvbly"
                            },
                            new ODataProperty
                            {
                                Name = "QueriedDateTime",
                                Value = new DateTimeOffset(new DateTime(634646431705072026, DateTimeKind.Utc))
                            }
                        }
                    }
                }
            };

            orderEntry2.Properties = new[] { orderEntry2P1, orderEntry2P2 };

            if (!hasModel)
            {
                orderEntry2P1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });

                Concurrency_nestedInfo.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
            }

            return new ODataResourceWrapper()
            {
                Resource = orderEntry2,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>() { Concurrency_nestedInfo }
            };
        }

        public static ODataNestedResourceInfo AddOrderEntryCustomNavigation(ODataResource orderEntry, Dictionary<string, object> expectedOrderObject, bool hasModel)
        {
            string myServiceUri = "http://myservice.svc/";
            orderEntry.Id = new Uri(myServiceUri + "Order(-9)");

            orderEntry.EditLink = new Uri(myServiceUri + "Order(-9)");

            orderEntry.ETag = "orderETag";
            orderEntry.ReadLink = new Uri(myServiceUri + "orderEntryReadLink");

            expectedOrderObject[JsonLightConstants.ODataIdAnnotationName] = orderEntry.Id.ToString();
            expectedOrderObject[JsonLightConstants.ODataETagAnnotationName] = orderEntry.ETag;
            expectedOrderObject[JsonLightConstants.ODataEditLinkAnnotationName] = orderEntry.EditLink.AbsoluteUri;
            expectedOrderObject[JsonLightConstants.ODataReadLinkAnnotationName] = orderEntry.ReadLink.OriginalString;

            var orderEntry2Navigation = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri("Order(-9)/Customer", UriKind.Relative),
                AssociationLinkUrl = new Uri("Order(-9)/Customer/$ref", UriKind.Relative),
            };
            expectedOrderObject["Customer" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = "Order(-9)/Customer";
            expectedOrderObject["Customer" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = "Order(-9)/Customer/$ref";

            if (hasModel)
            {
                // Login navigation is not specified by user, thus will not be in no-model result
                expectedOrderObject["Login" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = orderEntry.ReadLink + "/Login";
                expectedOrderObject["Login" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = orderEntry.ReadLink + "/Login/$ref";
            }

            return orderEntry2Navigation;
        }

        public ODataResourceWrapper CreateCustomerResourceWrapperNoMetadata(bool hasModel)
        {
            var customerEntry = new ODataResource()
            {
                TypeName = NameSpace + "Customer"
            };

            var customerEntryP1 = new ODataProperty { Name = "CustomerId", Value = -9 };
            var customerEntryP2 = new ODataProperty { Name = "Name", Value = "CustomerName" };

            var primaryContactInfo_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "PrimaryContactInfo",
                    IsCollection = false,
                },

                NestedResourceOrResourceSet = CreatePrimaryContactODataWrapper()
            };

            var BackupContactInfo_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "BackupContactInfo",
                    IsCollection = true,
                },

                NestedResourceOrResourceSet = CreateBackupContactODataWrapper()
            };

            var Auditing_nestedInfoWrapper = new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Auditing",
                    IsCollection = false,
                },

                NestedResourceOrResourceSet = CreateAuditInforWrapper()
            };

            if (!hasModel)
            {
                primaryContactInfo_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
                BackupContactInfo_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
                Auditing_nestedInfoWrapper.NestedResourceInfo.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsComplex = true });
            }

            customerEntry.Properties = new[]
                {
                    customerEntryP1, customerEntryP2,
                };

            if (!hasModel)
            {
                customerEntry.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    NavigationSourceName = "Customers",
                    NavigationSourceEntityTypeName = NameSpace + "Customer",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                });
                customerEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return new ODataResourceWrapper()
            {
                Resource = customerEntry,
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    primaryContactInfo_nestedInfoWrapper,
                    BackupContactInfo_nestedInfoWrapper,
                    Auditing_nestedInfoWrapper
                }
            };
        }

        public static ODataResource CreateLoginEntryNoMetadata(bool hasModel)
        {
            var loginEntry = new ODataResource()
            {
                TypeName = NameSpace + "Login"
            };

            var loginEntryP1 = new ODataProperty { Name = "Username", Value = "2" };
            var loginEntryP2 = new ODataProperty { Name = "CustomerId", Value = 6084 };
            loginEntry.Properties = new[] { loginEntryP1, loginEntryP2 };

            if (!hasModel)
            {
                if (loginEntry.Properties is List<ODataProperty> properties)
                {
                    var usernameProperty = properties.SingleOrDefault(p => p.Name == "Username");
                    if (usernameProperty != null)
                    {
                        usernameProperty.SetSerializationInfo(new ODataPropertySerializationInfo
                        {
                            PropertyKind = ODataPropertyKind.Key
                        });
                    }
                }
            }

            return loginEntry;
        }

        internal ODataProperty AddCustomerMediaProperty(ODataResource customerEntry, Dictionary<string, object> expectedCustomerObject)
        {
            var thumbnailProperty = new ODataProperty()
            {
                Name = "Thumbnail",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri(_baseUri.AbsoluteUri + "Customer(-9)/Thumbnail"),
                    ReadLink = new Uri(_baseUri.AbsoluteUri + "Customer(-9)/ThumbnailReadLink"),
                }
            };
            var properties = customerEntry.Properties.ToList();
            properties.Add(thumbnailProperty);
            customerEntry.Properties = properties.ToArray();

            expectedCustomerObject.Add("Thumbnail" + JsonLightConstants.ODataMediaEditLinkAnnotationName, (thumbnailProperty.Value as ODataStreamReferenceValue).EditLink.AbsoluteUri);
            expectedCustomerObject.Add("Thumbnail" + JsonLightConstants.ODataMediaReadLinkAnnotationName, (thumbnailProperty.Value as ODataStreamReferenceValue).ReadLink.AbsoluteUri);

            return thumbnailProperty;
        }

        public ODataNestedResourceInfo CreateCustomerOrderNavigation(Dictionary<string, object> expectedCustomerObject)
        {
            // create navigation and add expected metadata for no-model verification, use non-trival navigation link to verify association link is calculated
            var orderNavigation = new ODataNestedResourceInfo() { Name = "Orders", IsCollection = true, Url = new Uri(_baseUri.AbsoluteUri + "Customerdf(-9)/MyOrders") };
            expectedCustomerObject["Orders" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = orderNavigation.Url.AbsoluteUri;
            expectedCustomerObject["Orders" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = _baseUri.AbsoluteUri + "Customer(-9)/Orders/$ref";
            return orderNavigation;
        }

        public ODataNestedResourceInfo CreateExpandedCustomerLoginsNavigation(Dictionary<string, object> expectedCustomerObject)
        {
            // create expand navigation and add expected infor for no-model verification
            string myServiceUri = "http://myservice.svc/";
            var expandedLoginsNavigation = new ODataNestedResourceInfo()
            {
                Name = "Logins",
                IsCollection = true,
                Url = new Uri(myServiceUri + "Customer(-9)/Logins"),
                AssociationLinkUrl = new Uri(_baseUri.AbsoluteUri + "Customer(-9)/Logins/$ref"),
            };
            expectedCustomerObject["Logins" + "@" + JsonLightConstants.ODataNavigationLinkUrlAnnotationName] = expandedLoginsNavigation.Url.AbsoluteUri;
            expectedCustomerObject["Logins" + "@" + JsonLightConstants.ODataAssociationLinkUrlAnnotationName] = expandedLoginsNavigation.AssociationLinkUrl.AbsoluteUri;
            return expandedLoginsNavigation;
        }

        public static ODataResource CreatePersonEntryNoMetadata(bool hasModel)
        {
            var personEntry = new ODataResource()
            {
                TypeName = NameSpace + "Person"
            };

            var personEntryP1 = new ODataProperty { Name = "PersonId", Value = -5 };
            var personEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "xhsdckkeqzvlnprheujeycqrglfehtdocildrequohlffazfgtvmddyqsaxrojqxrsckohrakdxlrghgmzqnyruzu"
            };

            personEntry.Properties = new[] { personEntryP1, personEntryP2 };

            if (!hasModel)
            {
                personEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return personEntry;
        }

        public static ODataResource CreateEmployeeEntryNoMetadata(bool hasModel)
        {
            var employeeEntry = new ODataResource()
            {
                TypeName = NameSpace + "Employee"
            };

            var employeeEntryP1 = new ODataProperty { Name = "PersonId", Value = -3 };
            var employeeEntryP2 = new ODataProperty
            {
                Name = "Name",
                Value = "ybqmssrdtjßcbhhmfxvhoxlssekuuibnmltiahdssxnpktmtorxfmeßbbujc"
            };

            var employeeEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = -465010984 };
            var employeeEntryP4 = new ODataProperty { Name = "Salary", Value = 0 };
            var employeeEntryP5 = new ODataProperty
            {
                Name = "Title",
                Value = "ミソまグたя縷ｦ弌ダゼ亜ゼをんゾ裹亜マゾダんタァハそポ縷ぁボグ黑珱ぁяポグソひゾひЯグポグボ欲を亜"
            };

            employeeEntry.Properties = new[] { employeeEntryP1, employeeEntryP2, employeeEntryP3, employeeEntryP4, employeeEntryP5 };

            if (!hasModel)
            {
                employeeEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return employeeEntry;
        }

        public static ODataResource CreateSpecialEmployeeEntryNoMetadata(bool hasModel)
        {
            var employeeEntry = new ODataResource()
            {
                TypeName = NameSpace + "SpecialEmployee"
            };

            var employeeEntryP1 = new ODataProperty { Name = "PersonId", Value = -10 };
            var employeeEntryP2 = new ODataProperty { Name = "Name", Value = "a special employee" };
            var employeeEntryP3 = new ODataProperty { Name = "ManagersPersonId", Value = 47 };
            var employeeEntryP4 = new ODataProperty { Name = "Salary", Value = 4091 };
            var employeeEntryP5 = new ODataProperty { Name = "Title", Value = "a special title" };
            var employeeEntryP6 = new ODataProperty { Name = "CarsVIN", Value = -1911530027 };
            var employeeEntryP7 = new ODataProperty { Name = "Bonus", Value = -37730565 };
            var employeeEntryP8 = new ODataProperty { Name = "IsFullyVested", Value = false };
            employeeEntry.Properties = new[] { employeeEntryP1, employeeEntryP2, employeeEntryP3, employeeEntryP4, employeeEntryP5, employeeEntryP6, employeeEntryP7, employeeEntryP8 };

            if (!hasModel)
            {
                employeeEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return employeeEntry;
        }

        public static ODataResource CreateCarEntryNoMetadata(bool hasModel)
        {
            var carEntry = new ODataResource()
            {
                TypeName = NameSpace + "Car",
            };

            // properties and named streams
            var carEntryP1 = new ODataProperty { Name = "VIN", Value = 11 };
            var carEntryP2 = new ODataProperty
            {
                Name = "Description",
                Value = "cenbviijieljtrtdslbuiqubcvhxhzenidqdnaopplvlqc"
            };

            carEntry.Properties = new[] { carEntryP1, carEntryP2 };

            carEntry.InstanceAnnotations.Add(
                new ODataInstanceAnnotation(
                    "carEntry.AnnotationName",
                    new ODataPrimitiveValue("carEntryAnnotationValue")));

            if (!hasModel)
            {
                carEntry.SetSerializationInfo(
                    new ODataResourceSerializationInfo()
                    {
                        NavigationSourceName = "Cars",
                        NavigationSourceEntityTypeName = NameSpace + "Car",
                        NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                    });

                carEntryP1.SetSerializationInfo(new ODataPropertySerializationInfo()
                {
                    PropertyKind = ODataPropertyKind.Key
                });
            }

            return carEntry;
        }

        /// <summary>
        /// Read the response message and perform given verifications
        /// </summary>
        /// <param name="isFeed">Whether the response has a feed</param>
        /// <param name="responseMessage">The response message</param>
        /// <param name="expectedSet">Expected IEdmEntitySet</param>
        /// <param name="expectedType">Expected IEdmEntityType</param>
        /// <param name="verifyFeed">Action to verify the feed</param>
        /// <param name="verifyEntry">Action to verify the entry</param>
        /// <param name="verifyNavigation">Action to verify the navigation</param>
        public async Task ReadAndVerifyFeedEntryMessageAsync(
            bool isFeed,
            TestStreamResponseMessage responseMessage,
            IEdmEntitySet expectedSet,
            IEdmEntityType expectedType,
            Action<ODataResourceSet> verifyFeed,
            Action<ODataResource> verifyEntry,
            Action<ODataNestedResourceInfo> verifyNavigation)
        {
            var settings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
            settings.ShouldIncludeAnnotation = s => true;
            ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, _model);
            ODataReader reader = isFeed
                ? await messageReader.CreateODataResourceSetReaderAsync(expectedSet, expectedType)
                : await messageReader.CreateODataResourceReaderAsync(expectedSet, expectedType);

            while (await reader.ReadAsync())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceSetEnd:
                        {
                            if (verifyFeed != null)
                            {
                                verifyFeed((ODataResourceSet)reader.Item);
                            }

                            break;
                        }
                    case ODataReaderState.ResourceEnd:
                        {
                            if (verifyEntry != null && reader.Item != null)
                            {
                                verifyEntry((ODataResource)reader.Item);
                            }

                            break;
                        }
                    case ODataReaderState.NestedResourceInfoEnd:
                        {
                            if (verifyNavigation != null)
                            {
                                verifyNavigation((ODataNestedResourceInfo)reader.Item);
                            }

                            break;
                        }
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        /// <summary>
        /// Read and return the stream content
        /// </summary>
        /// <param name="stream">The stream content</param>
        /// <returns></returns>
        public async Task<string> ReadStreamContentAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);

            return await streamReader.ReadToEndAsync();
        }

        /// <summary>
        /// Verify that two atom/json writer results are equivalent
        /// </summary>
        /// <param name="writerOuput1">The first string</param>
        /// <param name="writerOutput2">The second string</param>
        /// <param name="mimeType">The mime type</param>
        public void VerifyPayloadString(string writerOuput1, string writerOutput2, string mimeType)
        {
            if (string.Equals(mimeType, MimeTypes.ApplicationAtomXml, StringComparison.Ordinal))
            {
                // resulting atom payloads with/without model should be the same except for the updated time stamps
                const string pattern = @"<updated>([A-Za-z0-9\-\:]{20})\</updated>";
                const string replacement = "<updated>0000-00-00T00:00:00Z</updated>";
                writerOuput1 = Regex.Replace(writerOuput1, pattern, (match) => replacement);
                writerOutput2 = Regex.Replace(writerOutput2, pattern, (match) => replacement);
                Assert.Equal(writerOuput1, writerOutput2);
            }
            else
            {
                Assert.Equal(writerOuput1, writerOutput2);
            }
        }

        /// <summary>
        /// Construct the default expected Json object with all metadata
        /// </summary>
        /// <param name="entityType">The IEdmEntityType</param>
        /// <param name="relativeEditLink">The relative edit link</param>
        /// <param name="entry">The ODataEntry </param>
        /// <param name="hasModel">Whether IEdmModel is provided to writer</param>
        /// <param name="isDerivedType">Whether the entry is of derived type</param>
        /// <returns>The expected Json object</returns>
        internal Dictionary<string, object> ComputeExpectedFullMetadataEntryObject(IEdmEntityType entityType, string relativeEditLink, ODataResource entry, bool hasModel, bool isDerivedType = false)
        {
            var derivedTypeNameSegment = string.Empty;

            if (isDerivedType)
            {
                derivedTypeNameSegment = "/" + NameSpace + entityType.Name;
            }

            Dictionary<string, object> expectedEntryObject = new Dictionary<string, object>();
            expectedEntryObject.Add(JsonLightConstants.ODataTypeAnnotationName, "#" + NameSpace + entityType.Name);
            expectedEntryObject.Add(JsonLightConstants.ODataIdAnnotationName, entry.Id == null ? relativeEditLink : entry.Id.OriginalString);
            expectedEntryObject.Add(JsonLightConstants.ODataEditLinkAnnotationName, entry.EditLink == null ? relativeEditLink + derivedTypeNameSegment : entry.EditLink.AbsoluteUri);

            // when the writer has IEdmModel, expect other metadata in addition to id/edit/readlink 
            if (hasModel)
            {
                // add expected actions
                var boundedActions = _model.FindDeclaredBoundOperations(entityType);
                foreach (var action in boundedActions)
                {
                    var actionFullName = action.FullName();
                    var bindingTypeName = isDerivedType ? "/" + action.Parameters.First().Type.FullName() : "";
                    Dictionary<string, object> actionObject = new Dictionary<string, object>
                        {
                            {"title", actionFullName},
                            {"target", _baseUri.AbsoluteUri + relativeEditLink + bindingTypeName + "/" + actionFullName},
                        };
                    expectedEntryObject.Add("#" + actionFullName, actionObject);
                }

                var baseTypeToAddItem = entityType;

                while (baseTypeToAddItem != null)
                {
                    // add expected navigation properties
                    foreach (var navigation in baseTypeToAddItem.DeclaredNavigationProperties())
                    {
                        var navigationLinkKey = navigation.Name + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName;

                        if (!expectedEntryObject.ContainsKey(navigationLinkKey))
                        {
                            expectedEntryObject.Add(navigationLinkKey, string.Concat(_baseUri.AbsoluteUri, relativeEditLink, derivedTypeNameSegment, "/", navigation.Name));
                        }
                    }

                    baseTypeToAddItem = baseTypeToAddItem.BaseEntityType();
                }
            }

            return expectedEntryObject;
        }

        /// <summary>
        /// Construct expected MLE/named stream metadata
        /// </summary>
        /// <param name="entityType">The IEdmEntityType</param>
        /// <param name="relativeEditLink">The relative edit link</param>
        /// <param name="entry">The ODataEntry</param>
        /// <param name="expectedEntryObject">The expected Json object</param>
        /// <param name="hasStream">Whether the entity type has MLE stream</param>
        /// <param name="hasModel">Whether IEdmModel is provided to writer</param>
        internal void ComputeDefaultExpectedFullMetadataEntryMedia(IEdmEntityType entityType, string relativeEditLink, ODataResource entry, Dictionary<string, object> expectedEntryObject, bool hasStream, bool hasModel)
        {
            if (hasStream)
            {
                expectedEntryObject.Add(JsonLightConstants.ODataMediaEditLinkAnnotationName, relativeEditLink + "/$value");
            }

            if (hasModel)
            {
                foreach (var property in entityType.DeclaredProperties.Where(dp => string.Equals(dp.Type.FullName(), "Edm.Stream", StringComparison.Ordinal)))
                {
                    expectedEntryObject.Add(property.Name + JsonLightConstants.ODataMediaEditLinkAnnotationName, _baseUri.AbsoluteUri + relativeEditLink + "/" + property.Name);
                    expectedEntryObject.Add(property.Name + JsonLightConstants.ODataMediaReadLinkAnnotationName, _baseUri.AbsoluteUri + relativeEditLink + "/" + property.Name);
                }
            }
        }
    }
}
