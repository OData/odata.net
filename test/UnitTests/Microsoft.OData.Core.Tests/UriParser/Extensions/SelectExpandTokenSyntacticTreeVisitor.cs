//---------------------------------------------------------------------
// <copyright file="SelectExpandTokenSyntacticTreeVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Tests.UriParser
{
    internal class SelectExpandTokenSyntacticTreeVisitor : SyntacticTreeVisitor<string>
    {
        /// <summary>
        /// Visits an ExpandToken
        /// </summary>
        /// <param name="tokenIn">The ExpandToken to visit</param>
        /// <returns>A QueryNode bound to this ExpandToken</returns>
        public override string Visit(ExpandToken tokenIn)
        {
            StringBuilder sb = new StringBuilder("$expand=");

            bool termTokenAdded = false;
            foreach (var term in tokenIn.ExpandTerms)
            {
                if (!termTokenAdded)
                {
                    termTokenAdded = true;
                }
                else
                {
                    sb.Append(",");
                }

                VisitSelectExpandTermToken(term, sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Visits a SelectToken
        /// </summary>
        /// <param name="tokenIn">The SelectToken to bind</param>
        /// <returns>A QueryNode bound to this SelectToken</returns>
        public override string Visit(SelectToken tokenIn)
        {
            StringBuilder sb = new StringBuilder("$select=");

            bool termTokenAdded = false;
            foreach (var term in tokenIn.SelectTerms)
            {
                if (!termTokenAdded)
                {
                    termTokenAdded = true;
                }
                else
                {
                    sb.Append(",");
                }

                VisitSelectExpandTermToken(term, sb);
            }

            return sb.ToString();
        }

        private void VisitSelectExpandTermToken(SelectExpandTermToken term, StringBuilder sb)
        {
            string pathString = term.PathToProperty.ToPathString();
            sb.Append(pathString);

            bool firstInnerOptionsAdded = false;
            if (term.FilterOption != null)
            {
                sb.Append("(");
                sb.Append(term.FilterOption.Accept(this));
                firstInnerOptionsAdded = true;
            }

            if (term.OrderByOptions != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                bool orderbyAdded = false;
                sb.Append("$orderby=");
                foreach (var orderby in term.OrderByOptions)
                {
                    if (!orderbyAdded)
                    {
                        sb.Append(",");
                        orderbyAdded = true;
                    }
                    sb.Append(orderby.Accept(this));
                }

                firstInnerOptionsAdded = true;
            }

            if (term.TopOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append("$top=" + term.TopOption.Value.ToString());
                firstInnerOptionsAdded = true;
            }

            if (term.SkipOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append("$skip=" + term.SkipOption.Value.ToString());
                firstInnerOptionsAdded = true;
            }

            if (term.CountQueryOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append("$count=" + term.CountQueryOption.Value.ToString().ToLower());
                firstInnerOptionsAdded = true;
            }

            if (term.SearchOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append(term.SearchOption.Accept(this));
                firstInnerOptionsAdded = true;
            }

            if (term.SelectOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append(term.SelectOption.Accept(this));
                firstInnerOptionsAdded = true;
            }

            if (term.ComputeOption != null)
            {
                if (!firstInnerOptionsAdded)
                {
                    sb.Append("(");
                }
                else
                {
                    sb.Append(";");
                }

                sb.Append(term.ComputeOption.Accept(this));
                firstInnerOptionsAdded = true;
            }

            ExpandTermToken expandTerm = term as ExpandTermToken;

            if (expandTerm != null)
            {
                if (expandTerm.ExpandOption != null)
                {
                    if (!firstInnerOptionsAdded)
                    {
                        sb.Append("(");
                    }
                    else
                    {
                        sb.Append(";");
                    }

                    sb.Append(expandTerm.ExpandOption.Accept(this));
                    firstInnerOptionsAdded = true;
                }

                if (expandTerm.LevelsOption != null)
                {
                    if (!firstInnerOptionsAdded)
                    {
                        sb.Append("(");
                    }
                    else
                    {
                        sb.Append(";");
                    }

                    sb.Append("$levels=" + expandTerm.LevelsOption.Value.ToString());
                    firstInnerOptionsAdded = true;
                }

                if (expandTerm.ApplyOptions != null)
                {
                    if (!firstInnerOptionsAdded)
                    {
                        sb.Append("(");
                    }
                    else
                    {
                        sb.Append(";");
                    }

                    bool applyAdded = false;
                    sb.Append("$apply=");
                    foreach (var apply in expandTerm.ApplyOptions)
                    {
                        if (!applyAdded)
                        {
                            sb.Append(",");
                            applyAdded = true;
                        }
                        sb.Append(apply.Accept(this));
                    }

                    firstInnerOptionsAdded = true;
                }
            }

            if (firstInnerOptionsAdded)
            {
                sb.Append(")");
            }
        }
    }
}