//---------------------------------------------------------------------
// <copyright file="EntityFrameworkDataProviderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// Specifies provider-level settings used during Entity Framework-based 
    /// data service build process. 
    /// </summary>
    [ImplementationName(typeof(DataProviderSettings), "EntityFramework")]
    public class EntityFrameworkDataProviderSettings : DataProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the EntityFrameworkDataProviderSettings class.
        /// </summary>
        public EntityFrameworkDataProviderSettings()
            : base("EF")
        {
            this.EFProvider = "SQL";
            this.EntityModelSourceFilesGenerator = "POCO";
            this.MappingGenerator = "AnnotationBased";
            this.ApplyConceptualModelStoreProviderFixup = false;
            this.ContextServices = "ObjectContext";
            this.PocoOption = PocoOption.None;

            // EF using ObjectContext does support MEST but Taupo fails to create databases in this case
            // Implementing support on the database creation side for MEST doesn't feel important enough
            // to do given MEST isn't a common scenario for now
            this.SupportsMest = false;

            // EF doesn't support spatial yet
            this.SupportsSpatial = false;
        }

        /// <summary>
        /// indicating if provider uses sql server.
        /// </summary>
        [IgnoreDataMember]
        public override bool IsRunOnSqlServer
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the entity model sources generator to use.
        /// </summary>
        /// <remarks>Name must match [ImplementationName] defined on the implementation class.</remarks>
        [InjectTestParameter("ServiceBuilder/Provider/EF/EntityGenerator", HelpText = "Examples: POCO, Product", DefaultValueDescription = "POCO", IsObsolete = true)]
        [InjectTestParameter("ServiceBuilder/Provider/EF/EntityModelSourceFilesGenerator", HelpText = "Examples: POCO, Product", DefaultValueDescription = "POCO")]
        public string EntityModelSourceFilesGenerator { get; set; }

        /// <summary>
        /// Gets or sets the name of the mapping generator to use.
        /// </summary>
        /// <remarks>Name must match [ImplementationName] defined on the implementation class.</remarks>
        [InjectTestParameter("ServiceBuilder/Provider/EF/MapingGenerator", HelpText = "Examples: AnnotationBased", DefaultValueDescription = "AnnotationBased", IsObsolete = true)]
        [InjectTestParameter("ServiceBuilder/Provider/EF/MappingGenerator", HelpText = "Examples: AnnotationBased", DefaultValueDescription = "AnnotationBased")]
        public string MappingGenerator { get; set; }

        /// <summary>
        /// Gets or sets the database provider.
        /// </summary>
        [InjectTestParameter("ServiceBuilder/Provider/EF/DatabaseProvider", HelpText = "Examples: SQL, CE4.0", DefaultValueDescription = "SQL")]
        public string EFProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply store provider-specific fixup to conceptual model.
        /// </summary>
        [InjectTestParameter("ServiceBuilder/Provider/EF/ApplyStoreSpecificModelFixup", DefaultValueDescription = "False")]
        public bool ApplyConceptualModelStoreProviderFixup { get; set; }

        /// <summary>
        /// Gets or sets the database server name to connect to.
        /// </summary>
        [InjectTestParameter("ServiceBuilder/Provider/EF/ServerName", DefaultValueDescription = "(none)", HelpText = "Custom name of the server to connect to.")]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets connection string that should be used.
        /// </summary>
        [InjectTestParameter("ServiceBuilder/Provider/EF/ConnectionString", DefaultValueDescription = "(none)", HelpText = "Custom connection string to use")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the context type indicating the type of entity framework workspace.
        /// </summary>
        [InjectTestParameter("ServiceBuilder/Provider/EF/ContextType", DefaultValueDescription = "ObjectContext", HelpText = "Examples: ObjectContext, DbContext")]
        public string ContextServices { get; set; }

        /// <summary>
        /// Gets or sets the Poco Option
        /// </summary>
        /// <value>The poco option.</value>
        [InjectTestParameter("ServiceBuilder/Provider/EF/PocoOption", DefaultValueDescription = "None", HelpText = "The type of POCO class to be generated. Example: for generating POCO classes with all properties virtual, specify 'AllPropertiesVirtual'")]
        public PocoOption PocoOption { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the service method resolver
        /// </summary>
        [InjectDependency]
        [IgnoreDataMember]
        public ServiceMethodResolver ServiceMethodResolver { get; set; }

        /// <summary>
        /// Gets or sets the Remove spatial feature fixup
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public RemoveSpatialFeatureFixup RemoveSpatialFeatureFixup { get; set; }

        /// <summary>
        /// Gets or sets the Remove Multi Value Fixup
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public RemoveMultiValueFixup RemoveMultiValueFixup { get; set; }

        /// <summary>
        /// Gets or sets the SetDefaultCollectionTypes Fixup 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public SetDefaultCollectionTypesFixup SetDefaultCollectionTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets the Remove concurrenty annotation on derived types fixup.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public RemoveConcurrencyAnnotationFromDerivedTypesFixup RemoveConcurrencyAnnotationFromDerivedTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use client payload verification.
        /// </summary>
        /// <value>
        /// <c>true</c> if using client payload verification; otherwise, <c>false</c>.
        /// </value>
        [InjectTestParameter("UsePayloadDrivenVerification", DefaultValueDescription = "False", HelpText = "Whether the tests should use payload-driven verification")]
        [IgnoreDataMember]
        public override bool UsePayloadDrivenVerification
        {
            get
            {
                return true;
            }

            set
            {
                value = base.UsePayloadDrivenVerification;

                if (value == false)
                {
                    this.Logger.WriteLine(LogLevel.Info, "Cannot set payload verification to false on EF provider!");
                }
            }
        }

        /// <summary>
        /// Returns a fixup for making provider-specific alterations to the model, or null if no alterations are needed
        /// </summary>
        /// <returns>A provider-specific fixup or null</returns>
        public override IEntityModelFixup GetProviderSpecificModelFixup()
        {
            // This is a temporary workaround till we can serialize csdl back from the server with data-generation hints.
            return new CompositeEntityModelFixup(this.RemoveMultiValueFixup, this.RemoveSpatialFeatureFixup, new FixupForDataGeneration(), this.SetDefaultCollectionTypesFixup, this.RemoveConcurrencyAnnotationFromDerivedTypesFixup);
        }

        /// <summary>
        /// Returns the service method resolver specific to the EF provider
        /// </summary>
        /// <returns>the EF provider specific service method resolver</returns>
        public override IServiceMethodResolver GetProviderSpecificServiceModelResolver()
        {
            return this.ServiceMethodResolver;
        }

        /// <summary>
        /// Fixup for data generation so that generated data is valid and compares the same in CLR in SQL.
        /// </summary>
        private class FixupForDataGeneration : IEntityModelFixup
        {
            private static Dictionary<Type, DataGenerationHint[]> predefinedHintsPerType = new Dictionary<Type, DataGenerationHint[]>()
            {
                { 
                    typeof(DateTime), new DataGenerationHint[]
                                { 
                                    DataGenerationHints.MinValue(DateTime.Parse("1753-01-01 00:00:00.000", CultureInfo.InvariantCulture)),
                                    DataGenerationHints.MaxValue(DateTime.Parse("9999-12-31 23:59:59.997", CultureInfo.InvariantCulture)),
                                    DataGenerationHints.FractionalSeconds(2),
                                }
                },
                { 
                    typeof(float), new DataGenerationHint[]
                            { 
                                DataGenerationHints.MinValue((float)-3.40E38),
                                DataGenerationHints.MaxValue((float)+3.40E38),
                                DataGenerationHints.FractionalDigits(6),
                            }
                },
                { 
                    typeof(double), new DataGenerationHint[]
                            { 
                                DataGenerationHints.MinValue((double)-1.79E308),
                                DataGenerationHints.MaxValue((double)+1.79E308),
                                DataGenerationHints.FractionalDigits(14),
                            }
                },
                { 
                    typeof(DateTimeOffset), new DataGenerationHint[]
                             {
                                DataGenerationHints.MinValue(DateTimeOffset.Parse("0001-01-01 00:00:00.0000000 -14:00", CultureInfo.InvariantCulture)),
                                DataGenerationHints.MaxValue(DateTimeOffset.Parse("9999-12-31 23:59:59.9999999 +14:00", CultureInfo.InvariantCulture)),
                                DataGenerationHints.FractionalSeconds(7),
                            }
                },
                { 
                    typeof(TimeSpan), new DataGenerationHint[]
                            { 
                                DataGenerationHints.MinValue(TimeSpan.Parse("00:00:00.0000000", CultureInfo.InvariantCulture)),
                                DataGenerationHints.MaxValue(TimeSpan.Parse("23:59:59.9999999", CultureInfo.InvariantCulture)),
                                DataGenerationHints.FractionalSeconds(7),
                            }
                },
            };
            
            /// <summary>
            /// Adds data generation hints and fixups types so that generated data is valid and compares the same in CLR in SQL.
            /// </summary>
            /// <param name="model">Entity model schema.</param>
            public void Fixup(EntityModelSchema model)
            {
                List<NamedStructuralType> visited = new List<NamedStructuralType>();
                
                foreach (var complexType in model.ComplexTypes)
                {
                    Fixup(complexType, visited);
                }

                foreach (var entityType in model.EntityTypes)
                {
                    Fixup(entityType, visited);
                }

                foreach (var function in model.Functions)
                {
                    Fixup(function);
                }
            }

            private static void Fixup(NamedStructuralType structuralType, List<NamedStructuralType> visited)
            {
                if (visited.Contains(structuralType))
                {
                    return;
                }

                visited.Add(structuralType);

                foreach (MemberProperty property in structuralType.Properties)
                {
                    PrimitiveDataType primitiveDataType = property.PropertyType as PrimitiveDataType;
                    ComplexDataType complexDataType = property.PropertyType as ComplexDataType;

                    if (complexDataType != null)
                    {
                        Fixup(complexDataType.Definition, visited);
                    }
                    else if (primitiveDataType != null)
                    {
                        Type clrType = primitiveDataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                        ExceptionUtilities.CheckObjectNotNull(clrType, "PrimitiveClrTypeFacet has not been defined for the property: '{0}.{1}'.", structuralType.Name, property.Name);

                        AddDataGenerationHints(property, clrType);

                        property.PropertyType = FixupType(primitiveDataType, clrType);
                    }
                }
            }

            private static void Fixup(Function function)
            {
                foreach (var parameter in function.Parameters)
                {
                    var primitiveDataType = parameter.DataType as PrimitiveDataType;
                    if (primitiveDataType != null)
                    {
                        Type clrType = primitiveDataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                        ExceptionUtilities.CheckObjectNotNull(clrType, "PrimitiveClrTypeFacet has not been defined for the parameter '{0}' on function '{1}'.", parameter.Name, function.Name);

                        AddDataGenerationHints(parameter, clrType);

                        parameter.DataType = FixupType(primitiveDataType, clrType);
                    }
                }
            }

            private static void AddDataGenerationHints(FunctionParameter parameter, Type clrType)
            {
                DataGenerationHint[] hints;
                if (predefinedHintsPerType.TryGetValue(clrType, out hints))
                {
                    parameter.WithDataGenerationHints(hints);
                }
            }

            private static void AddDataGenerationHints(MemberProperty property, Type clrType)
            {
                DataGenerationHint[] hints;
                if (predefinedHintsPerType.TryGetValue(clrType, out hints))
                {
                    property.WithDataGenerationHints(hints);
                }
            }

            private static PrimitiveDataType FixupType(PrimitiveDataType primitiveDataType, Type clrType)
            {
                // Add precision and scale for decimal properties.
                if (clrType == typeof(decimal))
                {
                    if (!primitiveDataType.Facets.Any(f => f is NumericPrecisionFacet))
                    {
                        primitiveDataType = primitiveDataType.WithFacet(new NumericPrecisionFacet(28));
                    }

                    if (!primitiveDataType.Facets.Any(f => f is NumericScaleFacet))
                    {
                        int precision = primitiveDataType.GetFacet<NumericPrecisionFacet>().Value;
                        primitiveDataType = primitiveDataType.WithFacet(new NumericScaleFacet(Math.Min(4, precision)));
                    }
                }

                return primitiveDataType;
            }            
        }
    }
}