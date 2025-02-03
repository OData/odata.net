namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _commonExpr_BWS_COMMA_BWSParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._commonExpr_BWS_COMMA_BWS> Instance { get; } = from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._commonExpr_BWS_COMMA_BWS(_commonExpr_1, _BWS_1, _COMMA_1, _BWS_2);
    }
    
}
