//---------------------------------------------------------------------
// <copyright file="ITextTemplateTransformation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Text template transformation (T4) runner.
    /// </summary>
    public interface ITextTemplateTransformation
    {
        /// <summary>
        /// Runs the specified T4 template.
        /// </summary>
        /// <param name="templateFile">T4 template to run</param>
        /// <param name="outputFile">Output file</param>
        /// <param name="parameters">Named parameters to the template</param>
        /// <remarks>Parameter value can be read by the template with Host.ResolveParameterValue("","","parameterName")</remarks>
        void Transform(string templateFile, string outputFile, IEnumerable<KeyValuePair<string, string>> parameters);
    }
}
