namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._annotationExpr> Instance { get; } = from _annotation_1 in __GeneratedOdata.Parsers.Rules._annotationParser.Instance
from _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1 in __GeneratedOdata.Parsers.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._annotationExpr(_annotation_1, _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr_1.GetOrElse(null));
    }
    
}
