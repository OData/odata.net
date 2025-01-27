namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _substringMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._substringMethodCallExpr> Instance { get; } = from _ʺx73x75x62x73x74x72x69x6Ex67ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx73x75x62x73x74x72x69x6Ex67ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _BWS_3 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_2 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_4 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COMMA_BWS_commonExpr_BWS_1 in __GeneratedOdata.Parsers.Inners._COMMA_BWS_commonExpr_BWSParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._substringMethodCallExpr(_ʺx73x75x62x73x74x72x69x6Ex67ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _COMMA_1, _BWS_3, _commonExpr_2, _BWS_4, _COMMA_BWS_commonExpr_BWS_1.GetOrElse(null), _CLOSE_1);
    }
    
}
