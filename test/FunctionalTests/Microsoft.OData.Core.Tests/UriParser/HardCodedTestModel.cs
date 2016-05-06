//---------------------------------------------------------------------
// <copyright file="HardCodedTestModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Core.Tests.UriParser
{
    /// <summary>
    /// Class to provide a test model for semantic unit tests.
    /// TODO: Use constructable APIs instead of string CSDL and remember all the objects.
    /// </summary>
    /// <remarks>
    /// Try not to use this class anymore. Too many tests have a dependency on the same model.
    /// </remarks>
    internal class HardCodedTestModel
    {
        private static readonly IEdmModel Model = GetEdmModel();

        public static IEdmModel TestModel
        {
            get { return Model; }
        }

        internal static IEdmModel GetEdmModel()
        {
            var model = new EdmModel();

            #region Type Definitions

            var FullyQualifiedNamespaceIdType = new EdmTypeDefinition("Fully.Qualified.Namespace", "IdType", EdmPrimitiveTypeKind.Double);
            var FullyQualifiedNamespaceIdTypeReference = new EdmTypeDefinitionReference(FullyQualifiedNamespaceIdType, false);
            model.AddElement(FullyQualifiedNamespaceIdType);

            var FullyQualifiedNamespaceNameType = new EdmTypeDefinition("Fully.Qualified.Namespace", "NameType", EdmPrimitiveTypeKind.String);
            var FullyQualifiedNamespaceNameTypeReference = new EdmTypeDefinitionReference(FullyQualifiedNamespaceNameType, false);
            model.AddElement(FullyQualifiedNamespaceNameType);

            var FullyQualifiedNamespaceUInt16Reference = model.GetUInt16("Fully.Qualified.Namespace", false);
            var FullyQualifiedNamespaceUInt32Reference = model.GetUInt32("Fully.Qualified.Namespace", false);
            var FullyQualifiedNamespaceUInt64Reference = model.GetUInt64("Fully.Qualified.Namespace", false);

            #endregion

            #region Enum Types
            var colorType = new EdmEnumType("Fully.Qualified.Namespace", "ColorPattern", EdmPrimitiveTypeKind.Int64, true);
            colorType.AddMember("Red", new EdmIntegerConstant(1L));
            colorType.AddMember("Blue", new EdmIntegerConstant(2L));
            colorType.AddMember("Yellow", new EdmIntegerConstant(4L));
            colorType.AddMember("Solid", new EdmIntegerConstant(8L));
            colorType.AddMember("Striped", new EdmIntegerConstant(16L));
            colorType.AddMember("SolidRed", new EdmIntegerConstant(9L));
            colorType.AddMember("SolidBlue", new EdmIntegerConstant(10L));
            colorType.AddMember("SolidYellow", new EdmIntegerConstant(12L));
            colorType.AddMember("RedBlueStriped", new EdmIntegerConstant(19L));
            colorType.AddMember("RedYellowStriped", new EdmIntegerConstant(21L));
            colorType.AddMember("BlueYellowStriped", new EdmIntegerConstant(22L));
            model.AddElement(colorType);
            var colorTypeReference = new EdmEnumTypeReference(colorType, false);
            var nullableColorTypeReference = new EdmEnumTypeReference(colorType, true);

            var NonFlagShapeType = new EdmEnumType("Fully.Qualified.Namespace", "NonFlagShape", EdmPrimitiveTypeKind.SByte, false);
            NonFlagShapeType.AddMember("Rectangle", new EdmIntegerConstant(1));
            NonFlagShapeType.AddMember("Triangle", new EdmIntegerConstant(2));
            NonFlagShapeType.AddMember("foursquare", new EdmIntegerConstant(3));
            model.AddElement(NonFlagShapeType);
            #endregion

            #region Structured Types
            var FullyQualifiedNamespacePerson = new EdmEntityType("Fully.Qualified.Namespace", "Person", null, false, false);
            var FullyQualifiedNamespaceEmployee = new EdmEntityType("Fully.Qualified.Namespace", "Employee", FullyQualifiedNamespacePerson, false, false);
            var FullyQualifiedNamespaceManager = new EdmEntityType("Fully.Qualified.Namespace", "Manager", FullyQualifiedNamespaceEmployee, false, false);
            var FullyQualifiedNamespaceOpenEmployee = new EdmEntityType("Fully.Qualified.Namespace", "OpenEmployee", FullyQualifiedNamespaceEmployee, false, true);
            var FullyQualifiedNamespaceDog = new EdmEntityType("Fully.Qualified.Namespace", "Dog", null, false, false);
            var FullyQualifiedNamespaceLion = new EdmEntityType("Fully.Qualified.Namespace", "Lion", null, false, false);
            var FullyQualifiedNamespaceChimera = new EdmEntityType("Fully.Qualified.Namespace", "Chimera", null, false, true);
            var FullyQualifiedNamespacePainting = new EdmEntityType("Fully.Qualified.Namespace", "Painting", null, false, true);
            var FullyQualifiedNamespaceFramedPainting = new EdmEntityType("Fully.Qualified.Namespace", "FramedPainting", FullyQualifiedNamespacePainting, false, true);
            var FullyQualifiedNamespaceUserAccount = new EdmEntityType("Fully.Qualified.Namespace", "UserAccount", null, false, false);
            var FullyQualifiedNamespacePet1 = new EdmEntityType("Fully.Qualified.Namespace", "Pet1", null, false, false);
            var FullyQualifiedNamespacePet2 = new EdmEntityType("Fully.Qualified.Namespace", "Pet2", null, false, false);
            var FullyQualifiedNamespacePet3 = new EdmEntityType("Fully.Qualified.Namespace", "Pet3", null, false, false);
            var FullyQualifiedNamespacePet4 = new EdmEntityType("Fully.Qualified.Namespace", "Pet4", null, false, false);
            var FullyQualifiedNamespacePet5 = new EdmEntityType("Fully.Qualified.Namespace", "Pet5", null, false, false);
            var FullyQualifiedNamespacePet6 = new EdmEntityType("Fully.Qualified.Namespace", "Pet6", null, false, false);

            var FullyQualifiedNamespaceAddress = new EdmComplexType("Fully.Qualified.Namespace", "Address");
            var FullyQualifiedNamespaceOpenAddress = new EdmComplexType("Fully.Qualified.Namespace", "OpenAddress", null, false, true);
            var FullyQualifiedNamespaceHomeAddress = new EdmComplexType("Fully.Qualified.Namespace", "HomeAddress", FullyQualifiedNamespaceAddress);

            var FullyQualifiedNamespaceHeartbeat = new EdmComplexType("Fully.Qualified.Namespace", "Heartbeat");

            var FullyQualifiedNamespacePersonTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePerson, true);
            var FullyQualifiedNamespaceEmployeeTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceEmployee, true);
            var FullyQualifiedNamespaceManagerTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceManager, true);
            var FullyQualifiedNamespaceOpenEmployeeTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceOpenEmployee, true);
            var FullyQualifiedNamespaceDogTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceDog, true);
            var FullyQualifiedNamespaceLionTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceLion, true);
            var FullyQualifiedNamespacePet1TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet1, true);
            var FullyQualifiedNamespacePet2TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet2, true);
            var FullyQualifiedNamespacePet3TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet3, true);
            var FullyQualifiedNamespacePet4TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet4, true);
            var FullyQualifiedNamespacePet5TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet5, true);
            var FullyQualifiedNamespacePet6TypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePet6, true);
            var FullyQualifiedNamespacePaintingTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespacePainting, true);
            var FullyQualifiedNamespaceFramedPaintingTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceFramedPainting, true);
            var FullyQualifiedNamespaceUserAccountTypeReference = new EdmEntityTypeReference(FullyQualifiedNamespaceUserAccount, true);

            var FullyQualifiedNamespaceLion_ID1 = FullyQualifiedNamespaceLion.AddStructuralProperty("ID1", EdmCoreModel.Instance.GetInt32(false));
            var FullyQualifiedNamespaceLion_ID2 = FullyQualifiedNamespaceLion.AddStructuralProperty("ID2", EdmCoreModel.Instance.GetInt32(false));
            FullyQualifiedNamespaceLion.AddStructuralProperty("AngerLevel", EdmCoreModel.Instance.GetDouble(true));
            FullyQualifiedNamespaceLion.AddStructuralProperty("AttackDates", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDateTimeOffset(true))));
            FullyQualifiedNamespaceLion.AddStructuralProperty("LionHeartbeat", new EdmComplexTypeReference(FullyQualifiedNamespaceHeartbeat, true));
            FullyQualifiedNamespaceLion.AddKeys(new IEdmStructuralProperty[] { FullyQualifiedNamespaceLion_ID1, FullyQualifiedNamespaceLion_ID2, });
            model.AddElement(FullyQualifiedNamespaceLion);

            var FullyQualifiedNamespaceAddressTypeReference = new EdmComplexTypeReference(FullyQualifiedNamespaceAddress, true);
            var FullyQualifiedNamespaceOpenAddressTypeReference = new EdmComplexTypeReference(FullyQualifiedNamespaceOpenAddress, true);
            var FullyQualifiedNamespacePerson_ID = FullyQualifiedNamespacePerson.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            var FullyQualifiedNamespacePerson_SSN = FullyQualifiedNamespacePerson.AddStructuralProperty("SSN", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("Shoe", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeographyPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeographyLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeographyPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeometryPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeometryLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeometryPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeographyCollection", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true))));
            FullyQualifiedNamespacePerson.AddStructuralProperty("GeometryCollection", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true))));
            var FullyQualifiedNamespacePerson_Name = FullyQualifiedNamespacePerson.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            var FullyQualifiedNamespacePerson_FirstName = FullyQualifiedNamespacePerson.AddStructuralProperty("FirstName", FullyQualifiedNamespaceNameTypeReference);
            FullyQualifiedNamespacePerson.AddStructuralProperty("Prop.With.Periods", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyDate", EdmCoreModel.Instance.GetDate(false));
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyDates", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDate(true))));
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyTimeOfDay", EdmCoreModel.Instance.GetTimeOfDay(false));
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyTimeOfDays", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetTimeOfDay(true))));
            FullyQualifiedNamespacePerson.AddStructuralProperty("Birthdate", EdmCoreModel.Instance.GetDateTimeOffset(false));
            FullyQualifiedNamespacePerson.AddStructuralProperty("FavoriteDate", EdmCoreModel.Instance.GetDateTimeOffset(true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("TimeEmployed", EdmCoreModel.Instance.GetDuration(true));
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyAddress", FullyQualifiedNamespaceAddressTypeReference);
            FullyQualifiedNamespacePerson.AddStructuralProperty("MyOpenAddress", FullyQualifiedNamespaceOpenAddressTypeReference);
            FullyQualifiedNamespacePerson.AddStructuralProperty("PreviousAddresses", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceAddressTypeReference)));
            FullyQualifiedNamespacePerson.AddStructuralProperty("FavoriteColors", new EdmCollectionTypeReference(new EdmCollectionType(colorTypeReference)));
            FullyQualifiedNamespacePerson.AddStructuralProperty("FavoriteNumber", FullyQualifiedNamespaceUInt16Reference);
            FullyQualifiedNamespacePerson.AddStructuralProperty("StockQuantity", FullyQualifiedNamespaceUInt32Reference);
            FullyQualifiedNamespacePerson.AddStructuralProperty("LifeTime", FullyQualifiedNamespaceUInt64Reference);
            FullyQualifiedNamespacePerson.AddKeys(FullyQualifiedNamespacePerson_ID);
            var FullyQualifiedNamespacePerson_MyDog = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyDog", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = FullyQualifiedNamespaceDog });
            var FullyQualifiedNamespacePerson_MyRelatedDogs = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyFriendsDogs", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespaceDog });
            var FullyQualifiedNamespacePerson_MyPaintings = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyPaintings", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespacePainting });
            var FullyQualifiedNamespacePerson_MyFavoritePainting = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyFavoritePainting", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = FullyQualifiedNamespacePainting });
            var FullyQualifiedNamespacePerson_MyLions = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "MyLions",
                TargetMultiplicity = EdmMultiplicity.Many,
                Target = FullyQualifiedNamespaceLion,
                DependentProperties = new List<IEdmStructuralProperty>()
                {
                    FullyQualifiedNamespacePerson_ID
                },
                PrincipalProperties = new List<IEdmStructuralProperty>()
                {
                    FullyQualifiedNamespaceLion_ID1
                }
            });
            FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "MyContainedDog",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = FullyQualifiedNamespaceDog,
                    ContainsTarget = true
                });
            FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "MyContainedChimeras",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = FullyQualifiedNamespaceChimera,
                    ContainsTarget = true
                });

            var FullyQualifiedNamespacePerson_MyPet2Set = FullyQualifiedNamespacePerson.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyPet2Set", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespacePet2, });

            model.AddAlternateKeyAnnotation(FullyQualifiedNamespacePerson, new Dictionary<string, IEdmProperty>()
            {
                {"SocialSN", FullyQualifiedNamespacePerson_SSN}
            });

            model.AddAlternateKeyAnnotation(FullyQualifiedNamespacePerson, new Dictionary<string, IEdmProperty>()
            {
                {"NameAlias", FullyQualifiedNamespacePerson_Name},
                {"FirstNameAlias", FullyQualifiedNamespacePerson_FirstName}
            });

            model.AddElement(FullyQualifiedNamespacePerson);

            FullyQualifiedNamespaceEmployee.AddStructuralProperty("WorkEmail", EdmCoreModel.Instance.GetString(true));
            var FullyQualifiedNamespaceEmployee_PaintingsInOffice = FullyQualifiedNamespaceEmployee.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "PaintingsInOffice", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespacePainting });
            var FullyQualifiedNamespaceEmployee_Manager = FullyQualifiedNamespaceEmployee.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Manager", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = FullyQualifiedNamespaceManager });
            var FullyQualifiedNamespaceEmployee_OfficeDog = FullyQualifiedNamespaceDog.AddBidirectionalNavigation
                (
                    new EdmNavigationPropertyInfo()
                    {
                        Name = "EmployeeOwner",
                        TargetMultiplicity = EdmMultiplicity.One,
                        Target = FullyQualifiedNamespaceEmployee
                    },

                    new EdmNavigationPropertyInfo()
                    {
                        Name = "OfficeDog",
                        TargetMultiplicity = EdmMultiplicity.One,
                        Target = FullyQualifiedNamespaceDog
                    }
                );
            model.AddElement(FullyQualifiedNamespaceEmployee);

            FullyQualifiedNamespaceManager.AddStructuralProperty("NumberOfReports", EdmCoreModel.Instance.GetInt32(true));
            var FullyQualifiedNamespaceManager_DirectReports = FullyQualifiedNamespaceManager.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "DirectReports", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespaceEmployee });
            model.AddElement(FullyQualifiedNamespaceManager);

            model.AddElement(FullyQualifiedNamespaceOpenEmployee);

            var FullyQualifiedNamespaceDog_ID = FullyQualifiedNamespaceDog.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            FullyQualifiedNamespaceDog.AddStructuralProperty("Color", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespaceDog.AddStructuralProperty("Nicknames", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            FullyQualifiedNamespaceDog.AddStructuralProperty("Breed", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespaceDog.AddStructuralProperty("NamedStream", EdmCoreModel.Instance.GetStream(true));
            FullyQualifiedNamespaceDog.AddKeys(new IEdmStructuralProperty[] { FullyQualifiedNamespaceDog_ID, });
            var FullyQualifiedNamespaceDog_MyPeople = FullyQualifiedNamespaceDog.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "MyPeople", TargetMultiplicity = EdmMultiplicity.Many, Target = FullyQualifiedNamespacePerson });
            var FullyQualifiedNamespaceDog_FastestOwner = FullyQualifiedNamespaceDog.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "FastestOwner", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = FullyQualifiedNamespacePerson });
            var FullyQualifiedNamespaceDog_LionWhoAteMe = FullyQualifiedNamespaceDog.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "LionWhoAteMe",
                    TargetMultiplicity = EdmMultiplicity.One,
                    Target = FullyQualifiedNamespaceLion,
                },
                new EdmNavigationPropertyInfo()
                {
                    Name = "DogThatIAte",
                    TargetMultiplicity = EdmMultiplicity.One,
                    Target = FullyQualifiedNamespaceDog,
                    DependentProperties = new List<IEdmStructuralProperty>()
                    { 
                        FullyQualifiedNamespaceLion_ID1
                    },
                    PrincipalProperties = new List<IEdmStructuralProperty>()
                    {
                        FullyQualifiedNamespaceDog_ID
                    }
                });
            var FullyQualifiedNamespaceDog_LionsISaw = FullyQualifiedNamespaceDog.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "LionsISaw",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = FullyQualifiedNamespaceLion,
                },
                new EdmNavigationPropertyInfo()
                {
                    Name = "DogsSeenMe",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = FullyQualifiedNamespaceDog,
                    DependentProperties = new List<IEdmStructuralProperty>()
                    { 
                        FullyQualifiedNamespaceLion_ID1
                    },
                    PrincipalProperties = new List<IEdmStructuralProperty>()
                    {
                        FullyQualifiedNamespaceDog_ID
                    }
                });
            var FullyQualifiedNamespaceLion_Friends = FullyQualifiedNamespaceLion.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "Friends",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = FullyQualifiedNamespaceLion,
                    DependentProperties = new List<IEdmStructuralProperty>()
                    { 
                        FullyQualifiedNamespaceLion_ID2
                    },
                    PrincipalProperties = new List<IEdmStructuralProperty>()
                    {
                        FullyQualifiedNamespaceLion_ID1
                    }
                });
            model.AddElement(FullyQualifiedNamespaceDog);

            var fullyQualifiedNamespaceChimeraKey1 = FullyQualifiedNamespaceChimera.AddStructuralProperty("Rid", EdmCoreModel.Instance.GetInt32(false));
            var fullyQualifiedNamespaceChimeraKey2 = FullyQualifiedNamespaceChimera.AddStructuralProperty("Gid", EdmPrimitiveTypeKind.Guid);
            var fullyQualifiedNamespaceChimeraKey3 = FullyQualifiedNamespaceChimera.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var fullyQualifiedNamespaceChimeraKey4 = FullyQualifiedNamespaceChimera.AddStructuralProperty("Upgraded", EdmPrimitiveTypeKind.Boolean);
            FullyQualifiedNamespaceChimera.AddStructuralProperty("Level", EdmPrimitiveTypeKind.Int32);
            FullyQualifiedNamespaceChimera.AddKeys(new IEdmStructuralProperty[] { fullyQualifiedNamespaceChimeraKey1, fullyQualifiedNamespaceChimeraKey2, fullyQualifiedNamespaceChimeraKey3, fullyQualifiedNamespaceChimeraKey4 });
            model.AddElement(FullyQualifiedNamespaceChimera);

            var FullyQualifiedNamespacePainting_ID = FullyQualifiedNamespacePainting.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            FullyQualifiedNamespacePainting.AddStructuralProperty("Artist", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespacePainting.AddStructuralProperty("Value", EdmCoreModel.Instance.GetDecimal(true));
            FullyQualifiedNamespacePainting.AddStructuralProperty("Colors", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            FullyQualifiedNamespacePainting.AddKeys(new IEdmStructuralProperty[] { FullyQualifiedNamespacePainting_ID, });
            var FullyQualifiedNamespacePainting_Owner = FullyQualifiedNamespacePainting.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Owner", TargetMultiplicity = EdmMultiplicity.ZeroOrOne, Target = FullyQualifiedNamespacePerson });
            model.AddElement(FullyQualifiedNamespacePainting);

            FullyQualifiedNamespaceFramedPainting.AddStructuralProperty("FrameColor", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceFramedPainting);

            var FullyQualifiedNamespaceUserAccount_UserName = FullyQualifiedNamespaceUserAccount.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespaceUserAccount.AddKeys(new IEdmStructuralProperty[] { FullyQualifiedNamespaceUserAccount_UserName, });
            model.AddElement(FullyQualifiedNamespaceUserAccount);

            FullyQualifiedNamespaceAddress.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespaceAddress.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(true));
            FullyQualifiedNamespaceAddress.AddStructuralProperty("NextHome", FullyQualifiedNamespaceAddressTypeReference);
            FullyQualifiedNamespaceAddress.AddStructuralProperty("MyNeighbors", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(FullyQualifiedNamespaceAddress);

            FullyQualifiedNamespaceHomeAddress.AddStructuralProperty("HomeNO", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceHomeAddress);

            model.AddElement(FullyQualifiedNamespaceOpenAddress);

            FullyQualifiedNamespaceHeartbeat.AddStructuralProperty("Frequency", EdmCoreModel.Instance.GetDouble(true));
            model.AddElement(FullyQualifiedNamespaceHeartbeat);

            FullyQualifiedNamespacePet1.AddKeys(FullyQualifiedNamespacePet1.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int64, false));
            FullyQualifiedNamespacePet1.AddStructuralProperty("SingleID", EdmPrimitiveTypeKind.Single, false);
            FullyQualifiedNamespacePet1.AddStructuralProperty("DoubleID", EdmPrimitiveTypeKind.Double, false);
            FullyQualifiedNamespacePet1.AddStructuralProperty("DecimalID", EdmPrimitiveTypeKind.Decimal, false);
            FullyQualifiedNamespacePet1.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            model.AddElement(FullyQualifiedNamespacePet1);

            FullyQualifiedNamespacePet2.AddKeys(FullyQualifiedNamespacePet2.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Single, false));
            FullyQualifiedNamespacePet2.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            FullyQualifiedNamespacePet2.AddStructuralProperty("PetColorPattern", colorTypeReference);
            model.AddElement(FullyQualifiedNamespacePet2);

            FullyQualifiedNamespacePet3.AddKeys(FullyQualifiedNamespacePet3.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Double, false));
            FullyQualifiedNamespacePet3.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            model.AddElement(FullyQualifiedNamespacePet3);

            FullyQualifiedNamespacePet4.AddKeys(FullyQualifiedNamespacePet4.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Decimal, false));
            FullyQualifiedNamespacePet4.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            model.AddElement(FullyQualifiedNamespacePet4);

            FullyQualifiedNamespacePet5.AddKeys(FullyQualifiedNamespacePet5.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Boolean, false));
            FullyQualifiedNamespacePet5.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            model.AddElement(FullyQualifiedNamespacePet5);

            FullyQualifiedNamespacePet6.AddKeys(FullyQualifiedNamespacePet6.AddStructuralProperty("ID", FullyQualifiedNamespaceIdTypeReference));
            FullyQualifiedNamespacePet6.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
            model.AddElement(FullyQualifiedNamespacePet6);

            // entity type with enum as key
            var fullyQualifiedNamespaceShape = new EdmEntityType("Fully.Qualified.Namespace", "Shape", null, false, false);
            fullyQualifiedNamespaceShape.AddKeys(fullyQualifiedNamespaceShape.AddStructuralProperty("Color", colorTypeReference));
            #endregion

            #region Operations

            var FullyQualifiedNamespaceGetPet1Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet1", FullyQualifiedNamespacePet1TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet1Function.AddParameter("id", EdmCoreModel.Instance.GetInt64(false));
            model.AddElement(FullyQualifiedNamespaceGetPet1Function);

            var FullyQualifiedNamespaceGetPet2Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet2", FullyQualifiedNamespacePet2TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet2Function.AddParameter("id", EdmCoreModel.Instance.GetSingle(false));
            model.AddElement(FullyQualifiedNamespaceGetPet2Function);

            var FullyQualifiedNamespaceGetPet3Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet3", FullyQualifiedNamespacePet3TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet3Function.AddParameter("id", EdmCoreModel.Instance.GetDouble(false));
            model.AddElement(FullyQualifiedNamespaceGetPet3Function);

            var FullyQualifiedNamespaceGetPet4Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet4", FullyQualifiedNamespacePet4TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet4Function.AddParameter("id", EdmCoreModel.Instance.GetDecimal(false));
            model.AddElement(FullyQualifiedNamespaceGetPet4Function);

            var FullyQualifiedNamespaceGetPet5Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet5", FullyQualifiedNamespacePet5TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet5Function.AddParameter("id", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(FullyQualifiedNamespaceGetPet5Function);

            var FullyQualifiedNamespaceGetPet6Function = new EdmFunction("Fully.Qualified.Namespace", "GetPet6", FullyQualifiedNamespacePet6TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPet6Function.AddParameter("id", FullyQualifiedNamespaceIdTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetPet6Function);

            var FullyQualifiedNamespaceGetPetCountFunction = new EdmFunction("Fully.Qualified.Namespace", "GetPetCount", FullyQualifiedNamespacePet5TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPetCountFunction.AddParameter("colorPattern", colorTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetPetCountFunction);

            var FullyQualifiedNamespaceTryGetPetCountFunction = new EdmFunction("Fully.Qualified.Namespace", "TryGetPetCount", FullyQualifiedNamespacePet5TypeReference, false, null, true);
            FullyQualifiedNamespaceGetPetCountFunction.AddParameter("colorPattern", nullableColorTypeReference);
            model.AddElement(FullyQualifiedNamespaceTryGetPetCountFunction);

            var FullyQualifiedNamespaceWalkAction = new EdmAction("Fully.Qualified.Namespace", "Walk", FullyQualifiedNamespaceAddressTypeReference, true, null);
            FullyQualifiedNamespaceWalkAction.AddParameter("dog", FullyQualifiedNamespaceDogTypeReference);
            model.AddElement(FullyQualifiedNamespaceWalkAction);

            var FullyQualifiedNamespaceFindMyOwnerFunction = new EdmFunction("Fully.Qualified.Namespace", "FindMyOwner", FullyQualifiedNamespacePersonTypeReference, false, null, false);
            FullyQualifiedNamespaceFindMyOwnerFunction.AddParameter("dogsName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceFindMyOwnerFunction);

            var FullyQualifiedNamespaceHasHatFunction = new EdmFunction("Fully.Qualified.Namespace", "HasHat", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasHatFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceHasHatFunction);

            var FullyQualifiedNamespaceHasHatFunction2 = new EdmFunction("Fully.Qualified.Namespace", "HasHat", EdmCoreModel.Instance.GetInt32(true), true, null, false);
            FullyQualifiedNamespaceHasHatFunction2.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceHasHatFunction2.AddParameter("onCat", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceHasHatFunction2);

            var FullyQualifiedNamespaceHasJobFunction = new EdmFunction("Fully.Qualified.Namespace", "HasJob", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasJobFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceHasJobFunction);

            var FullyQualifiedNamespaceAllHaveDogFunction = new EdmFunction("Fully.Qualified.Namespace", "AllHaveDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceAllHaveDogFunction.AddParameter("people", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespacePersonTypeReference)));
            model.AddElement(FullyQualifiedNamespaceAllHaveDogFunction);

            var FullyQualifiedNamespaceAllHaveDogFunction2 = new EdmFunction("Fully.Qualified.Namespace", "AllHaveDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceAllHaveDogFunction2.AddParameter("people", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespacePersonTypeReference)));
            FullyQualifiedNamespaceAllHaveDogFunction2.AddParameter("inOffice", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceAllHaveDogFunction2);

            var FullyQualifiedNamespaceFireAllAction = new EdmAction("Fully.Qualified.Namespace", "FireAll", EdmCoreModel.Instance.GetBoolean(true), true, null);
            FullyQualifiedNamespaceFireAllAction.AddParameter("employees", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespacePersonTypeReference)));
            model.AddElement(FullyQualifiedNamespaceFireAllAction);

            var FullyQualifiedNamespaceHasDogFunction = new EdmFunction("Fully.Qualified.Namespace", "HasDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasDogFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceHasDogFunction);

            var FullyQualifiedNamespaceHasDogFunction2 = new EdmFunction("Fully.Qualified.Namespace", "HasDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasDogFunction2.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceHasDogFunction2.AddParameter("inOffice", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceHasDogFunction2);

            var FullyQualifiedNamespaceHasDogFunction3 = new EdmFunction("Fully.Qualified.Namespace", "HasDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasDogFunction3.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceHasDogFunction3.AddParameter("inOffice", EdmCoreModel.Instance.GetBoolean(true));
            FullyQualifiedNamespaceHasDogFunction3.AddParameter("name", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceHasDogFunction3);

            var FullyQualifiedNamespaceHasDogFunction4 = new EdmFunction("Fully.Qualified.Namespace", "HasDog", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceHasDogFunction4.AddParameter("person", FullyQualifiedNamespaceEmployeeTypeReference);
            FullyQualifiedNamespaceHasDogFunction4.AddParameter("inOffice", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceHasDogFunction4);

            var FullyQualifiedNamespaceGetMyDogFunction = new EdmFunction("Fully.Qualified.Namespace", "GetMyDog", FullyQualifiedNamespaceDogTypeReference, true, new EdmPathExpression("person/MyDog"), true);
            FullyQualifiedNamespaceGetMyDogFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetMyDogFunction);

            var FullyQualifiedNamespaceAllMyFriendsDogsFunction = new EdmFunction("Fully.Qualified.Namespace", "AllMyFriendsDogs", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceDogTypeReference)), true, new EdmPathExpression("person/MyFriendsDogs"), true);
            FullyQualifiedNamespaceAllMyFriendsDogsFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceAllMyFriendsDogsFunction);

            var FullyQualifiedNamespaceAllMyFriendsDogsNonComposableFunction = new EdmFunction("Fully.Qualified.Namespace", "AllMyFriendsDogsNonComposable", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceDogTypeReference)), true, new EdmPathExpression("person/MyFriendsDogs"), false);
            FullyQualifiedNamespaceAllMyFriendsDogsNonComposableFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceAllMyFriendsDogsNonComposableFunction);

            var FullyQualifiedNamespaceAllMyFriendsDogsFunction2 = new EdmFunction("Fully.Qualified.Namespace", "AllMyFriendsDogs", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceDogTypeReference)), true, new EdmPathExpression("person/MyFriendsDogs"), true);
            FullyQualifiedNamespaceAllMyFriendsDogsFunction2.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceAllMyFriendsDogsFunction2.AddParameter("inOffice", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceAllMyFriendsDogsFunction2);

            var FullyQualifiedNamespaceAllMyFriendsDogs_NoSetFunction = new EdmFunction("Fully.Qualified.Namespace", "AllMyFriendsDogs_NoSet", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceDogTypeReference)), true, null, false);
            FullyQualifiedNamespaceAllMyFriendsDogs_NoSetFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceAllMyFriendsDogs_NoSetFunction);

            var FullyQualifiedNamespaceOwnerOfFastestDogFunction = new EdmFunction("Fully.Qualified.Namespace", "OwnerOfFastestDog", FullyQualifiedNamespacePersonTypeReference, true, new EdmPathExpression("dogs/FastestOwner"), true);
            FullyQualifiedNamespaceOwnerOfFastestDogFunction.AddParameter("dogs", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceDogTypeReference)));
            model.AddElement(FullyQualifiedNamespaceOwnerOfFastestDogFunction);

            var FullyQualifiedNamespaceOwnsTheseDogsFunction = new EdmFunction("Fully.Qualified.Namespace", "OwnsTheseDogs", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceOwnsTheseDogsFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceOwnsTheseDogsFunction.AddParameter("dogNames", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(FullyQualifiedNamespaceOwnsTheseDogsFunction);

            var FullyQualifiedNamespaceIsInTheUSFunction = new EdmFunction("Fully.Qualified.Namespace", "IsInTheUS", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceIsInTheUSFunction.AddParameter("address", FullyQualifiedNamespaceAddressTypeReference);
            model.AddElement(FullyQualifiedNamespaceIsInTheUSFunction);

            var FullyQualifiedNamespaceMoveAction = new EdmAction("Fully.Qualified.Namespace", "Move", EdmCoreModel.Instance.GetBoolean(true), true, null);
            FullyQualifiedNamespaceMoveAction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceMoveAction.AddParameter("streetAddress", EdmCoreModel.Instance.GetString(true));

            var FullyQualifiedNamespaceCanMoveToAddressFunction = new EdmFunction("Fully.Qualified.Namespace", "CanMoveToAddress", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceCanMoveToAddressFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceCanMoveToAddressFunction.AddParameter("address", FullyQualifiedNamespaceAddressTypeReference);
            model.AddElement(FullyQualifiedNamespaceCanMoveToAddressFunction);

            var FullyQualifiedNamespaceIsAddressGoodFunction = new EdmFunction("Fully.Qualified.Namespace", "IsAddressGood", EdmCoreModel.Instance.GetBoolean(true), false, null, false);
            FullyQualifiedNamespaceIsAddressGoodFunction.AddParameter("address", FullyQualifiedNamespaceAddressTypeReference);
            model.AddElement(FullyQualifiedNamespaceIsAddressGoodFunction);

            var FullyQualifiedNamespaceCanMoveToAddressesFunction = new EdmFunction("Fully.Qualified.Namespace", "CanMoveToAddresses", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceCanMoveToAddressesFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceCanMoveToAddressesFunction.AddParameter("addresses", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceAddressTypeReference)));
            model.AddElement(FullyQualifiedNamespaceCanMoveToAddressesFunction);

            var FullyQualifiedNamespaceIsOlderThanByteFunction = new EdmFunction("Fully.Qualified.Namespace", "IsOlderThanByte", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceIsOlderThanByteFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceIsOlderThanByteFunction.AddParameter("age", EdmCoreModel.Instance.GetByte(true));
            model.AddElement(FullyQualifiedNamespaceIsOlderThanByteFunction);

            var FullyQualifiedNamespaceIsOlderThanSByteFunction = new EdmFunction("Fully.Qualified.Namespace", "IsOlderThanSByte", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceIsOlderThanSByteFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceIsOlderThanSByteFunction.AddParameter("age", EdmCoreModel.Instance.GetSByte(true));
            model.AddElement(FullyQualifiedNamespaceIsOlderThanSByteFunction);

            var FullyQualifiedNamespaceIsOlderThanShortFunction = new EdmFunction("Fully.Qualified.Namespace", "IsOlderThanShort", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceIsOlderThanShortFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceIsOlderThanShortFunction.AddParameter("age", EdmCoreModel.Instance.GetInt16(true));
            model.AddElement(FullyQualifiedNamespaceIsOlderThanShortFunction);

            var FullyQualifiedNamespaceIsOlderThanSingleFunction = new EdmFunction("Fully.Qualified.Namespace", "IsOlderThanSingle", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            FullyQualifiedNamespaceIsOlderThanSingleFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceIsOlderThanSingleFunction.AddParameter("age", EdmCoreModel.Instance.GetSingle(true));
            model.AddElement(FullyQualifiedNamespaceIsOlderThanSingleFunction);

            var FullyQualifiedNamespacePaintAction = new EdmAction("Fully.Qualified.Namespace", "Paint", FullyQualifiedNamespacePaintingTypeReference, true, new EdmPathExpression("person/MyPaintings"));
            FullyQualifiedNamespacePaintAction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespacePaintAction);

            var FullyQualifiedNamespaceMoveAction2 = new EdmAction("Fully.Qualified.Namespace", "Move", EdmCoreModel.Instance.GetBoolean(true), true, null);
            FullyQualifiedNamespaceMoveAction2.AddParameter("employee", FullyQualifiedNamespaceEmployeeTypeReference);
            FullyQualifiedNamespaceMoveAction2.AddParameter("building", EdmCoreModel.Instance.GetInt32(true));
            FullyQualifiedNamespaceMoveAction2.AddParameter("room", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(FullyQualifiedNamespaceMoveAction2);

            var FullyQualifiedNamespaceRestoreAction = new EdmAction("Fully.Qualified.Namespace", "Restore", EdmCoreModel.Instance.GetBoolean(true), true, null);
            FullyQualifiedNamespaceRestoreAction.AddParameter("painting", FullyQualifiedNamespacePaintingTypeReference);
            model.AddElement(FullyQualifiedNamespaceRestoreAction);

            var FullyQualifiedNamespaceChangeStateAction = new EdmAction("Fully.Qualified.Namespace", "ChangeState", EdmCoreModel.Instance.GetBoolean(true), true, null);
            FullyQualifiedNamespaceChangeStateAction.AddParameter("address", FullyQualifiedNamespaceAddressTypeReference);
            FullyQualifiedNamespaceChangeStateAction.AddParameter("newState", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceChangeStateAction);

            var FullyQualifiedNamespaceGetMyPersonFunction = new EdmFunction("Fully.Qualified.Namespace", "GetMyPerson", FullyQualifiedNamespacePersonTypeReference, true, null, false);
            FullyQualifiedNamespaceGetMyPersonFunction.AddParameter("dog", FullyQualifiedNamespaceDogTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetMyPersonFunction);
            model.AddElement(FullyQualifiedNamespaceMoveAction);

            var FullyQualifiedNamespaceGetCoolPeopleAction = new EdmFunction("Fully.Qualified.Namespace", "GetCoolPeople", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespacePersonTypeReference)), false, null, true /*isComposable*/);
            FullyQualifiedNamespaceGetCoolPeopleAction.AddParameter("id", EdmCoreModel.Instance.GetInt32(true));
            FullyQualifiedNamespaceGetCoolPeopleAction.AddParameter("limit", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(FullyQualifiedNamespaceGetCoolPeopleAction);

            var FullyQualifiedNamespaceGetHotPeopleAction = new EdmFunction("Fully.Qualified.Namespace", "GetHotPeople", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespacePersonTypeReference)), true, new EdmPathExpression("person"), true /*isComposable*/);
            FullyQualifiedNamespaceGetHotPeopleAction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceGetHotPeopleAction.AddParameter("limit", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(FullyQualifiedNamespaceGetHotPeopleAction);

            var FullyQualifiedNamespaceGetCoolestPersonAction = new EdmFunction("Fully.Qualified.Namespace", "GetCoolestPerson", FullyQualifiedNamespacePersonTypeReference, false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetCoolestPersonAction);

            var FullyQualifiedNamespaceGetCoolestPersonWithStyleAction = new EdmFunction("Fully.Qualified.Namespace", "GetCoolestPersonWithStyle", FullyQualifiedNamespacePersonTypeReference, false, null, true /*isComposable*/);
            FullyQualifiedNamespaceGetCoolestPersonWithStyleAction.AddParameter("styleID", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(FullyQualifiedNamespaceGetCoolestPersonWithStyleAction);

            var FullyQualifiedNamespaceGetBestManagerAction = new EdmFunction("Fully.Qualified.Namespace", "GetBestManager", FullyQualifiedNamespaceManagerTypeReference, false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetBestManagerAction);

            var FullyQualifiedNamespaceGetNothingAction = new EdmAction("Fully.Qualified.Namespace", "GetNothing", null, false, null);
            model.AddElement(FullyQualifiedNamespaceGetNothingAction);

            var FullyQualifiedNamespaceGetSomeNumberAction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeNumber", EdmCoreModel.Instance.GetInt32(true), false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeNumberAction);

            var FullyQualifiedNamespaceGetSomeAddressAction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeAddress", FullyQualifiedNamespaceAddressTypeReference, false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeAddressAction);

            var FullyQualifiedNamespaceGetSomeNumbersAction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))), false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeNumbersAction);

            var FullyQualifiedNamespaceGetSomeColorsFunction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeColors", new EdmCollectionTypeReference(new EdmCollectionType(colorTypeReference)), false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeColorsFunction);

            var FullyQualifiedNamespaceGetSomeColorFunction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeColor", colorTypeReference, false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeColorFunction);

            var FullyQualifiedNamespaceGetSomeAddressesAction = new EdmFunction("Fully.Qualified.Namespace", "GetSomeAddresses", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceAddressTypeReference)), false, null, true /*isComposable*/);
            model.AddElement(FullyQualifiedNamespaceGetSomeAddressesAction);

            var FullyQualifiedNamespaceResetAllDataAction = new EdmAction("Fully.Qualified.Namespace", "ResetAllData", null, false, null);
            model.AddElement(FullyQualifiedNamespaceResetAllDataAction);

            var FullyQualifiedNamespaceGetMostImporantPersonFunction = new EdmFunction("Fully.Qualified.Namespace", "GetMostImporantPerson", FullyQualifiedNamespacePersonTypeReference, false, null, false);
            model.AddElement(FullyQualifiedNamespaceGetMostImporantPersonFunction);

            var FullyQualifiedNamespaceGetMostImporantPersonFunction2 = new EdmFunction("Fully.Qualified.Namespace", "GetMostImporantPerson", FullyQualifiedNamespacePersonTypeReference, false, null, false);
            FullyQualifiedNamespaceGetMostImporantPersonFunction2.AddParameter("city", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceGetMostImporantPersonFunction2);

            var FullyQualifiedNamespaceGetPriorNumbersFunction = new EdmFunction("Fully.Qualified.Namespace", "GetPriorNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))), true, null, true);
            FullyQualifiedNamespaceGetPriorNumbersFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetPriorNumbersFunction);

            var FullyQualifiedNamespaceGetPriorAddressesFunction = new EdmFunction("Fully.Qualified.Namespace", "GetPriorAddresses", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceAddressTypeReference)), true, null, true);
            FullyQualifiedNamespaceGetPriorAddressesFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetPriorAddressesFunction);

            var FullyQualifiedNamespaceGetPriorAddressFunction = new EdmFunction("Fully.Qualified.Namespace", "GetPriorAddress", FullyQualifiedNamespaceAddressTypeReference, true, null, true);
            FullyQualifiedNamespaceGetPriorAddressFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetPriorAddressFunction);

            var FullyQualifiedNamespaceGetFavoriteColorsFunction = new EdmFunction("Fully.Qualified.Namespace", "GetFavoriteColors", new EdmCollectionTypeReference(new EdmCollectionType(colorTypeReference)), true, null, true);
            FullyQualifiedNamespaceGetFavoriteColorsFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetFavoriteColorsFunction);

            var FullyQualifiedNamespaceGetFavoriteColorFunction = new EdmFunction("Fully.Qualified.Namespace", "GetFavoriteColor", colorTypeReference, true, null, true);
            FullyQualifiedNamespaceGetFavoriteColorFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetFavoriteColorFunction);

            var FullyQualifiedNamespaceGetNearbyPriorAddressesFunction = new EdmFunction("Fully.Qualified.Namespace", "GetNearbyPriorAddresses", new EdmCollectionTypeReference(new EdmCollectionType(FullyQualifiedNamespaceAddressTypeReference)), true, null, false);
            FullyQualifiedNamespaceGetNearbyPriorAddressesFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceGetNearbyPriorAddressesFunction.AddParameter("currentLocation", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
            FullyQualifiedNamespaceGetNearbyPriorAddressesFunction.AddParameter("limit", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(FullyQualifiedNamespaceGetNearbyPriorAddressesFunction);

            var FullyQualifiedNamespaceGetColorAtPositionFunction = new EdmFunction("Fully.Qualified.Namespace", "GetColorAtPosition", EdmCoreModel.Instance.GetString(true), true, null, false);
            FullyQualifiedNamespaceGetColorAtPositionFunction.AddParameter("painting", FullyQualifiedNamespacePaintingTypeReference);
            FullyQualifiedNamespaceGetColorAtPositionFunction.AddParameter("position", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true));
            FullyQualifiedNamespaceGetColorAtPositionFunction.AddParameter("includeAlpha", EdmCoreModel.Instance.GetBoolean(true));
            model.AddElement(FullyQualifiedNamespaceGetColorAtPositionFunction);

            var FullyQualifiedNamespaceMoveEveryoneAction = new EdmAction("Fully.Qualified.Namespace", "MoveEveryone", EdmCoreModel.Instance.GetBoolean(true), false, null);
            FullyQualifiedNamespaceMoveEveryoneAction.AddParameter("streetAddress", EdmCoreModel.Instance.GetString(true));
            model.AddElement(FullyQualifiedNamespaceMoveEveryoneAction);

            var FullyQualifiedNamespaceGetFullNameFunction = new EdmFunction("Fully.Qualified.Namespace", "GetFullName", FullyQualifiedNamespaceNameTypeReference, true, null, true);
            FullyQualifiedNamespaceGetNearbyPriorAddressesFunction.AddParameter("person", FullyQualifiedNamespacePersonTypeReference);
            FullyQualifiedNamespaceGetFullNameFunction.AddParameter("nickname", FullyQualifiedNamespaceNameTypeReference);
            model.AddElement(FullyQualifiedNamespaceGetFullNameFunction);

            #endregion

            #region Context Container

            var FullyQualifiedNamespaceContext = new EdmEntityContainer("Fully.Qualified.Namespace", "Context");
            model.AddElement(FullyQualifiedNamespaceContext);

            var FullyQualifiedNamespaceContextPeople = FullyQualifiedNamespaceContext.AddEntitySet("People", FullyQualifiedNamespacePerson);
            var FullyQualifiedNamespaceContextDogs = FullyQualifiedNamespaceContext.AddEntitySet("Dogs", FullyQualifiedNamespaceDog);
            var FullyQualifiedNamespaceContextLions = FullyQualifiedNamespaceContext.AddEntitySet("Lions", FullyQualifiedNamespaceLion);
            var FullyQualifiedNamespaceContextPaintings = FullyQualifiedNamespaceContext.AddEntitySet("Paintings", FullyQualifiedNamespacePainting);
            var FullyQualifiedNamespaceContextUsers = FullyQualifiedNamespaceContext.AddEntitySet("Users", FullyQualifiedNamespaceUserAccount);
            var FullyQualifiedNamespaceContextPet1Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet1Set", FullyQualifiedNamespacePet1);
            var FullyQualifiedNamespaceContextPet2Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet2Set", FullyQualifiedNamespacePet2);
            var FullyQualifiedNamespaceContextPet3Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet3Set", FullyQualifiedNamespacePet3);
            var FullyQualifiedNamespaceContextPet4Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet4Set", FullyQualifiedNamespacePet4);
            var FullyQualifiedNamespaceContextPet5Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet5Set", FullyQualifiedNamespacePet5);
            var FullyQualifiedNamespaceContextPet6Set = FullyQualifiedNamespaceContext.AddEntitySet("Pet6Set", FullyQualifiedNamespacePet6);
            var FullyQualifiedNamespaceContextChimera = FullyQualifiedNamespaceContext.AddEntitySet("Chimeras", FullyQualifiedNamespaceChimera);

            FullyQualifiedNamespaceContext.AddEntitySet("Shapes", fullyQualifiedNamespaceShape);

            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyDog, FullyQualifiedNamespaceContextDogs);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyRelatedDogs, FullyQualifiedNamespaceContextDogs);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyLions, FullyQualifiedNamespaceContextLions);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyPaintings, FullyQualifiedNamespaceContextPaintings);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyFavoritePainting, FullyQualifiedNamespaceContextPaintings);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespaceEmployee_PaintingsInOffice, FullyQualifiedNamespaceContextPaintings);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespaceEmployee_Manager, FullyQualifiedNamespaceContextPeople);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespaceManager_DirectReports, FullyQualifiedNamespaceContextPeople);
            FullyQualifiedNamespaceContextPeople.AddNavigationTarget(FullyQualifiedNamespacePerson_MyPet2Set, FullyQualifiedNamespaceContextPet2Set);
            FullyQualifiedNamespaceContextDogs.AddNavigationTarget(FullyQualifiedNamespaceDog_MyPeople, FullyQualifiedNamespaceContextPeople);
            FullyQualifiedNamespaceContextDogs.AddNavigationTarget(FullyQualifiedNamespaceDog_FastestOwner, FullyQualifiedNamespaceContextPeople);
            FullyQualifiedNamespaceContextDogs.AddNavigationTarget(FullyQualifiedNamespaceDog_LionsISaw, FullyQualifiedNamespaceContextLions);
            FullyQualifiedNamespaceContextLions.AddNavigationTarget(FullyQualifiedNamespaceLion_Friends, FullyQualifiedNamespaceContextLions);
            FullyQualifiedNamespaceContextPaintings.AddNavigationTarget(FullyQualifiedNamespacePainting_Owner, FullyQualifiedNamespaceContextPeople);

            // Add singleton
            var FullQualifiedNamespaceSingletonBoss = FullyQualifiedNamespaceContext.AddSingleton("Boss", FullyQualifiedNamespacePerson);
            FullQualifiedNamespaceSingletonBoss.AddNavigationTarget(FullyQualifiedNamespacePerson_MyDog, FullyQualifiedNamespaceContextDogs);
            FullQualifiedNamespaceSingletonBoss.AddNavigationTarget(FullyQualifiedNamespacePerson_MyPaintings, FullyQualifiedNamespaceContextPaintings);
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet1", FullyQualifiedNamespaceGetPet1Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet1Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet2", FullyQualifiedNamespaceGetPet2Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet2Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet3", FullyQualifiedNamespaceGetPet3Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet3Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet4", FullyQualifiedNamespaceGetPet4Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet4Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet5", FullyQualifiedNamespaceGetPet5Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet5Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPet6", FullyQualifiedNamespaceGetPet6Function, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet6Set));
            FullyQualifiedNamespaceContext.AddFunctionImport("GetPetCount", FullyQualifiedNamespaceGetPetCountFunction, new EdmEntitySetReferenceExpression(FullyQualifiedNamespaceContextPet5Set));

            FullyQualifiedNamespaceContext.AddFunctionImport("FindMyOwner", FullyQualifiedNamespaceFindMyOwnerFunction, new EdmEntitySetReferenceExpression(model.FindEntityContainer("Fully.Qualified.Namespace.Context").FindEntitySet("People")));

            FullyQualifiedNamespaceContext.AddFunctionImport("IsAddressGood", FullyQualifiedNamespaceIsAddressGoodFunction, null);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetCoolPeople", FullyQualifiedNamespaceGetCoolPeopleAction, new EdmEntitySetReferenceExpression(model.FindEntityContainer("Fully.Qualified.Namespace.Context").FindEntitySet("People")));

            FullyQualifiedNamespaceContext.AddFunctionImport("GetCoolestPerson", FullyQualifiedNamespaceGetCoolestPersonAction, new EdmEntitySetReferenceExpression(model.FindEntityContainer("Fully.Qualified.Namespace.Context").FindEntitySet("People")));

            FullyQualifiedNamespaceContext.AddFunctionImport("GetCoolestPersonWithStyle", FullyQualifiedNamespaceGetCoolestPersonWithStyleAction, new EdmEntitySetReferenceExpression(model.FindEntityContainer("Fully.Qualified.Namespace.Context").FindEntitySet("People")));

            FullyQualifiedNamespaceContext.AddFunctionImport("GetBestManager", FullyQualifiedNamespaceGetBestManagerAction, new EdmEntitySetReferenceExpression(model.FindEntityContainer("Fully.Qualified.Namespace.Context").FindEntitySet("People")));

            FullyQualifiedNamespaceContext.AddActionImport("GetNothing", FullyQualifiedNamespaceGetNothingAction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetSomeNumber", FullyQualifiedNamespaceGetSomeNumberAction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetSomeAddress", FullyQualifiedNamespaceGetSomeAddressAction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetSomeNumbers", FullyQualifiedNamespaceGetSomeNumbersAction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetSomeAddresses", FullyQualifiedNamespaceGetSomeAddressesAction);

            FullyQualifiedNamespaceContext.AddActionImport("ResetAllData", FullyQualifiedNamespaceResetAllDataAction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetMostImporantPerson", FullyQualifiedNamespaceGetMostImporantPersonFunction);

            FullyQualifiedNamespaceContext.AddFunctionImport("GetMostImporantPerson", FullyQualifiedNamespaceGetMostImporantPersonFunction2);

            FullyQualifiedNamespaceContext.AddActionImport("MoveEveryone", FullyQualifiedNamespaceMoveEveryoneAction);

            #endregion

            try
            {
                // serialize edm
                XDocument document = new XDocument();
                IEnumerable<EdmError> errors;
                using (var writer = document.CreateWriter())
                {
                    EdmxWriter.TryWriteEdmx(model, writer, EdmxTarget.OData, out errors).Should().BeTrue();
                }

                string doc = document.ToString();

                // deserialize edm xml
                // TODO: remove the above model building codes.
                IEdmModel parsedModel;
                if (EdmxReader.TryParse(XmlReader.Create(new StringReader(HardCodedTestModelXml.MainModelXml)), (Uri uri) =>
                {
                    if (string.Equals(uri.AbsoluteUri, "http://submodel1/"))
                    {
                        return XmlReader.Create(new StringReader(HardCodedTestModelXml.SubModelXml1));
                    }
                    else if (string.Equals(uri.AbsoluteUri, "http://submodel2/"))
                    {
                        return XmlReader.Create(new StringReader(HardCodedTestModelXml.SubModelXml2));
                    }
                    else if (string.Equals(uri.AbsoluteUri, "http://submodel3/"))
                    {
                        return XmlReader.Create(new StringReader(HardCodedTestModelXml.SubModelXml3));
                    }
                    else if (string.Equals(uri.AbsoluteUri, "http://submodel4/"))
                    {
                        return XmlReader.Create(new StringReader(HardCodedTestModelXml.SubModelXml4));
                    }

                    throw new Exception("edmx:refernece must have a valid url." + uri.AbsoluteUri);
                }, out parsedModel, out errors))
                {
                   return parsedModel;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        internal class HardCodedTestModelXml
        {
            #region main model xml
            internal static string MainModelXml = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://submodel1"">
    <edmx:Include Namespace=""Fully.Qualified.Namespace"" Alias=""RefAlias1"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://submodel2"">
    <edmx:Include Namespace=""Fully.Qualified.Namespace"" Alias=""RefAlias2"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://submodel3"">
    <edmx:Include Namespace=""Fully.Qualified.Namespace"" Alias=""RefAlias3"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://submodel4"">
    <edmx:Include Namespace=""Fully.Qualified.Namespace"" Alias=""RefAlias4"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""MainAlias"">
      <Action Name=""MoveEveryone"">
        <Parameter Name=""streetAddress"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
      <EntityContainer Name=""Context"">
        <EntitySet Name=""People"" EntityType=""Fully.Qualified.Namespace.Person"">
          <NavigationPropertyBinding Path=""MyDog"" Target=""Dogs"" />
          <NavigationPropertyBinding Path=""MyFriendsDogs"" Target=""Dogs"" />
          <NavigationPropertyBinding Path=""MyLions"" Target=""Lions"" />
          <NavigationPropertyBinding Path=""MyPaintings"" Target=""Paintings"" />
          <NavigationPropertyBinding Path=""MyFavoritePainting"" Target=""Paintings"" />
          <NavigationPropertyBinding Path=""Fully.Qualified.Namespace.Employee/PaintingsInOffice"" Target=""Paintings"" />
          <NavigationPropertyBinding Path=""Fully.Qualified.Namespace.Employee/Manager"" Target=""People"" />
          <NavigationPropertyBinding Path=""Fully.Qualified.Namespace.Manager/DirectReports"" Target=""People"" />
          <NavigationPropertyBinding Path=""MyPet2Set"" Target=""Pet2Set"" />
        </EntitySet>
        <EntitySet Name=""Dogs"" EntityType=""Fully.Qualified.Namespace.Dog"">
          <NavigationPropertyBinding Path=""MyPeople"" Target=""People"" />
          <NavigationPropertyBinding Path=""FastestOwner"" Target=""People"" />
          <NavigationPropertyBinding Path=""LionsISaw"" Target=""Lions"" />
        </EntitySet>
        <EntitySet Name=""Shapes"" EntityType=""Fully.Qualified.Namespace.Shape"" />
        <EntitySet Name=""Lions"" EntityType=""Fully.Qualified.Namespace.Lion"">
          <NavigationPropertyBinding Path=""Friends"" Target=""Lions"" />
        </EntitySet>
        <EntitySet Name=""Paintings"" EntityType=""Fully.Qualified.Namespace.Painting"">
          <NavigationPropertyBinding Path=""Owner"" Target=""People"" />
        </EntitySet>
        <EntitySet Name=""PetCategories"" EntityType=""Fully.Qualified.Namespace.PetCategory"" />
        <EntitySet Name=""Users"" EntityType=""Fully.Qualified.Namespace.UserAccount"" />
        <EntitySet Name=""Pet1Set"" EntityType=""Fully.Qualified.Namespace.Pet1"" />
        <EntitySet Name=""Pet2Set"" EntityType=""Fully.Qualified.Namespace.Pet2"" />
        <EntitySet Name=""Pet3Set"" EntityType=""Fully.Qualified.Namespace.Pet3"" />
        <EntitySet Name=""Pet4Set"" EntityType=""Fully.Qualified.Namespace.Pet4"" />
        <EntitySet Name=""Pet5Set"" EntityType=""Fully.Qualified.Namespace.Pet5"" />
        <EntitySet Name=""Pet6Set"" EntityType=""Fully.Qualified.Namespace.Pet6"" />
        <EntitySet Name=""Chimeras"" EntityType=""Fully.Qualified.Namespace.Chimera"" />
        <Singleton Name=""Boss"" Type=""Fully.Qualified.Namespace.Person"">
          <NavigationPropertyBinding Path=""MyDog"" Target=""Dogs"" />
          <NavigationPropertyBinding Path=""MyPaintings"" Target=""Paintings"" />
        </Singleton>
        <FunctionImport Name=""GetPet1"" Function=""Fully.Qualified.Namespace.GetPet1"" EntitySet=""Pet1Set"" />
        <FunctionImport Name=""GetPet2"" Function=""Fully.Qualified.Namespace.GetPet2"" EntitySet=""Pet2Set"" />
        <FunctionImport Name=""GetPet3"" Function=""Fully.Qualified.Namespace.GetPet3"" EntitySet=""Pet3Set"" />
        <FunctionImport Name=""GetPet4"" Function=""Fully.Qualified.Namespace.GetPet4"" EntitySet=""Pet4Set"" />
        <FunctionImport Name=""GetPet5"" Function=""Fully.Qualified.Namespace.GetPet5"" EntitySet=""Pet5Set"" />
        <FunctionImport Name=""GetPet6"" Function=""Fully.Qualified.Namespace.GetPet6"" EntitySet=""Pet6Set"" />
        <FunctionImport Name=""GetPetCountNullable"" Function=""Fully.Qualified.Namespace.GetPetCountNullable"" EntitySet=""Pet5Set"" />
        <FunctionImport Name=""GetPetCount"" Function=""Fully.Qualified.Namespace.GetPetCount"" EntitySet=""Pet5Set"" />
        <FunctionImport Name=""TryGetPetCount"" Function=""Fully.Qualified.Namespace.TryGetPetCount"" EntitySet=""Pet5Set"" />
        <FunctionImport Name=""FindMyOwner"" Function=""Fully.Qualified.Namespace.FindMyOwner"" EntitySet=""People"" />
        <FunctionImport Name=""IsAddressGood"" Function=""Fully.Qualified.Namespace.IsAddressGood"" />
        <FunctionImport Name=""GetCoolPeople"" Function=""Fully.Qualified.Namespace.GetCoolPeople"" EntitySet=""People"" />
        <FunctionImport Name=""GetCoolestPerson"" Function=""Fully.Qualified.Namespace.GetCoolestPerson"" EntitySet=""People"" />
        <FunctionImport Name=""GetCoolestPersonWithStyle"" Function=""Fully.Qualified.Namespace.GetCoolestPersonWithStyle"" EntitySet=""People"" />
        <FunctionImport Name=""GetBestManager"" Function=""Fully.Qualified.Namespace.GetBestManager"" EntitySet=""People"" />
        <ActionImport Name=""GetNothing"" Action=""Fully.Qualified.Namespace.GetNothing"" />
        <FunctionImport Name=""GetSomeNumber"" Function=""Fully.Qualified.Namespace.GetSomeNumber"" />
        <FunctionImport Name=""GetSomeAddress"" Function=""Fully.Qualified.Namespace.GetSomeAddress"" />
        <FunctionImport Name=""GetSomeNumbers"" Function=""Fully.Qualified.Namespace.GetSomeNumbers"" />
        <FunctionImport Name=""GetSomeAddresses"" Function=""Fully.Qualified.Namespace.GetSomeAddresses"" />
        <FunctionImport Name=""GetSomeColors"" Function=""Fully.Qualified.Namespace.GetSomeColors"" />
        <FunctionImport Name=""GetSomeColor"" Function=""Fully.Qualified.Namespace.GetSomeColor"" />
        <ActionImport Name=""ResetAllData"" Action=""Fully.Qualified.Namespace.ResetAllData"" />
        <FunctionImport Name=""GetMostImporantPerson"" Function=""Fully.Qualified.Namespace.GetMostImporantPerson"" />
        <ActionImport Name=""MoveEveryone"" Action=""Fully.Qualified.Namespace.MoveEveryone"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            #endregion

            #region sub model xml1
            internal static string SubModelXml1 = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""SubAlias1"">
      <EnumType Name=""ColorPattern"" UnderlyingType=""Edm.Int64"" IsFlags=""true"">
        <Member Name=""Red"" Value=""1"" />
        <Member Name=""Blue"" Value=""2"" />
        <Member Name=""Yellow"" Value=""4"" />
        <Member Name=""Solid"" Value=""8"" />
        <Member Name=""Striped"" Value=""16"" />
        <Member Name=""SolidRed"" Value=""9"" />
        <Member Name=""SolidBlue"" Value=""10"" />
        <Member Name=""SolidYellow"" Value=""12"" />
        <Member Name=""RedBlueStriped"" Value=""19"" />
        <Member Name=""RedYellowStriped"" Value=""21"" />
        <Member Name=""BlueYellowStriped"" Value=""22"" />
      </EnumType>
      <EnumType Name=""NonFlagShape"" UnderlyingType=""Edm.SByte"">
        <Member Name=""Rectangle"" Value=""1"" />
        <Member Name=""Triangle"" Value=""2"" />
        <Member Name=""foursquare"" Value=""3"" />
      </EnumType>
      <EntityType Name=""Lion"">
        <Key>
          <PropertyRef Name=""ID1"" />
          <PropertyRef Name=""ID2"" />
        </Key>
        <Property Name=""ID1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ID2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""AngerLevel"" Type=""Edm.Double"" />
        <Property Name=""AttackDates"" Type=""Collection(Edm.DateTimeOffset)"" />
        <Property Name=""LionHeartbeat"" Type=""Fully.Qualified.Namespace.Heartbeat"" />
        <NavigationProperty Name=""DogThatIAte"" Type=""Fully.Qualified.Namespace.Dog"" Nullable=""false"" Partner=""LionWhoAteMe"">
          <ReferentialConstraint Property=""ID1"" ReferencedProperty=""ID"" />
        </NavigationProperty>
        <NavigationProperty Name=""DogsSeenMe"" Type=""Collection(Fully.Qualified.Namespace.Dog)"" Nullable=""false"" Partner=""LionsISaw"">
          <ReferentialConstraint Property=""ID1"" ReferencedProperty=""ID"" />
        </NavigationProperty>
        <NavigationProperty Name=""Friends"" Type=""Collection(Fully.Qualified.Namespace.Lion)"" Nullable=""false"">
          <ReferentialConstraint Property=""ID2"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
      </EntityType>
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""SSN"" Type=""Edm.String"" />
        <Property Name=""Shoe"" Type=""Edm.String"" />
        <Property Name=""GeographyPoint"" Type=""Edm.GeographyPoint"" SRID=""4326"" />
        <Property Name=""GeographyLineString"" Type=""Edm.GeographyLineString"" SRID=""4326"" />
        <Property Name=""GeographyPolygon"" Type=""Edm.GeographyPolygon"" SRID=""4326"" />
        <Property Name=""GeometryPoint"" Type=""Edm.GeometryPoint"" SRID=""0"" />
        <Property Name=""GeometryLineString"" Type=""Edm.GeometryLineString"" SRID=""0"" />
        <Property Name=""GeometryPolygon"" Type=""Edm.GeometryPolygon"" SRID=""0"" />
        <Property Name=""GeographyCollection"" Type=""Collection(Edm.GeographyPoint)"" SRID=""4326"" />
        <Property Name=""GeometryCollection"" Type=""Collection(Edm.GeometryPoint)"" SRID=""0"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""FirstName"" Type=""Fully.Qualified.Namespace.NameType"" />
        <Property Name=""Prop.With.Periods"" Type=""Edm.String"" />
        <Property Name=""MyDate"" Type=""Edm.Date"" Nullable=""false"" />
        <Property Name=""MyDates"" Type=""Collection(Edm.Date)"" />
        <Property Name=""MyTimeOfDay"" Type=""Edm.TimeOfDay"" Nullable=""false"" />
        <Property Name=""MyTimeOfDays"" Type=""Collection(Edm.TimeOfDay)"" />
        <Property Name=""Birthdate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Property Name=""FavoriteDate"" Type=""Edm.DateTimeOffset"" />
        <Property Name=""TimeEmployed"" Type=""Edm.Duration"" />
        <Property Name=""MyAddress"" Type=""Fully.Qualified.Namespace.Address"" />
        <Property Name=""MyOpenAddress"" Type=""Fully.Qualified.Namespace.OpenAddress"" />
        <Property Name=""PreviousAddresses"" Type=""Collection(Fully.Qualified.Namespace.Address)"" />
        <Property Name=""FavoriteColors"" Type=""Collection(Fully.Qualified.Namespace.ColorPattern)"" />
        <Property Name=""FavoriteNumber"" Type=""Fully.Qualified.Namespace.UInt16"" />
        <Property Name=""StockQuantity"" Type=""Fully.Qualified.Namespace.UInt32"" />
        <Property Name=""LifeTime"" Type=""Fully.Qualified.Namespace.UInt64"" />
        <NavigationProperty Name=""MyDog"" Type=""Fully.Qualified.Namespace.Dog"" />
        <NavigationProperty Name=""MyFriendsDogs"" Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
        <NavigationProperty Name=""MyPaintings"" Type=""Collection(Fully.Qualified.Namespace.Painting)"" />
        <NavigationProperty Name=""MyFavoritePainting"" Type=""Fully.Qualified.Namespace.Painting"" />
        <NavigationProperty Name=""MyLions"" Type=""Collection(Fully.Qualified.Namespace.Lion)"">
          <ReferentialConstraint Property=""ID"" ReferencedProperty=""ID1"" />
        </NavigationProperty>
        <NavigationProperty Name=""MyContainedDog"" Type=""Fully.Qualified.Namespace.Dog"" ContainsTarget=""true"" />
        <NavigationProperty Name=""MyContainedChimeras"" Type=""Collection(Fully.Qualified.Namespace.Chimera)"" ContainsTarget=""true"" />
        <NavigationProperty Name=""MyPet2Set"" Type=""Collection(Fully.Qualified.Namespace.Pet2)"" />
        <Annotation Term=""OData.Community.Keys.V1.AlternateKeys"">
          <Collection>
            <Record Type=""OData.Community.Keys.V1.AlternateKey"">
              <PropertyValue Property=""Key"">
                <Collection>
                  <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                    <PropertyValue Property=""Alias"" String=""SocialSN"" />
                    <PropertyValue Property=""Name"" PropertyPath=""SSN"" />
                  </Record>
                </Collection>
              </PropertyValue>
            </Record>
            <Record Type=""OData.Community.Keys.V1.AlternateKey"">
              <PropertyValue Property=""Key"">
                <Collection>
                  <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                    <PropertyValue Property=""Alias"" String=""NameAlias"" />
                    <PropertyValue Property=""Name"" PropertyPath=""Name"" />
                  </Record>
                  <Record Type=""OData.Community.Keys.V1.PropertyRef"">
                    <PropertyValue Property=""Alias"" String=""FirstNameAlias"" />
                    <PropertyValue Property=""Name"" PropertyPath=""FirstName"" />
                  </Record>
                </Collection>
              </PropertyValue>
            </Record>
          </Collection>
        </Annotation>
      </EntityType>
      <EntityType Name=""Employee"" BaseType=""Fully.Qualified.Namespace.Person"">
        <Property Name=""WorkEmail"" Type=""Edm.String"" />
        <NavigationProperty Name=""PaintingsInOffice"" Type=""Collection(Fully.Qualified.Namespace.Painting)"" />
        <NavigationProperty Name=""Manager"" Type=""Fully.Qualified.Namespace.Manager"" />
        <NavigationProperty Name=""OfficeDog"" Type=""Fully.Qualified.Namespace.Dog"" Nullable=""false"" Partner=""EmployeeOwner"" />
      </EntityType>
      <EntityType Name=""Manager"" BaseType=""Fully.Qualified.Namespace.Employee"">
        <Property Name=""NumberOfReports"" Type=""Edm.Int32"" />
        <NavigationProperty Name=""DirectReports"" Type=""Collection(Fully.Qualified.Namespace.Employee)"" />
      </EntityType>
      <EntityType Name=""OpenEmployee"" BaseType=""Fully.Qualified.Namespace.Employee"" OpenType=""true"" />
      <EntityType Name=""Dog"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
        <Property Name=""Nicknames"" Type=""Collection(Edm.String)"" />
        <Property Name=""Breed"" Type=""Edm.String"" />
        <Property Name=""NamedStream"" Type=""Edm.Stream"" />
        <NavigationProperty Name=""EmployeeOwner"" Type=""Fully.Qualified.Namespace.Employee"" Nullable=""false"" Partner=""OfficeDog"" />
        <NavigationProperty Name=""MyPeople"" Type=""Collection(Fully.Qualified.Namespace.Person)"" />
        <NavigationProperty Name=""FastestOwner"" Type=""Fully.Qualified.Namespace.Person"" />
        <NavigationProperty Name=""LionWhoAteMe"" Type=""Fully.Qualified.Namespace.Lion"" Nullable=""false"" Partner=""DogThatIAte"" />
        <NavigationProperty Name=""LionsISaw"" Type=""Collection(Fully.Qualified.Namespace.Lion)"" Nullable=""false"" Partner=""DogsSeenMe"" />
      </EntityType>
      <EntityType Name=""Shape"">
        <Key>
          <PropertyRef Name=""Color"" />
        </Key>
        <Property Name=""Color"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""false"" />
      </EntityType>
      <ComplexType Name=""Heartbeat"">
        <Property Name=""Frequency"" Type=""Edm.Double"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            #endregion

            #region sub model xml2
            internal static string SubModelXml2 = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""SubAlias2"">
      <TypeDefinition Name=""IdType"" UnderlyingType=""Edm.Double"" />
      <TypeDefinition Name=""NameType"" UnderlyingType=""Edm.String"" />
      <TypeDefinition Name=""UInt16"" UnderlyingType=""Edm.Int32"" />
      <TypeDefinition Name=""UInt32"" UnderlyingType=""Edm.Int64"" />
      <TypeDefinition Name=""UInt64"" UnderlyingType=""Edm.Decimal"" />
      <EntityType Name=""Chimera"" OpenType=""true"">
        <Key>
          <PropertyRef Name=""Rid"" />
          <PropertyRef Name=""Gid"" />
          <PropertyRef Name=""Name"" />
          <PropertyRef Name=""Upgraded"" />
        </Key>
        <Property Name=""Rid"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Gid"" Type=""Edm.Guid"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""Upgraded"" Type=""Edm.Boolean"" />
        <Property Name=""Level"" Type=""Edm.Int32"" />
      </EntityType>
      <EntityType Name=""Painting"" OpenType=""true"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Artist"" Type=""Edm.String"" />
        <Property Name=""Value"" Type=""Edm.Decimal"" />
        <Property Name=""Colors"" Type=""Collection(Edm.String)"" />
        <NavigationProperty Name=""Owner"" Type=""Fully.Qualified.Namespace.Person"" />
      </EntityType>
      <EntityType Name=""FramedPainting"" BaseType=""Fully.Qualified.Namespace.Painting"" OpenType=""true"">
        <Property Name=""FrameColor"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""UserAccount"">
        <Key>
          <PropertyRef Name=""UserName"" />
        </Key>
        <Property Name=""UserName"" Type=""Edm.String"" />
      </EntityType>
      <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" />
        <Property Name=""City"" Type=""Edm.String"" />
        <Property Name=""NextHome"" Type=""Fully.Qualified.Namespace.Address"" />
        <Property Name=""MyNeighbors"" Type=""Collection(Edm.String)"" />
      </ComplexType>
      <ComplexType Name=""HomeAddress"" BaseType=""Fully.Qualified.Namespace.Address"">
        <Property Name=""HomeNO"" Type=""Edm.String"" />
      </ComplexType>
      <ComplexType Name=""OpenAddress"" OpenType=""true"" />
      <EntityType Name=""Pet1"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int64"" Nullable=""false"" />
        <Property Name=""SingleID"" Type=""Edm.Single"" Nullable=""false"" />
        <Property Name=""DoubleID"" Type=""Edm.Double"" Nullable=""false"" />
        <Property Name=""DecimalID"" Type=""Edm.Decimal"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Pet2"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Single"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
        <Property Name=""PetColorPattern"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""Pet3"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Double"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Pet4"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Decimal"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Pet5"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Boolean"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Pet6"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Fully.Qualified.Namespace.IdType"" Nullable=""false"" />
        <Property Name=""Color"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""PetCategory"">
        <Key>
          <PropertyRef Name=""PetCategorysColorPattern"" />
        </Key>
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""PetCategorysColorPattern"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""false"" />
      </EntityType>
      <Function Name=""GetPet1"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Int64"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet1"" />
      </Function>
      <Function Name=""GetPet2"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Single"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet2"" />
      </Function>
      <Function Name=""GetPet3"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Double"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet3"" />
      </Function>
      <Function Name=""GetPet4"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Decimal"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet4"" />
      </Function>
      <Function Name=""GetPet5"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Boolean"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet5"" />
      </Function>
      <Function Name=""GetPet6"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Fully.Qualified.Namespace.IdType"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet6"" />
      </Function>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            #endregion

            #region sub model xml3
            internal static string SubModelXml3 = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""SubAlias3"">
            
      <Function Name=""GetPetCountNullable"" IsComposable=""true"">
        <Parameter Name=""colorPattern"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""true"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet5"" />
      </Function>
      <Function Name=""GetPetCount"" IsComposable=""true"">
        <Parameter Name=""colorPattern"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""false"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet5"" />
      </Function>
      <Function Name=""TryGetPetCount"" IsComposable=""true"">
        <Parameter Name=""colorPattern"" Type=""Fully.Qualified.Namespace.ColorPattern"" Nullable=""true"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Pet5"" />
      </Function>
      <Action Name=""Walk"" IsBound=""true"">
        <Parameter Name=""dog"" Type=""Fully.Qualified.Namespace.Dog"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Address"" />
      </Action>
      <Function Name=""FindMyOwner"">
        <Parameter Name=""dogsName"" Type=""Edm.String"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""HasHat"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""HasHat"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""onCat"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Edm.Int32"" />
      </Function>
      <Function Name=""HasJob"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""AllHaveDog"" IsBound=""true"">
        <Parameter Name=""people"" Type=""Collection(Fully.Qualified.Namespace.Person)"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""AllHaveDog"" IsBound=""true"">
        <Parameter Name=""people"" Type=""Collection(Fully.Qualified.Namespace.Person)"" />
        <Parameter Name=""inOffice"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Action Name=""FireAll"" IsBound=""true"">
        <Parameter Name=""employees"" Type=""Collection(Fully.Qualified.Namespace.Person)"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
      <Function Name=""HasDog"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""HasDog"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""inOffice"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""HasDog"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""inOffice"" Type=""Edm.Boolean"" />
        <Parameter Name=""name"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""HasDog"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Employee"" />
        <Parameter Name=""inOffice"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""GetMyDog"" IsBound=""true"" EntitySetPath=""person/MyDog"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Dog"" />
      </Function>
      <Function Name=""AllMyFriendsDogs"" IsBound=""true"" EntitySetPath=""person/MyFriendsDogs"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
      </Function>
      <Function Name=""AllMyFriendsDogsNonComposable"" IsBound=""true"" EntitySetPath=""person/MyFriendsDogs"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
      </Function>
      <Function Name=""AllMyFriendsDogs"" IsBound=""true"" EntitySetPath=""person/MyFriendsDogs"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""inOffice"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
      </Function>
      <Function Name=""AllMyFriendsDogs_NoSet"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
      </Function>
      <Function Name=""OwnerOfFastestDog"" IsBound=""true"" EntitySetPath=""dogs/FastestOwner"" IsComposable=""true"">
        <Parameter Name=""dogs"" Type=""Collection(Fully.Qualified.Namespace.Dog)"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""OwnsTheseDogs"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""dogNames"" Type=""Collection(Edm.String)"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsInTheUS"" IsBound=""true"">
        <Parameter Name=""address"" Type=""Fully.Qualified.Namespace.Address"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""CanMoveToAddress"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""address"" Type=""Fully.Qualified.Namespace.Address"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsAddressGood"">
        <Parameter Name=""address"" Type=""Fully.Qualified.Namespace.Address"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""CanMoveToAddresses"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""addresses"" Type=""Collection(Fully.Qualified.Namespace.Address)"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsOlderThanByte"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""age"" Type=""Edm.Byte"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsOlderThanSByte"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""age"" Type=""Edm.SByte"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsOlderThanShort"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""age"" Type=""Edm.Int16"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Function Name=""IsOlderThanSingle"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""age"" Type=""Edm.Single"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Function>
      <Action Name=""Paint"" IsBound=""true"" EntitySetPath=""person/MyPaintings"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Painting"" />
      </Action>
      <Action Name=""Move"" IsBound=""true"">
        <Parameter Name=""employee"" Type=""Fully.Qualified.Namespace.Employee"" />
        <Parameter Name=""building"" Type=""Edm.Int32"" />
        <Parameter Name=""room"" Type=""Edm.Int32"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            #endregion

            #region sub model xml4
            internal static string SubModelXml4 = @"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Fully.Qualified.Namespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""SubAlias4"">
      
      <Action Name=""Restore"" IsBound=""true"">
        <Parameter Name=""painting"" Type=""Fully.Qualified.Namespace.Painting"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
      <Action Name=""ChangeState"" IsBound=""true"">
        <Parameter Name=""address"" Type=""Fully.Qualified.Namespace.Address"" />
        <Parameter Name=""newState"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
      <Function Name=""GetMyPerson"" IsBound=""true"">
        <Parameter Name=""dog"" Type=""Fully.Qualified.Namespace.Dog"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Action Name=""Move"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""streetAddress"" Type=""Edm.String"" />
        <ReturnType Type=""Edm.Boolean"" />
      </Action>
      <Function Name=""GetCoolPeople"" IsComposable=""true"">
        <Parameter Name=""id"" Type=""Edm.Int32"" />
        <Parameter Name=""limit"" Type=""Edm.Int32"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Person)"" />
      </Function>
      <Function Name=""GetHotPeople"" IsBound=""true"" EntitySetPath=""person"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""limit"" Type=""Edm.Int32"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Person)"" />
      </Function>
      <Function Name=""GetCoolestPerson"" IsComposable=""true"">
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""GetCoolestPersonWithStyle"" IsComposable=""true"">
        <Parameter Name=""styleID"" Type=""Edm.Int32"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""GetBestManager"" IsComposable=""true"">
        <ReturnType Type=""Fully.Qualified.Namespace.Manager"" />
      </Function>
      <Action Name=""GetNothing"" />
      <Function Name=""GetSomeNumber"" IsComposable=""true"">
        <ReturnType Type=""Edm.Int32"" />
      </Function>
      <Function Name=""GetSomeAddress"" IsComposable=""true"">
        <ReturnType Type=""Fully.Qualified.Namespace.Address"" />
      </Function>
      <Function Name=""GetSomeNumbers"" IsComposable=""true"">
        <ReturnType Type=""Collection(Edm.Int32)"" />
      </Function>
      <Function Name=""GetSomeColors"" IsComposable=""true"">
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.ColorPattern)"" />
      </Function>
      <Function Name=""GetSomeColor"" IsComposable=""true"">
        <ReturnType Type=""Fully.Qualified.Namespace.ColorPattern"" />
      </Function>
      <Function Name=""GetSomeAddresses"" IsComposable=""true"">
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Address)"" />
      </Function>
      <Action Name=""ResetAllData"" />
      <Function Name=""GetMostImporantPerson"">
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""GetMostImporantPerson"">
        <Parameter Name=""city"" Type=""Edm.String"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Person"" />
      </Function>
      <Function Name=""GetPriorNumbers"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Edm.Int32)"" />
      </Function>
      <Function Name=""GetPriorAddresses"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Address)"" />
      </Function>
      <Function Name=""GetPriorAddress"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Fully.Qualified.Namespace.Address"" />
      </Function>
      <Function Name=""GetFavoriteColors"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.ColorPattern)"" />
      </Function>
      <Function Name=""GetFavoriteColor"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <ReturnType Type=""Fully.Qualified.Namespace.ColorPattern"" />
      </Function>
      <Function Name=""GetNearbyPriorAddresses"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""currentLocation"" Type=""Edm.GeographyPoint"" SRID=""4326"" />
        <Parameter Name=""limit"" Type=""Edm.Int32"" />
        <ReturnType Type=""Collection(Fully.Qualified.Namespace.Address)"" />
      </Function>
      <Function Name=""GetColorAtPosition"" IsBound=""true"">
        <Parameter Name=""painting"" Type=""Fully.Qualified.Namespace.Painting"" />
        <Parameter Name=""position"" Type=""Edm.GeometryPoint"" SRID=""0"" />
        <Parameter Name=""includeAlpha"" Type=""Edm.Boolean"" />
        <ReturnType Type=""Edm.String"" />
      </Function>
      <Function Name=""GetFullName"" IsBound=""true"">
        <Parameter Name=""person"" Type=""Fully.Qualified.Namespace.Person"" />
        <Parameter Name=""nickname"" Type=""Fully.Qualified.Namespace.NameType"" />
        <ReturnType Type=""Fully.Qualified.Namespace.NameType"" />
      </Function>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            #endregion
        }

        public static ODataUri ParseUri(string input, IEdmModel model, int? maxDepth = null)
        {
            var serviceBaseUri = new Uri("http://server/service/");
            var queryUri = new Uri(serviceBaseUri, input);
            ODataUriParser parser = new ODataUriParser(model, serviceBaseUri, queryUri);
            if (maxDepth.HasValue)
            {
                parser.Settings.FilterLimit = maxDepth.Value;
                return parser.ParseUri();
            }
            else
            {
                return parser.ParseUri();
            }
        }

        public static IEdmEntityType GetPet1Type()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Pet1") as IEdmEntityType;
        }

        public static IEdmEntityType GetPet2Type()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Pet2") as IEdmEntityType;
        }

        public static IEdmEntityType GetPet3Type()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Pet3") as IEdmEntityType;
        }

        public static IEdmEntityType GetPet4Type()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Pet4") as IEdmEntityType;
        }

        public static IEdmEntityType GetPet5Type()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Pet5") as IEdmEntityType;
        }

        public static IEdmEntitySet GetPet1Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet1Set");
        }

        public static IEdmEntitySet GetPet2Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet2Set");
        }

        public static IEdmEntitySet GetPet3Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet3Set");
        }

        public static IEdmEntitySet GetPet4Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet4Set");
        }

        public static IEdmEntitySet GetPet5Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet5Set");
        }

        public static IEdmEntitySet GetPet6Set()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Pet6Set");
        }

        public static IEdmEntitySet GetPeopleSet()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("People");
        }

        public static IEdmContainedEntitySet GetContainedDogEntitySet()
        {
            IEdmEntitySet peopleSet = GetPeopleSet();
            IEdmNavigationProperty containedDogProp = GetPersonMyContainedDogNavProp();
            return peopleSet.FindNavigationTarget(containedDogProp) as IEdmContainedEntitySet;
        }

        public static IEdmEntitySet GetLionSet()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Lions");
        }

        public static IEdmEntitySet GetDogsSet()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Dogs");
        }

        public static IEdmEntitySet GetPaintingsSet()
        {
            return TestModel.FindEntityContainer("Context").FindEntitySet("Paintings");
        }

        public static IEdmSingleton GetBossSingleton()
        {
            return TestModel.FindEntityContainer("Context").FindSingleton("Boss");
        }

        public static IEdmEntityType GetLionType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Lion") as IEdmEntityType;
        }

        /// <summary>
        /// Gets a type reference to a lion. We use 'false' for nullable because that is the value the product should set
        /// it to when we have to create a reference (like for the item type of the collection you are filtering or something).
        /// </summary>
        public static IEdmEntityTypeReference GetLionTypeReference()
        {
            return new EdmEntityTypeReference(GetLionType(), false);
        }

        public static IEdmEntityType GetPersonType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Person") as IEdmEntityType;
        }

        /// <summary>
        /// Gets a type reference to a person. We use 'false' for nullable because that is the value the product should set
        /// it to when we have to create a reference (like for the item type of the collection you are filtering or something).
        /// </summary>
        public static IEdmEntityTypeReference GetPersonTypeReference()
        {
            return new EdmEntityTypeReference(GetPersonType(), false);
        }

        /// <summary>
        /// Gets a type reference to an employee. We use 'false' for nullable because that is the value the product should set
        /// it to when we have to create a reference (like for the item type of the collection you are filtering or something).
        /// </summary>
        public static IEdmEntityTypeReference GetEmployeeTypeReference()
        {
            return new EdmEntityTypeReference(GetEmployeeType(), false);
        }

        public static IEdmEntityType GetEmployeeType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Employee") as IEdmEntityType;
        }

        public static IEdmEntityType GetOpenEmployeeType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.OpenEmployee") as IEdmEntityType;
        }

        public static IEdmEntityTypeReference GetManagerTypeReference()
        {
            return new EdmEntityTypeReference(GetManagerType(), false);
        }

        public static IEdmEntityType GetManagerType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Manager") as IEdmEntityType;
        }

        public static IEdmEntityType GetDogType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Dog") as IEdmEntityType;
        }

        /// <summary>
        /// Gets a type reference to a dog. We use 'false' for nullable because that is the value the product should set
        /// it to when we have to create a reference (like for the item type of the collection you are filtering or something).
        /// </summary>
        public static IEdmEntityTypeReference GetDogTypeReference()
        {
            return new EdmEntityTypeReference(GetDogType(), false);
        }

        public static IEdmEntityType GetPaintingType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Painting") as IEdmEntityType;
        }

        public static IEdmEntityType GetFramedPaintingType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.FramedPainting") as IEdmEntityType;
        }

        /// <summary>
        /// Gets a type reference to a painting. We use 'false' for nullable because that is the value the product should set
        /// it to when we have to create a reference (like for the item type of the collection you are filtering or something).
        /// </summary>
        public static IEdmEntityTypeReference GetPaintingTypeReference()
        {
            return new EdmEntityTypeReference(GetPaintingType(), false);
        }

        public static IEdmComplexType GetAddressType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Address") as IEdmComplexType;
        }

        public static IEdmComplexTypeReference GetAddressReference()
        {
            return new EdmComplexTypeReference(GetAddressType(), false);
        }

        public static IEdmComplexType GetHomeAddressType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.HomeAddress") as IEdmComplexType;
        }

        public static IEdmComplexTypeReference GetHomeAddressReference()
        {
            return new EdmComplexTypeReference(GetHomeAddressType(), false);
        }

        public static IEdmNavigationProperty GetPersonMyDogNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyDog");
        }

        public static IEdmNavigationProperty GetPersonMyContainedDogNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyContainedDog");
        }

        public static IEdmNavigationProperty GetPersonMyPet2SetNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyPet2Set");
        }

        public static IEdmNavigationProperty GetPersonMyLionsNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyLions");
        }

        public static IEdmNavigationProperty GetPersonMyPaintingsNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyPaintings");
        }

        public static IEdmNavigationProperty GetPersonMyFavoritePaintingNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyFavoritePainting");
        }

        public static IEdmStructuralProperty GetPersonShoeProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("Shoe");
        }

        public static IEdmStructuralProperty GetPersonNameProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("Name");
        }

        public static IEdmStructuralProperty GetPersonFirstNameProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("FirstName");
        }

        public static IEdmStructuralProperty GetPersonFavoriteDateProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("FavoriteDate");
        }

        public static IEdmStructuralProperty GetPersonMyDateProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyDate");
        }

        public static IEdmStructuralProperty GetPersonMyDatesProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyDates");
        }

        public static IEdmStructuralProperty GetPersonMyTimeOfDayProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyTimeOfDay");
        }

        public static IEdmStructuralProperty GetPersonMyTimeOfDaysProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyTimeOfDays");
        }

        public static IEdmStructuralProperty GetPersonBirthdateProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("Birthdate");
        }

        public static IEdmStructuralProperty GetPersonTimeEmployedProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("TimeEmployed");
        }

        public static IEdmStructuralProperty GetPersonPreviousAddressesProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("PreviousAddresses");
        }

        public static IEdmStructuralProperty GetPersonFavoriteColorsProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("FavoriteColors");
        }

        public static IEdmNavigationProperty GetPersonMyFriendsDogsProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyFriendsDogs");
        }

        public static IEdmStructuralProperty GetPersonAddressProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("MyAddress");
        }

        public static IEdmStructuralProperty GetPersonGeographyPointProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeographyPoint");
        }

        public static IEdmStructuralProperty GetPersonGeographyLineStringProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeographyLineString");
        }

        public static IEdmStructuralProperty GetPersonGeographyPolygonProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeographyPolygon");
        }

        public static IEdmStructuralProperty GetPersonGeometryPointProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeometryPoint");
        }

        public static IEdmStructuralProperty GetPersonGeometryLineStringProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeometryLineString");
        }

        public static IEdmStructuralProperty GetPersonGeometryPolygonProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeometryPolygon");
        }

        public static IEdmStructuralProperty GetPersonGeographyCollectionProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeographyCollection");
        }

        public static IEdmStructuralProperty GetEmployeeWorkEmailProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Employee")).FindProperty("WorkEmail");
        }

        public static IEdmNavigationProperty GetEmployeePaintingsInOfficeNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Employee")).FindProperty("PaintingsInOffice");
        }

        public static IEdmNavigationProperty GetEmployeeOfficeDogNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Employee")).FindProperty("OfficeDog");
        }

        public static IEdmStructuralProperty GetManagerNumberOfReportsProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Manager")).FindProperty("NumberOfReports");
        }

        public static IEdmStructuralProperty GetPersonGeometryCollectionProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Person")).FindProperty("GeometryCollection");
        }

        public static IEdmStructuralProperty GetLionAttackDatesProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Lion")).FindProperty("AttackDates");
        }

        public static IEdmStructuralProperty GetLionId1Property()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Lion")).FindProperty("ID1");
        }

        public static IEdmStructuralProperty GetLionId2Property()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Lion")).FindProperty("ID2");
        }

        public static IEdmStructuralProperty GetAddressCityProperty()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Address")).FindProperty("City");
        }

        public static IEdmStructuralProperty GetPet2PetColorPatternProperty()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Pet2")).FindProperty("PetColorPattern");
        }

        public static IEdmStructuralProperty GetAddressMyNeighborsProperty()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Address")).FindProperty("MyNeighbors");
        }

        // ToDo: Don't support NavProps in Complex types yet
        // ToDo: When the work is done to allow Nav props in complex types make sure we can select and expand them
        public static IEdmNavigationProperty GetAddressMyFavoriteNeighborNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Address")).FindProperty("MyFavoriteNeighbor");
        }

        public static IEdmNavigationProperty GetDogMyPeopleNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Dog")).FindProperty("MyPeople");
        }

        public static IEdmNavigationProperty GetDogLionWhoAteMeNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Dog")).FindProperty("LionWhoAteMe");
        }

        public static IEdmStructuralProperty GetDogNicknamesProperty()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Dog")).FindProperty("Nicknames");
        }

        public static IEdmFunction GetHasDogOverloadWithOneParameter()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.Count() == 1).As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForEmployeeHasDogWithTwoParameters()
        {
            return TestModel.EntityContainer.FindOperationImports("HasDog").Single(f => f.Operation.Parameters.Count() == 2 && f.Operation.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmOperation GetFunctionForEmployeeHasDogWithTwoParameters()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.Count() == 2 && f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmFunction GetHasDogOverloadForPeopleWithTwoParameters()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.Count() == 2 && f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person").As<IEdmFunction>();
        }

        public static IEdmOperationImport GetHasDogOverloadImportForPeopleWithThreeParameters()
        {
            return TestModel.EntityContainer.FindOperationImports("HasDog").Single(f => f.Operation.Parameters.Count() == 3 && f.Operation.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person");
        }

        public static IEdmOperation GetHasDogOverloadForPeopleWithThreeParameters()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.Count() == 3 && f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person");
        }

        public static IEdmOperationImport GetFunctionImportForGetPet1()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet1").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPet2()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet2").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPet3()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet3").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPet4()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet4").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPet5()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet5").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPet6()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPet6").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetPetCount()
        {
            return TestModel.EntityContainer.FindOperationImports("GetPetCount").Single();
        }

        public static IEdmFunction GetFunctionForHasJob()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasJob").OfType<IEdmFunction>().Single();
        }

        public static IEdmOperationImport GetFunctionImportForAllHaveDogWithOneParameter()
        {
            return TestModel.EntityContainer.FindOperationImports("AllHaveDog").Single(f => f.Operation.Parameters.Count() == 1);
        }

        public static IEdmFunction GetFunctionForAllHaveDogWithOneParameter()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.AllHaveDog").Single(f => f.Parameters.Count() == 1).As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForAllHaveDogWithTwoParameters()
        {
            return TestModel.EntityContainer.FindOperationImports("AllHaveDog").Single(f => f.Operation.Parameters.Count() == 2);
        }

        public static IEdmFunction GetFunctionForAllHaveDogWithTwoParameters()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.AllHaveDog").Single(f => f.Parameters.Count() == 2).As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForGetCoolestPerson()
        {
            return TestModel.EntityContainer.FindOperationImports("GetCoolestPerson").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetCoolPeople()
        {
            return TestModel.EntityContainer.FindOperationImports("GetCoolPeople").Single();
        }

        public static IEdmOperation GetFunctionForGetHotPeople()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetHotPeople").Single();
        }

        public static IEdmOperation GetFunctionForGetCoolPeople()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetCoolPeople").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetBestManager()
        {
            return TestModel.EntityContainer.FindOperationImports("GetBestManager").Single();
        }

        public static IEdmOperationImport GetFunctionImportForResetAllData()
        {
            return TestModel.EntityContainer.FindOperationImports("ResetAllData").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetMyDog()
        {
            return TestModel.EntityContainer.FindOperationImports("GetMyDog").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetSomeNumbers()
        {
            return TestModel.EntityContainer.FindOperationImports("GetSomeNumbers").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetSomeAddresses()
        {
            return TestModel.EntityContainer.FindOperationImports("GetSomeAddresses").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetSomeColors()
        {
            return TestModel.EntityContainer.FindOperationImports("GetSomeColors").Single();
        }

        public static IEdmFunction GetFunctionForGetPriorAddresses()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetPriorAddresses").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForGetPriorNumbers()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetPriorNumbers").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForGetFavoriteColors()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetFavoriteColors").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForGetMyDog()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetMyDog").Single().As<IEdmFunction>();
        }

        public static IEdmFunctionImport GetFunctionImportIsAddressGood()
        {
            return TestModel.EntityContainer.FindOperationImports("IsAddressGood").Single().As<IEdmFunctionImport>();
        }

        public static IEdmFunction GetFunctionForAllMyFriendsDogs()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.AllMyFriendsDogs").Single(f => f.Parameters.Count() == 1).As<IEdmFunction>();
        }

        public static IEdmOperationImport[] GetAllFunctionImportsForGetMostImportantPerson()
        {
            return TestModel.EntityContainer.FindOperationImports("GetMostImporantPerson").ToArray();
        }

        public static IEdmFunction GetFunctionForCanMoveToAddress()
        {
            var functions = TestModel.FindOperations("Fully.Qualified.Namespace.CanMoveToAddress").OfType<IEdmFunction>().ToList();
            return functions.First();
        }

        public static IEdmOperationImport GetFunctionImportForCanMoveToAddresses()
        {
            return TestModel.EntityContainer.FindOperationImports("CanMoveToAddresses").Single();
        }

        public static IEdmFunction GetFunctionForCanMoveToAddresses()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.CanMoveToAddresses").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForIsOlderThanByte()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.IsOlderThanByte").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForIsOlderThanSByte()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.IsOlderThanSByte").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForIsOlderThanShort()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.IsOlderThanShort").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForIsOlderThanSingle()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.IsOlderThanSingle").Single().As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForOwnsTheseDogs()
        {
            return TestModel.EntityContainer.FindOperationImports("OwnsTheseDogs").Single();
        }

        public static IEdmFunction GetFunctionForOwnsTheseDogs()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.OwnsTheseDogs").Single().As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForFindMyOwner()
        {
            return TestModel.EntityContainer.FindOperationImports("FindMyOwner").Single();
        }

        public static IEdmFunction GetFunctionForFindMyOwner()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.FindMyOwner").Single().As<IEdmFunction>();
        }

        public static IEdmOperationImport GetFunctionImportForIsInTheUs()
        {
            return TestModel.EntityContainer.FindOperationImports("IsInTheUS").Single();
        }

        public static IEdmOperationImport GetFunctionImportForGetMyPerson()
        {
            return TestModel.EntityContainer.FindOperationImports("GetMyPerson").Single();
        }

        public static IEdmFunction GetFunctionForGetMyPerson()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetMyPerson").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetFunctionForGetFullName()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetFullName").Single().As<IEdmFunction>();
        }

        public static IEdmStructuralProperty GetDogColorProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Dog")).FindProperty("Color");
        }

        public static IEdmStructuralProperty GetDogIdProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Dog")).FindProperty("ID");
        }

        public static IEdmStructuralProperty GetPaintingArtistProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Painting")).FindProperty("Artist");
        }

        public static IEdmStructuralProperty GetPaintingColorsProperty()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Painting")).FindProperty("Colors");
        }

        public static IEdmOperation GetDogWalkAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Walk").Single();
        }

        public static IEdmOperationImport GetDogWalkActionImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Walk").Single();
        }

        public static IEdmOperationImport GetFireAllActionImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("FireAll").Single();
        }

        public static IEdmOperation GetFireAllAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.FireAll").Single();
        }

        public static IEdmOperationImport[] GetAllHasDogOverloadImports()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("HasDog").ToArray();
        }

        public static IEdmOperation[] GetAllHasDogOverloads()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").ToArray();
        }

        public static IEdmOperation GetAllMyFriendsDogs_NoSet()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet").Single();
        }

        public static IEdmOperationImport GetHasDogOverloadForEmployeeImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("HasDog").Single(f => f.Operation.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmOperation GetHasDogOverloadForPeople()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person");
        }

        public static IEdmOperation GetHasDogOverloadForEmployee()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmOperationImport[] GetAllHasDogOverloadsForPeople()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("HasDog").Where(f => f.Operation.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person").ToArray();
        }

        public static IEdmFunction[] GetAllHasDogFunctionOverloadsForPeople()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Where(f => f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person").OfType<IEdmFunction>().ToArray();
        }

        public static IEdmOperationImport[] GetAllMoveOverloadImports()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Move").ToArray();
        }

        public static IEdmOperation[] GetAllMoveOverloads()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Move").ToArray();
        }

        public static IEdmOperationImport[] GetAllHasHatOverloads()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("HasHat").ToArray();
        }

        public static IEdmOperationImport GetMoveOverloadForEmployeeImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Move").Single(f => f.Operation.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmOperation GetMoveOverloadForPerson()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Move").Single(f => f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Person");
        }

        public static IEdmOperation GetMoveOverloadForEmployee()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Move").Single(f => f.Parameters.First().Type.FullName() == "Fully.Qualified.Namespace.Employee");
        }

        public static IEdmFunction GetColorAtPositionFunction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetColorAtPosition").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetNearbyPriorAddressesFunction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.GetNearbyPriorAddresses").Single().As<IEdmFunction>();
        }

        public static IEdmStructuralProperty GetAddressStreetProp()
        {
            return (IEdmStructuralProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Address")).FindProperty("Street");
        }

        public static IEdmOperationImport GetRestoreActionImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Restore").Single();
        }

        public static IEdmOperation GetRestoreAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Restore").Single();
        }

        public static IEdmOperationImport GetPersonPaintActionImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Paint").Single();
        }

        public static IEdmOperation GetPersonPaintAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Paint").Single();
        }

        public static IEdmNavigationProperty GetPaintingOwnerNavProp()
        {
            return (IEdmNavigationProperty)((IEdmStructuredType)TestModel.FindType("Fully.Qualified.Namespace.Painting")).FindProperty("Owner");
        }

        public static IEdmStructuralProperty GetPaintingValueProperty()
        {
            return (IEdmStructuralProperty)GetPaintingType().FindProperty("Value");
        }

        public static IEdmOperationImport GetFunctionImportForPaintingRestoreAction()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("Restore").Single();
        }

        public static IEdmStructuralProperty GetDogNamedStream()
        {
            return (IEdmStructuralProperty)GetDogType().FindProperty("NamedStream");
        }

        public static IEdmOperationImport GetVoidServiceOperation()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("GetNothing").Single();
        }

        public static IEdmOperation GetPersonMoveAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.Move").Where(a => a.Parameters.Count() == 2).Single();
        }

        public static IEdmOperationImport GetChangeStateActionImport()
        {
            return TestModel.FindEntityContainer("Context").FindOperationImports("ChangeState").Single();
        }

        public static IEdmOperation GetChangeStateAction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.ChangeState").Single();
        }

        public static IEdmFunction GetHasJobFunction()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasJob").Single().As<IEdmFunction>();
        }

        public static IEdmFunction GetHasDogOneParameterOverload()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").Single(f => f.Parameters.Count() == 1).As<IEdmFunction>();
        }

        public static IEdmFunction[] GetHasDogOverloads()
        {
            return TestModel.FindOperations("Fully.Qualified.Namespace.HasDog").OfType<IEdmFunction>().ToArray();
        }

        public static IEdmStructuralProperty GetPersonPropWithPeriods()
        {
            return (IEdmStructuralProperty)GetPersonType().FindProperty("Prop.With.Periods");
        }

        public static IEdmComplexType GetHeatbeatComplexType()
        {
            return TestModel.FindType("Fully.Qualified.Namespace.Heartbeat") as IEdmComplexType;
        }

    }
}
