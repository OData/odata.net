//---------------------------------------------------------------------
// <copyright file="StreamsModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// Model aimed at covering interesting shapes specific to streams (both media-resources and named streams)
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "Streams", HelpText = "Default model for streams")]
    public class StreamsModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        //// [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is unavoidable, we need to create entire model here.")]
        //// [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Locals created by the compiler.")]
        public EntityModelSchema GenerateModel()
        {
            var model = new EntityModelSchema()
            {
                new EntityType("Photo")
                {
                    new MemberProperty("PhotoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("OwnerId", DataTypes.Integer.Nullable(true)),
                    new MemberProperty("Width", DataTypes.Integer.Nullable(true)),
                    new MemberProperty("Height", DataTypes.Integer.Nullable(true)),
                    new MemberProperty("ConcurrencyToken", DataTypes.String.Nullable(true)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new NavigationProperty("People", "People_Photos", "Photo", "People"),
                    new HasStreamAnnotation(),
                },
                new EntityType("Customer")
                {
                    new MemberProperty("CustomerId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100)),
                    new MemberProperty("ContactInfo", DataTypes.ComplexType.WithName("ContactDetails")),
                    new MemberProperty("Thumbnail", DataTypes.Stream),
                    new MemberProperty("Video", DataTypes.Stream),
                    new NavigationProperty("Orders", "Customer_Orders", "Customer", "Order"),
                    new NavigationProperty("Husband", "Husband_Wife", "Wife", "Husband"),
                    new NavigationProperty("Wife", "Husband_Wife", "Husband", "Wife"),
                    new NavigationProperty("Info", "Customer_CustomerInfo", "Customer", "Info"),
                },
                 new ComplexType("Phone")
                {
                    new MemberProperty("PhoneNumber", DataTypes.String.WithMaxLength(16)),
                    new MemberProperty("Extension", DataTypes.String.WithMaxLength(16).Nullable(true)),
                },
                new ComplexType("ContactDetails")
                {
                    new MemberProperty("Email", DataTypes.String.WithMaxLength(32)),
                    new MemberProperty("HomePhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("WorkPhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("MobilePhone", DataTypes.ComplexType.WithName("Phone")),
                },
                  new EntityType("Car")
                {
                    new MemberProperty("VIN", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Description", DataTypes.String.Nullable(true)),
                    new MemberProperty("Photo", DataTypes.Stream),
                    new MemberProperty("Video", DataTypes.Stream),
                    new HasStreamAnnotation(),
                },
                  new EntityType("CustomerInfo")
                {
                    new MemberProperty("CustomerInfoId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Information", DataTypes.String.Nullable(true)),
                },
                 new EntityType("Order")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("CustomerId", DataTypes.Integer.Nullable(true)),
                    new NavigationProperty("Customer", "Customer_Orders", "Order", "Customer"),
                    new NavigationProperty("OrderLines", "Order_OrderLines", "Order", "OrderLines"),
                },
                new EntityType("OrderLine")
                {
                    new MemberProperty("OrderId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Quantity", DataTypes.Integer),
                    new MemberProperty("ConcurrencyToken", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("OrderLineStream", DataTypes.Stream),
                    new NavigationProperty("Order", "Order_OrderLines", "OrderLines", "Order"),
                    new NavigationProperty("Product", "Product_OrderLines", "OrderLines", "Product"),
                },
                new EntityType("BackOrderLine")
                {
                    BaseType = "OrderLine"
                },
                new EntityType("Product")
                {
                    new MemberProperty("ProductId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Description", DataTypes.String.Nullable(true).WithUnicodeSupport(true).WithMaxLength(1000)),
                    new MemberProperty("BaseConcurrency", DataTypes.String) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("Picture", DataTypes.Stream),
                },
                new EntityType("People")
                {
                    new MemberProperty("PeopleId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(100).Nullable(true)),
                    new MemberProperty("ConcurrencyToken", DataTypes.String.Nullable(true)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new NavigationProperty("Photos", "People_Photos", "People", "Photo"),
                    new HasStreamAnnotation(),
                },
               new AssociationType("Customer_CustomerInfo")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One),
                    new AssociationEnd("Info", "CustomerInfo", EndMultiplicity.ZeroOne),
                },
                 new AssociationType("Customer_Orders")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Order", "Order", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("Order", "CustomerId")
                        .ReferencesPrincipalProperties("Customer", "CustomerId"),
                },
                  new AssociationType("Order_OrderLines")
                {
                    new AssociationEnd("Order", "Order", EndMultiplicity.One),
                    new AssociationEnd("OrderLines", "OrderLine", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("OrderLines", "OrderId")
                        .ReferencesPrincipalProperties("Order", "OrderId"),
                },
                 new AssociationType("Product_OrderLines")
                {
                    new AssociationEnd("Product", "Product", EndMultiplicity.One),
                    new AssociationEnd("OrderLines", "OrderLine", EndMultiplicity.Many),
                    new ReferentialConstraint()
                        .WithDependentProperties("OrderLines", "ProductId")
                        .ReferencesPrincipalProperties("Product", "ProductId"),
                },
                 new AssociationType("Husband_Wife")
                {
                    new AssociationEnd("Husband", "Customer", EndMultiplicity.ZeroOne) { Annotations = { new PrincipalAnnotation() } },
                    new AssociationEnd("Wife", "Customer", EndMultiplicity.ZeroOne),
                },            
                 new AssociationType("People_Photos")
                {
                    new AssociationEnd("People", "People", EndMultiplicity.Many),
                    new AssociationEnd("Photo", "Photo", EndMultiplicity.Many),
                },
            };

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("Streams").Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);

            return model;
        }
    }
}
