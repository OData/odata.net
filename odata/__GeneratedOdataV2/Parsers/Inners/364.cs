namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_rootExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_rootExpr> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._valueⲻseparatorParser.Instance
from _rootExpr_1 in __GeneratedOdataV2.Parsers.Rules._rootExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_rootExpr(_valueⲻseparator_1, _rootExpr_1);
    }
    
}
