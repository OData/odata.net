//---------------------------------------------------------------------
// <copyright file="ODataQueryValueEntityGraphPrettyPrinter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Visits the payload structure and prints an entity graph for easy visualization
    /// </summary>
    public class ODataQueryValueEntityGraphPrettyPrinter : IQueryValueVisitor<string>
    {
        private StringBuilder builder;
        private int maxDepth;
        private int currentDepth;

        /// <summary>
        /// Gets or sets the convention-based link generator to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataConventionBasedLinkGenerator LinkGenerator { get; set; }

        /// <summary>
        /// Gets or sets the Astoria Service Descriptor
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaServiceDescriptor AstoriaServiceDescriptor { get; set; }

        /// <summary>
        /// Prints an entity graph
        /// </summary>
        /// <param name="rootQueryValue">The root element of the Query</param>
        /// <param name="maximumPayloadDepth">Indicates how far within the payload graph to traverse.</param>
        /// <returns>Returns a string to help visualize the Entity graph</returns>
        public string PrettyPrint(QueryValue rootQueryValue, int maximumPayloadDepth)
        {
            this.maxDepth = maximumPayloadDepth;
            ExceptionUtilities.CheckArgumentNotNull(rootQueryValue, "rootQueryValue");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.builder = new StringBuilder();
            this.currentDepth = 0;
            rootQueryValue.Accept(this);

            return this.builder.ToString();
        }

        /// <summary>
        /// Visits the QueryCollectionValue
        /// </summary>
        /// <param name="value">Value to visit</param>
        /// <returns>a null value</returns>
        public string Visit(QueryCollectionValue value)
        {
            if (value.Elements != null)
            {
                foreach (var element in value.Elements)
                {
                    element.Accept(this);
                }
            }

            return null;
        }

        /// <summary>
        /// Visits the QueryRecordValue
        /// </summary>
        /// <param name="value">Value to visit</param>
        /// <returns>will always throw</returns>
        public string Visit(QueryRecordValue value)
        {
            throw new TaupoNotSupportedException();
        }

        /// <summary>
        /// Visits the QueryReferenceValue
        /// </summary>
        /// <param name="value">Value to visit</param>
        /// <returns>will always throw</returns>
        public string Visit(QueryReferenceValue value)
        {
            throw new TaupoNotSupportedException();
        }

        /// <summary>
        /// Visits the QueryScalarValue
        /// </summary>
        /// <param name="value">Value to visit</param>
        /// <returns>will always throw</returns>
        public string Visit(QueryScalarValue value)
        {
            this.builder.AppendLine(string.Format(CultureInfo.InvariantCulture, value.IsNull ? "<null>" : value.Value.ToString()));
            return null;
        }

        /// <summary>
        /// Visits the QueryStructuralValue
        /// </summary>
        /// <param name="value">Value to visit</param>
        /// <returns>a string that represents a query structural value</returns>
        public string Visit(QueryStructuralValue value)
        {
            var indent = this.GenerateIndent();
            var queryEntityType = value.Type as QueryEntityType;
            if (queryEntityType != null)
            {
                var idValue = this.LinkGenerator.GenerateEntityId(value).Replace(this.AstoriaServiceDescriptor.ServiceUri.AbsoluteUri, string.Empty);
                this.builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}Id:{1}", indent, idValue));

                // do not expand below configured level of expands likely writing out querydataset
                if (this.currentDepth > this.maxDepth)
                {
                    return null;
                }

                foreach (var member in queryEntityType.Properties.Where(p => p.IsNavigationProperty()))
                {
                    var memberValue = value.GetValue(member.Name);
                    this.currentDepth++;
                    var childIndent = this.GenerateIndent();
                    this.builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}", childIndent, member.Name));
                    this.currentDepth++;
                    memberValue.Accept(this);
                    this.currentDepth -= 2;
                }
            }

            return null;
        }

        private string GenerateIndent()
        {
            return new string(' ', this.currentDepth);
        }
    }
}
