using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.OData.Core.Metadata;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.Aggregation
{
    public class ApplyParser
    {
 
        /// <summary>
        /// Parse an apply query 
        /// </summary>
        /// <param name="applyQuery">The apply query</param>
        /// <param name="oDataUriParserConfiguration"></param>
        /// <param name="edmType">the EDM type of the entity set</param>
        /// <param name="edmNavigationSource"></param>
        /// <returns>an <see cref="ApplyClause"/> that contains a list of transformation clause objects as a List of Tuples 
        /// where item1 is the transformation name and item2 is the relevant Clause object as <see cref="AggregationTransformationBase"/></returns>
        internal static ApplyClause ParseApplyImplementation(string applyQuery, ODataUriParserConfiguration oDataUriParserConfiguration, IEdmType edmType, IEdmNavigationSource edmNavigationSource)
        {
            var res = new ApplyClause();
            if (applyQuery.StartsWith(UriQueryConstants.ApplyQueryOption))
                applyQuery = applyQuery.Substring(7);

            var trasformations = new List<Tuple<string, AggregationTransformationBase>>();
            List<Tuple<string, string>> trasformationsToParse = GetTransformations(applyQuery);
          
            foreach (var transformationPair in trasformationsToParse)
            {
                var trasformationName = transformationPair.Item2;
                var trasformationQuery = transformationPair.Item1;
                switch (trasformationName)
                {
                    case UriQueryConstants.AggregateTransformation:
                        trasformations.Add(new Tuple<string, AggregationTransformationBase>(
                            UriQueryConstants.AggregateTransformation,
                            ParseAggregate(res, trasformationQuery, oDataUriParserConfiguration, edmType, edmNavigationSource)));
                        break;
                    case UriQueryConstants.GroupbyTransformation:
                        trasformations.Add(new Tuple<string, AggregationTransformationBase>(
                            UriQueryConstants.GroupbyTransformation,
                            ParseGroupBy(res, trasformationQuery, oDataUriParserConfiguration, edmType, edmNavigationSource)));
                        break;
                    case UriQueryConstants.FilterTransformation:
                        trasformations.Add(new Tuple<string, AggregationTransformationBase>(
                            UriQueryConstants.FilterTransformation,
                            ParseFilter(res, trasformationQuery, oDataUriParserConfiguration, edmType, edmNavigationSource)));
                        break;
                    default:
                        throw new ODataException("Unsupported aggregation transformation");
                }
            }
            res.Transformations = trasformations;

            return res;
        }

        /// <summary>
        /// Split the apply query that might contain a list of transformations into a list of single transformation queries as a list of tuples when item1 is the transformation query and item2 is the transformation name
        /// </summary>
        /// <param name="applyQuery">The apply query</param>
        /// <returns>a list of tuples when item1 is the transformation query and item2 is the transformation name</returns>
        private static List<Tuple<string, string>> GetTransformations(string applyQuery)
        {
            var transformationsInQuery = SplitQuery(applyQuery);

            //If transformationsInQuery is empty there is only one transformation in the apply query
            if (transformationsInQuery.Count == 0)
            {
                return new List<Tuple<string, string>>() { new Tuple<string, string>(applyQuery, GetTransformation(applyQuery)) };
            }
            else
            {
                var result = new List<Tuple<string, string>>();
                var firstTransformation = applyQuery.Substring(0, transformationsInQuery.First().Item1);
                result.Add(new Tuple<string, string>(firstTransformation, GetTransformation(firstTransformation)));
                for (int i = 0; i < transformationsInQuery.Count - 1; i++)
                {
                    var beginIndex = transformationsInQuery[i].Item1;
                    var endIndex = transformationsInQuery[i+1].Item1;
                    result.Add(new Tuple<string, string>(applyQuery.Substring(beginIndex, endIndex-beginIndex), transformationsInQuery[i].Item2));
                }

                result.Add(new Tuple<string, string>(applyQuery.Substring(transformationsInQuery.Last().Item1), transformationsInQuery.Last().Item2));

                return result;
            }
        }

        /// <summary>
        /// Get a transformation name
        /// </summary>
        /// <param name="query">the query to parse</param>
        /// <returns></returns>
        private static string GetTransformation(string query)
        {
            try
            {
                // all transformation names are followed by a '(' in the query
                var end = query.IndexOf('(');
                return query.Substring(0, end);
            }
            catch (Exception)
            {
                throw new ODataException("Could not find aggregation transformation in the apply query");
            }
            
        }

        /// <summary>
        /// Create a ApplyFilterClause from a parse transformation query 
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="query"></param>
        /// <param name="oDataUriParserConfiguration"></param>
        /// <param name="edmType"></param>
        /// <param name="edmNavigationSource"></param>
        /// <returns></returns>
        private static ApplyFilterClause ParseFilter(ApplyClause apply, string query, ODataUriParserConfiguration oDataUriParserConfiguration, IEdmType edmType, IEdmNavigationSource edmNavigationSource)
        {
            query = IsolateQuery(query, UriQueryConstants.FilterTransformation).Trim('(',')');
            return new ApplyFilterClause()
            {
                Apply = apply,
                Filter = ODataQueryOptionParser.ParseFilterImplementation(
                    query,
                    oDataUriParserConfiguration,
                    edmType,
                    edmNavigationSource),
                RawQueryString = query
            };
        }

        /// <summary>
        /// Parse queries such as:
        /// "aggregate(Amount with sum as Total)" and "aggregate(Amount mul Product/TaxRate with sum as Tax)"
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="query"></param>
        /// <param name="oDataUriParserConfiguration"></param>
        /// <param name="edmType"></param>
        /// <param name="edmNavigationSource"></param>
        /// <returns></returns>
        private static ApplyAggregateClause ParseAggregate(ApplyClause apply, string query,  ODataUriParserConfiguration oDataUriParserConfiguration, IEdmType edmType, IEdmNavigationSource edmNavigationSource)
        {
            query = IsolateQuery(query, UriQueryConstants.AggregateTransformation);
            string aggregatableProperty;
            string aggregationMethod;
            string alias;
            
            aggregatableProperty = GetAggregatableProperty(query, true, out alias, out aggregationMethod);

            //create an projection Expression to the property 
            var AggregatablePropertyExpression = ODataQueryOptionParser.ParseExpressionImplementation(
                    aggregatableProperty,
                    oDataUriParserConfiguration,
                    edmType,
                    edmNavigationSource);

            return new ApplyAggregateClause()
            {
                AggregatablePropertyExpression = AggregatablePropertyExpression,
                Alias = alias,
                AggregationMethod = aggregationMethod,
                AggregatableProperty = aggregatableProperty,
                Apply = apply
            };
        }

        /// <summary>
        /// Get the name of the aggregated property, aggregation method and alias as defined in the query as <see cref="string"/>
        /// </summary>
        /// <param name="query">The query to parse</param>
        /// <param name="validate">Check for syntax error</param>
        /// <param name="alias">The alias found</param>
        /// <param name="aggregationMethod">The aggregation method found</param>
        /// <returns>The aggregated property, aggregation method and alias as defined in the query</returns>
        private static string GetAggregatableProperty(string query, bool validate, out string alias, out string aggregationMethod)
        {
            string aggregatableProperty;
            var verbs = query.Split(' ');
            alias = verbs.Last();
            int withIndex = verbs.Find("with");
            if (validate)
            {
                if (withIndex == -1)
                {
                    throw new ODataException("Syntax error: aggregation query does not contain a 'with' statement");
                }
                if (withIndex == 0)
                {
                    throw new ODataException(
                        "Syntax error: aggregation query does not contain an aggregation property before the 'with' statement");
                }
            }

            aggregationMethod = verbs[withIndex + 1];
            aggregatableProperty = verbs[0];
            if (withIndex == 1)
            {
                return aggregatableProperty;
            }
            else if (withIndex > 1)
            {
                for (int i = 1; i < withIndex; i++)
                {
                    aggregatableProperty = string.Format("{0} {1}", aggregatableProperty, verbs[i]);
                }
                return aggregatableProperty;
            }
            else
            {
                return query;
            }
        }

        /// <summary>
        /// Parse queries such as : "groupby(Customer/Name,Customer/ID,Product/Name,Account)" or "groupby((Customer/Country,Product/Name), aggregate(Amount with sum as Total))"
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="query"></param>
        /// <param name="oDataUriParserConfiguration"></param>
        /// <param name="edmType"></param>
        /// <param name="edmNavigationSource"></param>
        /// <returns></returns>
        private static ApplyGroupbyClause ParseGroupBy(ApplyClause apply, string query,
            ODataUriParserConfiguration oDataUriParserConfiguration, IEdmType edmType, IEdmNavigationSource edmNavigationSource)
        {
            string selectQuery;
            ApplyAggregateClause aggregateClause = null;

            query = IsolateQuery(query, UriQueryConstants.GroupbyTransformation);
            if (query.StartsWith("(") && query.EndsWith(")"))
            {
                query = query.TrimOne('(', ')');
            }
            var p = query.IndexOf(UriQueryConstants.AggregateTransformation + '(');
            if (p == -1)
            {
                if (query.StartsWith("(") && query.EndsWith(")"))
                {
                    query = query.TrimOne('(', ')');
                }
                selectQuery = query;
            }
            else
            {
                selectQuery = query.Substring(0, p).Trim().Trim(',').TrimOne('(', ')');
                aggregateClause = ParseAggregate(apply, query.Substring(p),oDataUriParserConfiguration, edmType, edmNavigationSource);
            }

            var selectedStatements = selectQuery.Split(',');
            string aggregationMethod, alias;
            List<ExpressionClause> aggregatablePropertyExpressions = null;
            try
            {
                aggregatablePropertyExpressions =
                            selectedStatements.Select(statement => ODataQueryOptionParser.ParseExpressionImplementation(
                                GetAggregatableProperty(statement, false, out alias, out aggregationMethod),
                                oDataUriParserConfiguration,
                                edmType,
                                edmNavigationSource)).ToList();
            }
            catch (Exception)
            {
                //parsing of some expressions (like property on enum such as DateTimeOffset/Minute) are not supported so ODataQueryOptionParser.ParseExpressionImplementation will fail
                aggregatablePropertyExpressions = null;
            }
            
            
            return new ApplyGroupbyClause()
            {
                SelectedStatements = selectedStatements,
                SelectedPropertiesExpressions = aggregatablePropertyExpressions,
                Aggregate = aggregateClause,
                Apply = apply
            };
        }
        
        /// <summary>
        /// Isolate a single transformation statement from a transformation query. For example: "aggregate(Amount with sum as Total)" returns "Amount with sum as Total"
        /// </summary>
        /// <param name="query">transformation query</param>
        /// <param name="trasformation">transformation name</param>
        /// <returns>transformation statement such as "Amount with sum as Total"</returns>
        private static string IsolateQuery(string query, string trasformation)
        {
            if (query.StartsWith(string.Format("/{0}(",trasformation)))
                query = query.Substring(trasformation.Length + 2);
            if (query.StartsWith(string.Format("{0}(",trasformation)))
                query = query.Substring(trasformation.Length + 1);
           
            query = query.TrimOne(')');

            return query;
        }

        /// <summary>
        /// Get list of all transformations positions in the query sorted by position as a 
        /// list of tuples where item1 is the position and item2 is the transformation name
        /// </summary>
        /// <param name="applyQuery">The apply query</param>
        /// <returns>list of tuples where item1 is the position and item2 is the transformation name</returns>
        private static List<Tuple<int, string>> SplitQuery(string applyQuery)
        {
            var transformationsInQuery = new List<Tuple<int, string>>();

            //Get all the aggregate transformation positions
            transformationsInQuery.AddRange(FindTransformationPosionsInQuery(applyQuery, UriQueryConstants.AggregateTransformation));
            //Get all the group-by transformation positions
            transformationsInQuery.AddRange(FindTransformationPosionsInQuery(applyQuery, UriQueryConstants.GroupbyTransformation));
            //Get all the filter transformation positions
            transformationsInQuery.AddRange(FindTransformationPosionsInQuery(applyQuery, UriQueryConstants.FilterTransformation));

            transformationsInQuery.Sort((x, y) =>
            {
                if (x.Item1 > y.Item1)
                {
                    return 1;
                }
                if (x.Item1 < y.Item1)
                {
                    return -1;
                }

                return 0;
            });
            return transformationsInQuery;
        }


        /// <summary>
        /// return a list of transformation positions in the query as a list of tuples where item1 is the position and item2 is the transformation name.
        /// A query can contain multiple instances of the same type of transformation.
        /// </summary>
        /// <param name="applyQuery">The whole apply query</param>
        /// <param name="trasformation">transformation name</param>
        /// <returns>list of tuples where item1 is the position and item2 is the transformation name</returns>
        private static List<Tuple<int, string>> FindTransformationPosionsInQuery(string applyQuery, string trasformation)
        {
            List<Tuple<int, string>> resPoints = new List<Tuple<int, string>>();

            var parsedQuery = applyQuery;
            while (parsedQuery.IndexOf("/" + trasformation) != -1)
            {
                var p = parsedQuery.IndexOf("/" + trasformation);
                if (resPoints.Count == 0)
                    resPoints.Add(new Tuple<int, string>(p, trasformation));
                else
                {
                    resPoints.Add(new Tuple<int, string>(p + resPoints.Last().Item1 + 1, trasformation));
                }

                parsedQuery = parsedQuery.Substring(p + 1);
            }

            return resPoints;
        }
    }
}
