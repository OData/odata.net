namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataⲻmaxversionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._odataⲻmaxversion> Instance { get; } = from _ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6EʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdataV2.Parsers.Rules._OWSParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _DIGIT_2 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._odataⲻmaxversion(_ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ_1, _ʺx3Aʺ_1, _OWS_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1), _ʺx2Eʺ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_2));
    }
    
}
