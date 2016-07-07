//---------------------------------------------------------------------
// <copyright file="PluggableFormatServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.PluggableFormat;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// The class implements an in memory data source that provides IQueryables of model types
    /// </summary>
    public class PluggableFormatServiceDataSource : ODataReflectionDataSource
    {
        public PluggableFormatServiceDataSource()
        {
            this.OperationProvider = new PluggableFormatOperationProvider<PluggableFormatServiceDataSource>();
        }

        #region Entity Set Resources

        /// <summary>
        /// Returns an entity set of Person.
        /// </summary>
        public EntityCollection<Person> People
        {
            get;
            private set;
        }

        public EntityCollection<Product> Products
        {
            get;
            private set;
        }
        #endregion

        public override void Reset()
        {
            this.People = new EntityCollection<Person>();
            this.Products = new EntityCollection<Product>();
        }

        /// <summary>
        /// Populates the data source.
        /// </summary>
        public override void Initialize()
        {
            this.People.AddRange(new List<Person>()
            {
                new Person()
                {
                    Id = 31,
                    Picture = new byte[] { 05, 08 },
                    Numbers = new List<int> { 3, 5, 7 },
                    BusinessCard = new ContactInfo()
                    {
                        N = "Name1",
                        //OpenProperties =
                        //{
                        //    {"Tel_Home", "01"},
                        //}
                    }
                },
                new Person()
                {
                    Id = 32,
                    Numbers = new List<int>{},
                    BusinessCard = new ContactInfo()
                    {
                        N = "Name2",
                        //OpenProperties =
                        //{
                        //    {"Tel_Home", "02"},
                        //}
                    }
                },
            });

            this.Products.AddRange(new[]
                {
                    new Product()
                    {
                        Id = 1,
                        Name = "Pear",
                    }, 
                    new Product()
                    {
                        Id = 2,
                        Name = "Banana",
                        Info = new ProductInfo()
                        {
                            Site = "G2",
                            Serial = 0x3FF,
                        }
                    }, 
                    new Product()
                    {
                        Id = 3,
                        Name = "Mangosteen",
                        Info = new ProductInfo()
                        {
                            Site = "G3",
                            Serial = 0x3FE,
                        }
                    }, 
                });
        }

        protected override IEdmModel CreateModel()
        {
            return TestHelper.GetModel("Microsoft.Test.OData.Services.PluggableFormat.Csdl.PluggableFormat.xml");
        }

        protected override void ConfigureContainer(IContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.AddService<ODataMediaTypeResolver, PluggableFormatResolver>(ServiceLifetime.Singleton);
        }
    }
}