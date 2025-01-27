namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_rootExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._valueⲻseparator_rootExpr> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdata.Parsers.Rules._valueⲻseparatorParser.Instance
from _rootExpr_1 in __GeneratedOdata.Parsers.Rules._rootExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._valueⲻseparator_rootExpr(_valueⲻseparator_1, _rootExpr_1);
    }
    
}
