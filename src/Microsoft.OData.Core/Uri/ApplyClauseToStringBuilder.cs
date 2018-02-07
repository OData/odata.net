using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Text;

namespace Microsoft.OData
{
    internal sealed class ApplyClauseToStringBuilder
    {
        private readonly NodeToStringBuilder _nodeToStringBuilder;
        private readonly StringBuilder _query;

        public ApplyClauseToStringBuilder()
        {
            _nodeToStringBuilder = new NodeToStringBuilder();
            _query = new StringBuilder();
        }

        public string TranslateApplyClause(ApplyClause applyClause)
        {
            ExceptionUtils.CheckArgumentNotNull(applyClause, nameof(applyClause));

            _query.Append(ExpressionConstants.QueryOptionApply);
            _query.Append(ExpressionConstants.SymbolEqual);

            bool appendSlash = false;
            foreach (TransformationNode transformation in applyClause.Transformations)
            {
                appendSlash = AppendSlash(appendSlash);
                Translate(transformation);
            }

            return appendSlash ? _query.ToString() : string.Empty;
        }

        private bool AppendComma(bool appendComma)
        {
            if (appendComma)
                _query.Append(ExpressionConstants.SymbolComma);
            return true;
        }

        private void AppendExpression(SingleValueNode expression)
        {
            string text = Uri.EscapeDataString(_nodeToStringBuilder.TranslateNode(expression));
            _query.Append(text);
        }

        private bool AppendSlash(bool appendSlash)
        {
            if (appendSlash)
                _query.Append(ExpressionConstants.SymbolForwardSlash);
            return true;
        }

        private void AppendWord(string word)
        {
            _query.Append(word);
            _query.Append(ExpressionConstants.SymbolEscapedSpace);
        }

        private static string GetAggregationMethodName(AggregateExpression aggExpression)
        {
            switch (aggExpression.Method)
            {
                case AggregationMethod.Average:
                    return ExpressionConstants.KeywordAverage;
                case AggregationMethod.CountDistinct:
                    return ExpressionConstants.KeywordCountDistinct;
                case AggregationMethod.Max:
                    return ExpressionConstants.KeywordMax;
                case AggregationMethod.Min:
                    return ExpressionConstants.KeywordMin;
                case AggregationMethod.Sum:
                    return ExpressionConstants.KeywordSum;
                case AggregationMethod.VirtualPropertyCount:
                    return ExpressionConstants.QueryOptionCount;
                case AggregationMethod.Custom:
                    return aggExpression.MethodDefinition.MethodLabel;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aggExpression), "unknown AggregationMethod " + aggExpression.Method.ToString());
            }
        }

        private void Translate(AggregateTransformationNode transformation)
        {
            bool appendComma = false;
            foreach (AggregateExpression aggExpression in transformation.Expressions)
            {
                appendComma = AppendComma(appendComma);

                if (aggExpression.Method != AggregationMethod.VirtualPropertyCount)
                {
                    AppendExpression(aggExpression.Expression);
                    _query.Append(ExpressionConstants.SymbolEscapedSpace);
                    AppendWord(ExpressionConstants.KeywordWith);
                }

                AppendWord(GetAggregationMethodName(aggExpression));
                AppendWord(ExpressionConstants.KeywordAs);
                _query.Append(aggExpression.Alias);
            }
        }

        private void Translate(ComputeTransformationNode transformation)
        {
            bool appendComma = false;
            foreach (ComputeExpression computeExpression in transformation.Expressions)
            {
                appendComma = AppendComma(appendComma);

                AppendExpression(computeExpression.Expression);
                _query.Append(ExpressionConstants.SymbolEscapedSpace);
                AppendWord(ExpressionConstants.KeywordAs);
                _query.Append(computeExpression.Alias);
            }
        }

        private void Translate(FilterTransformationNode transformation)
        {
            AppendExpression(transformation.FilterClause.Expression);
        }

        private void Translate(GroupByTransformationNode transformation)
        {
            bool appendComma = false;
            foreach (GroupByPropertyNode node in transformation.GroupingProperties)
            {
                if (appendComma)
                    AppendComma(appendComma);
                else
                {
                    appendComma = true;
                    _query.Append(ExpressionConstants.SymbolOpenParen);
                }

                if (node.Expression != null)
                    AppendExpression(node.Expression);

                bool appendSlash = false;
                foreach (GroupByPropertyNode childNode in node.ChildTransformations)
                {
                    appendSlash = AppendSlash(appendSlash);
                    AppendExpression(childNode.Expression);
                }
            }
            if (appendComma)
                _query.Append(ExpressionConstants.SymbolClosedParen);

            if (transformation.ChildTransformations != null)
            {
                AppendComma(true);
                Translate(transformation.ChildTransformations);
            }
        }

        private void Translate(TransformationNode transformation)
        {
            switch (transformation.Kind)
            {
                case TransformationNodeKind.Aggregate:
                    _query.Append(ExpressionConstants.KeywordAggregate);
                    break;
                case TransformationNodeKind.GroupBy:
                    _query.Append(ExpressionConstants.KeywordGroupBy);
                    break;
                case TransformationNodeKind.Filter:
                    _query.Append(ExpressionConstants.KeywordFilter);
                    break;
                case TransformationNodeKind.Compute:
                    _query.Append(ExpressionConstants.KeywordCompute);
                    break;
                default:
                    throw new NotSupportedException("unknown TransformationNodeKind value " + transformation.Kind.ToString());
            }

            _query.Append(ExpressionConstants.SymbolOpenParen);

            GroupByTransformationNode groupByTransformation;
            AggregateTransformationNode aggTransformation;
            FilterTransformationNode filterTransformation;
            ComputeTransformationNode computeTransformation;
            if ((groupByTransformation = transformation as GroupByTransformationNode) != null)
                Translate(groupByTransformation);
            else if ((aggTransformation = transformation as AggregateTransformationNode) != null)
                Translate(aggTransformation);
            else if ((filterTransformation = transformation as FilterTransformationNode) != null)
                Translate(filterTransformation);
            else if ((computeTransformation = transformation as ComputeTransformationNode) != null)
                Translate(computeTransformation);
            else
                throw new NotSupportedException("unknown TransformationNode type " + transformation.GetType().Name);

            _query.Append(ExpressionConstants.SymbolClosedParen);
        }
    }
}
