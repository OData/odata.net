//---------------------------------------------------------------------
// <copyright file="DistinctAssociationSetService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Util
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;

    public class TestDbContext : DbContext
    {
        public DbSet<ReferenceProduct> ReferenceProducts { get; set; }
        public DbSet<VariableProduct> VariableProducts { get; set; }
        public DbSet<Metadata> Metadatas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // if I remove these lines, the generated metadata is correct
            // on my real scenario I need it to customize the way database is generated, etc.
            modelBuilder.Entity<VariableProduct>();
            modelBuilder.Entity<ReferenceProduct>();
            modelBuilder.Entity<ProductBase>();
            modelBuilder.Entity<Metadata>();
        }
    }

    public abstract class ProductBase
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Int32 ProductId { get; set; }
    }

    public class ReferenceProduct : ProductBase
    {
        public ICollection<Metadata> Metadatas { get; set; }
    }

    public class VariableProduct : ProductBase
    {
        public ICollection<Metadata> Metadatas { get; set; }
    }

    public class Metadata
    {
        public Int32 MetadataId { get; set; }
    }


    public class DistinctAssociationSetService : DataService<TestDbContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }
    }
}
