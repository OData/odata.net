namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Fʺ_metadataOptionsParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_metadataOptions> Instance { get; } = from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _metadataOptions_1 in __GeneratedOdata.Parsers.Rules._metadataOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx3Fʺ_metadataOptions(_ʺx3Fʺ_1, _metadataOptions_1);
    }
    
}
