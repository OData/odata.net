//---------------------------------------------------------------------
// <copyright file="OpenTypesComplexPropertiesModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// Generates a model which reproduces problems with Complex types on Open Types which include non string value members.
    /// </summary>
    [ImplementationName(typeof(IModelGenerator), "OpenTypesComplexPropertiesModelGenerator")]
    public class OpenTypesComplexPropertiesModelGenerator : IModelGenerator
    {
        /// <summary>
        /// Generates the model.
        /// </summary>
        /// <returns>A Valid model containing one open entity containing one dynamic complex type</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is unavoidable, we need to create entire model here.")]
        [SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Locals created by the compiler.")]
        public EntityModelSchema GenerateModel()
        {
            var model = new EntityModelSchema()
                {
                    new EntityType("Customer")
                    {
                        Properties = 
                        {
                            new MemberProperty("CustomerId", DataTypes.Integer) { IsPrimaryKey = true },
                            new MemberProperty("Name", DataTypes.String.WithMaxLength(100)).WithDataGenerationHints(DataGenerationHints.InterestingValue<string>(string.Empty), DataGenerationHints.AnsiString, DataGenerationHints.MinLength(3)),                  
                            new MemberProperty("PrimaryContactInfo1", DataTypes.ComplexType.WithName("ContactDetails"))
                            {
                                Annotations =
                                {
                                    new MetadataDeclaredPropertyAnnotation(),
                                }
                            },
                            new MemberProperty("PrimaryContactInfo2", DataTypes.ComplexType.WithName("ContactDetails")),
                            new MemberProperty("Thumbnail", DataTypes.Stream),
                        },
                        IsOpen = true,
                    },
                    new ComplexType("ContactDetails")
                    {
                        new MemberProperty("FirstContacted", DataTypes.Binary),                        
                        new MemberProperty("LastContacted", DataTypes.DateTime.WithTimeZoneOffset(true)),
                        new MemberProperty("Contacted", DataTypes.DateTime),
                        new MemberProperty("GUID", DataTypes.Guid),
                        new MemberProperty("PreferedContactTime", DataTypes.TimeOfDay),                                                                                              
                        new MemberProperty("Byte", EdmDataTypes.Byte),
                        new MemberProperty("SignedByte", EdmDataTypes.SByte),
                        new MemberProperty("Double", EdmDataTypes.Double),
                        new MemberProperty("Single", EdmDataTypes.Single),
                        new MemberProperty("Short", EdmDataTypes.Int16),
                        new MemberProperty("Int", EdmDataTypes.Int32),
                        new MemberProperty("Long", EdmDataTypes.Int64),                                                                                      
                    },  
                };      

            // Apply default fixups
            new ResolveReferencesFixup().Fixup(model);
            new ApplyDefaultNamespaceFixup("OpenTypesComplexPropertiesModelGenerator").Fixup(model);
            new AddDefaultContainerFixup().Fixup(model);

            EntityContainer container = model.GetDefaultEntityContainer();
            if (!container.Annotations.OfType<EntitySetRightsAnnotation>().Any())
            {
                container.Annotations.Add(new EntitySetRightsAnnotation() { Value = EntitySetRights.All });
            }

            return model;
       }
    }
}
