//---------------------------------------------------------------------
// <copyright file="ClientExpectedErrorComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for verifying the SendingRequest event raised by the data service context
    /// </summary>
    [ImplementationName(typeof(IClientExpectedErrorComparer), "Default")]
    public class ClientExpectedErrorComparer : IClientExpectedErrorComparer
    {
        private IStringResourceVerifier systemDataServicesVerifier;
        private IStringResourceVerifier systemDataServicesClientVerifier;

        /// <summary>
        /// Initializes a new instance of the ClientExpectedErrorComparer class.
        /// </summary>
        /// <param name="systemDataServicesVerifier">Resource Verifier for Microsoft.OData.Service</param>
        /// <param name="systemDataServicesClientVerifier">Resource Verifier for Microsoft.OData.Client</param>
        public ClientExpectedErrorComparer(IStringResourceVerifier systemDataServicesVerifier, IStringResourceVerifier systemDataServicesClientVerifier)
        {
            this.systemDataServicesVerifier = systemDataServicesVerifier;
            this.systemDataServicesClientVerifier = systemDataServicesClientVerifier;
            this.ShouldVerifyServerMessageInClientException = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Expected message is a server or client error
        /// </summary>
        [InjectTestParameter("ShouldVerifyServerMessageInClientException", DefaultValueDescription = "true", HelpText = "If true will verify server error in the Client Exception")]
        public bool ShouldVerifyServerMessageInClientException { get; set; }

        /// <summary>
        /// Gets or sets the Assertion handler
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the Xml error payload deserializer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector ProtocolFormatStrategySelector { get; set; }

        /// <summary>
        /// Compares the ClientExpectedException to the provided exception and verifies the exception is correct
        /// </summary>
        /// <param name="expectedClientException">Expected Client Exception</param>
        /// <param name="exception">Actual Exception</param>
        public void Compare(ExpectedClientErrorBaseline expectedClientException, Exception exception)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            if (expectedClientException == null)
            {
                string exceptionMessage = null;
                if (exception != null)
                {
                    exceptionMessage = exception.ToString();
                }

                this.Assert.IsNull(exception, "Expected to not recieve an exception:" + exceptionMessage);
                return;
            }

            this.Assert.IsNotNull(exception, "Expected Exception to not be null");
            this.Assert.AreEqual(expectedClientException.ExpectedExceptionType, exception.GetType(), string.Format(CultureInfo.InvariantCulture, "ExceptionType is not equal, receieved the following exception: {0}", exception.ToString()));
            
            if (expectedClientException.HasServerSpecificExpectedMessage)
            {
                if (this.ShouldVerifyServerMessageInClientException)
                {
                    this.Assert.IsNotNull(exception.InnerException, "Expected Inner Exception to contain ODataError");
                    byte[] byteArrPayload = HttpUtilities.DefaultEncoding.GetBytes(exception.InnerException.Message);
                    ODataErrorPayload errorPayload = this.ProtocolFormatStrategySelector.DeserializeAndCast<ODataErrorPayload>(null, MimeTypes.ApplicationXml, byteArrPayload);

                    expectedClientException.ExpectedExceptionMessage.VerifyMatch(this.systemDataServicesVerifier, errorPayload.Message, true);
                }   
            }
            else
            {
                expectedClientException.ExpectedExceptionMessage.VerifyMatch(this.systemDataServicesClientVerifier, exception.Message, true);
            }
        }
    }
}
