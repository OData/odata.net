namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _orderbyItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._orderbyItem> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1 in __GeneratedOdata.Parsers.Inners._RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._orderbyItem(_commonExpr_1, _RWS_Ⲥʺx61x73x63ʺⳆʺx64x65x73x63ʺↃ_1.GetOrElse(null));
    }
    
}
