namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx3Aʺ_portParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx3Aʺ_port> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _port_1 in __GeneratedOdata.Parsers.Rules._portParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Aʺ_port(_ʺx3Aʺ_1, _port_1);
    }
    
}
