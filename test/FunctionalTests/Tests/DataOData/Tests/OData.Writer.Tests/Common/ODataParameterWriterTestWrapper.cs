//---------------------------------------------------------------------
// <copyright file="ODataParameterWriterTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataParameterWriter which allows for transparent monitoring and changing
    /// of the writer behavior.
    /// </summary>
    public sealed class ODataParameterWriterTestWrapper : ODataParameterWriter
    {
        /// <summary>
        /// The underlying writer to wrap.
        /// </summary>
        private readonly ODataParameterWriter parameterWriter;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly WriterTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterWriter">The writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataParameterWriterTestWrapper(ODataParameterWriter parameterWriter, WriterTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(parameterWriter, "parameterWriter");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.parameterWriter = parameterWriter;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// The underlying writer.
        /// </summary>
        public ODataParameterWriter ParameterWriter
        {
            get { return this.parameterWriter; }
        }
        
        /// <summary>
        /// Start writing a parameter payload.
        /// </summary>
        public override void WriteStart()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.parameterWriter.WriteStart();
            }
            else
            {
                this.parameterWriter.WriteStartAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a parameter payload.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Start writing a value parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        public override void WriteValue(string parameterName, object parameterValue)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.parameterWriter.WriteValue(parameterName, parameterValue);
            }
            else
            {
                this.parameterWriter.WriteValueAsync(parameterName, parameterValue).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a value parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteValueAsync(string parameterName, object parameterValue)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter"/> to write the value of a collection parameter.
        /// </summary>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        /// <returns>The newly created <see cref="ODataCollectionWriter"/>.</returns>
        public override ODataCollectionWriter CreateCollectionWriter(string parameterName)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionWriterTestWrapper(this.parameterWriter.CreateCollectionWriter(parameterName), this.testConfiguration);
            }
            else
            {
                return this.parameterWriter.CreateCollectionWriterAsync(parameterName)
                    .ContinueWith(task => new ODataCollectionWriterTestWrapper(task.Result, this.testConfiguration), TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter"/> to write the value of a collection parameter.
        /// </summary>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        /// <returns>A running task for the created writer.</returns>
        public override Task<ODataCollectionWriter> CreateCollectionWriterAsync(string parameterName)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        public override ODataWriter CreateResourceWriter(string parameterName)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.parameterWriter.CreateResourceWriter(parameterName), this.testConfiguration);
            }
            else
            {
                return this.parameterWriter.CreateResourceWriterAsync(parameterName)
                    .ContinueWith(task => new ODataWriterTestWrapper(task.Result, this.testConfiguration), TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        public override Task<ODataWriter> CreateResourceWriterAsync(string parameterName)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        public override ODataWriter CreateResourceSetWriter(string parameterName)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.parameterWriter.CreateResourceSetWriter(parameterName), this.testConfiguration);
            }
            else
            {
                return this.parameterWriter.CreateResourceSetWriterAsync(parameterName)
                    .ContinueWith(task => new ODataWriterTestWrapper(task.Result, this.testConfiguration), TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        public override Task<ODataWriter> CreateResourceSetWriterAsync(string parameterName)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Finish writing a parameter payload.
        /// </summary>
        public override void WriteEnd()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.parameterWriter.WriteEnd();
            }
            else
            {
                this.parameterWriter.WriteEndAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously finish writing a parameter payload.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteEndAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public override void Flush()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.parameterWriter.Flush();
            }
            else
            {
                this.parameterWriter.FlushAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }
    }
}
