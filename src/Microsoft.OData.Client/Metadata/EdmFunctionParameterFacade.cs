//---------------------------------------------------------------------
// <copyright file="EdmFunctionParameterFacade.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client.Metadata
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Wraps a function parameter from the server-side model.
    /// </summary>
    internal class EdmFunctionParameterFacade : IEdmFunctionParameter
    {
        /// <summary>
        /// The function parameter from the server-side model to wrap.
        /// </summary>
        private readonly IEdmFunctionParameter serverFunctionParameter;

        /// <summary>
        /// The type of this function parameter.
        /// </summary>
        private readonly IEdmTypeReference type;

        /// <summary>
        /// The function or function import that declared this parameter.
        /// </summary>
        private readonly IEdmFunctionBase declaringFunction;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmFunctionParameterFacade"/> class.
        /// </summary>
        /// <param name="serverFunctionParameter">The function parameter from the server-side model to wrap.</param>
        /// <param name="declaringFunctionFacade">The function import facade which this parameter belongs to.</param>
        /// <param name="modelFacade">The edm model facade this function import belongs to.</param>
        public EdmFunctionParameterFacade(IEdmFunctionParameter serverFunctionParameter, EdmFunctionImportFacade declaringFunctionFacade, EdmModelFacade modelFacade)
        {
            Debug.Assert(serverFunctionParameter != null, "serverFunctionParameter != null");
            Debug.Assert(declaringFunctionFacade != null, "declaringFunctionFacade != null");
            Debug.Assert(modelFacade != null, "modelFacade != null");

            this.serverFunctionParameter = serverFunctionParameter;
            this.declaringFunction = declaringFunctionFacade;
            this.type = modelFacade.GetOrCreateEntityTypeFacadeOrReturnNonEntityServerType(serverFunctionParameter.Type.Definition).ToEdmTypeReference(serverFunctionParameter.Type.IsNullable);
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.serverFunctionParameter.Name; }
        }

        /// <summary>
        /// Gets the type of this function parameter.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the function or function import that declared this parameter.
        /// </summary>
        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }

        /// <summary>
        /// Gets the mode of this function parameter.
        /// </summary>
        public EdmFunctionParameterMode Mode
        {
            get { return this.serverFunctionParameter.Mode; }
        }
    }
}