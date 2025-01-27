namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPv4addressParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._IPv4address> Instance { get; } = from _decⲻoctet_1 in __GeneratedOdata.Parsers.Rules._decⲻoctetParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _decⲻoctet_2 in __GeneratedOdata.Parsers.Rules._decⲻoctetParser.Instance
from _ʺx2Eʺ_2 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _decⲻoctet_3 in __GeneratedOdata.Parsers.Rules._decⲻoctetParser.Instance
from _ʺx2Eʺ_3 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _decⲻoctet_4 in __GeneratedOdata.Parsers.Rules._decⲻoctetParser.Instance
select new __GeneratedOdata.CstNodes.Rules._IPv4address(_decⲻoctet_1, _ʺx2Eʺ_1, _decⲻoctet_2, _ʺx2Eʺ_2, _decⲻoctet_3, _ʺx2Eʺ_3, _decⲻoctet_4);
    }
    
}
