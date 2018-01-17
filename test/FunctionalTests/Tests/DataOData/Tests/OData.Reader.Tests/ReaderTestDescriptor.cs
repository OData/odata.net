//---------------------------------------------------------------------
// <copyright file="ReaderTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Base test descriptor class for all reader tests.
    /// </summary>
    public abstract class ReaderTestDescriptor
    {
        /// <summary>
        /// Cached model. This is filled only once the model is requested which happens when the first test configuration
        /// is ran using this test descriptor. After that the test descriptor should not change so it's safe to cache.
        /// </summary>
        protected IEdmModel cachedModel;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ReaderTestDescriptor()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other test descriptor to copy values from.</param>
        protected ReaderTestDescriptor(ReaderTestDescriptor other)
        {
            this.Model = other.Model;
            this.Annotations = other.Annotations;
            this.ExpectedResultCallback = other.ExpectedResultCallback;
            this.DebugDescription = other.DebugDescription;
            this.SkipTestConfiguration = other.SkipTestConfiguration;
        }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public virtual ODataPayloadKind PayloadKind { get; set; }

        /// <summary>
        /// The model to use for the test - can be null.
        /// </summary>
        protected IEdmModel Model { get; set; }

        /// <summary>
        /// The annotations for the test - can be null.
        /// </summary>
        public string Annotations { get; set; }

        /// <summary>
        /// A func which returns expected result for the test based on the test configuration.
        /// </summary>
        public Func<ReaderTestConfiguration, ReaderTestExpectedResult> ExpectedResultCallback
        {
            get;
            set;
        }

        /// <summary>
        /// A description of the test, used for debugging.
        /// </summary>
        public string DebugDescription { get; set; }

        /// <summary>
        /// If the func is specified it is executed before the test is ran for a given test configuration.
        /// If the func returns true, the test is not ran and "success" is reported right away.
        /// </summary>
        public Func<ReaderTestConfiguration, bool> SkipTestConfiguration { get; set; }

        /// <summary>
        /// Removes the cached model, forcing it to be regenerated on the next test run.
        /// </summary>
        public void ResetCachedModel()
        {
            this.cachedModel = null;
        }

        /// <summary>
        /// Runs the test specified by this test descriptor.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use for running the test.</param>
        public virtual void RunTest(ReaderTestConfiguration testConfiguration)
        {
            if (this.ShouldSkipForTestConfiguration(testConfiguration))
            {
                return;
            }

            TestMessage message = this.CreateInputMessage(testConfiguration);
            IEdmModel model = this.GetMetadataProvider(testConfiguration);
            ReaderTestExpectedResult expectedResult = this.GetExpectedResult(testConfiguration);
            ExceptionUtilities.Assert(expectedResult != null, "The expected result could not be determined for the test. Did you specify it?");

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                using (ODataMessageReaderTestWrapper messageReaderWrapper = TestReaderUtils.CreateMessageReader(message, model, testConfiguration))
                {
                    expectedResult.VerifyResult(messageReaderWrapper, this.PayloadKind, testConfiguration);
                }
            });

            try
            {
                expectedResult.VerifyException(exception);
            }
            catch (Exception)
            {
                this.TraceFailureInformation(testConfiguration);
                throw;
            }
        }

        /// <summary>
        /// Returns text description of the test.
        /// </summary>
        /// <returns>A humanly readable description of the test, used for debugging.</returns>
        public override string ToString()
        {
            string result = string.Format("Payload Kind: {0}", this.PayloadKind);
            if (this.DebugDescription != null)
            {
                result = this.DebugDescription + Environment.NewLine + result;
            }

            return result;
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected abstract TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration);

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected virtual ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            if (this.ExpectedResultCallback != null)
            {
                return this.ExpectedResultCallback(testConfiguration);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected virtual bool ShouldSkipForTestConfiguration(ReaderTestConfiguration testConfiguration)
        {
            if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets The model to use for the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The model to use for the test.</returns>
        protected virtual IEdmModel GetMetadataProvider(ReaderTestConfiguration testConfiguration)
        {
            if (this.cachedModel == null && this.Model != null)
            {
                if (this.Annotations != null)
                {
                    this.cachedModel = EdmModelBuilder.BuildAnnotationModel(this.Annotations, this.Model);
                }
                else
                {
                    this.cachedModel = this.Model;
                }
            }

            return this.cachedModel;
        }

        /// <summary>
        /// If overriden dumps the content of an input message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the input message.</returns>
        protected virtual string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
        {
            return "<Message content not available>";
        }

        /// <summary>
        /// If overridden dumps additional description of the test descriptor for the specified testConfiguration.
        /// This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>String description of the test.</returns>
        protected virtual string DumpAdditionalTestDescriptions(ReaderTestConfiguration testConfiguration)
        {
            return string.Empty;
        }

        /// <summary>
        /// Traces interesting information if on test failure.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        protected void TraceFailureInformation(ReaderTestConfiguration testConfiguration)
        {
            try
            {
                Trace.WriteLine("----- HTTP Message start ------------------------------------------");

                // Create the input message again so that we can read it.
                TestMessage message = this.CreateInputMessage(testConfiguration);
                TestRequestMessage requestMessage = message as TestRequestMessage;
                if (requestMessage != null)
                {
                    Trace.WriteLine(requestMessage.Method.ToString() + " " + requestMessage.Url + " HTTP/1.1");
                }

                TestResponseMessage responseMessage = message as TestResponseMessage;
                if (responseMessage != null)
                {
                    Trace.WriteLine(responseMessage.StatusCode.ToString());
                }

                foreach (var header in message.Headers)
                {
                    Trace.WriteLine(header.Key + ": " + header.Value);
                }

                Trace.WriteLine("");

                Trace.WriteLine(this.DumpInputMessageContent(testConfiguration));
                Trace.WriteLine("----- HTTP Message end --------------------------------------------");

                string additionalDescription = this.DumpAdditionalTestDescriptions(testConfiguration);
                if (!string.IsNullOrEmpty(additionalDescription))
                {
                    Trace.WriteLine("");
                    Trace.WriteLine("----- Additional test description ---------------------------------");
                    Trace.WriteLine(additionalDescription);
                }
            }
            catch (Exception innerException)
            {
                // Ignore all exceptions here since we want to fail with the original test exception.
                Trace.WriteLine("Failed to dump the test message.");
                Trace.WriteLine(innerException);
            }
        }

        /// <summary>
        /// Creates a copy of this ReaderTestDescriptor (not implemented as this class is abstract)
        /// </summary>
        public abstract object Clone();
    }
}
