//---------------------------------------------------------------------
// <copyright file="ODataWriterFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using System.Linq;
    using global::Xunit;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Xunit.Performance;

    public class ODataWriterFeedTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly IEdmEntitySet TestEntitySet = Model.EntityContainer.FindEntitySet("Product");
        private const int MaxStreamSize = 220000000;

        private static readonly Stream WriteStream = new MemoryStream(MaxStreamSize);

        [Benchmark]
        public void WriteBasicEntry_1000()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: false, excludeSpatial: false, entryCount: 1000, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryExcludeSpatial_1000()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: false, excludeSpatial: false, entryCount: 1000, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryWithExpansions_100()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: true, excludeSpatial: false, entryCount: 100, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryWithExpansionsExcludeSpatial_100()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: true, excludeSpatial: true, entryCount: 100, isFullValidation: true);
        }

        [Benchmark]
        public void WriteBasicEntryNoValidation_1000()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: false, excludeSpatial: false, entryCount: 1000, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryExcludeSpatialNoValidation_1000()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: false, excludeSpatial: false, entryCount: 1000, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryWithExpansionsNoValidation_100()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: true, excludeSpatial: false, entryCount: 100, isFullValidation: true);
        }

        [Benchmark]
        public void WriteEntryWithExpansionsExcludeSpatialNoValidation_100()
        {
            WriteFeedTestAndMeasure(expandNavigationLinks: true, excludeSpatial: true, entryCount: 100, isFullValidation: true);
        }

        private void WriteFeedTestAndMeasure(bool expandNavigationLinks, bool excludeSpatial, int entryCount, bool isFullValidation)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Reuse the same stream
                WriteStream.Seek(0, SeekOrigin.Begin);

                using (iteration.StartMeasurement())
                {
                    using (var messageWriter = ODataMessageHelper.CreateMessageWriter(WriteStream, Model, ODataMessageKind.Request, isFullValidation))
                    {
                        WriterTestMetaProperties(messageWriter, expandNavigationLinks, excludeSpatial, entryCount);
                    }
                }
            }
        }

        private void WriterTestMetaProperties(ODataMessageWriter messageWriter, bool expandNavigationLinks, bool excludeSpatial, int entryCount)
        {
            #region ODatalib Writer code
            var writer = messageWriter.CreateODataFeedWriter(TestEntitySet);
            writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product") });

            for (int i0 = 0; i0 < entryCount; ++i0)
            {
                var entry1 = excludeSpatial ? EntryWithoutSpatialData() : EntryWithSpatialData();
                writer.WriteStart(entry1);

                #region Nav Prop Group
                if (expandNavigationLinks)
                {
                    #region Nav Prop 1
                    writer.WriteStart(new ODataNavigationLink { Name = "BillOfMaterials", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/BillOfMaterials") });
                    for (int i3 = 0; i3 < 10; ++i3)
                    {
                        #region entry 4
                        var entry4 = new ODataEntry
                        {
                            Properties = new[]
                            {
                                new ODataProperty {Name = "BillOfMaterialsID", Value = 5}, 
                                new ODataProperty {Name = "ProductAssemblyID", Value = -1}, 
                                new ODataProperty {Name = "ComponentID", Value = 1}, 
                                new ODataProperty {Name = "StartDate", Value = DateTimeOffset.Now}, 
                                new ODataProperty {Name = "EndDate", Value = null}, 
                                new ODataProperty {Name = "UnitMeasureCode", Value = "abc"}, 
                                new ODataProperty {Name = "BOMLevel", Value = (short)0}, 
                                new ODataProperty {Name = "PerAssemblyQty", Value = Decimal.MinusOne}, 
                                new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                            }
                        };
                        #endregion

                        writer.WriteStart(entry4);
                        writer.WriteEnd(); // entry4
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 2
                    writer.WriteStart(new ODataNavigationLink { Name = "BillOfMaterials1", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/BillOfMaterials1") });
                    for (int i6 = 0; i6 < 10; ++i6)
                    {
                        var entry7 = new ODataEntry
                        {
                            Properties = new[]
                            {
                                new ODataProperty {Name = "BillOfMaterialsID", Value = 8}, 
                                new ODataProperty {Name = "ProductAssemblyID", Value = 1}, 
                                new ODataProperty {Name = "ComponentID", Value = -1},
                                new ODataProperty {Name = "StartDate", Value = DateTimeOffset.MaxValue}, 
                                new ODataProperty {Name = "EndDate", Value = DateTimeOffset.Now}, 
                                new ODataProperty {Name = "UnitMeasureCode", Value = "abc"}, 
                                new ODataProperty {Name = "BOMLevel", Value = Int16.MaxValue}, 
                                new ODataProperty {Name = "PerAssemblyQty", Value = Decimal.MinusOne}, 
                                new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MaxValue},
                            }
                        };
                        writer.WriteStart(entry7);
                        writer.WriteEnd(); // entry7
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 3
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductModel", IsCollection = false, });
                    var entry9 = new ODataEntry
                    {
                        Properties = new[]
                        {
                            new ODataProperty {Name = "ProductModelID", Value = 10}, 
                            new ODataProperty {Name = "Name", Value = "abc123"}, 
                            new ODataProperty {Name = "CatalogDescription", Value = "abc123"}, 
                            new ODataProperty {Name = "Instructions", Value = "abc123"},
                            new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()},
                            new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                        }
                    };
                    writer.WriteStart(entry9);
                    writer.WriteEnd(); // entry9
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 4
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductSubcategory", IsCollection = false, });
                    var entry11 = new ODataEntry
                    {
                        Properties = new[]
                        {
                            new ODataProperty {Name = "ProductSubcategoryID", Value = 12}, 
                            new ODataProperty {Name = "ProductCategoryID", Value = 1}, 
                            new ODataProperty {Name = "Name", Value = string.Empty}, 
                            new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()}, 
                            new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                        }
                    };
                    writer.WriteStart(entry11);
                    writer.WriteEnd(); // entry11
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 5
                    writer.WriteStart(new ODataNavigationLink { Name = "UnitMeasure", IsCollection = false, });
                    var entry13 = new ODataEntry
                    {
                        Properties = new[]
                        {
                            new ODataProperty {Name = "UnitMeasureCode", Value = "abcdef12345"}, 
                            new ODataProperty {Name = "Name", Value = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"}, 
                            new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                        }
                    };
                    writer.WriteStart(entry13);
                    writer.WriteEnd(); // entry13
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 6
                    writer.WriteStart(new ODataNavigationLink { Name = "UnitMeasure1", IsCollection = false, });
                    var entry14 = new ODataEntry
                    {
                        Properties = new[]
                        {
                            new ODataProperty {Name = "UnitMeasureCode", Value = "abcdef12345"}, 
                            new ODataProperty {Name = "Name", Value = "abc123"}, 
                            new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MaxValue},
                        }
                    };
                    writer.WriteStart(entry14);
                    writer.WriteEnd(); // entry14
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 7
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductCostHistory", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductCostHistory") });
                    for (int i15 = 0; i15 < 10; ++i15)
                    {
                        var entry16 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ProductID", Value = 17},
                                    new ODataProperty {Name = "StartDate", Value = DateTimeOffset.Parse("2012-06-08T16:20:53.2388635-07:00")}, 
                                    new ODataProperty {Name = "EndDate", Value = DateTimeOffset.Now}, 
                                    new ODataProperty {Name = "StandardCost", Value = Decimal.Zero}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MaxValue},
                                }
                        };
                        writer.WriteStart(entry16);
                        writer.WriteEnd(); // entry16
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 8
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductDocument", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductDocument") });
                    for (int i18 = 0; i18 < 10; ++i18)
                    {
                        var entry19 = new ODataEntry
                        {
                            Properties = new[]
                            {
                                new ODataProperty {Name = "ProductID", Value = 20}, 
                                new ODataProperty {Name = "DocumentID", Value = 21}, 
                                new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                            }
                        };
                        writer.WriteStart(entry19);
                        writer.WriteEnd(); // entry19
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 9
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductInventory", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductInventory") });
                    for (int i22 = 0; i22 < 10; ++i22)
                    {
                        var entry23 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ProductID", Value = 24}, 
                                    new ODataProperty {Name = "LocationID", Value = (short)25}, 
                                    new ODataProperty {Name = "Shelf", Value = "abc123"}, 
                                    new ODataProperty {Name = "Bin", Value = Byte.MinValue}, 
                                    new ODataProperty {Name = "Quantity", Value = (short)0}, 
                                    new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                                }
                        };
                        writer.WriteStart(entry23);
                        writer.WriteEnd(); // entry23
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 10
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductListPriceHistory", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductListPriceHistory") });
                    for (int i26 = 0; i26 < 10; ++i26)
                    {
                        var entry27 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ProductID", Value = 28}, 
                                    new ODataProperty {Name = "StartDate", Value = DateTimeOffset.Parse("2012-06-08T16:20:53.2544949-07:00")}, 
                                    new ODataProperty {Name = "EndDate", Value = DateTimeOffset.MaxValue}, 
                                    new ODataProperty {Name = "ListPrice", Value = Decimal.Zero}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                                }
                        };
                        writer.WriteStart(entry27);
                        writer.WriteEnd(); // entry27
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 11
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductProductPhoto", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductProductPhoto") });
                    for (int i29 = 0; i29 < 10; ++i29)
                    {
                        var entry30 = new ODataEntry
                        {
                            Properties = new[]
                            {
                                new ODataProperty {Name = "ProductID", Value = 31}, 
                                new ODataProperty {Name = "ProductPhotoID", Value = 32}, 
                                new ODataProperty {Name = "Primary", Value = false}, 
                                new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                            }
                        };
                        writer.WriteStart(entry30);
                        writer.WriteEnd(); // entry30
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 12
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductReview", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductReview") });
                    for (int i33 = 0; i33 < 10; ++i33)
                    {
                        var entry34 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ProductReviewID", Value = 35}, 
                                    new ODataProperty {Name = "ProductID", Value = Int32.MaxValue}, 
                                    new ODataProperty {Name = "ReviewerName", Value = string.Empty}, 
                                    new ODataProperty {Name = "ReviewDate", Value = DateTimeOffset.MinValue},
                                    new ODataProperty {Name = "EmailAddress", Value = string.Empty}, 
                                    new ODataProperty {Name = "Rating", Value = 0}, 
                                    new ODataProperty {Name = "Comments", Value = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                                }
                        };
                        writer.WriteStart(entry34);
                        writer.WriteEnd(); // entry34
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 13
                    writer.WriteStart(new ODataNavigationLink { Name = "ProductVendor", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ProductVendor") });
                    for (int i36 = 0; i36 < 10; ++i36)
                    {
                        var entry37 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ProductID", Value = 38}, 
                                    new ODataProperty {Name = "VendorID", Value = 39}, 
                                    new ODataProperty {Name = "AverageLeadTime", Value = Int32.MinValue}, 
                                    new ODataProperty {Name = "StandardPrice", Value = Decimal.MinValue}, 
                                    new ODataProperty {Name = "LastReceiptCost", Value = Decimal.MinValue}, 
                                    new ODataProperty {Name = "LastReceiptDate", Value = DateTimeOffset.MaxValue}, 
                                    new ODataProperty {Name = "MinOrderQty", Value = 0}, 
                                    new ODataProperty {Name = "MaxOrderQty", Value = 1},
                                    new ODataProperty {Name = "OnOrderQty", Value = 1}, 
                                    new ODataProperty {Name = "UnitMeasureCode", Value = string.Empty},
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                                }
                        };
                        writer.WriteStart(entry37);
                        writer.WriteEnd(); // entry37
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 14
                    writer.WriteStart(new ODataNavigationLink { Name = "PurchaseOrderDetail", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/PurchaseOrderDetail") });
                    for (int i40 = 0; i40 < 10; ++i40)
                    {
                        var entry41 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "PurchaseOrderID", Value = 42},
                                    new ODataProperty {Name = "PurchaseOrderDetailID", Value = 43},
                                    new ODataProperty {Name = "DueDate", Value = DateTimeOffset.Now},
                                    new ODataProperty {Name = "OrderQty", Value = (short)0}, 
                                    new ODataProperty {Name = "ProductID", Value = Int32.MaxValue}, 
                                    new ODataProperty {Name = "UnitPrice", Value = Decimal.Zero},
                                    new ODataProperty {Name = "LineTotal", Value = Decimal.MinValue},
                                    new ODataProperty {Name = "ReceivedQty", Value = 123.456m}, 
                                    new ODataProperty {Name = "RejectedQty", Value = 123.456m}, 
                                    new ODataProperty {Name = "StockedQty", Value = Decimal.MaxValue}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                                }
                        };
                        writer.WriteStart(entry41);
                        writer.WriteEnd(); // entry41
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 15
                    writer.WriteStart(new ODataNavigationLink { Name = "ShoppingCartItem", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/ShoppingCartItem") });
                    for (int i44 = 0; i44 < 10; ++i44)
                    {
                        var entry45 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "ShoppingCartItemID", Value = 46},
                                    new ODataProperty {Name = "ShoppingCartID", Value = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"}, 
                                    new ODataProperty {Name = "Quantity", Value = -1}, 
                                    new ODataProperty {Name = "ProductID", Value = 0}, 
                                    new ODataProperty {Name = "DateCreated", Value = DateTimeOffset.Now}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                                }
                        };
                        writer.WriteStart(entry45);
                        writer.WriteEnd(); // entry45
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion Nav Prop 16

                    #region Nav Prop 16
                    writer.WriteStart(new ODataNavigationLink { Name = "SpecialOfferProduct", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/SpecialOfferProduct") });
                    for (int i47 = 0; i47 < 10; ++i47)
                    {
                        var entry48 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "SpecialOfferID", Value = 49},
                                    new ODataProperty {Name = "ProductID", Value = 50}, 
                                    new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()},
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                                }
                        };
                        writer.WriteStart(entry48);
                        writer.WriteEnd(); // entry48
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 17
                    writer.WriteStart(new ODataNavigationLink { Name = "TransactionHistory", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/TransactionHistory") });
                    for (int i51 = 0; i51 < 10; ++i51)
                    {
                        var entry52 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "TransactionID", Value = 53}, 
                                    new ODataProperty {Name = "ProductID", Value = -1}, 
                                    new ODataProperty {Name = "ReferenceOrderID", Value = Int32.MaxValue}, 
                                    new ODataProperty {Name = "ReferenceOrderLineID", Value = 1}, 
                                    new ODataProperty {Name = "TransactionDate", Value = DateTimeOffset.MinValue},
                                    new ODataProperty {Name = "TransactionType", Value = "a"}, 
                                    new ODataProperty {Name = "Quantity", Value = Int32.MinValue}, 
                                    new ODataProperty {Name = "ActualCost", Value = Decimal.MaxValue}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MinValue},
                                }
                        };
                        writer.WriteStart(entry52);
                        writer.WriteEnd(); // entry52
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion

                    #region Nav Prop 18
                    writer.WriteStart(new ODataNavigationLink { Name = "WorkOrder", IsCollection = true, });
                    writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc/Product(2)/WorkOrder") });
                    for (int i54 = 0; i54 < 10; ++i54)
                    {
                        var entry55 = new ODataEntry
                        {
                            Properties = new[]
                                {
                                    new ODataProperty {Name = "WorkOrderID", Value = 56}, 
                                    new ODataProperty {Name = "ProductID", Value = Int32.MinValue}, 
                                    new ODataProperty {Name = "OrderQty", Value = 1}, 
                                    new ODataProperty {Name = "StockedQty", Value = -1}, 
                                    new ODataProperty {Name = "ScrappedQty", Value = (short)1}, 
                                    new ODataProperty {Name = "StartDate", Value = DateTimeOffset.MaxValue},
                                    new ODataProperty {Name = "EndDate", Value = DateTimeOffset.Now},
                                    new ODataProperty {Name = "DueDate", Value = DateTimeOffset.MinValue}, 
                                    new ODataProperty {Name = "ScrapReasonID", Value = Int16.MaxValue}, 
                                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.Now},
                                }
                        };
                        writer.WriteStart(entry55);
                        writer.WriteEnd(); // entry55
                    }
                    writer.WriteEnd(); // feed
                    writer.WriteEnd(); // nav prop
                    #endregion
                }
                #endregion

                writer.WriteEnd(); // entry1
            }
            writer.WriteEnd(); // feed
            writer.Flush();
            #endregion
        }

        private ODataEntry EntryWithSpatialData()
        {
            var entry = new ODataEntry
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ProductID", Value = 2}, 
                    new ODataProperty {Name = "Name", Value = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"}, 
                    new ODataProperty {Name = "ProductNumber", Value = "abcdefghijklmnopqrstuvwxy"}, 
                    new ODataProperty {Name = "MakeFlag", Value = false}, 
                    new ODataProperty {Name = "FinishedGoodsFlag", Value = false}, 
                    new ODataProperty {Name = "Color", Value = "abcdefghijklmno"}, 
                    new ODataProperty {Name = "SafetyStockLevel", Value = (short)0}, 
                    new ODataProperty {Name = "ReorderPoint", Value = (short)-1}, 
                    new ODataProperty {Name = "StandardCost", Value = Decimal.MaxValue}, 
                    new ODataProperty {Name = "ListPrice", Value = Decimal.Zero},
                    new ODataProperty {Name = "Size", Value = null}, 
                    new ODataProperty {Name = "SizeUnitMeasureCode", Value = string.Empty},
                    new ODataProperty {Name = "WeightUnitMeasureCode", Value = "abc"}, 
                    new ODataProperty {Name = "Weight", Value = Decimal.MaxValue},
                    new ODataProperty {Name = "DaysToManufacture", Value = 0}, 
                    new ODataProperty {Name = "ProductLine", Value = "ab"},
                    new ODataProperty {Name = "Class", Value = "ab"}, 
                    new ODataProperty {Name = "Style", Value = "ab"},
                    new ODataProperty {Name = "ProductSubcategoryID", Value = Int32.MinValue}, 
                    new ODataProperty {Name = "ProductModelID", Value = null}, 
                    new ODataProperty {Name = "SellStartDate", Value = DateTimeOffset.Now}, 
                    new ODataProperty {Name = "SellEndDate", Value = null}, 
                    new ODataProperty {Name = "DiscontinuedDate", Value = DateTimeOffset.Now}, 
                    new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()}, 
                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MaxValue},
                    new ODataProperty {Name = "LuckyNumbers", Value = new ODataCollectionValue
                    {
                        TypeName = "Collection(Edm.Int64)", 
                        Items = Enumerable.Range(0, 10).Select(n => -1L)
                    }}, 
                    new ODataProperty {Name = "TimeZones", Value = new ODataCollectionValue
                    {
                        TypeName = "Collection(PerformanceServices.Edm.AdventureWorks.TimeZone)", 
                        Items = Enumerable.Range(0, 10).Select(n => new ODataComplexValue
                        {
                            TypeName = "PerformanceServices.Edm.AdventureWorks.TimeZone", 
                            Properties = new[]
                            {
                                new ODataProperty 
                                {
                                    Name = "Region", Value = GeographyFactory.Polygon().Ring(33.1, -110).LineTo(1, 2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build()
                                },
                                new ODataProperty
                                {
                                    Name = "Offset", Value = DateTimeOffset.Now
                                }, 
                                new ODataProperty
                                {
                                    Name = "StartTime", Value = TimeSpan.MaxValue
                                },
                            }
                        })
                    }}, 
                    new ODataProperty {Name = "OpenProperty0", Value = Byte.MinValue}, 
                    new ODataProperty {Name = "OpenProperty1", Value = 0L}, 
                    new ODataProperty {Name = "OpenProperty2", Value = GeographyFactory.Point(32, -100).Build()}, 
                    new ODataProperty {Name = "OpenProperty3", Value = new ODataComplexValue
                    {
                        TypeName = "PerformanceServices.Edm.AdventureWorks.AddressSpatial", 
                        Properties = new[]
                        {
                            new ODataProperty {Name = "Coordinates", Value = GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(0.1, 0.1).Build()},
                            new ODataProperty {Name = "StreetCoordinates", Value = GeographyFactory.LineString(32, -100).LineTo(0, 100).LineTo(0.9, -10.3).LineTo(16.85, 35).Build()}, 
                            new ODataProperty {Name = "Region", Value = GeometryFactory.Polygon().Ring(33.1, -110).LineTo(1, 2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build()}, 
                            new ODataProperty {Name = "NeighbouringRegions", Value = GeographyFactory.MultiPolygon().Polygon().Ring(33.1, -110).LineTo(35.97, -110.15).LineTo(11.45, 87.75).LineTo(-1, -0.9).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).LineTo(9.01, 1).Polygon().Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).LineTo(0.9, 100.5).Ring(33.1, -110).LineTo(35.97, -110.15).LineTo(11.45, 87.75).LineTo(88.77, 33.55).Build()},
                        }
                    }}, 
                    new ODataProperty {Name = "OpenProperty4", Value = GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(0.1, 0.1).Build()},
                }
            };

            return entry;
        }

        private ODataEntry EntryWithoutSpatialData()
        {
            var entry = new ODataEntry
            {
                Properties = new[]
                {
                    new ODataProperty {Name = "ProductID", Value = 2}, 
                    new ODataProperty {Name = "Name", Value = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"}, 
                    new ODataProperty {Name = "ProductNumber", Value = "abcdefghijklmnopqrstuvwxy"}, 
                    new ODataProperty {Name = "MakeFlag", Value = false}, 
                    new ODataProperty {Name = "FinishedGoodsFlag", Value = false}, 
                    new ODataProperty {Name = "Color", Value = "abcdefghijklmno"}, 
                    new ODataProperty {Name = "SafetyStockLevel", Value = (short)0}, 
                    new ODataProperty {Name = "ReorderPoint", Value = (short)-1}, 
                    new ODataProperty {Name = "StandardCost", Value = Decimal.MaxValue}, 
                    new ODataProperty {Name = "ListPrice", Value = Decimal.Zero},
                    new ODataProperty {Name = "Size", Value = null}, 
                    new ODataProperty {Name = "SizeUnitMeasureCode", Value = string.Empty},
                    new ODataProperty {Name = "WeightUnitMeasureCode", Value = "abc"}, 
                    new ODataProperty {Name = "Weight", Value = Decimal.MaxValue},
                    new ODataProperty {Name = "DaysToManufacture", Value = 0}, 
                    new ODataProperty {Name = "ProductLine", Value = "ab"},
                    new ODataProperty {Name = "Class", Value = "ab"}, 
                    new ODataProperty {Name = "Style", Value = "ab"},
                    new ODataProperty {Name = "ProductSubcategoryID", Value = Int32.MinValue}, 
                    new ODataProperty {Name = "ProductModelID", Value = null}, 
                    new ODataProperty {Name = "SellStartDate", Value = DateTimeOffset.Now}, 
                    new ODataProperty {Name = "SellEndDate", Value = null}, 
                    new ODataProperty {Name = "DiscontinuedDate", Value = DateTimeOffset.Now}, 
                    new ODataProperty {Name = "rowguid", Value = Guid.NewGuid()}, 
                    new ODataProperty {Name = "ModifiedDate", Value = DateTimeOffset.MaxValue},
                    new ODataProperty {Name = "LuckyNumbers", Value = new ODataCollectionValue
                    {
                        TypeName = "Collection(Edm.Int64)", 
                        Items = Enumerable.Range(0, 10).Select(n => -1L)
                    }}, 
                    new ODataProperty {Name = "TimeZones", Value = new ODataCollectionValue
                    {
                        TypeName = "Collection(PerformanceServices.Edm.AdventureWorks.TimeZone)", 
                        Items = Enumerable.Range(0, 10).Select(n => new ODataComplexValue
                        {
                            TypeName = "PerformanceServices.Edm.AdventureWorks.TimeZone", 
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "Offset", Value = DateTimeOffset.Now
                                }, 
                                new ODataProperty
                                {
                                    Name = "StartTime", Value = TimeSpan.MaxValue
                                },
                            }
                        })
                    }}, 
                    new ODataProperty {Name = "OpenProperty0", Value = Byte.MinValue}, 
                    new ODataProperty {Name = "OpenProperty1", Value = 0L}, 
                    new ODataProperty {Name = "OpenProperty2", Value = GeographyFactory.Point(32, -100).Build()}, 
                    new ODataProperty {Name = "OpenProperty4", Value = GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(0.1, 0.1).Build()},
                }
            };

            return entry;
        }
    }
}
