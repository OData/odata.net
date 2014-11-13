//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    #endregion Namespaces

    /// <summary>
    /// The counting option for the resource expression
    /// </summary>
    internal enum CountOption
    {
        /// <summary>No counting</summary>
        None,

        /// <summary>Translates to the $count segment.</summary>
        ValueOnly,

        /// <summary>Translates to the $inlinecount=allpages query option</summary>
        InlineAll
    }

    /// <summary>
    /// Abstract base class for expressions that support Query Options
    /// </summary>
    internal abstract class ResourceExpression : Expression
    {
        #region Fields

        /// <summary>Source expression.</summary>
        protected readonly Expression source;

        /// <summary>Singleton InputReferenceExpression that should be used to indicate a reference to this element of the resource path</summary>
        protected InputReferenceExpression inputRef;

        /// <summary>The CLR type this node will evaluate into.</summary>
        private Type type;

        /// <summary>expand paths</summary>
        private List<string> expandPaths;

        /// <summary>The count query option for the resource set</summary>
        private CountOption countOption;

        /// <summary>custom query options</summary>
        private Dictionary<ConstantExpression, ConstantExpression> customQueryOptions;

        /// <summary>projection expression</summary>
        private ProjectionQueryOptionExpression projection;

        /// <summary>Uri version for the expression and also the expand and projection paths</summary>
        private Version uriVersion;

        #endregion Fields

#if WINDOWS_PHONE_MANGO
        /// <summary>
        /// Creates a Resource expression
        /// </summary>
        /// <param name="source">the source expression</param>
        /// <param name="type">the return type of the expression</param>
        /// <param name="expandPaths">the expand paths</param>
        /// <param name="countOption">the count option</param>
        /// <param name="customQueryOptions">The custom query options</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        internal ResourceExpression(Expression source, ExpressionType nodeType, Type type, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion)
            : base(nodeType, type)
        {
            this.source = source;
            this.type = type;
            this.expandPaths = expandPaths ?? new List<string>();
            this.countOption = countOption;
            this.customQueryOptions = customQueryOptions ?? new Dictionary<ConstantExpression, ConstantExpression>(ReferenceEqualityComparer<ConstantExpression>.Instance);
            this.projection = projection;
            this.ResourceTypeAs = resourceTypeAs;
            this.uriVersion = uriVersion ?? Util.DataServiceVersion1;
        }
#else
        /// <summary>
        /// Creates a Resource expression
        /// </summary>
        /// <param name="source">the source expression</param>
        /// <param name="type">the return type of the expression</param>
        /// <param name="expandPaths">the expand paths</param>
        /// <param name="countOption">the count option</param>
        /// <param name="customQueryOptions">The custom query options</param>
        /// <param name="projection">the projection expression</param>
        /// <param name="resourceTypeAs">TypeAs type</param>
        /// <param name="uriVersion">version of the Uri from the expand and projection paths</param>
        internal ResourceExpression(Expression source, Type type, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion)
        {
            this.source = source;
            this.type = type;
            this.expandPaths = expandPaths ?? new List<string>();
            this.countOption = countOption;
            this.customQueryOptions = customQueryOptions ?? new Dictionary<ConstantExpression, ConstantExpression>(ReferenceEqualityComparer<ConstantExpression>.Instance);
            this.projection = projection;
            this.ResourceTypeAs = resourceTypeAs;
            this.uriVersion = uriVersion ?? Util.DataServiceVersion1;
        }

        /// <summary>
        /// The <see cref="Type"/> of the value represented by this <see cref="Expression"/>.
        /// </summary>
        public override Type Type
        {
            get { return this.type; }
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "type", Justification = "It is the value being used to set the field")]
        abstract internal ResourceExpression CreateCloneWithNewType(Type type);

        abstract internal bool HasQueryOptions { get; }

        /// <summary>
        /// Resource type for this expression (for sets, this is the element type).
        /// Never null.
        /// </summary>
        internal abstract Type ResourceType { get; }

        /// <summary>
        /// The resource type that this expression is explicitly converted to by a TypeAs
        /// expression (i.e., "as" operator in C#, "TryCast" in VB). Null if no conversion.
        /// </summary>
        internal Type ResourceTypeAs { get; set; }

        /// <summary>
        /// Uri version from the expand and projection paths
        /// </summary>
        internal Version UriVersion
        {
            get
            {
                return this.uriVersion;
            }
        }

        /// <summary>
        /// Does this expression produce at most 1 resource?
        /// </summary>
        abstract internal bool IsSingleton { get; }

        /// <summary>
        /// Expand query option for ResourceSet
        /// </summary>
        internal virtual List<string> ExpandPaths
        {
            get { return this.expandPaths; }
            set { this.expandPaths = value; }
        }

        /// <summary>
        /// Count query option for ResourceSet
        /// </summary>
        internal virtual CountOption CountOption
        {
            get { return this.countOption; }
            set { this.countOption = value; }
        }

        /// <summary>
        /// custom query options for ResourceSet
        /// </summary>
        internal virtual Dictionary<ConstantExpression, ConstantExpression> CustomQueryOptions
        {
            get { return this.customQueryOptions; }
            set { this.customQueryOptions = value; }
        }

        /// <summary>Description of the projection on a resource.</summary>
        /// <remarks>
        /// This property is set by the ProjectionAnalyzer component (so it
        /// mutates this instance), or by the ResourceBinder when it clones
        /// a ResourceExpression.
        /// </remarks>
        internal ProjectionQueryOptionExpression Projection
        {
            get { return this.projection; }
            set { this.projection = value; }
        }

        /// <summary>
        /// Gets the source expression.
        /// </summary>
        internal Expression Source
        {
            get
            {
                return this.source;
            }
        }

        /// <summary>
        /// Creates an <see cref="InputReferenceExpression"/> that refers to this component of the resource path. 
        /// The returned expression is guaranteed to be reference-equal (object.ReferenceEquals)
        /// to any other InputReferenceExpression that also refers to this resource path component.
        /// </summary>
        /// <returns>The InputReferenceExpression that refers to this resource path component</returns>
        internal InputReferenceExpression CreateReference()
        {
            if (this.inputRef == null)
            {
                this.inputRef = new InputReferenceExpression(this);
            }

            return this.inputRef;
        }

        /// <summary>Raise the UriVersion if it is lower than <paramref name="newVersion"/>.</summary>
        /// <param name="newVersion">Uri version from the expand and projection paths</param>
        internal void RaiseUriVersion(Version newVersion)
        {
            Debug.Assert(newVersion != null, "newVersion != null");
            WebUtil.RaiseVersion(ref this.uriVersion, newVersion);
        }
    }
}
