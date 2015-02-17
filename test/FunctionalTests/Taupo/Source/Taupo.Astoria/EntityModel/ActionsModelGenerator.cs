//---------------------------------------------------------------------
// <copyright file="ActionsModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
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
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Model aimed at covering interesting shapes specific to any/all query scenarios 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is required")]
    [ImplementationName(typeof(IModelGenerator), "Actions", HelpText = "Default model for actions")]
    public class ActionsModelGenerator : IModelGenerator
    {
        private Dictionary<DataType, bool> useDataTypes;
       
        /// <summary>        
        /// Initializes a new instance of the ActionsModelGenerator class
        /// </summary>
        public ActionsModelGenerator()
        {
            this.RemoveHigherVersionModelFeaturesExceptActionWithMultiValue = false;
            this.AdaptModelForCallOrderTests = false;
        }

        /// <summary>
        /// Gets or sets the Data Provider Settings used
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public DataProviderSettings DataProviderSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whetherWill remove all higher version model features and adds only an action with that has a multiValue
        /// </summary>
        [InjectTestParameter("RemoveHigherVersionModelFeaturesExceptActionWithMultiValue", DefaultValueDescription = "false", HelpText = "Indicates whether to remove multivalues from entityTypes and functions")]
        public bool RemoveHigherVersionModelFeaturesExceptActionWithMultiValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whetherWill remove all higher version model features and adds only an action with that has a multiValue
        /// </summary>
        [InjectTestParameter("AdaptModelForCallOrderTests", DefaultValueDescription = "false", HelpText = "Indicates that we are adapting the actions test for call order by removing various aspects of the model to simplify")]
        public bool AdaptModelForCallOrderTests { get; set; }

        /// <summary>
        /// Gets or sets the RandomNumberGenerator 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns> Valid <see cref="EntityModelSchema"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is unavoidable, we need to create entire model here.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is unavoidable, we need to create entire model here.")]
        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Locals created by the compiler.")]
        public EntityModelSchema GenerateModel()
        {
            this.useDataTypes = new Dictionary<DataType, bool>();

            var model = new EntityModelSchema()
            {
                new EntityType("Movie")
                {
                    Annotations = 
                    {
                        new HasStreamAnnotation(),
                    },
                    Properties = 
                    {
                        new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                        new MemberProperty("Name", EdmDataTypes.String()),
                        new MemberProperty("LengthInMinutes", EdmDataTypes.Int32) { Annotations = { new ConcurrencyTokenAnnotation() } },
                        new MemberProperty("ReleaseYear", EdmDataTypes.DateTime()),
                        new MemberProperty("Trailer", EdmDataTypes.Stream),
                        new MemberProperty("FullMovie", EdmDataTypes.Stream),
                        new MemberProperty("IsAwardWinner", EdmDataTypes.Boolean),
                        new MemberProperty("AddToQueueValue", EdmDataTypes.Boolean),
                        new MemberProperty("AddToQueueValue2", EdmDataTypes.Boolean),
                        new MemberProperty("MovieHomePage", EdmDataTypes.String()).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(10)),  
                        new MemberProperty("Description", EdmDataTypes.String()).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.AnsiString, DataGenerationHints.MinLength(10)),
                    },

                    NavigationProperties = 
                    {
                        new NavigationProperty("MovieRatings", "MovieRating_Movie", "Movie", "MovieRating"),
                        new NavigationProperty("Actors", "Movies_Actors", "Movies", "Actors"),
                    }
                },
                new ComplexType("Phone")
                {
                    new MemberProperty("PhoneNumber", EdmDataTypes.String()),
                    new MemberProperty("Extension", EdmDataTypes.String().Nullable(true)),
                },
                new ComplexType("ContactDetails")
                {
                    new MemberProperty("PhoneMultiValue", DataTypes.CollectionOfComplex("Phone")),
                },
                new ComplexType("Rating")
                {
                    new MemberProperty("Comments", EdmDataTypes.String()),
                    new MemberProperty("FiveStarRating", EdmDataTypes.Byte),
                    new MemberProperty("Tags", DataTypes.CollectionType.WithElementDataType(DataTypes.String)),
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
                new EntityType("MovieRating")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("Rating", DataTypes.ComplexType.WithDefinition("Rating")),
                    new MemberProperty("IsCreatedByCustomer", EdmDataTypes.Boolean),
                    new NavigationProperty("Movie", "MovieRating_Movie", "MovieRating", "Movie"),
                },
                new EntityType("Actor")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("FirstName", EdmDataTypes.String()),
                    new MemberProperty("LastName", EdmDataTypes.String()),
                    new MemberProperty("Age", EdmDataTypes.Int32),
                    new MemberProperty("IsAwardWinner", EdmDataTypes.Boolean),
                    new MemberProperty("ContactDetails", DataTypes.ComplexType.WithName("ContactDetails")),
                    new MemberProperty("PrimaryPhoneNumber", DataTypes.ComplexType.WithName("Phone")),
                    new MemberProperty("AdditionalPhoneNumbers", DataTypes.CollectionOfComplex("Phone")),
                    new MemberProperty("AlternativeNames", DataTypes.CollectionType.WithElementDataType(DataTypes.String)),
                    new NavigationProperty("Movies", "Movies_Actors", "Actors", "Movies"),
                },
                new EntityType("Producer")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("FirstName", EdmDataTypes.String()),
                    new MemberProperty("LastName", EdmDataTypes.String()),
                    new MemberProperty("Toggle", EdmDataTypes.Boolean),
                },
                new EntityType("ExecutiveProducer")
                {
                    BaseType = "Producer"
                },
                new EntityType("ActorMovieRating")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("IsStar", EdmDataTypes.Boolean),
                    new MemberProperty("Rating", DataTypes.ComplexType.WithDefinition("Rating")),
                    new NavigationProperty("Actor", "ActorMovieRating_Actor", "ActorMovieRating", "Actor"),
                    new NavigationProperty("Movie", "ActorMovieRating_Movie", "ActorMovieRating", "Movie"),
                },
                new EntityType("AllTypes")
                {
                    // This EntityType contains only primitive properties
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("ToggleProperty", EdmDataTypes.Boolean),
                    new MemberProperty("BooleanProperty", EdmDataTypes.Boolean),
                    new MemberProperty("StringProperty", EdmDataTypes.String()).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("ByteProperty", EdmDataTypes.Byte),
                    new MemberProperty("DateTimeProperty", EdmDataTypes.DateTime()),
                    new MemberProperty("DecimalProperty", EdmDataTypes.Decimal()),
                    new MemberProperty("DoubleProperty", EdmDataTypes.Double),
                    new MemberProperty("GuidProperty", EdmDataTypes.Guid),
                    new MemberProperty("Int16Property", EdmDataTypes.Int16),
                    new MemberProperty("Int32Property", EdmDataTypes.Int32),
                    new MemberProperty("Int64Property", EdmDataTypes.Int64),
                    new MemberProperty("SingleProperty", EdmDataTypes.Single),
                    new MemberProperty("BinaryProperty", EdmDataTypes.Binary()),
                    new MemberProperty("DateTimeOffsetProperty", EdmDataTypes.DateTimeOffset().NotNullable()),
                    new MemberProperty("TimeSpanProperty", EdmDataTypes.Time().NotNullable()),
                    new MemberProperty("NullableDateTimeOffsetProperty", EdmDataTypes.DateTimeOffset().Nullable()),
                    new MemberProperty("NullableTimeSpanProperty", EdmDataTypes.Time().Nullable()),

                    new MemberProperty("ByteCollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Byte)).WithDataGenerationHints(DataGenerationHints.NoNulls, DataGenerationHints.MaxCount(0)),
                    new MemberProperty("DoubleCollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Double)).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("Int32CollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Int32)).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("StringCollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String())).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("DateTimeOffsetCollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.DateTimeOffset())).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("TimeSpanCollectionProperty", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.Time())).WithDataGenerationHints(DataGenerationHints.NoNulls),

                    //// TODO: add the following
                    ////new MemberProperty("NullStringProperty", DataTypes.String).WithDataGenerationHints(DataGenerationHints.AllNulls),
                    ////new MemberProperty("NullBinaryProperty", DataTypes.Binary.WithMaxLength(500)).WithDataGenerationHints(DataGenerationHints.AllNulls),
                },
                new EntityType("AllSpatialTypes")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("ToggleProperty", EdmDataTypes.Boolean),
                    new MemberProperty("Int32Property", EdmDataTypes.Int32),

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
                new EntityType("DVDCustomer")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("Name", EdmDataTypes.String()),
                    new MemberProperty("EMail", EdmDataTypes.String()),
                    new MemberProperty("Visa", EdmDataTypes.Int32),
                    new MemberProperty("BalancePaid", EdmDataTypes.Boolean),
                    new NavigationProperty("DVDShipActivities", "DVDCustomer_DVDShipActivities", "DVDCustomer", "DVDShipActivity"),
                },
                new EntityType("DVDShipActivity")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("OrderDescription", EdmDataTypes.String()),
                    new MemberProperty("Title", EdmDataTypes.String()),
                    new MemberProperty("ShipTime", EdmDataTypes.DateTime()),
                    new MemberProperty("ReturnTime", EdmDataTypes.DateTime()),
                    new MemberProperty("ProblemReport", EdmDataTypes.String()),
                    new NavigationProperty("DVDCustomer", "DVDCustomer_DVDShipActivities", "DVDShipActivity", "DVDCustomer"),
                },
                new AssociationType("DVDCustomer_DVDShipActivities")
                {
                    new AssociationEnd("DVDCustomer", "DVDCustomer", EndMultiplicity.One),
                    new AssociationEnd("DVDShipActivity", "DVDShipActivity", EndMultiplicity.Many),
                },
                new AssociationType("Movies_Actors")
                {
                    new AssociationEnd("Movies", "Movie", EndMultiplicity.Many),
                    new AssociationEnd("Actors", "Actor", EndMultiplicity.Many),
                },
                new AssociationType("ActorMovieRating_Movie")
                {
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.ZeroOne),
                    new AssociationEnd("ActorMovieRating", "ActorMovieRating", EndMultiplicity.Many),
                },
                new AssociationType("ActorMovieRating_Actor")
                {
                    new AssociationEnd("Actor", "Actor", EndMultiplicity.ZeroOne),
                    new AssociationEnd("ActorMovieRating", "ActorMovieRating", EndMultiplicity.Many),
                },
                new AssociationType("MovieRating_Movie")
                {
                    new AssociationEnd("MovieRating", "MovieRating", EndMultiplicity.Many),
                    new AssociationEnd("Movie", "Movie", EndMultiplicity.ZeroOne),
                },
                new Function("CheckoutFirstMovie")
                {
                    new ToggleBoolPropertyValueActionAnnotation() { SourceEntitySet = "Movie", ToggleProperty = "IsAwardWinner" },
                    new ServiceOperationAnnotation() { BindingKind = OperationParameterBindingKind.Never, IsAction = true }
                },
                new Function("PayCustomerBalance")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "BalancePaid", ReturnProperty = "DVDShipActivities" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                             EntitySetPath = "customer/DVDShipActivities",
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("customer", DataTypes.EntityType.WithDefinition("DVDCustomer")),
                    },
                    
                    ReturnType = DataTypes.CollectionOfEntities("DVDShipActivity"),
                },
                new Function("AddToQueue")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "AddToQueueValue", ReturnProperty = "Id" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("movie", DataTypes.EntityType.WithDefinition("Movie")),
                    },
                    
                    ReturnType = EdmDataTypes.Int32
                },
                new Function("UpVoteExecutiveProducer")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "Toggle", ReturnProperty = "FirstName" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                             BindingKind = OperationParameterBindingKind.Sometimes
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("executiveProducer", DataTypes.EntityType.WithDefinition("ExecutiveProducer")),
                    },
                    
                    ReturnType = EdmDataTypes.String()
                },
                new Function("UpVoteProducer")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "Toggle", ReturnProperty = "FirstName" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                             BindingKind = OperationParameterBindingKind.Sometimes
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("producer", DataTypes.EntityType.WithDefinition("Producer")),
                    },
                    
                    ReturnType = EdmDataTypes.String()
                },
                new Function("AddToQueue2")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "AddToQueueValue2", ReturnProperty = "Id" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                             BindingKind = OperationParameterBindingKind.Sometimes
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("movie", DataTypes.EntityType.WithDefinition("Movie")),
                    },

                    ReturnType = EdmDataTypes.Int32
                },
                new Function("AddToQueueThrowError")
                {
                    Annotations =
                    {
                        new ThrowDataServiceExceptionAnnotation() { ErrorStatusCode = 500, ErrorMessage = "Throwing error in AddToQueueThrowError function" },
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "AddToQueueValue", ReturnProperty = "Id" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("movie", DataTypes.EntityType.WithDefinition("Movie")),
                    },

                    ReturnType = EdmDataTypes.Int32
                },

                new Function("MultiParameterFunction")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "AddToQueueValue", ReturnProperty = "Id" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("movie", DataTypes.EntityType.WithDefinition("Movie")),
                        new FunctionParameter("author", EdmDataTypes.String()),
                        new FunctionParameter("comments", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String())),
                    },

                    ReturnType = EdmDataTypes.Int32
                },
                //// Declaring an action in the Actor Entity that will have a name collision with the 'Age' property
                new Function("Age")
                {
                    Annotations =
                    {
                        new ToggleBoolPropertyValueActionAnnotation() { ToggleProperty = "IsAwardWinner", ReturnProperty = "FirstName" },
                        new ServiceOperationAnnotation()
                        {
                             IsAction = true,
                        }
                    },

                    Parameters = 
                    {
                        new FunctionParameter("actor", DataTypes.EntityType.WithDefinition("Actor")),
                    },
                    
                    ReturnType = EdmDataTypes.String()
                }
            };

            this.AddServiceOperationsToModel(model);

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("NetflixActions").Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);

            // add MEST scenarios in the model
            if (this.DataProviderSettings.SupportsMest)
            {
                EntityContainer ec = model.EntityContainers.Single();
                EntityType customerEntityType = ec.EntitySets.Single(es => es.Name == "DVDCustomer").EntityType;
                EntitySet instantWatchCustomerEntitySet = new EntitySet("InstantWatchCustomer", customerEntityType);
                ec.Add(instantWatchCustomerEntitySet);

                EntityType activityEntityType = ec.EntitySets.Single(es => es.Name == "DVDShipActivity").EntityType;
                EntitySet instantWatchActivityEntitySet = new EntitySet("InstantWatchActivity", activityEntityType);
                ec.Add(instantWatchActivityEntitySet);

                AssociationType at = model.Associations.Single(a => a.Name == "DVDCustomer_DVDShipActivities");
                ec.Add(new AssociationSet("InstantWatchCustomer_InstantWatchActivities", at)
                {
                    Ends =
                    {
                        new AssociationSetEnd(at.Ends[0], instantWatchCustomerEntitySet),
                        new AssociationSetEnd(at.Ends[1], instantWatchActivityEntitySet),
                    }
                });
            }

            // Only add these actions if its not using the model for call order tests
            if (!this.AdaptModelForCallOrderTests)
            {
                this.AddEntityTypeDrivenToggleActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "ActorMovieRating"));
                this.AddEntityTypeDrivenToggleActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "Movie"));
                this.AddEntityTypeDrivenToggleActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "Actor"));
                this.AddEntityTypeDrivenToggleActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "AllTypes"));

                if (this.DataProviderSettings.SupportsSpatial)
                {
                    this.AddEntityTypeDrivenToggleActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "AllSpatialTypes"));
                }

                this.AddIncrementIntegerPropertyActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "AllTypes"), "Int32Property");

                if (this.DataProviderSettings.SupportsSpatial)
                {
                    this.AddIncrementIntegerPropertyActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "AllSpatialTypes"), "Int32Property");
                }

                this.AddIncrementIntegerPropertyActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "Actor"), "Age");
                this.AddIncrementIntegerPropertyActions(model, model.EntityContainers.First().EntitySets.Single(es => es.Name == "Movie"), "LengthInMinutes");
            }

            // Add page size = 1 for all exposed entity sets in the model
            var movieEntitySet = model.EntityContainers.First().EntitySets.Single(es => es.Name == "Movie");
            movieEntitySet.Annotations.Add(new PageSizeAnnotation() { PageSize = 1 });

            this.SetupBindingParameterCollectionTypeAnnotation(model);

            if (this.RemoveHigherVersionModelFeaturesExceptActionWithMultiValue)
            {
                new AddDefaultContainerFixup().Fixup(model);
                new SetDefaultDataServiceConfigurationBehaviors() { MaxProtocolVersion = DataServiceProtocolVersion.V4 }.Fixup(model);

                var functionsToSave = model.Functions.Where(f => f.IsAction() && f.ReturnType != null && f.Parameters.Count > 0);
                var functionToSave = functionsToSave.Where(f => f.Parameters.Any(p => p.DataType is CollectionDataType && !(((CollectionDataType)p.DataType).ElementDataType is EntityDataType))).ToArray().FirstOrDefault();
                new RemoveHigherVersionFeaturesFixup(DataServiceProtocolVersion.V4).Fixup(model);

                model.Add(functionToSave);
            }

            // Remove streams from the model if this is being used for call order tests
            if (this.AdaptModelForCallOrderTests)
            {
                new RemoveNamedStreamsFixup().Fixup(model);
                model.EntityTypes.ForEach(et => et.Annotations.RemoveAll(a => a.GetType() == typeof(HasStreamAnnotation)));

                // Add Query Interceptors for all sets so they will be triggered when running via call order
                model.EntityContainers.Single().EntitySets.ForEach(es => es.Annotations.Add(new ConstantInterceptorAnnotation() { FilterConstant = true }));
            }

            return model;
        }

        private static void AddCollectionContractTypeAnnotation(AnnotatedItem item, DataType dataType)
        {
            var collectionDataType = dataType as CollectionDataType;
            if (collectionDataType != null)
            {
                if (collectionDataType.ElementDataType is PrimitiveDataType)
                {
                    item.Annotations.Add(new CollectionContractTypeAnnotation() { FullTypeName = "IEnumerable", IsGeneric = true });
                }
                else
                {
                    item.Annotations.Add(new CollectionContractTypeAnnotation() { FullTypeName = "IEnumerable", IsGeneric = true });
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Will refactor at some point")]
        private void AddEntityTypeDrivenToggleActions(EntityModelSchema schema, EntitySet entitySet)
        {
            int functionCount = 0;

            var entityType = entitySet.EntityType;

            // Create a bunch of functions based on the entityType provided
            var toggleProperty = this.Random.ChooseFrom(entityType.AllProperties.Where(p => typeof(BooleanDataType).IsAssignableFrom(p.PropertyType.GetType())));

            var properties = entityType.Properties.Where(p => !p.IsStream()).AsEnumerable();

            var navigationProperties = entityType.NavigationProperties;

            var bindingTypes = new DataType[] { DataTypes.EntityType.WithDefinition(entityType), DataTypes.CollectionOfEntities(entityType) };

            // Create a function where this type is returned and a input parameter
            foreach (var memberProperty in properties)
            {
                if (memberProperty == toggleProperty)
                {
                    continue;
                }

                // Skip if we have used a datatype already
                if (this.useDataTypes.ContainsKey(memberProperty.PropertyType))
                {
                    continue;
                }
                else
                {
                    this.useDataTypes.Add(memberProperty.PropertyType, true);
                }

                foreach (var bindingDataType in bindingTypes)
                {
                    string funcNameString = bindingDataType is CollectionDataType ? "_FuncCollectionBound_" : "_Func_";
                    var function = new Function(entityType.NamespaceName, entityType.Name + "_" + memberProperty.Name + funcNameString + functionCount)
                    {
                        ReturnType = memberProperty.PropertyType,
                        Parameters = 
                        {
                            new FunctionParameter(entityType.Name, bindingDataType),
                            new FunctionParameter(memberProperty.Name, memberProperty.PropertyType)
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true, BindingKind = OperationParameterBindingKind.Sometimes },
                            new ActionWithSingleParameterReturnedAnnotation(),
                        }
                    };

                    schema.Add(function);
                    functionCount++;

                    var noReturnFunction = new Function(entityType.NamespaceName, entityType.Name + "_" + memberProperty.Name + funcNameString + functionCount)
                    {
                        Parameters = 
                        {
                            new FunctionParameter(entityType.Name, bindingDataType),
                            new FunctionParameter(memberProperty.Name, memberProperty.PropertyType)
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true },
                            new ToggleBoolPropertyValueActionAnnotation()                    
                            {
                                ToggleProperty = toggleProperty.Name,
                            }
                        }
                    };

                    schema.Add(noReturnFunction);
                    functionCount++;

                    var noParameterFunction = new Function(entityType.NamespaceName, entityType.Name + "_" + memberProperty.Name + funcNameString + functionCount)
                    {
                        ReturnType = memberProperty.PropertyType,
                        Parameters = 
                        {
                            new FunctionParameter(entityType.Name, bindingDataType),
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true },
                            new ToggleBoolPropertyValueActionAnnotation()                    
                            {
                                ToggleProperty = toggleProperty.Name,
                                ReturnProperty = memberProperty.Name,
                            }
                        }
                    };

                    schema.Add(noParameterFunction);
                    functionCount++;
                }
            }

            // Create a function where this type is returned and a input parameter
            foreach (var navigationProperty in navigationProperties)
            {
                var navigationEntityType = navigationProperty.ToAssociationEnd.EntityType;
                DataType returnType = DataTypes.EntityType.WithDefinition(navigationEntityType);
                if (navigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                {
                    returnType = DataTypes.CollectionType.WithElementDataType(returnType);
                }

                // Skip if we have used a datatype already
                if (this.useDataTypes.ContainsKey(returnType))
                {
                    continue;
                }
                else
                {
                    this.useDataTypes.Add(returnType, true);
                }

                foreach (var bindingDataType in bindingTypes)
                {
                    string funcNameString = bindingDataType is CollectionDataType ? "_FuncCollectionBound_" : "_Func_";
                    var navigationFunction = new Function(entityType.NamespaceName, entityType.Name + "_" + navigationProperty.Name + funcNameString + functionCount)
                    {
                        ReturnType = returnType,
                        Parameters = 
                        {
                            new FunctionParameter(entityType.Name, bindingDataType),
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true },
                            new ToggleBoolPropertyValueActionAnnotation()                    
                            {
                                ToggleProperty = toggleProperty.Name,
                                ReturnProperty = navigationProperty.Name,
                            }
                        }
                    };

                    schema.Add(navigationFunction);
                    functionCount++;

                    var noReturnNavigationFunction = new Function(entityType.NamespaceName, entityType.Name + "_" + navigationProperty.Name + funcNameString + functionCount)
                    {
                        Parameters = 
                        {
                            new FunctionParameter(entityType.Name, bindingDataType),
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true },
                            new ToggleBoolPropertyValueActionAnnotation()                    
                            {
                                ToggleProperty = toggleProperty.Name,
                            }
                        }
                    };

                    schema.Add(noReturnNavigationFunction);
                    functionCount++;
                }
            }
        }

        private void AddIncrementIntegerPropertyActions(EntityModelSchema model, EntitySet entitySet, string propertyName)
        {
            var entityType = entitySet.EntityType;
            model.Add(
                new Function(entityType.NamespaceName, "ReturnBindingEntity" + entityType.Name)
                    {
                        ReturnType = DataTypes.EntityType.WithDefinition(entityType),
                        Parameters = 
                        {
                            new FunctionParameter("bindingEntity", DataTypes.EntityType.WithDefinition(entityType)),
                        },
                        Annotations =
                        {
                            new ServiceOperationAnnotation() { IsAction = true },
                            new IncrementIntegerPropertyValueActionAnnotation()                    
                            {
                                IntegerProperty = propertyName
                            }
                        }
                    });
        }

        private void SetupBindingParameterCollectionTypeAnnotation(EntityModelSchema model)
        {
            foreach (Function function in model.Functions.Where(f => f.IsAction()))
            {
                foreach (FunctionParameter p in function.Parameters)
                {
                    AddCollectionContractTypeAnnotation(p, p.DataType);
                }

                AddCollectionContractTypeAnnotation(function, function.ReturnType);
            }
        }

        private void AddServiceOperationsToModel(EntityModelSchema model)
        {
            model.Add(new Function("GetActor")
            {
                ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Actor")),
                Annotations = 
                {
                    new LegacyServiceOperationAnnotation 
                    { 
                        Method = HttpVerb.Get,
                        ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                    },
                    new FunctionBodyAnnotation
                    {
                        //// Enable the following line to add this to root query for cross feature testing
                        //// IsRoot = true, 
                        FunctionBodyGenerator = (schema) =>
                        {
                            var entitySet = schema.GetDefaultEntityContainer().EntitySets.Single(es => es.Name.Equals("Actor"));
                            return CommonQueryBuilder.Root(entitySet);
                        },
                    },
                }
            });

            model.Add(new Function("GetPrimitiveString")
            {
                ReturnType = EdmDataTypes.String().NotNullable(),
                Annotations = 
                {
                    new LegacyServiceOperationAnnotation
                    {
                        Method = HttpVerb.Get,
                    },
                    new FunctionBodyAnnotation
                    {
                        FunctionBody = CommonQueryBuilder.Constant("Foo"),
                    },
                },
            });

            // Add a service operation that requires parameter
            model.Add(new Function("GetArgumentPlusOne")
            {
                ReturnType = EdmDataTypes.Int32.NotNullable(),
                Annotations =
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Get,
                        },
                        new FunctionBodyAnnotation
                        {
                            FunctionBody = CommonQueryBuilder.Add(CommonQueryBuilder.FunctionParameterReference("arg1"), CommonQueryBuilder.Constant((int)1)),
                        },
                    },
                Parameters = 
                    {
                        new FunctionParameter("arg1", EdmDataTypes.Int32.NotNullable()),
                    },
            });

            // Add a WebInvoke service operation
            model.Add(
                new Function("WebInvokeGetMovie")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Movie")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Post,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        },

                        new FunctionBodyAnnotation
                        {
                            //// Enable the following line to add this to root query for cross feature testing
                            //// IsRoot = true, 
                            FunctionBodyGenerator = (schema) =>
                            {
                                var entitySet = schema.GetDefaultEntityContainer().EntitySets.Single(es => es.Name.Equals("Movie"));
                                return CommonQueryBuilder.Root(entitySet);
                            },
                        } 
                    },
                });

            model.Add(
                new Function("WebInvokeGetMovieWithParameter")
                {
                    ReturnType = DataTypes.CollectionType.WithElementDataType(DataTypes.EntityType.WithName("Movie")),
                    Annotations = 
                    {
                        new LegacyServiceOperationAnnotation
                        {
                            Method = HttpVerb.Post,
                            ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        },

                        new FunctionBodyAnnotation
                        {
                            FunctionBodyGenerator = (schema) =>
                            {
                                var entitySet = schema.GetDefaultEntityContainer().EntitySets.Single(es => es.Name.Equals("Movie"));
                                return CommonQueryBuilder.Root(entitySet);
                            },
                        } 
                    },
                    Parameters = 
                    {
                        new FunctionParameter("arg1", EdmDataTypes.Int32.NotNullable()),
                        new FunctionParameter("arg2", EdmDataTypes.String().NotNullable()),
                    },
                });
        }
    }
}
