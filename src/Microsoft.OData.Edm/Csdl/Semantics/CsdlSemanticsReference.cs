//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlReference.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("CsdlSemanticsReference({Uri})")]
    internal class CsdlSemanticsReference : CsdlSemanticsElement, IEdmReference
    {
        protected readonly CsdlSemanticsModel model;
        protected CsdlReference reference;

        private readonly Cache<CsdlSemanticsReference, List<IEdmInclude>> includesCache = new Cache<CsdlSemanticsReference, List<IEdmInclude>>();
        private static readonly Func<CsdlSemanticsReference, List<IEdmInclude>> ComputeIncludeFunc = (me) => me.ComputeIncludes();

        private readonly Cache<CsdlSemanticsReference, List<IEdmIncludeAnnotations>> includeAnnotationsCache = new Cache<CsdlSemanticsReference, List<IEdmIncludeAnnotations>>();
        private static readonly Func<CsdlSemanticsReference, List<IEdmIncludeAnnotations>> ComputeIncludeAnnotationsFunc = (me) => me.ComputeIncludeAnnotations();

        public CsdlSemanticsReference(CsdlSemanticsModel model, CsdlReference reference)
            : base(reference)
        {
            this.model = model;
            this.reference = reference;
        }

        public override CsdlSemanticsModel Model => model;

        public override CsdlElement Element => reference;

        public Uri Uri => new Uri(this.reference.Uri, UriKind.RelativeOrAbsolute);

        public IEnumerable<IEdmInclude> Includes
        {
            get { return this.includesCache.GetValue(this, ComputeIncludeFunc, null); }
        }

        public IEnumerable<IEdmIncludeAnnotations> IncludeAnnotations
        {
            get { return this.includeAnnotationsCache.GetValue(this, ComputeIncludeAnnotationsFunc, null); }
        }

        protected List<IEdmInclude> ComputeIncludes()
        {
            List<IEdmInclude> includes = new List<IEdmInclude>();
            foreach (var include in this.reference.Includes)
            {
                includes.Add(new CsdlSemanticsInclude(this.model, include));
            }

            return includes;
        }

        protected List<IEdmIncludeAnnotations> ComputeIncludeAnnotations()
        {
            List<IEdmIncludeAnnotations> includeAnnotatations = new List<IEdmIncludeAnnotations>();
            foreach (var includeAnnotation in this.reference.IncludeAnnotations)
            {
                includeAnnotatations.Add(new CsdlSemanticsIncludeAnnotations(this.model, includeAnnotation));
            }

            return includeAnnotatations;
        }
    }
}
