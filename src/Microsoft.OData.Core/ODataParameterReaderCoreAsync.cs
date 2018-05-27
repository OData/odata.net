//---------------------------------------------------------------------
// <copyright file="ODataParameterReaderCoreAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
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

#if PORTABLELIB
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
        /// Cretes an <see cref="ODataReader"/> to read the resource set value of type <paramref name="expectedResourceType"/>.
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
        protected override Task<bool> ReadAsynchronously()
        {
            switch (this.State)
            {
                case ODataParameterReaderState.Start:
#if DEBUG
                    return this.ReadAtStartImplementationAsync()
                        .FollowOnSuccessWith(t =>
                            {
                                Debug.Assert(
                                    this.State == ODataParameterReaderState.Value ||
                                    this.State == ODataParameterReaderState.Resource ||
                                    this.State == ODataParameterReaderState.ResourceSet ||
                                    this.State == ODataParameterReaderState.Collection ||
                                    this.State == ODataParameterReaderState.Completed,
                                    "ReadAtStartImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Resource, ODataParameterReaderState.ResourceSet, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);
                                return t.Result;
                            });
#else
                    return this.ReadAtStartImplementationAsync();
#endif

                case ODataParameterReaderState.Value:   // fall through
                case ODataParameterReaderState.Resource:
                case ODataParameterReaderState.ResourceSet:
                case ODataParameterReaderState.Collection:
                    this.OnParameterCompleted();
#if DEBUG
                    return this.ReadNextParameterImplementationAsync()
                        .FollowOnSuccessWith(t =>
                            {
                                Debug.Assert(
                                    this.State == ODataParameterReaderState.Value ||
                                    this.State == ODataParameterReaderState.Resource ||
                                    this.State == ODataParameterReaderState.ResourceSet ||
                                    this.State == ODataParameterReaderState.Collection ||
                                    this.State == ODataParameterReaderState.Completed,
                                    "ReadNextParameterImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Resource, ODataParameterReaderState.ResourceSet, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);
                                return t.Result;
                            });
#else
                    return this.ReadNextParameterImplementationAsync();
#endif

                case ODataParameterReaderState.Exception:    // fall through
                case ODataParameterReaderState.Completed:
                    Debug.Assert(false, "This case should have been caught earlier.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));

                default:
                    Debug.Assert(false, "Unsupported parameter reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));
            }
        }
#endif
    }
}
