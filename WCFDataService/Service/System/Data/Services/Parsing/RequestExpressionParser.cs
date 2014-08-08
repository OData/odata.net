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

namespace System.Data.Services.Parsing
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Strings = System.Data.Services.Strings;
    #endregion Namespaces

    /// <summary>Use this class to parse an expression in the Astoria URI format.</summary>
    [DebuggerDisplay("ExpressionParser ({lexer.text})")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Won't Fix")]
    internal class RequestExpressionParser
    {
        #region Fields

        /// <summary>Provider of data and metadata.</summary>
        private readonly DataServiceProviderWrapper provider;

        /// <summary>The expression text to parse.</summary>
        private readonly string expressionText;

        /// <summary>The service's metadata-provider-based edm-model.</summary>
        private readonly MetadataProviderEdmModel model;

        /// <summary>The target resource type or null.</summary>
        private readonly ResourceType targetResourceType;

        /// <summary>The target resource set or null.</summary>
        private readonly ResourceSetWrapper targetResourceSet;

        #endregion Fields

        #region Constructors

        /// <summary>Initializes a new <see cref="RequestExpressionParser"/>.</summary>
        /// <param name="service">Service with data and configuration.</param>
        /// <param name="requestDescription">RequestDescription instance containing information about the current request being parsed.</param>
        /// <param name="expression">Expression to parse.</param>
        internal RequestExpressionParser(IDataService service, RequestDescription requestDescription, string expression)
        {
            Debug.Assert(service != null, "service != null");
            Debug.Assert(expression != null, "expression != null");
            Debug.Assert(requestDescription != null, "requestDescription != null");

            this.provider = service.Provider;

            this.expressionText = expression;
            this.model = this.provider.GetMetadataProviderEdmModel();

            this.targetResourceType = requestDescription.TargetResourceType;
            this.targetResourceSet = requestDescription.TargetResourceSet;
        }

        #endregion Constructors

        /// <summary>Parses the text expression for $filter.</summary>
        /// <returns>The parsed filter clause.</returns>
        internal FilterClause ParseFilter()
        {
            IEdmSchemaType targetType = this.targetResourceType != null ? this.model.EnsureSchemaType(this.targetResourceType) : null;
            IEdmEntitySet targetSet = this.targetResourceSet != null ? this.model.EnsureEntitySet(this.targetResourceSet) : null;

            try
            {
                return ODataUriParser.ParseFilter(this.expressionText, this.model, targetType, targetSet);
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
        }

        /// <summary>Parses the text expression for ordering.</summary>
        /// <returns>An orderby clause.</returns>
        internal OrderByClause ParseOrderBy()
        {
            IEdmSchemaType targetType = this.targetResourceType != null ? this.model.EnsureSchemaType(this.targetResourceType) : null;
            IEdmEntitySet targetSet = this.targetResourceSet != null ? this.model.EnsureEntitySet(this.targetResourceSet) : null;

            try
            {
                return ODataUriParser.ParseOrderBy(this.expressionText, this.model, targetType, targetSet);
            }
            catch (ODataException ex)
            {
                throw new DataServiceException(400, null, ex.Message, null, ex);
            }
        }
    }
}
