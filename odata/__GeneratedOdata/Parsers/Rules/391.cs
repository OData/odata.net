namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IPvFutureParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IPvFuture> Instance { get; } = from _ʺx76ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx76ʺParser.Instance
from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._IPvFuture(_ʺx76ʺ_1, _HEXDIG_1, _ʺx2Eʺ_1, _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1);
    }
    
}
