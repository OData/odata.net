//---------------------------------------------------------------------
// <copyright file="AnnotatableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatableTests : EdmLibTestCaseBase
    {
        private IEdmStringTypeReference stringRef = EdmCoreModel.Instance.GetString(false);

        [TestMethod]
        public void AnnotatableNullChecks()
        {
            var model = new EdmModel();

            var qq = new[] { new MyQualifiedName(null, "bar"), new MyQualifiedName("foo", null), new MyQualifiedName(null, null) };
            var annotatable = this.GetEdmAnnotatables().First();
            foreach (var q in qq)
            {
                try
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);
                    Assert.Fail("exception expected");
                }
                catch (ArgumentNullException)
                {
                }

                model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
            }
        }

        [TestMethod]
        public void AnnotableGetAnnotation_ReturnsNull_IfNotSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                foreach (string nonExist in new[] { "foo", string.Empty })
                {
                    object result = model.GetAnnotationValue(annotatable, nonExist, nonExist);
                    Assert.IsNull(result, "GetAnnotation() on {0}.", annotatable);
                }

                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void AnnotableGetAnnotation_ReturnsWhatsBeenSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                for (int i = 0; i < qualifiedNames.Count(); i++)
                {
                    MyQualifiedName q = qualifiedNames[i];
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

                    Assert.AreEqual(i + 1, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
                }

                // get
                object result = model.GetAnnotationValue(annotatable,  "_Not_Exist_", "_Not_Exist_");
                Assert.IsNull(result, "GetAnnotation() on {0}.", annotatable);

                foreach (MyQualifiedName q in qualifiedNames)
                {
                    result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName, result, "GetAnnotation() on {0}.", annotatable);

                    IEdmDirectValueAnnotation annotation = model.DirectValueAnnotations(annotatable).FirstOrDefault(a => a.NamespaceUri == q.NamespaceName && a.Name == q.Name);
                    Assert.IsNotNull(annotation, "GetAnnotation() on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName, ((IEdmDirectValueAnnotation)annotation).Value, "GetAnnotation() on {0}.", annotatable);
                }
            }
        }

        [TestMethod]
        public void AnnotableGetAnnotation_ReturnsUpdatedValue()
        {
            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();
                var model = new EdmModel();

                // set
                for (int i = 0; i < qualifiedNames.Count(); i++)
                {
                    MyQualifiedName q = qualifiedNames[i];
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

                    Assert.AreEqual(i + 1, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
                }

                // update
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName + q.MyFullName);
                    object result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName + q.MyFullName, result, "GetAnnotation() on {0}.", annotatable);
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void AnnotableGetAnnotation_ReturnsNull_IfDeleted()
        {
            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();
                var model = new EdmModel();

                // set
                for (int i = 0; i < qualifiedNames.Count(); i++)
                {
                    MyQualifiedName q = qualifiedNames[i];
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName);

                    Assert.AreEqual(i + 1, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
                }

                // delete
                for (int i = 0; i < qualifiedNames.Length; i++)
                {
                    MyQualifiedName q = qualifiedNames[i];
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);

                    Assert.AreEqual(qualifiedNames.Length - i - 1, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
                }

                model.SetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_", null);

                foreach (MyQualifiedName q in qualifiedNames)
                {
                    object result = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsNull(result, "GetAnnotation() on {0}.", annotatable);
                }

                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetAnnotation_ReturnsNull_IfNotSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                foreach (string nonExist in new[] { "foo", string.Empty })
                {
                    string result2 = model.GetAnnotationValue<string>(annotatable);
                    Assert.IsNull(result2, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                    string result3 = model.GetAnnotationValue<string>(annotatable, nonExist, nonExist);
                    Assert.IsNull(result3, "(Extension) GetAnnotation<T>(EdmTermName) on {0}.", annotatable);

                    string result4 = model.GetAnnotationValue<string>(annotatable, nonExist, nonExist);
                    Assert.IsNull(result4, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);
                }

                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetInternalAnnotation_ReturnsWhatsBeenSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                // set
                model.SetAnnotationValue(annotatable, "foo");
                model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
                model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

                Assert.AreEqual(3, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // get
                string result1 = model.GetAnnotationValue<string>(annotatable);
                Assert.AreEqual("foo", result1, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                MyQualifiedName result2 = model.GetAnnotationValue<MyQualifiedName>(annotatable);
                Assert.AreEqual(new MyQualifiedName("foo", "bar"), result2, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                IEdmType result3 = model.GetAnnotationValue<IEdmType>(annotatable);
                Assert.IsTrue(result3 is EdmComplexType, "(Extension) GetAnnotation<T>() on {0}.", annotatable);
                Assert.AreEqual("foo.foo", (result3 as IEdmSchemaType).FullName(), "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                Object result4 = model.GetAnnotationValue<Object>(annotatable);
                Assert.IsNull(result4, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                Assert.AreEqual(3, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetInternalAnnotation_ReturnsUpdatedValue()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                // set
                model.SetAnnotationValue(annotatable, "foo");
                model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
                model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

                Assert.AreEqual(3, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // update
                model.SetAnnotationValue<Object>(annotatable, "bar");
                Object result0 = model.GetAnnotationValue<Object>(annotatable);
                Assert.AreEqual(result0, "bar", "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                model.SetAnnotationValue<string>(annotatable, "updated");
                result0 = model.GetAnnotationValue<Object>(annotatable);
                Assert.AreEqual(result0, "bar", "(Extension) GetAnnotation<T>() on {0}.", annotatable);
                string result1 = model.GetAnnotationValue<string>(annotatable);
                Assert.AreEqual("updated", result1, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                model.SetAnnotationValue<IEdmType>(annotatable, new EdmEntityType("bar", "bar", null, false, false));
                IEdmType result3 = model.GetAnnotationValue<IEdmType>(annotatable);
                Assert.IsTrue(result3 is EdmEntityType, "(Extension) GetAnnotation<T>() on {0}.", annotatable);
                Assert.AreEqual("bar.bar", (result3 as IEdmSchemaType).FullName(), "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                Assert.AreEqual(4, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetInternalAnnotation_ReturnsNull_IfDeleted()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                // set
                model.SetAnnotationValue(annotatable, "foo");
                model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
                model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));

                Assert.AreEqual(3, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // delete
                model.SetAnnotationValue<Object>(annotatable, null);
                model.SetAnnotationValue<MyQualifiedName>(annotatable, null);

                string result1 = model.GetAnnotationValue<string>(annotatable);
                Assert.AreEqual("foo", result1, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                MyQualifiedName result2 = model.GetAnnotationValue<MyQualifiedName>(annotatable);
                Assert.IsNull(result2, "(Extension) GetAnnotation<T>() on {0}.", annotatable);

                Assert.AreEqual(2, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetEdmVocabularyAnnotation_ReturnsWhatsBeenSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // get
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsTrue(result1 is EdmStringConstant, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName, (result1 as IEdmStringValue).Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                    var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName, result2.Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                }

                var result3 = model.GetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_");
                Assert.IsNull(result3, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetEdmVocabularyAnnotation_ReturnsWhatsBeenSetAsSets()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                List<IEdmDirectValueAnnotationBinding> annotations = new List<IEdmDirectValueAnnotationBinding>();
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    annotations.Add(new EdmDirectValueAnnotationBinding(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName)));
                }

                model.SetAnnotationValues(annotations);

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // get
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsTrue(result1 is EdmStringConstant, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName, (result1 as IEdmStringValue).Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                    var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName, result2.Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                }

                var result3 = model.GetAnnotationValue(annotatable, "_Not_Exist_", "_Not_Exist_");
                Assert.IsNull(result3, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                object[] values = model.GetAnnotationValues(annotations);
                for (int index = 0; index < values.Length; index++)
                {
                    Assert.AreEqual(annotations[index].NamespaceUri + ":::" + annotations[index].Name, ((IEdmStringValue)values[index]).Value, "Annotation value fetched in a set.");
                }
            }
        }

        [TestMethod]
        public void ExtensionGetEdmVocabularyAnnotation_ReturnsUpdatedValue()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // update
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName + q.MyFullName));

                    var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsTrue(result1 is EdmStringConstant, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName + q.MyFullName, (result1 as IEdmStringValue).Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                    var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName + q.MyFullName, result2.Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetEdmVocabularyAnnotation_ReturnsNull_IfDeleted()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // delete
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    // ?? inconsistency with GetAnnoation: should be EdmTermName
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);
                }

                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                foreach (MyQualifiedName q in qualifiedNames)
                {
                    var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsNull(result1, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                    var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.IsNull(result2, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                }
            }
        }

        [TestMethod]
        public void ExtensionGetGeneralAnnotation_ReturnsWhatsBeenSet()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // get
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    var result1 = model.GetAnnotationValue(annotatable, q.NamespaceName, q.Name);
                    Assert.IsTrue(result1 is EdmStringConstant, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName, (result1 as IEdmStringValue).Value, "(Extension) GetAnnotation(EdmTermName) for IEdmValue on {0}.", annotatable);

                    var result2 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName, result2.Value, "(Extension) GetAnnotation<T>(EdmTermName) on {0}.", annotatable);

                    var result3 = model.GetAnnotationValue<IEdmValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.IsTrue(result3 is EdmStringConstant, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);
                    Assert.AreEqual(q.MyFullName, (result3 as IEdmStringValue).Value, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);
                }

                var result4 = model.GetAnnotationValue<IEdmValue>(annotatable, "_Not_Exist_", "_Not_Exist_");
                Assert.IsNull(result4, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetGeneralAnnotation_ReturnsUpdatedValue()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // update
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName + q.MyFullName));

                    var result1 = model.GetAnnotationValue<IEdmStringValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName + q.MyFullName, result1.Value, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);

                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, q.MyFullName + " Again");

                    var result2 = model.GetAnnotationValue<string>(annotatable, q.NamespaceName, q.Name);
                    Assert.AreEqual(q.MyFullName + " Again", result2, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetGeneralAnnotation_ReturnsNull_IfDeleted()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                // set
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);

                // delete
                foreach (MyQualifiedName q in qualifiedNames)
                {
                    // ?? this is somewhat weird!!
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, null);

                    var result = model.GetAnnotationValue<IEdmValue>(annotatable, q.NamespaceName, q.Name);
                    Assert.IsNull(result, "(Extension) GetAnnotation<T>(string, string) on {0}.", annotatable);
                }

                Assert.AreEqual(0, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void ExtensionGetGeneralAnnotation_Throws_ForNonMatchingType()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                var qualifiedNames = this.GetInterestingQualifiedNames().ToArray();

                foreach (MyQualifiedName q in qualifiedNames)
                {
                    model.SetAnnotationValue(annotatable, q.NamespaceName, q.Name, new EdmStringConstant(this.stringRef, q.MyFullName));

                    this.VerifyThrowsException(
                        typeof(InvalidOperationException), 
                        () => model.GetAnnotationValue<string>(annotatable, q.NamespaceName, q.Name));
                }

                Assert.AreEqual(qualifiedNames.Count(), model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }
        }

        [TestMethod]
        public void UsingInternalUri_Should_Throw()
        {
            var model = new EdmModel();

            foreach (IEdmElement annotatable in this.GetEdmAnnotatables())
            {
                model.GetAnnotationValue(annotatable, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "foo");
                model.SetAnnotationValue(annotatable, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "foo", "bar");
            }
        }

        [TestMethod]
        public void ExtensionSetInternalAnnotation_Validation()
        {
            var model = new EdmModel();
            var fredFlintstone = new EdmComplexType("Flintstones", "Fred");
            fredFlintstone.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(fredFlintstone);

            foreach (IEdmElement annotatable in model.SchemaElements)
            {
                // set
                model.SetAnnotationValue(annotatable, "foo");
                model.SetAnnotationValue(annotatable, new MyQualifiedName("foo", "bar"));
                model.SetAnnotationValue<IEdmType>(annotatable, new EdmComplexType("foo", "foo", null, false));
                model.SetAnnotationValue<Dictionary<string, int>>(annotatable, new Dictionary<string, int>());

                Assert.AreEqual(4, model.DirectValueAnnotations(annotatable).Count(), "Wrong # of Annotations on {0}.", annotatable);
            }

            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Model expected to be valid.");
        }

        private IEnumerable<IEdmElement> GetEdmAnnotatables()
        {
            return new IEdmElement[] {
                new EdmComplexType("", ""),
                new EdmEntityContainer("", ""),
                new EdmEntityType("", ""),
                new EdmEntitySet(new EdmEntityContainer("", ""), "", new EdmEntityType("", "")),
                EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                    new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One },
                    new EdmNavigationPropertyInfo() { Name = "", Target = new EdmEntityType("", ""), TargetMultiplicity = EdmMultiplicity.One }),
                new EdmStructuredValue(null, Enumerable.Empty<IEdmPropertyValue>()),
                new EdmStringConstant(null, ""),
                new EdmStructuralProperty(new EdmComplexType("", ""), "", EdmCoreModel.Instance.GetBoolean(true)),
            };
        }

        private IEnumerable<MyQualifiedName> GetInterestingQualifiedNames()
        {
            return new[] { 
                    new MyQualifiedName("", ""),
                    new MyQualifiedName("", "bar"),
                    new MyQualifiedName("foo", ""),
                    new MyQualifiedName("foo", "bar"),
                    new MyQualifiedName("foo2", "bar"),
                    new MyQualifiedName("foo", "bar2"),

                    // surprisingly, null also works!
                    // almineev: not any more
                    // new MyQualifiedName(null, "bar"),
                    // new MyQualifiedName("foo", null),
                    // new MyQualifiedName(null, null),
                };
        }

        private class MyQualifiedName : IEquatable<MyQualifiedName>
        {
            public MyQualifiedName(string namespaceName, string name)
            {
                this.NamespaceName = namespaceName;
                this.Name = name;
            }

            public string NamespaceName { get; set; }
            public string Name { get; set; }

            public string MyFullName
            {
                get
                {
                    var sb = new StringBuilder();
                    sb.Append(this.NamespaceName).Append(":::").Append(this.Name);
                    return sb.ToString();
                }
            }

            public override bool Equals(object obj)
            {
                var other = obj as MyQualifiedName;
                if (other == null)
                {
                    return false;
                }

                return this.Equals(other);
            }

            public bool Equals(MyQualifiedName other)
            {
                return this.NamespaceName == other.NamespaceName && this.Name == other.Name;
            }

            public override int GetHashCode()
            {
                return this.MyFullName.GetHashCode();
            }
        }
    }
}
