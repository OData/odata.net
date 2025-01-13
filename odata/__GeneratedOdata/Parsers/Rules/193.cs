namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _isofExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._isofExpr> Instance { get; } = from _ʺx69x73x6Fx66ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx69x73x6Fx66ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_BWS_COMMA_BWS_1 in __GeneratedOdata.Parsers.Inners._commonExpr_BWS_COMMA_BWSParser.Instance.Optional()
from _qualifiedTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedTypeNameParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._isofExpr(_ʺx69x73x6Fx66ʺ_1, _OPEN_1, _BWS_1, _commonExpr_BWS_COMMA_BWS_1.GetOrElse(null), _qualifiedTypeName_1, _BWS_2, _CLOSE_1);
    }
    
}
