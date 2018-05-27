//---------------------------------------------------------------------
// <copyright file="CamelCaseModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace microsoft.odata.sampleService.models.camelcase
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class namedObject : ClrObject
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class order : namedObject
    {
        private EntityCollection<billingRecord> navVariable;

        public order()
        {
            navVariable = new EntityCollection<billingRecord>(DataSourceManager.GetCurrentDataSource<CamelCaseDataSource>().billingRecords);
        }

        public string description { get; set; }

        public orderDetail detail { get; set; }

        public Collection<string> lineItems { get; set; }

        public category categories { get; set; }

        public EntityCollection<billingRecord> billingRecords
        {
            get
            {
                return this.navVariable.Cleanup();
            }
        }

        public void SetlineItems(ODataCollectionValue values)
        {
            this.lineItems = new Collection<string>();
            foreach (var item in values.Items)
            {
                this.lineItems.Add(item.ToString());
            }
        }
    }

    public class billingRecord : namedObject
    {
        public billingType type { get; set; }

        public decimal amount { get; set; }
    }

    public class store : namedObject
    {
        public string address { get; set; }
    }

    public class orderDetail : ClrObject
    {
        public decimal amount { get; set; }
        public int quantity { get; set; }
    }

    public enum billingType : long
    {
        fund = 0,
        refund = 1,
        chargeback = 2,
    }

    [Flags]
    public enum category
    {
        toy = 1,
        food = 2,
        cloth = 4,
        drink = 8,
        computer = 16,
    }
}