namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contentⲻidParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._contentⲻid> Instance { get; } = from _ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdata.Parsers.Rules._OWSParser.Instance
from _requestⲻid_1 in __GeneratedOdata.Parsers.Rules._requestⲻidParser.Instance
select new __GeneratedOdata.CstNodes.Rules._contentⲻid(_ʺx43x6Fx6Ex74x65x6Ex74x2Dx49x44ʺ_1, _ʺx3Aʺ_1, _OWS_1, _requestⲻid_1);
    }
    
}
