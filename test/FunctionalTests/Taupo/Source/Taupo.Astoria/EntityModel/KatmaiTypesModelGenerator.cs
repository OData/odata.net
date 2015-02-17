//---------------------------------------------------------------------
// <copyright file="KatmaiTypesModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Model aimed at covering interesting shapes specific to katmai types
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "KatmaiTypes", HelpText = "Default model for katmai types")]
    public class KatmaiTypesModelGenerator : IModelGenerator
    {
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
                new EntityType("KatmaiEntity_DTO")
                {
                    new MemberProperty("Id", DataTypes.DateTime.WithTimeZoneOffset(true)) { IsPrimaryKey = true },
                    new MemberProperty("ETag", DataTypes.DateTime.Nullable().WithTimeZoneOffset(true)) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("Published", DataTypes.DateTime.NotNullable().WithTimeZoneOffset(true)),
                    new MemberProperty("Complex", DataTypes.ComplexType.WithName("KatmaiComplex_DTO")).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("Collection", DataTypes.CollectionType.WithElementDataType(DataTypes.DateTime.WithTimeZoneOffset(true))),
                    new MemberProperty("ComplexCollection", DataTypes.CollectionOfComplex("KatmaiComplex_DTO")),

                    new NavigationProperty("Ref", "KatmaiLink_DTO", "DTO", "Link"),
                },
                new ComplexType("KatmaiComplex_DTO")
                {
                    new MemberProperty("Updated", DataTypes.DateTime.NotNullable().WithTimeZoneOffset(true)),
                    new MemberProperty("Nullable", DataTypes.DateTime.Nullable().WithTimeZoneOffset(true)),
                    new MemberProperty("Collection", DataTypes.CollectionType.WithElementDataType(DataTypes.DateTime.WithTimeZoneOffset(true))),
                },
                new EntityType("KatmaiEntity_TS")
                {
                    new MemberProperty("Id", DataTypes.TimeOfDay) { IsPrimaryKey = true },
                    new MemberProperty("ETag", DataTypes.TimeOfDay.Nullable()) { Annotations = { new ConcurrencyTokenAnnotation() } },
                    new MemberProperty("Title", DataTypes.TimeOfDay),
                    new MemberProperty("Complex", DataTypes.ComplexType.WithName("KatmaiComplex_TS")).WithDataGenerationHints(DataGenerationHints.NoNulls),
                    new MemberProperty("Collection", DataTypes.CollectionType.WithElementDataType(DataTypes.TimeOfDay)),
                    new MemberProperty("ComplexCollection", DataTypes.CollectionOfComplex("KatmaiComplex_TS")),

                    new NavigationProperty("Ref", "KatmaiLink_TS", "TS", "Link"),
                },
                new ComplexType("KatmaiComplex_TS")
                {
                    new MemberProperty("Summary", DataTypes.TimeOfDay),
                    new MemberProperty("Nullable", DataTypes.TimeOfDay.Nullable()),
                    new MemberProperty("Collection", DataTypes.CollectionType.WithElementDataType(DataTypes.TimeOfDay)),
                },
                new EntityType("KatmaiLink")
                {
                    new MemberProperty("DTO", DataTypes.DateTime.WithTimeZoneOffset(true)) { IsPrimaryKey = true },
                    new MemberProperty("TS", DataTypes.TimeOfDay) { IsPrimaryKey = true },

                    new NavigationProperty("DTOEntity", "KatmaiLink_DTO", "Link", "DTO"),
                    new NavigationProperty("TSEntity", "KatmaiLink_TS", "Link", "TS"),
                },
                new AssociationType("KatmaiLink_DTO")
                {
                    Ends = 
                    {
                        new AssociationEnd("Link", "KatmaiLink", EndMultiplicity.Many),
                        new AssociationEnd("DTO", "KatmaiEntity_DTO", EndMultiplicity.One),
                    },
                    ReferentialConstraint = 
                        new ReferentialConstraint()
                            .WithDependentProperties("Link", "DTO")
                            .ReferencesPrincipalProperties("DTO", "Id"),
                },
                new AssociationType("KatmaiLink_TS")
                {
                    Ends = 
                    {
                        new AssociationEnd("Link", "KatmaiLink", EndMultiplicity.Many),
                        new AssociationEnd("TS", "KatmaiEntity_TS", EndMultiplicity.One),
                    },
                    ReferentialConstraint = 
                        new ReferentialConstraint()
                            .WithDependentProperties("Link", "TS")
                            .ReferencesPrincipalProperties("TS", "Id"),
                },
                new EntityType("Calendar")
                {
                    new MemberProperty("UID", DataTypes.Integer.NotNullable()) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String.NotNullable().WithMaxLength(512)),
                    new MemberProperty("IsBaseCalendar", DataTypes.Boolean.NotNullable()),
                    new MemberProperty("WeekDays", DataTypes.CollectionOfComplex("WeekDay")),
                },
                new ComplexType("WeekDay")
                {
                    new MemberProperty("DayType", DataTypes.Integer),
                    new MemberProperty("DayWorking", DataTypes.Boolean),
                    new MemberProperty("TimePeriod", DataTypes.ComplexType.WithName("TimePeriod").NotNullable()),
                    new MemberProperty("WorkingTimes", DataTypes.CollectionOfComplex("WorkingTime")),
                },
                new ComplexType("TimePeriod")
                {
                    new MemberProperty("FromDate", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                    new MemberProperty("ToDate", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                },
                new ComplexType("WorkingTime")
                {
                    new MemberProperty("FromTime", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                    new MemberProperty("ToTime", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()),
                },
                new EntityType("Footrace")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),

                    new NavigationProperty("Times", "Footrace_Time", "Footrace", "Time"),
                },
                new EntityType("Person")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("Name", DataTypes.String),

                    new NavigationProperty("Calendars", "Person_Calendar", "Person", "Calendar"),
                    new NavigationProperty("RaceTimes", "Person_Time", "Person", "Time"),
                },
                new EntityType("FootraceTime")
                {
                    new MemberProperty("Id", DataTypes.Integer) { IsPrimaryKey = true },
                    new MemberProperty("StartTime", DataTypes.DateTime.WithTimeZoneOffset(true)),
                    new MemberProperty("ElapsedTime", DataTypes.TimeOfDay),
                    new MemberProperty("EndTime", DataTypes.DateTime.WithTimeZoneOffset(true)),

                    new NavigationProperty("Person", "Person_Time", "Time", "Person"),
                    new NavigationProperty("FootRace", "Footrace_Time", "Time", "Footrace"),
                },
                new AssociationType("Person_Calendar")
                {
                    Ends = 
                    {
                        new AssociationEnd("Person", "Person", EndMultiplicity.One),
                        new AssociationEnd("Calendar", "Calendar", EndMultiplicity.Many),
                    },
                },
                new AssociationType("Person_Time")
                {
                    Ends = 
                    {
                        new AssociationEnd("Person", "Person", EndMultiplicity.One),
                        new AssociationEnd("Time", "FootraceTime", EndMultiplicity.Many),
                    },
                },
                 new AssociationType("Footrace_Time")
                {
                    Ends = 
                    {
                        new AssociationEnd("Footrace", "Footrace", EndMultiplicity.One),
                        new AssociationEnd("Time", "FootraceTime", EndMultiplicity.Many),
                    },
                },
                new EntityContainer("KatmaiTypeContainer")
                {
                    new DataServiceConfigurationAnnotation()
                    {
                        UseVerboseErrors = true,
                    },
                    new DataServiceBehaviorAnnotation()
                    {
                        MaxProtocolVersion = DataServiceProtocolVersion.V4,
                    },
                    new EntitySet("DateTimeOffsets", "KatmaiEntity_DTO")
                    {
                        new PageSizeAnnotation() { PageSize = 3 },
                    },
                    new EntitySet("TimeSpans", "KatmaiEntity_TS")
                    {
                        new PageSizeAnnotation() { PageSize = 7 },
                    },
                    new EntitySet("KatmaiLinks", "KatmaiLink"),
                    new AssociationSet("KatmaiLinks_DTO")
                    {
                        AssociationType = "KatmaiLink_DTO",
                        Ends =
                        {
                            new AssociationSetEnd("Link", "KatmaiLinks"),
                            new AssociationSetEnd("DTO", "DateTimeOffsets"),
                        }
                    },
                    new AssociationSet("KatmaiLinks_TS")
                    {
                        AssociationType = "KatmaiLink_TS",
                        Ends =
                        {
                            new AssociationSetEnd("Link", "KatmaiLinks"),
                            new AssociationSetEnd("TS", "TimeSpans"),
                        }
                    },
                    new EntitySet("Calendars", "Calendar"),
                    new EntitySet("People", "Person"),
                    new EntitySet("Footraces", "Footrace"),
                    new EntitySet("FootraceTimes", "FootraceTime"),
                    new AssociationSet("People_Calendars")
                    {
                        AssociationType = "Person_Calendar",
                        Ends =
                        {
                            new AssociationSetEnd("Person", "People"),
                            new AssociationSetEnd("Calendar", "Calendars"),
                        }
                    },
                    new AssociationSet("People_Times")
                    {
                        AssociationType = "Person_Time",
                        Ends =
                        {
                            new AssociationSetEnd("Person", "People"),
                            new AssociationSetEnd("Time", "FootraceTimes"),
                        }
                    },
                    new AssociationSet("Footraces_Times")
                    {
                        AssociationType = "Footrace_Time",
                        Ends =
                        {
                            new AssociationSetEnd("Footrace", "Footraces"),
                            new AssociationSetEnd("Time", "FootraceTimes"),
                        }
                    },
                }
            };

            // TODO: re-enable when product bug for service ops with type segments are fixed
            ////new AddRootServiceOperationsFixup().Fixup(model);

            model.Add(CreateSimpleServiceOperation("GetDateTimeOffset", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()));
            model.Add(CreateSimpleServiceOperation("GetNullableDateTimeOffset", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable()));
            model.Add(CreateSimpleServiceOperation("GetTimeSpan", DataTypes.TimeOfDay.NotNullable()));
            model.Add(CreateSimpleServiceOperation("GetNullableTimeSpan", DataTypes.TimeOfDay.Nullable()));

            model.Add(CreateCollectionServiceOperation("GetDateTimeOffsets", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()));
            model.Add(CreateCollectionServiceOperation("GetNullableDateTimeOffsets", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable()));
            model.Add(CreateCollectionServiceOperation("GetTimeSpans", DataTypes.TimeOfDay.NotNullable()));
            model.Add(CreateCollectionServiceOperation("GetNullableTimeSpans", DataTypes.TimeOfDay.Nullable()));

            model.Add(CreateSingleEntityServiceOperation("GetFirstDTOEntityGreaterThan", "DateTimeOffsets", "KatmaiEntity_DTO", "Published", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()));
            model.Add(CreateSingleEntityServiceOperation("GetFirstNullableDTOEntityGreaterThan", "DateTimeOffsets", "KatmaiEntity_DTO", "ETag", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable()));
            model.Add(CreateSingleEntityServiceOperation("GetFirstTSEntityGreaterThan", "TimeSpans", "KatmaiEntity_TS", "Title", DataTypes.TimeOfDay.NotNullable()));
            model.Add(CreateSingleEntityServiceOperation("GetFirstNullableTSEntityGreaterThan", "TimeSpans", "KatmaiEntity_TS", "ETag", DataTypes.TimeOfDay.Nullable()));

            model.Add(CreateMultipleEntityServiceOperation("GetDTOEntitiesGreaterThan", "DateTimeOffsets", "KatmaiEntity_DTO", "Published", DataTypes.DateTime.WithTimeZoneOffset(true).NotNullable()));
            model.Add(CreateMultipleEntityServiceOperation("GetNullableDTOEntitiesGreaterThan", "DateTimeOffsets", "KatmaiEntity_DTO", "ETag", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable()));
            model.Add(CreateMultipleEntityServiceOperation("GetTSEntitiesGreaterThan", "TimeSpans", "KatmaiEntity_TS", "Title", DataTypes.TimeOfDay.NotNullable()));
            model.Add(CreateMultipleEntityServiceOperation("GetNullableTSEntitiesGreaterThan", "TimeSpans", "KatmaiEntity_TS", "ETag", DataTypes.TimeOfDay.Nullable()));

            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("KatmaiTypes").Fixup(model);

            return model;
        }

        private static Function CreateSimpleServiceOperation(string name, DataType type)
        {
            return new Function(name)
            {
                ReturnType = type,
                Parameters = 
                {
                    new FunctionParameter("arg", type),
                },
                Annotations =
                {
                    new LegacyServiceOperationAnnotation()
                    {
                        Method = HttpVerb.Get
                    },
                    new FunctionBodyAnnotation()
                    {
                        FunctionBody = CommonQueryBuilder.FunctionParameterReference("arg"),
                    },
                }
            };
        }

        private static Function CreateCollectionServiceOperation(string name, DataType type)
        {
            return new Function(name)
            {
                ReturnType = DataTypes.CollectionType.WithElementDataType(type),
                Parameters = 
                {
                    new FunctionParameter("arg1", type),
                    new FunctionParameter("arg2", type),
                    new FunctionParameter("arg3", type),
                },
                Annotations =
                {
                    new LegacyServiceOperationAnnotation()
                    {
                        Method = HttpVerb.Get,
                        ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IEnumerable,
                    },
                    new FunctionBodyAnnotation()
                    {
                        FunctionBody = LinqBuilder.AnonymousArray(
                            CommonQueryBuilder.FunctionParameterReference("arg1"),
                            CommonQueryBuilder.FunctionParameterReference("arg2"),
                            CommonQueryBuilder.FunctionParameterReference("arg3")),
                    },
                }
            };
        }

        private static Function CreateSingleEntityServiceOperation(string name, string entitySetName, string entityTypeName, string propertyName, DataType type)
        {
            return new Function(name)
            {
                ReturnType = DataTypes.EntityType.WithName(entityTypeName),
                Parameters = 
                {
                    new FunctionParameter("arg", type),
                },
                Annotations =
                {
                    new LegacyServiceOperationAnnotation()
                    {
                        Method = HttpVerb.Get,
                        ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                        SingleResult = true,
                    },
                    new FunctionBodyAnnotation()
                    {
                        FunctionBodyGenerator = 
                            (EntityModelSchema model) =>
                            {
                                var entitySet = model.EntityContainers.SelectMany(c => c.EntitySets).Single(s => s.Name == entitySetName);

                                Func<LinqParameterExpression, QueryExpression> lambdaWithoutNullCheck = o => o.Property(propertyName).GreaterThan(CommonQueryBuilder.FunctionParameterReference("arg"));
                                Func<LinqParameterExpression, QueryExpression> lambda = lambdaWithoutNullCheck;
                                if (type.IsNullable)
                                {
                                    lambda = o => lambdaWithoutNullCheck(o).And(o.Property(propertyName).IsNotNull());
                                }

                                return CommonQueryBuilder.Root(entitySet).Where(lambda).Take(1);
                            },
                    },
                }
            };
        }

        private static Function CreateMultipleEntityServiceOperation(string name, string entitySetName, string entityTypeName, string propertyName, DataType type)
        {
            return new Function(name)
            {
                ReturnType = DataTypes.CollectionOfEntities(entityTypeName),
                Parameters = 
                {
                    new FunctionParameter("arg", type),
                },
                Annotations =
                {
                    new LegacyServiceOperationAnnotation()
                    {
                        Method = HttpVerb.Get,
                        ReturnTypeQualifier = ServiceOperationReturnTypeQualifier.IQueryable,
                    },
                    new FunctionBodyAnnotation()
                    {
                        FunctionBodyGenerator = 
                            (EntityModelSchema model) =>
                            {
                                var entitySet = model.EntityContainers.SelectMany(c => c.EntitySets).Single(s => s.Name == entitySetName);
                                return CommonQueryBuilder.Root(entitySet).Where(o => o.Property(propertyName).GreaterThan(CommonQueryBuilder.FunctionParameterReference("arg")));
                            },
                    },
                }
            };
        }
    }
}
