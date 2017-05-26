//---------------------------------------------------------------------
// <copyright file="MetadataUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.XPath;
    using System;
    using System.Reflection;
    using System.Data.Test.Astoria.FullTrust;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.ModuleCore;
    using EdmError = Microsoft.OData.Edm.Validation.EdmError;

    public class RawFunctionImportContainer
    {
        public List<string> FuncAttributes;
        public string FuncName;
        public Dictionary<string, string> FuncParameters;
        public string ReturnType;

        public RawFunctionImportContainer()
        {
            FuncAttributes = new List<string>();
            FuncName = string.Empty;
            FuncParameters = new Dictionary<string, string>();
            ReturnType = string.Empty;
        }


    }

    public static class MetadataUtil
    {
        public static void CompareMetadata(IEdmModel expectedItems, IEdmModel actualItems, bool isReflectionBasedProvider, string defaultEntityContainerName)
        {
            // Check if the version is equal
            // TODO: Remove old versions of EDM and EDMX
            //AstoriaTestLog.AreEqual(expectedItems.GetEdmVersion(), actualItems.GetEdmVersion(), "Edm version did not match");

            int totalNavProps = 0;
            foreach (var edmType in expectedItems.SchemaElements.OfType<IEdmSchemaType>())
            {
                if (edmType.TypeKind == EdmTypeKind.Primitive)
                {
                    continue;
                }

                IEdmSchemaType dataWebEdmType = null;

                // For reflection based providers, there might be more than one container in the original schema
                // and hence the metadata maynot contain all the types;
                if (null == (dataWebEdmType = actualItems.FindType(edmType.FullName())))
                {
                    if (TypeIsOrphaned(edmType, expectedItems))
                    {
                        // Type is unused, so it's OK if it's not there.
                        continue;
                    }

                    continue;
                }

                int numberOfNavProps = 0;
                CompareEdmType(edmType, dataWebEdmType, isReflectionBasedProvider, out numberOfNavProps);
                totalNavProps += numberOfNavProps;
            }

            if (isReflectionBasedProvider)
            {
                AstoriaTestLog.IsTrue(totalNavProps == actualItems.SchemaElements.OfType<IEdmStructuredType>().SelectMany(t => t.DeclaredProperties).OfType<IEdmNavigationProperty>().Count(),
                    "Number of association must be equal to number of nav properties in the model");
            }

            CompareEntityContainer(expectedItems.EntityContainer, actualItems.EntityContainer);
        }

        public static void CompareMetadata(IEdmModel expectedItems, IEdmModel actualItems, bool isReflectionBasedProvider, string defaultEntityContainerName, ServiceContainer serviceContainer)
        {
            //
            //  Verify the metadata version is correct, the number of entity containers is 1, the entity
            //  container names match, and the number of entity types are the same
            //
            AstoriaTestLog.AreEqual(1.1, actualItems.GetEdmVersion(), "The metadata version was not correct");
            AstoriaTestLog.AreEqual(serviceContainer.Workspace.ContextTypeName, actualItems.EntityContainer.Name, "Entity Container names do not match");
            if (!isReflectionBasedProvider)
            {
                AstoriaTestLog.AreEqual(expectedItems.SchemaElements.OfType<IEdmEntityType>().Count(), actualItems.SchemaElements.OfType<IEdmEntityType>().Count(), "Entity Type Counts do not match");
            }

            foreach (IEdmOperationImport metadataExposedFunction in actualItems.EntityContainer.OperationImports())
            {
                AstoriaTestLog.TraceInfo("--> " + metadataExposedFunction.Name);
            }

            //
            //  For each entity type exposed through the metadata endpoint, verify the following
            //      1.  Verify it has an equivalent definition in the resource container
            //      2.  Verify the entity type name is correct
            //      3.  Verify that no navigation property is exposed in the entity type
            //      4.  Verify that the property count in the entity type
            //
            IEnumerable<ComplexType> rtTypeCollection = serviceContainer.AllTypes.Where(a => a.GetType().Name.Equals("ResourceType"));
            AstoriaTestLog.TraceInfo(rtTypeCollection.Count().ToString());

            foreach (IEdmEntityType metadataExposedEntityType in actualItems.SchemaElements.OfType<IEdmEntityType>())
            {
                ResourceType resourceContainerEntityType = serviceContainer.AllTypes.Where(b => b.GetType().Name.Equals("ResourceType") & (b.FullName.Equals(metadataExposedEntityType.FullName()))).FirstOrDefault() as ResourceType;
                if (resourceContainerEntityType == null)
                    continue;

                AstoriaTestLog.IsNotNull(resourceContainerEntityType, "Did not find entity type in resource container");
                AstoriaTestLog.AreEqual(resourceContainerEntityType.FullName, metadataExposedEntityType.FullName(), "Entity Type name mismatch");

                //
                //  Verify the name, type, and nullable attribute values
                //
                ResourceContainer rc = serviceContainer.ResourceContainers.Where(a => a.BaseType.Name.Equals(metadataExposedEntityType.Name)).FirstOrDefault();
                if (rc == null)
                    continue;
                int navCount = rc.BaseType.Properties.Cast<NodeProperty>().Cast<ResourceProperty>().Where(a => a.IsNavigation).Count();

                AstoriaTestLog.TraceInfo(rc.Name);
                AstoriaTestLog.TraceInfo(navCount.ToString());
                AstoriaTestLog.AreEqual(resourceContainerEntityType.Properties.Count - navCount, metadataExposedEntityType.Properties().Count(), "Edm Property count mismatch");
                foreach (IEdmProperty metadataExposedProperty in metadataExposedEntityType.Properties())
                {
                    string errorStringEntityTypeProperty = metadataExposedEntityType.FullName() + " : " + metadataExposedProperty.Name;

                    NodeProperty resourceContainerProperty = resourceContainerEntityType.Properties.Where(a => a.Name.Equals(metadataExposedProperty.Name)).FirstOrDefault();
                    AstoriaTestLog.IsNotNull(resourceContainerProperty, "Did not find property -->" + errorStringEntityTypeProperty + "<-- in resource container");
                    if (resourceContainerProperty == null)
                        continue;

                    if (metadataExposedProperty.Type.IsBinary())
                    {
                        if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.LinqToSql))
                            AstoriaTestLog.AreEqual("System.Data.Linq.Binary", resourceContainerProperty.Type.ClrType.FullName, errorStringEntityTypeProperty + " type is incorrect");
                        else
                            AstoriaTestLog.AreEqual("System.Byte[]", resourceContainerProperty.Type.ClrType.FullName, errorStringEntityTypeProperty + " type is incorrect");
                    }
                    else
                    {
                        AstoriaTestLog.AreEqual(metadataExposedProperty.Type.FullName(), resourceContainerProperty.Type.FullName, errorStringEntityTypeProperty + " type is incorrect");
                    }
                }
            }
        }


        public static void CompareMetadata(IEdmModel actualItems, bool isReflectionBasedProvider, string defaultEntityContainerName, ServiceContainer serviceContainer)
        {
            CompareMetadata(null, actualItems, isReflectionBasedProvider, defaultEntityContainerName, serviceContainer);
        }

        private static bool TypeIsOrphaned(IEdmType edmType, IEdmModel items)
        {
            Debug.Assert(edmType != null, "edmType != null");
            Debug.Assert(items != null, "items != null");

            if (edmType.TypeKind == EdmTypeKind.Entity)
            {
                // For entity types, if there is no set in the whole schema to which the type can belong to,
                // then the entity type is orphaned.
                IEdmEntityContainer container = items.EntityContainer;
                if (container != null)
                {
                    foreach (IEdmEntitySet set in container.EntitySets())
                    {
                        if (IsAssignableFrom(set.EntityType(), edmType))
                            return false;
                    }
                }
            }

            // If the type is association type and there are no navigation properties referring this association type,
            // then its okay to drop it too
            return true;
        }

        private static bool IsAssignableFrom(IEdmType candidateSupertype, IEdmType candidateSubtype)
        {
            return candidateSubtype.IsOrInheritsFrom(candidateSupertype);
        }

        private static XPathExpression FindCsdlSchemaRecursiveExpression;

        /// <summary>
        /// Checks whether the specified stream contains valid metadata and 
        /// returns a loaded <see cref="IEdmModel"/>.
        /// </summary>
        /// <param name="resultStream">Stream to read from.</param>
        /// <param name="resultLocation">Optional file name to save file to.</param>
        /// <returns>The loaded <see cref="IEdmModel"/>.</returns>
        public static IEdmModel IsValidMetadata(Stream resultStream, string resultLocation)
        {
            Debug.Assert(resultStream != null, "resultStream != null");

            // If we have to save, let's avoid reformatting and instead
            // load from disk.
            XPathDocument document;
            if (String.IsNullOrEmpty(resultLocation))
            {
                document = new XPathDocument(resultStream);
            }
            else
            {
                using (FileStream output = new FileStream(resultLocation, FileMode.Create, FileAccess.Write))
                {
                    IOUtil.CopyStream(resultStream, output);
                }

                document = new XPathDocument(resultLocation);
            }

            return IsValidMetadata(document);
        }

        /// <summary>
        /// Checks whether the specified document contains valid metadata and 
        /// returns a loaded <see cref="IEdmModel"/>.
        /// </summary>
        /// <param name="document">Document to validate.</param>
        /// <returns>The loaded <see cref="IEdmModel"/>.</returns>
        public static IEdmModel IsValidMetadata(XPathDocument document)
        {
            var reader = document.CreateNavigator().ReadSubtree();
            var model = CsdlReader.Parse(reader);
            IEnumerable<EdmError> errors;
            if (!model.Validate(out errors))
            {
                throw new TestFailedException(string.Join(Environment.NewLine, errors.Select(e => e.ToString())));
            }

            return model;
        }

        public static Stream WriteStringToStream(string input)
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static void CompareEntityContainer(IEdmEntityContainer edmEntityContainer, IEdmEntityContainer dataWebEntityContainer)
        {
            AstoriaTestLog.AreEqual(edmEntityContainer.FullName(), dataWebEntityContainer.FullName());

            int numberOfEntitySets = 0;
            foreach (IEdmEntitySet entitySetBase in edmEntityContainer.EntitySets())
            {
                // compare the entity set base
                IEdmEntitySet dataWebEntitySetBase = dataWebEntityContainer.FindEntitySet(entitySetBase.Name);

                AstoriaTestLog.IsNotNull(dataWebEntitySetBase);
                AstoriaTestLog.AreEqual(entitySetBase.Name, dataWebEntitySetBase.Name);
                AstoriaTestLog.AreEqual(entitySetBase.EntityType().FullName(), dataWebEntitySetBase.EntityType().FullName());

                ++numberOfEntitySets;
            }

            AstoriaTestLog.IsTrue(numberOfEntitySets == dataWebEntityContainer.EntitySets().Count(), "Number of baseEntitySets must match");
        }

        private static void CompareEdmType(IEdmType edmEdmType, IEdmType dataWebEdmType, bool isReflectionBasedProvider, out int numberOfNavProps)
        {
            if (edmEdmType == null)
            {
                throw new ArgumentNullException("edmEdmType");
            }
            if (dataWebEdmType == null)
            {
                throw new ArgumentNullException("dataWebEdmType");
            }

            AstoriaTestLog.IsTrue(edmEdmType.TypeKind == EdmTypeKind.Entity ||
                           edmEdmType.TypeKind == EdmTypeKind.Complex,
                            "Only structural type expected - " + edmEdmType.TypeKind.ToString());

            AstoriaTestLog.IsTrue(edmEdmType.TypeKind == dataWebEdmType.TypeKind,
                "BuiltInTypeKind must be the name");

            if (edmEdmType.TypeKind == EdmTypeKind.Entity)
            {
                List<IEdmStructuralProperty> edmKeyMembers = ((IEdmEntityType)edmEdmType).Key().ToList();
                List<IEdmStructuralProperty> dataWebKeyMembers = ((IEdmEntityType)dataWebEdmType).Key().ToList();

                AstoriaTestLog.IsTrue(edmKeyMembers.Count == dataWebKeyMembers.Count, "Key members count must match");
                foreach (IEdmStructuralProperty member1 in edmKeyMembers)
                {
                    // make sure the member with the same name in the key members list
                    // Other comparisions will be done in the member comparison
                    AstoriaTestLog.IsTrue(dataWebKeyMembers.Any(p => p.Name == member1.Name));
                }
            }

            CompareMembers((IEdmStructuredType)edmEdmType, (IEdmStructuredType)dataWebEdmType, isReflectionBasedProvider, out numberOfNavProps);
        }

        private static void CompareMembers(IEdmStructuredType edmEdmType, IEdmStructuredType dataWebEdmType, bool isReflectionBasedProvider, out int totalNavProps)
        {
            // find the number of nav properties on this type (not the base type). This will help us to determine how many associations 
            // be generated
            totalNavProps = edmEdmType.DeclaredProperties.OfType<IEdmNavigationProperty>().Count();
            AstoriaTestLog.IsTrue(edmEdmType.DeclaredProperties.Count() == dataWebEdmType.DeclaredProperties.Count(), "Number of members must match");

            foreach (IEdmProperty member1 in edmEdmType.DeclaredProperties)
            {
                IEdmProperty member2 = dataWebEdmType.FindProperty(member1.Name);
                AstoriaTestLog.IsNotNull(member2);

                // WCF DS server will always write DateTimeOffset as the data type, if the backend model has DateTime as the data type
                string edmMemberTypeName = member1.Type.FullName();
                if (edmMemberTypeName == "Edm.DateTime")
                {
                    AstoriaTestLog.IsTrue(member2.Type.FullName() == "Edm.DateTimeOffset", "For DateTime properties in the model, we should generate");
                }
                else
                {
                    AstoriaTestLog.IsTrue(edmMemberTypeName == member2.Type.FullName(), "Type must match");
                }

                AstoriaTestLog.IsTrue(member1.Type.IsNullable == member2.Type.IsNullable, "nullability must match");

                // TODO: if its a navigation, compare referential constraint!
            }
        }
    }

    public static class TestXmlConstants
    {
        /// <summary> Schema Namespace For Edm.</summary>
        public const string EdmV1Namespace = "http://schemas.microsoft.com/ado/2006/04/edm";

        /// <summary> Schema namespace for Oasis </summary>
        public const string EdmOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";
    }
}

