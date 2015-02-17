//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementEntityGraphPrettyPrinter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Globalization;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Prints an entity graph for easy visualization
    /// </summary>
    public class ODataPayloadElementEntityGraphPrettyPrinter : ODataPayloadElementVisitorBase
    {
        private StringBuilder builder;
        private int currentDepth;

        /// <summary>
        /// Gets or sets the Astoria Service Descriptor
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaServiceDescriptor AstoriaServiceDescriptor { get; set; }

        /// <summary>
        /// Prints an entity graph
        /// </summary>
        /// <param name="rootElement">The root element of the payload</param>
        /// <returns> an entity graph to visualize</returns>
        public string PrettyPrint(ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.builder = new StringBuilder();
            this.currentDepth = 0;
            this.Recurse(rootElement);
            return this.builder.ToString();
        }

        /// <summary>
        /// Pretty prints for an EntityInstance
        /// </summary>
        /// <param name="payloadElement">Entity Instance to print</param>
        public override void Visit(EntityInstance payloadElement)
        {
            var indentString = this.GenerateIndent();
            string id = payloadElement.Id.Replace(this.AstoriaServiceDescriptor.ServiceUri.AbsoluteUri, string.Empty);
            this.builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}Id:{1}", indentString, id));
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits a Navigation Property and appends the name on to the string builder
        /// </summary>
        /// <param name="payloadElement">Navigation Property Instance</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            this.currentDepth++;
            var indentString = this.GenerateIndent();
            this.builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}", indentString, payloadElement.Name));
            this.currentDepth++;
            base.Visit(payloadElement);
            this.currentDepth--;
            this.currentDepth--;
        }

        private string GenerateIndent()
        {
            return new string(' ', this.currentDepth);
        }
    }
}
