//---------------------------------------------------------------------
// <copyright file="TestModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// A helper class to create our test models using the <see cref="ModelBuilder"/>.
    /// </summary>
    internal static class TestModels
    {
        /// <summary>
        /// Build a test model shared across several tests.
        /// </summary>
        /// <returns>Returns the test model.</returns>
        internal static EntityModelSchema BuildTestModel()
        {
            // The metadata model
            EntityModelSchema model = new EntityModelSchema();

            ComplexType addressType = model.ComplexType("Address")
                .Property("Street", EdmDataTypes.String())
                .Property("Zip", EdmDataTypes.Int32);

            EntityType officeType = model.EntityType("OfficeType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Address", DataTypes.ComplexType.WithDefinition(addressType));

            EntityType cityType = model.EntityType("CityType")
                .KeyProperty("Id", EdmDataTypes.Int32)
                .Property("Name", EdmDataTypes.String())
                .NavigationProperty("CityHall", officeType)
                .NavigationProperty("DOL", officeType)
                .NavigationProperty("PoliceStation", officeType, true)
                .NamedStream("Skyline")
                .Property("MetroLanes", DataTypes.CollectionType.WithElementDataType(EdmDataTypes.String()));

            EntityType cityWithMapType = model.EntityType("CityWithMapType")
                .WithBaseType(cityType)
                .DefaultStream();

            EntityType cityOpenType = model.EntityType("CityOpenType")
                .WithBaseType(cityType)
                .OpenType();

            EntityType personType = model.EntityType("Person")
                .KeyProperty("Id", EdmDataTypes.Int32);
            personType = personType.NavigationProperty("Friend", personType);

            EntityType employeeType = model.EntityType("Employee")
                .WithBaseType(personType)
                .Property("CompanyName", EdmDataTypes.String());

            EntityType managerType = model.EntityType("Manager")
                .WithBaseType(employeeType)
                .Property("Level", EdmDataTypes.Int32);

            model.Fixup();

            // Fixed up models will have entity sets for all base entity types.
            model.EntitySet("Employee", employeeType);
            model.EntitySet("Manager", managerType);

            return model;
        }
   }
}
