namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Aʺ_portParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ_port> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _port_1 in __GeneratedOdataV2.Parsers.Rules._portParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ_port(_ʺx3Aʺ_1, _port_1);
    }
    
}
