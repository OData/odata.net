//---------------------------------------------------------------------
// <copyright file="OperationImportSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a call to an actionimport, or functionImport.
    /// </summary>
    //// TODO: update code that is duplicate between operation and operation import segment, likely could share a baseclass.
    public sealed class OperationImportSegment : ODataPathSegment
    {
        /// <summary>
        /// Sentinel type marking that we could not determine the return type for this segment.
        /// </summary>
        private static readonly IEdmType UnknownSentinel = new EdmEnumType("Sentinel", "UndeterminableTypeMarker");

        /// <summary>
        /// The list of possible FunctionImport overloads for this segment.
        /// </summary>
        private readonly ReadOnlyCollection<IEdmOperationImport> operationImports;

        /// <summary>
        /// the list of parameters to this operation.
        /// </summary>
        private readonly ReadOnlyCollection<OperationSegmentParameter> parameters;

        /// <summary>
        /// The <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.
        /// This will be null if entities are not returned by this operation, or if there is any ambiguity.
        /// </summary>
        private readonly IEdmEntitySetBase entitySet;

        /// <summary>
        /// The type of item returned by the operation(s), if known.
        /// </summary>
        private readonly IEdmType computedReturnEdmType;

        /// <summary>
        /// Build a segment representing a call to an operation - action, function, or service operation.
        /// </summary>
        /// <param name="operationImport">A single operation import that this segment will represent.</param>
        /// <param name="entitySet">The <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input operation is null.</exception>
        public OperationImportSegment(IEdmOperationImport operationImport, IEdmEntitySetBase entitySet)
            : this()
        {
            ExceptionUtils.CheckArgumentNotNull(operationImport, "operationImport");
            this.operationImports = new ReadOnlyCollection<IEdmOperationImport>(new[] { operationImport });
            this.Identifier = operationImport.Name;
            this.entitySet = entitySet;
            this.computedReturnEdmType = operationImport.Operation.ReturnType != null ? operationImport.Operation.ReturnType.Definition : null;
            this.EnsureTypeAndSetAreCompatable();

            if (this.computedReturnEdmType != null)
            {
                this.TargetEdmNavigationSource = entitySet;
                this.TargetEdmType = computedReturnEdmType;
                this.TargetKind = this.TargetEdmType.GetTargetKindFromType();
                this.SingleResult = computedReturnEdmType.TypeKind != EdmTypeKind.Collection;
            }
            else
            {
                this.TargetEdmNavigationSource = null;
                this.TargetEdmType = null;
                this.TargetKind = RequestTargetKind.VoidOperation;
            }
        }

        /// <summary>
        /// Build a segment representing a call to an operation - action, function, or service operation.
        /// </summary>
        /// <param name="operationImport">A single operation import that this segment will represent.</param>
        /// <param name="entitySet">The <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.</param>
        /// <param name="parameters">The list of parameters supplied to this segment.</param>
        public OperationImportSegment(IEdmOperationImport operationImport, IEdmEntitySetBase entitySet, IEnumerable<OperationSegmentParameter> parameters)
            : this(operationImport, entitySet)
        {
            this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? new List<OperationSegmentParameter>() : parameters.ToList());
        }

        /// <summary>
        /// Build a segment representing a call to an operation - action, function, or service operation.
        /// </summary>
        /// <param name="operationImports">The list of possible FunctionImport overloads for this segment.</param>
        /// <param name="entitySet">The <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input operations is null.</exception>
        public OperationImportSegment(IEnumerable<IEdmOperationImport> operationImports, IEdmEntitySetBase entitySet)
            : this()
        {
            // DEVNOTE: This ctor is only used in Select and Expand currently.
            ExceptionUtils.CheckArgumentNotNull(operationImports, "operations");
            this.operationImports = new ReadOnlyCollection<IEdmOperationImport>(operationImports.ToList());

            // check for empty after we copy locally, so that we don't do multiple enumeration of input
            ExceptionUtils.CheckArgumentCollectionNotNullOrEmpty(this.operationImports, "operations");

            this.Identifier = this.operationImports.First().Name;

            // Determine the return type of the operation. This is only possible if all the candidate operations agree on the return type.
            // TODO: Because we work on types and not type references, if there are nullability differences we'd ignore them...
            IEdmType typeSoFar = this.operationImports.First().Operation.ReturnType != null
                                     ? this.operationImports.First().Operation.ReturnType.Definition
                                     : null;
            if (typeSoFar == null)
            {
                // This is for void operations
                if (this.operationImports.Any(operation => operation.Operation.ReturnType != null))
                {
                    typeSoFar = UnknownSentinel;
                }
            }
            else if (this.operationImports.Any(operationImport => !typeSoFar.IsEquivalentTo(operationImport.Operation.ReturnType.Definition)))
            {
                typeSoFar = UnknownSentinel;
            }

            this.computedReturnEdmType = typeSoFar;
            this.entitySet = entitySet;
            this.EnsureTypeAndSetAreCompatable();
        }

        /// <summary>
        /// Creates a segment representing a call to an operation - action, function or service operation.
        /// </summary>
        /// <param name="operationImports">The list of possible FunctionImport overloads for this segment.</param>
        /// <param name="entitySet">The <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.</param>
        /// <param name="parameters">The list of parameters supplied to this segment.</param>
        public OperationImportSegment(IEnumerable<IEdmOperationImport> operationImports, IEdmEntitySetBase entitySet, IEnumerable<OperationSegmentParameter> parameters)
            : this(operationImports, entitySet)
        {
            this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? new List<OperationSegmentParameter>() : parameters.ToList());
        }

        /// <summary>
        /// Creates a segment representing a call to an operation - action, function or service operation.
        /// </summary>
        private OperationImportSegment()
        {
            this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(new List<OperationSegmentParameter>());
        }

        /// <summary>
        /// Gets the list of possible operation import overloads for this segment.
        /// TODO: Change this to a property that returns just an operationImport.
        /// </summary>
        public IEnumerable<IEdmOperationImport> OperationImports
        {
            get { return this.operationImports.AsEnumerable(); }
        }

        /// <summary>
        /// Gets the list of parameters for this segment.
        /// </summary>
        public IEnumerable<OperationSegmentParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="OperationSegment"/>.
        /// </summary>
        /// <remarks>
        /// This value will be null for void service operations.
        /// If there are multiple candidate operations with varying return types, then this property will throw.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the type is unknown.</exception>
        public override IEdmType EdmType
        {
            get
            {
                if (ReferenceEquals(this.computedReturnEdmType, UnknownSentinel))
                {
                    throw new ODataException(ODataErrorStrings.OperationSegment_ReturnTypeForMultipleOverloads);
                }

                return this.computedReturnEdmType;
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmEntitySetBase"/> containing the entities that this function returns.
        /// This will be null if entities are not returned by this operation, or if there is any ambiguity.
        /// </summary>
        public IEdmEntitySetBase EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handle interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            OperationImportSegment otherOperation = other as OperationImportSegment;
            return otherOperation != null &&
                   otherOperation.OperationImports.SequenceEqual(this.OperationImports) &&
                   otherOperation.EntitySet == this.entitySet;
        }

        /// <summary>
        /// Ensures that the entity set and computed return type make sense.
        /// </summary>
        /// <exception cref="ODataException">Throws if the return type computed from the function call is null, or if the return type is not in the same hierarchy as the entity set provided.</exception>
        private void EnsureTypeAndSetAreCompatable()
        {
            // The return type should be in the type hierarchy of the set, We could be a little tighter but we don't want
            // to overdo it here.
            // If they didn't get us an entity set, or if we couldn't compute a single return type, then we don't need these checks
            if (this.entitySet == null || this.computedReturnEdmType == UnknownSentinel)
            {
                return;
            }

            // Void operations cannot specificy return entity set
            if (this.computedReturnEdmType == null)
            {
                throw new ODataException(ODataErrorStrings.OperationSegment_CannotReturnNull);
            }

            // Unwrap the return type if it's a collection
            var unwrappedCollectionType = this.computedReturnEdmType;
            var collectoinType = this.computedReturnEdmType as IEdmCollectionType;
            if (collectoinType != null)
            {
                unwrappedCollectionType = collectoinType.ElementType.Definition;
            }

            // Ensure that the return type is in the same type hierarhcy as the entity set provided
            if (!this.entitySet.EntityType().IsOrInheritsFrom(unwrappedCollectionType) && !unwrappedCollectionType.IsOrInheritsFrom(this.entitySet.EntityType()))
            {
                throw new ODataException(ODataErrorStrings.OperationSegment_CannotReturnNull);
            }
        }
    }
}
