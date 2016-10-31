//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Responsible for resolving the element type of an entity set with reader (i.e., looser) semantics.
    /// </summary>
    internal sealed class EdmTypeReaderResolver : EdmTypeResolver
    {
        /// <summary>The model to use or null if no model is available.</summary>
        private readonly IEdmModel model;

        /// <summary>Reader behavior if the caller is a reader, null if no reader behavior is available.</summary>
        private readonly ODataReaderBehavior readerBehavior;

        /// <summary>The version of the payload being read.</summary>
        private readonly ODataVersion version;

        /// <summary>Creates a new entity set element type resolver with all the information needed when resolving for reading scenarios.</summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        public EdmTypeReaderResolver(IEdmModel model, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            this.model = model;
            this.readerBehavior = readerBehavior;
            this.version = version;
        }

        /// <summary>Returns the element type of the given entity set.</summary>
        /// <param name="entitySet">The entity set to get the element type of.</param>
        /// <returns>The <see cref="IEdmEntityType"/> representing the element type of the <paramref name="entitySet" />.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "entitySet.ElementType is allowed here and the reader code paths should call this method to get to the ElementType of a set.")]
        internal override IEdmEntityType GetElementType(IEdmEntitySet entitySet)
        {
            DebugUtils.CheckNoExternalCallers();

            return entitySet == null ? null : (IEdmEntityType)this.ResolveType(entitySet.ElementType);
        }

        /// <summary>
        /// Returns the return type of the given function import.
        /// </summary>
        /// <param name="functionImport">The function import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImport"/>.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "functionImport.ReturnType is allowed here and the reader code paths should call this method to get to the ReturnType of a function import.")]
        internal override IEdmTypeReference GetReturnType(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();

            if (functionImport != null && functionImport.ReturnType != null)
            {
                return this.ResolveTypeReference(functionImport.ReturnType);
            }

            return null;
        }

        /// <summary>
        /// Returns the return type of the given function import group.
        /// </summary>
        /// <param name="functionImportGroup">The function import group to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImportGroup"/>.</returns>
        internal override IEdmTypeReference GetReturnType(IEnumerable<IEdmFunctionImport> functionImportGroup)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImportGroup != null, "functionImportGroup != null");

            IEdmFunctionImport firstFunctionImport = functionImportGroup.FirstOrDefault();
            Debug.Assert(firstFunctionImport != null, "firstFunctionImport != null");
            Debug.Assert(
                functionImportGroup.All(f =>
                {
                    IEdmTypeReference returnType = this.GetReturnType(f);
                    IEdmTypeReference actual = this.GetReturnType(firstFunctionImport);
                    return returnType == null && actual == null || returnType.IsEquivalentTo(actual);
                }),
                "In a valid model, the return type of function imports from the same function import group should be the same.");
            return this.GetReturnType(firstFunctionImport);
        }

        /// <summary>
        /// Gets the function parameter type for read and calls the client type resolver to resolve type when it is specified.
        /// </summary>
        /// <param name="functionParameter">The function parameter to resolve the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the function parameter; or null if no such type could be found.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "functionParameter.Type is allowed here and the reader code paths should call this method to get to the Type of a function parameter.")]
        internal override IEdmTypeReference GetParameterType(IEdmFunctionParameter functionParameter)
        {
            DebugUtils.CheckNoExternalCallers();

            return functionParameter == null ? null : this.ResolveTypeReference(functionParameter.Type);
        }

        /// <summary>
        /// Resolves the given type reference if a client type resolver is available.
        /// </summary>
        /// <param name="typeReferenceToResolve">Type reference to resolve.</param>
        /// <returns>The resolved type reference.</returns>
        private IEdmTypeReference ResolveTypeReference(IEdmTypeReference typeReferenceToResolve)
        {
            Debug.Assert(typeReferenceToResolve != null, "typeReferenceToResolve != null");
            Debug.Assert(this.readerBehavior != null, "readerBehavior != null");

            Func<IEdmType, string, IEdmType> customTypeResolver = this.readerBehavior.TypeResolver;
            if (customTypeResolver == null)
            {
                return typeReferenceToResolve;
            }

            return this.ResolveType(typeReferenceToResolve.Definition).ToTypeReference(typeReferenceToResolve.IsNullable);
        }

        /// <summary>
        /// Resolves the given type if a client type resolver is available.
        /// </summary>
        /// <param name="typeToResolve">Type to resolve.</param>
        /// <returns>The resolved type.</returns>
        private IEdmType ResolveType(IEdmType typeToResolve)
        {
            Debug.Assert(typeToResolve != null, "typeToResolve != null");
            Debug.Assert(this.model != null, "model != null");
            Debug.Assert(this.readerBehavior != null, "readerBehavior != null");

            Func<IEdmType, string, IEdmType> customTypeResolver = this.readerBehavior.TypeResolver;
            if (customTypeResolver == null)
            {
                return typeToResolve;
            }

            Debug.Assert(this.readerBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesClient, "Custom type resolver can only be specified in WCF DS Client behavior.");
            EdmTypeKind typeKind;

            // MetadataUtils.ResolveTypeName() does not allow entity collection types however both functionImport.ReturnType and functionParameter.Type can be of entity collection types.
            // We don't want to relax MetadataUtils.ResolveTypeName() since the rest of ODL only allows primitive and complex collection types and is currently relying on the method to
            // enforce this.  So if typeToResolve is an entity collection type, we will resolve the item type and reconstruct the collection type from the resolved item type.
            IEdmCollectionType collectionTypeToResolve = typeToResolve as IEdmCollectionType;
            if (collectionTypeToResolve != null && collectionTypeToResolve.ElementType.IsEntity())
            {
                IEdmTypeReference itemTypeReferenceToResolve = collectionTypeToResolve.ElementType;
                IEdmType resolvedItemType = MetadataUtils.ResolveTypeName(this.model, null /*expectedType*/, itemTypeReferenceToResolve.FullName(), customTypeResolver, this.version, out typeKind);
                return new EdmCollectionType(resolvedItemType.ToTypeReference(itemTypeReferenceToResolve.IsNullable));
            }

            return MetadataUtils.ResolveTypeName(this.model, null /*expectedType*/, typeToResolve.ODataFullName(), customTypeResolver, this.version, out typeKind);
        }
    }
}
