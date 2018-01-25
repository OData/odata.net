//---------------------------------------------------------------------
// <copyright file="ClientMetadataAnnotationWithPOCO.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientMetadataAnnotationWithPOCO
    {
        private const string ExternalTargetingQualifier = "ExternalTargeting";
        private const string DescriptionV1 = "Org.OData.Core.V1.Description";
        private const string RecordAnnotation = "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.RecordAnnotation";
        private const string NavOfDerivedETAnnotationTerm = "Microsoft.OData.Client.TDDUnitTests.Tests.Annotation.UserDefinedClientProxy.NavOfDerivedETAnnotation";

        private UserDefinedServiceContainer dsc = new UserDefinedServiceContainer(new Uri("http://UserDefinedService"));

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void GetAnnotationTargetingBoundFunction()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<ET, QueryOperationResponse<CT>>, string>(
                (et) => dsc.FunctionWithoutParameter(et),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionWithoutParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionBoundToCollection()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<ICollection<ET>, QueryOperationResponse<CT>>, string>(
                (ets) => dsc.GetAllCTsOfETSets(ets),
                DescriptionV1,
                out annotationValue);
            Assert.IsTrue(result);
            Assert.AreEqual("FunctionBoundToCollectionOfEntity", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingBoundAction()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<ET, OperationResponse>, string>(
                (et) => dsc.ActionWithoutReturnType(et),
                DescriptionV1,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ActionWithoutReturnType", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingFunctionImport()
        {
            string annotationValue = null;
            var result = dsc.TryGetAnnotation<Func<ET, OperationResponse>, string>(
                (et) => dsc.UnboundFunctionWithParameter(et),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnFunctionImport", annotationValue);

            annotationValue = null;
            result = dsc.TryGetAnnotation<Func<ET, OperationResponse>, string>(
                (et) => dsc.UnboundFunctionWithParameter(et),
                DescriptionV1,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("FunctionImportWithParameter", annotationValue);
        }

        [TestMethod]
        public void GetAnnotationTargetingActionImport()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<ET, OperationResponse>, string>(
                (et) => dsc.UnboundActionWithParameter(et),
                DescriptionV1,
                ExternalTargetingQualifier,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnActionImport", annotationValue);

            annotationValue = null;

            result = dsc.TryGetAnnotation<Func<ET, OperationResponse>, string>(
                (et) => dsc.UnboundActionWithParameter(et),
                DescriptionV1,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ActionImportWithParameter", annotationValue);
        }
#endif

        [TestMethod]
        public void GetAnnotationTargetingType()
        {
            RecordAnnotationType annotationValue = null;

            ET et = new ET() { UserName = "ET", ComplexP = new CT { Name = "CT" } };
            var result = dsc.TryGetAnnotation(
                et,
                RecordAnnotation,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ET", annotationValue.Name);
            Assert.AreEqual(1, annotationValue.OtherProperties.Count);
            Assert.AreEqual("CT", annotationValue.OtherProperties.ElementAt(0));
        }

        [TestMethod]
        public void GetAnnotationTargetingPropertyInfo()
        {
            RecordAnnotationType annotationValue = null;

            DerivedET et = new DerivedET()
            {
                UserName = "ET",
                ComplexP = new DerivedCT { Name = "DerivedCT", Description = "DerivedET Description" },
                DerivedComplexP = new DerivedCT { Name = "DerivedCT1", Description = "DerivedET1 Description" }
            };

            var result = dsc.TryGetAnnotation<Func<DerivedCT>, RecordAnnotationType>(
                () => et.DerivedComplexP,
                RecordAnnotation,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("ET", annotationValue.Name);
            Assert.AreEqual(1, annotationValue.OtherProperties.Count);
            Assert.AreEqual("DerivedCT", annotationValue.OtherProperties.ElementAt(0));
            Assert.AreEqual(1, annotationValue.CollectionOfCTP.Count);
            Assert.AreEqual("DerivedCT", annotationValue.CollectionOfCTP.ElementAt(0).Name);
            Assert.AreEqual(1, annotationValue.CollectionOfDerivedCTP.Count);
            var derivedCT = annotationValue.CollectionOfCTP.ElementAt(0) as DerivedCT;
            Assert.AreEqual("DerivedET Description", derivedCT.Description);
        }

        [TestMethod]
        public void GetAnnotationTargetingNavigationProperty()
        {
            IEnumerable<DerivedET> annotationValue = null;

            DerivedET et = new DerivedET()
            {
                UserName = "ET",
                ComplexP = new DerivedCT { Name = "DerivedCT", Description = "DerivedET Description" },
                DerivedComplexP = new DerivedCT { Name = "DerivedCT1", Description = "DerivedET1 Description" },
                NavP = new List<ET>
                {
                    new DerivedET()
                    {
                        UserName = "DerivedET1",
                        ComplexP = new CT { Name = "DerivedCT" },
                        DerivedComplexP = new DerivedCT { Name = "DerivedCT1", Description = "DerivedET1 Description" },
                        NavP = null,
                    },
                    new DerivedET()
                    {
                        UserName = "DerivedET2",
                        ComplexP = new CT { Name = "DerivedCT" },
                        NavP = new List<ET>()
                    },
                    new DerivedET()
                    {
                        UserName = "DerivedET2",
                        ComplexP = new CT { Name = "DerivedCT" }
                    },
                    new ET()
                    {
                        UserName = "ET1",
                        ComplexP = new DerivedCT { Name = "DerivedCT", Description = "DerivedET Description" },
                    }
                }
            };

            var result = dsc.TryGetAnnotation<Func<ICollection<ET>>, IEnumerable<DerivedET>>(
                () => et.NavP,
                NavOfDerivedETAnnotationTerm,
                out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual(3, annotationValue.Count());
            Assert.AreEqual("DerivedCT1", annotationValue.ElementAt(0).DerivedComplexP.Name);
            Assert.IsNull(annotationValue.ElementAt(0).NavP);
            Assert.AreEqual(0, annotationValue.ElementAt(1).NavP.Count);
            Assert.IsNull(annotationValue.ElementAt(2).DerivedComplexP);
        }

        [TestMethod]
        public void GeAnnotationTargetingEntitySet()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<IQueryable<ET>>, string>(
                           () => dsc.DerivedETSets,
                           DescriptionV1,
                           ExternalTargetingQualifier,
                           out annotationValue);

            Assert.IsTrue(result);
            Assert.AreEqual("TestExternalTargetingAnnotationOnDerivedEntitySets", annotationValue);
        }

        [TestMethod]
        public void GeAnnotationForNonExistingEntitySet()
        {
            string annotationValue = null;

            var result = dsc.TryGetAnnotation<Func<IQueryable<ET>>, string>(
                           () => dsc.NonExistingETSets,
                           DescriptionV1,
                           ExternalTargetingQualifier,
                           out annotationValue);

            Assert.IsFalse(result);
        }
    }
}