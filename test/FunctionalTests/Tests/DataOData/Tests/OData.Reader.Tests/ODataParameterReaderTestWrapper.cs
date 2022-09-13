//---------------------------------------------------------------------
// <copyright file="ODataParameterReaderTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataParameterReader which allows for transparent monitoring and changing
    /// of the reader behavior.
    /// </summary>
    public sealed class ODataParameterReaderTestWrapper : ODataParameterReader
    {
        /// <summary>
        /// The underlying reader to wrap.
        /// </summary>
        private readonly ODataParameterReader parameterReader;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parameterReader">The reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataParameterReaderTestWrapper(ODataParameterReader parameterReader, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(parameterReader, "parameterReader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.parameterReader = parameterReader;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// The underlying reader.
        /// </summary>
        public ODataParameterReader ParameterReader
        {
            get { return this.parameterReader; }
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override ODataParameterReaderState State
        {
            get { return this.parameterReader.State; }
        }

        /// <summary>
        /// The name of the current parameter that is being read.
        /// </summary>
        public override string Name
        {
            get { return this.parameterReader.Name; }
        }

        /// <summary>
        /// The value of the current parameter that is being read.
        /// </summary>
        /// <remarks>
        /// This property returns a primitive value or null when State is ODataParameterReaderState.Value.
        /// This property returns null when State is ODataParameterReaderState.Entry, ODataParameterReaderState.ResourceSet or ODataParameterReaderState.Collection.
        /// </remarks>
        public override object Value
        {
            get { return this.parameterReader.Value; }
        }

        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the entry value when the state is ODataParameterReaderState.Entry.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Entry, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the entry value when the state is ODataParameterReaderState.Entry.</returns>
        public override ODataReader CreateResourceReader()
        {
            return new ODataReaderTestWrapper(this.parameterReader.CreateResourceReader(), this.testConfiguration);
        }

        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the feed value when the state is ODataParameterReaderState.ResourceSet.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.ResourceSet, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the feed value when the state is ODataParameterReaderState.ResourceSet.</returns>
        public override ODataReader CreateResourceSetReader()
        {
            return new ODataReaderTestWrapper(this.parameterReader.CreateResourceSetReader(), this.testConfiguration);
        }

        /// <summary>
        /// This method creates an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Collection, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.</returns>
        public override ODataCollectionReader CreateCollectionReader()
        {
            return new ODataCollectionReaderTestWrapper(this.parameterReader.CreateCollectionReader(), this.testConfiguration);
        }

        /// <summary>
        /// Reads the next item from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override bool Read()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.parameterReader.Read();
            }
            else
            {
                return this.parameterReader.ReadAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously reads the next item from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override Task<bool> ReadAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }
    }
}
