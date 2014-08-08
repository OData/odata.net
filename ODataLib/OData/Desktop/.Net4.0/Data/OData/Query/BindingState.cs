//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
