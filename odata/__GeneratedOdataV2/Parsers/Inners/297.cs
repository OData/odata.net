namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_BWS_commonExpr_BWSParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_BWS_commonExpr_BWS> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_BWS_commonExpr_BWS(_COMMA_1, _BWS_1, _commonExpr_1, _BWS_2);
    }
    
}
