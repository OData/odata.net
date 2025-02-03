namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _metadataOptionsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._metadataOptions> Instance { get; } = from _metadataOption_1 in __GeneratedOdataV2.Parsers.Rules._metadataOptionParser.Instance
from _Ⲥʺx26ʺ_metadataOptionↃ_1 in Inners._Ⲥʺx26ʺ_metadataOptionↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._metadataOptions(_metadataOption_1, _Ⲥʺx26ʺ_metadataOptionↃ_1);
    }
    
}
