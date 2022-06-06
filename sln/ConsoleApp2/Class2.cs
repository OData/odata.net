using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace ConsoleApp2
{
    class Class2
    {
    }

    public static class Extension
    {
        public static Expression Replace(this Expression expression, string toReplace, string replacement, StringComparison comparisonType)
        {
            if (expression is And and)
            {
                return new And(and.LeftExpression.Replace(toReplace, replacement, comparisonType), and.RightExpression.Replace(toReplace, replacement, comparisonType));
            }
            else if (expression is Or or)
            {
                return new Or(or.LeftExpression.Replace(toReplace, replacement, comparisonType), or.RightExpression.Replace(toReplace, replacement, comparisonType));
            }
            else if (expression is Other other)
            {
                //// TODO this is where string manipulation would not truly be appropriate, leverage the underlying parsing library to properly remove references to the string to be replaced
                return new Other(other.Expression.Replace(toReplace, replacement, comparisonType));
            }
            else if (expression is All)
            {
                return expression;
            }

            throw new InvalidOperationException($"{nameof(expression)} is of unknown type '{expression.GetType()}'");
        }
    }

    public sealed class Provider : IProvider<GraphRecommendation>
    {
        private readonly IProvider<Recommendataion> recommendationProvider;

        private readonly IProvider<RecommendationResource> recommendationResourceProvider;

        public Provider(IProvider<Recommendataion> recommendationProvider, IProvider<RecommendationResource> recommendationResourceProvider)
        {
            this.recommendationProvider = recommendationProvider;
            this.recommendationResourceProvider = recommendationResourceProvider;
        }

        public IEnumerable<GraphRecommendation> Query(Expression expression)
        {
            if (expression.UsesRecommendation && expression.UsesRecommendationResource)
            {
                // query is on both recommendation and recommendationresource
                if (expression is And and)
                {
                    return this.Query(and.LeftExpression).Intersect(this.Query(and.RightExpression), GraphRecommendationIdComparer.Instance);
                }
                else if (expression is Or or)
                {
                    return this.Query(or.LeftExpression).Union(this.Query(or.RightExpression), GraphRecommendationIdComparer.Instance);
                }
                else if (expression is Other other)
                {
                    Func<Recommendataion, string> propertyValue;
                    if (string.Equals(other.RecommendationProperty, "SomeProp", StringComparison.OrdinalIgnoreCase))
                    {
                        propertyValue = recommendation => recommendation.SomeProp.ToString();
                    }
                    else
                    {
                        //// TODO only the single "SomeProp" property would be supported by this, similar code can be written to support other properties
                        throw new InvalidOperationException($"No property with name '{other.RecommendationProperty}' exists on '{nameof(GraphRecommendation)}'");
                    }

                    Func<Recommendataion, string> recommendationResourceExpression;
                    if (other.Operator == Operator.Eq)
                    {
                        recommendationResourceExpression = recommendation => $"{other.RecommendationResourceProperty} {other.Operator} {propertyValue(recommendation)}";
                    }
                    else
                    {
                        //// TODO only equals operator would be supported by this, similar code can be written to support other operators
                        throw new InvalidOperationException($"The operator '{other.Operator}' is not currently supported");
                    }

                    return this.recommendationProvider
                        .Query(All.Instance)
                        .Select(recommendation => new GraphRecommendation()
                        {
                            Id = recommendation.Id,
                            SomeProp = recommendation.SomeProp,
                            RecommendationResources = this.recommendationResourceProvider
                                .Query(
                                    new And(
                                        new Other($"RecommendationId eq '{recommendation.Id}'"),
                                        new Other(recommendationResourceExpression(recommendation))))
                                .Select(recommendationResource => new GraphRecommendationResource()
                                {
                                    Id = recommendationResource.Id,
                                    OtherProp = recommendationResource.OtherProp,
                                })
                                .ToArray(),
                        })
                        .Where(recommendation => recommendation.RecommendationResources.Any());
                }

                // "All" doesn't "use" recommendation or recommendation resource, so we shouldn't get here if the expression is "All"
                throw new InvalidOperationException($"{nameof(expression)} is of unknown type '{expression.GetType()}'");
            }
            else if (expression.UsesRecommendation)
            {
                // query is on recommendation only
                return this.recommendationProvider
                    .Query(expression)
                    .Select(recommendation => new GraphRecommendation()
                    {
                        Id = recommendation.Id,
                        SomeProp = recommendation.SomeProp,
                        RecommendationResources = this.recommendationResourceProvider
                            .Query(new Other($"RecommendationId eq '{recommendation.Id}'"))
                            .Select(recommendationResource => new GraphRecommendationResource()
                            {
                                Id = recommendationResource.Id,
                                OtherProp = recommendationResource.OtherProp,
                            }),
                    });
            }
            else if (expression.UsesRecommendationResource)
            {
                // query is on recommendationresource only
                return this.recommendationResourceProvider
                    .Query(expression.Replace("recommendationresource/", string.Empty, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(_ => _.RecommendationId)
                    .SelectMany(recommendationResourceGroup => this.recommendationProvider
                        .Query(new Other($"Id eq '{recommendationResourceGroup.Key}'"))
                        .Select(recommendation => new GraphRecommendation()
                        {
                            Id = recommendation.Id,
                            SomeProp = recommendation.SomeProp,
                            RecommendationResources = recommendationResourceGroup
                                .Select(recommendationResource => new GraphRecommendationResource()
                                {
                                    Id = recommendationResource.Id,
                                    OtherProp = recommendationResource.OtherProp,
                                }),
                        }));
            }
            else
            {
                // query refers to neither entity type, apply the expression to both
                return this.recommendationProvider
                    .Query(expression)
                    .Select(recommendation => new GraphRecommendation()
                    {
                        Id = recommendation.Id,
                        SomeProp = recommendation.SomeProp,
                        RecommendationResources = this.recommendationResourceProvider
                            .Query(
                                new And(
                                    expression,
                                    new Other($"RecommendationId eq '{recommendation.Id}'")))
                            .Select(recommendationResource => new GraphRecommendationResource()
                            {
                                Id = recommendationResource.Id,
                                OtherProp = recommendationResource.OtherProp,
                            }),
                    });
            }
        }
    }

    public sealed class Recommendataion
    {
        public string Id { get; }

        public int SomeProp { get; }
    }

    public sealed class RecommendationResource
    {
        public string Id { get; }

        public string RecommendationId { get; }

        public bool OtherProp { get; }
    }

    public sealed class GraphRecommendation
    {
        public string Id { get; set; }

        public int SomeProp { get; set; }

        public IEnumerable<GraphRecommendationResource> RecommendationResources { get; set; }
    }

    public sealed class GraphRecommendationResource
    {
        public string Id { get; set; }
        
        public bool OtherProp { get; set; }
    }

    public sealed class GraphRecommendationIdComparer : IEqualityComparer<GraphRecommendation>
    {
        private GraphRecommendationIdComparer()
        {
        }

        public static GraphRecommendationIdComparer Instance { get; } = new GraphRecommendationIdComparer();

        public bool Equals(GraphRecommendation x, GraphRecommendation y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id;
        }

        public int GetHashCode(GraphRecommendation obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public interface IProvider<T>
    {
        IEnumerable<T> Query(Expression expression);
    }

    public abstract class Expression
    {
        internal Expression()
        {
        }

        public abstract bool UsesRecommendation { get; }

        public abstract bool UsesRecommendationResource { get; }
    }

    public sealed class Or : Expression
    {
        public Or(Expression leftExpression, Expression rightExpression)
            : base()
        {
            this.LeftExpression = leftExpression;
            this.RightExpression = rightExpression;
        }

        public Expression LeftExpression { get; }

        public Expression RightExpression { get; }

        public override bool UsesRecommendation
        {
            get
            {
                return this.LeftExpression.UsesRecommendation || this.RightExpression.UsesRecommendation;
            }
        }

        public override bool UsesRecommendationResource
        {
            get
            {
                return this.LeftExpression.UsesRecommendationResource || this.RightExpression.UsesRecommendationResource;
            }
        }
    }

    public sealed class And : Expression
    {
        public And(Expression leftExpression, Expression rightExpression)
            : base()
        {
            this.LeftExpression = leftExpression;
            this.RightExpression = rightExpression;
        }

        public Expression LeftExpression { get; }

        public Expression RightExpression { get; }

        public override bool UsesRecommendation
        {
            get
            {
                return this.LeftExpression.UsesRecommendation || this.RightExpression.UsesRecommendation;
            }
        }

        public override bool UsesRecommendationResource
        {
            get
            {
                return this.LeftExpression.UsesRecommendationResource || this.RightExpression.UsesRecommendationResource;
            }
        }
    }

    public sealed class Other : Expression
    {
        public Other(string expression)
        {
            this.Expression = expression;
        }

        public override bool UsesRecommendation
        {
            get
            {
                return !string.IsNullOrEmpty(RecommendationProperty);
            }
        }

        public string RecommendationProperty { get; }

        public override bool UsesRecommendationResource
        {
            get
            {
                return !string.IsNullOrEmpty(RecommendationResourceProperty);
            }
        }

        public string RecommendationResourceProperty { get; }

        public Operator? Operator { get; }

        public string Expression { get; }
    }

    public enum Operator
    {
        Eq = 0,
    }

    public sealed class All : Expression
    {
        private All()
        {
        }

        public static All Instance { get; } = new All();

        public override bool UsesRecommendation
        {
            get
            {
                return false;
            }
        }

        public override bool UsesRecommendationResource
        {
            get
            {
                return false;
            }
        }
    }
}
