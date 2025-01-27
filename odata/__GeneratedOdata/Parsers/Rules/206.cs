namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rootExprColParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._rootExprCol> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdata.Parsers.Rules._beginⲻarrayParser.Instance
from _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1 in __GeneratedOdata.Parsers.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdata.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdata.CstNodes.Rules._rootExprCol(_beginⲻarray_1, _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
