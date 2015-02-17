//---------------------------------------------------------------------
// <copyright file="PocoClientCodeLayerGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Client code generator that uses simple POCO types
    /// </summary>
    [ImplementationName(typeof(IClientCodeLayerGenerator), "POCO", HelpText = "Uses locally generated poco classes")]
    public class PocoClientCodeLayerGenerator : StronglyTypedObjectLayerCodeGenerator, IClientCodeLayerGenerator
    {
        /// <summary>
        /// Initializes a new instance of the PocoClientCodeLayerGenerator class and uses the StronglyTypedClrTypeReferenceResolver to resolve any CLR types
        /// </summary>
        public PocoClientCodeLayerGenerator()
            : base(new StronglyTypedClrTypeReferenceResolver())
        {
        }

        /// <summary>
        /// Generates the client-side proxy classes then calls the given callback
        /// </summary>
        /// <param name="continuation">The async continuation to report completion on</param>
        /// <param name="serviceRoot">The root uri of the service</param>
        /// <param name="model">The model for the service</param>
        /// <param name="language">The language to generate code in</param>
        /// <param name="onCompletion">The action to invoke with the generated code</param>
        public void GenerateClientCode(IAsyncContinuation continuation, Uri serviceRoot, EntityModelSchema model, IProgrammingLanguageStrategy language, Action<string> onCompletion)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(serviceRoot, "serviceRoot");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(language, "language");
            ExceptionUtilities.CheckArgumentNotNull(onCompletion, "onCompletion");

            var compileUnit = new CodeCompileUnit();
            this.GenerateObjectLayer(compileUnit, model);
            this.GenerateContextType(compileUnit, model);

            string clientCode = language.CreateCodeGenerator().GenerateCodeFromCompileUnit(compileUnit);

            onCompletion(clientCode);
            continuation.Continue();
        }

        /// <summary>
        /// Adds a named stream property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="namedStreamProperty">The named stream property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected override void DeclareNamedStreamProperty(MemberProperty namedStreamProperty, CodeTypeDeclaration parentClass)
        {
            parentClass.AddAutoImplementedProperty(Code.TypeRef("Microsoft.OData.Client.DataServiceStreamLink"), namedStreamProperty.Name);
        }

        /// <summary>
        /// Generates and adds stream related attributes and elements to the entity type
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The entity types declaration</param>
        protected override void GenerateHasStreamEntityTypeCodeElements(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            ExceptionUtilities.Assert(entityType.HasStream(), "This method should not be called for entity types without stream.");
            
            ClientMediaEntryAnnotation clientMediaEntryAnnotation = entityType.Annotations.OfType<ClientMediaEntryAnnotation>().FirstOrDefault();
            if (clientMediaEntryAnnotation != null)
            {
                // generate MediaEntry and MimeTypeProperty properties and attributes for V1 style stream support
                var attributeArguments1 = new CodeAttributeArgument[]
                {
                    new CodeAttributeArgument(Code.Primitive(clientMediaEntryAnnotation.MediaEntryName)),
                };
                var attributeArguments2 = new CodeAttributeArgument[]
                {
                    new CodeAttributeArgument(Code.Primitive(clientMediaEntryAnnotation.MediaEntryName)),
                    new CodeAttributeArgument(Code.Primitive(clientMediaEntryAnnotation.MimeTypePropertyName))
                };

                entityTypeClass.AddCustomAttribute(Code.TypeRef("MediaEntry"), attributeArguments1);
                entityTypeClass.AddCustomAttribute(Code.TypeRef("MimeTypeProperty"), attributeArguments2);
                entityTypeClass.AddAutoImplementedProperty(Code.TypeRef<byte[]>(), clientMediaEntryAnnotation.MediaEntryName);
                entityTypeClass.AddAutoImplementedProperty(Code.TypeRef<string>(), clientMediaEntryAnnotation.MimeTypePropertyName);
            }
            else
            {
                // No ClientMediaEntryAnnotation is found, generate HasStream atttribute for V2 and up stream support
                entityTypeClass.AddCustomAttribute(Code.TypeRef("HasStream"));
            }
        }

        /// <summary>
        /// Gets the list of namespaces to import
        /// </summary>
        /// <returns>The namespaces needed for the objects</returns>
        protected override IList<string> GetNamespaceImports()
        {
            var list = base.GetNamespaceImports();
            list.Add("Microsoft.OData.Client");
            return list;
        }

        /// <summary>
        /// Generates the DataServiceContext used by the tests to query a Data Service
        /// </summary>
        /// <param name="compileUnit">Code compile unit which contains the client object layer</param>
        /// <param name="model">The schema for which DataServiceContext type should be generated</param>
        protected void GenerateContextType(CodeCompileUnit compileUnit, EntityModelSchema model)
        {
            // public class GeneratedContext : DataServiceContext
            // {
            var contextNamespace = compileUnit.Namespaces[0];
            var contextType = contextNamespace.DeclareType("GeneratedContext");
            contextType.BaseTypes.Add(Code.TypeRef("DataServiceContext"));

            // public GeneratedContext(Uri serviceRoot)
            //   : base(serviceRoot)
            // { }
            var constructor = contextType.AddConstructor().WithArgument(Code.TypeRef<Uri>(), "serviceRoot");
            constructor.BaseConstructorArgs.Add(Code.Variable("serviceRoot"));

            // public GeneratedContext(Uri serviceRoot, DataServiceProtocolVersion maxProtocolVersion)
            //   : base(serviceRoot, maxProtocolVersion)
            // { }
            var constructor1 = contextType.AddConstructor().WithArgument(Code.TypeRef<Uri>(), "serviceRoot").WithArgument(Code.TypeRef("Microsoft.OData.Client.ODataProtocolVersion"), "maxProtocolVersion");
            constructor1.BaseConstructorArgs.AddRange(new CodeExpression[] { Code.Variable("serviceRoot"), Code.Variable("maxProtocolVersion") });

            foreach (var entitySet in model.EntityContainers.SelectMany(c => c.EntitySets))
            {
                // public DataServiceQuery<entityType> <EntitySetName>
                // {
                //   get
                //   {
                //     return base.CreateQuery<entityType>("<entitySetName>");
                //   }
                // }
                var entityType = Code.TypeRef(entitySet.EntityType.FullName);
                var property = contextType.AddProperty(Code.GenericType("DataServiceQuery", entityType), entitySet.Name);
                property.GetStatements.Add(Code.Return(new CodeBaseReferenceExpression().Call("CreateQuery", new CodeTypeReference[] { entityType }, Code.Primitive(entitySet.Name))));
            }
        }

        /// <summary>
        /// Generates the [DataServiceKey] attribute on a given client type
        /// </summary>
        /// <param name="entityType">The complex type's metadata</param>
        /// <param name="entityTypeClass">The complex types declaration</param>
        protected override void GenerateKeyAttribute(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            if (entityType.Annotations.OfType<ClientTypeHasNoKeysAnnotation>().Any())
            {
                this.GenerateDataServiceEntityAttribute(entityTypeClass);
            }
            else
            {
                base.GenerateKeyAttribute(entityType, entityTypeClass);
            }

            DataServiceEntitySetAnnotation entitySetAnnotation = entityType.Annotations.OfType<DataServiceEntitySetAnnotation>().SingleOrDefault();
            if (entitySetAnnotation != null)
            {
                this.GenerateEntitySetAttribute(entityTypeClass, entitySetAnnotation);
            }
        }

        /// <summary>
        /// Adds functions into the Poco code generator
        /// </summary>
        /// <param name="namespaceName">Namespace of functions to add</param>
        /// <param name="codeNamespace">Namespace to put static Extension type container</param>
        /// <param name="functions">functions from the model</param>
        protected override void AddFunctionsInNamespaceIfRequired(string namespaceName, CodeNamespace codeNamespace, IEnumerable<Function> functions)
        {
            // Only get functions that are bound to an entityType or collection other wise not interesting to add an extension method
            IEnumerable<Function> namespaceFunctions = functions.Where(func => func.NamespaceName == namespaceName && func.Annotations.OfType<ServiceOperationAnnotation>().Any(s => s.BindingKind.IsBound()));

            if (functions != null)
            {
                CodeTypeDeclaration functionExtensionClass = null;

                foreach (Function function in namespaceFunctions)
                {
                    // add the extension method container class
                    if (functionExtensionClass == null)
                    {
                        functionExtensionClass = codeNamespace.DeclareExtensionMethodContainerType(namespaceName + "ExtensionMethod");
                    }

                    this.DeclareExtensionMethod(functionExtensionClass, function);
                }
            }
        }

        /// <summary>
        /// Declare the extension method given the function container class.
        /// </summary>
        /// <param name="functionExtensionClass">Function container class</param>
        /// <param name="func">Function to add</param>
        protected virtual void DeclareExtensionMethod(CodeTypeDeclaration functionExtensionClass, Function func)
        {
            // retrieve parameters and add the extension method
            ServiceOperationAnnotation actionAnnotation = func.Annotations.OfType<ServiceOperationAnnotation>().Single();
            ExceptionUtilities.Assert(actionAnnotation.BindingKind.IsBound(), "Action function cannot generate extension method with out function having a binding");
            FunctionParameter bindingTypeParameter = func.Parameters[0];

            CodeMemberMethod method = functionExtensionClass.AddExtensionMethod(
                func.Name,
                new CodeParameterDeclarationExpression(
                    this.GetParameterTypeOrFunctionReturnTypeReference(bindingTypeParameter.Annotations, bindingTypeParameter.DataType),
                    bindingTypeParameter.Name));

            // add non-binding type parameters
            foreach (var parameter in func.Parameters.Where(p => p.Name != bindingTypeParameter.Name))
            {
                method.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        this.GetParameterTypeOrFunctionReturnTypeReference(parameter.Annotations, parameter.DataType),
                        parameter.Name));
            }

            // throw exception if the client code method is called
            method.Statements.Add(
                new CodeThrowExceptionStatement(
                    new CodeObjectCreateExpression(
                        new CodeTypeReference(typeof(InvalidOperationException)),
                        new CodeExpression[] { })));

            // add method return type
            if (func.ReturnType != null)
            {
                method.ReturnType = this.GetParameterTypeOrFunctionReturnTypeReference(func.Annotations, func.ReturnType);
            }
        }

        /// <summary>
        /// Helper to get CodeTypeReference for given data type.
        /// </summary>
        /// <param name="annotations">list of annotations to find Collection Type from</param>
        /// <param name="type">DataType to get CodeTypeReference</param>
        /// <returns>CodeTypeReference for the DataType</returns>
        protected CodeTypeReference GetParameterTypeOrFunctionReturnTypeReference(IEnumerable<Annotation> annotations, DataType type)
        {
            ExceptionUtilities.CheckObjectNotNull(type, "DataType is null.");

            var collectionDataType = type as CollectionDataType;
            if (collectionDataType != null)
            {
                return this.GetCollectionType(CodeGenerationTypeUsage.Declaration, annotations, collectionDataType.ElementDataType);
            }

            ReferenceDataType referenceDataType = type as ReferenceDataType;
            if (referenceDataType != null)
            {
                ExceptionUtilities.Assert(referenceDataType.EntityType != null, "Does not handle non-EntityType referenceDataType");
                return this.BackingTypeResolver.ResolveClrTypeReference(referenceDataType.EntityType);
            }
            else
            {
                return this.BackingTypeResolver.ResolveClrTypeReference(type);
            }
        }

        /// <summary>
        /// Adds the [DataServiceEntity] attribute on the client type
        /// </summary>
        /// <param name="entityTypeClass">The class declaration for the client type</param>
        private void GenerateDataServiceEntityAttribute(CodeTypeDeclaration entityTypeClass)
        {
            entityTypeClass.AddCustomAttribute(Code.TypeRef("DataServiceEntity"));
        }

        /// <summary>
        /// Adds the [EntitySet('setName')] attribute on the client type
        /// </summary>
        /// <param name="entityTypeClass">The class declaration for the client type</param>
        /// <param name="entitySetAnnotation">The DataServiceEntitySetAnnotation which contains the name of the set</param>
        private void GenerateEntitySetAttribute(CodeTypeDeclaration entityTypeClass, DataServiceEntitySetAnnotation entitySetAnnotation)
        {
            var entitySetAttribute = entityTypeClass.AddCustomAttribute(Code.TypeRef("EntitySet"));
            entitySetAttribute.Arguments.Add(new CodeAttributeArgument(Code.Primitive(entitySetAnnotation.EntitySetName)));
        }
    }
}