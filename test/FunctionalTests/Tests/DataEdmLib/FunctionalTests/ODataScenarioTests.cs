//---------------------------------------------------------------------
// <copyright file="ODataScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataScenarioTests : EdmLibTestCaseBase
    {
        private IEdmModel baseModel;
        private IEdmModel vocabularyDefinitionModel;
        private Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> operationsLookup;

        public ODataScenarioTests()
        {
            this.EdmVersion = EdmVersion.Latest;
        }

        private const string baseModelCsdl = @"
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";

        private const string vocabularyDefinitionModelCsdl = @"
<Schema Namespace=""bar"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Int32Value"" Type=""Int32"" />
    <Term Name=""MoreTransformedPersonTerm"" Type=""bar.MoreTransformedPerson"" />
    <EntityType Name=""TransformedPerson"">
        <Property Name=""Age"" Type=""Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""MoreTransformedPerson"" BaseType=""bar.TransformedPerson"">
        <Property Name=""Description"" Type=""String"" />
    </EntityType>
</Schema>";

        private const string largerModelCsdl = @"
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person0"" BaseType=""foo.Person9"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person1"" BaseType=""foo.Person0"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person2"" BaseType=""foo.Person1"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person3"" BaseType=""foo.Person2"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person4"" BaseType=""foo.Person3"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person5"" BaseType=""foo.Person4"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person6"" BaseType=""foo.Person5"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person7"" BaseType=""foo.Person6"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person8"" BaseType=""foo.Person7"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""Person9"" BaseType=""foo.Person8"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Blame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Shame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Declaim"" Type=""String"" Nullable=""false"" />
        <Property Name=""Tame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Came"" Type=""String"" Nullable=""false"" />
        <Property Name=""Fame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Dame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Pame"" Type=""String"" Nullable=""false"" />
        <Property Name=""Same"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";


        private void SetupModels()
        {
            this.baseModel = this.Parse(baseModelCsdl);
            this.vocabularyDefinitionModel = this.Parse(vocabularyDefinitionModelCsdl);
        }

        // ??? There is no way to control aliases for unresolved Terms
        [TestMethod]
        public void ServerScenario_NoDefinition()
        {
            this.SetupModels();

            // Added <using> to make test case pass
            const string applicationCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""foo.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel);

            IEdmModel combinedModel = new CombiningModel(this.baseModel, applicationModel);

            IEnumerable<string> actualCsdls = this.GetSerializerResult(combinedModel);

            var expectedCsdls = CsdlCombiner.GetCombinedCsdls(new[] { applicationCsdl, baseModelCsdl });
            this.CompareCsdls(expectedCsdls, actualCsdls);
        }

        [TestMethod]
        public void ServerScenario_HasDefinition()
        {
            this.SetupModels();

            const string applicationCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>";
            IEdmModel applicationModel = this.Parse(applicationCsdl, this.baseModel, this.vocabularyDefinitionModel);

            IEdmModel combinedModel = new CombiningModel(this.baseModel, applicationModel);

            IEnumerable<string> actualCsdls = this.GetSerializerResult(combinedModel);

            const string usingCsdl = @"
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
</Schema>";
            var expectedCsdls = CsdlCombiner.GetCombinedCsdls(new[] { applicationCsdl, baseModelCsdl, usingCsdl });
            this.CompareCsdls(expectedCsdls, actualCsdls);
        }

        [TestMethod]
        public void MultithreadedAnnotations()
        {
            this.SetupModels();

            IEdmEntityType person = baseModel.FindEntityType("foo.Person");
            Assert.IsNotNull(person, "Personage");

            IEdmProperty name = person.FindProperty("Name");
            Assert.IsNotNull(name, "Person Name");

            string bogus = "http://bogus.com";
            string goop = "goop";
            string droop = "droop";
            string scoop = "scoop";

            var setters = new IEdmDirectValueAnnotationBinding[]
            {
                new EdmDirectValueAnnotationBinding(person, bogus, goop, 0),
                new EdmDirectValueAnnotationBinding(person, bogus, droop, 0),
                new EdmDirectValueAnnotationBinding(person, bogus, scoop, 0),
                new EdmDirectValueAnnotationBinding(name, bogus, goop, 0),
                new EdmDirectValueAnnotationBinding(name, bogus, droop, 0),
                new EdmDirectValueAnnotationBinding(name, bogus, scoop, 0)
            };

            var getters = new IEdmDirectValueAnnotationBinding[]
            {
                new EdmDirectValueAnnotationBinding(person, bogus, goop),
                new EdmDirectValueAnnotationBinding(person, bogus, droop),
                new EdmDirectValueAnnotationBinding(person, bogus, scoop),
                new EdmDirectValueAnnotationBinding(name, bogus, goop),
                new EdmDirectValueAnnotationBinding(name, bogus, droop),
                new EdmDirectValueAnnotationBinding(name, bogus, scoop)
            };

            baseModel.SetAnnotationValues(setters);

            bool foundNonzero = false;

            ThreadIndex = 0;
            System.Threading.ParameterizedThreadStart threadStart = new System.Threading.ParameterizedThreadStart(AnnotationsThreadFunction);

            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread newThread = new System.Threading.Thread(threadStart);
                newThread.Start(baseModel);

                object[] values = baseModel.GetAnnotationValues(getters);
                int firstValue = (int)values[0];
                foreach (object value in values)
                {
                    Assert.AreEqual(firstValue, (int)value, "Person and name values");
                }
            }

            for (int i = 0; i < 10000; i++)
            {
                object[] values = baseModel.GetAnnotationValues(getters);
                int firstValue = (int)values[0];
                if (firstValue != 0)
                {
                    foundNonzero = true;
                }

                foreach (object value in values)
                {
                    Assert.AreEqual(firstValue, (int)value, "Person and name values");
                }
            }

            Assert.IsTrue(foundNonzero, "Nonzero value");
        }

        private static int ThreadIndex;

        private static void AnnotationsThreadFunction(object o)
        {
            IEdmModel model = (IEdmModel)o;

            IEdmEntityType person = model.FindEntityType("foo.Person");
            IEdmProperty name = person.FindProperty("Name");

            string bogus = "http://bogus.com";
            string goop = "goop";
            string droop = "droop";
            string scoop = "scoop";

            int index = ++ThreadIndex;

            for (int i = 0; i < 10000; i++)
            {
                var setters = new IEdmDirectValueAnnotationBinding[]
                {
                    new EdmDirectValueAnnotationBinding(person, bogus, goop, index),
                    new EdmDirectValueAnnotationBinding(person, bogus, droop, index),
                    new EdmDirectValueAnnotationBinding(person, bogus, scoop, index),
                    new EdmDirectValueAnnotationBinding(name, bogus, goop, index),
                    new EdmDirectValueAnnotationBinding(name, bogus, droop, index),
                    new EdmDirectValueAnnotationBinding(name, bogus, scoop, index)
                };

                model.SetAnnotationValues(setters);
            }
        }

        [TestMethod]
        public void MultithreadedAnnotationsOnLargerModel()
        {
            IEdmModel model = this.Parse(largerModelCsdl);

            string bogus = "http://bogus.com";
            string goop = "goop";
            string droop = "droop";
            string scoop = "scoop";

            List<IEdmDirectValueAnnotationBinding> setters = new List<IEdmDirectValueAnnotationBinding>();
            List<IEdmDirectValueAnnotationBinding> getters = new List<IEdmDirectValueAnnotationBinding>();

            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, goop, 0));
                setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, droop, 0));
                setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, scoop, 0));

                IEdmEntityType entityType = element as IEdmEntityType;
                if (entityType != null)
                {
                    foreach (IEdmStructuralProperty property in entityType.DeclaredProperties)
                    {
                        setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, goop, 0));
                        setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, droop, 0));
                        setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, scoop, 0));
                    }
                }
            };

            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                getters.Add(new EdmDirectValueAnnotationBinding(element, bogus, goop));
                getters.Add(new EdmDirectValueAnnotationBinding(element, bogus, droop));
                getters.Add(new EdmDirectValueAnnotationBinding(element, bogus, scoop));

                IEdmEntityType entityType = element as IEdmEntityType;
                if (entityType != null)
                {
                    getters.Add(new EdmDirectValueAnnotationBinding(entityType.FindProperty("Name"), bogus, goop));
                    getters.Add(new EdmDirectValueAnnotationBinding(entityType.FindProperty("Shame"), bogus, droop));
                    getters.Add(new EdmDirectValueAnnotationBinding(entityType.FindProperty("Pame"), bogus, scoop));
                }
            };

            Assert.IsTrue(setters.Count() >= 330, "Setters count");
            Assert.IsTrue(getters.Count() >= 60, "Getters count");

            model.SetAnnotationValues(setters);

            bool foundNonzero = false;

            ThreadIndexForLargerModel = 0;
            System.Threading.ParameterizedThreadStart threadStart = new System.Threading.ParameterizedThreadStart(AnnotationsThreadFunctionForLargerModel);

            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread newThread = new System.Threading.Thread(threadStart);
                newThread.Start(model);

                object[] values = model.GetAnnotationValues(getters);
                int firstValue = (int)values[0];
                foreach (object value in values)
                {
                    Assert.AreEqual(firstValue, (int)value, "Person and name values");
                }
            }

            for (int i = 0; i < 10000; i++)
            {
                object[] values = model.GetAnnotationValues(getters);
                int firstValue = (int)values[0];
                if (firstValue != 0)
                {
                    foundNonzero = true;
                }

                foreach (object value in values)
                {
                    Assert.AreEqual(firstValue, (int)value, "Person and name values");
                }
            }

            Assert.IsTrue(foundNonzero, "Nonzero value");
        }

        private static int ThreadIndexForLargerModel;

        private static void AnnotationsThreadFunctionForLargerModel(object o)
        {
            IEdmModel model = (IEdmModel)o;

            string bogus = "http://bogus.com";
            string goop = "goop";
            string droop = "droop";
            string scoop = "scoop";

            int index = ++ThreadIndexForLargerModel;

            for (int i = 0; i < 10000; i++)
            {
                List<IEdmDirectValueAnnotationBinding> setters = new List<IEdmDirectValueAnnotationBinding>();

                foreach (IEdmSchemaElement element in model.SchemaElements)
                {
                    setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, goop, index));
                    setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, droop, index));
                    setters.Add(new EdmDirectValueAnnotationBinding(element, bogus, scoop, index));

                    IEdmEntityType entityType = element as IEdmEntityType;
                    if (entityType != null)
                    {
                        foreach (IEdmStructuralProperty property in entityType.DeclaredProperties)
                        {
                            setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, goop, index));
                            setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, droop, index));
                            setters.Add(new EdmDirectValueAnnotationBinding(property, bogus, scoop, index));
                        }
                    }
                };

                model.SetAnnotationValues(setters);
            }
        }

        [TestMethod]
        public void MultithreadedPropertyAccess()
        {
            IEdmModel model = this.Parse(largerModelCsdl);
            BaseTypes = new IEdmEntityType[10];
            Properties = new IEnumerable<IEdmProperty>[10];
            PropertyTypes = new IEdmTypeReference[100];

            System.Threading.ParameterizedThreadStart threadStart = new System.Threading.ParameterizedThreadStart(PropertiesThreadFunction);

            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread newThread = new System.Threading.Thread(threadStart);
                newThread.Start(model);
            }

            int nonNullBaseTypeCount = 0;
            int nonNullPropertiesCount = 0;
            int nonNullPropertyTypeCount = 0;

            for (int i = 0; i < 10000; i++)
            {
                foreach (IEdmEntityType baseType in BaseTypes)
                {
                    Assert.AreNotEqual(BadBaseType, baseType, "Base Type");

                    if (baseType != null)
                    {
                        nonNullBaseTypeCount++;
                    }
                }

                foreach (IEnumerable<IEdmProperty> properties in Properties)
                {
                    Assert.AreNotEqual(BadProperties, properties, "Properties");

                    if (properties != null)
                    {
                        nonNullPropertiesCount++;
                    }
                }

                foreach (IEdmTypeReference type in PropertyTypes)
                {
                    Assert.AreNotEqual(BadTypeReference, type, "Property type");

                    if (type != null)
                    {
                        nonNullPropertyTypeCount++;
                    }
                }
            }

            Assert.IsTrue(nonNullBaseTypeCount > 50000, "Many base types");
            Assert.IsTrue(nonNullPropertiesCount > 50000, "Many properties enumerables");
            Assert.IsTrue(nonNullPropertyTypeCount > 500000, "Many property types");
        }

        private static IEdmEntityType[] BaseTypes;
        private static IEnumerable<IEdmProperty>[] Properties;
        private static IEdmTypeReference[] PropertyTypes;
        private readonly static IEdmEntityType BadBaseType = new EdmEntityType("badd", "bbad");
        private readonly static IEdmProperty[] BadProperties = new IEdmProperty[0];
        private readonly static IEdmTypeReference BadTypeReference = new MutableBinaryTypeReference() { IsNullable = true };

        private sealed class MutableBinaryTypeReference : IEdmBinaryTypeReference
        {
            public bool IsUnbounded
            {
                get;
                set;
            }

            public int? MaxLength
            {
                get;
                set;
            }

            public bool IsNullable
            {
                get;
                set;
            }

            public IEdmType Definition
            {
                get;
                set;
            }
        }

        private static void PropertiesThreadFunction(object o)
        {
            IEdmModel model = (IEdmModel)o;

            int index = 0;
            foreach (IEdmSchemaElement element in model.SchemaElements)
            {
                IEdmEntityType entityType = element as IEdmEntityType;
                if (entityType != null)
                {
                    IEdmEntityType baseType = entityType.BaseEntityType();

                    if (BaseTypes[index] == null)
                    {
                        BaseTypes[index] = baseType;
                    }

                    if (BaseTypes[index] != baseType)
                    {
                        BaseTypes[index] = BadBaseType;
                    }

                    IEnumerable<IEdmProperty> declaredProperties = entityType.DeclaredProperties;

                    if (Properties[index] == null)
                    {
                        Properties[index] = declaredProperties;
                    }

                    if (Properties[index] != declaredProperties)
                    {
                        Properties[index] = BadProperties;
                    }

                    int innerIndex = 0;
                    foreach (IEdmProperty property in declaredProperties)
                    {
                        IEdmTypeReference propertyType = property.Type;

                        int effectiveIndex = (index * 10) + innerIndex;
                        if (PropertyTypes[effectiveIndex] == null)
                        {
                            PropertyTypes[effectiveIndex] = propertyType;
                        }

                        if (PropertyTypes[effectiveIndex] != propertyType)
                        {
                            PropertyTypes[effectiveIndex] = BadTypeReference;
                        }

                        innerIndex++;
                    }

                    index++;
                }
            }
        }

        [TestMethod]
        public void Serialize_Customization_ParsedModel_NoDefinition()
        {
            // Added <using> to make testcase pass
            const string applicationCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel applicationModel = this.Parse(applicationCsdl);

            var annotation = applicationModel.VocabularyAnnotations.Single();
            annotation.SetSchemaNamespace(applicationModel, "anotherOne");
            annotation.SetSerializationLocation(applicationModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            IEnumerable<string> actualCsdls = this.GetSerializerResult(applicationModel);

            this.CompareCsdls(new[] { applicationCsdl }, actualCsdls);
        }

        [TestMethod]
        public void Serialize_Customization_ParsedModel_HasDefinition()
        {
            this.SetupModels();

            const string applicationCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel applicationModel = this.Parse(applicationCsdl, this.vocabularyDefinitionModel);

            // ??? this does not work
            var annotation = applicationModel.VocabularyAnnotations.Single();
            annotation.SetSchemaNamespace(applicationModel, "anotherOne");
            annotation.SetSerializationLocation(applicationModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            IEnumerable<string> actualCsdls = this.GetSerializerResult(applicationModel);

            this.CompareCsdls(new[] { applicationCsdl }, actualCsdls);
        }

        [TestMethod]
        public void ClientScenario_NoDefinition()
        {
            const string applicationCsdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Annotations Target=""foo.Person"">
        <Annotation Term=""bar.MoreTransformedPersonTerm"">
            <Record>
                <PropertyValue Property=""Description"" String=""I know it!"" />
            </Record>
        </Annotation>
    </Annotations>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel applicationModel = this.Parse(applicationCsdl);

            // query
            var person = applicationModel.FindEntityType("foo.Person");
            var annotation = person.VocabularyAnnotations(applicationModel).Single();

            Assert.AreEqual("MoreTransformedPersonTerm", annotation.Term.Name, "term name");
            Assert.AreEqual("bar", annotation.Term.Namespace, "term namespace uri");
            Assert.IsNull(annotation.Qualifier, "term qualifier");
        }

        private IEdmModel Parse(string csdl, params IEdmModel[] referencedModels)
        {
            return this.GetParserResult(new[] { csdl }, referencedModels);
        }

        private void CompareCsdls(IEnumerable<string> expectedCsdls, IEnumerable<string> actualCsdls)
        {
            var verifier = new SerializerResultVerifierUsingXml();

            var expected = expectedCsdls.Select(s => XElement.Load(new StringReader(s)));
            var actual = actualCsdls.Select(s => XElement.Load(new StringReader(s)));

            verifier.Verify(expected, actual);
        }

        private class CombiningModel : IEdmModel
        {
            private IEdmModel baseModel;
            private IEdmModel applicationModel;
            private CombiningAnnotationsManager annotationsManager;

            public CombiningModel(IEdmModel baseModel, IEdmModel applicationModel)
            {
                this.baseModel = baseModel;
                this.applicationModel = applicationModel;
                this.annotationsManager = new CombiningAnnotationsManager(baseModel, applicationModel);
            }

            public IEnumerable<IEdmSchemaElement> SchemaElements
            {
                get { return this.baseModel.SchemaElements; }
            }

            public IEnumerable<string> DeclaredNamespaces
            {
                get { return this.SchemaElements.Select(s => s.Namespace).Distinct(); }
            }

            public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
            {
                get { return this.baseModel.VocabularyAnnotations.Concat(this.applicationModel.VocabularyAnnotations); }
            }

            public IEnumerable<IEdmModel> ReferencedModels
            {
                get { return this.baseModel.ReferencedModels; }
            }

            public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
            {
                get { return this.annotationsManager; }
            }

            public IEdmEntityContainer EntityContainer
            {
                get { return this.baseModel.EntityContainer; }
            }

            public IEdmSchemaType FindDeclaredType(string qualifiedName)
            {
                return this.baseModel.FindType(qualifiedName);
            }

            public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
            {
                return this.baseModel.FindOperations(qualifiedName);
            }

            public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
            {
                return this.baseModel.FindDeclaredBoundOperations(bindingType);
            }

            public virtual IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
            {
                return this.FindDeclaredOperations(qualifiedName).Where(o => o.IsBound && o.Parameters.Any() && o.HasEquivalentBindingType(bindingType));
            }

            public IEdmTerm FindDeclaredTerm(string qualifiedName)
            {
                return this.baseModel.FindTerm(qualifiedName);
            }

            public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
            {
                return this.baseModel.FindVocabularyAnnotations(element).Concat(this.applicationModel.FindVocabularyAnnotations(element));
            }

            public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
            {
                return Enumerable.Empty<IEdmStructuredType>();
            }

            private class CombiningAnnotationsManager : IEdmDirectValueAnnotationsManager
            {
                private IEdmModel baseModel;
                private IEdmModel applicationModel;

                public CombiningAnnotationsManager(IEdmModel baseModel, IEdmModel applicationModel)
                {
                    this.baseModel = baseModel;
                    this.applicationModel = applicationModel;
                }

                public void SetAnnotationValue(IEdmElement element, string namespaceName, string localName, object value)
                {
                    this.applicationModel.DirectValueAnnotationsManager.SetAnnotationValue(element, namespaceName, localName, value);
                }

                public void SetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
                {
                    foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
                    {
                        this.SetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotation.Value);
                    }
                }

                public object GetAnnotationValue(IEdmElement element, string namespaceName, string localName)
                {
                    return this.baseModel.DirectValueAnnotationsManager.GetAnnotationValue(element, namespaceName, localName) ?? this.applicationModel.DirectValueAnnotationsManager.GetAnnotationValue(element, namespaceName, localName);
                }

                public object[] GetAnnotationValues(IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
                {
                    object[] values = new object[annotations.Count()];

                    int index = 0;
                    foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
                    {
                        values[index++] = this.GetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name);
                    }

                    return values;
                }

                public IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element)
                {
                    return this.baseModel.DirectValueAnnotationsManager.GetDirectValueAnnotations(element).Concat(this.applicationModel.DirectValueAnnotationsManager.GetDirectValueAnnotations(element));
                }
            }
        }

        private class CsdlCombiner
        {
            public static IEnumerable<string> GetCombinedCsdls(IEnumerable<string> originalCsdls)
            {
                // TODO: make sure same XmlNamespace, otherwise cannot simply combine
                var originalSchemaElements = originalCsdls.Select(c => XElement.Load(new StringReader(c)));
                XNamespace xmlns = originalSchemaElements.First().Name.Namespace;

                var namespaces = originalSchemaElements.Select(e => e.Attribute("Namespace").Value).Distinct();
                var combinedXElements = namespaces.Select(ns => new XElement(xmlns + "Schema", new XAttribute("Namespace", ns))).ToArray();

                foreach (var original in originalSchemaElements)
                {
                    // TODO: handle other attributes, like NamespaceUri
                    var combined = combinedXElements.Single(e => e.Attribute("Namespace").Value == original.Attribute("Namespace").Value);
                    combined.Add(original.Elements().ToArray());
                }

                return combinedXElements.Select(e => e.ToString());
            }
        }

        private class Person
        {
            public string Name { get; set; }
        }

        // [EdmLib] NRE thrown from Cache when compute func throws repeatedly
        [TestMethod]
        public void Exception_From_Compute_Does_Not_Make_Cache_To_Fail_With_NullRefException()
        {
            var complexType = new EdmComplexType("Bar", "Foo");
            var property = new StubEdmStructuralProperty(null) { DeclaringType = complexType };
            complexType.AddProperty(property);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    complexType.FindProperty(null);
                }
                catch (ArgumentNullException) { }
            }
        }
    }
}
