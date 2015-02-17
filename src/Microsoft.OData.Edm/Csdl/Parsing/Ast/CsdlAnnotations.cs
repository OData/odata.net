//---------------------------------------------------------------------
// <copyright file="CsdlAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Annotations element.
    /// </summary>
    internal class CsdlAnnotations
    {
        private readonly List<CsdlAnnotation> annotations;
        private readonly string target;
        private readonly string qualifier;

        public CsdlAnnotations(IEnumerable<CsdlAnnotation> annotations, string target, string qualifier)
        {
            this.annotations = new List<CsdlAnnotation>(annotations);
            this.target = target;
            this.qualifier = qualifier;
        }

        public IEnumerable<CsdlAnnotation> Annotations
        {
            get { return this.annotations; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public string Target
        {
            get { return this.target; }
        }
    }
}
