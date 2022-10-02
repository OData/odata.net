//---------------------------------------------------------------------
// <copyright file="ODataParameterReaderCoreAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData parameter readers that verifies a proper sequence of read calls on the reader with truly async operations.
    /// </summary>
    internal abstract class ODataParameterReaderCoreAsync : ODataParameterReaderCore
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read from.</param>
        /// <param name="operation">The operation whose parameters are being read.</param>
        protected ODataParameterReaderCoreAsync(
            ODataInputContext inputContext,
            IEdmOperation operation)
            : base(inputContext, operation)
        {
        }

        /// <summary>
        /// This method asynchronously creates an <see cref="ODataReader"/> to read the resource value when the state is ODataParameterReaderState.Resource.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an <see cref="ODataReader"/> to read the resource value when the state is ODataParameterReaderState.Resource.
        /// </returns>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Resource, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        public override Task<ODataReader> CreateResourceReaderAsync()
        {
            this.VerifyCanCreateSubReader(ODataParameterReaderState.Resource);
            this.subReaderState = SubReaderState.Active;
            Debug.Assert(this.Name != null, "this.Name != null");
            Debug.Assert(this.Value == null, "this.Value == null");
            IEdmStructuredType expectedResourceType = (IEdmStructuredType)this.GetParameterTypeReference(this.Name).Definition;

            return this.CreateResourceReaderAsync(expectedResourceType);
        }

        /// <summary>
        /// This method asynchronously creates an <see cref="ODataReader"/> to read the resource set value when the state is ODataParameterReaderState.ResourceSet.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an <see cref="ODataReader"/> to read the resource set value when the state is ODataParameterReaderState.ResourceSet.
        /// </returns>
        /// <remarks>
        /// When the state is ODataParameterReaderState.ResourceSet, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        public override Task<ODataReader> CreateResourceSetReaderAsync()
        {
            this.VerifyCanCreateSubReader(ODataParameterReaderState.ResourceSet);
            this.subReaderState = SubReaderState.Active;
            Debug.Assert(this.Name != null, "this.Name != null");
            Debug.Assert(this.Value == null, "this.Value == null");
            IEdmStructuredType expectedResourceType = (IEdmStructuredType)((IEdmCollectionType)this.GetParameterTypeReference(this.Name).Definition).ElementType.Definition;

            return this.CreateResourceSetReaderAsync(expectedResourceType);
        }

        /// <summary>
        /// This method asynchronously creates an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.
        /// </returns>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Collection, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        public override Task<ODataCollectionReader> CreateCollectionReaderAsync()
        {
            this.VerifyCanCreateSubReader(ODataParameterReaderState.Collection);
            this.subReaderState = SubReaderState.Active;
            Debug.Assert(this.Name != null, "this.Name != null");
            Debug.Assert(this.Value == null, "this.Value == null");
            IEdmTypeReference expectedItemTypeReference = ((IEdmCollectionType)this.GetParameterTypeReference(this.Name).Definition).ElementType;

            return this.CreateCollectionReaderAsync(expectedItemTypeReference);
        }

        /// <summary>
        /// Implementation of the parameter reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state Value, Resource, Resource Set or Collection state.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadNextParameterImplementationAsync();

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource value of type <paramref name="expectedResourceType"/>.</returns>
        protected abstract Task<ODataReader> CreateResourceReaderAsync(IEdmStructuredType expectedResourceType);

        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.
        /// </summary>
        /// <param name="expectedResourceType">Expected resource set element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.</returns>
        protected abstract Task<ODataReader> CreateResourceSetReaderAsync(IEdmStructuredType expectedResourceType);

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        protected abstract Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference);

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        protected override async Task<bool> ReadAsynchronously()
        {
            bool result;

            switch (this.State)
            {
                case ODataParameterReaderState.Start:
                    result = await this.ReadAtStartImplementationAsync()
                        .ConfigureAwait(false);

                    Debug.Assert(
                        this.State == ODataParameterReaderState.Value ||
                        this.State == ODataParameterReaderState.Resource ||
                        this.State == ODataParameterReaderState.ResourceSet ||
                        this.State == ODataParameterReaderState.Collection ||
                        this.State == ODataParameterReaderState.Completed,
                        "ReadAtStartImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Resource, ODataParameterReaderState.ResourceSet, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);

                    return result;
                case ODataParameterReaderState.Value:   // fall through
                case ODataParameterReaderState.Resource:
                case ODataParameterReaderState.ResourceSet:
                case ODataParameterReaderState.Collection:
                    this.OnParameterCompleted();
                    result = await this.ReadNextParameterImplementationAsync()
                        .ConfigureAwait(false);

                    Debug.Assert(
                        this.State == ODataParameterReaderState.Value ||
                        this.State == ODataParameterReaderState.Resource ||
                        this.State == ODataParameterReaderState.ResourceSet ||
                        this.State == ODataParameterReaderState.Collection ||
                        this.State == ODataParameterReaderState.Completed,
                        "ReadNextParameterImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Resource, ODataParameterReaderState.ResourceSet, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);

                    return result;
                case ODataParameterReaderState.Exception:    // fall through
                case ODataParameterReaderState.Completed:
                    Debug.Assert(false, "This case should have been caught earlier.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));

                default:
                    Debug.Assert(false, "Unsupported parameter reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));
            }
        }
    }
}
