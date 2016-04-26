//---------------------------------------------------------------------
// <copyright file="AnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the object model's annotation infrastructure.
    /// </summary>
    [TestClass, TestCase]
    public class AnnotationTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test functionality of empty annotations.")]
        public void EmptyAnnotationsTest()
        {
            TestAnnotatable annotatable = new TestAnnotatable();
            Annotation1 retrievedOne = annotatable.GetAnnotation<Annotation1>();
            this.Assert.IsNull(retrievedOne, "Expected to not find an annotation of type Annotation1.");

            annotatable.SetAnnotation<Annotation1>(null);
            retrievedOne = annotatable.GetAnnotation<Annotation1>();
            this.Assert.IsNull(retrievedOne, "Expected to not find an annotation of type Annotation1.");
        }

        [TestMethod, Variation(Description = "Test functionality of annotations with a single entry.")]
        public void SingletonAnnotationsTest()
        {
            TestAnnotatable annotatable = new TestAnnotatable();
            Annotation1 one = new Annotation1();
            annotatable.SetAnnotation(one);

            Annotation1 retrievedOne = annotatable.GetAnnotation<Annotation1>();
            this.Assert.AreSame(one, retrievedOne,
                "Expected retrieved annotation to be reference equal with set annotation.");

            Annotation1 anotherOne = new Annotation1();
            annotatable.SetAnnotation<Annotation1>(anotherOne);
            retrievedOne = annotatable.GetAnnotation<Annotation1>();
            this.Assert.AreSame(anotherOne, retrievedOne,
                "Expected retrieved annotation to be reference equal with new annotation replacing original one.");

            Annotation2 retrievedTwo = annotatable.GetAnnotation<Annotation2>();
            this.Assert.IsNull(retrievedTwo, "Expected to not find an annotation of type Annotation2.");

            annotatable.SetAnnotation<Annotation2>(null);

            retrievedTwo = annotatable.GetAnnotation<Annotation2>();
            this.Assert.IsNull(retrievedTwo, "Expected to not find an annotation of type Annotation2.");

            annotatable.SetAnnotation<Annotation1>(null);
            retrievedOne = annotatable.GetAnnotation<Annotation1>();
            this.Assert.IsNull(retrievedOne, "Expected to not find an annotation of type Annotation1.");
        }

        [TestMethod, Variation(Description = "Test functionality of annotations with a multiple entries.")]
        public void MultipleAnnotationsTest()
        {
            // this triggers resizing the array (from 2 to 4 items) and filling it up to the last spot
            Annotation1 one = new Annotation1();
            Annotation2 two = new Annotation2();
            Annotation3 three = new Annotation3();
            Annotation4 four = new Annotation4();

            TestAnnotatable annotatable = new TestAnnotatable();
            annotatable.SetAnnotation(one);
            annotatable.SetAnnotation(two);
            annotatable.SetAnnotation(three);
            annotatable.SetAnnotation(four);

            // TODO: ckerer: call the combination engine with an annotatable with 3 and 4 elements here!

            Annotation3 retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.AreSame(three, retrievedThree,
                "Expected retrieved annotation to be reference equal with originally set one.");

            Annotation4 retrievedFour = annotatable.GetAnnotation<Annotation4>();
            this.Assert.AreSame(four, retrievedFour,
                "Expected retrieved annotation to be reference equal with originally set one.");

            annotatable.SetAnnotation<Annotation3>(null);
            retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.IsNull(retrievedThree, "Expected to not find an annotation of type Annotation3.");

            retrievedFour = annotatable.GetAnnotation<Annotation4>();
            this.Assert.AreSame(four, retrievedFour,
                "Expected retrieved annotation to be reference equal with originally set one.");

            annotatable.SetAnnotation<Annotation3>(null);
            retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.IsNull(retrievedThree, "Expected to not find an annotation of type Annotation3.");

            annotatable.SetAnnotation<Annotation4>(null);
            retrievedFour = annotatable.GetAnnotation<Annotation4>();
            this.Assert.IsNull(retrievedFour, "Expected to not find an annotation of type Annotation4.");
        }

        [TestMethod, Variation(Description = "Test functionality of annotations with a polymorphic entries.")]
        public void PolymorphicAnnotationsTest()
        {
            Annotation3 three = new Annotation3();
            Annotation3Subtype threeSub = new Annotation3Subtype();

            TestAnnotatable annotatable = new TestAnnotatable();
            annotatable.SetAnnotation(three);
            annotatable.SetAnnotation(threeSub);

            // TODO: ckerer: call the combination engine with an annotatable where the entries are reversed!

            Annotation3 retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.AreSame(three, retrievedThree,
                "Expected retrieved annotation to be reference equal with originally set one.");

            Annotation3Subtype retrievedThreeSub = annotatable.GetAnnotation<Annotation3Subtype>();
            this.Assert.AreSame(threeSub, retrievedThreeSub,
                "Expected retrieved annotation to be reference equal with originally set one.");

            annotatable.SetAnnotation<Annotation3Subtype>(null);
            retrievedThreeSub = annotatable.GetAnnotation<Annotation3Subtype>();
            this.Assert.IsNull(retrievedThreeSub, "Expected to not find an annotation of type Annotation3Subtype.");

            retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.AreSame(three, retrievedThree,
                "Expected retrieved annotation to be reference equal with originally set one.");

            annotatable.SetAnnotation<Annotation3>(null);
            retrievedThree = annotatable.GetAnnotation<Annotation3>();
            this.Assert.IsNull(retrievedThreeSub, "Expected to not find an annotation of type Annotation3Subtype.");
        }

        private sealed class TestAnnotatable : ODataAnnotatable
        {
        }

        private sealed class Annotation1
        {
        }

        private sealed class Annotation2
        {
        }

        private class Annotation3
        {
        }

        private class Annotation3Subtype : Annotation3
        {
        }

        private class Annotation4
        {
        }
    }
}
