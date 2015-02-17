//---------------------------------------------------------------------
// <copyright file="NetflixModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// Model aimed at covering interesting shapes specific to any/all query scenarios 
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "Netflix", HelpText = "Default model for any/all")]
    public class NetflixModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        public EntityModelSchema GenerateModel()
        {
            var model = new EntityModelSchema()
            {
                new EntityType("Movie")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),
                    new MemberProperty("ReleaseYear", DataTypes.DateTime),
                    new MemberProperty("Trailer", DataTypes.Stream),
                    new MemberProperty("FullMovie", DataTypes.Stream),
                    new NavigationProperty("Director", "Movie_Director", "Movie", "Director"),
                    new NavigationProperty("Actors", "Movie_Actors", "Movie", "Actor"),
                    new NavigationProperty("Awards", "Movie_Awards", "Movie", "Award"),
                    new HasStreamAnnotation(),
                },
                new EntityType("Person")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("FirstName", DataTypes.String),
                    new MemberProperty("SurName", DataTypes.String),
                    new MemberProperty("DateOfBirth", DataTypes.DateTime),
                    new MemberProperty("EmailBag", DataTypes.CollectionType.WithElementDataType(DataTypes.String.WithMaxLength(32))).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("HomePhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("WorkPhone", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("MobilePhoneBag", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("Phone"))),
                    new NavigationProperty("DirectedMovies", "Person_Movies", "Person", "Movie"),
                    new NavigationProperty("Awards", "Person_Awards", "Person", "Award"),
                },
                 new EntityType("Award")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),
                    new MemberProperty("AwardsDate", DataTypes.DateTime),
                    new MemberProperty("AwardedBy", DataTypes.String),
                    new MemberProperty("AwardKindBag", DataTypes.CollectionType.WithElementDataType(DataTypes.ComplexType.WithName("AwardKind"))),
                    new NavigationProperty("Movie", "Movie_Awards", "Award", "Movie"),
                    new NavigationProperty("Recipient", "Person_Awards", "Award", "Person")
                },
                 new EntityType("Actor")
                {
                    BaseType = "Person",
                },
                new EntityType("MegaStar")
                {
                    BaseType = "Actor",
                },
                new EntityType("AllBagTypes")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("BagOfPrimitiveToLinks", DataTypes.CollectionType.WithElementDataType(DataTypes.String)).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("BagOfDecimals", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Decimal())),
                    new MemberProperty("BagOfDoubles", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Double)),
                    new MemberProperty("BagOfSingles", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Single)),
                    new MemberProperty("BagOfBytes", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Byte)),
                    new MemberProperty("BagOfInt16s", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int16)),
                    new MemberProperty("BagOfInt32s", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32)),
                    new MemberProperty("BagOfDates", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.DateTime())),
                    new MemberProperty("BagOfGuids", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Guid)),
                },
                 new ComplexType("Phone")
                {
                    new MemberProperty("PhoneNumber", DataTypes.String.WithMaxLength(16)),
                    new MemberProperty("Extension", DataTypes.String.WithMaxLength(16).Nullable(true)),
                },
                  new ComplexType("AwardKind")
                {
                    new MemberProperty("Name", DataTypes.String.WithMaxLength(16)),
                    new MemberProperty("Type", DataTypes.String.WithMaxLength(16).Nullable(true)),
                },
                  new AssociationType("Movie_Director")
                {
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.Many),
                    new AssociationEnd("Director", "Person", EndMultiplicity.One),
                },
                 new AssociationType("Movie_Actors")
                {
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.Many),
                    new AssociationEnd("Actor", "Actor", EndMultiplicity.Many),
                },
                 new AssociationType("Movie_Awards")
                {
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Award", "Award", EndMultiplicity.Many),
                },
                 new AssociationType("Person_Awards")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.One),
                    new AssociationEnd("Award", "Award", EndMultiplicity.Many),
                },
                new AssociationType("Person_Movies")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.One),
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.Many),
                },
            };

            // Add a service operation for each base entity type
            new AddRootServiceOperationsFixup().Fixup(model);

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("Netflix").Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);

            return model;
        }
    }
}
