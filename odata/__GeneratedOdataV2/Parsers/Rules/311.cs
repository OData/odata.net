namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionLiteral> Instance { get; } = from _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺParser.Instance
from _geoLiteral_1 in __GeneratedOdataV2.Parsers.Rules._geoLiteralParser.Instance
from _ⲤCOMMA_geoLiteralↃ_1 in Inners._ⲤCOMMA_geoLiteralↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionLiteral(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1, _geoLiteral_1, _ⲤCOMMA_geoLiteralↃ_1, _CLOSE_1);
    }
    
}
