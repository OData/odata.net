namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_metadataOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_metadataOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3FʺParser.Instance
from _metadataOptions_1 in __GeneratedOdataV2.Parsers.Rules._metadataOptionsParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ_metadataOptions(_ʺx3Fʺ_1, _metadataOptions_1);
    }
    
}
