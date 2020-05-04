//---------------------------------------------------------------------
// <copyright file="ClientMetadataAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.AnnotationTargetingOperationTestsProxy;
    using Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.ConstantAnnotationProxy;
    using Xunit;
    using Edm = Microsoft.OData.Edm;
    using OperationProxy = Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.AnnotationTargetingOperationTestsProxy;

    public class ClientMetadataAnnotationTests
    {
        private const string DescriptionV1 = "Org.OData.Core.V1.Description";
        private const string RecordAnnotationTerm = "OperationTestService.RecordAnnotation";
        private const string NavAnnotationTerm = "OperationTestService.NavAnnotation";
        private const string NavOfDerivedETAnnotationTerm = "OperationTestService.NavOfDerivedETAnnotation";
        private const string CollectionOfCTPropertyAnnotation = "OperationTestService.CollectionOfCTPropertyAnnotation";
        private const string constAnnotationQualifiedNamePrefix = "ConstantAnnotationService.";
        private const string ExternalTargetingQualifier = "ExternalTargeting";
        private const string NonExistingTermName = "NonExistingTermName";

        DefaultContainerPlus defaultContainer = new DefaultContainerPlus(new Uri("http://odata.org/"));
        OperationTestServiceContainerPlus operationTestServiceContainer = new OperationTestServiceContainerPlus(new Uri("http://odata.org/"));

        #region Constant Annotation

        [Fact]
        public void GetConstantAnnotationDefinedInAttribute()
        {
            var dsc = new ConstantAnnotationContainerPlus(new Uri("http://odata.org/"));

            ETForConstAsAttributePlus entity = new ETForConstAsAttributePlus();
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "BinaryTerm", new byte[] { 0x12, 0x34 });
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "BooleanTerm", false);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DateTerm", new Edm.Date(2000, 01, 01));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DateTimeOffsetTerm", new DateTimeOffset(2000, 1, 1, 16, 0, 0, new TimeSpan(0)));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DecimalTerm", 2.15m);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DurationTerm", new TimeSpan(0, 0, 0, 0, 0));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "FloatTerm", 1.0);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "GuidTerm", new Guid("21EC2020-3AEA-1069-A2DD-08002B30309E"));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "Int32Term", 32L);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "Int64Term", 33L);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "StringTerm", "test2");
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "TimeOfDayTerm", new Edm.TimeOfDay(20, 45, 0, 0));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "EnumMemberHasFlagTerm", AccessLevelPlus.ReadPlus | AccessLevelPlus.WritePlus);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "EnumMemberTerm", ColorPlus.BluePlus);
        }

        [Fact]
        public void GetConstantAnnotationDefinedInElement()
        {
            var dsc = new ConstantAnnotationContainerPlus(new Uri("http://odata.org/"));

            ETForConstAsElementPlus entity = new ETForConstAsElementPlus();
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "BinaryTerm", new byte[] { 0x12, 0x34 });
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "BooleanTerm", true);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DateTerm", new Edm.Date(2000, 01, 01));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DateTimeOffsetTerm", new DateTimeOffset(2000, 1, 1, 16, 0, 0, new TimeSpan(0)));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DecimalTerm", 3.14m);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "DurationTerm", new TimeSpan(0, 0, 0, 0, 0));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "FloatTerm", 2.2);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "GuidTerm", new Guid("21EC2020-3AEA-1069-A2DD-08002B30309D"));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "Int32Term", 42L);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "Int64Term", 64L);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "StringTerm", "test");
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "TimeOfDayTerm", new Edm.TimeOfDay(21, 45, 0, 0));
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "EnumMemberHasFlagTerm", AccessLevelPlus.ReadPlus);
            GetAndCheckAnnotation(entity, dsc, constAnnotationQualifiedNamePrefix + "EnumMemberTerm", ColorPlus.YellowPlus);
        }

        [Fact]
        public void GetNullAnnotationDefinedInElement()
        {
            var dsc = new ConstantAnnotationContainerPlus(new Uri("http://odata.org/"));

            ETForNullAsElementPlus entity = new ETForNullAsElementPlus();
            GetAndCheckAnnotation<ETForNullAsElementPlus, byte[]>(entity, dsc, constAnnotationQualifiedNamePrefix + "BinaryTerm", null);
            GetAndCheckAnnotation<ETForNullAsElementPlus, string>(entity, dsc, constAnnotationQualifiedNamePrefix + "StringTerm", null);
        }

        private void GetAndCheckAnnotation<TEntity, T>(TEntity entity, ConstantAnnotationContainerPlus dsc, string termName, T expectedValue)
        {
            T annotationValue = default(T);
            var result = dsc.TryGetAnnotation(entity, termName, out annotationValue);
            Assert.True(result);
            if (annotationValue is byte[])
            {
                var binaryValue = annotationValue as byte[];
                var expectedBinaryValue = expectedValue as byte[];
                Assert.True(binaryValue.SequenceEqual(expectedBinaryValue));
            }
            else
            {
                Assert.Equal(expectedValue, annotationValue);
            }
        }

        #endregion Constant annotation

        #region Annotation targeting Edm type by using object instance

        [Fact]
        public void GetConstantAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            string description = null;
            var result = defaultContainer.TryGetAnnotation(person, DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("A person contains basic properties", description);
        }

        [Fact]
        public void GetConstantAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus vipCustomer = CreateVipCustomer();
            string description = null;
            var result = defaultContainer.TryGetAnnotation(vipCustomer, DescriptionV1, out description);
            Assert.Equal("A person contains basic properties", description);
        }

        [Fact]
        public void GetPathAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            string billingAddress = null;
            string CompanyAddress = null;

            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.BillingAddress", out billingAddress);
            Assert.True(result);
            Assert.Equal("Lianhua Road", billingAddress);

            result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.CompanyAddress", out CompanyAddress);
            Assert.True(result);
            Assert.Equal("Zixing Road", CompanyAddress);
        }

        [Fact]
        public void GetPathAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus person = CreateVipCustomer();

            string billingAddress = null;
            string CompanyAddress = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.BillingAddress", out billingAddress);
            Assert.True(result);
            Assert.Equal("Xuanwu Street", billingAddress);
            result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.CompanyAddress", out CompanyAddress);
            Assert.True(result);
            Assert.Equal("Wudong Road", CompanyAddress);
        }

        [Fact]
        public void GetRecordAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();

            PersonInPublicViewPlus personInPublicView = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.PersonView", out personInPublicView);
            Assert.True(result);
            Assert.NotNull(personInPublicView);
            Assert.Equal("NelsonW", personInPublicView.DisplayNamePlus);
            Assert.Equal(PersonGenderPlus.FemalePlus, personInPublicView.MaleOrFemalePlus);
        }

        [Fact]
        public void GetRecordAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus person = CreateVipCustomer();

            PersonInPublicViewPlus personInPublicView = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.PersonView", out personInPublicView);
            Assert.True(result);
            Assert.NotNull(personInPublicView);
            Assert.Equal("TomN", personInPublicView.DisplayNamePlus);
            Assert.Equal(PersonGenderPlus.MalePlus, personInPublicView.MaleOrFemalePlus);
        }

        [Fact]
        public void GetCollectionAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            IEnumerable<string> seoTerms = null;

            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out seoTerms);
            Assert.True(result);
            Assert.NotNull(seoTerms);
            Assert.Equal(2, seoTerms.Count());
            Assert.Equal("Nelson", seoTerms.ElementAt(0));
            Assert.Equal("NelsonW", seoTerms.ElementAt(1));
        }

        [Fact]
        public void GetPathAnnotationTargetingComplexType()
        {
            PersonPlus person = CreatePerson();
            string cityName = null;

            var result = defaultContainer.TryGetAnnotation(person.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityName);
            Assert.True(result);
            Assert.Equal("Shanghai", cityName);
        }

        [Fact]
        public void GetPathAnnotationTargetingDerivedComplexType()
        {
            PersonPlus person = CreatePerson();
            string cityName = null;

            var result = defaultContainer.TryGetAnnotation(person.CompanyLocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityName);
            Assert.True(result);
            Assert.Equal("Beijing", cityName);
        }

        [Fact]
        public void GetConstantAnnotationTargetingEnumType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(person.GenderPlus, DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("This Enum type indicates a person's gender", description);
        }

        #endregion Annotation targeting Type

        #region Annotation targeting Edm type by using Type

        [Fact]
        public void GetConstantAnnotationTargetingEntityTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(PersonPlus), DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("A person contains basic properties", description);
        }

        [Fact]
        public void GetConstantAnnotationTargetingDerivedEntityTypeByUsingType()
        {
            VipCustomerPlus vipCustomer = CreateVipCustomer();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(VipCustomerPlus), DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("A person contains basic properties", description);
        }

        [Fact]
        public void GetConstantAnnotationTargetingComplexTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(CompanyLocationPlus), DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("Company name for this person", description);
        }

        [Fact]
        public void GetConstantAnnotationTargetingEnumTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(PersonGenderPlus), DescriptionV1, out description);
            Assert.True(result);
            Assert.Equal("This Enum type indicates a person's gender", description);
        }

        #endregion Annotation targeting Type

        #region Annotation targeting bound function

        [Fact]
        public void GetAnnotationTargetingBoundFunction()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithoutParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundFunctionCalledFromDerivedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithoutParameterBoundToDerivedET", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundFunctionWithParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s) => et.FunctionWithParameterPlus(s),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingOverridedBoundFunctionWithParameterOverride()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => et.FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityTypeWithDerviedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => et.FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionOfEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionofEntityCalledFromCollectionofDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfDerivedEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionofBaseEntityCalledFromCollectionofDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfBaseEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfBaseEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToEntityByCallingExtensionMethods()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.SingleETPlus.FunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityByCallingExtensionMethodsWithDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.ByKey("userName").FunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameterBoundToDerivedET", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityByCallingExtensionMethodsWithDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => operationTestServiceContainer.DerivedETSetsPlus.ByKey("userName").FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        #endregion

        #region Annotation targeting bound function by using MethodInfo

        [Fact]
        public void GetAnnotationTargetingBoundFunctionByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithoutParameterPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithoutParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(DerivedETPlus).GetMethod("FunctionWithoutParameterPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithoutParameterBoundToDerivedET", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionWithParameterBoundToEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingOverloadedFunctionWithParameterBoundToEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityTypeCalledFromDerviedEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(DerivedETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionOfEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfEntityPlus",
                new Type[] { typeof(DataServiceQuery<ETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionofEntityCalledFromCollectionofDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfEntityPlus",
                new Type[] { typeof(DataServiceQuery<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfDerivedEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToCollectionofBaseEntityCalledFromCollectionofDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfBaseEntityPlus",
                new Type[] { typeof(DataServiceQuery<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionBoundToCollectionOfBaseEntity", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToEntityByCallingExtensionMethodsByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<ETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityByCallingExtensionMethodsWithDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithOneParameterBoundToDerivedET", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityByCallingExtensionMethodsWithDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<DerivedETPlus>), typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionWithTwoParameters", annotationValue);
        }

        #endregion

        #region Annotation targeting bound action

        [Fact]
        public void GetAnnotationTargetingBoundActionWithoutReturnType()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceActionQuery>, string>(
                () => et.ActionWithoutReturnTypePlus(),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithoutReturnType", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundActionWithParameterWithoutReturnType()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuery>, string>(
                (s1) => et.ActionWithParameterWithoutReturnTypePlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithParameterWithoutReturnType", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundActionWithoutParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceActionQuerySingle<CTPlus>>, string>(
                () => et.ActionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithoutParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundActionWithParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuerySingle<CTPlus>>, string>(
                (s1) => et.ActionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithOneParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingOverloadedActionBoundToCollectionOfEntityType()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuery<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.ActionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionBoundToCollectionOfEntity", annotationValue);
        }
        #endregion Annotation targeting bound Action

        #region Annotation targeting bound action by using MethodInfo

        [Fact]
        public void GetAnnotationTargetingBoundActionWithoutReturnTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("ActionWithoutReturnTypePlus"),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithoutReturnType", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingBoundActionWithParameterWithoutReturnTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("ActionWithParameterWithoutReturnTypePlus", new Type[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionWithParameterWithoutReturnType", annotationValue);
        }

        #endregion

        #region Annotation targeting Navigation Property or Property

        [Fact]
        public void GetConstantAnnotationTargetingProperty()
        {
            bool annotationValue = false;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<long>, bool>(
                () => person.ConcurrencyPlus, "Org.OData.Core.V1.Computed", out annotationValue);
            Assert.True(result);
            Assert.True(annotationValue);
        }

        [Fact]
        public void GetPathAnnotationTargetingProperty()
        {
            string annotationValue = null;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<LocationPlus>, string>(
                () => person.LocationPlus,
                "Microsoft.OData.SampleService.Models.TripPin.CompanyCityName",
                out annotationValue);
            Assert.True(result);
            Assert.Equal("Beijing", annotationValue);
        }

        [Fact]
        public void GetConstantAnnotationTargetingNavigationProperty()
        {
            string annotationValue = null;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceCollection<PersonPlus>>, string>(
                () => person.FriendsPlus,
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("A friend of this person", annotationValue);
        }

        #endregion

        #region Annotation targeting navigation property or property by using PropertyInfo

        [Fact]
        public void GetConstantAnnotationTargetingPropertyByUsingPropertyInfo()
        {
            bool annotationValue = false;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("ConcurrencyPlus"), "Org.OData.Core.V1.Computed", out annotationValue);
            Assert.True(result);
            Assert.True(annotationValue);
        }

        [Fact]
        public void GetPathAnnotationTargetingPropertyByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("LocationPlus"),
                "Microsoft.OData.SampleService.Models.TripPin.CompanyCityName",
                out annotationValue);
            Assert.False(result);
            Assert.Null(annotationValue);
        }

        [Fact]
        public void GetConstantAnnotationTargetingNavigationPropertyByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("FriendsPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("A friend of this person", annotationValue);
        }
        #endregion

        #region Annotation targeting entityContainer
        [Fact]
        public void GetAnnotationTargetingEntityContainer()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<string>(defaultContainer, DescriptionV1, out annotationValue);
            Assert.True(result);
            Assert.Equal("TripPin Service", annotationValue);
        }
        #endregion

        #region Annotation targeting operation import

        [Fact]
        public void GetAnnotationTargetingFunctionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundFunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionImportWithParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingActionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundActionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionImportWithParameter", annotationValue);
        }

        #endregion

        #region Annotation targeting operation import by using MethodInfo

        [Fact]
        public void GetAnnotationTargetingFunctionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundFunctionWithParameterPlus", new[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("FunctionImportWithParameter", annotationValue);
        }

        [Fact]
        public void GetAnnotationTargetingActionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundActionWithParameterPlus", new[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("ActionImportWithParameter", annotationValue);
        }

        #endregion

        #region Annotation targeting EntitySet or Singleton

        [Fact]
        public void GetCollectionOfConstantAnnotationTargetingEntitySet()
        {
            IEnumerable<Nullable<long>> annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, IEnumerable<Nullable<long>>>(
                () => defaultContainer.PeoplePlus,
                "Microsoft.OData.SampleService.Models.TripPin.VipCustomerWhiteList",
                out annotationValue);
            Assert.True(result);
            var actualAnnotationValue = annotationValue as List<Nullable<long>>;
            Assert.NotNull(actualAnnotationValue);
            Assert.Equal(1, actualAnnotationValue[0]);
            Assert.Equal(2, actualAnnotationValue[1]);
        }

        [Fact]
        public void GetConstantAnnotationTargetingEntitySet()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, string>(
                () => defaultContainer.PeoplePlus, DescriptionV1, out annotationValue);
            Assert.True(result);
            Assert.Equal("Entity set of Person", annotationValue);
        }

        [Fact]
        public void GetRecordAnnotationTargetingEntitySet()
        {
            SearchRestrictionsTypePlus annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, SearchRestrictionsTypePlus>(
                () => defaultContainer.PeoplePlus, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotationValue);
            Assert.True(result);
            Assert.Equal(true, annotationValue.SearchablePlus);
            Assert.Equal(SearchExpressionsPlus.NonePlus | SearchExpressionsPlus.NOTPlus, annotationValue.UnsupportedExpressionsPlus);
        }

        [Fact]
        public void GetAnnotationTargetingSingleton()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuerySingle<PersonPlus>>, string>(
                () => defaultContainer.MePlus, DescriptionV1, out annotationValue);
            Assert.True(result);
            Assert.Equal("Singleton of Person", annotationValue);
        }

        [Fact]
        public void GetRecordAnnotationTargetingSingleton()
        {
            SearchRestrictionsTypePlus annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuerySingle<PersonPlus>>, SearchRestrictionsTypePlus>(
                () => defaultContainer.MePlus, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotationValue);
            Assert.True(result);
            Assert.Equal(true, annotationValue.SearchablePlus);
            Assert.Equal(SearchExpressionsPlus.NonePlus | SearchExpressionsPlus.NOTPlus, annotationValue.UnsupportedExpressionsPlus);
        }
        #endregion

        #region Path Record Collection Annotation

        [Fact]
        public void GetRecordAnnotationWithPathAndCollectionTargetingEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<RecordAnnotationTypePlus>(
                et,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Equal("ET", annotationValue.NamePlus);
            Assert.Single(annotationValue.OtherPropertiesPlus);
            Assert.Equal("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.Empty(annotationValue.CollectionOfCTPPlus);
            Assert.Empty(annotationValue.CollectionOfDerivedCTPPlus);
        }

        [Fact]
        public void GetOverridedRecordAnnotationWithPathAndCollectionTargetingDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<RecordAnnotationTypePlus>(
                et,
                RecordAnnotationTerm,
                out annotationValue);

            // Annotation override : Derived entity type contains same annotation with that of the base entity type
            Assert.True(result);
            Assert.Equal("DerivedET", annotationValue.NamePlus);
            Assert.Equal(3, annotationValue.OtherPropertiesPlus.Count());
            Assert.Equal("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.Equal("DerivedCT", annotationValue.OtherPropertiesPlus[1]);
            Assert.Equal("DerivedCT Description", annotationValue.OtherPropertiesPlus[2]);
            Assert.Equal(2, annotationValue.CollectionOfCTPPlus.Count());
            Assert.Equal("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);
            Assert.Equal("DerivedCT", annotationValue.CollectionOfCTPPlus[1].NamePlus);
            Assert.Single(annotationValue.CollectionOfDerivedCTPPlus);
            Assert.Equal("DerivedCT", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
        }

        [Fact]
        public void GetRecordAnnotationWithPathAndCollectionTargetingPropertyInEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.ComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Equal("ET", annotationValue.NamePlus);
            Assert.Empty(annotationValue.OtherPropertiesPlus);
            Assert.Single(annotationValue.CollectionOfCTPPlus);
            Assert.Equal("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);
            Assert.Empty(annotationValue.CollectionOfDerivedCTPPlus);
        }

        [Fact]
        public void GetRecordAnnotationWithPathAndCollectionTargetingPropertyInDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.ComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Equal("DerivedET", annotationValue.NamePlus);
            Assert.Empty(annotationValue.OtherPropertiesPlus);
            Assert.Single(annotationValue.CollectionOfCTPPlus);

            // With type cast <Path>ComplexP/OperationTestService.DerivedCT</Path>
            Assert.Equal("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);

            Assert.Empty(annotationValue.CollectionOfDerivedCTPPlus);

            et.ComplexPPlus = new DerivedCTPlus()
            {
                NamePlus = "DerivedCT",
                DescriptionPlus = "DerivedCT Description"
            };

            annotationValue = null;
            result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.ComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.Single(annotationValue.CollectionOfDerivedCTPPlus);
            Assert.Equal("DerivedCT", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
            Assert.Equal("DerivedCT Description", annotationValue.CollectionOfDerivedCTPPlus[0].DescriptionPlus);
        }

        [Fact]
        public void GetRecordAnnotationWithPathAndCollectionTargetingDerivedComplexTypePropertyInDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DerivedCTPlus>, RecordAnnotationTypePlus>(
                () => et.DerivedComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Equal("DerivedET", annotationValue.NamePlus);
            Assert.Single(annotationValue.OtherPropertiesPlus);
            Assert.Equal("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.Single(annotationValue.CollectionOfCTPPlus);
            Assert.Equal("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);

            // With type cast <Path>ComplexP/OperationTestService.DerivedCT</Path>
            Assert.Empty(annotationValue.CollectionOfDerivedCTPPlus);

            et.ComplexPPlus = new DerivedCTPlus
            {
                NamePlus = "DerivedCT1",
                DescriptionPlus = "DerivedCT1 Description"
            };

            annotationValue = null;
            result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.DerivedComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.Single(annotationValue.CollectionOfDerivedCTPPlus);
            Assert.Equal("DerivedCT1", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
            Assert.Equal("DerivedCT1 Description", annotationValue.CollectionOfDerivedCTPPlus[0].DescriptionPlus);
        }

        [Fact]
        public void GetRecordAnnotationWithoutExpressionTargetingDerivedComplexType()
        {
            RecordAnnotationTypePlus annotationValue = null;

            DerivedETPlus et = CreateDerivedET();
            var result = operationTestServiceContainer.TryGetAnnotation(
                et.DerivedComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);
            // For annotations embedded within a property of an entity type or complex type,
            // the path expression is evaluated starting at the directly enclosing type.
            // This allows e.g. specifying the value of an annotation on one property to be
            // calculated from values of other properties of the same type.
            Assert.True(result);
            Assert.Null(annotationValue.NamePlus);
            Assert.Empty(annotationValue.OtherPropertiesPlus);
        }

        [Fact]
        public void GetCollectionAnnotationWithPathOfCollectionNavigationPropertyTargetingEntityType()
        {
            IEnumerable<ETPlus> annotationValue = null;

            ETPlus et = CreateET();
            et.NavPPlus = new DataServiceCollection<ETPlus>(operationTestServiceContainer, "ETSets", null, null);
            et.NavPPlus.Add(CreateET());
            et.NavPPlus.Add(CreateDerivedET());

            var result = operationTestServiceContainer.TryGetAnnotation(
                et,
                NavAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Equal(2, annotationValue.Count());
            Assert.Equal("ET", annotationValue.ElementAt(0).UserNamePlus);
            Assert.Equal("DerivedET", annotationValue.ElementAt(1).UserNamePlus);
        }

        [Fact]
        public void GetCollectionAnnotationWithCollectionOfPathOfSingleNavigationPropertyTargetingEntityType()
        {
            IEnumerable<ETPlus> annotationValue = null;
            DerivedETPlus et = CreateDerivedET();
            et.SingleNavPPlus = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<IEnumerable<ETPlus>>(
                et,
                NavAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Single(annotationValue);
            Assert.Equal("ET", annotationValue.ElementAt(0).UserNamePlus);
        }

        [Fact]
        public void GetCollectionAnnotationWithPathContainingTypeCastTargetingNavigation()
        {
            IEnumerable<DerivedETPlus> annotationValue = null;

            ETPlus et = CreateET();
            et.NavPPlus = new DataServiceCollection<ETPlus>(operationTestServiceContainer, "ETSets", null, null);
            et.NavPPlus.Add(CreateET());
            et.NavPPlus.Add(CreateDerivedET());
            var nav1 = CreateDerivedET();
            nav1.NavPPlus = null;
            et.NavPPlus.Add(nav1);
            var nav2 = CreateDerivedET();
            nav2.NavPPlus = new DataServiceCollection<ETPlus>();
            et.NavPPlus.Add(nav2);

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceCollection<ETPlus>>, IEnumerable<DerivedETPlus>>(
                () => et.NavPPlus,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            // Only return the navigation property of derived Type 
            // <Path>NavP/OperationTestService.DerivedET</Path>
            Assert.True(result);
            Assert.Equal(3, annotationValue.Count());
            Assert.Equal("DerivedET", annotationValue.ElementAt(0).UserNamePlus);
            Assert.Null(annotationValue.ElementAt(1).NavPPlus);
            Assert.Empty(annotationValue.ElementAt(2).NavPPlus);
        }

        [Fact]
        public void GetCollectionAnnotationWithPathOfSingleNavigationPropertyContainingTypeCastTargetingNavigation()
        {
            IEnumerable<DerivedETPlus> annotationValue = null;

            DerivedETPlus et = CreateDerivedET();
            et.SingleNavPPlus = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlus>, IEnumerable<DerivedETPlus>>(
                () => et.SingleNavPPlus,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Empty(annotationValue);

            et.SingleNavPPlus = CreateDerivedET();

            result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlus>, IEnumerable<DerivedETPlus>>(
                () => et.SingleNavPPlus,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            Assert.True(result);
            Assert.Single(annotationValue);
            Assert.True(annotationValue.ElementAt(0) is DerivedETPlus);
            Assert.Equal("DerivedET", annotationValue.ElementAt(0).UserNamePlus);
        }

        [Fact]
        public void GetPathAnnotationWithDollarCountTargetingEntityType()
        {
            int annotationValue;

            ETPlus et = CreateET();
            et.NavPPlus = new DataServiceCollection<ETPlus>(operationTestServiceContainer, "ETSets", null, null);
            et.NavPPlus.Add(CreateET());
            et.NavPPlus.Add(CreateDerivedET());

            var result = operationTestServiceContainer.TryGetAnnotation(
                et,
                "OperationTestService.NavCountAnnotation",
                out annotationValue);

            Assert.True(result);
            Assert.Equal(2, annotationValue);
        }

        [Fact]
        public void GetPathAnnotationWithTermCastTargetingEntityType()
        {
            string annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation(
                et,
                "OperationTestService.TermCastAnnotation",
                out annotationValue);

            Assert.True(result);
            Assert.Equal("ET", annotationValue);
        }

        [Fact]
        public void GetPathAnnotationWithNavigationPropertyAndTermCastTargetingEntityType()
        {
            IEnumerable<DerivedETPlus> annotationValue = null;

            ETPlus et = CreateET();
            et.NavPPlus = new DataServiceCollection<ETPlus>(operationTestServiceContainer, "ETSets", null, null);
            et.NavPPlus.Add(CreateET());
            et.NavPPlus.Add(CreateDerivedET());

            var result = operationTestServiceContainer.TryGetAnnotation(
                et,
                "OperationTestService.NavOfDerivedETAnnotation",
                "EntityType",
                out annotationValue);

            Assert.True(result);
            Assert.NotNull(annotationValue);
            Assert.Single(annotationValue);
        }

        #endregion Path Record Collection Annotation

        #region External Targeting annotation

        [Fact]
        public void GetExternalTargetingAnnotationsForEntityType()
        {
            Action<ETPlus> action = (et) =>
            {
                IEnumerable<CTPlus> collectionOfCTPropertyAnnotation = null;

                var result = operationTestServiceContainer.TryGetAnnotation(
                    et,
                    CollectionOfCTPropertyAnnotation,
                    ExternalTargetingQualifier,
                    out collectionOfCTPropertyAnnotation);

                Assert.True(result);
                Assert.Single(collectionOfCTPropertyAnnotation);
                Assert.Equal("CT", collectionOfCTPropertyAnnotation.ElementAt(0).NamePlus);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [Fact]
        public void GetExternalTargetingAnnotationsForProperty()
        {
            Action<ETPlus> action = (et) =>
            {
                et.ComplexPPlus = new DerivedCTPlus()
                {
                    NamePlus = "DerviedCT",
                    DescriptionPlus = "DerviedCT Description"
                };

                IEnumerable<DerivedCTPlus> collectionOfCTPropertyAnnotation = null;

                var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, IEnumerable<DerivedCTPlus>>(
                    () => et.ComplexPPlus,
                    "OperationTestService.CollectionOfDerviedCTAnnotation",
                    ExternalTargetingQualifier,
                    out collectionOfCTPropertyAnnotation);

                Assert.True(result);
                Assert.Single(collectionOfCTPropertyAnnotation);
                Assert.Equal("DerviedCT", collectionOfCTPropertyAnnotation.ElementAt(0).NamePlus);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [Fact]
        public void GetExternalTargetingAnnotationForFunctionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundFunctionWithParameterPlus(s1),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("TestExternalTargetingAnnotationOnFunctionImport", annotationValue);
        }

        [Fact]
        public void GetExternalTargetingAnnotationForActionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundActionWithParameterPlus", new Type[] { typeof(string) }),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("TestExternalTargetingAnnotationOnActionImport", annotationValue);
        }

        [Fact]
        public void GetExternalTargetingAnnotationForEntitySetByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetProperty("DerivedETSetsPlus"),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("TestExternalTargetingAnnotationOnDerivedEntitySets", annotationValue);
        }

        [Fact]
        public void GetExternalTargetingAnnotationForSingleton()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlusSingle>, string>(
                () => operationTestServiceContainer.SingleETPlus,
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.True(result);
            Assert.Equal("TestExternalTargetingAnnotationOnSingleton", annotationValue);
        }

        #endregion

        #region NonExisting Annotation

        [Fact]
        public void GetNonExistingAnnotationTargetingType()
        {
            Action<ETPlus> action = (et) =>
            {
                string annotationValue = null;
                var result = operationTestServiceContainer.TryGetAnnotation(
                    et,
                    NonExistingTermName,
                    out annotationValue);
                Assert.False(result);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [Fact]
        public void GetNonExistingAnnotationTargetingPropertyInfo()
        {
            Action<ETPlus> action = (et) =>
            {
                string annotationValue = null;
                var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, string>(
                    () => et.ComplexPPlus,
                    NonExistingTermName,
                    out annotationValue);
                Assert.False(result);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [Fact]
        public void GetNonExistingAnnotationTargetingBoundFunction()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                NonExistingTermName,
                out annotationValue);
            Assert.False(result);
        }

        [Fact]
        public void GetNonExistingAnnotationTargetingBoundFunctionCalledFromDerivedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                NonExistingTermName,
                out annotationValue);
            Assert.False(result);
        }

        [Fact]
        public void GetNonExistingAnnotationTargetingFunctionBoundToCollectionOfEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.False(result);

            result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.False(result);
        }

        [Fact]
        public void GetNonExistingAnnotationTargetingFunctionBoundToEntityAsExtensionMethod()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.SingleETPlus.FunctionWithParameterPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.False(result);
        }

        #endregion

        private ETPlus CreateET()
        {
            return new ETPlus()
            {
                UserNamePlus = "ET",
                ComplexPPlus = new CTPlus
                {
                    NamePlus = "CT"
                }
            };
        }

        private DerivedETPlus CreateDerivedET()
        {
            return new DerivedETPlus()
            {
                UserNamePlus = "DerivedET",
                ComplexPPlus = new CTPlus
                {
                    NamePlus = "CT"
                },
                DerivedComplexPPlus = new DerivedCTPlus
                {
                    NamePlus = "DerivedCT",
                    DescriptionPlus = "DerivedCT Description"
                }
            };
        }

        private PersonPlus CreatePerson()
        {
            return new PersonPlus()
            {
                UserNamePlus = "NelsonW",
                FirstNamePlus = "Nelson",
                LastNamePlus = "White",
                GenderPlus = PersonGenderPlus.FemalePlus,
                LocationPlus = new LocationPlus { AddressPlus = "Lianhua Road", CityPlus = new CityPlus() { CountryRegionPlus = "China", NamePlus = "Shanghai" } },
                CompanyLocationPlus = new CompanyLocationPlus { AddressPlus = "Zixing Road", CityPlus = new CityPlus() { CountryRegionPlus = "China", NamePlus = "Beijing" } }
            };
        }

        private VipCustomerPlus CreateVipCustomer()
        {
            return new VipCustomerPlus()
            {
                UserNamePlus = "TomN",
                FirstNamePlus = "Tom",
                LastNamePlus = "Nolan ",
                GenderPlus = PersonGenderPlus.MalePlus,
                LocationPlus = new LocationPlus { AddressPlus = "Xuanwu Street", CityPlus = new CityPlus() { CountryRegionPlus = "China", NamePlus = "Nanjing" } },
                CompanyLocationPlus = new CompanyLocationPlus { AddressPlus = "Wudong Road", CityPlus = new CityPlus() { CountryRegionPlus = "China", NamePlus = "Suzhou" } },
                VipNumberPlus = 1000
            };
        }
    }
}