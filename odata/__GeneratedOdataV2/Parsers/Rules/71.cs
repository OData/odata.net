namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _filterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._filter> Instance { get; } = from _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _boolCommonExpr_1 in __GeneratedOdataV2.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1, _EQ_1, _boolCommonExpr_1);
    }
    
}
