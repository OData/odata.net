namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionLiteral> Instance { get; } = from _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺParser.Instance
from _geoLiteral_1 in __GeneratedOdata.Parsers.Rules._geoLiteralParser.Instance
from _ⲤCOMMA_geoLiteralↃ_1 in Inners._ⲤCOMMA_geoLiteralↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionLiteral(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Ex28ʺ_1, _geoLiteral_1, _ⲤCOMMA_geoLiteralↃ_1, _CLOSE_1);
    }
    
}
