//---------------------------------------------------------------------
// <copyright file="SpatialModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Model aimed at covering interesting shapes specific to spatial types
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "Spatial", HelpText = "Default model for spatial")]
    public class SpatialModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Initializes a new instance of the SpatialModelGenerator class.
        /// </summary>
        public SpatialModelGenerator()
        {
            this.RunStability = RunStability.Stable;
        }

        /// <summary>
        /// Gets or sets the RunStability
        /// </summary>
        [InjectTestParameter("RunStability", DefaultValueDescription = "Stable", HelpText = "Parameter to specify RunStability, Stable or Unstable test")]
        public RunStability RunStability { get; set; }

        /// <summary>
        /// This adds service operations that take arguments of type Spatial to the given model.
        /// </summary>
        /// <param name="model">The model to which the service operations will be added</param>
        /// <param name="runStability">The stability of the tests where this model is being used. Operations will only be added to unstable runs to prevent failures in BVTs</param>
        public static void AddServiceOperationsWithSpatialArguments(EntityModelSchema model, RunStability runStability)
        {
            // The run stability is a parameter to allow unit tests to call this, for code coverage
            if (runStability == RunStability.Unstable)
            {
                // TODO: Remove check for test stability when product supports Service Operations and Properties of Derived Types
                new AddRootServiceOperationsFixup().Fixup(model);

                // TODO: Remove check for test stability when product supports Service Operations with Spatial Arguments
                model.Add(CreateSimpleServiceOperation("Function00", EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function01", EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function02", EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function03", EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function04", EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function05", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function06", EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function07", EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function08", EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function09", EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function10", EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function11", EdmDataTypes.GeometryMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function12", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
                model.Add(CreateSimpleServiceOperation("Function13", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid), EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)));
            }
        }

        /// <summary>
        /// Creates a Function that takes one or more arguments, and returns the first of its arguments
        /// </summary>
        /// <param name="name">
        /// The name of the function 
        /// </param>
        /// <param name="firstArgumentType">The type of the first argument for the function</param>
        /// <param name="otherArgumentsTypes">The types of the rest of the arguments for the function</param>
        /// <returns> A Function that returns the first of its parameters.</returns>
        public static Function CreateSimpleServiceOperation(string name, DataType firstArgumentType, params DataType[] otherArgumentsTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(firstArgumentType, "firstArgumentType");
            ExceptionUtilities.CheckArgumentNotNull(otherArgumentsTypes, "otherArgumentsTypes");

            var function = new Function(name)
            {
                ReturnType = firstArgumentType,
                Parameters = 
                {
                    new FunctionParameter("arg0", firstArgumentType),
                },
                Annotations =
                {
                    new LegacyServiceOperationAnnotation()
                    {
                        Method = HttpVerb.Get
                    },
                    new FunctionBodyAnnotation()
                    {
                        FunctionBody = CommonQueryBuilder.FunctionParameterReference("arg0"),
                    },
                }
            };

            for (int i = 0; i < otherArgumentsTypes.Length; i++)
            {
                function.Parameters.Add(new FunctionParameter("arg" + (i + 1).ToString(CultureInfo.InvariantCulture), otherArgumentsTypes[i]));
            }

            return function;
        }

        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Model declaration")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Model declaration")]
        public EntityModelSchema GenerateModel()
        {
            var model = new EntityModelSchema()
            {
                new EntityType("Store")
                {
                    IsAbstract = true,
                    Properties = 
                    {
                        new MemberProperty("StoreId", DataTypes.Integer) { IsPrimaryKey = true },
                        new MemberProperty("Name", DataTypes.String.Nullable(false)),
                        new MemberProperty("Location", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("ConcurrencyToken", DataTypes.String.Nullable(true).WithMaxLength(4)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    },
                    NavigationProperties =
                    {
                        new NavigationProperty("FavoriteOf", "Person_FavoriteStore", "Store", "Person"),
                    }
                },
                new EntityType("CoffeeShop")
                {
                    BaseType = "Store",
                    Properties =
                    {
                        new MemberProperty("Entrance", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("EmergencyExit", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                    },
                    NavigationProperties = 
                    {
                        new NavigationProperty("Flavors", "CoffeeShop_Flavors", "CoffeeShop", "Flavor"),
                    }
                },
                new EntityType("Pizzeria")
                {
                    BaseType = "Store",
                    Properties = 
                    {
                        new MemberProperty("DeliveryAreas", EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("DeliveryRoutes", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))),
                        new MemberProperty("ReceptionDesk", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("Oven", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    },
                },
                new EntityType("CoffeeFlavor")
                {
                    new MemberProperty("Name", DataTypes.String.Nullable(false)) { IsPrimaryKey = true },
                    new MemberProperty("Description", DataTypes.String.Nullable(true)),
                    new MemberProperty("ConcurrencyToken", DataTypes.String.Nullable(true).WithMaxLength(4)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("Grown", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))),
                    new NavigationProperty("StoresSoldAt", "CoffeeShop_Flavors", "Flavor", "CoffeeShop"),
                    new NavigationProperty("FavoriteOf", "Person_FavoriteFlavor", "Flavor", "Person"),
                },
                new EntityType("Person")
                {
                    new MemberProperty("FirstName", DataTypes.String.Nullable(false)) { IsPrimaryKey = true },
                    new MemberProperty("LastName", DataTypes.String.Nullable(false)) { IsPrimaryKey = true },
                    new MemberProperty("CurrentAddress", DataTypes.ComplexType.WithName("Address")),
                    new MemberProperty("PastAddresses", DataTypes.CollectionOfComplex("Address")),
                    new MemberProperty("RecentLocations", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))),
                    new NavigationProperty("FavoriteStore", "Person_FavoriteStore", "Person", "Store"),
                    new NavigationProperty("FavoriteCoffeeFlavor", "Person_FavoriteFlavor", "Person", "Flavor"),
                    new NavigationProperty("Photos", "Person_Photos", "Person", "Photo"),
                    new NavigationProperty("FavoritePhoto", "Person_FavoritePhoto", "Person", "Photo"),
                },
                new EntityType("Photo")
                {
                    new HasStreamAnnotation(),
                    new MemberProperty("StoreId", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("WhereTaken", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new NavigationProperty("Owner", "Person_Photos", "Photo", "Person"),
                },
                new EntityType("PhotoWithThumbnail")
                {
                    BaseType = "Photo",
                    Properties = 
                    {
                        new MemberProperty("Thumbnail", DataTypes.Stream),
                    },
                },
                new EntityType("HikingTrail")
                {
                    new MemberProperty("Name", DataTypes.String) { IsPrimaryKey = true },
                },
                new EntityType("HikingTrailWithCoordinates")
                {
                    BaseType = "HikingTrail",
                    Properties = 
                    {
                        new MemberProperty("MainPath", EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("AlternatePaths", EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid)),
                        new MemberProperty("TrailHead", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                    },
                },
                new EntityType("AllSpatialTypes")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },

                    new MemberProperty("Geog", EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogPoint", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogLine", EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogPolygon", EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogCollection", EdmDataTypes.GeographyCollection.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiPoint", EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiLine", EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiPolygon", EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid)),

                    new MemberProperty("Geom", EdmDataTypes.Geometry.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomPoint", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomLine", EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomPolygon", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomCollection", EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiPoint", EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiLine", EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiPolygon", EdmDataTypes.GeometryMultiPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    
                    new MemberProperty("Complex", DataTypes.ComplexType.WithName("AllSpatialTypesComplex")),
                },
                new EntityType("AllSpatialCollectionTypes")
                {
                    IsAbstract = true,
                    Properties = 
                    {
                        new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    }
                },
                new EntityType("AllSpatialCollectionTypes_Simple")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                        new MemberProperty("ManyGeogPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeogLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),    
                        new MemberProperty("ManyGeogPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),

                        new MemberProperty("ManyGeomPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                },
                
                 new EntityType("AllSpatialCollectionTypes_Intermediate")
                 {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                        new MemberProperty("ManyGeog", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Geography.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),    
                        new MemberProperty("ManyGeogCollection", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyCollection.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeom", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Geometry.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomCollection", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryCollection.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                 },
                new EntityType("AllSpatialCollectionTypes_MultiPoint")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                        new MemberProperty("ManyGeogMultiPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomMultiPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                },
                new EntityType("AllSpatialCollectionTypes_MultiLine")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                        new MemberProperty("ManyGeogMultiLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomMultiLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                },
                new EntityType("AllSpatialCollectionTypes_MultiPolygon")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                        new MemberProperty("ManyGeogMultiPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                        new MemberProperty("ManyGeomMultiPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                },
                new EntityType("AllSpatialCollectionTypes_Complex1")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                         new MemberProperty("ManyComplex", DataTypes.CollectionOfComplex("AllSpatialTypesComplex")).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    },
                },
                new EntityType("AllSpatialCollectionTypes_Complex2")
                {
                    BaseType = "AllSpatialCollectionTypes",
                    Properties = 
                    {
                         new MemberProperty("ManyCollectionComplex", DataTypes.ComplexType.WithName("AllSpatialCollectionTypesComplex")),
                    },
                },
                new EntityType("ApplicationWindow")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("Origin", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("Border", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("MultiTouchPoints", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))),
                    new NavigationProperty("Controls", "Window_Controls", "Window", "Control"),
                },
                new EntityType("ApplicationControl")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("Origin", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("Border", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new NavigationProperty("Window", "Window_Controls", "Control", "Window"),
                },
                new EntityType("Shape")
                {
                    IsAbstract = true,
                    Properties = 
                    {
                        new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    },
                },
                new EntityType("Point")
                {
                    BaseType = "Shape",
                    Properties = 
                    {
                        new MemberProperty("PointValue", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    }
               },
                new EntityType("Line")
                {
                    BaseType = "Shape",
                    Properties = 
                    {
                        new MemberProperty("LineValue", EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid)),
                    }
                },
                new EntityType("Polygon")
                {
                    BaseType = "Shape",
                    Properties = 
                    {
                        new MemberProperty("Value", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    }
                },
                new ComplexType("Address")
                {
                    new MemberProperty("Location", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                },
                new ComplexType("AllSpatialTypesComplex")
                {
                    new MemberProperty("Geog", EdmDataTypes.Geography.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogPoint", EdmDataTypes.GeographyPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogLine", EdmDataTypes.GeographyLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogPolygon", EdmDataTypes.GeographyPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogCollection", EdmDataTypes.GeographyCollection.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiPoint", EdmDataTypes.GeographyMultiPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiLine", EdmDataTypes.GeographyMultiLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeogMultiPolygon", EdmDataTypes.GeographyMultiPolygon.WithSrid(SpatialConstants.VariableSrid)),

                    new MemberProperty("Geom", EdmDataTypes.Geometry.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomPoint", EdmDataTypes.GeometryPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomLine", EdmDataTypes.GeometryLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomPolygon", EdmDataTypes.GeometryPolygon.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomCollection", EdmDataTypes.GeometryCollection.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiPoint", EdmDataTypes.GeometryMultiPoint.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiLine", EdmDataTypes.GeometryMultiLineString.WithSrid(SpatialConstants.VariableSrid)),
                    new MemberProperty("GeomMultiPolygon", EdmDataTypes.GeometryMultiPolygon.WithSrid(SpatialConstants.VariableSrid)),
                },
                new ComplexType("AllSpatialCollectionTypesComplex")
                {
                    new MemberProperty("ManyGeog", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Geography.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogCollection", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyCollection.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogMultiPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogMultiLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeogMultiPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeographyMultiPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                
                    new MemberProperty("ManyGeom", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Geometry.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomCollection", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryCollection.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomMultiPoint", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiPoint.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomMultiLine", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiLineString.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                    new MemberProperty("ManyGeomMultiPolygon", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.GeometryMultiPolygon.NotNullable().WithSrid(SpatialConstants.VariableSrid))).WithDataGenerationHints(DataGenerationHints.MaxCount(2)),
                },
                new AssociationType("CoffeeShop_Flavors")
                {
                    new AssociationEnd("CoffeeShop", "CoffeeShop", EndMultiplicity.Many),
                    new AssociationEnd("Flavor", "CoffeeFlavor", EndMultiplicity.Many),
                },
                new AssociationType("Person_FavoriteStore")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.Many),
                    new AssociationEnd("Store", "Store", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Person_FavoriteFlavor")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.Many),
                    new AssociationEnd("Flavor", "CoffeeFlavor", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Person_Photos")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Photo", "Photo", EndMultiplicity.Many),
                },
                new AssociationType("Person_FavoritePhoto")
                {
                    new AssociationEnd("Person", "Person", EndMultiplicity.Many),
                    new AssociationEnd("Photo", "Photo", EndMultiplicity.ZeroOne),
                },
                new AssociationType("Window_Controls")
                {
                    new AssociationEnd("Window", "ApplicationWindow", EndMultiplicity.ZeroOne),
                    new AssociationEnd("Control", "ApplicationControl", EndMultiplicity.Many),
                },
                new EntityContainer("SpatialContainer")
                {
                    new DataServiceConfigurationAnnotation()
                    {
                        UseVerboseErrors = true,
                    },
                    new DataServiceBehaviorAnnotation()
                    {
                        AcceptSpatialLiteralsInQuery = true,
                        MaxProtocolVersion = DataServiceProtocolVersion.V4,
                    },
                    new EntitySet("Stores", "Store")
                    {
                        new PageSizeAnnotation() { PageSize = 3 },
                    },
                    new EntitySet("Flavors", "CoffeeFlavor"),
                    new EntitySet("People", "Person"),
                    new EntitySet("Photos", "Photo")
                    {
                        new PageSizeAnnotation() { PageSize = 5 },
                    },
                    new EntitySet("Trails", "HikingTrail"),
                    new EntitySet("AllTypesSet", "AllSpatialTypes")
                    {
                        new PageSizeAnnotation() { PageSize = 2 },
                    },
                    new EntitySet("AllCollectionTypesSet", "AllSpatialCollectionTypes"),
                    new EntitySet("ApplicationWindows", "ApplicationWindow")
                    {
                        new PageSizeAnnotation() { PageSize = 4 },
                    },
                    new EntitySet("ApplicationControls", "ApplicationControl"),
                    new EntitySet("Shapes", "Shape"),
                    new AssociationSet("Stores_Flavors", "CoffeeShop_Flavors")
                    {
                        new AssociationSetEnd("CoffeeShop", "Stores"),
                        new AssociationSetEnd("Flavor", "Flavors"),
                    },
                    new AssociationSet("People_Stores", "Person_FavoriteStore")
                    {
                        new AssociationSetEnd("Person", "People"),
                        new AssociationSetEnd("Store", "Stores"),
                    },
                    new AssociationSet("People_Flavors", "Person_FavoriteFlavor")
                    {
                        new AssociationSetEnd("Person", "People"),
                        new AssociationSetEnd("Flavor", "Flavors"),
                    },
                    new AssociationSet("People_Photos", "Person_Photos")
                    {
                        new AssociationSetEnd("Person", "People"),
                        new AssociationSetEnd("Photo", "Photos"),
                    },
                    new AssociationSet("People_FavoritePhotos", "Person_FavoritePhoto")
                    {
                        new AssociationSetEnd("Person", "People"),
                        new AssociationSetEnd("Photo", "Photos"),
                    },
                    new AssociationSet("Windows_Controls", "Window_Controls")
                    {
                        new AssociationSetEnd("Window", "ApplicationWindows"),
                        new AssociationSetEnd("Control", "ApplicationControls"),
                    },
                }
            };

            AddServiceOperationsWithSpatialArguments(model, this.RunStability);

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("Spatial").Fixup(model);

            return model;
        }
    }
}
