//---------------------------------------------------------------------
// <copyright file="ResourceExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
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

        /// <summary>
        /// Translates to the $count segment.
        /// Example: http://host/service/Categories(1)/Products/$count
        /// </summary>
        CountSegment,

        /// <summary>
        /// Translates to the $count=true query option.
        /// Example: http://host/service/Products?$count=true
        /// </summary>
        CountQueryTrue,

        /// <summary>
        /// Translates to the $count=false query option.
        /// Example: http://host/service/Products?$count=false
        /// </summary>
        CountQueryFalse
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

        /// <summary>name of operation</summary>
        private string operationName;

        /// <summary>names and values of parameters</summary>
        private Dictionary<string, string> operationParameters;

        /// <summary>false for function, true for action</summary>
        private bool isAction;

        #endregion Fields

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
            : this(source, type, expandPaths, countOption, customQueryOptions, projection, resourceTypeAs, uriVersion, null, null, false)
        {
        }

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
        /// <param name="operationName">name of function</param>
        /// <param name="operationParameters">parameters' names and values of function</param>
        /// <param name="isAction">action flag</param>
        internal ResourceExpression(Expression source, Type type, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection, Type resourceTypeAs, Version uriVersion, string operationName, Dictionary<string, string> operationParameters, bool isAction)
        {
            this.source = source;
            this.type = type;
            this.expandPaths = expandPaths ?? new List<string>();
            this.countOption = countOption;
            this.customQueryOptions = customQueryOptions ?? new Dictionary<ConstantExpression, ConstantExpression>(ReferenceEqualityComparer<ConstantExpression>.Instance);
            this.projection = projection;
            this.ResourceTypeAs = resourceTypeAs;
            this.uriVersion = uriVersion ?? Util.ODataVersion4;
            this.operationName = operationName;
            this.OperationParameters = operationParameters ?? new Dictionary<string, string>(StringComparer.Ordinal);
            this.isAction = isAction;
        }

        /// <summary>
        /// The <see cref="Type"/> of the value represented by this <see cref="Expression"/>.
        /// </summary>
        public override Type Type
        {
            get { return this.type; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "type", Justification = "It is the value being used to set the field")]
        abstract internal ResourceExpression CreateCloneWithNewType(Type type);

        abstract internal bool HasQueryOptions { get; }

        abstract internal bool IsOperationInvocation { get; }

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
        /// operation name
        /// </summary>
        internal string OperationName
        {
            get { return this.operationName; }
            set { this.operationName = value; }
        }

        /// <summary>
        /// operation parameter names and parameters pair for Resource
        /// </summary>
        internal Dictionary<string, string> OperationParameters
        {
            get { return this.operationParameters; }
            set { this.operationParameters = value; }
        }

        /// <summary>
        /// false for function, true for action
        /// </summary>
        internal bool IsAction
        {
            get { return this.isAction; }
            set { this.isAction = value; }
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
