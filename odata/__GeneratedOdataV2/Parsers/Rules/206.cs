namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rootExprColParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._rootExprCol> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._beginⲻarrayParser.Instance
from _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1 in __GeneratedOdataV2.Parsers.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._rootExprCol(_beginⲻarray_1, _rootExpr_ЖⲤvalueⲻseparator_rootExprↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
