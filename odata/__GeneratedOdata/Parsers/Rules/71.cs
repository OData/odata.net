namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _filterParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._filter> Instance { get; } = from _Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _boolCommonExpr_1 in __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._filter(_Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ_1, _EQ_1, _boolCommonExpr_1);
    }
    
}
