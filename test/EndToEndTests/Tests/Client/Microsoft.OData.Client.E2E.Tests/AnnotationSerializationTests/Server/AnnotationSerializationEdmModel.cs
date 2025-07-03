//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server
{
    public class AnnotationSerializationEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Order>("Orders");
            modelBuilder.EnumType<OrderStatus>();
            modelBuilder.ComplexType<Address>();
            modelBuilder.ComplexType<City>();
            modelBuilder.ComplexType<State>();
            modelBuilder.EntityType<VipOrder>();
            modelBuilder.ComplexType<VipAddress>();
            modelBuilder.ComplexType<VipCity>();
            modelBuilder.ComplexType<VipState>();

            return modelBuilder.GetEdmModel();
        }
    }
}
