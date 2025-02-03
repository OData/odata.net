namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPvFutureParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._IPvFuture> Instance { get; } = from _ʺx76ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx76ʺParser.Instance
from _HEXDIG_1 in __GeneratedOdataV2.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, null)
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._IPvFuture(_ʺx76ʺ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._HEXDIG>(_HEXDIG_1), _ʺx2Eʺ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ>(_ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1));
    }
    
}
