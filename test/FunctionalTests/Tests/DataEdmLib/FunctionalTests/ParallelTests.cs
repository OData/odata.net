//---------------------------------------------------------------------
// <copyright file="ParallelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal interface IParModel
    {
        IEdmModel Model { get; }
        IEdmEntityType Person { get; }
        IEdmEntitySet PersonSet { get; }
        IEdmNavigationProperty PersonNavPetCon { get; }
        IEdmNavigationProperty PersonNavPetUnknown { get; }
    }

    internal class EdmParModel : IParModel
    {
        public readonly EdmModel model;
        public readonly EdmEntityType person;
        public readonly EdmEntityType pet;
        public readonly EdmEntitySet personSet;
        public readonly EdmEntitySet petSet;
        public readonly EdmNavigationProperty personNavPet;
        public readonly EdmNavigationProperty personNavPetCon;
        public readonly EdmNavigationProperty personNavPetUnknown;

        public EdmParModel()
        {
            model = new EdmModel();
            person = new EdmEntityType("NS", "Person");
            pet = new EdmEntityType("NS", "Pet");
            model.AddElement(person);
            model.AddElement(pet);

            this.personNavPet = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "NavPet",
                Target = pet,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

            this.personNavPetCon = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "NavPetCon",
                Target = pet,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                ContainsTarget = true,
            });

            this.personNavPetUnknown = person.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "NavPetUnknown",
                Target = pet,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
            });

            var container = new EdmEntityContainer("NS", "Con");
            model.AddElement(container);
            personSet = container.AddEntitySet("PersonSet", person);
            petSet = container.AddEntitySet("PetSet", pet);

            personSet.AddNavigationTarget(this.personNavPet, petSet);
        }

        public IEdmModel Model
        {
            get { return model; }
        }

        public IEdmEntityType Person
        {
            get { return person; }
        }

        public IEdmEntitySet PersonSet
        {
            get { return personSet; }
        }

        public IEdmNavigationProperty PersonNavPetCon
        {
            get { return personNavPetCon; }
        }

        public IEdmNavigationProperty PersonNavPetUnknown
        {
            get { return this.personNavPetUnknown; }
        }
    }

    internal class CsdlParModel : IParModel
    {
        // parse from EdmParModel;
        private static readonly IEdmModel BaseModel = new EdmParModel().Model;

        public CsdlParModel()
        {
            #region parse Model
            StringBuilder builder = new StringBuilder();
            TextWriter tw = null;
            try
            {
                tw = new StringWriter(builder);
                using (var xw = XmlWriter.Create(tw))
                {
                    IEnumerable<EdmError> errors;
                    CsdlWriter.TryWriteCsdl(BaseModel, xw, CsdlTarget.OData, out errors);
                }
            }
            catch (Exception)
            {
                if (tw != null)
                {
                    tw.Dispose();
                }
            }

            TextReader tr = null;
            try
            {
                tr = new StringReader(builder.ToString());
                using (var xr = XmlReader.Create(tr))
                {
                    Model = CsdlReader.Parse(xr);
                }
            }
            catch (Exception)
            {
                if (tr != null)
                {
                    tr.Dispose();
                }
            }

            Debug.Assert(Model != null);
            #endregion

            Person = Model.FindEntityType("NS.Person");
            Debug.Assert(Person != null);

            PersonSet = Model.FindDeclaredEntitySet("PersonSet");
            Debug.Assert(PersonSet != null);

            PersonNavPetCon = Person.GetNavigationProperty("NavPetCon");
            Debug.Assert(PersonNavPetCon != null);

            PersonNavPetUnknown = Person.GetNavigationProperty("NavPetUnknown");
            Debug.Assert(PersonNavPetUnknown != null);
        }

        public IEdmModel Model { get; private set; }
        public IEdmEntityType Person { get; private set; }
        public IEdmEntitySet PersonSet { get; private set; }
        public IEdmNavigationProperty PersonNavPetCon { get; private set; }
        public IEdmNavigationProperty PersonNavPetUnknown { get; private set; }
    }

    internal class ParState
    {
        public IEdmModel Model;
    }

    internal class FindNavigationTargetState : ParState
    {
        public IEdmNavigationSource Target;
    }

    [TestClass]
    public class ParallelTests
    {
        /// <summary>
        /// The combinatorial engine to use in matrix based tests.
        /// </summary>
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [TestInitialize]
        public void Init()
        {
            CombinatorialEngineProvider = new FullCombinatorialEngineProvider();
        }

        [TestMethod]
        public void FindNavigationTargetContainmentInParallel()
        {
            this.RunParallelTestsWithParModel<FindNavigationTargetState>(
                (parModel, parState) =>
                {
                    Interlocked.CompareExchange(ref parState.Model, parModel.Model, null);
                    Assert.AreEqual(parState.Model, parModel.Model);

                    var target = parModel.PersonSet.FindNavigationTarget(parModel.PersonNavPetCon);
                    Assert.IsNotNull(target);
                    Assert.IsTrue(target is IEdmContainedEntitySet);

                    // Should point to same target.
                    Interlocked.CompareExchange(ref parState.Target, target, null);
                    Assert.AreEqual(parState.Target, target);
                });
        }

        [TestMethod]
        public void FindNavigationTargetUnknownInParallel()
        {
            this.RunParallelTestsWithParModel<FindNavigationTargetState>(
                (parModel, parState) =>
                {
                    Interlocked.CompareExchange(ref parState.Model, parModel.Model, null);
                    Assert.AreEqual(parState.Model, parModel.Model);

                    var target = parModel.PersonSet.FindNavigationTarget(parModel.PersonNavPetUnknown);
                    Assert.IsNotNull(target);
                    Assert.IsTrue(target is IEdmUnknownEntitySet);

                    // Should point to same target.
                    Interlocked.CompareExchange(ref parState.Target, target, null);
                    Assert.AreEqual(parState.Target, target);
                });
        }

        [TestMethod]
        public void ParseEnumTypeInParallel()
        {
            var color = new EdmEnumType("ns", "color");
            color.AddMember("White", new EdmEnumMemberValue(0));
            int errorCount = 0;
            Parallel.ForEach(
                Enumerable.Range(0, 20),
                index =>
                {
                    try
                    {
                        long data;
                        color.TryParseEnum("White", false, out data);
                    }
                    catch (Exception)
                    {
                        Interlocked.Increment(ref errorCount);
                    }
                });

            Assert.AreEqual(0, errorCount);
        }

        [TestMethod]
        public void FindVocabularyAnnotationInParallel()
        {
            int annotationCount = 30;
            var edmModel = new EdmParModel().Model as EdmModel;
            var container = edmModel.EntityContainer;

            for (int i = 0; i < annotationCount; i++)
            {
                EdmTerm term = new EdmTerm("NS", "Test" + i, EdmPrimitiveTypeKind.String);
                EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(
                            container,
                            term,
                            new EdmStringConstant("desc" + i));
                edmModel.AddVocabularyAnnotation(annotation);
            }

            IEdmModel loadedEdmModel = null;
            using (var ms = new MemoryStream())
            {
                var xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true });

                IEnumerable<EdmError> errors;
                var res = CsdlWriter.TryWriteCsdl(edmModel, xw, CsdlTarget.OData, out errors);
                xw.Flush();
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(ms))
                {
                    var metadata = sr.ReadToEnd();
                    loadedEdmModel = CsdlReader.Parse(XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(metadata))));
                }
            }
            container = loadedEdmModel.EntityContainer;

            int errorCount = 0;
            int totalAnnotationCount = 0;
            int taskCount = 100;

            Parallel.ForEach(
                Enumerable.Range(0, taskCount),
                index =>
                {
                    try
                    {
                        var count = loadedEdmModel.FindVocabularyAnnotations(container).ToList().Count();
                        Interlocked.Add(ref totalAnnotationCount, count);
                    }
                    catch (Exception ew)
                    {
                        Console.WriteLine(ew);
                        Interlocked.Increment(ref errorCount);
                    }
                });

            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(taskCount * annotationCount, totalAnnotationCount);
        }

        /*
         * The followings are parallel case in model building scenarios, do not consider they can be called in parallel
        [TestMethod]
        public void AddVocabularyAnnotationInParallel()
        {
            int count = 30;
            IEnumerable<IEdmVocabularyAnnotation> annotationsSample = null;

            this.RunParallelTestsWithEdmParModel(
                edmParModel =>
                {
                    EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(
                        edmParModel.Person,
                        CoreVocabularyModel.DescriptionTerm,
                        new EdmStringConstant("desc"));
                    edmParModel.model.AddVocabularyAnnotation(annotation);

                    var annotations = edmParModel.model.FindDeclaredVocabularyAnnotations(edmParModel.Person);
                    // Should point to same annotation set.
                    Interlocked.CompareExchange(ref annotationsSample, annotations, null);
                    Assert.AreEqual(annotationsSample, annotations);
                },
                count);

            Assert.IsNotNull(annotationsSample);
            Assert.AreEqual(count, annotationsSample.Count());
        }

        [TestMethod]
        public void AddElementInParallel()
        {
            int count = 100;
            IEdmModel modelSample = null;

            this.RunParallelTestsWithEdmParModel(
                edmParModel =>
                {
                    Interlocked.CompareExchange(ref modelSample, edmParModel.Model, null);
                    Assert.AreEqual(modelSample, edmParModel.Model);

                    EdmComplexType ctp = new EdmComplexType("NS", "tr");
                    edmParModel.model.AddElement(ctp);

                },
                count);

            Assert.IsNotNull(modelSample);
            Assert.AreEqual(count + 3, modelSample.SchemaElements.Count());
        }

        private void RunParallelTestsWithEdmParModel(Action<EdmParModel> runTest, int parallelLevel = 5)
        {
            var locParModel = new EdmParModel();
            if (!Debugger.IsAttached)
            {
                Parallel.ForEach(Enumerable.Range(0, parallelLevel), _ => runTest(locParModel));
            }
            else
            {
                // run one instance when debugging.
                runTest(locParModel);
            }
        }
        */

        private void RunParallelTestsWithParModel<TParState>(Action<IParModel, TParState> runTest, int parallelLevel = 5)
            where TParState : ParState, new()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new IParModel[] { new EdmParModel(), new CsdlParModel() },
                parModel =>
                {
                    var state = new TParState();
                    var locParModel = parModel;
                    if (!Debugger.IsAttached)
                    {
                        Parallel.ForEach(Enumerable.Range(0, parallelLevel), _ => runTest(locParModel, state));
                    }
                    else
                    {
                        // run one instance when debugging.
                        runTest(locParModel, state);
                    }
                });
        }
    }
}
