//---------------------------------------------------------------------
// <copyright file="CamelCaseDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Edm;
    using microsoft.odata.sampleService.models.camelcase;

    public class CamelCaseDataSource : ODataReflectionDataSource
    {
        public EntityCollection<order> orders { get; private set; }
        public EntityCollection<billingRecord> billingRecords { get; private set; }
        public store myStore { get; private set; }

        public override void Reset()
        {
            this.orders = new EntityCollection<order>();
            this.billingRecords = new EntityCollection<billingRecord>();
        }

        public override void Initialize()
        {
            this.myStore = new store()
            {
                id = 1,
                name = "Microsoft Store",
                address = "One Microsoft Way",
            };

            this.orders.AddRange(new List<order>()
            {
                new order
                {
                    id = 1,
                    name = "PC0001",
                    description = "PC0001 - 2014/3/24",
                    categories = category.computer,
                    detail = new orderDetail
                    {
                        amount = 599.99m,
                        quantity = 1,
                    },
                    lineItems = new Collection<string>
                    {
                        "Surface 2",
                        "Surface RT"
                    }                    
                },
                new order
                {
                    id = 2,
                    name = "MIX0001",
                    description = "MIX0001 - 2014/02/16",
                    categories = category.toy | category.cloth,
                    detail = new orderDetail
                    {
                        amount = 339.99m,
                        quantity = 2,
                    },
                    lineItems = new Collection<string>
                    {
                        "XBOX 360",
                        "Microsoft T-shirt"
                    }  
                },
            });

            this.billingRecords.AddRange(new List<billingRecord>()
            {
                new billingRecord()
                {
                    id = 1,
                    name = "BR001",
                    amount = 599.99m,
                    type = billingType.fund,
                },
                new billingRecord()
                {
                    id = 2,
                    name = "BR002",
                    amount = 199.99m,
                    type = billingType.refund,
                },
                new billingRecord()
                {
                    id = 3,
                    name = "BR003",
                    amount = 399.99m,
                    type = billingType.fund,
                }
            });

            this.orders[0].billingRecords.Add(billingRecords[0]);
            this.orders[0].billingRecords.Add(billingRecords[1]);
            this.orders[1].billingRecords.Add(billingRecords[2]);

        }

        protected override IEdmModel CreateModel()
        {
            return CamelCaseInMemoryModel.CreateModel("microsoft.odata.sampleService.models.camelcase");
        }
    }
}