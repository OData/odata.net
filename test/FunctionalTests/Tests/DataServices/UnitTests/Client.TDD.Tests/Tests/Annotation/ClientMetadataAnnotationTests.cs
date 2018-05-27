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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Edm = Microsoft.OData.Edm;
    using OperationProxy = Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.AnnotationTargetingOperationTestsProxy;

    [TestClass]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
            Assert.IsTrue(result);
            if (annotationValue is byte[])
            {
                var binaryValue = annotationValue as byte[];
                var expectedBinaryValue = expectedValue as byte[];
                Assert.IsTrue(binaryValue.SequenceEqual(expectedBinaryValue));
            }
            else
            {
                Assert.AreEqual(expectedValue, annotationValue);
            }
        }

        #endregion Constant annotation

        #region Annotation targeting Edm type by using object instance

        [TestMethod]
        public void GetConstantAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            string description = null;
            var result = defaultContainer.TryGetAnnotation(person, DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("A person contains basic properties", description);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus vipCustomer = CreateVipCustomer();
            string description = null;
            var result = defaultContainer.TryGetAnnotation(vipCustomer, DescriptionV1, out description);
            Assert.AreEqual("A person contains basic properties", description);
        }

        [TestMethod]
        public void GetPathAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            string billingAddress = null;
            string CompanyAddress = null;

            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.BillingAddress", out billingAddress);
            Assert.IsTrue(result);
            Assert.AreEqual("Lianhua Road", billingAddress);

            result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.CompanyAddress", out CompanyAddress);
            Assert.IsTrue(result);
            Assert.AreEqual("Zixing Road", CompanyAddress);
        }

        [TestMethod]
        public void GetPathAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus person = CreateVipCustomer();

            string billingAddress = null;
            string CompanyAddress = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.BillingAddress", out billingAddress);
            Assert.IsTrue(result);
            Assert.AreEqual("Xuanwu Street", billingAddress);
            result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.CompanyAddress", out CompanyAddress);
            Assert.IsTrue(result);
            Assert.AreEqual("Wudong Road", CompanyAddress);
        }

        [TestMethod]
        public void GetRecordAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();

            PersonInPublicViewPlus personInPublicView = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.PersonView", out personInPublicView);
            Assert.IsTrue(result);
            Assert.IsNotNull(personInPublicView);
            Assert.AreEqual("NelsonW", personInPublicView.DisplayNamePlus);
            Assert.AreEqual(PersonGenderPlus.FemalePlus, personInPublicView.MaleOrFemalePlus);
        }

        [TestMethod]
        public void GetRecordAnnotationTargetingDerivedEntityType()
        {
            VipCustomerPlus person = CreateVipCustomer();

            PersonInPublicViewPlus personInPublicView = null;
            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.PersonView", out personInPublicView);
            Assert.IsTrue(result);
            Assert.IsNotNull(personInPublicView);
            Assert.AreEqual("TomN", personInPublicView.DisplayNamePlus);
            Assert.AreEqual(PersonGenderPlus.MalePlus, personInPublicView.MaleOrFemalePlus);
        }

        [TestMethod]
        public void GetCollectionAnnotationTargetingEntityType()
        {
            PersonPlus person = CreatePerson();
            IEnumerable<string> seoTerms = null;

            var result = defaultContainer.TryGetAnnotation(person, "Microsoft.OData.SampleService.Models.TripPin.SeoTerms", out seoTerms);
            Assert.IsTrue(result);
            Assert.IsNotNull(seoTerms);
            Assert.AreEqual(2, seoTerms.Count());
            Assert.AreEqual("Nelson", seoTerms.ElementAt(0));
            Assert.AreEqual("NelsonW", seoTerms.ElementAt(1));
        }

        [TestMethod]
        public void GetPathAnnotationTargetingComplexType()
        {
            PersonPlus person = CreatePerson();
            string cityName = null;

            var result = defaultContainer.TryGetAnnotation(person.LocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityName);
            Assert.IsTrue(result);
            Assert.AreEqual("Shanghai", cityName);
        }

        [TestMethod]
        public void GetPathAnnotationTargetingDerivedComplexType()
        {
            PersonPlus person = CreatePerson();
            string cityName = null;

            var result = defaultContainer.TryGetAnnotation(person.CompanyLocationPlus, "Microsoft.OData.SampleService.Models.TripPin.CityName", out cityName);
            Assert.IsTrue(result);
            Assert.AreEqual("Beijing", cityName);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingEnumType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(person.GenderPlus, DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("This Enum type indicates a person's gender", description);
        }

        #endregion Annotation targeting Type

        #region Annotation targeting Edm type by using Type

        [TestMethod]
        public void GetConstantAnnotationTargetingEntityTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(PersonPlus), DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("A person contains basic properties", description);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingDerivedEntityTypeByUsingType()
        {
            VipCustomerPlus vipCustomer = CreateVipCustomer();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(VipCustomerPlus), DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("A person contains basic properties", description);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingComplexTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(CompanyLocationPlus), DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("Company name for this person", description);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingEnumTypeByUsingType()
        {
            PersonPlus person = CreatePerson();
            string description = null;

            var result = defaultContainer.TryGetAnnotation(typeof(PersonGenderPlus), DescriptionV1, out description);
            Assert.IsTrue(result);
            Assert.AreEqual("This Enum type indicates a person's gender", description);
        }

        #endregion Annotation targeting Type

        #region Annotation targeting bound function

        [TestMethod]
        public void GetAnnotationTargetingBoundFunction()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithoutParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundFunctionCalledFromDerivedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithoutParameterBoundToDerivedET", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundFunctionWithParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s) => et.FunctionWithParameterPlus(s),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingOverridedBoundFunctionWithParameterOverride()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => et.FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityTypeWithDerviedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => et.FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionOfEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionofEntityCalledFromCollectionofDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfDerivedEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionofBaseEntityCalledFromCollectionofDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfBaseEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfBaseEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToEntityByCallingExtensionMethods()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.SingleETPlus.FunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityByCallingExtensionMethodsWithDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.ByKey("userName").FunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameterBoundToDerivedET", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityByCallingExtensionMethodsWithDerivedEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1, s2) => operationTestServiceContainer.DerivedETSetsPlus.ByKey("userName").FunctionWithParameterPlus(s1, s2),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        #endregion

        #region Annotation targeting bound function by using MethodInfo

        [TestMethod]
        public void GetAnnotationTargetingBoundFunctionByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithoutParameterPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithoutParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(DerivedETPlus).GetMethod("FunctionWithoutParameterPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithoutParameterBoundToDerivedET", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionWithParameterBoundToEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingOverloadedFunctionWithParameterBoundToEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityTypeCalledFromDerviedEntityTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(DerivedETPlus).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionOfEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfEntityPlus",
                new Type[] { typeof(DataServiceQuery<ETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionofEntityCalledFromCollectionofDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfEntityPlus",
                new Type[] { typeof(DataServiceQuery<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfDerivedEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollectionofBaseEntityCalledFromCollectionofDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod(
                "FunctionBoundToCollectionOfBaseEntityPlus",
                new Type[] { typeof(DataServiceQuery<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfBaseEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToEntityByCallingExtensionMethodsByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<ETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToDerivedEntityByCallingExtensionMethodsWithDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<DerivedETPlus>), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithOneParameterBoundToDerivedET", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToBaseEntityByCallingExtensionMethodsWithDerivedEntityByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationProxy.ExtensionMethods).GetMethod("FunctionWithParameterPlus", new Type[] { typeof(DataServiceQuerySingle<DerivedETPlus>), typeof(string), typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithTwoParameters", annotationValue);
        }

        #endregion

        #region Annotation targeting bound action

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithoutReturnType()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceActionQuery>, string>(
                () => et.ActionWithoutReturnTypePlus(),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithoutReturnType", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithParameterWithoutReturnType()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuery>, string>(
                (s1) => et.ActionWithParameterWithoutReturnTypePlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithParameterWithoutReturnType", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithoutParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceActionQuerySingle<CTPlus>>, string>(
                () => et.ActionWithoutParameterPlus(),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithoutParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithParameter()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuerySingle<CTPlus>>, string>(
                (s1) => et.ActionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithOneParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingOverloadedActionBoundToCollectionOfEntityType()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuery<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.ActionBoundToCollectionOfEntityPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionBoundToCollectionOfEntity", annotationValue);
        }
        #endregion Annotation targeting bound Action

        #region Annotation targeting bound action by using MethodInfo

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithoutReturnTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("ActionWithoutReturnTypePlus"),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithoutReturnType", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundActionWithParameterWithoutReturnTypeByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(ETPlus).GetMethod("ActionWithParameterWithoutReturnTypePlus", new Type[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithParameterWithoutReturnType", annotationValue);
        }

        #endregion

        #region Annotation targeting Navigation Property or Property

        [TestMethod]
        public void GetConstantAnnotationTargetingProperty()
        {
            bool annotationValue = false;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<long>, bool>(
                () => person.ConcurrencyPlus, "Org.OData.Core.V1.Computed", out annotationValue);
            Assert.IsTrue(result);
            Assert.IsTrue(annotationValue);
        }

        [TestMethod]
        public void GetPathAnnotationTargetingProperty()
        {
            string annotationValue = null;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<LocationPlus>, string>(
                () => person.LocationPlus,
                "Microsoft.OData.SampleService.Models.TripPin.CompanyCityName",
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("Beijing", annotationValue);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingNavigationProperty()
        {
            string annotationValue = null;
            PersonPlus person = CreatePerson();

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceCollection<PersonPlus>>, string>(
                () => person.FriendsPlus,
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("A friend of this person", annotationValue);
        }

        #endregion

        #region Annotation targeting navigation property or property by using PropertyInfo

        [TestMethod]
        public void GetConstantAnnotationTargetingPropertyByUsingPropertyInfo()
        {
            bool annotationValue = false;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("ConcurrencyPlus"), "Org.OData.Core.V1.Computed", out annotationValue);
            Assert.IsTrue(result);
            Assert.IsTrue(annotationValue);
        }

        [TestMethod]
        public void GetPathAnnotationTargetingPropertyByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("LocationPlus"),
                "Microsoft.OData.SampleService.Models.TripPin.CompanyCityName",
                out annotationValue);
            Assert.IsFalse(result);
            Assert.IsNull(annotationValue);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingNavigationPropertyByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation(
                typeof(PersonPlus).GetProperty("FriendsPlus"),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("A friend of this person", annotationValue);
        }
        #endregion

        #region Annotation targeting entityContainer
        [TestMethod]
        public void GetAnnotationTargetingEntityContainer()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<string>(defaultContainer, DescriptionV1, out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("TripPin Service", annotationValue);
        }
        #endregion

        #region Annotation targeting operation import

        [TestMethod]
        public void GetAnnotationTargetingFunctionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundFunctionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionImportWithParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingActionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceActionQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundActionWithParameterPlus(s1),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionImportWithParameter", annotationValue);
        }

        #endregion

        #region Annotation targeting operation import by using MethodInfo

        [TestMethod]
        public void GetAnnotationTargetingFunctionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundFunctionWithParameterPlus", new[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionImportWithParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingActionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundActionWithParameterPlus", new[] { typeof(string) }),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("ActionImportWithParameter", annotationValue);
        }

        #endregion

        #region Annotation targeting EntitySet or Singleton

        [TestMethod]
        public void GetCollectionOfConstantAnnotationTargetingEntitySet()
        {
            IEnumerable<Nullable<long>> annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, IEnumerable<Nullable<long>>>(
                () => defaultContainer.PeoplePlus,
                "Microsoft.OData.SampleService.Models.TripPin.VipCustomerWhiteList",
                out annotationValue);
            Assert.IsTrue(result);
            var actualAnnotationValue = annotationValue as List<Nullable<long>>;
            Assert.IsNotNull(actualAnnotationValue);
            Assert.AreEqual(1, actualAnnotationValue[0]);
            Assert.AreEqual(2, actualAnnotationValue[1]);
        }

        [TestMethod]
        public void GetConstantAnnotationTargetingEntitySet()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, string>(
                () => defaultContainer.PeoplePlus, DescriptionV1, out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("Entity set of Person", annotationValue);
        }

        [TestMethod]
        public void GetRecordAnnotationTargetingEntitySet()
        {
            SearchRestrictionsTypePlus annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuery<PersonPlus>>, SearchRestrictionsTypePlus>(
                () => defaultContainer.PeoplePlus, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual(true, annotationValue.SearchablePlus);
            Assert.AreEqual(SearchExpressionsPlus.NonePlus | SearchExpressionsPlus.NOTPlus, annotationValue.UnsupportedExpressionsPlus);
        }

        [TestMethod]
        public void GetAnnotationTargetingSingleton()
        {
            string annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuerySingle<PersonPlus>>, string>(
                () => defaultContainer.MePlus, DescriptionV1, out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("Singleton of Person", annotationValue);
        }

        [TestMethod]
        public void GetRecordAnnotationTargetingSingleton()
        {
            SearchRestrictionsTypePlus annotationValue = null;

            var result = defaultContainer.TryGetAnnotation<Func<DataServiceQuerySingle<PersonPlus>>, SearchRestrictionsTypePlus>(
                () => defaultContainer.MePlus, "Microsoft.OData.SampleService.Models.TripPin.SearchRestrictions", out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual(true, annotationValue.SearchablePlus);
            Assert.AreEqual(SearchExpressionsPlus.NonePlus | SearchExpressionsPlus.NOTPlus, annotationValue.UnsupportedExpressionsPlus);
        }
        #endregion

        #region Path Record Collection Annotation

        [TestMethod]
        public void GetRecordAnnotationWithPathAndCollectionTargetingEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<RecordAnnotationTypePlus>(
                et,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ET", annotationValue.NamePlus);
            Assert.AreEqual(1, annotationValue.OtherPropertiesPlus.Count());
            Assert.AreEqual("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.AreEqual(0, annotationValue.CollectionOfCTPPlus.Count());
            Assert.AreEqual(0, annotationValue.CollectionOfDerivedCTPPlus.Count());
        }

        [TestMethod]
        public void GetOverridedRecordAnnotationWithPathAndCollectionTargetingDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<RecordAnnotationTypePlus>(
                et,
                RecordAnnotationTerm,
                out annotationValue);

            // Annotation override : Derived entity type contains same annotation with that of the base entity type
            Assert.IsTrue(result);
            Assert.AreEqual("DerivedET", annotationValue.NamePlus);
            Assert.AreEqual(3, annotationValue.OtherPropertiesPlus.Count());
            Assert.AreEqual("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.AreEqual("DerivedCT", annotationValue.OtherPropertiesPlus[1]);
            Assert.AreEqual("DerivedCT Description", annotationValue.OtherPropertiesPlus[2]);
            Assert.AreEqual(2, annotationValue.CollectionOfCTPPlus.Count());
            Assert.AreEqual("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);
            Assert.AreEqual("DerivedCT", annotationValue.CollectionOfCTPPlus[1].NamePlus);
            Assert.AreEqual(1, annotationValue.CollectionOfDerivedCTPPlus.Count());
            Assert.AreEqual("DerivedCT", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
        }

        [TestMethod]
        public void GetRecordAnnotationWithPathAndCollectionTargetingPropertyInEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.ComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ET", annotationValue.NamePlus);
            Assert.AreEqual(0, annotationValue.OtherPropertiesPlus.Count());
            Assert.AreEqual(1, annotationValue.CollectionOfCTPPlus.Count());
            Assert.AreEqual("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);
            Assert.AreEqual(0, annotationValue.CollectionOfDerivedCTPPlus.Count());
        }

        [TestMethod]
        public void GetRecordAnnotationWithPathAndCollectionTargetingPropertyInDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, RecordAnnotationTypePlus>(
                () => et.ComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("DerivedET", annotationValue.NamePlus);
            Assert.AreEqual(0, annotationValue.OtherPropertiesPlus.Count());
            Assert.AreEqual(1, annotationValue.CollectionOfCTPPlus.Count());

            // With type cast <Path>ComplexP/OperationTestService.DerivedCT</Path>
            Assert.AreEqual("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);

            Assert.AreEqual(0, annotationValue.CollectionOfDerivedCTPPlus.Count());

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

            Assert.AreEqual(1, annotationValue.CollectionOfDerivedCTPPlus.Count());
            Assert.AreEqual("DerivedCT", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
            Assert.AreEqual("DerivedCT Description", annotationValue.CollectionOfDerivedCTPPlus[0].DescriptionPlus);
        }

        [TestMethod]
        public void GetRecordAnnotationWithPathAndCollectionTargetingDerivedComplexTypePropertyInDerivedEntityType()
        {
            RecordAnnotationTypePlus annotationValue = null;
            DerivedETPlus et = CreateDerivedET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DerivedCTPlus>, RecordAnnotationTypePlus>(
                () => et.DerivedComplexPPlus,
                RecordAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("DerivedET", annotationValue.NamePlus);
            Assert.AreEqual(1, annotationValue.OtherPropertiesPlus.Count());
            Assert.AreEqual("CT", annotationValue.OtherPropertiesPlus[0]);
            Assert.AreEqual(1, annotationValue.CollectionOfCTPPlus.Count());
            Assert.AreEqual("CT", annotationValue.CollectionOfCTPPlus[0].NamePlus);

            // With type cast <Path>ComplexP/OperationTestService.DerivedCT</Path>
            Assert.AreEqual(0, annotationValue.CollectionOfDerivedCTPPlus.Count());

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

            Assert.AreEqual(1, annotationValue.CollectionOfDerivedCTPPlus.Count());
            Assert.AreEqual("DerivedCT1", annotationValue.CollectionOfDerivedCTPPlus[0].NamePlus);
            Assert.AreEqual("DerivedCT1 Description", annotationValue.CollectionOfDerivedCTPPlus[0].DescriptionPlus);
        }

        [TestMethod]
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
            Assert.IsTrue(result);
            Assert.IsNull(annotationValue.NamePlus);
            Assert.AreEqual(0, annotationValue.OtherPropertiesPlus.Count());
        }

        [TestMethod]
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

            Assert.IsTrue(result);
            Assert.AreEqual(2, annotationValue.Count());
            Assert.AreEqual("ET", annotationValue.ElementAt(0).UserNamePlus);
            Assert.AreEqual("DerivedET", annotationValue.ElementAt(1).UserNamePlus);
        }

        [TestMethod]
        public void GetCollectionAnnotationWithCollectionOfPathOfSingleNavigationPropertyTargetingEntityType()
        {
            IEnumerable<ETPlus> annotationValue = null;
            DerivedETPlus et = CreateDerivedET();
            et.SingleNavPPlus = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<IEnumerable<ETPlus>>(
                et,
                NavAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual(1, annotationValue.Count());
            Assert.AreEqual("ET", annotationValue.ElementAt(0).UserNamePlus);
        }

        [TestMethod]
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
            Assert.IsTrue(result);
            Assert.AreEqual(3, annotationValue.Count());
            Assert.AreEqual("DerivedET", annotationValue.ElementAt(0).UserNamePlus);
            Assert.IsNull(annotationValue.ElementAt(1).NavPPlus);
            Assert.AreEqual(0, annotationValue.ElementAt(2).NavPPlus.Count);
        }

        [TestMethod]
        public void GetCollectionAnnotationWithPathOfSingleNavigationPropertyContainingTypeCastTargetingNavigation()
        {
            IEnumerable<DerivedETPlus> annotationValue = null;

            DerivedETPlus et = CreateDerivedET();
            et.SingleNavPPlus = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlus>, IEnumerable<DerivedETPlus>>(
                () => et.SingleNavPPlus,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual(0, annotationValue.Count());

            et.SingleNavPPlus = CreateDerivedET();

            result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlus>, IEnumerable<DerivedETPlus>>(
                () => et.SingleNavPPlus,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual(1, annotationValue.Count());
            Assert.IsTrue(annotationValue.ElementAt(0) is DerivedETPlus);
            Assert.AreEqual("DerivedET", annotationValue.ElementAt(0).UserNamePlus);
        }

        [TestMethod]
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

            Assert.IsTrue(result);
            Assert.AreEqual(2, annotationValue);
        }

        [TestMethod]
        public void GetPathAnnotationWithTermCastTargetingEntityType()
        {
            string annotationValue = null;
            ETPlus et = CreateET();

            var result = operationTestServiceContainer.TryGetAnnotation(
                et,
                "OperationTestService.TermCastAnnotation",
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ET", annotationValue);
        }

        [TestMethod]
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

            Assert.IsTrue(result);
            Assert.IsNotNull(annotationValue);
            Assert.AreEqual(1, annotationValue.Count());
        }

        #endregion Path Record Collection Annotation

        #region External Targeting annotation

        [TestMethod]
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

                Assert.IsTrue(result);
                Assert.AreEqual(1, collectionOfCTPropertyAnnotation.Count());
                Assert.AreEqual("CT", collectionOfCTPropertyAnnotation.ElementAt(0).NamePlus);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [TestMethod]
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

                Assert.IsTrue(result);
                Assert.AreEqual(1, collectionOfCTPropertyAnnotation.Count());
                Assert.AreEqual("DerviedCT", collectionOfCTPropertyAnnotation.ElementAt(0).NamePlus);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [TestMethod]
        public void GetExternalTargetingAnnotationForFunctionImport()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<CTPlus>>, string>(
                (s1) => operationTestServiceContainer.UnboundFunctionWithParameterPlus(s1),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnFunctionImport", annotationValue);
        }

        [TestMethod]
        public void GetExternalTargetingAnnotationForActionImportByUsingMethodInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetMethod("UnboundActionWithParameterPlus", new Type[] { typeof(string) }),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnActionImport", annotationValue);
        }

        [TestMethod]
        public void GetExternalTargetingAnnotationForEntitySetByUsingPropertyInfo()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation(
                typeof(OperationTestServiceContainerPlus).GetProperty("DerivedETSetsPlus"),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnDerivedEntitySets", annotationValue);
        }

        [TestMethod]
        public void GetExternalTargetingAnnotationForSingleton()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<ETPlusSingle>, string>(
                () => operationTestServiceContainer.SingleETPlus,
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnSingleton", annotationValue);
        }

        #endregion

        #region NonExisting Annotation

        [TestMethod]
        public void GetNonExistingAnnotationTargetingType()
        {
            Action<ETPlus> action = (et) =>
            {
                string annotationValue = null;
                var result = operationTestServiceContainer.TryGetAnnotation(
                    et,
                    NonExistingTermName,
                    out annotationValue);
                Assert.IsFalse(result);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [TestMethod]
        public void GetNonExistingAnnotationTargetingPropertyInfo()
        {
            Action<ETPlus> action = (et) =>
            {
                string annotationValue = null;
                var result = operationTestServiceContainer.TryGetAnnotation<Func<CTPlus>, string>(
                    () => et.ComplexPPlus,
                    NonExistingTermName,
                    out annotationValue);
                Assert.IsFalse(result);
            };

            action(CreateET());
            action(CreateDerivedET());
        }

        [TestMethod]
        public void GetNonExistingAnnotationTargetingBoundFunction()
        {
            string annotationValue = null;
            ETPlus et = new ETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                NonExistingTermName,
                out annotationValue);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetNonExistingAnnotationTargetingBoundFunctionCalledFromDerivedEntityType()
        {
            string annotationValue = null;
            DerivedETPlus et = new DerivedETPlus();

            var result = operationTestServiceContainer.TryGetAnnotation<Func<DataServiceQuerySingle<CTPlus>>, string>(
                () => et.FunctionWithoutParameterPlus(),
                NonExistingTermName,
                out annotationValue);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetNonExistingAnnotationTargetingFunctionBoundToCollectionOfEntity()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.ETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.IsFalse(result);

            result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuery<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.DerivedETSetsPlus.FunctionBoundToCollectionOfEntityPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetNonExistingAnnotationTargetingFunctionBoundToEntityAsExtensionMethod()
        {
            string annotationValue = null;

            var result = operationTestServiceContainer.TryGetAnnotation<Func<string, DataServiceQuerySingle<ETPlus>>, string>(
                (s1) => operationTestServiceContainer.SingleETPlus.FunctionWithParameterPlus(s1),
                NonExistingTermName,
                out annotationValue);
            Assert.IsFalse(result);
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