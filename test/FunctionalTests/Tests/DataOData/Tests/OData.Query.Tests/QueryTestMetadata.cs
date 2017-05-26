//---------------------------------------------------------------------
// <copyright file="QueryTestMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.DataClasses;
    #endregion Namespaces

    /// <summary>
    /// Helper class to define a test metadata model and some other helper methods to work with it.
    /// </summary>
    public static class QueryTestMetadata
    {
        public static IEdmModel BuildTestMetadata(IEntityModelPrimitiveTypeResolver primitiveTypeResolver, IDataServiceProviderFactory provider)
        {
            var schema = new EntityModelSchema()
            {
                new ComplexType("TestNS","Address")
                {
                    new MemberProperty("City", DataTypes.String.Nullable()), 
                    new MemberProperty("Zip", DataTypes.Integer),
                    new ClrTypeAnnotation(typeof(Address))
                },
                new EntityType("TestNS","Customer")
                {
                    new MemberProperty("ID", DataTypes.Integer){IsPrimaryKey=true},
                    new MemberProperty("Name", DataTypes.String.Nullable()),
                    new MemberProperty("Emails", DataTypes.CollectionType.WithElementDataType(DataTypes.String.Nullable())),
                    new MemberProperty("Address", DataTypes.ComplexType.WithName("TestNS","Address")),
                    new ClrTypeAnnotation(typeof(Customer))
                },
                new EntityType("TestNS","MultiKey")
                {
                    new MemberProperty("KeyB", DataTypes.FloatingPoint){IsPrimaryKey=true},
                    new MemberProperty("KeyA", DataTypes.String.Nullable()){IsPrimaryKey=true},
                    new MemberProperty("Keya", DataTypes.Integer){IsPrimaryKey=true},
                    new MemberProperty("NonKey", DataTypes.String.Nullable()),
                    new ClrTypeAnnotation(typeof(MultiKey))
                },
                new EntityType("TestNS","TypeWithPrimitiveProperties")
                {
                      new MemberProperty("ID", DataTypes.Integer){ IsPrimaryKey=true},
                      new MemberProperty("StringProperty", DataTypes.String.Nullable()),
                      new MemberProperty("BoolProperty", DataTypes.Boolean),
                      new MemberProperty("NullableBoolProperty", DataTypes.Boolean.Nullable()),
                      new MemberProperty("ByteProperty", EdmDataTypes.Byte),
                      new MemberProperty("NullableByteProperty",  EdmDataTypes.Byte.Nullable()),
                      new MemberProperty("DateTimeProperty", DataTypes.DateTime.WithTimeZoneOffset(true)),
                      new MemberProperty("NullableDateTimeProperty", DataTypes.DateTime.WithTimeZoneOffset(true).Nullable()),
                      new MemberProperty("DecimalProperty", EdmDataTypes.Decimal()),
                      new MemberProperty("NullableDecimalProperty", EdmDataTypes.Decimal().Nullable()),
                      new MemberProperty("DoubleProperty", EdmDataTypes.Double),
                      new MemberProperty("NullableDoubleProperty", EdmDataTypes.Double.Nullable()),
                      new MemberProperty("GuidProperty", DataTypes.Guid),
                      new MemberProperty("NullableGuidProperty", DataTypes.Guid.Nullable()),
                      new MemberProperty("Int16Property", EdmDataTypes.Int16),
                      new MemberProperty("NullableInt16Property", EdmDataTypes.Int16.Nullable()),
                      new MemberProperty("Int32Property", DataTypes.Integer),
                      new MemberProperty("NullableInt32Property", DataTypes.Integer.Nullable()),
                      new MemberProperty("Int64Property", EdmDataTypes.Int64),
                      new MemberProperty("NullableInt64Property", EdmDataTypes.Int64.Nullable()),
                      new MemberProperty("SByteProperty", EdmDataTypes.SByte),
                      new MemberProperty("NullableSByteProperty", EdmDataTypes.SByte.Nullable()),
                      new MemberProperty("SingleProperty", EdmDataTypes.Single),
                      new MemberProperty("NullableSingleProperty", EdmDataTypes.Single.Nullable()),
                      new MemberProperty("BinaryProperty", DataTypes.Binary.Nullable()),
                      new ClrTypeAnnotation(typeof(TypeWithPrimitiveProperties))
                }
            };

            List<FunctionParameter> defaultParameters = new List<FunctionParameter>()
            {
                new FunctionParameter("soParam", DataTypes.Integer) 
            };

            EntitySet customersSet = new EntitySet("Customers", "Customer");
            EntityContainer container = new EntityContainer("BinderTestMetadata")
            {
                customersSet,
                new EntitySet("MultiKeys", "MultiKey"),
                new EntitySet("TypesWithPrimitiveProperties","TypeWithPrimitiveProperties")
            };
            schema.Add(container);

            EntityDataType customerType = DataTypes.EntityType.WithName("TestNS", "Customer");
            EntityDataType multiKeyType = DataTypes.EntityType.WithName("TestNS", "MultiKey");
            EntityDataType entityTypeWithPrimitiveProperties = DataTypes.EntityType.WithName("TestNS", "TypeWithPrimitiveProperties");

            ComplexDataType addressType = DataTypes.ComplexType.WithName("TestNS", "Address");

            addressType.Definition.Add(new ClrTypeAnnotation(typeof(Address)));
            customerType.Definition.Add(new ClrTypeAnnotation(typeof(Customer)));
            multiKeyType.Definition.Add(new ClrTypeAnnotation(typeof(MultiKey)));
            entityTypeWithPrimitiveProperties.Definition.Add(new ClrTypeAnnotation(typeof(TypeWithPrimitiveProperties)));

            //container.Add(CreateServiceOperation("VoidServiceOperation", defaultParameters, null, ODataServiceOperationResultKind.Void));
            //container.Add(CreateServiceOperation("DirectValuePrimitiveServiceOperation", defaultParameters, DataTypes.Integer, ODataServiceOperationResultKind.DirectValue));
            //container.Add(CreateServiceOperation("DirectValueComplexServiceOperation", defaultParameters, addressType, ODataServiceOperationResultKind.DirectValue));
            //container.Add(CreateServiceOperation("DirectValueEntityServiceOperation", defaultParameters, customerType, customersSet, ODataServiceOperationResultKind.DirectValue));

            //container.Add(CreateServiceOperation("EnumerationPrimitiveServiceOperation", defaultParameters, DataTypes.CollectionType.WithElementDataType(DataTypes.Integer), ODataServiceOperationResultKind.Enumeration));
            //container.Add(CreateServiceOperation("EnumerationComplexServiceOperation", defaultParameters, addressType, ODataServiceOperationResultKind.Enumeration));
            //container.Add(CreateServiceOperation("EnumerationEntityServiceOperation", defaultParameters, DataTypes.CollectionOfEntities(customerType.Definition), customersSet, ODataServiceOperationResultKind.Enumeration));

            //container.Add(CreateServiceOperation("QuerySinglePrimitiveServiceOperation", defaultParameters, DataTypes.Integer, ODataServiceOperationResultKind.QueryWithSingleResult));
            //container.Add(CreateServiceOperation("QuerySingleComplexServiceOperation", defaultParameters, addressType, ODataServiceOperationResultKind.QueryWithSingleResult));
            //container.Add(CreateServiceOperation("QuerySingleEntityServiceOperation", defaultParameters, customerType, customersSet, ODataServiceOperationResultKind.QueryWithSingleResult));

            //container.Add(CreateServiceOperation("QueryMultiplePrimitiveServiceOperation", defaultParameters, DataTypes.Integer, ODataServiceOperationResultKind.QueryWithMultipleResults));
            //container.Add(CreateServiceOperation("QueryMultipleComplexServiceOperation", defaultParameters, addressType, ODataServiceOperationResultKind.QueryWithMultipleResults));
            //container.Add(CreateServiceOperation("QueryMultipleEntityServiceOperation", defaultParameters, DataTypes.CollectionOfEntities(customerType.Definition), customersSet, ODataServiceOperationResultKind.QueryWithMultipleResults));

            //container.Add(CreateServiceOperation("ServiceOperationWithNoParameters", new List<FunctionParameter>(), DataTypes.CollectionOfEntities(customerType.Definition), customersSet, ODataServiceOperationResultKind.QueryWithMultipleResults));
            //container.Add(CreateServiceOperation(
            //                        "ServiceOperationWithMultipleParameters",
            //                         new List<FunctionParameter>()
            //                         {
            //                             new FunctionParameter("paramInt", DataTypes.Integer),  
            //                             new FunctionParameter("paramString", DataTypes.String.Nullable()),
            //                             new FunctionParameter("paramNullableBool", DataTypes.Boolean.Nullable()) 
            //                         },
            //                         DataTypes.CollectionOfEntities(customerType.Definition),
            //                         customersSet,
            //                         ODataServiceOperationResultKind.QueryWithMultipleResults));

            new ApplyDefaultNamespaceFixup("TestNS").Fixup(schema);
            new ResolveReferencesFixup().Fixup(schema);
            primitiveTypeResolver.ResolveProviderTypes(schema, new EdmDataTypeResolver());

            return provider.CreateMetadataProvider(schema);
        }
    }
}
