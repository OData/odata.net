//---------------------------------------------------------------------
// <copyright file="FunctionSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Represents a function call within a uri
    /// </summary>
    public class FunctionSegment : ODataUriSegment
    {
        /// <summary>
        /// Initializes a new instance of the FunctionSegment class
        /// </summary>
        /// <param name="function">the function representing the Function</param>
        internal FunctionSegment(Function function)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            this.Container = null;
            this.Function = function;
            this.UseParentheses = false;
        }

        /// <summary>
        /// Initializes a new instance of the FunctionSegment class
        /// </summary>
        /// <param name="function">the function representing the Function</param>
        /// <param name="useParentheses">used to determine if the uri generated will have parentheses</param>
        internal FunctionSegment(Function function, bool useParentheses)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            this.Container = null;
            this.Function = function;
            this.UseParentheses = useParentheses;
        }

        /// <summary>
        /// Initializes a new instance of the FunctionSegment class
        /// </summary>
        /// <param name="function">the function representing the Function</param>
        /// <param name="container">the entity container for the function</param>
        internal FunctionSegment(Function function, EntityContainer container)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            this.Container = container;
            this.Function = function;
            this.UseParentheses = false;
        }

        /// <summary>
        /// Initializes a new instance of the FunctionSegment class
        /// </summary>
        /// <param name="function">the function representing the Function</param>
        /// <param name="container">the entity container for the function</param>
        /// <param name="useParentheses">used to determine if the uri generated will have parentheses</param>
        internal FunctionSegment(Function function, EntityContainer container, bool useParentheses)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            this.Container = container;
            this.Function = function;
            this.UseParentheses = useParentheses;
        }

        /// <summary>
        /// Gets the Entity Container for the function
        /// </summary>
        public EntityContainer Container { get; private set; } 

        /// <summary>
        /// Gets the Function
        /// </summary>
        public Function Function { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to use parenthese or not
        /// </summary>
        public bool UseParentheses { get; private set; }

        /// <summary>
        /// Gets the segment type
        /// </summary>
        public override ODataUriSegmentType SegmentType
        {
            get { return ODataUriSegmentType.Function; }
        }

        /// <summary>
        /// Gets whether or not the segment has a preceding slash
        /// </summary>
        protected internal override bool HasPrecedingSlash
        {
            get { return true; }
        }
    }
}
