namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _computeItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._computeItem> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx61x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x73ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _computedProperty_1 in __GeneratedOdata.Parsers.Rules._computedPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._computeItem(_commonExpr_1, _RWS_1, _ʺx61x73ʺ_1, _RWS_2, _computedProperty_1);
    }
    
}
