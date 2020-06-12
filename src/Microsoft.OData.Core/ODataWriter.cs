//---------------------------------------------------------------------
// <copyright file="ODataWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System;
    using System.IO;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData writers.
    /// </summary>
    public abstract class ODataWriter
    {
        /// <summary>Starts the writing of a resource set.</summary>
        /// <param name="resourceSet">The resource set or collection to write.</param>
        public abstract void WriteStart(ODataResourceSet resourceSet);

        /// <summary>Writes a resource set.</summary>
        /// <param name="resourceSet">The resource set or collection to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataResourceSet resourceSet)
        {
            WriteStart(resourceSet);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a resource set and performs an action in-between.</summary>
        /// <param name="resourceSet">The resource set or collection to write.</param>
        /// <param name="nestedAction">The action to perform in-between writing the resource set.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataResourceSet resourceSet, Action nestedAction)
        {
            WriteStart(resourceSet);
            nestedAction();
            WriteEnd();
            return this;
        }


        /// <summary> Asynchronously start writing a resource set. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="resourceSet">The resource set or collection to write.</param>
        public virtual Task WriteStartAsync(ODataResourceSet resourceSet)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(resourceSet));
        }

        /// <summary>Starts the writing of a delta resource set.</summary>
        /// <param name="deltaResourceSet">The resource set or collection to write.</param>
        public virtual void WriteStart(ODataDeltaResourceSet deltaResourceSet)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes a delta resource set.</summary>
        /// <param name="deltaResourceSet">The delta resource set or collection to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeltaResourceSet deltaResourceSet)
        {
            WriteStart(deltaResourceSet);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a delta resource set and performs an action in-between.</summary>
        /// <param name="deltaResourceSet">The delta resource set or collection to write.</param>
        /// <param name="nestedAction">The action to perform in-between writing the resource set.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeltaResourceSet deltaResourceSet, Action nestedAction)
        {
            WriteStart(deltaResourceSet);
            nestedAction();
            WriteEnd();
            return this;
        }


        /// <summary> Asynchronously start writing a resource set. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="deltaResourceSet">The resource set or collection to write.</param>
        public virtual Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(deltaResourceSet));
        }

        /// <summary>Starts the writing of a resource.</summary>
        /// <param name="resource">The resource or item to write.</param>
        public abstract void WriteStart(ODataResource resource);

        /// <summary>Writes a resource.</summary>
        /// <param name="resource">The resource or item to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataResource resource)
        {
            WriteStart(resource);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a resource and performs an action in-between.</summary>
        /// <param name="resource">The resource or item to write.</param>
        /// <param name="nestedAction">The action to perform in-between the writing.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataResource resource, Action nestedAction)
        {
            WriteStart(resource);
            nestedAction();
            WriteEnd();
            return this;
        }

        /// <summary>Writes a deleted resource.</summary>
        /// <param name="deletedResource">The deleted resource to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeletedResource deletedResource)
        {
            WriteStart(deletedResource);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a deleted resource and performs an action in-between.</summary>
        /// <param name="deletedResource">The deleted resource to write.</param>
        /// <param name="nestedAction">The action to perform in-between the writing.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeletedResource deletedResource, Action nestedAction)
        {
            WriteStart(deletedResource);
            nestedAction();
            WriteEnd();
            return this;
        }

        /// <summary>Writes a delta link.</summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeltaLink deltaLink)
        {
            WriteDeltaLink(deltaLink);
            return this;
        }

        /// <summary>Writes a deleted link.</summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataDeltaDeletedLink deltaDeletedLink)
        {
            WriteDeltaDeletedLink(deltaDeletedLink);
            return this;
        }


        /// <summary> Asynchronously start writing a resource. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="resource">The resource or item to write.</param>
        public virtual Task WriteStartAsync(ODataResource resource)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(resource));
        }


        /// <summary> Starts writing a deleted resource.</summary>
        /// <param name="deletedResource">The deleted resource to write.</param>
        public virtual void WriteStart(ODataDeletedResource deletedResource)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Asynchronously writing a delta deleted resource.
        /// </summary>
        /// <param name="deletedResource">The deleted resource to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public virtual Task WriteStartAsync(ODataDeletedResource deletedResource)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(deletedResource));
        }


        /// <summary>
        /// Write a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        public virtual void WriteDeltaLink(ODataDeltaLink deltaLink)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Asynchronously writing a delta link.
        /// </summary>
        /// <param name="deltaLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public virtual Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteDeltaLink(deltaLink));
        }


        /// <summary>
        /// Write a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta deleted link to write.</param>
        public virtual void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Asynchronously write a delta deleted link.
        /// </summary>
        /// <param name="deltaDeletedLink">The delta link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public virtual Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteDeltaDeletedLink(deltaDeletedLink));
        }


        /// <summary>Starts the writing of a nested resource info.</summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        public abstract void WriteStart(ODataNestedResourceInfo nestedResourceInfo);

        /// <summary>Writes a nested resource info.</summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataNestedResourceInfo nestedResourceInfo)
        {
            WriteStart(nestedResourceInfo);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a nested resource info and performs an action in-between.</summary>
        /// <param name="nestedResourceInfo">The nested resource info to write.</param>
        /// <param name="nestedAction">The action to perform in-between the writing.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataNestedResourceInfo nestedResourceInfo, Action nestedAction)
        {
            WriteStart(nestedResourceInfo);
            nestedAction();
            WriteEnd();
            return this;
        }


        /// <summary> Asynchronously start writing a nested resource info. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="nestedResourceInfo">The nested resource info to writer.</param>
        public virtual Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(nestedResourceInfo));
        }

        /// <summary>Writes a primitive value within an untyped collection.</summary>
        /// <param name="primitiveValue">The primitive value to write.</param>
        public virtual void WritePrimitive(ODataPrimitiveValue primitiveValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes a primitive value within an untyped collection.</summary>
        /// <param name="primitiveValue">The primitive value to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter Write(ODataPrimitiveValue primitiveValue)
        {
            WritePrimitive(primitiveValue);
            return this;
        }


        /// <summary> Asynchronously write a primitive value within an untyped collection. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="primitiveValue">The primitive value to write.</param>
        public virtual Task WritePrimitiveAsync(ODataPrimitiveValue primitiveValue)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WritePrimitive(primitiveValue));
        }

        /// <summary>Writes a primitive property within a resource.</summary>
        /// <param name="primitiveProperty">The primitive property to write.</param>
        public virtual void WriteStart(ODataPropertyInfo primitiveProperty)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes a primitive property within a resource.</summary>
        /// <param name="primitiveProperty">The primitive property to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public ODataWriter Write(ODataProperty primitiveProperty)
        {
            WriteStart(primitiveProperty);
            WriteEnd();
            return this;
        }

        /// <summary>Writes a primitive property within a resource.</summary>
        /// <param name="primitiveProperty">The primitive property to write.</param>
        /// <param name="nestedAction">The action to perform in-between the writing.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public ODataWriter Write(ODataProperty primitiveProperty, Action nestedAction)
        {
            WriteStart(primitiveProperty);
            nestedAction();
            WriteEnd();
            return this;
        }


        /// <summary> Asynchronously write a primitive property within a resource. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="primitiveProperty">The primitive property to write.</param>
        public virtual Task WriteStartAsync(ODataPropertyInfo primitiveProperty)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStart(primitiveProperty));
        }

        /// <summary>Creates a stream for writing a binary value.</summary>
        /// <returns>A stream to write a binary value to.</returns>
        public virtual Stream CreateBinaryWriteStream()
        {
            throw new NotImplementedException();
        }

        /// <summary>Creates a stream for writing a binary value.</summary>
        /// <param name="stream">The stream to write.</param>
        /// <returns>This ODataWriter, allowing for chaining operations.</returns>
        public ODataWriter WriteStream(ODataBinaryStreamValue stream)
        {
            Stream writeStream = this.CreateBinaryWriteStream();
            stream.Stream.CopyTo(writeStream);
            writeStream.Flush();
            writeStream.Dispose();
            return this;
        }


        /// <summary>Asynchronously creates a stream for writing a binary value.</summary>
        /// <returns>A stream to write a binary value to.</returns>
        public virtual Task<Stream> CreateBinaryWriteStreamAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateBinaryWriteStream());
        }

        /// <summary>Creates a TextWriter for writing a string value.</summary>
        /// <returns>A TextWriter to write a string value.</returns>
        public virtual TextWriter CreateTextWriter()
        {
            throw new NotImplementedException();
        }


        /// <summary>Asynchronously creates a TextWriter for writing a string value.</summary>
        /// <returns>A TextWriter to write a string value.</returns>
        public virtual Task<TextWriter> CreateTextWriterAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateTextWriter());
        }

        /// <summary>Finishes the writing of a resource set, a resource, or a nested resource info.</summary>
        public abstract void WriteEnd();


        /// <summary> Asynchronously finish writing a resource set, resource, or nested resource info. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public virtual Task WriteEndAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteEnd());
        }

        /// <summary> Writes an entity reference link, which is used to represent binding to an existing resource in a request payload. </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a nested resource info written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public abstract void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink);


        /// <summary> Asynchronously writes an entity reference link, which is used to represent binding to an existing resource in a request payload. </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a nested resource info written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public virtual Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteEntityReferenceLink(entityReferenceLink));
        }

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public abstract void Flush();


        /// <summary>Flushes the write buffer to the underlying stream asynchronously.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public virtual Task FlushAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.Flush());
        }
    }
}
