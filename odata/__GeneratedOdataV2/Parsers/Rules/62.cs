namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _computeItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._computeItem> Instance { get; } = from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx61x73ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx61x73ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _computedProperty_1 in __GeneratedOdataV2.Parsers.Rules._computedPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._computeItem(_commonExpr_1, _RWS_1, _ʺx61x73ʺ_1, _RWS_2, _computedProperty_1);
    }
    
}
