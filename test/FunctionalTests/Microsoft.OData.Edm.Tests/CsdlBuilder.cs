//---------------------------------------------------------------------
// <copyright file="CsdlBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Tests
{
    public class CsdlBuilder
    {
        internal static CsdlEntityContainer EntityContainer(
            string name,
            string extends = null,
            CsdlEntitySet[] entitySets = default(CsdlEntitySet[]),
            CsdlSingleton[] singletons = default(CsdlSingleton[]),
            CsdlOperationImport[] operationImports = default(CsdlOperationImport[]),
            CsdlDocumentation documentation = null,
            CsdlLocation location = null)
        {
            if (entitySets == null)
            {
                entitySets = new CsdlEntitySet[] { };
            }

            if (operationImports == null)
            {
                operationImports = new CsdlOperationImport[] { };
            }

            if (singletons == null)
            {
                singletons = new CsdlSingleton[] { };
            }

            return new CsdlEntityContainer(
                name,
                extends,
                entitySets,
                singletons,
                operationImports,
                documentation,
                location);
        }

        internal static CsdlEntityType EntityType(
            string name, 
            string baseName = null, 
            bool isAbstract = false,
            bool isOpen = false,
            bool hasStream = false,
            CsdlKey csdlKey = null,
            CsdlProperty[] properties = default(CsdlProperty[]),
            CsdlNavigationProperty[] navProps = default(CsdlNavigationProperty[]),
            CsdlDocumentation documentation = null,
            CsdlLocation location = null)
        {
            if (properties == null)
            {
                properties = new CsdlProperty[] { };
            }

            if (navProps == null)
            {
                navProps = new CsdlNavigationProperty[] { };
            }

            return new CsdlEntityType(
                name,
                baseName, 
                isAbstract,
                isOpen,
                hasStream,
                csdlKey, 
                properties, 
                navProps, 
                documentation, 
                location);
        }

        internal static CsdlOperationParameter Parameter(
            string name,
            string fullTypeReferenceName,
            CsdlDocumentation documentation = null,
            CsdlLocation location = null)
        {
            return new CsdlOperationParameter(name, new CsdlNamedTypeReference(fullTypeReferenceName, true, null), documentation, location);
        }

        internal static CsdlFunction Function(
            string name, 
            CsdlOperationParameter[] parameters = default(CsdlOperationParameter[]), 
            CsdlTypeReference typeReference = null, 
            bool isBound = false, 
            string entitySetPath = null, 
            bool isComposable = false,
            CsdlDocumentation documentation = null, 
            CsdlLocation location = null)
        {
            if (parameters == null)
            {
                parameters = new CsdlOperationParameter[] { };
            }

            return new CsdlFunction(
                name,
                parameters,
                typeReference,
                isBound,
                entitySetPath,
                isComposable,
                documentation,
                location);
        }

        internal static CsdlAction Action(
            string name,
            CsdlOperationParameter[] parameters = default(CsdlOperationParameter[]),
            CsdlTypeReference typeReference = null,
            bool isBound = false,
            string entitySetPath = null,
            CsdlDocumentation documentation = null,
            CsdlLocation location = null)

        {
            if (parameters == null)
            {
                parameters = new CsdlOperationParameter[] { };
            }

            return new CsdlAction(
                name,
                parameters,
                typeReference,
                isBound,
                entitySetPath,
                documentation,
                location);
        }

        internal static CsdlSchema Schema(
            string namespaceName,
            string alias = null,
            Version version = null,
            CsdlStructuredType[] csdlStructuredTypes = default(CsdlStructuredType[]),
            CsdlEnumType[] csdlEnumTypes = default(CsdlEnumType[]),
            CsdlOperation[] csdlOperations = default(CsdlOperation[]),
            CsdlTerm[] csdlTerms = default(CsdlTerm[]),
            CsdlEntityContainer[] csdlEntityContainers = default(CsdlEntityContainer[]),
            CsdlAnnotations[] csdlAnnotations = default(CsdlAnnotations[]),
            CsdlTypeDefinition[] csdlTypeDefinitions = default(CsdlTypeDefinition[]),
            CsdlDocumentation documentation = null,
            CsdlLocation location = null)
        {
            if (csdlStructuredTypes == null)
            {
                csdlStructuredTypes = new CsdlStructuredType[] { };
            }

            if (csdlEnumTypes == null)
            {
                csdlEnumTypes = new CsdlEnumType[] { };
            }

            if (csdlOperations == null)
            {
                csdlOperations = new CsdlOperation[] { };
            }

            if (csdlTerms == null)
            {
                csdlTerms = new CsdlTerm[] { };
            }

            if (csdlEntityContainers == null)
            {
                csdlEntityContainers = new CsdlEntityContainer[] { };
            }

            if (csdlAnnotations == null)
            {
                csdlAnnotations = new CsdlAnnotations[] { };
            }

            if (csdlTypeDefinitions == null)
            {
                csdlTypeDefinitions = new CsdlTypeDefinition[] { };
            }

            var csdlSchema = new CsdlSchema(
                namespaceName,
                alias,
                version,
                csdlStructuredTypes,
                csdlEnumTypes,
                csdlOperations,
                csdlTerms,
                csdlEntityContainers,
                csdlAnnotations,
                csdlTypeDefinitions,
                documentation /*documentation*/,
                location /*location*/);

            return csdlSchema;
        }
    }
}
