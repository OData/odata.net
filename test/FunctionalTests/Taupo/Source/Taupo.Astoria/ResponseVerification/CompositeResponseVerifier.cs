//---------------------------------------------------------------------
// <copyright file="CompositeResponseVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A verifier that uses multiple internal verifiers
    /// </summary>
    public class CompositeResponseVerifier : ResponseVerifierBase
    {
        /// <summary>
        /// Initializes a new instance of the CompositeResponseVerifier class
        /// </summary>
        /// <param name="verifiers">The initial set of internal verifiers</param>
        public CompositeResponseVerifier(params IResponseVerifier[] verifiers)
            : base()
        {
            this.Verifiers = verifiers.ToList();
        }

        /// <summary>
        /// Gets or sets the response verification context
        /// </summary>
        [InjectDependency]
        public IResponseVerificationContext Context { get; set; }

        /// <summary>
        /// Gets the list of verifiers that make up the composite verifier
        /// </summary>
        public IList<IResponseVerifier> Verifiers { get; private set; }

        /// <summary>
        /// Verify the given OData request/response pair against all attached verifiers that apply
        /// </summary>
        /// <param name="request">The request to verify</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);

            IDisposable scope = null;
            if (this.Context != null)
            {
                scope = this.Context.Begin(request);
            }

            try
            {
                foreach (var verifier in this.Verifiers)
                {
                    // make sure the verifier applies
                    var selective = verifier as ISelectiveResponseVerifier;
                    if (selective == null || (selective.Applies(request) && selective.Applies(response)))
                    {
                        try
                        {
                            verifier.Verify(request, response);
                        }
                        catch (ResponseVerificationException e)
                        {
                            // wrap and re-throw to preserve original call-stack. Ideally we would just let these through.
                            throw new ResponseVerificationException(e);
                        }
                        catch (Exception e)
                        {
                            this.Logger.WriteLine(LogLevel.Error, "Verifier '{0}' threw unexpected exception '{1}'", verifier, e.Message);
                            this.ReportFailure(request, response);
                            throw new ResponseVerificationException(e);
                        }
                    }
                }
            }
            finally
            {
                if (scope != null)
                {
                    scope.Dispose();
                }
            }
        }
    }
}
