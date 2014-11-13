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

namespace Microsoft.Data.OData.Query
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;

    /// <summary>
    /// Encapsulates the state of metadata binding.
    /// TODO: finish moving fields from MetadataBinder here and see if anything can be removed.
    /// </summary>
    internal sealed class BindingState
    {        
        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// The dictionary used to store mappings between Any visitor and corresponding segment paths
        /// </summary>
        private readonly Stack<RangeVariable> rangeVariables = new Stack<RangeVariable>();

        /// <summary>
        /// If there is a  $filter or $orderby, then this member holds the reference to the parameter node for the 
        /// implicit parameter ($it) for all expressions.
        /// </summary>
        private RangeVariable implicitRangeVariable;

        /// <summary>
        /// Collection of query option tokens associated with the currect query being processed.
        /// If a given query option is bound it should be removed from this collection.
        /// </summary>
        private List<CustomQueryOptionToken> queryOptions;

        /// <summary>
        /// Constructs a <see cref="BindingState"/> with the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        internal BindingState(ODataUriParserConfiguration configuration)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            this.configuration = configuration;
        }

        /// <summary>
        /// The model used for binding.
        /// </summary>
        internal IEdmModel Model
        {
            get 
            { 
                DebugUtils.CheckNoExternalCallers(); 
                return this.configuration.Model; 
            }
        }

        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        internal ODataUriParserConfiguration Configuration
        {
            get 
            { 
                DebugUtils.CheckNoExternalCallers(); 
                return this.configuration; 
            }
        }

        /// <summary>
        /// If there is a  $filter or $orderby, then this member holds the reference to the parameter node for the 
        /// implicit parameter ($it) for all expressions.
        /// </summary>
        internal RangeVariable ImplicitRangeVariable
        {
            get 
            { 
                DebugUtils.CheckNoExternalCallers();
                return this.implicitRangeVariable; 
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.implicitRangeVariable == null || value == null, "This should only get set once when first starting to bind a tree.");
                this.implicitRangeVariable = value;
            }
        }

        /// <summary>
        /// The dictionary used to store mappings between Any visitor and corresponding segment paths
        /// </summary>
        internal Stack<RangeVariable> RangeVariables
        {
            get
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.rangeVariables;
            }
        }

        /// <summary>
        /// Collection of query option tokens associated with the currect query being processed.
        /// If a given query option is bound it should be removed from this collection.
        /// </summary>
        internal List<CustomQueryOptionToken> QueryOptions
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.queryOptions;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.queryOptions = value;
            }
        }
    }
}
