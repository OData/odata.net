namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._annotationExpr> Instance { get; } = from _annotation_1 in __GeneratedOdataV2.Parsers.Rules._annotationParser.Instance
from _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1 in __GeneratedOdataV2.Parsers.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._annotationExpr(_annotation_1, _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1.GetOrElse(null));
    }
    
}
