namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx26ʺ_metadataOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_metadataOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx26ʺParser.Instance
from _metadataOption_1 in __GeneratedOdataV2.Parsers.Rules._metadataOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ_metadataOption(_ʺx26ʺ_1, _metadataOption_1);
    }
    
}
