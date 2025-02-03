namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColPathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexColPathExpr> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _collectionPathExpr_1 in __GeneratedOdataV2.Parsers.Rules._collectionPathExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._complexColPathExpr(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _collectionPathExpr_1.GetOrElse(null));
    }
    
}
