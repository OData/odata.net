namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _castExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._castExpr> Instance { get; } = from _ʺx63x61x73x74ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx63x61x73x74ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_BWS_COMMA_BWS_1 in __GeneratedOdataV2.Parsers.Inners._commonExpr_BWS_COMMA_BWSParser.Instance.Optional()
from _qualifiedTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedTypeNameParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._castExpr(_ʺx63x61x73x74ʺ_1, _OPEN_1, _BWS_1, _commonExpr_BWS_COMMA_BWS_1.GetOrElse(null), _qualifiedTypeName_1, _BWS_2, _CLOSE_1);
    }
    
}
